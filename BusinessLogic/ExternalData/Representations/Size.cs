namespace BusinessLogic.ExternalData.Representations {
    /// <summary>
    /// Размер
    /// </summary>
    public class Size {
        public Size(int width, int height) {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; private set; }
    }
}