using System;
using System.Collections.Generic;
using NLPWrapper.ExternalObjects;

namespace Sandbox.Classes {
    public class TextParser {
        private readonly ITextAnalyzer _textAnalyzer;

        public TextParser() {
            _textAnalyzer = TextAnalyzerFactory.Create();
        }

        public void ParseSentence(string textSentence) {
            Sentence sentence = _textAnalyzer.ParseSentence(textSentence);
            /*PartSentence partSentence = sentence.PartSentence;
            ShowWords(partSentence);
            Console.WriteLine();*/
            ShowSentence(sentence);
        }

        private static void ShowSentence(Sentence sentence) {
            foreach (Word word in sentence.Words) {
                Console.Write(" {0}({1} {2})", word.Text, string.Join(",", word.NormalForms), word.Type);
            }
            Console.WriteLine("\r\n------------");
        }

        public void ParseText(string text) {
            List<Sentence> sentences = _textAnalyzer.ParseText(text);
            for (int i = 0; i < sentences.Count; i++) {
                Console.Write("{0}", i + 1);
                ShowSentence(sentences[i]);
            }
        }

        /*private static void ShowWords(PartSentence partSentence) {
            WriteWords(partSentence);
            foreach (PartSentence childPartSentence in partSentence.Children) {
                ShowWords(childPartSentence);
            }
        }

        private static void WriteWords(PartSentence partSentence) {
            foreach (Word word in partSentence.Words) {
                Console.Write(" {0}({1} {2})", word.Text, string.Join(",", word.NormalForms), word.Type);
            }
        }*/
    }
}