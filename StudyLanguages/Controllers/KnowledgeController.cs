using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models.Knowledge;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class KnowledgeController : Controller {
        private const int MAX_COUNT_ITEMS_PER_DAY = 200;
        private const int MAX_COUNT_ITEMS_SHOWED_PER_ONCE = 100;

        //TODO: вынести в отдельный класс
        private const string INVALID_DATA = "Переданы некорректные данные!";

        [UserId]
        [UserLanguages]
        public ActionResult Index(long userId, UserLanguages userLanguages) {
            const string EMPTY_WALL_VIEW = "EmptyWall";

            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.MyKnowledge)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            if (IdValidator.IsInvalid(userId) || UserLanguages.IsInvalid(userLanguages)) {
                return View(EMPTY_WALL_VIEW);
            }

            IUserKnowledgeQuery userKnowledgeQuery = CreateUserKnowledgeQuery(userId);
            List<UserKnowledgeItem> items = GetData(userKnowledgeQuery, userLanguages, long.MaxValue);
            if (items == null || EnumerableValidator.IsEmpty(items)) {
                return View(EMPTY_WALL_VIEW);
            }

            //TODO: если будет тормозить - получать ajax'ом данные через action GetStatistic
            UserKnowledgeStatistic statistic = userKnowledgeQuery.GetStatistic();

            return View("Index", new KnowledgeWallModel(userLanguages, items) {
                Statistic = statistic
            });
        }

        private static IUserKnowledgeQuery CreateUserKnowledgeQuery(long userId) {
            //TODO: создавать с помощью IoCModule
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            return new UserKnowledgeQuery(userId, languageId);
        }

        [UserId(true)]
        [UserLanguages]
        [HttpPost]
        public JsonResult Add(long userId, UserKnowledgeItem knowledgeItem) {
            return AddMany(userId, new List<UserKnowledgeItem> {knowledgeItem});
        }

        [UserId(true)]
        [UserLanguages]
        [HttpPost]
        public JsonResult AddMany(long userId, List<UserKnowledgeItem> knowledgeItems) {
            IUserKnowledgeQuery userKnowledgeQuery = CreateUserKnowledgeQuery(userId);
            if (EnumerableValidator.IsNullOrEmpty(knowledgeItems) || knowledgeItems.Any(userKnowledgeQuery.IsInvalid)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "KnowledgeController.AddMany пользователь с идентификатором {0}, передал некорректные данные",
                    userId);
                return JsonResultHelper.Error(INVALID_DATA);
            }
            List<KnowledgeAddStatus> statuses = userKnowledgeQuery.Add(knowledgeItems, MAX_COUNT_ITEMS_PER_DAY);
            KnowledgeAddStatus summaryStatus = EnumerableValidator.IsCountEquals(statuses, knowledgeItems)
                                                   ? GetSummaryStatus(statuses)
                                                   : KnowledgeAddStatus.Error;
            if (summaryStatus == KnowledgeAddStatus.Ok) {
                return JsonResultHelper.Success(true);
            }

            if (summaryStatus == KnowledgeAddStatus.ReachMaxLimit) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "KnowledgeController.AddMany пользователь с идентификатором {0}, достиг лимит данных за сегодня",
                    userId);
                return JsonResultHelper.Error(
                    string.Format(
                        "Сегодня вы уже добавили максимальное количество данных на обучение. В день вы можете добавлять не более {0} порций знаний. Завтра Вы вновь сможете добавлять новые элементы, а пока, рекомендуем изучить сегодняшний материал.",
                        MAX_COUNT_ITEMS_PER_DAY));
            }

            if (summaryStatus == KnowledgeAddStatus.AlreadyExists) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "KnowledgeController.AddMany пользователь с идентификатором {0}, уже добавлял данные",
                    userId);
                return JsonResultHelper.Error("Ранее вы уже добавляли эти данные на обучение.");
            }

            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                "KnowledgeController.AddMany. Для пользователя с идентификатором {0} не удалось добавить данные, какая-то ошибка!",
                userId);
            return
                JsonResultHelper.Error("Не удалось добавить порцию знаний! Попробуйте позже.");
        }

        private static KnowledgeAddStatus GetSummaryStatus(IEnumerable<KnowledgeAddStatus> statuses) {
            List<KnowledgeAddStatus> uniqueStatuses = statuses.Distinct().ToList();
            if (uniqueStatuses.Count == 1) {
                //статус один - вернуть его
                return uniqueStatuses[0];
            }

            if (HasAddStatus(uniqueStatuses, KnowledgeAddStatus.Error)) {
                //есть ошибки - общий статус ошибочный
                return KnowledgeAddStatus.Error;
            }

            if (HasAddStatus(uniqueStatuses, KnowledgeAddStatus.Ok)) {
                //ошибок нет и есть успешные статусы - общий статус успешный
                return KnowledgeAddStatus.Ok;
            }

            //NOTE: СЮДА НЕ ДОЛЖНЫ ПОПАСТЬ, т.к. какие-то странные статусы остались - берем первый попавшийся???
            return uniqueStatuses.First();
        }

        private static bool HasAddStatus(IEnumerable<KnowledgeAddStatus> statuses, KnowledgeAddStatus knowledgeAddStatus) {
            return statuses.Any(e => e == knowledgeAddStatus);
        }

        [UserId]
        [HttpPost]
        public ActionResult RemoveOrRestore(long userId, long id, bool needRemove) {
            if (IdValidator.IsInvalid(userId) || IdValidator.IsInvalid(id)) {
                return JsonResultHelper.Error(INVALID_DATA);
            }

            IUserKnowledgeQuery userKnowledgeQuery = CreateUserKnowledgeQuery(userId);
            bool isRemoved = needRemove ? userKnowledgeQuery.Remove(id) : userKnowledgeQuery.Restore(id);
            if (isRemoved) {
                return JsonResultHelper.Success(true);
            }

            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                "KnowledgeController.RemoveOrRestore. Для пользователя с идентификатором {0} не удалось {1} данные с идентификатором {2}",
                userId, needRemove ? "удалить" : "восстановить", id);
            return JsonResultHelper.Error();
        }

        [UserId]
        [UserLanguages]
        public JsonResult Load(long userId, UserLanguages userLanguages, long prevId) {
            if (IdValidator.IsInvalid(userId) || UserLanguages.IsInvalid(userLanguages) || IdValidator.IsInvalid(prevId)) {
                return JsonResultHelper.Error(INVALID_DATA);
            }

            IUserKnowledgeQuery userKnowledgeQuery = CreateUserKnowledgeQuery(userId);
            List<UserKnowledgeItem> items = GetData(userKnowledgeQuery, userLanguages, prevId);
            var htmlItems = new List<string>();
            foreach (UserKnowledgeItem userKnowledgeItem in items ?? new List<UserKnowledgeItem>(0)) {
                string item = OurHtmlHelper.RenderRazorViewToString(ControllerContext, "PartialWallItem",
                                                                    userKnowledgeItem);
                htmlItems.Add(item);
            }
            return
                JsonResultHelper.GetUnlimitedJsonResult(
                    new {sourceLanguageId = userLanguages.From.Id, items = htmlItems});
        }

        private static List<UserKnowledgeItem> GetData(IUserKnowledgeQuery userKnowledgeQuery,
                                                       UserLanguages userLanguages,
                                                       long prevId) {
            List<UserKnowledgeItem> items = userKnowledgeQuery.GetData(userLanguages.From.Id, userLanguages.To.Id,
                                                                       KnowledgeStatus.All, prevId,
                                                                       MAX_COUNT_ITEMS_SHOWED_PER_ONCE);
            return items;
        }

        [UserId]
        [HttpPost]
        public JsonResult GetExistenceIds(long userId, List<long> ids, KnowledgeDataType dataType) {
            /*             List<long> ids = new List<long>();
            KnowledgeDataType dataType = KnowledgeDataType.SentenceTranslation;*/
            List<long> parsedIds = ids != null ? ids.Where(IdValidator.IsValid).ToList() : new List<long>(0);
            if (IdValidator.IsInvalid(userId) || EnumerableValidator.IsEmpty(parsedIds)
                || EnumValidator.IsInvalid(dataType)) {
                return JsonResultHelper.Error(INVALID_DATA);
            }

            IUserKnowledgeQuery userKnowledgeQuery = CreateUserKnowledgeQuery(userId);
            List<long> existenceIds = userKnowledgeQuery.GetExistenceIds(parsedIds, dataType);
            return JsonResultHelper.GetUnlimitedJsonResult(existenceIds);
        }

        [UserId]
        public JsonResult GetStatistic(long userId) {
            if (IdValidator.IsInvalid(userId)) {
                return JsonResultHelper.GetUnlimitedJsonResult(new object());
            }

            IUserKnowledgeQuery userKnowledgeQuery = CreateUserKnowledgeQuery(userId);

            UserKnowledgeStatistic statistic = userKnowledgeQuery.GetStatistic();
            return JsonResultHelper.GetUnlimitedJsonResult(statistic);
        }
    }
}