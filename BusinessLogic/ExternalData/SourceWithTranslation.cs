using BusinessLogic.Data;

namespace BusinessLogic.ExternalData {
    public class SourceWithTranslation : ISeries, ISourceWithTranslation {
        public PronunciationForUser Source { get; private set; }
        public PronunciationForUser Translation { get; private set; }

        public long Id { get; private set; }

        public bool HasImage { get; set; }

        #region ISeries Members

        public bool IsCurrent { get; set; }

        #endregion

        internal void Set(long id, PronunciationEntity source, PronunciationEntity translation) {
            Set(id, new PronunciationForUser(source), new PronunciationForUser(translation));
        }

        public void Set(long id, PronunciationForUser source, PronunciationForUser translation) {
            Id = id;
            Source = source;
            Translation = translation;
            IsCurrent = false;
        }
    }
}