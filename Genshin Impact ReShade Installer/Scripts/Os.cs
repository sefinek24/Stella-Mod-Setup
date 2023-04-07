using System;
using System.Globalization;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace Genshin_Stella_Setup.Scripts
{
    internal abstract class Os
    {
        private static readonly RegistryKey LocalMachine = Environment.Is64BitProcess
            ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            : Registry.LocalMachine;

        private static readonly RegistryKey RegistryKey = LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion");

        public static readonly string DeviceId = GetDeviceId();
        public static readonly string Name = GetOs();
        public static readonly string Build = GetBuild();
        public static readonly string Version = GetVersion();
        public static readonly string Bits = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
        public static readonly string AllInfos = $"{Name} {Version} [{Build}]";

        public static readonly string TimeZone = TimeZoneInfo.Local.ToString();
        public static readonly string RegionEngName = RegionInfo.CurrentRegion.EnglishName;
        public static readonly string RegionName = RegionInfo.CurrentRegion.Name;

        private static string GetDeviceId()
        {
            var computerSystemProduct = new ManagementClass("Win32_ComputerSystemProduct").GetInstances().Cast<ManagementObject>().FirstOrDefault();

            var deviceId = computerSystemProduct?.Properties["UUID"].Value.ToString();
            if (!string.IsNullOrEmpty(deviceId)) return deviceId;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to retrieve device identifier.");
            while (true) Console.ReadLine();
        }

        private static string GetOs()
        {
            try
            {
                return RegistryKey.GetValue("ProductName").ToString();
            }
            catch
            {
                return "?";
            }
        }

        private static string GetBuild()
        {
            try
            {
                return RegistryKey.GetValue("CurrentBuild") + "." + RegistryKey.GetValue("UBR");
            }
            catch
            {
                return "?";
            }
        }

        private static string GetVersion()
        {
            try
            {
                return RegistryKey.GetValue("DisplayVersion").ToString();
            }
            catch
            {
                return "?";
            }
        }
    }
}
