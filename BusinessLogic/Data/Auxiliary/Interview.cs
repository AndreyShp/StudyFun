namespace BusinessLogic.Data.Auxiliary {
    public class Interview {
        public long Id { get; set; }
        public string Text { get; set; }
        public int CountAnswers { get; set; }
        public long? ParentInterviewId { get; set; }
    }
}