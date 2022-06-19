using System.IO;
using System.Threading.Tasks;

namespace PartyQuiz.Utils
{
    public static class AssetFileUtil
    {
        internal static async Task CopyAssetAsync(string sourcePath, string destinationPath)
        {
            await using Stream source = File.Open(sourcePath, FileMode.Open);
            await using Stream destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);
        }

        public static string SaveAsset(byte[] bytes, string fileName)
        {
            var tempPath = Path.GetTempPath();
            var assetPath = Path.Combine(tempPath, fileName);

            File.WriteAllBytesAsync(assetPath, bytes).HandleExceptions();
            return assetPath;
        }
    }
}