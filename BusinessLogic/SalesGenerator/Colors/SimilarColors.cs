using System.Collections.Generic;

namespace BusinessLogic.SalesGenerator.Colors {
    public class SimilarColors {
        private readonly Dictionary<OurColor, int> _colors = new Dictionary<OurColor, int>();

        public SimilarColors(OurColor baseColor, int count) {
            Count = 0;
            BaseColor = baseColor;
            AddColor(baseColor, count);
        }

        public OurColor BaseColor { get; private set; }

        public int Count { get; private set; }

        public void AddColor(OurColor color, int count) {
            if (!HasColor(color)) {
                _colors.Add(color, 0);
            }
            _colors[color] += count;
            Count += count;
        }

        public IEnumerable<OurColor> GetColors() {
            return _colors.Keys;
        }

        public bool HasColor(OurColor color) {
            return _colors.ContainsKey(color);
        }
    }
}