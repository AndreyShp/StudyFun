using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models.Knowledge;

namespace StudyLanguages.Controllers {
    public class KnowledgeGeneratorController : Controller {
        [UserId(true)]
        public ActionResult Index(long userId) {
            if (IsInvalid(userId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            GeneratorModel generatorModel = GenerateItems(userId);
            return View("../Knowledge/Generator", generatorModel);
        }

        private GeneratorModel GenerateItems(long userId) {
            long languageFromId = WebSettingsConfig.Instance.GetLanguageFromId();
            long languageToId = WebSettingsConfig.Instance.GetLanguageToId();
            var knowledgeGeneratorQuery = new KnowledgeGeneratorQuery(userId, languageFromId, languageToId);
            Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>> generatedItems =
                knowledgeGeneratorQuery.Generate(new Dictionary<KnowledgeDataType, int> {
                    {KnowledgeDataType.WordTranslation, 30},
                    {KnowledgeDataType.PhraseTranslation, 15},
                    {KnowledgeDataType.SentenceTranslation, 5}
                });

            string userKey = GetUserKey(userId);
            WriteItemsToTempData(userKey, generatedItems);

            var result = new GeneratorModel(ControllerContext, generatedItems);
            return result;
        }

        private void WriteItemsToTempData(string userKey,
                                          Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>> generatedItems) {
            ControllerContext.Controller.TempData[userKey] = generatedItems;
        }

        [UserId]
        public ActionResult Download(long userId, DocumentType type) {
            if (IsInvalid(userId)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "KnowledgeGeneratorController.Download передан некорректный идентификатор пользователя: {0}",
                    userId);
                return RedirectToAction("Index");
            }
            string userKey = GetUserKey(userId);
            var generatedItems =
                ControllerContext.Controller.TempData[userKey] as
                Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>>;
            if (EnumerableValidator.IsNullOrEmpty(generatedItems)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "KnowledgeGeneratorController.Download для пользователя с идентификатором {0} не удалось найти сгенерированные данные во временных данных",
                    userId);
                return RedirectToAction("Index");
            }

            //записать данные опять, т.к. они удаляются после считывания
            WriteItemsToTempData(userKey, generatedItems);

            string header = WebSettingsConfig.Instance.GetTemplateText(SectionId.KnowledgeGenerator, TemplateId.Header);

            var downloader = new GeneratedKnowledgeDownloader(WebSettingsConfig.Instance.DomainWithProtocol,
                                                              CommonConstants.GetFontPath(Server)) { Header = header };
            var documentGenerator = downloader.Download(type, header, generatedItems);
            
            return File(documentGenerator.Generate(), documentGenerator.ContentType, documentGenerator.FileName);
        }

        [UserId]
        public JsonResult Generate(long userId) {
            if (IsInvalid(userId)) {
                return JsonResultHelper.Error();
            }

            GeneratorModel generatorModel = GenerateItems(userId);
            return JsonResultHelper.GetUnlimitedJsonResult(generatorModel.HtmlItems);
        }

        private static bool IsInvalid(long userId) {
            return WebSettingsConfig.Instance.IsSectionForbidden(SectionId.KnowledgeGenerator)
                   || IdValidator.IsInvalid(userId);
        }

        private static string GetUserKey(long userId) {
            return "generatedInfo_" + userId;
        }
    }
}