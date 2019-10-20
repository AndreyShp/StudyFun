using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.DataQuery.UserRepository;
using BusinessLogic.DataQuery.UserRepository.Tasks;
using BusinessLogic.Validators;
using StudyLanguages.Configs;

namespace StudyLanguages.Helpers.Sitemap {
    public class UserTasksWriter {
        private readonly SitemapItemWriter _sitemapItemWriter = new SitemapItemWriter();

        public void WriteTasks(XElement root) {
            _sitemapItemWriter.WriteUrlToResult(root, UrlBuilder.GetUserTasksUrl(), 0.5m);

            RepositoryFactory repositoryFactory = WebSettingsConfig.Instance.GetRepositoryFactory();
            UserTasksRepository userTasksRepository = repositoryFactory.CreateUserRepository(IdValidator.INVALID_ID);
            List<UserTask> tasks = userTasksRepository.GetTasks();
            foreach (UserTask userTask in tasks) {
                string url = UrlBuilder.GetUserTaskUrl(userTask.AuthorId, userTask.Id);
                _sitemapItemWriter.WriteUrlToResult(root, url, 0.4m);
            }
        }
    }
}