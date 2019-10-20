using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Trainer;
using StudyLanguages.Models;
using StudyLanguages.Models.Main;
using StudyLanguages.Models.Trainer;

namespace StudyLanguages.Controllers {
    public abstract class BaseKnowledgeTrainerController : Controller {
        protected abstract KnowledgeDataType KnowledgeDataType { get; }
        protected abstract string ControllerName { get; }
        protected abstract string PageName { get; }

        private KnowledgeSourceType KnowledgeSourceType {
            get { return KnowledgeSourceType.Knowledge; }
        }

        [UserId]
        [UserLanguages]
        public ActionResult Index(long userId, UserLanguages userLanguages) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.MyKnowledge)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (IsInvalid(userId, userLanguages)) {
                return RedirectToKnowledgeWall();
            }

            IUserRepetitionIntervalQuery userRepetitionIntervalQuery = CreateUserRepetitionIntervalQuery(userId,
                                                                                                         RepetitionType.
                                                                                                             All);

            var trainerHelper = new TrainerHelper(userRepetitionIntervalQuery, userLanguages);
            TrainerModel model = trainerHelper.GetTrainerModel(Request);
            
            var pageRequiredData = new PageRequiredData(SectionId.MyKnowledge);
            pageRequiredData.Title = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Title, PageName);
            pageRequiredData.Keywords = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Keywords, PageName);
            pageRequiredData.Description = WebSettingsConfig.Instance.GetTemplateText(pageRequiredData.SectionId, PageId.Cards, TemplateId.Description, PageName);
            model.PageRequiredData = pageRequiredData;
            model.SetMarkUrl = Url.Action("SetMark", ControllerName, null, Request.Url.Scheme);
            model.BreadcrumbsItems = new List<BreadcrumbItem> {
                new BreadcrumbItem
                {Title = "Стена знаний", Action = "Index", ControllerName = RouteConfig.USER_KNOWLEDGE_CONTROLLER},
                new BreadcrumbItem {IsActive = true, Title = PageName}
            };
            if (EnumerableValidator.IsNullOrEmpty(model.Items)) {
                return RedirectToKnowledgeWall();
            }

            ViewData[OurHtmlHelper.ViewDataFlags.SKIP_POPUP_PANEL] = true;
            return View("../Trainer/Index", model);
        }

        private static bool IsInvalid(long userId, UserLanguages userLanguages) {
            return IdValidator.IsInvalid(userId) || UserLanguages.IsInvalid(userLanguages);
        }

        private RedirectToRouteResult RedirectToKnowledgeWall() {
            return RedirectToAction("Index", RouteConfig.USER_KNOWLEDGE_CONTROLLER);
        }

        private IUserRepetitionIntervalQuery CreateUserRepetitionIntervalQuery(long userId,
                                                                               RepetitionType repetitionType) {
            //TODO: создавать с помощью IoCModule
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            IUserRepetitionKnowledgeQuery repetitionQuery = new UserRepetitionKnowledgeQuery(userId, languageId,
                                                                                             KnowledgeDataType);
            return new UserRepetitionIntervalQuery(userId, languageId, KnowledgeSourceType, repetitionQuery, repetitionType);
        }

        [UserId]
        [UserLanguages]
        [HttpPost]
        public JsonResult SetMark(long userId,
                                  UserLanguages userLanguages,
                                  KnowledgeMark mark,
                                  TrainerItem item,
                                  RepetitionType repetitionType = RepetitionType.All) {
            var itemDataType = (KnowledgeDataType) (item != null ? item.DataType : int.MinValue);
            if (IsInvalid(userId, userLanguages) || EnumValidator.IsInvalid(mark) || item == null
                || IdValidator.IsInvalid(item.DataId) || EnumValidator.IsInvalid(itemDataType)
                || EnumValidator.IsInvalid(repetitionType)) {
                //TODO: писать сообщение
                return JsonResultHelper.Error();
            }

            IUserRepetitionIntervalQuery userRepetitionIntervalQuery = CreateUserRepetitionIntervalQuery(userId,
                                                                                                         repetitionType);

            var repetitionInterval = new UserRepetitionIntervalItem
            {DataId = item.DataId, DataType = itemDataType, SourceType = KnowledgeSourceType};

            var trainerHelper = new TrainerHelper(userRepetitionIntervalQuery, userLanguages);
            return trainerHelper.SetMark(Request, mark, repetitionInterval);
        }

        [UserId]
        [UserLanguages]
        public JsonResult LoadPopupTrainer(long userId, UserLanguages userLanguages) {
            if (IsInvalid(userId, userLanguages)) {
                return JsonResultHelper.Error();
            }

            List<TrainerItem> items = GetOverdueItems(userId, userLanguages);

            return
                JsonResultHelper.GetUnlimitedJsonResult(
                    new {
                        sourceLanguageId = userLanguages.From.Id,
                        items,
                        html =
                        OurHtmlHelper.RenderRazorViewToString(ControllerContext, "PartialTrainer",
                                                              new PartialTrainerParams {
                                                                  NeedShow = true,
                                                                  BtnClass = "btn-xs",
                                                                  LeftBtnContainerClass =
                                                                      "popup-knowledge-panel-mark-btn",
                                                                  CenterBtnContainerClass =
                                                                      "popup-knowledge-panel-mark-btn",
                                                                  RightBtnContainerClass =
                                                                      "popup-knowledge-panel-mark-btn",
                                                                  TrainerPanelClass = "",
                                                                  TagTitle = "h3",
                                                                  TagTranslation = "h4"
                                                              })
                    });
        }

        private List<TrainerItem> GetOverdueItems(long userId, UserLanguages userLanguages) {
            IUserRepetitionIntervalQuery userRepetitionIntervalQuery = CreateUserRepetitionIntervalQuery(userId,
                                                                                                         RepetitionType.
                                                                                                             Overdue);
            var trainerHelper = new TrainerHelper(userRepetitionIntervalQuery, userLanguages);
            TrainerModel model = trainerHelper.GetTrainerModel(Request);
            List<TrainerItem> items = model.Items;
            return items;
        }

        [UserId]
        [UserLanguages]
        public JsonResult LoadData(long userId, UserLanguages userLanguages) {
            if (IsInvalid(userId, userLanguages)) {
                return JsonResultHelper.Error();
            }

            List<TrainerItem> items = GetOverdueItems(userId, userLanguages);
            return JsonResultHelper.GetUnlimitedJsonResult(items);
        }
    }
}