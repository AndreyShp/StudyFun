using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers.Sitemap {
    public abstract class BaseGroupsWriter {
        private const decimal TRAINER_GROUP_PRIORITY = 0.4m;
        protected const decimal INNER_GROUP_PRIORITY = 0.3m;

        protected readonly SitemapItemWriter SitemapItemWriter = new SitemapItemWriter();
        protected readonly UserLanguages _userLanguages;
        private readonly long _languageId;

        protected BaseGroupsWriter(UserLanguages userLanguages, long languageId) {
            _userLanguages = userLanguages;
            _languageId = languageId;
        }

        protected abstract GroupType GroupType { get; }
        protected abstract void WriteGroupContent(XElement root, long groupId, string name);
        protected abstract void Load();
        protected abstract string GetUrlByName(string name);
        protected abstract string GetSmartTrainerUrl(string name);
        protected abstract string GetManualTrainerUrl(string name);
        protected abstract string GetAllGroupsUrl();

        public void WriteGroups(XElement root) {
            Load();

            string allGroupsUrl = GetAllGroupsUrl();
            SitemapItemWriter.WriteUrlToResult(root, allGroupsUrl, 1);

            IGroupsQuery groupsQuery = new GroupsQuery(_languageId);
            List<GroupForUser> visibleGroups = groupsQuery.GetVisibleGroups(GroupType);

            foreach (GroupForUser visibleGroup in visibleGroups) {
                string name = visibleGroup.Name;
                string url = GetUrlByName(name);
                SitemapItemWriter.WriteUrlToResult(root, url, 0.9m);

                url = GetSmartTrainerUrl(name);
                SitemapItemWriter.WriteUrlToResult(root, url, TRAINER_GROUP_PRIORITY, ChangeFrequency.YEARLY);

                url = GetManualTrainerUrl(name);
                SitemapItemWriter.WriteUrlToResult(root, url, INNER_GROUP_PRIORITY, ChangeFrequency.YEARLY);
                //записать содержимое группы
                WriteGroupContent(root, visibleGroup.Id, name);
            }
        }
    }
}