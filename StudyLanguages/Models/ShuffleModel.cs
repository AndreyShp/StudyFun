using System.Collections.Generic;
using BusinessLogic.DataQuery;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models {
    public class ShuffleModel : BaseSeriesModel<SourceWithTranslation> {
        public ShuffleModel(UserLanguages userLanguages, List<SourceWithTranslation> sentenceWithTranslations)
            : base(userLanguages, sentenceWithTranslations) {}

        public int MinCountToLoadPortion {
            get { return BaseRandomQuery.MIN_COUNT; }
        }
    }
}