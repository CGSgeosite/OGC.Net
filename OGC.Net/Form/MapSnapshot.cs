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
using System.Diagnostics;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GMap.NET.MapProviders;
using Geosite.GeositeServer.Vector;
using System.Text.RegularExpressions;
using Geosite.GeositeServer.Raster;

namespace Geosite
{
    public partial class MapSnapshot : Form
    {
        private readonly MainForm _mainForm;
        private readonly BackgroundWorker _backgroundWorker = new();
        private readonly List<GPoint> _tileArea = new();
        private RectLatLng _area;

        public MapSnapshot(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
        }

        private void MapSnapshot_Load(object sender, EventArgs e)
        {
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            SnapshotStatusLabel.Text = _mainForm.GetCopyright;
            SnapshotRefresh_Click();
        }

        private void MapSnapshot_Enter(object sender, EventArgs e)
        {
            Activate();
        }

        private void SnapshotRefresh_Click(object sender = null, EventArgs e = null)
        {
            var mapBox = _mainForm.MapBox;
            //var minZoom = mapBox.MinZoom;
            //var maxZoom = mapBox.MaxZoom;
            var zoom = (int)mapBox.Zoom;
            _area = mapBox.ViewArea;
            SnapshotTop.Text = Ellipsoid.Degree2Dms(Degree: $"{_area.Top}", Digit: "1");
            SnapshotLeft.Text = Ellipsoid.Degree2Dms(Degree: $"{_area.Left}", Digit: "1");
            SnapshotBottom.Text = Ellipsoid.Degree2Dms(Degree: $"{_area.Bottom}", Digit: "1");
            SnapshotRight.Text = Ellipsoid.Degree2Dms(Degree: $"{_area.Right}", Digit: "1");
            var copyright = mapBox.MapProvider.Copyright;
            mapBox.MapProvider.Copyright = null;
            mapBox.Refresh();
            SnapshotPicture.BackgroundImage = mapBox.ToImage();
            mapBox.MapProvider.Copyright = copyright;
            mapBox.Refresh();
            SnapshotZoom.Text = $@"{zoom:00}";
            SnapshotZoom.DropDownItems.Clear();
            for (var i = zoom; i <= mapBox.MaxZoom; i++)
            {
                var item = new ToolStripMenuItem
                {
                    Text = $@"{i}",
                    Checked = i == zoom,
                    CheckOnClick = true
                };
                item.Click += SnapshotZoom_Click;
                SnapshotZoom.DropDownItems.Add(value: item);
            }
        }

        private void SnapshotSave_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = @"Image(*.jpg)|*.jpg"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                SnapshotFileTextBox.Text = saveFileDialog.FileName;
            SnapshotRun.Enabled = !string.IsNullOrWhiteSpace(value: SnapshotFileTextBox.Text);
        }

