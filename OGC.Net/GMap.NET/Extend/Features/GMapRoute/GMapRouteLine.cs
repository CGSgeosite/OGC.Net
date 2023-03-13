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

using GMap.NET.WindowsForms;

namespace GMap.NET.Extend
{
    /// <summary>
    /// Circle Marker
    /// </summary>
    [Serializable]
    public class GMapRouteLine : GMapRoute
    {
        private Pen _strokeNormal;
        public Pen StrokeNormal
        {
            get => _strokeNormal;
            set => _strokeNormal = value;
        }

        private Pen _strokeSelected;
        public Pen StrokeSelected
        {
            get => _strokeSelected;
            set => _strokeSelected = value;
        }

        public bool IsSelected;

        public GMapRouteLine(
            IEnumerable<PointLatLng> points, string name,
            Pen strokeNormal = null,
            Pen strokeSelected = null
            ) : base(points, name)
        {
            StrokeNormal = strokeNormal ?? new Pen(
                color: Color.FromArgb(alpha: 255, red: 0, green: 0, blue: 0),
                width: 1
            );
            StrokeSelected = strokeSelected ?? new Pen(
                color: Color.FromArgb(red: 255, green: 202, blue: 0),
                width: 1
            );
            IsSelected = false;
        }

        public override void OnRender(Graphics g)
        {
            Stroke = IsSelected ? StrokeSelected : StrokeNormal;
            base.OnRender(g);
        }

        public override void Dispose()
        {
            StrokeNormal?.Dispose();
            StrokeSelected?.Dispose();
            base.Dispose();
        }
    }
}
