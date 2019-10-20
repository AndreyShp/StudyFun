using System.Drawing;
using Point = BusinessLogic.ExternalData.Representations.Point;

namespace BusinessLogic.SalesGenerator.Colors {
    public class PartImageCreator {
        private readonly Painter _painter;

        public PartImageCreator(Painter painter) {
            _painter = painter;
        }

        public PartImageData CutImage(Point origLeftCorner, Point origRightCorner, Image image) {
            const int MIN_WIDTH = 100;
            const int MIN_HEIGHT = 100;

            int width = origRightCorner.X - origLeftCorner.X;
            int height = origRightCorner.Y - origLeftCorner.Y;

            var partImageData = new PartImageData {HeightRectangle = height, WidthRectangle = width};
            Point leftCorner = origLeftCorner;
            bool isSmall;
            if (width < MIN_WIDTH || height < MIN_HEIGHT) {
                int newLeftX = origLeftCorner.X - MIN_WIDTH / 2;
                int newLeftY = origLeftCorner.Y - MIN_HEIGHT / 2;
                int newRightX = origRightCorner.X + MIN_WIDTH / 2;
                int newRightY = origRightCorner.Y + MIN_HEIGHT / 2;
                if (newLeftX < 0 || newLeftY < 0) {
                    newLeftX = origLeftCorner.X;
                    newRightX = origRightCorner.X + MIN_WIDTH;
                }

                if (newLeftY < 0) {
                    newLeftY = origLeftCorner.Y;
                    newRightY = origRightCorner.Y + MIN_HEIGHT;
                }

                if (newRightX > image.Width) {
                    newLeftX = origLeftCorner.X - MIN_WIDTH;
                    if (newLeftX < 0) {
                        newLeftX = origLeftCorner.X;
                    }
                    newRightX = origRightCorner.X;
                }

                if (newRightY > image.Height) {
                    newLeftY = origLeftCorner.Y - MIN_HEIGHT;
                    if (newLeftY < 0) {
                        newLeftY = origLeftCorner.Y;
                    }
                    newRightY = origRightCorner.Y;
                }

                origLeftCorner = new Point(origLeftCorner.X - newLeftX, origLeftCorner.Y - newLeftY);
                origRightCorner = new Point(origLeftCorner.X + width, origLeftCorner.Y + height);

                width = newRightX - newLeftX;
                height = newRightY - newLeftY;
                leftCorner = new Point(newLeftX, newLeftY);

                isSmall = true;
            } else {
                origLeftCorner = new Point(0, 0);
                origRightCorner = new Point(width, height);
                isSmall = false;
            }

            partImageData.Bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(partImageData.Bitmap);
            graphics.DrawImage(image, new Rectangle(0, 0, width, height),
                               new Rectangle(leftCorner.X, leftCorner.Y, width, height), GraphicsUnit.Pixel);

            partImageData.BrushColor = GetBrushColor(partImageData.Bitmap, origLeftCorner, origRightCorner);
            if (isSmall) {
                _painter.DrawRectangle(graphics, partImageData.BrushColor, origLeftCorner, origRightCorner);
            }
            graphics.Dispose();
            return partImageData;
        }

        private static Color GetBrushColor(Bitmap bitmap, Point origLeftCorner, Point origRightCorner) {
            /*int countWhiteTones = 0;
            int countBlackTones = 0;
            for (int x = origLeftCorner.X; x < origRightCorner.X; x++) {
                for (int y = origLeftCorner.Y; y < origRightCorner.Y; y++) {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    var ourColor = new OurColor(pixelColor);
                    if (ourColor.NumericColor <= 0x777777) {
                        countBlackTones++;
                    } else {
                        countWhiteTones++;
                    }
                }
            }
            Color brushColor = Color.DarkRed;
            if (countBlackTones > countWhiteTones) {
                brushColor = Color.Red;
            }*/
            Color brushColor = Color.Green;
            return brushColor;
        }
    }
}