namespace BusinessLogic.Data.Comparison {
    public class ComparisonItem {
        public const int TITLE_LENGTH = 100;

        public long Id { get; set; }
        public long GroupComparisonId { get; set; }
        public string Title { get; set; }
        public string TitleTranslation { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}