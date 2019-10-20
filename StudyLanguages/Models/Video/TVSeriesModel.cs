using System.Collections.Generic;

namespace StudyLanguages.Models.Video {
    public class TVSeriesModel {
        public string Title { get; set; }

        public string OrigTitle { get; set; }

        public IEnumerable<TVSeriesShortModel> Series { get; set; }
    }
}