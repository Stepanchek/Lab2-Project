using System.IO;
using JetBrains.Annotations;
using PartyQuiz.Serialization;
using PartyQuiz.Siq;
using PartyQuiz.Structure.Runtime;
using SFB;

namespace PartyQuiz.Constructor
{
    internal sealed class PackLoader
    {
        private readonly ExtensionFilter _extensions = new(string.Empty, "siq", "pq");

        [CanBeNull]
        internal Pack LoadPack()
        {
            var paths = StandaloneFileBrowser
                .OpenFilePanel("Open pack file", string.Empty, new[] { _extensions }, false);

            if (paths.Length <= 0)
                return null;

            var path = paths[0];
            var extension = Path.GetExtension(path);

            switch (extension)
            {
                case ".siq":
                    return LoadSiq(path);
                case ".pq":
                    return LoadPq(path);
            }

            return null;
        }

        private Pack LoadSiq(string path)
        {
            var packPath = StandaloneFileBrowser.SaveFilePanel("Save pack", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path), "pq");

            if (string.IsNullOrEmpty(packPath))
                return null;

            SiqConverter.ConvertSiqFile(path, packPath);
            return LoadPq(packPath);
        }

        private Pack LoadPq(string path)
        {
            var pack = Helper.Import(path, true);
            return pack;
        }
    }
}