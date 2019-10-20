using System;
using System.Collections.Generic;

namespace BusinessLogic.DataQuery.UserRepository.Texts {
    public class UserText {
        public string Title { get; set; }
        public string Text { get; set; }
        public List<TextWord> Words { get; set; }
        public long Pointer { get; set; }
        public DateTime CreationDate { get; set; }
    }
}