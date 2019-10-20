using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models.Video;

namespace StudyLanguages.Controllers {
    public class TVSeriesController : BaseController {
        private const string SERIES_FOLDER = "Series";

        public ActionResult Index() {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.TVSeries)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            /*var tvSeriesQuery = new TVSeriesQuery(WebSettingsConfig.Instance.DefaultUserLanguages.From.Id);
            var series = tvSeriesQuery.GetSeries();

            return View("../Video/TVSeries", series);*/

            return new EmptyResult();
        }

        public ActionResult Detail(string baseUrlPart) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.TVSeries)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            var tvSeriesQuery = new TVSeriesQuery(WebSettingsConfig.Instance.DefaultUserLanguages.From.Id);
            List<TVSeriesWatch> watches = tvSeriesQuery.GetSeriesWatches(baseUrlPart);
            if (EnumerableValidator.IsEmpty(watches)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            TVSeriesInfo seriesInfo = watches[0].GetSeriesInfo();
            var model = new TVSeriesModel {
                Title = seriesInfo.Title,
                OrigTitle = seriesInfo.OrigTitle,
                Series = ConvertWatchesToModels(watches)
            };

            return View("../Video/TVSeriesIndex", model);
        }

        [UserId]
        public ActionResult Watch(long userId, string baseUrlPart, int season, int episode) {
            if (IsInvalid(baseUrlPart, season, episode)) {
                //TODO: на другой урл
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            string urlPart = TVSeriesQuery.GetUrlPart(baseUrlPart, season, episode);
            TVSeriesWatch watch = GetSeriesWatch(urlPart);
            if (watch == null) {
                //TODO: на другой урл
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            watch.IsAdmin = WebSettingsConfig.Instance.IsAdmin(userId);
            return View("../Video/TVSeriesDetail", watch);
        }

        private static bool IsInvalid(string baseUrlPart, int season, int episode) {
            return WebSettingsConfig.Instance.IsSectionForbidden(SectionId.TVSeries)
                   || string.IsNullOrWhiteSpace(baseUrlPart) || season <= 0 || episode <= 0;
        }

        public JsonResult GetAnotherSeries(long parentId, long id) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.TVSeries) || IdValidator.IsInvalid(parentId)
                || IdValidator.IsInvalid(id)) {
                //TODO: на другой урл
                return JsonResultHelper.Error();
            }

            var tvSeriesQuery = new TVSeriesQuery(WebSettingsConfig.Instance.DefaultUserLanguages.From.Id);
            List<TVSeriesWatch> watches = tvSeriesQuery.GetSeriesWatches(parentId, id);
            IEnumerable<TVSeriesShortModel> result = ConvertWatchesToModels(watches);

            return JsonResultHelper.GetUnlimitedJsonResult(result);
        }

        private IEnumerable<TVSeriesShortModel> ConvertWatchesToModels(IEnumerable<TVSeriesWatch> watches) {
            IEnumerable<TVSeriesShortModel> result = watches.Select(e => {
                Tuple<string, string, string> parts = TVSeriesQuery.GetUrlParts(e.GetUrlPart());
                var pars = new {baseUrlPart = parts.Item1, season = parts.Item2, episode = parts.Item3};

                return new TVSeriesShortModel {
                    Title = e.TransTitle,
                    Episode = e.Episode,
                    Url = Url.Action("Watch", RouteConfig.TV_SERIES_CONTROLLER, pars),
                    ImageUrl = Url.Action("GetImageByName", RouteConfig.TV_SERIES_CONTROLLER, pars),
                };
            });
            return result;
        }

        private static TVSeriesWatch GetSeriesWatch(string urlPart) {
            var tvSeriesQuery = new TVSeriesQuery(WebSettingsConfig.Instance.DefaultUserLanguages.From.Id);
            TVSeriesWatch result = tvSeriesQuery.GetSeriesWatch(urlPart);
            return result;
        }

        [HttpGet]
        [Cache]
        public FileResult GetVideo(string urlPart) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.TVSeries)) {
                return null;
            }
            TVSeriesWatch seriesWatch = GetSeriesWatch(urlPart);
            if (seriesWatch == null) {
                return null;
            }

            string fullFileName = WebSettingsConfig.Instance.GetDataFileName(SERIES_FOLDER, seriesWatch.GetPathToFiles(),
                                                                             seriesWatch.VideoFileName);
            byte[] fileContents = ReadFile(fullFileName);
            if (EnumerableValidator.IsNullOrEmpty(fileContents)) {
                return null;
            }

            TVSeriesInfo seriesInfo = seriesWatch.GetSeriesInfo();
            return File(fileContents, "video/mp4", seriesInfo.OrigTitle + "_" + seriesWatch.VideoFileName);
        }

        [HttpGet]
        [Cache]
        public ActionResult GetImageByName(string baseUrlPart, int season, int episode) {
            if (IsInvalid(baseUrlPart, season, episode)) {
                //TODO: на другой урл
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            string urlPart = TVSeriesQuery.GetUrlPart(baseUrlPart, season, episode);
            TVSeriesWatch seriesWatch = GetSeriesWatch(urlPart);
            if (seriesWatch == null) {
                return null;
            }

            string fullFileName = WebSettingsConfig.Instance.GetDataFileName(SERIES_FOLDER, seriesWatch.GetPathToFiles(),
                                                                             seriesWatch.ImageFileName);
            TVSeriesInfo seriesInfo = seriesWatch.GetSeriesInfo();
            return GetImage(seriesInfo.OrigTitle + "_" + seriesWatch.ImageFileName, e => ReadFile(fullFileName));
        }

        private static byte[] ReadFile(string fullFileName) {
            byte[] result = null;
            if (System.IO.File.Exists(fullFileName)) {
                result = System.IO.File.ReadAllBytes(fullFileName);
            }
            return result;
        }

        /*[HttpGet]
        [Cache]
        public FileResult GetNativeSubtitles() {
            string fileName = "1_1.srt";
            string fullFileName = WebSettingsConfig.Instance.GetDataFileName("Series", fileName);
            byte[] fileContents = System.IO.File.ReadAllBytes(fullFileName);

            return File(fileContents, "text/plain", fileName);
        }

        [HttpGet]
        [Cache]
        public FileResult GetParsedSubtitles() {
            string fileName = "1_1.srt";
            string fullFileName = WebSettingsConfig.Instance.GetDataFileName("Video", fileName);
            byte[] fileContents = System.IO.File.ReadAllBytes(fullFileName);

            return File(fileContents, "text/plain", fileName);
        }*/
    }
}