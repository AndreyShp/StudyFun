using System.Collections.Generic;
using BusinessLogic.Export;

namespace StudyLanguages.Models {
    public class DownloadBtnModel {
        private readonly Dictionary<DocumentType, string> _urlsByTypes = new Dictionary<DocumentType, string>();

        public DownloadBtnModel(string title, Dictionary<DocumentType, string> urlsByTypes)
            : this(title, null, urlsByTypes) {}

        public DownloadBtnModel(string title, string idForGoal, Dictionary<DocumentType, string> urlsByTypes) {
            Title = title;
            _urlsByTypes = urlsByTypes ?? new Dictionary<DocumentType, string>(0);
            IdForGoal = idForGoal;
            SizeBtn = Size.Normal;
        }

        public string Title { get; private set; }
        public string IdForGoal { get; private set; }
        public Size SizeBtn { get; set; }
        public string Url { get; set; }

        public string GetUrl(DocumentType type) {
            string result;
            return _urlsByTypes.TryGetValue(type, out result) ? result : null;
        }

        public enum Size {
            Normal = 0,
            Small = 1,
            Biggest = 2
        }
    }
}