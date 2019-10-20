using System;
using System.Text;

namespace Sandbox.Classes {
    public class TextFormatter {
        public static string FirstUpperCharAndTrim(string text) {
            string result = Trim(text);
            if (string.IsNullOrEmpty(result)) {
                return text;
            }
            return char.ToUpperInvariant(result[0]) + GetExceptFirstChar(result);
        }

        public static string ToLowerAndTrim(string text) {
            string result = Trim(text);
            if (string.IsNullOrEmpty(result)) {
                return text;
            }
            return result.ToLowerInvariant();
        }

        public static string ToLowerAndNotTouchFirstCharWords(string value) {
            if (string.IsNullOrEmpty(value)) {
                return value;
            }
            string[] words = value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();
            foreach (string word in words) {
                if (result.Length > 0) {
                    result.Append(" ");
                }
                result.Append(word[0] + GetExceptFirstChar(word));
            }
            return result.ToString();
        }

        private static string GetExceptFirstChar(string word) {
            return word.Length > 1 ? word.Substring(1) : string.Empty;
        }

        private static string Trim(string text) {
            return (text ?? string.Empty).Trim();
        }
    }
}