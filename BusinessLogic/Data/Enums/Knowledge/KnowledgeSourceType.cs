using System;

namespace BusinessLogic.Data.Enums.Knowledge {
    /// <summary>
    /// Тип источника откуда взяли данные
    /// </summary>
    [Flags]
    public enum KnowledgeSourceType {
        /// <summary>
        /// Знания
        /// </summary>
        Knowledge = 1,
        /// <summary>
        /// Слова по теме
        /// </summary>
        GroupWord = 2,
        /// <summary>
        /// Фразы по теме
        /// </summary>
        GroupPhrase = 4,
        /// <summary>
        /// Визуальный словарь по теме
        /// </summary>
        VisualDictionary = 8,
    }
}