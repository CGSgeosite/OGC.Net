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

using System.ComponentModel;
using Geosite.Properties;
using GMap.NET.Extend;
using GMap.NET;
using GMap.NET.WindowsForms;

/*
 * 鸣谢：chatGPT 在相交算法中提供的并行思路
 */
namespace Geosite
{
    public partial class TopologyCheckerForm : Form
    {
        public static GMapOverlay Features;

        private readonly MainForm _mainForm;

        private int _dangleCount;
        private int _pseudoCount;
        private int _coincideCount;
        private int _overlayCount;
        private int _intersectionCount;

        private readonly BackgroundWorker _backgroundWorker = new();

        public TopologyCheckerForm(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        private void TopologyCheckerForm_Load(object sender, EventArgs e)
        {
            var workingArea = Screen.GetWorkingArea(this);
            Location = new Point(
                workingArea.Width - Size.Width,
                workingArea.Height - Size.Height
            );

            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            CheckBoxDangle.Checked = RegEdit.Getkey(keyname: "CheckBoxDangle", defaultvalue: "0") == "1";
            CheckBoxPseudo.Checked = RegEdit.Getkey(keyname: "CheckBoxPseudo", defaultvalue: "0") == "1";
            CheckBoxCoincide.Checked = RegEdit.Getkey(keyname: "CheckBoxCoincide", defaultvalue: "0") == "1";
            CheckBoxOverlay.Checked = RegEdit.Getkey(keyname: "CheckBoxOverlay", defaultvalue: "0") == "1";
            CheckBoxIntersection.Checked = RegEdit.Getkey(keyname: "CheckBoxIntersection", defaultvalue: "0") == "1";

            TopologyStatusLabel.Text = _mainForm.GetCopyright;
        }

        private void TopologyRun_Click(object sender, EventArgs e)
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
                TopologyRun.Image = Resources.run;
                toolTip1.SetToolTip(TopologyRun, "Start");
                TopologyProgressBar.Value = 0;
                TopologyProgressBar.Visible = false;
            }
            else
            {
                LabelDangle.Text = CheckBoxDangle.Checked ? "0" : "";
                LabelPseudo.Text = CheckBoxPseudo.Checked ? "0" : "";
                LabelCoincide.Text = CheckBoxCoincide.Checked ? "0" : "";
                LabelOverlay.Text = CheckBoxOverlay.Checked ? "0" : "";
                LabelIntersection.Text = CheckBoxIntersection.Checked ? "0" : "";
                TopologyStatusLabel.Text = _mainForm.GetCopyright;

                //拓扑运算屏蔽膜，默认：0=不执行检查；1=悬挂节点；2=伪节点；4=线段存在的重叠点；8=重叠点；16=相交节点
                var topologyMask = (CheckBoxDangle.Checked ? 0b1 : 0b0) |
                                   (CheckBoxPseudo.Checked ? 0b10 : 0b0) |
                                   (CheckBoxCoincide.Checked ? 0b100 : 0b0) |
                                   (CheckBoxOverlay.Checked ? 0b1000 : 0b0) |
                                   (CheckBoxIntersection.Checked ? 0b10000 : 0b0);

                lock (Features.Markers)
                {
                    Features.Markers.Clear();
                }

                Application.DoEvents();

                _dangleCount =
                    _pseudoCount =
                        _coincideCount =
                            _overlayCount =
                                _intersectionCount = 0;

                var mapViewArea = _mainForm.MapBox.ViewArea;
                var north = mapViewArea.Top;
                var south = mapViewArea.Bottom;
                var west = mapViewArea.Left;
                var east = mapViewArea.Right;

                var features = MapView.Features;

                List<PointLatLng> markersList = null;
                List<Polyline> routesList = null;
                List<Polyline> polygonsList = null;

                var tip = false;

                var markers = features.Markers;
                if (markers is { Count: > 0 } && (topologyMask & 0b1000) > 0)
                {
                    _mainForm.FileLoadLogAdd(TopologyStatusLabel.Text = @"Finding Points in MapView ...");
                    Application.DoEvents();
                    markersList = (from marker in markers select marker.Position into position let lat = position.Lat let lng = position.Lng where lat >= south && lat <= north && lng >= west && lng <= east select position).ToList();
                    if (markersList.Count == 0)
                    {
                        _mainForm.FileLoadLogAdd(TopologyStatusLabel.Text = @"No points found in MapView.");
                        Application.DoEvents();
                    }
                }

                var routes = features.Routes;
                if (routes is { Count: > 0 } &&
                    ((topologyMask & 0b1) > 0 || (topologyMask & 0b10) > 0 || (topologyMask & 0b100) > 0 ||
                     (topologyMask & 0b1000) > 0 || (topologyMask & 0b10000) > 0)
                   )
                {
                    _mainForm.FileLoadLogAdd(TopologyStatusLabel.Text = @"Finding Polylines in MapView ...");
                    Application.DoEvents();
                    routesList = (from route in routes where (from position in route.Points let lat = position.Lat let lng = position.Lng where lat >= south && lat <= north && lng >= west && lng <= east select lat).Any() select new Polyline(route.Points)).ToList();
                    if (routesList.Count == 0)
                    {
                        _mainForm.FileLoadLogAdd(TopologyStatusLabel.Text = @"No polylines found in MapView.");
                        Application.DoEvents();
                    }
                    else
                    {
                        if ((topologyMask & 0b10000) > 0 && routesList.Count > 100)
                            tip = true;
                    }
                }

                var polygons = features.Polygons;
                if (polygons is { Count: > 0 } &&
                    ((topologyMask & 0b1) > 0 || (topologyMask & 0b10) > 0 || (topologyMask & 0b100) > 0 ||
                     (topologyMask & 0b1000) > 0 || (topologyMask & 0b10000) > 0))
                {
                    _mainForm.FileLoadLogAdd(TopologyStatusLabel.Text = @"Finding Polygons in MapView ...");
                    Application.DoEvents();
                    polygonsList = (from polygon in features.Polygons where (from position in polygon.Points let lat = position.Lat let lng = position.Lng where lat >= south && lat <= north && lng >= west && lng <= east select lat).Any() select new Polyline(polygon.Points)).ToList();
                    if (polygonsList.Count == 0)
                    {
                        _mainForm.FileLoadLogAdd(TopologyStatusLabel.Text = @"No polygons found in MapView.");
                        Application.DoEvents();
                    }
                    else
                    {
                        if ((topologyMask & 0b10000) > 0 && polygonsList.Count > 100)
                            tip = true;
                    }
                }

                if (tip)
                    switch (
                        MessageBox.Show(
                            @"Intersection checking may be very time-consuming. Continue?",
                            @"Tip",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question
                        )
                    )
                    {
                        case DialogResult.Cancel:
                        {
                            return;
                        }
                        case DialogResult.No:
                        {
                            CheckBoxIntersection.Checked = false;
                            topologyMask ^= 0b10000;
                            break;
                        }
                    }

                TopologyRun.Image = Resources.stop;
                toolTip1.SetToolTip(TopologyRun, "Cancel");
                TopologyProgressBar.Value = 0;
                TopologyProgressBar.Visible = true;
                Application.DoEvents();

                _backgroundWorker.RunWorkerAsync(
                    (
                        topologyMask,
                        markersList,
                        routesList,
                        polygonsList
                    )
                );
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is not BackgroundWorker worker || e.Argument == null)
                return;
            var argument =
            (
                (
                int topologyMask,
                List<PointLatLng> markersList,
                List<Polyline> routesList,
                List<Polyline> polygonsList
                )
            )e.Argument;

