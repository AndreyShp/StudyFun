using System;
using System.Collections.Specialized;
using System.Text;

namespace StudyLanguages.Helpers {
    public class HttpContextHelper {
        public static string ParamsToString(NameValueCollection pars, Func<string, bool> nameValidator) {
            var result = new StringBuilder();
            foreach (string key in pars.Keys) {
                if (!nameValidator(key)) {
                    continue;
                }

                if (result.Length > 0) {
                    result.Append(";");
                }
                result.AppendFormat("{0}={1}", key, pars.Get(key));
            }
            return result.ToString();
        }
    }
}