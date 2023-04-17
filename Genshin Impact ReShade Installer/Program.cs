using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        // Links
        public const string AppWebsite = "https://genshin.sefinek.net";
        public const string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Genshin Stella Mod by Sefinek");

        // Other
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; GenshinStellaSetup/{AppVersion}; +{AppWebsite})";

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
                    { "cpuId", Os.CpuId },
                    { "deviceId", Os.DeviceId },
                    { "regionCode", RegionInfo.CurrentRegion.Name },
                    { "regionName", Os.RegionEngName },
                    { "timezone", Os.TimeZone },
                    { "osName", Os.Name },
                    { "osVersion", Os.Version },
                    { "osBuild", Os.Build },
                    { "setupVersion", AppVersion },
                    { "secretKey", Utils.EncodeString(Data.SecretKey) }
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

                    Log.Output($"Successfully authorized with status code {status}!");

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
                Log.SaveErrorLog(ex, true);
                Log.ErrorAndExit(new Exception(ex.Message), false, false);
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

                Log.ErrorAndExit(new Exception(
                        $"Sorry, your operating system version is deprecated and not supported.\nGo to Windows Update and check for updates. If you need help, contact to the developers. We can help you!\n\nSupported OS list: https://github.com/sefinek24/Genshin-Impact-ReShade#--supported-operating-systems\n\n» Your version: {Environment.OSVersion.Version.Build}\n» Version higher than: 19041"),
                    false, false);
            }

            if (Os.Bits != "64-bit")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR\n");

                Log.ErrorAndExit(new Exception($"Sorry, your operating system architecture is not supported.\n\n» Your: {Os.Bits}\n» Required: 64-bit"), false, false);
            }

            if (Os.Version != "22H2" && Os.Version != "21H2")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("ATTENTION REQUIRED");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    "Your operating system version is outdated, and this mod has not been tested on it.\nErrors may occur during installation.\n\n" +
                    "Please go to Windows Update and check for updates.\nIf updates are not available, contact the developer for further assistance on manually installing this mod.\n\n" +
                    $"» Your version: {Os.Version}\n" +
                    "» Recommended versions: 22H2 or 21H2\n");

                Log.SaveErrorLog(new Exception($"Old operating system version: {Os.Version}"), true);
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

            if (Process.GetProcessesByName(AppName).Length > 1)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("FAILED");

                const string alreadyRunning = "Another instance of the application is already running.";
                MessageBox.Show(alreadyRunning, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.SaveErrorLog(new Exception(alreadyRunning), true);

                Environment.Exit(0);
            }

            var isWarning = false;
            if (File.Exists($@"{Installation.Folder}\Genshin Stella Mod.exe") || File.Exists($@"{Installation.Folder}\data\libs\Genshin Stella Mod.pdb"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    "» You currently have an installed copy of the mod on your computer.\n\n" +
                    "If you want to perform a clean, fresh installation, delete the Genshin-Impact-ReShade folder from your C: drive.\n" +
                    "Remember to save your custom presets if you had any!\n");

                Log.Output($"Found installed instance of Genshin Impact Stella Mod in {Installation.Folder}.");
                isWarning = true;
            }

            var start = Utils.AnalyzeFiles();
            if (!start)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR");

                Log.ErrorAndExit(new Exception("» The required DLL file or other directories could not be found.\nPlease extract all the files from the ZIP archive or re-download the installer."), false, false);
                isWarning = true;
            }

            if (!isWarning)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");
            }


            // 5
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Sending a consent request... ");
            Console.ResetColor();

            var getAccess = await Access.Get();
            if (getAccess.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK");

                Log.Output("Received consent to install.");
            }
            else
            {
                Log.ErrorAndExit(new Exception(string.IsNullOrEmpty(getAccess.Response)
                    ? "Failed to receive consent to install. Unknown reason."
                    : $"Oh no! Failed to receive consent to install.\n\n» Information:\n{getAccess.Message}\n\n» Reason:\n{getAccess.Response}"), false, false);
            }


            // 6
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("• Searching for new updates... ");
            Console.ResetColor();

            var client = new WebClient();
            client.Headers.Add("user-agent", UserAgent);
            var json = await client.DownloadStringTaskAsync($"{Telemetry.ApiUrl}/version/app/installer");
            Log.Output(json);
            var res = JsonConvert.DeserializeObject<InstallerVersion>(json);

            var remoteVersion = res.Installer.Version;
            var remoteVerDate = DateTime.Parse(res.Installer.ReleaseDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();

            if (remoteVersion != AppVersion)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(
                    $"• This setup is outdated. Please download the latest version from:\n{AppWebsite}\n\n" +
                    $"» Your version   : v{AppVersion}\n" +
                    $"» Latest version : v{remoteVersion} {(res.Installer.Beta ? "Beta" : "stable")} from {remoteVerDate}\n" +
                    $"» Size           : ~{res.Installer.Size}\n");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("» Open the official website now to download? [Yes/no]: ");
                Console.ResetColor();

                Log.Output($"This program is outdated. Your version: {AppVersion}, latest: {remoteVersion}");
                await Telemetry.Post("Found new updates.");

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

            var connection = await Internet.CheckConnection();
            if (!connection) return;

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


            try
            {
                await Actions.Questions();
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(ex, false, true);
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
