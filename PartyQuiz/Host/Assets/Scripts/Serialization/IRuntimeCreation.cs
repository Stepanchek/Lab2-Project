using PartyQuiz.Serialization.Importers;

namespace PartyQuiz.Serialization
{
    internal interface IRuntimeCreation<out T>
    {
        T CreateRuntime(IPackImporter importer);
    }
}