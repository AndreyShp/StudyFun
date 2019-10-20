using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using BusinessLogic.Data.Comparison;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData.Comparisons {
    public class ComparisonForUser {
        private readonly Dictionary<AdditionalType, string> _additionalInfo = new Dictionary<AdditionalType, string>();

        internal ComparisonForUser(GroupComparison groupComparison)
            : this(groupComparison.Title, groupComparison.Description) {
            Id = groupComparison.Id;
            SortInfo = new SortInfo(groupComparison);
            FillAdditionalInfo(groupComparison.AdditionalInfo);
        }

        public ComparisonForUser(string title, string description) {
            Title = title;
            Description = description;
            Items = new List<ComparisonItemForUser>();
        }

        public long Id { get; private set; }

        [ScriptIgnore]
        public string Title { get; private set; }

        public bool HasDescription {
            get { return !string.IsNullOrWhiteSpace(Description); }
        }

        /// <summary>
        /// Название группы различия в нижнем регистре
        /// </summary>
        public string LowerName {
            get { return Title.ToLowerInvariant(); }
        }

        public string Description { get; private set; }

        public List<ComparisonItemForUser> Items { get; private set; }
        /// <summary>
        /// Информация для сортировки
        /// </summary>
        [ScriptIgnore]
        public SortInfo SortInfo { get; private set; }

        public void AddItem(ComparisonItemForUser item) {
            Items.Add(item);
        }

        public bool IsValid() {
            return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description)
                   && EnumerableValidator.IsNotEmpty(Items) && Items.All(e => e.IsValid());
        }

        internal string GetAdditionalInfo() {
            if (EnumerableValidator.IsEmpty(_additionalInfo)) {
                return null;
            }

            var xElement = new XElement("Info");
            foreach (var additionalInfo in _additionalInfo) {
                xElement.Add(new XElement(additionalInfo.Key.ToString(), additionalInfo.Value));
            }
            return xElement.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Возвращает дополнительную информацию по типу, если она есть
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetAdditionalInfoByType(AdditionalType type, string defaultValue) {
            string result;
            if (!_additionalInfo.TryGetValue(type, out result)) {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Добавляет дополнительную информацию указанного типа
        /// </summary>
        /// <param name="type">тип</param>
        /// <param name="value">дополнительное значение</param>
        public void AddAdditional(AdditionalType type, string value) {
            if (!_additionalInfo.ContainsKey(type) && !string.IsNullOrWhiteSpace(value)) {
                _additionalInfo.Add(type, value);
            }
        }

        private void FillAdditionalInfo(string rawAdditionalType) {
            if (string.IsNullOrWhiteSpace(rawAdditionalType)) {
                return;
            }
            XElement xElement = XElement.Parse(rawAdditionalType);
            foreach (XElement child in xElement.Elements()) {
                AdditionalType additionalType;
                string value = child.Value;
                if (Enum.TryParse(child.Name.ToString(), true, out additionalType) && !string.IsNullOrWhiteSpace(value)) {
                    AddAdditional(additionalType, value);
                }
            }
        }
    }
}