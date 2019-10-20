namespace BusinessLogic.ExternalData {
    public interface ISourceWithTranslation {
        PronunciationForUser Source { get; }
        PronunciationForUser Translation { get; }
        long Id { get; }
    }
}