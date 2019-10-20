using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;
using BusinessLogic.Helpers;
using BusinessLogic.Validators;

namespace BusinessLogic.PaymentSystems {
    /// <summary>
    /// Обертка для работы с платежной системой Robokassa
    /// </summary>
    public class RobokassaApi {
        /// <summary>
        /// Формат цены
        /// </summary>
        private const string PRICE_FORMAT = "0.00";

        private readonly RobokassaSecurityParams _securityParams;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="securityParams">пар-ры безопасности</param>
        public RobokassaApi(RobokassaSecurityParams securityParams) {
            _securityParams = securityParams;
        }

        /// <summary>
        /// Возвращает урл для передачи robokassa'е
        /// </summary>
        /// <param name="paymentId">номер счета в магазине (должен быть уникальным для магазина). Может принимать значения от 1 до 2147483647 (2^31-1)</param>
        /// <param name="price">цена товара</param>
        /// <param name="description">описание товара</param>
        /// <returns></returns>
        public string GetPaymentUrl(long paymentId, decimal price, string description) {
            string sLogin = _securityParams.Login;
            string sPass = _securityParams.Password1;
            string sPrice = price.ToString(PRICE_FORMAT, CultureInfo.InvariantCulture);

            string crc = GetCrc(string.Format("{0}:{1}:{2}:{3}", sLogin, sPrice, paymentId, sPass));
            string result = "https://auth.robokassa.ru/Merchant/Index.aspx?"
                            + ParamNames.LOGIN + "=" + HttpUtility.UrlPathEncode(sLogin)
                            + "&" + ParamNames.SUMM + "=" + sPrice
                            + "&" + ParamNames.PAYMENT_ID + "=" + paymentId
                            + "&" + ParamNames.DESCRIPTION + "=" + HttpUtility.UrlPathEncode(description)
                            + "&" + ParamNames.SIGNATURE_VALUE + "=" + crc;
            return result;
        }

        public RobokassaPaymentResult ProcessResult(NameValueCollection pars) {
            return GetPaymentResult(pars, _securityParams.Password2);
        }

        public string GetResponseResultOk(int paymentId) {
            return string.Format("OK{0}", paymentId);
        }

        public RobokassaPaymentResult ProcessSuccess(NameValueCollection pars) {
            return GetPaymentResult(pars, _securityParams.Password1);
        }

        private static RobokassaPaymentResult GetPaymentResult(NameValueCollection pars, string password) {
            RobokassaPaymentResult result = GetPaymentResult(pars);
            if (result == null) {
                return null;
            }
            string sCrc = pars[ParamNames.SIGNATURE_VALUE] ?? string.Empty;
            string sMyCrc = GetCrc(string.Format("{0}:{1}:{2}", result.DirtyPrice, result.PaymentId, password));
            if (!sMyCrc.Equals(sCrc, StringComparison.InvariantCultureIgnoreCase)) {
                return null;
            }
            return result;
        }

        public RobokassaPaymentResult ProcessFail(NameValueCollection pars) {
            return GetPaymentResult(pars);
        }

        private static RobokassaPaymentResult GetPaymentResult(NameValueCollection pars) {
            string dirtyPaymentId = pars[ParamNames.PAYMENT_ID];
            string dirtyPrice = pars[ParamNames.SUMM];
            int paymentId;
            decimal price;

            if (string.IsNullOrEmpty(dirtyPaymentId) || string.IsNullOrEmpty(dirtyPrice)
                || !int.TryParse(dirtyPaymentId, out paymentId)
                || !decimal.TryParse(dirtyPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out price)
                || IdValidator.IsInvalid(paymentId) || price <= 0) {
                return null;
            }

            return new RobokassaPaymentResult(paymentId, price, dirtyPrice);
        }

        private static string GetCrc(string sCrcBase) {
            var md5Helper = new Md5Helper(Encoding.ASCII);
            return md5Helper.GetHash(sCrcBase, 1);
        }

        public static bool IsValidParamName(string name) {
            return ParamNames.LOGIN.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                   || ParamNames.SUMM.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                   || ParamNames.PAYMENT_ID.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                   || ParamNames.DESCRIPTION.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                   || ParamNames.SIGNATURE_VALUE.Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        #region Nested type: ParamNames

        private static class ParamNames {
            public const string LOGIN = "MrchLogin";
            public const string SUMM = "OutSum";
            public const string PAYMENT_ID = "InvId";
            public const string DESCRIPTION = "Desc";
            public const string SIGNATURE_VALUE = "SignatureValue";
        }

        #endregion

        //TODO: добавить метод для получения суммы с учетом комиссии
    }
}