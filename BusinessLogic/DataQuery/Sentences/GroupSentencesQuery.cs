using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Sentence;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Sentences {
    public class GroupSentencesQuery : BaseQuery, IGroupSentencesQuery {
        #region IGroupSentencesQuery Members

        /// <summary>
        /// Возвращает предложения для определенной группы
        /// </summary>
        /// <param name="userLanguages">язык</param>
        /// <param name="groupId">идентификатор группы, для которой нужно получить предложения</param>
        /// <returns>список предложений для группы</returns>
        public List<SourceWithTranslation> GetSentencesByGroup(UserLanguages userLanguages, long groupId) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var sentencesWithTranslationsQuery = (from s1 in c.Sentence
                                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                                      join gw in c.GroupSentence on st.Id equals
                                                          gw.SentenceTranslationId
                                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                                      where gw.GroupId == groupId &&
                                                            ((s1.LanguageId == sourceLanguageId
                                                              && s2.LanguageId == translationLanguageId)
                                                             ||
                                                             (s1.LanguageId == translationLanguageId
                                                              && s2.LanguageId == sourceLanguageId))
                                                      orderby gw.Rating descending , gw.Id
                                                      select new {st, s1, s2});
                List<SourceWithTranslation> innerResult =
                    sentencesWithTranslationsQuery.AsEnumerable().Select(
                        e => ConvertToGroupSentenceWithTranslation(e.st.Id, e.st.Image, sourceLanguageId, e.s1, e.s2)).
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
            var sentencesQuery = new SentencesQuery();
            SourceWithTranslation sentenceWithTranslation = sentencesQuery.GetOrCreate(SentenceType.FromGroup, source,
                                                                                       translation, image,
                                                                                       null);
            if (sentenceWithTranslation == null || IdValidator.IsInvalid(sentenceWithTranslation.Id)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "GroupSentencesQuery.GetOrCreate can't add sentence {0} with translation {1}, image {2}, rating {3}",
                    source.Text, translation.Text,
                    image != null ? image.Length.ToString(CultureInfo.InvariantCulture) : "<NULL>", rating);
                return null;
            }
            SourceWithTranslation result = Adapter.ReadByContext(c => {
                var wordsWithTranslationsQuery = (from s1 in c.Sentence
                                                  join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                                  join gs in c.GroupSentence on st.Id equals gs.SentenceTranslationId
                                                  join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                                  where
                                                      gs.GroupId == groupForUser.Id
                                                      && st.Id == sentenceWithTranslation.Id
                                                  select new {gs, st, s1, s2});
                var firstRecord = wordsWithTranslationsQuery.AsEnumerable().FirstOrDefault();
                if (firstRecord == null) {
                    return null;
                }
                //сохранить возможно изменившийся рейтинг
                GroupSentence groupSentence = firstRecord.gs;
                groupSentence.Rating = rating;
                c.SaveChanges();

                SourceWithTranslation innerResult = ConvertToGroupSentenceWithTranslation(firstRecord.st.Id,
                                                                                          firstRecord.st.Image,
                                                                                          sentenceWithTranslation
                                                                                              .Source.LanguageId,
                                                                                          firstRecord.s1,
                                                                                          firstRecord.s2);
                return innerResult;
            });
            if (result == null) {
                result = Create(groupForUser, sentenceWithTranslation, rating);
            }
            return result;
        }

        public Dictionary<long, List<SourceWithTranslation>> GetForAllGroups(UserLanguages userLanguages) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            Dictionary<long, List<SourceWithTranslation>> result = Adapter.ReadByContext(c => {
                var sentencesWithTranslationsQuery = (from s1 in c.Sentence
                                                      join st in c.SentenceTranslation on s1.Id equals st.SentenceId1
                                                      join gw in c.GroupSentence on st.Id equals
                                                          gw.SentenceTranslationId
                                                      join s2 in c.Sentence on st.SentenceId2 equals s2.Id
                                                      where ((s1.LanguageId == sourceLanguageId
                                                              && s2.LanguageId == translationLanguageId)
                                                             ||
                                                             (s1.LanguageId == translationLanguageId
                                                              && s2.LanguageId == sourceLanguageId))
                                                      orderby gw.GroupId , gw.Rating descending , gw.Id
                                                      select new {gw.GroupId, st, s1, s2});

                var innerResult = new Dictionary<long, List<SourceWithTranslation>>();
                foreach (var item in sentencesWithTranslationsQuery.AsEnumerable()) {
                    long groupId = item.GroupId;
                    List<SourceWithTranslation> sentencesInGroup;
                    if (!innerResult.TryGetValue(groupId, out sentencesInGroup)) {
                        sentencesInGroup = new List<SourceWithTranslation>();
                        innerResult.Add(groupId, sentencesInGroup);
                    }

                    SourceWithTranslation sentence = ConvertToGroupSentenceWithTranslation(item.st.Id,
                                                                                           item.st.Image,
                                                                                           sourceLanguageId,
                                                                                           item.s1, item.s2);
                    sentencesInGroup.Add(sentence);
                }
                return innerResult;
            });
            return result;
        }

        #endregion

        private SourceWithTranslation Create(GroupForUser groupForUser,
                                             SourceWithTranslation sentenceWithTranslation,
                                             int? rating) {
            SourceWithTranslation result = null;
            Adapter.ActionByContext(context => {
                var groupSentence = new GroupSentence
                {SentenceTranslationId = sentenceWithTranslation.Id, GroupId = groupForUser.Id, Rating = rating};
                context.GroupSentence.Add(groupSentence);
                context.SaveChanges();
                if (IdValidator.IsValid(groupSentence.Id)) {
                    result = new SourceWithTranslation();
                    result.Set(groupSentence.Id, sentenceWithTranslation.Source, sentenceWithTranslation.Translation);
                } else {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "GroupSentencesQuery.Create can't add sentence with translation for sentence with id {0}, translation with id {1}, rating {2}",
                        sentenceWithTranslation.Source.Id, sentenceWithTranslation.Translation.Id,
                        rating);
                }
            });
            return result;
        }

        /// <summary>
        /// Из двух предложений создает предложение с переводом
        /// </summary>
        /// <param name="id">уникальный идентификатор предложения с переводом</param>
        /// <param name="image">изображение для предложения с переводом</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="sentence1">первое предложение</param>
        /// <param name="sentence2">второе предложение</param>
        /// <returns>предложение с переводом</returns>
        private static SourceWithTranslation ConvertToGroupSentenceWithTranslation(long id,
                                                                                   byte[] image,
                                                                                   long sourceLanguageId,
                                                                                   Sentence sentence1,
                                                                                   Sentence sentence2) {
            return ConverterEntities.ConvertToSourceWithTranslation(id, image, sourceLanguageId, sentence1, sentence2);
        }
    }
}