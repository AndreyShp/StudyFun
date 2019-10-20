using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.DataQuery.UserRepository.Texts;
using BusinessLogic.ExternalData;
using BusinessLogic.Helpers.Caches;
using BusinessLogic.SalesGenerator;
using BusinessLogic.Video;
using Sandbox.Classes;
using Sandbox.Classes.GroupFiller;
using Sandbox.Classes.Video;

namespace Sandbox {
    internal class Program {
        private static void Main(string[] args) {
            /*var colors = new[] {
                Color.Gray, Color.DarkGray, Color.DimGray, Color.Gainsboro, Color.Lavender, Color.Silver,
                Color.LightSlateGray, Color.White, Color.WhiteSmoke, Color.Thistle
            };

            foreach (var color in colors) {
                int numericColor = ColorToInt(color);
                Console.WriteLine("{0} {1} Проверочное {2}{3}{4}", color, numericColor, color.R.ToString("X"), color.G.ToString("X"),color.B.ToString("X") );
            }*/

            /* var purchasedGoodsQuery = new PurchasedGoodsQuery();
            long paymentId = purchasedGoodsQuery.WantToBuyGoods(new PurchasedGoodsForUser {
                Goods = new byte[0],
                LanguageId = 3,
                ShortDescription = "краткое описание",
                FullDescription = "полное описание",
                Price = 10,
                PaymentSystem = PaymentSystem.Robokassa
            });
            purchasedGoodsQuery.FailedPurchased(paymentId);

            string aa = MoneyFormatter.ToRubles(87.21m);
*/

            //YandexVideoSearcher.Run();

            /*VideoProcessor.Run();*/

            var tvSeriesCreator = new TVSeriesCreator(LanguageShortName.En, @"C:\Projects\StudyLanguages\StudyLanguages\App_Data\Series", "Friends", "Друзья");
            tvSeriesCreator.Create(1, 1, @"C:\Projects\Сериалы\Друзья\1_1.mp4",
                                   @"C:\Projects\Сериалы\Друзья\Субтитры\en\Friends.S01E01.DVDRip.SAINTS.en.srt", 
                                   "The One Where Monica Gets a Roommate", "Эпизод, где Моника берёт новую соседку", new TimeSpan(0, 0, 12)
                );
            tvSeriesCreator.Create(1, 2, @"C:\Projects\Сериалы\Друзья\1_2.mp4",
                                   @"C:\Projects\Сериалы\Друзья\Субтитры\en\Friends.S01E02.DVDRip.SAINTS.en.srt",
                                   "The One With The Sonogram At The End", "Эпизод с Сонограммой в конце", new TimeSpan(0, 0, 3)
                );

            /*var sentenceWordsQuery = new SentenceWordsQuery(3);
            var aaaa = sentenceWordsQuery.GetWordsBySentenceId(10);
*/
            Console.WriteLine("Все!!!");
            Console.ReadLine();
            return;

            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                           @"..\..\..\StudyLanguages\App_Data\arial.ttf");
            string domain = "studyfun.ru";
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FilesForSale");

            /*ZipCompressor compressor = new ZipCompressor();
            compressor.AddTextFile("1.txt", "ajajajs jas");
            compressor.AddTextFile("2.txt", "ajajajs j1123as");
            compressor.WriteArchive(Path.Combine(outputPath, "proba.zip"));*/

            var query = new RepresentationsQuery(3);
            var ids = new HashSet<long>(query.GetVisibleWithoutAreas().Select(e => e.Id));
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            const int COUNT = 10;
            for (int i = 0; i < COUNT; i++) {
                var creator = new RepresentationsSalesGenerator(domain, fontPath,
                                                                new DiskCache(
                                                                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                                                 "Caches")));
                creator.Generate(LanguageShortName.En, LanguageShortName.Ru, ids);
            }
            stopWatch.Stop();
            Console.WriteLine("Среднее время {0}. Общее время {1}!!!", stopWatch.Elapsed.TotalSeconds / COUNT,
                              stopWatch.Elapsed.TotalSeconds);
            Console.ReadLine();
            return;

