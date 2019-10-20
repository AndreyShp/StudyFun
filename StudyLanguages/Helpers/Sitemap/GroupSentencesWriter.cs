using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers.Sitemap {
    public class GroupSentencesWriter : BaseGroupsWriter {
        private Dictionary<long, List<SourceWithTranslation>> _sentencesForAllGroups;

        public GroupSentencesWriter(UserLanguages userLanguages, long languageId) : base(userLanguages, languageId) {}

        protected override GroupType GroupType {
            get { return GroupType.BySentence; }
        }

        protected override void WriteGroupContent(XElement root, long groupId, string name) {
            List<SourceWithTranslation> sentences;
            if (!_sentencesForAllGroups.TryGetValue(groupId, out sentences)) {
                return;
            }
            foreach (SourceWithTranslation sentenceFromGroup in sentences) {
                string sentenceForUrl = UrlBuilder.GetSpecialSentencesTrainerUrl(name,
                                                                                 sentenceFromGroup.Source.Id.ToString(
                                                                                     CultureInfo.InvariantCulture),
                                                                                 sentenceFromGroup.Translation.Id.
                                                                                     ToString(
                                                                                         CultureInfo.InvariantCulture));
                SitemapItemWriter.WriteUrlToResult(root, sentenceForUrl, INNER_GROUP_PRIORITY, ChangeFrequency.YEARLY);
            }
        }

        protected override void Load() {
            IGroupSentencesQuery groupWordsQuery = new GroupSentencesQuery();
            _sentencesForAllGroups = groupWordsQuery.GetForAllGroups(_userLanguages);
        }

        protected override string GetUrlByName(string name) {
            return UrlBuilder.GetGroupSentencesUrl(name);
        }

        protected override string GetSmartTrainerUrl(string name) {
            return UrlBuilder.GetGroupPhrasesTrainerUrl(name);
        }

        protected override string GetManualTrainerUrl(string name) {
            return UrlBuilder.GetSentencesTrainerUrl(name);
        }

        protected override string GetAllGroupsUrl() {
            return UrlBuilder.GetAllGroupSentencesUrl();
        }
    }
}