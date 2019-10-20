namespace BusinessLogic.Data.Enums.Money {
    /// <summary>
    /// Статус купленного товара
    /// </summary>
    public enum PurchasedStatus : byte {
        /// <summary>
        /// Статус неизвестен
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Ожидаем оплаты от пользователя
        /// </summary>
        WaitPayment = 1,

        /// <summary>
        /// Успешно оплачено
        /// </summary>
        Success = 2,

        /// <summary>
        /// Не удалось оплатить
        /// </summary>
        Fail = 3,

        /// <summary>
        /// Посланно клиенту
        /// </summary>
        PostToCustomer = 100
    }
}