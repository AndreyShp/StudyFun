using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;

namespace StudyLanguages.Controllers {
    public class TrainerSentencesKnowledgeController : BaseKnowledgeTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.SentenceTranslation; }
        }

        protected override string ControllerName {
            get { return RouteConfig.USER_TRAINER_SENTENCES_CONTROLLER; }
        }

        protected override string PageName {
            get { return "Карточки моих предложений"; }
        }
    }
}