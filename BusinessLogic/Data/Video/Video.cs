using System;

namespace BusinessLogic.Data.Video {
    public class Video {
        public long Id { get; set; }
        public string Title { get; set; }
        public string HtmlCode { get; set; }
        public byte[] Image { get; set; }
        public bool IsVisible { get; set; }
        public int? Rating { get; set; }
        public DateTime LastModified { get; set; }
        public long LanguageId { get; set; }
        public byte Type { get; set; }
    }
}