using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models;
using StudyLanguages.Models.Main;
using StudyLanguages.Models.Trainer;

namespace StudyLanguages.Controllers {
    public class TrainerGroupWordsController : BaseGroupItemsTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.WordTranslation; }
        }

        protected override KnowledgeSourceType KnowledgeSourceType {
            get { return KnowledgeSourceType.GroupWord; }
        }

        protected override SectionId SectionId {
            get { return SectionId.GroupByWords; }
        }

        [UserId(true)]
        [SelectedGroup(GroupType.ByWord)]
        [UserLanguages]
        public ActionResult Index(long userId, GroupForUser group, UserLanguages userLanguages) {
            long groupId = GetGroupId(group);
            return GetIndex(userId, userLanguages, groupId, model => SetModel(group.Name, model));
        }

        private void SetModel(string groupName, TrainerModel model) {
            var pageRequiredData = new PageRequiredData(SectionId.GroupByWords);
            pageRequiredData.Title = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Title, groupName);
            pageRequiredData.Keywords = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Keywords, groupName);
            pageRequiredData.Description = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Description, groupName);

            model.PageRequiredData = pageRequiredData;
            model.BreadcrumbsItems = BreadcrumbsHelper.GetWords(Url, groupName, pageRequiredData.Title);
            model.SetMarkUrl = Url.Action("SetMark", RouteConfig.USER_TRAINER_GROUP_WORDS_CONTROLLER,
                                          new {group = groupName + "/"}, Request.Url.Scheme);
        }

        [UserId]
        [SelectedGroup(GroupType.ByWord)]
        [UserLanguages]
        [HttpPost]
        public JsonResult SetMark(long userId,
                                  GroupForUser group,
                                  UserLanguages userLanguages,
                                  KnowledgeMark mark,
                                  TrainerItem item) {
            long groupId = GetGroupId(group);
            return SetMarkAndGetModel(userId, userLanguages, groupId, mark, item);
        }

        protected override RedirectToRouteResult RedirectToParentPage() {
            return RedirectToAction("Index", RouteConfig.GROUPS_BY_WORDS_CONTROLLER);
        }

        protected override IUserRepetitionKnowledgeQuery CreateRepetitionQuery(long userId,
                                                                               long languageId,
                                                                               long representationId) {
            return new UserRepetitionGroupWordsQuery(userId, languageId, representationId);
        }

        private static long GetGroupId(GroupForUser group) {
            return group != null ? group.Id : IdValidator.INVALID_ID;
        }
    }
}