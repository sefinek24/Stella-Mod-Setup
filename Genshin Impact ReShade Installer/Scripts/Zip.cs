using System;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Ionic.Zlib;

namespace Genshin_Stella_Mod_Setup.Scripts
{
    internal abstract class Zip
    {
        private static string GetCleanFolderName(string source, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return string.Empty;

            var result = filePath.Substring(source.Length);
            if (result.StartsWith("\\")) result = result.Substring(1);
            result = result.Substring(0, result.Length - new FileInfo(filePath).Name.Length);

            return result;
        }

        public static void Create(string source, string destination)
        {
            using (var zip = new ZipFile { CompressionLevel = CompressionLevel.BestCompression })
            {
                var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories)
                    .Where(f => Path.GetExtension(f).ToLowerInvariant() != ".zip").ToArray();

                foreach (var f in files) zip.AddFile(f, GetCleanFolderName(source, f));

                var destinationFilename = destination;
                if (Directory.Exists(destination) && !destination.EndsWith(".zip"))
                    destinationFilename +=
                        $"\\{new DirectoryInfo(source).Name}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffffff}.zip";

                zip.Save(destinationFilename);
            }
        }
    }
}

// Source: https://stackoverflow.com/a/50407976
