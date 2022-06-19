using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Serialization.Importers
{
    public interface IPackImporter
    {
        Media<Sprite> ImportSprite(string path);
        Media<AudioClip> ImportAudioClip(string path);
        string ImportPack();
    }
}