using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Extensions;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за получение знаний пользователя по периодичным повторениям
    /// </summary>
    public class UserRepetitionKnowledgeQuery : BaseQuery, IUserRepetitionKnowledgeQuery {
        private const int SOURCE_TYPE = (int) KnowledgeSourceType.Knowledge;
        private readonly KnowledgeDataType _dataType;
        private readonly long _languageId;
        private readonly long _userId;

        //TODO: создавать этот класс с помощью IoCModule
        public UserRepetitionKnowledgeQuery(long userId, long languageId, KnowledgeDataType dataType) {
            _userId = userId;
            _languageId = languageId;
            _dataType = dataType;
        }

        #region IUserRepetitionKnowledgeQuery Members

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionQuery(StudyLanguageContext c,
                                                                                     DateTime minNextTimeShow,
                                                                                     DateTime maxNextTimeShow,
                                                                                     int count) {
            DateTime minDateTime = new DateTime().GetDbDateTime();
            var joinedSequence = c.UserKnowledge.Join(c.UserRepetitionInterval,
                                                      uk => new {uk.UserId, uk.LanguageId, uk.DataType, uk.DataId},
                                                      uri =>
                                                      new {
                                                          uri.UserId,
                                                          uri.LanguageId,
                                                          uri.DataType,
                                                          DataId = (long?) uri.DataId
                                                      },
                                                      (uk, uri) => new {uk, uri});
            joinedSequence =
                joinedSequence.Where(
                    e =>
                    e.uk.UserId == _userId && e.uk.LanguageId == _languageId && e.uk.DeletedDate == minDateTime
                    && (e.uri.SourceType & SOURCE_TYPE) == SOURCE_TYPE);
            if (_dataType != KnowledgeDataType.All) {
                joinedSequence = joinedSequence.Where(e => e.uk.DataType == (int) _dataType);
            }
            joinedSequence =
                joinedSequence.Where(e => e.uri.NextTimeShow > minNextTimeShow && e.uri.NextTimeShow <= maxNextTimeShow)
                    .OrderBy(e => e.uri.NextTimeShow);
            /*new { uk.UserId, uk.DataId }*/
            /* (from uk in c.UserKnowledge
                                  join uri in c.UserRepetitionInterval on new { uk.UserId, uk.DataId } equals new { uri.UserId, DataId = (long?)uri.DataId }
                                  where (uk.UserId == _userId && uk.DataType == _dataType
                                         && uk.DeletedDate == minDateTime 
                                         && (uri.SourceType & _sourceType) == _sourceType
                                         && uri.NextTimeShow <= maxNextTimeShow)
                                  orderby uri.NextTimeShow
                                  select new {uk, uri}).Take(count);*/

            IEnumerable<Tuple<UserKnowledge, UserRepetitionInterval>> joinedData =
                joinedSequence.AsEnumerable().Take(count).Select(e => new Tuple<UserKnowledge, UserRepetitionInterval>(e.uk, e.uri));
            return joinedData.ToList();
        }

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionNewQuery(StudyLanguageContext c,
                                                                                        int count) {
            DateTime minDateTime = new DateTime().GetDbDateTime();
            /*var joinedSequence = (from uk in c.UserKnowledge
                                  join uri in c.UserRepetitionInterval on new { uk.UserId, uk.DataId } equals new { uri.UserId, DataId = (long?) uri.DataId } into joinedUri                                  
                                  from uri in joinedUri.DefaultIfEmpty()
                                  where uk.UserId == _userId
                                  && uk.DataType == _dataType
                                  && uk.DeletedDate == minDateTime
                                  && uri == null
                                  select new {uk, uri}).Take(count);*/
            /*var joinedSequence = c.UserKnowledge.GroupJoin(c.UserRepetitionInterval,
                                                           uk => new {uk.UserId, uk.DataId},
                                                           uri => new {uri.UserId, DataId = (long?) uri.DataId},
                                                           (uk, uri) => new {uk, uri});
            joinedSequence = joinedSequence.Where(e => e.uk.UserId == _userId && e.uk.DeletedDate == minDateTime);
            var joinedData =
                joinedSequence.Where(e => e.uri == null).SelectMany(e => e.uri.DefaultIfEmpty(), (e, uri) => new { e.uk }).Take(count).AsEnumerable();*/

            var joinedSequence = c.UserKnowledge.GroupJoin(c.UserRepetitionInterval,
                                                           uk => new {uk.UserId, uk.LanguageId, uk.DataType, uk.DataId},
                                                           uri =>
                                                           new {uri.UserId, uri.LanguageId, uri.DataType, DataId = (long?) uri.DataId},
                                                           (uk, uri) => new {uk, uri});
            var joinedData = joinedSequence.SelectMany(e => e.uri.DefaultIfEmpty(), (e, uri) => new {e.uk, uri})
                .Where(e => e.uk.UserId == _userId && e.uk.LanguageId == _languageId && e.uk.DeletedDate == minDateTime)
                .Where(e => e.uri == null);
            if (_dataType != KnowledgeDataType.All) {
                joinedData = joinedData.Where(e => e.uk.DataType == (int) _dataType);
            }
            return
                joinedData.AsEnumerable().Take(count).Select(
                    e => new Tuple<UserKnowledge, UserRepetitionInterval>(e.uk, null)).ToList();
        }

        #endregion

        /*var joinedSequence = (from uk in c.UserKnowledge
                              join uri in c.UserRepetitionInterval on uk.Id equals uri.DataId into joinedUri
                              from uri in joinedUri.DefaultIfEmpty()
                              where uk.UserId == _userId && uk.DeletedDate == minDateTime &&
                                    (uri == null
                                     ||
                                     (uri.SourceType == _sourceType && uri.NextTimeShow <= maxNextTimeShow))
                              select new { uk, uri }).Take(count);*/
    }
}