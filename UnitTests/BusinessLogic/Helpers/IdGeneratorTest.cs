using BusinessLogic.Helpers;
using NUnit.Framework;

namespace UnitTests.BusinessLogic.Helpers {
    [TestFixture]
    public class IdGeneratorTest {
        [TestCase(null, 1, false)]
        [TestCase("", 1, false)]
        [TestCase("1", 2, false)]
        [TestCase("1", 0, false)]
        [TestCase("A", 1, false, Description = "Заглавные буквы не разрешены")]
        [TestCase("ф", 1, false, Description = "Кириллица не разрешена")]
        [TestCase("я", 1, false, Description = "Кириллица не разрешена")]
        [TestCase("а", 1, false, Description = "Кириллица не разрешена")]
        [TestCase("_", 1, false, Description = "Подчеркивание не разрешено")]
        [TestCase("$", 1, false, Description = "Бакс не разрешен")]
        [TestCase("g", 1, false, Description = "g не разрешено")]
        [TestCase("f", 1, true)]
        [TestCase("1", 1, true)]
        [TestCase("9", 1, true)]
        [TestCase("f12a", 3, false)]
        [TestCase("f12a", 5, false)]
        [TestCase("f12a", 4, true)]
        [TestCase("abcdef0123456789", 16, true)]
        [TestCase("abcdeF0123456789", 16, false)]
        public void IsValid(string hash, int expectedLength, bool expectedResult) {
            bool result = IdGenerator.IsValid(hash, expectedLength);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void GenerateByLength(int length) {
            string hash = IdGenerator.GenerateByLength(length);

            Assert.That(hash.Length, Is.EqualTo(length));
        }

        [Test]
        public void CheckChars() {
            var idGenerator = new IdGenerator();
            string hash = idGenerator.GenerateByCount(1);

            foreach (char ch in hash) {
                Assert.That((ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f'), Is.True, "Попался символ " + ch);
            }
        }

        [Test]
        public void CheckHashLen() {
            var idGenerator = new IdGenerator();
            string hash = idGenerator.GenerateByCount(3);

            Assert.That(hash.Length, Is.EqualTo(IdGenerator.DEFAULT_LEN));
        }

        [Test]
        public void CheckUniqueHashes() {
            var idGenerator = new IdGenerator();
            string hash1 = idGenerator.GenerateByCount(1);
            string hash2 = idGenerator.GenerateByCount(1);

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void IsInvalidDownloadId() {
            var idGenerator = new IdGenerator();
            string hash = idGenerator.GenerateByCount(3);

            Assert.That(hash.Length, Is.EqualTo(IdGenerator.DEFAULT_LEN));
        }

        [Test]
        public void New() {
            var idGenerator = new IdGenerator();
            string hash = idGenerator.GenerateByCount(1);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash, Is.Not.EqualTo(string.Empty));
        }
    }
}