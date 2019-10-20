namespace BusinessLogic.Data.Comparison {
    public class ComparisonRule {
        public long Id { get; set; }
        public long ComparisonItemId { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}