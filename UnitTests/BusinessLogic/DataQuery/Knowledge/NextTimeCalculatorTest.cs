using System;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Knowledge;
using NUnit.Framework;

namespace UnitTests.BusinessLogic.DataQuery.Knowledge {
    [TestFixture]
    public class NextTimeCalculatorTest {
        private readonly NextTimeCalculator _nextTimeCalculator = new NextTimeCalculator();

        private void AssertCalculate(KnowledgeMark mark, double ratio, TimeSpan minTime, TimeSpan maxTime) {
            TimeSpan result = _nextTimeCalculator.Calculate(mark, ratio);
            Assert.That(result, Is.GreaterThanOrEqualTo(minTime));
            Assert.That(result, Is.LessThanOrEqualTo(maxTime));
        }

        private void AssertCalculate(KnowledgeMark mark,
                                     TimeSpan minTimeLower,
                                     TimeSpan minTimeUpper,
                                     TimeSpan maxTimeLower,
                                     TimeSpan maxTimeUpper) {
            AssertCalculate(mark, 0, minTimeLower, minTimeUpper);
            AssertCalculate(mark, 0.5, minTimeLower, maxTimeUpper);
            AssertCalculate(mark, 1, maxTimeLower, maxTimeUpper);
        }

        [Test]
        public void CalculateDontRemember() {
            var minTimeLower = new TimeSpan(0, 10, 0);
            var minTimeUpper = new TimeSpan(0, 15, 60);
            var maxTimeLower = new TimeSpan(1, 20, 50, 0);
            var maxTimeUpper = new TimeSpan(1, 23, 60, 60);

            AssertCalculate(KnowledgeMark.DontRemember, minTimeLower, minTimeUpper, maxTimeLower, maxTimeUpper);
        }

        [Test]
        public void CalculateNormal() {
            var minTimeLower = new TimeSpan(2, 0, 0, 0);
            var minTimeUpper = new TimeSpan(2, 3, 60, 60);
            var maxTimeLower = new TimeSpan(10, 20, 0, 0);
            var maxTimeUpper = new TimeSpan(10, 23, 60, 60);

            AssertCalculate(KnowledgeMark.Normal, minTimeLower, minTimeUpper, maxTimeLower, maxTimeUpper);
        }

        [Test]
        public void CalculateUnknown() {
            var minTimeLower = new TimeSpan(0, 20, 0);
            var minTimeUpper = new TimeSpan(0, 30, 60);
            var maxTimeLower = new TimeSpan(0, 50, 0);
            var maxTimeUpper = new TimeSpan(0, 60, 60);

            AssertCalculate((KnowledgeMark) 412, minTimeLower, minTimeUpper, maxTimeLower, maxTimeUpper);
        }

        [Test]
        public void CalculateVeryEasy() {
            var minTimeLower = new TimeSpan(10, 0, 0, 0);
            var minTimeUpper = new TimeSpan(10, 3, 60, 60);
            var maxTimeLower = new TimeSpan(365, 0, 0, 0);
            var maxTimeUpper = new TimeSpan(365, 3, 60, 60);

            AssertCalculate(KnowledgeMark.VeryEasy, minTimeLower, minTimeUpper, maxTimeLower, maxTimeUpper);
        }
    }
}