using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery {
    public interface IPronunciationQuery {
        IPronunciation GetById(long id);

        void FillSpeak(long languageId);
    }
}