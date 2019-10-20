using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Comparisons;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Auxiliaries {
    public class AllMaterialsQuery {
        //NOTE: добавляя сюда значение - добавьте также в FixedSalesModel и изменить здесь метод AllMaterialsSalesGenerator
        private readonly Func<SectionId, bool> _forbiddenSectionsChecker;
        private readonly Dictionary<SectionId, Tuple<string, string>> _info =
            new Dictionary<SectionId, Tuple<string, string>> {
                {SectionId.GroupByWords, new Tuple<string, string>("Слова по темам", "Тема")},
                {SectionId.GroupByPhrases, new Tuple<string, string>("Фразы по темам", "Тема")},
                {SectionId.VisualDictionary, new Tuple<string, string>("Визуальные словари", "Название словаря")},
                {SectionId.FillDifference, new Tuple<string, string>("Правила употребления", "Правила сравнения")},
                {SectionId.Video, new Tuple<string, string>("Тексты из видео", "Название видео")},
                {SectionId.PopularWord, new Tuple<string, string>("Минилекс Гуннемарка", "")}
            };

        private readonly long _languageId;
        private readonly UserLanguages _userLanguages;

        public AllMaterialsQuery(long languageId,
                                 UserLanguages userLanguages,
                                 Func<SectionId, bool> forbiddenSectionsChecker) {
            _languageId = languageId;
            _userLanguages = userLanguages;
            _forbiddenSectionsChecker = forbiddenSectionsChecker;
        }

        public Dictionary<SectionId, List<string>> GetWithoutData() {
            Dictionary<SectionId, List<Tuple<long, string, DateTime>>> result = GetDataBySections();
            return result.ToDictionary(e => e.Key, e => e.Value.Select(v => v.Item2).ToList());
        }

        public Dictionary<SectionId, List<Tuple<long, string, DateTime>>> GetDataBySections() {
            var result = new Dictionary<SectionId, List<Tuple<long, string, DateTime>>>();

            AddToResult(SectionId.GroupByWords, () => GetGroups(GroupType.ByWord),
                        e => new Tuple<long, string, DateTime>(e.Id, e.Name, e.SortInfo.LastModified),
                        result);
            AddToResult(SectionId.FillDifference, GetComparisons, e => new Tuple<long, string, DateTime>(e.Id, e.Title, DateTime.MinValue), result);
            AddToResult(SectionId.PopularWord, () => new List<string> {GetHeader(SectionId.PopularWord)},
                        e => new Tuple<long, string, DateTime>(IdValidator.INVALID_ID, e, DateTime.MinValue),
                        result);
            AddToResult(SectionId.VisualDictionary, GetVisualDictionaries,
                        e => new Tuple<long, string, DateTime>(e.Id, e.Title, e.SortInfo.LastModified),
                        result);
            AddToResult(SectionId.GroupByPhrases, () => GetGroups(GroupType.BySentence),
                        e => new Tuple<long, string, DateTime>(e.Id, e.Name, e.SortInfo.LastModified),
                        result);
            AddToResult(SectionId.Video, GetVideos,
                        e => new Tuple<long, string, DateTime>(e.Id, e.Title, e.SortInfo.LastModified), result);

            //TODO: если будет тупить, то можно распараллелить получение данных
            return result;
        }

        private void AddToResult<TElem>(SectionId sectionId,
                                        Func<List<TElem>> getterElements,
                                        Func<TElem, Tuple<long, string, DateTime>> converter,
                                        Dictionary<SectionId, List<Tuple<long, string, DateTime>>> result) {
            if (_forbiddenSectionsChecker(sectionId)) {
                return;
            }

            List<TElem> data = getterElements();
            if (EnumerableValidator.IsNotNullAndNotEmpty(data)) {
                result.Add(sectionId, data.Select(converter).OrderBy(e => e.Item2).ToList());
            }
        }

        public string GetHeader(SectionId sectionId) {
            return _info[sectionId].Item1;
        }

        public string GetTableHeader(SectionId sectionId) {
            return _info[sectionId].Item2;
        }

        private List<GroupForUser> GetGroups(GroupType groupType) {
            var groupsQuery = new GroupsQuery(_languageId);
            return groupsQuery.GetVisibleGroups(groupType);
        }

        internal List<SourceWithTranslation> GetWordsByGroup(long groupId) {
            var wordsQuery = new GroupWordsQuery();
            return wordsQuery.GetWordsByGroup(_userLanguages, groupId);
        }

        internal List<SourceWithTranslation> GetSentencesByGroup(long groupId) {
            var sentencesQuery = new GroupSentencesQuery();
            return sentencesQuery.GetSentencesByGroup(_userLanguages, groupId);
        }

        private List<RepresentationForUser> GetVisualDictionaries() {
            var representationsQuery = new RepresentationsQuery(_languageId);
            return representationsQuery.GetVisibleWithoutAreas();
        }

        internal RepresentationForUser GetVisualDictionary(string title) {
            var representationsQuery = new RepresentationsQuery(_languageId);
            return representationsQuery.GetWithAreas(_userLanguages, title);
        }

        private List<ComparisonForUser> GetComparisons() {
            var comparisonsQuery = new ComparisonsQuery(_languageId);
            return comparisonsQuery.GetVisibleWithoutRules();
        }

        internal ComparisonForUser GetComparison(string title) {
            var comparisonsQuery = new ComparisonsQuery(_languageId);
            return comparisonsQuery.GetWithFullInfo(_userLanguages, title);
        }

        private List<VideoForUser> GetVideos() {
            var videosQuery = new VideosQuery(_languageId);
            return videosQuery.GetVisibleWithText(VideoType.Clip);
        }

        internal VideoForUser GetVideo(string title) {
            var videosQuery = new VideosQuery(_languageId);
            return videosQuery.Get(title);
        }

        internal List<SourceWithTranslation> GetPopularWords() {
            var popularWordsQuery = new PopularWordsQuery();
            return popularWordsQuery.GetWordsByType(_userLanguages, PopularWordType.Minileks);
        }
    }
}