using System.Collections.Generic;
using System.Linq;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models.TopPanel {
    /// <summary>
    /// Описывает пункт меню
    /// </summary>
    public class MenuItem {
        private MenuItem(SectionId sectionId, string title, string action, string controller) {
            SectionId = sectionId;
            Title = title;
            Action = action;
            Controller = controller;
            Children = new List<MenuItem>();
        }

        /// <summary>
        /// Идентификатор раздела
        /// </summary>
        public SectionId SectionId { get; private set; }

        /// <summary>
        /// Название пункта меню
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Действие, которое нужно вызвать при выборе пункта меню
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Контроллер, который нужно вызвать при выборе пункта меню
        /// </summary>
        public string Controller { get; private set; }

        /// <summary>
        /// Данные для маршрутизации
        /// </summary>
        public object RouteValues { get; set; }

        /// <summary>
        /// Класс для меню
        /// </summary>
        public string Class { get; private set; }

        /// <summary>
        /// Нужен ли разделитель
        /// </summary>
        public bool NeedDivider { get; private set; }

        /// <summary>
        /// Подменю
        /// </summary>
        public List<MenuItem> Children { get; set; }

        /// <summary>
        /// Определяет есть ли подменю
        /// </summary>
        public bool HasChildren() {
            return EnumerableValidator.IsNotNullAndNotEmpty(Children);
        }

        /// <summary>
        /// Определяет есть ли дочерний элемент с идентификатором
        /// </summary>
        /// <param name="sectionId">идентификатор секции</param>
        /// <returns>true - есть дочерний элемент с идентификатором, false - нет дочернего элемента с идентификаторами</returns>
        public bool HasChildWithSectionId(SectionId sectionId) {
            return sectionId != SectionId.No && HasChildren() && Children.Any(e => e.SectionId == sectionId);
        }

        /// <summary>
        /// Возвращает пункты меню
        /// </summary>
        /// <param name="availableSectionIds">доступные секции</param>
        /// <returns>пункты меню</returns>
        public static List<MenuItem> GetMenuItems(HashSet<SectionId> availableSectionIds) {
            const int MAX_COUNT_VISIBLE_MENU_ITEMS = 6;

            var items = new List<MenuItem> {
                new MenuItem(SectionId.GroupByWords, "Слова по темам", "Index", RouteConfig.GROUPS_BY_WORDS_CONTROLLER)
            };

            AddSubmenu(availableSectionIds, "Полезности", new [] {
                new MenuItem(SectionId.FillDifference, "Почувствуй разницу", "Index",
                             RouteConfig.GROUPS_BY_COMPARISONS_CONTROLLER),
                new MenuItem(SectionId.PopularWord, "Минилекс слов", "Index", RouteConfig.POPULAR_WORDS_CONTROLLER)
            }, items);

            items.AddRange(new[] {
                new MenuItem(SectionId.VisualDictionary, "Визуальные словари", "Index",
                             RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME),
                new MenuItem(SectionId.GroupByPhrases, "Фразы по темам", "Index",
                             RouteConfig.GROUPS_BY_SENTENCES_CONTROLLER),
                new MenuItem(SectionId.No,
                             "Знания <b class=\"caret\"></b><div id=\"knowledgeShortInfo\" title=\"Ваших знаний за сегодня/за все время\" class=\"knowledge-short-info\"></div>",
                             null, null) {
                                 Class = "knowledge-menu-item",
                                 Children = new List<MenuItem> {
                                     new MenuItem(SectionId.MyKnowledge, "Стена знаний", "Index",
                                                  RouteConfig.USER_KNOWLEDGE_CONTROLLER),
                                     new MenuItem(SectionId.KnowledgeGenerator, "Генератор знаний", "Index",
                                                  RouteConfig.KNOWLEDGE_GENERATOR_CONTROLLER),
                                 }
                             },
            });

            AddSubmenu(availableSectionIds, "Видео", new[] {
                //TODO: добавить ссылку на все сериалы
                new MenuItem(SectionId.TVSeries, "Сериал «" + CommonConstants.FRIENDS_TV_SERIES + "»", "Detail", RouteConfig.TV_SERIES_CONTROLLER) { RouteValues = new { baseUrlPart = CommonConstants.FRIENDS_TV_SERIES }},
                new MenuItem(SectionId.Video, "Видеоролики", "Index", RouteConfig.VIDEO_CONTROLLER)
            }, items);
                
            items.AddRange(new [] {
                new MenuItem(SectionId.UserTasks, "Задания", "Index", RouteConfig.USER_TASKS_CONTROLLER),
                new MenuItem(SectionId.Sentences, "Предложения", "Index", RouteConfig.HOME_CONTROLLER),
                new MenuItem(SectionId.Audio, "Аудирование", "Index", RouteConfig.AUDIO_WORDS_CONTROLLER),
                new MenuItem(SectionId.WordTranslation, "Перевод слов", "Index",
                             RouteConfig.WORDS_TRANSLATION_CONTROLLER),
                new MenuItem(SectionId.PhraseVerbTranslation, "Перевод фразовых глаголов", "Index",
                             RouteConfig.PHRASAL_VERBS_TRANLATION_CONTROLLER)
            });             

            List<MenuItem> availableSections =
                items.Where(
                    e =>
                    availableSectionIds.Contains(e.SectionId)
                    || (e.HasChildren() && e.Children.Any(e2 => availableSectionIds.Contains(e2.SectionId)))).ToList();
            List<MenuItem> result;
            if (availableSections.Count > MAX_COUNT_VISIBLE_MENU_ITEMS) {
                result = new List<MenuItem>();
                result.AddRange(availableSections.Take(MAX_COUNT_VISIBLE_MENU_ITEMS));

                var otherMenus = availableSections.Skip(MAX_COUNT_VISIBLE_MENU_ITEMS).ToList();
                otherMenus.Add(new MenuItem(SectionId.No, "На главную", "Index",
                                                        RouteConfig.MAIN_CONTROLLER_NAME) { NeedDivider = true });

                AddSubmenu(availableSectionIds, "Другое", otherMenus, result);
            } else {
                result = availableSections;
            }
            return result;
        }

        private static void AddSubmenu(HashSet<SectionId> availableSectionIds,
                                       string title,
                                       IEnumerable<MenuItem> menuItems,
                                       List<MenuItem> result) {

            var children = menuItems.Where(menuItem => menuItem.SectionId == SectionId.No || availableSectionIds.Contains(menuItem.SectionId)).ToList();
            if (EnumerableValidator.IsEmpty(children)) {
                return;
            }

            MenuItem item;
            if (children.Count == 1) {
                item = children[0];
            } else {
                item = new MenuItem(SectionId.No, title + " <b class=\"caret\"></b>", null, null) {Children = children};
            }
            result.Add(item);
        }
    }
}