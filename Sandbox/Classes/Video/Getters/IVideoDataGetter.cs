using BusinessLogic.ExternalData;
using Sandbox.Classes.Video.Getters.Data;

namespace Sandbox.Classes.Video.Getters {
    public interface IVideoDataGetter {
        IVideoData GetVideoData(string url);

        IVideoData CreateFromString(string data);

        string ConvertToString(IVideoData data);

        bool IsInvalid(IVideoData data, LanguageShortName shortName);
    }
}