            string text = File.ReadAllText(@"C:\Projects\StudyLanguages\Тексты\Белоснежка.txt", Encoding.UTF8);
            var separator = new TextSeparator(text);
            List<TextWord> words = separator.GetWords();
            Console.WriteLine(words.Count);
            for (int i = 0; i < words.Count; i += 100) {
                string partWords = string.Join(" ", words.Select(e => e.Word).Skip(i).Take(100));
                Console.WriteLine(partWords);
                Console.ReadLine();
            }
            if (words.Count % 100 > 0) {
                string partWords = string.Join(" ",
                                               words.Select(e => e.Word).Skip(words.Count / 100).Take(words.Count % 100));
                Console.WriteLine(partWords);
            }
            Console.ReadLine();
            return;

            var popularWordsCreator = new PopularWordsCreator(LanguageShortName.De, PopularWordType.Minileks);
            popularWordsCreator.Create(@"C:\Projects\StudyLanguages\Источники слов\Minileks_De.csv");
            Console.WriteLine("С минилексом всё!!!");

            popularWordsCreator = new PopularWordsCreator(LanguageShortName.Fr, PopularWordType.Minileks);
            popularWordsCreator.Create(@"C:\Projects\StudyLanguages\Источники слов\Minileks_Fr.csv");
            Console.WriteLine("С минилексом всё!!!");

            popularWordsCreator = new PopularWordsCreator(LanguageShortName.It, PopularWordType.Minileks);
            popularWordsCreator.Create(@"C:\Projects\StudyLanguages\Источники слов\Minileks_It.csv");
            Console.WriteLine("С минилексом всё!!!");

            popularWordsCreator = new PopularWordsCreator(LanguageShortName.Es, PopularWordType.Minileks);
            popularWordsCreator.Create(@"C:\Projects\StudyLanguages\Источники слов\Minileks_Es.csv");
            Console.WriteLine("С минилексом всё!!!");
            Console.ReadLine();
            return;

            /* SpeakerFiller.Fill();
            return;*/
            /*const LanguageShortName LANGUAGE_NAME = LanguageShortName.Es;
            var converterDataToOtherLanguage = new ConverterDataToOtherLanguage(LANGUAGE_NAME);
            var minileksType = PopularWordType.Minileks;
            converterDataToOtherLanguage.ConvertPopularWords(minileksType);
            converterDataToOtherLanguage.ConvertVisualDictionaries();
            converterDataToOtherLanguage.ConvertGroupWords();
            converterDataToOtherLanguage.ConvertGroupSentences();

            var minileksFileName = converterDataToOtherLanguage.GetFileName(minileksType);
            OtherLanguagesFilesCreator otherLanguagesFilesCreator = new OtherLanguagesFilesCreator(LANGUAGE_NAME);
            otherLanguagesFilesCreator.ConvertMinileks(minileksFileName);
            otherLanguagesFilesCreator.ConvertVisualDictionaries();
            otherLanguagesFilesCreator.ConvertGroupWords();
            otherLanguagesFilesCreator.ConvertGroupSentences();

            Console.WriteLine("Всё");
            Console.ReadLine();
            return;*/
            //C:\HostingSpaces\dreyFake@yandex.ru1\studyfun.ru\data\StudyFun.bak 
            //C:\HostingSpaces\ВАШ_ЛОГИН\ВАШ_ДОМЕН\wwwroot\

            /*var url =
                "http://studyfun.ru/%d0%a2%d1%80%d0%b5%d0%bd%d0%b0%d0%b6%d0%b5%d1%80%20%d1%81%d0%bb%d0%be%d0%b2%20%d0%bf%d0%be%20%d1%82%d0%b5%d0%bc%d0%b5/%d0%9c%d0%b5%d0%b1%d0%b5%d0%bb%d1%8c/rubbush/%d0%bc%d1%83%d1%81%d0%be%d1%80";
            var request =WebRequest.CreateHttp(url);
            var resp = request.GetResponse();
            var headers = resp.Headers;
            StringBuilder sb = new StringBuilder();
            var streamReader = new StreamReader(resp.GetResponseStream());
            var content = streamReader.ReadToEnd();*/

