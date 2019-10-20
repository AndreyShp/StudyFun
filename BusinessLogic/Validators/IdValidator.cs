namespace BusinessLogic.Validators {
    public static class IdValidator {
        public const long INVALID_ID = 0;

        public static bool IsInvalid(long id) {
            return !IsValid(id);
        }

        public static bool IsValid(long id) {
            return id > 0;
        }
    }
}