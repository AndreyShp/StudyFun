using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models.Trainer {
    public class TrainerModel : BaseLanguageModel {
        public TrainerModel(UserLanguages userLanguages)
            : base(userLanguages) {
            Items = new List<TrainerItem>();
        }

        public List<TrainerItem> Items { get; private set; }

        public List<BreadcrumbItem> BreadcrumbsItems { get; set; }

        /*public SectionId MenuActiveItem { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }*/
        public PageRequiredData PageRequiredData { get; set; }
        public string SetMarkUrl { get; set; }

        public void AddItem(TrainerItem item) {
            Items.Add(item);
        }
    }
}