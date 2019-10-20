using System;
using System.IO;
using System.Speech.Synthesis;
using BusinessLogic.Logger;

namespace BusinessLogic.Helpers.Speakers {
    /// <summary>
    /// Класс конвертирует слово/текст в аудио формат
    /// </summary>
    internal class EnglishSpeaker : ISpeaker {
        private readonly AudioConverter _mp3Converter = new AudioConverter();
        /* public byte[] ConvertTextToWavStreamAsync(string text) {
            /*var task = new Task<Stream>(() => ConvertTextToAudio(text));
            task.Start();
            task.Wait();
            return task.Result;#1#


            byte[] byteArr = null;

            var t = new System.Threading.Thread(() =>
            {
                SpeechSynthesizer ss = new SpeechSynthesizer();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    
                //    ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                    ss.SetOutputToWaveStream(memoryStream);
                    ss.Speak(text);

                    byteArr = memoryStream.ToArray();
                }
            });
            t.Start();
            t.Join();
            return byteArr;
        }*/

        /*byte[] byteArr = null;

                SpeechSynthesizer ss = new SpeechSynthesizer();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    
                    ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                    ss.SetOutputToWaveStream(memoryStream);
                    ss.SpeakCompleted += (sender, args) =>
                    {
                        manualResetEvent.Set();
                        ss.Dispose();
                    };
                    ss.SpeakAsync(text);
                    manualResetEvent.WaitOne();

                    byteArr = memoryStream.ToArray();*/

        #region ISpeaker Members

        /// <summary>
        /// Конвертировать текст в фафл WAV
        /// </summary>
        /// <param name="text">текст</param>
        /// <returns>WAV файл в виде набора байтов</returns>
        public byte[] ConvertTextToAudio(string text) {
            var pbuilder = new PromptBuilder();
            var pStyle = new PromptStyle
            {Emphasis = PromptEmphasis.None, Rate = PromptRate.Medium, Volume = PromptVolume.ExtraLoud};

            pbuilder.StartStyle(pStyle);
            pbuilder.StartParagraph();
            pbuilder.StartVoice(VoiceGender.Neutral, VoiceAge.Adult, 2);
            pbuilder.StartSentence();
            pbuilder.AppendText(text);
            pbuilder.EndSentence();
            pbuilder.EndVoice();
            pbuilder.EndParagraph();
            pbuilder.EndStyle();

            try {
                using (var memoryStream = new MemoryStream()) {
                    var speech = new SpeechSynthesizer();
                    speech.SetOutputToWaveStream(memoryStream);
                    speech.Speak(pbuilder);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return _mp3Converter.ConvertWavToMp3(memoryStream);
                }
            } catch (Exception e) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorException(
                    "Speaker.ConvertTextToAudio возникло исключение {0}", e);
            }
            return null;
        }

        #endregion
    }
}