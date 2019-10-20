namespace BusinessLogic.Data.Sentence {
    public class ShuffleSentence {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long SentenceTranslationId { get; set; }
        public bool IsShown { get; set; }
        //public int Type { get; set; }
    }
}