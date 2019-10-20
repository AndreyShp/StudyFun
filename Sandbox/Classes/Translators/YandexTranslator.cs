using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Translators {
    internal class YandexTranslator : ITranslator {
        //TODO: подсчитывать кол-во символов и запросов

        private const string API_KEY =
            "trnsl.1.1.20140315T121811Z.9308a6437f8758dc.cf905ffdfa3a209458986dbe094e8b26d6adddec";

        #region ITranslator Members

        public string Name {
            get { return "Yandex"; }
        }

        public List<string> Translate(LanguageShortName from, LanguageShortName to, string text) {
            string url = GetUrl(from, to, text);
            try {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                using (var resp = (HttpWebResponse) request.GetResponse()) {
                    if (resp.StatusCode == HttpStatusCode.OK) {
                        var streamReader = new StreamReader(resp.GetResponseStream());
                        string content = streamReader.ReadToEnd();
                        return GetTranslationFromContent(content);
                    }
                }
            } catch (Exception) {
                //TODO: логирование
            }
            return null;
        }

        #endregion

        private static string GetUrl(LanguageShortName from, LanguageShortName to, string text) {
            return string.Format(
                "https://translate.yandex.net/api/v1.5/tr.json/translate?key={0}&text={1}&lang={2}-{3}", API_KEY,
                HttpUtility.UrlEncode(text),
                from.ToString().ToLowerInvariant(), to.ToString().ToLowerInvariant());
        }

        private static List<string> GetTranslationFromContent(string content) {
            var parsedResponse = new JavaScriptSerializer().Deserialize<dynamic>(content);
            int code = parsedResponse["code"];
            if (code != 200) {
                return null;
            }

            object[] translation = parsedResponse["text"];
            List<string> result = translation != null
                                      ? translation.Cast<string>().Where(e => !string.IsNullOrWhiteSpace(e)).Select(
                                          e => e.Trim()).ToList()
                                      : new List<string>(0);
            return result;
        }
    }
}