using System;

namespace BusinessLogic.DataQuery {
    public interface ICleaner {
        bool Clean(DateTime maxDateForRemove);
    }
}