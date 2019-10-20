using System.Collections;
using BusinessLogic;
using BusinessLogic.Helpers;
using NUnit.Framework;

namespace UnitTests.BusinessLogic {
    [TestFixture]
    public class UserUniqueIdTest {
        private static IEnumerable ValidValues {
            get {
                var validHash = IdGenerator.GenerateByLength(UserUniqueId.USER_HASH_LEN);
                var invalidHash1 = " " + validHash.Substring(1);
                var invalidHash2 = validHash.Substring(0, validHash.Length - 1) + " ";
                var invalidHash3 = "Я" + validHash.Substring(1);
                var invalidHash4 = validHash.Substring(0, validHash.Length - 1) + "Я";
                var invalidHash5 = validHash.Substring(0, validHash.Length - 1) + "_";
                yield return new TestCaseData(invalidHash1, false);
                yield return new TestCaseData(invalidHash2, false);
                yield return new TestCaseData(invalidHash3, false);
                yield return new TestCaseData(invalidHash4, false);
                yield return new TestCaseData(invalidHash5, false);
                yield return new TestCaseData(validHash + "1", false);
                yield return new TestCaseData(validHash, true);
            }
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("123asdfbxcxd3444", false)]
        [TestCaseSource("ValidValues")]
        public void IsValid(string hash, bool expectedResult) {
            var userUniqueId = new UserUniqueId();
            bool result = userUniqueId.IsValid(hash);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void CheckHashLen() {
            var userUniqueId = new UserUniqueId();
            string hash = userUniqueId.New();

            Assert.That(hash.Length, Is.EqualTo(UserUniqueId.USER_HASH_LEN));
        }

        [Test]
        public void CheckUniqueHashes() {
            var userUniqueId = new UserUniqueId();
            string hash1 = userUniqueId.New();
            string hash2 = userUniqueId.New();

            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void New() {
            var userUniqueId = new UserUniqueId();
            string hash = userUniqueId.New();

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash, Is.Not.EqualTo(string.Empty));
        }
    }
}