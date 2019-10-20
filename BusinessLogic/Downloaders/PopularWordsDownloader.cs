using System.Collections.Generic;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Downloaders {
    public class PopularWordsDownloader {
        private readonly string _domain;
        private readonly string _fontPath;

        public PopularWordsDownloader(string domain, string fontPath) {
            _domain = domain;
            _fontPath = fontPath;
        }

        public string Header { get; set; }

        public DocumentationGenerator Download(DocumentType docType,
                                               string fileName,
                                               List<SourceWithTranslation> words) {
            DocumentationGenerator documentGenerator = DocumentationGenerator.Create(_domain, _fontPath, docType, fileName);

            var tableData = new TableData(2, true);
            tableData.AddHeader("Слово", "Перевод");
            foreach (SourceWithTranslation word in words) {
                tableData.AddRow(word.Source.Text, word.Translation.Text);
            }

            if (string.IsNullOrEmpty(Header)) {
                Header = "Минилекс Гуннемарка";
            }

            documentGenerator.AddHeader(Header);
            documentGenerator.AddTable(tableData);

            return documentGenerator;
        }
    }
}