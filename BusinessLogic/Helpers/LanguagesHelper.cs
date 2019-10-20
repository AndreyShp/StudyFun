using System;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Helpers {
    public class LanguagesHelper {
        /// <summary>
        /// Возвращает название языка на русском языке
        /// </summary>
        /// <param name="shortName">название языка</param>
        /// <returns>название языка на русском языке с маленькой буквы</returns>
        public static string GetPrettyLowerName(LanguageShortName shortName) {
            switch (shortName) {
                case LanguageShortName.Ru:
                    return "русский";
                case LanguageShortName.En:
                    return "английский";
                case LanguageShortName.De:
                    return "немецкий";
                case LanguageShortName.Fr:
                    return "французский";
                case LanguageShortName.It:
                    return "итальянский";
                case LanguageShortName.Es:
                    return "испанский";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Возвращает часть названия языка(без окончания) на русском языке
        /// </summary>
        /// <param name="shortName">название языка</param>
        /// <returns>название языка на русском языке с маленькой буквы</returns>
        public static string GetLowerNameWithoutEnding(LanguageShortName shortName) {
            string result = GetPrettyLowerName(shortName);
            if (result != null) {
                result = TrimEnding(result, "ий");
            }
            return result;
        }

        private static string TrimEnding(string result, string ending) {
            int endingIndex = result.LastIndexOf(ending, StringComparison.InvariantCultureIgnoreCase);
            if (endingIndex != -1 && endingIndex == (result.Length - ending.Length)) {
                result = result.Substring(0, endingIndex);
            }
            return result;
        }
    }
}