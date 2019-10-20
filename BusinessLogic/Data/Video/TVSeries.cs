namespace BusinessLogic.Data.Video {
    /// <summary>
    /// Сериалы
    /// </summary>
    public class TVSeries {
        public long Id { get; set; }
        public string Info { get; set; }
        public string UrlPart { get; set; }
        /// <summary>
        /// Идентификатор родителя, т.к. сериалы могут быть сгруппированы по сезонам
        /// </summary>
        public long ParentId { get; set; }
        public bool IsVisible { get; set; }
        public long LanguageId { get; set; }
        public byte DataType { get; set; }
    }
}