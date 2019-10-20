namespace BusinessLogic.Data.Video {
    public class VideoSentence {
        public long Id { get; set; }
        public long VideoId { get; set; }
        public string Source { get; set; }
        public string Translation { get; set; }
        public int Order { get; set; }
    }
}