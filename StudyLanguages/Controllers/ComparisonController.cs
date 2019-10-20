using System.IO;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.Helpers;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class ComparisonController : BaseController {
        [UserLanguages]
        public ActionResult Index(UserLanguages userLanguages, string group) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.FillDifference)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.GROUPS_BY_COMPARISONS_CONTROLLER);
            }

            ComparisonForUser comparisonForUser = GetComparisonForUser(userLanguages, group);
            if (comparisonForUser == null) {
                return RedirectToActionPermanent("Index", RouteConfig.GROUPS_BY_COMPARISONS_CONTROLLER);
            }

            return View(comparisonForUser);
        }

        [UserLanguages]
        public ActionResult Download(UserLanguages userLanguages, string group, DocumentType type) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.GROUPS_BY_COMPARISONS_CONTROLLER);
            }

            ComparisonForUser comparisonForUser = GetComparisonForUser(userLanguages, group);
            if (comparisonForUser == null) {
                return RedirectToActionPermanent("Index", RouteConfig.GROUPS_BY_COMPARISONS_CONTROLLER);
            }

            var downloader = new ComparisonDownloader(WebSettingsConfig.Instance.DomainWithProtocol,
                                                      CommonConstants.GetFontPath(Server));
            var documentGenerator = downloader.Download(type, comparisonForUser.Title, comparisonForUser);

            return File(documentGenerator.Generate(), documentGenerator.ContentType, documentGenerator.FileName);
        }

        private static ComparisonForUser GetComparisonForUser(UserLanguages userLanguages, string group) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var comparisonsQuery = new ComparisonsQuery(languageId);
            ComparisonForUser comparisonForUser = comparisonsQuery.GetWithFullInfo(userLanguages, group);
            return comparisonForUser;
        }

        [HttpPost]
        public EmptyResult NewVisitor(long id) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();

            var ratingByIpsQuery = new RatingByIpsQuery(languageId);
            ratingByIpsQuery.AddNewVisitor(RemoteClientHelper.GetClientIpAddress(Request), id, RatingPageType.Comparison);
            return new EmptyResult();
        }
    }
}