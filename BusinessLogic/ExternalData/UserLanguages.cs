using BusinessLogic.Data;

namespace BusinessLogic.ExternalData {
    /// <summary>
    /// Текущие языки пользователя
    /// </summary>
    public class UserLanguages {
        /// <summary>
        /// Язык, с которого нужно переводить
        /// </summary>
        public Language From { get; set; }

        /// <summary>
        /// Язык, на который нужно переводить 
        /// </summary>
        public Language To { get; set; }

        /// <summary>
        /// Определяет корректность
        /// </summary>
        private bool IsValid() {
            return From != null && To != null && From.Id != To.Id;
        }

        public static bool IsValid(UserLanguages userLanguages) {
            return userLanguages != null && userLanguages.IsValid();
        }

        public static bool IsInvalid(UserLanguages userLanguages) {
            return !IsValid(userLanguages);
        }
    }
}