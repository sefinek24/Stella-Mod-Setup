using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Media;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Genshin_Stella_Setup.Scripts;
using IWshRuntimeLibrary;
using Microsoft.WindowsAPICodePack.Taskbar;
using File = System.IO.File;

namespace Genshin_Stella_Setup
{
    internal abstract class Installation
    {
        // Dependencies
        private const string Dependencies = "Dependencies";
        private const string MainSetup = Dependencies + @"\Genshin Stella Mod Setup.exe";
        private const string WtWin10Setup = Dependencies + @"\WindowsTerminal_Win10.msixbundle";
        private const string WtWin11Setup = Dependencies + @"\WindowsTerminal_Win11.msixbundle";

        // Other
        public const string Folder = @"C:\Genshin-Impact-ReShade";
        public const string VcLibsSetup = Dependencies + @"\Microsoft.VCLibs.x64.14.00.Desktop.appx";
        public static readonly string InstalledViaSetup = Program.AppData + @"\installed-via-setup.sfn";

        public static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        public static readonly string WindowsApps = ProgramFiles + @"\WindowsApps";
        public static readonly string Packages = Environment.GetEnvironmentVariable("LocalAppData") + @"\Packages";

        // Variables
        private static bool _wtBackupSkipped;
        private static string _wtSettings;
        private static string _wtLocalState;
        public static int ProcessInt = 1;
        public static int VcLibsAttemptNumber = 0;

        public static async Task Start()
        {
            Console.Clear();

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
            TaskbarManager.Instance.SetProgressValue(5, 100);

            var date = DateTime.Now;

            // Info
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"» Main server (sefinek.net): Piła, Poland                 » Start time: {date}\n» Proxy: WAW, FRA [Cloudflare]                            » Estimated time: ~1 minute\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Installing, please wait. This may take a while. Do not use your computer during this process.");
            Console.ResetColor();

            Console.WriteLine($"{Actions.Line}\n");


            if (RegionInfo.CurrentRegion.Name == "RU")
                try
                {
                    new SoundPlayer { SoundLocation = @"Data\sound.wav" }.Play();
                }
                catch (Exception ex)
                {
                    Log.SaveErrorLog(ex, true);
                }


            // ----------------------- 1 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Preparing...");

            using (var sw = File.AppendText(Log.OutputFile))
            {
                await sw.WriteLineAsync(
                    $"⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                          Genshin Impact Mod Pack 2023 by Sefinek\n   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                   Installation started!\n⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆     » Estimated time: ~1 minute\n    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » Start time: {date}\n   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇\n  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟\n" +
                    "=========================================================================================\n" +
                    //
                    $"• Installation folder: {Folder}\n" +
                    $"• Program files x64: {ProgramFiles}\n" +
                    $"• Windows apps: {WindowsApps}\n" +
                    $"• Packages: {Packages}\n\n" +
                    //
                    $"• Main setup file: {MainSetup}\n" +
                    $"• VCLibs setup: {VcLibsSetup}\n" +
                    $"• Windows Terminal [Win 10] setup: {WtWin10Setup}\n" +
                    $"• Windows Terminal [Win 11] setup: {WtWin11Setup}\n\n" +
                    //
                    $"• Game path: {Actions.GameDirGlobal}\n" +
                    $"• Game folder: {Actions.GameDirGlobal}\n" +
                    $"• ReShade config: {Actions.ReShadeConfig}\n" +
                    $"• ReShade log file: {Actions.ReShadeLogFile}\n\n" +
                    //
                    $"• Attempt number: {Actions.AttemptNumber}\n" +
                    $"• VCLibs attempt: {VcLibsAttemptNumber}\n" +
                    $"• Shortcut: {Actions.ShortcutQuestion}\n" +
                    $"• Menu shortcuts: {Actions.MShortcutQuestion}\n" +
                    "=========================================================================================\n\n"
                );
            }

            // Ping
            var connection = await Internet.CheckConnection();
            if (!connection) return;

