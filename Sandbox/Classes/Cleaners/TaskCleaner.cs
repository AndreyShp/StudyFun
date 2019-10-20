using System;
using System.Collections.Generic;
using BusinessLogic;
using BusinessLogic.DataQuery;

namespace Sandbox.Classes.Cleaners {
    public class TaskCleaner {
        public static void Clear() {
            //удалить старых пользователей
            var usersCleaner = new UsersCleaner();
            bool result = usersCleaner.RemoveOld();

            IUsersQuery usersQuery = new UsersQuery();
            List<long> userIds = usersQuery.GetAllUserIds();

            var userKnowledgeCleaner = new UsersKnowledgeCleaner();
            result &= userKnowledgeCleaner.RemoveDeleted(userIds);

            var usersRepetitionIntervalCleaner = new UsersRepetitionIntervalCleaner();
            result &= usersRepetitionIntervalCleaner.RemoveWithoutData(userIds);

            if (result) {
                Console.WriteLine("Все что хотели удалить - все удалили!");
            } else {
                Console.WriteLine("Не все удалили! При удалении что-то пошло не так:(");
            }
            Console.ReadLine();
        }
    }
}