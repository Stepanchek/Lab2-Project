using System.IO;

namespace PartyQuiz.Utils
{
    public sealed class ConstructorUtils
    {
        public static string GetTempPath(string path)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(path));

            AssetFileUtil.CopyAssetAsync(path, tempPath).HandleExceptions();

            return tempPath;
        }
    }
}