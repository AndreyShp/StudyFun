using System.Collections.Generic;

namespace Sandbox.Classes.Colors {
    public class SimilarColors {
        private readonly Dictionary<int, int> _colors = new Dictionary<int, int>();

        public SimilarColors(int numericColor) {
            Count = 0;
            AddColor(numericColor);
        }

        public int Count { get; private set; }

        public void AddColor(int numericColor) {
            if (!HasColor(numericColor)) {
                _colors.Add(numericColor, 0);
            }
            _colors[numericColor]++;
            Count++;
        }

        public IEnumerable<int> GetColors() {
            return _colors.Keys;
        }

        public bool HasColor(int numericColor) {
            return _colors.ContainsKey(numericColor);
        }
    }
}