using System;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Trainer;
using StudyLanguages.Models.Main;
using StudyLanguages.Models.Trainer;

namespace StudyLanguages.Controllers {
    public abstract class BaseGroupItemsTrainerController : Controller {
        protected abstract KnowledgeDataType KnowledgeDataType { get; }
        protected abstract KnowledgeSourceType KnowledgeSourceType { get; }
        protected abstract SectionId SectionId { get; }

        protected ActionResult GetIndex(long userId,
                                        UserLanguages userLanguages,
                                        long groupId,
                                        Action<TrainerModel> modelSetter) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (IdValidator.IsInvalid(userId) || UserLanguages.IsInvalid(userLanguages)
                || IdValidator.IsInvalid(groupId)) {
                return RedirectToParentPage();
            }

            IUserRepetitionIntervalQuery userRepetitionIntervalQuery = CreateUserRepetitionIntervalQuery(userId, groupId);

            var trainerHelper = new TrainerHelper(userRepetitionIntervalQuery, userLanguages);
            TrainerModel model = trainerHelper.GetTrainerModel(Request);
            modelSetter(model);
            if (EnumerableValidator.IsNullOrEmpty(model.Items)) {
                return RedirectToParentPage();
            }

            ViewData[OurHtmlHelper.ViewDataFlags.SKIP_POPUP_PANEL] = true;
            return View("../Trainer/Index", model);
        }

        protected abstract RedirectToRouteResult RedirectToParentPage();

        private IUserRepetitionIntervalQuery CreateUserRepetitionIntervalQuery(long userId, long groupId) {
            //TODO: создавать с помощью IoCModule
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            IUserRepetitionKnowledgeQuery repetitionQuery = CreateRepetitionQuery(userId, languageId, groupId);
            return new UserRepetitionIntervalQuery(userId, languageId, KnowledgeSourceType, repetitionQuery,
                                                   RepetitionType.All);
        }

        protected abstract IUserRepetitionKnowledgeQuery CreateRepetitionQuery(long userId,
                                                                               long languageId,
                                                                               long representationId);

        protected JsonResult SetMarkAndGetModel(long userId,
                                                UserLanguages userLanguages,
                                                long groupId,
                                                KnowledgeMark mark,
                                                TrainerItem item) {
            var itemDataType = (KnowledgeDataType) (item != null ? item.DataType : int.MinValue);
            if (IdValidator.IsInvalid(userId) || UserLanguages.IsInvalid(userLanguages) || EnumValidator.IsInvalid(mark)
                || item == null
                || IdValidator.IsInvalid(item.DataId) || EnumValidator.IsInvalid(itemDataType)
                || IdValidator.IsInvalid(groupId)) {
                //TODO: писать сообщение
                return JsonResultHelper.Error();
            }

            IUserRepetitionIntervalQuery userRepetitionIntervalQuery = CreateUserRepetitionIntervalQuery(userId, groupId);

            var repetitionInterval = new UserRepetitionIntervalItem
            {DataId = item.DataId, DataType = itemDataType, SourceType = KnowledgeSourceType};

            var trainerHelper = new TrainerHelper(userRepetitionIntervalQuery, userLanguages);
            return trainerHelper.SetMark(Request, mark, repetitionInterval);
        }
    }
}