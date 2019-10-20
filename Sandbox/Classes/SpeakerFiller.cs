using BusinessLogic.Data;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes {
    public class SpeakerFiller {
        public static void Fill() {
            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language language = languages.GetByShortName(LanguageShortName.En);
            long englishLanguageId = language.Id;

            IPronunciationQuery pronunciationQuery = new SentencesQuery();
            pronunciationQuery.FillSpeak(englishLanguageId);

            pronunciationQuery = new WordsQuery();
            pronunciationQuery.FillSpeak(englishLanguageId);
        }
    }
}