using System.Collections.Generic;

namespace BusinessLogic.ExternalData.Auxiliaries {
    public class Interview {
        public Interview(long id, string question) {
            Id = id;
            Question = question;
            Answers = new List<InterviewAnswer>();
        }

        public long Id { get; private set; }
        public string Question { get; private set; }
        public List<InterviewAnswer> Answers { get; private set; }

        public void AddAnswer(long id, string text) {
            Answers.Add(new InterviewAnswer(id, text));
        }
    }
}