using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Extensions;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за создание/получение знаний пользователя
    /// </summary>
    public class UserKnowledgeQuery : BaseQuery, IUserKnowledgeQuery {
        private readonly long _languageId;
        private readonly long _userId;

        public UserKnowledgeQuery(long userId, long languageId) {
            _userId = userId;
            _languageId = languageId;
        }

        #region IUserKnowledgeQuery Members

        /// <summary>
        /// Получает данные пользователя
        /// </summary>
        /// <param name="sourceLanguageId">идентификатор языка с которого переводят</param>
        /// <param name="translationLanguageId">идентификатор языка на который переводят</param>
        /// <param name="status">статус данных, которые нужно получить</param>
        /// <param name="prevId">максимальный идентификатор предыдущей записи</param>
        /// <param name="count">кол-во записей для получения</param>
        /// <returns></returns>
        public List<UserKnowledgeItem> GetData(long sourceLanguageId,
                                               long translationLanguageId,
                                               KnowledgeStatus status,
                                               long prevId,
                                               int count) {
            List<UserKnowledgeItem> result = Adapter.ReadByContext(c => {
                DateTime minDate = GetDefaultDeletedDate();
                IQueryable<UserKnowledge> usersKnowledgesQuery =
                    c.UserKnowledge.Where(
                        e =>
                        e.UserId == _userId && e.LanguageId == _languageId && e.Id < prevId && e.DeletedDate == minDate);
                if (status != KnowledgeStatus.All) {
                    usersKnowledgesQuery = usersKnowledgesQuery.Where(e => e.Status == (int) status);
                }
                usersKnowledgesQuery = usersKnowledgesQuery.OrderByDescending(e => e.CreationDate).Take(count);

                List<UserKnowledge> userKnowledges = usersKnowledgesQuery.ToList();
                List<UserKnowledgeItem> innerResult = ConvertEntitiesToItems(sourceLanguageId, translationLanguageId,
                                                                             userKnowledges);
                return innerResult;
            }, new List<UserKnowledgeItem>(0));
            return result;
        }

        /// <summary>
        /// Добавляет "новые пункты знаний" пользователю
        /// </summary>
        /// <param name="knowledgeItems">новые пункты знаний</param>
        /// <param name="maxCountItemsPerDay">максимальное кол-во записей в день на добавление</param>
        /// <returns>список статусов добавления пунктов, для каждого пункта свой статус</returns>
        public List<KnowledgeAddStatus> Add(List<UserKnowledgeItem> knowledgeItems, int maxCountItemsPerDay) {
            var result = new List<KnowledgeAddStatus>();
            Adapter.ActionByContext(c => {
                //очистить данные, т.к. в случае ошибки этот метод вызовется еще раз, а коллекции будут заполнены от предыдущей итерации
                result.Clear();
                var uniqueHashes = new HashSet<string>();
                var entitiesToSave = new List<Tuple<UserKnowledgeItem, UserKnowledge>>();

                bool canAdd = CanAdd(maxCountItemsPerDay, c);
                if (!canAdd) {
                    result.AddRange(Enumerable.Repeat(KnowledgeAddStatus.ReachMaxLimit, knowledgeItems.Count));
                    return;
                }

                foreach (UserKnowledgeItem knowledgeItem in knowledgeItems) {
                    string hash = GetUniqueHash(knowledgeItem);
                    if (uniqueHashes.Contains(hash)) {
                        //с таким хэшем уже был в этой пачке - не пробовать сохранять - считаем, что уже сохранен
                        result.Add(KnowledgeAddStatus.AlreadyExists);
                        continue;
                    }

                    Tuple<KnowledgeAddStatus, UserKnowledge> statusWithEntity = ConvertItemToEntity(hash, knowledgeItem,
                                                                                                    c);
                    uniqueHashes.Add(hash);
                    KnowledgeAddStatus statusBeforeSave = statusWithEntity.Item1;
                    if (statusBeforeSave == KnowledgeAddStatus.Ok) {
                        entitiesToSave.Add(new Tuple<UserKnowledgeItem, UserKnowledge>(knowledgeItem,
                                                                                       statusWithEntity.Item2));
                    } else {
                        result.Add(statusBeforeSave);
                    }
                }
                c.SaveChanges();
                foreach (var pairToSave in entitiesToSave) {
                    KnowledgeAddStatus status;
                    if (IdValidator.IsValid(pairToSave.Item2.Id)) {
                        status = KnowledgeAddStatus.Ok;
                    } else {
                        status = KnowledgeAddStatus.Error;
                        LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                            "UserKnowledgeQuery.Add не удалось добавить пользователю с идентификатором {0} для языка {1} новый пункт знаний {2}",
                            _userId, _languageId, KnowledgeToLogString(pairToSave.Item1));
                    }
                    result.Add(status);
                }
            });
            return result;
        }

        public bool IsInvalid(UserKnowledgeItem knowledgeItem) {
            return IdValidator.IsInvalid(_userId) || IdValidator.IsInvalid(_languageId) || knowledgeItem == null
                   || IdValidator.IsInvalid(knowledgeItem.DataId)
                   || EnumValidator.IsInvalid(knowledgeItem.DataType);
        }

        /// <summary>
        /// Удаляет данные
        /// </summary>
        /// <param name="id">идентификатор данных, которые нужно удалить</param>
        /// <returns>true - данные удалены, иначе false</returns>
        public bool Remove(long id) {
            return SetDeletedDate(id, DateTime.Now);
        }

        /// <summary>
        /// Восстанавливает данные
        /// </summary>
        /// <param name="id">идентификатор данных, которые нужно восстановить</param>
        /// <returns>true - данные восстановлены, иначе false</returns>
        public bool Restore(long id) {
            return SetDeletedDate(id, GetDefaultDeletedDate());
        }

        /// <summary>
        /// Получает статистику пользователя
        /// </summary>
        /// <returns>статистика пользователя</returns>
        public UserKnowledgeStatistic GetStatistic() {
            DateTime currentDate = GetCurrentDate();
            var defaultValue = new UserKnowledgeStatistic {
                Today = new UserKnowledgeStatistic.CountStatistic(),
                Total =
                    new UserKnowledgeStatistic.CountStatistic()
            };
            DateTime minDate = new DateTime().GetDbDateTime();
            UserKnowledgeStatistic result = Adapter.ReadByContext(c => {
                var innerResult = new UserKnowledgeStatistic();

                IQueryable<IGrouping<int, UserKnowledge>> query =
                    c.UserKnowledge.Where(
                        e => e.UserId == _userId && e.LanguageId == _languageId && e.DeletedDate == minDate).GroupBy(
                            e => e.DataType);
                innerResult.Total = GetCountStatistic(query);

                query = c.UserKnowledge
                    .Where(
                        e =>
                        e.UserId == _userId && e.LanguageId == _languageId && e.CreationDate >= currentDate
                        && e.DeletedDate == minDate)
                    .GroupBy(e => e.DataType);
                innerResult.Today = GetCountStatistic(query);
                return innerResult;
            }, defaultValue);
            return result;
        }

        public List<UserKnowledgeItem> ConvertEntitiesToItems(long sourceLanguageId,
                                                              long translationLanguageId,
                                                              IEnumerable<UserKnowledge> userKnowledges) {
            var knowledgesConverter = new KnowledgesParser();
            var result = userKnowledges.Select(e => new UserKnowledgeItem(e)).ToList();
            knowledgesConverter.FillItemsParsedData(sourceLanguageId, translationLanguageId, result);
            return result;
        }

        /// <summary>
        /// Возвращает идентификаторы, которые есть у пользователя в знаниях
        /// </summary>
        /// <param name="ids">идентификаторы, для которых нужно определить существование данных</param>
        /// <param name="dataType">тип данных, для которых переданы идентификаторы</param>
        /// <returns>идентификаторы данных, которые есть у пользователя в знаниях</returns>
        public List<long> GetExistenceIds(List<long> ids, KnowledgeDataType dataType) {
            DateTime minDate = GetDefaultDeletedDate();
            var parsedDataType = (int) dataType;
            List<long> result = Adapter.ReadByContext(c => {
                return
                    c.UserKnowledge.Where(
                        e =>
                        e.UserId == _userId && e.LanguageId == _languageId && e.DataType == parsedDataType
                        && e.DataId != null
                        && ids.Contains((long) e.DataId) && e.DeletedDate == minDate).Select(
                            e => (long) e.DataId).ToList();
            }, new List<long>(0));
            return result;
        }

        /// <summary>
        /// Удаляет данные пользователя, которые были удалены пользователем - ранее текущей даты
        /// </summary>
        /// <returns>true - данные удалены, false - данные не удалось удалить</returns>
        public bool RemoveDeleted() {
            DateTime maxDeleted = DateTime.Today.AddDays(-3);
            DateTime defaultDeletedDate = GetDefaultDeletedDate();
            return Adapter.ReadByContext(c => {
                const string SQL_COMMAND = "delete from [UserKnowledge] where UserId={0} and LanguageId={1} and DeletedDate!={2} and DeletedDate<{3}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             _userId, _languageId, defaultDeletedDate, maxDeleted
                                                         });
                return true;
            });
        }

        #endregion

        private UserKnowledgeStatistic.CountStatistic GetCountStatistic(IEnumerable<IGrouping<int, UserKnowledge>> query) {
            var result = new UserKnowledgeStatistic.CountStatistic();
            foreach (var groupingRow in query) {
                var dataType = (KnowledgeDataType) groupingRow.Key;
                int count = groupingRow.Count();

                switch (dataType) {
                    case KnowledgeDataType.WordTranslation:
                        result.CountWords += count;
                        break;
                    case KnowledgeDataType.PhraseTranslation:
                        result.CountPhrases += count;
                        break;
                    case KnowledgeDataType.SentenceTranslation:
                        result.CountSentences += count;
                        break;
                    default:
                        LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                            "UserKnowledgeQuery.GetCountStatistic некорректный тип данных пользователя. Идентификатор пользователя {0}, идентификатор языка {1}, тип данных {2}",
                            _userId, _languageId, dataType);
                        break;
                }
            }
            return result;
        }

        private Tuple<KnowledgeAddStatus, UserKnowledge> ConvertItemToEntity(string hash,
                                                                             UserKnowledgeItem knowledgeItem,
                                                                             StudyLanguageContext c) {
            if (IsInvalid(knowledgeItem)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "UserKnowledgeQuery.Add переданы некорректные данные пользователю. Идентификатор пользователя {0}, идентификатор языка {1}, новый пункт знаний {2}",
                    _userId, _languageId, KnowledgeToLogString(knowledgeItem));
                return new Tuple<KnowledgeAddStatus, UserKnowledge>(KnowledgeAddStatus.Error, null);
            }

            UserKnowledge userKnowledge = c.UserKnowledge.FirstOrDefault(e => e.Hash == hash);
            if (userKnowledge != null) {
                if (GetDefaultDeletedDate() == userKnowledge.DeletedDate) {
                    return new Tuple<KnowledgeAddStatus, UserKnowledge>(KnowledgeAddStatus.AlreadyExists, userKnowledge);
                }
                userKnowledge.CreationDate = DateTime.Now;
                userKnowledge.DeletedDate = GetDefaultDeletedDate();
            } else {
                userKnowledge = AddKnowledge(knowledgeItem, c);
            }

            return new Tuple<KnowledgeAddStatus, UserKnowledge>(KnowledgeAddStatus.Ok, userKnowledge);
        }

        private static string KnowledgeToLogString(UserKnowledgeItem knowledgeItem) {
            return knowledgeItem != null
                       ? string.Format("dataId={0}, dataType={1}, data={2}, tip={3}",
                                       knowledgeItem.DataId, knowledgeItem.DataType, knowledgeItem.Data,
                                       knowledgeItem.Tip)
                       : "<NULL>";
        }

        /// <summary>
        /// Определяет может ли пользователь добавлять данные(не превышен ли сегодняшний лимит) 
        /// </summary>
        /// <param name="maxCountItemsPerDay">максимальное кол-во записей, которые пользователь может добавлять за сутки</param>
        /// <returns>true-пользователь может добавлять, false-пользователь не может добавлять</returns>
        private bool CanAdd(int maxCountItemsPerDay, StudyLanguageContext c) {
            DateTime minDate = GetDefaultDeletedDate();
            DateTime currentDate = GetCurrentDate();
            int countAddedItemsPerToday =
                c.UserKnowledge.Count(
                    e =>
                    e.UserId == _userId && e.LanguageId == _languageId && e.CreationDate >= currentDate
                    && e.DeletedDate == minDate);
            return countAddedItemsPerToday < maxCountItemsPerDay;
        }

        private static DateTime GetCurrentDate() {
            return DateTime.Today;
        }

        private UserKnowledge AddKnowledge(UserKnowledgeItem knowledgeItem, StudyLanguageContext c) {
            var userKnowledge = new UserKnowledge {
                UserId = _userId,
                LanguageId = _languageId,
                Data = knowledgeItem.Data,
                DataId = knowledgeItem.DataId,
                DataType = (int) knowledgeItem.DataType,
                Tip = knowledgeItem.Tip,
                Status = (byte) KnowledgeStatus.New,
                CreationDate = DateTime.Now,
                DeletedDate = GetDefaultDeletedDate(),
                SystemData = knowledgeItem.SystemData,
                Hash = GetUniqueHash(knowledgeItem)
            };
            c.UserKnowledge.Add(userKnowledge);
            return userKnowledge;
        }

        private static DateTime GetDefaultDeletedDate() {
            return new DateTime().GetDbDateTime();
        }

        private bool SetDeletedDate(long id, DateTime deletedDate) {
            bool result = false;
            Adapter.ActionByContext(c => {
                var sqlQueryParams = new List<object> {
                    id,
                    _userId,
                    _languageId,
                    deletedDate
                };

                //TODO: написать нормальную поддержку обновления и джоинов
                var sqlQuery = new StringBuilder("update UserKnowledge set DeletedDate={3}");
                sqlQuery.Append(" where Id={0} and UserId={1} and LanguageId={2}");

                int count = c.Database.ExecuteSqlCommand(sqlQuery.ToString(), sqlQueryParams.ToArray());
                result = count == 1;
            });
            return result;
        }

        private string GetUniqueHash(UserKnowledgeItem knowledgeItem) {
            const string SEPARATOR = "_";

            string data = _userId.ToString(CultureInfo.InvariantCulture) + SEPARATOR
                          + _languageId.ToString(CultureInfo.InvariantCulture)
                          + SEPARATOR + (knowledgeItem.Data ?? "<NULL>")
                          + SEPARATOR + knowledgeItem.DataId.ToString(CultureInfo.InvariantCulture) + SEPARATOR
                          + ((int) knowledgeItem.DataType).ToString(CultureInfo.InvariantCulture);
            var md5Helper = new Md5Helper();
            return md5Helper.GetHash(data, 1);
        }

        //TODO: удалить метод после конвертации
        public bool Convert() {
            bool result = false;
            Adapter.ActionByContext(c => {
                var newHashes = new HashSet<string>();
                List<UserKnowledge> userKnowledges =
                    c.UserKnowledge.Where(e => e.Hash != null && e.UserId == _userId && e.LanguageId == _languageId).
                        ToList();
                foreach (UserKnowledge userKnowledge in userKnowledges) {
                    var item = new UserKnowledgeItem(userKnowledge);
                    userKnowledge.Hash = GetUniqueHash(item);
                    newHashes.Add(userKnowledge.Hash);
                }
                c.SaveChanges();

                List<UserKnowledge> afterSaveKnowledges =
                    c.UserKnowledge.Where(e => e.Hash != null && e.UserId == _userId && e.LanguageId == _languageId).
                        ToList();
                result = userKnowledges.Count == afterSaveKnowledges.Count
                         && afterSaveKnowledges.All(e => newHashes.Contains(e.Hash));
            });
            return result;
        }
    }
}