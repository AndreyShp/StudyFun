using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.Data.Representation;
using BusinessLogic.Extensions;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за получение слов визуального словаря для темы для периодичных повторений
    /// </summary>
    public class UserRepetitionVisualWordsQuery : BaseQuery, IUserRepetitionKnowledgeQuery {
        private readonly int _dataType;
        private readonly long _languageId;
        private readonly long _representationId;
        private readonly long _userId;

        public UserRepetitionVisualWordsQuery(long userId, long languageId, long representationId) {
            _userId = userId;
            _languageId = languageId;
            _representationId = representationId;
            _dataType = (int) KnowledgeDataType.WordTranslation;
        }

        #region IUserRepetitionKnowledgeQuery Members

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionQuery(StudyLanguageContext c,
                                                                                     DateTime minNextTimeShow,
                                                                                     DateTime maxNextTimeShow,
                                                                                     int count) {
            var joinedSequence = c.RepresentationArea.Join(c.UserRepetitionInterval,
                                                           ra => ra.WordTranslationId,
                                                           uri => uri.DataId,
                                                           (ra, uri) => new {ra, uri});
            joinedSequence =
                joinedSequence.Where(e =>
                                     e.ra.RepresentationId == _representationId && e.uri.UserId == _userId
                                     && e.uri.LanguageId == _languageId
                                     && e.uri.DataType == _dataType);

            joinedSequence =
                joinedSequence.Where(e => e.uri.NextTimeShow > minNextTimeShow && e.uri.NextTimeShow <= maxNextTimeShow)
                    .OrderBy(e => e.uri.NextTimeShow);

            IEnumerable<Tuple<UserKnowledge, UserRepetitionInterval>> joinedData =
                joinedSequence.AsEnumerable().Take(count).Select(e => ConvertRow(e.ra, e.uri));
            return joinedData.ToList();
        }

        public List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionNewQuery(StudyLanguageContext c,
                                                                                        int count) {
            var joinedSequence = c.RepresentationArea.GroupJoin(c.UserRepetitionInterval,
                                                                ra => ra.WordTranslationId,
                                                                uri => uri.DataId,
                                                                (ra, uri) => new {ra, uri});
            var joinedData = joinedSequence.SelectMany(
                e =>
                e.uri.Where(f => f.UserId == _userId && f.LanguageId == _languageId && f.DataType == _dataType).
                    DefaultIfEmpty(),
                (e, uri) => new {e.ra, uri})
                .Where(e => e.ra.RepresentationId == _representationId)
                .Where(e => e.uri == null);

            return
                joinedData.AsEnumerable().Take(count).Select(
                    e => ConvertRow(e.ra, null)).ToList();
        }

        #endregion

        private Tuple<UserKnowledge, UserRepetitionInterval> ConvertRow(RepresentationArea area,
                                                                        UserRepetitionInterval userRepetitionInterval) {
            DateTime minDateTime = new DateTime().GetDbDateTime();
            var userKnowledge = new UserKnowledge {
                Id = area.Id,
                DataId = area.WordTranslationId,
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