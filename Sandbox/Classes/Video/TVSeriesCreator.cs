using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Video;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;
using BusinessLogic.Video;
using BusinessLogic.Video.Subtitles;

namespace Sandbox.Classes.Video {
    public class TVSeriesCreator {
        private readonly long _languageId;
        private readonly string _origSeriesName;
        private readonly string _outputPath;
        private readonly TVSeriesQuery _seriesQuery;
        private readonly string _transSeriesName;

        public TVSeriesCreator(LanguageShortName language,
                               string outputPath,
                               string origSeriesName,
                               string transSeriesName) {
            _outputPath = outputPath;
            _origSeriesName = origSeriesName;
            _transSeriesName = transSeriesName;
            var languagesQuery = new LanguagesQuery(language, LanguageShortName.Ru);
            _languageId = languagesQuery.GetByShortName(language).Id;
            _seriesQuery = new TVSeriesQuery(_languageId);
        }

        public void Create(int season,
                           int episode,
                           string videoFile,
                           string sourceSubtitlesFile,
                           string origTitle,
                           string transTitle, TimeSpan thumbnailTime) {

            string urlPart = _transSeriesName;
            var tvSeriesInfo = new TVSeriesInfo {
                OrigTitle = _origSeriesName,
                Title = _transSeriesName,
                Description = null,
                PathToFiles = _origSeriesName,
                ImageFileName = null,
            };
            
            tvSeriesInfo.SetUrlPart(urlPart);
            var coverId = _seriesQuery.SaveSeriesInfo(tvSeriesInfo);

            string newFileName = episode.ToString(CultureInfo.InvariantCulture);
            string timeShiftFile = episode.ToString(CultureInfo.InvariantCulture);
            if (season > 0) {
                timeShiftFile = season + "_" + timeShiftFile;
                timeShiftFile = timeShiftFile + "_time.txt";
            }

            timeShiftFile = Path.Combine(Path.GetDirectoryName(sourceSubtitlesFile), timeShiftFile);

            var sourceAnalyzer = new SubtitlesAnalyzer(sourceSubtitlesFile, Encoding.UTF8);
            List<Subtitle> sourceSubtitles = sourceAnalyzer.Analyze(timeShiftFile);

            var tvSeriesWatch = new TVSeriesWatch {
                Season = season,
                Episode = episode,
                OrigTitle = origTitle,
                TransTitle = transTitle,
                Description = null,
                Subtitles = sourceSubtitles,
                VideoFileName = newFileName + Path.GetExtension(videoFile),
            };
            tvSeriesWatch.SetSeriesInfo(tvSeriesInfo);
            urlPart = TVSeriesQuery.GetUrlPart(urlPart, season, episode);
            tvSeriesWatch.SetUrlPart(urlPart);

            string fullPath = Path.Combine(_outputPath, tvSeriesWatch.GetPathToFiles());
            if (!Directory.Exists(fullPath)) {
                Directory.CreateDirectory(fullPath);
            }

            string destVideoFile = Path.Combine(fullPath, tvSeriesWatch.VideoFileName);
            File.Copy(videoFile, destVideoFile, true);
                
            tvSeriesWatch.ImageFileName = newFileName + ".jpeg";
            CreateImage(videoFile, thumbnailTime, fullPath, tvSeriesWatch);

            long tvSeriesId = _seriesQuery.SaveSeriesWatch(coverId, tvSeriesWatch);
            if (IdValidator.IsInvalid(tvSeriesId)) {
                Console.WriteLine("Не удалось добавить видео {0}", videoFile);
            }

            /*int partNumber = 1;
            var videoCutter = new VideoCutter(videoFileName, @"C:\Projects\Сериалы\Друзья\1_1_parts");
            foreach (Subtitle episode in enEpisodes)
            {
                //TODO: соединить с другими частями и получить более длинный отрывок
                if (episode.Duration.TotalSeconds < 2)
                {
                    continue;
                }
                string outFileName = partNumber.ToString(CultureInfo.InvariantCulture) + ".mp4";
                if (!videoCutter.Cut(outFileName, episode.TimeFrom, episode.TimeTo))
                {
                    Console.WriteLine("Не удалось разрезать файл {0}, часть №{1}", videoFileName, partNumber);
                    Console.ReadLine();
                }
                Console.WriteLine("Обработана часть {0} у видео {1}", partNumber, videoFileName);
                partNumber++;
            }*/

            /*string ruFileName =
                @"C:\Projects\Сериалы\Друзья\Субтитры\ru\Friends - 1x01 - The One Where Monica Gets A Roommate.ru.srt";
            var ruAnalyzer = new SubtitlesAnalyzer(ruFileName, Encoding.GetEncoding(1251));
            var ruEpisodes = ruAnalyzer.Analyze();

            var matcher = new VideoEpisodesMatcher(enEpisodes, ruEpisodes);
            var result = matcher.Match();

            foreach (var tuple in result) {
                Console.WriteLine(tuple.Item1.ToPrettyFormat() + "\r\n" + tuple.Item2.ToPrettyFormat());
                Console.ReadLine();
            }*/
        }

        private static void CreateImage(string videoFile, TimeSpan thumbnailTime, string fullPath, TVSeriesWatch tvSeriesWatch) {
            var imageFileName = Path.Combine(fullPath, tvSeriesWatch.ImageFileName);
            if (File.Exists(imageFileName)) {
                File.Delete(imageFileName);
            }

            var videoTransformation = new VideoTransformation(videoFile, fullPath);
            if (!videoTransformation.GetImage(tvSeriesWatch.ImageFileName, thumbnailTime)) {
                Console.WriteLine("НЕ удалось создать картинку {0}!", tvSeriesWatch.ImageFileName);
            }
        }
    }
}