using System;
using BusinessLogic.Data;
using BusinessLogic.Data.Comparison;
using BusinessLogic.Data.Representation;

namespace BusinessLogic.ExternalData {
    public class SortInfo {
        private const int DEFAULT_RATING = 0;

        internal SortInfo(Group @group) {
            LastModified = @group.LastModified;
            Rating = @group.Rating ?? DEFAULT_RATING;
            Name = group.Name;
        }

        internal SortInfo(Representation representation) {
            LastModified = representation.LastModified;
            Rating = representation.Rating ?? DEFAULT_RATING;
            Name = representation.Title;
        }

        internal SortInfo(GroupComparison groupComparison) {
            LastModified = groupComparison.LastModified;
            Rating = groupComparison.Rating ?? DEFAULT_RATING;
            Name = groupComparison.Title;
        }

        internal SortInfo(Data.Video.Video video) {
            LastModified = video.LastModified;
            Rating = video.Rating ?? DEFAULT_RATING;
            Name = video.Title;
        }

        public DateTime LastModified { get; private set; }
        public int Rating { get; private set; }
        public string Name { get; private set; }
    }
}