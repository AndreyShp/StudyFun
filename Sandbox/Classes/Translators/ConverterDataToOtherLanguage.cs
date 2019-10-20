using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.Validators;

namespace Sandbox.Classes.Translators {
    public class ConverterDataToOtherLanguage {
        private readonly Dictionary<long, LanguageShortName> _languageShortNames =
            new Dictionary<long, LanguageShortName>();
        private readonly LanguageShortName _languageTo;
        private readonly Translator _translator = new Translator();
        private UserLanguages _userLanguages;

        public ConverterDataToOtherLanguage(LanguageShortName languageTo) {
            _languageTo = languageTo;
        }

        private void LoadLanguages() {
            if (_userLanguages != null) {
                return;
            }

            var languagesQuery = new LanguagesQuery(LanguageShortName.Ru,
                                                    LanguageShortName.En);
            _userLanguages = languagesQuery.GetLanguages(null);

            AddToShortName(languagesQuery, _userLanguages.From.Id);
            AddToShortName(languagesQuery, _userLanguages.To.Id);
        }

        private void AddToShortName(ILanguagesQuery languagesQuery, long id) {
            _languageShortNames.Add(id, languagesQuery.GetShortNameById(id));
        }

        public void ConvertGroupWords() {
            LoadLanguages();

            IGroupsQuery groupsQuery = new GroupsQuery(_userLanguages.To.Id);
            Dictionary<long, string> visibleGroups =
                groupsQuery.GetVisibleGroups(GroupType.ByWord).ToDictionary(
                    e => e.Id, e => e.Name);
            IGroupWordsQuery groupWordsQuery = new GroupWordsQuery();

            foreach (var visibleGroup in visibleGroups) {
                long groupId = visibleGroup.Key;
                if (!visibleGroups.ContainsKey(groupId)) {
                    continue;
                }

                string groupName = visibleGroups[groupId];
                string fileName = string.Format(@"C:\Projects\StudyLanguages\Источники для групп\Word\{0}\Xml\{1}.xml",
                                                _languageTo, groupName);

                if (File.Exists(fileName)) {
                    Console.WriteLine("Группа \"{0}\" уже существует - пропустить", groupName);
                    continue;
                }

                List<SourceWithTranslation> words = groupWordsQuery.GetWordsByGroup(_userLanguages, groupId);

                Console.WriteLine("Начали обрабатывать группу слов \"{0}\"", groupName);

                SaveConvertedWords(fileName, words);
            }

            Console.WriteLine(
                "Переконвертировали группы со словами. Воспользовались дополнительными словарями {0} раз",
                _translator.CountExtraCalls);
        }

        public void ConvertGroupSentences() {
            LoadLanguages();

            IGroupsQuery groupsQuery = new GroupsQuery(_userLanguages.To.Id);
            Dictionary<long, string> visibleGroups =
                groupsQuery.GetVisibleGroups(GroupType.BySentence).ToDictionary(
                    e => e.Id, e => e.Name);
            IGroupSentencesQuery groupSentencesQuery = new GroupSentencesQuery();

            foreach (var visibleGroup in visibleGroups) {
                long groupId = visibleGroup.Key;
                if (!visibleGroups.ContainsKey(groupId)) {
                    continue;
                }

                string groupName = visibleGroups[groupId];
                string fileName = string.Format(@"C:\Projects\StudyLanguages\Источники для групп\Group\{0}\Xml\{1}.xml",
                                                _languageTo, groupName);

                if (File.Exists(fileName)) {
                    Console.WriteLine("Группа \"{0}\" уже существует - пропустить", groupName);
                    continue;
                }

                List<SourceWithTranslation> sentences = groupSentencesQuery.GetSentencesByGroup(_userLanguages, groupId);

                Console.WriteLine("Начали обрабатывать группу фраз \"{0}\"", groupName);

                SaveConvertedWords(fileName, sentences);
            }

            Console.WriteLine(
                "Переконвертировали группы с фразами. Воспользовались дополнительными словарями {0} раз",
                _translator.CountExtraCalls);
        }

