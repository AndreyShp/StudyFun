using BusinessLogic.Data.Enums.Knowledge;

namespace StudyLanguages.Models.Knowledge {
    public class KnowledgePanelModel {
        public long DataId { get; set; }
        public KnowledgeDataType DataType { get; set; }
        public string ClassName { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMany { get; set; }
    }
}