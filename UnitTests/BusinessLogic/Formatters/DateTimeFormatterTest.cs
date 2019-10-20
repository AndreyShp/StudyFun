using System;
using BusinessLogic.Formatters;
using NUnit.Framework;

namespace UnitTests.BusinessLogic.Formatters {
    [TestFixture]
    public class DateTimeFormatterTest {
        private static void AssertToDDMMYYYY_HHMMSS(DateTime dateTime, string expectedResult) {
            string result = DateTimeFormatter.ToDDMMYYYY_HHMMSS(dateTime);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void GetNewUrl_GroupSentences() {
            AssertToDDMMYYYY_HHMMSS(new DateTime(2010, 1, 1), "01.01.2010 00:00:00");
            AssertToDDMMYYYY_HHMMSS(new DateTime(2012, 2, 29, 23, 59, 59), "29.02.2012 23:59:59");
            AssertToDDMMYYYY_HHMMSS(new DateTime(2014, 4, 6, 4, 5, 6), "06.04.2014 04:05:06");
        }
    }
}