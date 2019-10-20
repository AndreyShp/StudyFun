using BusinessLogic.Data.Enums.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    public interface IKnowledgeItem {
        /// <summary>
        /// Идентификатор данных
        /// </summary>
        long DataId { get; set; }
        /// <summary>
        /// Тип данных
        /// </summary>
        KnowledgeDataType DataType { get; set; }
        /// <summary>
        /// Распарсенные данные
        /// </summary>
        object ParsedData { get; set; }
    }
}