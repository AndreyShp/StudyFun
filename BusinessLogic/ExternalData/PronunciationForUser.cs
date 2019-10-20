using BusinessLogic.Data;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData {
    public class PronunciationForUser {
        internal PronunciationForUser(PronunciationEntity pronunciation)
            : this(
                pronunciation.Id, pronunciation.Text,
                EnumerableValidator.IsNotNullAndNotEmpty(pronunciation.Pronunciation), pronunciation.LanguageId) {}

        public PronunciationForUser(long id, string text, bool hasPronunciation, long languageId) {
            Id = id;
            Text = (text ?? string.Empty).Trim();
            HasPronunciation = hasPronunciation;
            LanguageId = languageId;
        }

        public long Id { get; private set; }
        public string Text { get; private set; }
        public bool HasPronunciation { get; private set; }
        public long LanguageId { get; private set; }
    }
}