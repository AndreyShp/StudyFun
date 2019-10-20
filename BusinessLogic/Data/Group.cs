using System;

namespace BusinessLogic.Data {
    public class Group {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public int? Rating { get; set; }
        public byte[] Image { get; set; }
        public int Type { get; set; }
        public DateTime LastModified { get; set; }
        public long LanguageId { get; set; }
    }
}