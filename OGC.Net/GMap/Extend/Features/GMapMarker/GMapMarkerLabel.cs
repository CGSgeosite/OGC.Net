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

#nullable enable
using GMap.NET.WindowsForms;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GMap.NET.Extend
{
    /// <summary>
    /// Label Marker
    /// </summary>
    public class GMapMarkerLabel : GMapMarker, ISerializable
    {
        public Color LabelColorNormal { get; set; }
        public Color LabelColorSelected { get; set; }

        public bool IsSelected;

        public Font? LabelFont { get; set; }

        public string LabelText { get; set; }

        public GMapMarkerLabel(PointLatLng position, string labelText, Color? labelColorNormal = null, Color? labelColorSelected = null, Font? labelFont = null) : base(position)
        {
            LabelText = labelText;
            LabelColorNormal = labelColorNormal ?? Color.FromArgb(red: 30, green: 30, blue: 30);
            LabelColorSelected = labelColorSelected ?? Color.FromArgb(red: 255, green: 202, blue: 0);
            LabelFont = labelFont ?? new Font("Arial", 12);
            IsSelected = false;
        }

        public override void OnRender(Graphics g)
        {
            if (LabelFont == null)
                return;

            using var path = GetStringPath(
                LabelText,
                g.DpiY,
                new RectangleF(
                    LocalPosition.X,
                    LocalPosition.Y,
                    Size.Width,
                    Size.Height
                ),
                LabelFont,
                StringFormat.GenericTypographic
            );
            var labelColor = IsSelected ? LabelColorSelected : LabelColorNormal;
            using var brush = new SolidBrush(labelColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(Pens.White, path);
            g.FillPath(brush, path);
        }

        private static GraphicsPath GetStringPath(string s, float dpi, RectangleF rect, Font font, StringFormat format)
        {
            var path = new GraphicsPath();
            path.AddString(s, font.FontFamily, (int)font.Style, dpi * font.SizeInPoints / 72, rect, format);
            return path;
        }

        public override void Dispose()
        {
            LabelFont?.Dispose();
            base.Dispose();
        }
    }
}
