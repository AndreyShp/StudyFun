using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.ExternalData.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    /// <summary>
    /// Отвечает за генерацию знаний для пользователя
    /// </summary>
    public class KnowledgeGeneratorQuery : BaseQuery, IKnowledgeGeneratorQuery {
        private readonly long _mainLanguageId;
        private readonly long _translationLanguageId;
        private readonly long _userId;

        public KnowledgeGeneratorQuery(long userId, long mainLanguageId, long translationLanguageId) {
            _userId = userId;
            _mainLanguageId = mainLanguageId;
            _translationLanguageId = translationLanguageId;
        }

        public Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>> Generate(
            Dictionary<KnowledgeDataType, int> counts) {
            var sourceItems = new List<GeneratedKnowledgeItem>();

            int countWords = counts[KnowledgeDataType.WordTranslation];
            WriteShuffleWordsToResult(countWords, sourceItems);

            int countPhrases = counts[KnowledgeDataType.PhraseTranslation];
            WriteShuffledSentencesToResult(KnowledgeDataType.PhraseTranslation, SentenceType.FromGroup, countPhrases,
                                           sourceItems);

            int countSentences = counts[KnowledgeDataType.SentenceTranslation];
            int countAddedSentences = WriteShuffledSentencesToResult(KnowledgeDataType.SentenceTranslation,
                                                                     SentenceType.Separate, countSentences,
                                                                     sourceItems);
            if (countAddedSentences < countSentences) {
                //добавили меньше, чем нужно - попробуем добрать из примеров предложений
                WriteShuffledSentencesToResult(KnowledgeDataType.SentenceTranslation, SentenceType.ComparisonExample,
                                               countSentences - countAddedSentences,
                                               sourceItems);
            }

            var knowledgesConverter = new KnowledgesParser();
            knowledgesConverter.FillItemsParsedData(_mainLanguageId,
                                                    _translationLanguageId,
                                                    sourceItems);

            var result = new Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>>();
            foreach (GeneratedKnowledgeItem sourceItem in sourceItems) {
                KnowledgeDataType dataType = sourceItem.DataType;
                List<GeneratedKnowledgeItem> items;
                if (!result.TryGetValue(dataType, out items)) {
                    items = new List<GeneratedKnowledgeItem>();
                    result.Add(dataType, items);
                }
                items.Add(sourceItem);
            }
            return result;
        }

        private int WriteShuffledSentencesToResult(KnowledgeDataType dataType,
                                                   SentenceType sentenceType,
                                                   int count,
                                                   List<GeneratedKnowledgeItem> result) {
            List<long> ids = Adapter.ExecuteStoredProcedure("get_shuffle_sentences", fields => (long) fields[0],
                                                            _userId, (int) dataType, (int) sentenceType, _mainLanguageId,
                                                            _translationLanguageId, count);
            WriteDataToResult(dataType, result, ids);
            return ids != null ? ids.Count : 0;
        }

        private void WriteShuffleWordsToResult(int count, List<GeneratedKnowledgeItem> result) {
            const KnowledgeDataType DATA_TYPE = KnowledgeDataType.WordTranslation;

            List<long> ids = Adapter.ExecuteStoredProcedure("get_shuffle_words", fields => (long) fields[0],
                                                            _userId, (int) DATA_TYPE, _mainLanguageId,
                                                            _translationLanguageId, count);
            WriteDataToResult(DATA_TYPE, result, ids);
        }

        private static void WriteDataToResult(KnowledgeDataType dataType,
                                              List<GeneratedKnowledgeItem> result,
                                              IEnumerable<long> ids) {
            if (ids != null) {
                result.AddRange(ids.Select(e => new GeneratedKnowledgeItem {DataId = e, DataType = dataType}));
            }
        }
    }
}