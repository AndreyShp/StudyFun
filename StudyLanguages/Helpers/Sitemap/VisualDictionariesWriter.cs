using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.ExternalData.Representations;

namespace StudyLanguages.Helpers.Sitemap {
    public class VisualDictionariesWriter {
        private readonly long _languageId;
        private readonly SitemapItemWriter _sitemapItemWriter = new SitemapItemWriter();

        public VisualDictionariesWriter(long languageId) {
            _languageId = languageId;
        }

        public void WriteDictionaries(XElement root, ISalesSettings salesSettings) {
            _sitemapItemWriter.WriteUrlToResult(root, UrlBuilder.GetVisualDictionariesUrl(), 1, ChangeFrequency.MONTHLY);
            
            IRepresentationsQuery representationsQuery = new RepresentationsQuery(_languageId);
            List<RepresentationForUser> visibleDictionaries = representationsQuery.GetVisibleWithoutAreas();
            foreach (RepresentationForUser visibleDictionary in visibleDictionaries) {
                string title = visibleDictionary.Title;
                string url = UrlBuilder.GetVisualDictionaryUrl(title);
                _sitemapItemWriter.WriteUrlToResult(root, url, 0.9m);

                url = UrlBuilder.GetVisualDictionaryTrainerUrl(title);
                _sitemapItemWriter.WriteUrlToResult(root, url, 0.4m, ChangeFrequency.YEARLY);
            }
        }
    }
}