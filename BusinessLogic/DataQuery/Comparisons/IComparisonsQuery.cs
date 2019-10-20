using System.Collections.Generic;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;

namespace BusinessLogic.DataQuery.Comparisons {
    public interface IComparisonsQuery {
        List<ComparisonForUser> GetVisibleWithoutRules(int count = 0);

        ComparisonForUser GetWithFullInfo(UserLanguages userLanguages, string title);

        ComparisonForUser GetOrCreate(ComparisonForUser comparisonForUser);
    }
}