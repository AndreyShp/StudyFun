using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.DataQuery;
using BusinessLogic.ExternalData;
using StudyLanguages.Configs;

namespace StudyLanguages.Filters {
    public class UserLanguagesAttribute : ActionFilterAttribute {
        private const string COOKIE_NAME = "languages";
        private readonly string _userLanguagesParamName;

        public UserLanguagesAttribute(string userLanguagesParamName = "userLanguages") {
            _userLanguagesParamName = userLanguagesParamName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            List<long> parsedLanguagesIds = GetLanguagesIdsFromCookie(filterContext);
            var languages = new LanguagesQuery(WebSettingsConfig.Instance.DefaultLanguageFrom,
                                               WebSettingsConfig.Instance.DefaultLanguageTo);
            UserLanguages userLanguages = languages.GetLanguages(parsedLanguagesIds);

            filterContext.ActionParameters[_userLanguagesParamName] = userLanguages;
            filterContext.RouteData.Values[_userLanguagesParamName] = userLanguages;
        }

        private static List<long> GetLanguagesIdsFromCookie(ActionExecutingContext filterContext) {
            HttpCookie languagesCookie = filterContext.HttpContext.Request.Cookies[COOKIE_NAME];
            if (languagesCookie == null) {
                return new List<long>(0);
            }
            string[] dirtyLanguages = languagesCookie.Value.Split(new[] {"%3B"}, StringSplitOptions.RemoveEmptyEntries);
            return dirtyLanguages.Select(e => {
                long id;
                if (!long.TryParse(e, out id)) {
                    id = 0;
                }
                return id;
            }).Where(e => e > 0).ToList();
        }
    }
}