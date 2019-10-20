using System.Collections.Generic;

namespace BusinessLogic.DataQuery.UserRepository.Texts {
    public class TextSeparator {
        private readonly string _text;

        public TextSeparator(string text) {
            _text = text;
        }

        public List<TextWord> GetWords() {
            var result = new List<TextWord>();
            var currentWord = new TextWord();
            for (int i = 0; i < _text.Length; i++) {
                char ch = _text[i];
                bool isPunctuation = char.IsPunctuation(ch);
                bool isNotBreakChar = ch == '-';
                if (isNotBreakChar || char.IsLetterOrDigit(ch) || isPunctuation) {
                    currentWord.AddCharToWord(ch, i);
                }

                if (isNotBreakChar) {
                    continue;
                }

                if (ch == '.' || ch == ',' || ch == '!' || ch == '?' || char.IsSeparator(ch)) {
                    AddWordToResultIfNeed(currentWord, result);
                    currentWord = new TextWord();
                }
            }

            AddWordToResultIfNeed(currentWord, result);
            return result;
        }

        private static void AddWordToResultIfNeed(TextWord currentWord, List<TextWord> result) {
            if (!string.IsNullOrEmpty(currentWord.Word)) {
                result.Add(currentWord);
            }
        }
    }
}