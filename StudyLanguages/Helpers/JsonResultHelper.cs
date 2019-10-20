using System.Web.Mvc;

namespace StudyLanguages.Helpers {
    public class JsonResultHelper {
        public static JsonResult Error() {
            return Success(false);
        }

        public static JsonResult Error(string message) {
            return GetUnlimitedJsonResult(new {success = false, message});
        }

        public static JsonResult Success(bool success) {
            return GetUnlimitedJsonResult(new {success});
        }

        public static JsonResult GetUnlimitedJsonResult(dynamic data) {
            var result = new JsonResult {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
            return result;
        }
    }
}