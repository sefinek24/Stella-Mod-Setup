namespace Genshin_Stella_Setup.Models
{
    // GET /api/v4/genshin-stella-mod/access/setup
    public class SetupAccess
    {
        public bool Success { get; set; }

        // public int Status { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
    }


    // GET /api/v4/genshin-stella-mod/version/app/setup
    internal class SetupVersion
    {
        // public string Status { get; set; }
        public ApiData Setup { get; set; }
    }

    public class ApiData
    {
        public string Version { get; set; }
        public bool Beta { get; set; }
        public string ReleaseDate { get; set; }
        public string Size { get; set; }
    }


    // GET /api/v2/random/animal/cat
    public class SefinekApi
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
