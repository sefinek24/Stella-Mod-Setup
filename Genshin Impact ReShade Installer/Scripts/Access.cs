using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
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
                var obj = new NameValueCollection
                {
                    { "identity", Os.FullIdentity },
                    { "deviceId", Os.DeviceId },
                    { "regionCode", RegionInfo.CurrentRegion.Name },
                    { "regionName", Os.RegionEngName },
                    { "osName", Os.Name },
                    { "osBuild", Os.Build },
                    { "setupVersion", Program.AppVersion }
                };

                var wc = new WebClient();
                wc.Headers.Add("User-Agent", Program.UserAgent);
                wc.Headers.Add("Authorization", "Bearer " + Telemetry.BearerToken);
                var res = await wc.UploadValuesTaskAsync($"{Telemetry.ApiUrl}/access/setup", obj);
                var json = Encoding.UTF8.GetString(res);
                Log.Output(json);

                var setupAccess = JsonConvert.DeserializeObject<SetupAccess>(json);
                return setupAccess;
            }
            catch (WebException ex)
            {
                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.MethodNotAllowed)
                    using (var reader = new StreamReader(ex.Response.GetResponseStream() ?? throw new InvalidOperationException()))
                    {
                        var responseJson = await reader.ReadToEndAsync();
                        var deserializeObject = JsonConvert.DeserializeObject<SetupAccess>(responseJson);

                        Log.SaveErrorLog(new Exception($"Failed to receive consent to install. Method not allowed.\n\n{responseJson}"), false);
                        return deserializeObject;
                    }

                if (ex.Response != null && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                    using (var reader = new StreamReader(ex.Response.GetResponseStream() ?? throw new InvalidOperationException()))
                    {
                        var responseJson = await reader.ReadToEndAsync();
                        var deserializeObject = JsonConvert.DeserializeObject<SetupAccess>(responseJson);

                        Log.SaveErrorLog(new Exception($"Failed to receive consent to install. Unauthorized.\n\n{responseJson}"), false);
                        return deserializeObject;
                    }

                Log.ErrorAndExit(new Exception($"HTTP error: {ex.Message}"), false, false);
                return null;
            }
        }
    }
}
