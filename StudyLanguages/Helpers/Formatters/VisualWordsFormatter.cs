using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData.Representations;

namespace StudyLanguages.Helpers.Formatters {
    public static class VisualWordsFormatter {
        public static int GetCountInFirstColumn(List<RepresentationAreaForUser> areas) {
            int result = areas.Count / 2;
            if (areas.Count % 2 > 0) {
                result++;
            }
            return result;
        }

        public static int GetRecommendedImageWidth(RepresentationForUser representation) {
            const decimal MAX_WIDTH = 50;

            if (representation.WidthPercent.HasValue) {
                return representation.WidthPercent.Value;
            }

            Size size = representation.Size;
            if (size.Width >= size.Height) {
                return 44;
            }
            decimal ratio = size.Width / (decimal) size.Height;
            /*if (ratio > 1) {
                ratio = ratio - 1;
            } else if (ratio > 0) {*/
            ratio = 1 - ratio;
            //}
            decimal recommendedPercent = Math.Abs(MAX_WIDTH - ratio * 100);
            return Convert.ToInt32(Math.Floor(recommendedPercent));
        }
    }
}