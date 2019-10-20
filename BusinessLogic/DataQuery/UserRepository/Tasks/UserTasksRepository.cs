using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataQuery.NoSql;
using BusinessLogic.Helpers;

namespace BusinessLogic.DataQuery.UserRepository.Tasks {
    public class UserTasksRepository {
        private const string TASK_TABLE = "Task";
        private const string SORT_TASK_TABLE = "SortTask";
        private const string COMMENT_TABLE = "Comment";
        private readonly Md5Helper _md5Helper = new Md5Helper();
        private readonly IRepositoryCache _repositoryCache;
        private readonly long _userId;

        public UserTasksRepository(IRepositoryCache repositoryCache, long userId) {
            _repositoryCache = repositoryCache;
            _userId = userId;
        }

        public UserTask AddTask(UserTask userTask) {
            userTask.Id = _md5Helper.GetHash(userTask.Text);
            userTask.DeletedDate = 0;
            bool isAddedToCommon = AddToCommonTasks(userTask);

            KeyValueRepository userRepository = _repositoryCache.GetUserRepository(_userId);
            UserTask result = null;
            if (isAddedToCommon && userRepository.Set(TASK_TABLE, userTask.Id, userTask)) {
                result = userRepository.Select<string, UserTask>(TASK_TABLE, userTask.Id, null);
            }
            return result;
        }

        public bool RemoveOrRestoreTask(string key, bool needRemove) {
            KeyValueRepository userRepository = _repositoryCache.GetUserRepository(_userId);
            //найти таск у текущего пользователя
            bool result = userRepository.SyncSet<string, UserTask>(TASK_TABLE, key, userTask => {
                if (needRemove && userTask.IsNotDeleted()) {
                    userTask.DeletedDate = DateTime.Now.Ticks;
                } else if (!needRemove) {
                    userTask.DeletedDate = 0;
                }
            });
            return result;
        }

        public List<UserTask> GetTasks() {
            //var userRepository = _repositoryCache.GetUserRepository(_userId);
            KeyValueRepository commonRepository = _repositoryCache.GetCommonRepository();

            List<List<CommonTaskInfo>> tasks =
                commonRepository.SelectAll<long, List<CommonTaskInfo>>(SORT_TASK_TABLE, SortOrder.Desc)
                ?? new List<List<CommonTaskInfo>>();
            var result = new List<UserTask>();

            //TODO: возвращать не все, а определенное кол-во

            //TODO: брать данные для текущего пользователя

            //NOTE: Distinct обеспечивается за счет перегрузки св-в в CommonTaskInfo
            foreach (CommonTaskInfo commonTaskInfo in tasks.SelectMany(e => e).Distinct()) {
                KeyValueRepository userRepository = _repositoryCache.GetUserRepository(commonTaskInfo.UserId);
                //var aaa = userRepository.SelectAll<string, UserTask>(TASK_TABLE);
                UserTask userTask = GetUserTask(userRepository, commonTaskInfo.Id);
                if (userTask != null && userTask.IsNotDeleted()) {
                    result.Add(userTask);
                }
            }
            return result;
        }

        private static UserTask GetUserTask(KeyValueRepository userRepository, string taskKey) {
            UserTask userTask = userRepository.Select<string, UserTask>(TASK_TABLE, taskKey, null);
            return userTask;
        }

        public UserTask GetTask(string key, bool fillComments) {
            KeyValueRepository userRepository = _repositoryCache.GetUserRepository(_userId);
            UserTask userTask = userRepository.Select<string, UserTask>(TASK_TABLE, key, null);
            if (userTask != null && !userTask.IsNotDeleted()) {
                //таск удален
                userTask = null;
            }
            if (userTask != null && fillComments) {
                List<TaskComment> comments = GetComments(key, 0);
                userTask.SetComments(comments);
            }
            return userTask;
        }

        public List<TaskComment> GetComments(string key, int lastShowedComment) {
            KeyValueRepository userRepository = _repositoryCache.GetUserRepository(_userId);
            List<TaskComment> comments = userRepository.Select(COMMENT_TABLE, key, new List<TaskComment>());

            var result = new List<TaskComment>();
            int countComments = comments.Count;
            if (countComments > lastShowedComment) {
                result = comments.Take(countComments - lastShowedComment).ToList();
            }
            return result;
        }

        public bool AddComment(string key, TaskComment comment) {
            KeyValueRepository userRepository = _repositoryCache.GetUserRepository(_userId);

            //TODO: проверить что такого коммента от этого же пользователя нет
            return userRepository.SyncSet(COMMENT_TABLE, key,
                                          comments => comments.Insert(0, comment),
                                          () => new List<TaskComment> {comment});
        }

        private bool AddToCommonTasks(UserTask userTask) {
            KeyValueRepository commonRepository = _repositoryCache.GetCommonRepository();

            var value = new CommonTaskInfo {Id = userTask.Id, UserId = _userId};
            bool result = commonRepository.SyncSet(SORT_TASK_TABLE, userTask.CreationDate,
                                                   dataByDate =>
                                                   dataByDate.Add(value),
                                                   () => new List<CommonTaskInfo>
                                                   {value});
            return result;
        }
    }
}