using System.Collections.Generic;

namespace NLPWrapper.ExternalObjects {
    public interface ITextAnalyzer {
        Sentence ParseSentence(string sentence, bool needClear = true);

        List<Sentence> ParseText(string text);
    }
}