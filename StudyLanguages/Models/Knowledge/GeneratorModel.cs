using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;
using BusinessLogic.Validators;
using StudyLanguages.Helpers;

namespace StudyLanguages.Models.Knowledge {
    public class GeneratorModel {
        public GeneratorModel(ControllerContext controllerContext,
                              Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>> items) {
            HtmlItems = new List<string>();
            IEnumerable<GeneratedKnowledgeItem> generatedWords = GetGeneratedItems(KnowledgeDataType.WordTranslation,
                                                                                   items);
            List<SourceWithTranslation> words =
                generatedWords.Select(e => (SourceWithTranslation) e.ParsedData).ToList();
            if (EnumerableValidator.IsNotNullAndNotEmpty(words)) {
                HtmlItems.Add(GetHtml(controllerContext, KnowledgeDataType.WordTranslation, words));
            }

            IEnumerable<GeneratedKnowledgeItem> generatedPhrases = GetGeneratedItems(
                KnowledgeDataType.PhraseTranslation, items);
            List<SourceWithTranslation> phrases =
                generatedPhrases.Select(e => (SourceWithTranslation) e.ParsedData).ToList();
            if (EnumerableValidator.IsNotNullAndNotEmpty(phrases)) {
                HtmlItems.Add(GetHtml(controllerContext, KnowledgeDataType.PhraseTranslation, phrases));
            }

            IEnumerable<GeneratedKnowledgeItem> generatedSentences =
                GetGeneratedItems(KnowledgeDataType.SentenceTranslation, items);
            List<SourceWithTranslation> sentences =
                generatedSentences.Select(e => (SourceWithTranslation) e.ParsedData).ToList();
            if (EnumerableValidator.IsNotNullAndNotEmpty(sentences)) {
                HtmlItems.Add(GetHtml(controllerContext, KnowledgeDataType.SentenceTranslation, sentences));
            }
        }

        public List<string> HtmlItems { get; set; }

        private static string GetHtml(ControllerContext controllerContext,
                               KnowledgeDataType knowledgeDataType,
                               List<SourceWithTranslation> items) {
            return OurHtmlHelper.RenderRazorViewToString(controllerContext, "PartialKnowledgeItems",
                                                         new Tuple<KnowledgeDataType, List<SourceWithTranslation>>(
                                                             knowledgeDataType, items));
        }

        private static IEnumerable<GeneratedKnowledgeItem> GetGeneratedItems(KnowledgeDataType dataType,
                                                                             Dictionary
                                                                                 <KnowledgeDataType,
                                                                                 List<GeneratedKnowledgeItem>> items) {
            List<GeneratedKnowledgeItem> generatedItems;
            if (!items.TryGetValue(dataType, out generatedItems)) {
                generatedItems = new List<GeneratedKnowledgeItem>();
            }
            return generatedItems;
        }
    }
}