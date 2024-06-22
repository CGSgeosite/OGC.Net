using Microsoft.Win32;
using System.Diagnostics;

namespace Geosite
{
    /// <summary>
    /// 注册表类
    /// </summary>
    static class RegEdit
    {
        static string _registerKeyName;

        private static string RegisterKey =>
            !string.IsNullOrWhiteSpace(_registerKeyName)
                ? _registerKeyName
                : _registerKeyName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule?.FileName);

        public static string GetKey(string key, string defaultValue = null)
        {
            using var oldRegistryKey = Registry.CurrentUser.OpenSubKey(RegisterKey, false);
            return oldRegistryKey?.GetValue(key, defaultValue)?.ToString();
        }

        public static void SetKey(string key, string defaultValue)
        {
            using var oldRegistryKey = Registry.CurrentUser.OpenSubKey(RegisterKey, true);
            if (oldRegistryKey != null)
                oldRegistryKey.SetValue(key, defaultValue);
            else
            {
                using var newRegistryKey = Registry.CurrentUser.CreateSubKey(RegisterKey);
                newRegistryKey?.SetValue(key, defaultValue);
            }
        }

        public static void DeleteRegistry()
        {
            Registry.CurrentUser.DeleteSubKey(RegisterKey);
        }
    }
}
