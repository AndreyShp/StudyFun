using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using BusinessLogic;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers.Sitemap {
    public class HomeWriter {
        private const int COUNT_SENTENCES = 1000;

        private readonly SitemapItemWriter _sitemapItemWriter = new SitemapItemWriter();
        private readonly UserLanguages _userLanguages;

        public HomeWriter(UserLanguages userLanguages) {
            _userLanguages = userLanguages;
        }

        public void Write(XElement root) {
            _sitemapItemWriter.WriteUrlToResult(root, UrlBuilder.GetSentenceHomeUrl(), 0.75m, ChangeFrequency.MONTHLY);

            ISentencesQuery sentencesQuery = new SentencesQuery();
            List<SourceWithTranslation> sourcesWithTranslation = sentencesQuery.GetByCount(_userLanguages,
                                                                                           SentenceType.Separate,
                                                                                           COUNT_SENTENCES);
            foreach (SourceWithTranslation sourceWithTranslation in sourcesWithTranslation) {
                string url =
                    UrlBuilder.GetSentenceHomeTrainerUrl(
                        sourceWithTranslation.Source.Id.ToString(CultureInfo.InvariantCulture),
                        sourceWithTranslation.Translation.Id.ToString(CultureInfo.InvariantCulture));
                _sitemapItemWriter.WriteUrlToResult(root, url, 0.2m, ChangeFrequency.YEARLY);
            }
        }
    }
}