        private void SnapshotZoom_Click(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var zoom = int.Parse(s: ((ToolStripMenuItem)sender).Text);
                foreach (var item in SnapshotZoom.DropDownItems)
                {
                    if (item.GetType().Name == "ToolStripMenuItem")
                    {
                        var theItem = (ToolStripMenuItem)item;
                        theItem.Checked = int.Parse(s: theItem.Text) == zoom;
                    }
                }
                SnapshotZoom.Text = $@"{zoom:00}";
            }
        }

        private void SnapshotRun_Click(object sender, EventArgs e)
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
                SnapshotRun.Image = Properties.Resources.run;
                SnapshotRun.ToolTipText = @"Start";
                SnapshotProgressBar.Value = 0;
                SnapshotProgressBar.Visible = false;
                SnapshotTools.Enabled =
                    SnapshotAreaPanel.Enabled =
                        SnapshotSave.Enabled = SnapshotFileTextBox.Enabled = SnapshotZoom.Enabled = true;
            }
            else
            {
                var snapshotTopText = Ellipsoid.Degree2Dms(DMS: SnapshotTop.Text);
                if (string.IsNullOrWhiteSpace(value: snapshotTopText))
                {
                    MessageBox.Show(text: @"Unable to parse top latitude", caption: @"Warning");
                    return;
                }
                var snapshotTop = double.Parse(s: snapshotTopText);
                if (snapshotTop is < -90 or > 90)
                {
                    MessageBox.Show(text: @"Top latitude should be between [-90, +90]", caption: @"Warning");
                    return;
                }
                var snapshotLeftText = Ellipsoid.Degree2Dms(DMS: SnapshotLeft.Text);
                if (string.IsNullOrWhiteSpace(value: snapshotLeftText))
                {
                    MessageBox.Show(text: @"Unable to parse left longitude", caption: @"Warning");
                    return;
                }
                var snapshotLeft = double.Parse(s: snapshotLeftText);
                if (snapshotLeft is < -180 or > 180)
                {
                    MessageBox.Show(text: @"Left longitude should be between [-180, +180]", caption: @"Warning");
                    return;
                }
                var snapshotBottomText = Ellipsoid.Degree2Dms(DMS: SnapshotBottom.Text);
                if (string.IsNullOrWhiteSpace(value: snapshotBottomText))
                {
                    MessageBox.Show(text: @"Unable to parse bottom latitude", caption: @"Warning");
                    return;
                }
                var snapshotBottom = double.Parse(s: snapshotBottomText);
                if (snapshotBottom is < -90 or > 90)
                {
                    MessageBox.Show(text: @"Bottom latitude should be between [-90, +90]", caption: @"Warning");
                    return;
                }
                var snapshotRightText = Ellipsoid.Degree2Dms(DMS: SnapshotRight.Text);
                if (string.IsNullOrWhiteSpace(value: snapshotRightText))
                {
                    MessageBox.Show(text: @"Unable to parse right longitude", caption: @"Warning");
                    return;
                }
                var snapshotRight = double.Parse(s: snapshotRightText);
                if (snapshotRight is < -180 or > 180)
                {
                    MessageBox.Show(text: @"Right longitude should be between [-180, +180]", caption: @"Warning");
                    return;
                }
                if (snapshotTop <= snapshotBottom)
                {
                    MessageBox.Show(text: @"Top should be greater than Bottom", caption: @"Warning");
                    return;
                }
                if (snapshotRight <= snapshotLeft)
                {
                    MessageBox.Show(text: @"Right should be greater than Left", caption: @"Warning");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(value: SnapshotFileTextBox.Text))
                {
                    var mapBox = _mainForm.MapBox;
                    _area = RectLatLng.FromLTRB(leftLng: snapshotLeft, topLat: snapshotTop, rightLng: snapshotRight, bottomLat: snapshotBottom);
                    mapBox.SetZoomToFitRect(rect: _area);
                    SnapshotPicture.BackgroundImage = mapBox.ToImage();
                    SnapshotRun.Image = Properties.Resources.stop;
                    SnapshotRun.ToolTipText = @"Cancel";
                    SnapshotProgressBar.Value = 0;
                    SnapshotProgressBar.Visible = true;
                    SnapshotTools.Enabled =
                        SnapshotAreaPanel.Enabled =
                            SnapshotSave.Enabled = SnapshotFileTextBox.Enabled = SnapshotZoom.Enabled = false;
                    Application.DoEvents();
                    lock (_tileArea)
                    {
                        _tileArea.Clear();
                        _tileArea.AddRange(
                            collection: mapBox.MapProvider.Projection.GetAreaTileList(
                                rect: _area,
                                zoom: int.Parse(s: SnapshotZoom.Text),
                                padding: 0
                            )
                        );
                        _tileArea.TrimExcess();
                    }
                    mapBox.HoldInvalidation = true;
                    _backgroundWorker.RunWorkerAsync(
                        argument: new MapViewInfo(
                            area: _area,
                            zoom: int.Parse(s: SnapshotZoom.Text),
                            type: mapBox.MapProvider,
                            file: SnapshotFileTextBox.Text,
                            epsg4326: (string)EPSG4326Switch.Tag == "1"
                        )
                    );
                }
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument == null)
                return;
            var mapViewInfo = (MapViewInfo)e.Argument;
            if (mapViewInfo.Area.IsEmpty || string.IsNullOrWhiteSpace(value: mapViewInfo.File))
                return;
            var epsg3857 = mapViewInfo.File; //暂支持【jpg】格式【xxx.EPSG3857.jpg】
            var directoryName = Path.GetDirectoryName(epsg3857);
            var fileName = Regex.Split(Path.GetFileNameWithoutExtension(epsg3857), @"[\.]")[0];
            var fileExtensionName = Path.GetExtension(epsg3857);
            var epsg3857File = Path.Combine(directoryName ?? "", fileName + ".EPSG3857" + fileExtensionName);
            e.Result = epsg3857;
            var topLeft =
                mapViewInfo.Type.Projection.FromLatLngToPixel(
                    p: mapViewInfo.Area.LocationTopLeft,
                    zoom: mapViewInfo.Zoom
                );
            var rightBottom = mapViewInfo.Type.Projection.FromLatLngToPixel(
                lat: mapViewInfo.Area.Bottom,
                lng: mapViewInfo.Area.Right,
                zoom: mapViewInfo.Zoom
            );
            var pxDelta = new GPoint(x: rightBottom.X - topLeft.X, y: rightBottom.Y - topLeft.Y);
            var maxOfTiles = mapViewInfo.Type.Projection.GetTileMatrixMaxXY(zoom: mapViewInfo.Zoom);
            _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Declare Memory ...");
            try
            {
                using var imageDestination = new Bitmap(width: (int)pxDelta.X, height: (int)pxDelta.Y);
                using (var gfx = Graphics.FromImage(image: imageDestination))
                {
                    gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gfx.SmoothingMode = SmoothingMode.HighQuality;
                    var i = 0;

                    // 渲染瓦片层
                    lock (_tileArea)
                    {
                        foreach (var p in _tileArea)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            _backgroundWorker.ReportProgress(
                                percentProgress: (int)((double)++i / _tileArea.Count * 100),
                                userState: $@"{_tileArea.IndexOf(item: p)} / {_tileArea.Count}");
                            var tileOverlays = mapViewInfo.Type.Overlays;
                            if (GMapProvider.OverlayTiles.Count > 0)
                                tileOverlays =
                                    tileOverlays == null
                                        ? GMapProvider.OverlayTiles.ToArray()
                                        : tileOverlays.Concat(GMapProvider.OverlayTiles).ToArray();
                            if (tileOverlays != null)
                            {
                                foreach (var tp in tileOverlays)
                                {
                                    var tile =
                                        tp.InvertedAxisY //TMS
                                            ? GMaps.Instance.GetImageFrom(provider: tp,
                                                pos: new GPoint(x: p.X, y: maxOfTiles.Height - p.Y),
                                                zoom: mapViewInfo.Zoom, result: out _) as GMapImage
                                            : GMaps.Instance.GetImageFrom(provider: tp, pos: p, zoom: mapViewInfo.Zoom,
                                                result: out _) as GMapImage;
                                    if (tile != null)
                                        using (tile)
                                        {
                                            var img = tile.Img;
                                            var x = p.X * mapViewInfo.Type.Projection.TileSize.Width - topLeft.X;
                                            var y = p.Y * mapViewInfo.Type.Projection.TileSize.Width - topLeft.Y;
                                            var alpha = tp.Alpha ?? 1.0f;
                                            if (1f - alpha >= 1e-6)
                                            {
                                                using var newBitmap = new Bitmap(img.Width, img.Height);
                                                using var g = Graphics.FromImage(newBitmap);
                                                using (var attributes = new ImageAttributes())
                                                {
                                                    attributes.SetColorMatrix(
                                                        newColorMatrix: new ColorMatrix { Matrix33 = alpha },
                                                        mode: ColorMatrixFlag.Default, type: ColorAdjustType.Bitmap);
                                                    g.DrawImage(
                                                        img,
                                                        new Rectangle(0, 0, img.Width, img.Height),
                                                        0,
                                                        0,
                                                        img.Width,
                                                        img.Height,
                                                        GraphicsUnit.Pixel,
                                                        attributes);
                                                }

                                                gfx.DrawImage(
                                                    newBitmap,
                                                    x,
                                                    y,
                                                    mapViewInfo.Type.Projection.TileSize.Width,
                                                    mapViewInfo.Type.Projection.TileSize.Height
                                                );
                                            }
                                            else
                                            {
                                                gfx.DrawImage(
                                                    img,
                                                    x,
                                                    y,
                                                    mapViewInfo.Type.Projection.TileSize.Width,
                                                    mapViewInfo.Type.Projection.TileSize.Height
                                                );
                                            }
                                        }
                                }
                            }
                        }
                    }

                    // 矢量要素
                    // 渲染点要素（按钉、点圆、贴图）
                    lock (MapView.Features.Markers)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Markers ...");
                        foreach (var marker in MapView.Features.Markers)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (marker.IsVisible)
                            {
                                var markerPosition = marker.Position;
                                var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: markerPosition.Lat,
                                    lng: markerPosition.Lng, zoom: mapViewInfo.Zoom);
                                px.Offset(dx: 0, dy: 0);
                                px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                px.Offset(dx: marker.Offset.X, dy: marker.Offset.Y);
                                gfx.ResetTransform();
                                gfx.TranslateTransform(dx: -marker.LocalPosition.X, dy: -marker.LocalPosition.Y);
                                gfx.TranslateTransform(dx: (int)px.X, dy: (int)px.Y);
                                marker.OnRender(g: gfx);
                            }
                        }

                        // 渲染点要素的提示文本
                        foreach (var marker in MapView.Features.Markers)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (marker.IsVisible && marker.ToolTip != null && marker.IsVisible &&
                                !string.IsNullOrEmpty(value: marker.ToolTipText))
                            {
                                var markerPosition = marker.Position;
                                var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: markerPosition.Lat,
                                    lng: markerPosition.Lng, zoom: mapViewInfo.Zoom);
                                px.Offset(dx: 0, dy: 0);
                                px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                px.Offset(dx: marker.Offset.X, dy: marker.Offset.Y);
                                gfx.ResetTransform();
                                gfx.TranslateTransform(dx: -marker.LocalPosition.X, dy: -marker.LocalPosition.Y);
                                gfx.TranslateTransform(dx: (int)px.X, dy: (int)px.Y);
                                marker.ToolTip.OnRender(g: gfx);
                            }
                        }

                        gfx.ResetTransform();
                    }

                    // 渲染线要素
                    lock (MapView.Features.Routes)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Routes ...");
                        foreach (var route in MapView.Features.Routes)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (route.IsVisible)
                            {
                                using var routeRender = new GraphicsPath();
                                for (var j = 0; j < route.Points.Count; j++)
                                {
                                    var routePoint = route.Points[index: j];
                                    var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: routePoint.Lat,
                                        lng: routePoint.Lng, zoom: mapViewInfo.Zoom);
                                    px.Offset(dx: 0, dy: 0);
                                    px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                    var p2 = px;
                                    if (j == 0)
                                        routeRender.AddLine(x1: p2.X, y1: p2.Y, x2: p2.X, y2: p2.Y);
                                    else
                                    {
                                        var p = routeRender.GetLastPoint();
                                        routeRender.AddLine(x1: p.X, y1: p.Y, x2: p2.X, y2: p2.Y);
                                    }
                                }
                                if (routeRender.PointCount > 0) 
                                    gfx.DrawPath(pen: route.Stroke, path: routeRender);
                            }
                        }
                    }

                    // 渲染面要素
                    lock (MapView.Features.Polygons)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Polygons ...");
                        foreach (var polygon in MapView.Features.Polygons)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (polygon.IsVisible)
                            {
                                using var polygonRender = new GraphicsPath();
                                for (var j = 0; j < polygon.Points.Count; j++)
                                {
                                    var polygonPoint = polygon.Points[index: j];
                                    var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: polygonPoint.Lat,
                                        lng: polygonPoint.Lng, zoom: mapViewInfo.Zoom);
                                    px.Offset(dx: 0, dy: 0);
                                    px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                    var p2 = px;
                                    if (j == 0)
                                        polygonRender.AddLine(x1: p2.X, y1: p2.Y, x2: p2.X, y2: p2.Y);
                                    else
                                    {
                                        var p = polygonRender.GetLastPoint();
                                        polygonRender.AddLine(x1: p.X, y1: p.Y, x2: p2.X, y2: p2.Y);
                                    }
                                }
                                if (polygonRender.PointCount > 0)
                                {
                                    polygonRender.CloseFigure();
                                    gfx.FillPath(brush: polygon.Fill, path: polygonRender);
                                    gfx.DrawPath(pen: polygon.Stroke, path: polygonRender);
                                }
                            }
                        }
                    }

                    // 图幅分幅网格要素
                    // 渲染点要素（按钉、点圆、贴图）
                    lock (MapGrid.Features.Markers)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Markers ...");
                        foreach (var marker in MapGrid.Features.Markers)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (marker.IsVisible)
                            {
                                var markerPosition = marker.Position;
                                var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: markerPosition.Lat,
                                    lng: markerPosition.Lng, zoom: mapViewInfo.Zoom);
                                px.Offset(dx: 0, dy: 0);
                                px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                px.Offset(dx: marker.Offset.X, dy: marker.Offset.Y);
                                gfx.ResetTransform();
                                gfx.TranslateTransform(dx: -marker.LocalPosition.X, dy: -marker.LocalPosition.Y);
                                gfx.TranslateTransform(dx: (int)px.X, dy: (int)px.Y);
                                marker.OnRender(g: gfx);
                            }
                        }

                        // 渲染点要素的提示文本
                        foreach (var marker in MapGrid.Features.Markers)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (marker.IsVisible && marker.ToolTip != null && marker.IsVisible && !string.IsNullOrEmpty(value: marker.ToolTipText))
                            {
                                var markerPosition = marker.Position;
                                var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: markerPosition.Lat, lng: markerPosition.Lng, zoom: mapViewInfo.Zoom);
                                px.Offset(dx: 0, dy: 0);
                                px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                px.Offset(dx: marker.Offset.X, dy: marker.Offset.Y);
                                gfx.ResetTransform();
                                gfx.TranslateTransform(dx: -marker.LocalPosition.X, dy: -marker.LocalPosition.Y);
                                gfx.TranslateTransform(dx: (int)px.X, dy: (int)px.Y);
                                marker.ToolTip.OnRender(g: gfx);
                            }
                        }
                        gfx.ResetTransform();
                    }

                    // 渲染线要素
                    lock (MapGrid.Features.Routes)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Routes ...");
                        foreach (var route in MapGrid.Features.Routes)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (route.IsVisible)
                            {
                                using var routeRender = new GraphicsPath();
                                for (var j = 0; j < route.Points.Count; j++)
                                {
                                    var routePoint = route.Points[index: j];
                                    var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: routePoint.Lat, lng: routePoint.Lng, zoom: mapViewInfo.Zoom);
                                    px.Offset(dx: 0, dy: 0);
                                    px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                    var p2 = px;
                                    if (j == 0)
                                        routeRender.AddLine(x1: p2.X, y1: p2.Y, x2: p2.X, y2: p2.Y);
                                    else
                                    {
                                        var p = routeRender.GetLastPoint();
                                        routeRender.AddLine(x1: p.X, y1: p.Y, x2: p2.X, y2: p2.Y);
                                    }
                                }
                                if (routeRender.PointCount > 0) 
                                    gfx.DrawPath(pen: route.Stroke, path: routeRender);
                            }
                        }
                    }

                    // 渲染面要素
                    lock (MapGrid.Features.Polygons)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Polygons ...");
                        foreach (var polygon in MapGrid.Features.Polygons)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (polygon.IsVisible)
                            {
                                using var polygonRender = new GraphicsPath();
                                for (var j = 0; j < polygon.Points.Count; j++)
                                {
                                    var polygonPoint = polygon.Points[index: j];
                                    var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: polygonPoint.Lat,
                                        lng: polygonPoint.Lng, zoom: mapViewInfo.Zoom);
                                    px.Offset(dx: 0, dy: 0);
                                    px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                    var p2 = px;
                                    if (j == 0)
                                        polygonRender.AddLine(x1: p2.X, y1: p2.Y, x2: p2.X, y2: p2.Y);
                                    else
                                    {
                                        var p = polygonRender.GetLastPoint();
                                        polygonRender.AddLine(x1: p.X, y1: p.Y, x2: p2.X, y2: p2.Y);
                                    }
                                }
                                if (polygonRender.PointCount > 0)
                                {
                                    polygonRender.CloseFigure();
                                    gfx.FillPath(brush: polygon.Fill, path: polygonRender);
                                    gfx.DrawPath(pen: polygon.Stroke, path: polygonRender);
                                }
                            }
                        }
                    }

                    // 拓扑要素
                    // 渲染点要素（点框）
                    lock (TopologyCheckerForm.Features.Markers)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Markers ...");
                        foreach (var marker in TopologyCheckerForm.Features.Markers)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (marker.IsVisible)
                            {
                                var markerPosition = marker.Position;
                                var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: markerPosition.Lat, lng: markerPosition.Lng, zoom: mapViewInfo.Zoom);
                                px.Offset(dx: 0, dy: 0);
                                px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                px.Offset(dx: marker.Offset.X, dy: marker.Offset.Y);
                                gfx.ResetTransform();
                                gfx.TranslateTransform(dx: -marker.LocalPosition.X, dy: -marker.LocalPosition.Y);
                                gfx.TranslateTransform(dx: (int)px.X, dy: (int)px.Y);
                                marker.OnRender(g: gfx);
                            }
                        }

                        // 渲染点要素的提示文本
                        foreach (var marker in TopologyCheckerForm.Features.Markers)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (marker.IsVisible && marker.ToolTip != null && marker.IsVisible && !string.IsNullOrEmpty(value: marker.ToolTipText))
                            {
                                var markerPosition = marker.Position;
                                var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: markerPosition.Lat, lng: markerPosition.Lng, zoom: mapViewInfo.Zoom);
                                px.Offset(dx: 0, dy: 0);
                                px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                px.Offset(dx: marker.Offset.X, dy: marker.Offset.Y);
                                gfx.ResetTransform();
                                gfx.TranslateTransform(dx: -marker.LocalPosition.X, dy: -marker.LocalPosition.Y);
                                gfx.TranslateTransform(dx: (int)px.X, dy: (int)px.Y);
                                marker.ToolTip.OnRender(g: gfx);
                            }
                        }
                        gfx.ResetTransform();
                    }

                    // 渲染线要素
                    lock (TopologyCheckerForm.Features.Routes)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Routes ...");
                        foreach (var route in TopologyCheckerForm.Features.Routes)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (route.IsVisible)
                            {
                                using var routeRender = new GraphicsPath();
                                for (var j = 0; j < route.Points.Count; j++)
                                {
                                    var routePoint = route.Points[index: j];
                                    var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: routePoint.Lat,
                                        lng: routePoint.Lng, zoom: mapViewInfo.Zoom);
                                    px.Offset(dx: 0, dy: 0);
                                    px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                    var p2 = px;
                                    if (j == 0)
                                        routeRender.AddLine(x1: p2.X, y1: p2.Y, x2: p2.X, y2: p2.Y);
                                    else
                                    {
                                        var p = routeRender.GetLastPoint();
                                        routeRender.AddLine(x1: p.X, y1: p.Y, x2: p2.X, y2: p2.Y);
                                    }
                                }
                                if (routeRender.PointCount > 0) 
                                    gfx.DrawPath(pen: route.Stroke, path: routeRender);
                            }
                        }
                    }

                    // 渲染面要素
                    lock (TopologyCheckerForm.Features.Polygons)
                    {
                        _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Rendering Polygons ...");
                        foreach (var polygon in TopologyCheckerForm.Features.Polygons)
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (polygon.IsVisible)
                            {
                                using var polygonRender = new GraphicsPath();
                                for (var j = 0; j < polygon.Points.Count; j++)
                                {
                                    var polygonPoint = polygon.Points[index: j];
                                    var px = mapViewInfo.Type.Projection.FromLatLngToPixel(lat: polygonPoint.Lat, lng: polygonPoint.Lng, zoom: mapViewInfo.Zoom);
                                    px.Offset(dx: 0, dy: 0);
                                    px.Offset(dx: -topLeft.X, dy: -topLeft.Y);
                                    var p2 = px;
                                    if (j == 0)
                                        polygonRender.AddLine(x1: p2.X, y1: p2.Y, x2: p2.X, y2: p2.Y);
                                    else
                                    {
                                        var p = polygonRender.GetLastPoint();
                                        polygonRender.AddLine(x1: p.X, y1: p.Y, x2: p2.X, y2: p2.Y);
                                    }
                                }
                                if (polygonRender.PointCount > 0)
                                {
                                    polygonRender.CloseFigure();
                                    gfx.FillPath(brush: polygon.Fill, path: polygonRender);
                                    gfx.DrawPath(pen: polygon.Stroke, path: polygonRender);
                                }
                            }
                        }
                    }
                }
                _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Save EPSG:3857 file ...");
                imageDestination.Save(filename: epsg3857File, format: ImageFormat.Jpeg);
                (double north, double west, double south, double east) boundary4326 =
                (
                    mapViewInfo.Area.Lat,
                    mapViewInfo.Area.Lng,
                    mapViewInfo.Area.Lat - mapViewInfo.Area.HeightLat,
                    mapViewInfo.Area.Lng + mapViewInfo.Area.WidthLng
                );
                var westNorth = Ellipsoid.WebMercator(boundary4326.west, boundary4326.north).Split(",");
                var eastSouth = Ellipsoid.WebMercator(boundary4326.east, boundary4326.south).Split(",");
                (double north, double west, double south, double east) boundary3857 =
                (
                    double.Parse(westNorth[1]),
                    double.Parse(westNorth[0]),
                    double.Parse(eastSouth[1]),
                    double.Parse(eastSouth[0])
                );
                try
                {
                    using var worldFile = File.CreateText(path: Path.ChangeExtension(epsg3857File, "jpw"));
                    worldFile.WriteLine((boundary3857.east - boundary3857.west) / pxDelta.X);
                    worldFile.WriteLine(value: "0.0");
                    worldFile.WriteLine(value: "0.0");
                    worldFile.WriteLine((boundary3857.south - boundary3857.north) / pxDelta.Y);
                    worldFile.WriteLine(boundary3857.west);
                    worldFile.WriteLine(boundary3857.north);
                    worldFile.Close();
                }
                catch
                {
                    //
                }
                if (mapViewInfo.Epsg4326)
                {
                    _backgroundWorker.ReportProgress(percentProgress: -1, userState: "Save EPSG:4326 file ...");
                    var epsg4326File = Path.Combine(directoryName ?? "", fileName + ".EPSG4326" + fileExtensionName);
                    BitMap.Epsg3857ToEpsg4326(
                        epsg3857File,
                        boundary3857,
                        (pxDelta.X, pxDelta.Y),
                        epsg4326File,
                        boundary4326
                    );
                }
            }
            catch (Exception er)
            {
                _backgroundWorker.ReportProgress(percentProgress: -1, userState: er.Message); //通常因为瓦片太多而导致内存溢出
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressPercentage = e.ProgressPercentage;
            if (progressPercentage >= 0)
                SnapshotProgressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
                SnapshotStatusLabel.Text = $@"{e.UserState}";
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                    {
                        try
                        {
                            Process.Start(fileName: e.Result as string ?? string.Empty);
                        }
                        catch
                        {
                            //
                        }
                    }
                    SnapshotStatusLabel.Text = @"Done.";
                }
                else
                {
                    MessageBox.Show(text: $@"Error:{e.Error}", caption: @"OGC.NET", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    SnapshotStatusLabel.Text = @"Failed.";
                }
            }
            else
                SnapshotStatusLabel.Text = @"Cancelled.";
            SnapshotProgressBar.Value = 0;
            SnapshotTools.Enabled =
                SnapshotAreaPanel.Enabled =
                    SnapshotSave.Enabled = SnapshotFileTextBox.Enabled =
                        SnapshotZoom.Enabled = SnapshotPicture.Enabled = true;
            SnapshotProgressBar.Visible = false;
            SnapshotRun.Image = Properties.Resources.run;
            SnapshotRun.ToolTipText = @"Start";
        }

        private void MapSnapshot_FormClosing(object sender, FormClosingEventArgs e)
        {
            _backgroundWorker.CancelAsync();
        }

        private void EPSG4326Switch_Click(object sender, EventArgs e)
        {
            EPSG4326Switch.Tag = (string)EPSG4326Switch.Tag == "1" ? "0" : "1";
            EPSG4326Switch.Image = (string)EPSG4326Switch.Tag == "1" ? Properties.Resources.checkedbox : Properties.Resources.checkbox;
        }
    }

    public struct MapViewInfo
    {
        public RectLatLng Area;
        public readonly int Zoom;
        public readonly GMapProvider Type;
        public readonly string File;
        public readonly bool Epsg4326;

        public MapViewInfo(RectLatLng area, int zoom, GMapProvider type, string file, bool epsg4326 = true)
        {
            Area = area;
            Zoom = zoom;
            Type = type;
            File = file;
            Epsg4326 = epsg4326;
        }
    }
}
