using System;
using DBreeze;
using DBreeze.Transactions;

namespace BusinessLogic.DataQuery.NoSql {
    public class DBEngine : IDisposable {
        private volatile DBreezeEngine _engine;

        private readonly string _fullFileName;
        private readonly object _sync = new object();

        public DBEngine(string fullFileName) {
            _fullFileName = fullFileName;
        }

        #region IDisposable Members

        public void Dispose() {
            if (_engine != null) {
                _engine.Dispose();
                _engine = null;
            }
        }

        #endregion

        private void CreateDbIfNeed() {
            if (_engine != null) {
                return;
            }

            lock (_sync) {
                if (_engine == null) {
                    _engine = new DBreezeEngine(_fullFileName);
                }
            }
        }

        public bool Tran(Func<Transaction, bool> action) {
            try {
                CreateDbIfNeed();
                using (Transaction tran = _engine.GetTransaction(eTransactionTablesLockTypes.SHARED)) {
                    if (action(tran)) {
                        tran.Commit();
                        return true;
                    }
                    tran.Rollback();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}