using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;

namespace StudyLanguages.Filters {
    public class UserIdAttribute : ActionFilterAttribute {
        private const string COOKIE_NAME = "uniqueUser";
        private readonly bool _needCreate;

        private readonly string _uniqueParamName;
        private readonly UserUniqueId _userUniqueId = new UserUniqueId();
        private readonly IUsersQuery _usersQuery;

        public UserIdAttribute(bool needCreate = false, string uniqueParamName = "userId") {
            _needCreate = needCreate;
            _uniqueParamName = uniqueParamName;
            _usersQuery = new UsersQuery();
            _usersQuery.OnChangeLastActivity = userId => Task.Factory.StartNew(() => {
                try {
                    long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
                    IUserKnowledgeQuery userKnowledgeQuery = new UserKnowledgeQuery(userId, languageId);
                    userKnowledgeQuery.RemoveDeleted();

                    var repetitionQuery = new UserRepetitionKnowledgeQuery(userId, languageId, KnowledgeDataType.All);
                    IUserRepetitionIntervalQuery userRepetitionIntervalQuery =
                        new UserRepetitionIntervalQuery(userId, languageId,
                                                        KnowledgeSourceType.Knowledge,
                                                        repetitionQuery, RepetitionType.All);
                    userRepetitionIntervalQuery.RemoveWithoutData();
                } catch (Exception e) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "UserIdAttribute.Constructor. СЮДА НЕ ДОЛЖНЫ БЫЛИ ПОПАСТЬ! При вызове для пользователя с идентификатором {0} возникло исключение {1}!",
                        userId, e);
                }
            });
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            long userId = GetUserId(filterContext.HttpContext);

            filterContext.ActionParameters[_uniqueParamName] = userId;
            filterContext.RouteData.Values[_uniqueParamName] = userId;
        }

        public string GetUserUniqueIdFromCookie(HttpContextBase httpContext) {
            HttpCookie cookie = httpContext.Request.Cookies[COOKIE_NAME];

            string result = null;
            if (cookie != null) {
                result = cookie.Value != null ? cookie.Value.ToLower() : null;
                if (!_userUniqueId.IsValid(result)) {
                    result = null;
                }

                if (result != null
                    && !string.Equals(WebSettingsConfig.Instance.CookieWideDomain, cookie.Domain,
                                      StringComparison.InvariantCultureIgnoreCase)) {
                    //кука весит не на том домене - перевесить куку
                    AddCookie(httpContext, result);
                }
            }
            return result;
        }

        public long GetUserId(HttpContextBase httpContext) {
            string userUniqueId = GetUserUniqueIdFromCookie(httpContext);
            bool needCreateNewUser = string.IsNullOrEmpty(userUniqueId) && _needCreate;
            if (needCreateNewUser) {
                userUniqueId = GenerateNewUserUnique(httpContext);
            }

            if (string.IsNullOrEmpty(userUniqueId)) {
                return IdValidator.INVALID_ID;
            }

            string ip = RemoteClientHelper.GetClientIpAddress(httpContext.Request);
            long userId = needCreateNewUser
                              ? _usersQuery.CreateByHash(userUniqueId, ip)
                              : _usersQuery.GetByHash(userUniqueId, ip);
            if (IdValidator.IsInvalid(userId) && !needCreateNewUser) {
                //нужно было получить по идентификатору, но не получили - создать аккаунт
                userId = _usersQuery.CreateByHash(userUniqueId, ip);
            }

            return userId;
        }

        public bool SetNewCookie(HttpContextBase httpContext, string newCookieValue) {
            if (!_userUniqueId.IsValid(newCookieValue)) {
                return false;
            }

            string ip = RemoteClientHelper.GetClientIpAddress(httpContext.Request);
            long userId = _usersQuery.GetByHash(newCookieValue, ip);
            if (IdValidator.IsInvalid(userId)) {
                return false;
            }

            HttpCookie cookie = httpContext.Response.Cookies[COOKIE_NAME];
            if (cookie == null) {
                return false;
            }

            cookie.Value = newCookieValue;
            httpContext.Response.Cookies[COOKIE_NAME].Value = newCookieValue;

            return true;
        }

        private string GenerateNewUserUnique(HttpContextBase httpContext) {
            string result = _userUniqueId.New();
            AddCookie(httpContext, result);
            return result;
        }

        private static void AddCookie(HttpContextBase context, string result) {
            context.Response.Cookies.Add(
                new HttpCookie(COOKIE_NAME, result) {
                    Expires = DateTime.MaxValue,
                    Domain = WebSettingsConfig.Instance.CookieWideDomain
                });
        }
    }
}