using System.Collections.Specialized;
using BusinessLogic.PaymentSystems;
using NUnit.Framework;

namespace UnitTests.BusinessLogic.PaymentSystems {
    [TestFixture]
    public class RobokassaApiTest {
        private const string LOGIN = "TestLoginRobokassa";
        private const string PASSWORD1 = "TestPa$$w0rdRobokassa1";
        private const string PASSWORD2 = "TestPa$$w0rdRobokassa2";

        [TestCase(0)]
        [TestCase(int.MaxValue)]
        [TestCase(-947)]
        public void GetResponseResultOk(int name) {
            var robokassaApi = new RobokassaApi(null);
            string responseResult = robokassaApi.GetResponseResultOk(name);

            Assert.That(responseResult, Is.EqualTo("OK" + name));
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("OutSumm", false)]
        [TestCase(" OutSum", false)]
        [TestCase("OutSum ", false)]
        [TestCase("asdklaskd", false)]
        [TestCase("MrchLogin", true)]
        [TestCase("mrchlogin", true)]
        [TestCase("OutSum", true)]
        [TestCase("OutSUM", true)]
        [TestCase("InvId", true)]
        [TestCase("INVId", true)]
        [TestCase("Desc", true)]
        [TestCase("desc", true)]
        [TestCase("SignatureValue", true)]
        [TestCase("SIGNATUREVALUE", true)]
        public void IsValidParamName(string name, bool expectedResult) {
            bool isValid = RobokassaApi.IsValidParamName(name);
            Assert.That(isValid, Is.EqualTo(expectedResult));
        }

        [TestCase(PASSWORD1 + "$", 1341, 0.1d, "some test description",
            "https://auth.robokassa.ru/Merchant/Index.aspx?MrchLogin=TestLoginRobokassa&OutSum=0.10&InvId=1341&Desc=some%20test%20description&SignatureValue=f95af72d9b74201101852a055e325f18"
            )]
        [TestCase(PASSWORD1, 1341, 0.1d, "some test description",
            "https://auth.robokassa.ru/Merchant/Index.aspx?MrchLogin=TestLoginRobokassa&OutSum=0.10&InvId=1341&Desc=some%20test%20description&SignatureValue=c87e8ad87007cf087ab54d2e4e31fb1b"
            )]
        [TestCase(PASSWORD1, 34525, 123.45d, "привет",
            "https://auth.robokassa.ru/Merchant/Index.aspx?MrchLogin=TestLoginRobokassa&OutSum=123.45&InvId=34525&Desc=%d0%bf%d1%80%d0%b8%d0%b2%d0%b5%d1%82&SignatureValue=eb122994d31f257dd6a3d9ae5ba339c7"
            )]
        public void GetPaymentUrl(string password1, int paymentId, decimal price, string description, string expectedUrl) {
            var securityParams = new RobokassaSecurityParams(LOGIN, password1, PASSWORD2);
            var robokassaApi = new RobokassaApi(securityParams);

            string url = robokassaApi.GetPaymentUrl(paymentId, price, description);

            Assert.That(url, Is.EqualTo(expectedUrl));
        }

        [TestCase(PASSWORD2, "0", "123.45", "f590351e6a58d86c545fa647ce6a50ca",
            Description = "Некорректный идентификатор")]
        [TestCase(PASSWORD2, "-1", "123.45", "f590351e6a58d86c545fa647ce6a50ca",
            Description = "Некорректный идентификатор")]
        [TestCase(PASSWORD2, "98", "0", "f590351e6a58d86c545fa647ce6a50ca", Description = "Некорректная цена")]
        [TestCase(PASSWORD2, "98", "-1", "f590351e6a58d86c545fa647ce6a50ca", Description = "Некорректная цена")]
        [TestCase(PASSWORD2, "98", "123.45", "f590351e6a58d86c545fa647ce6a50cс", Description = "Некорректный crc")]
        public void ProcessResultNull(string password2, string paymentId, string price, string crc) {
            var securityParams = new RobokassaSecurityParams(LOGIN, PASSWORD1, password2);
            var robokassaApi = new RobokassaApi(securityParams);
            NameValueCollection pars = CreateNameValueCollection(paymentId, price, crc);

            RobokassaPaymentResult robokassaPaymentResult = robokassaApi.ProcessResult(pars);

            Assert.That(robokassaPaymentResult, Is.Null);
        }

        [TestCase("ajkasjkdajk", "8", "0.01", "a18ae259fd4d61ac8831d59898d9abcf", 8, 0.01d)]
        [TestCase(PASSWORD2, "98", "123.45", "f590351e6a58d86c545fa647ce6a50ca", 98, 123.45d)]
        [TestCase(PASSWORD2 + " ", "98", "123.45", "091e2a3d67eaa3ba805e87e41522c059", 98, 123.45d)]
        public void ProcessResult(string password2,
                                  string paymentId,
                                  string price,
                                  string crc,
                                  int expectedPaymentId,
                                  decimal expectedPrice) {
            var securityParams = new RobokassaSecurityParams(LOGIN, PASSWORD1, password2);
            var robokassaApi = new RobokassaApi(securityParams);
            NameValueCollection pars = CreateNameValueCollection(paymentId, price, crc);

            RobokassaPaymentResult robokassaPaymentResult = robokassaApi.ProcessResult(pars);

            Assert.That(robokassaPaymentResult, Is.Not.Null);
            Assert.That(robokassaPaymentResult.PaymentId, Is.EqualTo(expectedPaymentId));
            Assert.That(robokassaPaymentResult.Price, Is.EqualTo(expectedPrice));
        }

