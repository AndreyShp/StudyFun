using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;

namespace BusinessLogic.DataQuery.Words {
    public interface IPopularWordsQuery {
        /// <summary>
        /// ¬озвращает слова по типу попул€рности
        /// </summary>
        /// <param name="userLanguages">€зык</param>
        /// <param name="type">тип попул€рных слов</param>
        /// <returns>список слов по типу</returns>
        List<SourceWithTranslation> GetWordsByType(UserLanguages userLanguages, PopularWordType type);

        /// <summary>
        /// —оздает слова по типу попул€рности
        /// </summary>
        /// <param name="source">слово</param>
        /// <param name="translation">перевод</param>
        /// <param name="type">тип попул€рности</param>
        /// <returns>созданные слова дл€ группы, или ничего</returns>
        SourceWithTranslation GetOrCreate(PronunciationForUser source,
                                          PronunciationForUser translation,
                                          PopularWordType type);
    }
}