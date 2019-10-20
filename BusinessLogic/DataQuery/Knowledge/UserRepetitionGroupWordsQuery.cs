using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Data.Word;
using BusinessLogic.Extensions;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за получение слов для опреденной темы для периодичных повторений
    /// </summary>
    public class UserRepetitionGroupWordsQuery : BaseQuery, IUserRepetitionKnowledgeQuery {
        private readonly int _dataType;
        private readonly long _groupId;
        private readonly long _userId;
        private readonly long _languageId;

        public UserRepetitionGroupWordsQuery(long userId, long languageId, long groupId) {
            _userId = userId;
            _languageId = languageId;
            _groupId = groupId;
            _dataType = (int)KnowledgeDataType.WordTranslation;
        }

        #region IUserRepetitionKnowledgeQuery Members

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionQuery(StudyLanguageContext c,
                                                                                     DateTime minNextTimeShow,
                                                                                     DateTime maxNextTimeShow,
                                                                                     int count) {
            var joinedSequence = c.GroupWord.Join(c.UserRepetitionInterval,
                                                  gw => gw.WordTranslationId,
                                                  uri => uri.DataId,
                                                  (gw, uri) => new {gw, uri});
            joinedSequence =
                joinedSequence.Where(e =>
                                     e.gw.GroupId == _groupId && e.uri.UserId == _userId && e.uri.LanguageId == _languageId
                                     && e.uri.DataType == _dataType);

            joinedSequence =
                joinedSequence.Where(e => e.uri.NextTimeShow > minNextTimeShow && e.uri.NextTimeShow <= maxNextTimeShow)
                    .OrderBy(e => e.uri.NextTimeShow);

            IEnumerable<Tuple<UserKnowledge, UserRepetitionInterval>> joinedData =
                joinedSequence.AsEnumerable().Take(count).Select(e => ConvertRow(e.gw, e.uri));
            return joinedData.ToList();
        }

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionNewQuery(StudyLanguageContext c,
                                                                                        int count) {
            var joinedSequence = c.GroupWord.GroupJoin(c.UserRepetitionInterval,
                                                       gw => gw.WordTranslationId,
                                                       uri => uri.DataId,
                                                       (gw, uri) => new {gw, uri});
            var joinedData = joinedSequence.SelectMany(
                e => e.uri.Where(f => f.UserId == _userId && f.LanguageId == _languageId && f.DataType == _dataType).DefaultIfEmpty(),
                (e, uri) => new {e.gw, uri})
                .Where(e => e.gw.GroupId == _groupId)
                .Where(e => e.uri == null);

            return
                joinedData.AsEnumerable().Take(count).Select(
                    e => ConvertRow(e.gw, null)).ToList();
        }

        #endregion

        private Tuple<UserKnowledge, UserRepetitionInterval> ConvertRow(GroupWord groupWord,
                                                                        UserRepetitionInterval userRepetitionInterval) {
            DateTime minDateTime = new DateTime().GetDbDateTime();
            var userKnowledge = new UserKnowledge {
                Id = groupWord.Id,
                DataId = groupWord.WordTranslationId,
                DataType = _dataType,
                Data = null,
                UserId = _userId,
                LanguageId = _languageId,
                CreationDate = minDateTime,
                DeletedDate = minDateTime,
                Hash = null,
                Tip = null,
                SystemData = null,
                Status = (int) KnowledgeStatus.Unknown
            };
            return new Tuple<UserKnowledge, UserRepetitionInterval>(userKnowledge, userRepetitionInterval);
        }
    }
}