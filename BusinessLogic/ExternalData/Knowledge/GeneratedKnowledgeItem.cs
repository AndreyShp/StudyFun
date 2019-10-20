using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;

namespace BusinessLogic.ExternalData.Knowledge {
    public class GeneratedKnowledgeItem : IKnowledgeItem {
        #region IKnowledgeItem Members

        public long DataId { get; set; }
        public KnowledgeDataType DataType { get; set; }
        public object ParsedData { get; set; }

        #endregion
    }
}