        public void ConvertVisualDictionaries() {
            LoadLanguages();

            var representationsQuery = new RepresentationsQuery(_userLanguages.To.Id);
            Dictionary<long, string> visibleDictionaries =
                representationsQuery.GetVisibleWithoutAreas().ToDictionary(e => e.Id, e => e.Title);

            foreach (var visualDictionary in visibleDictionaries) {
                long id = visualDictionary.Key;
                if (!visibleDictionaries.ContainsKey(id)) {
                    continue;
                }

                string title = visibleDictionaries[id];
                string fileName =
                    string.Format(@"C:\Projects\StudyLanguages\Источники визуального словаря\{0}\Xml\{1}.xml",
                                  _languageTo, title);

                if (File.Exists(fileName)) {
                    Console.WriteLine("Визуальный словарь \"{0}\" уже существует - пропустить", title);
                    continue;
                }

                Console.WriteLine("Начали обрабатывать визуальный словарь \"{0}\"", title);

                RepresentationForUser representation = representationsQuery.GetWithAreas(_userLanguages,
                                                                                         visualDictionary.Value);
                if (representation == null) {
                    Console.WriteLine(
                        "Не удалось получить данные из словаря \"{0}\" - пропустить. Нажмите enter для продолжения...",
                        title);
                    Console.ReadLine();
                    continue;
                }
                List<SourceWithTranslation> words =
                    representation.Areas.Select(e => {
                        var sourceWithTranslation = new SourceWithTranslation();
                        sourceWithTranslation.Set(e.WordTranslationId, e.Source, e.Translation);
                        return sourceWithTranslation;
                    }).ToList();

                SaveConvertedWords(fileName, words);
            }

            Console.WriteLine(
                "Переконвертировали визуальные словари. Воспользовались дополнительными словарями {0} раз",
                _translator.CountExtraCalls);
        }

        public void ConvertPopularWords(PopularWordType popularWordType) {
            LoadLanguages();

            var popularWordsQuery = new PopularWordsQuery();
            List<SourceWithTranslation> words = popularWordsQuery.GetWordsByType(_userLanguages, popularWordType);

            var fileName = GetFileName(popularWordType);
            SaveConvertedWords(fileName, words);

            Console.WriteLine(
                "Переконвертировали популярные слова с типом {0}. Воспользовались дополнительными словарями {1} раз",
                fileName, _translator.CountExtraCalls);
        }

        public string GetFileName(PopularWordType popularWordType) {
            string fileName = string.Format(@"C:\Projects\StudyLanguages\Источники слов\{0}_{1}.xml",
                                            popularWordType, _languageTo);
            return fileName;
        }

        private void SaveConvertedWords(string fileName,
                                        IEnumerable<SourceWithTranslation> words) {
            var translationSaver = new TranslationSaver(fileName);
            foreach (SourceWithTranslation sourceWithTranslation in words) {
                PronunciationForUser source = sourceWithTranslation.Source;
                PronunciationForUser translation = sourceWithTranslation.Translation;

                var textsWithLanguages = new List<Tuple<LanguageShortName, string>> {
                    new Tuple<LanguageShortName, string>(_languageShortNames[source.LanguageId],
                                                         source.Text),
                    new Tuple<LanguageShortName, string>(_languageShortNames[translation.LanguageId],
                                                         translation.Text),
                };

                string sourceWords = string.Join(", ", textsWithLanguages.Select(e => e.Item2));

                List<string> bestTranslations = _translator.Translate(_languageTo, textsWithLanguages);
                List<string> otherTranslations = _translator.GetOtherTranslations();
                translationSaver.Write(sourceWithTranslation, bestTranslations, otherTranslations);

                if (EnumerableValidator.IsNotEmpty(bestTranslations)) {
                    Console.WriteLine("Лучшие переводы для {0} это {1}", sourceWords,
                                      string.Join(", ", bestTranslations));
                    //TODO: сохранять в БД, предварительно изменив регистр в соответствии с английским словом
                    continue;
                }

                if (EnumerableValidator.IsEmpty(otherTranslations)) {
                    //не нашли вообще переводов - сообщить
                    Console.WriteLine("ERROR: Для {0} не нашли вообще переводов. Пропустить", sourceWords);
                }

                Console.WriteLine(
                    "WARNING: Для слова {0} нашли переводы {1}, но среди них нет лучшего. Пропустить", sourceWords,
                    string.Join(", ", otherTranslations));
            }

            translationSaver.Save();
        }
    }
}