using System.Drawing;
using Point = BusinessLogic.ExternalData.Representations.Point;

namespace BusinessLogic.SalesGenerator.Colors {
    public class Painter {
        public void DrawRectangle(Graphics graphics, Color color, Point leftCorner, Point rightCorner) {
            var pen = new Pen(color) {Width = 2};

            int width = rightCorner.X - leftCorner.X;
            int height = rightCorner.Y - leftCorner.Y;
            graphics.DrawRectangle(pen, leftCorner.X, leftCorner.Y, width, height);
        }
    }
}