using System;
using System.Collections.Generic;

namespace BusinessLogic.DataQuery {
    public interface IUsersQuery {
        Action<long> OnChangeLastActivity { get; set; }
        long GetByHash(string userHash, string ip);
        long CreateByHash(string userHash, string ip);

        bool RemoveByLastActivity(DateTime maxLastActivity);

        List<long> GetAllUserIds();

        bool UpdateEmail(long id, string email);
    }
}