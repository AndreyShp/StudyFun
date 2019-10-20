namespace BusinessLogic.Data.Enums.Money {
    /// <summary>
    /// Статус оплаты
    /// </summary>
    public enum PaymentStatus : byte {
        /// <summary>
        /// Статус неизвестен
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// В процессе оплаты
        /// </summary>
        InProcess = 1,

        /// <summary>
        /// Успешно оплачено
        /// </summary>
        Success = 2,

        /// <summary>
        /// Не удалось оплатить
        /// </summary>
        Fail = 3
    }
}