using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using BusinessLogic.ExternalData;
using Point = BusinessLogic.ExternalData.Representations.Point;

namespace BusinessLogic.SalesGenerator.Colors {
    public class WordsWriter {
        private const int MAX_SIZE_WORDS_WALL = 200;

        private readonly List<PointWithSign> _candidateFirstPoints = new List<PointWithSign>();
        private readonly List<PointWithSign> _candidateSecondPoints = new List<PointWithSign>();
        private readonly int _centerX;
        private readonly int _centerY;

        private readonly List<PointWithSign> _firstSidePoints = new List<PointWithSign>();
        private readonly int _height;
        private readonly bool _isHorizontalLine;
        private readonly int _leftX;
        private readonly int _leftY;
        private readonly Painter _painter;
        private readonly Rectangle[] _rectangles;
        private readonly List<PointWithSign> _secondSidePoints = new List<PointWithSign>();
        private readonly int _width;

        public WordsWriter(Painter painter, int initialWidth, int initialHeight) {
            _painter = painter;
            _width = initialWidth;
            _height = initialHeight;
            _leftX = 0;
            _leftY = 0;

            if (_width * 3 > _height) {
                _height += MAX_SIZE_WORDS_WALL * 2;
                _leftY = MAX_SIZE_WORDS_WALL;
                _rectangles = new[] {
                    new Rectangle(0, 0, initialWidth, MAX_SIZE_WORDS_WALL),
                    new Rectangle(0, _leftY + initialHeight, initialWidth, MAX_SIZE_WORDS_WALL),
                };

                _isHorizontalLine = true;
                _centerY = _height / 2;
            } else {
                _width += MAX_SIZE_WORDS_WALL * 2;
                _leftX = MAX_SIZE_WORDS_WALL;
                _rectangles = new[] {
                    new Rectangle(0, 0, MAX_SIZE_WORDS_WALL, initialHeight),
                    new Rectangle(_leftX + initialWidth, 0, MAX_SIZE_WORDS_WALL, initialHeight),
                };

                _isHorizontalLine = false;
                _centerX = _width / 2;
            }
        }

        public void AddRectangle(Point leftCorner, Point rightCorner, ISourceWithTranslation text) {
            var leftPointWithShift = new Point(leftCorner.X + _leftX, leftCorner.Y + _leftY);
            var rightPointWithShift = new Point(rightCorner.X + _leftX, rightCorner.Y + _leftY);

            var pointWithSign = new PointWithSign(leftPointWithShift, rightPointWithShift, text);
            if (_isHorizontalLine) {
                AddPoint(pointWithSign, leftPointWithShift.Y, rightPointWithShift.Y, _centerY);
            } else {
                AddPoint(pointWithSign, leftPointWithShift.X, rightPointWithShift.X, _centerX);
            }
        }

        private void AddPoint(PointWithSign pointWithSign, int leftCoord, int rightCoord, int centerCoord) {
            if (leftCoord < centerCoord && rightCoord < centerCoord) {
                _firstSidePoints.Add(pointWithSign);
            } else if (leftCoord > centerCoord && rightCoord > centerCoord) {
                _secondSidePoints.Add(pointWithSign);
            } else {
                if (centerCoord - leftCoord > rightCoord - centerCoord) {
                    _candidateFirstPoints.Add(pointWithSign);
                } else {
                    _candidateSecondPoints.Add(pointWithSign);
                }
            }
        }

        public Image GetImageWithSigns(Image image) {
            var bitmap = new Bitmap(_width, _height);
            Graphics graphics = Graphics.FromImage(bitmap);
            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.CompositingMode = CompositingMode.SourceOver;

            graphics.FillRectangles(new SolidBrush(Color.White), _rectangles);
            graphics.DrawImage(image, _leftX, _leftY, image.Width, image.Height);

            MoveMiddlePoints();
            WriteWords(_firstSidePoints, _rectangles[0], Color.Green, graphics);
            WriteWords(_secondSidePoints, _rectangles[1], Color.Blue, graphics);
            return bitmap;
        }

