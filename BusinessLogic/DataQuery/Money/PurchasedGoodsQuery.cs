using System;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums.Money;
using BusinessLogic.Data.Money;
using BusinessLogic.Extensions;
using BusinessLogic.ExternalData.Sales;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using Newtonsoft.Json;

namespace BusinessLogic.DataQuery.Money {
    public class PurchasedGoodsQuery : BaseQuery {
        /// <summary>
        /// Добавляет купленный товар и создает платеж
        /// </summary>
        /// <param name="purchasedGoodsForUser">данные о купленном товаре</param>
        /// <returns>true - купленный товар и платеж успешно добавлены, false - купленный товар и платеж не удалось добавить</returns>
        public long WantToBuyGoods<T>(PurchasedGoodsForUser<T> purchasedGoodsForUser) {
            if (purchasedGoodsForUser == null || purchasedGoodsForUser.Price <= 0
                || EnumValidator.IsInvalid(purchasedGoodsForUser.GoodsId)) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PurchasedGoodsQuery.WantToBuyGoods. Не удалось сохранить платежку. {0}",
                                            purchasedGoodsForUser != null
                                                ? "Некорректная цена: " + purchasedGoodsForUser.Price
                                                : "purchasedGoodsForUser == null");
                return IdValidator.INVALID_ID;
            }

