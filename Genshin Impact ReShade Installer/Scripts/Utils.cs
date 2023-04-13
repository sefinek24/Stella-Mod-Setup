using System;
using System.IO;
using System.Text;

namespace Genshin_Stella_Setup.Scripts
{
    internal class Utils
    {
        private static readonly string[] Dirs = { "Data", "Dependencies", "Data/Images", "Data/Libs" };

        public static bool AnalyzeFiles()
        {
            foreach (var dir in Dirs)
            {
                if (Directory.Exists(dir)) continue;
                Log.SaveErrorLog(new Exception($"Required folder \"{dir}\" was not found."), true);

                return false;
            }

            return true;
        }

        public static string EncodeString(string input)
        {
            var result = new StringBuilder();
            foreach (var c in input)
                if (c >= 33 && c <= 126)
                {
                    var rotated = (char)((c - 33 + 47) % 94 + 33);
                    result.Append(rotated);
                }
                else
                {
                    result.Append(c);
                }

            return result.ToString();
        }

        public static string GetAppData()
        {
            if (!Directory.Exists(Installation.Packages))
            {
                Log.Output($"{Installation.Packages} was not found.");
                return null;
            }

            var dirs = Directory.GetDirectories(Installation.Packages, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

            var path = "";
            foreach (var dir in dirs) path = dir;

            return path;
        }

        public static string GetProgramFiles()
        {
            if (!Directory.Exists(Installation.WindowsApps))
            {
                Log.Output($"{Installation.WindowsApps} was not found.");
                return null;
            }

            var dirs = Directory.GetDirectories(Installation.WindowsApps, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

            var path = "";
            foreach (var dir in dirs) path = dir;

            return path;
        }
    }
}
