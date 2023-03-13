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

using Microsoft.Win32;
using System.Diagnostics;

namespace Geosite
{
    static class RegEdit
    {
        static string _registerKeyName;

        private static string Registerkey =>
            !string.IsNullOrWhiteSpace(_registerKeyName)
                ? _registerKeyName
                : _registerKeyName =
                    Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule?.FileName);

        public static string Getkey(string keyname, string defaultvalue = "")
        {
            using var oldRegistryKey = Registry.CurrentUser.OpenSubKey(Registerkey, false);
            return oldRegistryKey?.GetValue(keyname, defaultvalue).ToString();
        }

        public static void Setkey(string keyname, string defaultvalue = "")
        {
            using var oldRegistryKey = Registry.CurrentUser.OpenSubKey(Registerkey, true);
            if (oldRegistryKey != null)
                oldRegistryKey.SetValue(keyname, defaultvalue);
            else
            {
                using var newRegistryKey = Registry.CurrentUser.CreateSubKey(Registerkey);
                newRegistryKey?.SetValue(keyname, defaultvalue);
            }
        }
    }
}
