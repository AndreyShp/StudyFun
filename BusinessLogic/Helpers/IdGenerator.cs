using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BusinessLogic.Helpers {
    /// <summary>
    /// Генератор идентификаторов
    /// </summary>
    public class IdGenerator {
        /// <summary>
        /// Длина ключа по умолчанию
        /// </summary>
        public const int DEFAULT_LEN = 32;

        /// <summary>
        /// Длина уникального ключа покупки
        /// </summary>
        public const int DOWNLOAD_ID_LEN = 50;

        private readonly Md5Helper _md5Helper = new Md5Helper();

        /// <summary>
        /// Генерирует идентификатор
        /// </summary>
        /// <param name="count">кол-во раз генерирования хэша</param>
        /// <returns>сгенерированный идентификатор</returns>
        public string GenerateByCount(int count) {
            string data = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture) + Guid.NewGuid();
            return _md5Helper.GetHash(data, count);
        }

        /// <summary>
        /// Генерирует идентификатор по длине
        /// </summary>
        /// <returns>сгенерированный идентификатор</returns>
        public static string GenerateByLength(int length) {
            var rnd = new Random();
            var idGenerator = new IdGenerator();
            var result = new StringBuilder();
            do {
                int countComputeHash = rnd.Next(1, 4);
                result.Append(idGenerator.GenerateByCount(countComputeHash));
            } while (result.Length < length);
            if (result.Length > length) {
                result = result.Remove(length, result.Length - length);
            }
            return result.ToString();
        }

        /// <summary>
        /// Проверяет корректность пользовательского идентификатора
        /// </summary>
        /// <param name="id">идентификатор пользователя</param>
        /// <param name="expectedLength">корректная длина хэша</param>
        /// <returns>true - идентификатор корректный, false - идентификатор некорректный</returns>
        public static bool IsValid(string id, int expectedLength) {
            var allowedChars =
                new HashSet<char>(new[] {'a', 'b', 'c', 'd', 'e', 'f', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});
            return id != null && id.Length == expectedLength && id.All(allowedChars.Contains);
        }

        /// <summary>
        /// Проверяет корректность идентификатора загрузки
        /// </summary>
        /// <param name="id">идентификатор загрузки</param>
        /// <returns>true - идентификатор корректный, false - идентификатор некорректный</returns>
        public static bool IsInvalidDownloadId(string id) {
            return !IsValid(id, DOWNLOAD_ID_LEN);
        }
    }
}