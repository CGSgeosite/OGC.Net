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
    public class GMapPolygonArea : GMapPolygon
    {
        private Pen _strokeNormal;
        public Pen StrokeNormal
        {
            get => _strokeNormal;
            set => _strokeNormal = value;
        }

        private Brush _fillNormal;
        public Brush FillNormal
        {
            get => _fillNormal;
            set => _fillNormal = value;
        }

        private Pen _strokeSelected;
        public Pen StrokeSelected
        {
            get => _strokeSelected;
            set => _strokeSelected = value;
        }

        private Brush _fillSelected;
        public Brush FillSelected
        {
            get => _fillSelected;
            set => _fillSelected = value;
        }

        public bool IsSelected;
        
        public GMapPolygonArea(
            List<PointLatLng> points, string name,
            Pen strokeNormal = null, //常态画笔（矩形边框颜色，像素宽度）
            Brush fillNormal = null,
            Pen strokeSelected = null, //选择态画笔（矩形边框颜色，像素宽度）
            Brush fillSelected = null
            ) : base(points, name)
        {
            StrokeNormal = strokeNormal ?? new Pen(
                color: Color.FromArgb(alpha: 255, red: 255, green: 63, blue: 34),
                width: 1
            );
            FillNormal = fillNormal ?? new SolidBrush(color: Color.FromArgb(alpha: 50, red: StrokeNormal.Color.R, green: StrokeNormal.Color.G, blue: StrokeNormal.Color.B));

            StrokeSelected = strokeSelected ?? new Pen(
                color: Color.FromArgb(red: 255, green: 202, blue: 0),
                width: 1
            );
            FillSelected = fillSelected ?? new SolidBrush(color: Color.FromArgb(alpha: 50, red: StrokeSelected.Color.R, green: StrokeSelected.Color.G, blue: StrokeSelected.Color.B));

            IsSelected = false;
        }

        public override void OnRender(Graphics g)
        {
            Stroke = IsSelected ? StrokeSelected : StrokeNormal;
            Fill = IsSelected ? FillSelected : FillNormal;
            base.OnRender(g);
        }

        public override void Dispose()
        {
            StrokeNormal?.Dispose();
            FillNormal?.Dispose();

            StrokeSelected?.Dispose();
            FillSelected?.Dispose();

            base.Dispose();
        }
    }
}
