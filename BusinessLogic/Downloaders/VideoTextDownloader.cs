using System.Collections.Generic;
using BusinessLogic.Export;
using BusinessLogic.ExternalData.Videos;

namespace BusinessLogic.Downloaders {
    public class VideoTextDownloader {
        private readonly string _domain;
        private readonly string _fontPath;

        public VideoTextDownloader(string domain, string fontPath) {
            _domain = domain;
            _fontPath = fontPath;
        }

        public DocumentationGenerator Download(DocumentType docType,
                                               string fileName,
                                               VideoForUser video) {
            DocumentationGenerator documentGenerator = DocumentationGenerator.Create(_domain, _fontPath, docType, fileName);

            var fields = new List<string> {"Отрывок текста"};
            if (video.HasAnyTranslation) {
                fields.Add("Перевод");
            }
            var tableData = new TableData(fields.Count, false);
            tableData.AddHeader(fields);
            foreach (var sentence in video.Sentences) {
                fields = new List<string> {sentence.Item1};
                if (video.HasTranslation(sentence)) {
                    fields.Add(sentence.Item2);
                }
                tableData.AddRow(fields);
            }

            documentGenerator.AddHeader(string.Format("Текст из видео «{0}»", video.Title));
            documentGenerator.AddTable(tableData);

            return documentGenerator;
        }
    }
}