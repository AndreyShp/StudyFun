using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public abstract class BaseTranslatorController : Controller {
        protected abstract string ControllerName { get; }
        protected abstract WordType WordType { get; }
        protected abstract SectionId SectionId { get; }

        [UserLanguages]
        public ActionResult Index(UserLanguages userLanguages) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }
            return View(new TranslatorModel(userLanguages));
        }

        /// <summary>
        /// Ищет слова попавшие под введенную маску
        /// </summary>
        /// <param name="userLanguages">языки пользователя</param>
        /// <param name="query">маска введенная пользователем</param>
        /// <returns>список слов подходящих под маску, с переводами</returns>
        [UserLanguages]
        public JsonResult Search(UserLanguages userLanguages, string query) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return JsonResultHelper.Error();
            }
            WordsByPattern result;
            if (!string.IsNullOrEmpty(query)) {
                var wordsQuery = new WordsQuery();
                result = wordsQuery.GetLikeWords(userLanguages, query, WordType);
                if (EnumerableValidator.IsEmpty(result.Words)) {
                    //попробовать перевести символы из латинских в русские, из русских в латинские
                    result = GetKeyboardLayoutResult(userLanguages, query, wordsQuery);
                }
            } else {
                result = new WordsByPattern();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private WordsByPattern GetKeyboardLayoutResult(UserLanguages userLanguages, string query, WordsQuery wordsQuery) {
            var keyboardLayoutConverter = new KeyboardLayoutConverter();
            List<string> convertedQueries = keyboardLayoutConverter.Convert(query);
            foreach (string convertedQuery in convertedQueries) {
                WordsByPattern result = wordsQuery.GetLikeWords(userLanguages, convertedQuery, WordType);
                if (EnumerableValidator.IsNotEmpty(result.Words)) {
                    result.NewPattern = convertedQuery;
                    return result;
                }
            }
            return new WordsByPattern();
        }

        /// <summary>
        /// Возвращает перевод для слова
        /// </summary>
        /// <param name="userLanguages">языки пользователя</param>
        /// <param name="query">слово, которое нужно перевести</param>
        /// <returns>список слов, которые переводят</returns>
        [UserLanguages]
        public JsonResult GetTranslations(UserLanguages userLanguages, string query) {
            if (UserLanguages.IsInvalid(userLanguages)) {
                return JsonResultHelper.Error();
            }
            List<PronunciationForUser> result = GetTranslationsByQuery(userLanguages, query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<PronunciationForUser> GetTranslationsByQuery(UserLanguages userLanguages, string query) {
            var wordsQuery = new WordsQuery();
            return wordsQuery.GetTranslations(userLanguages, query, WordType);
        }

        /// <summary>
        /// Возвращает перевод для слова
        /// </summary>
        /// <param name="sourceLanguage">язык, на котором введено слово</param>
        /// <param name="destinationLanguage">язык, на который нужно перевести слово</param>
        /// <param name="word">слово, которое нужно перевести</param>
        /// <returns>переведенное слово</returns>
        public ActionResult Translation(string sourceLanguage, string destinationLanguage, string word) {
            const string ACTION_NAME = "Index";

            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            var languages = new LanguagesQuery(WebSettingsConfig.Instance.DefaultLanguageFrom,
                                               WebSettingsConfig.Instance.DefaultLanguageTo);

            LanguageShortName source = LanguagesQuery.ParseShortName(sourceLanguage);
            LanguageShortName destination = LanguagesQuery.ParseShortName(destinationLanguage);
            UserLanguages userLanguages = languages.GetLanguagesByShortNames(source, destination);
            if (UserLanguages.IsInvalid(userLanguages) || string.IsNullOrEmpty(word)) {
                return RedirectToAction(ACTION_NAME, ControllerName);
            }

            List<PronunciationForUser> translationWords = GetTranslationsByQuery(userLanguages, word);
            return View(ACTION_NAME, new TranslatorModel(userLanguages, word, translationWords));
        }
    }
}