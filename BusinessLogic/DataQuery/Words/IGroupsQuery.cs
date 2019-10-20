using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.Words {
    public interface IGroupsQuery {
        List<GroupForUser> GetVisibleGroups(GroupType type, int count = 0);

        GroupForUser GetVisibleGroupByName(GroupType type, string name);

        GroupForUser GetOrCreate(GroupType type, string name, byte[] image, int? rating = null);
    }
}