using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.DataQuery;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;
using StudyLanguages.Models;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class ShuffleController : Controller {
        private readonly BaseRandomQuery _query;
        private readonly SectionId _sectionId;
        private readonly string _viewName;

        public ShuffleController(BaseRandomQuery query, string viewName, SectionId sectionId) {
            _query = query;
            _viewName = viewName;
            _sectionId = sectionId;
        }

        public ActionResult Index(long userId, UserLanguages userLanguages) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(_sectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }
            List<SourceWithTranslation> sentencesWithTranslations = IdValidator.IsValid(userId)
                                                                        ? _query.GetRandom(userId, userLanguages)
                                                                        : null;
            return View(_viewName, new ShuffleModel(userLanguages, sentencesWithTranslations));
        }

        public JsonResult MarkAsShowed(long userId, long id, UserLanguages userLanguages) {
            if (IsInvalid(id, userId) || UserLanguages.IsInvalid(userLanguages)) {
                return JsonResultHelper.Error();
            }
            bool result = _query.MarkAsShowed(userId, userLanguages, id);
            return JsonResultHelper.Success(result);
        }

        public JsonResult GetPrevPortion(long userId, long id, UserLanguages userLanguages) {
            return GetPortion(userId, id,
                              userUnique =>
                              _query.GetPrevPortion(userUnique, id, userLanguages));
        }

        public JsonResult GetNextPortion(long userId, long id, UserLanguages userLanguages) {
            return GetPortion(userId, id,
                              userUnique =>
                              _query.GetNextPortion(userUnique, id, userLanguages));
        }

        public ActionResult Translation(long userId, long? sourceId, long? translationId) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(_sectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (!sourceId.HasValue || !translationId.HasValue) {
                return RedirectToAction(_viewName);
            }
            List<SourceWithTranslation> sourceWithTranslations = _query.GetExact(userId,
                                                                                 sourceId.Value,
                                                                                 translationId.Value);
            if (sourceWithTranslations == null || sourceWithTranslations.Count == 0) {
                return RedirectToAction(_viewName);
            }
            var languages = new LanguagesQuery(WebSettingsConfig.Instance.DefaultLanguageFrom,
                                               WebSettingsConfig.Instance.DefaultLanguageTo);
            SourceWithTranslation currentSentence = sourceWithTranslations.Single(e => e.IsCurrent);
            UserLanguages userLanguages = languages.GetLanguages(new List<long> {
                currentSentence.Source.LanguageId,
                currentSentence.Translation.LanguageId
            });
            if (UserLanguages.IsInvalid(userLanguages)) {
                return RedirectToAction(_viewName);
            }
            return View(_viewName, new ShuffleModel(userLanguages, sourceWithTranslations));
        }

        public JsonResult GetPortion(long userId,
                                     long id,
                                     Func<long, List<SourceWithTranslation>>
                                         getter) {
            if (IsInvalid(id, userId)) {
                return Json(new {success = false});
            }

            List<SourceWithTranslation> sourceWithTranslations = getter(userId);
            //TODO: возможно увеличить максимальную длину ответа
            return Json(sourceWithTranslations, JsonRequestBehavior.AllowGet);
        }

        public void DeleteUserHistory(long userId) {
            if (IdValidator.IsInvalid(userId)) {
                return;
            }
            if (!_query.DeleteUserHistory(userId)) {
                //TODO: логировать
            }
        }

        private static bool IsInvalid(long id, long userId) {
            return IdValidator.IsInvalid(id) || IdValidator.IsInvalid(userId);
        }
    }
}