using System.IO;

namespace BusinessLogic.Export {
    public interface IFileGenerator {
        void AddHeader(string header, bool isLargeBottomSpacing);

        void AddSubheader(string header, bool isLargeBottomSpacing);

        void AddTable(TableData tableData);

        void AddParagraph(string text, TextFormat textFormat);

        MemoryStream GetStream();
    }
}