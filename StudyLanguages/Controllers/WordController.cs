using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class WordController : BaseTranslatorController {
        protected override string ControllerName {
            get { return "Word"; }
        }
        protected override WordType WordType {
            get { return WordType.Default; }
        }
        protected override SectionId SectionId {
            get { return SectionId.WordTranslation; }
        }
    }
}