using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using BusinessLogic.Logger;

namespace StudyLanguages.Helpers {
    /// <summary>
    /// Provides various image untilities, such as high quality resizing and the ability to save a JPEG.
    /// </summary>
    public static class ImageUtilities {
        /// <summary>
        /// Изменяет размер изображения
        /// </summary>
        /// <param name="imageBytes">изображение jpeg в виде массива байт</param>
        /// <param name="height">высота изображения</param>
        /// <returns>массив байт </returns>
        public static byte[] ResizeImage(byte[] imageBytes, int height) {
            try {
                return GetResizedImage(imageBytes, height);
            } catch (Exception e) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorFormat(
                    "ImageUtilities. При конвертации изображения длинной {0} в высоту {1} возникло исключение {2}",
                    imageBytes.Length, height, e);
                return imageBytes;
            }
        }

        private static byte[] GetResizedImage(byte[] imageBytes, int height) {
            var byteStream = new MemoryStream(imageBytes);
            Image image = Image.FromStream(byteStream);
            int width;
            if (height < image.Size.Height) {
                decimal scaleCoef = height / (decimal) image.Size.Height;
                width = Convert.ToInt32(Math.Floor(scaleCoef * image.Size.Width));
            } else {
                height = image.Size.Height;
                width = image.Size.Width;
            }

            //a holder for the result
            var result = new Bitmap(width, height);
            // set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result)) {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            var resultImage = new MemoryStream();
            result.Save(resultImage, ImageFormat.Jpeg);
            return resultImage.ToArray();
        }
    }
}