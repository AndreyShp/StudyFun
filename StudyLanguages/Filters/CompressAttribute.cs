using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace StudyLanguages.Filters {
    public class CompressAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            HttpRequestBase request = filterContext.HttpContext.Request;
            HttpResponseBase response = filterContext.HttpContext.Response;

            CompressStream(request, response);
        }

        public static void CompressStream(HttpRequestBase request, HttpResponseBase response) {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) {
                return;
            }
            string encoding = response.Headers["Content-encoding"];
            if (!string.IsNullOrEmpty(encoding)) {
                return;
            }

            acceptEncoding = acceptEncoding.ToLowerInvariant();

            if (acceptEncoding.Contains("gzip")) {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            } else if (acceptEncoding.Contains("deflate")) {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}