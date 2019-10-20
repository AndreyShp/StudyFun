using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Words;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace Sandbox.Classes {
    internal class WordFiller {
        private readonly Regex _htmlRegex = new Regex("<li\\s+class=\"input\"\\s+id=\"deldis\\d+\"\\s*>"
                                                      +
                                                      ".*?<input\\s+name=\"src\"[^>]+value=\"(?<source>[^\"]+)\"[^>]*>.*?"
                                                      +
                                                      "<input\\s+name=\"dst\"[^>]+value=\"(?<destination>[^\"]+)\"[^>]*>.*?</li>",
                                                      RegexOptions.IgnoreCase | RegexOptions.Singleline
                                                      | RegexOptions.Compiled);
        private readonly Regex _xmlRegex = new Regex("(?<!\\([^)]*),(?![^(]*\\))",
                                                     RegexOptions.IgnoreCase | RegexOptions.Singleline
                                                     | RegexOptions.Compiled);
        private int _counter;
        private long _englishLanguageId;
        private bool _isLoaded;
        private long _russianLanguageId;
        private WordsQuery _words;

        public void FillFromHtml(string fileName, Action<int, string, IEnumerable<string>, bool> callBack) {
            Load();
            string fileContent;
            using (var reader = new StreamReader(fileName, Encoding.UTF8)) {
                fileContent = reader.ReadToEnd();
            }
            MatchCollection matches = _htmlRegex.Matches(fileContent);
            foreach (Match match in matches) {
                string source = match.Groups["source"].Value.Trim().ToLowerInvariant();
                string destination = match.Groups["destination"].Value.Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination)) {
                    continue;
                }

                var translations = new[] {source};
                bool isSaved = CreateWordWithTranslation(destination, translations, WordType.Default);
                if (!isSaved) {
                    int i = 0;
                }
                callBack(++_counter, destination, translations, isSaved);
            }
        }

        public void FillFromXml(string fileName, Action<int, string, IEnumerable<string>, bool> callBack) {
            Load();

            XDocument doc = XDocument.Load(fileName);
            foreach (XElement cardElement in doc.Root.Elements("card")) {
                XElement wordElement = cardElement.XPathSelectElement("word");
                if (wordElement == null) {
                    return;
                }
                string word = wordElement.Value.Trim();
                if (string.IsNullOrEmpty(word)) {
                    return;
                }
                IEnumerable<XElement> translations =
                    cardElement.XPathSelectElements("meanings/meaning/translations/word");
                List<string> dirtyTranslationsWords = translations.Where(e => !string.IsNullOrEmpty(e.Value))
                    .SelectMany(e => _xmlRegex.Split(e.Value)).Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();

                if (EnumerableValidator.IsEmpty(dirtyTranslationsWords)) {
                    return;
                }

                bool isSaved = CreateWordWithTranslation(word, dirtyTranslationsWords, WordType.Default);
                if (!isSaved) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "WordFiller.FillFromXml can't add word {0}, translations: {1}", word,
                        string.Join(",", dirtyTranslationsWords));
                }
                callBack(++_counter, word, dirtyTranslationsWords, isSaved);
            }
        }

        private bool CreateWordWithTranslation(string englishWord,
                                               IEnumerable<string> translatedWords,
                                               WordType wordType) {
            string sourceText = englishWord;
            var wordWithTranslation = new WordWithTranslation(GetWord(_englishLanguageId, sourceText));
            foreach (string translatedWord in translatedWords) {
                wordWithTranslation.AddTranslation(GetWord(_russianLanguageId, translatedWord));
            }

            bool result = _words.Create(wordWithTranslation);
            return result;
        }

        private static Word GetWord(long languageId, string sourceText) {
            return new Word {LanguageId = languageId, Text = sourceText};
        }

        private void Load() {
            if (_isLoaded) {
                return;
            }

            SetLanguages();

            _words = new WordsQuery();
            _counter = 0;
            _isLoaded = true;
        }

        private void SetLanguages() {
            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language english = languages.GetByShortName(LanguageShortName.En);
            Language russian = languages.GetByShortName(LanguageShortName.Ru);

            _englishLanguageId = english.Id;
            _russianLanguageId = russian.Id;
        }

        public void FillByCSV(string fileName, Action<int, string, string, bool> callBack) {
            var csvReader = new CsvReader(fileName);

            SetLanguages();
            var wordsQuery = new WordsQuery();

            int i = 0;
            do {
                string[] line = csvReader.ReadLine();
                if (line == null) {
                    break;
                }
                if (line.Length < 2) {
                    continue;
                }

                string source = line[0].Trim();
                string[] translations = line[1].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string translation in translations) {
                    WordWithTranslation wordWithTranslation = wordsQuery.GetOrCreate(
                        new PronunciationForUser(IdValidator.INVALID_ID, source, false, _englishLanguageId),
                        new PronunciationForUser(IdValidator.INVALID_ID, translation, false, _russianLanguageId), null,
                        WordType.PhrasalVerb);
                    callBack(++i, source, translation, wordWithTranslation != null);
                }
            } while (true);
        }
    }
}