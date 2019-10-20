using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Words {
    public class ShuffleWordsQuery : BaseRandomQuery, ICleaner {
        private readonly int _shuffleType;
        private readonly int _wordType;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="wordType">тип слов, которые мешаем</param>
        /// <param name="shuffleType">тип данных, которые мешаем</param>
        public ShuffleWordsQuery(WordType wordType, ShuffleType shuffleType) {
            _shuffleType = (int) shuffleType;
            _wordType = (int) wordType;
        }

        /// <summary>
        /// Получает count случайных предложений с переводом для текущего пользователя
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="translationLanguageId">язык, на который нужно переводить</param>
        /// <param name="count">кол-во предложений, которое необходимо получить</param>
        /// <returns>>список случайных предложений</returns>
        protected override List<SourceWithTranslation> GetSourceWithTranslations(long userId,
                                                                                 long sourceLanguageId,
                                                                                 long translationLanguageId,
                                                                                 int count) {
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var joinedSequence = (from w1 in c.Word
                                      join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                      join w2 in c.Word on wt.WordId2 equals w2.Id
                                      join sw in c.ShuffleWord on wt.Id equals sw.WordTranslationId
                                      where (sw.UserId == userId && sw.IsShown == false &&
                                             sw.Type == _shuffleType
                                             && (w1.Type & _wordType) == _wordType && (w2.Type & _wordType) == _wordType
                                             && ((w1.LanguageId == sourceLanguageId && w1.Pronunciation != null
                                                  && w2.LanguageId == translationLanguageId)
                                                 ||
                                                 (w1.LanguageId == translationLanguageId
                                                  && w2.LanguageId == sourceLanguageId && w2.Pronunciation != null)))
                                      orderby sw.Id ascending
                                      select new {sw.WordTranslationId, wt.Image, w1, w2}).Take(count);
                return
                    joinedSequence.AsEnumerable().Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.WordTranslationId, e.Image, sourceLanguageId, e.w1, e.w2)).ToList();
            });
            return result;
        }

        protected override Tuple<long, byte[], PronunciationEntity, PronunciationEntity> GetBySourceAndTranslation(
            long sourceId, long translationId) {
            Tuple<long, byte[], PronunciationEntity, PronunciationEntity> result =
                Adapter.ReadByContext(
                    c => {
                        var sourceWithTranslation = (from w1 in c.Word
                                                     join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                                     join w2 in c.Word on wt.WordId2 equals w2.Id
                                                     where ((w1.Type & _wordType) == _wordType
                                                            && (w2.Type & _wordType) == _wordType &&
                                                            ((wt.WordId1 == sourceId
                                                              && wt.WordId2 == translationId)
                                                             ||
                                                             (wt.WordId2 == sourceId
                                                              && wt.WordId1 == translationId)))
                                                     select new {wt.Id, wt.Image, w1, w2}).FirstOrDefault();
                        return sourceWithTranslation != null
                                   ? new Tuple<long, byte[], PronunciationEntity, PronunciationEntity>(
                                         sourceWithTranslation.Id,
                                         sourceWithTranslation.Image,
                                         sourceWithTranslation.w1,
                                         sourceWithTranslation.w2)
                                   : new Tuple<long, byte[], PronunciationEntity, PronunciationEntity>(IdValidator.INVALID_ID, null,
                                                                                               null, null);
                    });
            return result;
        }

        protected override long GetMaxId(long userId, long sourceLanguageId, long translationLanguageId) {
            return Adapter.ReadByContext(c => (from w1 in c.Word
                                               join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                               join w2 in c.Word on wt.WordId2 equals w2.Id
                                               join sw in c.ShuffleWord on wt.Id equals sw.WordTranslationId
                                               where (sw.UserId == userId &&
                                                      sw.Type == _shuffleType
                                                      && (w1.Type & _wordType) == _wordType
                                                      && (w2.Type & _wordType) == _wordType &&
                                                      ((w1.LanguageId == sourceLanguageId && w1.Pronunciation != null
                                                        && w2.LanguageId == translationLanguageId)
                                                       ||
                                                       (w1.LanguageId == translationLanguageId
                                                        && w2.LanguageId == sourceLanguageId && w2.Pronunciation != null)))
                                               select wt.Id).Max(), IdValidator.INVALID_ID);
        }

        protected override List<SourceWithTranslation> GetPrevById(long userId,
                                                                   long id,
                                                                   long sourceLanguageId,
                                                                   long translationLanguageId,
                                                                   int count) {
            ShuffleWord shuffleWord = GetShuffleWord(userId, id);
            if (shuffleWord == null) {
                return new List<SourceWithTranslation>(0);
            }
            long shuffleWordId = shuffleWord.Id;

            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var joinedSequence = (from w1 in c.Word
                                      join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                      join w2 in c.Word on wt.WordId2 equals w2.Id
                                      join sw in c.ShuffleWord on wt.Id equals sw.WordTranslationId
                                      where (sw.UserId == userId && sw.Id < shuffleWordId &&
                                             sw.Type == _shuffleType
                                             && (w1.Type & _wordType) == _wordType && (w2.Type & _wordType) == _wordType &&
                                             ((w1.LanguageId == sourceLanguageId && w1.Pronunciation != null
                                               && w2.LanguageId == translationLanguageId)
                                              ||
                                              (w1.LanguageId == translationLanguageId
                                               && w2.LanguageId == sourceLanguageId && w2.Pronunciation != null)))
                                      orderby sw.Id descending
                                      select new {sw.Id, wt.Image, sw.WordTranslationId, w1, w2}).Take(count);
                return
                    joinedSequence.AsEnumerable().OrderBy(e => e.Id).Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.WordTranslationId, e.Image, sourceLanguageId, e.w1, e.w2)).
                        ToList();
            });
            return result;
        }

        protected override List<SourceWithTranslation> GetNextById(long userId,
                                                                   long id,
                                                                   long sourceLanguageId,
                                                                   long translationLanguageId,
                                                                   int count) {
            ShuffleWord shuffleWord = GetShuffleWord(userId, id);
            if (shuffleWord == null) {
                return new List<SourceWithTranslation>(0);
            }
            long shuffleWordId = shuffleWord.Id;

            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var joinedSequence = (from w1 in c.Word
                                      join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                      join w2 in c.Word on wt.WordId2 equals w2.Id
                                      join sw in c.ShuffleWord on wt.Id equals sw.WordTranslationId
                                      where (sw.UserId == userId && sw.Id > shuffleWordId &&
                                             sw.Type == _shuffleType
                                             && (w1.Type & _wordType) == _wordType && (w2.Type & _wordType) == _wordType &&
                                             ((w1.LanguageId == sourceLanguageId && w1.Pronunciation != null
                                               && w2.LanguageId == translationLanguageId)
                                              ||
                                              (w1.LanguageId == translationLanguageId
                                               && w2.LanguageId == sourceLanguageId && w2.Pronunciation != null)))
                                      orderby sw.Id ascending
                                      select new {sw.Id, sw.WordTranslationId, wt.Image, w1, w2}).Take(count);
                return
                    joinedSequence.AsEnumerable().OrderBy(e => e.Id).Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.WordTranslationId, e.Image, sourceLanguageId, e.w1, e.w2)).
                        ToList();
            });
            return result;
        }

        private ShuffleWord GetShuffleWord(long userId, long id) {
            ShuffleWord result =
                Adapter.ReadByContext(
                    c => c.ShuffleWord.FirstOrDefault(e => e.UserId == userId && e.WordTranslationId == id));
            return result;
        }

        protected override SourceWithTranslation GetBySourceAndTranslation(long userId,
                                                                           long sourceId,
                                                                           long translationId) {
            SourceWithTranslation result = Adapter.ReadByContext(c => {
                var sourceWithTranslation = (from w1 in c.Word
                                             join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                             join w2 in c.Word on wt.WordId2 equals w2.Id
                                             join sw in c.ShuffleWord on wt.Id equals sw.WordTranslationId
                                             where (sw.UserId == userId &&
                                                    sw.Type == _shuffleType
                                                    && (w1.Type & _wordType) == _wordType
                                                    && (w2.Type & _wordType) == _wordType &&
                                                    ((wt.WordId1 == sourceId
                                                      && wt.WordId2 == translationId)
                                                     ||
                                                     (wt.WordId2 == sourceId
                                                      && wt.WordId1 == translationId)))
                                             orderby sw.Id descending
                                             select new {sw.WordTranslationId, wt.Image, w1, w2}).FirstOrDefault();

                if (sourceWithTranslation == null) {
                    return null;
                }

                long sourceLanguageId = sourceWithTranslation.w1.Id == sourceId
                                            ? sourceWithTranslation.w1.LanguageId
                                            : sourceWithTranslation.w2.LanguageId;
                return ConverterEntities.ConvertToSourceWithTranslation(sourceWithTranslation.WordTranslationId,
                                                      sourceWithTranslation.Image, sourceLanguageId,
                                                      sourceWithTranslation.w1, sourceWithTranslation.w2);
            });
            return result;
        }

        /// <summary>
        /// Метод добавляет в таблицу ShuffleWord count случайных слов с переводом 
        /// </summary>
        /// <param name="userId">уникальный идентификатор пользователя</param>
        /// <param name="sourceLanguageId">язык, с которого нужно переводить</param>
        /// <param name="translationLanguageId">язык, на который нужно переводить</param>
        /// <param name="count">кол-во слов, которые необходимо добавить</param>
        /// <returns>уникальное значение, которое присвоенно всем только, что вставленным записям</returns>
        protected override bool InsertShuffle(long userId,
                                              long sourceLanguageId,
                                              long translationLanguageId,
                                              int count) {
            bool result = Adapter.ExecuteStoredProcedure("insert_shuffle_words", _wordType, _shuffleType,
                                                         userId, sourceLanguageId,
                                                         translationLanguageId, count);
            if (!result) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "ShuffleWordsQuery.InsertShuffle для пользователя {0} не удалось вставить новые слова. Язык источник {1}, язык для перевода {2}, тип слов {3}, тип перемешивания {4}, кол-во вставляемых слов {5}",
                    userId, sourceLanguageId, translationLanguageId, _wordType, _shuffleType, count);
            }
            return result;
        }

        public override bool MarkAsShowed(long userId,
                                          UserLanguages userLanguages,
                                          long id) {
            Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                const string SQL_COMMAND = "update ShuffleWord set IsShown=1"
                                           +
                                           " from ShuffleWord as sw inner join WordTranslation wt on sw.WordTranslationId=wt.Id"
                                           + " inner join [Word] as w1 on wt.WordId1=w1.Id"
                                           + " inner join [Word] as w2 on wt.WordId2=w2.Id"
                                           + " where sw.UserId={0} and sw.WordTranslationId={1}"
                                           + " and sw.Type={4} and (w1.Type & {5})={5} and (w2.Type & {5})={5}"
                                           + " and ((w1.LanguageId={2} and w1.Pronunciation is not null"
                                           +
                                           " and w2.LanguageId={3}) or (w1.LanguageId={3} and w2.LanguageId={2} and w2.Pronunciation is not null))";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             userId, id, userLanguages.From.Id,
                                                             userLanguages.To.Id, _shuffleType, _wordType
                                                         });
            });
            return true;
        }

        public override bool DeleteUserHistory(long userId) {
            Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку удаления и джоинов
                const string SQL_COMMAND = "delete from ShuffleWord where UserId={0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             userId
                                                         });
            });
            return true;
        }

        public List<SourceWithTranslation> GetByCount(UserLanguages userLanguages, int count) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;
            List<SourceWithTranslation> result = Adapter.ReadByContext(c => {
                var joinedSequence = (from w1 in c.Word
                                      join wt in c.WordTranslation on w1.Id equals wt.WordId1
                                      join w2 in c.Word on wt.WordId2 equals w2.Id
                                      where ((w1.Type & _wordType) == _wordType && (w2.Type & _wordType) == _wordType &&
                                             ((w1.LanguageId == sourceLanguageId && w1.Pronunciation != null
                                               && w2.LanguageId == translationLanguageId)
                                              ||
                                              (w1.LanguageId == translationLanguageId
                                               && w2.LanguageId == sourceLanguageId && w2.Pronunciation != null)))
                                      select new {wt.Id, wt.Image, w1, w2}).Take(count);
                return
                    joinedSequence.AsEnumerable().Select(
                        e => ConverterEntities.ConvertToSourceWithTranslation(e.Id, e.Image, sourceLanguageId, e.w1, e.w2)).
                        ToList();
            });
            return result;
        }

        /// <inheritdoc />
        public bool Clean(DateTime maxDateForRemove) {
            var res = Adapter.ActionByContext(c => {
                        const string SQL_COMMAND = "delete FROM [ShuffleWord]"
                                                  + " FROM [ShuffleWord] sw left join[User] u on sw.UserId = u.id"
                                                  + " where u.id is null";
                        c.Database.ExecuteSqlCommand(SQL_COMMAND);
                    });
            return res;
        }
    }
}