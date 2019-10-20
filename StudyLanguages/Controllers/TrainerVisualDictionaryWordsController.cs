using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.ExternalData;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models;
using StudyLanguages.Models.Trainer;

namespace StudyLanguages.Controllers {
    public class TrainerVisualDictionaryWordsController : BaseGroupItemsTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.WordTranslation; }
        }

        protected override KnowledgeSourceType KnowledgeSourceType {
            get { return KnowledgeSourceType.VisualDictionary; }
        }

        protected override SectionId SectionId {
            get { return SectionId.VisualDictionary; }
        }

        [UserId(true)]
        [UserLanguages]
        public ActionResult Index(long userId, UserLanguages userLanguages, string group) {
            long representationId = GetRepresentationId(group);
            return GetIndex(userId, userLanguages, representationId, model => SetModel(group, model));
        }

        private void SetModel(string groupName, TrainerModel model) {
            var pageRequiredData = new PageRequiredData(SectionId.VisualDictionary);
            pageRequiredData.Title = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Title, groupName);
            pageRequiredData.Keywords = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Keywords, groupName);
            pageRequiredData.Description = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Description, groupName);

            model.PageRequiredData = pageRequiredData;
            model.BreadcrumbsItems = BreadcrumbsHelper.GetVisualDictionary(Url, groupName, pageRequiredData.Title);
            model.SetMarkUrl = Url.Action("SetMark", RouteConfig.USER_TRAINER_VISUAL_WORDS_CONTROLLER,
                                          new {group = groupName + "/"}, Request.Url.Scheme);
        }

        [UserId]
        [UserLanguages]
        [HttpPost]
        public JsonResult SetMark(long userId,
                                  UserLanguages userLanguages,
                                  string group,
                                  KnowledgeMark mark,
                                  TrainerItem item) {
            long representationId = GetRepresentationId(group);
            return SetMarkAndGetModel(userId, userLanguages, representationId, mark, item);
        }

        protected override RedirectToRouteResult RedirectToParentPage() {
            return RedirectToAction("Index", RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME);
        }

        protected override IUserRepetitionKnowledgeQuery CreateRepetitionQuery(long userId,
                                                                               long languageId,
                                                                               long representationId) {
            return new UserRepetitionVisualWordsQuery(userId, languageId, representationId);
        }

        private long GetRepresentationId(string group) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var representationsQuery = new RepresentationsQuery(languageId);
            return representationsQuery.GetId(group);
        }
    }
}