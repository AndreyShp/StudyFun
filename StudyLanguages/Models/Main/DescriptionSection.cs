using System;
using System.Collections.Generic;

namespace StudyLanguages.Models.Main {
    public class DescriptionSection {
        public DescriptionTitle Title { get; set; }
        public string Description { get; set; }
        public string MostPopularTitle { get; set; }
        public List<DescriptionSectionItem> Items { get; set; }

        public Func<DescriptionSectionItem, string> UrlItemGetter { get; set; }
        public Func<DescriptionSectionItem, string> UrlImageItemGetter { get; set; }
        public Func<DescriptionSectionItem, string> TitleItemGetter { get; set; }
    }
}