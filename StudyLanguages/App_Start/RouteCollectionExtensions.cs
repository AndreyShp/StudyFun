using System.Web.Routing;

namespace StudyLanguages.App_Start {
    public static class RouteCollectionExtensions {
        public static Route MapPrettyRoute(this RouteCollection routes,
                                           string name,
                                           string url,
                                           object defaults) {
            var route = new PrettyRouteConfig(url, defaults);
            routes.Add(name, route);
            return route;
        }

        public static Route MapPrettyRoute(this RouteCollection routes,
                                           string name,
                                           string url,
                                           object defaults,
                                           object constraints) {
            var route = new PrettyRouteConfig(url, defaults, constraints);
            routes.Add(name, route);
            return route;
        }
    }
}