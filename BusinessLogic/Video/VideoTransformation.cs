using System;
using System.Diagnostics;
using System.IO;
using BusinessLogic.Formatters;

namespace BusinessLogic.Video {
    public class VideoTransformation {
        private readonly string _outFolder;
        private readonly string _sourceFile;

        public VideoTransformation(string sourceFile, string outFolder) {
            _sourceFile = sourceFile;
            _outFolder = outFolder;
        }

        public bool GetImage(string outFileName, TimeSpan time, int width = -1, int height = 150) {
            string outFile = Path.Combine(_outFolder, outFileName);
            string args = string.Format("-i \"{0}\" -ss {1} -vcodec mjpeg -vframes 1 -an -vf scale={2}:{3} \"{4}\"",
                                        _sourceFile, DateTimeFormatter.ToHHMMSSFFF(time, '.'), width, height, outFile);

            return RunCommand(args);
        }

        public bool Cut(string outFileName, TimeSpan from, TimeSpan to) {
            string outFile = Path.Combine(_outFolder, outFileName);

            string args = string.Format("-i \"{0}\" -ss {1} -to {2} -c:v copy -c:a copy \"{3}\"",
                                        _sourceFile, DateTimeFormatter.ToHHMMSSFFF(from, '.'),
                                        DateTimeFormatter.ToHHMMSSFFF(to, '.'), outFile);

            return RunCommand(args);
        }

        private bool RunCommand(string args) {
            try {
                if (!Directory.Exists(_outFolder)) {
                    Directory.CreateDirectory(_outFolder);
                }

                var appFullFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Video", "ffmpeg.exe");
                Process process = Process.Start(appFullFile, args);
                process.WaitForExit();
                return process.ExitCode == 0;
            } catch (Exception e) {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}