            var videoNames = new[] {
                "Amsterdam Travel Guide",
                "Bali Travel Guide",
                "Bangkok Travel Guide",
                "Berlin Travel Guide",
                "Boston Travel Guide",
                "Cairns Travel Guide",
                "Cancun Travel Guide",
                "Chicago Travel Guide",
                "Fiji Travel Guide",
                "Gold Coast Travel Guide",
                "Hawaii's Big Island Travel Guide",
                "Hong Kong Travel Guide",
                "Honolulu Travel Guide",
                "Las Vegas Travel Guide",
                "Lombok Travel Guide",
                "London Travel Guide",
                "Los Angeles Travel Guide",
                "Melbourne Travel Guide",
                "Miami Travel Guide",
                "Ottawa Travel Guide",
                "Paris Travel Guide",
                "Perth Travel Guide",
                "Prague Travel Guide",
                "Rio de Janeiro Travel Guide",
                "Rome Travel Guide",
                "San Francisco Travel Guide",
                "Singapore Travel Guide",
                "Sydney Travel Guide",
                "Taipei Travel Guide",
                "Toronto Travel Guide",
                "Wellington Travel Guide",
                /* "Jaguar Attacks Crocodile",
                "London Native English Speaker Interviews Part 1",
                "London Native English Speaker Interviews Part 2",
                "London Native English Speaker Interviews Part 3",
                "London Native English Speaker Interviews Part 4",
                "London Native English Speaker Interviews Part 5",
                "London Native English Speaker Interviews Part 6",
                "Effective Listening Skills",
                "British vs American Accents Part 1",
                "British vs American Accents Part 2",
                "Farage vs Clegg Row over Russia's role in Crimea",*/
                //, "Steve Jobs speech at Stanford",
                // "Useful Russian accent",
                //"Why I Hate School But Love Education"
            };
            /*var videosFiller = new VideosFiller();
            foreach (string name in videoNames) {
                string videoFile = string.Format(@"C:\Projects\StudyLanguages\Источники видео\{0}.txt", name);
                var videoReader = new VideoReader(videoFile);
                VideoForUser videoForUser = videoReader.Read();
                if (videoForUser != null) {
                    videosFiller.Fill(videoForUser);
                }
            }
            Console.WriteLine("С видео всё!");
            Console.ReadLine();
            return;*/

            /*var items = new[] {"1", "2", "3", "4", "5"};
            var memo = new MemoTrainerDemo(items);
            for (int i = 1; i <= 100; i++) {
                string item = memo.Get();
                Console.WriteLine("Взяли пункт: {0}\r\nВведите оценку от 0 до 5... 0 - полное затменее, 5 - великолепный ответ", item);
                string dirtyMark = Console.ReadLine();
                int mark;
                if (int.TryParse(dirtyMark, out mark) && mark >= 0 && mark <= 5) {
                    memo.SetMark(item, mark);
                }

                if (i % items.Length == 0) {
                    Console.WriteLine(memo.GetDump());
                }
            }
            return;*/

            /*LemmatizerHelper.Run();
            return;*/

            /*TaskCleaner.Clear();
            return;*/

            /*var wordFilesNames2 = new[] {
                "Хобби и увлечения"/*, "Образование", "Отдых и досуг", "Праздники", "Строительство", "Школьные предметы"#1#};
            foreach (var groupName in wordFilesNames2) {
                WordImageFiller.Fill(groupName, @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\");
            }
            Console.WriteLine("Все!");
            Console.ReadLine();
            return;*/