            var topologyMask = argument.topologyMask;
            var markersList = argument.markersList;
            var routesList = argument.routesList;
            var polygonsList = argument.polygonsList;

            if (markersList is { Count: > 0 })
            {
                var count = markersList.Count;
                var resultNodes = new Dictionary<PointLatLng, int>();
                for (var index = 0; index < count; index++)
                {
                    var point = markersList[index];
                    if (resultNodes.TryGetValue(point, out var overlayCount))
                    {
                        resultNodes[point] = overlayCount + 1;
                        _overlayCount++;
                        ShowNode((point, 0b1000));
                    }
                    else
                        resultNodes.Add(point, 1);
                    worker.ReportProgress(
                        percentProgress: 100 * (index + 1) / count,
                        userState: $"Point Overlap Checking ({index + 1} / {count})"
                    );
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                worker.ReportProgress(
                    percentProgress: -1,
                    userState: $"Point Overlap Checked. ({count} / {count})"
                );
            }

            if (routesList is { Count: > 0 })
            {
                CheckNodes(
                    worker,
                    e,
                    "Polyline",
                    polylines: routesList,
                    topologyMask: topologyMask
                );
            }

            if (polygonsList is { Count: > 0 })
            {
                CheckNodes(
                    worker,
                    e,
                    "Polygon",
                    polylines: polygonsList,
                    topologyMask: topologyMask
                );
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var message = e.UserState!.ToString();
            var progressPercentage = e.ProgressPercentage;
            if (progressPercentage is >= 0 and <= 100)
                TopologyProgressBar.Value = e.ProgressPercentage;
            else
                _mainForm.FileLoadLogAdd(message);
            TopologyStatusLabel.Text = message;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(
                    text: $@"Error:{e.Error}",
                    caption: @"OGC.NET",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Error
                );
                TopologyStatusLabel.Text = @"Failed.";
            }
            else
            {
                TopologyStatusLabel.Text = e.Cancelled ? @"Topology check cancelled." : @"Topology check completed.";
                LabelDangle.Text = CheckBoxDangle.Checked ? $"{_dangleCount}" : "";
                LabelPseudo.Text = CheckBoxPseudo.Checked ? $"{_pseudoCount}" : "";
                LabelCoincide.Text = CheckBoxCoincide.Checked ? $"{_coincideCount}" : "";
                LabelOverlay.Text = CheckBoxOverlay.Checked ? $"{_overlayCount}" : "";
                LabelIntersection.Text = CheckBoxIntersection.Checked ? $"{_intersectionCount}" : "";
            }
            TopologyRun.Image = Resources.run;
            toolTip1.SetToolTip(TopologyRun, "Start");
            TopologyProgressBar.Value = 0;
            TopologyProgressBar.Visible = false;
        }

