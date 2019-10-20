using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers.Sitemap {
    public class GroupWordsWriter : BaseGroupsWriter {
        private Dictionary<long, List<SourceWithTranslation>> _wordsForAllGroups;

        public GroupWordsWriter(UserLanguages userLanguages, long languageId) : base(userLanguages, languageId) { }

        protected override GroupType GroupType {
            get { return GroupType.ByWord; }
        }

        protected override void WriteGroupContent(XElement root, long groupId, string name) {
            List<SourceWithTranslation> words;
            if (!_wordsForAllGroups.TryGetValue(groupId, out words)) {
                return;
            }
            foreach (SourceWithTranslation wordFromGroup in words) {
                string wordForUrl = UrlBuilder.GetSpecialWordsTrainerUrl(name, wordFromGroup.Source.Text, wordFromGroup.Translation.Text);
                SitemapItemWriter.WriteUrlToResult(root, wordForUrl, INNER_GROUP_PRIORITY, ChangeFrequency.YEARLY);
            }
        }

        protected override void Load() {
            IGroupWordsQuery groupWordsQuery = new GroupWordsQuery();
            _wordsForAllGroups = groupWordsQuery.GetForAllGroups(_userLanguages);
        }

        protected override string GetUrlByName(string name) {
            return UrlBuilder.GetGroupWordsUrl(name);
        }

        protected override string GetSmartTrainerUrl(string name) {
            return UrlBuilder.GetGroupWordsTrainerUrl(name);
        }

        protected override string GetManualTrainerUrl(string name) {
            return UrlBuilder.GetWordsTrainerUrl(name);
        }

        protected override string GetAllGroupsUrl() {
            return UrlBuilder.GetAllGroupWordsUrl();
        }
    }
}