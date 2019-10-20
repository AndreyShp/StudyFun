using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class VisualDictionariesController : BaseController {
        public ActionResult Index() {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.VisualDictionary)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();

            var representationsQuery = new RepresentationsQuery(languageId);
            List<RepresentationForUser> representations = representationsQuery.GetVisibleWithoutAreas();
            var sorter = new GroupsSorter(HttpContext.Request.Cookies);
            sorter.Sort(representations);

            List<GroupForUser> convertedRepresentations =
                representations.Select(e => new GroupForUser(e.Id, e.Title, true)).ToList();
            return View(convertedRepresentations);
        }
    }
}