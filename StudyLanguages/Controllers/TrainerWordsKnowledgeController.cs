using BusinessLogic.Data.Enums.Knowledge;

namespace StudyLanguages.Controllers {
    public class TrainerWordsKnowledgeController : BaseKnowledgeTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.WordTranslation; }
        }

        protected override string ControllerName {
            get { return RouteConfig.USER_TRAINER_WORDS_CONTROLLER; }
        }

        protected override string PageName {
            get { return "Карточки моих слов"; }
        }
    }
}