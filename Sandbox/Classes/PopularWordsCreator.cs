using System;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes {
    internal class PopularWordsCreator {
        private readonly LanguageShortName _from;
        private readonly PopularWordType _type;

        public PopularWordsCreator(LanguageShortName from, PopularWordType type) {
            _from = from;
            _type = type;
        }

        public void Create(string file) {
            var csvReader = new CsvReader(file);

            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language from = languages.GetByShortName(_from);
            Language russian = languages.GetByShortName(LanguageShortName.Ru);

            //заголовок не учитывать
            string[] firstLine = csvReader.ReadLine();

            var popularWordsQuery = new PopularWordsQuery();
            do {
                string[] line = csvReader.ReadLine();
                if (line == null) {
                    break;
                }
                if (line.Length < 2) {
                    continue;
                }

                SourceWithTranslation popularWord = popularWordsQuery.GetOrCreate(CreateWordForUser(line[0], from),
                                                                                  CreateWordForUser(line[1], russian),
                                                                                  _type);
                Console.WriteLine("{0}: {1}", popularWord != null ? "Сохранено" : "Не сохранено",
                                  line.Aggregate((e1, e2) => e1 + " -> " + e2));
            } while (true);
        }

        private static PronunciationForUser CreateWordForUser(string text, Language language) {
            return new PronunciationForUser(IdValidator.INVALID_ID, text, false, language.Id);
        }
    }
}