using System;
using System.IO;
using System.IO.Compression;

namespace BusinessLogic.SalesGenerator {
    public class ZipCompressor : IDisposable {
        private MemoryStream _memoryStream;
        private ZipArchive _zipArchive;

        public ZipCompressor() {
            _memoryStream = new MemoryStream();
            _zipArchive = new ZipArchive(_memoryStream, ZipArchiveMode.Create, true);
        }

        #region IDisposable Members

        public void Dispose() {
            CloseZipArchive();
            CloseMemoryStream();
        }

        #endregion

        private void CloseMemoryStream() {
            if (_memoryStream != null) {
                _memoryStream.Close();
                _memoryStream.Dispose();
                _memoryStream = null;
            }
        }

        public void WriteArchive(string outputZipArchive) {
            CloseZipArchive();
            using (var fileStream = new FileStream(outputZipArchive, FileMode.Create)) {
                _memoryStream.Seek(0, SeekOrigin.Begin);
                _memoryStream.CopyTo(fileStream);
            }
            CloseMemoryStream();
        }

        public byte[] GetArchive() {
            CloseZipArchive();
            _memoryStream.Seek(0, SeekOrigin.Begin);
            byte[] result = _memoryStream.ToArray();
            CloseMemoryStream();
            return result;
        }

        private void CloseZipArchive() {
            if (_zipArchive != null) {
                _zipArchive.Dispose();
                _zipArchive = null;
            }
        }

        public void AddFileToArchive(string entryName, Stream entrySourceStream) {
            WriteToZipEntry(entryName, entryStream => {
                entrySourceStream.Seek(0, SeekOrigin.Begin);
                entrySourceStream.CopyTo(entryStream);
            });
        }

        public void AddFileToArchive(string entryName, byte[] entrySource) {
            WriteToZipEntry(entryName, entryStream => entryStream.Write(entrySource, 0, entrySource.Length));
        }

        private void WriteToZipEntry(string entryName, Action<Stream> entryStreamFiller) {
            ZipArchiveEntry zipEntry = _zipArchive.CreateEntry(entryName, CompressionLevel.Optimal);

            using (Stream entryStream = zipEntry.Open()) {
                entryStreamFiller(entryStream);
            }
        }

        /*public void AddTextFile(string entryName, string fileContent) {
            var memStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memStream)) {
                streamWriter.Write(fileContent);
                AddFileToArchive(entryName, memStream);
            }

           /* ZipArchiveEntry demoFile = _zipArchive.CreateEntry(entryName);

            using (Stream entryStream = demoFile.Open()) {
                using (var streamWriter = new StreamWriter(entryStream)) {
                    streamWriter.Write(fileContent);
                }
            }#1#
        }*/

        /*public void Compress(string outputZipArchive)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    WriteZipEntry("1.txt", archive);
                    WriteZipEntry("2.txt", archive);
                }

                using (var fileStream = new FileStream(outputZipArchive, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }*/

        /*public void Compress(string outputPath) {
            using (var memoryStream = new MemoryStream()) {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
                    ZipArchiveEntry demoFile = archive.CreateEntry("foo.txt");

                    using (Stream entryStream = demoFile.Open()) {
                        using (var streamWriter = new StreamWriter(entryStream)) {
                            streamWriter.Write("Bar!");
                        }
                    }
                }

                using (var fileStream = new FileStream(@"C:\test.zip", FileMode.Create)) {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }*/
    }
}