using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Helpers {
    public static class OurHtmlHelper {
        private const string NEW_LINE = "<br />";

        //NOTE: меняя порядок здесь - менять и в _Layout.js Speaker.getHtml
        public const string SPEAKER_PATTERN_HTML =
            "<span class=\"glyphicon glyphicon-volume-down speaker\" title=\"Прослушать\" onclick=\"Speaker.Speak({0},{1});\"></span>";

        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model) {
            controllerContext.Controller.ViewData.Model = model;
            using (var sw = new StringWriter()) {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View,
                                                  controllerContext.Controller.ViewData,
                                                  controllerContext.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string GetSpeakerHtml(PronunciationForUser pronunciationForUser, SpeakerDataType type) {
            string sourceText = String.Format("{0}{1}",
                                              pronunciationForUser.HasPronunciation
                                                  ? String.Format(SPEAKER_PATTERN_HTML, pronunciationForUser.Id,
                                                                  (int) type)
                                                  : String.Empty, pronunciationForUser.Text);
            return sourceText;
        }

        public static string HtmlEncode(string text, bool decodeBeforeEncode = false) {
            string result;
            text = (text ?? string.Empty).Trim();
            if (decodeBeforeEncode) {
                result = WebUtility.HtmlDecode(text);
            } else {
                result = text;
            }
            result = WebUtility.HtmlEncode(result);
            return result;
        }

        public static string PrepareStringFromUser(string str) {
            var result = str.Trim();
            result = HtmlEncodeManyStrings(result, true);
            result = HtmlCorrectBadWords(result);
            return result;
        }

        public static string HtmlEncodeManyStrings(string text, bool decodeBeforeEncode = false) {
            string result = HtmlEncode(text, decodeBeforeEncode);
            result = result.Replace("\r\n", "\n").Replace("\n", NEW_LINE);
            return result;
        }

        public static string HtmlInOneLineAndGetSubstr(string text, int countChars = 100) {
            var result = text.Replace(NEW_LINE, " ");
            if (result.Length > countChars) {
                result = result.Substring(0, countChars) + "...";
            }
            return result;
        }

        public static string HtmlCorrectBadWords(string text) {
            string result = text;
            //TODO: фильтрация мата
            return result;
        }

        #region Nested type: ViewDataFlags

        public static class ViewDataFlags {
            public const string SKIP_POPUP_PANEL = "SkipPopupPanel";
        }

        #endregion
    }
}