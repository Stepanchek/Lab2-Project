using System.IO;
using JetBrains.Annotations;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Serialization.Exporters
{
    internal sealed class PackExporter : IPackExporter
    {
        private readonly string _rootPath;
        private readonly Zipper _zipper = new();

        internal PackExporter(string rootPath)
        {
            _rootPath = rootPath;
        }

        public string ExportSprite(Media<Sprite> asset)
        {
            return ExportAsset(asset, "pics");
        }

        public string ExportAudioClip(Media<AudioClip> asset)
        {
            return ExportAsset(asset, "audio");
        }

        public void ExportPack(string packJson)
        {
            if (string.IsNullOrEmpty(_rootPath))
                return;
            
            Debug.Log($"Pack exported to {_rootPath}");

            _zipper.SaveToFile(packJson, _rootPath);
        }

        private string ExportAsset<T>([CanBeNull] Media<T> asset, string directoryName) where T : Object
        {
            if (asset == null)
                return string.Empty;

            if (asset.Source == null)
                return string.Empty;
            
            var assetPath = asset.Path;
            var name = Path.GetFileNameWithoutExtension(assetPath);

            var assetName = $"{name}_{asset.Source.GetHashCode()}_{Path.GetExtension(assetPath)}";

            var path = $@"{directoryName}/{assetName}";
            var stream = new FileStream(assetPath, FileMode.Open);

            _zipper.AddStream(path, stream, assetPath);

            return path;
        }
    }
}