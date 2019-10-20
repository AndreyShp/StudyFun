using System;
using System.Web.Mvc;
using BusinessLogic.Logger;

namespace StudyLanguages.Controllers {
    public class ErrorsController : Controller {
        [HttpGet]
        public ActionResult NotFound() {
            ActionResult result;

            object model = Request.Url.PathAndQuery;
            if (!Request.IsAjaxRequest()) {
                result = View("Error404", model);
            } else {
                result = PartialView("Error404", model);
            }
            return result;
        }

        [HttpGet]
        public ActionResult Unknown() {
            ActionResult result;

            Exception ex = HttpContext.Server.GetLastError();
            if (ex != null) {
                LoggerWrapper.RemoteMessage(LoggingType.Error, ex.ToString());
            }
            string description = null;
            /*LoggerWrapper.LogTo(LoggerName.Errors).InfoFormat("пробуем логер!!!");
            try {
                var sw = new StreamWriter(@"C:\HostingSpaces\dreyFake@yandex.ru1\studyfun.ru\wwwroot\Logs\test.txt");
                sw.WriteLine("Test");
                sw.Flush();
                sw.Close();
                //description = "Мы разбираемся с проблемой";
            } catch (Exception e2) {
                //description = string.Format("Что-то пошло не так {0}:(", e2);
            }*/

            /*string description = Session["errorDescription"] as string;*/
            ViewData["errorDescription"] = description;
            if (!Request.IsAjaxRequest()) {
                result = View("Error");
            } else {
                result = PartialView("Error");
            }
            return result;
        }
    }
}