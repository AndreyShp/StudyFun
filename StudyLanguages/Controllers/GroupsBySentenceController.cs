using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class GroupsBySentenceController : BaseGroupsController {
        //
        // GET: /Group/
        protected override GroupType GroupType {
            get { return GroupType.BySentence; }
        }

        protected override SectionId SectionId {
            get { return SectionId.GroupByPhrases; }
        }
    }
}