using System;
using System.Collections.Specialized;
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
using Genshin_Stella_Setup.Models;
using Genshin_Stella_Setup.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Genshin_Stella_Setup
{
    internal abstract class Program
    {
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static readonly string AppData = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Genshin Stella Mod by Sefinek");

        public const string AppWebsite = "https://genshin.sefinek.net";
        public const string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

        public static readonly string UserAgent =
            $"Mozilla/5.0 (compatible; GenshinModSetup/{AppVersion}; +{AppWebsite})";

        private static readonly string[] Dirs = { "Data", "Dependencies", "Data/Images", "Data/Libs" };

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

        public static string Rot47(string input)
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


        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                           Genshin Impact Stella Mod 2023 - Early access");
            Console.WriteLine($"                                        Version: v{AppVersion}\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("» Author  : Sefinek#0001 [Country: Poland]");
            Console.WriteLine("» Website : " + AppWebsite);
            Console.WriteLine("» Discord : " + DiscordUrl);
            Console.ResetColor();
            Console.WriteLine(Actions.Line);

            Console.Title = $"{AppName} • v{AppVersion}";

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                                         » Important «\n                Please unzip downloaded ZIP archive before installation. Good luck!\n");


            // 1
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Authorizing... ");
            Console.ResetColor();

            Log.Output("Connecting to Sefinek API and authorizing...");

            try
            {
                var obj = new NameValueCollection
                {
                    { "deviceId", Os.DeviceId },
                    { "regionCode", RegionInfo.CurrentRegion.Name },
                    { "regionName", Os.RegionEngName },
                    { "osName", Os.Name },
                    { "osBuild", Os.Build },
                    { "setupVersion", AppVersion },
                    { "secretKey", Rot47(Data.SecretKey) }
                };

                var webClient = new WebClient();
                webClient.Headers.Add("User-Agent", UserAgent);
                var responseBytes = await webClient.UploadValuesTaskAsync($"{Telemetry.ApiUrl}/access/authorize", obj);
                var responseJson = Encoding.UTF8.GetString(responseBytes);

                var token = JObject.Parse(responseJson)["token"]?.Value<string>();
                var status = JObject.Parse(responseJson)["status"]?.Value<int>();
                if (token != null)
                {
                    Telemetry.BearerToken = token;

                    Log.Output( $"Successfully authorized with status code {status}!");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAILED");

                    Log.ErrorAndExit(new Exception($"Authorization failed. HTTP error {status}"), false, false);
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(new Exception($"{ex.Message}\n\nMore information: {ex}"), false, false);
            }


            // 3
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Checking your region... ");
            Console.ResetColor();

            switch (Os.RegionName)
            {
                case "RU":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Russia XDD do you like clowns?");
                    // Console.WriteLine("RUSSIAN BIG FAT PIG XDDDDDDDDDDDD NICE TRY\n");

                    var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    File.WriteAllText($@"{desktop}\Dr. Wengel May 16, 1995.txt", FileContent.One);
                    Process.Start(@"Data\clown.mp4");

                    File.WriteAllText($@"{desktop}\Russian rat.txt", FileContent.Two);
                    Log.Output("Russian fat pig.");
                    break;
                case "PL":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK - O kurwa polak lub ukrainiec mieszkający w polsce nwn");

                    Log.Output("Poland gigachad, slava Poland!");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");

                    Log.Output($"Region: {Os.RegionName}");
                    break;
            }


            // 3
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Checking PC requirements... ");
            Console.ResetColor();

            if (Environment.OSVersion.Version.Build <= 19041)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR\n");

                Log.ErrorAndExit(
                    new Exception(
                        $"Sorry, your operating system version is deprecated and not supported.\nGo to Windows Update and check for updates. If you need help, contact to the developers. We can help you!\n\nSupported OS list: https://github.com/sefinek24/Genshin-Impact-ReShade#--supported-operating-systems\n\n» Your version: {Environment.OSVersion.Version.Build}\n» Version higher than: 19041"),
                    false, false);
            }

            if (Os.Bits != "64-bit")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR\n");

                Log.ErrorAndExit(
                    new Exception(
                        $"Sorry, your operating system architecture is not supported.\n\n» Your: {Os.Bits}\n» Required: 64-bit"),
                    false, false);
            }

            if (Os.Version.ToUpper() != "22H2")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning\n");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(
                    $"Your operating system version is old and this mod was not tested on it. Errors may occur during installation.\n\nGo to Windows Update and check for updates.\nYou can still manually install this mod. Contact to the developer how to do this.\n\n» Your version: {Os.Version}\n» Recommended: 22H2\n");
                Log.SaveErrorLog(new Exception("Old operating system version."), false);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }


            // 4
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Checking other data... ");
            Console.ResetColor();

            if (Directory.Exists(Installation.Folder))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    "You currently have an installed copy of the mod on your computer.\n" +
                    "If you want to perform a clean, fresh installation, delete the Genshin-Impact-ReShade folder from your C: drive.\n" +
                    "Remember to save your custom presets if you had any!"
                );

                Log.Output("Found installed instance of Genshin Impact Stella mod.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }


            // 5
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Sending a consent request... ");
            Console.ResetColor();

            var getAccess = await Access.Get();
            if (getAccess.Data.Allow)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");

                Log.Output("Received consent to install.");
            }
            else
            {
                Log.ErrorAndExit(new Exception(
                    string.IsNullOrEmpty(getAccess.Data.Reason)
                        ? "Failed to receive consent to install. Unknown reason."
                        : $"Failed to receive consent to install.\n\n» Reason:\n{getAccess.Data.Reason}"
                ), false, false);
            }


            // 6
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Searching for new updates... ");
            Console.ResetColor();

            var client = new WebClient();
            client.Headers.Add("user-agent", UserAgent);
            var json = await client.DownloadStringTaskAsync($"{Telemetry.ApiUrl}/version/apps");
            Log.Output($"Received: {json}");

            var res = JsonConvert.DeserializeObject<InstallerVersion>(json);

            if (res.Installer.Version != AppVersion)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning\n");
                Console.ResetColor();

                Console.WriteLine(
                    $"This setup is outdated. Please download the latest version from:\n{AppWebsite}\n\n" +
                    $"• Your version   : v{AppVersion}\n" +
                    $"• Latest version : v{res.Installer.Version} {(res.Installer.Beta ? "Beta" : "stable")} from {res.Installer.ReleaseDate}\n" +
                    $"• Size           : {res.Installer.Size}\n"
                );


                Log.Output($"This program is outdated. Your version: {AppVersion}, latest: {res.Installer.Version}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("» Open the official website now to download? [Yes/no]: ");
                Console.ResetColor();

                var websiteQuestion = Console.ReadLine()?.ToLower();
                switch (websiteQuestion)
                {
                    case "y":
                    case "yes":
                        Console.Write("Opening... ");
                        Process.Start(AppWebsite);
                        Console.WriteLine("Done\n\nYou can close this window.");

                        Log.Output($"Opened {AppWebsite} in default browser.");
                        break;
                    case "n":
                    case "no":
                        Console.WriteLine("Canceled. You can close this window.");
                        break;
                    default:
                        await Actions.WrongAnswer("Start");
                        break;
                }

                Actions.Close();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }


            // 7
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Starting... ");
            Console.ResetColor();

            if (Process.GetProcessesByName(AppName).Length > 1)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("FAILED");

                string alreadyRunning = "Another instance of the application is already running.";
                Log.Output(alreadyRunning);
                MessageBox.Show(alreadyRunning, AppName,MessageBoxButtons.OK, MessageBoxIcon.Information);

                Environment.Exit(0);
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(
                "Congratulations! Your computer appears to meet the hardware requirements.\n" +
                "Please answer the following questions by typing 'Yes' or 'No'.\n"
            );


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("                            x Your game files will NOT be modified! x");
            Console.WriteLine("                          x If you need help, join my Discord server. x");
            Console.ResetColor();
            Console.WriteLine();

            var start = AppReady();
            if (start)
            {
                try
                {
                    await Actions.Questions();
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
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        }
    }
}
