using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Video;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Validators;
using Newtonsoft.Json;

namespace BusinessLogic.DataQuery.Video {
    /// <summary>
    /// Отвечает за сохранение информации по сериалам
    /// </summary>
    public class TVSeriesQuery : BaseQuery {
        public const string PATH_DELIMITER = "\\";
        private const string URL_PARTS_DELIMITER = "|";
        private const string URL_DELIMITER = "_";

        private readonly long _languageId;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="languageId">идентификатор языка, на котором нужно получать видео</param>
        public TVSeriesQuery(long languageId) {
            _languageId = languageId;
        }

        public static string GetUrlPart(string baseUrlPath, int season = 0, int episode = 0) {
            string result = baseUrlPath;
            if (season == 0 && episode == 0) {
                return result;
            }

            string part = null;
            if (season > 0) {
                part = season.ToString(CultureInfo.InvariantCulture);
            }

            if (episode > 0) {
                if (!string.IsNullOrEmpty(part)) {
                    part += URL_DELIMITER;
                }
                part += episode.ToString(CultureInfo.InvariantCulture);
            }

            result += URL_PARTS_DELIMITER + part;
            return result;
        }

        public static Tuple<string, string, string> GetUrlParts(string urlPart) {
            string[] mainParts = urlPart.Split(new[] {URL_PARTS_DELIMITER}, StringSplitOptions.RemoveEmptyEntries);

            string[] parts = mainParts[1].Split(new[] {URL_DELIMITER}, StringSplitOptions.RemoveEmptyEntries);
            string episode = parts[0];
            string season = null;
            if (parts.Length > 1) {
                season = parts[0];
                episode = parts[1];
            }
            return new Tuple<string, string, string>(mainParts[0], season, episode);
        }

        /// <summary>
        /// Сохраняет серию сериала
        /// </summary>
        /// <param name="parentId">идентификатор родителя(сезона или названия сериала)</param>
        /// <param name="seriesInfo">информация о серии</param>
        /// <returns>идентификатор добавленного сериала</returns>
        public long SaveSeriesWatch(long parentId, TVSeriesWatch seriesInfo) {
            string urlPart = seriesInfo != null ? seriesInfo.GetUrlPart() : null;
            if (IdValidator.IsInvalid(parentId) || IsUrlPartInvalid(urlPart)) {
                return IdValidator.INVALID_ID;
            }

            string info = JsonConvert.SerializeObject(seriesInfo);
            long result = Save(parentId, urlPart, info, TVSeriesDataType.Series);
            return result;
        }

        /// <summary>
        /// Сохраняет обложку сериала(информацию о сериале)
        /// </summary>
        /// <param name="seriesInfo">информация о сериале</param>
        /// <returns>идентификатор добавленной обложки</returns>
        public long SaveSeriesInfo(TVSeriesInfo seriesInfo) {
            string urlPart = seriesInfo != null ? seriesInfo.GetUrlPart() : null;
            if (IsUrlPartInvalid(urlPart)) {
                return IdValidator.INVALID_ID;
            }

            string info = JsonConvert.SerializeObject(seriesInfo);
            long result = Save(IdValidator.INVALID_ID, urlPart, info, TVSeriesDataType.Cover);
            return result;
        }

        private long Save(long parentId, string urlPart, string info, TVSeriesDataType tvSeriesDataType) {
            long result = IdValidator.INVALID_ID;
            Adapter.ActionByContext(c => {
                TVSeries tvSeries = GetByUrlPart(c, urlPart);
                if (tvSeries == null) {
                    tvSeries = new TVSeries();
                    c.TVSeries.Add(tvSeries);
                }

                tvSeries.ParentId = parentId;
                tvSeries.Info = info;
                tvSeries.UrlPart = urlPart;
                tvSeries.IsVisible = true;
                tvSeries.LanguageId = _languageId;
                tvSeries.DataType = (byte) tvSeriesDataType;

                c.SaveChanges();

                result = tvSeries.Id;
            });
            return result;
        }

        public TVSeriesInfo GetSeriesInfo(string urlPart) {
            if (IsUrlPartInvalid(urlPart)) {
                return null;
            }

            return Adapter.ReadByContext(c => {
                TVSeries tvSeries = GetByUrlPart(c, urlPart);
                if (tvSeries == null) {
                    return null;
                }

                var result = JsonConvert.DeserializeObject<TVSeriesInfo>(tvSeries.Info);
                //result.SetUrlPart(urlPart);
                return result;
            });
        }

