using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Helpers;
using BusinessLogic.Helpers.Caches;
using Sandbox.Classes.Video.Getters;
using Sandbox.Classes.Video.Getters.Data;

namespace Sandbox.Classes.Video {
    public class VideoProcessor {
        private readonly DiskCache _diskCache;
        private readonly VideoLinksHelper _linksHelper;
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly LanguageShortName _shortName;

        private VideoProcessor(LanguageShortName shortName) {
            _shortName = shortName;

            string cachePath = Path.Combine(YandexVideoSearcher.BASE_PATH, "VideoDataCache", _shortName.ToString());

            _diskCache = new DiskCache(cachePath, false);
            _linksHelper =
                new VideoLinksHelper(YandexVideoSearcher.GetAllLinksFullFileName(YandexVideoSearcher.BASE_PATH,
                                                                                 shortName));
        }

        public void Process() {
            long languageId =
                new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown).GetByShortName(
                    _shortName).Id;

            const int MAX_DOMAIN_TO_PROCESS = 10;

            List<Tuple<string, List<string>>> linksByDomains = _linksHelper.Analyze();

            foreach (var tuple in linksByDomains.Take(MAX_DOMAIN_TO_PROCESS)) {
                IVideoDataGetter videoDataGetter = GetVideoDataGetterByDomain(tuple.Item1);
                if (videoDataGetter == null) {
                    continue;
                }

                int failCount = 0;
                int successCount = 0;
                List<string> links = tuple.Item2;
                foreach (string link in links) {
                    string encodedLink = FileHelper.EncodeFileName(link);

                    IVideoData videoData = GetVideoDataFromCache(videoDataGetter, encodedLink);
                    if (videoData == null) {
                        //для этой ссылки данных нет в кэше - получить их от Getter'а
                        videoData = videoDataGetter.GetVideoData(link);
                    }

                    if (videoData == null) {
                        Console.WriteLine("Для ссылки {0} НЕ удалось получить данные!", link);
                        failCount++;
                        continue;
                    }

                    SaveVideoDataToCache(videoDataGetter, encodedLink, videoData);

                    if (videoDataGetter.IsInvalid(videoData, _shortName)) {
                        Console.WriteLine("Для ссылки {0} данные некорректны!", link);
                        failCount++;
                        continue;
                    }

                    var videosQuery = new VideosQuery(languageId);
                    var videoForUser = new VideoForUser(videoData.Title, videoData.HtmlCode);
                    //TODO: поиск дубликатов видео

                    //TODO: сохранять дополнительную информацию в БД
                    VideoForUser result = videosQuery.GetOrCreate(VideoType.Movie, videoForUser, videoData.ThumnailImage, videoData.Rating);
                    if (result != null) {
                        successCount++;
                    } else {
                        Console.WriteLine("Не удалось добавить видео \"{0}\"!!!", videoData.Title);
                    }
                }
                Console.WriteLine("Обработан домен {0} из {1} ссылок успешно сохранены {2}, не удалось сохранить {3}",
                                  tuple.Item1, links.Count, successCount, failCount);
            }
        }

        private IVideoData GetVideoDataFromCache(IVideoDataGetter videoDataGetter, string encodedLink) {
            string serializedValue = _diskCache.Get(GetMainKey(encodedLink), Encoding.UTF8);
            if (string.IsNullOrEmpty(serializedValue)) {
                return null;
            }

            string[] rows = serializedValue.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            var mainData = _serializer.Deserialize<Dictionary<string, string>>(rows[0]);

            IVideoData result = videoDataGetter.CreateFromString(rows[1]);
            if (result != null) {
                result.Title = mainData["title"];
                result.Vid = mainData["vid"];
                result.HtmlCode = mainData["htmlCode"];
                string imageKey = mainData["imagePath"];
                result.ThumnailImage = _diskCache.Get(imageKey);
            }
            return result;
        }

        private static string GetMainKey(string encodedLink) {
            return encodedLink + ".txt";
        }

        private void SaveVideoDataToCache(IVideoDataGetter videoDataGetter, string encodedLink, IVideoData videoData) {
            string imageKey = encodedLink + "_image.jpeg";
            if (videoData.ThumnailImage != null) {
                bool isSavedImage = _diskCache.Save(imageKey, videoData.ThumnailImage);
                if (!isSavedImage) {
                    Console.WriteLine("Не удалось добавить изображение для видео \"{0}\" в кэш!!!", videoData.Title);
                }
            }

            string additionalData = videoDataGetter.ConvertToString(videoData);
            var mainData = new Dictionary<string, string> {
                {"title", videoData.Title},
                {"vid", videoData.Vid},
                {"htmlCode", videoData.HtmlCode},
                {"rating", videoData.Rating != null ? videoData.Rating.ToString() : string.Empty},
                {"imagePath", imageKey}
            };

            var data = new StringBuilder(_serializer.Serialize(mainData));
            data.AppendLine();
            data.Append(additionalData);

            bool isSaved = _diskCache.Save(GetMainKey(encodedLink), data.ToString(), Encoding.UTF8);
            if (!isSaved) {
                Console.WriteLine("Не удалось добавить видео \"{0}\" в кэш!!!", videoData.Title);
            }
        }

        private static IVideoDataGetter GetVideoDataGetterByDomain(string domain) {
            switch (domain) {
                case "youtube.com":
                case "www.youtube.com":
                    return new YouTubeGetter();
            }

            //TODO: реализовать остальные данные получатели данных с других сайтов
            return null;
        }

        public static void Run() {
            var videoProcessor = new VideoProcessor(LanguageShortName.En);
            videoProcessor.Process();
        }
    }
}