            // Check access
            var getAccess = await Access.Get();
            if (!getAccess.Success)
            {
                Log.ErrorAndExit(new Exception("The server unexpectedly refused installation.\nThe authorization key may have expired. Please try again."), false, false);
                return;
            }

            // Post data
            await Telemetry.Post("Installation in progress. Please wait...");

            TaskbarManager.Instance.SetProgressValue(20, 100);


            // ----------------------- 2 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Installing Microsoft Visual C++ 2015-2022 Redistributable (x64)... Skipped");

            // if (!File.Exists(Redist64Setup))
            // 	Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Redist64Setup}"), false, false);

            // await Cmd.Execute(Redist64Setup, $"/install /quiet /norestart /log \"{Log.Folder}\\VC_redist.x64_installation.log\"", null);

            // Log.Output("Installed Microsoft Visual C++ 2015-2022 Redistributable (x64).");

            TaskbarManager.Instance.SetProgressValue(30, 100);


            // ----------------------- 3 -----------------------
            Console.WriteLine(
                $"{ProcessInt++}/12 - Installing Microsoft Visual C++ 2015-2022 Redistributable (x86)... Skipped");

            // if (!File.Exists(Redist86Setup))
            //	Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Redist86Setup}"), false, false);

            // await Cmd.Execute(Redist86Setup, $"/install /quiet /norestart /log \"{Log.Folder}\\VC_redist.x86_installation.log\"", null);

            // Log.Output("Installed Microsoft Visual C++ 2015-2022 Redistributable (x86).");

            TaskbarManager.Instance.SetProgressValue(35, 100);


            // ----------------------- 4 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Installing .NET Framework 4.8... Skipped");

            // if (!File.Exists(Ndp48Setup))
            // 	Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Ndp48Setup}"), false, false);

            // await Cmd.Execute(Ndp48Setup, $"/q /norestart /log \"{Log.Folder}\\NET-Framework48_installation\"", null);

            // Log.Output("Installed Microsoft .NET Framework 4.8.");

            TaskbarManager.Instance.SetProgressValue(40, 100);


            // ----------------------- 5 -----------------------
            Console.Write($"{ProcessInt++}/12 - Uninstalling previous version... ");

            if (File.Exists($"{Folder}/unins000.exe") && File.Exists($"{Folder}/unins000.dat"))
            {
                Console.WriteLine();

                await Cmd.Execute($"{Folder}/unins000.exe", $"/SILENT /NORESTART /LOG=\"{Log.Folder}\\innosetup-logs.uninstall.log\"", null);
            }
            else
            {
                Console.WriteLine("Skipped");
            }


            TaskbarManager.Instance.SetProgressValue(50, 100);


            // ----------------------- 6 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Installing mod and our launcher in {Folder}...");

            if (!File.Exists(MainSetup))
                Log.ErrorAndExit(new Exception($"I can't find a required file.\n{MainSetup}"), false, false);

            await Cmd.Execute(MainSetup, $"/SILENT /NORESTART /SETUP /LOG=\"{Log.Folder}\\innosetup-logs.install.log\"", null);

            if (!Directory.Exists(Folder))
                Log.ErrorAndExit(new Exception($"I can't find main mod directory in: {Folder}"), false, false);

            TaskbarManager.Instance.SetProgressValue(60, 100);


            // ----------------------- 7 -----------------------
            Console.Write($"{ProcessInt++}/12 - Backing up the Windows Terminal configuration file in app data... ");

            var wtAppData1 = Utils.GetAppData();
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
                var wtState = $@"{_wtLocalState}\state.json";
                var readmeFile = $@"{_wtLocalState}\README.txt";
                Log.Output($"Files and directories of backup.\n» wtAppData1: {wtAppData1}\n» _wtLocalState: {_wtLocalState}\n» _wtSettings: {_wtSettings}\n» wtState: {wtState}\n» readmeFile: {readmeFile}");

