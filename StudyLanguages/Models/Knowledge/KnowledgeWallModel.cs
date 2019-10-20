using System.Collections.Generic;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;

namespace StudyLanguages.Models.Knowledge {
    public class KnowledgeWallModel : BaseLanguageModel {
        public KnowledgeWallModel(UserLanguages userLanguages, List<UserKnowledgeItem> items) : base(userLanguages) {
            Items = items;
        }

        public List<UserKnowledgeItem> Items { get; private set; }

        public UserKnowledgeStatistic Statistic { get; set; }
    }
}