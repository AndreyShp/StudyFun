using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataQuery.Sentences;
using BusinessLogic.ExternalData;
using BusinessLogic.Validators;
using NLPWrapper.ExternalObjects;
using Sentence = BusinessLogic.Data.Sentence.Sentence;

namespace BusinessLogic.Helpers.SentencesSearchEngines {
    public class SentencesSearchEngine : ISentencesSearchEngine {
        private readonly ISentenceWordsQuery _sentenceWordsQuery;
        private readonly ITextAnalyzer _textAnalyzer;

        internal SentencesSearchEngine(ISentenceWordsQuery sentenceWordsQuery,
                                       ITextAnalyzer textAnalyzer) {
            _sentenceWordsQuery = sentenceWordsQuery;
            _textAnalyzer = textAnalyzer;
        }

        #region ISentencesSearchEngine Members

        public List<string> FindSentences(string words, OrderWordsInSearch orderWordsInSearch) {
            var uniqueWords = new HashSet<string>(words.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            List<Sentence> sentences = _sentenceWordsQuery.FindSentencesByWords(uniqueWords.Select(e => e.Trim()).ToList(),
                                                                                orderWordsInSearch);
            if (EnumerableValidator.IsEmpty(sentences)) {
                NLPWrapper.ExternalObjects.Sentence sentence = _textAnalyzer.ParseSentence(words, false);
                var normalWords = new List<string>();
                foreach (Word word in sentence.Words) {
                    if (EnumerableValidator.IsNotEmpty(word.NormalForms)) {
                        normalWords.AddRange(word.NormalForms);
                    } else {
                        normalWords.Add(word.GetAppropriateWordText());
                    }
                }
                sentences = _sentenceWordsQuery.FindSentencesByWords(normalWords, orderWordsInSearch);
            }
            return sentences.Select(e => e.Text).ToList();
        }

        #endregion
    }
}