using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genshin_Impact_MP_Installer.Models;
using Genshin_Impact_MP_Installer.Scripts;
using Newtonsoft.Json;

namespace Genshin_Impact_MP_Installer
{
    internal abstract class Start
    {
        private static readonly string[] Dirs =
        {
            "Data", "Dependencies", @"Data\Images", @"Data\Lib"
        };

        private static bool AppReady()
        {
            foreach (var dir in Dirs)
            {
                if (Directory.Exists(dir)) continue;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Required folder \"{dir}\" was not found.");

                return false;
            }

            return true;
        }

        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                           Genshin Impact Mod Pack 2023 - Early access");
            Console.WriteLine($"                                       Version: v{Program.AppVersion}\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("» Author  : Sefinek#0001 [Country: Poland]");
            Console.WriteLine("» Website : " + Program.AppWebsite);
            Console.WriteLine("» Discord : " + Program.DiscordUrl);
            Console.ResetColor();
            Console.WriteLine(Program.Line);

            Console.Title = $"{Program.AppName} • v{Program.AppVersion}";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                             x Your game files WILL BE NOT modified x");
            Console.WriteLine("                         x If you need help, join to my Discord server! x");
            Console.ResetColor();
            Console.WriteLine();


            // 1
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Checking for new updates... ");
            Console.ResetColor();

            try
            {
                var client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                var json = client.DownloadString(
                    "https://api.sefinek.net/api/v1/genshin-impact-reshade/installer/version");
                var res = JsonConvert.DeserializeObject<InstallerVersion>(json);

                if (res.Version != Program.AppVersion)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Warning\n");
                    Console.ResetColor();

                    Console.WriteLine(
                        $"This setup is outdated. Please download the latest version from official website.\n{Program.AppWebsite}\n\n" +
                        $"• Your version: v{Program.AppVersion}\n" +
                        $"• Latest version: v{res.Version} from {res.LastUpdate}\n"
                    );

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("» Open official website now? [Yes/no]: ");
                    Console.ResetColor();
                    var websiteQuestion = Console.ReadLine()?.ToLower();
                    switch (websiteQuestion)
                    {
                        case "y":
                        case "yes":
                            Console.Write("Opening... ");
                            Process.Start(Program.AppWebsite);
                            Console.WriteLine("Done");
                            break;
                        case "n":
                        case "no":
                            Console.WriteLine("Canceled. You can close this window.");
                            break;
                        default:
                            await Program.WrongAnswer();
                            break;
                    }

                    Program.Close();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                }
            }
            catch (WebException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed [attempt {Program.AttemptNumber}/8]\n");
                Console.ResetColor();

                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse response)
                {
                    if ((int)response.StatusCode == 503)
                        Log.ErrorString(
                            "» Error 503. We can't search for new updates because the web server is unavailable.\n» Sorry for any problems.\n\n• ENG:\nThe server cannot handle the request because it is overloaded or down for maintenance.\nGenerally, this is a temporary state.\n\n• POL:\nSerwer nie może obsłużyć żądania, ponieważ jest przeciążony lub wyłączony z powodu konserwacji.\nZ reguły jest to stan tymczasowy.\n",
                            true);
                    else if ((int)response.StatusCode >= 500)
                        Log.ErrorString(
                            $"Oh, sorry. We can't search for new updates. Report this issue on our Discord server.\n\n• Error: {ex.Message}\n",
                            true);
                    else if ((int)response.StatusCode == 403)
                        Log.ErrorString(
                            $"Oh, sorry. We can't search for new updates. Probably your address IP is banned.\nAccess to the requested resource is forbidden. The server understood the request, but will not fulfill it.\n\n• Error:\n{ex.Message}\n",
                            true);
                    else
                        Log.ErrorString(
                            $"Oh, sorry. We can't search for new updates.\nPlease check your Internet connection or report this issue on our Discord server.\n\n• Error:\n{ex.Message}\n",
                            true);
                }
                else
                {
                    Log.ErrorString(
                        $"Sorry. We can't search for new updates. Unknown error code. Please check your Internet connection.\n\n• Error:\n{ex.Message}\n",
                        true);
                }

                Program.AttemptNumber++;
                if (Program.AttemptNumber >= 9)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("                    Too many attempts... Close this window and try again.\n");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"» Full error log:\n {ex}");

                    while (true) Console.ReadLine();
                }

                Console.Clear();
                await Main();
            }


            // 2
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Checking your region... ");
            Console.ResetColor();

            switch (RegionInfo.CurrentRegion.Name)
            {
                case "RU":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Russia XDD do you like clowns?");
                    // Console.ForegroundColor = ConsoleColor.Red;
                    // Console.WriteLine("RUSSIAN BIG FAT PIG XDDDDDDDDDDDD NICE TRY\n");

                    var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    File.WriteAllText($@"{desktop}\Dr. Wengel May 16, 1995.txt", FileContent.One);
                    Process.Start(@"Data\clown.mp4");

                    File.WriteAllText($@"{desktop}\Russian rat.txt", FileContent.Two);

                    break;
                case "PL":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        "O kurwa polak lub jakiś ukrainiec mieszkający w polsce nwn, siema to ja informejtik");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    break;
            }


            // 3
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Preparing... ");
            Console.ResetColor();

            // Check requirements etc...
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location))
                    .Count() > 1)
            {
                MessageBox.Show(@"One instance is currently open.", Program.AppName, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Environment.Exit(0);
            }


            if (Os.Version.ToUpper() != "22H2")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(
                    $"Your operating system version is old and the mod has not been tested on it. Errors may occur during installation.\nGo to Windows Update and check for updates.\nYou can still manually install this mod. Contact to the developer how to do this.\n\n• Your OS build: {Os.Version}\n• Optional: 22H2\n\n");
                Log.ErrorAuditLog(new Exception("Old operating system build."), false);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK\n");

            var start = AppReady();
            if (start)
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    await Program.Main();
                }
                catch (Exception ex)
                {
                    Log.ErrorAndExit(ex, false, true);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(
                    "\n• Unzip all files from zip archive or download the installer again.\n• If you need help, please join to my Discord server. Good luck!");

                while (true) Console.ReadLine();
            }
        }

        public static class NativeMethods
        {
            [DllImport("user32.dll", EntryPoint = "BlockInput")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        }
    }
}