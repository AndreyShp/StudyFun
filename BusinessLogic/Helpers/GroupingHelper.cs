using System;
using BusinessLogic.Data.Sentence;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Helpers {
    internal class GroupingHelper {
        public static Tuple<PronunciationForUser, PronunciationForUser> GroupByLanguages(long sourceLanguageId,
                                                                                         Word word1,
                                                                                         Word word2) {
            Word source;
            Word translation;
            if (word1.LanguageId == sourceLanguageId) {
                source = word1;
                translation = word2;
            } else {
                source = word2;
                translation = word1;
            }

            return new Tuple<PronunciationForUser, PronunciationForUser>(new PronunciationForUser(source),
                                                                         new PronunciationForUser(translation));
        }
    }
}