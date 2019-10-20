using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Formatters;
using BusinessLogic.Validators;

namespace BusinessLogic.Video.Subtitles {
    public class SubtitlesAnalyzer {
        private readonly Encoding _encoding;
        private readonly string _fileName;

        public SubtitlesAnalyzer(string fileName, Encoding encoding) {
            _fileName = fileName;
            _encoding = encoding;
        }

        public List<Subtitle> Analyze(string timeShiftFile) {
            const int MIN_ROWS = 3;
            string rowSeparator = Environment.NewLine;

            Dictionary<int, TimeSpan> timeShifts = GetTimeshift(timeShiftFile);

            var result = new List<Subtitle>();

            string allContent = File.ReadAllText(_fileName, _encoding);
            string[] blocks = allContent.Split(new[] {
                rowSeparator + rowSeparator
            }, StringSplitOptions.RemoveEmptyEntries);

            var currentTimeShift = new TimeSpan();
            int i = 1;
            foreach (string block in blocks) {
                string[] rows = block.Split(new[] {rowSeparator}, MIN_ROWS, StringSplitOptions.RemoveEmptyEntries);
                if (rows.Length < MIN_ROWS) {
                    Console.WriteLine("Блок {0} содержит менее {1} строк", i, MIN_ROWS);
                    continue;
                }

                string dirtyNumber = rows[0];
                if (dirtyNumber == "9999") {
                    continue;
                }
                int number = int.Parse(dirtyNumber);

                string dirtyTime = rows[1];
                var videoEpisode = new Subtitle {
                    Text = rows[2],
                };

                string[] timeParts = dirtyTime.Split(new[] {" --> "}, StringSplitOptions.RemoveEmptyEntries);
                if (timeParts.Length != 2) {
                    Console.WriteLine("Время в блоке {0} некорректно!", i);
                    continue;
                }

                TimeSpan from = DateTimeFormatter.ToTimeSpan(timeParts[0]);
                TimeSpan to = DateTimeFormatter.ToTimeSpan(timeParts[1]);

                TimeSpan time;
                if (timeShifts.TryGetValue(number, out time)) {
                    currentTimeShift = time - from;
                }

                from += currentTimeShift;
                to += currentTimeShift;

                videoEpisode.TimeFrom = from.TotalSeconds;
                videoEpisode.TimeTo = to.TotalSeconds;

                if (EnumerableValidator.IsNotEmpty(result) && videoEpisode.TimeFrom <= result[result.Count - 1].TimeTo) {
                    var prevSubtitle = result[result.Count - 1];
                    prevSubtitle.TimeTo = videoEpisode.TimeFrom - 0.1; //на 100мс сдвигаем время
                    if (prevSubtitle.TimeFrom >= prevSubtitle.TimeTo) {
                        Console.WriteLine("Что-то не то с временем для {0} !!! Время от {1} до {2}", prevSubtitle.Text, prevSubtitle.TimeFrom, prevSubtitle.TimeTo);
                    }
                }

                i++;

                result.Add(videoEpisode);
            }

            return result;
        }

        private static Dictionary<int, TimeSpan> GetTimeshift(string timeShiftFile) {
            if (!File.Exists(timeShiftFile)) {
                return new Dictionary<int, TimeSpan>(0);
            }

            var result = new Dictionary<int, TimeSpan>();
            string[] lines = File.ReadAllLines(timeShiftFile);
            foreach (string line in lines) {
                string[] parts = line.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length <= 1) {
                    continue;
                }

                int index = int.Parse(parts[0]);

                string dirtyTime = parts[1];
                char sign = dirtyTime[0];
                if (!char.IsDigit(sign)) {
                    dirtyTime = dirtyTime.Substring(1);
                }

                TimeSpan time = DateTimeFormatter.ToTimeSpan(dirtyTime);
                if (sign == '-') {
                    time = time.Negate();
                }

                result.Add(index, time);
            }
            return result;
        }
    }
}