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

using Geosite.FreeText.CSV;
using Geosite.FreeText.TXT;
using Geosite.ShapeFileHelper;
using Geosite.GeositeServer;
using Geosite.GeositeXML;
using Geosite.Messager;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.Extend;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders.GeositeMapProvider;

namespace Geosite
{
    /// <summary>
    /// 文件数据可视化预览类
    /// </summary>
    public class MapView : IDisposable
    {
        private readonly MainForm _mainForm;

        /// <summary>
        /// 矢量叠加层对象
        /// </summary>
        public static GMapOverlay Features;

        /// <summary>
        /// 后台工作者
        /// </summary>
        private readonly BackgroundWorker _backgroundWorker = new();

        /// <summary>
        /// 文件存储路径
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// 文件类型（MapGIS、ShapeFile、TXT、CSV、Excel、KML、GeositeXML、GeoJson）或者源自于GeositeServer-WebAPI-GeositeXML类型
        /// </summary>
        private readonly string _type;

        /// <summary>
        /// 分类属性
        /// </summary>
        private readonly XElement _property;

        /// <summary>
        /// 任务池
        /// </summary>
        public static List<MapView> Tasks = new();

        private readonly (Pen pen, int flag) _pointStyle;
        private readonly Pen _lineStyle;
        private readonly Pen _polygonStyle;

        /// <summary>
        /// 投影助手
        /// </summary>
        private readonly ProjectionHelper _projectionHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainForm">主窗体</param>
        /// <param name="path">文件存储路径</param>
        /// <param name="type">文件类型（MapGIS、ShapeFile、TXT、CSV、Excel、KML、GeositeXML、GeoJson）或者源自于GeositeServer-WebAPI-GeositeXML类型</param>
        /// <param name="property">暂支持瓦片服务或瓦片图层的属性</param>
        /// <param name="style">点（flag：0=圆圈 1=方块 2=按钉）线面渲染样式</param>
        /// <param name="projection">投影参照系</param>
        /// <exception cref="Exception"></exception>
        public MapView(MainForm mainForm, string path, string type, XElement property, ((Pen pen, int flag) point, Pen line, Pen polygon, Pen mapGrid)? style = null, XContainer projection = null)
        {
            _mainForm = mainForm;
            _path = path;
            _type = type;
            _property = property;

            _pointStyle = style?.point ?? (new Pen(color: Color.FromArgb(alpha: 255, red: 13, green: 110, blue: 253), width: 2), 0);
            _lineStyle = style?.line ?? new Pen(color: Color.FromArgb(alpha: 255, red: 0, green: 0, blue: 0), width: 1);
            _polygonStyle = style?.polygon ?? new Pen(color: Color.FromArgb(alpha: 255, red: 255, green: 63, blue: 34), width: 1);

            //注：由于本类目前仅支持【EPSG:4326】矢量数据源可视化，因此无需关切【Projection】中的【To】标签！
            var projectionFromX = projection?.Element(name: "From")?.Elements().FirstOrDefault();
            if (!uint.TryParse(s: projectionFromX?.Element(name: "Scale")?.Value ?? "1", result: out var sourceScale))
                sourceScale = 1;
            var projectionFromName = projectionFromX?.Name.LocalName;
            string sourceProjection = null;
            switch (projectionFromName)
            {
                case "Gauss-Kruger":
                {
                    //  new XElement(
                    //    "Gauss-Kruger",
                    //    new XElement(
                    //        "CentralMeridian",
                    //        99
                    //    ),
                    //    new XElement(
                    //        "Zone",
                    //        3
                    //    ),
                    //    new XElement(
                    //        "Scale", //折合成米制的比例尺分母
                    //        1
                    //    ),
                    //    new XElement(
                    //        "Srid",
                    //        1954
                    //    )
                    //  );
                    var srid = projectionFromX.Element(name: "Srid")?.Value ?? "2000";
                    var centralMeridian = double.Parse(s: projectionFromX.Element(name: "CentralMeridian")?.Value ?? "0");
                    double zone = 500000;
                    //switch (projectionFromX.Element("Zone")?.Value ?? "6")
                    //{
                    //    case "6":
                    //    {
                    //        zone +=
                    //        (
                    //            centralMeridian >= 0
                    //                ? Math.Floor(centralMeridian / 6) + 1
                    //                : Math.Floor((centralMeridian + 180) / 6) + 31
                    //        ) * 1000000;
                    //        break;
                    //    }
                    //    case "3":
                    //    {
                    //        zone +=
                    //        (
                    //            centralMeridian >= 0
                    //                ? centralMeridian >= 1.5
                    //                    ? Math.Floor((centralMeridian - 1.5) / 3) + 1
                    //                    : 120
                    //                : Math.Floor((centralMeridian + 180 - 1.5) / 3) + 61
                    //        ) * 1000000;
                    //        break;
                    //    }
                    //    //case "-1":
                    //    //default:
                    //    //{
                    //    //    break;
                    //    //}
                    //}
                    sourceProjection = ProjectionHelper.GetProjectionString(
                        srid: srid,
                        centralMeridian: centralMeridian,
                        x0: zone
                    );
                    break;
                }
                case "Lambert":
                {
                    //  new XElement(
                    //    "Lambert",
                    //    new XElement(
                    //        "CentralMeridian",
                    //        105
                    //    ),
                    //    new XElement(
                    //        "OriginLatitude",
                    //        0
                    //    ),
                    //    new XElement(
                    //        "Parallel1",
                    //        25
                    //    ),
                    //    new XElement(
                    //        "Parallel2",
                    //        47
                    //    ),
                    //    new XElement(
                    //        "Scale", //折合成米制的比例尺分母
                    //        1
                    //    ),
                    //    new XElement(
                    //        "Srid",
                    //        1980
                    //    )
                    //  );
                    var centralMeridian = double.Parse(s: projectionFromX.Element(name: "CentralMeridian")?.Value ?? "0");
                    var originLatitude = double.Parse(s: projectionFromX.Element(name: "OriginLatitude")?.Value ?? "0");
                    var parallel1 = double.Parse(s: projectionFromX.Element(name: "Parallel1")?.Value ?? "25");
                    var parallel2 = double.Parse(s: projectionFromX.Element(name: "Parallel2")?.Value ?? "47");
                    var srid = projectionFromX.Element(name: "Srid")?.Value ?? "2000";
                    sourceProjection = ProjectionHelper.GetProjectionString(
                        conic: "Lambert",
                        srid: srid,
                        centralMeridian: centralMeridian,
                        originLatitude: originLatitude,
                        parallel1: parallel1,
                        parallel2: parallel2
                    );
                    break;
                }
                case "Albers":
                {
                    //  new XElement("Albers",
                    //    new XElement(
                    //        "CentralMeridian",
                    //        105
                    //    ),
                    //    new XElement(
                    //        "OriginLatitude",
                    //        0
                    //    ),
                    //    new XElement(
                    //        "Parallel1",
                    //        25
                    //    ),
                    //    new XElement(
                    //        "Parallel2",
                    //        47
                    //    ),
                    //    new XElement(
                    //        "Scale", //折合成米制的比例尺分母
                    //        1
                    //    ),
                    //    new XElement(
                    //        "Srid",
                    //        1980
                    //    )
                    //  );
                    var centralMeridian = double.Parse(s: projectionFromX.Element(name: "CentralMeridian")?.Value ?? "0");
                    var originLatitude = double.Parse(s: projectionFromX.Element(name: "OriginLatitude")?.Value ?? "0");
                    var parallel1 = double.Parse(s: projectionFromX.Element(name: "Parallel1")?.Value ?? "25");
                    var parallel2 = double.Parse(s: projectionFromX.Element(name: "Parallel2")?.Value ?? "47");
                    var srid = projectionFromX.Element(name: "Srid")?.Value ?? "2000";
                    sourceProjection = ProjectionHelper.GetProjectionString(
                        conic: "Albers",
                        srid: srid,
                        centralMeridian: centralMeridian,
                        originLatitude: originLatitude,
                        parallel1: parallel1,
                        parallel2: parallel2
                    );
                    break;
                }
                case "Web-Mercator":
                {
                    // new XElement("Web-Mercator");
                    sourceProjection = ProjectionHelper.GetProjectionString(conic: "WebMercator", srid: "1984", centralMeridian: 0, originLatitude: 0, parallel1: 0, parallel2: 0);
                    break;
                }
                ////case "Geography":
                //default:
                //{
                //    //如果sourceProjection未定义，强行按【EPSG:4326】对待
                //    break;
                //}
            }

            //视图强行采用【EPSG:4326】，因此目标投影参数无需额外指定
            _projectionHelper = new ProjectionHelper(sourceProjection: sourceProjection, sourceScale: sourceScale);

            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            Tasks.Add(item: this);
        }

