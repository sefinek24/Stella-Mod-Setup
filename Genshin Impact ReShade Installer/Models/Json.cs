namespace Genshin_Impact_Mod_Setup.Models
{
    internal class InstallerVersion
    {
        public string Version { get; set; }
        public string Date { get; set; }
    }

    public class SefinekApi
    {
        public bool Success { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
