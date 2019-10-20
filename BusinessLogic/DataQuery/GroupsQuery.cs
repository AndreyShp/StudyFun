using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.DataQuery.Ratings;
using BusinessLogic.DataQuery.Words;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;

namespace BusinessLogic.DataQuery {
    public class GroupsQuery : BaseQuery, IGroupsQuery, IRatingQuery {
        private readonly long _languageId;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="languageId">идентификатор языка, на котором нужно получать группы</param>
        public GroupsQuery(long languageId) {
            _languageId = languageId;
        }

        #region IGroupsQuery Members

        /// <summary>
        /// Получает все видимые группы
        /// </summary>
        /// <param name="type">тип группы</param>
        /// <param name="count">кол-во записей, если значение 0 или отрицательное, то все записи</param>
        /// <returns>список всех видимых групп</returns>
        public List<GroupForUser> GetVisibleGroups(GroupType type, int count = 0) {
            var parsedType = (int) type;
            List<GroupForUser> result = Adapter.ReadByContext(c => {
                IOrderedQueryable<Group> wordsWithTranslationsQuery = (from g in c.Group
                                                                       where
                                                                           g.IsVisible && g.Type == parsedType
                                                                           && g.LanguageId == _languageId
                                                                       orderby g.Rating descending , g.Name
                                                                       select g);
                IEnumerable<Group> query;
                if (count > 0) {
                    query = wordsWithTranslationsQuery.Take(count).AsEnumerable();
                } else {
                    query = wordsWithTranslationsQuery.AsEnumerable();
                }

                List<GroupForUser> innerResult = query.Select(e => new GroupForUser(e)).ToList();
                return innerResult;
            });
            return result;
        }

        /// <summary>
        /// Получает видимую группу по-имени
        /// </summary>
        /// <param name="type">тип группы</param>
        /// <param name="name">имя группы</param>
        /// <returns>найденная группа или null</returns>
        public GroupForUser GetVisibleGroupByName(GroupType type, string name) {
            List<GroupForUser> allGroups = GetVisibleGroups(type);
            return allGroups.FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Создает невидимую группу
        /// </summary>
        /// <param name="groupType">тип группы</param>
        /// <param name="name">имя группы</param>
        /// <param name="image">изображение группы</param>
        /// <param name="rating">рейтинг группы</param>
        /// <returns>созданную группу иначе null</returns>
        public GroupForUser GetOrCreate(GroupType groupType, string name, byte[] image, int? rating = null) {
            Group group = null;
            Adapter.ActionByContext(c => {
                group = GetGroupByName(c, name, groupType);
                if (group != null) {
                    //сохранить возможно изменившееся поля
                    SetGroup(group, groupType, image, rating);
                    return;
                }

                group = new Group {Name = name, LanguageId = _languageId};
                SetGroup(group, groupType, image, rating);
                c.Group.Add(group);
            }, true);

            return group != null && IdValidator.IsValid(group.Id) ? new GroupForUser(group) : null;
        }

        #endregion

        #region IRatingQuery Members

        public bool IncRating(long entityId) {
            bool result = Adapter.ActionByContext(c => {
                //TODO: написать нормальную поддержку обновления и джоинов
                const string SQL_COMMAND = "update [Group] set Rating=coalesce(Rating, 0)+1 where Id={0}";
                int count = c.Database.ExecuteSqlCommand(SQL_COMMAND,
                                                         new object[] {
                                                             entityId
                                                         });
            });
            return result;
        }

        #endregion

        /// <summary>
        /// Получает изображение для группы
        /// </summary>
        /// <param name="name">название группы, для которой нужно получить изображение</param>
        /// <param name="groupType">тип группы</param>
        /// <returns>массив байт представляющий изображение</returns>
        public byte[] GetImage(string name, GroupType groupType) {
            byte[] result = Adapter.ReadByContext(c => {
                Group group = GetGroupByName(c, name, groupType);
                return group != null ? group.Image : null;
            });
            return result;
        }

        private static void SetGroup(Group group, GroupType groupType, byte[] image, int? rating) {
            group.Image = image;
            group.Rating = rating;
            group.Type = (int) groupType;
            group.LastModified = DateTime.Now;
        }

        /// <summary>
        /// Получает группу по-имени(в том числе и невидимую)
        /// </summary>
        /// <param name="context">Контекст</param>
        /// <param name="name">имя группы</param>
        /// <param name="groupType">тип группы</param>
        /// <returns>найденная группа или null</returns>
        private Group GetGroupByName(StudyLanguageContext context, string name, GroupType groupType) {
            var parsedType = (int) groupType;
            IOrderedQueryable<Group> wordsWithTranslationsQuery = (from g in context.Group
                                                                   where
                                                                       g.Name == name && g.Type == parsedType
                                                                       && g.LanguageId == _languageId
                                                                   orderby g.Rating descending , g.Name
                                                                   select g);
            return wordsWithTranslationsQuery.FirstOrDefault();
        }
    }
}