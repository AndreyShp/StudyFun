using System;
using System.Web;
using System.Web.Mvc;

namespace StudyLanguages.Filters {
    public class CacheAttribute : ActionFilterAttribute {
        public CacheAttribute() {
            Duration = TimeSpan.FromDays(365);
            //Endings = new HashSet<string>(new[] {".jpg", ".jpeg", ".png", ".gif", ".ttf", ".js", ".css", "image"});
        }

        public TimeSpan Duration { get; set; }

        /*public HashSet<string> Endings { get; set; }

        private bool ContainsEnding(string ending) {
            return ending != null && Endings.Contains(ending);
        }*/

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (Duration.TotalMilliseconds <= 0) {
                return;
            }

            /* Uri url = filterContext.HttpContext.Request.Url;
            string localPath = url != null
                                   ? url.LocalPath.ToLowerInvariant()
                                   : null;
            if (string.IsNullOrEmpty(localPath)) {
                return;
            }
            if (EnumerableValidator.IsNotNullAndNotEmpty(Endings)) {
                string fileName = Path.GetFileName(localPath);
                string extension = Path.GetExtension(localPath);
                if (!ContainsEnding(fileName) && !ContainsEnding(extension)) {
                    //файл не подходит под искомую маску
                    return;
                }

                //файл подошел под искомую маску - установить время кэша
            }*/

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(Duration));
            cache.SetMaxAge(Duration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }
}