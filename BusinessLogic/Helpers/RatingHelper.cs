namespace BusinessLogic.Helpers {
    public class RatingHelper {
        public static int GetRating(int? rating) {
            return rating.HasValue ? rating.Value : int.MinValue;
        }
    }
}