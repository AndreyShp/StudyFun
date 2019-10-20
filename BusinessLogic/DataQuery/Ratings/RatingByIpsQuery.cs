using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Rating;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Ratings {
    public class RatingByIpsQuery : BaseQuery, IRatingByIpsQuery, ICleaner {
        private readonly Dictionary<RatingPageType, IRatingQuery> _ratingQueriesByTypes =
            new Dictionary<RatingPageType, IRatingQuery>();

        public RatingByIpsQuery(long languageId) {
            _ratingQueriesByTypes.Add(RatingPageType.Word, new GroupsQuery(languageId));
            _ratingQueriesByTypes.Add(RatingPageType.Sentence, new GroupsQuery(languageId));
            _ratingQueriesByTypes.Add(RatingPageType.VisualDictionary, new RepresentationsQuery(languageId));
            _ratingQueriesByTypes.Add(RatingPageType.Comparison, new ComparisonsQuery(languageId));
            _ratingQueriesByTypes.Add(RatingPageType.Video, new VideosQuery(languageId));
        }

        #region IRatingByIpsQuery Members

        public bool AddNewVisitor(string ip, long entityId, RatingPageType ratingPageType) {
            if (!_ratingQueriesByTypes.ContainsKey(ratingPageType) || string.IsNullOrWhiteSpace(ip)
                || IdValidator.IsInvalid(entityId)) {
                return false;
            }

            var pageType = (int) ratingPageType;
            bool result = false;
            Adapter.ActionByContext(c => {
                RatingByIp rating =
                    c.RatingByIp.FirstOrDefault(e => e.Ip == ip && e.EntityId == entityId && e.PageType == pageType);
                if (rating != null) {
                    return;
                }
                rating = new RatingByIp {Ip = ip, EntityId = entityId, PageType = pageType};
                c.RatingByIp.Add(rating);
                c.SaveChanges();

                if (IdValidator.IsInvalid(rating.Id)) {
                    return;
                }
                result = _ratingQueriesByTypes[ratingPageType].IncRating(entityId);
            });
            return result;
        }

        public bool Clean(DateTime maxDateForRemove) {
            var res = Adapter.ActionByContext(c => {
                const string SQL_COMMAND = "delete from RatingByIp";
                c.Database.ExecuteSqlCommand(SQL_COMMAND);
            });
            return res;
        }

        #endregion
    }
}