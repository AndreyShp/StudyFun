using System.Collections.Generic;
using System.Xml.Linq;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Main;

namespace StudyLanguages.Configs {
    /// <summary>
    /// Описывает текстовые шаблоны для других разделов
    /// </summary>
    public class TextTemplates {
        private readonly Dictionary<SectionId, TemplateSection> _sectionsTemplates =
            new Dictionary<SectionId, TemplateSection>();

        /// <summary>
        /// Получить текст для раздела и по шаблону
        /// </summary>
        /// <param name="sectionId">идентификатор раздела</param>
        /// <param name="templateId">идентификатор шаблона</param>
        /// <param name="args">дополнительный шаблон</param>
        /// <returns>текстовые шаблоны</returns>
        public string Get(SectionId sectionId, TemplateId templateId, params object[] args) {
            return Get(sectionId, PageId.Index, templateId, args);
        }

        /// <summary>
        /// Получить текст для раздела и по шаблону
        /// </summary>
        /// <param name="sectionId">идентификатор раздела</param>
        /// <param name="pageId">идентификатор страницы</param>
        /// <param name="templateId">идентификатор шаблона</param>
        /// <param name="args">дополнительный шаблон</param>
        /// <returns>текстовые шаблоны</returns>
        public string Get(SectionId sectionId, PageId pageId, TemplateId templateId, params object[] args) {
            TemplateSection section;
            if (!_sectionsTemplates.TryGetValue(sectionId, out section)) {
                return null;
            }

            string result = section.Get(pageId, templateId, args);
            return result;
        }

        public void Load(XElement patternsElement) {
            _sectionsTemplates.Clear();

            foreach (XElement sectionElement in patternsElement.Elements("Section")) {
                var sectionId = XmlParseHelper.ParseAttribute<SectionId>(sectionElement, "Id");
                var pageId = XmlParseHelper.ParseAttribute<PageId>(sectionElement, "PageId");

                TemplateSection templateSection = GetOrAddSection(sectionId);

                Dictionary<TemplateId, string> templates =
                    XmlParseHelper.GetDictionary<TemplateId, string>(sectionElement, "Template");
                foreach (var template in templates) {
                    templateSection.Set(pageId,
                                        template.Key,
                                        template.Value);
                }
            }
        }

        private TemplateSection GetOrAddSection(SectionId sectionId) {
            TemplateSection section;
            if (!_sectionsTemplates.TryGetValue(sectionId, out section)) {
                section = new TemplateSection();
                _sectionsTemplates.Add(sectionId, section);
            }
            return section;
        }
    }
}