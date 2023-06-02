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

using GMap.NET.WindowsForms.Markers;

namespace GMap.NET.Extend
{
    /// <summary>
    /// Circle Marker
    /// </summary>
    [Serializable]
    public class GMapMarkerPushpin : GMarkerGoogle
    {
        private GMarkerGoogleType _pushpinNormal;
        public GMarkerGoogleType PushpinNormal
        {
            get => _pushpinNormal;
            set => _pushpinNormal = value;
        }

        private GMarkerGoogleType _pushpinSelected;
        public GMarkerGoogleType PushpinSelected
        {
            get => _pushpinSelected;
            set => _pushpinSelected = value;
        }

        public Bitmap BitmapSelected;

        public bool IsSelected;
        
        public GMapMarkerPushpin(
            PointLatLng position,
            GMarkerGoogleType pushpinNormal = GMarkerGoogleType.blue_pushpin,
            GMarkerGoogleType pushpinSelected = GMarkerGoogleType.yellow_pushpin
            ) : base(position, pushpinNormal)
        {
            PushpinNormal = pushpinNormal;
            PushpinSelected = pushpinSelected;
            Offset = new Point(x: -9, y: -31);//需调整偏移量：marker.Offset = new Point(-9, -31);
            IsSelected = false;

            BitmapSelected = new GMarkerGoogle(
                p: PointLatLng.Empty,
                type: PushpinSelected
            ).Bitmap;
        }

        public override void OnRender(Graphics g)
        {
            var image = IsSelected ? BitmapSelected : Bitmap;
            lock (image)
            {
                try
                {
                    if (_bitmapShadow != null)
                        g.DrawImage(
                            _bitmapShadow,
                            LocalPosition.X,
                            LocalPosition.Y,
                            59, //_bitmapShadow.Width
                            32 //_bitmapShadow.Height
                        );
                    g.DrawImage(image, LocalPosition.X, LocalPosition.Y, 32, 32);
                }
                catch
                {
                    //
                }
            }
        }

        public override void Dispose()
        {
            BitmapSelected?.Dispose();
            base.Dispose();
        }
    }
}
