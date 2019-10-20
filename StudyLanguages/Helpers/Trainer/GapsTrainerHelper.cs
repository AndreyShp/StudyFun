using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.ExternalData;
using StudyLanguages.Models.Groups;

namespace StudyLanguages.Helpers.Trainer {
    public class GapsTrainerHelper {
        public const char GAP_CHAR = '_';
        private readonly Random _rnd = new Random();

        public List<GapsTrainerItem> ConvertToItems(IEnumerable<ISourceWithTranslation> sourceWithTranslations) {
            return sourceWithTranslations.Select(ConvertToGapsItem).ToList();
        }

        private GapsTrainerItem ConvertToGapsItem(ISourceWithTranslation sourceWithTranslation) {
            var item = new GapsTrainerItem {
                Id = sourceWithTranslation.Id,
                TextForUser = GetTextWithGaps(sourceWithTranslation.Source.Text),
                Original = sourceWithTranslation.Source,
                Translation = sourceWithTranslation.Translation
            };
            return item;
        }

        private string GetTextWithGaps(string text) {
            var result = new StringBuilder();
            string[] words = text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words) {
                char[] wordWithGaps = GetWordWithGaps(word);
                if (result.Length > 0) {
                    result.Append(" ");
                }
                result.Append(wordWithGaps);
            }
            return result.ToString();
        }

        private char[] GetWordWithGaps(string word) {
            string trimmedWord = word.Trim();
            List<int> indexesToReplace = GetCandidatesToReplace(word);
            int countReplacedChars = GetCountCharsToReplace(indexesToReplace.Count);

            char[] result = trimmedWord.ToCharArray();
            while (countReplacedChars > 0) {
                int index = _rnd.Next(0, indexesToReplace.Count);

                int charIndex = indexesToReplace[index];
                result[charIndex] = GAP_CHAR;
                countReplacedChars--;
                indexesToReplace.RemoveAt(index);
            }
            return result;
        }

        /// <summary>
        /// Возвращает кол-во символов для замены
        /// </summary>
        /// <param name="maxCountToReplace">максимальное кол-во символов, которые можно заменять</param>
        /// <returns>кол-во символов для замены</returns>
        private int GetCountCharsToReplace(int maxCountToReplace) {
            if (maxCountToReplace <= 1) {
                return 0;
            }

            int median = maxCountToReplace / 2;
            int min = median - (median / 2);
            int max = median + (median / 2);
            if (maxCountToReplace % 2 != 0) {
                max++;
            }

            //+1, т.к. max включительно
            return _rnd.Next(min, max + 1);
        }

        /// <summary>
        /// Возвращает коллекцию индексов тех символов, которые можно заменять
        /// </summary>
        /// <param name="word">слово</param>
        /// <returns>коллекцию индексов тех символов, которые можно заменять</returns>
        private static List<int> GetCandidatesToReplace(string word) {
            var result = new List<int>();
            for (int i = 0; i < word.Length; i++) {
                char ch = word[i];
                if (char.IsLetter(ch)) {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}