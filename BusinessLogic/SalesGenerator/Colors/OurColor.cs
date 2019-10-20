using System;
using System.Drawing;

namespace BusinessLogic.SalesGenerator.Colors {
    public class OurColor : IComparable {
        private readonly byte _blue;
        private readonly byte _green;
        private readonly byte _red;

        public OurColor(Color color) {
            NumericColor = ColorToInt(color);
            _red = GetColorPart(NumericColor, 16);
            _green = GetColorPart(NumericColor, 8);
            _blue = GetColorPart(NumericColor, 0);
        }

        public int NumericColor { get; private set; }

        #region IComparable Members

        public int CompareTo(object obj) {
            var parsedObj = obj as OurColor;
            if (parsedObj == null) {
                return 0;
            }
            return NumericColor - parsedObj.NumericColor;
        }

        #endregion

        public override bool Equals(object obj) {
            var parsedObj = obj as OurColor;
            if (parsedObj == null) {
                return false;
            }
            return NumericColor.Equals(parsedObj.NumericColor);
        }

        public override int GetHashCode() {
            return NumericColor.GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0}, {1}, {2}", _red, _green, _blue);
        }

        private static int ColorToInt(Color color) {
            return (color.R << 16) | (color.G << 8) | (color.B);
        }

        public bool IsSimilarColor(OurColor color, byte delta) {
            return IsSimilarColor(this, color, delta);
        }

        public static bool IsSimilarColor(OurColor color1, OurColor color2, byte delta) {
            if (!IsPartInDelta(color1._red, color2._red, delta)) {
                return false;
            }
            if (!IsPartInDelta(color1._green, color2._green, delta)) {
                return false;
            }

            bool result = IsPartInDelta(color1._blue, color2._blue, delta);
            return result;
        }

        private static bool IsPartInDelta(byte partColor1, byte partColor2, byte maxDelta) {
            int delta = Math.Abs(partColor1 - partColor2);
            return delta <= maxDelta;
        }

        private static byte GetColorPart(int color, byte shift) {
            return (byte) ((color >> shift) & 255);
        }
    }
}