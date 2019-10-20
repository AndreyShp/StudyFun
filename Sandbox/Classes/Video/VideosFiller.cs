using System;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Videos;
using Sandbox.Classes.Video.Getters;

namespace Sandbox.Classes.Video {
    public class VideosFiller {
        public void Fill(VideoForUser videoForUser) {
            long languageId =
                new LanguagesQuery(LanguageShortName.Unknown, LanguageShortName.Unknown).GetByShortName(
                    LanguageShortName.En).Id;

            string title = videoForUser.Title;
            string htmlCode = videoForUser.HtmlCode;

            byte[] image = GetImage(title, htmlCode);

            var videosQuery = new VideosQuery(languageId);
            VideoForUser result = videosQuery.GetOrCreate(VideoType.Clip, videoForUser, image, null);
            if (result != null) {
                Console.WriteLine("Видео \"{0}\" успешно добавлено", title);
            } else {
                Console.WriteLine("Не удалось добавить видео \"{0}\"!!!", title);
            }
        }

        private static byte[] GetImage(string title, string htmlCode) {
            var videoGetter = new YouTubeGetter();
            byte[] image = videoGetter.GetThumbailImage(htmlCode);

            if (image == null) {
                Console.WriteLine("Не смогли получить изображение или изменить размер изображения для видео \"{0}\"!",
                                  title);
            }
            return image;
        }
    }
}