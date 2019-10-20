using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogic.DataQuery.Auxiliaries;
using BusinessLogic.Downloaders;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Helpers;
using BusinessLogic.Helpers.Caches;

namespace BusinessLogic.SalesGenerator {
    public class AllMaterialsSalesGenerator {
        private const DocumentType DOCUMENT_TYPE = DocumentType.Pdf;

        private readonly DiskCache _cache;
        private readonly string _domain;
        private readonly string _fontPath;
        private readonly Func<SectionId, bool> _forbiddenSectionsChecker;
        private readonly ZipCompressor _zipCompressor;

        public AllMaterialsSalesGenerator(string domain,
                                          string fontPath,
                                          DiskCache cache,
                                          Func<SectionId, bool> forbiddenSectionsChecker) {
            _domain = domain;
            _fontPath = fontPath;
            _cache = cache;
            _forbiddenSectionsChecker = forbiddenSectionsChecker;

            _zipCompressor = new ZipCompressor();
        }

        private static string GetKeyFromContent(
            Dictionary<SectionId, List<Tuple<long, string, DateTime>>> tupleBySections) {
            var allKeys = new StringBuilder();

            foreach (SectionId key in tupleBySections.Keys) {
                List<Tuple<long, string, DateTime>> tuple = tupleBySections[key];
                if (allKeys.Length > 0) {
                    allKeys.Append("_");
                }

                List<long> ids = tuple.Select(e => e.Item1).ToList();
                string keyPart = key + "-" + ids.Min() + "-" + ids.Max() + "-" + ids.Count() + "-"
                                 + tuple.Select(e => e.Item3).Max().Ticks;
                allKeys.Append(keyPart);
            }

            var md5Helper = new Md5Helper();
            string result = md5Helper.GetHash(allKeys.ToString());
            return result;
        }

        public byte[] Generate(long languageId, UserLanguages userLanguages) {
            var allMaterialsQuery = new AllMaterialsQuery(languageId, userLanguages, _forbiddenSectionsChecker);
            Dictionary<SectionId, List<Tuple<long, string, DateTime>>> tupleBySections =
                allMaterialsQuery.GetDataBySections();

            string keyFromContent = GetKeyFromContent(tupleBySections);
            string zipKey = "AllMaterials" + "_" + keyFromContent + "_" + userLanguages.From.ShortName + "_"
                            + userLanguages.To.ShortName + ".zip";
            byte[] result = _cache.Get(zipKey);
            if (result != null) {
                return result;
            }

            foreach (var pairBySection in tupleBySections) {
                SectionId sectionId = pairBySection.Key;
                string folderName = allMaterialsQuery.GetHeader(sectionId);

                foreach (var tuple in pairBySection.Value) {
                    DocumentationGenerator generator = GetGenerator(allMaterialsQuery, sectionId, tuple.Item1,
                                                                    tuple.Item2);
                    if (generator == null) {
                        //TODO: логировать и отсылать мне на почту
                        continue;
                    }

                    Stream fileContent = generator.Generate();
                    if (fileContent == null) {
                        //TODO: логировать и отсылать мне на почту
                        continue;
                    }

                    string entryName = folderName + "/" + generator.FileName;
                    _zipCompressor.AddFileToArchive(entryName, fileContent);
                }
            }

            result = _zipCompressor.GetArchive();
            WriteToCache(zipKey, result);
            return result;
        }

        private void WriteToCache(string key, byte[] data) {
            _cache.Save(key, data);
        }

        private DocumentationGenerator GetGenerator(AllMaterialsQuery allMaterialsQuery,
                                                    SectionId sectionId,
                                                    long id,
                                                    string title) {
            DocumentationGenerator result;
            switch (sectionId) {
                case SectionId.GroupByWords:
                    var groupWordsDownloader = new GroupDataDownloader(_domain, _fontPath) {
                        Header = string.Format("Слова на тему «{0}»", title),
                        TableHeader = "Слово"
                    };

                    List<SourceWithTranslation> words = allMaterialsQuery.GetWordsByGroup(id);
                    result = groupWordsDownloader.Download(DOCUMENT_TYPE, title, words);
                    break;

                case SectionId.GroupByPhrases:
                    var groupDataDownloader = new GroupDataDownloader(_domain, _fontPath) {
                        Header = string.Format("Фразы на тему «{0}»", title),
                        TableHeader = "Фраза"
                    };

                    List<SourceWithTranslation> sentences = allMaterialsQuery.GetSentencesByGroup(id);
                    result = groupDataDownloader.Download(DOCUMENT_TYPE, title, sentences);
                    break;

                case SectionId.VisualDictionary:
                    var visualDictionaryDownloader = new VisualDictionaryDownloader(_domain, _fontPath);

                    RepresentationForUser representationForUser = allMaterialsQuery.GetVisualDictionary(title);
                    result = visualDictionaryDownloader.Download(DOCUMENT_TYPE, title, representationForUser);
                    break;
                case SectionId.FillDifference:
                    var comparisonDownloader = new ComparisonDownloader(_domain, _fontPath);

                    ComparisonForUser comparisonForUser = allMaterialsQuery.GetComparison(title);
                    result = comparisonDownloader.Download(DOCUMENT_TYPE, title, comparisonForUser);
                    break;
                case SectionId.Video:
                    var videoTextDownloader = new VideoTextDownloader(_domain, _fontPath);

                    VideoForUser videoForUser = allMaterialsQuery.GetVideo(title);
                    result = videoTextDownloader.Download(DOCUMENT_TYPE, title, videoForUser);
                    break;
                case SectionId.PopularWord:
                    var popularWordsDownloader = new PopularWordsDownloader(_domain, _fontPath)
                    {Header = "Минилекс слов Гуннемарка"};

                    List<SourceWithTranslation> popularWords = allMaterialsQuery.GetPopularWords();
                    result = popularWordsDownloader.Download(DOCUMENT_TYPE, title, popularWords);
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }
    }
}