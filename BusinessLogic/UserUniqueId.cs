using System;
using System.Text;
using BusinessLogic.Helpers;

namespace BusinessLogic {
    /// <summary>
    /// Пользовательский идентификатор
    /// </summary>
    public class UserUniqueId {
        public const int USER_HASH_LEN = 96;
        
        /// <summary>
        /// Проверяет корректность пользовательского идентификатора
        /// </summary>
        /// <param name="userUniqueId">идентификатор пользователя</param>
        /// <returns>true - идентификатор корректный, false - идентификатор некорректный</returns>
        public bool IsValid(string userUniqueId) {
            return IdGenerator.IsValid(userUniqueId, USER_HASH_LEN);
        }

        /// <summary>
        /// Генерирует уникальный идентификатор пользователя
        /// </summary>
        /// <returns>уникальный идентификатор пользователя</returns>
        public string New() {
            return IdGenerator.GenerateByLength(USER_HASH_LEN);
        }
    }
}