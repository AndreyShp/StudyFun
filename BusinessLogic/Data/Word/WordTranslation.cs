namespace BusinessLogic.Data.Word {
    public class WordTranslation {
        public long Id { get; set; }
        public long WordId1 { get; set; }
        public long WordId2 { get; set; }
        public byte[] Image { get; set; }
        public int? Rating { get; set; }
    }
}