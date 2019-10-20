using System;
using System.Collections.Generic;
using NLPWrapper.ExternalObjects.Enums;

namespace NLPWrapper {
    internal class FullWordForms {
        private static readonly Dictionary<GrammarWordType, Dictionary<string, string>> _shortAndFullFormsWords =
            new Dictionary<GrammarWordType, Dictionary<string, string>>();

        public FullWordForms() {
            Array enumValues = Enum.GetValues(typeof (GrammarWordType));
            foreach (GrammarWordType value in enumValues) {
                _shortAndFullFormsWords.Add(value, new Dictionary<string, string>());
            }
            //TODO: вынести в БД
            _shortAndFullFormsWords[GrammarWordType.PersonalPronoun].Add("'m", "am");
        }

        public bool TryGetFullForm(GrammarWordType grammarWordType, string shortForm, out string fullForm) {
            return _shortAndFullFormsWords[grammarWordType].TryGetValue(shortForm, out fullForm);
        }
    }
}