        private void MoveMiddlePoints() {
            int summFirst = _firstSidePoints.Count + _candidateFirstPoints.Count;
            int summSecond = _secondSidePoints.Count + _candidateSecondPoints.Count;

            int diff = Math.Abs(summFirst - summSecond) / 2;
            while (diff > 0) {
                if (summFirst < summSecond) {
                    AddCandidateFromTo(_candidateSecondPoints, _secondSidePoints, _candidateFirstPoints);
                } else {
                    AddCandidateFromTo(_candidateFirstPoints, _firstSidePoints, _candidateSecondPoints);
                }
                diff--;
            }

            AddCandidatesToMain(_candidateFirstPoints, _firstSidePoints);
            AddCandidatesToMain(_candidateSecondPoints, _secondSidePoints);
        }

        private void WriteWords(IEnumerable<PointWithSign> points, Rectangle rectangle, Color color, Graphics graphics) {
            const int MAX_WIDTH_IN_PIXELS = 200;
            const int MAX_HEIGHT_IN_PIXELS = 30;
            const int DEFAULT_LEFT_PADDING = 10;
            const int DEFAULT_TOP_PADDING = 10;

            var brush = new SolidBrush(color);
            var font = new Font(FontFamily.GenericSerif, 25, FontStyle.Bold, GraphicsUnit.Pixel);
            int coordX = rectangle.Left + DEFAULT_LEFT_PADDING;
            int coordY = rectangle.Top + DEFAULT_TOP_PADDING;
            foreach (PointWithSign point in points) {
                var wordRectangle = new RectangleF(coordX, coordY, MAX_WIDTH_IN_PIXELS, MAX_HEIGHT_IN_PIXELS);
               /* var wordRectangleCenter = new System.Drawing.Point(coordX + MAX_WIDTH_IN_PIXELS / 2,
                                                                   coordY + MAX_HEIGHT_IN_PIXELS / 2);*/

                var stringFormat = new StringFormat
                {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
                graphics.DrawString(point.SourceText, font, brush, wordRectangle, stringFormat);
                _painter.DrawRectangle(graphics, color, point.LeftCorner, point.RightCorner);

                coordX += MAX_WIDTH_IN_PIXELS + DEFAULT_LEFT_PADDING;
                if (coordX + MAX_WIDTH_IN_PIXELS > rectangle.Left + rectangle.Width) {
                    coordX = rectangle.Left + DEFAULT_LEFT_PADDING;
                    coordY += MAX_HEIGHT_IN_PIXELS + DEFAULT_TOP_PADDING;
                }

                /*var pen = new Pen(brush) {Width = 2};
                graphics.DrawLine(pen, wordRectangleCenter, point.Center);*/
            }
        }

        private static void AddCandidatesToMain(ICollection<PointWithSign> candidates, List<PointWithSign> main) {
            main.AddRange(candidates);
            candidates.Clear();
        }

        private static void AddCandidateFromTo(IList<PointWithSign> from,
                                               IList<PointWithSign> fromMain,
                                               ICollection<PointWithSign> to) {
            PointWithSign point;
            if (from.Count > 0) {
                point = from[0];
                from.Remove(point);
            } else {
                point = fromMain[fromMain.Count - 1];
                fromMain.Remove(point);
            }
            to.Add(point);
        }

        #region Nested type: PointWithSign

        private class PointWithSign {
            public PointWithSign(Point leftCorner, Point rightCorner, ISourceWithTranslation text) {
                LeftCorner = leftCorner;
                RightCorner = rightCorner;
                SourceText = text.Source.Text;
                TranslationText = text.Translation.Text;
            }

            public Point LeftCorner { get; private set; }
            public Point RightCorner { get; private set; }
            public string SourceText { get; private set; }
            public string TranslationText { get; private set; }

            public System.Drawing.Point Center {
                get {
                    return new System.Drawing.Point(LeftCorner.X + (RightCorner.X - LeftCorner.X) / 2,
                                                    LeftCorner.Y + (RightCorner.Y - LeftCorner.Y) / 2);
                }
            }
        }

        #endregion
    }
}