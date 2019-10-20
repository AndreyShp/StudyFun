using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models;

namespace StudyLanguages.Controllers {
    public class PopularWordsController : BaseController {
        [UserLanguages]
        public ActionResult Index(UserLanguages userLanguages) {
            if (IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            List<SourceWithTranslation> words = GetWords(userLanguages);

            var model = new PopularItemsModel(userLanguages, words) {
                SpeakerDataType = SpeakerDataType.Word,
                KnowledgeDataType = KnowledgeDataType.WordTranslation,
                ControllerName = RouteConfig.POPULAR_WORDS_CONTROLLER,
                TableHeader = "Слово",
                LowerManyElems = "слова",
                LowerOneElem = "слов"
            };
            return View(model);
        }

        private static List<SourceWithTranslation> GetWords(UserLanguages userLanguages) {
            IPopularWordsQuery popularWordsQuery = GetPopularWordsQuery();
            List<SourceWithTranslation> words = popularWordsQuery.GetWordsByType(userLanguages, PopularWordType.Minileks);
            return words;
        }

        [UserLanguages]
        public ActionResult Download(UserLanguages userLanguages, DocumentType type) {
            if (IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            List<SourceWithTranslation> words = GetWords(userLanguages);

            string header = WebSettingsConfig.Instance.GetTemplateText(SectionId.PopularWord, PageId.Index,
                                                                       TemplateId.Header);

            var downloader = new PopularWordsDownloader(WebSettingsConfig.Instance.DomainWithProtocol,
                                                        CommonConstants.GetFontPath(Server)) {
                                                            Header = header
                                                        };
            DocumentationGenerator documentGenerator = downloader.Download(type, header, words);

            return File(documentGenerator.Generate(), documentGenerator.ContentType, documentGenerator.FileName);
        }

        private static bool IsInvalid(UserLanguages userLanguages) {
            return UserLanguages.IsInvalid(userLanguages)
                   || WebSettingsConfig.Instance.IsSectionForbidden(SectionId.PopularWord);
        }

        private static IPopularWordsQuery GetPopularWordsQuery() {
            IPopularWordsQuery popularWordsQuery = new PopularWordsQuery();
            return popularWordsQuery;
        }
    }
}