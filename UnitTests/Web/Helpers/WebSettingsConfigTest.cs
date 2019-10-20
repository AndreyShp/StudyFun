using BusinessLogic.DataQuery.Money;
using BusinessLogic.ExternalData;
using BusinessLogic.PaymentSystems;
using NUnit.Framework;
using StudyLanguages.Configs;

namespace UnitTests.Web.Helpers {
    [TestFixture]
    public class WebSettingsConfigTest {
        public WebSettingsConfigTest() {
            WebSettingsConfig.SetPath("../../");
        }

        [TestCase(SectionId.UserTasks, false)]
        [TestCase(SectionId.No, false)]
        [TestCase(SectionId.VisualDictionary, true)]
        public void CanBuy(SectionId sectionId, bool expectedResult) {
            bool canBuy = WebSettingsConfig.Instance.CanBuy(sectionId);

            Assert.That(canBuy, Is.EqualTo(expectedResult));
        }

        [TestCase(SectionId.Video)]
        [TestCase(SectionId.No)]
        public void GetSalesSettingsNull(SectionId sectionId) {
            ISalesSettings salesSettings = WebSettingsConfig.Instance.GetSalesSettings(sectionId);

            Assert.That(salesSettings, Is.Null);
        }

        [TestCase(SectionId.VisualDictionary, "", 1.85d)]
        [TestCase(SectionId.VisualDictionary, "Какое-то название словаря", 1.85d)]
        [TestCase(SectionId.VisualDictionary, "Автомобиль", 0d)]
        [TestCase(SectionId.VisualDictionary, "автомобиль", 0d)]
        [TestCase(SectionId.VisualDictionary, "АВТОМОБИЛЬ", 0d)]
        [TestCase(SectionId.VisualDictionary, "Пробный 1", 12d)]
        public void GetSalesSettingsByName(SectionId sectionId, string name, decimal expectedPrice) {
            ISalesSettings salesSettings = WebSettingsConfig.Instance.GetSalesSettings(sectionId);

            decimal price = salesSettings.GetPrice(0, name);

            Assert.That(price, Is.EqualTo(expectedPrice));
        }

        [TestCase(SectionId.VisualDictionary, 89, 123.45d)]
        [TestCase(SectionId.VisualDictionary, 34, 234.09d)]
        [TestCase(SectionId.VisualDictionary, 877, -19.10d)]
        public void GetSalesSettingsById(SectionId sectionId, long id, decimal expectedPrice) {
            ISalesSettings salesSettings = WebSettingsConfig.Instance.GetSalesSettings(sectionId);

            decimal price = salesSettings.GetPrice(id, string.Empty);

            Assert.That(price, Is.EqualTo(expectedPrice));
        }

        [Test]
        public void Discount() {
            ISalesSettings salesSettings = WebSettingsConfig.Instance.GetSalesSettings(SectionId.VisualDictionary);

            Assert.That(salesSettings.Discount, Is.EqualTo(0.25));
        }

        [Test]
        public void PathToTopBanner() {
            string path = WebSettingsConfig.Instance.PathToTopBanner;

            Assert.That(path, Is.EqualTo("TestPath/Content/images/Banner_top_test.gif"));
        }

        [Test]
        public void RobokassaSecurityParams() {
            RobokassaSecurityParams securityParams = WebSettingsConfig.Instance.RobokassaSecurityParams;

            Assert.That(securityParams, Is.Not.Null);
            Assert.That(securityParams.Login, Is.EqualTo("login.to.robokassa"));
            Assert.That(securityParams.Password1, Is.EqualTo("PassW0rd1"));
            Assert.That(securityParams.Password2, Is.EqualTo("Pa$$W0rd2"));
        }
    }
}