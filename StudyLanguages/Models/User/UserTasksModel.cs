using System;
using System.Collections.Generic;
using BusinessLogic.DataQuery.UserRepository.Tasks;

namespace StudyLanguages.Models.User {
    public class UserTasksModel {
        private readonly string _taskId;

        public UserTasksModel(string taskId, List<UserTask> tasks, bool isBanned) {
            _taskId = taskId;
            Tasks = tasks;
            IsBanned = isBanned;
        }

        public List<UserTask> Tasks { get; private set; }

        public bool IsBanned { get; private set; }

        public bool HasHighlightRows {
            get { return !string.IsNullOrEmpty(_taskId); }
        }

        public bool NeedHighlightTask(UserTask task) {
            return task.Id.Equals(_taskId, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}