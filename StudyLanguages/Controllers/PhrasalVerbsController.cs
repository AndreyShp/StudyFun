using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class PhrasalVerbsController : BaseTranslatorController {
        protected override string ControllerName {
            get { return "PhrasalVerbs"; }
        }
        protected override WordType WordType {
            get { return WordType.PhrasalVerb; }
        }
        protected override SectionId SectionId {
            get { return SectionId.PhraseVerbTranslation; }
        }
    }
}