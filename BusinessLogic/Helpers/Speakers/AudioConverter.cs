using System;
using System.IO;
using BusinessLogic.Logger;
using NAudio.Wave;
using Yeti.MMedia.Mp3;
using WaveStream = WaveLib.WaveStream;

namespace BusinessLogic.Helpers.Speakers {
    public class AudioConverter {
        internal byte[] ConvertWavToMp3(Stream stream) {
            var inStr = new WaveStream(stream);
            try {
                var convertedStream = new MemoryStream();
                var writer = new Mp3Writer(convertedStream, inStr.Format);
                try {
                    var buff = new byte[writer.OptimalBufferSize];
                    int read;
                    while ((read = inStr.Read(buff, 0, buff.Length)) > 0) {
                        writer.Write(buff, 0, read);
                    }

                    convertedStream.Seek(0, SeekOrigin.Begin);
                    return convertedStream.ToArray();
                } catch (Exception e) {
                    LoggerWrapper.LogTo(LoggerName.Errors).ErrorException(
                        "AudioConverter.ConvertWavToMp3 inner вылетело исключение {0}", e);
                } finally {
                    writer.Close();
                }
            } catch (Exception e) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorException(
                    "AudioConverter.ConvertWavToMp3 outer вылетело исключение {0}", e);
            } finally {
                inStr.Close();
            }
            return null;
        }

        public byte[] ConvertMp3ToWav(byte[] mp3) {
            try {
                var mp3Stream = new MemoryStream(mp3);
                mp3Stream.Seek(0, SeekOrigin.Begin);
                var convertedStream = new MemoryStream();
                
                using (var reader = new Mp3FileReader(mp3Stream)) {
                    var waveFormat = new WaveFormat(16000, 16, 1);
                    using (var waveFileWriter = new WaveFileWriter(convertedStream, waveFormat)) {
                        var buffer = new byte[1024];
                        int countReaded;
                        do {
                            countReaded = reader.Read(buffer, 0, buffer.Length);
                            if (countReaded > 0) {
                                waveFileWriter.Write(buffer, 0, countReaded);
                            }
                        } while (countReaded > 0);
                        convertedStream.Seek(0, SeekOrigin.Begin);
                        return convertedStream.ToArray();
                    }
                }
            } catch (Exception e) {
                LoggerWrapper.LogTo(LoggerName.Errors).ErrorException(
                    "AudioConverter.ConvertMp3ToWav вылетело исключение {0}", e);
            }
            return null;
        }
    }
}