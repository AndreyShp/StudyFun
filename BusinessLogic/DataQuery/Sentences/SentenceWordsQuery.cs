using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Sentence;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using NLPWrapper.ExternalObjects.Enums;

namespace BusinessLogic.DataQuery.Sentences {
    public class SentenceWordsQuery : BaseQuery, ISentenceWordsQuery {
        private readonly long _languageId;

        public SentenceWordsQuery(long languageId) {
            _languageId = languageId;
        }

        #region ISentenceWordsQuery Members

        public bool CreateOrUpdate(long sentenceId,
                                   long wordId,
                                   string originalText,
                                   int orderInSentence,
                                   GrammarWordType grammarWordType) {
            var parsedGrammarWordType = (int) grammarWordType;
            bool result = false;
            Adapter.ActionByContext(context => {
                SentenceWord sentenceWord =
                    context.SentenceWord.FirstOrDefault(
                        e => e.SentenceId == sentenceId && e.OrderInSentence == orderInSentence);
                if (sentenceWord == null) {
                    sentenceWord = new SentenceWord {
                        SentenceId = sentenceId,
                        OrderInSentence = orderInSentence
                    };
                    context.SentenceWord.Add(sentenceWord);
                }

                sentenceWord.WordId = wordId;
                sentenceWord.OriginalText = originalText;
                sentenceWord.GrammarType = parsedGrammarWordType;
                context.SaveChanges();
                result = IdValidator.IsValid(sentenceWord.Id);
            });
            return result;
        }

        public SentenceWithWords GetWordsBySentenceId(long sentenceId) {
            SentenceWithWords result = null;

            Adapter.ActionByContext(c => {
                var sentences = from s in c.Sentence
                                join sw in c.SentenceWord on s.Id equals sw.SentenceId
                                join w in c.Word on sw.WordId equals w.Id into ssw
                                from e in ssw.DefaultIfEmpty()
                                where s.Id == sentenceId && s.LanguageId == _languageId
                                orderby sw.OrderInSentence
                                select new {s, sw, ssw};
                var sentencesWithWords = sentences.ToList();
                foreach (var sentenceWithWord in sentencesWithWords) {
                    if (result == null) {
                        var sentence = new PronunciationForUser(sentenceWithWord.s);
                        result = new SentenceWithWords(sentence);
                    }

                    List<Word> words = sentenceWithWord.ssw.ToList();
                    if (EnumerableValidator.IsNotEmpty(words)) {
                        var word = new PronunciationForUser(words[0]);
                        result.AddWord(word);
                    } else {
                        result.AddWord(sentenceWithWord.sw.OriginalText);
                    }
                }
            });

            return result;
        }

