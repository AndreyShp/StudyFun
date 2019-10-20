using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.ExternalData;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models {
    public class PopularItemsModel : BaseSeriesModel<SourceWithTranslation> {
        public PopularItemsModel(UserLanguages userLanguages, List<SourceWithTranslation> sentenceWithTranslations)
            : base(userLanguages, sentenceWithTranslations) {}

        public SpeakerDataType SpeakerDataType { get; set; }
        public KnowledgeDataType KnowledgeDataType { get; set; }
        public string ControllerName { get; set; }
        public string TableHeader { get; set; }
        public string LowerManyElems { get; set; }
        public string LowerOneElem { get; set; }
    }
}