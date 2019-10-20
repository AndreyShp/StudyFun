using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BusinessLogic.Export.Pdf {
    public class PdfGenerator : IFileGenerator {
        private const int MARGIN = 36;

        private readonly BaseFont _baseFont;
        private readonly BaseColor _borderColor = new BaseColor(221, 221, 221);

        private readonly string _domain;
        //private readonly string _logoPath;
        private readonly string _subject;
        //TODO: вытащить логику по работе с таблицами, ячейками в отдельный класс передавать этот класс в PageEvents
        private Document _doc;
        private bool _hasContent;
        private MemoryStream _stream;
        private PdfWriter _writer;

        public PdfGenerator(string fontPath, /*string logoPath,*/ string domain, string subject) {
            _baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //_logoPath = logoPath;
            _domain = domain;
            _subject = subject;
        }

        #region IFileGenerator Members

        public void AddParagraph(string text, TextFormat textFormat) {
            CreateDocument();

            Font font = CreateFont();
            if (textFormat.IsBold || textFormat.IsItalic) {
                int style;
                if (textFormat.IsBold && textFormat.IsItalic) {
                    style = Font.BOLDITALIC;
                } else if (textFormat.IsBold) {
                    style = Font.BOLD;
                } else {
                    style = Font.ITALIC;
                }
                font.SetStyle(style);
            }

            var paragraph = new Paragraph(text, font);
            paragraph.Alignment = Element.ALIGN_LEFT;
            paragraph.SpacingBefore = _hasContent ? 10 : 0;

            if (textFormat.CountLeftPaddings > 0) {
                paragraph.IndentationLeft = textFormat.CountLeftPaddings * 15;
            }

            _doc.Add(paragraph);
            _hasContent = true;
        }

        public MemoryStream GetStream() {
            _writer.CloseStream = false;
            _writer.Flush();
            _doc.Close();
            _doc.Dispose();

            _stream.Seek(0, SeekOrigin.Begin);
            return _stream;
        }

        public void AddHeader(string header, bool isLargeBottomSpacing) {
            var settings = new PdfTextSettings {
                Align = Element.ALIGN_CENTER,
                TextSize = 24,
                SpacingBefore = _hasContent ? 10 : 0,
                SpacingAfter = isLargeBottomSpacing ? 30 : 15
            };
            AddHeader(header, settings);
        }

        public void AddSubheader(string header, bool isLargeBottomSpacing) {
            var settings = new PdfTextSettings {
                Align = Element.ALIGN_CENTER,
                TextSize = 18,
                SpacingBefore = _hasContent ? 15 : 0,
                SpacingAfter = isLargeBottomSpacing ? 15 : 10
            };
            AddHeader(header, settings);
        }

        public void AddTable(TableData tableData) {
            AddTable(tableData, true);
        }

        #endregion

        public byte[] GetAsBytes() {
            using (MemoryStream stream = GetStream()) {
                return stream.ToArray();
            }
        }

        public void AddTable(TableData tableData, bool needPaint) {
            CreateDocument();

            var table = new PdfPTable(tableData.CountColumns) {WidthPercentage = 100};
            float width = GetPageWidth();
            float[] widths = tableData.GetWidths(width);
            table.SetWidths(widths);

            AddHeaderToTable(tableData.Headers, table);

            for (int i = 0; i < tableData.Rows.Count; i++) {
                List<TableDataCell> fields = tableData.Rows[i];
                AddRowToTable(fields, needPaint && i % 2 == 0, table);
            }

            _doc.Add(table);
            _hasContent = true;
        }

        private float GetPageWidth() {
            float width = _doc.PageSize.Width - _doc.LeftMargin - _doc.RightMargin;
            return width;
        }

        public void AddImage(byte[] image, bool isLargeBottomSpacing) {
            CreateDocument();

            Image img = BytesToImage(image);
            float width = GetPageWidth();
            ScaleImage(width, img);
            img.Alignment = Element.ALIGN_CENTER;
            img.SpacingBefore = _hasContent ? 15 : 0;
            img.SpacingAfter = isLargeBottomSpacing ? 15 : 10;

            _doc.Add(img);
        }

        private static void ScaleImage(float maxSize, Image img) {
            float height;
            float width = maxSize;
            if (width < img.Width) {
                //масштабируем высоту
                height = width / img.Width * img.Height;
            } else {
                width = img.Width;
                height = img.Height;
            }
            if (height > maxSize) {
                //слишком большая по высоте нужно масштабировать
                width = maxSize / height * width;
                height = maxSize;
            }
            img.ScaleAbsolute(width, height);
        }

        private static Image BytesToImage(byte[] image) {
            return Image.GetInstance(image);
        }

        private void AddHeader(string header, PdfTextSettings settings) {
            CreateDocument();

            Font font = CreateFont(settings.TextSize);

            var paragraph = new Paragraph(header, font);
            paragraph.Alignment = settings.Align;
            paragraph.SpacingBefore = settings.SpacingBefore;
            paragraph.SpacingAfter = settings.SpacingAfter;

            _doc.Add(paragraph);
        }

        private void CreateDocument() {
            if (_doc != null) {
                return;
            }

            _doc = new Document(PageSize.A4, MARGIN, MARGIN, MARGIN, MARGIN);
            _doc.AddCreator(_domain);
            _doc.AddAuthor(_domain);
            _doc.AddSubject(_subject);
            _doc.AddCreationDate();

            _stream = new MemoryStream();
            _writer = PdfWriter.GetInstance(_doc, _stream);
            _writer.PageEvent = new PageEvents(_domain, _baseFont);
            _doc.Open();

            /*var logoTable = new PdfPTable(2);
            logoTable.HorizontalAlignment = Element.ALIGN_LEFT;
            logoTable.SetWidths(new[] {5, 100});

            Image logoImage = Image.GetInstance(_logoPath);
            logoImage.ScalePercent(60);
            var iconCell = new PdfPCell(logoImage) {Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE};
            logoTable.AddCell(iconCell);

            var logoText = new Phrase("Study fun", CreateFont(18));
            var logoTextCell = new PdfPCell(logoText) {Border = 0};
            logoTable.AddCell(logoTextCell);

            _doc.Add(logoTable);*/
        }

        public void NewPage() {
            CreateDocument();
            _doc.NewPage();
        }

        private void AddHeaderToTable(IEnumerable<string> fields, PdfPTable table) {
            Font font = CreateFont(fontStyle: Font.BOLD);
            foreach (string header in fields) {
                PdfPCell cell = CreateTextCell(header, font, Element.ALIGN_LEFT);
                cell.BorderColorBottom = _borderColor;
                cell.BorderWidthBottom = 0.5f;

                table.AddCell(cell);
            }
        }

        private void AddRowToTable(List<TableDataCell> fields,
                                   bool needPaint,
                                   PdfPTable table) {
            Font font = CreateFont();
            for (int i = 0; i < fields.Count; i++) {
                PdfPCell cell;
                TableDataCell field = fields[i];
                if (field.IsImage) {
                    byte[] img = field.GetImage();
                    cell = CreateImageCell(img, Element.ALIGN_LEFT, 140);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.UseVariableBorders = true;
                    cell.BorderWidthBottom = 0;
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthRight = 0;
                    cell.PaddingTop = 5;
                } else {
                    string textValue = field.GetText();
                    cell = CreateTextCell(textValue, font, Element.ALIGN_LEFT);
                }
                cell.PaddingBottom = 5;
                cell.PaddingLeft = 5;
                cell.PaddingRight = 5;
                cell.BorderColorTop = _borderColor;
                cell.BorderWidthTop = 0.5f;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                if (needPaint) {
                    cell.BackgroundColor = new BaseColor(249, 249, 249);
                }
                table.AddCell(cell);
            }
        }

        private static PdfPCell CreateTextCell(string text, Font font, int horizontalAlignment) {
            var phrase = new Phrase(text, font);
            var result = new PdfPCell(phrase) {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = Element.ALIGN_TOP,
                MinimumHeight = 22.0f,
                Border = 0,
                NoWrap = false,
            };
            return result;
        }

        private static PdfPCell CreateImageCell(byte[] image, int horizontalAlignment, float maxWidth) {
            Image img = BytesToImage(image);
            img.Alignment = horizontalAlignment;
            var result = new PdfPCell(img) {
                HorizontalAlignment = horizontalAlignment,
            };
            ScaleImage(maxWidth, img);
            return result;
        }

        private Font CreateFont(int size = 14, int fontStyle = Font.NORMAL) {
            return new Font(_baseFont, size, fontStyle, new BaseColor(51, 51, 51));
        }
    }
}