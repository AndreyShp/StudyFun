using System.Collections.Generic;
using BusinessLogic.Validators;

namespace BusinessLogic.ExternalData {
    /// <summary>
    /// Предложение со словами
    /// </summary>
    public class SentenceWithWords {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sentence">текст предложения</param>
        public SentenceWithWords(PronunciationForUser sentence) {
            Sentence = sentence;
            Words = new List<PronunciationForUser>(0);
        }

        /// <summary>
        /// Текст предложения
        /// </summary>
        public PronunciationForUser Sentence { get; private set; }

        /// <summary>
        /// Слова предложения в нужном порядке
        /// </summary>
        public List<PronunciationForUser> Words { get; private set; }

        /// <summary>
        /// Добавляет слово
        /// </summary>
        /// <param name="word">слово</param>
        public void AddWord(PronunciationForUser word) {
            if (word == null || string.IsNullOrWhiteSpace(word.Text)) {
                return;
            }
            Words.Add(word);
        }

        /// <summary>
        /// Добавляет слово
        /// </summary>
        /// <param name="word">слово</param>
        public void AddWord(string word) {
            AddWord(new PronunciationForUser(IdValidator.INVALID_ID, word, false, Sentence.LanguageId));
        }
    }
}