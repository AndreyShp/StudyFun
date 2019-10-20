using System;

namespace BusinessLogic.Data.Enums {
    /// <summary>
    /// Тип данных в таблице сериалов
    /// </summary>
    public enum TVSeriesDataType : byte {
        /// <summary>
        /// Обложка
        /// </summary>
        Cover = 1,

        /// <summary>
        /// Сезон
        /// </summary>
        Season = 2,

        /// <summary>
        /// Серия
        /// </summary>
        Series = 3,
    }
}