using System;
using System.IO;
using System.Text;

namespace Sandbox.Classes {
    public class CsvWriter : IDisposable {
        private StreamWriter _writer;

        public CsvWriter(string fileName) : this(fileName, Encoding.UTF8) {}

        public CsvWriter(string fileName, Encoding encoding) {
            _writer = new StreamWriter(fileName, false, encoding);
        }

        #region IDisposable Members

        public void Dispose() {
            Close();
        }

        #endregion

        public void WriteLine(params string[] values) {
            string line = string.Join(";", values);
            WriteLine(line);
        }

        public void WriteLine(string line) {
            if (_writer == null) {
                return;
            }
            _writer.WriteLine(line);
        }

        private void Close() {
            if (_writer != null) {
                _writer.Close();
                _writer = null;
            }
        }
    }
}