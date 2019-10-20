using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using BusinessLogic;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers.Sitemap {
    public class AudioWordsWriter {
        private const int COUNT_SENTENCES = 10000;

        private readonly SitemapItemWriter _sitemapItemWriter = new SitemapItemWriter();
        private readonly UserLanguages _userLanguages;

        public AudioWordsWriter(UserLanguages userLanguages) {
            _userLanguages = userLanguages;
        }

        public void Write(XElement root) {
            _sitemapItemWriter.WriteUrlToResult(root, UrlBuilder.GetAudioWordsUrl(), 0.75m, ChangeFrequency.MONTHLY);

            /*var shuffleWordsQuery = new ShuffleWordsQuery(WordType.Default, ShuffleType.Audio);
            List<SourceWithTranslation> sourcesWithTranslation = shuffleWordsQuery.GetByCount(_userLanguages, COUNT_SENTENCES);
            foreach (SourceWithTranslation sourceWithTranslation in sourcesWithTranslation) {
                string url =
                    UrlBuilder.GetAudioWordsTrainerUrl(
                        sourceWithTranslation.Source.Id.ToString(CultureInfo.InvariantCulture),
                        sourceWithTranslation.Translation.Id.ToString(CultureInfo.InvariantCulture));
                _sitemapItemWriter.WriteUrlToResult(root, url, 0.2m, ChangeFrequency.YEARLY);
            }*/
        }
    }
}