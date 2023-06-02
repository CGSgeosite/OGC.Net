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
    public class Functions
    {
        /// <summary>
        ///     gets rectangle with all objects inside
        /// </summary>
        /// <param name="overlayId">overlay id or null to check all except zoomInsignificant</param>
        /// <param name="mapControl"></param>
        /// <returns></returns>
        public static RectLatLng? GetRectOfAllPolygons(string overlayId, GMapControl mapControl)
        {
            var left = double.MaxValue;
            var top = double.MinValue;
            var right = double.MinValue;
            var bottom = double.MaxValue;
            foreach (var o in mapControl.Overlays)
            {
                if (overlayId == null && o.IsZoomSignificant || o.Id == overlayId)
                {
                    if (o.IsVisibile && o.Polygons.Count > 0)
                    {
                        foreach (var polygon in o.Polygons)
                        {
                            if (polygon.IsVisible && polygon.From.HasValue && polygon.To.HasValue)
                            {
                                foreach (var p in polygon.Points)
                                {
                                    // left
                                    if (p.Lng < left)
                                    {
                                        left = p.Lng;
                                    }

                                    // top
                                    if (p.Lat > top)
                                    {
                                        top = p.Lat;
                                    }

                                    // right
                                    if (p.Lng > right)
                                    {
                                        right = p.Lng;
                                    }

                                    // bottom
                                    if (p.Lat < bottom)
                                    {
                                        bottom = p.Lat;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            const double tolerance = 1e-15;

            return Math.Abs(left - double.MaxValue) > tolerance &&
                   Math.Abs(right - double.MinValue) > tolerance &&
                   Math.Abs(top - double.MinValue) > tolerance &&
                   Math.Abs(bottom - double.MaxValue) > tolerance
                ? RectLatLng.FromLTRB(left, top, right, bottom)
                : null;
        }
    }
}
