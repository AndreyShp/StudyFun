using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes.Translators {
    public class OtherLanguagesFilesCreator {
        private readonly LanguageShortName _languageTo;

        public OtherLanguagesFilesCreator(LanguageShortName languageTo) {
            _languageTo = languageTo;
        }

        public void ConvertGroupWords() {
            string path = string.Format(@"C:\Projects\StudyLanguages\Источники для групп\Word\{0}\", _languageTo);
            string[] files = Directory.GetFiles(Path.Combine(path, "Xml"), "*.xml");

            var xmlTranslationReader = new XmlTranslationReader();

            foreach (string file in files) {
                string name = Path.GetFileNameWithoutExtension(file);

                Console.WriteLine("Обрабатываем группу слов \"{0}\"", name);

                List<XmlTranslationReader.Item> items = xmlTranslationReader.Read(file);

                string destinationFile = Path.Combine(path, name + ".csv");
                using (var csvWriter = new CsvWriter(destinationFile)) {
                    csvWriter.WriteLine(name);
                    foreach (XmlTranslationReader.Item item in items) {
                        var fields = new List<string>();
                        string source;
                        if (EnumerableValidator.IsNotEmpty(item.Best)) {
                            source = item.Best[0].Trim();
                        } else {
                            source = "<UNKNOWN> - " + string.Join("|", item.Other);
                        }

                        fields.Add(source);
                        fields.Add(item.Source);

                        /* string imageFileName =
                            string.Format(
                                @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\{0}\{1}.jpg",
                                name, item.Translation);
                        if (File.Exists(imageFileName)) {
                            fields.Add(item.Translation);
                        }*/

                        csvWriter.WriteLine(fields.ToArray());
                    }
                }
            }

            Console.WriteLine("Все группы слов обработаны!");
        }

        public void ConvertMinileks(string fileName) {
            var xmlTranslationReader = new XmlTranslationReader();

            string name = Path.GetFileNameWithoutExtension(fileName);

            Console.WriteLine("Обрабатываем минилекс \"{0}\"", name);

            List<XmlTranslationReader.Item> items = xmlTranslationReader.Read(fileName);

            string path = Path.GetDirectoryName(fileName);
            string destinationFile = Path.Combine(path, name + ".csv");
            using (var csvWriter = new CsvWriter(destinationFile)) {
                csvWriter.WriteLine("Минилекс Гуннемарка");
                foreach (XmlTranslationReader.Item item in items) {
                    var fields = new List<string>();
                    string source;
                    if (EnumerableValidator.IsNotEmpty(item.Best)) {
                        source = item.Best[0].Trim();
                    } else {
                        source = "<UNKNOWN> - " + string.Join("|", item.Other);
                    }

                    fields.Add(source);
                    fields.Add(item.Source);

                    csvWriter.WriteLine(fields.ToArray());
                }
            }

            Console.WriteLine("Минилекс обработан!");
        }

        public void ConvertGroupSentences() {
            string path = string.Format(@"C:\Projects\StudyLanguages\Источники для групп\Group\{0}\", _languageTo);
            string[] files = Directory.GetFiles(Path.Combine(path, "Xml"), "*.xml");

            var xmlTranslationReader = new XmlTranslationReader();

            foreach (string file in files) {
                string name = Path.GetFileNameWithoutExtension(file);

                Console.WriteLine("Обрабатываем группу фраз \"{0}\"", name);

                List<XmlTranslationReader.Item> items = xmlTranslationReader.Read(file);

                string destinationFile = Path.Combine(path, name + ".csv");
                using (var csvWriter = new CsvWriter(destinationFile)) {
                    csvWriter.WriteLine(name);
                    foreach (XmlTranslationReader.Item item in items) {
                        var fields = new List<string>();
                        string source;
                        if (EnumerableValidator.IsNotEmpty(item.Best)) {
                            source = item.Best[0].Trim();
                        } else {
                            source = "<UNKNOWN> - " + string.Join("|", item.Other);
                        }

                        fields.Add(source);
                        fields.Add(item.Source);

                        /*string imageFileName =
                            string.Format(
                                @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\{0}\{1}.jpg",
                                name, item.Translation);
                        if (File.Exists(imageFileName)) {
                            fields.Add(item.Translation);
                        }*/

                        csvWriter.WriteLine(fields.ToArray());
                    }
                }
            }

            Console.WriteLine("Все группы фраз обработаны!");
        }

        public void ConvertVisualDictionaries() {
            string path = string.Format(@"C:\Projects\StudyLanguages\Источники визуального словаря\{0}\", _languageTo);
            string[] files = Directory.GetFiles(Path.Combine(path, "Xml"), "*.xml");

            var xmlTranslationReader = new XmlTranslationReader();

            const string PATTERN_VISUAL_DICTIONARY = @"C:\Projects\StudyLanguages\Источники визуального словаря\{0}.csv";
            const string PATTERN_KEY = "{0}_{1}";

            foreach (string file in files) {
                string name = Path.GetFileNameWithoutExtension(file);

                Console.WriteLine("Обрабатываем визуальный словарь \"{0}\"", name);

                List<XmlTranslationReader.Item> items = xmlTranslationReader.Read(file);

                string destinationFile = Path.Combine(path, name + ".csv");
                using (var csvWriter = new CsvWriter(destinationFile)) {
                    var csvReader = new CsvReader(string.Format(PATTERN_VISUAL_DICTIONARY, name));
                    string[] header = csvReader.ReadLine();
                    csvWriter.WriteLine(header);

                    Dictionary<string, string[]> lines = ConvertFieldsToDictionary(csvReader, PATTERN_KEY);
                    foreach (XmlTranslationReader.Item item in items) {
                        string key = string.Format(PATTERN_KEY, item.Translation, item.Source).ToLowerInvariant();
                        if (!lines.ContainsKey(key)) {
                            Console.WriteLine(
                                "В файле {0} не найдены слова {1} - {2}. Слова не будут добавлены в файл! Нажмите ввод...",
                                name, item.Translation, item.Source);
                            Console.ReadLine();
                            continue;
                        }

                        var fields = new List<string>();
                        string source;
                        if (EnumerableValidator.IsNotEmpty(item.Best)) {
                            source = item.Best[0].Trim();
                        } else {
                            source = "<UNKNOWN> - " + string.Join("|", item.Other);
                        }

                        fields.Add(source);
                        fields.Add(item.Source);
                        fields.AddRange(lines[key]);

                        csvWriter.WriteLine(fields.ToArray());
                    }

                    csvReader.Dispose();
                }
            }

            Console.WriteLine("Все визуальные словари обработаны!");
        }

        private static Dictionary<string, string[]> ConvertFieldsToDictionary(CsvReader csvReader, string PATTERN_KEY) {
            var lines = new Dictionary<string, string[]>();
            string[] sourceFields;
            do {
                sourceFields = csvReader.ReadLine();
                if (sourceFields == null) {
                    break;
                }
                lines.Add(string.Format(PATTERN_KEY, sourceFields[0], sourceFields[1]).ToLowerInvariant(),
                          sourceFields.Skip(2).ToArray());
            } while (true);
            return lines;
        }
    }
}