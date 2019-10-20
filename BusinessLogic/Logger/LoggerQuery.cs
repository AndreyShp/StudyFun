using System;
using BusinessLogic.DataQuery;

namespace BusinessLogic.Logger {
    public class LoggerQuery : BaseQuery, ICleaner {
        /// <summary>
        /// Добавляет запись в таблицу логов
        /// </summary>
        /// <param name="type">тип сообщения</param>
        /// <param name="message">текст сообщения</param>
        /// <returns>true - запись залогирована в БД успешно, false запись не удалось залогировать в БД</returns>
        public bool Create(LoggingType type, string message) {
            return Adapter.ActionByContext(c => {
                var logEntry = new LogEntry {
                    MessageType = (byte) type,
                    Message = message
                };
                c.LogEntry.Add(logEntry);
                c.SaveChanges();
            });
        }

        /// <inheritdoc />
        public bool Clean(DateTime maxDateForRemove) {
            var res = Adapter.ActionByContext(c => {
                                                  const string SQL_COMMAND = "delete FROM [LogEntry]";
                                                  c.Database.ExecuteSqlCommand(SQL_COMMAND);
                                              });
            return res;
        }
    }
}