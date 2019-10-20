using BusinessLogic.ExternalData;

namespace StudyLanguages.Models.Groups {
    /// <summary>
    /// Данные для заполнения пробелов
    /// </summary>
    public class GapsTrainerItem {
        public long Id { get; set; }
        public string TextForUser { get; set; }
        public PronunciationForUser Original { get; set; }
        public PronunciationForUser Translation { get; set; }
    }
}