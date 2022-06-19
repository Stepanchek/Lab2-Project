using System;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Serialization.Version3
{
    public enum EQuestionTypeV3
    {
        Normal,
        CatInPoke,
        Auction
    }

    public struct QuestionV3 : IRuntimeCreation<Question>
    {
        public int Price;
        public EQuestionTypeV3 Type;
        public string Text;
        public string Picture;
        public string Audio;
        public AnswerV3 Answer;

        public Question CreateRuntime(IPackImporter importer)
        {
            return new Question
            {
                Price = new ReactiveProperty<int>(Price),
                Type = new ReactiveProperty<EQuestionType>(QuestionTypeToType3()),
                Text = new ReactiveProperty<string>(Text),
                Picture = new ReactiveProperty<Media<Sprite>>(importer.ImportSprite(Picture)),
                Audio = new ReactiveProperty<Media<AudioClip>>(importer.ImportAudioClip(Audio)),
                Answer = Answer.CreateRuntime(importer),
            };
        }

        internal static QuestionV3 Export(Question question, IPackExporter exporter)
        {
            return new QuestionV3
            {
                Price = question.Price.Value,
                Type = QuestionType3ToType(question),
                Text = question.Text.Value,
                Picture = exporter.ExportSprite(question.Picture.Value),
                Audio = exporter.ExportAudioClip(question.Audio.Value),
                Answer = new AnswerV3
                {
                    Picture = exporter.ExportSprite(question.Answer.Picture.Value),
                    Text = question.Answer.Text.Value,
                },
            };
        }
        
        private EQuestionType QuestionTypeToType3()
        {
            return Type switch
            {
                EQuestionTypeV3.Normal => EQuestionType.Normal,
                EQuestionTypeV3.CatInPoke => EQuestionType.CatInPoke,
                EQuestionTypeV3.Auction => EQuestionType.Auction,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static EQuestionTypeV3 QuestionType3ToType(Question question)
        {
            return question.Type.Value switch
            {
                EQuestionType.Normal => EQuestionTypeV3.Normal,
                EQuestionType.CatInPoke => EQuestionTypeV3.CatInPoke,
                EQuestionType.Auction => EQuestionTypeV3.Auction,
                _ => throw new Exception($"Unknown question type: {question.Type}")
            };
        }
    }
}