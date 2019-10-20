namespace BusinessLogic.Data.Sentence {
    public class SentenceTranslation {
        public long Id { get; set; }
        public long SentenceId1 { get; set; }
        public long SentenceId2 { get; set; }
        public byte[] Image { get; set; }
        public int? Rating { get; set; }
        public int Type { get; set; }
    }
}