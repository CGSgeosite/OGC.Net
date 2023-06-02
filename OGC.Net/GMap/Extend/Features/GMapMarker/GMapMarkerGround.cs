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

using System.Drawing.Imaging;
using System.Runtime.Serialization;
using GMap.NET.WindowsForms;

namespace GMap.NET.Extend
{
    /// <summary>
    /// Ground Overlay Marker
    /// </summary>
    [Serializable]
    public class GMapMarkerGround : GMapMarker, ISerializable
    {
        public Image Image { get; set; }
        public PointLatLng TopLeft;
        public PointLatLng BottomRight;
        public float AlphaNormal;
        public float AlphaSelected;
        public bool IsSelected;
        /*
            GDI+��ColorMatrix�Ǹ�5X5�ľ��� - [R,G,B,A,W]
            [rr,gr,br,ar,wr]
            [rg,gg,bg,ag,wg]
            [rb,gb,bb,ab,wb]
            [ra,ga,ba,aa,wa]
            [rw,gw,bw,aw,ww]
            ��͸���������£�
            [1,0,0,0,0]
            [0,1,0,0,0]
            [0,0,1,0,0]
            [0,0,0,0.5,0]
            [0,0,0,0,1]
            ��ô��ɫ������ǣ�[R*1��G*1,B*1,A*0.5,W*1]
            ���磬�ҶȾ���
            new ColorMatrix(new[]
            {
                new[] {.3f, .3f, .3f, 0, 0},
                new[] {.59f, .59f, .59f, 0, 0},
                new[] {.11f, .11f, .11f, 0, 0}, 
                new float[] {0, 0, 0, 1, 0}, 
                new float[] {0, 0, 0, 0, 1}
            }         
         */
        //public ColorMatrix ColorMatrix = new(
        //    new[]
        //    {
        //        new[] { 1f, 0, 0, 0, 0 },
        //        new[] { 0, 1f, 0, 0, 0 },
        //        new[] { 0, 0, 1f, 0, 0 },
        //        new[] { 0, 0, 0, 1f, 0 },
        //        new[] { 0, 0, 0, 0, 1f }
        //    }
        //);

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="image">λͼ����</param>
        /// <param name="topLeft">���Ͻǣ�γ�Ⱦ��ȣ�</param>
        /// <param name="bottomRight">���½ǣ�γ�Ⱦ��ȣ�</param>
        /// <param name="alphaNormal">��̬͸���ȡ����ڵ���0��С�ڵ���1��Ĭ�ϣ�0.8f��</param>
        /// <param name="alphaSelected">ѡ��̬͸���ȡ����ڵ���0��С�ڵ���1��Ĭ�ϣ�1f��</param>
        public GMapMarkerGround(Image image, PointLatLng topLeft, PointLatLng bottomRight, float alphaNormal = 0.8f, float alphaSelected = 1f) : base(pos: topLeft)
        {
            Image = image;
            TopLeft = topLeft;
            BottomRight= bottomRight;
            AlphaNormal = alphaNormal is >= 0f and <= 1f ? alphaNormal : 0.8f;
            AlphaSelected = alphaSelected is >= 0f and <= 1f ? alphaSelected : 1f;
            //DisableRegionCheck = true;
            //Overlay.Control.IgnoreMarkerOnMouseWheel = false;
            IsSelected = false;
        }

        public override void OnRender(Graphics g)
        {
            if (Image == null)
                return;
            var topLeft = Overlay.Control.MapProvider.Projection.FromLatLngToPixel(p: TopLeft, zoom: (int) Overlay.Control.Zoom);
            var bottomRight = Overlay.Control.MapProvider.Projection.FromLatLngToPixel(p: BottomRight, zoom: (int) Overlay.Control.Zoom);
            Size = new Size(width: (int) (bottomRight.X - topLeft.X), height: (int) (bottomRight.Y - topLeft.Y));
            using var attributes = new ImageAttributes();
            attributes.SetColorMatrix(newColorMatrix: new ColorMatrix { Matrix33 = IsSelected ? AlphaSelected : AlphaNormal }, mode: ColorMatrixFlag.Default, type: ColorAdjustType.Bitmap);
            g.DrawImage(
                Image,
                new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height),
                0,
                0,
                Image.Width,
                Image.Height,
                GraphicsUnit.Pixel,
                attributes
            );
        }

        public override void Dispose()
        {
            Image?.Dispose();
            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info: info, context: context);
        }

        protected GMapMarkerGround(SerializationInfo info, StreamingContext context)
            : base(info: info, context: context)
        {
        }
        #endregion
    }
}