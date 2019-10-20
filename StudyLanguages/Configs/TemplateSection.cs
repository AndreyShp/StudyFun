using System.Collections.Generic;

namespace StudyLanguages.Configs {
    /// <summary>
    /// Описывает текстовые шаблоны для раздела
    /// </summary>
    public class TemplateSection {
        private readonly Dictionary<string, string> _templates = new Dictionary<string, string>();

        /// <summary>
        /// Получить текстовый шаблон
        /// </summary>
        /// <param name="pageId">идентификатор страницы</param>
        /// <param name="templateId">идентификатор текстового шаблона</param>
        /// <param name="args">параметры</param>
        /// <returns>текст</returns>
        public string Get(PageId pageId, TemplateId templateId, params object[] args) {
            string key = GetKey(templateId, pageId);

            string result;
            if (!_templates.TryGetValue(key, out result)) {
                return null;
            }

            return string.Format(result, args);
        }

        /// <summary>
        /// Установить текстовый шаблон
        /// </summary>
        /// <param name="pageId">идентификатор страницы</param>
        /// <param name="templateId">идентификатор текстового шаблона</param>
        /// <param name="text">текст</param>
        public void Set(PageId pageId, TemplateId templateId, string text) {
            string key = GetKey(templateId, pageId);
            if (_templates.ContainsKey(key)) {
                _templates[key] = text;
            } else {
                _templates.Add(key, text);
            }
        }

        private static string GetKey(TemplateId templateId, PageId pageId) {
            return templateId + "_" + pageId;
        }
    }
}