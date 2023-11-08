/******************************************************************************
 *
 * Name: OGC.net
 * Purpose: A free tool for reading ShapeFile, MapGIS, Excel/TXT/CSV, converting
 *          into GML, GeoJSON, ShapeFile, KML and GeositeXML, and pushing vector
 *          or raster to PostgreSQL database.
 *
 ******************************************************************************
 * (C) 2019-2023 Geosite Development Team of CGS (R)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using System.Diagnostics;
using Microsoft.Win32;

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
