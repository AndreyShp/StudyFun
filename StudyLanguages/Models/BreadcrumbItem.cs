using System.Web.Routing;

namespace StudyLanguages.Models {
    public class BreadcrumbItem {
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public string Action { get; set; }

        public bool IsActive { get; set; }
        public string Html { get; set; }

        public bool WithoutDelimiter { get; set; }

        public string LiClasses { get; set; }

        public object RouteValues { get; set; }

        public object HtmlAttributes { get; set; }
    }
}