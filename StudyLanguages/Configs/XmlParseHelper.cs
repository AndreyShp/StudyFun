using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using BusinessLogic.Validators;

namespace StudyLanguages.Configs {
    public static class XmlParseHelper {
        public static T Get<T>(XElement element, params string[] names) {
            if (EnumerableValidator.IsEmpty(names)) {
                return default(T);
            }

            XElement elem = GetElementByNames(element, names);
            string valueElementName = names[names.Length - 1];
            XElement valueElement = elem != null ? elem.Element(valueElementName) : null;
            return valueElement != null ? ParseValue<T>(GetElementValue(valueElement)) : default(T);
        }

        private static string GetElementValue(XElement valueElement) {
            return valueElement.Value;
        }

        private static XElement GetElementByNames(XElement element, string[] names) {
            XElement elem = element;
            int i = 0;
            do {
                if (elem == null || i >= names.Length - 1) {
                    break;
                }
                elem = elem.Element(names[i]);
                i++;
            } while (true);
            return elem;
        }

        public static List<T> GetList<T>(XElement element, params string[] names) {
            if (EnumerableValidator.IsEmpty(names)) {
                return null;
            }

            XElement elem = GetElementByNames(element, names);
            string valueElementName = names[names.Length - 1];

            var result = new List<T>();
            if (elem == null) {
                return result;
            }
            foreach (XElement valueElement in elem.Elements(valueElementName)) {
                string dirtyValue = GetElementValue(valueElement);
                var parsedValue = ParseValue<T>(dirtyValue);
                result.Add(parsedValue);
            }
            return result;
        }

        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(XElement element, params string[] names) {
            if (EnumerableValidator.IsEmpty(names)) {
                return null;
            }

            XElement elem = GetElementByNames(element, names);
            string valueElementName = names[names.Length - 1];

            var result = new Dictionary<TKey, TValue>();
            if (elem == null) {
                return result;
            }
            foreach (XElement valueElement in elem.Elements(valueElementName)) {
                var parsedKey = ParseAttribute<TKey>(valueElement, "key");
                var parsedValue = ParseAttribute<TValue>(valueElement, "value");
                result.Add(parsedKey, parsedValue);
            }
            return result;
        }

        public static T ParseAttribute<T>(XElement element, string name) {
            string dirtyValue = GetAttributeByName(element, name);
            return ParseValue<T>(dirtyValue);
        }

        private static string GetAttributeByName(XElement element, string name) {
            return element.Attribute(name).Value;
        }

        private static T ParseValue<T>(string dirtyValue) {
            Type type = typeof (T);
            if (type.BaseType != null && type.BaseType == typeof (Enum)) {
                type = type.BaseType;
            }

            if (type == typeof (Enum)) {
                return (T) Enum.Parse(typeof (T), dirtyValue);
            }

            if (type == typeof (string)) {
                return (T) (object) dirtyValue;
            }

            if (type == typeof (char)) {
                return (T) (object) char.Parse(dirtyValue);
            }

            if (type == typeof (bool)) {
                return (T) (object) bool.Parse(dirtyValue);
            }

            if (type == typeof (long)) {
                return (T) (object) long.Parse(dirtyValue);
            }

            if (type == typeof (int)) {
                return (T) (object) int.Parse(dirtyValue);
            }

            if (type == typeof (decimal)) {
                return (T) (object) decimal.Parse(dirtyValue, NumberStyles.Number, CultureInfo.InvariantCulture);
            }

            if (type == typeof (double)) {
                return (T) (object) double.Parse(dirtyValue, NumberStyles.Number, CultureInfo.InvariantCulture);
            }

            if (type == typeof (float)) {
                return (T) (object) float.Parse(dirtyValue, NumberStyles.Number, CultureInfo.InvariantCulture);
            }

            throw new InvalidCastException("XmlHelper не удалось преобразовать значение " + dirtyValue + " к типу "
                                           + typeof (T));
        }
    }
}