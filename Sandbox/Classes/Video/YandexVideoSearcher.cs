using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using BusinessLogic.ExternalData;

namespace Sandbox.Classes.Video {
    public class YandexVideoSearcher {
        public const string BASE_PATH = @"C:\Projects\Программы для сайта";
        private const string ALL_LINKS_FILE_NAME = "UniqueLinks.csv";

        private readonly string _linksDirectory;
        private readonly string _pageContentsDirectory;
        private readonly Regex _regex = new Regex("<a[^>]+class=\"serp-item__link\"[^>]+href=\"(?<href>[^\"]+)\"[^>]*>",
                                                  RegexOptions.IgnoreCase | RegexOptions.Singleline
                                                  | RegexOptions.Compiled);
        private readonly Random _rnd = new Random();
        private readonly LanguageShortName _shortName;

        private bool _prevBadPage;

        private YandexVideoSearcher(LanguageShortName shortName) {
            _shortName = shortName;

            _pageContentsDirectory = GetContentsDirectory(BASE_PATH, shortName);
            _linksDirectory = GetLinksDirectory(BASE_PATH, shortName);
        }

        public static string GetContentsDirectory(string basePath, LanguageShortName shortName) {
            string path = GetPath(basePath, shortName);
            return Path.Combine(path, "Contents");
        }

        private static string GetLinksDirectory(string basePath, LanguageShortName shortName) {
            string path = GetPath(basePath, shortName);
            return Path.Combine(path, "Links");
        }

        private static string GetPath(string basePath, LanguageShortName shortName) {
            return Path.Combine(basePath, "Yandex", shortName.ToString());
        }

        public void Search(IEnumerable<string> queries) {
            int countQueries = queries.Count();

            Console.WriteLine("{0} Начали язык {1} для запросов {2}... ", DateTime.Now.ToLocalTime(), _shortName,
                              countQueries);

            CreateDirectoryIfNeed(_pageContentsDirectory);
            CreateDirectoryIfNeed(_linksDirectory);

            var result = new HashSet<string>();
            try {
                int i = 1;
                foreach (string query in queries) {
                    Search(query, result);
                    Console.WriteLine("{0} Обработан запрос {1}({2} из {3})", DateTime.Now.ToLocalTime(), query, i,
                                      countQueries);
                    i++;
                }
            } catch (Exception e) {
                Console.WriteLine("{0} Для языка {1} вылетело исключение {2}!", DateTime.Now.ToLocalTime(), _shortName,
                                  e);
            }

            string allLinksFullFileName = GetAllLinksFullFileName(_linksDirectory);
            File.WriteAllLines(allLinksFullFileName, result);

            Console.WriteLine("{0} Закончили язык {1} для запросов {2}", DateTime.Now.ToLocalTime(), _shortName,
                              countQueries);
        }

        public static string GetAllLinksFullFileName(string basePath, LanguageShortName shortName) {
            string linksDirectory = GetLinksDirectory(basePath, shortName);
            return GetAllLinksFullFileName(linksDirectory);
        }

        private static string GetAllLinksFullFileName(string linksDirectory) {
            return Path.Combine(linksDirectory, ALL_LINKS_FILE_NAME);
        }

