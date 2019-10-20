using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;

namespace StudyLanguages.Models {
    public class VisualDictionaryModel : BaseLanguageModel {
        public VisualDictionaryModel(UserLanguages userLanguages, RepresentationForUser representation)
            : base(userLanguages) {
            Representation = representation;
        }

        public RepresentationForUser Representation { get; private set; }

        public CrossReferencesModel CrossReferencesModel { get; set; }
    }
}