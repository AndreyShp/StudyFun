using BusinessLogic;
using NUnit.Framework;

namespace UnitTests.BusinessLogic {
    [TestFixture]
    public class SingletonTest {
        private class TestSingleton : Singleton<TestSingleton> {}

        private class TestConfigurableSingleton : Singleton<TestConfigurableSingleton>, IConfigurable {
            internal int ConfigureCount { get; set; }

            #region IConfigurable Members

            public void Configure() {
                ConfigureCount++;
            }

            #endregion
        }

        [Test]
        public void ConfigurableInstance() {
            TestConfigurableSingleton first = TestConfigurableSingleton.Instance;
            TestConfigurableSingleton second = TestConfigurableSingleton.Instance;

            Assert.That(first, Is.EqualTo(second));
            Assert.That(TestConfigurableSingleton.Instance.ConfigureCount, Is.EqualTo(1));
        }

        [Test]
        public void Instance() {
            TestSingleton first = TestSingleton.Instance;
            TestSingleton second = TestSingleton.Instance;

            Assert.That(first, Is.EqualTo(second));
        }
    }
}