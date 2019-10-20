using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models.Groups {
    public class GroupModelOptions {
        private readonly Func<string, SourceWithTranslation, string> _linkUrlGetter;
        private readonly Dictionary<LinkId, string> _links;

        public GroupModelOptions(Dictionary<LinkId, string> links,
                                 Func<string, SourceWithTranslation, string> linkUrlGetter) {
            _links = links;
            _linkUrlGetter = linkUrlGetter;
        }

        /// <summary>
        /// Получает исходный текст для элемента
        /// </summary>
        public Func<SourceWithTranslation, string> SourceTextGetter { get; set; }

        /// <summary>
        /// Получает текст перевода для элемента
        /// </summary>
        public Func<SourceWithTranslation, string> TranslationTextGetter { get; set; }

        /// <summary>
        /// Возвращает урл для элемента
        /// <param group="linkId">идентификатор ссылки</param>
        /// <param group="item">элемент, для которого нужно получить ссылку</param>
        /// </summary>
        public LinkInfo GetLink(string patternUrl, LinkId linkId, SourceWithTranslation item) {
            string text;
            if (!_links.TryGetValue(linkId, out text)) {
                return null;
            }
            string url = item != null ? _linkUrlGetter(patternUrl, item) : null;
            return new LinkInfo(text, url ?? CommonConstants.EMPTY_LINK);
        }
    }
}