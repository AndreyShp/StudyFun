namespace BusinessLogic.DataQuery.UserRepository.Tasks {
    public class CommonTaskInfo {
        public string Id { get; set; }
        public long UserId { get; set; }

        public override int GetHashCode() {
            return (Id + "_" + UserId).GetHashCode();
        }

        public override bool Equals(object obj) {
            var parsedObj = obj as CommonTaskInfo;
            if (parsedObj == null) {
                return false;
            }
            return Id.Equals(parsedObj.Id) && UserId.Equals(parsedObj.UserId);
        }
    }
}