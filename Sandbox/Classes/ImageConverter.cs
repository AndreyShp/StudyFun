using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Sandbox.Classes {
    public class ImageConverter {
        public const int DEFAULT_IMAGE_SMALL_HEIGHT = 200;

        public static void Convert(string oldFileName, string newFileName) {
            Image image = Image.FromFile(oldFileName);

            //a holder for the result
            var result = new Bitmap(image.Width, image.Height);
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
            result.Save(newFileName, ImageFormat.Jpeg);
        }

        public static void ConvertAll() {
            var paths = new[] {
                @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок",
                @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\Гарниры",
                @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\Одежда и аксессуары",
                @"C:\Projects\StudyLanguages\Источники для групп\Источники картинок\Цвета"
            };
            foreach (string path in paths) {
                string[] fileNames = Directory.GetFiles(path, "*.png");
                foreach (string fileName in fileNames) {
                    Convert(fileName, fileName.Replace(".png", ".jpg"));
                    //File.Delete(fileName);
                }
            }
            foreach (string path in paths) {
                string[] fileNames = Directory.GetFiles(path, "*.png");
                foreach (string fileName in fileNames) {
                    try {
                        File.Delete(fileName);
                    } catch (Exception e) {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public static void ResizeSmallImages(string fileName, int height = DEFAULT_IMAGE_SMALL_HEIGHT) {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Bitmap result = GetResizedImage(imageBytes, height);
            result.Save(fileName, ImageFormat.Jpeg);
        }

        public static byte[] ResizeImage(byte[] image, int height = DEFAULT_IMAGE_SMALL_HEIGHT) {
            Bitmap bitmap = GetResizedImage(image, height);
            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Jpeg);
            return memoryStream.ToArray();
        }

        private static Bitmap GetResizedImage(byte[] imageBytes, int height) {
            var ms = new MemoryStream(imageBytes);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            Image image = Image.FromStream(ms);
            int width;
            if (height < image.Size.Height) {
                decimal scaleCoef = height / (decimal) image.Size.Height;
                width = System.Convert.ToInt32(Math.Floor(scaleCoef * image.Size.Width));
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
            return result;
        }

        public static void ResizeAllByMode() {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("Если нажмете 1 и Enter, то размер будет 200px. Если просто Enter, то 600px");
            string mode = Console.ReadLine() ?? string.Empty;
            int height = mode.Trim() == "1" ? DEFAULT_IMAGE_SMALL_HEIGHT : 600;
            int i = 0;
            ConvertBySearchPattern(directory, height, "*.jpg", ref i);
            ConvertBySearchPattern(directory, height, "*.jpeg", ref i);

            Console.WriteLine("У меня все:) Обработано {0} файлов. Нажмите Enter для выхода из программы...", i);
            Console.ReadLine();
        }

        private static void ConvertBySearchPattern(string directory, int height, string searchPattern, ref int counter) {
            string[] fileNames;
            try {
                fileNames = Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
            } catch (Exception e) {
                Console.WriteLine(
                    "ОЙ! При поиске файлов {0} в папке {1} возникло исключение {2}{3}Нажмите Enter для продолжения...",
                    directory, searchPattern, e, Environment.NewLine);
                Console.ReadLine();
                return;
            }

            foreach (string fileName in fileNames) {
                ++counter;
                try {
                    ResizeSmallImages(fileName, height);
                    Console.WriteLine("Обработан файл {0}.{1}", counter, fileName);
                } catch (Exception e) {
                    Console.WriteLine(
                        "ОЙ! Не удалось сохранить картинку {0}. Возможно она используется другим приложением?{1}Исключение: {2}{1}Нажмите Enter для продолжения...",
                        fileName, Environment.NewLine, e);
                    Console.ReadLine();
                }
            }
        }
    }
}