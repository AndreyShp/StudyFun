using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Words;
using BusinessLogic.Helpers;
using BusinessLogic.Helpers.Speakers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Words {
    public class WordsQuery : BaseQuery, IWordsQuery {
        public const int MIN_LIKE_WORDS = 8;

        private readonly SpeakerFactory _speakerFactory = new SpeakerFactory();

        #region IWordsQuery Members

        public IPronunciation GetById(long id) {
            Word result = Adapter.ReadByContext(c => {
                IQueryable<Word> wordsQuery = (from w in c.Word
                                               where w.Id == id
                                               select w);
                return wordsQuery.FirstOrDefault();
            });
            return result;
        }

        public void FillSpeak(long languageId) {
            ISpeaker speaker = _speakerFactory.Create(languageId);
            Adapter.ActionByContext(c => {
                IQueryable<Word> wordsQuery = (from w in c.Word
                                               where w.LanguageId == languageId && w.Pronunciation == null
                                               select w);
                List<Word> words = wordsQuery.ToList();
                foreach (Word word in words) {
                    word.Pronunciation = speaker.ConvertTextToAudio(word.Text);
                    c.SaveChanges();
                }
            });
        }

        public WordsByPattern GetLikeWords(UserLanguages userLanguages, string likePattern, WordType wordType) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            List<Word> words = GetWordsByLanguageIdAndLikePattern(likePattern, sourceLanguageId, translationLanguageId,
                                                                  wordType);
            var result = new WordsByPattern();
            if (words.Count == 0) {
                words = GetWordsByLanguageIdAndLikePattern(likePattern, translationLanguageId, sourceLanguageId,
                                                           wordType);
                result.IsChangedLanguage = words.Count > 0;
            }
            result.SetWords(words);
            return result;
        }

        public List<PronunciationForUser> GetTranslations(UserLanguages userLanguages, string query, WordType wordType) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            var parsedType = (int) wordType;
            List<PronunciationForUser> result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where ((w1.Text == query || w2.Text == query)
                                                         &&
                                                         ((w1.Type & parsedType) == parsedType
                                                          && (w2.Type & parsedType) == parsedType) &&
                                                         ((w1.LanguageId == sourceLanguageId
                                                           && w2.LanguageId == translationLanguageId)
                                                          ||
                                                          (w1.LanguageId == translationLanguageId
                                                           && w2.LanguageId == sourceLanguageId)))
                                                  select new {wt, w1, w2});
                var wordsWithTranslations = wordsWithTranslationsQuery.ToList();

                HashSet<string> uniqueTranslations = new HashSet<string>();
                var wordsByIds = new Dictionary<long, List<Tuple<int, PronunciationForUser>>>();
                foreach (var wordWithTranslation in wordsWithTranslations) {
                    Word translation = wordWithTranslation.w1.LanguageId == sourceLanguageId
                                           ? wordWithTranslation.w2
                                           : wordWithTranslation.w1;

                    if (!uniqueTranslations.Add(translation.Text)) {
                        //такое слово уже есть - дублирование не нужно
                        continue;
                    }

                    //TODO: сортировать на стороне БД, как в GroupWordsQuery???
                    int rating = RatingHelper.GetRating(wordWithTranslation.wt.Rating);
                    List<Tuple<int, PronunciationForUser>> translations;
                    if (!wordsByIds.TryGetValue(translation.Id, out translations)) {
                        translations = new List<Tuple<int, PronunciationForUser>>();
                        wordsByIds.Add(translation.Id, translations);
                    }
                    translations.Add(new Tuple<int, PronunciationForUser>(rating, new PronunciationForUser(translation)));
                }
                return
                    wordsByIds.Values.SelectMany(e => e).OrderByDescending(e => e.Item1).Select(e => e.Item2).ToList();
            });
            return result;
        }

        public bool Create(WordWithTranslation wordWithTranslation) {
            bool result = true;
            foreach (PronunciationForUser translation in wordWithTranslation.Translations) {
                WordWithTranslation createdWordWithTranslation = GetOrCreate(wordWithTranslation.Source, translation,
                                                                             null, wordWithTranslation.WordType);
                result &= createdWordWithTranslation != null;
            }
            return result;
        }

        public long GetOrCreate(long languageId,
                                string text,
                                WordType wordType = WordType.Default) {
            long result = IdValidator.INVALID_ID;
            Adapter.ActionByContext(context => {
                Word word = context.Word.FirstOrDefault(e => e.LanguageId == languageId && e.Text == text);
                if (word != null) {
                    result = word.Id;
                    return;
                }

                word = AddWord(context, languageId, text, wordType);
                context.SaveChanges();
                result = word.Id;
            });
            return result;
        }

        public WordWithTranslation GetOrCreate(PronunciationForUser source,
                                               PronunciationForUser translation,
                                               byte[] image,
                                               WordType wordType = WordType.Default,
                                               int? rating = null) {
            //TODO: проверять корректность параметров
            WordWithTranslation result = null;
            Adapter.ActionByContext(context => {
                IEnumerable<Word> sourceWords = FindWords(context, new List<PronunciationForUser> {source});
                IEnumerable<Word> translateWords = FindWords(context, new List<PronunciationForUser> {translation});

                Word sourceWord = GetOrAddWord(source, wordType, sourceWords, context);
                Word translationWord = GetOrAddWord(translation, wordType, translateWords, context);
                context.SaveChanges();

                if (sourceWord == null || IdValidator.IsInvalid(sourceWord.Id) || translationWord == null
                    || IdValidator.IsInvalid(translationWord.Id)) {
                    //все слова добавлены - если нужно, добавить связь между новым словом и переводом для него
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "WordsQuery.GetOrCreate can't add text {0} with translation {1}, image {2}, wordType {3}, rating {4}",
                        source.Text, translation.Text,
                        image != null ? image.Length.ToString(CultureInfo.InvariantCulture) : "<NULL>", wordType, rating);
                    result = null;
                    return;
                }

                WordTranslation wordTranslation = GetWordTranslation(context, sourceWord, translationWord);
                if (wordTranslation != null) {
                    //обновить возможно изменившийся рейтинг и изображение
                    wordTranslation.Image = image;
                    wordTranslation.Rating = rating;
                    context.SaveChanges();
                } else {
                    wordTranslation = Create(image, rating, context, sourceWord, translationWord);
                }

                if (IdValidator.IsValid(wordTranslation.Id)) {
                    result = new WordWithTranslation(sourceWord) {Id = wordTranslation.Id};
                    result.AddTranslation(translationWord);
                } else {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "WordsQuery.GetOrCreate can't add/get wordTranslation for text {0} with translation {1}, image {2}, wordType {3}, rating {4}",
                        source.Text, translation.Text,
                        image != null ? image.Length.ToString(CultureInfo.InvariantCulture) : "<NULL>", wordType, rating);
                }
            });
            return result;
        }

        public bool IsInvalid(PronunciationForUser wordForUser) {
            return wordForUser == null || string.IsNullOrEmpty(wordForUser.Text)
                   || IdValidator.IsInvalid(wordForUser.LanguageId);
        }

        public Dictionary<long, SourceWithTranslation> GetByTranslationsIds(List<long> wordsTrandlationsIds,
                                                                            long sourceLanguageId,
                                                                            long translationLanguageId) {
            return Adapter.ReadByContext(c => {
                var joinedSequence = (from w1 in c.Word
                                      join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                      join w2 in c.Word on wt.WordId2 equals w2.Id
                                      where (wordsTrandlationsIds.Contains(wt.Id) &&
                                             ((w1.LanguageId == sourceLanguageId
                                               && w2.LanguageId == translationLanguageId)
                                              ||
                                              (w1.LanguageId == translationLanguageId
                                               && w2.LanguageId == sourceLanguageId)))
                                      select new {wt.Id, wt.Image, w1, w2});
                return
                    joinedSequence.AsEnumerable().ToDictionary(e => e.Id,
                                                               e =>
                                                               ConverterEntities.ConvertToSourceWithTranslation(e.Id,
                                                                                                                e.Image,
                                                                                                                sourceLanguageId,
                                                                                                                e.w1,
                                                                                                                e.w2));
            });
        }

        public long GetIdByWordsForUser(PronunciationForUser source, PronunciationForUser translation) {
            WordWithTranslation wrd = GetOrCreate(source, translation, null);
            return wrd != null ? wrd.Id : IdValidator.INVALID_ID;
        }

        #endregion

        private Word GetOrAddWord(PronunciationForUser wordForUser,
                                  WordType wordType,
                                  IEnumerable<Word> words,
                                  StudyLanguageContext context) {
            Word word = words.FirstOrDefault();
            if (word == null) {
                word = AddWord(context, wordForUser.LanguageId, wordForUser.Text, wordType);
            } else {
                word.Type |= (int) wordType;
            }
            return word;
        }

        private static WordTranslation GetWordTranslation(StudyLanguageContext context,
                                                          Word sourceWord,
                                                          Word translationWord) {
            IQueryable<WordTranslation> wordsWithTranslationsQuery = (from wt in context.WordTranslation
                                                                      where
                                                                          wt.WordId1 == sourceWord.Id
                                                                          && wt.WordId2 == translationWord.Id
                                                                      select wt);
            WordTranslation wordTranslation = wordsWithTranslationsQuery.FirstOrDefault();
            return wordTranslation;
        }

        private static WordTranslation Create(byte[] image,
                                              int? rating,
                                              StudyLanguageContext context,
                                              Word sourceWord,
                                              Word translationWord) {
            WordTranslation wordTranslation =
                context.WordTranslation.Add(new WordTranslation
                {WordId1 = sourceWord.Id, WordId2 = translationWord.Id, Image = image, Rating = rating});
            context.WordTranslation.Add(wordTranslation);

            context.SaveChanges();
            return wordTranslation;
        }

        private static string NormalizeTextWord(string text) {
            string result = text.Trim();
            if (result.Length > 1) {
                //опустить все буквы кроме первой
                result = result[0] + result.Substring(1);
            }
            return result;
        }

        private static IEnumerable<Word> FindWords(StudyLanguageContext c, List<PronunciationForUser> words) {
            long languageId = words[0].LanguageId;
            List<string> texts = words.Select(wordForUser => wordForUser.Text).ToList();

            List<Word> result = (from w in c.Word
                                 where w.LanguageId == languageId && texts.Contains(w.Text)
                                 select w).ToList();
            return result;
        }

        private Word AddWord(StudyLanguageContext c,
                             long languageId,
                             string text,
                             WordType wordType) {
            ISpeaker speaker = _speakerFactory.Create(languageId);
            string normalizedText = NormalizeTextWord(text);

            var result = new Word {
                LanguageId = languageId,
                Text = normalizedText,
                Type = (int) wordType,
                Pronunciation = speaker.ConvertTextToAudio(normalizedText)
            };
            c.Word.Add(result);
            return result;
        }

        private List<Word> GetWordsByLanguageIdAndLikePattern(string likePattern,
                                                              long sourceLanguageId,
                                                              long translationLanguageId,
                                                              WordType wordType) {
            var type = (int) wordType;
            List<Word> result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where (w1.Type & type) == type && (w2.Type & type) == type &&
                                                        ((w1.LanguageId == sourceLanguageId
                                                          && w2.LanguageId == translationLanguageId
                                                          && w1.Text.StartsWith(likePattern))
                                                         ||
                                                         (w2.LanguageId == sourceLanguageId
                                                          && w1.LanguageId == translationLanguageId)
                                                         && w2.Text.StartsWith(likePattern))
                                                  select new {w1, w2}).Take(MIN_LIKE_WORDS);
                var wordsWithTranslations = wordsWithTranslationsQuery.ToList();
                return
                    wordsWithTranslations.Select(
                        wordsWithTranslation =>
                        wordsWithTranslation.w1.LanguageId == sourceLanguageId
                            ? wordsWithTranslation.w1
                            : wordsWithTranslation.w2).Distinct().ToList();
            });
            return result;
        }
    }
}