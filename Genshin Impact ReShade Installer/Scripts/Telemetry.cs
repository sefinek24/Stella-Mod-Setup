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
        // API
        // public const string ApiUrl = "https://api.sefinek.net/api/v3/genshin-stella-mod";
        public const string ApiUrl = " http://127.0.0.1:4010/api/v3/genshin-stella-mod";

        // Token
        public static string BearerToken = "";

        public static async Task Post(string data)
        {
            Log.Output($"Telemetry: {data}");

            var obj = new NameValueCollection
            {
                { "cpuId", Os.CpuId },
                { "deviceId", Os.DeviceId },
                { "regionName", Os.RegionEngName },
                { "regionCode", RegionInfo.CurrentRegion.Name },
                { "osName", Os.Name },
                { "osBuild", Os.Build },
                { "log", data }
            };

            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", Program.UserAgent);
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + BearerToken);
            var responseBytes = await webClient.UploadValuesTaskAsync($"{ApiUrl}/telemetry/send", obj);
            var responseString = Encoding.UTF8.GetString(responseBytes);
            Log.Output(responseString);
        }

        public static async Task Error(Exception error)
        {
            var obj = new NameValueCollection
            {
                { "cpuId", Os.CpuId },
                { "deviceId", Os.DeviceId },
                { "regionName", Os.RegionEngName },
                { "regionCode", RegionInfo.CurrentRegion.Name },
                { "osBuild", Os.Build },
                { "setupVersion", Program.AppVersion },
                { "error", error.ToString() }
            };

            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", Program.UserAgent);
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + BearerToken);
            var responseBytes = await webClient.UploadValuesTaskAsync($"{ApiUrl}/telemetry/error", obj);
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

                var content2 = "File innosetup-logs.install.log is empty.";
                if (File.Exists(Log.ModInstFile)) content2 = File.ReadAllText(Log.ModInstFile);

                // Send
                var obj = new NameValueCollection
                {
                    { "cpuId", Os.CpuId },
                    { "deviceId", Os.DeviceId },
                    { "regionName", Os.RegionEngName },
                    { "timezone", Os.TimeZone },
                    { "osName", Os.AllInfo },
                    { "setupVersion", Program.AppVersion },
                    { "setupOutput", content1 },
                    { "innoSetupOutput", content2 }
                };

                var webClient = new WebClient();
                webClient.Headers.Add("User-Agent", Program.UserAgent);
                webClient.Headers.Add("Authorization", $"Bearer {BearerToken}");
                var responseBytes = await webClient.UploadValuesTaskAsync($"{ApiUrl}/telemetry/setup/send-log-files", "PUT", obj);
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
