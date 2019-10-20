namespace BusinessLogic.Helpers.Speakers {
    /// <summary>
    /// Класс не умеет конвертировать текст в аудио формат
    /// </summary>
    internal class NullSpeaker : ISpeaker {
        #region ISpeaker Members

        /// <summary>
        /// Конвертировать текст в фафл WAV
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns>WAV файл в виде набора байтов</returns>
        public byte[] ConvertTextToAudio(string text) {
            return null;
        }

        #endregion
    }
}