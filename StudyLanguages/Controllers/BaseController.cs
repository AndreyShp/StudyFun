using System;
using System.Web.Mvc;
using BusinessLogic.DataQuery;
using BusinessLogic.Validators;
using StudyLanguages.Helpers;

namespace StudyLanguages.Controllers {
    public class BaseController : Controller {
        protected ActionResult GetImage(IImageQuery imageQuery, long id, Func<byte[], byte[]> imageModifier = null) {
            if (IdValidator.IsInvalid(id)) {
                return new EmptyResult();
            }
            byte[] image = imageQuery.GetImageById(id);
            return ImageToFile(imageModifier, image);
        }

        protected ActionResult GetImage(string name,
                                        Func<string, byte[]> imageGetter,
                                        Func<byte[], byte[]> imageModifier = null) {
            if (string.IsNullOrWhiteSpace(name) || imageGetter == null) {
                return new EmptyResult();
            }
            byte[] image = imageGetter(name);
            return ImageToFile(imageModifier, image);
        }

        private ActionResult ImageToFile(Func<byte[], byte[]> imageModifier, byte[] image) {
            if (image != null && imageModifier != null) {
                image = imageModifier(image);
            }
            return image != null ? File(image, CommonConstants.IMAGE_CONTENT_TYPE) : (ActionResult) (new EmptyResult());
        }
    }
}