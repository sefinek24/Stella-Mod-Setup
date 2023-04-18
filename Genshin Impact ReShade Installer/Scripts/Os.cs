using System;
using System.Globalization;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace Genshin_Stella_Setup.Scripts
{
    internal abstract class Os
    {
        // Registry
        private static readonly RegistryKey LocalMachine = Environment.Is64BitProcess ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : Registry.LocalMachine;
        private static readonly RegistryKey RegistryKey = LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion");

        // Device
        public static readonly string FullIdentity = $"{GetMotherBoardId()}/{GetCpuId()}/{GetDiskSerialNumber()}/{GetDeviceId()}";
        public static readonly string DeviceId = GetDeviceId();
        public static readonly string Name = GetOs();
        public static readonly string Build = GetBuild();
        public static readonly string Version = GetVersion().ToUpper();
        public static readonly string Bits = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
        public static readonly string AllInfo = $"{Name} {Version} [{Build}]";

        // Region
        public static readonly string TimeZone = TimeZoneInfo.Local.ToString();
        public static readonly string RegionEngName = RegionInfo.CurrentRegion.EnglishName;
        public static readonly string RegionName = RegionInfo.CurrentRegion.Name;

        private static string GetDiskSerialNumber()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            var serialNumber = "";
            foreach (var o in searcher.Get())
            {
                var wmiHd = (ManagementObject)o;
                serialNumber = wmiHd["SerialNumber"].ToString();
            }

            return serialNumber;
        }

        private static string GetCpuId()
        {
            var serialNumber = "";

            try
            {
                var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_processor");
                var collection = searcher.Get();

                foreach (var o in collection)
                {
                    var obj = (ManagementObject)o;
                    serialNumber = obj["ProcessorID"].ToString();
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(ex, false, true);
            }

            return serialNumber;
        }

        private static string GetMotherBoardId()
        {
            var serial = "";

            try
            {
                var mos = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                var moc = mos.Get();

                foreach (var o in moc)
                {
                    var mo = (ManagementObject)o;
                    serial = mo["SerialNumber"].ToString();
                }

                // if (serial != "None") return serial;
                // Log.ErrorAndExit(new Exception(
                //    "Serial number of the motherboard not found. Are you sure you know what you're doing?\n\nInstallation of this software on virtual machines is not allowed."), false, false);

                return serial;
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(ex, false, true);
                return null;
            }
        }

        private static string GetDeviceId()
        {
            var computerSystemProduct = new ManagementClass("Win32_ComputerSystemProduct").GetInstances().Cast<ManagementObject>().FirstOrDefault();

            var deviceId = computerSystemProduct?.Properties["UUID"].Value.ToString();
            return !string.IsNullOrEmpty(deviceId) ? deviceId : "Unknown";
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
