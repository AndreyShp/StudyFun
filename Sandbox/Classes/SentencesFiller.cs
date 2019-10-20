using System;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes {
    internal class SentencesFiller {
        public void Fill() {
            var csvReader = new CsvReader(@"C:\Projects\StudyLanguages\1.csv");
            string[] line;
            var sentences = new SentencesQuery();
            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language english = languages.GetByShortName(LanguageShortName.En);
            Language russian = languages.GetByShortName(LanguageShortName.Ru);

            do {
                line = csvReader.ReadLine();
                if (line != null) {
                    SourceWithTranslation sentenceWithTranslation =
                        sentences.GetOrCreate(SentenceType.Separate,
                                              new PronunciationForUser(IdValidator.INVALID_ID, line[0],
                                                                       false, english.Id),
                                              new PronunciationForUser(IdValidator.INVALID_ID, line[1],
                                                                       false, russian.Id),
                                              null, null);
                    Console.WriteLine("{0}: {1}", sentenceWithTranslation != null ? "Сохранено" : "Не сохранено",
                                      line.Aggregate((e1, e2) => e1 + " -> " + e2));
                }
            } while (line != null);

            /* for (int i = 1; i <= 100000; i++) {
                SentenceWithTranslation sentenceWithTranslation =
                    sentences.CreateSentencencesWithTranslation(english, "Test sentence number " + i, russian,
                                                                "Тестовое предложение № " + i);
                if (sentenceWithTranslation == null) {
                    Console.WriteLine("Не удалось сохранить предложение {0}. Нажмите Enter...", i);
                    Console.ReadLine();
                }

                if (i % 10000 == 0) {
                    Console.WriteLine("Сохранено {0} предложений", i);
                }
            }*/
        }
    }
}