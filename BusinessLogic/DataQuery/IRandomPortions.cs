using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery {
    internal interface IRandomPortions {
        List<SourceWithTranslation> GetRandom(long userId,
                                              UserLanguages userLanguages,
                                              int count);

        List<SourceWithTranslation> GetNextPortion(long userId,
                                                   long id,
                                                   UserLanguages userLanguages,
                                                   int count);

        List<SourceWithTranslation> GetPrevPortion(long userId,
                                                   long id,
                                                   UserLanguages userLanguages,
                                                   int countSentences);

        List<SourceWithTranslation> GetExact(long userId, long sourceId, long translationId);
    }
}