            /*var groupComparisonNames = new[] {
                "беспокоить, беспокоиться"/*,"юрист","отказаться", "открыть, открывать", "обращаться", "объединяться","нанимать", "напряжение", "несколько", "обычный"
                ,"дом", "живой","выбирать", "высокий","возможность", "возражать", "важный", "везучий", "недоставать", "повреждать", "повторять" 
                , "следить", "цель", "берег", "изменять", "бегать, бежать", "убегать", "магазин", "любопытный", "любить", "лить, литься", "смущать", "соглашаться"
                ,"быстрый","картина","отказывать","отклонять","хозяин","хватать","двигаться", "различие", "разрешение", "упоминать","защищать"
                ,"земля","интересный","оценивать","поездка","тихий", "товар","течь","современный","собирать","скучный","грубый","лекарство", "последний", "походить",
                "объединять", "общий", "обвинять", "носить", "недавно", "дело", "много", "настоящий", "главным образом", "голый", "делить",
                "главный", "замечание", "здоровый", "между", "жертва", "не любить", "лестница", "тайна", "называться", "направлять", "сердитый"
                ,"поздравлять", "полный", "колени", "команда", "одалживать", "около", "оставаться", "положение", "довольный", "до того как", "дорога", "достигать", 
                "домашняя работа", "происходить", "пятно", 
                "отвечать", "чайник", "часы" ,"твердый", "чинить", "pair и couple", "act и action", "lease и rent", "shade и shadow", "each и every"#1#
            };

            foreach (string groupComparisonName in groupComparisonNames) {
                var filler = new GroupComparisonFiller();
                filler.Create(
                    string.Format(@"C:\Projects\StudyLanguages\Источники сравнения\{0}.xml",
                                  groupComparisonName));
            }

            Console.WriteLine("С группами сравнения все!");
            Console.ReadLine();
            return;*/

            /*var sentenceParser = new TextParser();
            var fileNames = new[] {
                "Beethoven"/*, "Красная шапочка", "Золушка", "Белоснежка"#1#
            };
            foreach (string fileName in fileNames) {
                string text = File.ReadAllText(string.Format(@"C:\Projects\StudyLanguages\Тексты\{0}.txt", fileName));
                sentenceParser.ParseText(text);
                Console.WriteLine("\r\n");
            }*/

            /* var sentencesSearchEngine = SentencesSearchEngineFactory.Create(null, 3);
            var yyy = sentencesSearchEngine.FindSentences("die was" , OrderWordsInSearch.Any);
            yyy = sentencesSearchEngine.FindSentences("die was", OrderWordsInSearch.ExactWordForWord);
            var ddd = sentencesSearchEngine.FindSentences("be", OrderWordsInSearch.Any);
            var rrr = sentencesSearchEngine.FindSentences("tanque", OrderWordsInSearch.Any);
            var aaa = sentencesSearchEngine.FindSentences("what", OrderWordsInSearch.Any);
            var bbb = sentencesSearchEngine.FindSentences("died was", OrderWordsInSearch.Any);

            yyy = sentencesSearchEngine.FindSentences("she is", OrderWordsInSearch.Any);
            yyy = sentencesSearchEngine.FindSentences("she is", OrderWordsInSearch.ExactWordForWord);
            yyy = sentencesSearchEngine.FindSentences("is she", OrderWordsInSearch.ExactWordForWord);
            
            Console.WriteLine("С текстом все!");
            Console.ReadLine();
            return;*/

            /*
            TextProcessor textProcessor = new TextProcessor(3);
            var fileNames = new[] {
                "Beethoven", "Красная шапочка", "Золушка", "Белоснежка"
            };
            foreach (string fileName in fileNames) {
                string text = File.ReadAllText(string.Format(@"C:\Projects\StudyLanguages\Тексты\{0}.txt", fileName));
                textProcessor.AnalyzeText(text);
            }
            Console.WriteLine("С текстом все!");
            Console.ReadLine();
            return;*/

            /*ImageConverter.ResizeAllByMode();
            return;*/