            string serializedGoods;
            try {
                serializedGoods = JsonConvert.SerializeObject(purchasedGoodsForUser.Goods);
            } catch (Exception e) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PurchasedGoodsQuery.WantToBuyGoods. Товар с уникальным идентификатором {0} не удалось сериализовать из типа {1}. Исключение: {2}",
                                            purchasedGoodsForUser.UniqueDownloadId, typeof (T).FullName, e);
                return IdValidator.INVALID_ID;
            }

            long result = IdValidator.INVALID_ID;
            Adapter.Transaction(c => {
                var payment = new Payment {
                    Price = purchasedGoodsForUser.Price,
                    Status = PaymentStatus.InProcess,
                    Description = purchasedGoodsForUser.FullDescription,
                    UserId = purchasedGoodsForUser.UserId,
                    CreationDate = DateTime.Now,
                    PaymentDate = new DateTime().GetDbDateTime(),
                    System = purchasedGoodsForUser.PaymentSystem
                };
                c.Payment.Add(payment);
                c.SaveChanges();

                if (IdValidator.IsInvalid(payment.Id)) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.WantToBuyGoods. Не удалось сохранить платежку. UniqueDownloadId={0}, UserId={1}, Price={2}, Description={3}",
                                                purchasedGoodsForUser.UniqueDownloadId, purchasedGoodsForUser.UserId,
                                                purchasedGoodsForUser.Price, purchasedGoodsForUser.FullDescription);
                    return false;
                }

                var purchasedGoods = new PurchasedGoods {
                    UserId = purchasedGoodsForUser.UserId,
                    Price = purchasedGoodsForUser.Price,
                    Goods = serializedGoods,
                    LanguageId = purchasedGoodsForUser.LanguageId,
                    UniqueDownloadId = purchasedGoodsForUser.UniqueDownloadId,
                    FullDescription = purchasedGoodsForUser.FullDescription,
                    ShortDescription = purchasedGoodsForUser.ShortDescription,
                    PaymentId = payment.Id,
                    PurchaseDate = DateTime.Now,
                    PostToCustomerDate = new DateTime().GetDbDateTime(),
                    Status = PurchasedStatus.WaitPayment,
                    GoodsId = (byte) purchasedGoodsForUser.GoodsId
                };
                c.PurchasedGoods.Add(purchasedGoods);
                c.SaveChanges();

                bool innerResult = IdValidator.IsValid(purchasedGoods.Id);
                if (innerResult) {
                    result = payment.Id;
                } else {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.WantToBuyGoods. Не удалось сохранить купленные товары. PaymentId={0}, UniqueDownloadId={1}, UserId={2}, Price={3}, Description={4}, LanguageId={5}",
                                                payment.Id, purchasedGoodsForUser.UniqueDownloadId,
                                                purchasedGoodsForUser.UserId,
                                                purchasedGoodsForUser.Price, purchasedGoodsForUser.FullDescription,
                                                purchasedGoodsForUser.LanguageId);
                }
                return innerResult;
            });
            return result;
        }

        public bool SuccessfullyPurchased(long paymentId, decimal paidPrice) {
            return FinishPayment(paymentId, PaymentStatus.Success, payment => {
                bool result = payment.Price == paidPrice;
                if (!result) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.SuccessfullyPurchased. Платежка с идентификатором {0} оплачена меньше, чем ожидалось!!! Ожидалась цена {1}, пришла цена {2}!!!",
                                                paymentId, payment.Price, paidPrice);
                }
                return result;
            });
        }

        public bool FailedPurchased(long paymentId) {
            return FinishPayment(paymentId, PaymentStatus.Fail, null);
        }

        public PurchasedGoods Get(long paymentId) {
            return Adapter.ReadByContext(c => GetPurchasedGoods(c, paymentId));
        }

        public PurchasedGoods GetPaid(string uniqueDownloadId) {
            return Adapter.ReadByContext(c => {
                PurchasedGoods purchasedGoods = GetPurchasedGoods(uniqueDownloadId, c);
                return purchasedGoods != null && IsPaidStatus(purchasedGoods) ? purchasedGoods : null;
            });
        }

        private static bool IsPaidStatus(PurchasedGoods purchasedGoods) {
            return purchasedGoods.Status == PurchasedStatus.Success
                   || purchasedGoods.Status == PurchasedStatus.PostToCustomer;
        }

        private static PurchasedGoods GetPurchasedGoods(string uniqueDownloadId, StudyLanguageContext c) {
            return c.PurchasedGoods.FirstOrDefault(e => e.UniqueDownloadId == uniqueDownloadId);
        }

        private bool FinishPayment(long paymentId, PaymentStatus paymentStatus, Func<Payment, bool> additionalChecker) {
            return Adapter.Transaction(c => {
                Payment payment = c.Payment.FirstOrDefault(e => e.Id == paymentId);
                PurchasedGoods purchasedGoods = GetPurchasedGoods(c, paymentId);

                if (payment == null || purchasedGoods == null) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. Не удалось получить платежку или купленный товар. PaymentId={0}, payment={1}, purchasedGoods={2}, paymentStatus={3}",
                                                paymentId, payment == null ? "NULL" : "NOT NULL",
                                                purchasedGoods == null ? "NULL" : "NOT NULL",
                                                paymentStatus);
                    return false;
                }

                if (payment.Status != PaymentStatus.InProcess) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. Платежка с идентификатором {0} похоже пришла повторно! Статус у платежки в БД {1}, paymentStatus={2}",
                                                paymentId, payment.Status, paymentStatus);
                }

                if (purchasedGoods.Status == PurchasedStatus.PostToCustomer) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. У платежки с идентификатором {0} статус у платежки в БД {1}, paymentStatus={2}. Вернем true",
                                                paymentId, payment.Status, paymentStatus);
                    return true;
                }

                if (additionalChecker != null && !additionalChecker(payment)) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. Для платежки с идентификатором {0} additionalChecker вернул false",
                                                paymentId, payment.Status, paymentStatus);
                    return false;
                }

                payment.PaymentDate = DateTime.Now;
                payment.Status = paymentStatus;

                purchasedGoods.Status = paymentStatus == PaymentStatus.Success
                                            ? PurchasedStatus.Success
                                            : PurchasedStatus.Fail;

                c.SaveChanges();

                return true;
            });
        }

        private static PurchasedGoods GetPurchasedGoods(StudyLanguageContext context, long paymentId) {
            return context.PurchasedGoods.FirstOrDefault(e => e.PaymentId == paymentId);
        }

        public T GetJsonGoods<T>(string uniqueDownloadId) where T : class {
            T result = null;

            Adapter.Transaction(c => {
                PurchasedGoods purchasedGoods = GetPurchasedGoods(uniqueDownloadId, c);

                if (purchasedGoods == null) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.GetGoods. Для уникального идентификатора {0} не удалось найти купленный товар!!!",
                                                uniqueDownloadId);
                    return false;
                }

                if (!IsPaidStatus(purchasedGoods)) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.GetGoods. Товар с уникальным идентификатором {0} пытаются скачать не оплатив!!! Статус купленного товара {1}",
                                                uniqueDownloadId, purchasedGoods.Status);
                    return false;
                }

                try {
                    result = JsonConvert.DeserializeObject<T>(purchasedGoods.Goods);
                } catch (Exception e) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.GetGoods. Товар с уникальным идентификатором {0} не удалось десериализовать в тип {1}. JSON-описание товара: {2}. Исключение: {3}",
                                                uniqueDownloadId, typeof (T).FullName, purchasedGoods.Goods, e);
                    return false;
                }
                purchasedGoods.PostToCustomerDate = DateTime.Now;
                purchasedGoods.Status = PurchasedStatus.PostToCustomer;
                c.SaveChanges();
                return true;
            });
            return result;
        }
    }
}