        private static NameValueCollection CreateNameValueCollection(string paymentId, string price, string crc) {
            var pars = new NameValueCollection {
                {"OutSum", price},
                {"InvId", paymentId},
                {"SignatureValue", crc != null ? crc.ToUpper() : null},
            };
            return pars;
        }

        [TestCase(PASSWORD2, "0", "123.45", "f590351e6a58d86c545fa647ce6a50ca",
            Description = "Некорректный идентификатор")]
        [TestCase(PASSWORD2, "-1", "123.45", "f590351e6a58d86c545fa647ce6a50ca",
            Description = "Некорректный идентификатор")]
        [TestCase(PASSWORD2, "98", "0", "f590351e6a58d86c545fa647ce6a50ca", Description = "Некорректная цена")]
        [TestCase(PASSWORD2, "98", "-1", "f590351e6a58d86c545fa647ce6a50ca", Description = "Некорректная цена")]
        [TestCase(PASSWORD2, "98", "123.45", "f590351e6a58d86c545fa647ce6a50cс", Description = "Некорректный crc")]
        public void ProcessSuccessNull(string password1, string paymentId, string price, string crc) {
            var securityParams = new RobokassaSecurityParams(LOGIN, password1, PASSWORD2);
            var robokassaApi = new RobokassaApi(securityParams);
            NameValueCollection pars = CreateNameValueCollection(paymentId, price, crc);

            RobokassaPaymentResult robokassaPaymentResult = robokassaApi.ProcessSuccess(pars);

            Assert.That(robokassaPaymentResult, Is.Null);
        }

        [TestCase("ajkasjkdajk", "8", "0.01", "a18ae259fd4d61ac8831d59898d9abcf", 8, 0.01d)]
        [TestCase(PASSWORD2, "98", "123.45", "f590351e6a58d86c545fa647ce6a50ca", 98, 123.45d)]
        [TestCase(PASSWORD2 + " ", "98", "123.45", "091e2a3d67eaa3ba805e87e41522c059", 98, 123.45d)]
        public void ProcessSuccess(string password1,
                                   string paymentId,
                                   string price,
                                   string crc,
                                   int expectedPaymentId,
                                   decimal expectedPrice) {
            var securityParams = new RobokassaSecurityParams(LOGIN, password1, PASSWORD2);
            var robokassaApi = new RobokassaApi(securityParams);
            NameValueCollection pars = CreateNameValueCollection(paymentId, price, crc);

            RobokassaPaymentResult robokassaPaymentResult = robokassaApi.ProcessSuccess(pars);

            Assert.That(robokassaPaymentResult, Is.Not.Null);
            Assert.That(robokassaPaymentResult.PaymentId, Is.EqualTo(expectedPaymentId));
            Assert.That(robokassaPaymentResult.Price, Is.EqualTo(expectedPrice));
        }

        [TestCase("0", "123.45", Description = "Некорректный идентификатор")]
        [TestCase("-1", "123.45", Description = "Некорректный идентификатор")]
        [TestCase("98", "0", Description = "Некорректная цена")]
        [TestCase("98", "-1", Description = "Некорректная цена")]
        public void ProcessFailNull(string paymentId, string price) {
            var securityParams = new RobokassaSecurityParams(LOGIN, PASSWORD1, PASSWORD2);
            var robokassaApi = new RobokassaApi(securityParams);
            NameValueCollection pars = CreateNameValueCollection(paymentId, price, null);

            RobokassaPaymentResult robokassaPaymentResult = robokassaApi.ProcessFail(pars);

            Assert.That(robokassaPaymentResult, Is.Null);
        }

        [TestCase("8", "0.01", 8, 0.01d)]
        [TestCase("98", "123.45", 98, 123.45d)]
        [TestCase("7", "1.850000", 7, 1.85d, Description = "Пример реального урла http://studyfun.ru/FailPayment?inv_id=7&InvId=7&out_summ=1.850000&OutSum=1.850000&Culture=ru")]
        public void ProcessFail(string paymentId,
                                string price,
                                int expectedPaymentId,
                                decimal expectedPrice) {
            var securityParams = new RobokassaSecurityParams(LOGIN, PASSWORD1, PASSWORD2);
            var robokassaApi = new RobokassaApi(securityParams);
            NameValueCollection pars = CreateNameValueCollection(paymentId, price, null);

            RobokassaPaymentResult robokassaPaymentResult = robokassaApi.ProcessFail(pars);

            Assert.That(robokassaPaymentResult, Is.Not.Null);
            Assert.That(robokassaPaymentResult.PaymentId, Is.EqualTo(expectedPaymentId));
            Assert.That(robokassaPaymentResult.Price, Is.EqualTo(expectedPrice));
        }
    }
}