using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models;
using StudyLanguages.Models.Groups;

namespace StudyLanguages.Controllers {
    public class GroupSentenceController : BaseInnerGroupController {
        protected override RatingPageType RatingPageType {
            get { return RatingPageType.Sentence; }
        }

        protected override SectionId SectionId {
            get { return SectionId.GroupByPhrases; }
        }

        /*[UserLanguages]
        [SelectedGroup(GroupType.BySentence)]
        public ActionResult Index(UserLanguages userLanguages, GroupForUser group)
        {
            return ShowAll(userLanguages, group, CrossReferenceType.GroupSentence);
        }*/

        protected override string TableHeader {
            get { return "Фраза"; }
        }

        [UserLanguages]
        [SelectedGroup(GroupType.BySentence)]
        public ActionResult Index(UserLanguages userLanguages, GroupForUser group) {
            return ShowAll(userLanguages, group, CrossReferenceType.GroupSentence);
        }

        protected override RedirectToRouteResult GetRedirectToGroups() {
            return RedirectToActionPermanent("Index", RouteConfig.GROUPS_BY_SENTENCES_CONTROLLER);
        }

        public ActionResult AddNew() {
            var model = new GroupInfo {
                SectionId = SectionId.GroupByPhrases,
                ControllerName = RouteConfig.GROUP_SENTENCE_CONTROLLER,
                BaseControllerName = RouteConfig.GROUPS_BY_SENTENCES_CONTROLLER,
            };
            return GetNew(model);
        }

        [HttpGet]
        [Cache]
        public ActionResult Image(long id) {
            return GetImage(new SentencesQuery(), id);
        }

        [HttpGet]
        [SelectedGroup(GroupType.BySentence)]
        [Cache]
        public ActionResult GetImageByName(GroupForUser group) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var groupsQuery = new GroupsQuery(languageId);
            return GetImage(group != null ? group.Name : null, n => groupsQuery.GetImage(n, GroupType.BySentence));
        }

        [UserLanguages]
        [SelectedGroup(GroupType.BySentence)]
        public ActionResult Download(UserLanguages userLanguages, GroupForUser group, DocumentType type) {
            string fileName = string.Format("Фразы на тему {0}", group.LowerName.ToLowerInvariant());
            return GetFile(userLanguages, group, type, fileName);
        }

        [UserLanguages]
        [SelectedGroup(GroupType.BySentence)]
        public ActionResult Trainer(UserLanguages userLanguages, GroupForUser group) {
            return GetTrainer(userLanguages, group);
        }

        [SelectedGroup(GroupType.BySentence)]
        public ActionResult GapsTrainer(GroupForUser group) {
            return GetGapsTrainerView(group, model => {
                model.LoadNextBtnCaption = "Показать другие фразы";
                model.SpeakerDataType = SpeakerDataType.Sentence;
                model.BreadcrumbsItems = BreadcrumbsHelper.GetPhrases(Url, group.Name, CommonConstants.FILL_GAPS);
            });
        }

        protected override List<SourceWithTranslation> GetSourceWithTranslations(UserLanguages userLanguages,
                                                                                 GroupForUser group) {
            var groupSentencesQuery = new GroupSentencesQuery();
            List<SourceWithTranslation> result = groupSentencesQuery.GetSentencesByGroup(userLanguages, group.Id);
            return result;
        }

        /*[UserLanguages]
        [SelectedGroup(GroupType.BySentence)]
        public ActionResult Examples(UserLanguages userLanguages, GroupForUser group) {
            return GetExamples(userLanguages, group);
        }*/

        [UserLanguages]
        [SelectedGroup(GroupType.BySentence)]
        public ActionResult ShowSpecialItem(UserLanguages userLanguages,
                                            GroupForUser group,
                                            long elem1,
                                            long elem2) {
            if (UserLanguages.IsInvalid(userLanguages) || group == null || IdValidator.IsInvalid(elem1)
                || IdValidator.IsInvalid(elem2)) {
                return GetRedirectToGroups();
            }
            return ShowSpecial(userLanguages, group, model => FoundTranslation(elem1, elem2, model));
        }

        private static bool IsSentencesEquals(long id1, long id2, SourceWithTranslation e) {
            return IsSentencesEquals(id1, e.Source) && IsSentencesEquals(id2, e.Translation);
        }

        private static bool IsSentencesEquals(long searchId, PronunciationForUser sentence) {
            return searchId == sentence.Id;
        }

        protected SourceWithTranslation FoundTranslation(long item1,
                                                         long item2,
                                                         GroupModel model) {
            SourceWithTranslation foundTranslation =
                model.ElemsWithTranslations.FirstOrDefault(
                    e => IsSentencesEquals(item1, item2, e) || IsSentencesEquals(item2, item1, e));
            return foundTranslation;
        }

        protected override GroupModel GetModel(UserLanguages userLanguages,
                                               GroupForUser group) {
            List<SourceWithTranslation> sentencesWithTranslations = GetSourceWithTranslations(userLanguages, group);

            var options = new GroupModelOptions(new Dictionary<LinkId, string> {
                {LinkId.First, "Первая фраза"},
                {LinkId.Prev, "Предыдущая фраза"},
                {LinkId.Next, "Следующая фраза"},
                {LinkId.Last, "Последняя фраза"}
            }, (patternUrl, elem) => string.Format(patternUrl, elem.Source.Id, elem.Translation.Id)) {
                SourceTextGetter = elem => elem.Source.Text,
                TranslationTextGetter = elem => elem.Translation.Text,
            };

            var model = new GroupModel(group, SpeakerDataType.Sentence, KnowledgeDataType.PhraseTranslation, options,
                                       userLanguages,
                                       sentencesWithTranslations);
            return model;
        }
    }
}