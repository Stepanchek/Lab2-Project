using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PartyQuiz.Serialization.Version3;
using PartyQuiz.Utils;
using Unity.VisualScripting.IonicZip;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using SFB;

namespace PartyQuiz.Siq
{
    public sealed class SiqConverter : MonoBehaviour
    {
        private static Zipper _zipper;

#if UNITY_EDITOR
        [MenuItem("Tools/SiqConverter")]
#endif
        public static void Convert()
        {
            var path = StandaloneFileBrowser
                .OpenFilePanel("Open siq file", "", "siq", false)[0];

            var packPath =
                StandaloneFileBrowser.SaveFilePanel("Save pack", Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path), "pq");

            ConvertSiqFile(path, packPath);
        }

        public static void ConvertSiqFile(string path, string packPath)
        {
            _zipper = new Zipper();

            using (var zip = ZipFile.Read(path))
            {
                var content = zip["content.xml"];
                var stream = new MemoryStream();

                content.Extract(stream);
                stream.Position = 0;

                var reader = new XmlReaderNoNamespaces(stream);
                var serializer = new XmlSerializer(typeof(SiqData));
                var package = (SiqData)serializer.Deserialize(reader);

                stream.Dispose();

                var pack = new PackFileV3
                {
                    Version = 3,
                    Pack = new PackV3
                    {
                        Rounds = package.Rounds.Round.Select(round => new PartyQuiz.Serialization.Version3.RoundV3
                        {
                            Type = GetRoundType(round.Type),
                            Themes = round.Themes.Theme.Select(theme => new PartyQuiz.Serialization.Version3.ThemeV3
                            {
                                Name = theme.Name,
                                Questions = theme.Questions.Question.Where(q => q.Scenario.Atom[0].Type != "video")
                                    .Select(question => new PartyQuiz.Serialization.Version3.QuestionV3
                                    {
                                        Price = GetPrice(question),
                                        Text = GetText(question),
                                        Picture = GetPicture(zip, question),
                                        Audio = GetAudio(zip, question),
                                        Answer = GetAnswer(zip, question, question.Right.Answer),
                                        Type = GetQuestionType(question.Type)
                                    }).ToList()
                            }).ToList()
                        }).ToList()
                    }
                };

                var json = JsonConvert.SerializeObject(pack, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                _zipper.SaveToFile(json, packPath);
            }

            Debug.Log($"Export {Path.GetFileName(path)} done!");
        }

        private static int GetPrice(Question question)
        {
            return int.Parse(question.Price);
        }

        [CanBeNull]
        private static string GetText(Question question)
        {
            foreach (var atom in question.Scenario.Atom)
            {
                if (atom.Type == null)
                    return atom.Text;

                if (atom.Type == "marker")
                    return null;
            }

            return null;
        }

        [CanBeNull]
        private static string GetPicture(ZipFile zip, Question question)
        {
            foreach (var atom in question.Scenario.Atom)
            {
                if (atom.Type == "image")
                    return ExportAsset(zip, "Images", atom.Text, "pics");

                if (atom.Type == "marker")
                    return null;
            }

            return null;
        }

        [CanBeNull]
        private static string GetAudio(ZipFile zip, Question question)
        {
            foreach (var atom in question.Scenario.Atom)
            {
                if (atom.Type == "voice")
                    return ExportAsset(zip, "Audio", atom.Text, "audio");

                if (atom.Type == "marker")
                    return null;
            }

            return null;
        }

        private static AnswerV3 GetAnswer(ZipFile zip, Question question, string answer)
        {
            var answerData = new AnswerV3
            {
                Text = answer
            };

            var canCountAnswer = false;

            foreach (var atom in question.Scenario.Atom)
            {
                if (atom.Type == "marker")
                {
                    canCountAnswer = true;
                    continue;
                }

                if (canCountAnswer == false)
                    continue;

                if (atom.Type == "image")
                    answerData.Picture = ExportAsset(zip, "Images", atom.Text, "pics");
            }

            return answerData;
        }

        private static ERoundTypeV3 GetRoundType([CanBeNull] string type)
        {
            if (string.IsNullOrEmpty(type))
                return ERoundTypeV3.Normal;

            return type switch
            {
                "final" => ERoundTypeV3.Final,
                _ => ERoundTypeV3.Normal
            };
        }
        
        private static EQuestionTypeV3 GetQuestionType([CanBeNull] Type type)
        {
            if (type == null)
                return EQuestionTypeV3.Normal;

            switch (type.Name)
            {
                case "cat":
                case "bagcat":
                    return EQuestionTypeV3.CatInPoke;
                case "sponsored":
                case "auction":
                    return EQuestionTypeV3.Auction;
            }
            
            Debug.LogError("Unknown question type: " + type.Name);
            return EQuestionTypeV3.Normal;
        }

        private static string ExportAsset(ZipFile zip, string type, string assetName, string directoryName)
        {
            assetName = Uri.EscapeUriString(assetName[1..]);

            Debug.Log($"Export {assetName}");

            var content = zip[@$"{type}/{assetName}"];

            var stream = new MemoryStream();
            content.Extract(stream);

            _zipper.AddStream(@$"{directoryName}/{assetName}", stream, string.Empty);

            return directoryName + "/" + assetName;
        }
    }
}