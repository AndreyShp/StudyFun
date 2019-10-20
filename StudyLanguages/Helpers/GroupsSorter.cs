using System;
using System.Collections.Generic;
using System.Web;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Comparisons;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.ExternalData.Videos;
using BusinessLogic.Logger;
using StudyLanguages.Models.Groups;

namespace StudyLanguages.Helpers {
    public class GroupsSorter {
        public const string COOKIE_NAME = "groupsSortType";

        private readonly SortType _type;

        public GroupsSorter(HttpCookieCollection cookies) {
            _type = GetTypeByCookie(cookies);
        }

        public static List<Tuple<int, string, bool>> GetTypes(HttpCookieCollection cookies) {
            SortType selectedType = GetTypeByCookie(cookies);
            var items = new List<Tuple<int, string, bool>> {
                CreateInfoByType(SortType.Rating, "популярности(популярные выше)", selectedType),
                CreateInfoByType(SortType.Name, "алфавиту", selectedType),
                CreateInfoByType(SortType.LastModifiedDate, "дате(новые выше)", selectedType)
            };
            return items;
        }

        private static Tuple<int, string, bool> CreateInfoByType(SortType sortType, string title, SortType selectedType) {
            return new Tuple<int, string, bool>((int)sortType, title, sortType == selectedType);
        } 

        private static SortType GetTypeByCookie(HttpCookieCollection cookies) {
            HttpCookie cookie = cookies[COOKIE_NAME];
            int parsedCookie;
            if (cookie != null && int.TryParse(cookie.Value, out parsedCookie)
                && Enum.IsDefined(typeof (SortType), parsedCookie)) {
                return (SortType) parsedCookie;
            }
            return SortType.Rating;
        }

        public void Sort(List<RepresentationForUser> representations) {
            representations.Sort((x, y) => Compare(x.SortInfo, y.SortInfo));
        }

        public void Sort(List<GroupForUser> groups) {
            groups.Sort((x, y) => Compare(x.SortInfo, y.SortInfo));
        }

        public void Sort(List<ComparisonForUser> comparisons) {
            comparisons.Sort((x, y) => Compare(x.SortInfo, y.SortInfo));
        }

        public void Sort(List<VideoForUser> videos) {
            videos.Sort((x, y) => Compare(x.SortInfo, y.SortInfo));
        }

        private int Compare(SortInfo first, SortInfo second) {
            int result;
            if (_type == SortType.Rating) {
                //обратный порядок
                result = SortDesc(first.Rating.CompareTo(second.Rating));
                if (result == 0) {
                    result = SortByName(first, second);
                }
                return result;
            }

            if (_type == SortType.Name) {
                return SortByName(first, second);
            }

            if (_type == SortType.LastModifiedDate) {
                result = SortDesc(first.LastModified.CompareTo(second.LastModified));
                if (result == 0) {
                    result = SortByName(first, second);
                }
                return result;
            }

            LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                "GroupsSorter.Compare не реализована операция сортировки для типа {0}! Кинем исключение!!!", _type);
            throw new ArgumentException("Некорректный тип исключения");
        }

        private static int SortByName(SortInfo first, SortInfo second) {
            return string.Compare(first.Name, second.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        private int SortDesc(int result) {
            return result * -1;
        }
    }
}