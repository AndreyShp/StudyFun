using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using NUnit.Framework;

namespace UnitTests.BusinessLogic.Helpers {
    [TestFixture]
    public class LanguagesHelperTest {
        [TestCase(LanguageShortName.Es, "испанский")]
        [TestCase(LanguageShortName.De, "немецкий")]
        [TestCase(LanguageShortName.En, "английский")]
        [TestCase(LanguageShortName.It, "итальянский")]
        [TestCase(LanguageShortName.Fr, "французский")]
        [TestCase(LanguageShortName.Ru, "русский")]
        [TestCase(LanguageShortName.Pl, null)]
        public void GetPrettyLowerName(LanguageShortName shortName, string expectedResult) {
            string prettyLowerName = LanguagesHelper.GetPrettyLowerName(shortName);

            Assert.That(prettyLowerName, Is.EqualTo(expectedResult));
        }

        [TestCase(LanguageShortName.Es, "испанск")]
        [TestCase(LanguageShortName.De, "немецк")]
        [TestCase(LanguageShortName.En, "английск")]
        [TestCase(LanguageShortName.It, "итальянск")]
        [TestCase(LanguageShortName.Fr, "французск")]
        [TestCase(LanguageShortName.Ru, "русск")]
        [TestCase(LanguageShortName.Pl, null)]
        public void GetLowerNameWithoutEnding(LanguageShortName shortName, string expectedResult) {
            string lowerNameWithoutEnding = LanguagesHelper.GetLowerNameWithoutEnding(shortName);

            Assert.That(lowerNameWithoutEnding, Is.EqualTo(expectedResult));
        }
    }
}