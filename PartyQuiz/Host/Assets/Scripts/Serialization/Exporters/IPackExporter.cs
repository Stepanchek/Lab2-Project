using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Serialization.Exporters
{
    public interface IPackExporter
    {
        string ExportSprite(Media<Sprite> asset);
        string ExportAudioClip(Media<AudioClip> asset);
        void ExportPack(string packJson);
    }
}