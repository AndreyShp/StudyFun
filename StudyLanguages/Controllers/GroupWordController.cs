using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models;
using StudyLanguages.Models.Groups;

namespace StudyLanguages.Controllers {
    public class GroupWordController : BaseInnerGroupController {
        //
        // GET: /GroupWord/
        protected override RatingPageType RatingPageType {
            get { return RatingPageType.Word; }
        }

        protected override SectionId SectionId {
            get { return SectionId.GroupByWords; }
        }

        protected override string TableHeader {
            get { return "Слово"; }
        }

        [UserLanguages]
        [SelectedGroup(GroupType.ByWord)]
        public ActionResult Index(UserLanguages userLanguages, GroupForUser group) {
            return ShowAll(userLanguages, group, CrossReferenceType.GroupWord);
        }

        protected override RedirectToRouteResult GetRedirectToGroups() {
            return RedirectToActionPermanent("Index", RouteConfig.GROUPS_BY_WORDS_CONTROLLER);
        }

        public ActionResult AddNew() {
            var model = new GroupInfo {
                SectionId = SectionId.GroupByWords,
                ControllerName = RouteConfig.GROUP_WORD_CONTROLLER,
                BaseControllerName = RouteConfig.GROUPS_BY_WORDS_CONTROLLER,
            };
            return GetNew(model);
        }

        [HttpGet]
        [Cache]
        public ActionResult Image(long id) {
            return GetImage(new WordTranslationsQuery(), id);
        }

        [HttpGet]
        [SelectedGroup(GroupType.ByWord)]
        [Cache]
        public ActionResult GetImageByName(GroupForUser group) {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var groupsQuery = new GroupsQuery(languageId);
            return GetImage(group != null ? group.Name : null, n => groupsQuery.GetImage(n, GroupType.ByWord));
        }

        [UserLanguages]
        [SelectedGroup(GroupType.ByWord)]
        public ActionResult Trainer(UserLanguages userLanguages, GroupForUser group) {
            return GetTrainer(userLanguages, group);
        }

        [UserLanguages]
        [SelectedGroup(GroupType.ByWord)]
        public ActionResult ShowSpecialItem(UserLanguages userLanguages, GroupForUser group, string elem1, string elem2) {
            if (UserLanguages.IsInvalid(userLanguages) || group == null || string.IsNullOrWhiteSpace(elem1)
                || string.IsNullOrWhiteSpace(elem2)) {
                return GetRedirectToGroups();
            }
            return ShowSpecial(userLanguages, group, model => FoundTranslation(elem1, elem2, model));
        }

        [SelectedGroup(GroupType.ByWord)]
        public ActionResult GapsTrainer(GroupForUser group) {
            return GetGapsTrainerView(group, model => {
                model.LoadNextBtnCaption = "Показать другие слова";
                model.SpeakerDataType = SpeakerDataType.Word;
                model.BreadcrumbsItems = BreadcrumbsHelper.GetWords(Url, group.Name, CommonConstants.FILL_GAPS);
            });
        }

        protected override List<SourceWithTranslation> GetSourceWithTranslations(UserLanguages userLanguages,
                                                                                 GroupForUser group) {
            var groupWordsQuery = new GroupWordsQuery();
            List<SourceWithTranslation> wordsWithTranslations = groupWordsQuery.GetWordsByGroup(userLanguages,
                                                                                                group.Id);
            return wordsWithTranslations;
        }

        [UserLanguages]
        [SelectedGroup(GroupType.ByWord)]
        public ActionResult Download(UserLanguages userLanguages, GroupForUser group, DocumentType type) {
            string fileName = string.Format("Слова на тему {0}", group.LowerName.ToLowerInvariant());
            return GetFile(userLanguages, group, type, fileName);
        }

        protected SourceWithTranslation FoundTranslation(string item1,
                                                         string item2,
                                                         GroupModel model) {
            SourceWithTranslation foundTranslation =
                model.ElemsWithTranslations.FirstOrDefault(
                    e => IsWordsEquals(item1, item2, e) || IsWordsEquals(item2, item1, e));
            return foundTranslation;
        }

        private static bool IsWordsEquals(string word1, string word2, SourceWithTranslation e) {
            return IsWordsEquals(word1, e.Source) && IsWordsEquals(word2, e.Translation);
        }

        private static bool IsWordsEquals(string searchWord, PronunciationForUser word) {
            return searchWord.Equals(word.Text,
                                     StringComparison.InvariantCultureIgnoreCase);
        }

        protected override GroupModel GetModel(UserLanguages userLanguages,
                                               GroupForUser group) {
            List<SourceWithTranslation> wordsWithTranslations = GetSourceWithTranslations(userLanguages, group);

            var options = new GroupModelOptions(new Dictionary<LinkId, string> {
                {LinkId.First, "Первое слово"},
                {LinkId.Prev, "Предыдущее слово"},
                {LinkId.Next, "Следующее слово"},
                {LinkId.Last, "Последнее слово"}
            },
                                                (patternUrl, elem) =>
                                                string.Format(patternUrl,
                                                              UrlBuilder.EncodePartUrl(
                                                                  elem.Source.Text),
                                                              UrlBuilder.EncodePartUrl(
                                                                  elem.Translation.Text)));
            var model = new GroupModel(group, SpeakerDataType.Word, KnowledgeDataType.WordTranslation, options,
                                       userLanguages, wordsWithTranslations);
            return model;
        }
    }
}