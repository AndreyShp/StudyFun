using System;
using BusinessLogic.DataQuery;
using StudyLanguages.Helpers;

namespace Sandbox.Classes.Cleaners {
    /// <summary>
    /// Класс отвечает за удаления пользователей, которые давно не заходили в систему
    /// </summary>
    public class UsersCleaner {
        public bool RemoveOld() {
            IUsersQuery usersQuery = new UsersQuery();
            DateTime maxLastActivity = DateTime.Today.AddDays(-CommonConstants.COUNT_DAYS_TO_HOLD_DATA);
            if (!usersQuery.RemoveByLastActivity(maxLastActivity)) {
                Console.WriteLine("Не удалось удалить пользователей, которые последний раз заходили ранее {0}!",
                                  maxLastActivity);
                return false;
            }
            return true;
        }
    }
}