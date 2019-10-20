using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Text;
using BusinessLogic.Logger;

namespace BusinessLogic.Data {
    public class DbAdapter : IDbAdapter {
        #region IDbAdapter Members

        public bool ActionByContext(Action<StudyLanguageContext> action, bool needSaveChanges = false) {
            return DoByContext(context => {
                action(context);
                if (needSaveChanges) {
                    int numberOfObjects = context.SaveChanges();
                }
                return true;
            }, false);
        }

        public T ReadByContext<T>(Func<StudyLanguageContext, T> func, T defaultValue = default(T)) {
            return DoByContext(func, defaultValue);
        }

        public bool ExecuteStoredProcedure(string funcName, params object[] parameters) {
            bool result = ReadByContext(c => {
                string sql = GetSqlStoredProcedure(funcName, parameters, i => " {" + i + "}");
                c.Database.ExecuteSqlCommand(sql, parameters);
                return true;
            });
            return result;
        }

        public List<T> ExecuteStoredProcedure<T>(string funcName,
                                                 Func<List<object>, T> rowConverter,
                                                 params object[] parameters) {
            List<T> result = ReadByContext(c => {
                DbCommand cmd = c.Database.Connection.CreateCommand();
                cmd.CommandText = GetSqlStoredProcedure(funcName, parameters, i => {
                    DbParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = "@" + i;
                    parameter.Value = parameters[i];
                    cmd.Parameters.Add(parameter);

                    return parameter.ParameterName;
                });

                var innerResult = new List<T>();
                try {
                    c.Database.Connection.Open();
                    DbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read()) {
                        var row = new List<object>();
                        for (int i = 0; i < reader.FieldCount; i++) {
                            row.Add(reader[i]);
                        }
                        innerResult.Add(rowConverter(row));
                    }

                    reader.Close();
                } catch (Exception e) {
                    innerResult = null;
                } finally {
                    c.Database.Connection.Close();
                }
                return innerResult;
            });
            return result;
        }

        public bool Transaction(Func<StudyLanguageContext, bool> func) {
            return DoByContext(context => {
                bool result;
                using (DbContextTransaction transaction = context.Database.BeginTransaction()) {
                    result = func(context);
                    if (result) {
                        transaction.Commit();
                    }
                }
                return result;
            }, false);
        }

        #endregion

        private static string GetSqlStoredProcedure(string funcName,
                                                    object[] parameters,
                                                    Func<int, string> paramNameGetter) {
            var sb = new StringBuilder("exec dbo." + funcName);
            int countElements = parameters != null ? parameters.Length : 0;
            for (int i = 0; i < countElements; i++) {
                if (i > 0) {
                    sb.Append(",");
                }
                sb.Append(" " + paramNameGetter(i));
            }
            string sql = sb.ToString();
            return sql;
        }

        private static T DoByContext<T>(Func<StudyLanguageContext, T> func, T defaultValue) {
            const int MAX_ATTEMPTS = 3;

            int numberAttempt = 1;
            do {
                try {
                    using (var context = new StudyLanguageContext()) {
                        return func(context);
                    }
                } catch (Exception e) {
                    //TODO: возможно отлавливать только определенные исключения
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorException("DbAdapter.DoByContext исключение {0}",
                                                                          e, e);
                }
            } while (++numberAttempt <= MAX_ATTEMPTS);

            return defaultValue;
        }
    }
}