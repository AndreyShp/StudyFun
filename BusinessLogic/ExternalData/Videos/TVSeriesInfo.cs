namespace BusinessLogic.ExternalData.Videos {
    /// <summary>
    /// Информация о сериале
    /// </summary>
    public class TVSeriesInfo {
        private string _urlPart;
        private long _id;

        /// <summary>
        /// Название сериала в оригинале
        /// </summary>
        public string OrigTitle { get; set; }

        /// <summary>
        /// Название сериала в переводе
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание сериала
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Путь к файлам
        /// </summary>
        public string PathToFiles { get; set; }

        /// <summary>
        /// Имя файла с изображением
        /// </summary>
        public string ImageFileName { get; set; }

        public void SetUrlPart(string urlPart) {
            _urlPart = urlPart;
        }

        public string GetUrlPart() {
            return _urlPart;
        }

        public void SetId(long id) {
            _id = id;
        }

        public long GetId() {
            return _id;
        }
    }
}