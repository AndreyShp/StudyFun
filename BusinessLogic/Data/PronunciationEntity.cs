using BusinessLogic.ExternalData;

namespace BusinessLogic.Data {
    public class PronunciationEntity : IPronunciation {
        public long Id { get; set; }
        public string Text { get; set; }
        public long LanguageId { get; set; }

        #region IPronunciation Members

        public byte[] Pronunciation { get; set; }

        #endregion
    }
}