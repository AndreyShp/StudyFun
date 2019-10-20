using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.DataQuery.Knowledge;

namespace StudyLanguages.Controllers {
    public class TrainerAllKnowledgeController : BaseKnowledgeTrainerController {
        protected override KnowledgeDataType KnowledgeDataType {
            get { return KnowledgeDataType.All; }
        }

        protected override string ControllerName {
            get { return RouteConfig.USER_TRAINER_ALL_CONTROLLER; }
        }

        protected override string PageName {
            get { return "Карточки моих знаний"; }
        }
    }
}