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
using StudyLanguages.Models.Trainer;

namespace StudyLanguages.Controllers {
    public class TrainerGroupPhrasesController : BaseGroupItemsTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.PhraseTranslation; }
        }

        protected override KnowledgeSourceType KnowledgeSourceType {
            get { return KnowledgeSourceType.GroupPhrase; }
        }

        protected override SectionId SectionId {
            get { return SectionId.GroupByPhrases; }
        }

        [UserId(true)]
        [SelectedGroup(GroupType.BySentence)]
        [UserLanguages]
        public ActionResult Index(long userId, GroupForUser group, UserLanguages userLanguages) {
            long groupId = GetGroupId(group);
            return GetIndex(userId, userLanguages, groupId, model => SetModel(group.Name, model));
        }

        private void SetModel(string groupName, TrainerModel model) {
            var pageRequiredData = new PageRequiredData(SectionId.GroupByPhrases);
            pageRequiredData.Title = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards,
                                                                                TemplateId.Title, groupName);
            pageRequiredData.Keywords = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId,
                                                                                   PageId.Cards, TemplateId.Keywords,
                                                                                   groupName);
            pageRequiredData.Description = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId,
                                                                                      PageId.Cards,
                                                                                      TemplateId.Description, groupName);

            model.PageRequiredData = pageRequiredData;
            model.BreadcrumbsItems = BreadcrumbsHelper.GetPhrases(Url, groupName, pageRequiredData.Title);
            model.SetMarkUrl = Url.Action("SetMark", RouteConfig.USER_TRAINER_GROUP_PHRASES_CONTROLLER,
                                          new {group = groupName + "/"}, Request.Url.Scheme);
        }

        [UserId]
        [SelectedGroup(GroupType.BySentence)]
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
            return RedirectToAction("Index", RouteConfig.GROUPS_BY_SENTENCES_CONTROLLER);
        }

        protected override IUserRepetitionKnowledgeQuery CreateRepetitionQuery(long userId,
                                                                               long languageId,
                                                                               long representationId) {
            return new UserRepetitionGroupPhrasesQuery(userId, languageId, representationId);
        }

        private static long GetGroupId(GroupForUser group) {
            return group != null ? group.Id : IdValidator.INVALID_ID;
        }
    }
}