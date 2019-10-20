using System;

namespace BusinessLogic.Data.Knowledge {
    public class UserRepetitionInterval {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long LanguageId { get; set; }

        public long DataId { get; set; }
        public int DataType { get; set; }
        public int SourceType { get; set; }
        public int Mark { get; set; }
        public int RepetitionMark { get; set; }
        public int RepetitionTotal { get; set; }
        public DateTime NextTimeShow { get; set; }
    }
}