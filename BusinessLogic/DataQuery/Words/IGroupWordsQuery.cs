using System.Collections.Generic;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.Words {
    public interface IGroupWordsQuery {
        List<SourceWithTranslation> GetWordsByGroup(UserLanguages userLanguages, long groupId);

        SourceWithTranslation GetOrCreate(GroupForUser groupForUser,
                                          PronunciationForUser source,
                                          PronunciationForUser translation,
                                          byte[] image,
                                          int? rating);

        Dictionary<long, List<SourceWithTranslation>> GetForAllGroups(UserLanguages userLanguages);
    }
}