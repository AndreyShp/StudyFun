namespace BusinessLogic.Data.Enums.Money {
    /// <summary>
    /// Идентификатор товара(указывает что покупает пользователь)
    /// NOTE: ДОБАВЛЯЯ СЮДА ЗНАЧЕНИЕ, НЕ ЗАБУДЬТЕ ДОБАВИТЬ ЕГО В SWITCH В PartialDownloadPurchasedGoods
    /// </summary>
    public enum GoodsId : byte {
        /// <summary>
        /// Визуальные словари
        /// </summary>
        VisualDictionaries = 1,

        /// <summary>
        /// Все материалы сайта
        /// </summary>
        AllMaterials = 2,
    }
}