namespace BusinessLogic.Data.Enums.Money {
    /// <summary>
    /// Платежная система
    /// </summary>
    public enum PaymentSystem : byte {
        /// <summary>
        /// Неизвестна
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Робокасса
        /// </summary>
        Robokassa = 1,
    }
}