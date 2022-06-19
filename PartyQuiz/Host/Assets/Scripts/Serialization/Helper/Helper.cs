using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PartyQuiz.Serialization.Exporters;
using PartyQuiz.Serialization.Importers;
using PartyQuiz.Structure.Runtime;
using UnityEngine;

namespace PartyQuiz.Serialization
{
    public static class Helper
    {
        public struct PackHeader
        {
            public int Version;
        }

        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static void Export(Pack plan, IPackExporter exporter)
        {
            var pack = new Version3.PackFileV3
            {
                Version = 3,
                Pack = Version3.PackV3.Export(plan, exporter)
            };

            exporter.ExportPack(JsonConvert.SerializeObject
            (
                value: pack,
                settings: JsonSerializerSettings
            ));
        }

        public static Pack Import(string path, bool saveTempAssets = false)
        {
            var importer = new RuntimeImporter(path, saveTempAssets);
            var pack = Import(importer);
            
            pack.Name.Value = Path.GetFileNameWithoutExtension(path);

            Debug.Log($"Pack is loaded from {path}");
            importer.Dispose();

            return pack;
        }

        public static Pack Import(IPackImporter importer)
        {
            var packFileJson = importer.ImportPack();
            var header = JsonConvert.DeserializeObject<PackHeader>(packFileJson);

            return header.Version switch
            {
                1 => JsonConvert.DeserializeObject<Version1.PackFileV1>(packFileJson).Pack.CreateRuntime(importer),
                2 => JsonConvert.DeserializeObject<Version2.PackFileV2>(packFileJson).Pack.CreateRuntime(importer),
                3 => JsonConvert.DeserializeObject<Version3.PackFileV3>(packFileJson).Pack.CreateRuntime(importer),
                _ => throw new System.Exception($"Unknown pack file version {header.Version}")
            };
        }
    }
}