        /// <summary>
        /// 启动图形图像加载与渲染任务
        /// </summary>
        public void View()
        {
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();
            else
                _backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 取消图形图像加载渲染任务
        /// </summary>
        public void CancelTask()
        {
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (_type?.ToLower())
            {
                case "mapgis":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(); });
                        var fileType = Path.GetExtension(path: _path)?.ToLower();
                        if (fileType == ".mpj")
                        {
                            var mapgisProject = new MapGis.MapGisProject();
                            mapgisProject.Open(file: _path);
                            var resultString = mapgisProject.Content.ToString(formatting: Formatting.Indented);
                            _mainForm.BeginInvoke(method: () => { _mainForm.MapBoxPropertyText = resultString; });
                        }
                        else //.wt .wl .wp
                        {
                            using var mapgis = new MapGis.MapGisFile();
                            mapgis.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                            {
                                var progress = thisEvent.Progress;
                                if (progress == null)
                                    _backgroundWorker.ReportProgress(percentProgress: -1,
                                        userState: thisEvent.Message ?? string.Empty);
                            };
                            mapgis.Open(mapgisFile: _path);
                            if (mapgis.RecordCount == 0)
                                throw new Exception(message: "No features found.");
                            var capabilities = mapgis.GetCapabilities();
                            if (capabilities != null)
                            {
                                var bbox = (JArray)capabilities[propertyName: "bbox"]; //西南东北
                                if (bbox != null)
                                {
                                    var west = (double)bbox[index: 0];
                                    var south = (double)bbox[index: 1];
                                    var east = (double)bbox[index: 2];
                                    var north = (double)bbox[index: 3];

                                    if (_projectionHelper != null)
                                    {
                                        try
                                        {
                                            var westSouth = _projectionHelper.Project(geometry: new JArray { west, south });
                                            west = (double)westSouth[index: 0];
                                            south = (double)westSouth[index: 1];
                                        }
                                        catch
                                        {
                                            //如果投影失败，强制西南角
                                            west = -180;
                                            south = -90;
                                        }

                                        try
                                        {
                                            var eastNorth = _projectionHelper.Project(geometry: new JArray { east, north });
                                            east = (double)eastNorth[index: 0];
                                            north = (double)eastNorth[index: 1];
                                        }
                                        catch
                                        {
                                            //如果投影失败，强制东北角
                                            east = 180;
                                            north = 90;
                                        }
                                    }

                                    if (west is > 180 or < -180 || east is > 180 or < -180 || south is > 90 or < -90 || north is > 90 or < -90)
                                        throw new Exception(message: @"The boundary has exceeded [±180, ±90].");
                                    if (west > east)
                                        west = -180;
                                    if (west > east)
                                        east = 180;
                                    if (south > north)
                                        south = -90;
                                    if (south > north)
                                        north = 90;
                                    _mainForm.MapBox.BeginInvoke(
                                        method: () =>
                                        {
                                            _mainForm.MapBox.SetZoomToFitRect(
                                                rect: RectLatLng.FromLTRB(leftLng: west, topLat: north, rightLng: east, bottomLat: south)
                                            );
                                        }
                                    );
                                }

                                var count = FeaturesView(features: mapgis.GetFeature());
                                _backgroundWorker.ReportProgress(
                                    percentProgress: -1,
                                    userState:
                                    $@"{count} valid feature{(count > 1 ? "s" : "")} in {Path.GetFileName(path: _path)}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); });
                    }

                    break;
                }
                case "shapefile":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(); });
                        var codePage = ShapeFile.GetDbfCodePage
                        (
                            dbfFileName: Path.Combine
                            (
                                path1: Path.GetDirectoryName(path: _path) ?? "",
                                path2: Path.GetFileNameWithoutExtension(path: _path) + ".dbf"
                            )
                        );
                        using var shapeFile = new ShapeFileReader();
                        shapeFile.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                            {
                                _backgroundWorker.ReportProgress(
                                    percentProgress: -1,
                                    userState: thisEvent.Message ?? string.Empty
                                );
                            }
                        };
                        shapeFile.Open(filePath: _path, defaultCodePage: codePage.CodePage);
                        if (shapeFile.RecordCount == 0)
                            throw new Exception(message: "No features found.");
                        var capabilities = shapeFile.GetCapabilities();
                        if (capabilities != null)
                        {
                            var bbox = (JArray)capabilities[propertyName: "bbox"]; //西南东北
                            if (bbox != null)
                            {
                                var west = (double)bbox[index: 0];
                                var south = (double)bbox[index: 1];
                                var east = (double)bbox[index: 2];
                                var north = (double)bbox[index: 3];
                                if (_projectionHelper != null)
                                {
                                    try
                                    {
                                        var westSouth = _projectionHelper.Project(geometry: new JArray { west, south });
                                        west = (double)westSouth[index: 0];
                                        south = (double)westSouth[index: 1];
                                    }
                                    catch
                                    {
                                        //如果投影失败，强制西南角
                                        west = -180;
                                        south = -90;
                                    }

                                    try
                                    {
                                        var eastNorth = _projectionHelper.Project(geometry: new JArray { east, north });
                                        east = (double)eastNorth[index: 0];
                                        north = (double)eastNorth[index: 1];
                                    }
                                    catch
                                    {
                                        //如果投影失败，强制东北角
                                        east = 180;
                                        north = 90;
                                    }
                                }

                                if (west is > 180 or < -180 || east is > 180 or < -180 || south is > 90 or < -90 ||
                                    north is > 90 or < -90)
                                    throw new Exception(message: @"The boundary has exceeded [±180, ±90].");
                                if (west > east)
                                    west = -180;
                                if (west > east)
                                    east = 180;
                                if (south > north)
                                    south = -90;
                                if (south > north)
                                    north = 90;
                                _mainForm.MapBox.BeginInvoke(
                                    method: () =>
                                    {
                                        _mainForm.MapBox.SetZoomToFitRect(rect: RectLatLng.FromLTRB(leftLng: west,
                                            topLat: north, rightLng: east, bottomLat: south));
                                    }
                                );
                            }

                            var count = FeaturesView(features: shapeFile.GetFeature());
                            _backgroundWorker.ReportProgress(
                                percentProgress: -1,
                                userState: $@"{count} valid feature" + (count > 1 ? "s" : "") +
                                           $" in {Path.GetFileName(path: _path)}"
                            );
                        }
                    }
                    catch (Exception error)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); });
                    }

                    break;
                }
                case "txt":
                case "csv":
                case "excel":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(); });
                        var freeTextFields = _type switch
                        {
                            "txt" => TXT.GetFieldNames(file: _path),
                            "csv" => CSV.GetFieldNames(file: _path),
                            _ => FreeText.Excel.Excel.GetFieldNames(file: _path)
                        };
                        if (freeTextFields.Length == 0)
                            throw new Exception(message: "No valid fields found.");
                        string coordinateFieldName;
                        if (freeTextFields.Any(predicate: f => f == "_position_"))
                            coordinateFieldName = "_position_";
                        else
                        {
                            var txtForm = new FreeTextFieldForm(fieldNames: freeTextFields);
                            txtForm.ShowDialog();
                            var choice = txtForm.Ok;
                            if (choice == null)
                                throw new Exception(message: "Task Cancellation.");
                            coordinateFieldName = choice.Value ? txtForm.CoordinateFieldName : null;
                        }

                        FreeText.FreeText freeText = _type switch
                        {
                            "txt" => new TXT(coordinateFieldName: coordinateFieldName),
                            "csv" => new CSV(coordinateFieldName: coordinateFieldName),
                            _ => new FreeText.Excel.Excel(coordinateFieldName: coordinateFieldName)
                        };
                        freeText.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                    userState: thisEvent.Message ?? string.Empty);
                        };
                        freeText.Open(file: _path);
                        if (string.IsNullOrWhiteSpace(value: coordinateFieldName))
                        {
                            //如果默认坐标字段不存在或者未明确指定，尝试按xml格式读取并按文本输出
                            var getContent = new StringBuilder();
                            freeText.Export(saveAs: getContent, format: "xml");
                            _mainForm.BeginInvoke(method: () =>
                            {
                                _mainForm.MapBoxPropertyText = getContent.ToString();
                            });
                        }
                        else
                        {
                            if (freeText.RecordCount == 0)
                                throw new Exception(message: "No features found.");
                            var capabilities = freeText.GetCapabilities();
                            if (capabilities != null)
                            {
                                var bbox = (JArray)capabilities[propertyName: "bbox"]; //西南东北
                                if (bbox != null)
                                {
                                    var west = (double)bbox[index: 0];
                                    var south = (double)bbox[index: 1];
                                    var east = (double)bbox[index: 2];
                                    var north = (double)bbox[index: 3];

                                    if (_projectionHelper != null)
                                    {
                                        try
                                        {
                                            var westSouth = _projectionHelper.Project(geometry: new JArray { west, south });
                                            west = (double)westSouth[index: 0];
                                            south = (double)westSouth[index: 1];
                                        }
                                        catch
                                        {
                                            //如果投影失败，强制西南角
                                            west = -180;
                                            south = -90;
                                        }

                                        try
                                        {
                                            var eastNorth = _projectionHelper.Project(geometry: new JArray { east, north });
                                            east = (double)eastNorth[index: 0];
                                            north = (double)eastNorth[index: 1];
                                        }
                                        catch
                                        {
                                            //如果投影失败，强制东北角
                                            east = 180;
                                            north = 90;
                                        }
                                    }

                                    if (west is > 180 or < -180 || east is > 180 or < -180 || south is > 90 or < -90 ||
                                        north is > 90 or < -90)
                                        throw new Exception(message: @"The boundary has exceeded [±180, ±90].");
                                    if (west > east)
                                        west = -180;
                                    if (west > east)
                                        east = 180;
                                    if (south > north)
                                        south = -90;
                                    if (south > north)
                                        north = 90;
                                    _mainForm.MapBox.BeginInvoke(
                                        method: () =>
                                        {
                                            _mainForm.MapBox.SetZoomToFitRect(rect: RectLatLng.FromLTRB(leftLng: west,
                                                topLat: north, rightLng: east, bottomLat: south));
                                        }
                                    );
                                }

                                var count = FeaturesView(features: freeText.GetFeature());
                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                    userState: $@"{count} valid feature" + (count > 1 ? "s" : "") +
                                               $" in {Path.GetFileName(path: _path)}");
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally
                    {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); }
                        );
                    }

                    break;
                }
                case "geositexml":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(); }
                        );
                        using var xml = new GeositeXml.GeositeXml();
                        xml.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                    userState: thisEvent.Message ?? string.Empty);
                        };
                        var count = GeositeXmlView(
                            features: xml.GeositeXmlToGeositeXml(geositeXml: xml.GetTree(input: _path)).Root);
                        _backgroundWorker.ReportProgress(percentProgress: -1,
                            userState: $@"{count} valid feature" + (count > 1 ? "s" : "") +
                                       $" in {Path.GetFileName(path: _path)}");
                    }
                    catch (Exception error)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); });
                    }

                    break;
                }
                case "kml":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(); });
                        using var kml = new GeositeXml.GeositeXml();
                        kml.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                    userState: thisEvent.Message ?? string.Empty);
                        };
                        var count = GeositeXmlView(features: kml.KmlToGeositeXml(kml: kml.GetTree(input: _path)).Root);
                        _backgroundWorker.ReportProgress(percentProgress: -1,
                            userState:
                            $@"{count} valid feature{(count > 1 ? "s" : "")} in {Path.GetFileName(path: _path)}");
                    }
                    catch (Exception error)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); });
                    }

                    break;
                }
                case "geojson":
                {
                    try
                    {
                        using var geoJsonObject = new GeositeXml.GeositeXml();
                        geoJsonObject.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                    userState: thisEvent.Message ?? string.Empty);
                        };
                        var getGeositeXml = new StringBuilder();
                        geoJsonObject.GeoJsonToGeositeXml(input: _path, output: getGeositeXml);
                        if (getGeositeXml.Length > 0)
                        {
                            var count = GeositeXmlView(features: XElement.Parse(text: getGeositeXml.ToString()));
                            _backgroundWorker.ReportProgress(percentProgress: -1,
                                userState:
                                $@"{count} valid feature{(count > 1 ? "s" : "")} in {Path.GetFileName(path: _path)}");
                        }
                    }
                    catch (Exception error)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally
                    {
                        _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); });
                    }

                    break;
                }
                default:
                {
                    //文档树要素类型码构成的数组（类型码约定：
                    //0：非空间数据【默认】 ✔
                    //1：Point点 ✔
                    //2：Line线 ✔
                    //3：Polygon面 ✔
                    //4：Image地理贴图 ✔
                    //10000：Wms栅格金字塔瓦片服务类型[epsg:0 - 无投影瓦片]
                    //10001：Wms瓦片服务类型[epsg:4326 - 地理坐标系瓦片]
                    //10002：Wms栅格金字塔瓦片服务类型[epsg:3857 - 球体墨卡托瓦片] ✔
                    //11000：Wmts栅格金字塔瓦片类型[epsg:0 - 无投影瓦片]
                    //11001：Wmts栅格金字塔瓦片类型[epsg:4326 - 地理坐标系瓦片]
                    //11002：Wmts栅格金字塔瓦片类型[epsg:3857 - 球体墨卡托瓦片] ✔
                    //12000：WPS栅格平铺式瓦片类型[epsg:0 - 无投影瓦片]
                    //12001：WPS栅格平铺式瓦片类型[epsg:4326 - 地理坐标系瓦片]
                    //12002：WPS栅格平铺式瓦片类型[epsg:3857 - 球体墨卡托瓦片]
                    var typeArray = Regex.Split(
                        input: _type ?? "0",
                        pattern: @"[\s,\s]+"
                    );
                    var geositeServerArray = Regex.Split(
                        input: _path, //path layer leaf
                        pattern: "\b"
                    );
                    var count = 0L;
                    var webApi = geositeServerArray[0];
                    var layer = geositeServerArray[1];
                    var leaf = geositeServerArray[2];
                    if (typeArray.Contains(value: "0") || typeArray.Contains(value: "1") || typeArray.Contains(value: "2") ||
                        typeArray.Contains(value: "3") || typeArray.Contains(value: "4"))
                    {
                        //WFS服务模板示例：http://localhost:5000/getFeature?service=wfs&resultType=hits&typeNames=a.b&outputFormat=2&count=100
                        var callPath = $"{webApi}getFeature?service=wfs&resultType=hits&outputFormat=2&count=100&typeNames={layer}";
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: callPath);
                        var getResponse = new WebProxy().Call(
                            path: callPath,
                            timeout: 36000
                        );
                        if (getResponse.IsSuccessful)
                        {
                            var content = getResponse.Content;
                            if (content != null)
                            {
                                var geositeXml = XElement.Parse(text: content);
                                if (int.TryParse(s: geositeXml.Attribute(name: "numberMatched")?.Value, result: out var numberMatched) &&
                                    numberMatched > 0)
                                {
                                    if (!bool.TryParse(value: geositeXml.Attribute(name: "estimate")?.Value, result: out var estimate))
                                        estimate = false;
                                    _backgroundWorker.ReportProgress(
                                        percentProgress: -1,
                                        userState:
                                        $@"{(estimate ? "About " : "")}[{numberMatched}] feature{(numberMatched > 1 ? "s" : "")} found, loading ...");
                                    var next = geositeXml.Attribute(name: "next")?.Value;
                                    while (!string.IsNullOrWhiteSpace(value: next))
                                    {
                                        if (_backgroundWorker.CancellationPending)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        var theCount = count;

                                        var tip = $"{theCount} / {numberMatched} features loading ...";
                                        //_backgroundWorker.ReportProgress(
                                        //    percentProgress: -1,
                                        //    userState:
                                        //    tip);
                                        _mainForm.MapBox.BeginInvoke(method: () =>
                                        {
                                            _mainForm.SetStatusText(text: tip);
                                        });
                                        Application.DoEvents();
                                        getResponse = new WebProxy().Call(
                                            path: next,
                                            timeout: 0
                                        );
                                        if (getResponse.IsSuccessful)
                                        {
                                            content = getResponse.Content;
                                            if (content != null)
                                            {
                                                geositeXml = XElement.Parse(text: content);
                                                next = geositeXml.Attribute(name: "next")?.Value;
                                                if (!int.TryParse(s: geositeXml.Attribute(name: "numberReturned")?.Value,
                                                        result: out var numberReturned))
                                                    numberReturned = 0;
                                                if (numberReturned > 0)
                                                {
                                                    if (count == 0L)
                                                    {
                                                        var bbox = geositeXml.Element(name: "boundary");
                                                        if (bbox != null)
                                                        {
                                                            if (double.TryParse(s: bbox.Element(name: "north")?.Value,
                                                                    result: out var north))
                                                            {
                                                                if (double.TryParse(
                                                                        s: bbox.Element(name: "south")?.Value,
                                                                        result: out var south))
                                                                {
                                                                    if (double.TryParse(
                                                                            s: bbox.Element(name: "west")?.Value,
                                                                            result: out var west))
                                                                    {
                                                                        if (double.TryParse(
                                                                                s: bbox.Element(name: "east")?.Value,
                                                                                result: out var east))
                                                                        {
                                                                            if (west is > 180 or < -180 || west > east)
                                                                                west = -180;
                                                                            if (east is > 180 or < -180 || west > east)
                                                                                east = 180;
                                                                            if (south is > 90 or < -90 || south > north)
                                                                                south = -90;
                                                                            if (north is > 90 or < -90 || south > north)
                                                                                north = 90;
                                                                            _mainForm.MapBox.BeginInvoke(
                                                                                method: () =>
                                                                                {
                                                                                    _mainForm.MapBox.SetZoomToFitRect(
                                                                                        rect: RectLatLng.FromLTRB(
                                                                                            leftLng: west,
                                                                                            topLat: north,
                                                                                            rightLng: east,
                                                                                            bottomLat: south
                                                                                        )
                                                                                    );
                                                                                }
                                                                            );
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    count += GeositeXmlView(features: geositeXml, realZoom: false);
                                                }
                                                else
                                                    next = null;
                                            }
                                            else
                                                next = null;
                                        }
                                        else
                                            next = null;
                                    }
                                }
                                else
                                    _backgroundWorker.ReportProgress(percentProgress: -1, userState: @"No vector features found.");
                            }
                            else
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: @"No vector features found.");
                        }
                        else
                            _backgroundWorker.ReportProgress(percentProgress: -1, userState: getResponse.ErrorMessage);
                    }
                    else
                    {
                        if (typeArray.Contains(value: "10002"))
                        {
                            //不支持：10001：Wms瓦片服务类型[epsg:4326 - 地理坐标系瓦片]
                            //10002：Wms栅格金字塔瓦片服务类型[epsg:3857 - 球体墨卡托瓦片]
                            var callPath = $"{webApi}getTile?service=wms&layer={layer}";
                            _backgroundWorker.ReportProgress(percentProgress: -1, userState: callPath);
                            var getResponse = new WebProxy().Call(
                                path: callPath,
                                timeout: 5000
                            );
                            if (getResponse.IsSuccessful)
                            {
                                var content = getResponse.Content;
                                if (content != null)
                                    count += GeositeXmlView(
                                        features: new XElement(
                                            name: "members",
                                            content: XElement.Parse(text: content).Descendants(name: "wms").ToArray()
                                                .Select(
                                                    selector: wms =>
                                                        new XElement(
                                                            name: "member",
                                                            content: new object[]
                                                            {
                                                                new XAttribute(name: "type", value: "Tile"),
                                                                new XElement(name: "wms", content: wms.Value), 
                                                                _property
                                                            })
                                                ).ToList()
                                        ),
                                        realZoom: false);
                                else
                                    _backgroundWorker.ReportProgress(percentProgress: -1, userState: @"No WMS found.");
                            }
                            else
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: getResponse.ErrorMessage);
                        }
                        else
                        {
                            if (typeArray.Contains(value: "11002"))
                            {
                                //不支持：11001：Wmts栅格金字塔瓦片类型[epsg:4326 - 地理坐标系瓦片]
                                //11002：Wmts栅格金字塔瓦片类型[epsg:3857 - 球体墨卡托瓦片]
                                count += GeositeXmlView(
                                    features: new XElement(
                                        name: "member",
                                        content: new object[]
                                        {
                                            new XAttribute(name: "type", value: "Tile"),
                                            //注：GeositeServer提供的getTile指令支持采用括号封闭的(叶子id)充当图层路由，这比常规方式更加高效
                                            new XElement(
                                                name: "wms",
                                                content:
                                                $"{webApi}getTile?service=wmts&layer=({leaf})&tileMatrix={{z}}&tileCol={{x}}&tileRow={{y}}"
                                            ),
                                            _property
                                        }),
                                    realZoom: false);
                            }
                            else
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: $@"[{layer}] layer type is not supported.");
                        }
                    }

                    var resultMessage = $@"[{count}] feature{(count > 1 ? "s" : "")} loaded completed.";
                    _backgroundWorker.ReportProgress(
                        percentProgress: -1,
                        userState: resultMessage
                    );
                    _mainForm.MapBox.BeginInvoke(method: () => { _mainForm.SetStatusText(text: resultMessage); });
                    break;
                }
            }

            long FeaturesView(IEnumerable<JObject> features)
            {
                var count = 0;
                features?.AsParallel().ForAll(
                    action: feature =>
                    {
                        if (_backgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Application.DoEvents();
                        /* feature 对象样例：
                        {
                            "type": "Feature",
                            "timeStamp": "2023-02-10T10:45:05",
                            "style": {
                                "fillColor": "#DFE7FF",
                                "fillPattern": 0,
                                "patternHeight": 0,
                                "patternWidth": 0,
                                "lineWidth": 0,
                                "patternColor": "#FFFFFF",
                                "layer": 0,
                                "transparent": 0
                            },
                            "properties": {
                                "ID": {
                                    "type": "int",
                                    "length": 4,
                                    "value": 3
                                },
                                "面积": {
                                    "type": "double",
                                    "length": 8,
                                    "decimal": 6,
                                    "value": 1195.127851497529
                                },
                                "周长": {
                                    "type": "double",
                                    "length": 8,
                                    "decimal": 6,
                                    "value": 140.5087108013937
                                }
                            },
                            "id": 2,
                            "geometry": {
                                "type": "Polygon",
                                "coordinates": [
                                    [
                                        [-99.98667338516243,40.979094076655045],
                                        [-58.63127268829834,40.979094076655045],
                                        [-58.63127268829834,69.8780487804878],
                                        [-99.98667338516243,69.8780487804878],
                                        [-99.98667338516243,40.979094076655045]
                                    ]
                                ]
                            },
                            "centroid": [-99.98667338516243,40.979094076655045],
                            "bbox": [-99.98667338516243,40.979094076655045, -58.63127268829834,69.8780487804878]
                        }         
                        */
                        if (feature != null)
                        {
                            count++;
                            _mainForm.MapBox.BeginInvoke(
                                method: () =>
                                {
                                    var property = (JObject)feature[propertyName: "properties"];
                                    var style = (JObject)feature[propertyName: "style"];
                                    var geometry = (JObject)feature[propertyName: "geometry"];
                                    if (geometry != null)
                                    {
                                        var type = geometry[propertyName: "type"]?.ToString();
                                        var coordinate = (JArray)geometry[propertyName: "coordinates"];
                                        if (type != null && coordinate != null)
                                            try
                                            {
                                                switch (type)
                                                {
                                                    case "Point":
                                                    case "MultiPoint":
                                                    {
                                                        Point(type: type, coordinate: coordinate, property: property,
                                                            style: style);
                                                        break;
                                                    }
                                                    case "LineString":
                                                    case "MultiLineString":
                                                    {
                                                        Line(type: type, coordinate: coordinate, property: property,
                                                            style: style);
                                                        break;
                                                    }
                                                    case "Polygon":
                                                    case "MultiPolygon":
                                                    {
                                                        Polygon(type: type, coordinate: coordinate, property: property,
                                                            style: style);
                                                        break;
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                                    userState: ex.Message);
                                            }
                                    }
                                }
                            );
                        }
                    }
                );
                return count;
            }

            long GeositeXmlView(XElement features, bool realZoom = true)
            {
                var count = 0;
                if (features != null)
                {
                    if (realZoom)
                    {
                        var bbox = features.Element(name: "boundary");
                        if (bbox != null)
                        {
                            if (double.TryParse(s: bbox.Element(name: "north")?.Value, result: out var north))
                            {
                                if (double.TryParse(s: bbox.Element(name: "south")?.Value, result: out var south))
                                {
                                    if (double.TryParse(s: bbox.Element(name: "west")?.Value, result: out var west))
                                    {
                                        if (double.TryParse(s: bbox.Element(name: "east")?.Value, result: out var east))
                                        {
                                            if (west is > 180 or < -180 || west > east)
                                                west = -180;
                                            if (east is > 180 or < -180 || west > east)
                                                east = 180;
                                            if (south is > 90 or < -90 || south > north)
                                                south = -90;
                                            if (north is > 90 or < -90 || south > north)
                                                north = 90;
                                            _mainForm.MapBox.BeginInvoke(
                                                method: () =>
                                                {
                                                    _mainForm.MapBox.SetZoomToFitRect(
                                                        rect: RectLatLng.FromLTRB(
                                                            leftLng: west,
                                                            topLat: north,
                                                            rightLng: east,
                                                            bottomLat: south
                                                        )
                                                    );
                                                }
                                            );
                                        }
                                    }
                                }
                            }
                        }
                    }

                    features.DescendantsAndSelf(name: "member").AsParallel().ForAll(
                        action: member =>
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            Application.DoEvents();
                            count++;
                            _mainForm.MapBox.BeginInvoke(
                                method: () =>
                                {
                                    /*  要素类型
                                        "Point" 
                                        "Line" 
                                        "Polygon" 
                                        "Image" 
                                        "Tile"
                                        "" //非空间数据，通常为表格化文本
                                     */
                                    var type = member.Attribute(name: "type")?.Value;
                                    //要素几何（JSON/WKT/WKB/KML 格式）
                                    var geometryX = member.Element(name: "geometry");
                                    var geometryFormat =
                                        geometryX?.Attribute(name: "format")?.Value ?? "JSON"; //（JSON/WKT/WKB 格式）
                                    var geometryType = geometryX?.Attribute(name: "type")?.Value;
                                    var geometryString = geometryX?.Value.Trim(); //要素几何坐标（或者贴图边框）
                                    //要素属性
                                    var propertyX = member.Element(name: "property");
                                    //要素样式
                                    var styleX = member.Element(name: "style");
                                    switch (type)
                                    {
                                        case "Point":
                                        case "Line":
                                        case "Polygon":
                                        {
                                            if (geometryString != null)
                                            {
                                                var coordinate = geometryFormat.ToLower() switch
                                                {
                                                    "json" => JArray.Parse(json: geometryString),
                                                    "wkt" => JArray.Parse(json: OGCformat.WktToGeoJson(
                                                        geometryCode: type switch
                                                        {
                                                            "Point" => 0,
                                                            "Line" => 1,
                                                            "Polygon" => 2,
                                                            _ => -1
                                                        }, geometry: geometryString, simplify: true)),
                                                    "wkb" => JArray.Parse(json: OGCformat.WktToGeoJson(
                                                        geometryCode: type switch
                                                        {
                                                            "Point" => 0,
                                                            "Line" => 1,
                                                            "Polygon" => 2,
                                                            _ => -1
                                                        }, geometry: OGCformat.WkbToWkt(wkb: geometryString).wkt,
                                                        simplify: true)),
                                                    _ => null
                                                };
                                                JObject property = null;
                                                JObject style = null;
                                                try
                                                {
                                                    property = propertyX != null
                                                        ? JObject.Parse(
                                                            json: JsonConvert.SerializeXNode(node: propertyX))
                                                        : null;
                                                    style = styleX != null
                                                        ? JObject.Parse(json: JsonConvert.SerializeXNode(node: styleX))
                                                        : null;
                                                }
                                                catch
                                                {
                                                    //
                                                }
                                                finally
                                                {
                                                    switch (type)
                                                    {
                                                        case "Point":
                                                        {
                                                            Point(
                                                                type: geometryType,
                                                                coordinate: coordinate,
                                                                property: property,
                                                                style: style
                                                            );
                                                            break;
                                                        }
                                                        case "Line":
                                                        {
                                                            Line(
                                                                type: geometryType,
                                                                coordinate: coordinate,
                                                                property: property,
                                                                style: style
                                                            );
                                                            break;
                                                        }
                                                        case "Polygon":
                                                        {
                                                            Polygon(
                                                                type: geometryType,
                                                                coordinate: coordinate,
                                                                property: property,
                                                                style: style
                                                            );
                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                        }
                                        case "Image":
                                        {
                                            if (geometryString != null && styleX != null)
                                            {
                                                /*  贴图地址
                                                    <style>
                                                        <href>远程或本地图片</href>
                                                    </style>                                                 
                                                */
                                                var href = styleX.Element(name: "href")?.Value;
                                                if (!string.IsNullOrWhiteSpace(value: href))
                                                {
                                                    //贴图边框 - [[[west, south],[east, south],[east, north],[west, north],[west, south]]]
                                                    var coordinate = JArray.Parse(json: geometryString);
                                                    JObject property = null;
                                                    try
                                                    {
                                                        property = JObject.Parse(json: JsonConvert.SerializeXNode(node: propertyX));
                                                    }
                                                    catch
                                                    {
                                                        //
                                                    }
                                                    finally
                                                    {
                                                        Image(
                                                            href: Regex.Replace(
                                                                input: href,
                                                                pattern: @"[\s]*?([\s\S]*?)[\s]*",
                                                                replacement: "$1",
                                                                options: RegexOptions.Singleline |
                                                                         RegexOptions.Multiline
                                                            ),
                                                            coordinate: coordinate,
                                                            property: property
                                                        );
                                                    }
                                                }
                                            }

                                            break;
                                        }
                                        case "Tile":
                                        {
                                            var wms = member.Element(name: "wms")?.Value.Trim(); //需符合【{z} {x} {y}】模板
                                            if (!string.IsNullOrWhiteSpace(value: wms))
                                            {
                                                JObject property = null;
                                                try
                                                {
                                                    property = JObject.Parse(json: JsonConvert.SerializeXNode(node: propertyX));
                                                }
                                                catch
                                                {
                                                    //
                                                }
                                                finally
                                                {
                                                    Tile(urlFormat: wms, property: property);
                                                }
                                            }

                                            break;
                                        }
                                        default:
                                        {
                                            //非空间数据，仅显示属性
                                            if (propertyX != null)
                                                _mainForm.BeginInvoke(
                                                    method: () =>
                                                    {
                                                        _mainForm.MapBoxPropertyText +=
                                                            (
                                                                string.IsNullOrWhiteSpace(
                                                                    value: _mainForm.MapBoxPropertyText)
                                                                    ? ""
                                                                    : "\n"
                                                            ) +
                                                            propertyX.ToString(options: SaveOptions.None);
                                                    }
                                                );
                                            break;
                                        }
                                    }
                                }
                            );
                        }
                    );
                }

                return count;
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                LogMessageAdd(message: $@"{e.UserState}");
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispose();
        }

        private void Point(string type, JArray coordinate, JObject property, JObject style)
        {
            do
            {
                switch (type)
                {
                    case "Point":
                    {
                        if (_projectionHelper != null)
                            coordinate = _projectionHelper.Project(geometry: coordinate, type: type);
                        var lng = (double)coordinate[index: 0];
                        var lat = (double)coordinate[index: 1];
                        lock (Features.Markers)
                        {
                            switch (_pointStyle.flag)
                            {
                                case 0:
                                    //点圆
                                    Features.Markers.Add(
                                        item: new GMapMarkerCircle(position: new PointLatLng(lat: lat, lng: lng),
                                            strokeNormal: _pointStyle.pen)
                                        {
                                            IsHitTestVisible = true,
                                            Tag = (property, style)
                                        }
                                    );
                                    break;
                                default:
                                    //按钉
                                    Features.Markers.Add(
                                        item: new GMapMarkerPushpin(
                                            position: new PointLatLng(lat: lat, lng: lng)
                                        )
                                        {
                                            IsHitTestVisible = true,
                                            Tag = (property, style)
                                        }
                                    );
                                    break;
                            }
                        }

                        return;
                    }
                    case "MultiPoint":
                    {
                        if (_projectionHelper != null)
                            coordinate = _projectionHelper.Project(geometry: coordinate, type: type);
                        foreach (var point in coordinate)
                        {
                            var vertex = (JArray)point;
                            var lng = (double)vertex[index: 0];
                            var lat = (double)vertex[index: 1];
                            lock (Features.Markers)
                            {
                                switch (_pointStyle.flag)
                                {
                                    case 0:
                                        //点圆
                                        Features.Markers.Add(
                                            item: new GMapMarkerCircle(position: new PointLatLng(lat: lat, lng: lng),
                                                strokeNormal: _pointStyle.pen)
                                            {
                                                IsHitTestVisible = true,
                                                Tag = (property, style)
                                            }
                                        );
                                        break;
                                    default:
                                        //按钉
                                        Features.Markers.Add(
                                            item: new GMapMarkerPushpin(
                                                position: new PointLatLng(lat: lat, lng: lng)
                                            )
                                            {
                                                IsHitTestVisible = true,
                                                Tag = (property, style)
                                            }
                                        );
                                        break;
                                }
                            }
                        }

                        return;
                    }
                    default:
                    {
                        type = coordinate[index: 0]?.Type.ToString() switch
                        {
                            "Array" => "MultiPoint",
                            _ => "Point"
                        };
                        break;
                    }
                }
            } while (true);
        }

        private void Line(string type, JArray coordinate, JObject property, JObject style)
        {
            do
            {
                switch (type)
                {
                    case "LineString":
                    {
                        if (_projectionHelper != null)
                            coordinate = _projectionHelper.Project(geometry: coordinate, type: type);
                        lock (Features.Routes)
                            Features.Routes.Add(
                                item: new GMapRouteLine(
                                    points: (
                                        from JArray vertex in coordinate
                                        let lng = (double)vertex[index: 0]
                                        let lat = (double)vertex[index: 1]
                                        select new PointLatLng(lat: lat, lng: lng)
                                    ).ToList(),
                                    name: "LineString", strokeNormal: _lineStyle
                                )
                                {
                                    IsHitTestVisible = true,
                                    Tag = (property, style)
                                }
                            );
                        return;
                    }
                    case "MultiLineString":
                    {
                        if (_projectionHelper != null)
                            coordinate = _projectionHelper.Project(geometry: coordinate, type: type);
                        foreach (var line in coordinate)
                        {
                            lock (Features.Routes)
                                Features.Routes.Add(
                                    item: new GMapRouteLine(
                                        points: (
                                            from JArray vertex in (JArray)line
                                            let lng = (double)vertex[index: 0]
                                            let lat = (double)vertex[index: 1]
                                            select new PointLatLng(lat: lat, lng: lng)
                                        ).ToList(),
                                        name: "MultiLineString", strokeNormal: _lineStyle
                                    )
                                    {
                                        IsHitTestVisible = true,
                                        Tag = (property, style)
                                    }
                                );
                        }

                        return;
                    }
                    default:
                    {
                        type = coordinate[index: 0]?[key: 0]?.Type.ToString() switch
                        {
                            "Array" => "MultiLineString",
                            _ => "LineString"
                        };
                        break;
                    }
                }
            } while (true);
        }

        private void Polygon(string type, JArray coordinate, JObject property, JObject style)
        {
            do
            {
                switch (type)
                {
                    case "Polygon":
                    {
                        if (_projectionHelper != null)
                            coordinate = _projectionHelper.Project(geometry: coordinate, type: type);
                        //单面：  [[[x,y,...],[x,y,...],...],...]
                        //母子面：[[[x,y,...],[x,y,...],...],[[x,y,...],[x,y,...],[x,y,...],...],...]
                        for (var index = 0; index < coordinate.Count; index++)
                        {
                            var ring = (JArray)coordinate[index: index];
                            var lineList = new List<PointLatLng>();
                            foreach (var line in ring)
                            {
                                if (line != null)
                                {
                                    var theLine = (JArray)line;
                                    switch (theLine[index: 0].GetType().Name)
                                    {
                                        case "JArray":
                                        {
                                            lineList.AddRange(
                                                collection: from JArray vertex in theLine
                                                select new PointLatLng(
                                                    lat: (double)vertex[index: 1],
                                                    lng: (double)vertex[index: 0]
                                                )
                                            );
                                            break;
                                        }
                                        case "JValue":
                                        {
                                            lineList.Add(
                                                item: new PointLatLng(
                                                    lat: (double)theLine[index: 1],
                                                    lng: (double)theLine[index: 0]
                                                )
                                            );
                                            break;
                                        }
                                    }
                                }
                            }
                            lock (Features.Polygons)
                            {
                                //var polygonShow = new GMapPolygonArea(
                                //    points: lineList,
                                //    name: "Polygon",
                                //    strokeNormal: _polygonStyle,
                                //    fillNormal: index == 0
                                //        ? new SolidBrush(color: Color.FromArgb(alpha: 50, red: _polygonStyle.Color.R,
                                //            green: _polygonStyle.Color.G, blue: _polygonStyle.Color.B))
                                //        : new SolidBrush(color: Color.FromArgb(alpha: 255, red: 255, green: 255,
                                //            blue: 255))
                                //);

                                var polygonShow = new GMapPolygonArea(
                                    points: lineList,
                                    name: "Polygon",
                                    strokeNormal: _polygonStyle,
                                    //fillNormal: new SolidBrush(
                                    //    color: Color.FromArgb(
                                    //        alpha: index == 0 ? 50 : 255,
                                    //        red: index == 0 ? _polygonStyle.Color.R : Color.White.R,
                                    //        green: index == 0 ? _polygonStyle.Color.G : Color.White.G,
                                    //        blue: index == 0 ? _polygonStyle.Color.B : Color.White.B
                                    //    )
                                    //)
                                    fillNormal: new SolidBrush(
                                        color: Color.FromArgb(
                                            alpha: 50,
                                            red: _polygonStyle.Color.R,
                                            green: _polygonStyle.Color.G,
                                            blue: _polygonStyle.Color.B
                                        )
                                    )
                                );
                                if (index == 0)
                                {
                                    polygonShow.IsHitTestVisible = true;
                                    polygonShow.Tag = (property, style);
                                }
                                else
                                    polygonShow.IsHitTestVisible = false;
                                Features.Polygons.Add(item: polygonShow);
                            }
                        }
                        return;
                    }
                    case "MultiPolygon":
                    {
                        if (_projectionHelper != null)
                            coordinate = _projectionHelper.Project(geometry: coordinate, type: type);
                        //[[[[x,y,...],[x,y,...],[x,y,...],[x,y,...],[x,y,...],...],...],[[[x,y,...],[x,y,...],[x,y,...],[x,y,...],[x,y,...],...],...]] 
                        //MULTIPOLYGON(((0 0,4 0,4 4,0 4,0 0),(1 1,2 1,2 2,1 2,1 1)),((-1 -1,-1 -2,-2 -2,-2 -1,-1 -1)))
                        foreach (var onePolygon in coordinate)
                        {
                            var thePolygon = (JArray)onePolygon;
                            for (var index = 0; index < thePolygon.Count; index++)
                            {
                                var ring = thePolygon[index: index];
                                var lineList = new List<PointLatLng>();
                                foreach (var line in (JArray)ring)
                                {
                                    if (line != null)
                                    {
                                        var theLine = (JArray)line;
                                        switch (theLine[index: 0].GetType().Name)
                                        {
                                            case "JArray":
                                            {
                                                lineList.AddRange(
                                                    collection: from JArray vertex in theLine
                                                    select new PointLatLng(
                                                        lat: (double)vertex[index: 1],
                                                        lng: (double)vertex[index: 0]
                                                    )
                                                );
                                                break;
                                            }
                                            case "JValue":
                                            {
                                                lineList.Add(
                                                    item: new PointLatLng(
                                                        lat: (double)theLine[index: 1],
                                                        lng: (double)theLine[index: 0]
                                                    )
                                                );
                                                break;
                                            }
                                        }
                                    }
                                }

                                lock (Features.Polygons)
                                {
                                    //var polygonShow = new GMapPolygonArea(
                                    //    points: lineList,
                                    //    name: "Polygon",
                                    //    strokeNormal: _polygonStyle,
                                    //    fillNormal: index == 0
                                    //        ? new SolidBrush(color: Color.FromArgb(alpha: 50, red: _polygonStyle.Color.R,
                                    //            green: _polygonStyle.Color.G, blue: _polygonStyle.Color.B))
                                    //        : new SolidBrush(color: Color.FromArgb(alpha: 255, red: 255, green: 255,
                                    //            blue: 255))
                                    //);
                                    var polygonShow = new GMapPolygonArea(
                                        points: lineList,
                                        name: "Polygon",
                                        strokeNormal: _polygonStyle,
                                        fillNormal: new SolidBrush(
                                            color: Color.FromArgb(
                                                alpha: 50,
                                                red: _polygonStyle.Color.R,
                                                green: _polygonStyle.Color.G, 
                                                blue: _polygonStyle.Color.B
                                            )
                                        )
                                    );
                                    if (index == 0)
                                    {
                                        polygonShow.IsHitTestVisible = true;
                                        polygonShow.Tag = (property, style);
                                    }
                                    else
                                        polygonShow.IsHitTestVisible = false;
                                    Features.Polygons.Add(item: polygonShow);
                                }
                            }
                        }
                        return;
                    }
                    default:
                    {
                        type = coordinate[index: 0]?[key: 0]?[key: 0]?.Type.ToString() switch
                        {
                            "Array" => "MultiPolygon",
                            _ => "Polygon"
                        };
                        break;
                    }
                }
            } while (true);
        }

        private void Image(string href, JArray coordinate, JObject property, JObject style = null, int timeout = 5000)
        {
            try
            {
                var corner = (JArray)coordinate[index: 0];
                var westSouth = (JArray)corner[index: 0];
                var eastNorth = (JArray)corner[index: 2];
                var topLeft = new PointLatLng(lat: (double)eastNorth[index: 1], lng: (double)westSouth[index: 0]);
                var bottomRight = new PointLatLng(lat: (double)westSouth[index: 1], lng: (double)eastNorth[index: 0]);
                var theUri = new Uri(uriString: href);
                Bitmap image = null;
                if (theUri.IsFile)
                    image = new Bitmap(filename: theUri.LocalPath);
                else
                {
                    var getResponse = new WebProxy().Call(path: theUri.AbsoluteUri, timeout: timeout);
                    if (getResponse.IsSuccessful)
                    {
                        var blob = getResponse.RawBytes;
                        if (blob is { Length: > 0 })
                        {
                            using var ms = new MemoryStream(buffer: blob);
                            image = new Bitmap(stream: ms);
                        }
                    }
                }

                lock (Features.Markers)
                    Features.Markers.Add(
                        item: new GMapMarkerGround(image: image, topLeft: topLeft, bottomRight: bottomRight)
                        {
                            IsHitTestVisible = true,
                            Tag = (property, style)
                        }
                    );
            }
            catch (Exception e)
            {
                _backgroundWorker.ReportProgress(percentProgress: -1, userState: e.Message);
            }
        }

        private void Tile(string urlFormat, JObject property)
        {
            var propertyJson = property?["property"];
            var propertyHasValues = (propertyJson ?? false).HasValues;
            /*
             <property>
                   <maxZoom>3</maxZoom>
                   <minZoom>0</minZoom>
                   <boundary>
                       <east>180</east>
                       <west>-180</west>
                       <north>90</north>
                       <south>-90</south>
                   </boundary>
                   <tileSize>256</tileSize>
                   <type>11001</type>
               </property>
             */
            //10000：Wms栅格金字塔瓦片服务类型[epsg:0 - 无投影瓦片]
            //10001：Wms瓦片服务类型[epsg:4326 - 地理坐标系瓦片]
            //10002：Wms栅格金字塔瓦片服务类型[epsg:3857 - 球体墨卡托瓦片] ✔
            //11000：Wmts栅格金字塔瓦片类型[epsg:0 - 无投影瓦片]
            //11001：Wmts栅格金字塔瓦片类型[epsg:4326 - 地理坐标系瓦片]
            //11002：Wmts栅格金字塔瓦片类型[epsg:3857 - 球体墨卡托瓦片] ✔
            //12000：WPS栅格平铺式瓦片类型[epsg:0 - 无投影瓦片]
            //12001：WPS栅格平铺式瓦片类型[epsg:4326 - 地理坐标系瓦片]
            //12002：WPS栅格平铺式瓦片类型[epsg:3857 - 球体墨卡托瓦片]

            var srid =
                int.Parse(
                        propertyHasValues
                            ? Regex.Split(propertyJson?[key: "type"]?.Value<string>() ?? "11002", @"[,\s]+")[0]
                            : "11002"
                    ) switch
                    {
                        10001 or
                            11001 or
                            12001 => 4326,
                        10002 or
                            11002 or
                            12002 => 3857,
                        _ => 0
                    };
            var tileSize = propertyHasValues ? propertyJson?[key: "tileSize"]?.Value<int>() ?? 256 : 256;

            var wmsLayer = new MapProvider(srid, tileSize)
            {
                UrlFormat = urlFormat,
                MaxZoom = propertyHasValues
                    ? propertyJson?[key: "maxZoom"]?.Value<int>() ??
                      propertyJson?[key: "maxzoom"]?.Value<int>() ?? 18
                    : 18,
                MinZoom = propertyHasValues
                    ? propertyJson?[key: "minZoom"]?.Value<int>() ??
                      propertyJson?[key: "minzoom"]?.Value<int>() ?? 0
                    : 0
            };

            var boundary = propertyHasValues
                ? propertyJson?[key: "boundary"]
                : null;
            if (boundary != null)
            {
                var north = boundary[key: "north"]?.Value<double>();
                var south = boundary[key: "south"]?.Value<double>();
                var west = boundary[key: "west"]?.Value<double>();
                var east = boundary[key: "east"]?.Value<double>();
                if (north != null && south != null && west != null && east != null)
                    wmsLayer.Area = RectLatLng.FromLTRB(leftLng: west.Value, topLat: north.Value, rightLng: east.Value, bottomLat: south.Value);
            }

            wmsLayer.Alpha = propertyHasValues
                ? propertyJson?[key: "opacity"]?.Value<float>() ?? 1f
                : 1f;

            wmsLayer.ServerLetters = propertyHasValues
                ? propertyJson?[key: "subdomains"]?.Value<string>() ?? propertyJson?[key: "subDomains"]?.Value<string>()
                : "";

            if (propertyHasValues && propertyJson != null)
            {
                var copyright = propertyJson[key: "copyright"]?.Value<string>()?? propertyJson[key: "Copyright"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(copyright))
                    wmsLayer.Copyright = copyright;
            }

            GMapProvider.OverlayTiles.Add(item: wmsLayer);
            _mainForm.MapBox.ReloadMap();
        }

        private void LogMessageAdd(string message)
        {
            if (!string.IsNullOrWhiteSpace(value: message))
                _mainForm.BeginInvoke(method: () => { _mainForm.FileLoadLogAdd(input: message); });
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            _backgroundWorker?.CancelAsync();
            _backgroundWorker?.Dispose();
            Tasks.Remove(item: this);
        }
    }
}
