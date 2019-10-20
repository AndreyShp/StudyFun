using System.Collections.Generic;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;

namespace BusinessLogic.Downloaders {
    public class GroupDataDownloader {
        private readonly string _domain;
        private readonly string _fontPath;

        public GroupDataDownloader(string domain, string fontPath) {
            _domain = domain;
            _fontPath = fontPath;
        }

        public string Header { get; set; }

        public string TableHeader { get; set; }
        
        public DocumentationGenerator Download(DocumentType docType,
                                               string fileName,
                                               List<SourceWithTranslation> elemsWithTranslations) {
            DocumentationGenerator documentGenerator = DocumentationGenerator.Create(_domain, _fontPath, docType,
                                                                                     fileName);

            var tableData = new TableData(2, true);
            tableData.AddHeader(TableHeader, "Перевод");
            foreach (SourceWithTranslation elem in elemsWithTranslations) {
                tableData.AddRow(elem.Source.Text, elem.Translation.Text);
            }

            documentGenerator.AddHeader(Header);
            documentGenerator.AddTable(tableData);

            return documentGenerator;
        }
    }
}