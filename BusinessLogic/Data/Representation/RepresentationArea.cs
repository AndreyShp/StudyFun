namespace BusinessLogic.Data.Representation {
    public class RepresentationArea {
        public long Id { get; set; }
        public long RepresentationId { get; set; }
        public int LeftUpperX { get; set; }
        public int LeftUpperY { get; set; }
        public int RightBottomX { get; set; }
        public int RightBottomY { get; set; }
        public long WordTranslationId { get; set; }
        public int? Rating { get; set; }
    }
}