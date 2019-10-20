using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.Sentences {
    public interface ISentencesQuery : IPronunciationQuery {
        SourceWithTranslation GetOrCreate(SentenceType type,
                                          PronunciationForUser source,
                                          PronunciationForUser translation,
                                          byte[] image,
                                          int? rating = null);

        long GetOrCreate(long languageId, string text);

        List<SourceWithTranslation> GetByCount(UserLanguages userLanguages, SentenceType type, int count);

        Dictionary<long, SourceWithTranslation> GetByTranslationsIds(List<long> sentencesTrandlationsIds,
                                                         long sourceLanguageId,
                                                         long translationLanguageId);
    }
}