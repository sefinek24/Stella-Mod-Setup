namespace Genshin_Impact_MP_Installer.Models
{
	internal class InstallerVersion
	{
		public string Version { get; set; }
		public string LastUpdate { get; set; }
	}

	public class SefinekApi
	{
		public bool Success { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }
	}
}