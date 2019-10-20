using System.Globalization;
using System.Xml.Linq;
using StudyLanguages.Configs;

namespace StudyLanguages.Helpers.Sitemap {
    public class SitemapItemWriter {
        public void WriteUrlToResult(XElement root,
                                     string url,
                                     decimal priority,
                                     string changeFrequency = ChangeFrequency.DAILY) {
            string formattedUrl = UrlBuilder.ConcatDomainWithUrl(WebSettingsConfig.Instance.DomainWithProtocol, url);
            string formattedPriority = priority.ToString("0.00", CultureInfo.InvariantCulture);
            root.Add(
                new XElement("url",
                             new XElement("loc", formattedUrl),
                             new XElement("changefreq", changeFrequency),
                             new XElement("priority", formattedPriority)));
        }
    }
}