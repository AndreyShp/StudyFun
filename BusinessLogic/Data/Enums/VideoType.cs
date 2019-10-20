using System;

namespace BusinessLogic.Data.Enums {
    /// <summary>
    /// Тип видео
    /// </summary>
    [Flags]
    public enum VideoType : byte {
        /// <summary>
        /// Видеоролик
        /// </summary>
        Clip = 1,

        /// <summary>
        /// Фильм
        /// </summary>
        Movie = 2,
    }
}