using System.Collections.Generic;
using BusinessLogic.Data;
using BusinessLogic.Data.Enums;
using BusinessLogic.Data.Word;

namespace BusinessLogic.ExternalData.Words {
    public class WordWithTranslation {
        public WordWithTranslation(PronunciationEntity word) {
            Source = new PronunciationForUser(word);
            Translations = new List<PronunciationForUser>();
        }

        public long Id { get; set; }

        public PronunciationForUser Source { get; private set; }

        public List<PronunciationForUser> Translations { get; private set; }

        public WordType WordType { get; set; }

        public void AddTranslation(Word translation) {
            Translations.Add(new PronunciationForUser(translation));
        }
    }
}