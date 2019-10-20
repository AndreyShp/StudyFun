using System;

namespace BusinessLogic.Data.Representation {
    public class Representation {
        public long Id { get; set; }
        public string Title { get; set; }
        public byte[] Image { get; set; }
        public bool IsVisible { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int? Rating { get; set; }
        public byte? WidthPercent { get; set; }
        public DateTime LastModified { get; set; }
        public long LanguageId { get; set; }
    }
}