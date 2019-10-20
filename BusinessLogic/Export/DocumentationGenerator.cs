using System.IO;
using BusinessLogic.Export.Pdf;
using BusinessLogic.Validators;

namespace BusinessLogic.Export {
    public class DocumentationGenerator {
        private readonly IFileGenerator _fileGenerator;

        internal DocumentationGenerator(DocumentSettings settings) {
            DocumentType type = EnumValidator.IsValid(settings.Type) ? settings.Type : DocumentType.Pdf;

            if (type == DocumentType.Pdf) {
                FileName = settings.FileName + ".pdf";
                ContentType = "application/pdf";

                _fileGenerator = new PdfGenerator(settings.FontPath, settings.DomainWithProtocol, settings.FileName);
            }

            if (type == DocumentType.Txt) {
                FileName = settings.FileName + ".txt";
                ContentType = "text/plain";

                _fileGenerator = new TxtGenerator(settings.DomainWithProtocol);
            }
        }

        public string FileName { get; private set; }

        public string ContentType { get; private set; }

        public void AddHeader(string header, bool isLargeBottomSpacing = true) {
            _fileGenerator.AddHeader(header, isLargeBottomSpacing);
        }

        public void AddSubheader(string header, bool isLargeBottomSpacing = true) {
            _fileGenerator.AddSubheader(header, isLargeBottomSpacing);
        }

        public void AddParagraph(string text, TextFormat textFormat = null) {
            _fileGenerator.AddParagraph(text, textFormat ?? new TextFormat());
        }

        public void AddTable(TableData tableData) {
            _fileGenerator.AddTable(tableData);
        }

        public Stream Generate() {
            return _fileGenerator.GetStream();
        }

        internal static DocumentationGenerator Create(string domainWithProtocol,
                                                      string fontPath,
                                                      DocumentType type,
                                                      string fileName) {
            var documentSettings = new DocumentSettings {
                DomainWithProtocol = domainWithProtocol,
                FileName = fileName,
                Type = type,
                FontPath = fontPath
            };

            var documentGenerator = new DocumentationGenerator(documentSettings);
            return documentGenerator;
        }
    }
}