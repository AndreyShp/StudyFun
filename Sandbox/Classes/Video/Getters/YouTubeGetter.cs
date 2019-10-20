using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using BusinessLogic.ExternalData;
using Sandbox.Classes.Video.Getters.Data;

namespace Sandbox.Classes.Video.Getters {
    public class YouTubeGetter : IVideoDataGetter {
        private readonly Regex _regex = new Regex("\\s+",
                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase
                                                  | RegexOptions.Multiline);
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        private readonly string[] _titleExcludeWords = new[] {
            "[hd]", " hd ", "(hd)", "hdrip", "1080p"
        };
        private readonly string[] _titleStopWords = new[] {
            "korea", "japan", "thai", "tai", "hindi", "kung", "chinese", "cantonese", "hong kong", "iranian", "tibetan",
            "maryada",
            "philipine", "pinoy", "malayalam", "корейск", "китайск", "тайск", "иранск", "японск", "hot ", "xxx ",
            "adult "
        };

        private readonly Regex _youtubeRegex = new Regex("src=['\"].+/(?<vid>[^'\">]+)['\"]",
                                                         RegexOptions.IgnoreCase | RegexOptions.Singleline
                                                         | RegexOptions.Compiled);

        #region IVideoDataGetter Members

        public IVideoData GetVideoData(string url) {
            string vid = GetParamFromUrl(url, "v", '?');
            if (string.IsNullOrEmpty(vid)) {
                return null;
            }

            var result = new YoutubeVideoData {
                Vid = vid,
                HtmlCode = GetFrameHtmlById(vid),
                ThumnailImage = GetResizedImage(vid),
            };

            string metadata = LoadMetadata(vid);
            if (string.IsNullOrEmpty(metadata)) {
                return result;
            }

            NameValueCollection pars = HttpUtility.ParseQueryString(metadata);

            result.Title = pars["title"];
            result.ThumnailUrl = pars["thumbnail_url"];
            result.Keywords = pars["keywords"];
            result.Status = pars["status"];
            result.Author = pars["author"];
            result.Cid = pars["cid"];
            result.Oid = pars["oid"];
            result.Of = pars["of"];
            result.ViewCount = ParseToLong(pars, "view_count");
            result.LengthSeconds = ParseToLong(pars, "length_seconds");
            return result;
        }

        public IVideoData CreateFromString(string data) {
            return _serializer.Deserialize<YoutubeVideoData>(data);
        }

        public string ConvertToString(IVideoData data) {
            YoutubeVideoData parsedData = GetParsedData(data);
            return _serializer.Serialize(parsedData);
        }

        public bool IsInvalid(IVideoData data, LanguageShortName shortName) {
            YoutubeVideoData parsedData = GetParsedData(data);
            if (!"ok".Equals(parsedData.Status, StringComparison.InvariantCultureIgnoreCase)) {
                return true;
            }

            string title = data.Title.ToLowerInvariant();
            if (_titleStopWords.Any(title.Contains)) {
                return true;
            }

            if (title.Contains("18+")) {
                //а вдруг неприличное содержимое
                return true;
            }

            string trimmedTitle = _titleExcludeWords.Aggregate(title, (current, exlude) => current.Replace(exlude, ""));

            int latinCharsCount = 0;
            int russianCharsCount = 0;
            foreach (char ch in trimmedTitle) {
                if (ch >= 'a' && ch <= 'z') {
                    latinCharsCount++;
                } else if (ch >= 'а' && ch <= 'я') {
                    russianCharsCount++;
                }
            }

            if (russianCharsCount == 0 && latinCharsCount == 0) {
                //в названии нет ни русских ни латинских букв
                return true;
            }

            if (russianCharsCount > latinCharsCount) {
                /*string nameWithoutEnding = LanguagesHelper.GetLowerNameWithoutEnding(shortName);
                if (!title.Contains(nameWithoutEnding)) {*/
                //в названии русских букв больше - считаем что фильм не на том языке
                return true;
                //}
            }

            return false;
        }

        #endregion

        private static YoutubeVideoData GetParsedData(IVideoData data) {
            var result = data as YoutubeVideoData;
            if (result == null) {
                throw new ApplicationException("Что за фигня");
            }
            return result;
        }

        public StringBuilder GetSubtitles(string vid) {
            string url = string.Format("http://www.youtube.com/api/timedtext?lang=en&v={0}", vid);

            XElement root;
            try {
                XDocument xDocument = XDocument.Load(url, LoadOptions.None);
                root = xDocument.Root;
            } catch (Exception e) {
                return null;
            }

            if (root == null) {
                return null;
            }

            var rows = new StringBuilder();
            foreach (XElement xText in root.Elements("text")) {
                if (string.IsNullOrWhiteSpace(xText.Value)) {
                    continue;
                }

                string text = HttpUtility.HtmlDecode(xText.Value.Trim());
                text = _regex.Replace(text, " ");
                char lastChar = text[text.Length - 1];
                if (lastChar == '.' || lastChar == '?' || lastChar == '!') {
                    text += Environment.NewLine;
                } else {
                    text += ' ';
                }
                //заменить символ разделителя на запятую
                text = text.Replace(";", ",");
                rows.Append(text);
            }

            return rows;
        }

        public string GetFrameHtmlById(string vid) {
            return string.Format(
                "<iframe width=\"560\" height=\"315\" src=\"//www.youtube.com/embed/{0}\" frameborder=\"0\" allowfullscreen></iframe>",
                vid);
        }

        private static string LoadMetadata(string vid) {
            string metadata;

            try {
                var client = new WebClient();
                metadata = client.DownloadString("http://youtube.com/get_video_info?video_id=" + vid);
            } catch (Exception) {
                return null;
            }
            return metadata;
        }

        public byte[] GetThumbailImage(string htmlCode) {
            Match match = _youtubeRegex.Match(htmlCode);
            Group vidGroup = match.Groups["vid"];
            if (!vidGroup.Success || string.IsNullOrEmpty(vidGroup.Value)) {
                return null;
            }

            return GetResizedImage(vidGroup.Value);
        }

        private static byte[] GetResizedImage(string vid) {
            byte[] result = ReadThumbnailImage(vid);
            if (result == null) {
                return null;
            }

            try {
                result = ImageConverter.ResizeImage(result, 150);
            } catch (Exception e) {
                result = null;
            }
            return result;
        }

        private static byte[] ReadThumbnailImage(string vid) {
            try {
                var imageStream = new MemoryStream();
                var uri = new Uri(string.Format("http://img.youtube.com/vi/{0}/0.jpg", vid));
                WebRequest request = WebRequest.Create(uri);
                using (WebResponse resp = request.GetResponse()) {
                    var streamReader = new StreamReader(resp.GetResponseStream());
                    Stream stream = streamReader.BaseStream;

                    stream.CopyTo(imageStream);
                    streamReader.Close();
                    streamReader.Dispose();
                }

                return imageStream.Length > 0 ? imageStream.ToArray() : null;
            } catch (Exception e) {
                return null;
            }
        }

        private static long ParseToLong(NameValueCollection pars, string name) {
            long res;
            return long.TryParse(pars[name], out res) ? res : 0;
        }

        private static string GetParamFromUrl(string args, string key, char query) {
            int iqs = args.IndexOf(query);

            if (iqs != -1) {
                string queryString = (iqs < args.Length - 1) ? args.Substring(iqs + 1) : String.Empty;
                NameValueCollection nvcArgs = HttpUtility.ParseQueryString(queryString);
                return nvcArgs[key];
            }
            return string.Empty; // or throw an error
        }
    }
}