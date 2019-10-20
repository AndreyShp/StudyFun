using System;
using com.sun.tools.javac.util;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.process;
using edu.stanford.nlp.trees;
using java.io;
using Console = System.Console;

namespace Sandbox.Classes.NLP {
    public class TextAnalyzer {
        public static void DemoDP(LexicalizedParser lp, string fileName) {
            // This option shows loading and sentence-segment and tokenizing
            // a file using DocumentPreprocessor
            var tlp = new PennTreebankLanguagePack();
            GrammaticalStructureFactory gsf = tlp.grammaticalStructureFactory();
            // You could also create a tokenizer here (as below) and pass it
            // to DocumentPreprocessor
            foreach (List sentence in new DocumentPreprocessor(fileName)) {
                Tree parse = lp.apply(sentence);
                parse.pennPrint();

                GrammaticalStructure gs = gsf.newGrammaticalStructure(parse);
                java.util.List tdl = gs.typedDependenciesCCprocessed(true);
                Console.WriteLine("\n{0}\n", tdl);
            }
        }

        public static void DemoAPI(LexicalizedParser lp) {
            // This option shows parsing a list of correctly tokenized words
            var sent = new[] {"This", "is", "an", "easy", "sentence", "."};
            java.util.List rawWords = Sentence.toCoreLabelList(sent);
            Tree parse = lp.apply(rawWords);
            parse.pennPrint();

            // This option shows loading and using an explicit tokenizer
            const string Sent2 = "This is another sentence.";
            TokenizerFactory tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var sent2Reader = new StringReader(Sent2);
            java.util.List rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
            parse = lp.apply(rawWords2);

            var tlp = new PennTreebankLanguagePack();
            GrammaticalStructureFactory gsf = tlp.grammaticalStructureFactory();
            GrammaticalStructure gs = gsf.newGrammaticalStructure(parse);
            java.util.List tdl = gs.typedDependenciesCCprocessed();
            Console.WriteLine("\n{0}\n", tdl);

            var tp = new TreePrint("penn,typedDependenciesCollapsed");
            tp.printTree(parse);
        }

        public static void Start(string fileName) {
            LexicalizedParser lp = LexicalizedParser.loadModel( /*Program.ParserModel*/);
            if (!String.IsNullOrEmpty(fileName)) {
                DemoDP(lp, fileName);
            } else {
                DemoAPI(lp);
            }
        }
    }
}