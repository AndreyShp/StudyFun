using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData.Auxiliaries;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models {
    public class CrossReferencesModel {
        private readonly Dictionary<CrossReferenceType, List<CrossReference>> _crossReferencesByType =
            new Dictionary<CrossReferenceType, List<CrossReference>>();
        private readonly string _current;

        private readonly List<Tuple<CrossReferenceType, string>> _sortedTitlesBySortedTitlesByTypes =
            new List<Tuple<CrossReferenceType, string>>();
        private readonly Dictionary<CrossReferenceType, string> _titlesByTypes =
            new Dictionary<CrossReferenceType, string> {
                {CrossReferenceType.GroupWord, "слова по темам"},
                {CrossReferenceType.GroupSentence, "фразы по темам"},
                {CrossReferenceType.VisualDictionary, "визуальные словари по темам"}
            };

        public CrossReferencesModel(string current,
                                    CrossReferenceType currentType,
                                    IEnumerable<CrossReference> crossReferences) {
            if (crossReferences == null) {
                return;
            }
            _current = current;
            foreach (CrossReference crossReference in crossReferences) {
                CrossReferenceType type = crossReference.Type;

                List<CrossReference> crossReferencesByType;
                if (!_crossReferencesByType.TryGetValue(type, out crossReferencesByType)) {
                    crossReferencesByType = new List<CrossReference>();
                    _crossReferencesByType.Add(type, crossReferencesByType);

                    if (type != currentType) {
                        _sortedTitlesBySortedTitlesByTypes.Add(GetTitleByType(type));
                    }
                }
                crossReferencesByType.Add(crossReference);
                HasCrossReferences = true;
            }

            //если есть рекомендуемые данные для текущей тематики, то поместить их в начало
            if (_crossReferencesByType.ContainsKey(currentType)) {
                _sortedTitlesBySortedTitlesByTypes.Insert(0, GetTitleByType(currentType));
            }
        }

        public bool HasCrossReferences { get; private set; }

        public List<Tuple<CrossReferenceType, string>> SortedTitlesByTypes {
            get { return _sortedTitlesBySortedTitlesByTypes; }
        }

        public bool IsTheSameName(string name) {
            return string.Equals(_current, name, StringComparison.InvariantCultureIgnoreCase);
        }

        public List<CrossReference> GetReferencesByType(CrossReferenceType type) {
            List<CrossReference> result;
            if (!_crossReferencesByType.TryGetValue(type, out result)) {
                result = new List<CrossReference>(0);
            }
            return result.OrderBy(e => e.ReferenceName).ToList();
        }

        private Tuple<CrossReferenceType, string> GetTitleByType(CrossReferenceType type) {
            string title = "Смотрите " + _titlesByTypes[type];
            return new Tuple<CrossReferenceType, string>(type, title);
        }

        public string GetUrl(HttpRequestBase request, CrossReference crossReference) {
            if (crossReference.Type == CrossReferenceType.GroupWord) {
                return UrlBuilder.GetGroupWordsUrl(request, crossReference.ReferenceName);
            }
            if (crossReference.Type == CrossReferenceType.GroupSentence) {
                return UrlBuilder.GetGroupSentencesUrl(request, crossReference.ReferenceName);
            }
            if (crossReference.Type == CrossReferenceType.VisualDictionary) {
                return UrlBuilder.GetVisualDictionaryUrl(request, crossReference.ReferenceName);
            }
            return null;
        }
    }
}