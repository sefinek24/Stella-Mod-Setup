using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Genshin_Stella_Setup.Scripts
{
    internal abstract class Telemetry
    {
        public const string ApiUrl = "https://api.sefinek.net/api/v3/genshin-impact-reshade";
        // public const string ApiUrl = " http://127.0.0.1:4010/api/v3/genshin-impact-reshade";

        public static string BearerToken = "";

        public static async Task Post(string data)
        {
            Log.Output($"Telemetry: {data}");

            var obj = new NameValueCollection
            {
                { "deviceId", Os.DeviceId },
                { "regionName", Os.RegionEngName },
                { "regionCode", RegionInfo.CurrentRegion.Name },
                { "osName", Os.Name },
                { "osVersion", Os.Version },
                { "osBuild", Os.Build },
                { "setupVersion", Program.AppVersion },
                { "data", data }
            };

            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", Program.UserAgent);
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + BearerToken);
            var responseBytes = await webClient.UploadValuesTaskAsync($"{ApiUrl}/telemetry/setup/send", obj);
            var responseString = Encoding.UTF8.GetString(responseBytes);
            Log.Output(responseString);
        }

        public static async Task Error(Exception error)
        {
            var obj = new NameValueCollection
            {
                { "deviceId", Os.DeviceId },
                { "regionName", Os.RegionEngName },
                { "regionCode", RegionInfo.CurrentRegion.Name },
                { "osName", Os.Name },
                { "osVersion", Os.Version },
                { "osBuild", Os.Build },
                { "setupVersion", Program.AppVersion },
                { "error", error.ToString() }
            };

            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", Program.UserAgent);
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + BearerToken);
            var responseBytes = await webClient.UploadValuesTaskAsync($"{ApiUrl}/telemetry/setup/error", obj);
            var responseString = Encoding.UTF8.GetString(responseBytes);
            Log.Output(responseString);
        }

        public static async Task<bool> SendLogFiles()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Sending... ");

            try
            {
                // Files
                var content1 = "File setup.output.log is empty.";
                if (File.Exists(Log.OutputFile)) content1 = File.ReadAllText(Log.OutputFile);

                var content2 = "File installation.log is empty.";
                if (File.Exists(Log.ModInstFile)) content2 = File.ReadAllText(Log.ModInstFile);

                // Send
                var obj = new NameValueCollection
                {
                    { "deviceId", Os.DeviceId },
                    { "regionName", Os.RegionEngName },
                    { "timezone", Os.TimeZone },
                    { "osName", Os.AllInfos },
                    { "data", $"{content1}\n\n\n{content2}" }
                };

                var webClient = new WebClient();
                webClient.Headers.Add("User-Agent", Program.UserAgent);
                webClient.Headers.Add("Authorization", $"Bearer {BearerToken}");
                var responseBytes = await webClient.UploadValuesTaskAsync($"{ApiUrl}/telemetry/send-log-files", "PUT", obj);
                var json = Encoding.UTF8.GetString(responseBytes);

                Log.Output(json);

                // Debug log
                Log.Output("Log files was sent to developer.");

                // Console
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done");
                Console.ResetColor();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}\n");
                Console.ResetColor();

                Log.SaveErrorLog(ex, false);
                return false;
            }
        }
    }
}
