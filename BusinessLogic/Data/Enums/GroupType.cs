using System;

namespace BusinessLogic.Data.Enums {
    [Flags]
    public enum GroupType {
        ByWord = 1,
        BySentence = 2
    }
}