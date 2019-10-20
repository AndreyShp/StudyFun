using System;
using System.Web.Script.Serialization;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;

namespace BusinessLogic.ExternalData.Knowledge {
    public class UserRepetitionIntervalItem {
        internal UserRepetitionIntervalItem(UserRepetitionInterval repetitionInterval,
                                            UserKnowledgeItem data,
                                            DateTime unknownNextTimeToShow) {
            if (repetitionInterval != null) {
                SourceType = (KnowledgeSourceType) repetitionInterval.SourceType;
                NextTimeToShow = repetitionInterval.NextTimeShow;
            } else {
                NextTimeToShow = unknownNextTimeToShow;
            }
            DataId = data.DataId;
            DataType = data.DataType;
            Data = data;
        }

        public UserRepetitionIntervalItem() {}

        public long DataId { get; set; }
        public KnowledgeDataType DataType { get; set; }
        public KnowledgeSourceType SourceType { get; set; }

        public DateTime NextTimeToShow { get; set; }

        [ScriptIgnore]
        public object Data { get; private set; }
    }
}