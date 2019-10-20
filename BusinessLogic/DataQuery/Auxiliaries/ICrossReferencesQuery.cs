using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using ExternalCrossReference = BusinessLogic.ExternalData.Auxiliaries.CrossReference;

namespace BusinessLogic.DataQuery.Auxiliaries {
    public interface ICrossReferencesQuery {
        long Add(string source,
                 CrossReferenceType sourceType,
                 string destination,
                 CrossReferenceType destinationType);

        List<ExternalCrossReference> GetReferences(long sourceId, CrossReferenceType sourceType);

        bool RemoveAllExceptIds(HashSet<long> ids);
    }
}