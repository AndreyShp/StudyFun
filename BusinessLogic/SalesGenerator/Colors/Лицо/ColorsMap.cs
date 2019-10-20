using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Sandbox.Classes.Colors {
    public class ColorsMap {
        public static void GetColorMap(Image image) {
            //var result = new int[image.Width, image.Height];
            var groupedSimilarColors = new List<SimilarColors>();

            var frequencyColors = new Dictionary<int, int>();
            using (var bitmap = new Bitmap(image)) {
                for (int x = 0; x < bitmap.Width; x++) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        Color color = bitmap.GetPixel(x, y);

                        // AddColorToSimilarColors(color, groupedSimilarColors);

                        int numericColor = ColorToInt(color);
                        /*if (numericColor < 6000000) {
                            //цвет слишком темный для фона 
                            continue;
                        }*/

                        if (!frequencyColors.ContainsKey(numericColor)) {
                            frequencyColors.Add(numericColor, 0);
                        }
                        frequencyColors[numericColor]++;

                        //result[x, y] = color;
                    }
                }
            }

            //var similarColors = new Dictionary<int, HashSet<int>>();
            List<int> keys = frequencyColors.OrderByDescending(e => e.Value).Select(e => e.Key).ToList();
            for (int i = 0; i < keys.Count; i++) {
                /*foreach (var color2 in frequencyColors.Keys) {
                    if (color1 == color2) {
                        continue;
                    }

                    if (IsSimilarColor(color1, color2)) {
                        AddSimilar(similarColors, color1, color2);
                        AddSimilar(similarColors, color2, color1);
                        //similarColors.Add(numericColor, new HashSet<int> { key });
                    }
                }*/
                int numericColor = keys[i];
                int count = frequencyColors[numericColor];
                AddColorToSimilarColors(numericColor, count, groupedSimilarColors);
            }

            int ii = 0;

            /*
            long maxFrequency = 0;
            var groupedColorsByFrequency = new Dictionary<long, List<HashSet<Color>>>();
            List<Color> allColors = frequencyColors.OrderBy(e => GetColorOrderRank(e.Key)).Select(e => e.Key).ToList();
            long count = frequencyColors[allColors[0]];
            var groupedColors = new List<Color> { allColors[0] };
            for (int i = 1; i < allColors.Count; i++)
            {
                Color numericColor = allColors[i];
                int frequency = frequencyColors[numericColor];

                Color prevColor = groupedColors[groupedColors.Count - 1];
                if (!IsSimilarColor(prevColor, numericColor))
                {
                    //цвет нельзя группировать

                    List<HashSet<Color>> colors;
                    if (!groupedColorsByFrequency.TryGetValue(count, out colors))
                    {
                        colors = new List<HashSet<Color>>();
                        groupedColorsByFrequency.Add(count, colors);
                    }
                    colors.Add(new HashSet<Color>(groupedColors));
                    if (maxFrequency < count)
                    {
                        maxFrequency = count;
                    }
                    count = 0;
                    groupedColors = new List<Color>();
                }

                //цвет можно группировать
                groupedColors.Add(numericColor);
                count += frequency;
            }

            if (groupedColors.Count > 0)
            {
                if (maxFrequency < count)
                {
                    maxFrequency = count;
                }
                List<HashSet<Color>> colors;
                if (!groupedColorsByFrequency.TryGetValue(count, out colors))
                {
                    colors = new List<HashSet<Color>>();
                    groupedColorsByFrequency.Add(count, colors);
                }
                colors.Add(new HashSet<Color>(groupedColors));
            }*/

            List<SimilarColors> priority = groupedSimilarColors.OrderByDescending(e => e.Count).ToList();
            SimilarColors mostFrequencyColors = priority.First();
            using (var bitmap = new Bitmap(image)) {
                for (int x = 0; x < bitmap.Width; x++) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        Color color = bitmap.GetPixel(x, y);
                        int numericColor = ColorToInt(color);
                        if (mostFrequencyColors.HasColor(numericColor)) {
                            bitmap.SetPixel(x, y, Color.White);
                        }
                    }
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "2111.jpeg");
                bitmap.Save(path, ImageFormat.Jpeg);
            }
        }

        private static void AddSimilar(Dictionary<int, HashSet<int>> similarColors, int key, int numericColor) {
            HashSet<int> similar;
            if (!similarColors.TryGetValue(key, out similar)) {
                similar = new HashSet<int>();
                similarColors.Add(key, similar);
            }
            similar.Add(numericColor);
        }

        /*public static void GetColorMap(Image image) {
            var result = new int[image.Width,image.Height];
            var frequencyColors = new Dictionary<Color, int>();
            using (var bitmap = new Bitmap(image)) {
                for (int x = 0; x < bitmap.Width; x++) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        Color color = bitmap.GetPixel(x, y);
                        int numericColor = ColorToInt(color);
                        /*if (numericColor < 6000000) {
                            //цвет слишком темный для фона
                            continue;
                        }#1#

                        if (!frequencyColors.ContainsKey(color)) {
                            frequencyColors.Add(color, 0);
                        }
                        frequencyColors[color]++;
                        result[x, y] = numericColor;
                    }
                }
            }

            long maxFrequency = 0;
            var groupedColorsByFrequency = new Dictionary<long, List<HashSet<Color>>>();
            List<Color> allColors = frequencyColors.OrderBy(e => GetColorOrderRank(e.Key)).Select(e => e.Key).ToList();
            long count = frequencyColors[allColors[0]];
            var groupedColors = new List<Color> {allColors[0]};
            for (int i = 1; i < allColors.Count; i++) {
                Color numericColor = allColors[i];
                int frequency = frequencyColors[numericColor];

                Color prevColor = groupedColors[groupedColors.Count - 1];
                if (!IsSimilarColor(prevColor, numericColor)) {
                    //цвет нельзя группировать

                    List<HashSet<Color>> colors;
                    if (!groupedColorsByFrequency.TryGetValue(count, out colors)) {
                        colors = new List<HashSet<Color>>();
                        groupedColorsByFrequency.Add(count, colors);
                    }
                    colors.Add(new HashSet<Color>(groupedColors));
                    if (maxFrequency < count) {
                        maxFrequency = count;
                    }
                    count = 0;
                    groupedColors = new List<Color>();
                }

                //цвет можно группировать
                groupedColors.Add(numericColor);
                count += frequency;
            }

            if (groupedColors.Count > 0) {
                if (maxFrequency < count) {
                    maxFrequency = count;
                }
                List<HashSet<Color>> colors;
                if (!groupedColorsByFrequency.TryGetValue(count, out colors)) {
                    colors = new List<HashSet<Color>>();
                    groupedColorsByFrequency.Add(count, colors);
                }
                colors.Add(new HashSet<Color>(groupedColors));
            }

            List<HashSet<Color>> mostPopularColors = groupedColorsByFrequency[maxFrequency];
            HashSet<Color> popularColors = mostPopularColors.First();
            using (var bitmap = new Bitmap(image)) {
                for (int x = 0; x < bitmap.Width; x++) {
                    for (int y = 0; y < bitmap.Height; y++) {
                        Color color = bitmap.GetPixel(x, y);
                        int numericColor = ColorToInt(color);
                        if (popularColors.Contains(color)) {
                            bitmap.SetPixel(x, y, Color.White);
                        }
                    }
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "2111.jpeg");
                bitmap.Save(path, ImageFormat.Jpeg);
            }
        }*/

        /*public static void AddColorToSimilarColors(Color color, List<SimilarColors> groupedSimilarColors) {
            int numericColor = ColorToInt(color);
            AddColorToSimilarColors(numericColor, groupedSimilarColors);
        }*/

        public static void AddColorToSimilarColors(int numericColor, int count, List<SimilarColors> groupedSimilarColors) {
            foreach (SimilarColors similarColors in groupedSimilarColors) {
                if (similarColors.HasColor(numericColor)) {
                    similarColors.AddColor(numericColor, count);
                    return;
                }

                foreach (int similarColor in similarColors.GetColors()) {
                    if (IsSimilarColor(numericColor, similarColor)) {
                        similarColors.AddColor(numericColor, count);
                        return;
                    }
                }
            }
            groupedSimilarColors.Add(new SimilarColors(numericColor, count));
        }

        /*public static bool IsSimilarColor(Color prevColor, Color currentColor) {
            int prevNumeric = ColorToInt(prevColor);
            int curNumeric = ColorToInt(currentColor);
            return IsSimilarColor(prevNumeric, curNumeric);
        }*/

        public static bool IsSimilarColor(int prevColor, int currentColor) {
            /*const int MAX_DELTA = 30000;

            var max = prevColor + MAX_DELTA;
            var min = prevColor - MAX_DELTA;
            return min <= numericColor && numericColor <= max;*/
            Color color1 = Color.FromArgb((prevColor >> 16) & 255, (prevColor >> 8) & 255, prevColor & 255);
            Color color2 = Color.FromArgb((currentColor >> 16) & 255, (currentColor >> 8) & 255, currentColor & 255);

            if (!IsPartInDelta(prevColor, currentColor, 0)) {
                return false;
            }
            if (!IsPartInDelta(prevColor, currentColor, 8)) {
                return false;
            }

            bool result = IsPartInDelta(prevColor, currentColor, 16);
            return result;
        }

        private static bool IsPartInDelta(int color1, int color2, byte shift) {
            const int MAX_PART_DELTA = 34;

            int partColor1 = GetColorPart(color1, shift);
            int partColor2 = GetColorPart(color2, shift);
            int delta = Math.Abs(partColor1 - partColor2);
            return delta <= MAX_PART_DELTA;
        }

        private static int GetColorPart(int color2, byte shift) {
            return (color2 >> shift) & 255;
        }

        private static int ColorToInt(Color color) {
            return (color.R << 16) | (color.G << 8) | (color.B);
        }

        public static int GetColorOrderRank(Color color) {
            const int MAX_PART_DELTA = 10;
            int redDelta = Math.Abs(color.R - MAX_PART_DELTA);
            int greenDelta = Math.Abs(color.G - MAX_PART_DELTA);
            int blueDelta = Math.Abs(color.B - MAX_PART_DELTA);
            return (redDelta << 16) | (greenDelta << 8) | blueDelta;
        }
    }
}