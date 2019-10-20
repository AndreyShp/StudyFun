using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data.Auxiliary;
using BusinessLogic.Logger;
using BusinessLogic.Validators;
using ExternalInterview = BusinessLogic.ExternalData.Auxiliaries.Interview;

namespace BusinessLogic.DataQuery.Auxiliaries {
    public class InterviewsQuery : BaseQuery, IInterviewsQuery {
        public List<ExternalInterview> GetQuestions() {
            List<Interview> interviews =
                Adapter.ReadByContext(c => c.Interview.OrderBy(e => e.ParentInterviewId).ToList());
            var result = new Dictionary<long, ExternalInterview>();
            foreach (Interview interview in interviews) {
                if (interview.ParentInterviewId == null) {
                    result.Add(interview.Id, new ExternalInterview(interview.Id, interview.Text));
                    continue;
                }

                long parentId = interview.ParentInterviewId.Value;
                ExternalInterview question;
                if (!result.TryGetValue(parentId, out question)) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "InterviewsQuery.GetQuestions не найден родитель с идентификатором {0}!", parentId);
                    continue;
                }

                question.AddAnswer(interview.Id, interview.Text);
            }

            return result.Values.Where(e => EnumerableValidator.IsNotNullAndNotEmpty(e.Answers)).OrderBy(e => e.Id).ToList();
        }

        public bool IncrementCountAnswers(List<long> answersIds) {
            string ids = string.Join(",", answersIds);
            bool result = Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                string sqlCommand =
                    "update Interview set CountAnswers=CountAnswers+1 where Id in (" + ids + ") and ParentInterviewId is not null";
                int count = c.Database.ExecuteSqlCommand(sqlCommand);
            });
            return result;
        }
    }
}