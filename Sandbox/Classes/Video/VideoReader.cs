using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;
using Sandbox.Classes.Video.Getters;

namespace Sandbox.Classes.Video {
    public class VideoReader {
        private readonly string _fileName;

        public VideoReader(string fileName) {
            _fileName = fileName;
        }

        public VideoForUser Read() {
            string[] lines;
            if (!IsFileValid(out lines)) {
                return null;
            }

            string title = TrimLine(lines[0]);
            string htmlCode = GetHtmlCode(TrimLine(lines[1]));
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(htmlCode)) {
                Console.WriteLine("Из файла {0} не удалось получить заголовок или код-вставки", _fileName);
                return null;
            }

            var result = new VideoForUser(title, htmlCode);
            if (lines.Length < 3) {
                return result;
            }

            for (int i = 2; i < lines.Length; i++) {
                string line = lines[i];
                string trimmedLine = TrimLine(line);

                string[] sourceWithTranslation = trimmedLine.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                if (sourceWithTranslation.Length == 0) {
                    continue;
                }

                if (sourceWithTranslation.Length > 2) {
                    Console.WriteLine("В файле {0} раскололи строку на {1} частей. Строка {2}: {3}", _fileName,
                                      sourceWithTranslation.Length, i, line);
                }

                var sentence = new Tuple<string, string>(sourceWithTranslation[0].Trim(),
                                                         sourceWithTranslation.Length > 1
                                                             ? sourceWithTranslation[1].Trim()
                                                             : null);
                result.Sentences.Add(sentence);
            }
            return result;
        }

        private bool IsFileValid(out string[] lines) {
            if (!File.Exists(_fileName)) {
                Console.WriteLine("Не найден файл {0}", _fileName);
                lines = null;
                return false;
            }

            lines = File.ReadAllLines(_fileName);
            if (EnumerableValidator.IsNullOrEmpty(lines)) {
                Console.WriteLine("Содержимое файла {0} пустое", _fileName);
                return false;
            }

            if (lines.Length < 2) {
                Console.WriteLine("Файл {0} должен содержать как минимум 2 строки", _fileName);
                return false;
            }
            return true;
        }

        public VideoForUser ReadSubtitles() {
            string[] lines;
            if (!IsFileValid(out lines)) {
                return null;
            }

            if (lines.Length > 2) {
                Console.WriteLine("В файле {0} уже есть субтитры", _fileName);
                return null;
            }

            var videoGetter = new YouTubeGetter();

            StringBuilder rows = videoGetter.GetSubtitles(lines[1]);
            if (rows == null) {
                Console.WriteLine("Для файла {0} не удалось получить субтитры", _fileName);
            }

            VideoForUser result = null;
            if (rows != null && rows.Length > 0) {
                if (!lines[lines.Length - 1].EndsWith(Environment.NewLine)) {
                    rows.Insert(0, Environment.NewLine);
                }
                try {
                    rows.Length = rows.Length - Environment.NewLine.Length;
                    string text = rows.ToString();
                    File.AppendAllText(_fileName, text);
                    result = Read();
                    Console.WriteLine("Файл {0} успешно обработан", _fileName);
                } catch (Exception e) {
                    Console.WriteLine("Для файла {0} не удалось записать субтитры. Исключение:\r\n{1}",
                                      _fileName, e);
                }
            }
            return result;
        }

        private static string TrimLine(string line) {
            return (line ?? string.Empty).Trim();
        }

        private static string GetHtmlCode(string htmlCode) {
            if (string.IsNullOrEmpty(htmlCode)) {
                return htmlCode;
            }

            if (!htmlCode.StartsWith("<iframe", StringComparison.InvariantCultureIgnoreCase)) {
                //в качестве секции указан только идентификатор видео
                var videoGetter = new YouTubeGetter();
                htmlCode = videoGetter.GetFrameHtmlById(htmlCode);
            }
            return htmlCode;
        }
    }
}