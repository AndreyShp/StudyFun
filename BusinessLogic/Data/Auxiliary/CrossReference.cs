namespace BusinessLogic.Data.Auxiliary {
    public class CrossReference {
        public long Id { get; set; }
        public long SourceId { get; set; }
        public int SourceType { get; set; }
        public long DestinationId { get; set; }
        public int DestinationType { get; set; }
    }
}