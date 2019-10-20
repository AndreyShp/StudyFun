using System;
using System.Web.Script.Serialization;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData.Knowledge {
    public class UserKnowledgeItem : IKnowledgeItem {
        internal UserKnowledgeItem(UserKnowledge userKnowledge) {
            Id = userKnowledge.Id;
            DataId = userKnowledge.DataId.HasValue ? userKnowledge.DataId.Value : IdValidator.INVALID_ID;
            DataType = (KnowledgeDataType) userKnowledge.DataType;
            Data = userKnowledge.Data;
            Tip = userKnowledge.Tip;
            CreationDate = userKnowledge.CreationDate;
        }

        public UserKnowledgeItem() {}

        public long Id { get; private set; }
        public string Data { get; set; }
        [ScriptIgnore]
        public string SystemData { get; set; }
        public string Tip { get; set; }
        public DateTime CreationDate { get; set; }

        #region IKnowledgeItem Members

        public long DataId { get; set; }
        public KnowledgeDataType DataType { get; set; }
        [ScriptIgnore]
        public object ParsedData { get; set; }

        #endregion
    }
}