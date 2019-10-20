using System.Web.Mvc;
using BusinessLogic.Mailer;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class HelpController : Controller {
        //
        // GET: /Comment/

        public ActionResult Index() {
            return View();
        }

        public ActionResult Vacations() {
            return View("../Help/Vacations");
        }

        [HttpPost]
        public JsonResult FeedbackMessage(string message, string email) {
            //TODO: антиспам систему
            if (string.IsNullOrWhiteSpace(message)) {
                return JsonResultHelper.Error();
            }

            string text = string.Format("Домен: {0}\r\n" +
                                        "IP-адрес: {1}\r\n\r\n" +
                                        "Адрес для ответа: {2}\r\n" +
                                        "Сообщение:\r\n{3}",
                                        WebSettingsConfig.Instance.Domain,
                                        RemoteClientHelper.GetClientIpAddress(Request),
                                        OurHtmlHelper.HtmlEncode(email, true),
                                        OurHtmlHelper.HtmlEncode(message, true));

            var mailer = new Mailer();
            bool isSuccess = mailer.SendMail(MailAddresses.FEEDBACK, MailAddresses.FEEDBACK, "Обратная связь", text);
            return JsonResultHelper.Success(isSuccess);
        }
    }
}