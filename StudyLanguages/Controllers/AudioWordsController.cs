using System;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class AudioWordsController : BaseController {
        private readonly ShuffleWordsQuery _query;
        private readonly ShuffleController _shuffleController;
        private readonly UserLanguages _userLanguages;

        public AudioWordsController() {
            _query = new ShuffleWordsQuery(WordType.Default, ShuffleType.Audio);
            _shuffleController = new ShuffleController(_query, "Index", SectionId.Audio);
            _userLanguages = GetUserLanguages();
        }

        [UserId(true)]
        public ActionResult Index(long userId) {
            if (UserLanguages.IsInvalid(_userLanguages)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }
            return _shuffleController.Index(userId, _userLanguages);
        }

        [HttpPost]
        [UserId]
        public JsonResult MarkAsShowed(long userId, long id) {
            if (UserLanguages.IsInvalid(_userLanguages)) {
                return JsonResultHelper.Error();
            }
            return _shuffleController.MarkAsShowed(userId, id, _userLanguages);
        }

        [HttpGet]
        [UserId]
        public JsonResult GetPrevPortion(long userId, long id) {
            if (UserLanguages.IsInvalid(_userLanguages)) {
                return JsonResultHelper.Error();
            }
            return _shuffleController.GetPortion(userId, id,
                                                 userUnique =>
                                                 _query.GetPrevPortion(userId, id, _userLanguages));
        }

        [HttpGet]
        [UserId]
        public JsonResult GetNextPortion(long userId, long id) {
            if (UserLanguages.IsInvalid(_userLanguages)) {
                return JsonResultHelper.Error();
            }
            return _shuffleController.GetPortion(userId, id,
                                                 uId =>
                                                 _query.GetNextPortion(uId, id, _userLanguages));
        }

        [HttpGet]
        [UserId]
        public ActionResult Translation(long userId, long? sourceId, long? translationId) {
            return _shuffleController.Translation(userId, sourceId, translationId);
        }

        [HttpGet]
        [UserId]
        public ActionResult Reset(long userId) {
            _shuffleController.DeleteUserHistory(userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Cache]
        public ActionResult Image(long id) {
            if (IdValidator.IsInvalid(id)) {
                return new EmptyResult();
            }
            return GetImage(new WordTranslationsQuery(), id);
        }

        [HttpPost]
        [UserId]
        public JsonResult Check(long userId, string textToCheck, long sourceId) {
            if (IdValidator.IsInvalid(sourceId) || string.IsNullOrWhiteSpace(textToCheck)) {
                return JsonResultHelper.Error();
            }
            var wordsQuery = new WordsQuery();
            var word = wordsQuery.GetById(sourceId) as Word;
            if (word == null) {
                return JsonResultHelper.Error();
            }
            textToCheck = textToCheck.Trim();
            bool isEquals = string.Equals(word.Text, textToCheck, StringComparison.InvariantCultureIgnoreCase);
            return JsonResultHelper.Success(isEquals);
        }

        private static UserLanguages GetUserLanguages() {
            var languagesQuery = new LanguagesQuery(WebSettingsConfig.Instance.DefaultLanguageFrom,
                                                    WebSettingsConfig.Instance.DefaultLanguageTo);
            UserLanguages userLanguages = languagesQuery.GetLanguagesByShortNames(LanguageShortName.En,
                                                                                  LanguageShortName.Ru);
            return userLanguages;
        }
    }
}