using System;
using System.IO;
using JetBrains.Annotations;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Serialization.Importers
{
    internal sealed class RuntimeImporter : IPackImporter, IDisposable
    {
        private readonly string _rootPath;
        private readonly Zipper _zipper;
        private readonly bool _saveTempAssets;

        internal RuntimeImporter(string rootPath, bool saveTempAssets)
        {
            _rootPath = rootPath;
            _saveTempAssets = saveTempAssets;
            _zipper = new Zipper();
        }

        [CanBeNull]
        public Media<Sprite> ImportSprite([CanBeNull] string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            
            var imageBytes = _zipper.LoadFromZip(path).ToArray();
            var texture = new Texture2D(1, 1);

            texture.LoadImage(imageBytes);

            var sprite = Sprite.Create(
                texture,
                rect: new Rect(0f, 0f, texture.width, texture.height),
                pivot: new Vector2(0f, 0f),
                pixelsPerUnit: 100f
            );

            var assetPath = GetAssetPath(imageBytes, path);
            return new Media<Sprite>(sprite, assetPath);
        }

        [CanBeNull]
        public Media<AudioClip> ImportAudioClip([CanBeNull] string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            
            var clipBytes = _zipper.LoadFromZip(path).ToArray();
            var extension = Path.GetExtension(path);

            var clip = extension switch
            {
                ".ogg" => NAudioPlayer.FromOggData(clipBytes),
                ".mp3" => NAudioPlayer.FromMp3Data(clipBytes),
                ".wav" => NAudioPlayer.FromWavData(clipBytes),
                _ => null
            };

            if (clip == null)
            {
                Debug.LogError("Wrong extension: " + extension);
                return null;
            }

            clip.name = Path.GetFileNameWithoutExtension(path);
            var assetPath = GetAssetPath(clipBytes, path);
            return new Media<AudioClip>(clip, assetPath);
        }

        public string ImportPack()
        {
            return _zipper.LoadManifest(_rootPath);
        }

        public void Dispose()
        {
            _zipper.Dispose();
        }

        private string GetAssetPath(byte[] data, string path)
        {
            return _saveTempAssets
                ? AssetFileUtil.SaveAsset(data, Path.GetFileName(path))
                : string.Empty;
        }
    }
}