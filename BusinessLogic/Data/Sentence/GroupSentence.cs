namespace BusinessLogic.Data.Sentence {
    public class GroupSentence {
        public long Id { get; set; }
        public long GroupId { get; set; }
        public long SentenceTranslationId { get; set; }
        public int? Rating { get; set; }
    }
}