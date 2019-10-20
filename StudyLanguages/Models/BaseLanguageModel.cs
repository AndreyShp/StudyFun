using System.Web.Script.Serialization;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models {
    public abstract class BaseLanguageModel {
        protected BaseLanguageModel(UserLanguages userLanguages) {
            var javaScriptSerializer = new JavaScriptSerializer();
            JsLanguageFrom = javaScriptSerializer.Serialize(userLanguages.From);
            JsLanguageTo = javaScriptSerializer.Serialize(userLanguages.To);
        }

        public string JsLanguageFrom { get; private set; }
        public string JsLanguageTo { get; private set; }
    }
}