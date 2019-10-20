using System;

namespace BusinessLogic.Validators {
    public class EnumValidator {
        public static bool IsValid(Enum dirtyEnum) {
            bool result = Enum.IsDefined(dirtyEnum.GetType(), dirtyEnum);
            return result;
        }

        public static bool IsInvalid(Enum dirtyEnum) {
            return !IsValid(dirtyEnum);
        }
    }
}