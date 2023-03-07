using System.Collections.Generic;

namespace Genshin_Impact_Mod_Setup.Models
{
    public class SetupAccess
    {
        public bool Success { get; set; }
        public int Status { get; set; }
        public SetupData Data { get; set; }
    }

    public class SetupData
    {
        public bool Allow { get; set; }
        public string Reason { get; set; }
    }


    internal class InstallerVersion
    {
        public string Version { get; set; }
        public string Date { get; set; }
    }

    public class SefinekApi
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
