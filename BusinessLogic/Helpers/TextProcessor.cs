using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using NLPWrapper.ExternalObjects;
using NLPWrapper.ExternalObjects.Enums;
using ParsedWord = NLPWrapper.ExternalObjects.Word;
using SavedWord = BusinessLogic.Data.Word.Word;

namespace BusinessLogic.Helpers {
    public class TextProcessor : BaseQuery {
        private readonly HashSet<GrammarWordType> _excludeWordTypes = new HashSet<GrammarWordType> {
            GrammarWordType.Punctuation,
            GrammarWordType.ListItemMarker,
            GrammarWordType.CardinalNumber,
        };
        private readonly long _languageId;

        private readonly ISentenceWordsQuery _sentenceWordsQuery;
        private readonly ISentencesQuery _sentencesQuery;
        private readonly ITextAnalyzer _textAnalyzer;
        private readonly Dictionary<string, long> _wordsIdsByText = new Dictionary<string, long>();
        private readonly IWordsQuery _wordsQuery;

        public TextProcessor(long languageId) {
            _languageId = languageId;

            _textAnalyzer = TextAnalyzerFactory.Create();
            _wordsQuery = new WordsQuery();
            _sentencesQuery = new SentencesQuery();
            _sentenceWordsQuery = new SentenceWordsQuery(_languageId);
        }

        public void AnalyzeText(string text) {
            _wordsIdsByText.Clear();

            List<Sentence> sentences = _textAnalyzer.ParseText(text);
            foreach (Sentence sentence in sentences) {
                string sentenceText = (sentence.Text ?? string.Empty).Trim();
                List<Word> words = sentence.Words.Where(e => !_excludeWordTypes.Contains(e.Type)).ToList();
                if (words.Count < 3) {
                    //тип не предложение - пропустить
                    LoggerWrapper.LogTo(LoggerName.Errors).InfoFormat(
                        "TextProcessor.AnalyzeText PartSentence в Sentence не предложение. Предложение \"{0}\" содержит меньше трех слов!",
                        sentenceText);
                    continue;
                }
                if (!SaveSentence(sentenceText, words)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).InfoFormat(
                        "TextProcessor.AnalyzeText Предложение \"{0}\" не было сохранено!", sentenceText);
                }
            }
        }

        private bool SaveSentence(string sentenceText, List<Word> words) {
            //TODO: в случае неудачи - убирать за собой
            foreach (ParsedWord word in words) {
                string wordText = word.GetAppropriateWordText();
                long wordId = SaveWords(word, wordText);
                if (IdValidator.IsInvalid(wordId)) {
                    //не сохранять предложение, т.к. не все слова сохранены
                    return false;
                }
            }
            //TODO: поискать предложение с такими же словами, если есть, то не добавлять

            //сохранить предложение
            long sentenceId = _sentencesQuery.GetOrCreate(_languageId, sentenceText);
            if (IdValidator.IsInvalid(sentenceId)) {
                return false;
            }

            bool result = true;
            int orderInSentence = 1;
            foreach (ParsedWord word in words) {
                long wordId;
                string wordText = word.GetAppropriateWordText();
                if (!_wordsIdsByText.TryGetValue(wordText, out wordId)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "TextProcessor.SaveSentence не удалось найти идентификатор для слова {0}, предложение {1}(идентификатор {2})!",
                        wordText, sentenceId, sentenceText);
                    result = false;
                    break;
                }

                string originalText = word.Text;
                if (!_sentenceWordsQuery.CreateOrUpdate(sentenceId, wordId, originalText, orderInSentence, word.Type)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "TextProcessor.SaveSentence не удалось сохранить слово {0}(идентификатор {1}) для предложения {2}(идентификатор {3})!",
                        originalText, wordId, sentenceText, sentenceId);
                }
                orderInSentence++;
            }

            if (!_sentenceWordsQuery.DeleteGreaterOrEqualsOrderWords(sentenceId, orderInSentence)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "TextProcessor.SaveSentence не удалось удалить слова для предложения {0}(идентификатор {1}), у которых порядковый номер больше или равен {2}!",
                    sentenceText, sentenceId, orderInSentence);
            }
            return result;
        }

        private long SaveWords(Word word, string wordText) {
            long wordId;
            if (!_wordsIdsByText.TryGetValue(wordText, out wordId)) {
                //такого слова еще не было
                wordId = SaveNormalForms(word);
                if (IdValidator.IsInvalid(wordId)) {
                    wordId = SaveWord(wordText);
                }
                _wordsIdsByText.Add(wordText, wordId);
            }
            return wordId;
        }

        private long SaveWord(string wordText) {
            long result = _wordsQuery.GetOrCreate(_languageId, wordText);
            if (IdValidator.IsInvalid(result)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "TextProcessor.SaveWord не удалось сохранить слово {0}, для языка {1}",
                    wordText, _languageId);
            }
            return result;
        }

        private long SaveNormalForms(ParsedWord word) {
            long result = IdValidator.INVALID_ID;
            IEnumerable<string> normalForms =
                word.NormalForms.Where(e =>
                                       !string.Equals(e, word.Text, StringComparison.InvariantCultureIgnoreCase)
                                       && !string.Equals(e, word.FullText, StringComparison.InvariantCultureIgnoreCase));

            foreach (string normalForm in normalForms) {
                long normalFormWordId = _wordsQuery.GetOrCreate(_languageId, normalForm);
                if (IdValidator.IsValid(normalFormWordId)) {
                    result = normalFormWordId;
                } else {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "TextProcessor.SaveNormalForms не удалось сохранить нормальную форму слова {0}, для языка {1}",
                        normalForm, _languageId);
                }
            }
            return result;
        }
    }
}