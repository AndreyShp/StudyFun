using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.Sentences {
    public interface IGroupSentencesQuery {
        List<SourceWithTranslation> GetSentencesByGroup(UserLanguages userLanguages, long groupId);

        SourceWithTranslation GetOrCreate(GroupForUser groupForUser,
                                          PronunciationForUser source,
                                          PronunciationForUser translation,
                                          byte[] image,
                                          int? rating);

        Dictionary<long, List<SourceWithTranslation>> GetForAllGroups(UserLanguages userLanguages);
    }
}