using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Models.Groups {
    public class GroupInfo {
        public string Alt { get; set; }
        public string LowerManyElems { get; set; }
        public string LowerOneElem { get; set; }
        public string TableHeader { get; set; }

        public string ControllerName { get; set; }
        public string BaseControllerName { get; set; }
        public string UserTrainerControllerName { get; set; }
        public string PrettyControllerName { get; set; }

        public SectionId SectionId { get; set; }
    }
}