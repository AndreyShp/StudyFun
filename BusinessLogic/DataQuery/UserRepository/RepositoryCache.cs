using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using BusinessLogic.DataQuery.NoSql;

namespace BusinessLogic.DataQuery.UserRepository {
    public class RepositoryCache : IRepositoryCache {
        private const string USERS_FOLDER = "Users";
        private const string COMMON_FOLDER = "Common";

        //TODO: вызвать dispose у всех репозиториев при освобожении или перезапуске
        private readonly string _path;
        private readonly object _syncRoot = new object();
        private readonly ConcurrentDictionary<long, KeyValueRepository> _usersRepository =
            new ConcurrentDictionary<long, KeyValueRepository>();
        private volatile KeyValueRepository _commonRepository;

        public RepositoryCache(string path) {
            _path = path;
        }

        #region IRepositoryCache Members

        public KeyValueRepository GetCommonRepository() {
            if (_commonRepository == null) {
                lock (_syncRoot) {
                    if (_commonRepository == null) {
                        _commonRepository = new KeyValueRepository(new DBEngine(Path.Combine(_path, COMMON_FOLDER)));
                    }
                }
            }
            return _commonRepository;
        }

        public KeyValueRepository GetUserRepository(long userId) {
            //TODO: добавить кэширование репозитория для пользователя
            string fullPathName = GetFullPathName(_path, userId);
            KeyValueRepository repository = _usersRepository.GetOrAdd(userId, key => {
                var engine = new DBEngine(fullPathName);
                return new KeyValueRepository(engine);
            });
            return repository;
        }

        public void Reload() {
            if (_commonRepository != null) {
                lock (_syncRoot) {
                    if (_commonRepository != null) {
                        _commonRepository.Dispose();
                        _commonRepository = null;
                    }
                }
            }

            foreach (var pair in _usersRepository) {
                pair.Value.Dispose();
            }
            _usersRepository.Clear();
        }

        #endregion

        private static string GetFullPathName(string path, long userId) {
            //TODO: смотреть есть ли пользователь, то брать его repository из кэша 
            string folder = userId.ToString(CultureInfo.InvariantCulture);
            /*if (isVirtualPath) {
                string subfolder = VirtualPathUtility.Combine(path, SUBFOLDER);
                fullPathName = VirtualPathUtility.Combine(subfolder, folder);
            } else {*/
            string fullPathName = Path.Combine(path, USERS_FOLDER, folder);
            /*}*/
            return fullPathName;
        }
    }
}