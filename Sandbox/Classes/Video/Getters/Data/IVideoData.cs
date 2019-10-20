namespace Sandbox.Classes.Video.Getters.Data {
    public interface IVideoData {
        string Vid { get; set; }

        string HtmlCode { get; set; }

        string Title { get; set; }

        byte[] ThumnailImage { get; set; }

        int? Rating { get; }
    }
}