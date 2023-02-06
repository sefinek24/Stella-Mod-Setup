using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genshin_Impact_Mod_Setup.Forms;
using Genshin_Impact_Mod_Setup.Scripts;

namespace Genshin_Impact_Mod_Setup
{
	internal abstract class Program
	{
		public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
		public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		public static readonly string AppData = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Genshin Impact MP by Sefinek";
		public const string AppWebsite = "https://sefinek.net/genshin-impact-reshade";
		public const string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

		// Other
		public const string Line = "===============================================================================================";
		public static readonly string UserAgent = $"Mozilla/5.0 (compatible; GenshinModSetup/{AppVersion}; +https://genshin.sefinek.net)";

		// Questions
		public static string ShortcutQuestion;
		public static string MShortcutQuestion;

		// Other
		public static string GamePath;
		public static string GameDir;
		public static string ReShadeConfig;
		public static string ReShadeLogFile;

		// Attempts
		public static int AttemptNumber = 1;

		public static void Close()
		{
			Console.WriteLine();
			if (Debugger.IsAttached) return;

			Console.ReadLine();
			Environment.Exit(0);
		}

		private static void Canceled()
		{
			Console.WriteLine($"\n{Line}");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("                         Operation canceled. You can close this window.");

			while (true) Console.ReadLine();
		}

		public static async Task WrongAnswer(string fileName)
		{
			Console.ResetColor();
			Console.WriteLine($"\n{Line}");

			AttemptNumber++;
			if (AttemptNumber >= 6)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("                       Too many attempts. Close this window and try again.");

				while (true) Console.ReadLine();
			}

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("                   Wrong answer. Expected 'y' or 'n'. Click ENTER to try again.");
			Console.ResetColor();
			Console.ReadLine();

			switch (fileName)
			{
				case "Start":
					Console.Clear();
					await Start.Main();
					break;
				case "Program":
					await Main();
					break;
				default:
					Log.ErrorAndExit(new Exception("Failed."), false, true);
					break;
			}
		}

		public static async Task Main()
		{
			if (File.Exists(Installation.InstalledViaSetup))
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("» Delete old program data [Yes/no]: ");
				Console.ResetColor();

				string deleteData = Console.ReadLine() ?? string.Empty;
				if (Regex.Match(deleteData, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success) Directory.Delete(AppData, true);
			}

			if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);


			// Questions etc...
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("» Create a desktop shortcut [Yes/no]: ");
			Console.ResetColor();
			ShortcutQuestion = Console.ReadLine();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("» Create new shortcuts in start menu [Yes/no]: ");
			Console.ResetColor();
			MShortcutQuestion = Console.ReadLine();

			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("» Enter game path: ");
			Console.ResetColor();

			if (!File.Exists($@"{AppData}\game-path.sfn"))
				File.WriteAllText($@"{AppData}\game-path.sfn", @"C:\Program Files\Genshin Impact\Genshin Impact game\GenshinImpact.exe");

			GamePath = File.ReadAllText($@"{AppData}\game-path.sfn").Trim();
			GameDir = Path.GetDirectoryName(GamePath);

			ReShadeConfig = $@"{GameDir}\ReShade.ini";
			ReShadeLogFile = $@"{GameDir}\ReShade.log";

			if (!Directory.Exists(GameDir) || !File.Exists(GamePath))
			{
				Application.Run(new SelectPath { Icon = Icon.ExtractAssociatedIcon("Data/Images/52x52.ico") });

				GamePath = File.ReadAllText($@"{AppData}\game-path.sfn");
				GameDir = Path.GetDirectoryName(GamePath);

				ReShadeConfig = $@"{GameDir}\ReShade.ini";
				ReShadeLogFile = $@"{GameDir}\ReShade.log";
			}

			if (Directory.Exists(GameDir) && File.Exists(GamePath))
			{
				File.WriteAllText($@"{AppData}\game-path.sfn", GamePath);
				Console.WriteLine(GameDir);
			}
			else
			{
				Console.WriteLine("Unknown");
			}


			// Are you ready?
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("» All is ready. Install now? [Yes/no]: ");
			Console.ResetColor();

			string answer = Console.ReadLine()?.ToLower().Trim();
			switch (answer)
			{
				case "y":
				case "yes":
					Log.Output("Initialization...");
					await Installation.Start();
					break;

				case "n":
				case "no":
					Canceled();
					break;

				default:
					await WrongAnswer("Program");
					break;
			}

			Console.ReadLine();
		}
	}
}