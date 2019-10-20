using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StudyLanguages.App_Start {
    public class PrettyRouteConfig : Route {
        /*private readonly Dictionary<string, string> _controllers = new Dictionary<string, string> {};

        public PrettyRouteConfig(string url)
            : base(url, new MvcRouteHandler()) {
            Init();
        }

        public PrettyRouteConfig(string url, RouteValueDictionary defaults)
            : base(url, defaults, new MvcRouteHandler()) {
            Init();
        }

        public PrettyRouteConfig(string url,
                                 RouteValueDictionary defaults,
                                 RouteValueDictionary constraints)
            : base(url, defaults, constraints, new MvcRouteHandler()) {
            Init();
        }

        public PrettyRouteConfig(string url,
                                 RouteValueDictionary defaults,
                                 RouteValueDictionary constraints,
                                 RouteValueDictionary dataTokens)
            : base(url, defaults, constraints, dataTokens, new MvcRouteHandler()) {
            Init();
        }*/

        public PrettyRouteConfig(string url, object defaults) :
            base(url, new RouteValueDictionary(defaults), new MvcRouteHandler()) {
            Init();
        }

        public PrettyRouteConfig(string url, object defaults, object constraints) :
            base(
            url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints),
            new MvcRouteHandler()) {
            Init();
        }

        private void Init() {}

        public override RouteData GetRouteData(HttpContextBase httpContext) {
            RouteData routeData = base.GetRouteData(httpContext);
            ConvertFromPretty(routeData);
            /*values["controller"] = controllerName.Replace("-", "_");
            values["action"] = (values["action"] as string)
                .Replace("-", "_");*/
            return routeData;
        }

        public override VirtualPathData GetVirtualPath(
            RequestContext ctx, RouteValueDictionary values) {
            ConvertToPretty(values);
            /*values["controller"] = (values["controller"] as string)
                .Replace("_", "-").ToLower();
            values["action"] = (values["action"] as string)
                .Replace("_", "-").ToLower();*/
            return base.GetVirtualPath(ctx, values);
        }

        private void ConvertToPretty(RouteValueDictionary values) {
            var controllerName = values["controller"] as string;
            var action = values["action"] as string;
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(action)) {
                return;
            }
            /*if (controllerName == RouteConfig.VISUAL_DICTIONARY_CONTROLLER) {
                controllerName = RouteConfig.PRETTY_VISUAL_DICTIONARY_CONTROLLER;
            } else if (controllerName == RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME) {
                controllerName = RouteConfig.PRETTY_VISUAL_DICTIONARIES_CONTROLLER_NAME;
            }
*/
            //TODO: проверка на редиректы
            values["controller"] = controllerName;
            values["action"] = action;
        }

        private void ConvertFromPretty(RouteData routeData) {
            //TODO: проверка на редиректы
            if (routeData == null) {
                return;
            }
            RouteValueDictionary values = routeData.Values;
            var controllerName = values["controller"] as string;
            var action = values["action"] as string;
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(action)) {
                return;
            }
            /*if (controllerName == RouteConfig.PRETTY_VISUAL_DICTIONARY_CONTROLLER) {
                controllerName = RouteConfig.VISUAL_DICTIONARY_CONTROLLER;
            } else if (controllerName == RouteConfig.PRETTY_VISUAL_DICTIONARIES_CONTROLLER_NAME) {
                controllerName = RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME;
            }*/

            //TODO: проверка на редиректы
            values["controller"] = controllerName;
            values["action"] = action;
        }
    }
}