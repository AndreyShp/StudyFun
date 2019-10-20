using System.Web.Mvc;

namespace StudyLanguages.Models.Sales {
    public class SalesFinishStepModel {
        public SalesFinishStepModel(string uniqueDownloadId, UrlHelper url) {
            UniqueDownloadId = uniqueDownloadId;
            CheckUrl = url.Action("Check", RouteConfig.PAYMENT_CONTROLLER);
        }

        /// <summary>
        /// Уникальный идентификатор проверки
        /// </summary>
        public string UniqueDownloadId { get; private set; }
        
        /// <summary>
        /// Урл для проверки оплаты покупки
        /// </summary>
        public string CheckUrl { get; private set; }
    }
}