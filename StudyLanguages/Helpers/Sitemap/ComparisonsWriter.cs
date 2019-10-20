using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.ExternalData.Comparisons;

namespace StudyLanguages.Helpers.Sitemap {
    public class ComparisonsWriter {
        protected readonly SitemapItemWriter SitemapItemWriter = new SitemapItemWriter();
        private readonly long _languageId;

        public ComparisonsWriter(long languageId) {
            _languageId = languageId;
        }

        public void WriteComparisons(XElement root) {
            string url = UrlBuilder.GetComparisonsUrl();
            SitemapItemWriter.WriteUrlToResult(root, url, 1);

            IComparisonsQuery comparisonsQuery = new ComparisonsQuery(_languageId);
            List<ComparisonForUser> comparisons = comparisonsQuery.GetVisibleWithoutRules();
            foreach (ComparisonForUser comparison in comparisons) {
                url = UrlBuilder.GetComparisonUrl(comparison.Title);
                SitemapItemWriter.WriteUrlToResult(root, url, 0.9m);
            }
        }
    }
}