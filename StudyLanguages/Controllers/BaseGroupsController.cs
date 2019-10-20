using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public abstract class BaseGroupsController : BaseController {
        //
        // GET: /Group/

        protected abstract GroupType GroupType { get; }

        protected abstract SectionId SectionId { get; }

        protected List<GroupForUser> GetModel() {
            var languageId = WebSettingsConfig.Instance.GetLanguageFromId();

            IGroupsQuery groupsQuery = new GroupsQuery(languageId);
            List<GroupForUser> groups = groupsQuery.GetVisibleGroups(GroupType);
            var sorter = new GroupsSorter(HttpContext.Request.Cookies);
            sorter.Sort(groups);
            return groups;
        }

        public ActionResult Index() {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            List<GroupForUser> groups = GetModel();
            return View(groups);
        }
    }
}