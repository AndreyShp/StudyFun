using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Validators {
    public class EnumerableValidator {
        public static bool IsEmpty<T>(IEnumerable<T> elements) {
            return !IsNotEmpty(elements);
        }

        public static bool IsNotEmpty<T>(IEnumerable<T> elements) {
            return elements.Any();
        }

        public static bool IsNotNullAndNotEmpty<T>(IEnumerable<T> elements) {
            return elements != null && IsNotEmpty(elements);
        }

        public static bool IsNullOrEmpty<T>(IEnumerable<T> elements) {
            return elements == null || IsEmpty(elements);
        }

        public static bool IsCountEquals<T1, T2>(IEnumerable<T1> elements1, IEnumerable<T2> elements2) {
            return elements1.Count() == elements2.Count();
        }
    }
}