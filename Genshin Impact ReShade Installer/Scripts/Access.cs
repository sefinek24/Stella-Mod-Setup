using System.Net;
using System;
using System.Threading.Tasks;
using Genshin_Stella_Setup.Models;
using Newtonsoft.Json;

namespace Genshin_Stella_Setup.Scripts
{
    internal abstract class Access
    {
        public static async Task<SetupAccess> Get()
        {
            try
            {
                var webClient = new WebClient();
                webClient.Headers.Add("User-Agent", Program.UserAgent);
                webClient.Headers.Add("Authorization", "Bearer " + Telemetry.BearerToken);
                var res = await webClient.DownloadStringTaskAsync($"{Telemetry.ApiUrl}/access/setup");
                Log.Output($"Received: {res}");

                return JsonConvert.DeserializeObject<SetupAccess>(res);
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.ErrorAndExit(new Exception("HTTP error 401: Your authorization token expired. Please try again."), false, false);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"HTTP error: {ex.Message}");
                    Console.ReadLine();
                }

                return null;
            }
        }
    }
}
