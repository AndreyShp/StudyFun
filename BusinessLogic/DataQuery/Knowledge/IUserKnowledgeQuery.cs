using System;
using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Data.Knowledge;
using BusinessLogic.ExternalData.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    public interface IUserKnowledgeQuery {
        /// <summary>
        /// Получает данные пользователя
        /// </summary>
        /// <param name="sourceLanguageId">идентификатор языка, с которого переводят</param>
        /// <param name="translationLanguageId">идентификатор языка, на который переводят</param>
        /// <param name="status">статус данных, которые нужно получить</param>
        /// <param name="prevId">максимальный идентификатор предыдущей записи</param>
        /// <param name="count">кол-во записей для получения</param>
        /// <returns></returns>
        List<UserKnowledgeItem> GetData(long sourceLanguageId,
                                        long translationLanguageId,
                                        KnowledgeStatus status,
                                        long prevId,
                                        int count);

        /// <summary>
        /// Добавляет "новые пункты знаний" пользователю
        /// </summary>
        /// <param name="knowledgeItems">новые пункты знаний</param>
        /// <param name="maxCountItemsPerDay">максимальное кол-во записей в день на добавление</param>
        /// <returns>список статусов добавления пунктов, для каждого пункта свой статус</returns>
        List<KnowledgeAddStatus> Add(List<UserKnowledgeItem> knowledgeItems, int maxCountItemsPerDay);

        /*/// <summary>
        /// Получить статусы для данных по их идентификаторам
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <param name="dataType">тип данных</param>
        /// <param name="dataIds">идентификаторы данных, для которых нужно получить данные</param>
        /// <returns></returns>
        Dictionary<long, Status> GetStatusesByDataIds(long userId, DataType dataType, List<long> dataIds);*/

        /// <summary>
        /// Определяют являются ли данные некорректными для добавления
        /// </summary>
        /// <param name="knowledgeItem">данные на обучение</param>
        /// <returns>true - данные некорректны, false - данные корректны</returns>
        bool IsInvalid(UserKnowledgeItem knowledgeItem);

        /// <summary>
        /// Удаляет данные
        /// </summary>
        /// <param name="id">идентификатор данных, которые нужно удалить</param>
        /// <returns>true - данные удалены, иначе false</returns>
        bool Remove(long id);

        /// <summary>
        /// Восстанавливает данные
        /// </summary>
        /// <param name="id">идентификатор данных, которые нужно восстановить</param>
        /// <returns>true - данные восстановлены, иначе false</returns>
        bool Restore(long id);

        /// <summary>
        /// Получает статистику пользователя
        /// </summary>
        /// <returns>статистика пользователя</returns>
        UserKnowledgeStatistic GetStatistic();

        /// <summary>
        /// Переводит энтити в пользовательские данные со всей необходимой информацией
        /// </summary>
        /// <param name="sourceLanguageId">идентификатор языка с которого переводят</param>
        /// <param name="translationLanguageId">идентификатор языка на который переводят</param>
        /// <param name="userKnowledges">энтити</param>
        /// <returns>данные пользователя</returns>
        List<UserKnowledgeItem> ConvertEntitiesToItems(long sourceLanguageId,
                                                       long translationLanguageId,
                                                       IEnumerable<UserKnowledge> userKnowledges);

        /// <summary>
        /// Возвращает идентификаторы, которые есть у пользователя в знаниях
        /// </summary>
        /// <param name="ids">идентификаторы, для которых нужно определить существование данных</param>
        /// <param name="dataType">тип данных, для которых переданы идентификаторы</param>
        /// <returns>идентификаторы данных, которые есть у пользователя в знаниях</returns>
        List<long> GetExistenceIds(List<long> ids, KnowledgeDataType dataType);

        /// <summary>
        /// Удаляет данные пользователя, которые были удалены пользователем - ранее текущей даты
        /// </summary>
        /// <returns>true - данные удалены, false - данные не удалось удалить</returns>
        bool RemoveDeleted();
    }
}