namespace BusinessLogic.Logger {
    /// <summary>
    /// Тип сообщения
    /// </summary>
    public enum LoggingType : byte {
        /// <summary>
        /// Ошибка
        /// </summary>
        Error = 1,

        /// <summary>
        /// Информация
        /// </summary>
        Info = 10
    }
}