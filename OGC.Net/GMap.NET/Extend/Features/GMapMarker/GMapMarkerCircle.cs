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

using System.Runtime.Serialization;
using GMap.NET.WindowsForms;

namespace GMap.NET.Extend
{
    /// <summary>
    /// Circle Marker
    /// </summary>
    [Serializable]
    public class GMapMarkerCircle : GMapMarker, ISerializable
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

        public bool IsFilled;

        private int _radius;

        /// <summary>
        /// In Pixel / In Meters
        /// </summary>
        public int Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                //需对Size赋值，否则鼠标热点无法捕获
                Size = new Size(_radius * 2, _radius * 2);
            }
        }

        public GMapMarkerCircle(
            PointLatLng position, //圆心坐标
            int radius = 4,  //像素半径
            Pen strokeNormal = null, //常态画笔（边界颜色，像素宽度）
            Pen strokeSelected = null, //选择态画笔（边界颜色，像素宽度）
            bool isFilled = true //是否采用半透明边界色进行填充？
            ) : base(position)
        {
            Radius = radius;
            StrokeNormal = strokeNormal ?? new Pen(Color.FromArgb(255, 13, 110, 253), 2);
            StrokeSelected = strokeSelected ?? new Pen(Color.FromArgb(red: 255, green: 202, blue: 0), 2);
            IsFilled = isFilled;
            IsSelected = false;
        }

        public override void OnRender(Graphics g)
        {
            //如果是米制半径，可先行投影
            //Radius = (int)(Radius / Overlay.Control.MapProvider.Projection.GetGroundResolution((int)Overlay.Control.Zoom, Position.Lat));
            var rectangle = new Rectangle(
                LocalPosition.X - Radius,
                LocalPosition.Y - Radius,
                Size.Width,
                Size.Height
            );
            var pen = IsSelected ? StrokeSelected : StrokeNormal;
            if (IsFilled)
            {
                using var brush = new SolidBrush(Color.FromArgb(50, pen.Color));
                g.FillEllipse(brush, rectangle);
            }
            g.DrawEllipse(pen, rectangle);
        }

        public override void Dispose()
        {
            if (StrokeNormal != null)
            {
                StrokeNormal.Dispose();
                StrokeNormal = null;
            }
            if (StrokeSelected != null)
            {
                StrokeSelected.Dispose();
                StrokeSelected = null;
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected GMapMarkerCircle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}
