using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Genshin_Stella_Setup.Scripts
{
    internal abstract class Internet
    {
        private static async Task<bool> Ping(string host)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var buffer = new byte[32];
                    const int timeout = 9000;
                    var options = new PingOptions();
                    var reply = await ping.SendPingAsync(host, timeout, buffer, options);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(new Exception(
                    $"ERROR\n\nSorry. I cannot connect to the {host} server. Please check your network or antivirus program and try again.\n\n» Error:\n{ex.Message}"), false, false);
                return false;
            }
        }

        public static async Task<bool> CheckConnection()
        {
            var hostsToPing = new[] { "sefinek.net", "api.sefinek.net", "cdn.sefinek.net" };
            var pingTasks = hostsToPing.Select(Ping);
            var pingResults = await Task.WhenAll(pingTasks);
            return pingResults.All(x => x);
        }
    }
}
