using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Auxiliaries {
    internal class IdNameCache {
        private readonly Func<IEnumerable<Tuple<long, string>>> _dataGetter;
        private readonly ConcurrentDictionary<string, long> _idsCacheByNames = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<long, string> _namesCacheByIds = new ConcurrentDictionary<long, string>();

        public IdNameCache(Func<IEnumerable<Tuple<long, string>>> dataGetter) {
            _dataGetter = dataGetter;
        }

        private void Update() {
            IEnumerable<Tuple<long, string>> data = _dataGetter();
            if (data == null) {
                return;
            }
            foreach (var tuple in data) {
                long id = tuple.Item1;
                string name = tuple.Item2;
                _idsCacheByNames.AddOrUpdate(name, id, (k, old) => id);
                _namesCacheByIds.AddOrUpdate(id, name, (k, old) => name);
            }
        }

        public string GetName(long id) {
            string result;
            if (!_namesCacheByIds.TryGetValue(id, out result)) {
                Update();
                _namesCacheByIds.TryGetValue(id, out result);
            }
            return result;
        }

        public long GetId(string name) {
            long result;
            if (!_idsCacheByNames.TryGetValue(name, out result)) {
                Update();
                if (!_idsCacheByNames.TryGetValue(name, out result)) {
                    result = IdValidator.INVALID_ID;
                }
            }
            return result;
        }
    }
}