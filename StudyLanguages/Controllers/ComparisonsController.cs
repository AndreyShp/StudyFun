using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using StudyLanguages.Configs;
using StudyLanguages.Helpers;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class ComparisonsController : BaseController {
        public ActionResult Index() {
            if (WebSettingsConfig.Instance.IsSectionForbidden(SectionId.FillDifference)) {
                return RedirectToAction("Index", RouteConfig.MAIN_CONTROLLER_NAME);
            }

            var languageId = WebSettingsConfig.Instance.GetLanguageFromId();

            var comparisonsQuery = new ComparisonsQuery(languageId);
            List<ComparisonForUser> comparisons = comparisonsQuery.GetVisibleWithoutRules();
            var sorter = new GroupsSorter(HttpContext.Request.Cookies);
            sorter.Sort(comparisons);

            return View(comparisons);
        }
    }
}