using System.Web.Mvc;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;
using StudyLanguages.Filters;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Controllers {
    public class HomeController : Controller {
        private readonly SentencesQuery _query;
        private readonly ShuffleController _shuffleController;

        public HomeController() {
            _query = new SentencesQuery();
            _shuffleController = new ShuffleController(_query, "Index", SectionId.Sentences);
        }

        [UserId(true)]
        [UserLanguages]
        public ActionResult Index(long userId, UserLanguages userLanguages) {
            return _shuffleController.Index(userId, userLanguages);
        }

        [HttpPost]
        [UserId]
        [UserLanguages]
        public JsonResult MarkAsShowed(long userId, UserLanguages userLanguages, long id) {
            return _shuffleController.MarkAsShowed(userId, id, userLanguages);
        }

        [HttpGet]
        [UserId]
        [UserLanguages]
        public JsonResult GetPrevPortion(long userId, UserLanguages userLanguages, long id) {
            return _shuffleController.GetPortion(userId, id,
                                                 userUnique =>
                                                 _query.GetPrevPortion(userUnique, id, userLanguages));
        }

        [HttpGet]
        [UserId]
        [UserLanguages]
        public JsonResult GetNextPortion(long userId, UserLanguages userLanguages, long id) {
            return _shuffleController.GetPortion(userId, id,
                                                 userUnique =>
                                                 _query.GetNextPortion(userUnique, id, userLanguages));
        }

        [HttpGet]
        [UserId]
        public ActionResult Translation(long userId, long? sourceId, long? translationId) {
            return _shuffleController.Translation(userId, sourceId, translationId);
        }

        [HttpGet]
        [UserId]
        public ActionResult Reset(long userId) {
            _shuffleController.DeleteUserHistory(userId);
            return RedirectToAction("Index");
        }
    }
}