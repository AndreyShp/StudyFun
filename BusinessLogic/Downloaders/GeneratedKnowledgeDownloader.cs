using System;
using System.Collections.Generic;
using BusinessLogic.Data.Enums.Knowledge;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Knowledge;

namespace BusinessLogic.Downloaders {
    public class GeneratedKnowledgeDownloader {
        private static readonly Dictionary<KnowledgeDataType, Tuple<string, string>> _headers =
            new Dictionary<KnowledgeDataType, Tuple<string, string>> {
                {KnowledgeDataType.WordTranslation, new Tuple<string, string>("Слова", "Слово")},
                {KnowledgeDataType.PhraseTranslation, new Tuple<string, string>("Фразы", "Фраза")},
                {KnowledgeDataType.SentenceTranslation, new Tuple<string, string>("Предложения", "Предложение")},
            };
        private readonly string _domain;
        private readonly string _fontPath;

        public GeneratedKnowledgeDownloader(string domain, string fontPath) {
            _domain = domain;
            _fontPath = fontPath;
        }

        public string Header { get; set; }

        public static string GetHeader(KnowledgeDataType knowledgeDataType) {
            Tuple<string, string> tuple = GetTuple(knowledgeDataType);
            return tuple != null ? tuple.Item1 : null;
        }

        public static string GetTableHeader(KnowledgeDataType knowledgeDataType) {
            Tuple<string, string> tuple = GetTuple(knowledgeDataType);
            return tuple != null ? tuple.Item2 : null;
        }

        private static Tuple<string, string> GetTuple(KnowledgeDataType knowledgeDataType) {
            return _headers.ContainsKey(knowledgeDataType) ? _headers[knowledgeDataType] : null;
        }

        public DocumentationGenerator Download(DocumentType docType,
                                               string fileName,
                                               Dictionary<KnowledgeDataType, List<GeneratedKnowledgeItem>>
                                                   generatedItems) {
            DocumentationGenerator documentGenerator = DocumentationGenerator.Create(_domain, _fontPath, docType,
                                                                                     fileName);

            var tables = new Dictionary<string, TableData>();
            foreach (KnowledgeDataType knowledgeDataType in generatedItems.Keys) {
                var tableData = new TableData(2, true);

                string tableHeader = GetTableHeader(knowledgeDataType);
                if (!string.IsNullOrEmpty(tableHeader)) {
                    tableData.AddHeader(tableHeader, "Перевод");

                    string subHeader = GetHeader(knowledgeDataType);
                    tables.Add(subHeader, tableData);
                }

                List<GeneratedKnowledgeItem> items = generatedItems[knowledgeDataType];
                foreach (GeneratedKnowledgeItem item in items) {
                    var sourceTranslation = (SourceWithTranslation) item.ParsedData;
                    tableData.AddRow(sourceTranslation.Source.Text, sourceTranslation.Translation.Text);
                }
            }

            if (string.IsNullOrEmpty(Header)) {
                Header = "Генератор знаний";
            }

            documentGenerator.AddHeader(Header, false);
            foreach (var pair in tables) {
                documentGenerator.AddHeader(pair.Key);
                documentGenerator.AddTable(pair.Value);
            }

            return documentGenerator;
        }
    }
}