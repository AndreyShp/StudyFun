using System;

namespace BusinessLogic.Data.Enums {
    [Flags]
    public enum SentenceType {
        Default = 0,
        Separate = 1,
        FromGroup = 2,
        ComparisonExample = 4
    }
}