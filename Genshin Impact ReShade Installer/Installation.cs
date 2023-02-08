using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Media;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Genshin_Impact_Mod_Setup.Scripts;
using IWshRuntimeLibrary;
using Microsoft.WindowsAPICodePack.Taskbar;
using File = System.IO.File;

namespace Genshin_Impact_Mod_Setup
{
	internal abstract class Installation
	{
		// Dependencies
		private const string Dependencies = @"Dependencies";
		public static readonly string InstalledViaSetup = Program.AppData + @"\installed-via-setup.sfn";
		private const string MainSetup = Dependencies + @"\Genshin Impact Mod Setup.exe";
		private const string WtWin10Setup = Dependencies + @"\WindowsTerminal_Win10.msixbundle";
		private const string WtWin11Setup = Dependencies + @"\WindowsTerminal_Win11.msixbundle";
		public static readonly string VcLibsSetup = Dependencies + @"\Microsoft.VCLibs.x64.14.00.Desktop.appx";

		// Other
		public const string Folder = @"C:\Genshin-Impact-ReShade";
		public static readonly string ProgramFiles64 = Environment.GetEnvironmentVariable("ProgramW6432");
		public static readonly string WindowsApps = ProgramFiles64 + @"\WindowsApps";
		public static readonly string Packages = Environment.GetEnvironmentVariable("LocalAppData") + @"\Packages";

		// Variables
		private static bool _wtBackupSkipped;
		private static string _wtSettings;
		private static string _wtLocalState;
		public static int ProcessInt = 1;
		public static int VcLibsAttemptNumber = 0;

