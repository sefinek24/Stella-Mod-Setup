using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Genshin_Impact_Mod_Setup.Scripts
{
    internal abstract class ReShade
    {
        public static async Task DownloadFiles(string reShadeConfig, string reShadeLogFile)
        {
            var wbClient1 = new WebClient();
            wbClient1.Headers.Add("user-agent", Program.UserAgent);
            await wbClient1.DownloadFileTaskAsync(
                "https://cdn.sefinek.net/resources/genshin-impact-reshade/reshade/config", reShadeConfig);

            var wbClient2 = new WebClient();
            wbClient2.Headers.Add("user-agent", Program.UserAgent);
            await wbClient2.DownloadFileTaskAsync(
                "https://cdn.sefinek.net/resources/genshin-impact-reshade/reshade/log", reShadeLogFile);

            if (File.Exists(reShadeConfig) && File.Exists(reShadeLogFile))
            {
                Console.WriteLine("Done");
                Log.Output(
                    "ReShade.ini and ReShade.log was successfully downloaded.\n» Source 1: https://cdn.sefinek.net/resources/genshin-impact-reshade/reshade/config\n» Source 2: https://cdn.sefinek.net/resources/genshin-impact-reshade/reshade/log");
            }
            else
            {
                Console.WriteLine("Error");
                Log.ErrorString(
                    $"Something went wrong. Config and log file for ReShade was not found in: {reShadeConfig}", false);
            }
        }
    }
}
