using System;

namespace BusinessLogic.Logger {
    /// <summary>
    /// Как логируем
    /// </summary>
    [Flags]
    public enum LoggingSource : byte {
        /// <summary>
        /// Записываем в БД
        /// </summary>
        Db = 1,

        /// <summary>
        /// Посылаем по почте
        /// </summary>
        Mail = 2
    }
}