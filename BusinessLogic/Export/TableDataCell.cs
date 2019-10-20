namespace BusinessLogic.Export {
    public class TableDataCell {
        private readonly object _value;

        private TableDataCell(object value, bool isImage) {
            _value = value;
            IsImage = isImage;
        }

        public bool IsImage { get; set; }

        public string GetText() {
            return _value.ToString();
        }

        public byte[] GetImage() {
            return ((byte[]) _value);
        }

        public static TableDataCell CreateText(string value) {
            return new TableDataCell(value, false);
        }

        public static TableDataCell CreateImage(byte[] value) {
            return new TableDataCell(value, true);
        }
    }
}