        public bool DeleteGreaterOrEqualsOrderWords(long sentenceId, int orderInSentence) {
            return Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку удаления и джоинов
                const string SQL_COMMAND = "delete from SentenceWord where SentenceId={0} and OrderInSentence>={1}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             sentenceId, orderInSentence
                                                         });
            });
        }

        public List<Sentence> FindSentencesByWords(List<string> words,
                                                   OrderWordsInSearch orderWordsInSearch,
                                                   int maxCount = 5) {
            var uniqueWords = new HashSet<string>(words);
            int count = GetCount(maxCount);
            List<Sentence> result = Adapter.ReadByContext(c => {
                var sentences = from s in c.Sentence
                                join sw in c.SentenceWord on s.Id equals sw.SentenceId
                                join w in c.Word on sw.WordId equals w.Id
                                where
                                    s.LanguageId == _languageId
                                    && (uniqueWords.Contains(w.Text) || uniqueWords.Contains(sw.OriginalText))
                                orderby s.Id
                                select new {s, sw, w};
                var dataBySentences = sentences.Take(count).ToList();
                var innerResult = new List<Sentence>();
                var prevSentence = new Sentence {Id = IdValidator.INVALID_ID};
                var originalSentenceWords = new Dictionary<string, List<SentenceWord>>();
                var sentenceWords = new Dictionary<string, List<SentenceWord>>();
                foreach (var dataBySentence in dataBySentences) {
                    if (dataBySentence.s.Id != prevSentence.Id) {
                        //предложение изменилось - проверить слова

                        //добавить точное совпадение если есть
                        bool isAdded = AddAppropriateSentenceToResult(words, orderWordsInSearch, originalSentenceWords,
                                                                      prevSentence,
                                                                      innerResult);
                        if (!isAdded) {
                            //точных совпадений нет - добавить не точное совпадение если есть
                            AddAppropriateSentenceToResult(words, orderWordsInSearch, sentenceWords, prevSentence,
                                                           innerResult);
                        }

                        prevSentence = dataBySentence.s;
                        sentenceWords.Clear();
                        originalSentenceWords.Clear();

                        if (innerResult.Count == maxCount) {
                            //набрали нужное кол-во предложений
                            break;
                        }
                    }

                    string originalText = dataBySentence.sw.OriginalText.ToLowerInvariant();
                    string wordText = dataBySentence.w.Text.ToLowerInvariant();
                    if (!originalText.Equals(wordText)) {
                        AddSentenceWordByText(sentenceWords, wordText, dataBySentence.sw);
                    }
                    AddSentenceWordByText(originalSentenceWords, originalText, dataBySentence.sw);
                    AddSentenceWordByText(sentenceWords, originalText, dataBySentence.sw);
                }

                if (originalSentenceWords.Count > 0) {
                    //добавить точное совпадение если есть
                    bool isAdded = AddAppropriateSentenceToResult(words, orderWordsInSearch, originalSentenceWords,
                                                                  prevSentence,
                                                                  innerResult);
                    if (!isAdded) {
                        //точных совпадений нет - добавить не точное совпадение если есть
                        AddAppropriateSentenceToResult(words, orderWordsInSearch, sentenceWords, prevSentence,
                                                       innerResult);
                    }
                }

                return innerResult;
            }, new List<Sentence>());

            return result;
        }

        #endregion

        private static bool AddAppropriateSentenceToResult(IEnumerable<string> words,
                                                           OrderWordsInSearch orderWordsInSearch,
                                                           Dictionary<string, List<SentenceWord>> sentenceWordsByWords,
                                                           Sentence sentence,
                                                           List<Sentence> result) {
            bool isAppropriateSentence = true;
            HashSet<int> correctOrderInSentences = null;
            foreach (string word in words) {
                List<SentenceWord> sentenceWords;
                if (!sentenceWordsByWords.TryGetValue(word.ToLowerInvariant(), out sentenceWords)) {
                    //не все слова найдены в предложении - предложение нам не подходит
                    isAppropriateSentence = false;
                    break;
                }
                if (orderWordsInSearch != OrderWordsInSearch.ExactWordForWord) {
                    //порядок не важен
                    continue;
                }

                IEnumerable<int> currentOrderInSentence = sentenceWords.Select(e => e.OrderInSentence);
                if (correctOrderInSentences == null) {
                    //первое слово сравнивать не с чем - пропустить
                    correctOrderInSentences = new HashSet<int>(currentOrderInSentence);
                    continue;
                }

                var newCorrectOrderInSentences = new HashSet<int>();
                //порядок важен и есть предыдущее слово
                foreach (int orderInSentence in currentOrderInSentence) {
                    int prevOrderInSentence = orderInSentence - 1;
                    if (correctOrderInSentences.Contains(prevOrderInSentence)) {
                        //предыдущее слово найдено 
                        newCorrectOrderInSentences.Add(orderInSentence);
                    }
                }

                if (newCorrectOrderInSentences.Count == 0) {
                    //предыдущее слово не стоит перед текущим словом - предложение нам не подходит
                    isAppropriateSentence = false;
                    break;
                }
                correctOrderInSentences = newCorrectOrderInSentences;
            }

            if (isAppropriateSentence) {
                //TODO: убрать
                foreach (string word in words) {
                    sentence.Text = sentence.Text.Replace(word, "<b>" + word + "</b>");
                }
                result.Add(sentence);
            }
            return isAppropriateSentence;
        }

        private static void AddSentenceWordByText(Dictionary<string, List<SentenceWord>> sentenceWords,
                                                  string text,
                                                  SentenceWord sw) {
            if (!sentenceWords.ContainsKey(text)) {
                sentenceWords.Add(text, new List<SentenceWord>());
            }
            sentenceWords[text].Add(sw);
        }

        private static int GetCount(int maxCount) {
            int count = maxCount * 100;
            if (maxCount < 500) {
                count = 500;
            }
            return count;
        }
    }
}