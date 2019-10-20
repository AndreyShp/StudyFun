using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using StudyLanguages.Configs;

namespace StudyLanguages.Filters {
    public class SelectedGroupAttribute : ActionFilterAttribute {
        private readonly GroupType _type;
        private readonly string _uniqueParamName;

        public SelectedGroupAttribute(GroupType type, string uniqueParamName = "group") {
            _type = type;
            _uniqueParamName = uniqueParamName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            GroupForUser groupForUser = GetGroupForUser(filterContext);
            filterContext.ActionParameters[_uniqueParamName] = groupForUser;
            filterContext.RouteData.Values[_uniqueParamName] = groupForUser;
        }

        private GroupForUser GetGroupForUser(ActionExecutingContext filterContext) {
            var groupName = filterContext.RouteData.Values[RouteConfig.GROUP_PARAM_NAME] as string;
            if (string.IsNullOrWhiteSpace(groupName)) {
                return null;
            }

            var languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            var groupsQuery = new GroupsQuery(languageId);
            return groupsQuery.GetVisibleGroupByName(_type, groupName);
        }
    }
}