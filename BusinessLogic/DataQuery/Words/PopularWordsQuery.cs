using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Words;
using BusinessLogic.Helpers;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Words {
    public class PopularWordsQuery : BaseQuery, IPopularWordsQuery {
        #region IPopularWordsQuery Members

        /// <summary>
        /// Возвращает слова по типу популярности
        /// </summary>
        /// <param name="userLanguages">язык</param>
        /// <param name="type">тип популярных слов</param>
        /// <returns>список слов по типу</returns>
        public List<SourceWithTranslation> GetWordsByType(UserLanguages userLanguages, PopularWordType type) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            var parsedType = (int) type;
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join pw in c.PopularWord on wt.Id equals pw.WordTranslationId
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where pw.Type == parsedType &&
                                                        ((w1.LanguageId == sourceLanguageId
                                                          && w2.LanguageId == translationLanguageId)
                                                         ||
                                                         (w1.LanguageId == translationLanguageId
                                                          && w2.LanguageId == sourceLanguageId))
                                                  orderby pw.Id
                                                  select new {wt, w1, w2});
                List<SourceWithTranslation> innerResult =
                    wordsWithTranslationsQuery.AsEnumerable().Select(
                        e =>
                        ConverterEntities.ConvertToSourceWithTranslation(e.wt.Id, e.wt.Image, sourceLanguageId, e.w1,
                                                                         e.w2)).
                        ToList();
                return innerResult;
            });
            return result;
        }

        /// <summary>
        /// Создает слова по типу популярности
        /// </summary>
        /// <param name="source">слово</param>
        /// <param name="translation">перевод</param>
        /// <param name="type">тип популярности</param>
        /// <returns>созданные слова для группы, или ничего</returns>
        public SourceWithTranslation GetOrCreate(PronunciationForUser source,
                                                 PronunciationForUser translation,
                                                 PopularWordType type) {
            var wordsQuery = new WordsQuery();
            WordWithTranslation wordWithTranslation = wordsQuery.GetOrCreate(source, translation, null, WordType.Default,
                                                                             null);
            if (wordWithTranslation == null) {
                return null;
            }
            var parsedType = (int) type;
            SourceWithTranslation result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join pw in c.PopularWord on wt.Id equals pw.WordTranslationId
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where pw.Type == parsedType && wt.Id == wordWithTranslation.Id
                                                  select new {wt, w1, w2});
                var firstRecord = wordsWithTranslationsQuery.AsEnumerable().FirstOrDefault();
                if (firstRecord == null) {
                    return null;
                }

                SourceWithTranslation innerResult = ConverterEntities.ConvertToSourceWithTranslation(firstRecord.wt.Id,
                                                                                                     firstRecord.wt.
                                                                                                         Image,
                                                                                                     wordWithTranslation
                                                                                                         .Source.
                                                                                                         LanguageId,
                                                                                                     firstRecord.w1,
                                                                                                     firstRecord.w2);
                return innerResult;
            });
            if (result == null) {
                result = Create(wordWithTranslation, parsedType);
            }
            return result;
        }

        #endregion

        private SourceWithTranslation Create(WordWithTranslation wordWithTranslation, int type) {
            SourceWithTranslation result = null;
            Adapter.ActionByContext(context => {
                var popularWord = new PopularWord
                {WordTranslationId = wordWithTranslation.Id, Type = type};
                context.PopularWord.Add(popularWord);
                context.SaveChanges();
                if (IdValidator.IsValid(popularWord.Id)) {
                    result = new SourceWithTranslation();
                    result.Set(popularWord.Id, wordWithTranslation.Source, wordWithTranslation.Translations[0]);
                }
            });
            return result;
        }
    }
}