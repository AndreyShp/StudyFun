using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    public interface IUserRepetitionIntervalQuery {
        /// <summary>
        /// Возвращает count данных, которые нужно повторить
        /// </summary>
        /// <param name="sourceLanguageId">идентификатор языка с которого переводят</param>
        /// <param name="translationLanguageId">идентификатор языка на который переводят</param>
        /// <param name="count">кол-во записей</param>
        /// <returns>если есть, то данные которые нужно повторить, иначе пустой набор</returns>
        List<UserRepetitionIntervalItem> GetRepetitionIntervalItems(long sourceLanguageId,
                                                                    long translationLanguageId,
                                                                    int count);

        /// <summary>
        /// Выставляет оценку данным
        /// </summary>
        /// <param name="intervalItem">данные</param>
        /// <param name="mark">оценка</param>
        /// <returns>true - оценку удалось поставить, false - оценку не удалось поставить</returns>
        bool SetMark(UserRepetitionIntervalItem intervalItem, KnowledgeMark mark);

        /// <summary>
        /// Удаляет данные пользователя, для которых уже нет данных на странице со знаниями пользователя
        /// </summary>
        /// <returns>true - данные успешно удалены, false - не удалось удалить данные</returns>
        bool RemoveWithoutData();
    }
}