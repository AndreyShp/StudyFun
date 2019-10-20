using System;

namespace BusinessLogic.DataQuery.UserRepository.Tasks {
    public class TaskComment {
        public string Text { get; set; }
        public string Author { get; set; }
        public long AuthorId { get; set; }
        public long CreationDate { get; set; }
        public long Likes { get; set; }
        public long Dislikes { get; set; }
        public bool IsTheBestAnswer { get; set; }
    }
}