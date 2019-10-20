using System.Collections.Generic;
using System.Linq;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models {
    public class TranslatorModel : BaseLanguageModel {
        public TranslatorModel(UserLanguages userLanguages) : this(userLanguages, null, null) {}

        public TranslatorModel(UserLanguages userLanguages, string sourceWord, List<PronunciationForUser> translations)
            : base(userLanguages) {
            Source = sourceWord;
            Translations = (translations ?? new List<PronunciationForUser>(0)).ToList();
        }

        public string Source { get; private set; }

        public List<PronunciationForUser> Translations { get; private set; }

        public bool HasTranslations {
            get { return !string.IsNullOrEmpty(Source) && Translations.Count > 0; }
        }
    }
}