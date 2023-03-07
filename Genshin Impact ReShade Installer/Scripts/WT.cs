using System.IO;
using Genshin_Impact_Mod_Setup;

namespace Genshin_Stella_Mod_Setup.Scripts
{
    internal abstract class Wt
    {
        public static string GetAppData()
        {
            if (!Directory.Exists(Installation.Packages))
            {
                Log.Output($"{Installation.Packages} was not found.");
                return null;
            }

            var dirs = Directory.GetDirectories(Installation.Packages, "Microsoft.WindowsTerminal_*",
                SearchOption.AllDirectories);

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

            var dirs = Directory.GetDirectories(Installation.WindowsApps, "Microsoft.WindowsTerminal_*",
                SearchOption.AllDirectories);

            var path = "";
            foreach (var dir in dirs) path = dir;

            return path;
        }
    }
}
