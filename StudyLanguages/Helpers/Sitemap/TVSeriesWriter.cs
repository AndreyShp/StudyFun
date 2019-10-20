using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;

namespace StudyLanguages.Helpers.Sitemap {
    public class TVSeriesWriter {
        private readonly long _languageId;
        private readonly SitemapItemWriter _sitemapItemWriter = new SitemapItemWriter();

        public TVSeriesWriter(long languageId) {
            _languageId = languageId;
        }

        public void WriteTVSeries(XElement root) {
            var tvSeriesQuery = new TVSeriesQuery(_languageId);
            List<TVSeriesInfo> covers = tvSeriesQuery.GetSeriesCovers();

            foreach (TVSeriesInfo tvSeriesInfo in covers) {
                string baseUrlPart = tvSeriesInfo.GetUrlPart();
                _sitemapItemWriter.WriteUrlToResult(root, UrlBuilder.GetTVSeriesUrl(baseUrlPart), 0.7m,
                                                    ChangeFrequency.WEEKLY);

                List<TVSeriesWatch> watches = tvSeriesQuery.GetSeriesWatches(tvSeriesInfo.GetId(),
                                                                             IdValidator.INVALID_ID);
                foreach (TVSeriesWatch tvSeriesWatch in watches) {
                    Tuple<string, string, string> pars = TVSeriesQuery.GetUrlParts(tvSeriesWatch.GetUrlPart());
                    string url = UrlBuilder.GetTVSeriesDetailUrl(pars.Item1, pars.Item2, pars.Item3);
                    _sitemapItemWriter.WriteUrlToResult(root, url, 0.6m, ChangeFrequency.MONTHLY);
                }
            }
        }
    }
}