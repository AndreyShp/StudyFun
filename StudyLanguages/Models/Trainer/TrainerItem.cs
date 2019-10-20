namespace StudyLanguages.Models.Trainer {
    public class TrainerItem {
        public long DataId { get; set; }
        public int DataType { get; set; }
        public long NextTimeToShow { get; set; }
        public string HtmlSource { get; set; }
        public string HtmlTranslation { get; set; }
        public long SourceLanguageId { get; set; }
        public long TranslationLanguageId { get; set; }
        public string ImageUrl { get; set; }
    }
}