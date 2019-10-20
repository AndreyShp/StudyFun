using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Video;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Video {
    public class VideosQuery : BaseQuery, IVideosQuery, IRatingQuery {
        private readonly long _languageId;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="languageId">идентификатор языка, на котором нужно получать видео</param>
        public VideosQuery(long languageId) {
            _languageId = languageId;
        }

        #region IRatingQuery Members

        public bool IncRating(long entityId) {
            bool result = Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                const string SQL_COMMAND = "update Video set Rating=coalesce(Rating, 0)+1 where Id={0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             entityId
                                                         });
            });
            return result;
        }

        #endregion

        #region IVideosQuery Members

        public List<VideoForUser> GetVisible(VideoType type, int count = 0) {
            List<VideoForUser> result = Adapter.ReadByContext(c => {
                IQueryable<Data.Video.Video> videosQuery = (from v in c.Video
                                                            where v.IsVisible && v.LanguageId == _languageId
                                                            orderby v.Rating descending , v.Title
                                                            select v);
                if (count > 0) {
                    videosQuery = videosQuery.Take(count);
                }
                List<VideoForUser> innerResult =
                    videosQuery.AsEnumerable().Select(e => new VideoForUser(e)).ToList();
                return innerResult;
            });
            return result ?? new List<VideoForUser>(0);
        }

        public List<VideoForUser> GetVisibleWithText(VideoType type, int count = 0) {
            var result = GetVisible(type, count);
            if (EnumerableValidator.IsNullOrEmpty(result)) {
                return result;
            }
            Adapter.ActionByContext(c => {
                IQueryable<long> videoIdsWithText = c.VideoSentence.Select(e => e.VideoId).Distinct();
                var fast = new HashSet<long>(videoIdsWithText);
                result = result.Where(e => fast.Contains(e.Id)).ToList();
            });
            return result;
        }

        public VideoForUser Get(string title) {
            VideoForUser videoForUser = Adapter.ReadByContext(c => {
                Data.Video.Video video = GetByTitle(title);
                if (video == null) {
                    return null;
                }

                long videoId = video.Id;
                IOrderedQueryable<VideoSentence> videosQuery = (from vs in c.VideoSentence
                                                                where vs.VideoId == videoId
                                                                orderby vs.Order
                                                                select vs);

                var innerResult = new VideoForUser(video);
                foreach (VideoSentence videoSentence in videosQuery.ToList()) {
                    innerResult.Sentences.Add(new Tuple<string, string>(videoSentence.Source, videoSentence.Translation));
                }
                return innerResult;
            });

            return videoForUser;
        }

        public byte[] GetImage(string title) {
            Data.Video.Video video = GetByTitle(title);
            return video != null ? video.Image : null;
        }

        #endregion

        public VideoForUser GetOrCreate(VideoType type, VideoForUser videoForUser, byte[] image, int? rating) {
            if (EnumValidator.IsInvalid(type) || videoForUser == null || string.IsNullOrWhiteSpace(videoForUser.Title)
                || string.IsNullOrWhiteSpace(videoForUser.HtmlCode)
                /*|| EnumerableValidator.IsNullOrEmpty(videoForUser.Sentences)*/) {
                return null;
            }

            byte parsedVideoType = (byte) type;
            VideoForUser result = null;
            Adapter.ActionByContext(c => {
                Data.Video.Video video = GetOrCreateVideo(videoForUser, image, rating, parsedVideoType, c);
                long videoId = video.Id;
                if (IdValidator.IsInvalid(videoId)) {
                    return;
                }

                DeleteVideoSentences(c, videoId);

                result = new VideoForUser(video);
                int order = 1;
                foreach (var sentence in videoForUser.Sentences) {
                    var videoSentence = new VideoSentence {
                        VideoId = videoId,
                        Source = sentence.Item1,
                        Translation = sentence.Item2,
                        Order = order++
                    };
                    c.VideoSentence.Add(videoSentence);
                }
                c.SaveChanges();
            });

            return result;
        }

        private void DeleteVideoSentences(StudyLanguageContext c, long videoId) {
            const string SQL_COMMAND = "delete from VideoSentence where VideoId={0}";
            int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                     new object[] {
                                                         videoId
                                                     });
        }

        private Data.Video.Video GetOrCreateVideo(VideoForUser videoForUser,
                                                  byte[] image,
                                                  int? rating,
                                                  byte videoType,
                                                  StudyLanguageContext c) {
            string title = videoForUser.Title;
            string htmlCode = videoForUser.HtmlCode;

            Data.Video.Video video =
                c.Video.FirstOrDefault(
                    e =>
                    (e.Title == title || e.HtmlCode == htmlCode) && e.LanguageId == _languageId && e.Type == videoType);
            if (video == null) {
                video = new Data.Video.Video {
                    IsVisible = true,
                    Rating = rating,
                    LanguageId = _languageId,
                    Type = videoType
                };
                c.Video.Add(video);
            }

            video.Title = title;
            video.HtmlCode = htmlCode;
            video.Image = image;
            video.LastModified = DateTime.Now;
            c.SaveChanges();
            return video;
        }

        private Data.Video.Video GetByTitle(string title) {
            Data.Video.Video result = Adapter.ReadByContext(c => {
                IQueryable<Data.Video.Video> videosQuery = (from v in c.Video
                                                            where
                                                                v.Title == title && v.IsVisible
                                                                && v.LanguageId == _languageId
                                                            select v);
                return videosQuery.AsEnumerable().FirstOrDefault();
            });
            return result;
        }
    }
}