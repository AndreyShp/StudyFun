using System;
using BusinessLogic.Data.Enums.Money;

namespace BusinessLogic.Data.Money {
    /// <summary>
    /// Хранит данные об оплатах
    /// </summary>
    public class Payment {
        /// <summary>
        /// Идентификатор платежа
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Сумма оплаты
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Статус оплаты
        /// </summary>
        public PaymentStatus Status { get; set; }
        /// <summary>
        /// Описание товара
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Идентификатор пользователя(или 0 если не зарегистрирован)
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// Дата и время создания платежа
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Дата и время оплаты
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// Система оплаты
        /// </summary>
        public PaymentSystem System { get; set; }
    }
}