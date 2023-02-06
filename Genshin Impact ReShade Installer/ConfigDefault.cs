using System;
using System.Text.RegularExpressions;

namespace Genshin_Impact_Mod_Setup
{
	internal abstract class WhConfigDefault
	{
		private const string Url = "https://discordapp.com/api/webhooks";

		private static readonly bool Channel = !Regex.Match(Environment.UserName, "(?:sefinek)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success;

		// Channel ? "PRODUCTION" : "DEVELOPMENT"
		public static readonly string MainChannel = Channel ? $"{Url}/" : $"{Url}/";
		public static readonly string LogFiles = Channel ? $"{Url}/" : $"{Url}/";
	}
}

/* 
 * 
 * Rename this file to "Config.cs" from "ConfigDefault.cs" and change class name to "WhConfig".
 * Good luck (☆ω☆)
 *
 * https://sefinek.net/genshin-impact-reshade
 *
 *
 *
 */