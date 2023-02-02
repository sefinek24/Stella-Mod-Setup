using System.IO;

namespace Genshin_Impact_Mod_Setup.Scripts
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

			string[] dirs = Directory.GetDirectories(Installation.Packages, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

			string path = "";
			foreach (string dir in dirs) path = dir;

			return path;
		}

		public static string GetProgramFiles()
		{
			if (!Directory.Exists(Installation.WindowsApps))
			{
				Log.Output($"{Installation.WindowsApps} was not found.");
				return null;
			}

			string[] dirs = Directory.GetDirectories(Installation.WindowsApps, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

			string path = "";
			foreach (string dir in dirs) path = dir;

			return path;
		}
	}
}