﻿using System;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Ionic.Zlib;

namespace Genshin_Impact_MP_Installer.Scripts
{
	internal abstract class Zip
	{
		private static string GetCleanFolderName(string source, string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath)) return string.Empty;

			string result = filePath.Substring(source.Length);
			if (result.StartsWith("\\")) result = result.Substring(1);
			result = result.Substring(0, result.Length - new FileInfo(filePath).Name.Length);

			return result;
		}

		public static void Create(string source, string destination)
		{
			using (ZipFile zip = new ZipFile { CompressionLevel = CompressionLevel.BestCompression })
			{
				string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories).Where(f => Path.GetExtension(f).ToLowerInvariant() != ".zip").ToArray();

				foreach (string f in files) zip.AddFile(f, GetCleanFolderName(source, f));

				string destinationFilename = destination;
				if (Directory.Exists(destination) && !destination.EndsWith(".zip"))
					destinationFilename += $"\\{new DirectoryInfo(source).Name}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffffff}.zip";

				zip.Save(destinationFilename);
			}
		}
	}
}

// Source: https://stackoverflow.com/a/50407976