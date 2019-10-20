using System;
using System.IO;
using System.Text;

namespace Sandbox.Classes {
    public class CsvReader : IDisposable {
        private StreamReader _reader;

        public CsvReader(string fileName) : this(fileName, Encoding.UTF8) {}

        public CsvReader(string fileName, Encoding encoding) {
            _reader = new StreamReader(fileName, encoding);
        }

        #region IDisposable Members

        public void Dispose() {
            Close();
        }

        #endregion

        public string[] ReadLine() {
            if (_reader == null) {
                return null;
            }
            string result = _reader.ReadLine();
            if (result != null) {
                return result.Split(new[] {';'});
            }
            //конец файла достигнут
            Close();
            return null;
        }

        private void Close() {
            if (_reader != null) {
                _reader.Close();
                _reader = null;
            }
        }
    }
}