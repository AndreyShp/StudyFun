using BusinessLogic.Data;
using BusinessLogic.DataQuery;
using BusinessLogic.ExternalData;
using BusinessLogic.Logger;

namespace BusinessLogic.Helpers.Speakers {
    /// <summary>
    /// Создает класс отвечающий за произношение слова
    /// </summary>
    public class SpeakerFactory {
        private readonly ILanguagesQuery _languagesQuery = new LanguagesQuery(LanguageShortName.Unknown,
                                                                              LanguageShortName.Unknown);

        public ISpeaker Create(long languageId) {
            Language language = _languagesQuery.GetByShortName(LanguageShortName.En);
            if (language == null) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "SpeakerFactory.Create не смогли получить язык по имени {0}", LanguageShortName.En);
                return new NullSpeaker();
            }
            return language.Id == languageId ? (ISpeaker) new EnglishSpeaker() : new NullSpeaker();
        }
    }
}