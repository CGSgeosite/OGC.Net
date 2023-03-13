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
using GMap.NET.MapProviders.WmtsProvider;
using GMap.NET;
using GMap.NET.Extend;
using GMap.NET.WindowsForms;

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

        private string _path;

        private string _type;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainForm">主窗体</param>
        /// <param name="path">文件存储路径</param>
        /// <param name="type">文件类型（MapGIS、ShapeFile、TXT、CSV、Excel、KML、GeositeXML、GeoJson）</param>
        /// <exception cref="Exception"></exception>
        public MapView(MainForm mainForm, string path, string type)
        {
            _mainForm = mainForm;
            _path = path;
            _type = type;

            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;

            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
        }

        public void View()
        {
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();
            else
                _backgroundWorker.RunWorkerAsync();
        }

        public void LogMessageAdd(string message)
        {
            if (!string.IsNullOrWhiteSpace(value: message))
                _mainForm.BeginInvoke(
                    method: () => { _mainForm.FileLoadLogAdd(input: message); }
                );
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (_type?.ToLower())
            {
                case "mapgis":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(); }
                        );
                        var fileType = Path.GetExtension(path: _path)?.ToLower();
                        if (fileType == ".mpj")
                        {
                            var mapgisProject = new MapGis.MapGisProject();
                            mapgisProject.Open(file: _path);
                            var resultString = mapgisProject.Content.ToString(formatting: Formatting.Indented);
                            _mainForm.BeginInvoke(
                                method: () => { _mainForm.MapBoxPropertyText = resultString; }
                            );
                        }
                        else //.wt .wl .wp
                        {
                            using var mapgis = new MapGis.MapGisFile();
                            mapgis.OnMessagerEvent += delegate(object _, MessagerEventArgs thisEvent)
                            {
                                var progress = thisEvent.Progress;
                                if (progress == null)
                                    _backgroundWorker.ReportProgress(percentProgress: -1, userState: thisEvent.Message ?? string.Empty);
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
                                    //if (west is > 180 or < -180 || east is > 180 or < -180|| south is > 90 or < -90|| north is > 90 or < -90)
                                    //    throw new Exception(message: @"The file is not in longitude / latitude format.");
                                    if (west > east)
                                        west = -180;
                                    if (west > east)
                                        east = 180;
                                    if (south > north)
                                        south = -90;
                                    if (south > north)
                                        north = 90;
                                    _mainForm.MapBox.BeginInvoke(
                                        method: () => {
                                            _mainForm.MapBox.SetZoomToFitRect(rect: RectLatLng.FromLTRB(leftLng: west,
                                                topLat: north, rightLng: east, bottomLat: south));
                                        }
                                    );
                                }
                                var count = FeaturesView(features: mapgis.GetFeature());
                                _backgroundWorker.ReportProgress(percentProgress: -1,
                                    userState: $@"{count} valid feature{(count > 1 ? "s" : "")} in {Path.GetFileName(path: _path)}");
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
                case "shapefile":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(); }
                        );
                        var codePage = ShapeFile.GetDbfCodePage
                        (
                            dbfFileName: Path.Combine
                            (
                                path1: Path.GetDirectoryName(path: _path) ?? "",
                                path2: Path.GetFileNameWithoutExtension(path: _path) + ".dbf"
                            )
                        );
                        using var shapeFile = new ShapeFileReader();
                        shapeFile.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null) {
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: thisEvent.Message ?? string.Empty);
                            }
                        };
                        shapeFile.Open(filePath: _path, defaultCodePage: codePage.CodePage);
                        if (shapeFile.RecordCount == 0)
                            throw new Exception(message: "No features found.");
                        var capabilities = shapeFile.GetCapabilities();
                        if (capabilities != null)
                        {
                            var bbox = (JArray) capabilities[propertyName: "bbox"]; //西南东北
                            if (bbox != null)
                            {
                                var west = (double) bbox[index: 0];
                                var south = (double) bbox[index: 1];
                                var east = (double) bbox[index: 2];
                                var north = (double) bbox[index: 3];
                                //if (west is > 180 or < -180 || east is > 180 or < -180 || south is > 90 or < -90 || north is > 90 or < -90)
                                //    throw new Exception(message: @"The file is not in longitude / latitude format.");
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
                            _backgroundWorker.ReportProgress(percentProgress: -1, userState: $@"{count} valid feature" + (count > 1 ? "s" : "") + $" in {Path.GetFileName(path: _path)}");
                        }
                    }
                    catch (Exception error) {
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
                case "txt":
                case "csv":
                case "excel":
                {
                    try
                    {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(); }
                        );
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
                        freeText.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: thisEvent.Message ?? string.Empty);
                        };
                            freeText.Open(file: _path);
                        if (string.IsNullOrWhiteSpace(value: coordinateFieldName))
                        {
                            //如果默认坐标字段不存在或者未明确指定，尝试按xml格式读取并按文本输出
                            var getContent = new StringBuilder();
                            freeText.Export(saveAs: getContent, format: "xml");
                            _mainForm.BeginInvoke(
                                method: () => { _mainForm.MapBoxPropertyText = getContent.ToString(); }
                            );
                        }
                        else
                        {
                            if (freeText.RecordCount == 0)
                                throw new Exception(message: "No features found.");
                            var capabilities = freeText.GetCapabilities();
                            if (capabilities != null)
                            {
                                var bbox = (JArray) capabilities[propertyName: "bbox"]; //西南东北
                                if (bbox != null)
                                {
                                    var west = (double) bbox[index: 0];
                                    var south = (double) bbox[index: 1];
                                    var east = (double) bbox[index: 2];
                                    var north = (double) bbox[index: 3];
                                    //if (west is > 180 or < -180 || east is > 180 or < -180 || south is > 90 or < -90 || north is > 90 or < -90)
                                    //    throw new Exception(message: @"The file is not in longitude / latitude format.");
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
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: $@"{count} valid feature" + (count > 1 ? "s" : "") + $" in {Path.GetFileName(path: _path)}");
                            }
                        }
                    }
                    catch (Exception error) {
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
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: thisEvent.Message ?? string.Empty);
                        };
                        var count = GeositeXmlView(features: xml.GeositeXmlToGeositeXml(geositeXml: xml.GetTree(input: _path)).Root);
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: $@"{count} valid feature" + (count > 1 ? "s" : "") + $" in {Path.GetFileName(path: _path)}");
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
                case "kml":
                {
                    try {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(); }
                        );
                        using var kml = new GeositeXml.GeositeXml();
                        kml.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                        {
                            var progress = thisEvent.Progress;
                            if (progress == null)
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: thisEvent.Message ?? string.Empty);
                        };
                        var count = GeositeXmlView(features: kml.KmlToGeositeXml(kml: kml.GetTree(input: _path)).Root);
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: $@"{count} valid feature" + (count > 1 ? "s" : "") + $" in {Path.GetFileName(path: _path)}");
                    }
                    catch (Exception error) {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: error.Message);
                    }
                    finally {
                        _mainForm.MapBox.BeginInvoke(
                            method: () => { _mainForm.FilePreviewLoading.Run(onOff: false); }
                        );
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
                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: thisEvent.Message ?? string.Empty);
                        };
                        var getGeositeXml = new StringBuilder();
                        geoJsonObject.GeoJsonToGeositeXml(input: _path, output: getGeositeXml);
                        if (getGeositeXml.Length > 0)
                        {
                            var count = GeositeXmlView(features: XElement.Parse(text: getGeositeXml.ToString()));
                            _backgroundWorker.ReportProgress(percentProgress: -1,
                                userState: $@"{count} valid feature" + (count > 1 ? "s" : "") + $" in {Path.GetFileName(path: _path)}");
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
                //default:
                //    break;
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
                                                        Point(type: type, coordinate: coordinate, property: property,
                                                            style: style);
                                                        break;
                                                    case "LineString":
                                                    case "MultiLineString":
                                                        Line(type: type, coordinate: coordinate, property: property,
                                                            style: style);
                                                        break;
                                                    case "Polygon":
                                                    case "MultiPolygon":
                                                        Polygon(type: type, coordinate: coordinate, property: property,
                                                            style: style);
                                                        break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _backgroundWorker.ReportProgress(percentProgress: -1, userState: ex.Message);
                                            }
                                    }
                                }
                            );
                        }
                    }
                );
                return count;
            }

            long GeositeXmlView(XElement features)
            {
                var count = 0;
                if (features != null)
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
                                                _mainForm.MapBox.SetZoomToFitRect(rect: RectLatLng.FromLTRB(
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

                    _mainForm.BeginInvoke(
                        method: () => { _mainForm.MapBoxPropertyText = ""; }
                    );

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
                                    var geometryFormat = geometryX?.Attribute(name: "format")?.Value; //（JSON/WKT/WKB 格式）
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
                                                var coordinate = geometryFormat?.ToLower() switch
                                                {
                                                    "json" => JArray.Parse(json: geometryString),
                                                    "wkt" => JArray.Parse(json: OGCformat.WktToGeoJson(geometryCode: type switch
                                                    {
                                                        "Point" => 0,
                                                        "Line" => 1,
                                                        "Polygon" => 2,
                                                        _ => -1
                                                    }, geometry: geometryString, simplify: true)),
                                                    "wkb" => JArray.Parse(json: OGCformat.WktToGeoJson(geometryCode: type switch
                                                    {
                                                        "Point" => 0,
                                                        "Line" => 1,
                                                        "Polygon" => 2,
                                                        _ => -1
                                                    }, geometry: OGCformat.WkbToWkt(wkb: geometryString).wkt, simplify: true)),
                                                    _ => null
                                                };

                                                JObject property = null;
                                                JObject style = null;
                                                try
                                                {
                                                    property = propertyX != null
                                                        ? JObject.Parse(json: JsonConvert.SerializeXNode(node: propertyX))
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
                                                    finally
                                                    {
                                                        Image(
                                                            href: Regex.Replace(
                                                                input: href,
                                                                pattern: @"[\s]*?([\s\S]*?)[\s]*",
                                                                replacement: "$1",
                                                                options: RegexOptions.Singleline | RegexOptions.Multiline
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
                                                finally
                                                {
                                                    Tile(
                                                        urlFormat: wms,
                                                        property: property
                                                    );
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

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //var progressPercentage = e.ProgressPercentage;
            //if (progressPercentage >= 0)
            //{
            //    ; //progressBar.Value = e.ProgressPercentage;
            //}
            if (e.UserState != null)
                LogMessageAdd(message: $@"{e.UserState}");
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if (!e.Cancelled)
            //{
            //    if (e.Error == null)
            //    {
            //        if (e.Result != null)
            //        {
            //        }
            //    }
            //    else
            //    {
            //        //Failed
            //    }
            //}
            //else
            //{
            //    //Cancelled
            //}
        }

        public void Point(string type, JArray coordinate, JObject property, JObject style)
        {
            switch (type)
            {
                case "Point":
                {
                    var lng = (double)coordinate[index: 0];
                    var lat = (double)coordinate[index: 1];

                    ////按钉
                    //lock (Features.Markers)
                    //    Features.Markers.Add(
                    //        item: new GMapMarkerPushpin(
                    //            position: new PointLatLng(lat: lat, lng: lng)
                    //        )
                    //        {
                    //            IsHitTestVisible = true,
                    //            Tag = (property, style)
                    //        }
                    //    );

                    //点圆
                    lock (Features.Markers)
                        Features.Markers.Add(item: new GMapMarkerCircle(position: new PointLatLng(lat: lat, lng: lng))
                            {
                                IsHitTestVisible = true,
                                Tag = (property, style)
                            }
                        );

                    ////点方块
                    //lock (Features.Markers)
                    //    Features.Markers.Add(item: new GMapMarkerRect(new PointLatLng(lat: lat, lng: lng))
                    //        {
                    //            IsHitTestVisible = true,
                    //            Tag = (property: property, style: style)
                    //        }
                    //    );
                    break;
                }
                case "MultiPoint":
                {
                    foreach (var point in coordinate)
                    {
                        var vertex = (JArray)point;
                        var lng = (double)vertex[index: 0];
                        var lat = (double)vertex[index: 1];
                        lock (Features.Markers)
                            Features.Markers.Add(
                                item: new GMapMarkerPushpin(
                                    position: new PointLatLng(lat: lat, lng: lng)
                                )
                                {
                                    IsHitTestVisible = true,
                                    Tag = (property, style)
                                }
                            );
                    }
                    break;
                }
            }
        }

        public void Line(string type, JArray coordinate, JObject property, JObject style)
        {
            switch (type)
            {
                case "LineString":
                {
                    lock (Features.Routes)
                        Features.Routes.Add(item: new GMapRouteLine(
                                points: (
                                    from JArray vertex in coordinate
                                    let lng = (double)vertex[index: 0]
                                    let lat = (double)vertex[index: 1]
                                    select new PointLatLng(lat: lat, lng: lng)
                                ).ToList(),
                                name: "LineString"
                            )
                            {
                                IsHitTestVisible = true,
                                Tag = (property, style)
                            }
                        );
                    break;
                }
                case "MultiLineString":
                {
                    foreach (var line in coordinate)
                    {
                        lock (Features.Routes)
                            Features.Routes.Add(item: new GMapRouteLine(
                                    points: (
                                        from JArray vertex in (JArray)line
                                        let lng = (double)vertex[index: 0]
                                        let lat = (double)vertex[index: 1]
                                        select new PointLatLng(lat: lat, lng: lng)
                                    ).ToList(),
                                    name: "MultiLineString"
                                )
                                {
                                    IsHitTestVisible = true,
                                    Tag = (property, style)
                                }
                            );
                    }
                    break;
                }
            }
        }

        public void Polygon(string type, JArray coordinate, JObject property, JObject style)
        {
            switch (type)
            {
                case "Polygon":
                {
                    lock (Features.Polygons)
                        Features.Polygons.Add(item: new GMapPolygonArea(
                                points: (
                                    from JArray vertex in coordinate[index: 0]
                                    let lng = (double)vertex[index: 0]
                                    let lat = (double)vertex[index: 1]
                                    select new PointLatLng(lat: lat, lng: lng)
                                ).ToList(),
                                name: "Polygon"
                            )
                            {
                                IsHitTestVisible = true,
                                Tag = (property, style)
                            }
                        );
                    break;
                }
                case "MultiPolygon":
                {
                    foreach (var onePolygon in coordinate)
                    {
                        lock (Features.Polygons)
                            Features.Polygons.Add(item: new GMapPolygonArea(
                                    points: (
                                        from JArray vertex in onePolygon[key: 0]
                                        let lng = (double)vertex[index: 0]
                                        let lat = (double)vertex[index: 1]
                                        select new PointLatLng(lat: lat, lng: lng)
                                    ).ToList(),
                                    name: "MultiPolygon"
                                )
                                {
                                    IsHitTestVisible = true,
                                    Tag = (property, style)
                                }
                            );
                    }
                    break;
                }
            }
        }

        public void Image(string href, JArray coordinate, JObject property, JObject style = null, int timeout = 5000)
        {
            //coordinate 贴图边框 - [[[west, south],[east, south],[east, north],[west, north],[west, south]]]
            try
            {
                var corner = (JArray) coordinate[index: 0];
                var westSouth = (JArray) corner[index: 0];
                var eastNorth = (JArray) corner[index: 2];
                var topLeft = new PointLatLng(lat: (double) eastNorth[index: 1], lng: (double) westSouth[index: 0]);
                var bottomRight = new PointLatLng(lat: (double) westSouth[index: 1], lng: (double) eastNorth[index: 0]);

                var theUri = new Uri(uriString: href);
                Bitmap image = null;
                if (theUri.IsFile)
                {
                    //本地
                    image = new Bitmap(filename: theUri.LocalPath);
                }
                else
                {
                    //远程
                    var getResponse = new WebProxy().Call(path: theUri.AbsoluteUri,timeout: timeout);
                    if (getResponse.IsSuccessful)
                    {
                        var blob = getResponse.RawBytes;
                        if (blob is {Length: > 0})
                        {
                            using var ms = new MemoryStream(buffer: blob);
                            image = new Bitmap(stream: ms);
                        }
                    }
                }

                lock (Features.Markers)
                    Features.Markers.Add(
                        item: new GMapMarkerGround(
                            image: image,
                            topLeft: topLeft,
                            bottomRight: bottomRight
                        )
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

        public void Tile(string urlFormat, JObject property)
        {
            var propertyJson = property?[propertyName: "property"];
            var wmsLayer = WmtsProvider.Instance;
            wmsLayer.UrlFormat = urlFormat;
            wmsLayer.Alpha = propertyJson?[key: "opacity"]?.Value<float>() ?? 1f;
            wmsLayer.ServerLetters = propertyJson?[key: "subdomains"]?.Value<string>();
            //MinZoom MaxZoom Area
            GMapProvider.OverlayTiles.Add(item: wmsLayer);
            _mainForm.MapBox.ReloadMap();
        }

        public void Dispose()
        {
            _backgroundWorker?.CancelAsync();
            _backgroundWorker?.Dispose();
        }
    }
}
