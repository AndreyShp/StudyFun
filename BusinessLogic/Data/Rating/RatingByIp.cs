namespace BusinessLogic.Data.Rating {
    public class RatingByIp {
        public const int IP_LENGTH = 39;

        public long Id { get; set; }
        public int PageType { get; set; }
        public long EntityId { get; set; }
        public string Ip { get; set; }
    }
}