        private static void CreateDirectoryIfNeed(string directory) {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

        private void Search(string query, HashSet<string> result) {
            var csvWriter = new CsvWriter(Path.Combine(_linksDirectory, query + ".csv"));
            int page = 1;
            csvWriter.WriteLine("query", query);
            do {
                bool needSleep = false;
                string pageContent = GetCachedContent(page, query);
                if (string.IsNullOrEmpty(pageContent)) {
                    pageContent = GetPageContent(page, query);
                    needSleep = true;
                }

                if (string.IsNullOrEmpty(pageContent)) {
                    //страница не получена
                    throw new ApplicationException("Не получена страница " + query + ", #page = " + page);
                }

                MatchCollection matches = _regex.Matches(pageContent);
                if (matches.Count == 0) {
                    //не найдено ссылок
                    if (_prevBadPage) {
                        throw new ApplicationException("Не найдены ссылки. На странице " + query + ", #page = " + page);
                    }

                    //не понятно может больше нет видео по этому запросу - кинем исключение если и при следующем запросе то же самое
                    _prevBadPage = true;
                    break;
                }

                var resultFromPage = new List<string>();
                foreach (Match match in matches) {
                    Group group = match.Groups["href"];
                    if (group.Success) {
                        string link = group.Value;

                        csvWriter.WriteLine("link", link);
                        resultFromPage.Add(link);
                        result.Add(link);
                    }
                }

                if (resultFromPage.Count == 0) {
                    //не найдено ссылок
                    if (_prevBadPage) {
                        throw new ApplicationException("Не найдены href'ы у " + matches.Count + " ссылок. На странице "
                                                       + query + ", #page = " + page);
                    }

                    //не понятно может больше нет видео по этому запросу - кинем исключение если и при следующем запросе то же самое
                    _prevBadPage = true;
                    break;
                }

                _prevBadPage = false;
                SaveToCache(page, query, pageContent);
                csvWriter.WriteLine("process page", page.ToString(CultureInfo.InvariantCulture));

                page++;

                if (needSleep) {
                    Thread.Sleep(_rnd.Next(5000, 20000));
                }
                //максимально возможная страница для яндекса - 99, при странице 100, он возвращает 404
            } while (page < 100);

            csvWriter.Dispose();
        }

        private static string GetPageContent(int page, string query) {
            string url = string.Format("http://yandex.ru/video/search?text={0}&safety=1&path=main",
                                       HttpUtility.UrlPathEncode(query));
            if (page > 1) {
                url += "&p=" + (page - 1);
            }

            var result = new StringBuilder();
            try {
                var buffer = new byte[4096];
                HttpWebRequest request = WebRequest.CreateHttp(url);
                using (WebResponse response = request.GetResponse()) {
                    using (Stream stream = response.GetResponseStream()) {
                        int countReadedBytes;
                        do {
                            countReadedBytes = stream.Read(buffer, 0, buffer.Length);
                            if (countReadedBytes > 0) {
                                result.Append(Encoding.UTF8.GetString(buffer, 0, countReadedBytes));
                            }
                        } while (countReadedBytes > 0);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
            return result.ToString();
        }

        private string GetCachedContent(int page, string parQuery) {
            //TODO: использовать DiskCache
            string fullPath = GetCachedPageName(page, parQuery);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath) : null;
        }

        private void SaveToCache(int page, string parQuery, string content) {
            //TODO: использовать DiskCache
            string fullPath = GetCachedPageName(page, parQuery);
            File.WriteAllText(fullPath, content, Encoding.UTF8);
        }

        private string GetCachedPageName(int page, string parQuery) {
            return Path.Combine(_pageContentsDirectory, parQuery + "_" + page + ".html");
        }

        public static void Run() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var yandexVideoSearcher = new YandexVideoSearcher(LanguageShortName.En);
            yandexVideoSearcher.Search(new[] {
                "english movies",
                "english films",
                "фильмы на английском",
                "фильмы на английском языке",
                "фильмы на английском с субтитрами",
                "фильмы английский язык с субтитрами",
                "английские фильмы смотреть онлайн",
                "смотреть фильмы на английском",
                "фильм английский русский",
                "русские фильмы на английском",
                "русский фильм с английскими субтитрами",
                "английские фильмы с русскими субтитрами",
                "movies in english",
                "english movies online",
                "full movie english",
                "english movies with english subtitles",
                "english movies free",
                "english movies with russian subtitles",
                "english movies online"
            });

            yandexVideoSearcher = new YandexVideoSearcher(LanguageShortName.De);
            yandexVideoSearcher.Search(new[] {
                "фильмы на немецком языке с русскими субтитрами",
                "фильмы на немецком языке",
                "русские фильмы на немецком",
                "русский фильм с немецкими субтитрами",
            });

            yandexVideoSearcher = new YandexVideoSearcher(LanguageShortName.Es);
            yandexVideoSearcher.Search(new[] {
                "фильмы на испанском",
                "фильмы на испанском языке",
                "фильмы на испанском с субтитрами",
                "русские фильмы на испанском",
                "русский фильм с испанскими субтитрами",
            });

            yandexVideoSearcher = new YandexVideoSearcher(LanguageShortName.Fr);
            yandexVideoSearcher.Search(new[] {
                "фильмы на французском",
                "фильмы на французском языке",
                "фильмы на французском с субтитрами",
                "русские фильмы на французском",
                "русский фильм с французскими субтитрами",
            });

            yandexVideoSearcher = new YandexVideoSearcher(LanguageShortName.It);
            yandexVideoSearcher.Search(new[] {
                "фильмы на итальянском",
                "фильмы на итальянском языке",
                "фильмы на итальянском с субтитрами",
                "русские фильмы на итальянском",
                "русский фильм с итальянскими субтитрами",
            });

            stopwatch.Stop();

            Console.WriteLine("Собрали все ссылки по всем языкам! Работа заняла {0}", stopwatch.Elapsed);
            Console.ReadLine();
        }
    }
}