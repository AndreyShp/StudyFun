using BusinessLogic.Data.Enums.Money;

namespace StudyLanguages.Models.Sales {
    /// <summary>
    /// Модель успешной оплаты товаров
    /// </summary>
    public class SuccessPaymentModel {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="goodsDescription">описание купленного товара</param>
        /// <param name="uniqueDownloadId">уникальный идентификатор скачивания</param>
        /// <param name="goodsId"> </param>
        public SuccessPaymentModel(string goodsDescription, string uniqueDownloadId, GoodsId goodsId) {
            GoodsDescription = goodsDescription;
            UniqueDownloadId = uniqueDownloadId;
            GoodsId = goodsId;
        }

        /// <summary>
        /// Описание оплаченного товара
        /// </summary>
        public string GoodsDescription { get; private set; }

        /// <summary>
        /// Уникальный идентификатор купленного товара
        /// </summary>
        public string UniqueDownloadId { get; private set; }

        /// <summary>
        /// Категория купленного товара товара(указывает что купил пользователь)
        /// </summary>
        public GoodsId GoodsId { get; private set; }
    }
}