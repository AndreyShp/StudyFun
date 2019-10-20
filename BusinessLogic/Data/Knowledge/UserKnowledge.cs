using System;

namespace BusinessLogic.Data.Knowledge {
    public class UserKnowledge {
        public const int HASH_LENGTH = 32;

        public long Id { get; set; }
        public long UserId { get; set; }
        public long LanguageId { get; set; }
        /// <summary>
        /// Данные пользователя
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// Ссылка на источник данных(то, откуда пользователь взял данные)
        /// </summary>
        public long? DataId { get; set; }
        public int DataType { get; set; }
        public string Tip { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DeletedDate { get; set; }
        /// <summary>
        /// Системные данные
        /// </summary>
        public string SystemData { get; set; }
        /// <summary>
        /// Уникальный хэш записи
        /// </summary>
        public string Hash { get; set; }
    }
}