using System.Collections.Generic;
using ExternalInterview = BusinessLogic.ExternalData.Auxiliaries.Interview;

namespace BusinessLogic.DataQuery.Auxiliaries {
    public interface IInterviewsQuery {
        List<ExternalInterview> GetQuestions();
        bool IncrementCountAnswers(List<long> answersIds);
    }
}