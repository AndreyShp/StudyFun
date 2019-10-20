using System;
using BusinessLogic.Data.Enums.Money;

namespace BusinessLogic.Data.Money {
    /// <summary>
    /// Купленные товары
    /// </summary>
    public class PurchasedGoods {
        /// <summary>
        /// Идентификатор платежа
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Цена товара
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Статус купленного товара
        /// </summary>
        public PurchasedStatus Status { get; set; }
        /// <summary>
        /// Уникальный идентификатор скачивания
        /// </summary>
        public string UniqueDownloadId { get; set; }
        /// <summary>
        /// Полное описание купленных товаров
        /// </summary>
        public string FullDescription { get; set; }
        /// <summary>
        /// Краткое описание купленных товаров
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Идентификатор пользователя(или 0 если не зарегистрирован)
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// Дата и время покупки
        /// </summary>
        public DateTime PurchaseDate { get; set; }
        /// <summary>
        /// Дата и время отправки клиенту товара
        /// </summary>
        public DateTime PostToCustomerDate { get; set; }
        /// <summary>
        /// Идентификатор платежа
        /// </summary>
        public long PaymentId { get; set; }
        /// <summary>
        /// JSON описание купленного товара
        /// </summary>
        public string Goods { get; set; }
        /// <summary>
        /// Язык на котором пользователь покупал товар
        /// </summary>
        public long LanguageId { get; set; }
        /// <summary>
        /// Указывает что покупает пользователь
        /// </summary>
        public byte GoodsId { get; set; }

        //TODO: возможно добавить как можно связаться с пользователем
    }
}