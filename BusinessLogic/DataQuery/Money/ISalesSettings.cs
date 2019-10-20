namespace BusinessLogic.DataQuery.Money {
    /// <summary>
    /// Интерфейс настроек для продаж товаров
    /// </summary>
    public interface ISalesSettings {
        /// <summary>
        /// Скидка
        /// </summary>
        decimal Discount { get; }

        /// <summary>
        /// Суммарная цена со скидкой
        /// </summary>
        decimal SummDiscountPrice { get; set; }

        /// <summary>
        /// Получить цену
        /// </summary>
        /// <param name="id">идентификатор товара для продажи</param>
        /// <param name="name">название товара для продажи</param>
        /// <returns>цена за товар</returns>
        decimal GetPrice(long id, string name);

        /// <summary>
        /// Проверяет корректные ли настройки
        /// </summary>
        bool IsInvalid { get; }
    }
}