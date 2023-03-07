using System.Net;
using System;
using System.Threading.Tasks;
using Genshin_Impact_Mod_Setup.Models;
using Newtonsoft.Json;

namespace Genshin_Stella_Mod_Setup.Scripts
{
    internal abstract class Access
    {
        public static async Task<SetupAccess> Get()
        {
            try
            {
                var accessWebClient = new WebClient();
                accessWebClient.Headers.Add("User-Agent", Program.UserAgent);
                accessWebClient.Headers.Add("Authorization", "Bearer " + Telemetry.BearerToken);
                var responseJson = await accessWebClient.DownloadStringTaskAsync($"{Telemetry.ApiUrl}/access/setup");

                var setupResponse = JsonConvert.DeserializeObject<SetupAccess>(responseJson);
                return setupResponse;
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.ErrorAndExit(
                        new Exception("HTTP error 401: Your authorization token expired. Please try again."), false,
                        false);
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
