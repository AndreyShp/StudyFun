using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using BusinessLogic.Logger;
using SquishIt.Framework;
using SquishIt.Framework.Base;

namespace StudyLanguages.Helpers {
    public class Minimizer {
        private const string MIN_PART_NAME = ".min.";
        private const string PACKED_SUFFIX = ".packed.";

        public static MvcHtmlString MinimizeCssFile(string virtualPath) {
            return MinimizeCssFiles(virtualPath, virtualPath);
        }

        public static MvcHtmlString MinimizeJsFile(string virtualPath) {
            return MinimizeJsFiles(virtualPath, virtualPath);
        }

        public static MvcHtmlString MinimizeCssFiles(string bundleFileName, params string[] virtualPaths) {
            string newFileName = GetRenderFileName(Extensions.CSS, bundleFileName);
            return WriteFilesToBundle(Bundle.Css(), newFileName, virtualPaths);
        }

        public static MvcHtmlString MinimizeJsFiles(string bundleFileName, params string[] virtualPaths) {
            string newFileName = GetRenderFileName(Extensions.JS, bundleFileName);
            return WriteFilesToBundle(Bundle.JavaScript(), newFileName, virtualPaths);
        }

        private static string GetRenderFileName(string extension, string bundleFileName) {
            string result = bundleFileName.Replace(MIN_PART_NAME, ".");
            result = result.Replace(extension, string.Empty);
            result += PACKED_SUFFIX + "#" + extension;
            return result;
        }

        private static MvcHtmlString WriteFilesToBundle<T>(T bundle, string newFileName, params string[] virtualPaths)
            where T : BundleBase<T> {
            foreach (string virtualPath in virtualPaths) {
                if (NeedMinimize(virtualPath)) {
                    //файл не сжат
                    bundle = bundle.Add(virtualPath);
                } else {
                    //файл уже сжат
                    bundle = bundle.AddMinified(virtualPath);
                }
            }
#if DEBUG 
            bundle = bundle.ForceDebug();
#else
            bundle = bundle.ForceRelease();
#endif
        string scriptBlock = bundle.Render(newFileName);
            return new MvcHtmlString(scriptBlock);
        }

        private static bool NeedMinimize(string virtualPath) {
            return virtualPath.IndexOf(MIN_PART_NAME, StringComparison.InvariantCultureIgnoreCase) == -1;
        }

        public static void DeleteOldFiles(string basePath) {
            var mask = new Regex("^(?<fileName>.*?)" + Regex.Escape(PACKED_SUFFIX) + "([^\\.]*)(?<extension>\\.\\w+)$");

            var di = new DirectoryInfo(basePath);
            List<FileInfo> fileInfos =
                di.GetFiles("*" + PACKED_SUFFIX + "*", SearchOption.AllDirectories).OrderByDescending(
                    e => e.CreationTime > e.LastWriteTime
                             ? e.CreationTime
                             : e.LastWriteTime).ToList();

            var files = new HashSet<string>();
            foreach (FileInfo fi in fileInfos) {
                string fileName = fi.FullName;
                string maskFile = mask.Replace(fileName, "${fileName}${extension}");

                if (!files.Add(maskFile)) {
                    //для файла с такой же маской уже найден более поздний файл - удалить текущий
                    try {
                        fi.Delete();
                    } catch (Exception e) {
                        LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                            "Minimizer.DeleteOldFiles Не смогли удалить файл {0}. Исключение {1}",
                            fileName, e);
                    }
                }
            }
        }

        #region Nested type: Extensions

        private static class Extensions {
            public const string JS = ".js";
            public const string CSS = ".css";
        }

        #endregion
    }
}