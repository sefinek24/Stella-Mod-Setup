using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Genshin_Impact_MP_Installer.Scripts
{
	internal abstract class Cmd
	{
		public static bool RebootNeeded;

		public static async Task Execute(string app, string args, string workingDir)
		{
			try
			{
				Log.Output($"» Executed command: {app} {args}\n» Working directory: {workingDir}");

				Command action = Cli.Wrap(app).WithArguments(args).WithWorkingDirectory(workingDir).WithValidation(CommandResultValidation.None);
				BufferedCommandResult result = await action.ExecuteBufferedAsync();

				// Variables
				string stdout = result.StandardOutput;
				string stderr = result.StandardError;

				// StandardOutput
				Log.Output($"» Application: {app}\n» Exit code: {result.ExitCode}\n» Start time: {result.StartTime}\n» Exit time: {result.ExitTime}\n\n✅ STDOUT: {stdout}\n❌ STDERR: {stderr}");

				// StandardError
				if (result.ExitCode != 0)
				{
					string showCommand = !string.IsNullOrEmpty(app) ? $"\n\n» Executed command:\n{app} {args}" : "";
					string showWorkingDir = !string.IsNullOrEmpty(workingDir) ? $"\n\n» Working directory: {workingDir}" : "";
					string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
					string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error [stderr]:\n{stderr}" : "";
					string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";


					// VcLibs
					if (Installation.VcLibsAttemptNumber >= 3)
					{
						Log.ErrorAndExit(
							new Exception(
								$"Command execution failed because the underlying process (PowerShell) returned a non-zero exit code - {result.ExitCode}.\nI cannot install this required package. Reboot your PC or close all opened apps and try again.{info}"),
							false, false);
						return;
					}

					if (Regex.Match(stderr, "(?:80073D02)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("     » Something went wrong. I can't install this package!\n       Reboot your PC or close all opened apps from Microsoft Store.\n\n{stderr}");

						Log.ErrorAuditLog(new Exception($"I can't install Microsoft.VCLibs (attempt {Installation.VcLibsAttemptNumber}).\n\n{stderr}"), true);

						TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);

						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine("     » Click ENTER to try again...");
						Console.ReadLine();

						Console.ResetColor();
						await Installation.Start();
						return;
					}

					if (Regex.Match(stderr, "(?:80073CF3|Microsoft.VCLibs.)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
					{
						Installation.VcLibsAttemptNumber++;

						Log.Output($"Found missing dependency VCLibs. Attempt {Installation.VcLibsAttemptNumber}.");
						Log.ErrorAuditLog(new Exception($"Found missing dependency Microsoft.VCLibs (attempt {Installation.VcLibsAttemptNumber}).\n\n{stderr}"), true);

						try
						{
							ToastContentBuilder builder = new ToastContentBuilder().AddText("Ughh, sorry. We need more time 😥").AddText("Found missing dependency with name VCLibs.\nClose all Microsoft Store apps and go back to the installer!");
							builder.Show();
						}
						catch (Exception ex)
						{
							Log.ErrorAuditLog(ex, true);
						}

						// Preparing...
						Console.WriteLine($"{Installation.ProcessInt++}/20 - Preparing to install Microsoft Visual C++ 2015 UWP Desktop Package (attempt {Installation.VcLibsAttemptNumber}/3)...");

						TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine("     » ATTENTION: Close all Microsoft Store apps and press ENTER to continue...");
						Console.ResetColor();

						Console.ReadLine();

						// Close apps
						Process[] dllHostName = Process.GetProcessesByName("dllhost");
						if (dllHostName.Length != 0) await Execute("taskkill", "/F /IM dllhost.exe", null);
						Process[] wtName = Process.GetProcessesByName("WindowsTerminal");
						if (wtName.Length != 0) await Execute("taskkill", "/F /IM WindowsTerminal.exe", null);

						// Installing...
						Console.WriteLine($"{Installation.ProcessInt++}/20 - Installing Microsoft Visual C++ 2015 UWP Desktop Package...");

						if (!File.Exists(Installation.VcLibsSetup))
							Log.ErrorAndExit(new Exception($"I can't find a required file.\n{Installation.VcLibsSetup}"), false, false);

						Log.Output("Installing missing dependency VCLibs...");
						await Execute("powershell", $"Add-AppxPackage -Path {Installation.VcLibsSetup}", null);

						// Done!
						Log.Output("Installed Microsoft Visual C++ 2015 UWP Desktop Package.");

						Console.WriteLine("      » Successfully! Please reboot your PC and open the installer again!\n");

						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write("» Restart your computer now? This is required. [Yes/no]: ");
						Console.ResetColor();

						try
						{
							ToastContentBuilder builder = new ToastContentBuilder().AddText("First part was finished 🎉").AddText("VCLibs has been successfully installed, but now we need to restart your computer.");
							builder.Show();
						}
						catch (Exception ex)
						{
							Log.ErrorAuditLog(ex, true);
						}

						WebHook.InstalledVcLibs();

						string rebootPc = Console.ReadLine();
						if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
						{
							await Execute("shutdown", $"/r /t 25 /c \"{Program.AppName} - scheduled reboot, version {Program.AppVersion}.\n\nAfter restarting, run the installer again. If you need help, add me on Discord Sefinek#0001.\n\nGood luck!\"",
								null);

							Console.WriteLine("Your computer will restart in 25 seconds. Save your work!\nAfter restarting, run the installer again.");
							Log.Output("PC reboot was scheduled. Installed VCLibs.");

							WebHook.RebootIsScheduled();
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
								ToastContentBuilder builder = new ToastContentBuilder().AddText("Installation alert 📄")
									.AddText("Required dependency has been successfully installed, but your computer needs a restart. Please wait to complete installation.");
								builder.Show();
							}
							catch (Exception ex)
							{
								Log.ErrorAuditLog(ex, true);
							}

							Log.Output($"{app} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted.");

							RebootNeeded = true;
							return;
						}

						case 5:
							Log.ErrorAndExit(new Exception($"Software was denied access to a location for the purposes of saving, copying, opening, or loading files.\nRestart your computer or suspend antivirus program and try again.{info}"), false, false);
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