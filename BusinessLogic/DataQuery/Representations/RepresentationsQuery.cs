using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Representation;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.ExternalData.Sales;
using BusinessLogic.Helpers;
using BusinessLogic.Logger;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery.Representations {
    public class RepresentationsQuery : BaseQuery, IRepresentationsQuery, IRatingQuery {
        private readonly long _languageId;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="languageId">идентификатор языка, на котором нужно получать визуальные словари</param>
        public RepresentationsQuery(long languageId) {
            _languageId = languageId;
        }

        #region IRatingQuery Members

        public bool IncRating(long entityId) {
            bool result = Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                const string SQL_COMMAND = "update Representation set Rating=coalesce(Rating, 0)+1 where Id={0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             entityId
                                                         });
            });
            return result;
        }

        #endregion

        #region IRepresentationsQuery Members

        public RepresentationForUser GetOrCreate(RepresentationForUser representationForUser) {
            if (IsInvalid(representationForUser)) {
                return null;
            }

            bool isSuccess = true;
            RepresentationForUser result = null;
            Adapter.ActionByContext(c => {
                result = GetOrCreateRepresentation(representationForUser, c);
                if (result == null) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                        "RepresentationsQuery.GetOrCreate не удалось создать представление! Название: {0}, изображение: {1}",
                        representationForUser.Title,
                        representationForUser.Image != null
                            ? representationForUser.Image.Length.ToString(CultureInfo.InvariantCulture)
                            : "<NULL>");
                    isSuccess = false;
                    return;
                }

                var wordsQuery = new WordsQuery();
                foreach (RepresentationAreaForUser areaForUser in representationForUser.Areas) {
                    long wordTranslationId = wordsQuery.GetIdByWordsForUser(areaForUser.Source, areaForUser.Translation);
                    if (IdValidator.IsInvalid(wordTranslationId)) {
                        LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                            "RepresentationsQuery.GetOrCreate не найти связку для слов!" +
                            "Id представления: {0}, слово источник {1}, слово перевод {2}",
                            result.Id, areaForUser.Source.Text, areaForUser.Translation.Text);
                        isSuccess = false;
                        continue;
                    }

                    RepresentationArea representationArea = GetOrCreateArea(c, result.Id, wordTranslationId, areaForUser);
                    if (IdValidator.IsInvalid(representationArea.Id)) {
                        LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                            "RepresentationsQuery.GetOrCreate не удалось создать область представления! " +
                            "Id представления: {0}, левый верхний угол: {1}, правый нижний угол: {2}, исходное слово {3}, слово перевод {4}",
                            result.Id, areaForUser.LeftUpperCorner, areaForUser.RightBottomCorner,
                            areaForUser.Source.Text,
                            areaForUser.Translation.Text);
                        isSuccess = false;
                        continue;
                    }
                    var newRepresentationArea = new RepresentationAreaForUser(representationArea)
                    {Source = areaForUser.Source, Translation = areaForUser.Translation};
                    result.AddArea(newRepresentationArea);
                }

                if (isSuccess) {
                    //удалить слова из группы, которые не были переданы в этот раз в группу
                    DeleteOldVisualWords(c, result);
                }
            });
            return isSuccess ? result : null;
        }

        /// <summary>
        /// Возвращает список видимых представлений
        /// </summary>
        /// <param name="count">кол-во записей, если значение 0 или отрицательное, то все записи</param>
        /// <returns>список видимых представлений</returns>
        public List<RepresentationForUser> GetVisibleWithoutAreas(int count = 0) {
            List<RepresentationForUser> result = Adapter.ReadByContext(c => {
                IQueryable<Representation> representationsQuery = (from r in c.Representation
                                                                   where r.IsVisible && r.LanguageId == _languageId
                                                                   orderby r.Rating descending , r.Title
                                                                   select r);
                if (count > 0) {
                    representationsQuery = representationsQuery.Take(count);
                }
                List<RepresentationForUser> innerResult =
                    representationsQuery.AsEnumerable().Select(e => new RepresentationForUser(e)).ToList();
                return innerResult;
            });
            return result ?? new List<RepresentationForUser>(0);
        }

        /// <summary>
        /// Получает представление по названию
        /// </summary>
        /// <param name="userLanguages">языковые настройки пользователя</param>
        /// <param name="title">название представления</param>
        /// <returns>представление или null если не найдено</returns>
        public RepresentationForUser GetWithAreas(UserLanguages userLanguages, string title) {
            long sourceLanguageId = userLanguages.From.Id;
            long translationLanguageId = userLanguages.To.Id;

            RepresentationForUser result = Adapter.ReadByContext(c => {
                var representationsQuery = (from r in c.Representation
                                            join ra in c.RepresentationArea on r.Id equals
                                                ra.RepresentationId
                                            join wt in c.WordTranslation on ra.WordTranslationId equals wt.Id
                                            join w1 in c.Word on wt.WordId1 equals w1.Id
                                            join w2 in c.Word on wt.WordId2 equals w2.Id
                                            where r.Title == title && r.LanguageId == _languageId &&
                                                  ((w1.LanguageId == sourceLanguageId
                                                    && w2.LanguageId == translationLanguageId)
                                                   ||
                                                   (w1.LanguageId == translationLanguageId
                                                    && w2.LanguageId == sourceLanguageId))
                                            orderby ra.Rating descending
                                            select new {r, ra, w1, w2});
                var representationsInfos = representationsQuery.AsEnumerable();
                var firstRepresentation = representationsInfos.FirstOrDefault();
                if (firstRepresentation == null) {
                    return null;
                }
                var innerResult = new RepresentationForUser(firstRepresentation.r);
                foreach (var representationsInfo in representationsInfos) {
                    RepresentationArea repArea = representationsInfo.ra;

                    Tuple<PronunciationForUser, PronunciationForUser> tuple =
                        GroupingHelper.GroupByLanguages(sourceLanguageId,
                                                        representationsInfo.w1,
                                                        representationsInfo.w2);

                    var area = new RepresentationAreaForUser(repArea) {Source = tuple.Item1, Translation = tuple.Item2};
                    innerResult.AddArea(area);
                }
                return innerResult;
            });
            return result;
        }

        /// <summary>
        /// Получает изображение для представления
        /// </summary>
        /// <param name="title">название представления, для которого нужно получить изображение</param>
        /// <returns>массив байт представляющий изображение</returns>
        public byte[] GetImage(string title) {
            Representation representation = GetByName(title);
            return representation != null ? representation.Image : null;
        }

        /// <summary>
        /// Получает идентификатор визуального словаря по его названию
        /// </summary>
        /// <param name="title">название визуального словаря</param>
        /// <returns>идентификатор визуального словаря</returns>
        public long GetId(string title) {
            Representation representation = GetByName(title);
            return representation != null ? representation.Id : IdValidator.INVALID_ID;
        }

        /// <summary>
        /// Получает данные для продажи визуальных словарей
        /// </summary>
        /// <param name="userLanguages">данные по языку</param>
        /// <param name="salesSettings">настройки для продажи</param>
        /// <returns>данные для продажи визуальных словарей</returns>
        public List<SalesItemForUser> GetForSales(UserLanguages userLanguages, ISalesSettings salesSettings) {
            List<RepresentationForUser> withoutImages = GetVisibleWithoutAreas().OrderBy(e => e.Title).ToList();
            List<SalesItemForUser> result =
                withoutImages.Select(
                    e => new SalesItemForUser {Id = e.Id, Name = e.Title, Price = salesSettings.GetPrice(e.Id, e.Title)})
                    .ToList();
            return result;
        }

        public List<RepresentationForUser> GetBought(UserLanguages userLanguages, HashSet<long> ids) {
            //TODO: оптимизация запросов, вытянуть одним большим
            List<RepresentationForUser> withoutImages =
                GetVisibleWithoutAreas().Where(e => ids.Contains(e.Id)).OrderBy(e => e.Title).ToList();
            var result = new List<RepresentationForUser>();
            foreach (RepresentationForUser withoutImage in withoutImages) {
                RepresentationForUser representationImage = GetWithAreas(userLanguages, withoutImage.Title);
                result.Add(representationImage);
            }
            return result;
        }

        #endregion

        private static void DeleteOldVisualWords(StudyLanguageContext c, RepresentationForUser representationForUser) {
            var areasIds = new HashSet<long>(representationForUser.Areas.Select(e => e.Id));

            bool needRemove = false;
            List<RepresentationArea> areas =
                c.RepresentationArea.Where(e => e.RepresentationId == representationForUser.Id).ToList();
            foreach (RepresentationArea area in areas) {
                if (areasIds.Contains(area.Id)) {
                    continue;
                }
                c.RepresentationArea.Remove(area);
                needRemove = true;
            }
            if (needRemove) {
                c.SaveChanges();
            }
        }

        private static RepresentationArea GetOrCreateArea(StudyLanguageContext c,
                                                          long representationId,
                                                          long wordTranslationId,
                                                          RepresentationAreaForUser areaForUser) {
            RepresentationArea representationArea = c.RepresentationArea
                .FirstOrDefault(e => e.RepresentationId == representationId && e.WordTranslationId == wordTranslationId);

            if (representationArea == null) {
                representationArea = new RepresentationArea {
                    RepresentationId = representationId,
                    WordTranslationId = wordTranslationId
                };
                c.RepresentationArea.Add(representationArea);
            }

            representationArea.LeftUpperX = areaForUser.LeftUpperCorner.X;
            representationArea.LeftUpperY = areaForUser.LeftUpperCorner.Y;
            representationArea.RightBottomX = areaForUser.RightBottomCorner.X;
            representationArea.RightBottomY = areaForUser.RightBottomCorner.Y;
            c.SaveChanges();

            return representationArea;
        }

        private RepresentationForUser GetOrCreateRepresentation(RepresentationForUser representationForUser,
                                                                StudyLanguageContext c) {
            Size size = representationForUser.Size;
            Representation representation =
                c.Representation.FirstOrDefault(
                    e => e.Title == representationForUser.Title && e.LanguageId == _languageId);
            if (representation == null) {
                representation = new Representation {
                    Title = representationForUser.Title,
                    IsVisible = true,
                    LanguageId = _languageId
                };
                c.Representation.Add(representation);
            }

            representation.Image = representationForUser.Image;
            representation.Width = size.Width;
            representation.Height = size.Height;
            representation.WidthPercent = representationForUser.WidthPercent;
            representation.LastModified = DateTime.Now;
            c.SaveChanges();

            return IdValidator.IsValid(representation.Id) ? new RepresentationForUser(representation) : null;
        }

        private static bool IsInvalid(RepresentationForUser representationForUser) {
            Size size = representationForUser != null ? representationForUser.Size : null;

            bool result = representationForUser == null || string.IsNullOrEmpty(representationForUser.Title)
                          || IsInvalidSize(size) || IsInvalidImage(representationForUser.Image)
                          || !EnumerableValidator.IsNotNullAndNotEmpty(representationForUser.Areas);

            if (!result) {
                IWordsQuery wordsQuery = new WordsQuery();
                //проверить области
                List<RepresentationAreaForUser> areas = representationForUser.Areas;
                foreach (RepresentationAreaForUser area in areas) {
                    if (wordsQuery.IsInvalid(area.Source) || wordsQuery.IsInvalid(area.Translation)
                        || IsInvalid(size, area.LeftUpperCorner) || IsInvalid(size, area.RightBottomCorner)) {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private static bool IsInvalid(Size size, Point point) {
            return point == null || point.X < 0 || point.Y < 0 || point.X > size.Width || point.Y > size.Height;
        }

        private static bool IsInvalidSize(Size size) {
            return size == null || size.Height <= 0 || size.Width <= 0;
        }

        private static bool IsInvalidImage(byte[] image) {
            return image == null || image.Length == 0;
        }

        /// <summary>
        /// Получает представление по идентификатору
        /// </summary>
        /// <param name="name">имя представления</param>
        /// <returns>представление или null если не найдено</returns>
        private Representation GetByName(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                return null;
            }
            Representation result = Adapter.ReadByContext(c => {
                IQueryable<Representation> representationsQuery = (from r in c.Representation
                                                                   where r.Title == name && r.LanguageId == _languageId
                                                                   select r);
                return representationsQuery.FirstOrDefault();
            });
            return result;
        }
    }
}