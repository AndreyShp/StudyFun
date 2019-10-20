using System.Collections.Generic;
using BusinessLogic.Data.Enums;
using BusinessLogic.ExternalData.Videos;

namespace BusinessLogic.DataQuery.Video {
    public interface IVideosQuery {
        List<VideoForUser> GetVisible(VideoType type, int count = 0);

        List<VideoForUser> GetVisibleWithText(VideoType type, int count = 0);

        VideoForUser Get(string title);

        VideoForUser GetOrCreate(VideoType type, VideoForUser videoForUser, byte[] image, int? rating);

        byte[] GetImage(string title);
    }
}