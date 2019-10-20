using System.Collections.Generic;
using System.Linq;
using StudyLanguages.Helpers;
using StudyLanguages.Helpers.Trainer;

namespace StudyLanguages.Models.Groups {
    /// <summary>
    /// Модель для заполнения пробелов
    /// </summary>
    public class GapsTrainerModel {
        public const int MAX_COUNT = 10;

        public GapsTrainerModel(PageRequiredData pageRequiredData, List<GapsTrainerItem> items) {
            PageRequiredData = pageRequiredData;
            Items = items;
        }

        public PageRequiredData PageRequiredData { get; private set; }

        public SpeakerDataType SpeakerDataType { get; set; }
        public List<BreadcrumbItem> BreadcrumbsItems { get; set; }

        public string LoadNextBtnCaption { get; set; }
        public List<GapsTrainerItem> Items { get; private set; }

        public char GapChar {
            get { return GapsTrainerHelper.GAP_CHAR; }
        }
    }
}