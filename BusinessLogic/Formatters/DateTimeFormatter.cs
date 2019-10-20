using System;
using System.Globalization;

namespace BusinessLogic.Formatters {
    public static class DateTimeFormatter {
        public static string ToDDMMYYYY_HHMMSS(long ticks) {
            var dateTime = new DateTime(ticks);
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static string ToDDMMYYYY_HHMMSS(DateTime dateTime) {
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public static string ToDDMMYYYY_HH_MM_SS(DateTime dateTime) {
            return dateTime.ToString("dd.MM.yyyy HH_mm_ss");
        }

        public static TimeSpan ToTimeSpan(string time) {
            string format = @"hh\:mm\:ss";
            if (time.Contains(",")) {
                format += @"\,fff";
            }
            return TimeSpan.ParseExact(time, format, CultureInfo.InvariantCulture);
        }

        public static string ToHHMMSS(double seconds) {
            return ToHHMMSS(TimeSpan.FromSeconds(seconds));
        }

        public static string ToHHMMSS(TimeSpan time) {
            return time.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        public static string ToHHMMSSFFF(TimeSpan time, char millisecondDelimiter = ',') {
            return ToHHMMSS(time) + time.ToString("\\" + millisecondDelimiter + "fff", CultureInfo.InvariantCulture);
        }
    }
}