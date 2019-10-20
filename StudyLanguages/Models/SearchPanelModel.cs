using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models {
    public class SearchPanelModel {
        private readonly object _model;

        public SearchPanelModel(object model) {
            _model = model;
        }

        public string Tip { get; set; }

        public string PatternToSearchContainer { get; set; }

        public Tuple<string, string> AddNewBtn { get; set; }

        public List<Tuple<int, string, bool>> GetItems() {
            return GroupsSorter.GetTypes(HttpContext.Current.Request.Cookies);
        }

        public string GetJsElements() {
            var javaScriptSerializer = new JavaScriptSerializer();
            return javaScriptSerializer.Serialize(_model);
        }
    }
}