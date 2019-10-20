using BusinessLogic.Data.Enums;

namespace BusinessLogic.DataQuery.Ratings {
    public interface IRatingByIpsQuery {
        bool AddNewVisitor(string ip, long entityId, RatingPageType ratingPageType);
    }
}