        public TVSeriesWatch GetSeriesWatch(string urlPart) {
            if (IsUrlPartInvalid(urlPart)) {
                return null;
            }

            return Adapter.ReadByContext(c => {
                TVSeries tvSeries = GetByUrlPart(c, urlPart);
                if (tvSeries == null) {
                    return null;
                }

                TVSeries parentTVSeries = GetParent(c, tvSeries.ParentId);
                if (parentTVSeries == null) {
                    return null;
                }

                TVSeriesInfo coverInfo = ToCoverInfo(parentTVSeries);
                if (coverInfo == null) {
                    return null;
                }

                TVSeriesWatch result = ToTVSeriesWatch(tvSeries);
                result.SetSeriesInfo(coverInfo);
                return result;
            });
        }

        private static TVSeriesInfo ToCoverInfo(TVSeries tvSeries) {
            var result = JsonConvert.DeserializeObject<TVSeriesInfo>(tvSeries.Info);
            if (result != null) {
                result.SetUrlPart(tvSeries.UrlPart);
                result.SetId(tvSeries.Id);
            }
            return result;
        }

        public TVSeriesWatch GetSeriesWatch(long id) {
            return Adapter.ReadByContext(c => {
                TVSeries tvSeries = c.TVSeries.FirstOrDefault(e => e.LanguageId == _languageId && e.Id == id);
                if (tvSeries == null) {
                    return null;
                }

                TVSeriesWatch result = ToTVSeriesWatch(tvSeries);
                return result;
            });
        }

        private static TVSeriesWatch ToTVSeriesWatch(TVSeries tvSeries) {
            var result = JsonConvert.DeserializeObject<TVSeriesWatch>(tvSeries.Info);
            result.SetUrlPart(tvSeries.UrlPart);
            result.SetIds(tvSeries.Id, tvSeries.ParentId);
            return result;
        }

        public List<TVSeriesWatch> GetSeriesWatches(long parentId, long id) {
            return Adapter.ReadByContext(c => {
                IQueryable<TVSeries> tvSeries =
                    c.TVSeries.Where(e => e.LanguageId == _languageId && e.ParentId == parentId && e.Id != id);

                List<TVSeriesWatch> result = tvSeries.Select(ToTVSeriesWatch).OrderBy(e => e.Episode).ToList();
                return result;
            });
        }

        public List<TVSeriesInfo> GetSeriesCovers() {
            return Adapter.ReadByContext(c => {
                IQueryable<TVSeries> tvSeries =
                    c.TVSeries.Where(e => e.LanguageId == _languageId && e.DataType == (int) TVSeriesDataType.Cover);

                List<TVSeriesInfo> result = tvSeries.Select(ToCoverInfo).ToList();
                return result;
            });
        }

        public List<TVSeriesWatch> GetSeriesWatches(string urlPart) {
            return Adapter.ReadByContext(c => {
                TVSeries parentTVSeries =
                    c.TVSeries.FirstOrDefault(e => e.LanguageId == _languageId && e.UrlPart == urlPart);
                if (parentTVSeries == null) {
                    return new List<TVSeriesWatch>(0);
                }

                List<TVSeriesWatch> result = GetSeriesWatches(parentTVSeries.Id, IdValidator.INVALID_ID);
                if (EnumerableValidator.IsNotEmpty(result)) {
                    var coverInfo = JsonConvert.DeserializeObject<TVSeriesInfo>(parentTVSeries.Info);
                    result[0].SetSeriesInfo(coverInfo);
                }
                return result;
            });
        }

        private TVSeries GetParent(StudyLanguageContext c, long parentId) {
            TVSeries result = null;
            while (IdValidator.IsValid(parentId)) {
                result = c.TVSeries.FirstOrDefault(e => e.LanguageId == _languageId && e.Id == parentId);
                if (result == null || result.DataType == (byte) TVSeriesDataType.Cover) {
                    break;
                }
            }
            return result;
        }

        private static bool IsUrlPartInvalid(string urlPart) {
            return String.IsNullOrWhiteSpace(urlPart);
        }

        private TVSeries GetByUrlPart(StudyLanguageContext c, string urlPart) {
            return c.TVSeries.FirstOrDefault(e => e.LanguageId == _languageId && e.UrlPart == urlPart);
        }
    }
}