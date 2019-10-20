using System;

namespace BusinessLogic.Data.Enums {
    /// <summary>
    /// Источник предложения для пазла
    /// </summary>
    [Flags]
    public enum PuzzleSentenceSource : byte {
        /// <summary>
        /// Текст
        /// </summary>
        Text = 1,

        /// <summary>
        /// Видео
        /// </summary>
        Video = 2,

        /// <summary>
        /// Аудио
        /// </summary>
        Audio = 4,
    }
}