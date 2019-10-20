using System.IO;
using System.Text.RegularExpressions;

namespace BusinessLogic.Helpers {
    public static class FileHelper {
        public static string EncodeFileName(string fullFileName) {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(fullFileName, "");
        }
    }
}