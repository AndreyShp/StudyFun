using BusinessLogic.Export;
using BusinessLogic.ExternalData.Representations;

namespace BusinessLogic.Downloaders {
    public class VisualDictionaryDownloader {
        private readonly string _domain;
        private readonly string _fontPath;

        public VisualDictionaryDownloader(string domain, string fontPath) {
            _domain = domain;
            _fontPath = fontPath;
        }

        public DocumentationGenerator Download(DocumentType docType,
                                               string fileName,
                                               RepresentationForUser representationForUser) {
            DocumentationGenerator documentGenerator = DocumentationGenerator.Create(_domain, _fontPath, docType, fileName);

            var tableData = new TableData(2, true);
            tableData.AddHeader("Слово", "Перевод");
            foreach (RepresentationAreaForUser area in representationForUser.Areas) {
                tableData.AddRow(area.Source.Text, area.Translation.Text);
            }

            documentGenerator.AddHeader(string.Format("Визуальный словарь на тему «{0}»", representationForUser.Title));
            documentGenerator.AddTable(tableData);

            return documentGenerator;
        }
    }
}