using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Serialization.Version2
{
    public struct AnswerV2 : IRuntimeCreation<Answer>
    {
        public string Text;
        public string Picture;

        public Answer CreateRuntime(IPackImporter importer)
        {
            return new Answer
            {
                Text = new ReactiveProperty<string>(Text),
                Picture = new ReactiveProperty<Media<Sprite>>(importer.ImportSprite(Picture)),
            };
        }
    }
}