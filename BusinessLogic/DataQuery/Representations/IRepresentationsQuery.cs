using System.Collections.Generic;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.ExternalData.Sales;

namespace BusinessLogic.DataQuery.Representations {
    public interface IRepresentationsQuery {
        RepresentationForUser GetOrCreate(RepresentationForUser representationForUser);

        List<RepresentationForUser> GetVisibleWithoutAreas(int count = 0);

        RepresentationForUser GetWithAreas(UserLanguages userLanguages, string title);

        byte[] GetImage(string title);

        long GetId(string title);

        List<SalesItemForUser> GetForSales(UserLanguages userLanguages, ISalesSettings salesSettings);

        List<RepresentationForUser> GetBought(UserLanguages userLanguages, HashSet<long> ids);
    }
}