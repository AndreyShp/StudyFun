using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;

namespace StudyLanguages.Controllers {
    public class TrainerPhrasesKnowledgeController : BaseKnowledgeTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.PhraseTranslation; }
        }
        protected override string ControllerName {
            get { return RouteConfig.USER_TRAINER_PHRASES_CONTROLLER; }
        }
        protected override string PageName {
            get { return "Карточки моих фраз"; }
        }
    }
}