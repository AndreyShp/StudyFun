using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Helpers.SentencesSearchEngines {
    public interface ISentencesSearchEngine {
        List<string> FindSentences(string words, OrderWordsInSearch orderWordsInSearch);
    }
}