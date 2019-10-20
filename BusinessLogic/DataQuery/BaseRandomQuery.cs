using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery {
    public abstract class BaseRandomQuery : BaseQuery, IRandomPortions {
        /// <summary>
        /// Минимальное кол-во непрочитанных энтитей(слов, фраз, предложений)
        /// Если кол-во непрочитанных энтитей опускается до этой отметки(либо ниже) - пытаемся добавить энтити
        /// </summary>
        public const int MIN_COUNT = 5;

        /// <summary>
        /// Кол-во энтитей, которое нужно возвращать
        /// </summary>
        private const int PORTION_SIZE = 10;

        #region IRandomPortions Members

        /// <summary>
        /// Получает count случайных энтитей с переводом
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="userLanguages">информация о языкых текущего пользователя</param>
        /// <param name="count">кол-во энтитей, которое необходимо получить</param>
        /// <returns>список энтитей перемешанных случайным образом</returns>
        public List<SourceWithTranslation> GetRandom(long userId,
                                                     UserLanguages userLanguages,
                                                     int count = PORTION_SIZE) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            List<SourceWithTranslation> result = GetSourceWithTranslations(userId, sourceLanguageId,
                                                                           translationLanguageId, count);
            if (NeedCreate(result)) {
                //попробовать добавить новые энтити и вернуть большее кол-во энтитей
                result = GetMore(userId, count, result, sourceLanguageId, translationLanguageId);
            }

            SourceWithTranslation currentEntity = result.FirstOrDefault();
            List<SourceWithTranslation> previous;
            if (currentEntity != null) {
                currentEntity.IsCurrent = true;
                previous = GetPrevById(userId, currentEntity.Id,
                                       sourceLanguageId,
                                       translationLanguageId,
                                       count);
            } else {
                LoggerWrapper.LogTo(LoggerName.Default).DebugFormat(
                    "BaseRandomQuery.GetRandom у пользователя {0} нет новых предложений для показа, может все просмотрел?",
                    userId);
                long maxId = GetMaxId(userId, sourceLanguageId, translationLanguageId);

                previous = IdValidator.IsValid(maxId)
                               ? GetPrevById(userId, maxId, sourceLanguageId, translationLanguageId, count)
                               : new List<SourceWithTranslation>(0);
                SourceWithTranslation last = previous.LastOrDefault();
                if (last != null) {
                    last.IsCurrent = true;
                } else {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "BaseRandomQuery.GetRandom для пользователя {0} нет предложений для показа", userId);
                }
            }
            result.InsertRange(0, previous);
            return result;
        }

        public List<SourceWithTranslation> GetNextPortion(long userId,
                                                          long id,
                                                          UserLanguages userLanguages,
                                                          int count = PORTION_SIZE) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return new List<SourceWithTranslation>(0);
            }
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            List<SourceWithTranslation> result = GetNextById(userId, id, sourceLanguageId,
                                                             translationLanguageId, count);
            bool needCreate = NeedCreate(result);
            if (!needCreate) {
                return result;
            }
            //попробовать добавить новые предложения и вернуть большее кол-во предложений
            bool isInserted = InsertShuffle(userId, sourceLanguageId, translationLanguageId, count);
            if (!isInserted) {
                return result;
            }
            //result = GetNextById(userUnique, id, sourceLanguageId, translationLanguageId, count);
            SourceWithTranslation lastSentence = result.LastOrDefault();
            if (lastSentence == null) {
                //нет ни одного предложения - даже после вставки:(
                LoggerWrapper.LogTo(LoggerName.Default).InfoFormat(
                    "BaseRandomQuery.GetNextPortion для пользователя {0} нет ни одного предложения даже после вставки. Может все просмотрел?",
                    userId);
                return result;
            }

            //получаем энтити - которые вставили
            List<SourceWithTranslation> nextPortion = GetNextById(userId, lastSentence.Id,
                                                                  sourceLanguageId,
                                                                  translationLanguageId, count);
            result.AddRange(nextPortion);
            return result;
        }

        public List<SourceWithTranslation> GetPrevPortion(long userId,
                                                          long id,
                                                          UserLanguages userLanguages,
                                                          int countSentences = PORTION_SIZE) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return new List<SourceWithTranslation>(0);
            }
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            return GetPrevById(userId, id, sourceLanguageId, translationLanguageId, countSentences);
        }

        public List<SourceWithTranslation> GetExact(long userId, long sourceId, long translationId) {
            if (IdValidator.IsValid(userId)) {
                //пользователя знаем - может быть это его энтити - попробуем найти
                List<SourceWithTranslation> result = GetUserEntities(userId, sourceId, translationId);
                if (result.Count > 0) {
                    return result;
                }
            }
            Tuple<long, byte[], PronunciationEntity, PronunciationEntity> sourceAndTranslationWithId =
                GetBySourceAndTranslation(sourceId, translationId);
            long id = sourceAndTranslationWithId.Item1;
            PronunciationEntity source = sourceAndTranslationWithId.Item3;
            PronunciationEntity translation = sourceAndTranslationWithId.Item4;
            if (IdValidator.IsInvalid(id) || source == null || translation == null) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "BaseRandomQuery.GetExact для пользователя {0} не удалось получить данные с идентификаторами {1} и {2}",
                    userId, sourceId, translationId);
                return null;
            }
            long sourceLanguageId = source.Id == sourceId ? source.LanguageId : translation.LanguageId;
            SourceWithTranslation sentence = ConverterEntities.ConvertToSourceWithTranslation(id,
                                                                                              sourceAndTranslationWithId
                                                                                                  .Item2,
                                                                                              sourceLanguageId, source,
                                                                                              translation);
            sentence.IsCurrent = true;
            return new List<SourceWithTranslation> {sentence};
        }

        #endregion

        private List<SourceWithTranslation> GetUserEntities(long userId, long sourceId, long translationId) {
            SourceWithTranslation userSentence = GetBySourceAndTranslation(userId, sourceId, translationId);
            if (userSentence == null) {
                return new List<SourceWithTranslation>(0);
            }
            userSentence.IsCurrent = true;
            long sourceLanguageId = userSentence.Source.LanguageId;
            long translationLanguageId = userSentence.Translation.LanguageId;

            var result = new List<SourceWithTranslation>();
            List<SourceWithTranslation> prev = GetPrevById(userId, userSentence.Id,
                                                           sourceLanguageId,
                                                           translationLanguageId, PORTION_SIZE);
            result.AddRange(prev);
            result.Add(userSentence);

            List<SourceWithTranslation> next = GetNextById(userId, userSentence.Id,
                                                           sourceLanguageId,
                                                           translationLanguageId, PORTION_SIZE);

            result.AddRange(next);
            return result;
        }

        private static bool NeedCreate(List<SourceWithTranslation> result) {
            return result.Count <= MIN_COUNT;
        }

        private List<SourceWithTranslation> GetMore(long userId,
                                                    int count,
                                                    List<SourceWithTranslation> result,
                                                    long sourceLanguageId,
                                                    long translationLanguageId) {
            bool isInserted = InsertShuffle(userId, sourceLanguageId, translationLanguageId, count);
            if (!isInserted) {
                return result;
            }
            List<SourceWithTranslation> newResult = GetSourceWithTranslations(userId, sourceLanguageId,
                                                                              translationLanguageId,
                                                                              count);
            return newResult.Count > result.Count ? newResult : result;
        }

        /// <summary>
        /// Получает count случайных энтитей с переводом для текущего пользователя
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="translationLanguageId">язык, на который нужно переводить</param>
        /// <param name="count">кол-во энтитей, которое необходимо получить</param>
        /// <returns>>список случайных энтитей</returns>
        protected abstract List<SourceWithTranslation> GetSourceWithTranslations(long userId,
                                                                                 long sourceLanguageId,
                                                                                 long translationLanguageId,
                                                                                 int count);

        protected abstract Tuple<long, byte[], PronunciationEntity, PronunciationEntity> GetBySourceAndTranslation(
            long sourceId, long translationId);

        protected abstract long GetMaxId(long userId, long sourceLanguageId, long translationLanguageId);

        protected abstract List<SourceWithTranslation> GetPrevById(long userId,
                                                                   long id,
                                                                   long sourceLanguageId,
                                                                   long translationLanguageId,
                                                                   int count);

        protected abstract List<SourceWithTranslation> GetNextById(long userId,
                                                                   long id,
                                                                   long sourceLanguageId,
                                                                   long translationLanguageId,
                                                                   int count);

        protected abstract SourceWithTranslation GetBySourceAndTranslation(long userId,
                                                                           long sourceId,
                                                                           long translationId);

        /// <summary>
        /// Метод добавляет в таблицу со случайными энтитями(словами, предложениями) count случайных энтитей с переводом
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="translationLanguageId">язык, на который нужно переводить</param>
        /// <param name="count">кол-во энтитей, которые необходимо добавить</param>
        /// <returns>уникальное значение, которое присвоенно всем только, что вставленным записям</returns>
        protected abstract bool InsertShuffle(long userId,
                                              long sourceLanguageId,
                                              long translationLanguageId,
                                              int count);

        public abstract bool MarkAsShowed(long userId,
                                          UserLanguages userLanguages,
                                          long id);

        public abstract bool DeleteUserHistory(long userId);
    }
}