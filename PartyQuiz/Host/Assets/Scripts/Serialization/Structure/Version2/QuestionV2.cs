using System;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UniRx;
using UnityEngine;

namespace PartyQuiz.Serialization.Version2
{
    public enum EQuestionTypeV2
    {
        Normal,
        CatInPoke,
        Auction
    }

    public struct QuestionV2 : IRuntimeCreation<Question>
    {
        public int Price;
        public EQuestionTypeV2 Type;
        public string Text;
        public string Picture;
        public string Audio;
        public AnswerV2 Answer;

        public Question CreateRuntime(IPackImporter importer)
        {
            return new Question
            {
                Price = new ReactiveProperty<int>(Price),
                Type = new ReactiveProperty<EQuestionType>(QuestionTypeToType2()),
                Text = new ReactiveProperty<string>(Text),
                Picture = new ReactiveProperty<Media<Sprite>>(importer.ImportSprite(Picture)),
                Audio = new ReactiveProperty<Media<AudioClip>>(importer.ImportAudioClip(Audio)),
                Answer = Answer.CreateRuntime(importer),
            };
        }

        internal static QuestionV2 Export(Question question, IPackExporter exporter)
        {
            return new QuestionV2
            {
                Price = question.Price.Value,
                Type = QuestionType2ToType(question),
                Text = question.Text.Value,
                Picture = exporter.ExportSprite(question.Picture.Value),
                Audio = exporter.ExportAudioClip(question.Audio.Value),
                Answer = new AnswerV2
                {
                    Picture = exporter.ExportSprite(question.Answer.Picture.Value),
                    Text = question.Answer.Text.Value,
                },
            };
        }

        private EQuestionType QuestionTypeToType2()
        {
            return Type switch
            {
                EQuestionTypeV2.Normal => EQuestionType.Normal,
                EQuestionTypeV2.CatInPoke => EQuestionType.CatInPoke,
                EQuestionTypeV2.Auction => EQuestionType.Auction,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static EQuestionTypeV2 QuestionType2ToType(Question question)
        {
            return question.Type.Value switch
            {
                EQuestionType.Normal => EQuestionTypeV2.Normal,
                EQuestionType.CatInPoke => EQuestionTypeV2.CatInPoke,
                EQuestionType.Auction => EQuestionTypeV2.Auction,
                _ => throw new Exception($"Unknown question type: {question.Type}")
            };
        }
    }
}