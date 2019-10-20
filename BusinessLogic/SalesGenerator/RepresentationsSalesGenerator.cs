using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BusinessLogic.DataQuery;
using BusinessLogic.DataQuery.Representations;
using BusinessLogic.Export;
using BusinessLogic.Export.Pdf;
using BusinessLogic.ExternalData;
using BusinessLogic.ExternalData.Representations;
using BusinessLogic.Helpers;
using BusinessLogic.Helpers.Caches;
using BusinessLogic.SalesGenerator.Colors;
using Point = BusinessLogic.ExternalData.Representations.Point;

namespace BusinessLogic.SalesGenerator {
    public class RepresentationsSalesGenerator {
        private readonly DiskCache _cache;
        private readonly string _domain;
        private readonly string _fontPath;
        private readonly ZipCompressor _zipCompressor;

        public RepresentationsSalesGenerator(string domain, string fontPath, DiskCache cache) {
            _domain = domain;
            _fontPath = fontPath;
            _cache = cache;

            _zipCompressor = new ZipCompressor();
        }

        public byte[] Generate(LanguageShortName from, LanguageShortName to, HashSet<long> visualDictionariesIds) {
            var languagesQuery = new LanguagesQuery(from, to);
            UserLanguages userLanguages = languagesQuery.GetLanguagesByShortNames(from, to);
            var representationsQuery = new RepresentationsQuery(userLanguages.From.Id);

            List<RepresentationForUser> allRepresentations = representationsQuery.GetBought(userLanguages,
                                                                                            visualDictionariesIds);

            PdfGenerator commonPdfGenerator = allRepresentations.Count > 1
                                                  ? new PdfGenerator(_fontPath, _domain, "Все визуальные словари")
                                                  : null;

            var zipKey = GetZipKey(allRepresentations, @from, to);
            var result = _cache.Get(zipKey);
            if (result != null) {
                return result;
            }

            var painter = new Painter();
            var partImageCreator = new PartImageCreator(painter);
            foreach (RepresentationForUser representation in allRepresentations) {
                var tableData = new TableData(3, true);
                tableData.AddHeader(string.Empty, "Слово", "Перевод");

                string title = representation.Title;
                /*if (pictureName != "Лицо" /*&& pictureName != "Человек"#1#) {
                    continue;
                }*/

                var byteStream = new MemoryStream(representation.Image);
                Image image = Image.FromStream(byteStream);
                var wordsWriter = new WordsWriter(painter, image.Width, image.Height);

                foreach (RepresentationAreaForUser area in representation.Areas) {
                    Point leftCorner = area.LeftUpperCorner;
                    Point rightCorner = area.RightBottomCorner;

                    string partKey = representation.Id + "_part_" + area.Id + "_"
                                     + representation.SortInfo.LastModified.Ticks + "_" + from + "_" + to + ".jpeg";
                    byte[] partImageBytes = _cache.Get(partKey);
                    if (partImageBytes == null) {
                        PartImageData partImageData = partImageCreator.CutImage(leftCorner, rightCorner, image);

                        partImageBytes = ImageToBytes(partImageData.Bitmap);
                        WriteToCache(partKey, partImageBytes);
                    }
                    tableData.AddRow(new[] {
                        TableDataCell.CreateImage(partImageBytes), TableDataCell.CreateText(area.Source.Text),
                        TableDataCell.CreateText(area.Translation.Text)
                    });

                    wordsWriter.AddRectangle(leftCorner, rightCorner, area);
                }

                string fullKey = representation.Id + "_" + representation.SortInfo.LastModified.Ticks + "_" + from + "_" + to + ".jpeg";

                byte[] imageResult = _cache.Get(fullKey);
                if (imageResult == null) {
                    Image imageWithSign = wordsWriter.GetImageWithSigns(image);
                    imageResult = ImageToBytes(imageWithSign);

                    WriteToCache(fullKey, imageResult);
                }

                var pdfFileName = title + ".pdf";
                string pdfKey = representation.Id + "_" + representation.SortInfo.LastModified.Ticks + "_" + from + "_" + to + "_" + pdfFileName;
                byte[] pdfContent = _cache.Get(pdfKey);
                
                if (pdfContent == null) {
                    var pdfGenerator = new PdfGenerator(_fontPath, _domain, string.Format(
                        "Визуальный словарь на тему {0}",
                        title));
                    WriteDataToPdf(pdfGenerator, title, imageResult, tableData);
                    pdfContent = pdfGenerator.GetAsBytes();
                    WriteToCache(pdfKey, pdfContent);
                }
                _zipCompressor.AddFileToArchive(pdfFileName, pdfContent);

                if (commonPdfGenerator != null) {
                    WriteDataToPdf(commonPdfGenerator, title, imageResult, tableData);
                    commonPdfGenerator.NewPage();
                }
            }
            if (commonPdfGenerator != null) {
                WritePdfToArchive(commonPdfGenerator, "Всё в одном файле.pdf");
            }
            
            result = _zipCompressor.GetArchive();
            WriteToCache(zipKey, result);
            return result;
        }

        private static string GetZipKey(List<RepresentationForUser> allRepresentations, LanguageShortName from, LanguageShortName to) {
            var keyFromIds = string.Join("_", allRepresentations.OrderBy(e => e.Id).Select(e => e.Id)) + "_"
                             + allRepresentations.Max(e => e.SortInfo.LastModified).Ticks;
            var md5Helper = new Md5Helper();
            return "VisualDictionaries_" + md5Helper.GetHash(keyFromIds) + "_" + from + "_" + to + ".zip";
        }

        private void WriteToCache(string key, byte[] data) {
            _cache.Save(key, data);
        }

        private static void WriteDataToPdf(PdfGenerator pdfGenerator,
                                           string title,
                                           byte[] imageResult,
                                           TableData tableData) {
            pdfGenerator.AddHeader(string.Format("Визуальный словарь на тему «{0}»", title), false);

            pdfGenerator.AddImage(imageResult, true);
            pdfGenerator.NewPage();
            pdfGenerator.AddTable(tableData, false);
        }

        private static byte[] ImageToBytes(Image image) {
            var outputImage = new MemoryStream();
            image.Save(outputImage, ImageFormat.Jpeg);
            outputImage.Seek(0, SeekOrigin.Begin);
            byte[] result = outputImage.ToArray();
            return result;
        }

        private void WritePdfToArchive(PdfGenerator pdfGenerator, string fileName) {
            using (Stream pdfStream = pdfGenerator.GetStream()) {
                _zipCompressor.AddFileToArchive(fileName, pdfStream);
            }
        }
    }
}