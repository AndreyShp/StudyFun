using System.Collections.Generic;

namespace NLPWrapper.ExternalObjects {
    public class Sentence {
        private readonly PartSentence _partSentence;
        private readonly List<Word> _words = new List<Word>();

        public Sentence(PartSentence partSentence) {
            _partSentence = partSentence;
        }

        public PartSentence PartSentence {
            get { return _partSentence; }
        }

        public List<Word> Words {
            get { return _words; }
        }

        public string Text { get; internal set; }

        internal void AddWord(Word word) {
            _words.Add(word);
        }
    }
}