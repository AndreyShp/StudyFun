namespace BusinessLogic.ExternalData.Auxiliaries {
    public class InterviewAnswer {
        public InterviewAnswer(long id, string text) {
            Id = id;
            Text = text;
        }

        public long Id { get; private set; }
        public string Text { get; private set; }
    }
}