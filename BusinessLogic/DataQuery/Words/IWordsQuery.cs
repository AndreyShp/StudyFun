using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Words;

namespace BusinessLogic.DataQuery.Words {
    public interface IWordsQuery : IPronunciationQuery {
        WordsByPattern GetLikeWords(UserLanguages userLanguages, string likePattern, WordType wordType);

        List<PronunciationForUser> GetTranslations(UserLanguages userLanguages, string query, WordType wordType);

        bool Create(WordWithTranslation wordWithTranslation);

        long GetOrCreate(long languageId,
                         string text,
                         WordType wordType = WordType.Default);

        WordWithTranslation GetOrCreate(PronunciationForUser source,
                                        PronunciationForUser translation,
                                        byte[] image,
                                        WordType wordType,
                                        int? rating);

        long GetIdByWordsForUser(PronunciationForUser source, PronunciationForUser translation);

        bool IsInvalid(PronunciationForUser wordForUser);

        Dictionary<long, SourceWithTranslation> GetByTranslationsIds(List<long> wordsTrandlationsIds,
                                                         long sourceLanguageId,
                                                         long translationLanguageId);
    }
}