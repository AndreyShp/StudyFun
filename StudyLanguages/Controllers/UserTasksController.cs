using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.DataQuery.UserRepository;
using BusinessLogic.DataQuery.UserRepository.Tasks;
using BusinessLogic.ExternalData;
using BusinessLogic.Logger;
using BusinessLogic.Mailer;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;
using StudyLanguages.Models.User;

namespace StudyLanguages.Controllers {
    public class UserTasksController : Controller {
        private const string TASK_COOKIE_NAME = "NewTaskId";

        //
        // GET: /UserText/

        [UserId]
        public ActionResult Index(long userId) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.UserTasks)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            UserTasksRepository userTasksRepository = repositoryFactory.CreateUserRepository(userId);
            List<UserTask> tasks = userTasksRepository.GetTasks();

            HttpCookie newTaskCookie = HttpContext.Request.Cookies[TASK_COOKIE_NAME];
            string taskId = null;
            if (newTaskCookie != null) {
                taskId = newTaskCookie.Value;
                RemoveTaskCookie();
            }

            BanRepository banRepository = repositoryFactory.GetBanRepository();
            var banHelper = new BanHelper(Request);
            bool isBanned = banHelper.IsBanned(SectionId.UserTasks, userId, banRepository);

            var model = new UserTasksModel(taskId, tasks, isBanned);
            return View(model);
        }

        private void RemoveTaskCookie() {
            HttpContext.Response.Cookies.Add(new HttpCookie(TASK_COOKIE_NAME) {Expires = DateTime.Now.AddDays(-1)});
        }

        [UserId]
        public ActionResult NewTaskIndex(long userId) {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.UserTasks)) {
                return RedirectToAction("Index");
            }

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            BanRepository banRepository = repositoryFactory.GetBanRepository();
            var banHelper = new BanHelper(Request);
            bool isBanned = banHelper.IsBanned(SectionId.UserTasks, userId, banRepository);
            if (isBanned) {
                return RedirectToAction("Index");
            }

            return View("AddNew");
        }

        [UserId]
        public ActionResult DetailIndex(long userId, long authorId, string key) {
            //TODO: проверить валидность ключа на md5, проверить что у пользователя есть репозиторий
            if (IdValidator.IsInvalid(authorId) || string.IsNullOrEmpty(key)
                || WebSettingsConfig.Instance.IsSectionForbidden(SectionId.UserTasks)) {
                return RedirectToAction("Index");
            }

            RemoveTaskCookie();
            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            UserTasksRepository userTasksRepository = repositoryFactory.CreateUserRepository(authorId);
            UserTask userTask = userTasksRepository.GetTask(key, true);

            if (userTask == null) {
                return RedirectToAction("Index");
            }

            BanRepository banRepository = repositoryFactory.GetBanRepository();
            var banHelper = new BanHelper(Request);
            bool isBanned = banHelper.IsBanned(SectionId.UserTasks, userId, banRepository);

            userTask.SetIsBanned(isBanned);
            userTask.SetAllRights(userId == authorId);
            return View("Detail", userTask);
        }

        [HttpPost]
        [UserId(true)]
        public JsonResult AddTask(long userId, string task) {
            if (IdValidator.IsInvalid(userId) || string.IsNullOrWhiteSpace(task)
                || WebSettingsConfig.Instance.IsSectionForbidden(SectionId.UserTasks)) {
                return JsonResultHelper.Error();
            }

            if (task.Length > UserTasksSettings.TASK_MAX_LENGTH) {
                return JsonResultHelper.Error();
            }

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            BanRepository banRepository = repositoryFactory.GetBanRepository();
            var banHelper = new BanHelper(Request);
            bool isBanned = banHelper.IsBanned(SectionId.UserTasks, userId, banRepository);
            if (isBanned) {
                return JsonResultHelper.Error();
            }

            banHelper.RegisterEvent(SectionId.UserTasks, "AddTask", userId, banRepository);
            
            UserTasksRepository userTasksRepository = repositoryFactory.CreateUserRepository(userId);

            task = OurHtmlHelper.PrepareStringFromUser(task);

            //TODO: дописывать (автор) к имени пользователя
            var userTask = new UserTask {
                Author = null,
                AuthorId = userId,
                Text = task,
                CreationDate = DateTime.Now.Ticks,
                DeletedDate = 0
            };

            UserTask addedTask = userTasksRepository.AddTask(userTask);
            if (addedTask == null) {
                return JsonResultHelper.Error();
            }

            SendMail(string.Format("Пользователь {0} добавил таск {1}:\r\n{2}", userId, addedTask.Id, task));

            HttpContext.Response.Cookies.Add(new HttpCookie(TASK_COOKIE_NAME, addedTask.Id));
            string urlToRedirect = Url.Action("Index", RouteConfig.USER_TASKS_CONTROLLER, null, Request.Url.Scheme);
            return JsonResultHelper.GetUnlimitedJsonResult(new {success = true, urlToRedirect});
        }

        private static void SendMail(string body) {
            try {
                var mailer = new Mailer();
                mailer.SendMailToCopyAdmins(MailAddresses.SUPPORT, MailAddresses.SUPPORT, "Задания на " + WebSettingsConfig.Instance.Domain, body);
            } catch(Exception e) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat("UserTasksController.SendMail при попытке отправить письмо вылетело исключение {0}", e);
            }
        }

        [UserId]
        [HttpPost]
        public ActionResult RemoveOrRestore(long userId, string key, bool needRemove) {
            if (IdValidator.IsInvalid(userId) || string.IsNullOrWhiteSpace(key)
                || WebSettingsConfig.Instance.IsSectionForbidden(SectionId.UserTasks)) {
                return JsonResultHelper.Error();
            }

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            UserTasksRepository userTasksRepository = repositoryFactory.CreateUserRepository(userId);
            bool success = userTasksRepository.RemoveOrRestoreTask(key, needRemove);
            if (success) {
                SendMail(string.Format("Пользователь {0} {1} таск {2}.", userId, needRemove ? "удалил" : "восстановил", key));
                return JsonResultHelper.Success(true);
            }

            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                "UserTasksController.RemoveOrRestore. Для пользователя с идентификатором {0} не удалось {1} таск с идентификатором {2}",
                userId, needRemove ? "удалить" : "восстановить", key);
            return JsonResultHelper.Error();
        }

        [HttpPost]
        [UserId]
        public JsonResult AddComment(long userId, long authorId, string key, string comment, int lastShowedComment) {
            if (IdValidator.IsInvalid(authorId) || string.IsNullOrWhiteSpace(comment)
                || WebSettingsConfig.Instance.IsSectionForbidden(SectionId.UserTasks)) {
                return JsonResultHelper.Error();
            }

            if (comment.Length > UserTasksSettings.COMMENT_MAX_LENGTH) {
                return JsonResultHelper.Error();
            }

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            BanRepository banRepository = repositoryFactory.GetBanRepository();
            var banHelper = new BanHelper(Request);
            bool isBanned = banHelper.IsBanned(SectionId.UserTasks, userId, banRepository);
            if (isBanned) {
                return JsonResultHelper.Error();
            }

            banHelper.RegisterEvent(SectionId.UserTasks, "AddComment", userId, banRepository);

            UserTasksRepository userTasksRepository = repositoryFactory.CreateUserRepository(authorId);

            //TODO: получить пользователя и если у него указано имя, то заполнить Author

            comment = OurHtmlHelper.PrepareStringFromUser(comment);
            //TODO: дописывать (автор) к имени пользователя
            var taskComment = new TaskComment {
                Author = null,
                AuthorId = userId,
                CreationDate = DateTime.Now.Ticks,
                Text = comment
            };

            bool isAddedComment = userTasksRepository.AddComment(key, taskComment);
            List<TaskComment> comments = userTasksRepository.GetComments(key, lastShowedComment);

            SendMail(string.Format("Пользователь {0} оставил комментарий к таску {1} автора {2}:\r\n{3}", userId, key, authorId, comment));

            var htmlItems = new StringBuilder();
            foreach (TaskComment newComment in comments) {
                string item = OurHtmlHelper.RenderRazorViewToString(ControllerContext, "PartialTaskComment", newComment);
                htmlItems.Insert(0, item);
            }

            return
                JsonResultHelper.GetUnlimitedJsonResult(
                    new {
                        success = isAddedComment,
                        newComments = htmlItems.ToString(),
                        countNewComments = comments.Count
                    });
        }
    }
}