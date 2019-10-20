using System.Collections.Generic;
using NLPWrapper.ExternalObjects.Enums;

namespace NLPWrapper.ExternalObjects {
    public class PartSentence {
        private readonly List<PartSentence> _parts = new List<PartSentence>();
        private readonly List<Word> _words = new List<Word>();

        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Text { get; set; }
        public GrammarPhraseType Type { get; set; }

        public IEnumerable<Word> Words {
            get { return _words; }
        }

        public IEnumerable<PartSentence> Children {
            get { return _parts; }
        }

        internal PartSentence Parent { get; set; }

        internal void AddChildren(PartSentence part) {
            _parts.Add(part);
        }

        internal void AddWord(Word word) {
            _words.Add(word);
        }
    }
}