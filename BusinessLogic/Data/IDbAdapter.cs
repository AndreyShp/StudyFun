using System;
using System.Collections.Generic;

namespace BusinessLogic.Data {
    public interface IDbAdapter {
        bool ActionByContext(Action<StudyLanguageContext> action, bool needSaveChanges = false);
        T ReadByContext<T>(Func<StudyLanguageContext, T> action, T defaultValue = default(T));

        bool ExecuteStoredProcedure(string funcName, params object[] parameters);
        List<T> ExecuteStoredProcedure<T>(string funcName, Func<List<object>, T> rowConverter, params object[] parameters);
        bool Transaction(Func<StudyLanguageContext, bool> func);
    }
}