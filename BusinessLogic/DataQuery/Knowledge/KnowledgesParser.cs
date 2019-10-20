using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Knowledge {
    public class KnowledgesParser {
        private readonly ISentencesQuery _sentencesQuery;
        private readonly IWordsQuery _wordsQuery;

        public KnowledgesParser() {
            _wordsQuery = new WordsQuery();
            _sentencesQuery = new SentencesQuery();
        }

        /// <summary>
        /// Переводит энтити в пользовательские данные со всей необходимой информацией
        /// </summary>
        /// <param name="sourceLanguageId">идентификатор языка с которого переводят</param>
        /// <param name="translationLanguageId">идентификатор языка на который переводят</param>
        /// <param name="items">данные содержащие информацию о знаниях</param>
        /// <returns>данные пользователя</returns>
        public void FillItemsParsedData<T>(long sourceLanguageId,
                                           long translationLanguageId,
                                           List<T> items) where T : IKnowledgeItem {
            var wordsSearcher = new ItemsSearcher(KnowledgeDataType.WordTranslation);
            var sentencesSearcher = new ItemsSearcher(KnowledgeDataType.SentenceTranslation,
                                                      KnowledgeDataType.PhraseTranslation);
            foreach (T item in items) {
                if (IdValidator.IsInvalid(item.DataId)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "KnowledgesConverter.ConvertEntitiesToItems передан некорректный идентификатор данных(dataId) - {0}",
                        item.DataId);
                    continue;
                }

                wordsSearcher.AddIfNeed(item.DataType, item.DataId);
                sentencesSearcher.AddIfNeed(item.DataType, item.DataId);
            }

            if (wordsSearcher.HasIds) {
                Dictionary<long, SourceWithTranslation> words = _wordsQuery.GetByTranslationsIds(wordsSearcher.Ids,
                                                                                                 sourceLanguageId,
                                                                                                 translationLanguageId);
                wordsSearcher.Set(words);
            }
            if (sentencesSearcher.HasIds) {
                Dictionary<long, SourceWithTranslation> sentences =
                    _sentencesQuery.GetByTranslationsIds(sentencesSearcher.Ids, sourceLanguageId,
                                                         translationLanguageId);
                sentencesSearcher.Set(sentences);
            }

            for (int i = items.Count - 1; i >= 0; i--) {
                T item = items[i];
                item.ParsedData = wordsSearcher.Find(item.DataType, item.DataId);
                if (item.ParsedData == null) {
                    item.ParsedData = sentencesSearcher.Find(item.DataType, item.DataId);
                }
                if (item.ParsedData == null) {
                    //удалить данные, т.к. не удалось получить распарсенный вид
                    items.RemoveAt(i);
                }
            }
        }
    }
}