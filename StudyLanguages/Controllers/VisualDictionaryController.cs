using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Auxiliaries;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Auxiliaries;
using BusinessLogic.ExternalData.Representations;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Trainer;
using StudyLanguages.Models;
using StudyLanguages.Models.Groups;

namespace StudyLanguages.Controllers {
    public class VisualDictionaryController : BaseController {
        //
        // GET: /VisualDictionary/
        [UserLanguages]
        public ActionResult Index(UserLanguages userLanguages, string group) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.VisualDictionary)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME);
            }
            RepresentationForUser representationForUser = GetRepresentationForUser(userLanguages, group);
            if (representationForUser == null) {
                return RedirectToActionPermanent("Index", RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME);
            }

            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var crossReferencesQuery = new CrossReferencesQuery(languageId);
            List<CrossReference> crossReferences = crossReferencesQuery.GetReferences(representationForUser.Id,
                                                                                      CrossReferenceType.
                                                                                          VisualDictionary);
            var model = new VisualDictionaryModel(userLanguages, representationForUser);
            model.CrossReferencesModel = new CrossReferencesModel(representationForUser.Title,
                                                                  CrossReferenceType.VisualDictionary, crossReferences);
            return View(model);
        }

        public ActionResult GapsTrainer(string group) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.VisualDictionary)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            UserLanguages userLanguages = WebSettingsConfig.Instance.DefaultUserLanguages;
            RepresentationForUser representationForUser = GetRepresentationForUser(userLanguages, group);

            var gapsTrainerHelper = new GapsTrainerHelper();
            List<GapsTrainerItem> items = gapsTrainerHelper.ConvertToItems(representationForUser.Areas);

            var pageRequiredData = new PageRequiredData(SectionId.VisualDictionary, PageId.GapsTrainer, group);
            var model = new GapsTrainerModel(pageRequiredData, items);
            model.LoadNextBtnCaption = "Показать другие слова";
            model.SpeakerDataType = SpeakerDataType.Word;
            model.BreadcrumbsItems = BreadcrumbsHelper.GetVisualDictionary(Url, group, CommonConstants.FILL_GAPS);
            return View("GapsTrainer", model);
        }

        [UserLanguages]
        public ActionResult Download(UserLanguages userLanguages, string group, DocumentType type) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME);
            }
            RepresentationForUser representationForUser = GetRepresentationForUser(userLanguages, group);
            if (representationForUser == null) {
                return RedirectToActionPermanent("Index", RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME);
            }

            var downloader = new VisualDictionaryDownloader(WebSettingsConfig.Instance.DomainWithProtocol,
                                                            CommonConstants.GetFontPath(Server));
            string fileName = string.Format("Визуальный словарь на тему {0}",
                                            representationForUser.Title.ToLowerInvariant());
            DocumentationGenerator documentGenerator = downloader.Download(type, fileName, representationForUser);

            return File(documentGenerator.Generate(), documentGenerator.ContentType, documentGenerator.FileName);
        }

        [HttpPost]
        public EmptyResult NewVisitor(long id) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var ratingByIpsQuery = new RatingByIpsQuery(languageId);
            ratingByIpsQuery.AddNewVisitor(RemoteClientHelper.GetClientIpAddress(Request), id,
                                           RatingPageType.VisualDictionary);
            return new EmptyResult();
        }

        private static RepresentationForUser GetRepresentationForUser(UserLanguages userLanguages, string group) {
            if (string.IsNullOrEmpty(group)) {
                return null;
            }
            RepresentationsQuery representationsQuery = GetRepresentationsQuery();
            RepresentationForUser representationForUser = representationsQuery.GetWithAreas(userLanguages, group);
            return representationForUser;
        }

        private static RepresentationsQuery GetRepresentationsQuery() {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var representationsQuery = new RepresentationsQuery(languageId);
            return representationsQuery;
        }

        [HttpGet]
        [Cache]
        public ActionResult GetImageByName(string group, bool big = false) {
            const int MAX_IMAGE_HEIGHT = 150;
            RepresentationsQuery representationsQuery = GetRepresentationsQuery();
            return GetImage(group, representationsQuery.GetImage,
                            image => big ? image : ImageUtilities.ResizeImage(image, MAX_IMAGE_HEIGHT));
        }

        public JsonResult Preview(string group) {
            UserLanguages userLanguages = WebSettingsConfig.Instance.DefaultUserLanguages;
            RepresentationForUser representationForUser = GetRepresentationForUser(userLanguages, group);
            if (representationForUser == null) {
                return null;
            }
            return JsonResultHelper.GetUnlimitedJsonResult(representationForUser.Areas);
        }

        public ActionResult AddNew() {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.VisualDictionary)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            return View("Index");
        }
    }
}