namespace Genshin_Stella_Setup.Models
{
    public class SetupAccess
    {
        public bool Success { get; set; }

        // public int Status { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
    }


    internal class InstallerVersion
    {
        // public string Status { get; set; }
        public ApiData Installer { get; set; }
    }

    public class ApiData
    {
        public string Version { get; set; }
        public bool Beta { get; set; }
        public string ReleaseDate { get; set; }
        public string Size { get; set; }
    }

    public class SefinekApi
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
