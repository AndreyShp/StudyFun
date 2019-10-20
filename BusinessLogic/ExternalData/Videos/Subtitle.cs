using System.Collections.Generic;
using BusinessLogic.Formatters;

namespace BusinessLogic.ExternalData.Videos {
    /// <summary>
    /// Субтитр
    /// </summary>
    public class Subtitle {
        /// <summary>
        /// Текст субтитра на языке оригинала
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Перевод субтитра
        /// </summary>
        public string TranslationText { get; set; }

        /// <summary>
        /// Время от
        /// </summary>
        public double TimeFrom { get; set; }

        /// <summary>
        /// Время по
        /// </summary>
        public double TimeTo { get; set; }

        /// <summary>
        /// Идентификаторы слов из субтитров
        /// </summary>
        public List<long> WordTranslations { get; set; }

        /*/// <summary>
        /// Длительность текущего субтитра
        /// </summary>
        [ScriptIgnore]
        public TimeSpan Duration {
            get { return TimeTo - TimeFrom; }
        }
*/

        /// <summary>
        /// Представляет субтитр в красивом формате
        /// </summary>
        /// <returns></returns>
        public string ToPrettyFormat() {
            return TimeFrom + "-" + TimeTo +  /*"(" + DateTimeFormatter.ToHHMMSSFFF(Duration) + ")" + */ " " + Text;
        }
    }
}