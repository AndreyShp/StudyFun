using System.Collections.Generic;
using NLPWrapper.ExternalObjects.Enums;

namespace NLPWrapper {
    internal class TypesConverter {
        private static readonly Dictionary<string, GrammarWordType> _typesToGrammarWordTypes =
            new Dictionary<string, GrammarWordType> {
                {"cc", GrammarWordType.CoordinatingConjunction},
                {"cd", GrammarWordType.CardinalNumber},
                {"dt", GrammarWordType.Determiner},
                {"ex", GrammarWordType.ExistentialThere},
                {"fw", GrammarWordType.ForeignWord},
                {"in", GrammarWordType.PrepositionOrSubordinateConjunction},
                {"jj", GrammarWordType.Adjective},
                {"jjr", GrammarWordType.AdjectiveComparative},
                {"jjs", GrammarWordType.AdjectiveSuperlative},
                {"ls", GrammarWordType.ListItemMarker},
                {"md", GrammarWordType.Modal},
                {"nn", GrammarWordType.Noun},
                {"nnp", GrammarWordType.NounSingularProper},
                {"nnps", GrammarWordType.NounPluralProper},
                {"nns", GrammarWordType.NounPlural},
                {"pdt", GrammarWordType.Predeterminer},
                {"pos", GrammarWordType.PossessiveEnding},
                {"prp", GrammarWordType.PersonalPronoun},
                {"prp$", GrammarWordType.PossessivePronoun},
                {"rb", GrammarWordType.Adverb},
                {"rbr", GrammarWordType.AdverbComparative},
                {"rbs", GrammarWordType.AdverbSuperlative},
                {"rp", GrammarWordType.Particle},
                {"sym", GrammarWordType.Symbol},
                {"to", GrammarWordType.To},
                {"uh", GrammarWordType.Interjection},
                {"vb", GrammarWordType.VerbBaseForm},
                {"vbd", GrammarWordType.VerbPastTense},
                {"vbg", GrammarWordType.VerbGerundOrPresentParticiple},
                {"vbn", GrammarWordType.VerbPastParticiple},
                {"vbp", GrammarWordType.VerbNonThirdSingularPresent},
                {"vbz", GrammarWordType.VerbThirdSingularPresent},
                {"wdt", GrammarWordType.WhDeterminer},
                {"wp", GrammarWordType.WhPronoun},
                {"wp$", GrammarWordType.WhPossesivePronoun},
                {"wrb", GrammarWordType.WhAdverb},
                {"`", GrammarWordType.Punctuation},
                {"``", GrammarWordType.Punctuation},
                {",", GrammarWordType.Punctuation},
                {"'", GrammarWordType.Punctuation},
                {"''", GrammarWordType.Punctuation},
                {".", GrammarWordType.Punctuation},
                {":", GrammarWordType.Punctuation},
                {"$", GrammarWordType.Punctuation},
                {"#", GrammarWordType.Punctuation},
                {"-lrb-", GrammarWordType.Punctuation},
                {"-rrb-", GrammarWordType.Punctuation},
            };

        private static readonly Dictionary<string, GrammarPhraseType> _typesToGrammarPhraseTypes =
            new Dictionary<string, GrammarPhraseType> {
                {"s", GrammarPhraseType.Sentence},
                {"adjp", GrammarPhraseType.AdjectivePhrase},
                {"advp", GrammarPhraseType.AdverbPhrase},
                {"conjp", GrammarPhraseType.ConjunctionPhrase},
                {"intj", GrammarPhraseType.Interjection},
                {"lst", GrammarPhraseType.ListMarker},
                {"np", GrammarPhraseType.NounPhrase},
                {"pp", GrammarPhraseType.PrepositionalPhrase},
                {"prt", GrammarPhraseType.Particle},
                {"sbar", GrammarPhraseType.ClauseIntroducedBySubordinatingConjunction},
                {"ucp", GrammarPhraseType.UnlikeCoordinatedPhrase},
                {"vp", GrammarPhraseType.VerbPhrase},
            };

        public GrammarWordType ConvertToWordType(string type) {
            GrammarWordType result;
            return _typesToGrammarWordTypes.TryGetValue(type.ToLowerInvariant(), out result)
                       ? result
                       : GrammarWordType.Unknown;
        }

        public GrammarPhraseType ConvertToPhraseType(string type) {
            GrammarPhraseType result;
            return _typesToGrammarPhraseTypes.TryGetValue(type.ToLowerInvariant(), out result)
                       ? result
                       : GrammarPhraseType.Unknown;
        }
    }
}