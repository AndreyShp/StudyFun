namespace StudyLanguages.Helpers {
    public class StyleBuilder {
        public static string GetLinkClass(string url) {
            return url == CommonConstants.EMPTY_LINK ? " disabled" : string.Empty;
        }
    }
}