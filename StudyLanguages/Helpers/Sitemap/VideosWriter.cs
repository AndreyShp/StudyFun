using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.ExternalData.Videos;

namespace StudyLanguages.Helpers.Sitemap {
    public class VideosWriter {
        private readonly long _languageId;
        private readonly SitemapItemWriter _sitemapItemWriter = new SitemapItemWriter();

        public VideosWriter(long languageId) {
            _languageId = languageId;
        }

        public void WriteVideos(XElement root, VideoType type) {
            _sitemapItemWriter.WriteUrlToResult(root, UrlBuilder.GetVideosUrl(), 1, ChangeFrequency.WEEKLY);

            IVideosQuery videosQuery = new VideosQuery(_languageId);
            List<VideoForUser> videos = videosQuery.GetVisible(type);
            foreach (VideoForUser video in videos) {
                string title = video.Title;
                string url = UrlBuilder.GetVideoDetailUrl(title);
                _sitemapItemWriter.WriteUrlToResult(root, url, 0.9m);
            }
        }
    }
}