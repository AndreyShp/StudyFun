using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BusinessLogic.Validators;

namespace BusinessLogic.Export {
    public class TableData {
        private const int ORDER_NUMBER_COLUMN_WIDTH = 50;

        private readonly int _countColumns;
        private readonly List<string> _headers = new List<string>();
        private readonly bool _needNumberOrder;
        private readonly List<List<TableDataCell>> _rows = new List<List<TableDataCell>>();
        private int _rowNumber;

        public TableData(int countColumns, bool needNumberOrder) {
            _countColumns = countColumns;
            _needNumberOrder = needNumberOrder;
            if (_needNumberOrder) {
                _countColumns++;
            }
        }

        public int CountColumns {
            get { return _countColumns; }
        }

        public List<string> Headers {
            get { return _headers; }
        }

        public List<List<TableDataCell>> Rows {
            get { return _rows; }
        }

        public void AddHeader(params string[] headers) {
            if (EnumerableValidator.IsNotEmpty(headers)) {
                AddHeader(headers.ToList());
            }
        }

        public void AddHeader(IEnumerable<string> headers) {
            if (_needNumberOrder) {
                Headers.Add("№");
            }
            Headers.AddRange(headers);
        }

        public void AddRow(params string[] fields) {
            if (EnumerableValidator.IsNotEmpty(fields)) {
                AddRow(fields.Select(TableDataCell.CreateText).ToList());
            }
        }

        public void AddRow(IEnumerable<string> fields) {
            AddRow(fields.Select(TableDataCell.CreateText));
        }

        public void AddRow(IEnumerable<TableDataCell> fields) {
            var rowFields = new List<TableDataCell>();
            if (_needNumberOrder) {
                ++_rowNumber;
                rowFields.Add(TableDataCell.CreateText(_rowNumber.ToString(CultureInfo.InvariantCulture)));
            }
            rowFields.AddRange(fields);
            Rows.Add(rowFields);
        }

        public float[] GetWidths(float totalWidth) {
            if (_countColumns <= 1) {
                return new[] {totalWidth};
            }
            var result = new float[_countColumns];

            int startIndex = 0;
            if (_needNumberOrder) {
                result[0] = ORDER_NUMBER_COLUMN_WIDTH;
                totalWidth -= ORDER_NUMBER_COLUMN_WIDTH;
                startIndex++;
            }

            float columnWidth = totalWidth / (_countColumns - startIndex);
            for (int i = startIndex; i < _countColumns; i++) {
                result[i] = columnWidth;
            }

            return result;
        }
    }
}