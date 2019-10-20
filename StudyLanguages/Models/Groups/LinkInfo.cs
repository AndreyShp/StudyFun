using StudyLanguages.Helpers;

namespace StudyLanguages.Models.Groups {
    public class LinkInfo {
        public LinkInfo(string text, string url) {
            Text = text;
            Url = url;
        }

        public string Text { get; private set; }

        public string Url { get; private set; }

        public string Class {
            get { return Url == CommonConstants.EMPTY_LINK ? " disabled" : string.Empty; }
        }
    }
}