using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.ExternalData.Videos;
using StudyLanguages.Models;

namespace StudyLanguages.Helpers {
    public static class BreadcrumbsHelper {
        public static List<BreadcrumbItem> GetTVSeries(UrlHelper url,
                                                       TVSeriesInfo seriesInfo,
                                                       TVSeriesWatch seriesWatch) {
            string seasonHeader = "Серия " + seriesWatch.Episode;
            if (seriesWatch.Season > 0) {
                seasonHeader = "Сезон " + seriesWatch.Season + ". " + seasonHeader;
            }

            return new List<BreadcrumbItem> {
                new BreadcrumbItem {
                    Html =
                        string.Format("<a href='{0}'>Сериал &laquo;{1}&raquo;</a>",
                                      url.Action("Detail", RouteConfig.TV_SERIES_CONTROLLER, new { baseUrlPart = seriesInfo.GetUrlPart() }),
                                      seriesInfo.Title),
                },
                new BreadcrumbItem {IsActive = true, Title = seasonHeader}
            };
        }

        public static List<BreadcrumbItem> GetVisualDictionaryFirstLevel(UrlHelper url, string title) {
            return new List<BreadcrumbItem> {
                new BreadcrumbItem {
                    Title = "Визуальные словари",
                    Action = "Index",
                    ControllerName = RouteConfig.VISUAL_DICTIONARIES_CONTROLLER_NAME
                },
                new BreadcrumbItem {IsActive = true, Title = title}
            };
        }

        public static List<BreadcrumbItem> GetVisualDictionary(UrlHelper url, string group, string title) {
            List<BreadcrumbItem> result = GetVisualDictionaryFirstLevel(url, title);
            result.Insert(1, new BreadcrumbItem {
                Html = string.Format("<a href='{0}'>Визуальный словарь на тему &laquo;{1}&raquo;</a>",
                                     url.Action("Index", RouteConfig.VISUAL_DICTIONARY_CONTROLLER,
                                                new {group = group + "/"}), group)
            });
            return result;
        }

        public static List<BreadcrumbItem> GetWords(UrlHelper url, string group, string title) {
            return new List<BreadcrumbItem> {
                new BreadcrumbItem
                {Title = "Все темы", Action = "Index", ControllerName = RouteConfig.GROUPS_BY_WORDS_CONTROLLER},
                new BreadcrumbItem {
                    Html = string.Format("<a href='{0}'>Все слова на тему &laquo;{1}&raquo;</a>",
                                         url.Action("Index", RouteConfig.GROUP_WORD_CONTROLLER,
                                                    new {group = group + "/"}), group)
                },
                new BreadcrumbItem {IsActive = true, Title = title}
            };
        }

        public static List<BreadcrumbItem> GetPhrases(UrlHelper url, string group, string title) {
            return new List<BreadcrumbItem> {
                new BreadcrumbItem
                {Title = "Все темы", Action = "Index", ControllerName = RouteConfig.GROUPS_BY_SENTENCES_CONTROLLER},
                new BreadcrumbItem {
                    Html = string.Format("<a href='{0}'>Все фразы на тему &laquo;{1}&raquo;</a>",
                                         url.Action("Index", RouteConfig.GROUP_SENTENCE_CONTROLLER,
                                                    new {group = group + "/"}), group)
                },
                new BreadcrumbItem {IsActive = true, Title = title}
            };
        }

        public static void AddCheckPayment(List<BreadcrumbItem> breadcrumbsItems, UrlHelper url) {
            breadcrumbsItems.Add(new BreadcrumbItem {
                Html =
                    string.Format(
                        "<a href='{0}' title='Проверить оплату покупок'><span class='glyphicon glyphicon-shopping-cart'></span></a>",
                        url.Action("Check", RouteConfig.PAYMENT_CONTROLLER)),
                WithoutDelimiter = true,
                LiClasses = "pull-right"
            });
        }
    }
}