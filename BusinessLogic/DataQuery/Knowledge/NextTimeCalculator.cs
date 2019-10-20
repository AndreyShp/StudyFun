using System;
using BusinessLogic.Data.Enums;
using BusinessLogic.Logger;

namespace BusinessLogic.DataQuery.Knowledge {
    public class NextTimeCalculator {
        private readonly Random _random = new Random();

        public TimeSpan Calculate(KnowledgeMark mark, double ratio) {
            TimeSpan minDate;
            TimeSpan maxDate;

            TimeSpan repeatInterval;
            if (mark == KnowledgeMark.VeryEasy) {
                minDate = new TimeSpan(10, _random.Next(0, 3), _random.Next(0, 60), _random.Next(0, 60));
                maxDate = new TimeSpan(365, _random.Next(0, 3), _random.Next(0, 60), _random.Next(0, 60));
                int countDays = GetRepeatInterval(minDate.TotalDays, maxDate.TotalDays, ratio);
                repeatInterval = new TimeSpan(countDays, 0, 0, 0);

            } else if (mark == KnowledgeMark.Normal) {
                minDate = new TimeSpan(2, _random.Next(0, 3), _random.Next(0, 60), _random.Next(0, 60));
                maxDate = new TimeSpan(10, _random.Next(20, 23), _random.Next(50, 60), _random.Next(0, 60));
                int countHours = GetRepeatInterval(minDate.TotalHours, maxDate.TotalHours, ratio);
                repeatInterval = new TimeSpan(countHours, 0, 0);

            } else if (mark == KnowledgeMark.DontRemember) {
                minDate = new TimeSpan(0, _random.Next(10, 15), _random.Next(0, 60));
                maxDate = new TimeSpan(1, _random.Next(20, 23), _random.Next(50, 60), _random.Next(0, 60));
                int countMinutes = GetRepeatInterval(minDate.TotalMinutes, maxDate.TotalMinutes, ratio);
                repeatInterval = new TimeSpan(0, countMinutes, 0);

            } else {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "UserRepetitionIntervalQuery.SetNextByMark передан некорректная оценка {0}", mark);
                minDate = new TimeSpan(0, _random.Next(20, 30), _random.Next(0, 60));
                maxDate = new TimeSpan(0, _random.Next(50, 60), _random.Next(0, 60));
                int countSeconds = GetRepeatInterval(minDate.TotalSeconds, maxDate.TotalSeconds, ratio);
                repeatInterval = new TimeSpan(0, 0, countSeconds);
            }

            return repeatInterval;
        }

        private static int GetRepeatInterval(double minTime, double maxTime, double ratio) {
            return (int) (minTime + (maxTime - minTime) * ratio);
        }
    }
}