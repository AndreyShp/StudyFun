using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Helpers.SentencesSearchEngines {
    public class NullSentencesSearchEngine : ISentencesSearchEngine {
        #region ISentencesSearchEngine Members

        public List<string> FindSentences(string words, OrderWordsInSearch orderWordsInSearch) {
            return new List<string>(0);
        }

        #endregion
    }
}