using System;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class BaseAudioWordsController : BaseController {
        private readonly ShuffleWordsQuery _query;
        private readonly ShuffleController _shuffleController;
        private readonly UserLanguages _userLanguages;

        public BaseAudioWordsController(WordType wordType) {
            _query = new ShuffleWordsQuery(wordType, ShuffleType.Audio);
            _shuffleController = new ShuffleController(_query, "Index");
            _userLanguages = GetUserLanguages();
        }

        [UserId(true)]
        public ActionResult Index(string userUniqueId) {
            return _shuffleController.Index(userUniqueId, _userLanguages);
        }

        [HttpPost]
        [UserId]
        public JsonResult MarkAsShowed(string userUniqueId, long id) {
            return _shuffleController.MarkAsShowed(userUniqueId, id, _userLanguages);
        }

        [HttpGet]
        [UserId]
        public JsonResult GetPrevPortion(string userUniqueId, long id) {
            return _shuffleController.GetPortion(userUniqueId, id,
                                                 userUnique =>
                                                 _query.GetPrevPortion(userUnique, id, _userLanguages));
        }

        [HttpGet]
        [UserId]
        public JsonResult GetNextPortion(string userUniqueId, long id) {
            return _shuffleController.GetPortion(userUniqueId, id,
                                                 userUnique =>
                                                 _query.GetNextPortion(userUnique, id, _userLanguages));
        }

        [HttpGet]
        [UserId]
        public ActionResult Translation(string userUniqueId, long? sourceId, long? translationId) {
            return _shuffleController.Translation(userUniqueId, sourceId, translationId);
        }

        [HttpGet]
        [UserId]
        public ActionResult Reset(string userUniqueId) {
            _shuffleController.DeleteUserHistory(userUniqueId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Image(long id) {
            if (IdValidator.IsInvalid(id)) {
                return new EmptyResult();
            }
            return GetImage(IoCModule.Create<WordTranslationsQuery>(), id);
        }

        [HttpPost]
        public JsonResult Check(string userUniqueId, string textToCheck, long sourceId) {
            if (IdValidator.IsInvalid(sourceId) || string.IsNullOrWhiteSpace(textToCheck)) {
                return JsonResultHelper.Error();
            }
            var wordsQuery = IoCModule.Create<WordsQuery>();
            var word = wordsQuery.GetById(sourceId) as Word;
            if (word == null) {
                return JsonResultHelper.Error();
            }
            textToCheck = textToCheck.Trim();
            bool isEquals = string.Equals(word.Text, textToCheck, StringComparison.InvariantCultureIgnoreCase);
            return JsonResultHelper.Success(isEquals);
        }

        private static UserLanguages GetUserLanguages() {
            var languagesQuery = IoCModule.Create<LanguagesQuery>();
            UserLanguages userLanguages = languagesQuery.GetLanguagesByShortNames(LanguagesShortNames.EN,
                                                                                  LanguagesShortNames.RU);
            return userLanguages;
        }
    }
}