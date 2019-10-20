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
        /// ��������� ��������� ����� � ������� ������
        /// </summary>
        /// <param name="purchasedGoodsForUser">������ � ��������� ������</param>
        /// <returns>true - ��������� ����� � ������ ������� ���������, false - ��������� ����� � ������ �� ������� ��������</returns>
        public long WantToBuyGoods<T>(PurchasedGoodsForUser<T> purchasedGoodsForUser) {
            if (purchasedGoodsForUser == null || purchasedGoodsForUser.Price <= 0
                || EnumValidator.IsInvalid(purchasedGoodsForUser.GoodsId)) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PurchasedGoodsQuery.WantToBuyGoods. �� ������� ��������� ��������. {0}",
                                            purchasedGoodsForUser != null
                                                ? "������������ ����: " + purchasedGoodsForUser.Price
                                                : "purchasedGoodsForUser == null");
                return IdValidator.INVALID_ID;
            }

            string serializedGoods;
            try {
                serializedGoods = JsonConvert.SerializeObject(purchasedGoodsForUser.Goods);
            } catch (Exception e) {
                LoggerWrapper.RemoteMessage(LoggingType.Error,
                                            "PurchasedGoodsQuery.WantToBuyGoods. ����� � ���������� ��������������� {0} �� ������� ������������� �� ���� {1}. ����������: {2}",
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
                                                "PurchasedGoodsQuery.WantToBuyGoods. �� ������� ��������� ��������. UniqueDownloadId={0}, UserId={1}, Price={2}, Description={3}",
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
                                                "PurchasedGoodsQuery.WantToBuyGoods. �� ������� ��������� ��������� ������. PaymentId={0}, UniqueDownloadId={1}, UserId={2}, Price={3}, Description={4}, LanguageId={5}",
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
                                                "PurchasedGoodsQuery.SuccessfullyPurchased. �������� � ��������������� {0} �������� ������, ��� ���������!!! ��������� ���� {1}, ������ ���� {2}!!!",
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
                                                "PurchasedGoodsQuery.FinishPayment. �� ������� �������� �������� ��� ��������� �����. PaymentId={0}, payment={1}, purchasedGoods={2}, paymentStatus={3}",
                                                paymentId, payment == null ? "NULL" : "NOT NULL",
                                                purchasedGoods == null ? "NULL" : "NOT NULL",
                                                paymentStatus);
                    return false;
                }

                if (payment.Status != PaymentStatus.InProcess) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. �������� � ��������������� {0} ������ ������ ��������! ������ � �������� � �� {1}, paymentStatus={2}",
                                                paymentId, payment.Status, paymentStatus);
                }

                if (purchasedGoods.Status == PurchasedStatus.PostToCustomer) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. � �������� � ��������������� {0} ������ � �������� � �� {1}, paymentStatus={2}. ������ true",
                                                paymentId, payment.Status, paymentStatus);
                    return true;
                }

                if (additionalChecker != null && !additionalChecker(payment)) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.FinishPayment. ��� �������� � ��������������� {0} additionalChecker ������ false",
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
                                                "PurchasedGoodsQuery.GetGoods. ��� ����������� �������������� {0} �� ������� ����� ��������� �����!!!",
                                                uniqueDownloadId);
                    return false;
                }

                if (!IsPaidStatus(purchasedGoods)) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.GetGoods. ����� � ���������� ��������������� {0} �������� ������� �� �������!!! ������ ���������� ������ {1}",
                                                uniqueDownloadId, purchasedGoods.Status);
                    return false;
                }

                try {
                    result = JsonConvert.DeserializeObject<T>(purchasedGoods.Goods);
                } catch (Exception e) {
                    LoggerWrapper.RemoteMessage(LoggingType.Error,
                                                "PurchasedGoodsQuery.GetGoods. ����� � ���������� ��������������� {0} �� ������� ��������������� � ��� {1}. JSON-�������� ������: {2}. ����������: {3}",
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