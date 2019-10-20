namespace BusinessLogic.Data.Sentence {
    public class SentenceWord {
        public long Id { get; set; }
        public long SentenceId { get; set; }
        public long WordId { get; set; }
        public string OriginalText { get; set; }
        public int OrderInSentence { get; set; }
        public int GrammarType { get; set; }
    }
}