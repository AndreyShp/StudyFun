using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Knowledge;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Cleaners {
    /// <summary>
    /// Класс отвечает за удаления данных о повторениях, для данных, которые уже нет у пользователя
    /// </summary>
    public class UsersRepetitionIntervalCleaner {
        public bool RemoveWithoutData(List<long> userIds) {
            bool result = true;
            int i = 1;

            var languagesQuery = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            List<long> languagesIds =
                Enum.GetValues(typeof(LanguageShortName)).Cast<LanguageShortName>().Where(e => languagesQuery.GetByShortName(e) != null).Select(
                    e => languagesQuery.GetByShortName(e).Id).ToList();

            foreach (long userId in userIds) {
                foreach (long languageId in languagesIds) {
                    IUserRepetitionKnowledgeQuery repetitionQuery = new UserRepetitionKnowledgeQuery(userId, languageId,
                                                                                                     KnowledgeDataType.
                                                                                                         All);
                    IUserRepetitionIntervalQuery userRepetitionIntervalQuery = new UserRepetitionIntervalQuery(userId,
                                                                                                               languageId,
                                                                                                               KnowledgeSourceType
                                                                                                                   .
                                                                                                                   Knowledge,
                                                                                                               repetitionQuery,
                                                                                                               RepetitionType
                                                                                                                   .All);
                    if (!userRepetitionIntervalQuery.RemoveWithoutData()) {
                        Console.WriteLine(
                            "Не удалось удалить данные о повторениях для пользователя {0} для языка {1}!", userId,
                            languageId);
                        result = false;
                    }
                }
                if (i % 100 == 0) {
                    Console.WriteLine("Удалены данные о повторениях для {0} несуществующих пользователей из {1}!", i,
                                      userIds.Count);
                }
                i++;
            }
            return result;
        }
    }
}