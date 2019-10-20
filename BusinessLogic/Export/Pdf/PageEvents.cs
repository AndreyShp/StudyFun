using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BusinessLogic.Export.Pdf {
    public class PageEvents : PdfPageEventHelper {
        private const int FONT_SIZE = 10;
        private const float FIRST_COLUMN_WIDTH = 117;

        private readonly BaseFont _baseFont;
        private readonly string _domain;

        public PageEvents(string domain, BaseFont baseFont) {
            _domain = domain;
            _baseFont = baseFont;
        }

        public override void OnEndPage(PdfWriter writer, Document document) {
            const int COUNT_COLUMNS = 3;

            var footerTable = new PdfPTable(COUNT_COLUMNS);
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            footerTable.HorizontalAlignment = Element.ALIGN_LEFT;

            var columnWith = (footerTable.TotalWidth - FIRST_COLUMN_WIDTH) / (COUNT_COLUMNS - 1);
            footerTable.SetWidths(new[] {
                FIRST_COLUMN_WIDTH, columnWith, columnWith
            });

            var text = new Phrase("Подготовлено сервисом", new Font(_baseFont, FONT_SIZE, Font.NORMAL, BaseColor.GRAY));
            AddCell(text, footerTable, Element.ALIGN_LEFT);

            var link = new Font(_baseFont, FONT_SIZE, Font.UNDERLINE, new BaseColor(0, 0, 255));
            var anchor = new Anchor(_domain, link) {
                Reference = _domain
            };

            AddCell(anchor, footerTable, Element.ALIGN_LEFT);

            text = new Phrase(string.Format("Страница {0}", document.PageNumber),
                              new Font(_baseFont, 8, Font.NORMAL, BaseColor.GRAY));
            AddCell(text, footerTable, Element.ALIGN_RIGHT);
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin, writer.DirectContent);
        }

        private static void AddCell(Phrase text, PdfPTable table, int horizontalAlignment) {
            var cell = new PdfPCell(text) {
                PaddingTop = 10,
                Border = 0,
                BorderWidthTop = 0.5f,
                BorderColorTop = BaseColor.GRAY,
                HorizontalAlignment = horizontalAlignment
            };
            table.AddCell(cell);
        }
    }
}