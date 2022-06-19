using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZipFile = Unity.VisualScripting.IonicZip.ZipFile;

namespace PartyQuiz.Utils
{
    public sealed class Zipper : IDisposable
    {
        private const string MANIFEST = "manifest.json";

        private readonly Dictionary<string, Stream> _fileStreams = new();
        private readonly List<string> _tempFiles = new();
        private ZipFile _zip;

        public void AddStream(string name, Stream stream, string tempPath)
        {
            _fileStreams.Add(name, stream);
            _tempFiles.Add(tempPath);
        }

        public void SaveToFile(string json, string savePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var manifest = archive.CreateEntry(MANIFEST, CompressionLevel.NoCompression);
                    using (var entryStream = manifest.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(json);
                    }

                    foreach (var (name, stream) in _fileStreams)
                    {
                        AddToZip(archive, name, stream);
                    }
                }

                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }

            Dispose();
        }

        public string LoadManifest(string loadPath)
        {
            _zip = ZipFile.Read(loadPath);
            var manifestEntry = _zip[MANIFEST];

            using var stream = new MemoryStream();
            manifestEntry.Extract(stream);
            stream.Position = 0;

            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public MemoryStream LoadFromZip(string path)
        {
            var fileEntry = _zip[path];
            var stream = new MemoryStream();
            fileEntry.Extract(stream);
            stream.Position = 0;

            return stream;
        }

        private static void AddToZip(ZipArchive archive, string name, Stream stream)
        {
            var fileInArchive = archive.CreateEntry(name, CompressionLevel.NoCompression);
            using var entryStream = fileInArchive.Open();

            stream.Position = 0;
            stream.CopyTo(entryStream);
        }

        public void Dispose()
        {
            foreach (var (_, stream) in _fileStreams)
                stream.Dispose();

            _fileStreams.Clear();
            _zip?.Dispose();

            foreach (var tempFile in _tempFiles.Where(File.Exists))
                File.Delete(tempFile);

            _tempFiles.Clear();
        }
    }
}