using System;
using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Auxiliaries;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes {
    public class CrossReferencesFiller {
        public void Fill(LanguageShortName languageShortName, string fileName) {
            long languageId =
                new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown).GetByShortName(languageShortName).Id;
            var crossReferencesQuery = new CrossReferencesQuery(languageId);

            bool hasErrors = false;
            var ids = new HashSet<long>();
            var csvReader = new CsvReader(fileName);
            int i = 0;
            do {
                i++;
                string[] line = csvReader.ReadLine();
                if (line == null) {
                    break;
                }

                if (line.Length != 4) {
                    ShowError(
                        "Некорректное кол-во полей в строке {0}: Ожидаем 4, а полей {1}. Для продолжения нажмите Enter...",
                        i, line.Length);
                    continue;
                }

                string source = GetTrimmed(line[0]);
                CrossReferenceType sourceType = GetCrossType(line[1]);
                string destination = GetTrimmed(line[2]);
                CrossReferenceType destinationType = GetCrossType(line[3]);

                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination)
                    || sourceType == CrossReferenceType.Unknown || destinationType == CrossReferenceType.Unknown) {
                    ShowError("Некорректные данные в строке {0}: {1}. Для продолжения нажмите Enter...", i,
                              string.Join(";", line));
                    continue;
                }

                long id = crossReferencesQuery.Add(source, sourceType, destination, destinationType);
                if (IdValidator.IsValid(id)) {
                    ids.Add(id);
                    Console.WriteLine("Обработана строка под номером {0}", i);
                } else {
                    hasErrors = true;
                    ShowError(
                        "Не удалось сохранить данные из строки {0}. Источник {1}, тип источника {2}, приемник {3}, тип источника {4}. Для продолжения нажмите Enter...",
                        i, source, sourceType, destination, destinationType);
                }
            } while (true);
            if (!hasErrors) {
                crossReferencesQuery.RemoveAllExceptIds(ids);
            }

            Console.WriteLine("С ссылками все");
            Console.ReadLine();
        }

        private static void ShowError(string message, params object[] pars) {
            Console.WriteLine(message, pars);
            Console.ReadLine();
        }

        private static string GetTrimmed(string value) {
            return value.Trim();
        }

        private static CrossReferenceType GetCrossType(string value) {
            CrossReferenceType result;
            return Enum.TryParse(GetTrimmed(value), true, out result) ? result : CrossReferenceType.Unknown;
        }
    }
}