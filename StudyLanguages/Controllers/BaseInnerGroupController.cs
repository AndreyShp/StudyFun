using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Auxiliaries;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Auxiliaries;
using BusinessLogic.Helpers.SentencesSearchEngines;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Trainer;
using StudyLanguages.Models;
using StudyLanguages.Models.Groups;

namespace StudyLanguages.Controllers {
    public abstract class BaseInnerGroupController : BaseController {
        protected abstract RatingPageType RatingPageType { get; }
        protected abstract SectionId SectionId { get; }
        protected abstract string TableHeader { get; }

        public ActionResult GetTrainer(UserLanguages userLanguages, GroupForUser group) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages) || group == null) {
                return GetRedirectToGroups();
            }
            GroupModel model = GetModel(userLanguages, group);
            if (model.IsInvalid()) {
                return GetRedirectToGroups();
            }
            model.SetCurrent(model.ElemsWithTranslations[0]);
            return View("Index", model);
        }

        public ActionResult GetGapsTrainerView(GroupForUser group, Action<GapsTrainerModel> filler) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            UserLanguages userLanguages = WebSettingsConfig.Instance.DefaultUserLanguages;
            List<SourceWithTranslation> sourceWithTranslation = GetSourceWithTranslations(userLanguages, group);

            var gapsTrainerHelper = new GapsTrainerHelper();
            List<GapsTrainerItem> items = gapsTrainerHelper.ConvertToItems(sourceWithTranslation);

            var pageRequiredData = new PageRequiredData(SectionId, PageId.GapsTrainer, group.Name);
            var model = new GapsTrainerModel(pageRequiredData, items);
            filler(model);
            return View("GapsTrainer", model);
        }

        protected abstract List<SourceWithTranslation> GetSourceWithTranslations(UserLanguages userLanguages,
                                                                                 GroupForUser group);

        public ActionResult GetFile(UserLanguages userLanguages,
                                    GroupForUser group, DocumentType docType, string fileName) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages) || group == null) {
                return GetRedirectToGroups();
            }
            GroupModel model = GetModel(userLanguages, group);
            if (model.IsInvalid()) {
                return GetRedirectToGroups();
            }

            string header = WebSettingsConfig.Instance.GetTemplateText(SectionId, PageId.Detail, TemplateId.Header,
                                                                       group.Name);
            header = HttpUtility.HtmlDecode(header);

            var downloader = new GroupDataDownloader(WebSettingsConfig.Instance.DomainWithProtocol, CommonConstants.GetFontPath(Server)) {
                 Header = header, TableHeader = TableHeader
            };
            var documentGenerator = downloader.Download(docType, fileName, model.ElemsWithTranslations);

            Stream stream = documentGenerator.Generate();
            return File(stream, documentGenerator.ContentType, documentGenerator.FileName);
        }

        protected abstract RedirectToRouteResult GetRedirectToGroups();

        public ActionResult ShowAll(UserLanguages userLanguages,
                                    GroupForUser group,
                                    CrossReferenceType crossReferenceType) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages) || group == null) {
                return GetRedirectToGroups();
            }
            GroupModel model = GetModel(userLanguages, group);
            if (model.IsInvalid()) {
                return GetRedirectToGroups();
            }

            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var crossReferencesQuery = new CrossReferencesQuery(languageId);
            List<CrossReference> crossReferences = crossReferencesQuery.GetReferences(group.Id, crossReferenceType);
            model.CrossReferencesModel = new CrossReferencesModel(group.Name, crossReferenceType, crossReferences);
            return View("All", model);
        }

        protected ActionResult GetNew(GroupInfo groupInfo) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            return View("AddNewGroup", groupInfo);
        }

        /*public JsonResult GetExamples(UserLanguages userLanguages,
                                      GroupForUser group) {
            if (userLanguages == null || group == null || UserLanguages.IsInvalid(userLanguages)) {
                return JsonResultHelper.Error();
            }
            GroupModel model = GetModel(userLanguages, group);
            if (model.IsInvalid()) {
                return JsonResultHelper.Error();
            }

            var resultExamples = new List<Tuple<long, List<string>>>();
            var searchEngineFactory = new SentencesSearchEngineFactory();
            ISentencesSearchEngine fromSentencesSearchEngine = searchEngineFactory.Create(userLanguages.From.Id);
            ISentencesSearchEngine toSentencesSearchEngine = searchEngineFactory.Create(userLanguages.To.Id);

            foreach (SourceWithTranslation elem in model.ElemsWithTranslations) {
                model.SetCurrent(elem);

                AddExample(fromSentencesSearchEngine, elem.Source, resultExamples);
                AddExample(toSentencesSearchEngine, elem.Translation, resultExamples);
            }
            return JsonResultHelper.GetUnlimitedJsonResult(resultExamples);
        }*/

        /*protected JsonResult GetKnowledgeInfo(long userId, UserLanguages userLanguages, GroupForUser group) {
            if (IdValidator.IsInvalid(userId) || group == null) {
                return JsonResultHelper.Error();
            }
            GroupModel model = GetModel(userLanguages, group, GetPatternsForAll(group));
            if (model.IsInvalid()) {
                return JsonResultHelper.Error();
            }
            var knowledgeQuery = IoCModule.Create<UserKnowledgeQuery>();
            var statuses = //knowledgeQuery.GetStatusesByDataIds(model.ElemsWithTranslations.Select(e => e.Id).ToList());
            return GetUnlimitedJsonResult(statuses);
        }*/

        public void AddExample(ISentencesSearchEngine sentencesSearcher,
                               PronunciationForUser entity,
                               List<Tuple<long, List<string>>> result) {
            List<string> examples = sentencesSearcher.FindSentences(entity.Text, OrderWordsInSearch.ExactWordForWord);
            if (EnumerableValidator.IsEmpty(examples)) {
                return;
            }

            result.Add(new Tuple<long, List<string>>(entity.Id, examples));
        }

        public ActionResult ShowSpecial(UserLanguages userLanguages,
                                        GroupForUser group,
                                        Func<GroupModel, SourceWithTranslation> foundGetterByModel) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            GroupModel model = GetModel(userLanguages, group);
            if (model.IsInvalid()) {
                return GetRedirectToGroups();
            }
            SourceWithTranslation foundTranslation = foundGetterByModel(model);
            if (foundTranslation == null) {
                //искомые слова не найдены
                return GetRedirectToGroups();
            }
            //искомый элемент найден - пометить как текущее
            model.SetCurrent(foundTranslation);
            return View("Index", model);
        }

        protected abstract GroupModel GetModel(UserLanguages userLanguages,
                                               GroupForUser group);

        [HttpPost]
        public EmptyResult NewVisitor(long id) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var ratingByIpsQuery = new RatingByIpsQuery(languageId);
            ratingByIpsQuery.AddNewVisitor(RemoteClientHelper.GetClientIpAddress(Request), id, RatingPageType);
            return new EmptyResult();
        }
    }
}