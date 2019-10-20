using System;

namespace BusinessLogic.Data.Comparison {
    public class GroupComparison {
        public const int TITLE_LENGTH = 200;

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdditionalInfo { get; set; }
        public bool IsVisible { get; set; }
        public int? Rating { get; set; }
        public DateTime LastModified { get; set; }
        public long LanguageId { get; set; }
    }
}