		public static async Task Start()
		{
			TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
			TaskbarManager.Instance.SetProgressValue(5, 100);

			Console.WriteLine($"\n{Program.Line}");

			DateTime date = DateTime.Now;

			// Info
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine($"» Main server (sefinek.net): Piła, Poland                 » Start time: {date}\n» Proxy: WAW, FRA [Cloudflare]                            » Estimated time: ~1 minute\n");

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Installing, please wait. This may take a while. Do not use your computer during this process.\n");
			Console.ResetColor();


			if (RegionInfo.CurrentRegion.Name == "RU")
				try
				{
					SoundPlayer player = new SoundPlayer { SoundLocation = @"Data\sound.wav" };
					player.Play();
				}
				catch (Exception ex)
				{
					Log.ErrorAuditLog(ex, true);
				}


			// ----------------------- 1 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Preparing...");

			using (StreamWriter sw = File.AppendText(Log.OutputFile))
			{
				await sw.WriteLineAsync(
					$"⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                          Genshin Impact Mod Pack 2023 by Sefinek\n   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                   Installation started!\n⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆     » Estimated time: ~1 minute\n    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » Start time: {date}\n   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇\n  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟\n=========================================================================================\n" +
					//
					$"• Installation folder: {Folder}\n• Program files x64: {ProgramFiles64}\n• Windows apps: {WindowsApps}\n• Packages: {Packages}\n\n" +
					//
					$"• Main setup file: {MainSetup}\n• VCLibs setup: {VcLibsSetup}\n• Windows Terminal [Win 10] setup: {WtWin10Setup}\n• Windows Terminal [Win 11] setup: {WtWin11Setup}\n\n" +
					//
					$"• Game path: {Program.GamePath}\n• Game folder: {Program.GameDir}\n• ReShade config: {Program.ReShadeConfig}\n• ReShade log file: {Program.ReShadeLogFile}\n\n" +
					//
					$"• Attempt number: {Program.AttemptNumber}\n• VcLibs attempt: {VcLibsAttemptNumber}\n• Shortcut: {Program.ShortcutQuestion}\n• Menu shortcuts: {Program.MShortcutQuestion}\n" +
					"=========================================================================================\n\n");
			}

			// Ping
			bool connection = Internet.CheckConnection();
			if (!connection) return;

			// Send Discord WebHook
			WebHook.Installing();

			TaskbarManager.Instance.SetProgressValue(20, 100);


			// ----------------------- 2 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Installing Microsoft Visual C++ 2015-2022 Redistributable (x64)... Skipped");

			// if (!File.Exists(Redist64Setup))
			// 	Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Redist64Setup}"), false, false);

			// await Cmd.Execute(Redist64Setup, $"/install /quiet /norestart /log \"{Log.Folder}\\VC_redist.x64_installation.log\"", null);

			// Log.Output("Installed Microsoft Visual C++ 2015-2022 Redistributable (x64).");

			TaskbarManager.Instance.SetProgressValue(30, 100);


			// ----------------------- 3 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Installing Microsoft Visual C++ 2015-2022 Redistributable (x86)... Skipped");

			// if (!File.Exists(Redist86Setup))
			//	Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Redist86Setup}"), false, false);

			// await Cmd.Execute(Redist86Setup, $"/install /quiet /norestart /log \"{Log.Folder}\\VC_redist.x86_installation.log\"", null);

			// Log.Output("Installed Microsoft Visual C++ 2015-2022 Redistributable (x86).");

			TaskbarManager.Instance.SetProgressValue(40, 100);


			// ----------------------- 4 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Installing .NET Framework 4.8... Skipped");

			// if (!File.Exists(Ndp48Setup))
			// 	Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Ndp48Setup}"), false, false);

			// await Cmd.Execute(Ndp48Setup, $"/q /norestart /log \"{Log.Folder}\\NET-Framework48_installation\"", null);

			// Log.Output("Installed Microsoft .NET Framework 4.8.");

			TaskbarManager.Instance.SetProgressValue(50, 100);


			// ----------------------- 5 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Installing mod and our launcher in {Folder}...");

			if (!File.Exists(MainSetup))
				Log.ErrorAndExit(new Exception($"I can't find a required file.\n{MainSetup}"), false, false);

			await Cmd.Execute(MainSetup, $"/SILENT /NORESTART /LOG=\"{Log.Folder}\\mod_installation.log\"", null);

			if (!Directory.Exists(Folder))
				Log.ErrorAndExit(new Exception($"I can't find main mod directory in: {Folder}"), false, false);

			TaskbarManager.Instance.SetProgressValue(80, 100);


			// ----------------------- 6 -----------------------
			Console.Write($"{ProcessInt++}/11 - Backing up the Windows Terminal configuration file in app data... ");

			string wtAppData1 = Wt.GetAppData();
			if (string.IsNullOrEmpty(wtAppData1))
			{
				Console.WriteLine("Skipped");
				_wtBackupSkipped = true;
			}
			else
			{
				Console.WriteLine();

				_wtLocalState = $@"{wtAppData1}\LocalState";
				_wtSettings = $@"{_wtLocalState}\settings.json";
				string wtState = $@"{_wtLocalState}\state.json";
				string readmeFile = $@"{_wtLocalState}\README.txt";
				Log.Output($"Files and directories of backup.\n» wtAppData1: {wtAppData1}\n» _wtLocalState: {_wtLocalState}\n» _wtSettings: {_wtSettings}\n» wtState: {wtState}\n» readmeFile: {readmeFile}");

				try
				{
					using (StreamWriter sw = File.CreateText(readmeFile))
					{
						await sw.WriteAsync(
							$"⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                          Genshin Impact ReShade Mod Pack 2023\n   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                     Made by Sefinek\n⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆\n    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆\n   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇\n  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟\n=========================================================================================\n» Windows Terminal application configuration backup files from {date}.");
					}

					string zipFile = $@"{Program.AppData}\Windows Terminal\wt-config.backup-{DateTime.Now:HHmm_dd.MM.yyyy}.zip";
					Directory.CreateDirectory($@"{Program.AppData}\Windows Terminal");
					Zip.Create(_wtLocalState, zipFile);
					Log.Output($"The Windows Terminal application configuration files has been backed up.\n» Source: {_wtLocalState}\n» Backup: {zipFile}");
				}
				catch (Exception e)
				{
					Log.Error(e, false);
				}

				if (File.Exists(_wtSettings)) File.Delete(_wtSettings);
				if (File.Exists(wtState)) File.Delete(wtState);
				if (File.Exists(readmeFile)) File.Delete(readmeFile);
			}

			if (!_wtBackupSkipped)
			{
				WshShell shellBkp = new WshShell();
				string scPath = $@"{_wtLocalState}\WT Backup Folder.lnk";
				IWshShortcut shBkpWt = (IWshShortcut)shellBkp.CreateShortcut(scPath);
				shBkpWt.Description = "View backup folder.";
				shBkpWt.TargetPath = $@"{Program.AppData}\Windows Terminal";
				shBkpWt.Save();

				Log.Output($@"Created: {_wtLocalState}\WT Backup Folder.lnk");
			}
			else
			{
				Log.Output("Backup was skipped.");
			}

			TaskbarManager.Instance.SetProgressValue(60, 100);


			// ----------------------- 7 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Installing latest Windows Terminal...");

			if (!File.Exists(WtWin10Setup) || !File.Exists(WtWin11Setup))
				Log.ErrorAndExit(new Exception($"I can't find a required file.\n\n{WtWin10Setup} or {WtWin11Setup}"), false, false);

			Process[] dllHostName = Process.GetProcessesByName("dllhost");
			if (dllHostName.Length != 0) await Cmd.Execute("taskkill", "/F /IM dllhost.exe", null);
			Process[] wtName = Process.GetProcessesByName("WindowsTerminal");
			if (wtName.Length != 0) await Cmd.Execute("taskkill", "/F /IM WindowsTerminal.exe", null);

			if (Environment.OSVersion.Version.Build >= 22000)
			{
				await Cmd.Execute("powershell", $"Add-AppxPackage -Path {WtWin11Setup}", null);
				Log.Output($"Installed WT for Win 11: {WtWin11Setup}");
			}
			else
			{
				await Cmd.Execute("powershell", $"Add-AppxPackage -Path {WtWin10Setup}", null);
				Log.Output($"Installed WT for Win 10: {WtWin10Setup}");
			}

			TaskbarManager.Instance.SetProgressValue(70, 100);


			// ----------------------- 8 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Checking installed software...");

			string wtProgramFiles = Wt.GetProgramFiles();
			if (string.IsNullOrEmpty(wtProgramFiles))
			{
				Log.ErrorAndExit(new Exception($"Windows Terminal directory was not found in: {WindowsApps}"), false, false);
			}
			else
			{
				Log.Output($"Windows Terminal has been successfully installed in {wtProgramFiles}");

				string wtAppData2 = Wt.GetAppData();
				if (string.IsNullOrEmpty(wtAppData2)) Log.ErrorAndExit(new Exception("Fatal error. Code: 3781780149"), false, true);
				else _wtSettings = $@"{wtAppData2}\LocalState\settings.json";
			}

			TaskbarManager.Instance.SetProgressValue(75, 100);


			// ----------------------- 9 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Downloading config file for FPS Unlocker from cdn.sefinek.net... [~500 bytes]");

			try
			{
				if (!Directory.Exists($@"{Folder}\Data\Unlocker"))
					Directory.CreateDirectory($@"{Folder}\Data\Unlocker");

				string fpsUnlockCfgPath = $@"{Folder}\Data\Unlocker\unlocker.config.json";

				WebClient client = new WebClient();
				client.Headers.Add("user-agent", Program.UserAgent);
				await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/genshin-impact-reshade/unlocker-config", fpsUnlockCfgPath);

				string fpsUnlockerCfg = File.ReadAllText(fpsUnlockCfgPath);
				File.WriteAllText(fpsUnlockCfgPath, fpsUnlockerCfg.Replace("{GamePath}", Program.GamePath.Replace(@"\", @"\\")));
			}
			catch (Exception e)
			{
				Log.Error(e, false);
			}

			TaskbarManager.Instance.SetProgressValue(90, 100);


			// ----------------------- 10 -----------------------
			Console.Write($"{ProcessInt++}/11 - Downloading files from cdn.sefinek.net and configuring ReShade... ");

			if (Directory.Exists(Program.GameDir))
			{
				if (File.Exists(Program.ReShadeConfig))
				{
					File.Delete(Program.ReShadeConfig);
					Log.Output($"Removed old ReShade.ini file.\n» Path: {Program.ReShadeConfig}");
				}

				if (File.Exists(Program.ReShadeLogFile))
				{
					File.Delete(Program.ReShadeLogFile);
					Log.Output($"Removed old ReShade.log file.\n» Path: {Program.ReShadeLogFile}");
				}

				await ReShade.DownloadFiles(Program.ReShadeConfig, Program.ReShadeLogFile);
			}
			else
			{
				Console.WriteLine("You must configure ReShade manually");
				Log.Output("Configure ReShade manually.");
			}

			TaskbarManager.Instance.SetProgressValue(95, 100);


			// ----------------------- 11 -----------------------
			Console.WriteLine($"{ProcessInt++}/11 - Excellent! Finishing... ");

			if (Regex.Match(Program.ShortcutQuestion, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
				try
				{
					object shDesktop = "Desktop";
					WshShell shell = new WshShell();
					string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Genshin Impact Mod.lnk";
					IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);

					shortcut.Description = "Run official launcher made by Sefinek.";
					shortcut.IconLocation = $@"{Folder}\icons\52x52.ico";
					shortcut.WorkingDirectory = Folder;
					shortcut.TargetPath = $@"{Folder}\Genshin Impact Mod Launcher.exe";
					shortcut.Save();

					Log.Output("Desktop shortcut has been created.");
				}
				catch (Exception e)
				{
					Log.Error(e, false);
				}


			if (Regex.Match(Program.MShortcutQuestion, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
			{
				string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
				string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "Genshin Impact Mod Pack");
				if (!Directory.Exists(appStartMenuPath)) Directory.CreateDirectory(appStartMenuPath);

				try
				{
					string shortcutLocation = Path.Combine(appStartMenuPath, "Genshin Impact Mod Pack.lnk");
					WshShell shell = new WshShell();
					IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

					shortcut.Description = "Run official mod launcher made by Sefinek.";
					shortcut.IconLocation = $@"{Folder}\icons\52x52.ico";
					shortcut.WorkingDirectory = Folder;
					shortcut.TargetPath = $@"{Folder}\Genshin Impact Mod Launcher.exe";
					shortcut.Save();

					Log.Output("Start menu shortcut has been created.");


					using (StreamWriter writer = new StreamWriter($@"{appStartMenuPath}\Official website - Genshin Impact Mod Pack.url"))
					{
						await writer.WriteLineAsync("[InternetShortcut]\nURL=https://sefinek.net/genshin-impact-reshade");
					}

					using (StreamWriter writer = new StreamWriter($@"{appStartMenuPath}\Donate - Genshin Impact Mod Pack.url"))
					{
						await writer.WriteLineAsync("[InternetShortcut]\nURL=https://sefinek.net/support-me");
					}

					using (StreamWriter writer = new StreamWriter($@"{appStartMenuPath}\Gallery - Genshin Impact Mod Pack.url"))
					{
						await writer.WriteLineAsync("[InternetShortcut]\nURL=https://sefinek.net/genshin-impact-reshade/gallery?page=1");
					}

					using (StreamWriter writer = new StreamWriter($@"{appStartMenuPath}\Support - Genshin Impact Mod Pack.url"))
					{
						await writer.WriteLineAsync("[InternetShortcut]\nURL=https://sefinek.net/genshin-impact-reshade/support");
					}

					using (StreamWriter writer = new StreamWriter($@"{appStartMenuPath}\Leave feedback - Genshin Impact Mod Pack.url"))
					{
						await writer.WriteLineAsync("[InternetShortcut]\nURL=https://sefinek.net/genshin-impact-reshade/feedback");
					}

					Log.Output("Internet shortcuts has been created.");
				}
				catch (Exception e)
				{
					Log.Error(e, false);
				}
			}

			if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
			if (!File.Exists(InstalledViaSetup)) File.Create(InstalledViaSetup);

			// Done!
			await Finish.End();
		}
	}
}