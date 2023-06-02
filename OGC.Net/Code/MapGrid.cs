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

using Geosite.Models;
using GMap.NET.WindowsForms;
using System.ComponentModel;
using GMap.NET;
using GMap.NET.Extend;

namespace Geosite
{
    /// <summary>
    /// 国际图幅分幅网格类
    /// </summary>
    public class MapGrid
    {
        /// <summary>
        /// 国际标准图幅网格（矩形框矢量要素）叠加层对象
        /// </summary>
        public static GMapOverlay Features;

        /// <summary>
        /// 后台工作者
        /// </summary>
        private static readonly BackgroundWorker BackgroundWorker = new();

        /// <summary>
        /// 比例尺
        /// </summary>
        public static string AutoScale;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MapGrid()
        {
            BackgroundWorker.WorkerSupportsCancellation = true;
            BackgroundWorker.DoWork += BackgroundWorker_DoWork;
        }

        /// <summary>
        /// 网格可视化
        /// </summary>
        /// <param name="option">比例尺选项</param>
        /// <param name="zoom">图形窗口当前缩放级</param>
        /// <param name="boundary">图形窗口视图边界</param>
        /// <param name="gridPen">网格线画笔</param>
        public static void View(
            string option = null,
            int zoom = 0,
            ((double Latitude, double Longitude) TopLeft, (double Latitude, double Longitude) BottomRight)? boundary = null,
            Pen gridPen = null
            )
        {
            if (BackgroundWorker.IsBusy)
                BackgroundWorker.CancelAsync();
            else
            {
                lock (Features.Routes)
                {
                    Features.Markers?.Clear();
                    Features.Routes?.Clear();
                    Features.Polygons?.Clear();
                }
                if (option != null && boundary != null)
                    BackgroundWorker.RunWorkerAsync(argument: (option, zoom, boundary.Value, gridPen));
            }
        }

        private static void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = ((string option, int zoom, ((double Latitude, double Longitude) TopLeft, (double Latitude, double Longitude) BottomRight) boundary, Pen gridPen)?)e.Argument;
            if (argument == null) 
                return;
            var (option, zoom, boundary, gridPen) = argument.Value;
            var (longitudeLine, latitudeLine, scale) = MapGrids.GetMapGridInfo(
                zoom: option switch
                {
                    "1000000" => 0,
                    "500000" => 6,
                    "250000" => 7,  
                    "200000" => 8,
                    "100000" => 9,
                    "50000" => 10,
                    "25000" => 11,
                    "10000" => 12,
                    "5000" => 13,
                    _ => zoom //Auto
                },
                boundary: boundary
            );
            AutoScale = scale;
            gridPen ??= new Pen(Color.FromArgb(255, 255, 255), 1);
            var redPen = (Pen)(gridPen.Clone());
            redPen.Color = Color.Red;
            foreach (var (first, second) in longitudeLine.AsParallel())
            {
                if (BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Application.DoEvents();
                lock (Features.Routes)
                {
                    Features.Routes.Add(
                        item: new GMapRouteLine(
                            points: new List<PointLatLng>
                            {
                                new(lat: first.lat, lng: first.lng),
                                new(lat: second.lat, lng: second.lng)
                            },
                            name: "LineString",
                            strokeNormal: Math.Abs(value: first.lng) < 1e-15 ? 
                                redPen : //本初子午线
                                gridPen
                        )
                        {
                            IsHitTestVisible = false
                        }
                    );
                }
            }
            foreach (var (first, second) in latitudeLine.AsParallel())
            {
                if (BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                Application.DoEvents();
                lock (Features.Routes)
                {
                    Features.Routes.Add(
                        item: new GMapRouteLine(
                            points: new List<PointLatLng>
                            {
                                new(lat: first.lat, lng: first.lng),
                                new(lat: second.lat, lng: second.lng)
                            },
                            name: "LineString",
                            strokeNormal:
                            Math.Abs(value: first.lat) < 1e-15 ? 
                                redPen : //赤道线
                                gridPen
                        )
                        {
                            IsHitTestVisible = false
                        }
                    );
                }
            }
        }
    }
}
