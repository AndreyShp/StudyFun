using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Sales;
using BusinessLogic.Mailer;
using BusinessLogic.SalesGenerator;
using StudyLanguages.Configs;
using StudyLanguages.Controllers;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Sitemap;

namespace StudyLanguages {
    // Примечание: Инструкции по включению классического режима IIS6 или IIS7 
    // см. по ссылке http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Start() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Initialize your application
            WebSettingsConfig.Instance.SetDeferredSettings(webSettingsConfig => {
                ISalesSettings salesSettings = webSettingsConfig.GetSalesSettings(SectionId.VisualDictionary);
                if (salesSettings == null) {
                    return;
                }

                long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
                var representationsQuery = new RepresentationsQuery(languageId);

                List<SalesItemForUser> allSalesItems =
                    representationsQuery.GetForSales(WebSettingsConfig.Instance.DefaultUserLanguages,
                                                     salesSettings);
                var salesCalculator = new SalesCalculator(allSalesItems, allSalesItems, salesSettings.Discount);
                salesSettings.SummDiscountPrice = salesCalculator.SummDiscountPrice;
            }, webSettingsConfig => {
                //кладем в кэш файл sitemap.xml
                SitemapFileGenerator.Generate(false);
            }, webSettingsConfig => {
                //подчистить старые файлы js/css
                Minimizer.DeleteOldFiles(webSettingsConfig.WebPath);
            });
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            Uri url = Request.Url;
            if (url.Host.StartsWith("www.") && !url.IsLoopback) {
                var builder = new UriBuilder(url) {Host = url.Host.Substring(4)};
                PermanentRedirect(builder.Uri.ToString());
            }

            string newUrl = RedirectHelper.GetNewUrl(url.AbsoluteUri);
            if (!string.IsNullOrEmpty(newUrl)) {
                PermanentRedirect(newUrl);
            }
        }

        private void PermanentRedirect(string location) {
            Response.StatusCode = 301;
            Response.AddHeader("Location", location);
            Response.End();
        }

        protected void Application_EndRequest() {
            if (Context.Response.StatusCode == 404) {
                Response.Clear();

                MoveToError("NotFound", null);
            }
        }

        private void MoveToError(string action, HandleErrorInfo model) {
            var rd = new RouteData();
            rd.Values["controller"] = "Errors";
            rd.Values["action"] = action;

            var c = new ErrorsController();
            if (model != null) {
                c.ViewData.Model = model;
            }
            ((IController) c).Execute(new RequestContext(new HttpContextWrapper(Context), rd));
        }

        private void Application_Error(object sender, EventArgs e) {
            HttpContext httpContext = ((MvcApplication) sender).Context;
            Exception ex = Server.GetLastError();
            RouteData currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            /*try {
                var mailer = new Mailer();
                mailer.SendMail("Исключение studyfun.ru", ex.ToString());
            } catch (Exception exception) {}*/
            string currentController = " ";
            string currentAction = " ";
            if (currentRouteData != null) {
                currentController = (currentRouteData.Values["controller"] != null
                                         ? currentRouteData.Values["controller"].ToString()
                                         : " ");
                currentAction = (currentRouteData.Values["action"] != null
                                     ? currentRouteData.Values["action"].ToString()
                                     : " ");
            }
            httpContext.ClearError();
            httpContext.Response.Clear();
            var httpException = ex as HttpException;
            httpContext.Response.StatusCode = httpException != null
                                                  ? httpException.GetHttpCode()
                                                  : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            ExceptionsRegister.Register(WebSettingsConfig.Instance.Domain, ex);

            MoveToError("Unknown", new HandleErrorInfo(ex, currentController, currentAction));
            Response.End();
        }
    }
}