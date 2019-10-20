using System.Collections.Generic;
using BusinessLogic.Data.Sentence;
using BusinessLogic.ExternalData;
using NLPWrapper.ExternalObjects.Enums;

namespace BusinessLogic.DataQuery.Sentences {
    public interface ISentenceWordsQuery {
        bool CreateOrUpdate(long sentenceId,
                            long wordId,
                            string originalText,
                            int orderInSentence,
                            GrammarWordType grammarWordType);

        bool DeleteGreaterOrEqualsOrderWords(long sentenceId, int orderInSentence);

        List<Sentence> FindSentencesByWords(List<string> words,
                                            OrderWordsInSearch orderWordsInSearch,
                                            int maxCount = 5);

        SentenceWithWords GetWordsBySentenceId(long sentenceId);
    }
}