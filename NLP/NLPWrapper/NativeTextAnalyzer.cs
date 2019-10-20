using System.Collections.Generic;
using System.Linq;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.Coreference.Mention;
using OpenNLP.Tools.Lang.English;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;

namespace NLPWrapper {
    public class NativeTextAnalyzer {
        private static readonly HashSet<char> _firstCharTagForBaseForm = new HashSet<char> {
            'N',
            'n',
            'v',
            'V',
            'J',
            'j',
            'A',
            'a',
            'R',
            'r'
        };

        private readonly string _modelPath;
        private EnglishTreebankChunker _mChunker;
        private TreebankLinker _mCoreferenceFinder;
        private EnglishNameFinder _mNameFinder;
        private EnglishTreebankParser _mParser;
        private EnglishMaximumEntropyPosTagger _mPosTagger;
        private MaximumEntropySentenceDetector _mSentenceDetector;
        private EnglishMaximumEntropyTokenizer _mTokenizer;

        internal NativeTextAnalyzer(string modelPath, string dictionaryPath) {
            _modelPath = modelPath;
            DictionaryFactory.DictionaryPath = dictionaryPath;
        }

        internal string[] SplitSentences(string paragraph) {
            if (_mSentenceDetector == null) {
                _mSentenceDetector = new EnglishMaximumEntropySentenceDetector(_modelPath + "EnglishSD.nbin");
            }

            return _mSentenceDetector.SentenceDetect(paragraph);
        }

        internal string[] TokenizeSentence(string sentence) {
            if (_mTokenizer == null) {
                _mTokenizer = new EnglishMaximumEntropyTokenizer(_modelPath + "EnglishTok.nbin");
            }

            return _mTokenizer.Tokenize(sentence);
        }

        internal string[] PosTagTokens(string[] tokens) {
            if (_mPosTagger == null) {
                _mPosTagger = new EnglishMaximumEntropyPosTagger(_modelPath + "EnglishPOS.nbin",
                                                                 _modelPath + @"\Parser\tagdict");
            }

            return _mPosTagger.Tag(tokens);
        }

        internal string ChunkSentence(string[] tokens, string[] tags) {
            if (_mChunker == null) {
                _mChunker = new EnglishTreebankChunker(_modelPath + "EnglishChunk.nbin");
            }

            return _mChunker.GetChunks(tokens, tags);
        }

        internal Parse ParseSentence(string sentence) {
            if (_mParser == null) {
                _mParser = new EnglishTreebankParser(_modelPath, true, false);
            }

            return _mParser.DoParse(sentence);
        }

        internal string FindNames(string sentence) {
            if (_mNameFinder == null) {
                _mNameFinder = new EnglishNameFinder(_modelPath + "namefind\\");
            }

            var models = new[] {"date", "location", "money", "organization", "percentage", "person", "time"};
            return _mNameFinder.GetNames(models, sentence);
        }

        internal string FindNames(Parse sentenceParse) {
            if (_mNameFinder == null) {
                _mNameFinder = new EnglishNameFinder(_modelPath + "namefind\\");
            }

            var models = new[] {"date", "location", "money", "organization", "percentage", "person", "time"};
            return _mNameFinder.GetNames(models, sentenceParse);
        }

        internal string IdentifyCoreferents(string[] sentences) {
            if (_mCoreferenceFinder == null) {
                _mCoreferenceFinder = new TreebankLinker(_modelPath + "coref");
            }

            var parsedSentences = new List<Parse>();

            foreach (string sentence in sentences) {
                Parse sentenceParse = ParseSentence(sentence);
                string findNames = FindNames(sentenceParse);
                parsedSentences.Add(sentenceParse);
            }
            return _mCoreferenceFinder.GetCoreferenceParse(parsedSentences.ToArray());
        }

        internal bool NeedTryGetBaseForm(string tag) {
            if (string.IsNullOrWhiteSpace(tag)) {
                return false;
            }
            char firstChar = char.ToLowerInvariant(tag[0]);
            return _firstCharTagForBaseForm.Contains(firstChar);
        }

        internal List<string> GetLemmas(string text, string tag) {
            return (DictionaryFactory.GetDictionary().GetLemmas(text, tag) ?? new string[0]).ToList();
        }

        /*private void btnGender_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                string posTaggedSentence = string.Empty;

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    posTaggedSentence += tokens[currentTag] + @"/" + tags[currentTag] + " ";
                }

                output.Append(posTaggedSentence);
                output.Append("\r\n");
                output.Append(GenderModel.GenderMain(mModelPath + "coref\\gen", posTaggedSentence));
                output.Append("\r\n\r\n");
            }

            txtOut.Text = output.ToString();
        }

        private void btnSimilarity_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                string posTaggedSentence = string.Empty;

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    posTaggedSentence += tokens[currentTag] + @"/" + tags[currentTag] + " ";
                }

                output.Append(posTaggedSentence);
                output.Append("\r\n");
                output.Append(SimilarityModel.SimilarityMain(mModelPath + "coref\\sim", posTaggedSentence));
                output.Append("\r\n\r\n");
            }

            txtOut.Text = output.ToString();
        }

        private void btnCoreference_Click(object sender, EventArgs e)
        {
            string[] sentences = SplitSentences(txtIn.Text);

            txtOut.Text = IdentifyCoreferents(sentences);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    string tag = tags[currentTag];
                    string token = tokens[currentTag];

                    var baseForms = DictionaryFactory.GetDictionary().GetLemmas(token, tag.ToLowerInvariant());

                    output.Append(token).Append("/").Append(tag);
                    if (baseForms.Length > 1 || (baseForms.Length == 1 && baseForms[0] != token))
                    {
                        output.AppendFormat(" - {0}", string.Join(",", baseForms));
                    }
                    output.Append(" ");
                }

                output.Append("\r\n\r\n");
            }

            txtOut.Text = output.ToString();
        }

        #region Button click events

        private void btnSplit_Click(object sender, EventArgs e)
        {
            string[] sentences = SplitSentences(txtIn.Text);

            txtOut.Text = string.Join("\r\n\r\n", sentences);
        }

        private void btnTokenize_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                output.Append(string.Join(" | ", tokens)).Append("\r\n\r\n");
            }

            txtOut.Text = output.ToString();
        }

        private void btnPOSTag_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    output.Append(tokens[currentTag]).Append("/").Append(tags[currentTag]).Append(" ");
                }

                output.Append("\r\n\r\n");
            }

            txtOut.Text = output.ToString();
        }

        private void btnChunk_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                output.Append(ChunkSentence(tokens, tags)).Append("\r\n");
            }

            txtOut.Text = output.ToString();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                output.Append(ParseSentence(sentence).Show()).Append("\r\n\r\n");
            }

            txtOut.Text = output.ToString();
        }

        private void btnNameFind_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(txtIn.Text);

            foreach (string sentence in sentences)
            {
                output.Append(FindNames(sentence)).Append("\r\n");
            }

            txtOut.Text = output.ToString();
        }*/
    }
}