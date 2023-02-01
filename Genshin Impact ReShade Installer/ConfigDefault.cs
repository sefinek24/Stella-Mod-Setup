using System;
using System.Text.RegularExpressions;

namespace Genshin_Impact_MP_Installer
{
	internal abstract class ConfigDefault // Change to Config
	{
		private const string Url = "https://discordapp.com/api/webhooks";

		private static readonly bool Channel = !Regex.Match(Environment.UserName, "(?:sefinek)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success;

		public static readonly string MainWebHookUrl = Channel ? $"{Url}/" : $"{Url}/";

		public static readonly string LogsWebHookUrl = Channel ? $"{Url}/" : $"{Url}/";
	}
}