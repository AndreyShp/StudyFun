using System;
using System.Collections.Generic;
using BusinessLogic.Data;
using BusinessLogic.Data.Knowledge;

namespace BusinessLogic.DataQuery.Knowledge {
    public interface IUserRepetitionKnowledgeQuery {
        List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionQuery(StudyLanguageContext c,
                                                                              DateTime minNextTimeShow,
                                                                              DateTime maxNextTimeShow,
                                                                              int count);

        List<Tuple<UserKnowledge, UserRepetitionInterval>> GetRepetitionNewQuery(StudyLanguageContext c,
                                                                                 int count);
    }
}