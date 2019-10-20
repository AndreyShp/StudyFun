using System.IO;
using System.Text;
using System.Xml.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Configs;

namespace StudyLanguages.Helpers.Sitemap {
    public class SitemapFileGenerator {
        private const string SITEMAP_KEY = "sitemap.xml";

        public static byte[] Generate(bool needGetFromCache) {
            byte[] result = needGetFromCache ? WebSettingsConfig.Instance.CommonDiskCache.Get(SITEMAP_KEY) : null;
            if (result != null) {
                return result;
            }

            result = GenerateFileContent();
            return result;
        }

        private static byte[] GenerateFileContent() {
            byte[] fileContent;
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            WriteUrls(root);

            using (var ms = new MemoryStream()) {
                using (var writer = new StreamWriter(ms, Encoding.UTF8)) {
                    root.Save(writer);
                }

                fileContent = ms.ToArray();

                WebSettingsConfig.Instance.CommonDiskCache.Save(SITEMAP_KEY, fileContent);
            }
            return fileContent;
        }

        private static void WriteUrls(XElement root) {
            var sitemapItemWriter = new SitemapItemWriter();
            sitemapItemWriter.WriteUrlToResult(root, "", 1);

            UserLanguages userLanguages = WebSettingsConfig.Instance.DefaultUserLanguages;
            long languageId = WebSettingsConfig.Instance.GetLanguageFromId();
            if (UserLanguages.IsInvalid(userLanguages) || IdValidator.IsInvalid(languageId)) {
                return;
            }

            var visualDictionariesSalesSettings = WebSettingsConfig.Instance.GetSalesSettings(SectionId.VisualDictionary);
            if ( WebSettingsConfig.Instance.IsSectionAllowed(SectionId.VisualDictionary)) {
                var visualDictionariesWriter = new VisualDictionariesWriter(languageId);
                visualDictionariesWriter.WriteDictionaries(root, visualDictionariesSalesSettings);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.GroupByWords)) {
                var groupWordsWriter = new GroupWordsWriter(userLanguages, languageId);
                groupWordsWriter.WriteGroups(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.GroupByPhrases)) {
                var groupSentencesWriter = new GroupSentencesWriter(userLanguages, languageId);
                groupSentencesWriter.WriteGroups(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.FillDifference)) {
                var comparisonsWriter = new ComparisonsWriter(languageId);
                comparisonsWriter.WriteComparisons(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.PopularWord)) {
                string knowledgeGeneratorUrl = UrlBuilder.GetPopularWordsUrl();
                sitemapItemWriter.WriteUrlToResult(root, knowledgeGeneratorUrl, 0.5m, ChangeFrequency.YEARLY);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.UserTasks)) {
                var userTasksWriter = new UserTasksWriter();
                userTasksWriter.WriteTasks(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.KnowledgeGenerator)) {
                string knowledgeGeneratorUrl = UrlBuilder.GetKnowledgeGeneratorUrl();
                sitemapItemWriter.WriteUrlToResult(root, knowledgeGeneratorUrl, 0.4m, ChangeFrequency.ALWAYS);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.Video)) {
                var videosWriter = new VideosWriter(languageId);
                videosWriter.WriteVideos(root, VideoType.Clip);
                videosWriter.WriteVideos(root, VideoType.Movie);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.TVSeries)) {
                var videosWriter = new TVSeriesWriter(languageId);
                videosWriter.WriteTVSeries(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.Sentences)) {
                var homeWriter = new HomeWriter(userLanguages);
                homeWriter.Write(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.Audio)) {
                var audioWordsWriter = new AudioWordsWriter(userLanguages);
                audioWordsWriter.Write(root);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.WordTranslation)) {
                sitemapItemWriter.WriteUrlToResult(root,
                                                   UrlBuilder.GetTranslationDefaulUrl(
                                                       RouteConfig.WORDS_TRANSLATION_CONTROLLER), 0.1m,
                                                   ChangeFrequency.MONTHLY);
            }

            if (WebSettingsConfig.Instance.IsSectionAllowed(SectionId.PhraseVerbTranslation)) {
                sitemapItemWriter.WriteUrlToResult(root,
                                                   UrlBuilder.GetTranslationDefaulUrl(
                                                       RouteConfig.PHRASAL_VERBS_TRANLATION_CONTROLLER), 0.1m,
                                                   ChangeFrequency.MONTHLY);
            }
        }
    }
}