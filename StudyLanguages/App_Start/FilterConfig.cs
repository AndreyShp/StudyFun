using System.Web.Mvc;
using StudyLanguages.Filters;

namespace StudyLanguages {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new CacheAttribute());
            filters.Add(new CompressAttribute());
        }
    }
}