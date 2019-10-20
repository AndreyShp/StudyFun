using BusinessLogic.DataQuery.UserRepository.Tasks;

namespace BusinessLogic.DataQuery.UserRepository {
    public class RepositoryFactory {
        private readonly IRepositoryCache _repositoryCache;

        public RepositoryFactory(IRepositoryCache repositoryCache) {
            _repositoryCache = repositoryCache;
        }

        public UserTasksRepository CreateUserRepository(long userId) {
            return new UserTasksRepository(_repositoryCache, userId);
        }

        public BanRepository GetBanRepository() {
            return new BanRepository(_repositoryCache);
        }

        public void Reload() {
            _repositoryCache.Reload();
        }
    }
}