            /* ImagesFiller.FillRepresentation();
            Console.WriteLine("Визуальные словари пересохранены!");
            ImagesFiller.FillGroups();
            Console.WriteLine("Картинки пересохранены!");
            Console.ReadLine();
            return;
*/

            /*            
            Console.WriteLine("Озвученно");
            Console.ReadLine();
            return;*/

            /*new SentencesFiller().Fill();
            return;*/
            /* var dictionariesFileNames = new[] {
                "геометрические фигуры" /*, "отношения между людьми", "видимость", "погода", "климат и явления природы",
                ,"месяцы", "аптека", "в супермаркете", "канцелярские принадлежности", "мебель", "обувь",
                   ,"материалы", "на пляже", "хобби", "рост и телосложение", "волосы и прически", "уборка дома", "Стороны света, времена года, сутки", "магазины", "глаголы"
                  ,"состояние", "транспорт", "посуда", "Цифры", "женская сумочка", "флора и фауна"
                 ,"украшения", "дикие животные", "семья", "мужская одежда", "аптечка", "дни недели", "улица", "зимние виды спорта", "инструменты", "средства личной гигиены", 
                 "бытовая техника", "профессии", "женская одежда", "виды спорта", "игрушки", "школьный класс", "эмоции",
                 "автомобиль", "Мясные блюда", "Цвета", "Овощи", "Фрукты","Домашние животные", "глаголы движений", "cafe", "kitchen", "cabinet", 
                      "livingRoom", "hallway", "bedroom", "bathroom", "person", "face", "muscles", /*"internal organs", "skeleton"#1#
            };*/

            const LanguageShortName LANGUAGE = LanguageShortName.It;

            /*new CrossReferencesFiller().Fill(LANGUAGE, @"C:\Projects\StudyLanguages\CrossReferences.csv");
            return;*/

            List<string> dictionariesFileNames =
                Directory.GetFiles(@"C:\Projects\StudyLanguages\Источники визуального словаря\" + LANGUAGE + "\\",
                                   "*.csv").ToList();

            foreach (string dictionaryFileName in dictionariesFileNames) {
                var filler = new VisualDictionaryFiller(LANGUAGE);
                filler.Create(
                    /*string.Format(@"C:\Projects\StudyLanguages\Источники визуального словаря\{0}.csv",
                                  dictionaryFileName) */dictionaryFileName,
                                                                                                                                                                  @"C:\Projects\StudyLanguages\Источники визуального словаря\Картинки\{0}.jpg");
            }

            Console.WriteLine("С визуальными словарями все!");
            /* Console.ReadLine();
            return;*/

            /* 
            var dict = new Dictionary<string, string> {
                 {"Лицо", "face"},
                    {"Человек", "person"},
                    {"Мускулы","muscles"},
                 {"Скелет","Skeleton"},
                 {"Ванная","bathroom"},
                 {"Гостиная","livingRoom"},
                 {"Кабинет","cabinet"},
                 {"Кафе","cafe"},
                 {"Кухня","kitchen"},
                 {"Прихожая","hallway"},
                 {"Спальня","bedroom"},
                 
            };*/
            /*foreach (var pair in dict)
            {*/

            /* IWordsQuery wordsQuery = IoCModule.Create<WordsQuery>();
            var userLanguages = new UserLanguages {
                From = new Language {Id = 1, Name = "English", ShortName = "en"},
                To = new Language {Id = 2, Name = "Русский", ShortName = "ru"},
            };
            WordsByPattern a = wordsQuery.GetLikeWords(userLanguages, "get", WordType.PhrasalVerb);*/

            /*var wf = new WordFiller();
            for (int i = 1; i <= 3; i++) {
                wf.FillFromHtml(
                    string.Format(@"C:\Projects\StudyLanguages\Источники слов\ksocrat-ruen-dic-1.0.1\{0}000.htm", i),
                    (counter, source, translations, isSuccess) => {
                        Console.WriteLine("{0}. {1}: {2}", counter, isSuccess ? "Сохранено" : "Не сохранено",
                                          source + " -> " + string.Join(",", translations));
                    });
            }*/

