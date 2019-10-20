using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BusinessLogic.Export {
    public class TxtGenerator : IDisposable, IFileGenerator {
        private readonly string _domain;
        private bool _isEmpty = true;
        private MemoryStream _ms;
        private StreamWriter _writer;

        public TxtGenerator(string domain) : this(domain, Encoding.UTF8) {}

        private TxtGenerator(string domain, Encoding encoding) {
            _domain = domain;
            _ms = new MemoryStream();
            _writer = new StreamWriter(_ms, encoding);
        }

        #region IDisposable Members

        public void Dispose() {
            Close();
        }

        #endregion

        #region IFileGenerator Members

        public void AddHeader(string header, bool isLargeBottomSpacing) {
            WriteLine(header);
        }

        public void AddSubheader(string header, bool isLargeBottomSpacing) {
            WriteLine(header);
        }

        public void AddTable(TableData tableData) {
            foreach (var fields in tableData.Rows) {
                string row = string.Join(";", fields.Select(e => e.GetText()));
                WriteLine(row);
            }
        }

        public void AddParagraph(string text, TextFormat textFormat) {
            if (textFormat.CountLeftPaddings > 0) {
                text = string.Join(string.Empty, Enumerable.Repeat("\t", textFormat.CountLeftPaddings)) + text;
            }
            WriteLine(text);
        }

        public MemoryStream GetStream() {
            if (!_isEmpty) {
                WriteLine("Подготовлено сервисом " + _domain);
            }
            _writer.Flush();
            _ms.Position = 0;
            _ms.Flush();
            return _ms;
        }

        #endregion

        public void WriteLine(string line) {
            if (!_isEmpty) {
                _writer.WriteLine();
            }
            _writer.Write(line);
            _isEmpty = false;
        }

        private void Close() {
            if (_writer == null) {
                return;
            }

            _writer.Close();
            _writer = null;

            _ms.Close();
            _ms = null;
        }
    }
}