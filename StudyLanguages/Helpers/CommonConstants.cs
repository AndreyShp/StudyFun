using System.Web;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers {
    public class CommonConstants {
        public const string EMPTY_LINK = "javascript:void(0)";
        public const string EMPTY_IMAGE_PATH = "Content/images/empty.png";
        public const string PATTERN_URL = "speak?id={0}&type={1}&mp3Support={2}";
        public const string FILL_GAPS = "Заполните пробелы";

        public const string IMAGE_CONTENT_TYPE = "image/jpeg";

        private const string PDF_FONT_NAME = "~/App_Data/arial.ttf";

        public const int COUNT_DAYS_TO_HOLD_DATA = 60;

        //TODO: удалить заглушку
        public const string FRIENDS_TV_SERIES = "Друзья";

        public static string GetText(PronunciationForUser pronunciationForUser) {
            return pronunciationForUser.Text;
        }
        
        public static string GetFontPath(HttpServerUtilityBase server) {
            return server.MapPath(PDF_FONT_NAME);
        }
    }
}