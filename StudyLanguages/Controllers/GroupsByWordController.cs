using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class GroupsByWordController : BaseGroupsController {
        //
        // GET: /Group/
        protected override GroupType GroupType {
            get { return GroupType.ByWord; }
        }

        protected override SectionId SectionId {
            get { return SectionId.GroupByWords; }
        }
    }
}