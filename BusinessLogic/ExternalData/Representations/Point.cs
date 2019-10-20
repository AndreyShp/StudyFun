namespace BusinessLogic.ExternalData.Representations {
    /// <summary>
    /// Точка
    /// </summary>
    public class Point {
        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Координата по X оси
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Координата по Y оси
        /// </summary>
        public int Y { get; private set; }
    }
}