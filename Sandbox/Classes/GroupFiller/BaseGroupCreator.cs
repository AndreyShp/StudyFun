using System;
using System.IO;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace Sandbox.Classes.GroupFiller {
    internal abstract class BaseGroupCreator {
        protected abstract GroupType GroupType { get; }
        private readonly LanguageShortName _from;

        protected BaseGroupCreator(LanguageShortName from) {
            _from = from;
        }

        public void Create(string file) {
            var csvReader = new CsvReader(file);
            //var sentences = IoCModule.Create<SentencesQuery>();
            var languages = new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown);
            Language from = languages.GetByShortName(_from);
            Language russian = languages.GetByShortName(LanguageShortName.Ru);

            string[] firstLine = csvReader.ReadLine();
            if (firstLine == null || EnumerableValidator.IsEmpty(firstLine)) {
                return;
            }

            string groupName = TextFormatter.FirstUpperCharAndTrim(firstLine[0]);
            if (string.IsNullOrEmpty(groupName) || groupName.Length < 2) {
                return;
            }
            var groupsQuery = new GroupsQuery(from.Id);

            string fileName = firstLine.Length > 1 && !string.IsNullOrWhiteSpace(firstLine[1])
                                  ? firstLine[1].Trim()
                                  : groupName;
            string imageFileName = Path.Combine(@"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\",
                                                fileName + ".jpg");
            //создает или получает раннее созданную группу
            byte[] image = !string.IsNullOrEmpty(imageFileName) ? File.ReadAllBytes(imageFileName) : null;
            GroupForUser groupForUser = groupsQuery.GetOrCreate(GroupType, groupName, image);

            do {
                string[] line = csvReader.ReadLine();
                if (line == null) {
                    break;
                }
                if (line.Length < 2) {
                    continue;
                }
                image = line.Length > 2 && !string.IsNullOrEmpty(line[2]) ? File.ReadAllBytes(line[2]) : null;
                int? rating = null;
                int rat;
                if (line.Length > 3 && int.TryParse(line[3], out rat)) {
                    rating = rat;
                }

                bool isSuccess = Create(groupForUser, line, from, russian, image, rating);
                Console.WriteLine("{0}: {1}", isSuccess ? "Сохранено" : "Не сохранено",
                                  line.Aggregate((e1, e2) => e1 + " -> " + e2));
            } while (true);
        }

        protected abstract bool Create(GroupForUser groupForUser,
                                       string[] line,
                                       Language english,
                                       Language russian,
                                       byte[] image,
                                       int? rating);
    }
}