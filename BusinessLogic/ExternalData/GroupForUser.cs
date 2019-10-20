using System.Web.Script.Serialization;
using BusinessLogic.Data;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData {
    public class GroupForUser {
        public GroupForUser(Group group)
            : this(group.Id, group.Name, EnumerableValidator.IsNotNullAndNotEmpty(group.Image)) {
            SortInfo = new SortInfo(group);
        }

        public GroupForUser(long id, string name, bool hasImage) {
            Id = id;
            Name = name;
            HasImage = hasImage;
        }

        /// <summary>
        /// Идентификатор группы
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Имя группы
        /// </summary>
        [ScriptIgnore]
        public string Name { get; private set; }

        /// <summary>
        /// Имя группы в нижнем регистре
        /// </summary>
        public string LowerName {
            get { return Name.ToLowerInvariant(); }
        }

        /// <summary>
        /// Указывает есть ли изображение у текущей группы
        /// </summary>
        [ScriptIgnore]
        public bool HasImage { get; private set; }

        /// <summary>
        /// Информация для сортировки
        /// </summary>
        [ScriptIgnore]
        public SortInfo SortInfo { get; private set; }
    }
}