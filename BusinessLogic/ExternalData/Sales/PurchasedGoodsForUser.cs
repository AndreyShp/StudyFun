using BusinessLogic.Data.Enums.Money;

namespace BusinessLogic.ExternalData.Sales {
    /// <summary>
    /// Данные о купленном товаре
    /// </summary>
    public class PurchasedGoodsForUser<T> {
        /// <summary>
        /// Цена товара
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Уникальный идентификатор скачивания для пользователя
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
        /// Купленный товар
        /// </summary>
        public T Goods { get; set; }
        /// <summary>
        /// Язык на котором пользователь покупал товар
        /// </summary>
        public long LanguageId { get; set; }
        /// <summary>
        /// Система оплаты
        /// </summary>
        public PaymentSystem PaymentSystem { get; set; }
        /// <summary>
        /// Указывает что купил пользователь
        /// </summary>
        public GoodsId GoodsId { get; set; }
    }
}