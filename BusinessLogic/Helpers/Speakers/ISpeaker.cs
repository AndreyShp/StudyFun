namespace BusinessLogic.Helpers.Speakers {
    public interface ISpeaker {
        byte[] ConvertTextToAudio(string text);
    }
}