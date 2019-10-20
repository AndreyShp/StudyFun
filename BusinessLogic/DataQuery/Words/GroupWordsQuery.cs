using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Words;
using BusinessLogic.Helpers;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Words {
    public class GroupWordsQuery : BaseQuery, IGroupWordsQuery {
        #region IGroupWordsQuery Members

        /// <summary>
        /// Возвращает слова для определенной группы
        /// </summary>
        /// <param name="userLanguages">язык</param>
        /// <param name="groupId">идентификатор группы, для которой нужно получить слова</param>
        /// <returns>список слов для группы</returns>
        public List<SourceWithTranslation> GetWordsByGroup(UserLanguages userLanguages, long groupId) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join gw in c.GroupWord on wt.Id equals gw.WordTranslationId
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where gw.GroupId == groupId &&
                                                        ((w1.LanguageId == sourceLanguageId
                                                          && w2.LanguageId == translationLanguageId)
                                                         ||
                                                         (w1.LanguageId == translationLanguageId
                                                          && w2.LanguageId == sourceLanguageId))
                                                  orderby gw.Rating descending , gw.Id
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
        /// Создает слова для группы
        /// </summary>
        /// <param name="groupForUser">группа, к которой нужно добавить слово</param>
        /// <param name="source">слово</param>
        /// <param name="translation">перевод</param>
        /// <param name="image">изображение для слова</param>
        /// <param name="rating">рейтинг</param>
        /// <returns>созданные слова для группы, или ничего</returns>
        public SourceWithTranslation GetOrCreate(GroupForUser groupForUser,
                                                 PronunciationForUser source,
                                                 PronunciationForUser translation,
                                                 byte[] image,
                                                 int? rating) {
            var wordsQuery = new WordsQuery();
            WordWithTranslation wordWithTranslation = wordsQuery.GetOrCreate(source, translation, image,
                                                                             WordType.Default, null);
            if (wordWithTranslation == null) {
                return null;
            }
            SourceWithTranslation result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join gw in c.GroupWord on wt.Id equals gw.WordTranslationId
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where gw.GroupId == groupForUser.Id && wt.Id == wordWithTranslation.Id
                                                  select new {gw, wt, w1, w2});
                var firstRecord = wordsWithTranslationsQuery.AsEnumerable().FirstOrDefault();
                if (firstRecord == null) {
                    return null;
                }
                //сохранить возможно изменившийся рейтинг
                GroupWord groupWord = firstRecord.gw;
                groupWord.Rating = rating;
                c.SaveChanges();

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
                result = Create(groupForUser, wordWithTranslation, rating);
            }
            return result;
        }

        public Dictionary<long, List<SourceWithTranslation>> GetForAllGroups(UserLanguages userLanguages) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            Dictionary<long, List<SourceWithTranslation>> result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from w1 in c.Word
                                                  join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                  join gw in c.GroupWord on wt.Id equals gw.WordTranslationId
                                                  join w2 in c.Word on wt.WordId2 equals w2.Id
                                                  where ((w1.LanguageId == sourceLanguageId
                                                          && w2.LanguageId == translationLanguageId)
                                                         ||
                                                         (w1.LanguageId == translationLanguageId
                                                          && w2.LanguageId == sourceLanguageId))
                                                  orderby gw.GroupId , gw.Rating descending , gw.Id
                                                  select new {gw.GroupId, wt, w1, w2});

                var innerResult = new Dictionary<long, List<SourceWithTranslation>>();
                foreach (var e in wordsWithTranslationsQuery.AsEnumerable()) {
                    long groupId = e.GroupId;
                    List<SourceWithTranslation> wordsInGroup;
                    if (!innerResult.TryGetValue(groupId, out wordsInGroup)) {
                        wordsInGroup = new List<SourceWithTranslation>();
                        innerResult.Add(groupId, wordsInGroup);
                    }
                    SourceWithTranslation groupWord = ConverterEntities.ConvertToSourceWithTranslation(e.wt.Id,
                                                                                                       e.wt.Image,
                                                                                                       sourceLanguageId,
                                                                                                       e.w1, e.w2);
                    wordsInGroup.Add(groupWord);
                }
                return innerResult;
            });
            return result;
        }

        #endregion

        private SourceWithTranslation Create(GroupForUser groupForUser,
                                             WordWithTranslation wordWithTranslation,
                                             int? rating) {
            SourceWithTranslation result = null;
            Adapter.ActionByContext(context => {
                var groupWord = new GroupWord
                {WordTranslationId = wordWithTranslation.Id, GroupId = groupForUser.Id, Rating = rating};
                context.GroupWord.Add(groupWord);
                context.SaveChanges();
                if (IdValidator.IsValid(groupWord.Id)) {
                    result = new SourceWithTranslation();
                    result.Set(groupWord.Id, wordWithTranslation.Source, wordWithTranslation.Translations[0]);
                }
            });
            return result;
        }
    }
}