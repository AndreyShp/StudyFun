using System.Globalization;

namespace StudyLanguages.Helpers.Formatters {
    public static class MoneyFormatter {
        public static string ToRubles(decimal value) {
            return value.ToString("0.00", CultureInfo.GetCultureInfo("ru-RU"));
        }
    }
}