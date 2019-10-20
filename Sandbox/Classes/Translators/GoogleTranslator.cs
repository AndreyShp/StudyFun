using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Translators {
    internal class GoogleTranslator : ITranslator {
        private bool _isBanned;

        #region ITranslator Members

        public string Name {
            get { return "Google"; }
        }

        public List<string> Translate(LanguageShortName from, LanguageShortName to, string text) {
            if (_isBanned) {
                return null;
            }
            string url = string.Format(
                "http://translate.google.ru/translate_a/t?client=x&text={0}&hl={1}&sl={1}&tl={2}&ie=UTF-8&oe=UTF-8",
                HttpUtility.UrlEncode(text), from.ToString().ToLowerInvariant(), to.ToString().ToLowerInvariant());
            try {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                using (var resp = (HttpWebResponse) request.GetResponse()) {
                    if (resp.StatusCode == HttpStatusCode.OK) {
                        var streamReader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                        string content = streamReader.ReadToEnd();
                        return GetTranslationFromContent(content);
                    }
                    _isBanned = true;
                }
            } catch (Exception) {
                //TODO: логирование
                int y = 0;
            }
            return null;
        }

        #endregion

        private static List<string> GetTranslationFromContent(string content) {
            const string TRANSLATION_KEY = "trans";

            var parsedResponse = new JavaScriptSerializer().Deserialize<dynamic>(content);
            object[] sentences = parsedResponse["sentences"];

/*            if (sentences.Length == 0) {
                return null;
            }*/

            var data = (Dictionary<string, object>) sentences[0];
            /*if (!data.ContainsKey(TRANSLATION_KEY)) {
                return null;
            }
*/
            string translation = data[TRANSLATION_KEY].ToString();
            List<string> result = !string.IsNullOrWhiteSpace(translation)
                                      ? new List<string> {translation.Trim()}
                                      : new List<string>(0);
            return result;
        }
    }
}