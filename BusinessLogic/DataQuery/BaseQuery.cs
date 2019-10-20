using BusinessLogic.Data;

namespace BusinessLogic.DataQuery {
    public abstract class BaseQuery {
        protected internal IDbAdapter Adapter {
            get { return new DbAdapter(); }
        }
    }
}