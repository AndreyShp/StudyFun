using System;
using BusinessLogic.DataQuery.NoSql;

namespace BusinessLogic.DataQuery.UserRepository {
    public interface IRepositoryCache {
        KeyValueRepository GetCommonRepository();
        KeyValueRepository GetUserRepository(long userId);
        void Reload();
    }
}