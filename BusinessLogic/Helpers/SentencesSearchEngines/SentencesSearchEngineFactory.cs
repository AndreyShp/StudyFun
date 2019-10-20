using BusinessLogic.Data;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;
using BusinessLogic.Logger;
using NLPWrapper.ExternalObjects;

namespace BusinessLogic.Helpers.SentencesSearchEngines {
    /// <summary>
    /// Создает класс отвечающий за поиск предложений со словами
    /// </summary>
    public class SentencesSearchEngineFactory {
        private readonly ILanguagesQuery _languagesQuery = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);

        public ISentencesSearchEngine Create(long languageId) {
            Language language = _languagesQuery.GetByShortName(LanguageShortName.En);
            if (language == null) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "SentencesSearchEngineFactory.Create не смогли получить язык по имени {0}", LanguageShortName.En);
                return new NullSentencesSearchEngine();
            }

            var sentenceWordsQuery = new SentenceWordsQuery(languageId);
            return language.Id == languageId
                       ? (ISentencesSearchEngine)
                         new SentencesSearchEngine(sentenceWordsQuery, TextAnalyzerFactory.Create())
                       : new NullSentencesSearchEngine();
        }
    }
}