            /*WordFiller wf = new WordFiller();
            wf.FillByCSV(@"C:\Projects\StudyLanguages\Источники слов\phrasalverbs.csv",
                         (counter, source, translation, isSaved) =>
                         Console.WriteLine("{0}. {1}: {2}", counter, isSaved ? "Сохранено" : "Не сохранено",
                                           source + " -> " + translation));
*/
            /*var wordFilesNames = new[] {
                "meeting, congress", "religion"
                /*"election", "ideas, ideology", "man and society. human relations", "political system", "politics",
                "public bodies", "state power",
               "criminal offence", "land, state", "law",
                "agriculture", "construction", "hobby", "production","being at work", "work","capital", "property", "fleet", "railway",
                "linguistics", "man and society. human relations", "moral categories", 
                "maths", "science", "education", "environment", "matter", "space, celsetial bodies, atmosthere",
                "music", "theatre, circus, cinema, variety show", "creative work", "culture. art", "literature", "painting, sculpture, architecture" 
                "rule", "structure", "mass media", "telephone", "war. arms"
                "locality",
                 "time","family", "health care. medical treatment",
                "eyesight","speech" , "activity", "clothes", "guest", "leisure", "love", "sorry", "sport"#1#
            };*/

            List<string> phrasesFileNames =
                Directory.GetFiles(@"C:\Projects\StudyLanguages\Источники для групп\Group\" + LANGUAGE + "\\", "*.csv").
                    ToList();

            var groupSentenceFiller = new GroupSentenceCreator(LANGUAGE);
            foreach (string phrasesFileName in phrasesFileNames) {
                groupSentenceFiller.Create(phrasesFileName
                    /* string.Format(@"C:\Projects\StudyLanguages\Источники для групп\Group\De\{0}.csv", phrasesFileName)*/);
            }

            /* var wordsFileNames = new[] {
                "23 февраля", "8 марта" /*"дни недели, месяцы, времена года","meeting, congress", "religion"
                ,"election", "ideas, ideology", "man and society. human relations", "political system", "politics",
                "public bodies", "state power"
                ,"criminal offence", "land, state", "law",
                "agriculture", "construction", "economy", "fruits", "hobby", "industry", "kinds of sport", "materials", "pets", 
                "production", "sport", "vegetables", "being at work", "computer",
                "work", "machinery, tools", "capital", "property", "aviation", "fleet", "railway", "linguistics", "man and society. human relations", "moral categories",
                "maths", "physics, chemistry", "science", "education", "city transport", "state", "water"
                "parts of the world. continents", "climate, weather", "landscape",
                "environment", "matter", "space, celsetial bodies, atmosthere", "music", "theatre, circus, cinema, variety show" 
                 "creative work", "culture. art", "literature", "painting, sculpture, architecture" 
                "changing", "form", "replacement"
                "cause", "connecting", "order", "rule", "sequence", "structure", "mass media", "telephone", "post, telegraph", 
                "symptoms of disease", "war. arms", "documents", "international relations", "safety protection""direction","distance","locality","space",
                 "time", "time measurement", "time order", "existense, being", "sleep"
                "furniture","hygiene. make-up","things","human behaviour", "human relations", "quality", "significance", "valuation" 
                "degree", "measurement", "quantity", "valuation size and weight"
                "moving", "parts of plants", "plants" , "health care. medical treatment", "hearing", "life, age", "relating", "sense of smell",
                "taste", "temperature", "touch",
                "appearance", "eyesight", "sensation" "human body"
                "amphibians and reptiles", "animals", "birds", "fish", "group of animals. dwelling", "insects",
                "parts of animals bodies",
                "dwelling, house","money","movements", "cutlery", "dairy", "desserts", "drinks", "first course","flour","fruits" ,"grain","meat and seafood"
                ,"pernicious habits", "professions", "seasoning", "side dish", "snack", "vegetables"
                "at work", "speech"
                , "city, town, village", "clothes","colors","dairy","desserts","drinks","education","emotion","entertainment","family","footwear",
                "fruits","grain","leisure","meat and seafood","moving","professions","public holidays","sport","transport","vegetables","will, volition"#1#
            };*/

            List<string> wordsFileNames =
                Directory.GetFiles(@"C:\Projects\StudyLanguages\Источники для групп\Word\" + LANGUAGE + "\\", "*.csv").
                    ToList();

            var groupWordFiller = new GroupWordCreator(LANGUAGE);
            foreach (string wordFileName in wordsFileNames) {
                groupWordFiller.Create(
                    /*string.Format(@"C:\Projects\StudyLanguages\Источники для групп\Word\De\{0}.csv", wordFileName*/
                    wordFileName);
            }
            Console.WriteLine("Все!!!");

            // new GroupWordCreator().Create(@"C:\Projects\StudyLanguages\Источники для групп\drinks.csv");

            //new GroupWordCreator().Create(@"C:\Projects\StudyLanguages\Источники для групп\family.csv");

            //"(?<!\\(.*)(?!.*\\)),"
            /*Regex _regex = new Regex("(?<!\\([^)]*),(?![^(]*\\))", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            var aa =
                _regex.Split(
                    "ещё, кроме (с вопроситительными местоимениями, а также с местоимениями и наречиями, производными от some, any, no)");
            aa = _regex.Split(
                "принадлежащий ей (часто употребляется в грамматических конструкциях, в которых не может быть употреблена атрибутивная форма, например, фывфыв)");
            aa = _regex.Split("asdasd asd ,asdasdasdasd");*/

            /*var image = File.ReadAllBytes(@"C:\Projects\StudyLanguages\Источники картинок\Цвета\red.jpg");
            var id = 13788;*/
            /*IoCModule.Create<IDbAdapter>().ActionByContext(c => {
                IQueryable<Group> groupsQuery = (from g in c.Group
                                                 where g.Id == id
                                                 select g);
                Group group = groupsQuery.FirstOrDefault();
                group.Image = image;
            }, true);*/

            /*IoCModule.Create<IDbAdapter>().ActionByContext(c => {
                IQueryable<WordTranslation> wordTranslations = (from g in c.WordTranslation
                                                 where g.Id == id
                                                 select g);
                WordTranslation wordTranslation = wordTranslations.FirstOrDefault();
                wordTranslation.Image = image;
            }, true);

            var wordFiller = new WordFiller();*/
            /*var fileNames = new[] {
                @"C:\Projects\StudyLanguages\2000\2000 (En-Ru).xml",
                @"C:\Projects\StudyLanguages\2000\2000-F-L(En-Ru).xml",
                @"C:\Projects\StudyLanguages\2000\2000-M-P(En-Ru).xml",
                @"C:\Projects\StudyLanguages\2000\2000-Q-Z(En-Ru).xml"
            };*/
            /*            var fileNames = new[] {
                @"C:\Projects\StudyLanguages\ksocrat-ruen-dic-1.0.1\1000.htm",
                @"C:\Projects\StudyLanguages\ksocrat-ruen-dic-1.0.1\2000.htm",
                @"C:\Projects\StudyLanguages\ksocrat-ruen-dic-1.0.1\3000.htm",
            };
            foreach (string fileName in fileNames) {
                wordFiller.FillFromHtml(fileName,
                                (counter, word, translationsWords, isSuccess) =>
                                Console.WriteLine("{0}.{1} - {2} - {3}", counter, word,
                                                  string.Join(",", translationsWords), isSuccess ? "успех" : "НЕУДАЧА"));
            }*/

            /*bool result = wordFiller.Create("but", new[] {
                "но",
                "а",
                "однако",
                "зато",
                "впрочем",
                "тем не менее",
                "как не",
                "кроме",
                "за исключением"
            });
            if (!result) {
                Console.WriteLine("Не удалось сохранить слово!");
            }*/
            Console.ReadLine();
        }
    }
}