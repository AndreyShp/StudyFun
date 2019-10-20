using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox.Classes {
    public class MemoTrainerDemo {
        private readonly List<Info> _infos = new List<Info>();
        private const int MIN_COUNT_SHOWED = 0;

        private static readonly List<Func<Info, bool>> _periodsChangers = new List<Func<Info,bool>>();

        public MemoTrainerDemo(IEnumerable<string> data) {
            foreach (string item in data) {
                var info = new Info { Data = item, Ef = 2.5m, Repetition = MIN_COUNT_SHOWED, LastShowedTime = DateTime.Now };
                _infos.Add(info);
            }

            _periodsChangers.Add(info => {
                var result = info.Repetition == MIN_COUNT_SHOWED;
                if (result) {
                    info.Interval = 1;
                }
                return result;
            });
            _periodsChangers.Add(info => {
                var result = info.Repetition == MIN_COUNT_SHOWED + 1;
                if (result) {
                    info.Interval = 6;
                }
                return result;
            });
            _periodsChangers.Add(info => {
                var newInterval = info.Interval * info.Ef;
                info.Interval = (int)Math.Round(newInterval, MidpointRounding.AwayFromZero);
                return true;
            });
        }

        public string Get() {
            Info result = _infos.OrderBy(e => e.TimeToNextShow).FirstOrDefault();
            return result != null ? result.Data : null;
        }

        public void SetMark(string item, int mark) {
            const int MAX_MARK = 5;
            const decimal MIN_EF = 1.3m;

            Info foundInfo = _infos.FirstOrDefault(e => e.Data == item);
            if (foundInfo == null) {
                return;
            }
            
            foreach (var periodsChanger in _periodsChangers) {
                if (periodsChanger(foundInfo)) {
                    //TODO: логирование
                    break;
                }
            }

            foundInfo.Repetition++;
            foundInfo.Mark = mark;
            foundInfo.LastShowedTime = DateTime.Now;
            if (mark < 3) {
                foundInfo.Repetition = MIN_COUNT_SHOWED;
                return;
            }
            
            int pointsToMaxMark = (MAX_MARK - mark);
            decimal newEf = foundInfo.Ef + (0.1m - pointsToMaxMark * (0.08m + pointsToMaxMark * 0.02m));
            if (newEf < MIN_EF) {
                newEf = MIN_EF;
            }
            foundInfo.Ef = newEf;
        }

        public string GetDump() {
            StringBuilder sb = new StringBuilder();
            foreach (var info in _infos) {
                sb.AppendLine(string.Format("{0}. Ef = {1}. LastIntervalToShow = {2}, TimeToNextShow = {3}, Repetition = {4}, Mark = {5}", info.Data,
                                            info.Ef, info.Interval, info.TimeToNextShow, info.Repetition, info.Mark));
            }
            return sb.ToString();
        }

        #region Nested type: Info

        private class Info {
            public string Data { get; set; }
            public decimal Ef { get; set; }
            public int Mark { get; set; }
            public int Interval { get; set; }
            public DateTime LastShowedTime { get; set; }
            public DateTime TimeToNextShow {
                get { return LastShowedTime.AddMinutes(Interval); }
            }

            public int Repetition { get; set; }
        }

        #endregion
    }
}