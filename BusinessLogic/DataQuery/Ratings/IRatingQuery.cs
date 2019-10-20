namespace BusinessLogic.DataQuery.Ratings {
    internal interface IRatingQuery {
        bool IncRating(long entityId);
    }
}