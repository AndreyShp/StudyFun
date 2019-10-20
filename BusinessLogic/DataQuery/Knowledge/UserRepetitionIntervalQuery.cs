using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Extensions;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за получение данных по периодичным повторением
    /// </summary>
    public class UserRepetitionIntervalQuery : BaseQuery, IUserRepetitionIntervalQuery {
        private readonly long _languageId;
        private readonly NextTimeCalculator _nextTimeCalculator = new NextTimeCalculator();
        private readonly IUserRepetitionKnowledgeQuery _repetitionQuery;
        private readonly RepetitionType _repetitionType;
        private readonly int _sourceType;
        private readonly long _userId;
        private readonly IUserKnowledgeQuery _userKnowledgeQuery;

        public UserRepetitionIntervalQuery(long userId,
                                           long languageId,
                                           KnowledgeSourceType sourceType,
                                           IUserRepetitionKnowledgeQuery repetitionQuery,
                                           RepetitionType repetitionType) {
            _userId = userId;
            _languageId = languageId;
            _sourceType = (int) sourceType;
            _userKnowledgeQuery = new UserKnowledgeQuery(userId, languageId);
            _repetitionQuery = repetitionQuery;
            _repetitionType = repetitionType;
        }

        #region IUserRepetitionIntervalQuery Members

        /// <summary>
        /// Возвращает count данных, которые нужно повторить
        /// </summary>
        /// <param name="sourceLanguageId">идентификатор языка с которого переводят</param>
        /// <param name="translationLanguageId">идентификатор языка на который переводят</param>
        /// <param name="count">кол-во записей</param>
        /// <returns>если есть, то данные которые нужно повторить, иначе пустой набор</returns>
        public List<UserRepetitionIntervalItem> GetRepetitionIntervalItems(long sourceLanguageId,
                                                                           long translationLanguageId,
                                                                           int count) {
            List<UserRepetitionIntervalItem> result = Adapter.ReadByContext(c => {
                DateTime dateTimeNow = DateTime.Now;
                //получать данные для которых подошло данные для показа
                List<Tuple<UserKnowledge, UserRepetitionInterval>> joinedData = _repetitionQuery.GetRepetitionQuery(c,
                                                                                                                    DateTime
                                                                                                                        .
                                                                                                                        MinValue,
                                                                                                                    dateTimeNow,
                                                                                                                    count);
                //NOTE: определяем время для показа, для данных, которые никогда и никому не показывали - чтобы эти данные всегда были перед данными
                //NOTE: время для показа, которых еще не подошло, но после данных, время которых просрочено
                DateTime unknownNextTimeToShow = EnumerableValidator.IsEmpty(joinedData)
                                                     ? new DateTime().GetDbDateTime()
                                                     : dateTimeNow.AddMilliseconds(1);
                if (_repetitionType == RepetitionType.All) {
                    //получать данные, которые ни разу не показывали пользователю
                    AddOtherData(count, joinedData, wantage => _repetitionQuery.GetRepetitionNewQuery(c, wantage));
                    //получать данные для которых время показа - еще не подошло
                    AddOtherData(count, joinedData,
                                 wantage =>
                                 _repetitionQuery.GetRepetitionQuery(c, dateTimeNow, DateTime.MaxValue, wantage));
                }

                return ConvertToItems(sourceLanguageId, translationLanguageId, joinedData, unknownNextTimeToShow);
            }, new List<UserRepetitionIntervalItem>(0));
            return result;
        }

        /// <summary>
        /// Выставляет оценку данным
        /// </summary>
        /// <param name="intervalItem">данные</param>
        /// <param name="mark">оценка</param>
        /// <returns>true - оценку удалось поставить, false - оценку не удалось поставить</returns>
        public bool SetMark(UserRepetitionIntervalItem intervalItem, KnowledgeMark mark) {
            bool result = false;
            var parsedDataType = (int) intervalItem.DataType;
            Adapter.ActionByContext(c => {
                UserRepetitionInterval userRepetitionInterval =
                    c.UserRepetitionInterval.FirstOrDefault(
                        e =>
                        e.UserId == _userId && e.LanguageId == _languageId && e.DataId == intervalItem.DataId
                        && e.DataType == parsedDataType);
                if (userRepetitionInterval == null) {
                    userRepetitionInterval = new UserRepetitionInterval {
                        UserId = _userId,
                        LanguageId = _languageId,
                        DataId = intervalItem.DataId,
                        DataType = parsedDataType,
                        NextTimeShow = new DateTime().GetDbDateTime(),
                    };
                    userRepetitionInterval.SourceType |= (int) intervalItem.SourceType;
                    c.UserRepetitionInterval.Add(userRepetitionInterval);
                }

                var parsedMark = (int) mark;

                SetNextByMark(userRepetitionInterval, mark);
                if (parsedMark != userRepetitionInterval.Mark) {
                    //оценка не совпадает - кол-во повторений для оценки сбрасываем
                    userRepetitionInterval.RepetitionMark = 0;
                } else {
                    //оценка совпала - кол-во повторений для оценки установить
                    userRepetitionInterval.RepetitionMark++;
                }
                userRepetitionInterval.Mark = parsedMark;
                userRepetitionInterval.RepetitionTotal++;

                int count = c.SaveChanges();
                result = count == 1;
            });
            return result;
        }

        /// <summary>
        /// Удаляет данные пользователя, для которых уже нет данных на странице со знаниями пользователя
        /// </summary>
        /// <returns>true - данные успешно удалены, false - не удалось удалить данные</returns>
        public bool RemoveWithoutData() {
            return Adapter.ReadByContext(c => {
                //NOTE: SourceType намерено через == , а не через &, т.к. удалять можем только те данные которые пользователь просматривал из своего тренажера
                var joinedSequence = c.UserRepetitionInterval.GroupJoin(c.UserKnowledge,
                                                                        uri =>
                                                                        new {
                                                                            uri.UserId,
                                                                            uri.LanguageId,
                                                                            uri.DataType,
                                                                            DataId = (long?) uri.DataId
                                                                        },
                                                                        uk =>
                                                                        new {
                                                                            uk.UserId,
                                                                            uk.LanguageId,
                                                                            uk.DataType,
                                                                            uk.DataId
                                                                        },
                                                                        (uri, uk) => new {uri, uk});
                var joinedData = joinedSequence.SelectMany(e => e.uk.DefaultIfEmpty(), (e, uk) => new {e.uri, uk})
                    .Where(
                        e =>
                        e.uri.UserId == _userId && e.uri.LanguageId == _languageId && e.uri.SourceType == _sourceType)
                    .Where(e => e.uk == null);

                int i = 0;
                foreach (var data in joinedData.AsEnumerable()) {
                    c.UserRepetitionInterval.Remove(data.uri);
                    i++;
                    if (i % 100 == 0) {
                        c.SaveChanges();
                    }
                }
                c.SaveChanges();
                return true;
            });
        }

        #endregion

        private static void AddOtherData(int count,
                                         List<Tuple<UserKnowledge, UserRepetitionInterval>> joinedData,
                                         Func<int, List<Tuple<UserKnowledge, UserRepetitionInterval>>> dataGetter) {
            int wantage = count - joinedData.Count;
            if (wantage > 0) {
                List<Tuple<UserKnowledge, UserRepetitionInterval>> partition = dataGetter(wantage);
                joinedData.AddRange(partition);
            }
        }

        private void SetNextByMark(UserRepetitionInterval userRepetitionInterval, KnowledgeMark mark) {
            int prevMark = userRepetitionInterval.Mark;
            double ratio = 0;
            if (prevMark == ((int) mark)) {
                //оценка таже самая - взять отношение
                ratio = userRepetitionInterval.RepetitionMark / (double) userRepetitionInterval.RepetitionTotal;
                /*if (mark == KnowledgeMark.VeryEasy && userRepetitionInterval.RepetitionMark > 10 && ratio <= 0.2) {
                    //пользователь помнит слово хорошо уже более 10 раз ответил верно
                    userRepetitionInterval.RepetitionTotal = Math.Round(userRepetitionInterval.RepetitionTotal * 0.9);
                }*/
            }

            TimeSpan repeatInterval = _nextTimeCalculator.Calculate(mark, ratio);

            userRepetitionInterval.NextTimeShow = DateTime.Now.Add(repeatInterval);
        }

        private List<UserRepetitionIntervalItem> ConvertToItems(long sourceLanguageId,
                                                                long translationLanguageId,
                                                                IEnumerable
                                                                    <Tuple<UserKnowledge, UserRepetitionInterval>>
                                                                    joinedData,
                                                                DateTime unknownNextTimeToShow) {
            var userKnowledges = new List<UserKnowledge>();
            var repetitionsByItemIds = new Dictionary<long, UserRepetitionInterval>();
            foreach (var joinedRow in joinedData) {
                UserKnowledge uk = joinedRow.Item1;
                UserRepetitionInterval uri = joinedRow.Item2;
                userKnowledges.Add(uk);
                repetitionsByItemIds.Add(uk.Id, uri);
            }

            List<UserKnowledgeItem> items = _userKnowledgeQuery.ConvertEntitiesToItems(sourceLanguageId,
                                                                                       translationLanguageId,
                                                                                       userKnowledges);
            var innerResult = new List<UserRepetitionIntervalItem>();
            foreach (UserKnowledgeItem userKnowledgeItem in items) {
                long id = userKnowledgeItem.Id;
                UserRepetitionInterval repetitionItem = repetitionsByItemIds[id];
                var item = new UserRepetitionIntervalItem(repetitionItem, userKnowledgeItem, unknownNextTimeToShow);
                innerResult.Add(item);
            }

            return innerResult;
        }
    }
}