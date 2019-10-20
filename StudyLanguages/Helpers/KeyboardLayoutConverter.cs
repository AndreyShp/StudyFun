using System;
using System.Collections.Generic;
using System.Text;
using StudyLanguages.Configs;

namespace StudyLanguages.Helpers {
    public class KeyboardLayoutConverter {
        public List<string> Convert(string input) {
            var result = new List<string>();

            var cyrilic = new StringBuilder();
            var foreign = new StringBuilder();
            foreach (char ch in input) {
                char lowerChar = char.ToLower(ch);
                AddConvertedCharToResult(lowerChar, WebSettingsConfig.Instance.ForeignToRussianKeys, cyrilic);
                AddConvertedCharToResult(lowerChar, WebSettingsConfig.Instance.RussianToForeignKeys, foreign);
            }

            string cyrilicResult = cyrilic.ToString();
            string foreignResult = foreign.ToString();

            if (!string.Equals(cyrilicResult, input, StringComparison.InvariantCultureIgnoreCase)) {
                result.Add(cyrilicResult);
            }

            if (!string.Equals(foreignResult, input, StringComparison.InvariantCultureIgnoreCase)) {
                result.Add(foreignResult);
            }

            return result;
        }

        private static void AddConvertedCharToResult(char ch, Dictionary<char, char> charsMap, StringBuilder result) {
            char convertedChar;
            if (!charsMap.TryGetValue(ch, out convertedChar)) {
                convertedChar = ch;
            }
            result.Append(convertedChar);
        }
    }
}