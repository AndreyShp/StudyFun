using System.Web.Mvc;
using BusinessLogic.Data.Enums.Money;
using BusinessLogic.Data.Money;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.PaymentSystems;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Formatters;
using StudyLanguages.Models;
using StudyLanguages.Models.Sales;

namespace StudyLanguages.Controllers {
    /// <summary>
    /// Контроллер отвечающий за прием информации о платежах от платежных систем(пока только Robokassa)
    /// </summary>
    [NoCache]
    public class PaymentController : Controller {
        private const string ERROR_MESSAGE = "bad sign";

        /// <summary>
        /// Метод вызывается платежной системой сообщая нам, что платеж совершен
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Result() {
            RobokassaApi robokassaApi = ApiFactory.GetRobokassaApi(WebSettingsConfig.Instance);
            RobokassaPaymentResult paymentResult = robokassaApi.ProcessResult(Request.Params);
            if (paymentResult == null) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PaymentController.Result. PaymentResult is null. Params={0}",
                                            HttpContextHelper.ParamsToString(Request.Params, RobokassaApi.IsValidParamName));
                return Content(ERROR_MESSAGE);
            }

            var purchasedGoogsQuery = new PurchasedGoodsQuery();
            bool isSuccess = purchasedGoogsQuery.SuccessfullyPurchased(paymentResult.PaymentId, paymentResult.Price);
            if (!isSuccess) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PaymentController.Result. SuccessfullyPurchased вернул false. PaymentId={0}, Price={1}, Params={2}",
                                            paymentResult.PaymentId, paymentResult.Price,
                                            HttpContextHelper.ParamsToString(Request.Params, RobokassaApi.IsValidParamName));
                return Content(ERROR_MESSAGE);
            }

            LoggerWrapper.RemoteMessage(LoggingType.Info,
                                        "PaymentController.Result. Прошла оплата на сумму {0} с идентификатором {1}",
                                        MoneyFormatter.ToRubles(paymentResult.Price), paymentResult.PaymentId);

            string response = robokassaApi.GetResponseResultOk(paymentResult.PaymentId);
            return Content(response);
        }

        /// <summary>
        /// Метод вызывается платежной системой после оплаты и после сообщения нам о платеже(см. метод Result <see cref="Result"/>)
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult Success() {
            RobokassaApi robokassaApi = ApiFactory.GetRobokassaApi(WebSettingsConfig.Instance);
            RobokassaPaymentResult paymentResult = robokassaApi.ProcessSuccess(Request.Params);
            if (paymentResult == null) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PaymentController.Success. PaymentResult is null. Params={0}",
                                            HttpContextHelper.ParamsToString(Request.Params, RobokassaApi.IsValidParamName));
                return GetFailView();
            }

            var purchasedGoogsQuery = new PurchasedGoodsQuery();
            PurchasedGoods purchasedGoods = purchasedGoogsQuery.Get(paymentResult.PaymentId);
            if (purchasedGoods == null) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PaymentController.Success. GetUniqueDownloadId не вернул уникальный идентификатор скачивания. PaymentId={0}, Price={1}, Params={2}",
                                            paymentResult.PaymentId, paymentResult.Price,
                                            HttpContextHelper.ParamsToString(Request.Params, RobokassaApi.IsValidParamName));
                return GetFailView();
            }

            LoggerWrapper.RemoteMessage(LoggingType.Info,
                                        "PaymentController.Success. Перед тем как сообщить пользователю об успешном платеже на сумму {0} с идентификатором {1}",
                                        MoneyFormatter.ToRubles(paymentResult.Price), paymentResult.PaymentId);

            return GetSuccessView(purchasedGoods);
        }

        /// <summary>
        /// Метод вызывается платежной системой если платеж не удался
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult Fail() {
            RobokassaApi robokassaApi = ApiFactory.GetRobokassaApi(WebSettingsConfig.Instance);
            RobokassaPaymentResult paymentResult = robokassaApi.ProcessFail(Request.Params);
            if (paymentResult == null) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PaymentController.Fail. PaymentResult is null. Params={0}",
                                            HttpContextHelper.ParamsToString(Request.Params, RobokassaApi.IsValidParamName));
                return GetCancelView();
            }

            var purchasedGoogsQuery = new PurchasedGoodsQuery();
            bool isSuccess = purchasedGoogsQuery.FailedPurchased(paymentResult.PaymentId);
            if (!isSuccess) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PaymentController.Fail. FailedPurchased вернул false. PaymentId={0}, Price={1}, Params={2}",
                                            paymentResult.PaymentId, paymentResult.Price,
                                            HttpContextHelper.ParamsToString(Request.Params, RobokassaApi.IsValidParamName));
                return GetCancelView();
            }

            LoggerWrapper.RemoteMessage(LoggingType.Info,
                                        "PaymentController.Fail. Перед тем как сообщить пользователю об отменене платежа на сумму {0} с идентификатором {1}",
                                        MoneyFormatter.ToRubles(paymentResult.Price), paymentResult.PaymentId);

            return GetCancelView();
        }

        public ActionResult Check() {
            return View("../Sales/CheckPayment");
        }

        [UserId]
        public ActionResult CheckPurchasedGoods(long userId, string uniqueDownloadId) {
            if (IdGenerator.IsInvalidDownloadId(uniqueDownloadId)) {
                return JsonResultHelper.Error();
            }

            var purchasedGoogsQuery = new PurchasedGoodsQuery();
            PurchasedGoods purchasedGoods = purchasedGoogsQuery.GetPaid(uniqueDownloadId);

            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                "PaymentController.Check. Пользователь {0} проверял уникальный идентификатор покупки {1}. Статус: {2}!",
                userId, uniqueDownloadId, purchasedGoods != null ? "ОПЛАЧЕНО" : "НЕ оплачен");

            if (purchasedGoods == null) {
                return JsonResultHelper.Error();
            }

            string html = OurHtmlHelper.RenderRazorViewToString(ControllerContext, "PartialDownloadPurchasedGoods",
                                                                new SuccessPaymentModel(
                                                                    purchasedGoods.ShortDescription,
                                                                    purchasedGoods.UniqueDownloadId,
                                                                    (GoodsId)purchasedGoods.GoodsId));
            return JsonResultHelper.GetUnlimitedJsonResult(new {success = true, result = html});
        }

        private ViewResult GetSuccessView(PurchasedGoods purchasedGoods) {
            return View("../Sales/SuccessPayment",
                        new SuccessPaymentModel(purchasedGoods.ShortDescription, purchasedGoods.UniqueDownloadId, (GoodsId)purchasedGoods.GoodsId));
        }

        private ViewResult GetFailView() {
            var pageRequiredData = new PageRequiredData(SectionId.No) {
                Title = "Оплата не пройдена. Не удалось оплатить",
                Keywords = "Оплата не пройдена, неудачная оплата, непрошедший платеж",
                Description = "Оплата не пройдена. Не удалось оплатить"
            };
            var failPaymentModel = new FailPaymentModel(pageRequiredData, "Оплата не пройдена!");
            return View("../Sales/FailPayment", failPaymentModel);
        }

        private ViewResult GetCancelView() {
            var pageRequiredData = new PageRequiredData(SectionId.No) {
                Title = "Оплата отменена. Вы отменили оплату",
                Keywords = "Оплата отменена, отмена оплаты пользователем",
                Description = "Оплата отменена. Вы отменили оплату. Пользователь отменил оплату"
            };
            var failPaymentModel = new FailPaymentModel(pageRequiredData, "Вы отменили оплату!");
            return View("../Sales/FailPayment", failPaymentModel);
        }
    }
}