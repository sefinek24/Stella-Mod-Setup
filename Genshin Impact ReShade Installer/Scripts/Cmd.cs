using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Genshin_Stella_Setup.Scripts
{
    internal abstract class Cmd
    {
        public static bool RebootNeeded;

        public static async Task Execute(string app, string args, string workingDir)
        {
            Log.Output($"Execute command: {app} {args} {workingDir}");

            try
            {
                var action = Cli.Wrap(app)
                    .WithArguments(args)
                    .WithWorkingDirectory(workingDir)
                    .WithValidation(CommandResultValidation.None);
                var result = await action.ExecuteBufferedAsync();

                // Variables
                var stdout = result.StandardOutput;
                var stderr = result.StandardError;

                // StandardOutput
                var stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
                var stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
                Log.Output($"Successfully executed {app} command. Exit code: {result.ExitCode}, start time: {result.StartTime}, exit time: {result.ExitTime}{stdoutLine}{stderrLine}");

                // StandardError
                if (result.ExitCode != 0)
                {
                    var showCommand = !string.IsNullOrEmpty(app) ? $"\n\n» Executed command:\n{app} {args}" : "";
                    var showWorkingDir = !string.IsNullOrEmpty(workingDir)
                        ? $"\n\n» Working directory: {workingDir}"
                        : "";
                    var showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
                    var showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error [stderr]:\n{stderr}" : "";
                    var info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";


                    // VcLibs
                    if (Installation.VcLibsAttemptNumber >= 3)
                    {
                        Log.ErrorAndExit(
                            new Exception(
                                $"Command execution failed because the underlying process (PowerShell) returned a non-zero exit code - {result.ExitCode}.\nI can't install this required package. Reboot your PC or close all opened apps and try again.{info}"),
                            false, false);
                        return;
                    }

                    var wtProcess1 = Process.GetProcessesByName("WindowsTerminal");
                    if (wtProcess1.Length != 0) await Execute("taskkill", "/F /IM WindowsTerminal.exe", null);

                    if (Regex.Match(stderr, "(?:80073D02)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            $"     » We cannot install this package because some process is currently in use.\n       Reboot your PC or close all opened apps from Microsoft Store.\n\n{stderr}");

                        Log.SaveErrorLog(new Exception($"We cannot install this package because some process is currently in use.\n\n» Attempt: {Installation.VcLibsAttemptNumber}\n» Exit code: 80073D02\n\n{stderr}"),
                            true);
                        Log.Output($"I can't install VCLibs because some process is currently in use. Attempt: {Installation.VcLibsAttemptNumber}");

                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("     » Click ENTER to try again...");
                        Console.ReadLine();

                        Log.Output("Restarting session...");
                        Console.ResetColor();
                        await Installation.Start();
                        return;
                    }

                    if (Regex.Match(stderr, "(?:80073CF3|Microsoft.VCLibs.)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
                    {
                        Installation.VcLibsAttemptNumber++;

                        Log.Output($"Found missing dependency VCLibs. Attempt {Installation.VcLibsAttemptNumber}.");
                        Log.SaveErrorLog(new Exception($"Found missing dependency Microsoft.VCLibs.\n\nAttempt {Installation.VcLibsAttemptNumber}\nExit code: 80073CF3\n\n{stderr}"), true);

                        try
                        {
                            new ToastContentBuilder()
                                .AddText("Ughh, sorry. We need more time 😥")
                                .AddText("Found missing dependency with name VCLibs.\nClose all Microsoft Store apps and go back to the setup!")
                                .Show();
                        }
                        catch (Exception ex)
                        {
                            Log.SaveErrorLog(ex, true);
                            return;
                        }

                        // Preparing...
                        Console.WriteLine($"{Installation.ProcessInt++}/11 - Preparing to install Microsoft Visual C++ 2015 UWP Desktop Package (attempt {Installation.VcLibsAttemptNumber}/3)...");

                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("     » ATTENTION: Close all Microsoft Store apps and press ENTER to continue...");
                        Console.ResetColor();

                        Console.ReadLine();

                        // Close apps
                        var dllHostProcess = Process.GetProcessesByName("dllhost");
                        if (dllHostProcess.Length != 0) await Execute("taskkill", "/F /IM dllhost.exe", null);
                        var wtProcess2 = Process.GetProcessesByName("WindowsTerminal");
                        if (wtProcess2.Length != 0) await Execute("taskkill", "/F /IM WindowsTerminal.exe", null);

                        // Installing...
                        Console.WriteLine($"{Installation.ProcessInt++}/11 - Installing Microsoft Visual C++ 2015 UWP Desktop Package...");

                        if (!File.Exists(Installation.VcLibsSetup))
                            Log.ErrorAndExit(new Exception($"I can't find a required file. Please unpack downloaded zip archive.\nNot found: {Installation.VcLibsSetup}"), false, false);

                        Log.Output("Installing missing dependency VCLibs...");
                        await Execute("powershell", $"Add-AppxPackage -Path {Installation.VcLibsSetup}", null);

                        // Throw info
                        await Telemetry.Post("Successfully installed VCLibs.");
                        try
                        {
                            new ToastContentBuilder()
                                .AddText("First part was finished 🎉")
                                .AddText("VCLibs has been successfully installed, but now we need to restart your computer.")
                                .Show();
                        }
                        catch (Exception ex)
                        {
                            Log.SaveErrorLog(ex, true);
                        }

                        // Completed!
                        Log.Output("Installed Microsoft Visual C++ 2015 UWP Desktop Package.");
                        Console.WriteLine("      » Successfully! Please reboot your PC and open the installer again!\n");

                        // Reboot PC
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("» Restart your computer now? This is required. [Yes/no]: ");
                        Console.ResetColor();

                        var rebootPc = Console.ReadLine();
                        if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
                        {
                            await Execute("shutdown",
                                $"/r /t 25 /c \"{Program.AppName} - scheduled reboot, version {Program.AppVersion}.\n\nAfter restarting, run the installer again. If you need help, add me on Discord Sefinek#0001.\n\nGood luck!\"",
                                null);

                            Console.WriteLine("Your computer will restart in 25 seconds. Save your work!\nAfter restarting, run the installer again.");
                            Log.Output("PC reboot was scheduled. Installed VCLibs.");

                            await Telemetry.Post("Reboot was scheduled.");
                        }
                        else
                        {
                            Console.WriteLine("Woaah, okay!");
                        }

                        while (true) Console.ReadLine();
                    }


                    switch (result.ExitCode)
                    {
                        case 3010:
                        {
                            try
                            {
                                new ToastContentBuilder()
                                    .AddText("Installation alert 📄")
                                    .AddText("Required dependency has been successfully installed, but your computer needs a restart. Please wait to complete installation.")
                                    .Show();
                            }
                            catch (Exception ex)
                            {
                                Log.SaveErrorLog(ex, true);
                            }

                            Log.Output($"{app} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted.");

                            RebootNeeded = true;
                            return;
                        }

                        case 5:
                            Log.ErrorAndExit(
                                new Exception(
                                    $"Software was denied access to a location for the purposes of saving, copying, opening, or loading files.\nRestart your computer or suspend antivirus program and try again.{info}"),
                                false, false);
                            return;

                        default:
                        {
                            Log.ErrorAndExit(
                                new Exception(
                                    $"Command execution failed because the underlying process ({app.Replace(@"Dependencies\", "").Replace(@"C:\Program Files\Git\cmd\", "")}) returned a non-zero exit code - {result.ExitCode}.\nCheck your Internet connection, antivirus program or restart PC and try again.{info}"),
                                false, result.ExitCode != 128);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorAndExit(e, false, true);
            }
        }
    }
}
