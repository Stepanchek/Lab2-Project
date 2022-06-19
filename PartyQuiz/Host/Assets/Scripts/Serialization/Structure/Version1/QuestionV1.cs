using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Serialization.Version1
{
    public struct QuestionV1 : IRuntimeCreation<Question>
    {
        public int Price;
        public bool IsCatInPoke;
        public string Text;
        public string Picture;
        public string Audio;
        public AnswerV1 Answer;

        public Question CreateRuntime(IPackImporter importer)
        {
            return new Question
            {
                Price = new ReactiveProperty<int>(Price),
                Type = new ReactiveProperty<EQuestionType>(IsCatInPoke ? EQuestionType.CatInPoke : EQuestionType.Normal),
                Text = new ReactiveProperty<string>(Text),
                Picture = new ReactiveProperty<Media<Sprite>>(importer.ImportSprite(Picture)),
                Audio = new ReactiveProperty<Media<AudioClip>>(importer.ImportAudioClip(Audio)),
                Answer = Answer.CreateRuntime(importer),
            };
        }

        internal static QuestionV1 Export(Question question, IPackExporter exporter)
        {
            return new QuestionV1
            {
                Price = question.Price.Value,
                IsCatInPoke = question.Type.Value == EQuestionType.CatInPoke,
                Text = question.Text.Value,
                Picture = exporter.ExportSprite(question.Picture.Value),
                Audio = exporter.ExportAudioClip(question.Audio.Value),
                Answer = new AnswerV1
                {
                    Picture = exporter.ExportSprite(question.Answer.Picture.Value),
                    Text = question.Answer.Text.Value,
                },
            };
        }
    }
}