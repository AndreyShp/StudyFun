using System;
using System.Collections.Generic;
using System.Linq;
using NLPWrapper.ExternalObjects.Enums;
using OpenNLP.Tools.Parser;

namespace NLPWrapper.ExternalObjects {
    public class TextAnalyzer : ITextAnalyzer {
        private static readonly TypesConverter _typeConverter = new TypesConverter();
        private static readonly FullWordForms _fullWordForms = new FullWordForms();
        private readonly NativeTextAnalyzer _nativeTextAnalyzer;
        private readonly TextCleaner _textCleaner = new TextCleaner();

        //TODO: вынести в БД
        private readonly HashSet<string> _specialWords = new HashSet<string> {
            "january",
            "february",
            "march",
            "april",
            "may",
            "june",
            "july",
            "august",
            "september",
            "october",
            "november",
            "december",
            "monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday",
            "spring",
            "summer",
            "autumn",
            "winter"
        };

        internal TextAnalyzer(NativeTextAnalyzer nativeTextAnalyzer) {
            _nativeTextAnalyzer = nativeTextAnalyzer;
        }

        #region ITextAnalyzer Members

        public Sentence ParseSentence(string sentence, bool needClear = true) {
            var cleanSentence = needClear ? _textCleaner.Clear(sentence) : sentence;
            Parse parse = _nativeTextAnalyzer.ParseSentence(cleanSentence);
            Sentence result = ConvertToSentence(parse);
            result.Text = cleanSentence;
            return result;
        }

        public List<Sentence> ParseText(string text) {
            var cleanText = _textCleaner.Clear(text);
            string[] sentences = _nativeTextAnalyzer.SplitSentences(cleanText);
            return sentences.Select(s => ParseSentence(s, false)).ToList();
        }

        #endregion

        private Sentence ConvertToSentence(Parse parse) {
            if (parse.Type == MaximumEntropyParser.TopNode) {
                parse = parse.GetChildren()[0];
            }

            PartSentence parseSentence = ConvertParseToSentence(parse);
            var result = new Sentence(parseSentence);
            if (!parse.IsPosTag) {
                AddChildNodes(result, parseSentence, parse.GetChildren());
            } else {
                //внутри есть только слова
                Word word = ConvertToWord(result, parse);
                parseSentence.AddWord(word);
            }
            return result;
        }

        private static PartSentence ConvertParseToSentence(Parse parse) {
            int start = parse.Span.Start;
            int end = parse.Span.End;
            var parseSentence = new PartSentence {
                StartIndex = start,
                EndIndex = end,
                Type = _typeConverter.ConvertToPhraseType(parse.Type),
                Text = GetPartText(parse, start, end)
            };
            return parseSentence;
        }

        private void AddChildNodes(Sentence sentence, PartSentence partSentence, IEnumerable<Parse> childParses) {
            foreach (Parse childParse in childParses) {
                if (childParse.Type == MaximumEntropyParser.TokenNode) {
                    continue;
                }

                if (childParse.IsPosTag) {
                    //внутри есть только слова
                    Word word = ConvertToWord(sentence, childParse);
                    partSentence.AddWord(word);
                    continue;
                }

                //есть вложенные части предложений - обработать их
                PartSentence childPartSentence = ConvertParseToSentence(childParse);
                childPartSentence.Parent = partSentence;
                partSentence.AddChildren(childPartSentence);
                AddChildNodes(sentence, childPartSentence, childParse.GetChildren());
            }
        }

        private Word ConvertToWord(Sentence sentence, Parse parse) {
            int start = parse.Span.Start;
            int end = parse.Span.End;
            string text = GetPartText(parse, start, end);

            string dirtyType = parse.Type;
            GrammarWordType grammarWordType = _typeConverter.ConvertToWordType(dirtyType);
            var result = new Word(text, grammarWordType);

            string fullText = GetFullWord(text, grammarWordType);

            if (_nativeTextAnalyzer.NeedTryGetBaseForm(dirtyType)
                && !_specialWords.Contains(fullText.ToLowerInvariant())) {
                result.NormalForms = _nativeTextAnalyzer.GetLemmas(fullText, dirtyType.ToLowerInvariant());
            }

            if (!string.Equals(fullText, text, StringComparison.InvariantCultureIgnoreCase)) {
                result.FullText = fullText;
            }

            sentence.AddWord(result);
            return result;
        }

        private static string GetPartText(Parse parse, int start, int end) {
            return parse.Text.Substring(start, end - start);
        }

        private static string GetFullWord(string word, GrammarWordType grammarType) {
            string result;
            if (!_fullWordForms.TryGetFullForm(grammarType, word, out result)) {
                result = word;
            }
            return result;
        }
    }
}