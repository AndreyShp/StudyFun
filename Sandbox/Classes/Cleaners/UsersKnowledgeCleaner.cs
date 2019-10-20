using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Cleaners {
    /// <summary>
    /// Класс отвечает за удаления знаний пользователя, которые пользователь удалил
    /// </summary>
    public class UsersKnowledgeCleaner {
        public bool RemoveDeleted(List<long> userIds) {
            bool result = true;
            int i = 1;

            var languagesQuery = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            var languagesIds = Enum.GetValues(typeof (LanguageShortName)).Cast<LanguageShortName>()
                .Where(e => languagesQuery.GetByShortName(e) != null)
                .Select(e => languagesQuery.GetByShortName(e).Id).ToList();
            foreach (long userId in userIds) {
                foreach (var languageId in languagesIds) {
                    IUserKnowledgeQuery userKnowledgeQuery = new UserKnowledgeQuery(userId, languageId);
                    if (!userKnowledgeQuery.RemoveDeleted()) {
                        Console.WriteLine("Не удалось удалить знания пользователя {0} по языку {1}!", userId);
                        result = false;
                    }
                }
                if (i % 100 == 0) {
                    Console.WriteLine("Удалены старые данные для {0} пользователей из {1}!", i, userIds.Count);
                }
                i++;
            }
            return result;
        }
    }
}