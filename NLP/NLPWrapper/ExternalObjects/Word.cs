using System.Collections.Generic;
using NLPWrapper.ExternalObjects.Enums;

namespace NLPWrapper.ExternalObjects {
    public class Word {
        private List<string> _normalForms = new List<string>();

        public Word(string text, GrammarWordType type) {
            Text = text;
            Type = type;
        }

        public string Text { get; private set; }
        public GrammarWordType Type { get; private set; }
        public List<string> NormalForms {
            get { return _normalForms; }
            set { _normalForms = value ?? new List<string>(0); }
        }

        public string FullText { get; set; }

        public string GetAppropriateWordText() {
            return (string.IsNullOrWhiteSpace(FullText) ? Text : FullText).Trim();
        }
    }
}