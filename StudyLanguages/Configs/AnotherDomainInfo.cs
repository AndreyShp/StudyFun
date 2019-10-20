using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Configs {
    /// <summary>
    /// Информация о других доменах
    /// </summary>
    public class AnotherDomainInfo {
        private readonly string _name;
        private readonly string _link;
        private readonly Dictionary<SectionId, Tuple<string, string>> _pages = new Dictionary<SectionId, Tuple<string, string>>();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">название языка</param>
        /// <param name="link">ссылка на главную страницу другого языка</param>
        /// <param name="flag">флаг</param>
        public AnotherDomainInfo(string name, string link, string flag) {
            _name = name;
            _link = link;
            Flag = flag;
        }

        public string Flag { get; private set; }

        public string GetTitle(SectionId sectionId) {
            var linkInfo = GetBySection(sectionId);
            return string.IsNullOrWhiteSpace(linkInfo.Item2) ? _name : linkInfo.Item2;
        }

        public string GetLink(SectionId sectionId) {
            var linkInfo = GetBySection(sectionId);
            return string.IsNullOrWhiteSpace(linkInfo.Item1) ? _link : linkInfo.Item1;
        }

        private Tuple<string, string> GetBySection(SectionId sectionId) {
            Tuple<string, string> result;
            if (!_pages.TryGetValue(sectionId, out result)) {
                result = new Tuple<string, string>(null, null);
            }
            return result;
        }

        public void AddPage(SectionId sectionId, string url, string title) {
            if (!_pages.ContainsKey(sectionId)) {
                string fullUrl = string.Format("{0}/{1}", _link.TrimEnd('/'), url.TrimStart('/'));
                _pages.Add(sectionId, new Tuple<string, string>(fullUrl, title));
            }
        }

        public bool IsValid() {
            return !string.IsNullOrWhiteSpace(_name) && !string.IsNullOrWhiteSpace(_link)
                   && !string.IsNullOrWhiteSpace(Flag);
        }
    }
}