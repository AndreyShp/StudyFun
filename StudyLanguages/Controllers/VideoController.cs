using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Videos;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class VideoController : BaseController {
        public ActionResult Index(VideoType type) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.Video)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            IVideosQuery videosQuery = GetVideosQuery();
            List<VideoForUser> videos = videosQuery.GetVisible(type);

            var sorter = new GroupsSorter(HttpContext.Request.Cookies);
            sorter.Sort(videos);

            List<GroupForUser> convertedVideos =
                videos.Select(e => new GroupForUser(e.Id, e.Title, e.HasImage)).ToList();

            return View(convertedVideos);
        }

        public ActionResult Detail(string group) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.Video)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            VideoForUser video = GetVideoForUser(group);
            if (video == null) {
                return RedirectToAction("Index", new { type = VideoType.Clip });
            }
            return View("../Video/Detail", video);
        }

        public ActionResult Download(string group, DocumentType type) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.Video)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            VideoForUser video = GetVideoForUser(group);
            if (video == null) {
                return RedirectToAction("Index", new { type = VideoType.Clip });
            }

            string fileName = string.Format("Текст из видео {0}", video.Title.ToLowerInvariant());
            var downloader = new VideoTextDownloader(WebSettingsConfig.Instance.DomainWithProtocol,
                                                            CommonConstants.GetFontPath(Server));
            var documentGenerator = downloader.Download(type, fileName, video);

            return File(documentGenerator.Generate(), documentGenerator.ContentType, documentGenerator.FileName);
        }

        private static VideoForUser GetVideoForUser(string group) {
            if (string.IsNullOrEmpty(group)) {
                return null;
            }
            IVideosQuery videosQuery = GetVideosQuery();
            VideoForUser result = videosQuery.Get(group);
            return result;
        }

        private static IVideosQuery GetVideosQuery() {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            IVideosQuery videosQuery = new VideosQuery(languageId);
            return videosQuery;
        }

        [HttpPost]
        public EmptyResult NewVisitor(long id) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var ratingByIpsQuery = new RatingByIpsQuery(languageId);
            ratingByIpsQuery.AddNewVisitor(RemoteClientHelper.GetClientIpAddress(Request), id, RatingPageType.Video);
            return new EmptyResult();
        }

        [HttpGet]
        [Cache]
        public ActionResult GetImageByName(string group) {
            IVideosQuery videosQuery = GetVideosQuery();
            return GetImage(group, videosQuery.GetImage);
        }

        public ActionResult AddNew() {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.Video)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            return View("Index");
        }
    }
}