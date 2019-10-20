using BusinessLogic.ExternalData;
using StudyLanguages.Configs;

namespace StudyLanguages.Models {
    /// <summary>
    /// Данные необходимые для каждой страницы
    /// </summary>
    public class PageRequiredData {
        public PageRequiredData(SectionId sectionId) {
            SectionId = sectionId;
        }

        public PageRequiredData(SectionId sectionId, PageId pageId, params object[] args) {
            SectionId = sectionId;
            Title = WebSettingsConfig.Instance.GetTemplateText(sectionId, pageId, TemplateId.Title, args);
            Description = WebSettingsConfig.Instance.GetTemplateText(sectionId, pageId, TemplateId.Description, args);
            Keywords = WebSettingsConfig.Instance.GetTemplateText(sectionId, pageId, TemplateId.Keywords, args);
        }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Мета-тег description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Мета-тег keywords
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Право на страницу
        /// </summary>
        public SectionId SectionId { get; private set; }
    }
}