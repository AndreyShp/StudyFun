using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Data.Sentence;
using BusinessLogic.Extensions;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за получение фраз для опреденной темы для периодичных повторений
    /// </summary>
    public class UserRepetitionGroupPhrasesQuery : BaseQuery, IUserRepetitionKnowledgeQuery {
        private readonly int _dataType;
        private readonly long _groupId;
        private readonly long _userId;
        private readonly long _languageId;

        public UserRepetitionGroupPhrasesQuery(long userId, long languageId, long groupId) {
            _userId = userId;
            _languageId = languageId;
            _groupId = groupId;
            _dataType = (int) KnowledgeDataType.PhraseTranslation;
        }

        #region IUserRepetitionKnowledgeQuery Members

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionQuery(StudyLanguageContext c,
                                                                                     DateTime minNextTimeShow,
                                                                                     DateTime maxNextTimeShow,
                                                                                     int count) {
            var joinedSequence = c.GroupSentence.Join(c.UserRepetitionInterval,
                                                      gs => gs.SentenceTranslationId,
                                                      uri => uri.DataId,
                                                      (gs, uri) => new {gs, uri});
            joinedSequence =
                joinedSequence.Where(e =>
                                     e.gs.GroupId == _groupId && e.uri.UserId == _userId && e.uri.LanguageId == _languageId
                                     && e.uri.DataType == _dataType);

            joinedSequence =
                joinedSequence.Where(e => e.uri.NextTimeShow > minNextTimeShow && e.uri.NextTimeShow <= maxNextTimeShow)
                    .OrderBy(e => e.uri.NextTimeShow);

            IEnumerable<Tuple<UserKnowledge, UserRepetitionInterval>> joinedData =
                joinedSequence.AsEnumerable().Take(count).Select(e => ConvertRow(e.gs, e.uri));
            return joinedData.ToList();
        }

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionNewQuery(StudyLanguageContext c,
                                                                                        int count) {
            var joinedSequence = c.GroupSentence.GroupJoin(c.UserRepetitionInterval,
                                                           gs => gs.SentenceTranslationId,
                                                           uri => uri.DataId,
                                                           (gs, uri) => new {gs, uri});
            var joinedData = joinedSequence.SelectMany(
                e => e.uri.Where(f => f.UserId == _userId && f.LanguageId == _languageId && f.DataType == _dataType).DefaultIfEmpty(),
                (e, uri) => new {e.gs, uri})
                .Where(e => e.gs.GroupId == _groupId)
                .Where(e => e.uri == null);

            return
                joinedData.AsEnumerable().Take(count).Select(
                    e => ConvertRow(e.gs, null)).ToList();
        }

        #endregion

        private Tuple<UserKnowledge, UserRepetitionInterval> ConvertRow(GroupSentence groupSentence,
                                                                        UserRepetitionInterval userRepetitionInterval) {
            DateTime minDateTime = new DateTime().GetDbDateTime();
            var userKnowledge = new UserKnowledge {
                Id = groupSentence.Id,
                DataId = groupSentence.SentenceTranslationId,
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