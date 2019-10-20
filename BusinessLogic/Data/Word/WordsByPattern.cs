using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Data.Word {
    public class WordsByPattern {
        public List<string> Words { get; private set; }

        public bool IsChangedLanguage { get; set; }

        public void SetWords(List<Word> words) {
            Words = words.Select(e => e.Text).OrderBy(e => e).ToList();
        }

        public string NewPattern { get; set; }
    }
}