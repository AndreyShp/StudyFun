using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace StudyLanguages.App_Start {
    public class RegexConstraint : IRouteConstraint {
        private readonly Regex _regex;

        public RegexConstraint(string pattern,
                               RegexOptions options =
                                   RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase) {
            _regex = new Regex(pattern, options);
        }

        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext,
                          Route route,
                          string parameterName,
                          RouteValueDictionary values,
                          RouteDirection routeDirection) {
            object val;
            values.TryGetValue(parameterName, out val);
            string input = Convert.ToString(val, CultureInfo.InvariantCulture);
            var result = _regex.IsMatch(input);
            return result;
        }

        #endregion
    }
}