using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BusinessLogic.Data;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.Validators;
using Point = BusinessLogic.ExternalData.Representations.Point;
using Size = BusinessLogic.ExternalData.Representations.Size;

namespace Sandbox.Classes {
    internal class VisualDictionaryFiller {
        private readonly LanguageShortName _from;

        public VisualDictionaryFiller(LanguageShortName from) {
            _from = from;
        }

        public void Create(string fileName, string pathToImagePattern) {
            var csvReader = new CsvReader(fileName);

            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language from = languages.GetByShortName(_from);
            Language to = languages.GetByShortName(LanguageShortName.Ru);
            var representationsQuery = new RepresentationsQuery(from.Id);

            string[] line = csvReader.ReadLine();
            if (line.Length < 1 || string.IsNullOrEmpty(line[0])) {
                Console.WriteLine("Некорректная первая строка в файле {0}!", fileName);
                return;
            }

            int imageIndex = 0;
            if (line.Length >= 2 && !string.IsNullOrEmpty(line[1])) {
                imageIndex = 1;
            }

            string imageFileName = string.Format(pathToImagePattern, line[imageIndex].Trim());
            Image image = Image.FromFile(imageFileName);
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            byte[] imageBytes = memoryStream.ToArray();

            string visualDictionaryName = line[0];

            byte? widthPercent = null;
            if (line.Length > 2) {
                byte w;
                if (byte.TryParse(line[2], out w) && w > 0 && w <= 100) {
                    widthPercent = w;
                }
            }
            visualDictionaryName = char.ToUpper(visualDictionaryName[0]) + visualDictionaryName.Substring(1);

            var representationForUser = new RepresentationForUser(IdValidator.INVALID_ID, visualDictionaryName,
                                                                  imageBytes,
                                                                  new Size(image.Size.Width, image.Size.Height),
                                                                  widthPercent);
            bool hasErrors = false;
            do {
                line = csvReader.ReadLine();
                if (line != null) {
                    if (line.Length < 6) {
                        hasErrors = true;
                        break;
                    }

                    PronunciationForUser englishWord = CreateWordForUser(line[0], from);
                    PronunciationForUser russianWord = CreateWordForUser(line[1], to);

                    Point leftTopPoint = CreatePoint(line[2], line[3]);
                    Point rightBottomPoint = CreatePoint(line[4], line[5]);

                    if (englishWord == null || russianWord == null || leftTopPoint == null || rightBottomPoint == null) {
                        hasErrors = true;
                        break;
                    }

                    var representationArea = new RepresentationAreaForUser(IdValidator.INVALID_ID, leftTopPoint,
                                                                           rightBottomPoint)
                    {Source = russianWord, Translation = englishWord};
                    representationForUser.AddArea(representationArea);
                }
            } while (line != null);

            if (hasErrors) {
                Console.WriteLine("В файле {0} возникли ошибки! Файл не будет сохранен", fileName);
                return;
            }

            RepresentationForUser savedRepresentation = representationsQuery.GetOrCreate(representationForUser);
            Console.WriteLine("Визуальный словарь {0} {1}", representationForUser.Title,
                              savedRepresentation != null ? "сохранен" : "НЕ сохранен!");
        }

        private static Point CreatePoint(string x, string y) {
            int parsedX;
            int parsedY;
            return int.TryParse(x, out parsedX) && int.TryParse(y, out parsedY) ? new Point(parsedX, parsedY) : null;
        }

        private static PronunciationForUser CreateWordForUser(string word, Language language) {
            return !string.IsNullOrEmpty(word)
                       ? new PronunciationForUser(IdValidator.INVALID_ID, word, false, language.Id)
                       : null;
        }
    }
}