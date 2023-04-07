using System;
using System.IO;
using System.Net;
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
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", Program.UserAgent);
                wc.Headers.Add("Authorization", "Bearer " + Telemetry.BearerToken);
                var res = await wc.DownloadStringTaskAsync($"{Telemetry.ApiUrl}/access/setup");

                Log.Output($"Received: {res}");

                return JsonConvert.DeserializeObject<SetupAccess>(res);
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.MethodNotAllowed)
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var responseJson = await reader.ReadToEndAsync();
                        var deserializeObject = JsonConvert.DeserializeObject<SetupAccess>(responseJson);

                        Log.SaveErrorLog(new Exception($"Failed to receive consent to install.\n\n{responseJson}"), true);
                        return deserializeObject;
                    }

                Log.ErrorAndExit(new Exception($"HTTP error: {ex.Message}"), false, false);
                return null;
            }
        }
    }
}
