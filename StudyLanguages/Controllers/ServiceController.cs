using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Auxiliaries;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.UserRepository;
using BusinessLogic.DataQuery.UserRepository.Tasks;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Auxiliaries;
using BusinessLogic.ExternalData.Sales;
using BusinessLogic.Logger;
using BusinessLogic.SalesGenerator;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Sitemap;

namespace StudyLanguages.Controllers {
    public class ServiceController : Controller {
        //
        // GET: /SiteMap/

        public ActionResult Sitemap() {
            var fileContent = SitemapFileGenerator.Generate(true);

            return Content(Encoding.UTF8.GetString(fileContent), "text/xml", Encoding.UTF8); ;
        }

        public EmptyResult ReloadWebSettings() {
            WebSettingsConfig.Instance.Configure();
            return new EmptyResult();
        }

        public EmptyResult Clean() {
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var cleaners = new ICleaner[] {
                                              new LoggerQuery(),
                                              new RatingByIpsQuery(languageId),
                                              new UsersQuery(), 
                                              new SentencesQuery(), 
                                              new ShuffleWordsQuery(WordType.Default, ShuffleType.Usual), 
            };
            var maxDateToRemove = DateTime.Today.AddMonths(-2);
            foreach (var cleaner in cleaners) {
                cleaner.Clean(maxDateToRemove);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Банит пользователя
        /// </summary>
        /// <example>localhost/StudyLanguages/Service/BanUser?userId=59&browser=Firefox&userIp=::1&sectionId=UserTasks&banType=Today</example>
        /// <returns></returns>
        public EmptyResult BanUser() {
            string dirtyUserId = Request.Params["userId"];
            string dirtySectionId = Request.Params["sectionId"];
            string dirtyBanType = Request.Params["banType"];
            string userIp = Request.Params["userIp"];
            string browser = Request.Params["browser"];

            string remoteClientIp = RemoteClientHelper.GetClientIpAddress(Request);
            if (string.IsNullOrEmpty(remoteClientIp) || !remoteClientIp.Equals("176.214.39.34")) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "ServiceController.BanUser кто-то попытался забанить пользователя!!! Настройки: userId={0}, sectionId={1}, banType={2}, userIp={3}, browser={4}",
                    dirtyUserId, dirtySectionId, dirtyBanType, userIp, browser);
                return new EmptyResult();
            }

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            BanRepository banRepository = repositoryFactory.GetBanRepository();

            SectionId sectionId;
            BanType banType;
            long userId;
            if (!Enum.TryParse(dirtySectionId, true, out sectionId) || EnumValidator.IsInvalid(sectionId)
                || !Enum.TryParse(dirtyBanType, true, out banType) || EnumValidator.IsInvalid(banType)
                || !long.TryParse(dirtyUserId, out userId) || string.IsNullOrEmpty(userIp)
                || string.IsNullOrEmpty(browser)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "ServiceController.BanUser НЕ УДАЛОСЬ распрасить данные!!! Настройки: userId={0}, sectionId={1}, banType={2}, userIp={3}, browser={4}",
                    dirtyUserId, dirtySectionId, dirtyBanType, userIp, browser);
                return new EmptyResult();
            }

            if (!banRepository.AddBan(sectionId, banType, userId, userIp, browser)) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "ServiceController.BanUser НЕ УДАЛОСЬ забанить пользователя!!! Настройки: userId={0}, sectionId={1}, banType={2}, userIp={3}, browser={4}",
                    userId, sectionId, banType, userIp, browser);
            }

            return new EmptyResult();
        }

        public JsonResult GetInterviews() {
            IInterviewsQuery interviewsQuery = new InterviewsQuery();
            List<Interview> questions = interviewsQuery.GetQuestions();
            return JsonResultHelper.GetUnlimitedJsonResult(questions);
        }

        [HttpPost]
        public JsonResult AnswerInterview(List<long> answersIds) {
            List<long> validAnswersIds = (answersIds ?? new List<long>(0)).Where(IdValidator.IsValid).ToList();
            if (EnumerableValidator.IsNullOrEmpty(validAnswersIds)) {
                return JsonResultHelper.Error();
            }

            IInterviewsQuery interviewsQuery = new InterviewsQuery();
            bool isSuccess = interviewsQuery.IncrementCountAnswers(validAnswersIds);
            return JsonResultHelper.Success(isSuccess);
        }

        public void SpecialActions() {
            FillCache();
        }

        /// <summary>
        /// Заполняет кэш необходимыми данными если их там нет
        /// </summary>
        private void FillCache() {
            var tasks = new [] { new Task(GenerateAllMaterialsCache), new Task(GenerateVisualDictionariesCache)};
            foreach (var task in tasks) {
                task.Start();
            }
            Task.WaitAll(tasks);
        }

        private void GenerateAllMaterialsCache() {
            UserLanguages userLanguages = WebSettingsConfig.Instance.DefaultUserLanguages;
            GenerateAllMaterials(userLanguages);
            var contraUserLanguages = new UserLanguages {From = userLanguages.To, To = userLanguages.From};
            GenerateAllMaterials(contraUserLanguages);
        }

        private void GenerateAllMaterials(UserLanguages userLanguages) {
            var allMaterialsSalesGenerator =
                new AllMaterialsSalesGenerator(WebSettingsConfig.Instance.DomainWithProtocol,
                                               CommonConstants.GetFontPath(Server),
                                               WebSettingsConfig.Instance.
                                                   SalesPicturesCache,
                                               WebSettingsConfig.Instance.
                                                   IsSectionForbidden);
            allMaterialsSalesGenerator.Generate(WebSettingsConfig.Instance.GetLanguageFromId(), userLanguages);
        }

        private void GenerateVisualDictionariesCache() {
            ISalesSettings salesSettings = WebSettingsConfig.Instance.GetSalesSettings(SectionId.VisualDictionary);
            IEnumerable<SalesItemForUser> allSalesItems = GetSalesItems(salesSettings);
            if (allSalesItems == null) {
                return;
            }
            var idsToBuy = new HashSet<long>(allSalesItems.Select(e => e.Id));
            var representationsSalesGenerator =
                new RepresentationsSalesGenerator(WebSettingsConfig.Instance.DomainWithProtocol,
                                                  CommonConstants.GetFontPath(Server),
                                                  WebSettingsConfig.Instance.
                                                      SalesPicturesCache);
            representationsSalesGenerator.Generate(WebSettingsConfig.Instance.DefaultLanguageFrom,
                                                   WebSettingsConfig.Instance.DefaultLanguageTo,
                                                   idsToBuy);
        }

        private static IEnumerable<SalesItemForUser> GetSalesItems(ISalesSettings salesSettings) {
            if (salesSettings == null) {
                return new List<SalesItemForUser>(0);
            }
            var representationsQuery = new RepresentationsQuery(WebSettingsConfig.Instance.GetLanguageFromId());
            List<SalesItemForUser> salesItems =
                representationsQuery.GetForSales(WebSettingsConfig.Instance.DefaultUserLanguages, salesSettings);
            return salesItems;
        }
    }
}