                try
                {
                    using (var sw = File.CreateText(readmeFile))
                    {
                        await sw.WriteAsync(
                            $"⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact Stella Mod Pack 2023\n   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                     Made by Sefinek\n⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆\n    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆\n   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇\n  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟\n=========================================================================================\n» Windows Terminal application configuration backup files from {date}.");
                    }

                    var zipFile = $@"{Program.AppData}\Windows Terminal\wt-config.backup-{DateTime.Now:HHmm_dd.MM.yyyy}.zip";
                    Directory.CreateDirectory($@"{Program.AppData}\Windows Terminal");
                    Zip.Create(_wtLocalState, zipFile);
                    Log.Output($"The Windows Terminal application configuration files has been backed up.\n» Source: {_wtLocalState}\n» Backup: {zipFile}");
                }
                catch (Exception e)
                {
                    Log.ThrowError(e, false);
                }

                if (File.Exists(_wtSettings)) File.Delete(_wtSettings);
                if (File.Exists(wtState)) File.Delete(wtState);
                if (File.Exists(readmeFile)) File.Delete(readmeFile);
            }

            if (!_wtBackupSkipped)
            {
                var shellBkp = new WshShell();
                var scPath = $@"{_wtLocalState}\WT Backup Folder.lnk";
                var shBkpWt = (IWshShortcut)shellBkp.CreateShortcut(scPath);
                shBkpWt.Description = "View backup folder.";
                shBkpWt.TargetPath = $@"{Program.AppData}\Windows Terminal";
                shBkpWt.Save();

                Log.Output($@"Created: {_wtLocalState}\WT Backup Folder.lnk");
            }
            else
            {
                Log.Output("Backup was skipped.");
            }

            TaskbarManager.Instance.SetProgressValue(70, 100);


            // ----------------------- 8 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Installing latest Windows Terminal...");

            if (!File.Exists(WtWin10Setup) || !File.Exists(WtWin11Setup))
                Log.ErrorAndExit(new Exception($"I can't find a required file.\n\n{WtWin10Setup} or {WtWin11Setup}\n\nPlease unpack all files from zip archive and try again."), false, false);

            var dllHostName = Process.GetProcessesByName("dllhost");
            if (dllHostName.Length != 0) await Cmd.Execute("taskkill", "/F /IM dllhost.exe", null);
            var wtName = Process.GetProcessesByName("WindowsTerminal");
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

            TaskbarManager.Instance.SetProgressValue(75, 100);


            // ----------------------- 9 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Checking installed software...");

            var wtProgramFiles = Utils.GetProgramFiles();
            if (string.IsNullOrEmpty(wtProgramFiles))
            {
                Log.ErrorAndExit(new Exception($"Windows Terminal directory was not found in: {WindowsApps}"), false, false);
            }
            else
            {
                Log.Output($"Windows Terminal has been successfully installed in {wtProgramFiles}");

                var wtAppData2 = Utils.GetAppData();
                if (string.IsNullOrEmpty(wtAppData2))
                    Log.ErrorAndExit(new Exception("Fatal error. Code: 3781780149"), false, true);
                else _wtSettings = $@"{wtAppData2}\LocalState\settings.json";
            }

            TaskbarManager.Instance.SetProgressValue(80, 100);


            // ----------------------- 10 -----------------------
            Console.WriteLine(
                $"{ProcessInt++}/12 - Downloading config file for FPS Unlocker from cdn.sefinek.net... [~500 bytes]");

            try
            {
                var unlockerFolderPath = Path.Combine(Folder, "data", "unlocker");
                if (!Directory.Exists(unlockerFolderPath))
                    Directory.CreateDirectory(unlockerFolderPath);

                string fpsUnlockerConfig;
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", Program.UserAgent);
                    fpsUnlockerConfig = await client.DownloadStringTaskAsync("https://cdn.sefinek.net/resources/v2/genshin-impact-reshade/unlocker/unlocker.config.json");
                }

                var fpsUnlockerConfigPath = Path.Combine(unlockerFolderPath, "unlocker.config.json");

                var gameExePath = Actions.GameExeGlobal?.Replace("\\", "\\\\");
                var fpsUnlockerConfigContent = fpsUnlockerConfig.Replace("{GamePath}", gameExePath ?? string.Empty);

