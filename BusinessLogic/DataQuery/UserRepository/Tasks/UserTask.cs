using System.Collections.Generic;

namespace BusinessLogic.DataQuery.UserRepository.Tasks {
    public class UserTask {
        private bool _allRights;
        private List<TaskComment> _comments;
        private bool _isBanned;

        public string Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public long AuthorId { get; set; }
        public long CreationDate { get; set; }
        public long DeletedDate { get; set; }

        public void SetComments(List<TaskComment> comments) {
            _comments = comments;
        }

        public List<TaskComment> GetComments() {
            return _comments ?? new List<TaskComment>();
        }

        public bool IsNotDeleted() {
            return DeletedDate == 0;
        }

        public void SetIsBanned(bool isBanned) {
            _isBanned = isBanned;
        }

        public bool IsBanned() {
            return _isBanned;
        }

        public void SetAllRights(bool allRights) {
            _allRights = allRights;
        }

        public bool IsAllRights() {
            return _allRights;
        }
    }
}