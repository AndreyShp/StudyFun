namespace StudyLanguages.Models.Sales {
    /// <summary>
    /// Модель неудачной оплаты товаров
    /// </summary>
    public class FailPaymentModel {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pageRequiredData">дополнительная необходимая информация для страницы</param>
        /// <param name="header">заголовок для страницы</param>
        public FailPaymentModel(PageRequiredData pageRequiredData,
                                string header) {
            PageRequiredData = pageRequiredData;
            Header = header;
        }

        /// <summary>
        /// Дополнительная необходимая информация для страницы
        /// </summary>
        public PageRequiredData PageRequiredData { get; private set; }

        /// <summary>
        /// Заголовок для страницы
        /// </summary>
        public string Header { get; private set; }
    }
}