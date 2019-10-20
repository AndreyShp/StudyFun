using System.Web.Mvc;
using BusinessLogic.DataQuery;
using BusinessLogic.Logger;
using BusinessLogic.Mailer;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Filters;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class ProfileController : Controller {
        public static readonly string Message =
            "Мы оставляем за собой право удалить Вашего пользователя, если Вы не пользуйтесь сайтом более "
            + CommonConstants.COUNT_DAYS_TO_HOLD_DATA + " дней";

        //
        // GET: /Profile/

        public ActionResult Index() {
            string uniqueUserId = GetUserUniqueId();
            if (string.IsNullOrEmpty(uniqueUserId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            return View((object) uniqueUserId);
        }

        private string GetUserUniqueId() {
            return new UserIdAttribute().GetUserUniqueIdFromCookie(HttpContext);
        }

        [UserId]
        public JsonResult SetNewValue(long userId, string newValue) {
            if (IdValidator.IsInvalid(userId)) {
                return JsonResultHelper.Error();
            }

            bool result = new UserIdAttribute().SetNewCookie(HttpContext, newValue);
            return JsonResultHelper.Success(result);
        }

        [UserId]
        public JsonResult SendToMail(long userId, string email) {
            if (IdValidator.IsInvalid(userId) || string.IsNullOrWhiteSpace(email)) {
                return JsonResultHelper.Error();
            }

            string uniqueUserId = GetUserUniqueId();
            if (string.IsNullOrEmpty(uniqueUserId)) {
                return JsonResultHelper.Error();
            }

            string domain = WebSettingsConfig.Instance.Domain;

            //TODO: вынести в конфиг
            var mailerConfig = new MailerConfig {
                IsHtmlBody = true,
                DisplayName = "Сайт " + domain
            };

            const string SUBJECT = "Ваш уникальный идентификатор";
            string body = string.Format("Здравствуйте, Уважаемый пользователь!<br />"
                                        + "Ваш уникальный идентификатор на сайте {0}:<br /><b>{1}</b><br /><br />"
                                        + "<span style='font-size:12px;'><b>" + Message + "</b></span><br />"
                                        +
                                        "<span style='font-size: 11px;'>Если Вы не указывали этот адрес почты на сайте {0}, то просто удалите это письмо.<br />" +
                                        "Данное письмо не требует ответа.</span>",
                                        domain, uniqueUserId);

            var mailer = new Mailer();
            bool isSuccess = mailer.SendMail(MailAddresses.SUPPORT, email, SUBJECT, body, mailerConfig);

            if (isSuccess) {
                var usersQuery = new UsersQuery();
                if (!usersQuery.UpdateEmail(userId, email)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "ProfileController.SendToMail для пользователя с идентификатором {0}, не смогли обновить адрес электронной почты на {1}",
                        userId, email);
                }
            }

            return JsonResultHelper.Success(isSuccess);
        }
    }
}