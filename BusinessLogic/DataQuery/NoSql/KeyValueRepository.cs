using System;
using System.Collections.Generic;
using System.Linq;
using DBreeze.DataTypes;
using DBreeze.Transactions;

namespace BusinessLogic.DataQuery.NoSql {
    public class KeyValueRepository : IDisposable {
        private DBEngine _engine;

        public KeyValueRepository(DBEngine engine) {
            _engine = engine;
        }

        #region IDisposable Members

        public void Dispose() {
            if (_engine != null) {
                _engine.Dispose();
                _engine = null;
            }
        }

        #endregion

        public bool Set<TKey, TValue>(string tableName, TKey key, TValue value) {
            if (IsOwnType<TValue>()) {
                return SetOwnType(tableName, key, value);
            }
            return _engine.Tran(tran => {
                tran.Insert(tableName, key, value);
                return true;
            });
        }

        private bool SetOwnType<TKey, TValue>(string tableName, TKey key, TValue value) {
            return _engine.Tran(tran => {
                tran.Insert<TKey, DbMJSON<TValue>>(tableName, key, value);
                return true;
            });
        }

        private bool SyncSetOwnType<TKey, TValue>(string tableName,
                                                  TKey key,
                                                  Action<TValue> valueEditor,
                                                  Func<TValue> valueCreator = null) {
            return _engine.Tran(tran => {
                tran.SynchronizeTables(tableName);
                Row<TKey, DbMJSON<TValue>> row = tran.Select<TKey, DbMJSON<TValue>>(tableName, key);
                if (row.Exists) {
                    TValue value = row.Value.Get;
                    if (valueEditor != null) {
                        valueEditor(value);
                        tran.Insert<TKey, DbMJSON<TValue>>(tableName, key, value);
                        return true;
                    }
                    //редактор значения не задан, значит редактировать значения нельзя - возвращаем false
                } else if (valueCreator != null) {
                    TValue newValue = valueCreator();
                    tran.Insert<TKey, DbMJSON<TValue>>(tableName, key, newValue);
                    return true;
                }
                return false;
            });
        }

        public bool SyncSet<TKey, TValue>(string tableName,
                                          TKey key,
                                          Action<TValue> valueEditor,
                                          Func<TValue> valueCreator = null) {
            if (IsOwnType<TValue>()) {
                return SyncSetOwnType(tableName, key, valueEditor, valueCreator);
            }
            return _engine.Tran(tran => {
                tran.SynchronizeTables(tableName);
                Row<TKey, TValue> row = tran.Select<TKey, TValue>(tableName, key);
                if (row.Exists) {
                    TValue value = row.Value;
                    if (valueEditor != null) {
                        valueEditor(value);
                        tran.Insert(tableName, key, value);
                        return true;
                    }
                    //редактор значения не задан, значит редактировать значения нельзя - возвращаем false
                } else if (valueCreator != null) {
                    TValue newValue = valueCreator();
                    tran.Insert(tableName, key, newValue);
                    return true;
                }
                return false;
            });
        }

        public TValue Select<TKey, TValue>(string tableName, TKey key, TValue defaultValue) {
            if (IsOwnType<TValue>()) {
                return SelectOwnType(tableName, key, defaultValue);
            }

            return TranResult(tran => {
                Row<TKey, TValue> row = tran.Select<TKey, TValue>(tableName, key);
                TValue result = defaultValue;
                if (row.Exists) {
                    result = row.Value;
                }
                return result;
            }, defaultValue);
        }

        private TValue SelectOwnType<TKey, TValue>(string tableName, TKey key, TValue defaultValue) {
            return TranResult(tran => {
                Row<TKey, DbMJSON<TValue>> row = tran.Select<TKey, DbMJSON<TValue>>(tableName, key);
                TValue result = defaultValue;
                if (row.Exists) {
                    result = row.Value.Get;
                }
                return result;
            }, defaultValue);
        }

        public Dictionary<TKey, TValue> SelectAsDictionary<TKey, TValue>(string tableName) {
            if (IsOwnType<TValue>()) {
                return SelectAsDictionaryOwnType<TKey, TValue>(tableName);
            }
            return TranResult(tran => tran.SelectDictionary<TKey, TValue>(tableName), null);
        }

        public List<TValue> SelectAll<TKey, TValue>(string tableName, SortOrder sortOrder = SortOrder.Asc) {
            if (IsOwnType<TValue>()) {
                return SelectAllOwnType<TKey, TValue>(tableName, sortOrder);
            }
            return TranResult(tran => {
                IEnumerable<Row<TKey, TValue>> rows = sortOrder == SortOrder.Asc
                                                          ? tran.SelectForward<TKey, TValue>(tableName)
                                                          : tran.SelectBackward<TKey, TValue>(tableName);
                return rows.Where(e => e.Exists).Select(e => e.Value).ToList();
            }, null);
        }

        private List<TValue> SelectAllOwnType<TKey, TValue>(string tableName, SortOrder sortOrder = SortOrder.Asc) {
            return TranResult(tran => {
                IEnumerable<Row<TKey, DbMJSON<TValue>>> rows = sortOrder == SortOrder.Asc
                                                                   ? tran.SelectForward<TKey, DbMJSON<TValue>>(tableName)
                                                                   : tran.SelectBackward<TKey, DbMJSON<TValue>>(
                                                                       tableName);
                return rows.Where(e => e.Exists).Select(e => e.Value.Get).ToList();
            }, null);
        }

        public bool Remove<TKey>(string tableName, TKey key) {
            return _engine.Tran(tran => {
                tran.RemoveKey(tableName, key);
                return true;
            });
        }

        public bool RemoveAll(string tableName) {
            return _engine.Tran(tran => {
                tran.RemoveAllKeys(tableName, false);
                return true;
            });
        }

        private Dictionary<TKey, TValue> SelectAsDictionaryOwnType<TKey, TValue>(string tableName) {
            return
                TranResult(tran => {
                    Dictionary<TKey, DbMJSON<TValue>> res = tran.SelectDictionary<TKey, DbMJSON<TValue>>(tableName);
                    return res.ToDictionary(e => e.Key, e => e.Value.Get);
                }, null);
        }

        private TValue TranResult<TValue>(Func<Transaction, TValue> action, TValue defaultValue) {
            TValue result = defaultValue;
            _engine.Tran(tran => {
                result = action(tran);
                return true;
            });
            return result;
        }

        /*private bool Tran(Action<Transaction> action) {
            return Tran(tran => {
                action(tran);
                return true;
            });
        }*/

        private static bool IsOwnType<TValue>() {
            Type t = typeof (TValue);
            return !t.IsPrimitive && !t.IsValueType && (t != typeof (string));
        }
    }
}