namespace BusinessLogic.Data.Word {
    public class ShuffleWord {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long WordTranslationId { get; set; }
        public int Type { get; set; }
        public bool IsShown { get; set; }
    }
}