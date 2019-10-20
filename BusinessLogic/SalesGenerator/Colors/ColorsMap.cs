using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace BusinessLogic.SalesGenerator.Colors {
    public class ColorsMap {
        private const int MAX_COUNT_BASE_COLORS = 10;
        private const int MAX_DEVIATION = 25;
        private readonly string _fullPathToSave;

        public ColorsMap(string fullPathToSave) {
            _fullPathToSave = fullPathToSave;
        }

        public void GetColorMap(Image image) {
            //var result = new int[image.Width, image.Height];
            var groupedSimilarColors = new List<SimilarColors>();

            var frequencyColors = new Dictionary<OurColor, int>();
            using (var bitmap = new Bitmap(image)) {
                for (int x = 0; x < bitmap.Width; x++) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        Color color = bitmap.GetPixel(x, y);

                        // AddColorToSimilarColors(color, groupedSimilarColors);

                        /*if (numericColor < 6000000) {
                            //цвет слишком темный для фона 
                            continue;
                        }*/

                        var ourColor = new OurColor(color);

                        if (!frequencyColors.ContainsKey(ourColor)) {
                            frequencyColors.Add(ourColor, 0);
                        }
                        frequencyColors[ourColor]++;
                    }
                }
            }

            List<OurColor> keys = frequencyColors.OrderByDescending(e => e.Value).Select(e => e.Key).ToList();
            AddSimilarColors(keys, frequencyColors, groupedSimilarColors);

            List<SimilarColors> sortedSimilarColors = groupedSimilarColors.OrderByDescending(e => e.Count).ToList();

            decimal minCount = sortedSimilarColors[0].Count * 0.6m;
            List<SimilarColors> probablyBackgroundColors = sortedSimilarColors.Where(e => e.Count >= minCount).ToList();

            using (var bitmap = new Bitmap(image)) {
                for (int x = 0; x < bitmap.Width; x++) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        Color color = bitmap.GetPixel(x, y);
                        var ourColor = new OurColor(color);
                        if (probablyBackgroundColors.Any(e => e.HasColor(ourColor))) {
                            bitmap.SetPixel(x, y, Color.Red);
                        }
                    }
                }
                bitmap.Save(_fullPathToSave, ImageFormat.Jpeg);
            }
        }

        private static void AddSimilarColors(List<OurColor> keys,
                                             Dictionary<OurColor, int> frequencyColors,
                                             List<SimilarColors> groupedSimilarColors) {
            for (int i = 0; i < keys.Count; i++) {
                OurColor ourColor = keys[i];
                int frequency = frequencyColors[ourColor];
                bool isSimilar = AddColorToSimilarColors(ourColor, frequency, groupedSimilarColors);

                if (!isSimilar && groupedSimilarColors.Count < MAX_COUNT_BASE_COLORS) {
                    groupedSimilarColors.Add(new SimilarColors(ourColor, frequency));
                }
            }
        }

        public static bool AddColorToSimilarColors(OurColor ourColor,
                                                   int count,
                                                   List<SimilarColors> groupedSimilarColors) {
            foreach (SimilarColors similarColors in groupedSimilarColors) {
                if (similarColors.HasColor(ourColor)) {
                    similarColors.AddColor(ourColor, count);
                    return true;
                }

                if (ourColor.IsSimilarColor(similarColors.BaseColor, MAX_DEVIATION)) {
                    similarColors.AddColor(ourColor, count);
                    return true;
                }
            }
            return false;
        }
    }
}