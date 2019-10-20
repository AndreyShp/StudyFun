using System.Collections.Generic;
using System.Globalization;
using BusinessLogic.DataQuery.Video;

namespace BusinessLogic.ExternalData.Videos {
    /// <summary>
    /// Информация о серии сериала
    /// </summary>
    public class TVSeriesWatch {
        private long _id;
        private long _parentId;
        private TVSeriesInfo _tvSeriesInfo;
        private string _urlPart;

        /// <summary>
        /// Сезон
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        /// Серия
        /// </summary>
        public int Episode { get; set; }

        /// <summary>
        /// Название серии в оригинале
        /// </summary>
        public string OrigTitle { get; set; }

        /// <summary>
        /// Перевод названии серии
        /// </summary>
        public string TransTitle { get; set; }

        /// <summary>
        /// Описание серии
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Имя видео файла
        /// </summary>
        public string VideoFileName { get; set; }

        /// <summary>
        /// Имя файла с изображением
        /// </summary>
        public string ImageFileName { get; set; }

        //TODO: возможно добавить оценку imdb

        /// <summary>
        /// Субтитры
        /// </summary>
        public List<Subtitle> Subtitles { get; set; }

        public void SetSeriesInfo(TVSeriesInfo tvSeriesInfo) {
            _tvSeriesInfo = tvSeriesInfo;
        }

        /// <summary>
        /// Получает информацию о сериале
        /// </summary>
        public TVSeriesInfo GetSeriesInfo() {
            return _tvSeriesInfo;
        }

        public void SetUrlPart(string urlPart) {
            _urlPart = urlPart;
        }

        public string GetUrlPart() {
            return _urlPart;
        }

        public string GetPathToFiles() {
            string result = GetSeriesInfo().PathToFiles;
            if (Season > 0) {
                result += TVSeriesQuery.PATH_DELIMITER + Season.ToString(CultureInfo.InvariantCulture);
            }
            return result;
        }

        public long GetParentId() {
            return _parentId;
        }

        public long GetId() {
            return _id;
        }

        public void SetIds(long id, long parentId) {
            _id = id;
            _parentId = parentId;
        }

        public bool IsAdmin { get; set; }
    }
}