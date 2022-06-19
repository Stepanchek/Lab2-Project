using SFB;
using UnityEditor;

namespace PartyQuiz.Serialization
{
    public static class ImportPackEditor
    {
        [MenuItem("Tools/Import Pack")]
        public static void Import()
        {
            var packDirectory = StandaloneFileBrowser.OpenFilePanel("Open pack file", string.Empty, "pq", false)[0];
            Helper.Import(packDirectory);
        }
    }
}