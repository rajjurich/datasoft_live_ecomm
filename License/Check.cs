using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;

namespace License
{
    public static class Check
    {
        public static string systemInfo = GetSystemInfo();
        public static DateTime productExpiryDate = new DateTime(2068, 1, 1);
        public static Guid salt = new Guid("10360D0E-6FB5-48A4-9FFA-3D1E1C1E00DD");

        private static string GetSystemInfo()
        {
            var systemInfo = GetMacId() + GetHDDSerialNo();
            return EncryptSystemInfo(systemInfo, salt);
        }

        private static string EncryptSystemInfo(string systemInfo, Guid salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedSystemInfo =
                    string.Format("{0}{1}", salt, systemInfo);

                byte[] saltedSystemInfoAsBytes =
                    Encoding.UTF8.GetBytes(saltedSystemInfo);

                return Convert.ToBase64String(
                    sha256.ComputeHash(saltedSystemInfoAsBytes));
            }
        }

        private static string GetHDDSerialNo()
        {
            string drive = "C";
            ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
            managementObject.Get();
            return managementObject["VolumeSerialNumber"].ToString();
        }

        private static string GetMacId()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            string macId = string.Empty;
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (macId == String.Empty)// only return MAC Address 
                {
                    IPInterfaceProperties properties = networkInterface.GetIPProperties();
                    macId = networkInterface.GetPhysicalAddress().ToString();
                }
            }
            return macId;
        }
    }
}