                File.WriteAllText(fpsUnlockerConfigPath, fpsUnlockerConfigContent);
            }
            catch (Exception e)
            {
                Log.ThrowError(e, false);
            }


            TaskbarManager.Instance.SetProgressValue(90, 100);


            // ----------------------- 11 -----------------------
            Console.Write($"{ProcessInt++}/12 - Downloading files from cdn.sefinek.net and configuring ReShade... ");

            if (Directory.Exists(Actions.GameDirGlobal))
            {
                if (File.Exists(Actions.ReShadeConfig))
                {
                    File.Delete(Actions.ReShadeConfig);
                    Log.Output($"Removed old ReShade.ini file.\n» Path: {Actions.ReShadeConfig}");
                }

                if (File.Exists(Actions.ReShadeLogFile))
                {
                    File.Delete(Actions.ReShadeLogFile);
                    Log.Output($"Removed old ReShade.log file.\n» Path: {Actions.ReShadeLogFile}");
                }

                await ReShade.DownloadFiles();
            }
            else
            {
                Console.WriteLine("You must configure ReShade manually");
                Log.Output("Configure ReShade manually.");
            }

            TaskbarManager.Instance.SetProgressValue(99, 100);


            // ----------------------- 12 -----------------------
            Console.WriteLine($"{ProcessInt++}/12 - Excellent! Finishing... ");

            // Create shortcut on Desktop
            if (Regex.IsMatch(Actions.ShortcutQuestion, "(?:y)", RegexOptions.IgnoreCase))
                try
                {
                    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                    var shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");

                    var shell = new WshShell();
                    var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.Description = "Run official launcher for Genshin Impact Mod made by Sefinek.";
                    shortcut.IconLocation = Path.Combine(Folder, "icons", "52x52.ico");
                    shortcut.WorkingDirectory = Folder;
                    shortcut.TargetPath = Path.Combine(Folder, "Genshin Stella Mod.exe");

                    shortcut.Save();
                    Log.Output("Desktop shortcut has been created.");
                }
                catch (Exception e)
                {
                    Log.ThrowError(e, false);
                }


            // Start menu
            if (Regex.IsMatch(Actions.MShortcutQuestion, "^y$", RegexOptions.IgnoreCase))
            {
                var appStartMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Genshin Stella Mod");
                Directory.CreateDirectory(appStartMenuPath);

                try
                {
                    // Create shortcut in Start Menu
                    var shell = new WshShell();
                    var shortcutLocation = Path.Combine(appStartMenuPath, "Genshin Stella Mod.lnk");
                    var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
                    shortcut.Description = "Run official Genshin Stella Mod Launcher made by Sefinek.";
                    shortcut.IconLocation = Path.Combine(Folder, "icons", "52x52.ico");
                    shortcut.WorkingDirectory = Folder;
                    shortcut.TargetPath = Path.Combine(Folder, "Genshin Stella Mod.exe");
                    shortcut.Save();
                    Log.Output("Start menu shortcut has been created.");

                    // Create Internet shortcuts
                    var urls = new Dictionary<string, string>
                    {
                        { "Official website", "https://genshin.sefinek.net" },
                        { "Donate", "https://sefinek.net/support-me" },
                        { "Gallery", "https://sefinek.net/genshin-impact-reshade/gallery?page=1" },
                        { "Support", "https://sefinek.net/genshin-impact-reshade/support" },
                        { "Leave feedback", "https://sefinek.net/genshin-impact-reshade/feedback" }
                    };
                    foreach (var kvp in urls)
                    {
                        var url = Path.Combine(appStartMenuPath, $"{kvp.Key} - Genshin Stella Mod.url");
                        using (var writer = new StreamWriter(url))
                        {
                            await writer.WriteLineAsync($"[InternetShortcut]\nURL={kvp.Value}");
                            Log.Output($"Created new Internet shortcut: {kvp.Value}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.ThrowError(e, false);
                }
            }


            if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
            if (!File.Exists(InstalledViaSetup)) File.Create(InstalledViaSetup);

            // Done!
            await Finish.End();
        }
    }
}