        private void TopologyCheckerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (Features.Markers)
            {
                Features.Markers.Clear();
            }
            _backgroundWorker.CancelAsync();
        }

        private void TopologyCheck_Click(object sender, EventArgs e)
        {
            var theCheckBox = (CheckBox)sender;
            RegEdit.Setkey(keyname: theCheckBox.Name, defaultvalue: theCheckBox.Checked ? "1" : "0");
        }

        // 线或面要素拓扑检查：
        // 悬挂节点（如果某线段的收尾点仅被一条线段连接，则说明该点是一个悬挂节点，也包括孤点）
        // 伪节点（如果某线段的首点仅被两条线段连接，并且该点不属于其他线段的尾点，则说明该点是一个伪节点）
        // 自相交和互相交
        // 毛刺重叠点
        private void CheckNodes(
            BackgroundWorker worker,
            CancelEventArgs e,
            string geometryType,
            IReadOnlyList<Polyline> polylines,
            int topologyMask = 0
        )
        {
            var dangle = (topologyMask & 0b1) > 0;
            var pseudo = (topologyMask & 0b10) > 0;
            var coincide = (topologyMask & 0b100) > 0;
            var intersection = (topologyMask & 0b10000) > 0;
            var count = polylines.Count;
            if ((dangle || pseudo || coincide || intersection) && count > 0)
            {
                worker.ReportProgress(
                    percentProgress: -1,
                    userState: $"{geometryType} Topology Checking ..."
                );
                if (dangle || pseudo || coincide)
                {
                    var nodeDegreeDict = new Dictionary<PointLatLng, int>();
                    for (var index = 0; index < count; index++)
                    {
                        worker.ReportProgress(
                            percentProgress: 100 * (index + 1) / count,
                            userState: $"{geometryType} Node Checking ({index + 1} / {count})"
                        );
                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        var line = polylines[index].Points;
                        if (line.Count == 0)
                            continue;
                        var startPoint = line[0];
                        if (line.Count == 1)
                        {
                            nodeDegreeDict.TryGetValue(startPoint, out _);
                            nodeDegreeDict[startPoint] = -1;
                        }
                        else
                        {
                            var endPoint = line[^1];
                            if (!Equals(startPoint, endPoint))
                            {
                                nodeDegreeDict.TryGetValue(startPoint, out var startCount);
                                nodeDegreeDict.TryGetValue(endPoint, out var endCount);
                                nodeDegreeDict[startPoint] = startCount + (startCount < 0 ? 0 : 1);
                                nodeDegreeDict[endPoint] = endCount + (endCount < 0 ? 0 : 1);
                            }
                            else
                            {
                                if (line.Count == 2)
                                {
                                    nodeDegreeDict.TryGetValue(startPoint, out _);
                                    nodeDegreeDict[startPoint] = -2;
                                }
                                else
                                {
                                    nodeDegreeDict.TryGetValue(startPoint, out var overlapCount);
                                    nodeDegreeDict[startPoint] = overlapCount + (overlapCount < 0 ? 0 : 3);
                                }
                            }
                        }
                    }
                    foreach (var node in nodeDegreeDict)
                    {
                        switch (node.Value)
                        {
                            case -2:
                                if (coincide)
                                {
                                    _coincideCount++;
                                    ShowNode((node.Key, 0b100));
                                }
                                break;
                            case -1:
                            case 1:
                                if (dangle)
                                {
                                    _dangleCount++;
                                    ShowNode((node.Key, 0b1));
                                }
                                break;
                            case 2:
                                if (pseudo)
                                {
                                    _pseudoCount++;
                                    ShowNode((node.Key, 0b10));
                                }
                                break;
                                //default:
                                //    {
                                //        break;
                                //    }
                        }
                    }
                }
                if (intersection || coincide || dangle)
                    for (var i = 0; i < count; i++)
                    {
                        var outerLine = polylines[i];
                        var outerVertices = outerLine.Points;
                        var outerVerticesCount = outerVertices.Count;
                        if (outerVerticesCount > 0)
                        {
                            if (outerVerticesCount == 1)
                            {
                                if (dangle)
                                {
                                    _dangleCount++;
                                    ShowNode((outerVertices[0], 0b1));
                                }
                                continue;
                            }
                            var outerBox = outerLine.Boundary();
                            if (intersection || coincide)
                            {
                                // j 从 i 开始，意味着可检查自相交
                                for (var j = i; j < count; j++)
                                {
                                    worker.ReportProgress(
                                        percentProgress: 100 * (i + 1) / count,
                                        userState:
                                        $"{geometryType} Intersection Checking ({j + 1} - {i + 1} / {count})"
                                    );
                                    if (worker.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }
                                    var innerLine = polylines[j];
                                    var innerVertices = innerLine.Points;
                                    var innerVerticesCount = innerVertices.Count;
                                    if (innerVerticesCount > 1)
                                    {
                                        var innerBox = innerLine.Boundary();
                                        if (!(outerBox.west > innerBox.east ||
                                              outerBox.east < innerBox.west ||
                                              outerBox.south > innerBox.north ||
                                              outerBox.north < innerBox.south) ||
                                            !(innerBox.west > outerBox.east ||
                                              innerBox.east < outerBox.west ||
                                              innerBox.south > outerBox.north ||
                                              innerBox.north < outerBox.south))
                                        {
                                            Parallel.For(0, outerVerticesCount - 1, m =>
                                            {
                                                var pointM = outerVertices[m];
                                                if (coincide)
                                                {
                                                    var nodes = outerVertices.AsParallel()
                                                        .Where((pointK, index) => index < m && Equals(pointK, pointM))
                                                        .Select(pointK => (pointK, 0b100)).ToList();
                                                    if (nodes.Any())
                                                        foreach (var node in nodes)
                                                            ShowNode(node);
                                                }
                                                if (!intersection)
                                                    return;
                                                var outerSegment = new Segment(pointM, outerVertices[m + 1]);
                                                var outerSegmentBox = outerSegment.Boundary();
                                                if (!(outerSegmentBox.west > innerBox.east ||
                                                      outerSegmentBox.east < innerBox.west ||
                                                      outerSegmentBox.south > innerBox.north ||
                                                      outerSegmentBox.north < innerBox.south) ||
                                                    !(innerBox.west > outerSegmentBox.east ||
                                                      innerBox.east < outerSegmentBox.west ||
                                                      innerBox.south > outerSegmentBox.north ||
                                                      innerBox.north < outerSegmentBox.south))
                                                {
                                                    Parallel.For(0, innerVerticesCount - 1, n =>
                                                    {
                                                        var innerSegment = new Segment(innerVertices[n], innerVertices[n + 1]);
                                                        var innerSegmentBox = innerSegment.Boundary();
                                                        if (!(outerSegmentBox.west > innerSegmentBox.east ||
                                                              outerSegmentBox.east < innerSegmentBox.west ||
                                                              outerSegmentBox.south > innerSegmentBox.north ||
                                                              outerSegmentBox.north < innerSegmentBox.south) ||
                                                            !(innerSegmentBox.west > outerSegmentBox.east ||
                                                              innerSegmentBox.east < outerSegmentBox.west ||
                                                              innerSegmentBox.south > outerSegmentBox.north ||
                                                              innerSegmentBox.north < outerSegmentBox.south))
                                                        {
                                                            // 计算outerSegment与innerSegments的交点
                                                            var intersectionPoint = outerSegment.GetIntersection(innerSegment);
                                                            if (intersectionPoint != null)
                                                            {
                                                                // 由于_intersectionCount是共享变量，需要使用Interlocked.Increment方法来保证线程安全
                                                                Interlocked.Increment(ref _intersectionCount);
                                                                ShowNode((intersectionPoint.Value, 0b10000));
                                                            }
                                                        }
                                                    });
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                worker.ReportProgress(
                    percentProgress: -1,
                    userState: $"{geometryType} Topology Checked. ({count} / {count})"
                );
            }
        }

        private void ShowNode((PointLatLng point, int code) node)
        {
            var code = node.code;
            var point = node.point;
            _mainForm.MapBox.BeginInvoke(
                method: () =>
                {
                    switch (code)
                    {
                        case 0b1: //悬挂节点（如果某线段的收尾点仅被一条线段连接，则说明该点是一个悬挂节点，也包括孤点和零长度线段的首点）
                            {
                                lock (Features.Markers)
                                    Features.Markers.Add(
                                        item: new GMapMarkerRect(
                                            position: point,
                                            strokeNormal: new Pen(
                                                color: Color.FromArgb(alpha: 255, red: 255, green: 0,
                                                    blue: 0),
                                                width: 2))
                                        {
                                            //Tag = (
                                            //    JObject.Parse
                                            //    (
                                            //        $"{{\"Type\":\"dangle\",\"Code\":{code},\"Position\":{{\"Longitude\":{point.X},\"Latitude\":{point.Y}}}}}"
                                            //    ),
                                            //    (JObject)null
                                            //),
                                            IsHitTestVisible = false
                                        }
                                    );
                                break;
                            }
                        case 0b10: //伪节点（如果某线段的首点仅被两条线段连接，并且该点不属于其他线段的尾点，则说明该点是一个伪节点）
                            {
                                lock (Features.Markers)
                                    Features.Markers.Add(
                                        item: new GMapMarkerRect(
                                            position: point,
                                            strokeNormal: new Pen(
                                                color: Color.FromArgb(alpha: 255, red: 0, green: 255, blue: 0),
                                                width: 2))
                                        {
                                            //Tag = (
                                            //    JObject.Parse
                                            //    (
                                            //        $"{{\"Type\":\"pseudo\",\"Code\":{code},\"Position\":{{\"Longitude\":{point.X},\"Latitude\":{point.Y}}}}}"
                                            //    ),
                                            //    (JObject)null
                                            //),
                                            IsHitTestVisible = false
                                        }
                                    );
                                break;
                            }
                        case 0b100: //毛刺线段（或外挂微小多边形）重叠点
                            {
                                lock (Features.Markers)
                                {
                                    Features.Markers.Add(
                                        item: new GMapMarkerRect(
                                            position: point,
                                            strokeNormal: new Pen(
                                                color: Color.FromArgb(alpha: 255, red: 255, green: 255, blue: 0),
                                                width: 2))
                                        {
                                            //Tag = (
                                            //    JObject.Parse
                                            //    (
                                            //        $"{{\"Type\":\"coincide\",\"Code\":{code},\"Position\":{{\"Longitude\":{point.X},\"Latitude\":{point.Y}}}}}"
                                            //    ),
                                            //    (JObject)null
                                            //),
                                            IsHitTestVisible = false
                                        }
                                    );
                                }

                                break;
                            }
                        case 0b1000: //点要素重叠点
                            {
                                lock (Features.Markers)
                                {
                                    Features.Markers.Add(
                                        item: new GMapMarkerRect(
                                            position: point,
                                            strokeNormal: new Pen(
                                                color: Color.FromArgb(alpha: 255, red: 255, green: 127, blue: 127),
                                                width: 2))
                                        {
                                            //Tag = (
                                            //    JObject.Parse
                                            //    (
                                            //        $"{{\"Type\":\"overlap\",\"Code\":{0b1000},\"Count\":{pointResult.count},\"Position\":{{\"Longitude\":{pointResult.point.X},\"Latitude\":{pointResult.point.Y}}}}}"
                                            //    ),
                                            //    (JObject)null
                                            //),
                                            IsHitTestVisible = false
                                        }
                                    );
                                }

                                break;
                            }
                        case 0b10000: //互相交点（暂不识别自相交）
                            {
                                lock (Features.Markers)
                                {
                                    Features.Markers.Add(
                                        item: new GMapMarkerRect(
                                            position: point,
                                            strokeNormal: new Pen(
                                                color: Color.FromArgb(alpha: 255, red: 0, green: 255, blue: 255),
                                                width: 2))
                                        {
                                            //Tag = (
                                            //    JObject.Parse
                                            //    (
                                            //        $"{{\"Type\":\"intersection\",\"Code\":{code},\"Position\":{{\"Longitude\":{point.X},\"Latitude\":{point.Y}}}}}"
                                            //    ),
                                            //    (JObject)null
                                            //),
                                            IsHitTestVisible = false
                                        }
                                    );
                                }

                                break;
                            }
                    }
                }
            );
        }
    }
}
