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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using Geosite.FreeText.CSV;
using Geosite.FreeText.TXT;
using Geosite.Messager;
using Geosite.GeositeServer;
using Geosite.GeositeServer.PostgreSQL;
using Geosite.GeositeServer.Raster;
using Geosite.GeositeServer.Vector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GMap.NET.WindowsForms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.MapProviders.TiandituProviders;
using GMap.NET.MapProviders.ArcGISProviders;
using GMap.NET.Extend;
using Formatting = Newtonsoft.Json.Formatting;
using Geosite.QuickCopyFile;

namespace Geosite
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Software Copyright Information
        /// </summary>
        public readonly string GetCopyright;

        /// <summary>
        /// GeositeServer cluster user information, where name will serve as the forest name
        /// </summary>
        private (
            bool status, //连接状态
            int forest,  //群号
            string name  //用户名称
            ) _clusterUser;

        private DatabaseGrid _databaseGridObject;
        private CatalogTree _catalogTreeObject;

        /// <summary>
        /// Does the metadata dialog no longer pop up?
        /// </summary>
        private bool _noPromptMetaData;

        /// <summary>
        /// Does the hierarchical classification dialog box no longer pop up?
        /// </summary>
        private bool _noPromptLayersBuilder;

        /// <summary>
        /// Loading progress bar
        /// </summary>
        private LoadingBar _loading;

        /// <summary>
        /// The current program assembly, version, culture, and PublicKeyToken. 
        /// </summary>
        private readonly Assembly _asm;

        /// <summary>
        /// 瓦片服务加载视觉体验杆
        /// </summary>
        public LoadingBar TileLoading;

        /// <summary>
        /// 文件加载预览视觉体验杆
        /// </summary>
        public LoadingBar FilePreviewLoading;

        /// <summary>
        /// 空白底图名称
        /// </summary>
        private const string EmptyMapProviderKey = "None";

        /// <summary>
        /// 底图字典
        /// </summary>
        public Dictionary<string, GMapProvider> GMapProviderDictionary = new();

        /// <summary>
        /// 获取或设置要素属性框的文本内容  
        /// </summary>
        public string MapBoxPropertyText
        {
            get => MapBoxProperty.Text;
            set
            {
                BeginInvoke(method: () => { MapBoxProperty.Text = value ?? ""; });
            }
        }

        public void SetStatusText(string text)
        {
            statusText.Text = text ?? "";
        }

        private string _dataPoolGridCellValue;

        /// <summary>
        /// 集群用户是否具备管理员权限，默认：false
        /// </summary>
        private bool _administrator;

        /// <summary>
        /// 图形窗口内容是否处于拖动状态
        /// </summary>
        private bool _mapDrag;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            _asm = Assembly.GetExecutingAssembly();
            _noPromptMetaData = _noPromptLayersBuilder = false;
            GetCopyright =
                ((AssemblyCopyrightAttribute)_asm.GetCustomAttribute(attributeType: typeof(AssemblyCopyrightAttribute)))
                ?.Copyright;
        }

        private void InitializeBackgroundWorker()
        {
            fileWorker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                e.Result = FileWorkStart(fileBackgroundWorker: sender as BackgroundWorker, e: e);
            };
            fileWorker.ProgressChanged += FileWorkProgress;
            fileWorker.RunWorkerCompleted += FileWorkCompleted;
            vectorWorker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                e.Result = VectorWorkStart(vectorBackgroundWorker: sender as BackgroundWorker, e: e);
            };
            vectorWorker.ProgressChanged += VectorWorkProgress;
            vectorWorker.RunWorkerCompleted += VectorWorkCompleted;
            rasterWorker.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                e.Result = RasterWorkStart(rasterBackgroundWorker: sender as BackgroundWorker, e: e);
            };
            rasterWorker.ProgressChanged += RasterWorkProgress;
            rasterWorker.RunWorkerCompleted += RasterWorkCompleted;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $@"{((AssemblyTitleAttribute)_asm.GetCustomAttribute(attributeType: typeof(AssemblyTitleAttribute)))?.Title} - {((AssemblyFileVersionAttribute)_asm.GetCustomAttribute(attributeType: typeof(AssemblyFileVersionAttribute)))?.Version}";
            statusText.Text = GetCopyright;
            _loading = new LoadingBar(bar: DatabaseProgressBar);

            var key = ogcCard.Name;
            var defaultValue = RegEdit.Getkey(keyname: key);
            ogcCard.SelectedIndex = int.Parse(s: defaultValue ?? "2");

            key = DatabaseTabControl.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            DatabaseTabControl.SelectedIndex = int.Parse(s: defaultValue ?? "0");

            key = GeositeServerUrl.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            GeositeServerUrl.Text = defaultValue ?? "";

            key = Name;
            defaultValue = RegEdit.Getkey(keyname: key, defaultvalue: string.Empty);
            if (!string.IsNullOrWhiteSpace(value: defaultValue))
            {
                var splitArray = Regex.Split(
                    input: defaultValue,
                    pattern: @"[\s]*,[\s]*",
                    options: RegexOptions.Singleline | RegexOptions.Multiline
                );
                if (splitArray.Length == 5)
                {
                    switch (splitArray[0])
                    {
                        case "Maximized":
                            {
                                WindowState = FormWindowState.Maximized;
                                break;
                            }
                        case "Minimized":
                            {
                                WindowState = FormWindowState.Minimized;
                                break;
                            }
                        default: //Normal
                            {
                                WindowState = FormWindowState.Normal;
                                var locationX = int.Parse(s: splitArray[1]);
                                if (locationX < 0)
                                    locationX = 0;
                                var locationY = int.Parse(s: splitArray[2]);
                                if (locationY < 0)
                                    locationY = 0;
                                //Location = new System.Drawing.Point(x: locationX, y: locationY);
                                Location = new Point(x: locationX, y: locationY);
                                Size = new Size(width: int.Parse(s: splitArray[3]), height: int.Parse(s: splitArray[4]));
                                break;
                            }
                    }
                }
            }
            else
                StartPosition = FormStartPosition.CenterScreen;

            key = GeositeServerUser.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            GeositeServerUser.Text = defaultValue ?? "";

            key = GeositeServerPassword.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            GeositeServerPassword.Text = defaultValue ?? "";

            key = FormatStandard.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            FormatStandard.Checked = bool.Parse(value: defaultValue ?? "True");

            key = FormatTMS.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            FormatTMS.Checked = bool.Parse(value: defaultValue ?? "False");

            key = FormatMapcruncher.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            FormatMapcruncher.Checked = bool.Parse(value: defaultValue ?? "False");

            key = FormatArcGIS.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            FormatArcGIS.Checked = bool.Parse(value: defaultValue ?? "False");

            key = FormatDeepZoom.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            FormatDeepZoom.Checked = bool.Parse(value: defaultValue ?? "False");

            key = FormatRaster.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            FormatRaster.Checked = bool.Parse(value: defaultValue ?? "False");

            key = EPSG4326.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            EPSG4326.Checked = bool.Parse(value: defaultValue ?? "False");

            key = UpdateBox.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            UpdateBox.Checked = bool.Parse(value: defaultValue ?? "True");

            key = tileLevels.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            tileLevels.Text = defaultValue ?? "-1";

            key = themeNameBox.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            themeNameBox.Text = defaultValue ?? "";

            key = localTileFolder.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            localTileFolder.Text = defaultValue ?? "";

            key = ModelOpenTextBox.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            ModelOpenTextBox.Text = defaultValue ?? "";
            ModelSave.Enabled = !string.IsNullOrWhiteSpace(value: ModelOpenTextBox.Text);

            key = tilewebapi.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            tilewebapi.Text = defaultValue ?? "";

            key = wmtsNorth.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsNorth.Text = defaultValue ?? "90";

            key = wmtsSouth.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsSouth.Text = defaultValue ?? "-90";

            key = wmtsWest.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsWest.Text = defaultValue ?? "-180";

            key = wmtsEast.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsEast.Text = defaultValue ?? "180";

            key = subdomainsBox.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            subdomainsBox.Text = defaultValue ?? "";

            key = wmtsMinZoom.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsMinZoom.Text = defaultValue ?? "0";

            key = wmtsSpider.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsMinZoom.Enabled = wmtsMaxZoom.Enabled = !(wmtsSpider.Checked = bool.Parse(value: defaultValue ?? "False"));

            key = wmtsMaxZoom.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            wmtsMaxZoom.Text = defaultValue ?? "18";

            key = rasterTileSize.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            rasterTileSize.Text = defaultValue ?? "100";

            key = nodatabox.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            nodatabox.Text = defaultValue ?? "";

            key = maptilertoogc.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            maptilertoogc.Checked = bool.Parse(value: defaultValue ?? "True");

            key = mapcrunchertoogc.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            mapcrunchertoogc.Checked = bool.Parse(value: defaultValue ?? "False");

            key = ogctomapcruncher.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            ogctomapcruncher.Checked = bool.Parse(value: defaultValue ?? "False");

            key = ogctomaptiler.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            ogctomaptiler.Checked = bool.Parse(value: defaultValue ?? "False");

            key = MIMEBox.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            MIMEBox.Text = defaultValue ?? "png";

            key = rankList.Name;
            defaultValue = RegEdit.Getkey(keyname: key);
            rankList.Text = defaultValue ?? "-1";

            const string embedPath = "rtf"; // Path.Combine(path1: Directory.GetCurrentDirectory(), path2: "embed");
            try
            {
                FormatStandardBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "standard.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                FormatStandardBox.Text = error.Message;
            }
            try
            {
                FormatTMSBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "tms.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                FormatTMSBox.Text = error.Message;
            }
            try
            {
                FormatMapcruncherBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "mapcruncher.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                FormatMapcruncherBox.Text = error.Message;
            }
            try
            {
                FormatArcGISBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "arcgis.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                FormatArcGISBox.Text = error.Message;
            }
            try
            {
                FormatDeepZoomBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "deepzoom.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                FormatDeepZoomBox.Text = error.Message;
            }
            try
            {
                FormatRasterBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "raster.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                FormatRasterBox.Text = error.Message;
            }
            try
            {
                wmtsTipBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "wmtstip.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                wmtsTipBox.Text = error.Message;
            }
            try
            {
                modelTipBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "modeltip.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                modelTipBox.Text = error.Message;
            }
            try
            {
                convertTipBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "converttip.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                convertTipBox.Text = error.Message;
            }
            try
            {
                readmeTextBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "readme.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                readmeTextBox.Text = error.Message;
            }
            try
            {
                apiTextBox.LoadFile(path: Path.Combine(path1: embedPath, path2: "api.rtf"), fileType: RichTextBoxStreamType.RichText);
            }
            catch (Exception error)
            {
                apiTextBox.Text = error.Message;
            }

            var fadeIn = new System.Windows.Forms.Timer
            {
                Site = null,
                Tag = null,
                Enabled = false,
                Interval = 16
            };
            fadeIn.Tick += (_, _) =>
            {
                if (Opacity >= 1)
                {
                    fadeIn.Stop();
                    MapBox.SizeChanged += MapBox_SizeChanged;
                    DrawMapGrid();
                }
                else
                    Opacity += 0.05;
            };
            fadeIn.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            if (fileWorker.IsBusy && fileWorker.WorkerSupportsCancellation)
                fileWorker.CancelAsync();

            if (vectorWorker.IsBusy && vectorWorker.WorkerSupportsCancellation)
                vectorWorker.CancelAsync();

            if (rasterWorker.IsBusy && rasterWorker.WorkerSupportsCancellation)
                rasterWorker.CancelAsync();

            foreach (var task in MapView.Tasks)
                task.CancelTask();

            RegEdit.Setkey(
                keyname: Name,
                defaultvalue:
                $"{WindowState},{Location.X},{Location.Y},{Size.Width},{Size.Height}"
            );

            RegEdit.Setkey(
                keyname: MapBox.Name,
                defaultvalue: $"{MapBox.Zoom},{MapBox.Position.Lng},{MapBox.Position.Lat}"
            );

            var fadeOut = new System.Windows.Forms.Timer
            {
                Site = null,
                Tag = null,
                Enabled = false,
                Interval = 16
            };
            fadeOut.Tick += (_, _) =>
            {
                if (Opacity <= 0)
                {
                    fadeOut.Stop();
                    Close();
                }
                else
                    Opacity -= 0.1;
            };
            fadeOut.Start();
            if (Opacity == 0)
                e.Cancel = false;
        }

        private void DatabaseLog_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Modifiers & Keys.Control) == 0 || e.KeyCode != Keys.C)
                return;
            var selectedItems = DatabaseLog.SelectedItems;
            if (selectedItems.Count <= 0)
                return;
            var text = new StringBuilder();
            foreach (var item in selectedItems)
                if (item != null)
                    text.Append(value: item);
            if (text.Length > 0)
                Clipboard.SetText(text: string.Join(separator: Environment.NewLine, values: text), format: TextDataFormat.Text);
        }

        private void FileCheck()
        {
            statusText.Text = GetCopyright;
            FileRunButton.Enabled = FileGridView.Rows.Count > 0 && !string.IsNullOrWhiteSpace(value: vectorTargetFile.Text);
        }

        private void FileLoadLog_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Modifiers & Keys.Control) == 0 || e.KeyCode != Keys.C)
                return;
            var selectedItems = FileLoadLog.SelectedItems;
            if (selectedItems.Count <= 0)
                return;
            var text = new StringBuilder();
            foreach (var item in selectedItems)
                if (item != null)
                    text.Append(value: item);
            if (text.Length > 0)
                Clipboard.SetText(text: string.Join(separator: Environment.NewLine, values: text), format: TextDataFormat.Text);
        }

        private void VectorOpenFile_Click(object sender, EventArgs e)
        {
            var key = vectorOpenButton.Name;
            if (!int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex))
                filterIndex = 0;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog
            {
                Filter =
                    @"MapGIS|*.wt;*.wl;*.wp|" +
                    @"MapGIS|*.mpj|" +
                    @"ShapeFile|*.shp|" +
                    @"Excel Tab Delimited|*.txt|" +
                    @"Excel Comma Delimited|*.csv|" +
                    @"Excel|*.xls;*.xlsx;*.xlsb|" +
                    @"GoogleEarth(*.kml)|*.kml|" +
                    @"GeositeXML|*.xml|" +
                    @"GeoJson|*.geojson",
                FilterIndex = filterIndex,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: key, defaultvalue: $"{openFileDialog.FilterIndex}");
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        vectorTargetFile.Text = string.Empty;
                        SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        FileGridView.SelectAll();
                        CleanFileGridView();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 or 2 => "MapGIS",
                            3 => "ShapeFile",
                            4 => "TXT",
                            5 => "CSV",
                            6 => "Excel",
                            7 => "KML",
                            8 => "GeositeXML",
                            9 => "GeoJson",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var fileType = Path.GetExtension(path: theFile).ToLower();
                                if (fileType == ".mpj")
                                {
                                    var mapgisProject = new MapGis.MapGisProject();
                                    mapgisProject.Open(file: theFile);
                                    var files = mapgisProject.Content?["files"];
                                    if (files != null)
                                    {
                                        var currentPath = Path.GetDirectoryName(theFile);
                                        foreach (var file in files)
                                        {
                                            var getTheFile = Path.GetFullPath(Path.Combine(currentPath ?? "", (string)file["file"] ?? ""));
                                            if (File.Exists(getTheFile))
                                            {
                                                var theFileType = Path.GetExtension(path: getTheFile).ToLower();
                                                switch (theFileType)
                                                {
                                                    case ".wt":
                                                    case ".wl":
                                                    case ".wp":
                                                        {
                                                            var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: getTheFile)];
                                                            row.Height = 28;
                                                            var projectionButton = row.Cells[index: 1];
                                                            projectionButton.ToolTipText = "Unknown";
                                                            projectionButton.Value = "?";
                                                            var previewButton = row.Cells[index: 2];
                                                            previewButton.ToolTipText = theFileFormat;
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            FileLoadLogAdd(input: $"[{getTheFile}] in mpj is not supported.");
                                                            break;
                                                        }
                                                }
                                            }
                                            else
                                            {
                                                FileLoadLogAdd(input: $"[{getTheFile}] in mpj does not exist.");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                    row.Height = 28;
                                    var projectionButton = row.Cells[index: 1];
                                    projectionButton.ToolTipText = "Unknown";
                                    projectionButton.Value = "?";
                                    var previewButton = row.Cells[index: 2];
                                    previewButton.ToolTipText = theFileFormat;
                                }
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 2:
                            //{
                            //    SaveAsFormat.Items.Add(item: @"JSON(*.json)");
                            //    SaveAsFormat.SelectedIndex = 0;
                            //    break;
                            //}
                            case 1:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 8:
                                {
                                    SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                    SaveAsFormat.Items.Add(item: @"GeoJSON(*.geojson)");
                                    SaveAsFormat.Items.Add(item: @"GoogleEarth(*.kml)");
                                    SaveAsFormat.Items.Add(item: @"Gml(*.gml)");
                                    SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                    SaveAsFormat.SelectedIndex = 0;
                                    break;
                                }
                            case 7:
                            case 9:
                                {
                                    SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                    SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                    SaveAsFormat.SelectedIndex = 0;
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void MapGisIcon_Click(object sender, EventArgs e)
        {
            var key = mapgisButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            if (!int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex))
                filterIndex = 0;
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"MapGIS|*.wt;*.wl;*.wp|MapGIS|*.mpj",
                FilterIndex = filterIndex,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: key, defaultvalue: $"{openFileDialog.FilterIndex}");
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        vectorTargetFile.Text = string.Empty;
                        FileGridView.SelectAll();
                        CleanFileGridView();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 or 2 => "MapGIS",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var fileType = Path.GetExtension(path: theFile).ToLower();
                                if (fileType == ".mpj")
                                {
                                    var mapgisProject = new MapGis.MapGisProject();
                                    mapgisProject.Open(file: theFile);
                                    var files = mapgisProject.Content?["files"];
                                    if (files != null)
                                    {
                                        var currentPath = Path.GetDirectoryName(theFile);
                                        foreach (var file in files)
                                        {
                                            var getTheFile = Path.GetFullPath(Path.Combine(currentPath ?? "", (string)file["file"] ?? ""));
                                            if (File.Exists(getTheFile))
                                            {
                                                var theFileType = Path.GetExtension(path: getTheFile).ToLower();
                                                switch (theFileType)
                                                {
                                                    case ".wt":
                                                    case ".wl":
                                                    case ".wp":
                                                        {
                                                            var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: getTheFile)];
                                                            row.Height = 28;
                                                            var projectionButton = row.Cells[index: 1];
                                                            projectionButton.ToolTipText = "Unknown";
                                                            projectionButton.Value = "?";
                                                            var previewButton = row.Cells[index: 2];
                                                            previewButton.ToolTipText = theFileFormat;
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            FileLoadLogAdd(input: $"[{getTheFile}] in mpj is not supported.");
                                                            break;
                                                        }
                                                }
                                            }
                                            else
                                            {
                                                FileLoadLogAdd(input: $"[{getTheFile}] in mpj does not exist.");
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                    row.Height = 28;
                                    var projectionButton = row.Cells[index: 1];
                                    projectionButton.ToolTipText = "Unknown";
                                    projectionButton.Value = "?";
                                    var previewButton = row.Cells[index: 2];
                                    previewButton.ToolTipText = theFileFormat;
                                }
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 2:
                            //SaveAsFormat.Items.Add(item: @"JSON(*.json)");
                            //SaveAsFormat.SelectedIndex = 0;
                            //break;
                            case 1:
                                SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                SaveAsFormat.Items.Add(item: @"GeoJSON(*.geojson)");
                                SaveAsFormat.Items.Add(item: @"GoogleEarth(*.kml)");
                                SaveAsFormat.Items.Add(item: @"Gml(*.gml)");
                                SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                SaveAsFormat.SelectedIndex = 0;
                                break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void ArcGisIcon_Click(object sender, EventArgs e)
        {
            var key = arcgisIconButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"ShapeFile|*.shp",
                FilterIndex = 0,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        vectorTargetFile.Text = string.Empty;
                        FileGridView.SelectAll();
                        CleanFileGridView();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 => "ShapeFile",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                row.Height = 28;
                                var projectionButton = row.Cells[index: 1];
                                projectionButton.ToolTipText = "Unknown";
                                projectionButton.Value = "?";
                                var previewButton = row.Cells[index: 2];
                                previewButton.ToolTipText = theFileFormat;
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 1:
                                SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                SaveAsFormat.Items.Add(item: @"GeoJSON(*.geojson)");
                                SaveAsFormat.Items.Add(item: @"GoogleEarth(*.kml)");
                                SaveAsFormat.Items.Add(item: @"Gml(*.gml)");
                                SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                SaveAsFormat.SelectedIndex = 0;
                                break;
                                //default:
                                //    break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void TabTextIcon_Click(object sender, EventArgs e)
        {
            var key = tableIconButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"Excel|*.xls;*.xlsx;*.xlsb|Textual format|*.txt;*.csv",
                FilterIndex = 0,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        vectorTargetFile.Text = string.Empty;
                        SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 => "Excel",
                            2 => "TXT/CSV",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                row.Height = 28;
                                var projectionButton = row.Cells[index: 1];
                                projectionButton.ToolTipText = "Unknown";
                                projectionButton.Value = "?";
                                var previewButton = row.Cells[index: 2];
                                previewButton.ToolTipText = theFileFormat;
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 1:
                            case 2:
                                SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                SaveAsFormat.Items.Add(item: @"GeoJSON(*.geojson)");
                                SaveAsFormat.Items.Add(item: @"GoogleEarth(*.kml)");
                                SaveAsFormat.Items.Add(item: @"Gml(*.gml)");
                                SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                SaveAsFormat.SelectedIndex = 0;
                                break;
                                //default:
                                //    break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void GeoJsonIcon_Click(object sender, EventArgs e)
        {
            var key = geojsonIconButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"GeoJSON|*.geojson",
                FilterIndex = 0,
                Multiselect = true

            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        vectorTargetFile.Text = SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        FileGridView.SelectAll();
                        CleanFileGridView();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 => "GeoJSON",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                row.Height = 28;
                                var projectionButton = row.Cells[index: 1];
                                projectionButton.ToolTipText = "Unknown";
                                projectionButton.Value = "?";
                                var previewButton = row.Cells[index: 2];
                                previewButton.ToolTipText = theFileFormat;
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 1:
                                SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                SaveAsFormat.SelectedIndex = 0;
                                break;
                                //default:
                                //    break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void GeositeIcon_Click(object sender, EventArgs e)
        {
            var key = geositeIconButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"GeositeXML|*.xml",
                FilterIndex = 0,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        vectorTargetFile.Text = SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        FileGridView.SelectAll();
                        CleanFileGridView();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 => "GeositeXML",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                row.Height = 28;
                                var projectionButton = row.Cells[index: 1];
                                projectionButton.ToolTipText = "Unknown";
                                projectionButton.Value = "?";
                                var previewButton = row.Cells[index: 2];
                                previewButton.ToolTipText = theFileFormat;
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 1:
                                SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                SaveAsFormat.Items.Add(item: @"GeoJSON(*.geojson)");
                                SaveAsFormat.Items.Add(item: @"GoogleEarth(*.kml)");
                                SaveAsFormat.Items.Add(item: @"Gml(*.gml)");
                                SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                SaveAsFormat.SelectedIndex = 0;
                                break;
                                //default:
                                //    break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void KmlIcon_Click(object sender, EventArgs e)
        {
            var key = kmlIconButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"GoogleEarth(*.kml)|*.kml",
                FilterIndex = 0,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    var vectorSourceFiles = openFileDialog.FileNames;
                    if (vectorSourceFiles.Length > 0)
                    {
                        vectorTargetFile.Text = SaveAsFormat.Text = string.Empty;
                        SaveAsFormat.Items.Clear();
                        FileGridView.SelectAll();
                        CleanFileGridView();
                        var theFileFormat = openFileDialog.FilterIndex
                            switch
                        {
                            1 => "KML",
                            _ => null
                        };
                        if (theFileFormat != null)
                            foreach (var theFile in vectorSourceFiles)
                            {
                                var row = FileGridView.Rows[index: FileGridView.Rows.Add(values: theFile)];
                                row.Height = 28;
                                var projectionButton = row.Cells[index: 1];
                                projectionButton.ToolTipText = "Unknown";
                                projectionButton.Value = "?";
                                var previewButton = row.Cells[index: 2];
                                previewButton.ToolTipText = theFileFormat;
                            }
                        switch (openFileDialog.FilterIndex)
                        {
                            case 1:
                                SaveAsFormat.Items.Add(item: @"ESRI ShapeFile(*.shp)");
                                SaveAsFormat.Items.Add(item: @"GeositeXML(*.xml)");
                                SaveAsFormat.SelectedIndex = 0;
                                break;
                                //default:
                                //    break;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                FileLoadLogAdd(input: statusText.Text = error.Message);
            }
            FileCheck();
        }

        private void VectorSaveFile_Click(object sender, EventArgs e)
        {
            var vectorSourceFiles = FileGridView.Rows.Cast<DataGridViewRow>().Select(selector: col => col.Cells[index: 0].Value.ToString()).ToArray();
            var vectorSourceFileCount = vectorSourceFiles.Length;
            if (vectorSourceFileCount > 0)
            {
                var vectorSourceFileText = vectorSourceFiles[0];
                if (string.IsNullOrWhiteSpace(value: vectorSourceFileText))
                {
                    MessageBox.Show(text: @"Please select file[s] first", caption: @"Tip", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                    return;
                }
                var key = vectorSaveButton.Name;
                var path = key + "_path";
                var oldPath = RegEdit.Getkey(keyname: path);
                if (vectorSourceFileCount == 1)
                {
                    var sourceFileExt = Path.GetExtension(path: vectorSourceFileText).ToLower();
                    int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex);
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = sourceFileExt == ".geojson"
                            ? @"GeositeXML(*.xml)|*.xml|ESRI ShapeFile(*.shp)|*.shp"
                            : sourceFileExt == ".kml"
                                ? @"GeositeXML(*.xml)|*.xml|ESRI ShapeFile(*.shp)|*.shp"
                                : sourceFileExt == ".xml"
                                    ? @"ESRI ShapeFile(*.shp)|*.shp|GeoJSON(*.geojson)|*.geojson|GoogleEarth(*.kml)|*.kml|Gml(*.gml)|*.gml|GeositeXML(*.xml)|*.xml"
                                    : sourceFileExt == ".mpj"
                                        ? @"JSON(*.json)|*.json"
                                        : @"GeositeXML(*.xml)|*.xml|GeoJSON(*.geojson)|*.geojson|ESRI ShapeFile(*.shp)|*.shp|GoogleEarth(*.kml)|*.kml|Gml(*.gml)|*.gml|Text(*.txt)|*.txt",
                        FilterIndex = filterIndex
                    };
                    if (Directory.Exists(path: oldPath))
                        saveFileDialog.InitialDirectory = oldPath;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        RegEdit.Setkey(keyname: key, defaultvalue: $"{saveFileDialog.FilterIndex}");
                        RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: saveFileDialog.FileName));
                        vectorTargetFile.Text = saveFileDialog.FileName;
                    }
                    else
                        vectorTargetFile.Text = string.Empty;
                }
                else
                {
                    var openFolderDialog = new FolderBrowserDialog()
                    {
                        Description = @"Please select a destination folder",
                        ShowNewFolderButton = true
                    };
                    if (Directory.Exists(path: oldPath))
                        openFolderDialog.SelectedPath = oldPath;
                    if (openFolderDialog.ShowDialog() == DialogResult.OK)
                    {
                        RegEdit.Setkey(keyname: path, defaultvalue: openFolderDialog.SelectedPath);
                        vectorTargetFile.Text = openFolderDialog.SelectedPath;
                    }
                }
                FileCheck();
            }
        }

        private void FileRun_Click(object sender, EventArgs e)
        {
            if (fileWorker.IsBusy)
            {
                FileLoadLogAdd(input: statusText.Text = @"Task Cancelling ...");
                fileWorker.CancelAsync();
                FileLoadLogAdd(input: statusText.Text = @"Task canceled.");
                return;
            }
            statusProgress.Visible = true;
            fileWorker.RunWorkerAsync
            (
            argument: (FileGridViewRow: FileGridView.Rows.Cast<DataGridViewRow>().ToList(), TargetPath: vectorTargetFile.Text, SaveAsFormat: SaveAsFormat.Text)
            );
        }

        private string FileWorkStart(BackgroundWorker fileBackgroundWorker, DoWorkEventArgs e)
        {
            var argument = ((List<DataGridViewRow> FileGridViewRow, string TargetPath, string SaveAsFormat)?)e.Argument;
            if (argument != null)
            {
                var fileGridViewRow = argument.Value.FileGridViewRow;
                var sourceFiles = fileGridViewRow.Select(row => (string)row.Cells[0].Value).ToArray();
                var sourceProjections = fileGridViewRow.Select(row => (XElement)row.Cells[1].Tag).ToArray();
                var targetPath = argument.Value.TargetPath;
                var isDirectory = Path.GetExtension(path: targetPath) == string.Empty;
                var saveAsFormat = argument.Value.SaveAsFormat;
                var targetType = isDirectory
                    ? Regex.IsMatch(input: saveAsFormat, pattern: @"\(\*.json\)", options: RegexOptions.IgnoreCase)
                        ? ".json"
                        : Regex.IsMatch(input: saveAsFormat, pattern: @"\(\*.shp\)", options: RegexOptions.IgnoreCase)
                            ? ".shp"
                            : Regex.IsMatch(input: saveAsFormat, pattern: @"\(\*.geojson\)", options: RegexOptions.IgnoreCase)
                                ? ".geojson"
                                : Regex.IsMatch(input: saveAsFormat, pattern: @"\(\*.kml\)", options: RegexOptions.IgnoreCase)
                                    ? ".kml"
                                    : Regex.IsMatch(input: saveAsFormat, pattern: @"\(\*.gml\)", options: RegexOptions.IgnoreCase)
                                        ? ".gml"
                                        : Regex.IsMatch(input: saveAsFormat, pattern: @"\(\*.txt\)", options: RegexOptions.IgnoreCase)
                                            ? ".txt"
                                            : ".xml"
                    : Path.GetExtension(path: targetPath).ToLower();
                try
                {
                    for (var i = 0; i < sourceFiles.Length; i++)
                    {
                        if (fileBackgroundWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            break;
                        }
                        var sourceFile = sourceFiles[i];
                        var projectionX = sourceProjections[i];

                        string targetFile;
                        if (isDirectory)
                        {
                            var postfix = 0;
                            do
                            {
                                targetFile = Path.Combine(path1: targetPath, path2: Path.GetFileNameWithoutExtension(path: sourceFile) + (postfix == 0 ? "" : $"({postfix})") + targetType);
                                if (!File.Exists(path: targetFile))
                                    break;
                                postfix++;
                            } while (true);
                        }
                        else
                            targetFile = targetPath;
                        var fileType = Path.GetExtension(path: sourceFile)?.ToLower();
                        switch (fileType)
                        {
                            case ".shp":
                                {
                                    var codePage = ShapeFileHelper.ShapeFile.GetDbfCodePage(
                                        dbfFileName: Path.Combine(path1: Path.GetDirectoryName(path: sourceFile) ?? "", path2: Path.GetFileNameWithoutExtension(path: sourceFile) + ".dbf"),
                                        defaultCodePage: Encoding.Default.CodePage
                                    );
                                    using var shapeFile = new ShapeFileHelper.ShapeFileReader();
                                    var localI = i + 1;
                                    shapeFile.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                    {
                                        object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                                            ? sourceFiles.Length > 1
                                                ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                                                : thisEvent.Message
                                            : null;
                                        fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                                    };
                                    shapeFile.Open(filePath: sourceFile, defaultCodePage: codePage.CodePage, projection: projectionX);
                                    switch (targetType)
                                    {
                                        case ".shp":
                                            {
                                                shapeFile.Export(saveAs: targetFile, format: "shapefile");
                                                break;
                                            }
                                        case ".xml":
                                        case ".kml":
                                        case ".gml":
                                            {
                                                if (isDirectory)
                                                    shapeFile.Export(
                                                        saveAs: targetFile,
                                                        format: Path.GetExtension(path: targetFile).ToLower()[1..],
                                                        treePath: ConsoleIO.FilePathToXPath(path: sourceFile)
                                                    );
                                                else
                                                {
                                                    string treePathString = null;
                                                    XElement description = null;
                                                    var canDo = true;
                                                    if (!_noPromptLayersBuilder)
                                                    {
                                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: sourceFile).FullName);
                                                        getTreeLayers.ShowDialog();
                                                        if (getTreeLayers.Ok)
                                                        {
                                                            treePathString = getTreeLayers.TreePathString;
                                                            description = getTreeLayers.Description;
                                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                                        }
                                                        else
                                                            canDo = false;
                                                    }
                                                    else
                                                    {
                                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: sourceFile).FullName);
                                                    }
                                                    if (canDo)
                                                    {
                                                        shapeFile.Export(
                                                            saveAs: targetFile,
                                                            format: Path.GetExtension(path: targetFile).ToLower()[1..],
                                                            treePath: treePathString,
                                                            extraDescription: description
                                                        );
                                                    }
                                                }
                                                break;
                                            }
                                        case ".geojson":
                                            {
                                                shapeFile.Export(saveAs: targetFile);
                                                break;
                                            }
                                        case ".txt":
                                            {
                                                shapeFile.Export(saveAs: targetFile, format: "txt");
                                                break;
                                            }
                                    }
                                    break;
                                }
                            //case ".mpj":
                            //    {
                            //        var mapgisProject = new MapGis.MapGisProject();
                            //        var localI = i + 1;
                            //        mapgisProject.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                            //        {
                            //            object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                            //                ? sourceFiles.Length > 1
                            //                    ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                            //                    : thisEvent.Message
                            //                : null;
                            //            fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                            //        };
                            //        mapgisProject.Open(file: sourceFile);
                            //        mapgisProject.Export(saveAs: targetFile); 
                            //        break;
                            //    }
                            case ".txt":
                            case ".csv":
                            case ".xls":
                            case ".xlsx":
                            case ".xlsb":
                                {
                                    var freeTextFields = fileType switch
                                    {
                                        ".txt" => TXT.GetFieldNames(file: sourceFile),
                                        ".csv" => TXT.GetFieldNames(file: sourceFile),
                                        _ => FreeText.Excel.Excel.GetFieldNames(file: sourceFile)
                                    };
                                    if (freeTextFields.Length == 0)
                                        throw new Exception(message: "No valid fields found");
                                    string coordinateFieldName;
                                    if (freeTextFields.Any(predicate: f => f == "_position_"))
                                        coordinateFieldName = "_position_";
                                    else
                                    {
                                        if (isDirectory)
                                            coordinateFieldName = "_position_";
                                        else
                                        {
                                            var freeTextFieldsForm = new FreeTextFieldForm(fieldNames: freeTextFields);
                                            freeTextFieldsForm.ShowDialog();
                                            var choice = freeTextFieldsForm.Ok;
                                            if (choice == null)
                                                throw new Exception(message: "Task Cancellation");
                                            coordinateFieldName = choice.Value ? freeTextFieldsForm.CoordinateFieldName : null;
                                        }
                                    }
                                    FreeText.FreeText freeText = fileType switch
                                    {
                                        ".txt" => new TXT(coordinateFieldName: coordinateFieldName),
                                        ".csv" => new CSV(coordinateFieldName: coordinateFieldName),
                                        _ => new FreeText.Excel.Excel(coordinateFieldName: coordinateFieldName)
                                    };
                                    var localI = i + 1;
                                    freeText.OnMessagerEvent +=
                                        delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                                                ? sourceFiles.Length > 1
                                                    ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                                                    : thisEvent.Message
                                                : null;
                                            fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                                        };
                                    freeText.Open(file: sourceFile, projection: projectionX);
                                    switch (Path.GetExtension(path: targetFile)?.ToLower())
                                    {
                                        case ".shp":
                                            {
                                                freeText.Export(saveAs: targetFile, format: "shapefile");
                                                break;
                                            }
                                        case ".geojson":
                                            {
                                                freeText.Export(saveAs: targetFile);
                                                break;
                                            }
                                        case ".xml":
                                        case ".kml":
                                        case ".gml":
                                            {
                                                if (isDirectory)
                                                {
                                                    freeText.Export(
                                                        saveAs: targetFile,
                                                        format: Path.GetExtension(path: targetFile).ToLower()[1..],
                                                        treePath: ConsoleIO.FilePathToXPath(path: sourceFile));
                                                }
                                                else
                                                {
                                                    string treePathString = null;
                                                    XElement description = null;
                                                    var canDo = true;
                                                    if (!_noPromptLayersBuilder)
                                                    {
                                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: sourceFile).FullName);
                                                        getTreeLayers.ShowDialog();
                                                        if (getTreeLayers.Ok)
                                                        {
                                                            treePathString = getTreeLayers.TreePathString;
                                                            description = getTreeLayers.Description;
                                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                                        }
                                                        else
                                                            canDo = false;
                                                    }
                                                    else
                                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: sourceFile).FullName);
                                                    if (canDo)
                                                        freeText.Export(
                                                            saveAs: targetFile,
                                                            format: Path.GetExtension(path: targetFile).ToLower()[1..],
                                                            treePath: treePathString,
                                                            extraDescription: description
                                                        );
                                                }
                                                break;
                                            }
                                    }
                                }
                                break;
                            case ".kml":
                                {
                                    using var kml = new GeositeXml.GeositeXml(projectionX);
                                    var localI = i + 1;
                                    kml.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                    {
                                        object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                                            ? sourceFiles.Length > 1
                                                ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                                                : thisEvent.Message
                                            : null;
                                        fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                                    };
                                    if (isDirectory)
                                        switch (Path.GetExtension(path: targetFile)?.ToLower())
                                        {
                                            case ".xml":
                                                {
                                                    kml.KmlToGeositeXml(
                                                        input: sourceFile,
                                                        output: targetFile
                                                    );
                                                    break;
                                                }
                                            case ".shp":
                                                {
                                                    var geositeXml = kml.KmlToGeositeXml(input: sourceFile, output: null);
                                                    kml.GeositeXmlToShp(
                                                        geositeXml: geositeXml.Root,
                                                        shapeFileName: targetFile
                                                    );
                                                    break;
                                                }
                                        }
                                    else
                                    {
                                        XElement description = null;
                                        var canDo = true;
                                        if (!_noPromptLayersBuilder)
                                        {
                                            var getTreeLayers = new LayersBuilderForm();
                                            getTreeLayers.ShowDialog();
                                            if (getTreeLayers.Ok)
                                            {
                                                description = getTreeLayers.Description;
                                                _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                            }
                                            else
                                                canDo = false;
                                        }
                                        if (canDo)
                                            switch (Path.GetExtension(path: targetFile)?.ToLower())
                                            {
                                                case ".xml":
                                                    {
                                                        kml.KmlToGeositeXml(input: sourceFile, output: targetFile, extraDescription: description);
                                                        break;
                                                    }
                                                case ".shp":
                                                    {
                                                        var geositeXml = kml.KmlToGeositeXml(input: sourceFile, output: null, extraDescription: description);
                                                        kml.GeositeXmlToShp(geositeXml: geositeXml.Root, shapeFileName: targetFile);
                                                        break;
                                                    }
                                            }
                                    }
                                    break;
                                }
                            case ".xml":
                                {
                                    using var xml = new GeositeXml.GeositeXml(projectionX);
                                    var localI = i + 1;
                                    xml.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                    {
                                        object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                                            ? sourceFiles.Length > 1
                                                ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                                                : thisEvent.Message
                                            : null;
                                        fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                                    };
                                    if (isDirectory)
                                        switch (Path.GetExtension(path: targetFile)?.ToLower())
                                        {
                                            case ".kml":
                                                {
                                                    xml.GeositeXmlToKml(input: sourceFile, output: targetFile);
                                                    break;
                                                }
                                            case ".xml":
                                                {
                                                    xml.GeositeXmlToGeositeXml(input: sourceFile, output: targetFile);
                                                    break;
                                                }
                                            case ".gml":
                                                {
                                                    xml.GeositeXmlToGml(input: sourceFile, output: targetFile);
                                                    break;
                                                }
                                            case ".geojson":
                                                {
                                                    xml.GeositeXmlToGeoJson(input: sourceFile, output: targetFile);
                                                    break;
                                                }
                                            case ".shp":
                                                {
                                                    var geositeXml = xml.GeositeXmlToGeositeXml(input: sourceFile, output: null);
                                                    xml.GeositeXmlToShp(geositeXml: geositeXml.Root, shapeFileName: targetFile);
                                                    break;
                                                }
                                        }
                                    else
                                    {
                                        XElement description = null;
                                        var canDo = true;
                                        if (!_noPromptLayersBuilder)
                                        {
                                            var getTreeLayers = new LayersBuilderForm();
                                            getTreeLayers.ShowDialog();
                                            if (getTreeLayers.Ok)
                                            {
                                                description = getTreeLayers.Description;
                                                _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                            }
                                            else
                                                canDo = false;
                                        }

                                        if (canDo)
                                            switch (Path.GetExtension(path: targetFile)?.ToLower())
                                            {
                                                case ".kml":
                                                    {
                                                        xml.GeositeXmlToKml(input: sourceFile, output: targetFile, extraDescription: description);
                                                        break;
                                                    }
                                                case ".xml":
                                                    {
                                                        xml.GeositeXmlToGeositeXml(input: sourceFile, output: targetFile,
                                                            extraDescription: description);
                                                        break;
                                                    }
                                                case ".gml":
                                                    {
                                                        xml.GeositeXmlToGml(input: sourceFile, output: targetFile, extraDescription: description);
                                                        break;
                                                    }
                                                case ".geojson":
                                                    {
                                                        xml.GeositeXmlToGeoJson(input: sourceFile, output: targetFile, extraDescription: description);
                                                        break;
                                                    }
                                                case ".shp":
                                                    {
                                                        var geositeXml = xml.GeositeXmlToGeositeXml(input: sourceFile, output: null, extraDescription: description);
                                                        xml.GeositeXmlToShp(geositeXml: geositeXml.Root, shapeFileName: targetFile);
                                                        break;
                                                    }
                                            }
                                    }
                                    break;
                                }
                            case ".geojson":
                                {
                                    using var geoJsonObject = new GeositeXml.GeositeXml(projectionX);
                                    var localI = i + 1;
                                    geoJsonObject.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                    {
                                        object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                                            ? sourceFiles.Length > 1
                                                ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                                                : thisEvent.Message
                                            : null;
                                        fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                                    };
                                    if (isDirectory)
                                        switch (Path.GetExtension(path: targetFile)?.ToLower())
                                        {
                                            case ".xml":
                                                {
                                                    geoJsonObject.GeoJsonToGeositeXml(
                                                        input: sourceFile,
                                                        output: targetFile,
                                                        treePath: ConsoleIO.FilePathToXPath(path: sourceFile)
                                                    );
                                                    break;
                                                }
                                            case ".shp":
                                                {
                                                    var geositeXmlStringBuilder = new StringBuilder();
                                                    geoJsonObject.GeoJsonToGeositeXml(
                                                        input: sourceFile,
                                                        output: geositeXmlStringBuilder,
                                                        treePath: ConsoleIO.FilePathToXPath(path: sourceFile)
                                                    );
                                                    var geositeXml = XElement.Parse(text: geositeXmlStringBuilder.ToString());
                                                    geoJsonObject.GeositeXmlToShp(geositeXml: geositeXml, shapeFileName: targetFile);
                                                    break;
                                                }
                                        }
                                    else
                                    {
                                        string treePathString = null;
                                        XElement description = null;
                                        var canDo = true;
                                        if (!_noPromptLayersBuilder)
                                        {
                                            var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: sourceFile).FullName);
                                            getTreeLayers.ShowDialog();
                                            if (getTreeLayers.Ok)
                                            {
                                                treePathString = getTreeLayers.TreePathString;
                                                description = getTreeLayers.Description;
                                                _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                            }
                                            else
                                                canDo = false;
                                        }
                                        else
                                            treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: sourceFile).FullName);
                                        if (canDo)
                                            switch (Path.GetExtension(path: targetFile)?.ToLower())
                                            {
                                                case ".xml":
                                                    {
                                                        geoJsonObject.GeoJsonToGeositeXml(
                                                            input: sourceFile,
                                                            output: targetFile,
                                                            treePath: treePathString,
                                                            extraDescription: description
                                                        );
                                                        break;
                                                    }
                                                case ".shp":
                                                    {
                                                        var geositeXmlStringBuilder = new StringBuilder();
                                                        geoJsonObject.GeoJsonToGeositeXml(
                                                            input: sourceFile,
                                                            output: geositeXmlStringBuilder,
                                                            treePath: treePathString,
                                                            extraDescription: description
                                                        );
                                                        var geositeXml =
                                                            XElement.Parse(text: geositeXmlStringBuilder.ToString());
                                                        geoJsonObject.GeositeXmlToShp(
                                                            geositeXml: geositeXml,
                                                            shapeFileName: targetFile
                                                        );
                                                        break;
                                                    }
                                            }
                                    }
                                    break;
                                }
                            default:
                                {
                                    using var mapgis = new MapGis.MapGisFile();
                                    var localI = i + 1;
                                    mapgis.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                    {
                                        object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                                            ? sourceFiles.Length > 1
                                                ? $"[{localI}/{sourceFiles.Length}] {thisEvent.Message}"
                                                : thisEvent.Message
                                            : null;
                                        fileBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                                    };
                                    mapgis.Open(mapgisFile: sourceFile, projection: projectionX);
                                    switch (Path.GetExtension(path: targetFile)?.ToLower())
                                    {
                                        case ".shp":
                                            {
                                                mapgis.Export(saveAs: targetFile, format: "shapefile");
                                                break;
                                            }
                                        case ".xml":
                                        case ".kml":
                                        case ".gml":
                                            {
                                                if (isDirectory)
                                                    mapgis.Export(
                                                        saveAs: targetFile,
                                                        format: Path.GetExtension(path: targetFile).ToLower()[1..],
                                                        treePath: ConsoleIO.FilePathToXPath(path: sourceFile)
                                                    );
                                                else
                                                {
                                                    string treePathString = null;
                                                    XElement description = null;
                                                    var canDo = true;
                                                    if (!_noPromptLayersBuilder)
                                                    {
                                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: sourceFile).FullName);
                                                        getTreeLayers.ShowDialog();
                                                        if (getTreeLayers.Ok)
                                                        {
                                                            treePathString = getTreeLayers.TreePathString;
                                                            description = getTreeLayers.Description;
                                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                                        }
                                                        else
                                                            canDo = false;
                                                    }
                                                    else
                                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: sourceFile).FullName);
                                                    if (canDo)
                                                        mapgis.Export(
                                                            saveAs: targetFile,
                                                            format: Path.GetExtension(path: targetFile).ToLower()[1..],
                                                            treePath: treePathString,
                                                            extraDescription: description
                                                        );
                                                }
                                                break;
                                            }
                                        case ".geojson":
                                            {
                                                mapgis.Export(saveAs: targetFile);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                    }
                }
                catch (Exception error)
                {
                    return error.Message;
                }
            }
            return null;
        }

        private void FileWorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var userState = (string)e.UserState;
            var progressPercentage = e.ProgressPercentage;
            var percent = statusProgress.Value = progressPercentage is >= 0 and <= 100 ? progressPercentage : 0;
            statusText.Text = userState;
            if (percent % 10 == 0)
                statusBar.Refresh();
        }

        private void FileWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusProgress.Visible = false;
            if (e.Error != null)
                MessageBox.Show(text: e.Error.Message, caption: @"Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            else if (e.Cancelled)
                statusText.Text = @"Suspended!";
            else if (e.Result != null)
                statusText.Text = (string)e.Result;
        }

        /// <summary>
        /// Add a line of text to the file load log list
        /// </summary>
        /// <param name="input">input string</param>
        public void FileLoadLogAdd(string input)
        {
            BeginInvoke(
                method: () =>
                {
                    FileLoadLog.Items.Add(item: input);
                    FileLoadLog.SelectedIndex = FileLoadLog.Items.Count - 1;
                    FileLoadLog.SelectedIndex = -1;
                }
            );
        }

        /// <summary>
        /// Add a line of text to the database running log list
        /// </summary>
        /// <param name="input">input string</param>
        public void DatabaseLogAdd(string input)
        {
            BeginInvoke(
                method: () =>
                {
                    DatabaseLog.Items.Add(item: input);
                    DatabaseLog.SelectedIndex = DatabaseLog.Items.Count - 1;
                    DatabaseLog.SelectedIndex = -1;
                }
            );
        }

        /// <summary>
        /// 窗体控件事件响应函数（暂支持：RadioButton、CheckBox、ComboBox、TextBox、TabControl）
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void FormEventChanged(object sender, EventArgs e = null)
        {
            BeginInvoke(
                method: () =>
                {
                    switch (sender)
                    {
                        case TextBox textBox:
                            RegEdit.Setkey(keyname: textBox.Name, defaultvalue: textBox.Text);
                            break;
                        case RadioButton radioButton:
                            RegEdit.Setkey(keyname: radioButton.Name, defaultvalue: $"{radioButton.Checked}");
                            break;
                        case CheckBox checkBox:
                            RegEdit.Setkey(keyname: checkBox.Name, defaultvalue: $"{checkBox.Checked}");
                            break;
                        case ComboBox comboBox:
                            RegEdit.Setkey(keyname: comboBox.Name, defaultvalue: comboBox.Text);
                            break;
                        case TabControl tabControl:
                            RegEdit.Setkey(keyname: tabControl.Name, defaultvalue: $"{tabControl.SelectedIndex}");
                            break;
                    }
                }
            );
        }

        private void GeositeServer_LinkChanged(object sender, EventArgs e)
        {
            GeositeServerLink.BackgroundImage = Properties.Resources.link;
            _clusterUser.status =
                deleteForest.Enabled =
                    dataGridPanel.Enabled =
                        PostgresRun.Enabled = false;
            statusText.Text = GetCopyright;
            FormEventChanged(sender: sender);
        }

        private void GeositeServerLink_Click(object sender, EventArgs e)
        {
            var serverUrl = GeositeServerUrl.Text.Trim();
            var serverUser = GeositeServerUser.Text.Trim();
            var serverPassword = GeositeServerPassword.Text.Trim();
            if (!string.IsNullOrWhiteSpace(value: serverUrl))
            {
                try
                {
                    if (new Ping().Send(hostNameOrAddress: new UriBuilder(uri: serverUrl).Host, timeout: 10000) is { Status: IPStatus.Success })
                    {
                        new Task(
                            action: () =>
                            {
                                string errorMessage = null;
                                _administrator = false;
                                try
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            _loading.Run();
                                            DatabaseLogAdd(input: statusText.Text = @"GeositeServer Connecting ...");
                                            GeositeServerLink.BackgroundImage = Properties.Resources.link;
                                            dataGridPanel.Enabled =
                                                CatalogTreeView.Enabled =
                                                    deleteForest.Enabled =
                                                        _clusterUser.status =
                                                            PostgresRun.Enabled = false;
                                        }
                                    );
                                    var userX = GetClusterUserX(
                                        serverUrl: serverUrl,
                                        serverUser: serverUser,
                                        serverPassword: serverPassword,
                                        clientVersion: Copyright.VersionAttribute,
                                        timeout: 0
                                    );
                                    if (userX.result != null)
                                    {
                                        var server = userX.result.Element(name: "Servers")?.Element(name: "Server");
                                        var geositeServerVersion = server?.Element(name: "Version")?.Value.Trim() ?? "0.0.0.0";
                                        DatabaseLogAdd(input: $"GeositeServer Version - {geositeServerVersion}");
                                        /*  样例如下：
                                            <User>
                                              <Servers>
                                                <Server>
                                                  <Host>localhost</Host>
                                                  <Version>7.2023.2.28</Version>
                                                  <Copyright>(C) 2019-2023 Geosite Development Team of CGS (R)</Copyright>
                                                  <Error></Error>
                                                  <Username>postgres</Username>
                                                  <Password>数据库连接密码</Password>
                                                  <Database Size="4347 MB" Postgresql_Version="15.1 (Debian 15.1-1.pgdg110+1)" Postgis_Version="3.3.2" Pgroonga_Version="2.4.5">geositeserver</Database>
                                                  <Other></Other>
                                                  <CommandTimeout>30</CommandTimeout>
                                                  <Port>5432</Port>
                                                  <Pooling>true</Pooling>
                                                  <LoadBalanceHosts>false</LoadBalanceHosts>
                                                  <TargetSessionAttributes>any</TargetSessionAttributes>
                                                </Server>
                                              </Servers>
                                              <Forest Root="Data" Administrator="True" MachineName="GEOSITESERVER" OSVersion="Microsoft Windows NT 10.0.17763.0" ProcessorCount="8">-1</Forest>
                                            </User>                                         
                                         */
                                        var host = server?.Element(name: "Host")?.Value.Trim();
                                        try
                                        {
                                            var postgresAddress =
                                                new Ping().Send(hostNameOrAddress: host!, timeout: 3000);
                                            if (postgresAddress is { Status: IPStatus.Success })
                                            {
                                                host = postgresAddress.Address.ToString();
                                                if (host == "::1")
                                                    host = "127.0.0.1";
                                            }
                                            else
                                                errorMessage = @"PostgreSQL host connection timeout.";
                                        }
                                        catch
                                        {
                                            errorMessage = @"PostgreSQL host connection failed.";
                                        }

                                        if (string.IsNullOrWhiteSpace(value: errorMessage))
                                        {
                                            try
                                            {
                                                var versionArray = Regex.Split(input: geositeServerVersion, pattern: @"[\.]+");
                                                var versionMain = long.Parse(s: versionArray[0]) * 1e8;
                                                var versionYear = long.Parse(s: versionArray[1]) * 1e4;
                                                var versionMonth = long.Parse(s: versionArray[2]) * 1e2;
                                                var versionDay = long.Parse(s: versionArray[3]);
                                                if (versionMain + versionYear + versionMonth + versionDay >= 720230101) // 7.2023.1.1
                                                {
                                                    if (!int.TryParse(s: server?.Element(name: "Port")?.Value.Trim(),
                                                            result: out var port))
                                                        port = 5432;
                                                    var databaseX = server?.Element(name: "Database");
                                                    var database = databaseX?.Value.Trim();
                                                    try
                                                    {
                                                        var postgresqlVersion = databaseX
                                                            ?.Attribute(name: "Postgresql_Version")?.Value;
                                                        if (!string.IsNullOrWhiteSpace(value: postgresqlVersion))
                                                            DatabaseLogAdd(
                                                                input: $"PostgreSQL Version - {postgresqlVersion}");
                                                        var postgisVersion = databaseX
                                                            ?.Attribute(name: "Postgis_Version")?.Value;
                                                        if (!string.IsNullOrWhiteSpace(value: postgisVersion))
                                                            DatabaseLogAdd(
                                                                input: $"PostGIS Version - {postgisVersion}");
                                                        var pgroongaVersion = databaseX
                                                            ?.Attribute(name: "Pgroonga_Version")?.Value;
                                                        if (!string.IsNullOrWhiteSpace(value: pgroongaVersion))
                                                            DatabaseLogAdd(
                                                                input: $"PGroonga Version - {pgroongaVersion}");
                                                        var databaseSize = databaseX?.Attribute(name: "Size")?.Value;
                                                        if (!string.IsNullOrWhiteSpace(value: databaseSize))
                                                            DatabaseLogAdd(input: $"Database Size - {databaseSize}");
                                                    }
                                                    catch
                                                    {
                                                        //
                                                    }

                                                    var username = server?.Element(name: "Username")?.Value.Trim();
                                                    var password = server?.Element(name: "Password")?.Value.Trim();
                                                    var forestX = userX.result.Element(name: "Forest");
                                                    if (!int.TryParse(s: forestX?.Value.Trim(), result: out var forest))
                                                        forest = -1;
                                                    if (!bool.TryParse(
                                                            value: forestX?.Attribute(name: "Administrator")?.Value
                                                                .Trim() ?? "false", result: out _administrator))
                                                        _administrator = false;
                                                    var rootName = forestX?.Attribute(name: "Root")?.Value ?? "Root";
                                                    var geositeServerLink =
                                                        PostgreSqlHelper.Connection(
                                                            host: host,
                                                            port: port,
                                                            database: database,
                                                            username: username,
                                                            password: password,
                                                            tables: "forest,tree,branch,leaf");
                                                    switch (geositeServerLink.flag)
                                                    {
                                                        case -1:
                                                        case -2:
                                                        case 2:
                                                            {
                                                                _clusterUser.status = false;
                                                                errorMessage = geositeServerLink.Message;
                                                                break;
                                                            }
                                                        case 1:
                                                            {
                                                                _clusterUser.status = false;
                                                                Invoke(
                                                                    method: () =>
                                                                    {
                                                                        statusProgress.Visible = true;
                                                                        statusProgress.Value = 0;
                                                                    }
                                                                );
                                                                var tablePartitions =
                                                                    Math.Min(96, int.Parse(s: forestX
                                                                        ?.Attribute(name: "ProcessorCount")
                                                                        ?.Value ?? "1"));
                                                                if (PostgreSqlHelper.NonQuery(
                                                                        cmd:
                                                                        $"CREATE DATABASE {database} WITH OWNER = {username};",
                                                                        pooling: false, postgres: true, timeout: 0) != null)
                                                                {
                                                                    if ((long)PostgreSqlHelper.Scalar(
                                                                            cmd:
                                                                            "SELECT count(*) FROM pg_available_extensions WHERE name = 'postgis';",
                                                                            timeout: 0) > 0)
                                                                    {
                                                                        Invoke(
                                                                            method: () =>
                                                                            {
                                                                                statusProgress.Value = 10;
                                                                                DatabaseLogAdd(input: statusText.Text =
                                                                                    @"Create PostGIS extension ...");
                                                                            }
                                                                        );
                                                                        PostgreSqlHelper.NonQuery(
                                                                            cmd: "CREATE EXTENSION postgis;",
                                                                            pooling: false, timeout: 0);
                                                                        if ((long)PostgreSqlHelper.Scalar(
                                                                                cmd:
                                                                                "SELECT count(*) FROM pg_available_extensions WHERE name = 'postgis_raster';") >
                                                                            0)
                                                                        {
                                                                            Invoke(
                                                                                method: () =>
                                                                                {
                                                                                    statusProgress.Value = 16;
                                                                                    DatabaseLogAdd(input: statusText.Text =
                                                                                        @"Create postgis_raster extension ...");
                                                                                }
                                                                            );
                                                                            PostgreSqlHelper.NonQuery(
                                                                                cmd: "CREATE EXTENSION postgis_raster;",
                                                                                pooling: false, timeout: 0);
                                                                            if ((long)PostgreSqlHelper.Scalar(
                                                                                    cmd:
                                                                                    "SELECT count(*) FROM pg_available_extensions WHERE name = 'intarray';",
                                                                                    timeout: 0) > 0)
                                                                            {
                                                                                Invoke(
                                                                                    method: () =>
                                                                                    {
                                                                                        statusProgress.Value = 22;
                                                                                        DatabaseLogAdd(
                                                                                            input: statusText.Text =
                                                                                                @"Create intarray extension ...");
                                                                                    }
                                                                                );
                                                                                PostgreSqlHelper.NonQuery(
                                                                                    cmd: "CREATE EXTENSION intarray;",
                                                                                    pooling: false, timeout: 0);
                                                                                if ((long)PostgreSqlHelper.Scalar(
                                                                                        cmd:
                                                                                        "SELECT count(*) FROM pg_available_extensions WHERE name = 'pgroonga';",
                                                                                        timeout: 0) > 0)
                                                                                {
                                                                                    Invoke(
                                                                                        method: () =>
                                                                                        {
                                                                                            statusProgress.Value = 28;
                                                                                            DatabaseLogAdd(
                                                                                                input: statusText.Text =
                                                                                                    @"Create pgroonga extension ...");
                                                                                        }
                                                                                    );
                                                                                    PostgreSqlHelper.NonQuery(
                                                                                        cmd: "CREATE EXTENSION pgroonga;",
                                                                                        pooling: false, timeout: 0);
                                                                                    Invoke(
                                                                                        method: () =>
                                                                                        {
                                                                                            statusProgress.Value = 34;
                                                                                            DatabaseLogAdd(
                                                                                                input: statusText.Text =
                                                                                                    @"Create forest table（forest）...");
                                                                                        }
                                                                                    );
                                                                                    if (PostgreSqlHelper.NonQuery(
                                                                                         cmd: "CREATE TABLE forest " +
                                                                                         "(" +
                                                                                         "id INTEGER, name TEXT, property JSONB, timestamp INTEGER[], status SmallInt DEFAULT 0" +
                                                                                         ",CONSTRAINT forest_pkey PRIMARY KEY (id)" +
                                                                                         ",CONSTRAINT forest_status_constraint CHECK (status >= 0 AND status <= 7)" +
                                                                                         ") PARTITION BY HASH (id);" +
                                                                                         "COMMENT ON TABLE forest IS '森林表，此表是本系统的第一张表，用于存放节点森林基本信息，每片森林（节点群）将由若干颗文档树（GeositeXml）构成';" +
                                                                                         "COMMENT ON COLUMN forest.id IS '森林序号标识码（通常由注册表[register.xml]中[forest]节的先后顺序决定，亦可通过接口函数赋值），充当主键（唯一性约束）且通常大于等于0，若设为负值，便不参与后续对等，需通过额外工具进行【增删改】操作';" +
                                                                                         "COMMENT ON COLUMN forest.name IS '森林简要名称';" +
                                                                                         "COMMENT ON COLUMN forest.property IS '森林属性描述信息，通常放置图标链接、服务文档等显式定制化信息';" +
                                                                                         "COMMENT ON COLUMN forest.timestamp IS '森林创建时间戳（由[年月日：yyyyMMdd,时分秒：HHmmss]二元整型数组编码构成）';" +
                                                                                         "COMMENT ON COLUMN forest.status IS '森林状态码（介于0～7之间：0=非持久化明数据（参与对等）无值或失败、1=非持久化明数据（参与对等）正常、2=非持久化暗数据（参与对等）失败、3=非持久化 暗数据（参与对等）正常、4=持久化明数据（不参与对等）失败、5=持久化明数据（不参与对等）正常、6=持久化暗数据（不参与对等）失败、7=持久化暗数据（不参与对等）正常）';",
                                                                                         timeout: 0) != null)
                                                                                    {
                                                                                        //status 森林状态码（介于0～7之间）约定如下：
                                                                                        //持久化位 暗数据位 完整性位  |  值：含义
                                                                                        //======== ======== ========  |  ======================================
                                                                                        //0        0        0         |  0=非持久化明数据（参与对等）无值或失败    
                                                                                        //0        0        1         |  1=非持久化明数据（参与对等）正常          
                                                                                        //0        1        0         |  2=非持久化暗数据（参与对等）失败          
                                                                                        //0        1        1         |  3=非持久化 暗数据（参与对等）正常          
                                                                                        //1        0        0         |  4=持久化明数据（不参与对等）失败          
                                                                                        //1        0        1         |  5=持久化明数据（不参与对等）正常          
                                                                                        //1        1        0         |  6=持久化暗数据（不参与对等）失败          
                                                                                        //1        1        1         |  7=持久化暗数据（不参与对等）正常 

                                                                                        for (var i = 0;
                                                                                         i < tablePartitions;
                                                                                         i++)
                                                                                            PostgreSqlHelper.NonQuery(
                                                                                                cmd:
                                                                                                $"CREATE TABLE forest_{i} PARTITION OF forest FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                timeout: 0);
                                                                                        if (PostgreSqlHelper.NonQuery(
                                                                                             cmd:
                                                                                             "CREATE INDEX forest_name ON forest USING BTREE (name);" +
                                                                                             "CREATE INDEX forest_name_FTS ON forest USING PGROONGA (name);" +
                                                                                             "CREATE INDEX forest_property ON forest USING GIN (property);" +
                                                                                             "CREATE INDEX forest_property_FTS ON forest USING PGROONGA (property);" +
                                                                                             "CREATE INDEX forest_timestamp_yyyymmdd ON forest USING BTREE ((timestamp[1]));" +
                                                                                             "CREATE INDEX forest_timestamp_hhmmss ON forest USING BTREE ((timestamp[2]));" +
                                                                                             "CREATE INDEX forest_status ON forest USING BTREE (status);",
                                                                                             timeout: 0) != null)
                                                                                        {
                                                                                            PostgreSqlHelper.NonQuery(
                                                                                                cmd:
                                                                                                "CREATE TABLE forest_relation " +
                                                                                                "(" +
                                                                                                "forest INTEGER, action JSONB, detail XML" +
                                                                                                ",CONSTRAINT forest_relation_pkey PRIMARY KEY (forest)" +
                                                                                                ",CONSTRAINT forest_relation_cascade FOREIGN KEY (forest) REFERENCES forest (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                ") PARTITION BY HASH (forest);" +
                                                                                                "COMMENT ON TABLE forest_relation IS '节点森林关系描述表';" +
                                                                                                "COMMENT ON COLUMN forest_relation.forest IS '节点森林序号标识码';" +
                                                                                                "COMMENT ON COLUMN forest_relation.action IS '节点森林事务活动容器';" +
                                                                                                "COMMENT ON COLUMN forest_relation.detail IS '节点森林关系描述容器';",
                                                                                                timeout: 0);
                                                                                            for (var i = 0;
                                                                                             i < tablePartitions;
                                                                                             i++)
                                                                                                PostgreSqlHelper.NonQuery(
                                                                                                    cmd:
                                                                                                    $"CREATE TABLE forest_relation_{i} PARTITION OF forest_relation FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                    timeout: 0);
                                                                                            PostgreSqlHelper.NonQuery(
                                                                                                cmd:
                                                                                                "CREATE INDEX forest_relation_action_FTS ON forest_relation USING PGROONGA (action);" +
                                                                                                "CREATE INDEX forest_relation_action ON forest_relation USING GIN (action);",
                                                                                                timeout: 0);
                                                                                            Invoke(
                                                                                                method: () =>
                                                                                                {
                                                                                                    statusProgress.Value =
                                                                                                        40;
                                                                                                    DatabaseLogAdd(
                                                                                                        input: statusText
                                                                                                                .Text =
                                                                                                            @"Create tree table（tree）...");
                                                                                                }
                                                                                            );
                                                                                            if (PostgreSqlHelper.NonQuery(
                                                                                                 cmd:
                                                                                                 "CREATE TABLE tree " +
                                                                                                 "(" +
                                                                                                 "forest INTEGER, sequence INTEGER, id INTEGER, name TEXT, property JSONB, uri TEXT, timestamp INTEGER[], type INTEGER[], status SmallInt DEFAULT 0" +
                                                                                                 ",CONSTRAINT tree_pkey PRIMARY KEY (id)" +
                                                                                                 ",CONSTRAINT tree_cascade FOREIGN KEY (forest) REFERENCES forest (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                 ") PARTITION BY HASH (id);" +
                                                                                                 "COMMENT ON TABLE tree IS '树根表，此表是本系统的第二张表，用于存放某片森林（节点群）中的若干颗文档树（GeositeXML）';" +
                                                                                                 "COMMENT ON COLUMN tree.forest IS '文档树所属节点森林标识码';" +
                                                                                                 "COMMENT ON COLUMN tree.sequence IS '文档树在节点森林中排列顺序号（由所在森林内的[GeositeXML]文档编号顺序决定）且大于等于0';" +
                                                                                                 "COMMENT ON COLUMN tree.id IS '文档树标识码（相当于每棵树的树根编号），充当主键（唯一性约束）且大于等于0';" +
                                                                                                 "COMMENT ON COLUMN tree.name IS '文档树根节点简要名称';" +
                                                                                                 "COMMENT ON COLUMN tree.property IS '文档树根节点属性描述信息，通常放置根节点辅助说明信息';" +
                                                                                                 "COMMENT ON COLUMN tree.uri IS '文档树数据来源（存放路径及文件名）';" +
                                                                                                 "COMMENT ON COLUMN tree.timestamp IS '文档树编码印章，采用[节点森林序号,文档树序号,年月日（yyyyMMdd）,时分秒（HHmmss）]四元整型数组编码方式';" +
                                                                                                 "COMMENT ON COLUMN tree.type IS '文档树要素类型码构成的数组（类型码约定：0：非空间数据【默认】、1：Point点、2：Line线、3：Polygon面、4：Image地理贴图、10000：Wms栅格金字塔瓦片服务类型[epsg:0 - 无投影瓦片]、10001：Wms瓦片服务类型[epsg:4326 - 地理坐标系瓦片]、10002：Wms栅格金字塔瓦片服务类型[epsg:3857 - 球体墨卡托瓦片]、11000：Wmts栅格金字塔瓦片类型[epsg:0 - 无投影瓦片]、11001：Wmts栅格金字塔瓦片类型[epsg:4326 - 地理坐标系瓦片]、11002：Wmts栅格金字塔瓦片类型[epsg:3857 - 球体墨卡托瓦片]、12000：WPS栅格平铺式瓦片类型[epsg:0 - 无投影瓦片]、12001：WPS栅格平铺式瓦片类型[epsg:4326 - 地理坐标系瓦片]、12002：WPS栅格平铺式瓦片类型[epsg:3857 - 球体墨卡托瓦片]）';" +
                                                                                                 "COMMENT ON COLUMN tree.status IS '文档树状态码（介于0～7之间），继承自[forest.status]';",
                                                                                                 timeout: 0) != null)
                                                                                            {
                                                                                                for (var i = 0;
                                                                                                 i < tablePartitions;
                                                                                                 i++)
                                                                                                    PostgreSqlHelper
                                                                                                        .NonQuery(
                                                                                                            cmd:
                                                                                                            $"CREATE TABLE tree_{i} PARTITION OF tree FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                            timeout: 0);
                                                                                                PostgreSqlHelper.NonQuery(
                                                                                                    cmd:
                                                                                                    "CREATE SEQUENCE tree_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1;",
                                                                                                    timeout: 0);
                                                                                                if (PostgreSqlHelper
                                                                                                     .NonQuery(
                                                                                                         cmd:
                                                                                                         "CREATE INDEX tree_forest_sequence ON tree USING BTREE (forest, sequence);" +
                                                                                                         "CREATE INDEX tree_name ON tree USING BTREE (name);" +
                                                                                                         "CREATE INDEX tree_name_FTS ON tree USING PGROONGA (name);" +
                                                                                                         "CREATE INDEX tree_property ON tree USING GIN (property);" +
                                                                                                         "CREATE INDEX tree_property_FTS ON tree USING PGROONGA (property);" +
                                                                                                         "CREATE INDEX tree_timestamp_forest ON tree USING BTREE ((timestamp[1]));" +
                                                                                                         "CREATE INDEX tree_timestamp_tree ON tree USING BTREE ((timestamp[2]));" +
                                                                                                         "CREATE INDEX tree_timestamp_yyyymmdd ON tree USING BTREE ((timestamp[3]));" +
                                                                                                         "CREATE INDEX tree_timestamp_hhmmss ON tree USING BTREE ((timestamp[4]));" +
                                                                                                         "CREATE INDEX tree_type ON tree USING GIST (type gist__int_ops);" +
                                                                                                         "CREATE INDEX tree_status ON tree USING BTREE (status);",
                                                                                                         timeout: 0) !=
                                                                                                 null)
                                                                                                {
                                                                                                    PostgreSqlHelper
                                                                                                        .NonQuery(
                                                                                                            cmd:
                                                                                                            "CREATE TABLE tree_relation " +
                                                                                                            "(" +
                                                                                                            "tree INTEGER, action JSONB, detail XML" +
                                                                                                            ",CONSTRAINT tree_relation_pkey PRIMARY KEY (tree)" +
                                                                                                            ",CONSTRAINT tree_relation_cascade FOREIGN KEY (tree) REFERENCES tree (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                            ") PARTITION BY HASH (tree);" +
                                                                                                            "COMMENT ON TABLE tree_relation IS '文档树关系描述表';" +
                                                                                                            "COMMENT ON COLUMN tree_relation.tree IS '文档树的标识码';" +
                                                                                                            "COMMENT ON COLUMN tree_relation.action IS '文档树事务活动容器';" +
                                                                                                            "COMMENT ON COLUMN tree_relation.detail IS '文档树关系描述容器';",
                                                                                                            timeout: 0);
                                                                                                    for (var i = 0;
                                                                                                     i < tablePartitions;
                                                                                                     i++)
                                                                                                        PostgreSqlHelper
                                                                                                            .NonQuery(
                                                                                                                cmd:
                                                                                                                $"CREATE TABLE tree_relation_{i} PARTITION OF tree_relation FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                timeout: 0);
                                                                                                    PostgreSqlHelper
                                                                                                        .NonQuery(
                                                                                                            cmd:
                                                                                                            "CREATE INDEX tree_relation_action_FTS ON tree_relation USING PGROONGA (action);" +
                                                                                                            "CREATE INDEX tree_relation_action ON tree_relation USING GIN (action);",
                                                                                                            timeout: 0);
                                                                                                    Invoke(
                                                                                                        method: () =>
                                                                                                        {
                                                                                                            statusProgress
                                                                                                                .Value = 46;
                                                                                                            DatabaseLogAdd(
                                                                                                                input:
                                                                                                                statusText
                                                                                                                        .Text =
                                                                                                                    @"Create branch table（branch）...");
                                                                                                        }
                                                                                                    );
                                                                                                    if (PostgreSqlHelper
                                                                                                         .NonQuery(
                                                                                                             cmd:
                                                                                                             "CREATE TABLE branch " +
                                                                                                             "(" +
                                                                                                             "tree INTEGER, level SmallInt, name TEXT, property JSONB, id INTEGER, parent INTEGER DEFAULT 0" +
                                                                                                             ",CONSTRAINT branch_pkey PRIMARY KEY (id)" +
                                                                                                             ",CONSTRAINT branch_cascade FOREIGN KEY (tree) REFERENCES tree (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                             ") PARTITION BY HASH (id);" +
                                                                                                             "COMMENT ON TABLE branch IS '枝干谱系表，此表是本系统第三张表，用于存放某棵树（GeositeXml文档）的枝干体系';" +
                                                                                                             "COMMENT ON COLUMN branch.tree IS '枝干隶属文档树的标识码';" +
                                                                                                             "COMMENT ON COLUMN branch.level IS '枝干所处分类级别：1是树干、2是树枝、3是树杈、...、n是树梢';" +
                                                                                                             "COMMENT ON COLUMN branch.name IS '枝干简要名称';" +
                                                                                                             "COMMENT ON COLUMN branch.property IS '枝干属性描述信息，通常放置分类别名、分类链接、时间戳等定制化信息';" +
                                                                                                             "COMMENT ON COLUMN branch.id IS '枝干标识码，充当主键（唯一性约束）';" +
                                                                                                             "COMMENT ON COLUMN branch.parent IS '枝干的父级标识码（约定树根的标识码为0）';",
                                                                                                             timeout:
                                                                                                             0) != null)
                                                                                                    {
                                                                                                        for (var i = 0;
                                                                                                         i <
                                                                                                         tablePartitions;
                                                                                                         i++)
                                                                                                            PostgreSqlHelper
                                                                                                                .NonQuery(
                                                                                                                    cmd:
                                                                                                                    $"CREATE TABLE branch_{i} PARTITION OF branch FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                    timeout:
                                                                                                                    0);
                                                                                                        PostgreSqlHelper
                                                                                                            .NonQuery(
                                                                                                                cmd:
                                                                                                                "CREATE SEQUENCE branch_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1;",
                                                                                                                timeout: 0);
                                                                                                        if (PostgreSqlHelper
                                                                                                             .NonQuery(
                                                                                                                 cmd:
                                                                                                                 "CREATE INDEX branch_tree ON branch USING BTREE (tree);" +
                                                                                                                 "CREATE INDEX branch_level_name_parent ON branch USING BTREE (level, name, parent);" +
                                                                                                                 "CREATE INDEX branch_name ON branch USING BTREE (name);" +
                                                                                                                 "CREATE INDEX branch_name_FTS ON branch USING PGROONGA (name);" +
                                                                                                                 "CREATE INDEX branch_property_FTS ON branch USING PGROONGA (property);" +
                                                                                                                 "CREATE INDEX branch_property ON branch USING GIN (property);",
                                                                                                                 timeout
                                                                                                                 : 0) !=
                                                                                                         null)
                                                                                                        {
                                                                                                            PostgreSqlHelper
                                                                                                                .NonQuery(
                                                                                                                    cmd:
                                                                                                                    "CREATE TABLE branch_relation " +
                                                                                                                    "(" +
                                                                                                                    "branch INTEGER, action JSONB, detail XML" +
                                                                                                                    ",CONSTRAINT branch_relation_pkey PRIMARY KEY (branch)" +
                                                                                                                    ",CONSTRAINT branch_relation_cascade FOREIGN KEY (branch) REFERENCES branch (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                    ") PARTITION BY HASH (branch);" +
                                                                                                                    "COMMENT ON TABLE branch_relation IS '枝干关系描述表';" +
                                                                                                                    "COMMENT ON COLUMN branch_relation.branch IS '枝干标识码';" +
                                                                                                                    "COMMENT ON COLUMN branch_relation.action IS '枝干事务活动容器';" +
                                                                                                                    "COMMENT ON COLUMN branch_relation.detail IS '枝干关系描述容器';",
                                                                                                                    timeout:
                                                                                                                    0);
                                                                                                            for (var i = 0;
                                                                                                             i <
                                                                                                             tablePartitions;
                                                                                                             i++)
                                                                                                                PostgreSqlHelper
                                                                                                                    .NonQuery(
                                                                                                                        cmd:
                                                                                                                        $"CREATE TABLE branch_relation_{i} PARTITION OF branch_relation FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                        timeout
                                                                                                                        : 0);
                                                                                                            PostgreSqlHelper
                                                                                                                .NonQuery(
                                                                                                                    cmd:
                                                                                                                    "CREATE INDEX branch_relation_action_FTS ON branch_relation USING PGROONGA (action);" +
                                                                                                                    "CREATE INDEX branch_relation_action ON branch_relation USING GIN (action);",
                                                                                                                    timeout:
                                                                                                                    0);
                                                                                                            Invoke(
                                                                                                                method:
                                                                                                                () =>
                                                                                                                {
                                                                                                                    statusProgress
                                                                                                                            .Value =
                                                                                                                        52;
                                                                                                                    DatabaseLogAdd(
                                                                                                                        input
                                                                                                                        : statusText
                                                                                                                                .Text =
                                                                                                                            @"Create leaf table（leaf）...");
                                                                                                                }
                                                                                                            );
                                                                                                            if
                                                                                                                (PostgreSqlHelper
                                                                                                                     .NonQuery(
                                                                                                                         cmd
                                                                                                                         : "CREATE TABLE leaf " +
                                                                                                                         "(" +
                                                                                                                         "branch INTEGER, id BigInt, rank SmallInt DEFAULT -1, type INT DEFAULT 0, name TEXT, property INTEGER, timestamp INT[], frequency BigInt DEFAULT 0" +
                                                                                                                         ",CONSTRAINT leaf_pkey PRIMARY KEY (id)" +
                                                                                                                         ",CONSTRAINT leaf_cascade FOREIGN KEY (branch) REFERENCES branch (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                         ") PARTITION BY HASH (id);" +
                                                                                                                         "COMMENT ON TABLE leaf IS '叶子表，此表是本系统第四表，用于存放某个树梢挂接的若干叶子（实体要素）的摘要信息';" +
                                                                                                                         "COMMENT ON COLUMN leaf.branch IS '叶子要素隶属树梢（父级枝干）标识码';" +
                                                                                                                         "COMMENT ON COLUMN leaf.id IS '叶子要素标识码，充当主键（唯一性约束）';" +
                                                                                                                         "COMMENT ON COLUMN leaf.rank IS '叶子要素访问级别或权限序号，通常用于充当交互访问层的约束条件（比如：-1=不限制；0～n=逐级提升访问权限）';" +
                                                                                                                         "COMMENT ON COLUMN leaf.type IS '叶子要素类别码（0：非空间数据【默认】、1：Point点、2：Line线、3：Polygon面、4：Image地理贴图、10000：Wmts栅格金字塔瓦片服务类型[epsg:0 - 无投影瓦片]、10001：Wmts瓦片服务类型[epsg:4326 - 地理坐标系瓦片]、10002：Wmts栅格金字塔瓦片服务类型[epsg:3857 - 球体墨卡托瓦片]、11000：Tile栅格金字塔瓦片类型[epsg:0 - 无投影瓦片]、11001：Tile栅格金字塔瓦片类型[epsg:4326 - 地理坐标系瓦片]、11002：Tile栅格金字塔瓦片类型[epsg:3857 - 球体墨卡托瓦片]、12000：Tile栅格平铺式瓦片类型[epsg:0 - 无投影瓦片]、12001：Tile栅格平铺式瓦片类型[epsg:4326 - 地理坐标系瓦片]、12002：Tile栅格平铺式瓦片类型[epsg:3857 - 球体墨卡托瓦片]）';" +
                                                                                                                         "COMMENT ON COLUMN leaf.name IS '叶子要素名称';" +
                                                                                                                         "COMMENT ON COLUMN leaf.property IS '叶子要素属性架构哈希值';" +
                                                                                                                         "COMMENT ON COLUMN leaf.timestamp IS '叶子要素创建时间戳（由[年月日：yyyyMMdd,时分秒：HHmmss]二元整型数组编码构成）';" +
                                                                                                                         "COMMENT ON COLUMN leaf.frequency IS '叶子要素访问频度';",
                                                                                                                         timeout
                                                                                                                         : 0) !=
                                                                                                                 null)
                                                                                                            {
                                                                                                                for (var i =
                                                                                                                     0;
                                                                                                                 i <
                                                                                                                 tablePartitions;
                                                                                                                 i++)
                                                                                                                    PostgreSqlHelper
                                                                                                                        .NonQuery(
                                                                                                                            cmd
                                                                                                                            : $"CREATE TABLE leaf_{i} PARTITION OF leaf FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                            timeout
                                                                                                                            : 0);
                                                                                                                PostgreSqlHelper
                                                                                                                    .NonQuery(
                                                                                                                        cmd:
                                                                                                                        "CREATE SEQUENCE leaf_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 9223372036854775807 START 1 CACHE 1;",
                                                                                                                        timeout
                                                                                                                        : 0);
                                                                                                                if
                                                                                                                    (PostgreSqlHelper
                                                                                                                         .NonQuery(
                                                                                                                             cmd
                                                                                                                             :
                                                                                                                             "CREATE INDEX leaf_branch ON leaf USING BTREE (branch);" +
                                                                                                                             "CREATE INDEX leaf_rank ON leaf USING BTREE (rank);" +
                                                                                                                             "CREATE INDEX leaf_type ON leaf USING BTREE (type);" +
                                                                                                                             "CREATE INDEX leaf_name ON leaf USING BTREE (name);" +
                                                                                                                             "CREATE INDEX leaf_name_FTS ON leaf USING PGROONGA (name);" +
                                                                                                                             "CREATE INDEX leaf_property ON leaf USING BTREE (property);" +
                                                                                                                             "CREATE INDEX leaf_timestamp_yyyymmdd ON leaf USING BTREE ((timestamp[1]));" +
                                                                                                                             "CREATE INDEX leaf_timestamp_hhmmss ON leaf USING BTREE ((timestamp[2]));" +
                                                                                                                             "CREATE INDEX leaf_frequency_id ON leaf USING BTREE (frequency ASC NULLS LAST, id ASC NULLS LAST);",
                                                                                                                             timeout
                                                                                                                             : 0) !=
                                                                                                                     null)
                                                                                                                {
                                                                                                                    PostgreSqlHelper
                                                                                                                        .NonQuery(
                                                                                                                            cmd
                                                                                                                            :
                                                                                                                            "CREATE TABLE leaf_relation " +
                                                                                                                            "(" +
                                                                                                                            "leaf BigInt, action JSONB, detail XML" +
                                                                                                                            ",CONSTRAINT leaf_relation_pkey PRIMARY KEY (leaf)" +
                                                                                                                            ",CONSTRAINT leaf_relation_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                            ") PARTITION BY HASH (leaf);" +
                                                                                                                            "COMMENT ON TABLE leaf_relation IS '叶子关系描述表';" +
                                                                                                                            "COMMENT ON COLUMN leaf_relation.leaf IS '叶子要素标识码';" +
                                                                                                                            "COMMENT ON COLUMN leaf_relation.action IS '叶子事务活动容器';" +
                                                                                                                            "COMMENT ON COLUMN leaf_relation.detail IS '叶子关系描述容器';",
                                                                                                                            timeout
                                                                                                                            : 0);
                                                                                                                    for
                                                                                                                        (var
                                                                                                                         i =
                                                                                                                             0;
                                                                                                                         i <
                                                                                                                         tablePartitions;
                                                                                                                         i++)
                                                                                                                        PostgreSqlHelper
                                                                                                                            .NonQuery(
                                                                                                                                cmd
                                                                                                                                : $"CREATE TABLE leaf_relation_{i} PARTITION OF leaf_relation FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                timeout
                                                                                                                                : 0);
                                                                                                                    PostgreSqlHelper
                                                                                                                        .NonQuery(
                                                                                                                            cmd
                                                                                                                            :
                                                                                                                            "CREATE INDEX leaf_relation_action_FTS ON leaf_relation USING PGROONGA (action);" +
                                                                                                                            "CREATE INDEX leaf_relation_action ON leaf_relation USING GIN (action);",
                                                                                                                            timeout
                                                                                                                            : 0);
                                                                                                                    Invoke(
                                                                                                                        method
                                                                                                                        :
                                                                                                                        () =>
                                                                                                                        {
                                                                                                                            statusProgress
                                                                                                                                    .Value =
                                                                                                                                58;
                                                                                                                            DatabaseLogAdd(
                                                                                                                                input
                                                                                                                                : statusText
                                                                                                                                        .Text =
                                                                                                                                    @"Create leaf table（leaf_description）...");
                                                                                                                        }
                                                                                                                    );
                                                                                                                    if
                                                                                                                        (PostgreSqlHelper
                                                                                                                             .NonQuery(
                                                                                                                                 cmd
                                                                                                                                 :
                                                                                                                                 "CREATE TABLE leaf_description " +
                                                                                                                                 "(" +
                                                                                                                                 "leaf bigint, level SmallInt, sequence SmallInt, parent SmallInt, name TEXT, attribute JSONB, flag BOOLEAN DEFAULT false, type SmallInt DEFAULT 0, content Text, numericvalue Numeric" +
                                                                                                                                 ",CONSTRAINT leaf_description_pkey PRIMARY KEY (leaf, level, sequence, parent)" +
                                                                                                                                 ",CONSTRAINT leaf_description_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                 ") PARTITION BY HASH (leaf, level, sequence, parent);" +
                                                                                                                                 "COMMENT ON TABLE leaf_description IS '叶子要素表（leaf）的属性描述子表';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.leaf IS '叶子要素的标识码';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.level IS '字段（键）的嵌套层级';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.sequence IS '字段（键）的同级序号';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.parent IS '字段所属父级层级的排列序号';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.name IS '字段（键）的名称';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.attribute IS '字段（键）的属性，由若干扁平化键值对（KVP）构成';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.flag IS '字段（键）的逻辑标识（false：此键无值；true：此键有值）';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.type IS '字段（值）的数据类型码，目前支持：-1【分类型字段】、0【string（null）】、1【integer】、2【decimal】、3【hybrid】、4【boolean】';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.content IS '字段（值）的全文内容，以便实施全文检索以及自然语言处理';" +
                                                                                                                                 "COMMENT ON COLUMN leaf_description.numericvalue IS '字段（值）的数值型（1【integer】、2【decimal】、3【hybrid】、4【boolean】）容器，以便支持超大值域聚合计算';",
                                                                                                                                 timeout
                                                                                                                                 : 0) !=
                                                                                                                         null)
                                                                                                                    {
                                                                                                                        for
                                                                                                                            (var
                                                                                                                             i =
                                                                                                                                 0;
                                                                                                                             i <
                                                                                                                             tablePartitions;
                                                                                                                             i++)
                                                                                                                            PostgreSqlHelper
                                                                                                                                .NonQuery(
                                                                                                                                    cmd
                                                                                                                                    : $"CREATE TABLE leaf_description_{i} PARTITION OF leaf_description FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                    timeout
                                                                                                                                    : 0);
                                                                                                                        if
                                                                                                                            (PostgreSqlHelper
                                                                                                                                 .NonQuery(
                                                                                                                                     cmd
                                                                                                                                     : "CREATE INDEX leaf_description_name ON leaf_description USING BTREE (name);" +
                                                                                                                                     "CREATE INDEX leaf_description_name_FTS ON leaf_description USING PGROONGA (name);" +
                                                                                                                                     "CREATE INDEX leaf_description_flag ON leaf_description USING BTREE (flag);" +
                                                                                                                                     "CREATE INDEX leaf_description_type ON leaf_description USING BTREE (type);" +
                                                                                                                                     "CREATE INDEX leaf_description_content ON leaf_description USING PGROONGA (content);" +
                                                                                                                                     "CREATE INDEX leaf_description_numericvalue ON leaf_description USING BTREE (numericvalue);",
                                                                                                                                     timeout
                                                                                                                                     : 0) !=
                                                                                                                             null)
                                                                                                                        {
                                                                                                                            Invoke(
                                                                                                                                method
                                                                                                                                :
                                                                                                                                () =>
                                                                                                                                {
                                                                                                                                    statusProgress
                                                                                                                                            .Value =
                                                                                                                                        64;
                                                                                                                                    DatabaseLogAdd(
                                                                                                                                        input
                                                                                                                                        : statusText
                                                                                                                                                .Text =
                                                                                                                                            @"Create leaf table（leaf_style）...");
                                                                                                                                }
                                                                                                                            );
                                                                                                                            if
                                                                                                                                (PostgreSqlHelper
                                                                                                                                     .NonQuery(
                                                                                                                                         cmd
                                                                                                                                         : "CREATE TABLE leaf_style " +
                                                                                                                                         "(" +
                                                                                                                                         "leaf BigInt, style JSONB" +
                                                                                                                                         ",CONSTRAINT leaf_style_pkey PRIMARY KEY (leaf)" +
                                                                                                                                         ",CONSTRAINT leaf_style_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                         ") PARTITION BY HASH (leaf);" +
                                                                                                                                         "COMMENT ON TABLE leaf_style IS '叶子要素表（leaf）的样式子表';" +
                                                                                                                                         "COMMENT ON COLUMN leaf_style.leaf IS '叶子要素的标识码';" +
                                                                                                                                         "COMMENT ON COLUMN leaf_style.style IS '叶子要素可视化样式信息，由若干键值对（KVP）构成';",
                                                                                                                                         timeout
                                                                                                                                         : 0) !=
                                                                                                                                 null)
                                                                                                                            {
                                                                                                                                for
                                                                                                                                    (var
                                                                                                                                     i =
                                                                                                                                         0;
                                                                                                                                     i <
                                                                                                                                     tablePartitions;
                                                                                                                                     i++)
                                                                                                                                    PostgreSqlHelper
                                                                                                                                        .NonQuery(
                                                                                                                                            cmd
                                                                                                                                            : $"CREATE TABLE leaf_style_{i} PARTITION OF leaf_style FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                            timeout
                                                                                                                                            : 0);
                                                                                                                                if
                                                                                                                                    (PostgreSqlHelper
                                                                                                                                         .NonQuery(
                                                                                                                                             cmd
                                                                                                                                             : "CREATE INDEX leaf_style_style_FTS ON leaf_style USING PGROONGA (style);" +
                                                                                                                                             "CREATE INDEX leaf_style_style ON leaf_style USING GIN (style);",
                                                                                                                                             timeout
                                                                                                                                             : 0) !=
                                                                                                                                     null)
                                                                                                                                {
                                                                                                                                    Invoke(
                                                                                                                                        method
                                                                                                                                        : () =>
                                                                                                                                        {
                                                                                                                                            statusProgress
                                                                                                                                                    .Value =
                                                                                                                                                70;
                                                                                                                                            DatabaseLogAdd(
                                                                                                                                                input
                                                                                                                                                : statusText
                                                                                                                                                        .Text =
                                                                                                                                                    @"Create leaf table（leaf_geometry）...");
                                                                                                                                        }
                                                                                                                                    );
                                                                                                                                    if
                                                                                                                                        (PostgreSqlHelper
                                                                                                                                             .NonQuery(
                                                                                                                                                 cmd
                                                                                                                                                 : "CREATE TABLE leaf_geometry " +
                                                                                                                                                 "(" +
                                                                                                                                                 "leaf BigInt, coordinate GEOMETRY, boundary GEOMETRY, centroid GEOMETRY" +
                                                                                                                                                 ",CONSTRAINT leaf_geometry_pkey PRIMARY KEY (leaf)" +
                                                                                                                                                 ",CONSTRAINT leaf_geometry_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                                 ") PARTITION BY HASH (leaf);" +
                                                                                                                                                 "COMMENT ON TABLE leaf_geometry IS '叶子要素表（leaf）的几何坐标子表';" +
                                                                                                                                                 "COMMENT ON COLUMN leaf_geometry.leaf IS '叶子要素的标识码';" +
                                                                                                                                                 "COMMENT ON COLUMN leaf_geometry.coordinate IS '叶子要素几何坐标（【EPSG:4326】）';" +
                                                                                                                                                 "COMMENT ON COLUMN leaf_geometry.boundary IS '叶子要素几何边框（【EPSG:4326】）';" +
                                                                                                                                                 "COMMENT ON COLUMN leaf_geometry.centroid IS '叶子要素几何内点（通常用于几何瘦身、标注锚点等场景）';",
                                                                                                                                                 timeout
                                                                                                                                                 : 0) !=
                                                                                                                                         null)
                                                                                                                                    {
                                                                                                                                        for
                                                                                                                                            (var
                                                                                                                                             i =
                                                                                                                                                 0;
                                                                                                                                             i <
                                                                                                                                             tablePartitions;
                                                                                                                                             i++)
                                                                                                                                            PostgreSqlHelper
                                                                                                                                                .NonQuery(
                                                                                                                                                    cmd
                                                                                                                                                    : $"CREATE TABLE leaf_geometry_{i} PARTITION OF leaf_geometry FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                                    timeout
                                                                                                                                                    : 0);
                                                                                                                                        if
                                                                                                                                            (PostgreSqlHelper
                                                                                                                                                 .NonQuery(
                                                                                                                                                     cmd
                                                                                                                                                     : "CREATE INDEX leaf_geometry_coordinate ON leaf_geometry USING GIST (coordinate);" +
                                                                                                                                                     "CREATE INDEX leaf_geometry_boundary ON leaf_geometry USING GIST (boundary);" +
                                                                                                                                                     "CREATE INDEX leaf_geometry_centroid ON leaf_geometry USING GIST (centroid);",
                                                                                                                                                     timeout
                                                                                                                                                     : 0) !=
                                                                                                                                             null)
                                                                                                                                        {
                                                                                                                                            Invoke(
                                                                                                                                                method
                                                                                                                                                : () =>
                                                                                                                                                {
                                                                                                                                                    statusProgress
                                                                                                                                                            .Value =
                                                                                                                                                        76;
                                                                                                                                                    DatabaseLogAdd(
                                                                                                                                                        input
                                                                                                                                                        : statusText
                                                                                                                                                                .Text =
                                                                                                                                                            @"Create leaf table（leaf_tile）...");
                                                                                                                                                }
                                                                                                                                            );
                                                                                                                                            if
                                                                                                                                                (PostgreSqlHelper
                                                                                                                                                     .NonQuery(
                                                                                                                                                         cmd
                                                                                                                                                         : "CREATE TABLE leaf_tile " +
                                                                                                                                                         "(" +
                                                                                                                                                         "leaf BigInt, z INTEGER, x INTEGER, y INTEGER, tile RASTER, boundary geometry" +
                                                                                                                                                         ",CONSTRAINT leaf_tile_pkey PRIMARY KEY (leaf, z, x, y)" +
                                                                                                                                                         ",CONSTRAINT leaf_tile_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                                         ") PARTITION BY HASH (leaf, z, x, y);" +
                                                                                                                                                         "COMMENT ON TABLE leaf_tile IS '叶子要素表（leaf）的栅格瓦片子表，支持【四叉树金字塔式瓦片】和【平铺式地图瓦片】两种类型，每类瓦片的元数据信息需在叶子属性子表中的type进行表述';" +
                                                                                                                                                         "COMMENT ON COLUMN leaf_tile.leaf IS '叶子要素的标识码';" +
                                                                                                                                                         "COMMENT ON COLUMN leaf_tile.z IS '叶子瓦片缩放级（注：平铺式瓦片类型的z值强制为【-1】，四叉树金字塔式瓦片类型的z值通常介于【0～24】之间）';" +
                                                                                                                                                         "COMMENT ON COLUMN leaf_tile.x IS '叶子瓦片横向坐标编码';" +
                                                                                                                                                         "COMMENT ON COLUMN leaf_tile.y IS '叶子瓦片纵向坐标编码';" +
                                                                                                                                                         "COMMENT ON COLUMN leaf_tile.tile IS '叶子瓦片栅格影像（RASTER类型-WKB格式，目前支持【EPSG:4326】、【EPSG:3857】、【EPSG:0】）';" +
                                                                                                                                                         "COMMENT ON COLUMN leaf_tile.boundary IS '叶子瓦片几何边框（【EPSG:4326】）';",
                                                                                                                                                         timeout
                                                                                                                                                         : 0) !=
                                                                                                                                                 null)
                                                                                                                                            {
                                                                                                                                                for
                                                                                                                                                    (var
                                                                                                                                                     i =
                                                                                                                                                         0;
                                                                                                                                                     i <
                                                                                                                                                     tablePartitions;
                                                                                                                                                     i++)
                                                                                                                                                    PostgreSqlHelper
                                                                                                                                                        .NonQuery(
                                                                                                                                                            cmd
                                                                                                                                                            : $"CREATE TABLE leaf_tile_{i} PARTITION OF leaf_tile FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                                            timeout
                                                                                                                                                            : 0);
                                                                                                                                                if
                                                                                                                                                    (PostgreSqlHelper
                                                                                                                                                         .NonQuery(
                                                                                                                                                             cmd
                                                                                                                                                             : "CREATE INDEX leaf_tile_tile ON leaf_tile USING GIST (st_convexhull(tile));"
                                                                                                                                                             + "CREATE INDEX leaf_tile_boundary ON leaf_tile USING gist(boundary);"
                                                                                                                                                             + "CREATE INDEX leaf_tile_leaf_z ON leaf_tile USING btree (leaf ASC NULLS LAST, z DESC NULLS LAST);",
                                                                                                                                                             timeout
                                                                                                                                                             : 0) !=
                                                                                                                                                     null)
                                                                                                                                                {
                                                                                                                                                    Invoke(
                                                                                                                                                        method
                                                                                                                                                        : () =>
                                                                                                                                                        {
                                                                                                                                                            statusProgress
                                                                                                                                                                    .Value =
                                                                                                                                                                82;
                                                                                                                                                            DatabaseLogAdd(
                                                                                                                                                                input
                                                                                                                                                                : statusText
                                                                                                                                                                        .Text =
                                                                                                                                                                    @"Create leaf table（leaf_wms）...");
                                                                                                                                                        }
                                                                                                                                                    );
                                                                                                                                                    if
                                                                                                                                                        (PostgreSqlHelper
                                                                                                                                                             .NonQuery(
                                                                                                                                                                 cmd
                                                                                                                                                                 : "CREATE TABLE leaf_wms " +
                                                                                                                                                                 "(" +
                                                                                                                                                                 "leaf BigInt, wms TEXT, boundary geometry" +
                                                                                                                                                                 ",CONSTRAINT leaf_wms_pkey PRIMARY KEY (leaf)" +
                                                                                                                                                                 ",CONSTRAINT leaf_wms_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                                                 ") PARTITION BY HASH (leaf);" +
                                                                                                                                                                 "COMMENT ON TABLE leaf_wms IS '叶子要素表（leaf）的瓦片服务子表，元数据信息需在叶子属性表中的type中进行表述';" +
                                                                                                                                                                 "COMMENT ON COLUMN leaf_wms.leaf IS '叶子要素的标识码';" +
                                                                                                                                                                 "COMMENT ON COLUMN leaf_wms.wms IS '叶子要素服务地址模板，暂支持【OGC】、【BingMap】、【DeepZoom】和【ESRI】瓦片编码类型';" +
                                                                                                                                                                 "COMMENT ON COLUMN leaf_wms.boundary IS '叶子要素几何边框（EPSG:4326）';",
                                                                                                                                                                 timeout
                                                                                                                                                                 : 0) !=
                                                                                                                                                         null)
                                                                                                                                                    {
                                                                                                                                                        for
                                                                                                                                                            (var
                                                                                                                                                             i =
                                                                                                                                                                 0;
                                                                                                                                                             i <
                                                                                                                                                             tablePartitions;
                                                                                                                                                             i++)
                                                                                                                                                            PostgreSqlHelper
                                                                                                                                                                .NonQuery(
                                                                                                                                                                    cmd
                                                                                                                                                                    : $"CREATE TABLE leaf_wms_{i} PARTITION OF leaf_wms FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                                                    timeout
                                                                                                                                                                    : 0);
                                                                                                                                                        if
                                                                                                                                                            (PostgreSqlHelper
                                                                                                                                                                 .NonQuery(
                                                                                                                                                                     cmd
                                                                                                                                                                     : "CREATE INDEX leaf_wms_boundary ON leaf_wms USING gist(boundary);",
                                                                                                                                                                     timeout
                                                                                                                                                                     : 0) !=
                                                                                                                                                             null)
                                                                                                                                                        {
                                                                                                                                                            Invoke(
                                                                                                                                                                method
                                                                                                                                                                : () =>
                                                                                                                                                                {
                                                                                                                                                                    statusProgress
                                                                                                                                                                            .Value =
                                                                                                                                                                        88;
                                                                                                                                                                    DatabaseLogAdd(
                                                                                                                                                                        input
                                                                                                                                                                        : statusText
                                                                                                                                                                                .Text =
                                                                                                                                                                            @"Create leaf table（leaf_hits）...");
                                                                                                                                                                }
                                                                                                                                                            );
                                                                                                                                                            if
                                                                                                                                                                (PostgreSqlHelper
                                                                                                                                                                     .NonQuery(
                                                                                                                                                                         cmd
                                                                                                                                                                         : "CREATE TABLE leaf_hits " +
                                                                                                                                                                         "(" +
                                                                                                                                                                         "leaf BigInt, hits BigInt DEFAULT 0" +
                                                                                                                                                                         ",CONSTRAINT leaf_hits_pkey PRIMARY KEY (leaf)" +
                                                                                                                                                                         ",CONSTRAINT leaf_hits_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                                                         ") PARTITION BY HASH (leaf);" +
                                                                                                                                                                         "COMMENT ON TABLE leaf_hits IS '叶子要素表（leaf）的搜索命中率子表';" +
                                                                                                                                                                         "COMMENT ON COLUMN leaf_hits.leaf IS '叶子要素的标识码';" +
                                                                                                                                                                         "COMMENT ON COLUMN leaf_hits.hits IS '叶子要素的命中次数';",
                                                                                                                                                                         timeout
                                                                                                                                                                         : 0) !=
                                                                                                                                                                 null)
                                                                                                                                                            {
                                                                                                                                                                for
                                                                                                                                                                    (var
                                                                                                                                                                     i =
                                                                                                                                                                         0;
                                                                                                                                                                     i <
                                                                                                                                                                     tablePartitions;
                                                                                                                                                                     i++)
                                                                                                                                                                    PostgreSqlHelper
                                                                                                                                                                        .NonQuery(
                                                                                                                                                                            cmd
                                                                                                                                                                            : $"CREATE TABLE leaf_hits_{i} PARTITION OF leaf_hits FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                                                            timeout
                                                                                                                                                                            : 0);
                                                                                                                                                                Invoke(
                                                                                                                                                                    method
                                                                                                                                                                    : () =>
                                                                                                                                                                    {
                                                                                                                                                                        statusProgress
                                                                                                                                                                                .Value =
                                                                                                                                                                            94;
                                                                                                                                                                        DatabaseLogAdd(
                                                                                                                                                                            input
                                                                                                                                                                            : statusText
                                                                                                                                                                                    .Text =
                                                                                                                                                                                @"Create the temporal sub table of leaf table（leaf_temporal）...");
                                                                                                                                                                    }
                                                                                                                                                                );
                                                                                                                                                                if
                                                                                                                                                                    (PostgreSqlHelper
                                                                                                                                                                         .NonQuery(
                                                                                                                                                                             cmd
                                                                                                                                                                             : "CREATE TABLE leaf_temporal " +
                                                                                                                                                                             "(" +
                                                                                                                                                                             "leaf BigInt, birth BigInt[], death BigInt[]" +
                                                                                                                                                                             ",CONSTRAINT leaf_temporal_pkey PRIMARY KEY (leaf)" +
                                                                                                                                                                             ",CONSTRAINT leaf_temporal_cascade FOREIGN KEY (leaf) REFERENCES leaf (id) MATCH SIMPLE ON DELETE CASCADE NOT VALID" +
                                                                                                                                                                             ") PARTITION BY HASH (leaf);" +
                                                                                                                                                                             "COMMENT ON TABLE leaf_temporal IS '叶子要素表（leaf）的现世（生命期）子表';" +
                                                                                                                                                                             "COMMENT ON COLUMN leaf_temporal.leaf IS '叶子要素的标识码';" +
                                                                                                                                                                             "COMMENT ON COLUMN leaf_temporal.birth IS '叶子要素生命期的起始时间（由【年月日、时分秒】两个整型数据成员构成）';" +
                                                                                                                                                                             "COMMENT ON COLUMN leaf_temporal.death IS '叶子要素生命期的结束时间（由【年月日、时分秒】两个整型数据成员构成）';",
                                                                                                                                                                             timeout
                                                                                                                                                                             : 0) !=
                                                                                                                                                                     null)
                                                                                                                                                                {
                                                                                                                                                                    for
                                                                                                                                                                        (var
                                                                                                                                                                         i =
                                                                                                                                                                             0;
                                                                                                                                                                         i <
                                                                                                                                                                         tablePartitions;
                                                                                                                                                                         i++)
                                                                                                                                                                        PostgreSqlHelper
                                                                                                                                                                            .NonQuery(
                                                                                                                                                                                cmd
                                                                                                                                                                                : $"CREATE TABLE leaf_temporal_{i} PARTITION OF leaf_temporal FOR VALUES WITH (MODULUS {tablePartitions}, REMAINDER {i});",
                                                                                                                                                                                timeout
                                                                                                                                                                                : 0);
                                                                                                                                                                    if
                                                                                                                                                                        (PostgreSqlHelper
                                                                                                                                                                             .NonQuery(
                                                                                                                                                                                 cmd
                                                                                                                                                                                 : "CREATE INDEX leaf_temporal_birth_yearmmdd ON leaf_temporal USING BTREE ((birth[1]));" +
                                                                                                                                                                                 "CREATE INDEX leaf_temporal_birth_hhmmss ON leaf_temporal USING BTREE ((birth[2]));" +
                                                                                                                                                                                 "CREATE INDEX leaf_temporal_death_yearmmdd ON leaf_temporal USING BTREE ((death[1]));" +
                                                                                                                                                                                 "CREATE INDEX leaf_temporal_death_hhmmss ON leaf_temporal USING BTREE ((death[2]));",
                                                                                                                                                                                 timeout
                                                                                                                                                                                 : 0) !=
                                                                                                                                                                         null)
                                                                                                                                                                    {
                                                                                                                                                                        Invoke(
                                                                                                                                                                            method
                                                                                                                                                                            : () =>
                                                                                                                                                                            {
                                                                                                                                                                                statusProgress
                                                                                                                                                                                        .Value =
                                                                                                                                                                                    100;
                                                                                                                                                                                DatabaseLogAdd(
                                                                                                                                                                                    input
                                                                                                                                                                                    : statusText
                                                                                                                                                                                            .Text =
                                                                                                                                                                                        @"Create public functions ...");
                                                                                                                                                                            }
                                                                                                                                                                        );
                                                                                                                                                                        int
                                                                                                                                                                            .TryParse(
                                                                                                                                                                                s:
                                                                                                                                                                                $"{PostgreSqlHelper.Scalar(cmd: "SELECT count(*) FROM pg_proc WHERE proname = 'first_agg' OR proname = 'first';", timeout: 0)}",
                                                                                                                                                                                result
                                                                                                                                                                                : out
                                                                                                                                                                                var
                                                                                                                                                                                    firstAggregateExist);
                                                                                                                                                                        if
                                                                                                                                                                            (firstAggregateExist !=
                                                                                                                                                                             2)
                                                                                                                                                                            PostgreSqlHelper
                                                                                                                                                                                .NonQuery(
                                                                                                                                                                                    cmd
                                                                                                                                                                                    : "CREATE OR REPLACE FUNCTION public.first_agg (anyelement, anyelement)" +
                                                                                                                                                                                    "  RETURNS anyelement" +
                                                                                                                                                                                    "  LANGUAGE sql IMMUTABLE STRICT PARALLEL SAFE AS" +
                                                                                                                                                                                    "  'SELECT $1';" +
                                                                                                                                                                                    "  CREATE OR REPLACE AGGREGATE public.first (anyelement) (" +
                                                                                                                                                                                    "    SFUNC = public.first_agg" +
                                                                                                                                                                                    "    , STYPE = anyelement" +
                                                                                                                                                                                    "    , PARALLEL = safe" +
                                                                                                                                                                                    "    );",
                                                                                                                                                                                    timeout
                                                                                                                                                                                    : 0);
                                                                                                                                                                        int
                                                                                                                                                                            .TryParse(
                                                                                                                                                                                s:
                                                                                                                                                                                $"{PostgreSqlHelper.Scalar(cmd: "SELECT count(*) FROM pg_proc WHERE proname = 'last_agg' OR proname = 'last';", timeout: 0)}",
                                                                                                                                                                                result
                                                                                                                                                                                : out
                                                                                                                                                                                var
                                                                                                                                                                                    lastAggregateExist);
                                                                                                                                                                        if
                                                                                                                                                                            (lastAggregateExist !=
                                                                                                                                                                             2)
                                                                                                                                                                            PostgreSqlHelper
                                                                                                                                                                                .NonQuery(
                                                                                                                                                                                    cmd
                                                                                                                                                                                    : "CREATE OR REPLACE FUNCTION public.last_agg (anyelement, anyelement)" +
                                                                                                                                                                                    "  RETURNS anyelement" +
                                                                                                                                                                                    "  LANGUAGE sql IMMUTABLE STRICT PARALLEL SAFE AS" +
                                                                                                                                                                                    "  'SELECT $2';" +
                                                                                                                                                                                    "  CREATE OR REPLACE AGGREGATE public.last (anyelement) (" +
                                                                                                                                                                                    "    SFUNC = public.last_agg" +
                                                                                                                                                                                    "    , STYPE = anyelement" +
                                                                                                                                                                                    "    , PARALLEL = safe" +
                                                                                                                                                                                    "    );",
                                                                                                                                                                                    timeout
                                                                                                                                                                                    : 0);
                                                                                                                                                                        const
                                                                                                                                                                            string
                                                                                                                                                                            ogcBranches =
                                                                                                                                                                                "ogc_branches";
                                                                                                                                                                        int
                                                                                                                                                                            .TryParse(
                                                                                                                                                                                s:
                                                                                                                                                                                $"{PostgreSqlHelper.Scalar(cmd: $"SELECT count(*) FROM pg_proc WHERE proname = '{ogcBranches}';", timeout: 0)}",
                                                                                                                                                                                result
                                                                                                                                                                                : out
                                                                                                                                                                                var
                                                                                                                                                                                    ogcBranchesExist);
                                                                                                                                                                        if
                                                                                                                                                                            (ogcBranchesExist ==
                                                                                                                                                                             0)
                                                                                                                                                                            PostgreSqlHelper
                                                                                                                                                                                .NonQuery
                                                                                                                                                                                (
                                                                                                                                                                                    cmd
                                                                                                                                                                                    : $"CREATE OR REPLACE FUNCTION public.{ogcBranches}(typename text, path boolean DEFAULT NULL::boolean) RETURNS TABLE(branch integer) LANGUAGE 'plpgsql' AS $$" +
                                                                                                                                                                                    " DECLARE" +
                                                                                                                                                                                    "    layerArray text[] := string_to_array(typeName, '.');" +
                                                                                                                                                                                    "    levelSelectList text[];" +
                                                                                                                                                                                    "    levelWhereList text[];" +
                                                                                                                                                                                    "    parameters text[];" +
                                                                                                                                                                                    "    theTypeName text;" +
                                                                                                                                                                                    "    size integer;" +
                                                                                                                                                                                    "    index integer;" +
                                                                                                                                                                                    "    sql text;" +
                                                                                                                                                                                    " BEGIN" +
                                                                                                                                                                                    "    size := array_length(layerArray, 1);" +
                                                                                                                                                                                    "    IF size IS null THEN" +
                                                                                                                                                                                    "      size := 1;" +
                                                                                                                                                                                    "      layerArray[1] := '*';" +
                                                                                                                                                                                    "    END IF;" +
                                                                                                                                                                                    "    index := 0;" +
                                                                                                                                                                                    "    FOR i IN REVERSE size .. 1 LOOP" +
                                                                                                                                                                                    "      theTypeName := layerArray[i];" +
                                                                                                                                                                                    "      IF theTypeName <> '' AND theTypeName <> '*' AND theTypeName <> '＊' THEN" +
                                                                                                                                                                                    "        index := index + 1;" +
                                                                                                                                                                                    "        sql := ' AND name ILIKE $1[' || index || ']::text';" +
                                                                                                                                                                                    "        parameters[index] := theTypeName;" +
                                                                                                                                                                                    "      ELSE" +
                                                                                                                                                                                    "        sql := '';" +
                                                                                                                                                                                    "      END IF;" +
                                                                                                                                                                                    "      levelSelectList := array_append(levelSelectList, '(SELECT * FROM branch WHERE level = ' || i || sql || ') AS level' || i);" +
                                                                                                                                                                                    "      IF i > 1 THEN" +
                                                                                                                                                                                    "        levelWhereList := array_append(levelWhereList, 'level' || i || '.parent = level' || (i - 1) || '.id');" +
                                                                                                                                                                                    "      END IF;" +
                                                                                                                                                                                    "  END LOOP;" +
                                                                                                                                                                                    "  IF array_length(levelWhereList, 1) >= 1 THEN" +
                                                                                                                                                                                    "    sql := ' WHERE ' || array_to_string(levelWhereList, ' AND ');" +
                                                                                                                                                                                    "  ELSE" +
                                                                                                                                                                                    "    sql := '';" +
                                                                                                                                                                                    "  END IF;" +
                                                                                                                                                                                    "  sql :=" +
                                                                                                                                                                                    "    'WITH RECURSIVE cte AS' ||" +
                                                                                                                                                                                    "    '  (' ||" +
                                                                                                                                                                                    "    '    SELECT branch.* FROM branch,' ||" +
                                                                                                                                                                                    "    '    (' ||" +
                                                                                                                                                                                    "    '        SELECT level' || size ||'.* FROM ' || array_to_string(levelSelectList, ',') || sql ||" +
                                                                                                                                                                                    "    '    ) AS levels' ||" +
                                                                                                                                                                                    "    '    WHERE branch.id = levels.id' ||" +
                                                                                                                                                                                    "    '    UNION ALL' ||" +
                                                                                                                                                                                    "    '    SELECT branch.* FROM branch' ||" +
                                                                                                                                                                                    "    '    INNER JOIN cte' ||" +
                                                                                                                                                                                    "    '    ON branch.parent = cte.id' ||" +
                                                                                                                                                                                    "    '  )' ||" +
                                                                                                                                                                                    "    '  SELECT DISTINCT id FROM cte';" +
                                                                                                                                                                                    "  IF path IS NOT true THEN" +
                                                                                                                                                                                    "    sql := sql ||" +
                                                                                                                                                                                    "    '  AS cte1 WHERE NOT EXISTS' ||" +
                                                                                                                                                                                    "    '  (' ||" +
                                                                                                                                                                                    "    '    SELECT id FROM cte AS cte2' ||" +
                                                                                                                                                                                    "    '    WHERE cte1.id = cte2.parent' ||" +
                                                                                                                                                                                    "    '  )';" +
                                                                                                                                                                                    "  END IF;" +
                                                                                                                                                                                    "  RETURN QUERY EXECUTE sql USING parameters;" +
                                                                                                                                                                                    " END;" +
                                                                                                                                                                                    " $$",
                                                                                                                                                                                    timeout
                                                                                                                                                                                    : 0);
                                                                                                                                                                        const
                                                                                                                                                                            string
                                                                                                                                                                            ogcBranch =
                                                                                                                                                                                "ogc_branch";
                                                                                                                                                                        int
                                                                                                                                                                            .TryParse(
                                                                                                                                                                                s:
                                                                                                                                                                                $"{PostgreSqlHelper.Scalar(cmd: $"SELECT count(*) FROM pg_proc WHERE proname = '{ogcBranch}';", timeout: 0)}",
                                                                                                                                                                                result
                                                                                                                                                                                : out
                                                                                                                                                                                var
                                                                                                                                                                                    ogcBranchExist);
                                                                                                                                                                        if
                                                                                                                                                                            (ogcBranchExist ==
                                                                                                                                                                             0)
                                                                                                                                                                            PostgreSqlHelper
                                                                                                                                                                                .NonQuery
                                                                                                                                                                                (
                                                                                                                                                                                    cmd
                                                                                                                                                                                    : $"CREATE OR REPLACE FUNCTION public.{ogcBranch}(id integer) RETURNS TABLE(tree integer, levels smallint[], layer text[], layerproperty jsonb[], layerdetail xml[]) LANGUAGE 'plpgsql' AS $$" +
                                                                                                                                                                                    " BEGIN" +
                                                                                                                                                                                    "    RETURN QUERY" +
                                                                                                                                                                                    "    WITH RECURSIVE cte AS" +
                                                                                                                                                                                    "    (" +
                                                                                                                                                                                    "      SELECT branch.* FROM branch" +
                                                                                                                                                                                    "      WHERE branch.id = $1" +
                                                                                                                                                                                    "      UNION ALL" +
                                                                                                                                                                                    "      SELECT branch.* FROM branch" +
                                                                                                                                                                                    "      INNER JOIN cte" +
                                                                                                                                                                                    "      ON branch.id = cte.parent" +
                                                                                                                                                                                    "    )" +
                                                                                                                                                                                    "    SELECT * FROM" +
                                                                                                                                                                                    "    (" +
                                                                                                                                                                                    "      SELECT FIRST(t.tree) AS tree, ARRAY_AGG(t.level) as levels, ARRAY_AGG(t.name) AS layer, ARRAY_AGG(t.property) AS layerproperty, ARRAY_AGG(tt.detail) AS layerdetail" +
                                                                                                                                                                                    "      FROM" +
                                                                                                                                                                                    "      (" +
                                                                                                                                                                                    "        SELECT * FROM cte ORDER BY level" +
                                                                                                                                                                                    "      ) AS t" +
                                                                                                                                                                                    "      LEFT JOIN branch_relation AS tt" +
                                                                                                                                                                                    "      ON t.id = tt.branch" +
                                                                                                                                                                                    "    ) AS t" +
                                                                                                                                                                                    "    WHERE t.tree IS NOT NULL;" +
                                                                                                                                                                                    " END;" +
                                                                                                                                                                                    " $$",
                                                                                                                                                                                    timeout
                                                                                                                                                                                    : 0);
                                                                                                                                                                        _clusterUser
                                                                                                                                                                                .status =
                                                                                                                                                                            true;
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                        errorMessage =
                                                                                                                                                                            $"Failed to create some indexes of leaf_temporal - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                    errorMessage =
                                                                                                                                                                        $"Failed to create leaf_temporal - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                                errorMessage =
                                                                                                                                                                    $"Failed to create leaf_hits - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                            errorMessage =
                                                                                                                                                                $"Failed to create some indexes of leaf_wms - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                        errorMessage =
                                                                                                                                                            $"Failed to create leaf_wms - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                    errorMessage =
                                                                                                                                                        $"Failed to create some indexes of leaf_tile - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                                errorMessage =
                                                                                                                                                    $"Failed to create leaf_tile - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                            errorMessage =
                                                                                                                                                $"Failed to create some indexes of leaf_geometry - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                        errorMessage =
                                                                                                                                            $"Failed to create leaf_geometry - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                                }
                                                                                                                                else
                                                                                                                                    errorMessage =
                                                                                                                                        $"Failed to create some indexes of leaf_style - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                            }
                                                                                                                            else
                                                                                                                                errorMessage =
                                                                                                                                    $"Failed to create leaf_style - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                        }
                                                                                                                        else
                                                                                                                            errorMessage =
                                                                                                                                $"Failed to create some indexes of leaf_description - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                    }
                                                                                                                    else
                                                                                                                        errorMessage =
                                                                                                                            $"Failed to create leaf_description - {PostgreSqlHelper.ErrorMessage}";
                                                                                                                }
                                                                                                                else
                                                                                                                    errorMessage =
                                                                                                                        $"Failed to create some indexes of leaf - {PostgreSqlHelper.ErrorMessage}";
                                                                                                            }
                                                                                                            else
                                                                                                                errorMessage =
                                                                                                                    $"Failed to create leaf - {PostgreSqlHelper.ErrorMessage}";
                                                                                                        }
                                                                                                        else
                                                                                                            errorMessage =
                                                                                                                $"Failed to create some indexes of branch - {PostgreSqlHelper.ErrorMessage}";
                                                                                                    }
                                                                                                    else
                                                                                                        errorMessage =
                                                                                                            $"Failed to create branch - {PostgreSqlHelper.ErrorMessage}";
                                                                                                }
                                                                                                else
                                                                                                    errorMessage =
                                                                                                        $"Failed to create some indexes of tree - {PostgreSqlHelper.ErrorMessage}";
                                                                                            }
                                                                                            else
                                                                                                errorMessage =
                                                                                                    $"Failed to create tree - {PostgreSqlHelper.ErrorMessage}";
                                                                                        }
                                                                                        else
                                                                                            errorMessage =
                                                                                                $"Failed to create some indexes of forest - {PostgreSqlHelper.ErrorMessage}";
                                                                                    }
                                                                                    else
                                                                                        errorMessage =
                                                                                            $"Failed to create forest - {PostgreSqlHelper.ErrorMessage}";
                                                                                }
                                                                                else
                                                                                    errorMessage =
                                                                                        "No multilingual full text retrieval extension module (pgroonga) found.";
                                                                            }
                                                                            else
                                                                                errorMessage =
                                                                                    "One dimensional integer array extension module (intarray) not found.";
                                                                        }
                                                                        else
                                                                            errorMessage =
                                                                                "No raster data expansion module was found (postgis_raster).";
                                                                    }
                                                                    else
                                                                        errorMessage =
                                                                            "No vector data expansion module was found (postgis).";
                                                                }
                                                                else
                                                                    errorMessage =
                                                                        $"Unable to create database [{PostgreSqlHelper.ErrorMessage}].";

                                                                break;
                                                            }
                                                    }

                                                    if (string.IsNullOrWhiteSpace(value: errorMessage))
                                                    {
                                                        var tasks = new[]
                                                        {
                                                            Task.Factory.StartNew(action: () =>
                                                                {
                                                                    _clusterUser = (true, forest,
                                                                        GeositeServerUser.Text.Trim());
                                                                    _databaseGridObject = new DatabaseGrid(
                                                                        dataGridView: DatabaseGridView,
                                                                        firstPage: firstPage,
                                                                        previousPage: previousPage,
                                                                        nextPage: nextPage,
                                                                        lastPage: lastPage,
                                                                        pageBox: pagesBox,
                                                                        deleteTree: deleteTree,
                                                                        forest: forest
                                                                    );
                                                                }
                                                            ),
                                                            Task.Factory.StartNew(action: () =>
                                                                {
                                                                    if (_catalogTreeObject == null)
                                                                        _catalogTreeObject =
                                                                            new CatalogTree(
                                                                                catalogTreeView: CatalogTreeView,
                                                                                forest: forest,
                                                                                rootName: rootName
                                                                            );
                                                                    else
                                                                        _catalogTreeObject.InsertNodes();
                                                                }
                                                            )
                                                        };
                                                        Task.WaitAll(tasks: tasks);
                                                    }
                                                }
                                                else
                                                    errorMessage =
                                                        @"Please connect to a higher version of GeositeServer.";
                                            }
                                            catch (Exception ex)
                                            {
                                                errorMessage = ex.Message;
                                            }
                                        }
                                    }
                                    else
                                        errorMessage = userX.message;
                                }
                                finally
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            if (errorMessage == null)
                                            {
                                                DatabaseLogAdd(input: statusText.Text =
                                                    @"GeositeServer Connection OK.");
                                                GeositeServerLink.BackgroundImage = Properties.Resources.linkok;
                                                deleteForest.Enabled = true;
                                                _clusterUser.status = dataGridPanel.Enabled = true;
                                                PostgresRun.Enabled = true;
                                                OGCtoolTip.SetToolTip(PostgresRun, "Start");
                                            }
                                            else
                                            {
                                                _clusterUser.status = false;
                                                _databaseGridObject?.Clear();
                                                _catalogTreeObject?.Clear();
                                                DatabaseLogAdd(input: statusText.Text = errorMessage);
                                                GeositeServerLink.BackgroundImage = Properties.Resources.linkfail;
                                                deleteForest.Enabled = false;
                                            }

                                            statusProgress.Visible = false;
                                            statusProgress.Value = 0;
                                            Reindex.Enabled = ReClean.Enabled = errorMessage == null && _administrator;
                                            dataGridPanel.Enabled = CatalogTreeView.Enabled = true;
                                            _loading.Run(onOff: false);
                                        }
                                    );
                                }
                            }
                        ).Start();
                    }
                    else
                        Invoke(method: () =>
                            {
                                DatabaseLogAdd(input: statusText.Text = @"GeositeServer connection failed.");
                            }
                        );
                }
                catch
                {
                    Invoke(method: () =>
                    {
                        DatabaseLogAdd(input: statusText.Text = @"GeositeServer host connection failed.");
                    });
                }
            }
            else
                Invoke(method: () =>
                    {
                        DatabaseLogAdd(input: statusText.Text = @"GeositeServer URI cannot be empty.");
                    }
                );
        }

        private (XElement result, string message) GetClusterUserX(
            string serverUrl,
            string serverUser,
            string serverPassword,
            string clientVersion,
            int timeout = 3000)
        {
            try
            {
                return GeositeServerUsers.GetClusterUser(
                    url: serverUrl,
                    userName: serverUser, password: string.IsNullOrWhiteSpace(value: serverPassword) ? "" : $"{GeositeConfuser.Cryptography.HashEncoder(arg: serverPassword)}",
                    version: clientVersion,
                    timeout: timeout
                );
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }

        private void UpdateDatabaseSize(
            string serverUrl,
            string serverUser,
            string serverPassword,
            int timeout = 3000)
        {
            var userX = GetClusterUserX(
                serverUrl: serverUrl,
                serverUser: serverUser,
                serverPassword: serverPassword,
                clientVersion: Copyright.VersionAttribute,
                timeout: timeout
            );
            if (userX.result != null)
            {
                var size = userX.result.Element(name: "Servers")
                    ?.Element(name: "Server")
                    ?.Element(name: "Database")
                    ?.Attribute(name: "Size")
                    ?.Value ?? "";
                if (!string.IsNullOrWhiteSpace(value: size))
                    DatabaseLogAdd(input: $"Database Size - {size}");
            }
        }

        private void ReIndex_Click(object sender, EventArgs e)
        {
            new Task(
                    action: () =>
                    {
                        try
                        {
                            Invoke(
                                method: () =>
                                {
                                    _loading.Run();
                                    statusProgress.Visible = true;
                                    statusProgress.Value = 0;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE forest ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE forest;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 6;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE forest_relation ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE forest_relation;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 12;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE tree ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE tree;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 18;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE tree_relation ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE tree_relation;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 24;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE branch ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE branch;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 30;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE branch_relation ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE branch_relation;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 36;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 42;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_relation ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_relation;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 48;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_description ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_description;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 54;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_style ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_style;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 60;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_geometry ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_geometry;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 68;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_tile ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_tile;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 76;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_wms ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_wms;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 84;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_temporal ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_temporal;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 92;
                                    DatabaseLogAdd(input: statusText.Text = @"REINDEX TABLE leaf_hits ...");
                                }
                            );
                            if (PostgreSqlHelper.NonQuery(cmd: "REINDEX TABLE leaf_hits;", timeout: 0) == null)
                                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Value = 100;
                                    DatabaseLogAdd(input: statusText.Text = @"Reindex finished.");
                                }
                            );
                        }
                        catch (Exception error)
                        {
                            Invoke(
                                method: () =>
                                {
                                    DatabaseLogAdd(input: statusText.Text = @$"Reindex failed ({error.Message}).");
                                }
                            );
                        }
                        finally
                        {
                            Invoke(
                                method: () =>
                                {
                                    statusProgress.Visible = false;
                                    statusProgress.Value = 0;
                                    _loading.Run(onOff: false);
                                }
                            );
                        }
                    }
                )
                .Start();
        }

        private void ReClean_Click(object sender, EventArgs e)
        {
            var serverUrl = GeositeServerUrl.Text.Trim();
            var serverUser = GeositeServerUser.Text.Trim();
            var serverPassword = GeositeServerPassword.Text.Trim();
            new Task(
                action: () =>
                {
                    try
                    {
                        Invoke(
                            method: () =>
                            {
                                _loading.Run();
                                statusProgress.Visible = true;
                                statusProgress.Value = 0;
                                DatabaseLogAdd(input: statusText.Text = @"Access frequency synchronization ...");
                            }
                        );
                        GeositeHits.Refresh();
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 6;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE forest ...");
                            }
                        );

                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE forest;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT COUNT(*) FROM forest;",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"{count} record{(count > 1 ? "s" : "")} in forest.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 12;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE forest_relation ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE forest_relation;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 18;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE tree ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE tree;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT COUNT(*) FROM tree;",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"{count} record{(count > 1 ? "s" : "")} in tree.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 24;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE tree_relation ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE tree_relation;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 30;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE branch ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE branch;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT COUNT(*) FROM branch;",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"{count} record{(count > 1 ? "s" : "")} in branch.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 36;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE branch_relation ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE branch_relation;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 42;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 48;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_relation ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_relation;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 54;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_description ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_description;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf_description';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf_description.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 60;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_style ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_style;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf_style';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf_style.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 66;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_geometry ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_geometry;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf_geometry';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf_geometry.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 72;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_tile ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_tile;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf_tile';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf_tile.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 78;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_wms ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_wms;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf_wms';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf_wms.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 84;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_temporal ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_temporal;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                var count = long.Parse(
                                    (
                                        PostgreSqlHelper.Scalar(
                                            cmd: "SELECT reltuples::bigint AS estimate FROM pg_class WHERE relname = 'leaf_temporal';",
                                            timeout: 0
                                        )
                                    ).ToString() ?? "0"
                                );
                                DatabaseLogAdd(input: statusText.Text = $@"About {count} record{(count > 1 ? "s" : "")} in leaf_temporal.");
                            }
                        );
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 90;
                                DatabaseLogAdd(input: statusText.Text = @"VACUUM ANALYZE leaf_hits ...");
                            }
                        );
                        if (PostgreSqlHelper.NonQuery(cmd: "VACUUM ANALYZE leaf_hits;", timeout: 0) == null)
                            throw new Exception(message: PostgreSqlHelper.ErrorMessage);
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Value = 100;
                                UpdateDatabaseSize(serverUrl: serverUrl, serverUser: serverUser,
                                    serverPassword: serverPassword);
                                DatabaseLogAdd(input: statusText.Text = @"Reclean finished.");
                            }
                        );
                    }
                    catch (Exception error)
                    {
                        Invoke(
                            method: () => { DatabaseLogAdd(input: statusText.Text = @$"Reclean failed ({error.Message})."); }
                        );
                    }
                    finally
                    {
                        Invoke(
                            method: () =>
                            {
                                statusProgress.Visible = false;
                                statusProgress.Value = 0;
                                _loading.Run(onOff: false);
                            }
                        );
                    }
                }
            ).Start();
        }

        private void FirstPage_Click(object sender, EventArgs e)
        {
            new Task(
                action: () =>
                {
                    _loading.Run();
                    _databaseGridObject?.First();
                    _loading.Run(onOff: false);
                }
            ).Start();
        }

        private void PreviousPage_Click(object sender, EventArgs e)
        {
            new Task(
                action: () =>
                {
                    _loading.Run();
                    _databaseGridObject?.Previous();
                    _loading.Run(onOff: false);
                }
            ).Start();
        }

        private void NextPage_Click(object sender, EventArgs e)
        {
            new Task(
                action: () =>
                {
                    _loading.Run();
                    _databaseGridObject?.Next();
                    _loading.Run(onOff: false);
                }
            ).Start();
        }

        private void LastPage_Click(object sender, EventArgs e)
        {
            new Task(
                action: () =>
                {
                    _loading.Run();
                    _databaseGridObject?.Last();
                    _loading.Run(onOff: false);
                }
            ).Start();
        }

        private void DeleteTree_Click(object sender, EventArgs e)
        {
            if (_administrator)
            {
                var selectedRows = DatabaseGridView.SelectedRows;
                if (selectedRows.Count > 0 && MessageBox.Show(
                        text: $@"Are you sure you want to delete {selectedRows.Count} selected item{(selectedRows.Count == 1 ? "" : "s")}?",
                        caption: @"Caution",
                        buttons: MessageBoxButtons.YesNo,
                        icon: MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    var ids =
                        selectedRows
                            .Cast<DataGridViewRow>()
                            .Select(selector: row => row.Cells[index: 1])
                            .Select(selector: typeCell => Regex.Split(input: typeCell.ToolTipText, pattern: @"[\b]", options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline))
                            .Select(selector: typeCellArray => typeCellArray[0]).ToList();
                    new Task(
                        action: () =>
                        {
                            try
                            {
                                Invoke(
                                    method: () =>
                                    {
                                        _loading.Run();
                                        DatabaseLogAdd(input: statusText.Text = @"Deleting ...");
                                        dataGridPanel.Enabled = CatalogTreeView.Enabled = false;
                                    }
                                );
                                var del = PostgreSqlHelper.NonQuery(
                                    cmd: $"DELETE FROM tree WHERE id in ({string.Join(separator: ",", values: ids)});",
                                    timeout: 0
                                ) != null;
                                Invoke(method: () =>
                                {
                                    if (del)
                                    {
                                        var rowStack = new Stack<DataGridViewRow>();
                                        selectedRows
                                            .Cast<DataGridViewRow>()
                                            .Where(predicate: row => !row.IsNewRow)
                                            .ToList()
                                            .ForEach(action: row => rowStack.Push(item: row));
                                        while (rowStack.Count > 0)
                                        {
                                            try
                                            {
                                                DatabaseGridView.Rows.Remove(dataGridViewRow: rowStack.Pop());
                                            }
                                            catch (Exception error)
                                            {
                                                DatabaseLogAdd(input: statusText.Text = error.Message);
                                            }
                                        }
                                        _databaseGridObject?.Reset();
                                        _catalogTreeObject?.Reset();
                                    }
                                });
                            }
                            catch (Exception error)
                            {
                                Invoke(
                                    method: () =>
                                    {
                                        DatabaseLogAdd(input: statusText.Text = error.Message);
                                    }
                                );
                            }
                            finally
                            {
                                Invoke(
                                    method: () =>
                                    {
                                        DatabaseLogAdd(input: statusText.Text = @"Delete succeeded.");
                                        dataGridPanel.Enabled = CatalogTreeView.Enabled = true;
                                        _loading.Run(onOff: false);
                                    }
                                );
                            }
                        }
                    ).Start();
                }
            }
            else
                DatabaseLogAdd(input: statusText.Text = @"Administrator identity is required.");
        }

        private void DatabaseViewMenuItem_Click(object sender, EventArgs e)
        {
            if (_administrator)
            {
                try
                {
                    var selectedRows = DatabaseGridView.SelectedRows;
                    if (selectedRows.Count > 0)
                    {
                        var theSender = (ToolStripMenuItem)sender;
                        switch (theSender.Text)
                        {
                            case "Rename":
                                {
                                    var col = selectedRows[index: 0].Cells[index: 1].ColumnIndex - 1;
                                    var row = selectedRows[index: 0].Index;
                                    DatabaseGridView.CurrentCell = DatabaseGridView[columnIndex: col, rowIndex: row];
                                    DatabaseGridView.BeginEdit(selectAll: false);
                                    break;
                                }
                            case "Remove":
                                {
                                    DeleteTree_Click(sender: sender, e: e);
                                    break;
                                }
                        }
                    }
                }
                catch
                {
                    //
                }
            }
            else
                DatabaseLogAdd(input: statusText.Text = @"Administrator identity is required.");
        }

        private void DataPool_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;
            var colIndex = e.ColumnIndex; //系统自带的列 位于最左边 索引号为【-1】
            if (rowIndex >= 0) //标题行为【-1】
            {
                //var forest = _clusterUser.forest;
                var linkStatus = _clusterUser.status;
                if (linkStatus)
                {
                    var themeCell = ((DataGridView)sender).Rows[index: rowIndex].Cells[index: 1];
                    var themeInfo =
                        // $"{tree}\b{type}\b{layer}\b{leaf}\b{status}\b{timestamp}"
                        Regex.Split(input: themeCell.ToolTipText, pattern: @"[\b]");
                    var tree = int.Parse(themeInfo[0]);
                    if (colIndex == -1)
                    {
                        _loading.Run();
                        Invoke(method: () =>
                            {
                                DatabaseLogAdd(
                                    input: statusText.Text =
                                        @"Layer - [" +
                                        (string)PostgreSqlHelper.Scalar
                                        (
                                            cmd:
                                            "SELECT array_to_string(array_agg(name), '.') FROM (SELECT distinct on(level) name FROM branch WHERE tree = @tree ORDER BY level,id) AS route;",
                                            parameters: new Dictionary<string, object>
                                            {
                                                { "tree", tree }
                                            }
                                        ) +
                                        @"]"
                                );
                                _loading.Run(onOff: false);
                            }
                        );
                    }
                    else
                    {
                        if (colIndex == 4)
                        {
                            //强制进入图形预览卡
                            ogcCard.SelectedIndex = 0;
                            new MapView(
                                mainForm: this,
                                path: $"{new UriBuilder(GeositeServerUrl.Text.Trim()).Uri}\b{themeInfo[2]}\b{themeInfo[3]}",
                                type: themeInfo[1],
                                property: (XElement)themeCell.Tag,
                                style: PreviewStyleForm.Style,
                                projection: null
                            ).View();
                        }
                    }
                }
            }
        }

        private void DataPool_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (_administrator)
            {
                var dataGrid = (DataGridView)sender;
                var colIndex = e.ColumnIndex;
                var rowIndex = e.RowIndex;
                if (rowIndex >= 0 && colIndex is 0 or 1)
                    _dataPoolGridCellValue = $"{dataGrid.Rows[index: rowIndex].Cells[index: colIndex].Value}".Trim();
            }
            else
                e.Cancel = true;
        }

        private void DataPool_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_administrator)
            {
                var colIndex = e.ColumnIndex;
                var rowIndex = e.RowIndex;
                if (rowIndex >= 0 && colIndex is 0 or 1)
                {
                    var row = ((DataGridView)sender).Rows[index: rowIndex];
                    var col = row.Cells[index: colIndex];
                    var newValue = $"{col.Value}".Trim();
                    var oldValue = _dataPoolGridCellValue;
                    if (string.IsNullOrWhiteSpace(value: newValue))
                        newValue = oldValue;
                    else
                    {
                        if (colIndex == 0)
                            try
                            {
                                newValue = new XElement(name: newValue).Name.LocalName;
                            }
                            catch
                            {
                                newValue = oldValue;
                            }
                        else
                        {
                            if (int.TryParse(s: newValue, result: out var nv))
                            {
                                if (nv is < -1 or > 255)
                                    newValue = oldValue;
                            }
                            else
                                newValue = oldValue;
                        }
                    }
                    if (newValue != oldValue)
                    {
                        col.Value = newValue;
                        _loading.Run();
                        Invoke(method: () =>
                            {
                                var forest = _clusterUser.forest;
                                if (colIndex == 0)
                                {
                                    var oldId = PostgreSqlHelper.Scalar(
                                        cmd:
                                        "SELECT id FROM tree WHERE forest = @forest AND name ILIKE @name::text LIMIT 1;",
                                        parameters: new Dictionary<string, object>()
                                        {
                                            {"forest", forest},
                                            {"name", newValue}
                                        }
                                    );
                                    if (oldId != null)
                                    {
                                        //row.Cells[index: colIndex].Value = oldValue;
                                        MessageBox.Show(
                                            text: $@"Duplicate [{newValue}] are not allowed.",
                                            caption: @"Tip",
                                            buttons: MessageBoxButtons.OK,
                                            icon: MessageBoxIcon.Error
                                        );
                                        col.Value = oldValue;
                                    }
                                    else
                                    {
                                        var typeCellArray = Regex.Split(
                                            input: row.Cells[index: 1].ToolTipText,
                                            pattern: @"[\b]",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                                        var id = typeCellArray[0];
                                        DatabaseLogAdd(input: statusText.Text = @"Updating ...");
                                        Application.DoEvents();
                                        if (PostgreSqlHelper.NonQuery(cmd: "UPDATE tree SET name = @name WHERE id = @id;", parameters: new Dictionary<string, object> { { "name", newValue }, { "id", long.Parse(s: id) } }) == null)
                                        {
                                            col.Value = oldValue;
                                            DatabaseLogAdd(input: statusText.Text = PostgreSqlHelper.ErrorMessage);
                                        }
                                        else
                                            DatabaseLogAdd(input: statusText.Text = @"Update completed.");
                                    }
                                }
                                else
                                {
                                    DatabaseLogAdd(input: statusText.Text = @"Updating ...");
                                    Application.DoEvents();
                                    var updateCount = PostgreSqlHelper.NonQuery(
                                        cmd: "WITH " +
                                                             "treeId AS " +
                                                             "( " +
                                                             "    SELECT id FROM tree WHERE forest = @forest AND name ILIKE @name::text LIMIT 1 " +
                                                             ")," +
                                                             "branchId AS " +
                                                             "( " +
                                                             "    SELECT id FROM branch WHERE tree = ANY(SELECT * FROM treeId) " +
                                                             "    EXCEPT " + //仅保留树梢
                                                             "    SELECT parent FROM branch WHERE tree = ANY(SELECT * FROM treeId) " +
                                                             ") " +
                                                             "UPDATE leaf SET rank = @rank WHERE branch = ANY(SELECT * FROM branchId);",
                                        parameters: new Dictionary<string, object>()
                                        {
                                            {"forest", forest},
                                            {"name", row.Cells[index: 0].Value.ToString()},
                                            {"rank", short.Parse(s: newValue)}
                                        }
                                    );
                                    if (updateCount != null)
                                        DatabaseLogAdd(input: statusText.Text = $@"Update completed. [count: {updateCount}]");
                                    else
                                    {
                                        DatabaseLogAdd(input: statusText.Text = PostgreSqlHelper.ErrorMessage);
                                        col.Value = oldValue;
                                    }
                                }
                                _loading.Run(onOff: false);
                            }
                        );
                    }
                }
                _dataPoolGridCellValue = null;
            }
        }

        private void VectorOpen_Click(object sender, EventArgs e)
        {
            var key = VectorOpen.Name;
            int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex);
            var pathKey = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: pathKey);
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"MapGIS|*.wt;*.wl;*.wp|MapGIS|*.mpj|ShapeFile|*.shp|Excel Tab Delimited|*.txt|Excel Comma Delimited|*.csv|Excel|*.xls;*.xlsx;*.xlsb|GoogleEarth(*.kml)|*.kml|GeositeXML|*.xml|GeoJson|*.geojson",
                FilterIndex = filterIndex,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            RegEdit.Setkey(keyname: key, defaultvalue: $"{openFileDialog.FilterIndex}");
            RegEdit.Setkey(keyname: pathKey, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
            foreach (var path in openFileDialog.FileNames)
            {
                var fileType = Path.GetExtension(path: path).ToLower();
                if (fileType == ".mpj")
                {
                    var mapgisProject = new MapGis.MapGisProject();
                    mapgisProject.Open(file: path);
                    var files = mapgisProject.Content?["files"];
                    if (files != null)
                    {
                        var currentPath = Path.GetDirectoryName(path);
                        foreach (var file in files)
                        {
                            var getTheFile = Path.GetFullPath(Path.Combine(currentPath ?? "", (string)file["file"] ?? ""));
                            if (File.Exists(getTheFile))
                            {
                                var theFileType = Path.GetExtension(path: getTheFile).ToLower();
                                switch (theFileType)
                                {
                                    case ".wt":
                                    case ".wl":
                                    case ".wp":
                                        {
                                            var theme = Path.GetFileNameWithoutExtension(path: getTheFile);
                                            int rowIndex;
                                            try
                                            {
                                                rowIndex = vectorFilePool.Rows.Add(values: new object[] { new XElement(name: theme).Name.LocalName, getTheFile });
                                            }
                                            catch
                                            {
                                                rowIndex = vectorFilePool.Rows.Add(values: new object[] { $"Untitled_{theme}", getTheFile });
                                            }
                                            var row = vectorFilePool.Rows[rowIndex];
                                            row.Height = 28;
                                            var projectionButton = row.Cells[index: 2]; //0=Layer 1=URI 2=Projection 3=※（Status）
                                            projectionButton.ToolTipText = "Unknown";
                                            projectionButton.Value = "?";
                                            break;
                                        }
                                    default:
                                        {
                                            DatabaseLogAdd(input: $"[{getTheFile}] in mpj is not supported.");
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                DatabaseLogAdd(input: $"[{getTheFile}] in mpj does not exist.");
                            }
                        }
                    }
                }
                else
                {
                    var theme = Path.GetFileNameWithoutExtension(path: path);
                    int rowIndex;
                    try
                    {
                        rowIndex = vectorFilePool.Rows.Add(values: new object[] { new XElement(name: theme).Name.LocalName, path });
                    }
                    catch
                    {
                        rowIndex = vectorFilePool.Rows.Add(values: new object[] { $"Untitled_{theme}", path });
                    }
                    var row = vectorFilePool.Rows[rowIndex];
                    row.Height = 28;
                    var projectionButton = row.Cells[index: 2]; //0=Layer 1=URI 2=Projection 3=※（Status）
                    projectionButton.ToolTipText = "Unknown";
                    projectionButton.Value = "?";
                }
            }
        }

        private void VectorFilePool_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            VectorFilePool_RowsRemoved(sender: sender, e: null);
        }

        private void VectorFilePool_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            VectorFileClear.Enabled = vectorFilePool.Rows.Count > 0;
        }

        private void VectorFileClear_Click(object sender, EventArgs e)
        {
            foreach (var row in vectorFilePool.SelectedRows.Cast<DataGridViewRow>().Where(predicate: row => !row.IsNewRow))
            {
                try
                {
                    vectorFilePool.Rows.Remove(dataGridViewRow: row);
                }
                catch (Exception error)
                {
                    DatabaseLogAdd(input: statusText.Text = error.Message);
                }
            }
            VectorFilePool_RowsRemoved(sender: sender, e: null);
        }

        private void PostgresRun_Click(object sender, EventArgs e)
        {
            if (vectorWorker.IsBusy || rasterWorker.IsBusy)
            {
                if (vectorWorker.IsBusy && vectorWorker.WorkerSupportsCancellation)
                    vectorWorker.CancelAsync();
                if (rasterWorker.IsBusy && rasterWorker.WorkerSupportsCancellation)
                    rasterWorker.CancelAsync();
                PostgresRun.BackgroundImage = Properties.Resources.linkpush;
                OGCtoolTip.SetToolTip(PostgresRun, "Start");
                return;
            }
            switch (dataCards.SelectedIndex)
            {
                case 0:
                    RasterRunClick();
                    break;
                case 1:
                    VectorRunClick();
                    break;
            }
        }

        private void VectorRunClick()
        {
            if (vectorFilePool.SelectedRows.Cast<DataGridViewRow>().All(predicate: row => row.IsNewRow))
            {
                FileLoadLogAdd(input: statusText.Text = @"Please select one or more layers.");
                return;
            }
            _loading.Run();
            PostgresRun.BackgroundImage = Properties.Resources.linkcancel;
            OGCtoolTip.SetToolTip(PostgresRun, "Cancel");
            statusProgress.Visible = true;
            vectorWorker.RunWorkerAsync(
                argument: (
                    topology: false, rank: rankList.Text,
                    postgresLight: PostgresLight.Checked
                )
            );
        }

        private string VectorWorkStart(BackgroundWorker vectorBackgroundWorker, DoWorkEventArgs e)
        {
            var parameter = ((bool topology, string rank, bool postgresLight))e.Argument!;
            var doTopology = parameter.topology;
            var rank = parameter.rank;
            var forest = _clusterUser.forest;
            var oneForest = new GeositeXmlPush();
            var forestResult = oneForest.Forest(
                id: forest,
                name: _clusterUser.name);
            if (!forestResult.Success)
                return forestResult.Message;
            var status = (short)(parameter.postgresLight ? 4 : 6);
            string statusInfo = null;
            foreach (var row in vectorFilePool.SelectedRows.Cast<DataGridViewRow>().Where(predicate: row => !row.IsNewRow).OrderBy(keySelector: row => row.Index))
            {
                if (vectorBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                var theme = (string)row.Cells[index: 0].Value;
                var path = (string)row.Cells[index: 1].Value;
                var projectionX = (XElement)row.Cells[index: 2].Tag; //maybe null
                var statusCell = row.Cells[index: 3];
                Invoke(method: () => { vectorFilePool.CurrentCell = row.Cells[index: 3]; });
                var oldTree = PostgreSqlHelper.Scalar(
                    cmd: "SELECT id FROM tree WHERE forest = @forest AND (name ILIKE @name::text) LIMIT 1;",
                    parameters: new Dictionary<string, object>
                    {
                        { "forest", forest },
                        { "name", theme }
                    }
                );
                if (oldTree != null)
                    Invoke(
                        method: () =>
                        {
                            statusCell.Value = "✔!";
                            DatabaseLogAdd(input: statusCell.ToolTipText = $"[{theme}] theme already exists!");
                        }
                    );
                else
                {
                    Invoke(
                        method: () =>
                        {
                            statusCell.Value = "…";
                            DatabaseLogAdd(input: statusCell.ToolTipText = "Processing ...");
                        }
                    );
                    var sequenceMax =
                        PostgreSqlHelper.Scalar(
                            cmd: "SELECT sequence FROM tree WHERE forest = @forest ORDER BY sequence DESC LIMIT 1;",
                            parameters: new Dictionary<string, object>
                            {
                                { "forest", forest }
                            }
                        );
                    var sequence = sequenceMax == null ? 0 : 1 + int.Parse(s: $"{sequenceMax}");
                    var fileType = Path.GetExtension(path: path).ToLower();
                    switch (fileType)
                    {
                        case ".wt":
                        case ".wl":
                        case ".wp":
                            {
                                try
                                {
                                    string treePathString = null;
                                    XElement description = null;
                                    var canDo = true;
                                    if (!_noPromptLayersBuilder)
                                    {
                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: path).FullName);
                                        getTreeLayers.ShowDialog();
                                        if (getTreeLayers.Ok)
                                        {
                                            treePathString = getTreeLayers.TreePathString;
                                            description = getTreeLayers.Description;
                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                        }
                                        else
                                            canDo = false;
                                    }
                                    else
                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: path).FullName);
                                    if (canDo)
                                    {
                                        using var mapgis = new MapGis.MapGisFile();
                                        mapgis.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            vectorBackgroundWorker.ReportProgress(
                                                percentProgress: thisEvent.Progress ?? -1,
                                                userState: thisEvent.Message ?? string.Empty
                                            );
                                        };
                                        mapgis.Open(mapgisFile: path, projection: projectionX);
                                        if (mapgis.RecordCount == 0)
                                            throw new Exception(message: "No features found");
                                        mapgis.Fire(message: $"[{path}] preprocessing ...", code: 0);
                                        var getFileInfo = mapgis.GetCapabilities();
                                        var getFileType = $"{getFileInfo[propertyName: "fileType"]}";
                                        var fields = mapgis.GetField();
                                        var haveFields = fields.Length > 0;
                                        var featureCollectionX = new XElement(
                                            name: "FeatureCollection",
                                            content: new object[]
                                            {
                                                new XAttribute(name: "type", value: getFileType),
                                                new XAttribute(name: "timeStamp", value: $"{getFileInfo[propertyName: "timeStamp"]}"),
                                                new XElement(name: "name", content: theme)
                                            }
                                        );
                                        if (description != null)
                                            featureCollectionX.Add(
                                                content: new XElement
                                                (
                                                    name: "property",
                                                    content: description
                                                        .Elements()
                                                        .Select(selector: x => new XElement(name: $"{x.Name}", content: x.Value))
                                                )
                                            );
                                        var bbox = (JArray)getFileInfo[propertyName: "bbox"];
                                        featureCollectionX.Add(
                                            content: new XElement(
                                                name: "boundary",
                                                content: new object[]
                                                {
                                                    new XElement(name: "north", content: $"{bbox[index: 3]}"),
                                                    new XElement(name: "south", content: $"{bbox[index: 1]}"),
                                                    new XElement(name: "west", content: $"{bbox[index: 0]}"),
                                                    new XElement(name: "east", content: $"{bbox[index: 2]}")
                                                }
                                            )
                                        );
                                        var treeTimeStamp = $"{forest},{sequence},{DateTime.Parse(s: $"{getFileInfo[propertyName: "timeStamp"]}"): yyyyMMdd,HHmmss}";
                                        var treeResult =
                                            oneForest.Tree(
                                                timestamp: treeTimeStamp,
                                                treeX: featureCollectionX,
                                                uri: path,
                                                forestStatus: status
                                            );
                                        if (treeResult.Success)
                                        {
                                            var pointer = 0;
                                            var valid = 0;
                                            var treePath = treePathString;
                                            if (string.IsNullOrWhiteSpace(value: treePath))
                                                treePath = "Untitled";
                                            var treeNameArray = Regex.Split(input: treePath, pattern: @"[\/\\\|]+");
                                            var treeId = treeResult.Id;
                                            var treeType = new List<int>();
                                            var isOk = true;
                                            var recordCount = mapgis.RecordCount;
                                            XElement themeMetadataX = null;
                                            var canWork = true;
                                            if (!_noPromptMetaData)
                                            {
                                                var metaData = new MetaDataForm();
                                                metaData.ShowDialog();
                                                if (metaData.Ok)
                                                {
                                                    themeMetadataX = metaData.MetaDataX;
                                                    _noPromptMetaData = metaData.DonotPrompt;
                                                }
                                                else
                                                    canWork = false;
                                            }
                                            if (canWork)
                                            {
                                                XElement layerX = null;
                                                for (var index = treeNameArray.Length - 1; index >= 0; index--)
                                                    layerX = new XElement(
                                                        name: "layer",
                                                        content: new object[]
                                                        {
                                                        new XElement(name: "name", content: treeNameArray[index].Trim()),
                                                        index == treeNameArray.Length - 1 ? themeMetadataX : null,
                                                        index == treeNameArray.Length - 1
                                                            ? new XElement(name: "member")
                                                            : null,
                                                        layerX
                                                        }
                                                    );
                                                featureCollectionX.Add(content: layerX);
                                                var createRoute = oneForest.Branch(
                                                    forest: forest,
                                                    sequence: sequence,
                                                    tree: treeId,
                                                    leafX: featureCollectionX.Descendants(name: "member").First(),
                                                    leafRootX: featureCollectionX
                                                );
                                                if (!createRoute.Success)
                                                {
                                                    if (isOk)
                                                        Invoke(
                                                            method: () =>
                                                            {
                                                                statusCell.Value = "✘";
                                                                DatabaseLogAdd(input: statusCell.ToolTipText = createRoute.Message);
                                                            }
                                                        );
                                                    isOk = false;
                                                }
                                                else
                                                {
                                                    var timeWatch = new Stopwatch();
                                                    timeWatch.Start();
                                                    foreach (var feature in mapgis.GetFeature().AsParallel())
                                                    {
                                                        if (vectorBackgroundWorker.CancellationPending)
                                                        {
                                                            e.Cancel = true;
                                                            break;
                                                        }
                                                        pointer++;
                                                        if (feature != null)
                                                        {
                                                            var featureType = $"{feature[propertyName: "geometry"][key: "type"]}";
                                                            mapgis.Fire(
                                                                message: $"{featureType} [{pointer} / {recordCount}]",
                                                                code: 1,
                                                                progress: 100 * pointer / recordCount
                                                            );
                                                            var featureId = $"{feature[propertyName: "id"]}";
                                                            try
                                                            {
                                                                XElement elementDescriptionX;
                                                                if (haveFields)
                                                                {
                                                                    var fieldValues =
                                                                        ((JObject)feature[propertyName: "properties"])
                                                                        .Properties()
                                                                        .Select(selector: field =>
                                                                            $"{field.Value[key: "value"]}")
                                                                        .ToArray();
                                                                    elementDescriptionX = new XElement(name: "property");
                                                                    for (var item = 0; item < fields.Length; item++)
                                                                        elementDescriptionX.Add(
                                                                            content: new XElement(
                                                                                name: Regex.Replace(
                                                                                    input:
                                                                                    $"{fields[item][propertyName: "name"]}",
                                                                                    pattern: @"[:""（）\(\)]+", replacement: "_",
                                                                                    options: RegexOptions.IgnoreCase |
                                                                                    RegexOptions.Singleline |
                                                                                    RegexOptions.Multiline)
                                                                                ,
                                                                                content: fieldValues[item]
                                                                            )
                                                                        );
                                                                }
                                                                else
                                                                    elementDescriptionX = null;
                                                                var centroidArray = feature[propertyName: "centroid"];
                                                                var featureBoxArray = feature[propertyName: "bbox"];
                                                                if (!centroidArray.HasValues || !featureBoxArray.HasValues)
                                                                {
                                                                    if (isOk)
                                                                        Invoke(
                                                                            method: () =>
                                                                            {
                                                                                statusCell.Value = "!";
                                                                                DatabaseLogAdd(input: statusCell.ToolTipText =
                                                                                    $"Feature Id {featureId} : Invalid geometry");
                                                                            }
                                                                        );
                                                                    isOk = false;
                                                                    continue;
                                                                }
                                                                var featureCoordinates =
                                                                    GeositeXML.OGCformat.GeometryToWkt(geometry: (
                                                                        (JArray)feature[propertyName: "geometry"][
                                                                            key: "coordinates"], featureType));
                                                                var featureCentroid =
                                                                    GeositeXML.OGCformat.GeometryToWkt(geometry: (
                                                                        (JArray)centroidArray, "Point"));
                                                                var featureBox = (JArray)featureBoxArray;
                                                                var featureBoundary = GeositeXML.OGCformat.GeometryToWkt(
                                                                    geometry: (
                                                                        JArray.Parse(
                                                                            json:
                                                                            $"[[[{featureBox[index: 0]},{featureBox[index: 1]}],[{featureBox[index: 2]},{featureBox[index: 1]}],[{featureBox[index: 2]},{featureBox[index: 3]}],[{featureBox[index: 0]},{featureBox[index: 3]}],[{featureBox[index: 0]},{featureBox[index: 1]}]]]"),
                                                                        "Polygon"
                                                                    )
                                                                );
                                                                var featureTimeStamp = feature[propertyName: "timeStamp"]
                                                                    ?.Value<string>();
                                                                var style = (JObject)feature[propertyName: "style"];
                                                                var featureX = featureType switch
                                                                {
                                                                    "Point" or "MultiPoint" => new XElement(
                                                                        name: "member",
                                                                        content: new object[]
                                                                        {
                                                                        new XAttribute(name: "type", value: "Point"),
                                                                        new XAttribute(name: "typeCode", value: "1"),
                                                                        new XAttribute(name: "id", value: featureId),
                                                                        new XAttribute(name: "timeStamp",
                                                                            value: featureTimeStamp),
                                                                        new XAttribute(name: "rank", value: rank),
                                                                        elementDescriptionX,
                                                                        new XElement(
                                                                            name: "geometry",
                                                                            content: new object[]
                                                                            {
                                                                                new XAttribute(name: "format", value: "OGCWKT"),
                                                                                new XAttribute(name: "type", value: featureType),
                                                                                new XAttribute(name: "centroid", value: featureCentroid),
                                                                                new XAttribute(name: "boundary", value: featureBoundary),
                                                                                featureCoordinates
                                                                            }
                                                                        ),
                                                                        new XElement
                                                                        (
                                                                            name: "style",
                                                                            content: style.Properties()
                                                                                .Select
                                                                                (
                                                                                    selector: field =>
                                                                                        new XElement
                                                                                        (
                                                                                            name: field.Name,
                                                                                            content: field.Value
                                                                                                .ToString()
                                                                                        )
                                                                                )
                                                                        )
                                                                        }
                                                                    ),
                                                                    "LineString" or "MultiLineString" =>
                                                                        new XElement(
                                                                            name: "member",
                                                                            content: new object[]
                                                                            {
                                                                            new XAttribute(name: "type", value: "Line"),
                                                                            new XAttribute(name: "typeCode",
                                                                                value: "2"),
                                                                            new XAttribute(name: "id",
                                                                                value: featureId),
                                                                            new XAttribute(name: "timeStamp",
                                                                                value: featureTimeStamp),
                                                                            new XAttribute(name: "rank", value: rank),
                                                                            elementDescriptionX,
                                                                            new XElement(
                                                                                name: "geometry",
                                                                                content: new object[]
                                                                                {
                                                                                    new XAttribute(name: "format",
                                                                                        value: "OGCWKT"),
                                                                                    new XAttribute(name: "type",
                                                                                        value: featureType),
                                                                                    new XAttribute(name: "centroid",
                                                                                        value: featureCentroid),
                                                                                    new XAttribute(name: "boundary",
                                                                                        value: featureBoundary),
                                                                                    featureCoordinates
                                                                                }
                                                                            ),
                                                                            new XElement
                                                                            (
                                                                                name: "style",
                                                                                content: style.Properties()
                                                                                    .Select
                                                                                    (
                                                                                        selector: field =>
                                                                                            new XElement
                                                                                            (
                                                                                                name: field.Name,
                                                                                                content: field.Value
                                                                                                    .ToString()
                                                                                            )
                                                                                    )
                                                                            )
                                                                            }
                                                                        ),
                                                                    "Polygon" or "MultiPolygon" =>
                                                                        new XElement(
                                                                            name: "member",
                                                                            content: new object[]
                                                                            {
                                                                            new XAttribute(name: "type", value: "Polygon"),
                                                                            new XAttribute(name: "typeCode", value: "3"),
                                                                            new XAttribute(name: "id", value: featureId),
                                                                            new XAttribute(name: "timeStamp", value: featureTimeStamp),
                                                                            new XAttribute(name: "rank", value: rank),
                                                                            elementDescriptionX,
                                                                            new XElement(
                                                                                name: "geometry",
                                                                                content: new object[]
                                                                                {
                                                                                    new XAttribute(name: "format", value: "OGCWKT"),
                                                                                    new XAttribute(name: "type", value: featureType),
                                                                                    new XAttribute(name: "centroid", value: featureCentroid),
                                                                                    new XAttribute(name: "boundary", value: featureBoundary),
                                                                                    featureCoordinates
                                                                                }
                                                                            ),
                                                                            new XElement(
                                                                                name: "style",
                                                                                content: style.Properties()
                                                                                    .Select(
                                                                                        selector: field =>
                                                                                            new XElement(
                                                                                                name: field.Name,
                                                                                                content: field.Value
                                                                                                    .ToString())
                                                                                    )
                                                                            )
                                                                            }
                                                                        ),
                                                                    _ => null
                                                                };
                                                                if (featureX != null)
                                                                {
                                                                    var createLeaf = oneForest.Leaf(
                                                                        route: createRoute.Route,
                                                                        leafX: featureX,
                                                                        timestamp:
                                                                        $"{DateTime.Parse(s: featureX.Attribute(name: "timeStamp").Value): yyyyMMdd,HHmmss}",
                                                                        topology: doTopology
                                                                    );
                                                                    if (createLeaf.Success)
                                                                    {
                                                                        var theTreeType = createLeaf.Type;
                                                                        if (!treeType.Contains(item: theTreeType))
                                                                        {
                                                                            treeType.Add(item: theTreeType);
                                                                            oneForest.Tree(
                                                                                enclosure:
                                                                                (
                                                                                    treeId,
                                                                                    treeType,
                                                                                    false
                                                                                )
                                                                            );
                                                                        }
                                                                        valid++;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (isOk)
                                                                            Invoke(
                                                                                method: () =>
                                                                                {
                                                                                    statusCell.Value = "!";
                                                                                    DatabaseLogAdd(
                                                                                        input: statusCell.ToolTipText =
                                                                                            $"Feature Id {featureId} : {createLeaf.Message}");
                                                                                }
                                                                            );
                                                                        isOk = false;
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception localError)
                                                            {
                                                                if (isOk)
                                                                    Invoke(
                                                                        method: () =>
                                                                        {
                                                                            statusCell.Value = "!";
                                                                            DatabaseLogAdd(input: statusCell.ToolTipText =
                                                                                $"Feature Id {featureId} : {localError.Message}");
                                                                        }
                                                                    );
                                                                isOk = false;
                                                            }
                                                        }
                                                    }
                                                    timeWatch.Stop();
                                                    var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
                                                    var message = $"[{valid} feature{(valid > 1 ? "s" : "")}] have been pushed - {duration}";
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            DatabaseLogAdd(input: statusCell.ToolTipText = message);
                                                        }
                                                    );
                                                    mapgis.Fire(
                                                        message: " " + message,
                                                        code: 200);
                                                }
                                                oneForest.Tree(
                                                    enclosure:
                                                    (
                                                        treeId,
                                                        treeType,
                                                        isOk
                                                    )
                                                );
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        _databaseGridObject?.Reset();
                                                        _catalogTreeObject?.Reset();
                                                    }
                                                );
                                                if (isOk)
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            statusCell.Value = "✔";
                                                            statusCell.ToolTipText = "OK";
                                                        }
                                                    );
                                            }
                                            else
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        statusCell.Value = "?";
                                                        DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                                    }
                                                );
                                        }
                                        else
                                            Invoke(
                                                method: () =>
                                                {
                                                    statusCell.Value = "✘";
                                                    DatabaseLogAdd(input: statusCell.ToolTipText = treeResult.Message);
                                                }
                                            );
                                    }
                                    else
                                        Invoke(
                                            method: () =>
                                            {
                                                statusCell.Value = "?";
                                                DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                            }
                                        );
                                }
                                catch (Exception error)
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            statusCell.Value = "!";
                                            DatabaseLogAdd(input: statusCell.ToolTipText = error.Message);
                                        }
                                    );
                                }
                            }
                            break;
                        case ".shp":
                            {
                                try
                                {
                                    string treePathString = null;
                                    XElement description = null;
                                    var canDo = true;
                                    if (!_noPromptLayersBuilder)
                                    {
                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: path).FullName);
                                        getTreeLayers.ShowDialog();
                                        if (getTreeLayers.Ok)
                                        {
                                            treePathString = getTreeLayers.TreePathString;
                                            description = getTreeLayers.Description;
                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                        }
                                        else
                                            canDo = false;
                                    }
                                    else
                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: path).FullName);
                                    if (canDo)
                                    {
                                        var codePage = ShapeFileHelper.ShapeFile.GetDbfCodePage
                                        (
                                            dbfFileName: Path.Combine
                                            (
                                                path1: Path.GetDirectoryName(path: path) ?? "",
                                                path2: Path.GetFileNameWithoutExtension(path: path) + ".dbf"
                                            )
                                        );
                                        using var shapeFile = new ShapeFileHelper.ShapeFileReader();
                                        shapeFile.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            vectorBackgroundWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: thisEvent.Message ?? string.Empty);
                                        };
                                        shapeFile.Open(filePath: path, defaultCodePage: codePage.CodePage, projection: projectionX);
                                        if (shapeFile.RecordCount == 0)
                                            return "No features found";
                                        shapeFile.Fire(message: $"[{path}] preprocessing ...", code: 0);
                                        var getFileInfo = shapeFile.GetCapabilities();
                                        var getFileType = $"{getFileInfo[propertyName: "fileType"]}";
                                        var fields = shapeFile.GetField();
                                        var haveFields = fields.Count > 0;
                                        var featureCollectionX = new XElement(
                                            name: "FeatureCollection",
                                            content: new object[]
                                            {
                                                new XAttribute(name: "type", value: getFileType),
                                                new XAttribute(name: "timeStamp", value: $"{getFileInfo[propertyName: "timeStamp"]}"),
                                                new XElement(name: "name", content: theme)
                                            }
                                        );
                                        if (description != null)
                                            featureCollectionX.Add
                                            (
                                                content: new XElement
                                                (
                                                    name: "property",
                                                    content: description
                                                        .Elements()
                                                        .Select(selector: x =>
                                                            new XElement(name: $"{x.Name}", content: x.Value))
                                                )
                                            );
                                        var bbox = (JArray)getFileInfo[propertyName: "bbox"];
                                        featureCollectionX.Add(
                                            content: new XElement(
                                                name: "boundary",
                                                content: new object[]
                                                {
                                                    new XElement(name: "north", content: $"{bbox[index: 3]}"),
                                                    new XElement(name: "south", content: $"{bbox[index: 1]}"),
                                                    new XElement(name: "west", content: $"{bbox[index: 0]}"),
                                                    new XElement(name: "east", content: $"{bbox[index: 2]}")
                                                }
                                            )
                                        );
                                        var pointer = 0;
                                        var valid = 0;
                                        var getTreePath = treePathString;
                                        if (string.IsNullOrWhiteSpace(value: getTreePath))
                                            getTreePath = "Untitled";
                                        var treeNameArray = Regex.Split(input: getTreePath, pattern: @"[\/\\\|]+");
                                        var treeTimeStamp =
                                            $"{forest},{sequence},{DateTime.Parse(s: $"{getFileInfo[propertyName: "timeStamp"]}"): yyyyMMdd,HHmmss}";
                                        var treeResult =
                                            oneForest.Tree(
                                                timestamp: treeTimeStamp,
                                                treeX: featureCollectionX,
                                                uri: path,
                                                forestStatus: status
                                            );
                                        if (treeResult.Success)
                                        {
                                            var treeId = treeResult.Id;
                                            var treeType = new List<int>();
                                            var isOk = true;
                                            var recordCount = shapeFile.RecordCount;
                                            XElement themeMetadataX = null;
                                            var canWork = true;
                                            if (!_noPromptMetaData)
                                            {
                                                var metaData = new MetaDataForm();
                                                metaData.ShowDialog();
                                                if (metaData.Ok)
                                                {
                                                    themeMetadataX = metaData.MetaDataX;
                                                    _noPromptMetaData = metaData.DonotPrompt;
                                                }
                                                else
                                                    canWork = false;
                                            }
                                            if (canWork)
                                            {
                                                XElement layerX = null;
                                                for (var index = treeNameArray.Length - 1; index >= 0; index--)
                                                    layerX = new XElement(
                                                        name: "layer",
                                                        content: new object[]
                                                        {
                                                        new XElement(name: "name",
                                                            content: treeNameArray[index].Trim()),
                                                        index == treeNameArray.Length - 1 ? themeMetadataX : null,
                                                        index == treeNameArray.Length - 1
                                                            ? new XElement(name: "member")
                                                            : null,
                                                        layerX
                                                        }
                                                    );
                                                featureCollectionX.Add(content: layerX);
                                                var createRoute = oneForest.Branch(
                                                    forest: forest,
                                                    sequence: sequence,
                                                    tree: treeId,
                                                    leafX: featureCollectionX.Descendants(name: "member").First(),
                                                    leafRootX: featureCollectionX
                                                );
                                                if (!createRoute.Success)
                                                {
                                                    if (isOk)
                                                        Invoke(
                                                            method: () =>
                                                            {
                                                                statusCell.Value = "✘";
                                                                DatabaseLogAdd(input: statusCell.ToolTipText = createRoute.Message);
                                                            }
                                                        );
                                                    isOk = false;
                                                }
                                                else
                                                {
                                                    var timeWatch = new Stopwatch();
                                                    timeWatch.Start();
                                                    foreach (var feature in shapeFile.GetFeature().AsParallel())
                                                    {
                                                        if (vectorBackgroundWorker.CancellationPending)
                                                        {
                                                            e.Cancel = true;
                                                            break;
                                                        }
                                                        pointer++;
                                                        if (feature != null)
                                                        {
                                                            var featureType =
                                                                $"{feature[propertyName: "geometry"][key: "type"]}";
                                                            shapeFile.Fire(
                                                                message: $"{featureType} [{pointer} / {recordCount}]",
                                                                code: 1,
                                                                progress: (int)(100 * pointer / recordCount)
                                                            );
                                                            var featureId = $"{feature[propertyName: "id"]}";
                                                            try
                                                            {
                                                                XElement styleX = null;
                                                                var style = feature[propertyName: "style"];
                                                                if (style != null)
                                                                    styleX = new XElement(
                                                                        name: "style",
                                                                        content: ((JObject)style).Properties()
                                                                        .Select(
                                                                            selector: field =>
                                                                                new XElement(
                                                                                    name: field.Name,
                                                                                    content: field.Value.ToString()
                                                                                )
                                                                        )
                                                                    );
                                                                XElement elementDescriptionX;
                                                                if (haveFields)
                                                                {
                                                                    var fieldValues =
                                                                        ((JObject)feature[propertyName: "properties"])
                                                                        .Properties()
                                                                        .Select(selector: field =>
                                                                            $"{field.Value[key: "value"]}")
                                                                        .ToArray();
                                                                    elementDescriptionX = new XElement(name: "property");
                                                                    for (var item = 0; item < fields.Count; item++)
                                                                    {
                                                                        var fieldName = Regex.Replace(
                                                                            input:
                                                                            $"{fields[index: item][propertyName: "name"]}",
                                                                            pattern: @"[:""（）\(\)]+", replacement: "_",
                                                                            options: RegexOptions.IgnoreCase |
                                                                                     RegexOptions.Singleline |
                                                                                     RegexOptions.Multiline);
                                                                        elementDescriptionX.Add(
                                                                            content: new XElement(name: fieldName,
                                                                                content: fieldValues[item]));
                                                                    }
                                                                }
                                                                else
                                                                    elementDescriptionX = null;
                                                                var centroidArray = feature[propertyName: "centroid"];
                                                                var featureBoxArray = feature[propertyName: "bbox"];
                                                                if (!centroidArray.HasValues || !featureBoxArray.HasValues)
                                                                {
                                                                    if (isOk)
                                                                        Invoke(
                                                                            method: () =>
                                                                            {
                                                                                statusCell.Value = "!";
                                                                                DatabaseLogAdd(input: statusCell.ToolTipText =
                                                                                    $"Feature Id {featureId} : Invalid geometry");
                                                                            }
                                                                        );
                                                                    isOk = false;
                                                                    continue;
                                                                }
                                                                var featureCoordinates =
                                                                    GeositeXML.OGCformat.GeometryToWkt(geometry: (
                                                                        (JArray)feature[propertyName: "geometry"][
                                                                            key: "coordinates"], featureType));
                                                                var featureCentroid =
                                                                    GeositeXML.OGCformat.GeometryToWkt(geometry: (
                                                                        (JArray)centroidArray, "Point"));
                                                                var featureBox = (JArray)featureBoxArray;
                                                                var featureBoundary = GeositeXML.OGCformat.GeometryToWkt(
                                                                    geometry: (
                                                                        JArray.Parse(
                                                                            json:
                                                                            $"[[[{featureBox[index: 0]},{featureBox[index: 1]}],[{featureBox[index: 2]},{featureBox[index: 1]}],[{featureBox[index: 2]},{featureBox[index: 3]}],[{featureBox[index: 0]},{featureBox[index: 3]}],[{featureBox[index: 0]},{featureBox[index: 1]}]]]"),
                                                                        "Polygon"
                                                                    )
                                                                );
                                                                var featureTimeStamp = feature[propertyName: "timeStamp"]?.Value<string>();
                                                                var featureX = featureType switch
                                                                {
                                                                    "Point" or "MultiPoint" => new XElement(
                                                                        name: "member",
                                                                        content: new object[]
                                                                        {
                                                                        new XAttribute(name: "type", value: "Point"),
                                                                        new XAttribute(name: "typeCode", value: "1"),
                                                                        new XAttribute(name: "id", value: featureId),
                                                                        new XAttribute(name: "timeStamp", value: featureTimeStamp),
                                                                        new XAttribute(name: "rank", value: rank),
                                                                        elementDescriptionX,
                                                                        new XElement(
                                                                            name: "geometry",
                                                                            content: new object[]
                                                                            {
                                                                                new XAttribute(name: "format", value: "OGCWKT"),
                                                                                new XAttribute(name: "type", value: featureType),
                                                                                new XAttribute(name: "centroid", value: featureCentroid),
                                                                                new XAttribute(name: "boundary", value: featureBoundary),
                                                                                featureCoordinates
                                                                            }
                                                                        ),
                                                                        styleX
                                                                        }
                                                                    ),
                                                                    "LineString" or "MultiLineString" => new XElement(
                                                                        name: "member",
                                                                        content: new object[]
                                                                        {
                                                                        new XAttribute(name: "type", value: "Line"),
                                                                        new XAttribute(name: "typeCode", value: "2"),
                                                                        new XAttribute(name: "id", value: featureId),
                                                                        new XAttribute(name: "timeStamp",
                                                                            value: featureTimeStamp),
                                                                        new XAttribute(name: "rank", value: rank),
                                                                        elementDescriptionX,
                                                                        new XElement(
                                                                            name: "geometry",
                                                                            content: new object[]
                                                                            {
                                                                                new XAttribute(name: "format", value: "OGCWKT"),
                                                                                new XAttribute(name: "type", value: featureType),
                                                                                new XAttribute(name: "centroid", value: featureCentroid),
                                                                                new XAttribute(name: "boundary", value: featureBoundary),
                                                                                featureCoordinates
                                                                            }
                                                                        ),
                                                                        styleX
                                                                        }
                                                                    ),
                                                                    "Polygon" or "MultiPolygon" => new XElement(
                                                                        name: "member",
                                                                        content: new object[]
                                                                        {
                                                                        new XAttribute(name: "type", value: "Polygon"),
                                                                        new XAttribute(name: "typeCode", value: "3"),
                                                                        new XAttribute(name: "id", value: featureId),
                                                                        new XAttribute(name: "timeStamp", value: featureTimeStamp),
                                                                        new XAttribute(name: "rank", value: rank),
                                                                        elementDescriptionX,
                                                                        new XElement(
                                                                            name: "geometry",
                                                                            content: new object[]
                                                                            {
                                                                                new XAttribute(name: "format", value: "OGCWKT"),
                                                                                new XAttribute(name: "type", value: featureType),
                                                                                new XAttribute(name: "centroid", value: featureCentroid),
                                                                                new XAttribute(name: "boundary", value: featureBoundary),
                                                                                featureCoordinates
                                                                            }
                                                                        ),
                                                                        styleX
                                                                        }
                                                                    ),
                                                                    _ => null
                                                                };
                                                                if (featureX != null)
                                                                {
                                                                    var createLeaf = oneForest.Leaf(
                                                                        route: createRoute.Route,
                                                                        leafX: featureX,
                                                                        timestamp:
                                                                        $"{DateTime.Parse(s: featureX.Attribute(name: "timeStamp").Value): yyyyMMdd,HHmmss}",
                                                                        topology: doTopology
                                                                    );
                                                                    if (createLeaf.Success)
                                                                    {
                                                                        var theTreeType = createLeaf.Type;
                                                                        if (!treeType.Contains(item: theTreeType))
                                                                        {
                                                                            treeType.Add(item: theTreeType);
                                                                            oneForest.Tree(
                                                                                enclosure:
                                                                                (
                                                                                    treeId,
                                                                                    treeType,
                                                                                    false
                                                                                )
                                                                            );
                                                                        }
                                                                        valid++;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (isOk)
                                                                            Invoke(
                                                                                method: () =>
                                                                                {
                                                                                    statusCell.Value = "!";
                                                                                    DatabaseLogAdd(input: statusCell.ToolTipText = $"Feature Id {featureId} : {createLeaf.Message}");
                                                                                }
                                                                            );
                                                                        isOk = false;
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception localError)
                                                            {
                                                                if (isOk)
                                                                    Invoke(
                                                                        method: () =>
                                                                        {
                                                                            statusCell.Value = "!";
                                                                            DatabaseLogAdd(input: statusCell.ToolTipText = $"Feature Id {featureId} : {localError.Message}");
                                                                        }
                                                                    );
                                                                isOk = false;
                                                            }
                                                        }
                                                    }
                                                    timeWatch.Stop();
                                                    var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
                                                    var message = $"[{valid} feature{(valid > 1 ? "s" : "")}] have been pushed - {duration}";
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            DatabaseLogAdd(input: statusCell.ToolTipText = message);
                                                        }
                                                    );
                                                    shapeFile.Fire(
                                                        message: " " + message,
                                                        code: 200);
                                                }
                                                oneForest.Tree(
                                                    enclosure:
                                                    (
                                                        treeId,
                                                        treeType,
                                                        isOk
                                                    )
                                                );
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        _databaseGridObject?.Reset();
                                                        _catalogTreeObject?.Reset();
                                                    }
                                                );
                                                if (isOk)
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            statusCell.Value = "✔";
                                                            statusCell.ToolTipText = "OK";
                                                        }
                                                    );
                                            }
                                            else
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        statusCell.Value = "?";
                                                        DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                                    }
                                                );
                                        }
                                        else
                                            Invoke(
                                                method: () =>
                                                {
                                                    statusCell.Value = "✘";
                                                    DatabaseLogAdd(input: statusCell.ToolTipText = treeResult.Message);
                                                }
                                            );
                                    }
                                    else
                                        Invoke(
                                            method: () =>
                                            {
                                                statusCell.Value = "?";
                                                DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                            }
                                        );
                                }
                                catch (Exception error)
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            statusCell.Value = "!";
                                            DatabaseLogAdd(input: statusCell.ToolTipText = error.Message);
                                        }
                                    );
                                }
                            }
                            break;
                        case ".txt":
                        case ".csv":
                        case ".xls":
                        case ".xlsx":
                        case ".xlsb":
                            {
                                try
                                {
                                    var freeTextFields = fileType switch
                                    {
                                        ".txt" => TXT.GetFieldNames(file: path),
                                        ".csv" => CSV.GetFieldNames(file: path),
                                        _ => FreeText.Excel.Excel.GetFieldNames(file: path)
                                    };
                                    if (freeTextFields.Length == 0)
                                        throw new Exception(message: "No valid fields found");
                                    string coordinateFieldName;
                                    if (freeTextFields.Any(predicate: f => f == "_position_"))
                                        coordinateFieldName = "_position_";
                                    else
                                    {
                                        var txtForm = new FreeTextFieldForm(fieldNames: freeTextFields);
                                        txtForm.ShowDialog();
                                        var choice = txtForm.Ok;
                                        if (choice == null)
                                            throw new Exception(message: "Task Cancellation");
                                        coordinateFieldName = choice.Value ? txtForm.CoordinateFieldName : null;
                                    }
                                    string treePathString = null;
                                    XElement description = null;
                                    var canDo = true;
                                    if (!_noPromptLayersBuilder)
                                    {
                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: path).FullName);
                                        getTreeLayers.ShowDialog();
                                        if (getTreeLayers.Ok)
                                        {
                                            treePathString = getTreeLayers.TreePathString;
                                            description = getTreeLayers.Description;
                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                        }
                                        else
                                            canDo = false;
                                    }
                                    else
                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: path).FullName);
                                    if (canDo)
                                    {
                                        FreeText.FreeText freeText = fileType switch
                                        {
                                            ".txt" => new TXT(coordinateFieldName: coordinateFieldName),
                                            ".csv" => new CSV(coordinateFieldName: coordinateFieldName),
                                            _ => new FreeText.Excel.Excel(coordinateFieldName: coordinateFieldName)
                                        };
                                        freeText.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            vectorBackgroundWorker.ReportProgress(
                                                percentProgress: thisEvent.Progress ?? -1,
                                                userState: thisEvent.Message ?? string.Empty);
                                        };
                                        freeText.Open(file: path, projection: projectionX);
                                        if (freeText.RecordCount == 0)
                                            return "No features found";
                                        freeText.Fire(message: $"[{path}] preprocessing ...", code: 0);
                                        var getFileInfo = freeText.GetCapabilities();
                                        var getFileType = getFileInfo[propertyName: "fileType"]?.ToString();
                                        var fields = freeText.GetField();
                                        var haveFields = fields.Length > 0;
                                        var featureCollectionX = new XElement(
                                            name: "FeatureCollection",
                                            content: new object[]
                                            {
                                                new XAttribute(name: "type", value: getFileType ?? ""),
                                                new XAttribute(name: "timeStamp", value: $"{getFileInfo[propertyName: "timeStamp"]}"),
                                                new XElement(name: "name", content: theme)
                                            }
                                        );
                                        if (description != null)
                                            featureCollectionX.Add
                                            (
                                                content: new XElement(
                                                    name: "property",
                                                    content: description
                                                        .Elements()
                                                        .Select(selector: x =>
                                                            new XElement(name: $"{x.Name}", content: x.Value))
                                                )
                                            );
                                        var boundaryBox = (JArray)getFileInfo[propertyName: "bbox"];
                                        if (boundaryBox != null)
                                            featureCollectionX.Add(
                                                content: new XElement(
                                                    name: "boundary",
                                                    content: new object[]
                                                    {
                                                        new XElement(name: "north", content: $"{boundaryBox[index: 3]}"),
                                                        new XElement(name: "south", content: $"{boundaryBox[index: 1]}"),
                                                        new XElement(name: "west", content: $"{boundaryBox[index: 0]}"),
                                                        new XElement(name: "east", content: $"{boundaryBox[index: 2]}")
                                                    }
                                                )
                                            );
                                        var pointer = 0;
                                        var valid = 0;
                                        var treePath = treePathString;
                                        if (string.IsNullOrWhiteSpace(value: treePath))
                                            treePath = "Untitled";
                                        var treeNameArray = Regex.Split(input: treePath, pattern: @"[\/\\\|]+");
                                        var treeTimeStamp =
                                            $"{forest},{sequence},{DateTime.Parse(s: $"{getFileInfo[propertyName: "timeStamp"]}"): yyyyMMdd,HHmmss}";
                                        var treeResult =
                                            oneForest.Tree(
                                                timestamp: treeTimeStamp,
                                                treeX: featureCollectionX,
                                                uri: path,
                                                forestStatus: status
                                            );
                                        if (treeResult.Success)
                                        {
                                            var treeId = treeResult.Id;
                                            var treeType = new List<int>();
                                            var isOk = true;
                                            var recordCount = freeText.RecordCount;
                                            XElement themeMetadataX = null;
                                            var canWork = true;
                                            if (!_noPromptMetaData)
                                            {
                                                var metaData = new MetaDataForm();
                                                metaData.ShowDialog();
                                                if (metaData.Ok)
                                                {
                                                    themeMetadataX = metaData.MetaDataX;
                                                    _noPromptMetaData = metaData.DonotPrompt;
                                                }
                                                else
                                                    canWork = false;
                                            }

                                            if (canWork)
                                            {
                                                XElement layerX = null;
                                                for (var index = treeNameArray.Length - 1; index >= 0; index--)
                                                    layerX = new XElement(
                                                        name: "layer",
                                                        content: new object[]
                                                        {
                                                        new XElement(name: "name", content: treeNameArray[index].Trim()),
                                                        index == treeNameArray.Length - 1 ? themeMetadataX : null,
                                                        index == treeNameArray.Length - 1 ? new XElement(name: "member") : null,
                                                        layerX
                                                        }
                                                    );
                                                featureCollectionX.Add(content: layerX);
                                                var createRoute = oneForest.Branch(
                                                    forest: forest,
                                                    sequence: sequence,
                                                    tree: treeId,
                                                    leafX: featureCollectionX.Descendants(name: "member").First(),
                                                    leafRootX: featureCollectionX
                                                );
                                                if (!createRoute.Success)
                                                {
                                                    if (isOk)
                                                        Invoke(
                                                            method: () =>
                                                            {
                                                                statusCell.Value = "✘";
                                                                DatabaseLogAdd(input: statusCell.ToolTipText = createRoute.Message);
                                                            }
                                                        );
                                                    isOk = false;
                                                }
                                                else
                                                {
                                                    var timeWatch = new Stopwatch();
                                                    timeWatch.Start();
                                                    foreach (var feature in freeText.GetFeature().AsParallel())
                                                    {
                                                        if (vectorBackgroundWorker.CancellationPending)
                                                        {
                                                            e.Cancel = true;
                                                            break;
                                                        }
                                                        pointer++;
                                                        if (feature != null)
                                                        {
                                                            var featureType = feature[propertyName: "geometry"]?["type"]?.ToString();
                                                            freeText.Fire(
                                                                message:
                                                                $"{(string.IsNullOrWhiteSpace(value: featureType) ? "NonGeometry" : featureType)} [{pointer} / {recordCount}]",
                                                                code: 1,
                                                                progress: 100 * pointer / recordCount
                                                            );
                                                            var featureId = $"{feature[propertyName: "id"]}";
                                                            try
                                                            {
                                                                XElement elementDescriptionX;
                                                                if (haveFields)
                                                                {
                                                                    var fieldValues =
                                                                        ((JObject)feature[propertyName: "properties"])
                                                                        .Properties()
                                                                        .Select(selector: field => $"{field.Value[key: "value"]}")
                                                                        .ToArray();
                                                                    elementDescriptionX = new XElement(name: "property");
                                                                    for (var item = 0; item < fields.Length; item++)
                                                                        elementDescriptionX.Add(
                                                                            content: new XElement(
                                                                                name: Regex.Replace(
                                                                                    input:
                                                                                    $"{fields[item][propertyName: "name"]}",
                                                                                    pattern: @"[:""（）\(\)]+", replacement: "_",
                                                                                    options: RegexOptions.IgnoreCase |
                                                                                    RegexOptions.Singleline |
                                                                                    RegexOptions.Multiline),
                                                                                content: fieldValues[item]
                                                                            )
                                                                        );
                                                                }
                                                                else
                                                                    elementDescriptionX = null;
                                                                string featureCoordinates = null;
                                                                string featureCentroid = null;
                                                                string featureBoundary = null;
                                                                if (!string.IsNullOrWhiteSpace(value: featureType))
                                                                {
                                                                    var centroidArray = feature[propertyName: "centroid"];
                                                                    var featureBoxArray = feature[propertyName: "bbox"];
                                                                    featureCoordinates =
                                                                        GeositeXML.OGCformat.GeometryToWkt(geometry: (
                                                                            (JArray)feature[propertyName: "geometry"][
                                                                                key: "coordinates"], featureType));
                                                                    featureCentroid =
                                                                        GeositeXML.OGCformat.GeometryToWkt(geometry: (
                                                                            (JArray)centroidArray, "Point"));
                                                                    var featureBox = (JArray)featureBoxArray;
                                                                    featureBoundary = GeositeXML.OGCformat.GeometryToWkt(
                                                                        geometry: (
                                                                            JArray.Parse(
                                                                                json:
                                                                                $"[[[{featureBox[index: 0]},{featureBox[index: 1]}],[{featureBox[index: 2]},{featureBox[index: 1]}],[{featureBox[index: 2]},{featureBox[index: 3]}],[{featureBox[index: 0]},{featureBox[index: 3]}],[{featureBox[index: 0]},{featureBox[index: 1]}]]]"),
                                                                            "Polygon"
                                                                        )
                                                                    );
                                                                }
                                                                var featureTimeStamp = feature[propertyName: "timeStamp"]?.Value<string>();
                                                                var featureX = featureType switch
                                                                {
                                                                    "Point" or "MultiPoint" =>
                                                                        new XElement(
                                                                            name: "member",
                                                                            content: new object[]
                                                                            {
                                                                            new XAttribute(name: "type",
                                                                                value: "Point"),
                                                                            new XAttribute(name: "typeCode",
                                                                                value: "1"),
                                                                            new XAttribute(name: "id",
                                                                                value: featureId),
                                                                            new XAttribute(name: "timeStamp",
                                                                                value: featureTimeStamp),
                                                                            new XAttribute(name: "rank", value: rank),
                                                                            elementDescriptionX,
                                                                            new XElement(
                                                                                name: "geometry",
                                                                                content: new object[]
                                                                                {
                                                                                    new XAttribute(name: "format",
                                                                                        value: "OGCWKT"),
                                                                                    new XAttribute(name: "type",
                                                                                        value: featureType),
                                                                                    new XAttribute(name: "centroid",
                                                                                        value: featureCentroid),
                                                                                    new XAttribute(name: "boundary",
                                                                                        value: featureBoundary),
                                                                                    featureCoordinates
                                                                                }
                                                                            )
                                                                            }
                                                                        ),
                                                                    "LineString" or "MultiLineString" =>
                                                                        new XElement(
                                                                            name: "member",
                                                                            content: new object[]
                                                                            {
                                                                            new XAttribute(name: "type", value: "Line"),
                                                                            new XAttribute(name: "typeCode",
                                                                                value: "2"),
                                                                            new XAttribute(name: "id",
                                                                                value: featureId),
                                                                            new XAttribute(name: "timeStamp",
                                                                                value: featureTimeStamp),
                                                                            new XAttribute(name: "rank", value: rank),
                                                                            elementDescriptionX,
                                                                            new XElement(
                                                                                name: "geometry",
                                                                                content: new object[]
                                                                                {
                                                                                    new XAttribute(name: "format",
                                                                                        value: "OGCWKT"),
                                                                                    new XAttribute(name: "type",
                                                                                        value: featureType),
                                                                                    new XAttribute(name: "centroid",
                                                                                        value: featureCentroid),
                                                                                    new XAttribute(name: "boundary",
                                                                                        value: featureBoundary),
                                                                                    featureCoordinates
                                                                                }
                                                                            )
                                                                            }
                                                                        ),
                                                                    "Polygon" or "MultiPolygon" =>
                                                                        new XElement(
                                                                            name: "member",
                                                                            content: new object[]
                                                                            {
                                                                            new XAttribute(name: "type", value: "Polygon"),
                                                                            new XAttribute(name: "typeCode", value: "3"),
                                                                            new XAttribute(name: "id", value: featureId),
                                                                            new XAttribute(name: "timeStamp", value: featureTimeStamp),
                                                                            new XAttribute(name: "rank", value: rank),
                                                                            elementDescriptionX,
                                                                            new XElement(
                                                                                name: "geometry",
                                                                                content: new object[]
                                                                                {
                                                                                    new XAttribute(name: "format", value: "OGCWKT"),
                                                                                    new XAttribute(name: "type", value: featureType),
                                                                                    new XAttribute(name: "centroid", value: featureCentroid),
                                                                                    new XAttribute(name: "boundary", value: featureBoundary),
                                                                                    featureCoordinates
                                                                                }
                                                                            )
                                                                            }
                                                                        ),
                                                                    _ => new XElement(
                                                                        name: "member",
                                                                        content: new object[]
                                                                        {
                                                                        new XAttribute(name: "type", value: ""),
                                                                        new XAttribute(name: "typeCode", value: "0"),
                                                                        new XAttribute(name: "id", value: featureId),
                                                                        new XAttribute(name: "timeStamp", value: featureTimeStamp),
                                                                        new XAttribute(name: "rank", value: rank),
                                                                        elementDescriptionX
                                                                        }
                                                                    )
                                                                };
                                                                if (featureX != null)
                                                                {
                                                                    var createLeaf = oneForest.Leaf(
                                                                        route: createRoute.Route,
                                                                        leafX: featureX,
                                                                        timestamp: $"{DateTime.Parse(s: featureX.Attribute(name: "timeStamp").Value): yyyyMMdd,HHmmss}",
                                                                        topology: doTopology
                                                                    );
                                                                    if (createLeaf.Success)
                                                                    {
                                                                        var theTreeType = createLeaf.Type;
                                                                        if (!treeType.Contains(item: theTreeType))
                                                                        {
                                                                            treeType.Add(item: theTreeType);
                                                                            oneForest.Tree(
                                                                                enclosure:
                                                                                (
                                                                                    treeId,
                                                                                    treeType,
                                                                                    false
                                                                                )
                                                                            );
                                                                        }
                                                                        valid++;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (isOk)
                                                                            Invoke(
                                                                                method: () =>
                                                                                {
                                                                                    statusCell.Value = "!";
                                                                                    DatabaseLogAdd(input: statusCell.ToolTipText = $"Feature Id {featureId} : {createLeaf.Message}");
                                                                                }
                                                                            );
                                                                        isOk = false;
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception localError)
                                                            {
                                                                if (isOk)
                                                                    Invoke(
                                                                        method: () =>
                                                                        {
                                                                            statusCell.Value = "!";
                                                                            DatabaseLogAdd(input: statusCell.ToolTipText = localError.Message);
                                                                        }
                                                                    );
                                                                isOk = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (isOk)
                                                            {
                                                                var localPointer = pointer;
                                                                Invoke(
                                                                    method: () =>
                                                                    {
                                                                        statusCell.Value = "!";
                                                                        DatabaseLogAdd(input: statusCell.ToolTipText = $"Feature order {localPointer} : {freeText.ErrorMessage}");
                                                                    }
                                                                );
                                                            }
                                                            isOk = false;
                                                        }
                                                    }
                                                    timeWatch.Stop();
                                                    var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
                                                    var message = $"[{valid} feature{(valid > 1 ? "s" : "")}] have been pushed - {duration}";
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            DatabaseLogAdd(input: statusCell.ToolTipText = message);
                                                        }
                                                    );
                                                    freeText.Fire(
                                                        message: " " + message,
                                                        code: 200);
                                                }
                                                oneForest.Tree(enclosure: (treeId, treeType, isOk));
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        _databaseGridObject?.Reset();
                                                        _catalogTreeObject?.Reset();
                                                    }
                                                );
                                                if (isOk)
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            statusCell.Value = "✔";
                                                            statusCell.ToolTipText = "OK";
                                                        }
                                                    );
                                            }
                                            else
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        statusCell.Value = "?";
                                                        DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                                    }
                                                );
                                        }
                                        else
                                            Invoke(
                                                method: () =>
                                                {
                                                    statusCell.Value = "✘";
                                                    DatabaseLogAdd(input: statusCell.ToolTipText = treeResult.Message);
                                                }
                                            );
                                    }
                                    else
                                        Invoke(
                                            method: () =>
                                            {
                                                statusCell.Value = "?";
                                                DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                            }
                                        );
                                }
                                catch (Exception error)
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            statusCell.Value = "!";
                                            DatabaseLogAdd(input: statusCell.ToolTipText = error.Message);
                                        }
                                    );
                                }
                            }
                            break;
                        case ".xml":
                            {
                                try
                                {
                                    XElement description = null;
                                    var canDo = true;
                                    if (!_noPromptLayersBuilder)
                                    {
                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: path).FullName);
                                        getTreeLayers.ShowDialog();
                                        if (getTreeLayers.Ok)
                                        {
                                            description = getTreeLayers.Description;
                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                        }
                                        else
                                            canDo = false;
                                    }
                                    if (canDo)
                                    {
                                        using var xml = new GeositeXml.GeositeXml(projection: projectionX);
                                        xml.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            vectorBackgroundWorker.ReportProgress(
                                                percentProgress: thisEvent.Progress ?? -1,
                                                userState: thisEvent.Message ?? string.Empty
                                            );
                                        };
                                        xml.Fire(message: $"[{path}] preprocessing ...", code: 0);
                                        var getGeositeXml = xml.GeositeXmlToGeositeXml(geositeXml: xml.GetTree(input: path), output: null, extraDescription: description);
                                        var featureCollectionX = getGeositeXml.Root;
                                        if (featureCollectionX != null)
                                        {
                                            featureCollectionX.Element(name: "name").Value = theme;
                                            var treeTimeStamp = $"{forest},{sequence},{DateTime.Parse(s: featureCollectionX?.Attribute(name: "timeStamp")?.Value ?? DateTime.Now.ToString(format: "s")): yyyyMMdd,HHmmss}";
                                            var treeResult =
                                                oneForest.Tree(
                                                    timestamp: treeTimeStamp,
                                                    treeX: featureCollectionX,
                                                    uri: path,
                                                    forestStatus: status
                                                );
                                            if (treeResult.Success)
                                            {
                                                var treeId = treeResult.Id;
                                                var treeType = new List<int>();
                                                var isOk = true;
                                                foreach
                                                (
                                                    var leafArray in new[] { "member", "Member", "MEMBER" }
                                                        .Select
                                                        (
                                                            selector: leafName => featureCollectionX.DescendantsAndSelf(name: leafName).ToList()
                                                        )
                                                        .Where
                                                        (
                                                            predicate: leafX => leafX.Any()
                                                        )
                                                )
                                                {
                                                    var leafCount = leafArray.Count;
                                                    if (leafCount > 0)
                                                    {
                                                        var leafPointer = 0;
                                                        var valid = 0;
                                                        var timeWatch = new Stopwatch();
                                                        timeWatch.Start();
                                                        foreach (var leafX in leafArray.AsParallel())
                                                        {
                                                            if (vectorBackgroundWorker.CancellationPending)
                                                            {
                                                                e.Cancel = true;
                                                                break;
                                                            }
                                                            var createRoute = oneForest.Branch(
                                                                forest: forest,
                                                                sequence: sequence,
                                                                tree: treeId,
                                                                leafX: leafX,
                                                                leafRootX: featureCollectionX
                                                            );
                                                            if (!createRoute.Success)
                                                            {
                                                                if (isOk)
                                                                    Invoke(
                                                                        method: () =>
                                                                        {
                                                                            statusCell.Value = "✘";
                                                                            DatabaseLogAdd(input: statusCell.ToolTipText =
                                                                                createRoute.Message);
                                                                        }
                                                                    );
                                                                isOk = false;
                                                            }
                                                            else
                                                            {
                                                                ++leafPointer;
                                                                leafX.SetAttributeValue(name: "rank", value: rank);
                                                                var leafType = leafX.Attribute(name: "type")?.Value;
                                                                var createLeaf = oneForest.Leaf(
                                                                    route: createRoute.Route,
                                                                    leafX: leafX,
                                                                    timestamp:
                                                                    $"{DateTime.Parse(s: leafX?.Attribute(name: "timeStamp")?.Value ?? DateTime.Now.ToString(format: "s")): yyyyMMdd,HHmmss}",
                                                                    topology: doTopology
                                                                );
                                                                if (createLeaf.Success)
                                                                {
                                                                    xml.Fire(
                                                                        message:
                                                                        $"{leafType} [{leafPointer} / {leafCount}]",
                                                                        code: 1,
                                                                        progress: 100 * leafPointer / leafCount
                                                                    );
                                                                    var theTreeType = createLeaf.Type;
                                                                    if (!treeType.Contains(item: theTreeType))
                                                                    {
                                                                        treeType.Add(item: theTreeType);
                                                                        oneForest.Tree(
                                                                            enclosure:
                                                                            (
                                                                                treeId,
                                                                                treeType,
                                                                                false
                                                                            )
                                                                        );
                                                                    }
                                                                    valid++;
                                                                }
                                                                else
                                                                {
                                                                    if (isOk)
                                                                        Invoke(
                                                                            method: () =>
                                                                            {
                                                                                statusCell.Value = "!";
                                                                                DatabaseLogAdd(input: statusCell.ToolTipText =
                                                                                    createLeaf.Message);
                                                                            }
                                                                        );
                                                                    isOk = false;
                                                                }
                                                            }
                                                        }
                                                        timeWatch.Stop();
                                                        var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
                                                        var message = $"[{valid} feature{(valid > 1 ? "s" : "")}] have been pushed - {duration}";
                                                        Invoke(
                                                            method: () =>
                                                            {
                                                                DatabaseLogAdd(input: statusCell.ToolTipText = message);
                                                            }
                                                        );
                                                        xml.Fire(message: " " + message, code: 200);
                                                    }
                                                    break;
                                                }
                                                oneForest.Tree(enclosure: (treeId, treeType, isOk));
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        _databaseGridObject?.Reset();
                                                        _catalogTreeObject?.Reset();
                                                    }
                                                );
                                                if (isOk)
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            statusCell.Value = "✔";
                                                            statusCell.ToolTipText = "OK";
                                                        }
                                                    );
                                            }
                                            else
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        statusCell.Value = "✘";
                                                        DatabaseLogAdd(input: statusCell.ToolTipText = treeResult.Message);
                                                    }
                                                );
                                        }
                                        else
                                            throw new Exception(message: "Nothing");
                                    }
                                    else
                                        Invoke(
                                            method: () =>
                                            {
                                                statusCell.Value = "?";
                                                DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                            }
                                        );
                                }
                                catch (Exception error)
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            statusCell.Value = "!";
                                            DatabaseLogAdd(input: statusCell.ToolTipText = error.Message);
                                        }
                                    );
                                }
                            }
                            break;
                        case ".kml":
                            {
                                try
                                {
                                    XElement description = null;
                                    var canDo = true;
                                    if (!_noPromptLayersBuilder)
                                    {
                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: path).FullName);
                                        getTreeLayers.ShowDialog();
                                        if (getTreeLayers.Ok)
                                        {
                                            description = getTreeLayers.Description;
                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                        }
                                        else
                                            canDo = false;
                                    }
                                    if (canDo)
                                    {
                                        using var kml = new GeositeXml.GeositeXml(projection: projectionX);
                                        kml.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            vectorBackgroundWorker.ReportProgress(
                                                percentProgress: thisEvent.Progress ?? -1,
                                                userState: thisEvent.Message ?? string.Empty
                                            );
                                        };
                                        kml.Fire(message: $"[{path}] preprocessing ...", code: 0);
                                        var featureCollectionX = kml.KmlToGeositeXml(kml: kml.GetTree(input: path),
                                            output: null, extraDescription: description).Root;
                                        featureCollectionX.Element(name: "name").Value = theme;
                                        var treeTimeStamp =
                                            $"{forest},{sequence},{DateTime.Parse(s: featureCollectionX?.Attribute(name: "timeStamp")?.Value ?? DateTime.Now.ToString(format: "s")): yyyyMMdd,HHmmss}";
                                        var treeResult =
                                            oneForest.Tree(
                                                timestamp: treeTimeStamp,
                                                treeX: featureCollectionX,
                                                uri: path,
                                                forestStatus: status
                                            );
                                        if (treeResult.Success)
                                        {
                                            var treeId = treeResult.Id;
                                            var treeType = new List<int>();
                                            var isOk = true;
                                            foreach
                                            (
                                                var leafArray in new[] { "member", "Member", "MEMBER" }
                                                    .Select
                                                    (
                                                        selector: leafName => featureCollectionX.DescendantsAndSelf(name: leafName).ToList()
                                                    )
                                                    .Where
                                                    (
                                                        predicate: leafX => leafX.Any()
                                                    )
                                            )
                                            {
                                                var leafCount = leafArray.Count;
                                                if (leafCount > 0)
                                                {
                                                    var leafPointer = 0;
                                                    var valid = 0;
                                                    var timeWatch = new Stopwatch();
                                                    timeWatch.Start();
                                                    foreach (var leafX in leafArray.AsParallel())
                                                    {
                                                        if (vectorBackgroundWorker.CancellationPending)
                                                        {
                                                            e.Cancel = true;
                                                            break;
                                                        }
                                                        var createRoute = oneForest.Branch(
                                                            forest: forest,
                                                            sequence: sequence,
                                                            tree: treeId,
                                                            leafX: leafX,
                                                            leafRootX: featureCollectionX
                                                        );
                                                        if (!createRoute.Success)
                                                        {
                                                            if (isOk)
                                                                Invoke(
                                                                    method: () =>
                                                                    {
                                                                        statusCell.Value = "✘";
                                                                        DatabaseLogAdd(input: statusCell.ToolTipText = createRoute.Message);
                                                                    }
                                                                );
                                                            isOk = false;
                                                        }
                                                        else
                                                        {
                                                            leafX.SetAttributeValue(name: "rank", value: rank);
                                                            var leafType = leafX.Attribute(name: "type")?.Value;
                                                            var createLeaf = oneForest.Leaf(
                                                                route: createRoute.Route,
                                                                leafX: leafX,
                                                                timestamp:
                                                                $"{DateTime.Parse(s: leafX?.Attribute(name: "timeStamp")?.Value ?? DateTime.Now.ToString(format: "s")): yyyyMMdd,HHmmss}",
                                                                topology: doTopology
                                                            );
                                                            ++leafPointer;
                                                            if (createLeaf.Success)
                                                            {
                                                                var theTreeType = createLeaf.Type;
                                                                kml.Fire(
                                                                    message: $"{leafType} [{leafPointer} / {leafCount}]",
                                                                    code: 1,
                                                                    progress: 100 * leafPointer / leafCount
                                                                );
                                                                if (!treeType.Contains(item: theTreeType))
                                                                {
                                                                    treeType.Add(item: theTreeType);
                                                                    oneForest.Tree(
                                                                        enclosure:
                                                                        (
                                                                            treeId,
                                                                            treeType,
                                                                            false
                                                                        )
                                                                    );
                                                                }
                                                                valid++;
                                                            }
                                                            else
                                                            {
                                                                if (isOk)
                                                                    Invoke(
                                                                        method: () =>
                                                                        {
                                                                            statusCell.Value = "!";
                                                                            DatabaseLogAdd(input: statusCell.ToolTipText =
                                                                                createLeaf.Message);
                                                                        }
                                                                    );
                                                                isOk = false;
                                                            }
                                                        }
                                                    }
                                                    timeWatch.Stop();
                                                    var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
                                                    var message = $"[{valid} feature{(valid > 1 ? "s" : "")}] have been pushed - {duration}";
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            DatabaseLogAdd(input: statusCell.ToolTipText = message);
                                                        }
                                                    );
                                                    kml.Fire(message: " " + message, code: 200);
                                                }
                                                break;
                                            }
                                            oneForest.Tree(
                                                enclosure:
                                                (
                                                    treeId,
                                                    treeType,
                                                    isOk
                                                )
                                            );
                                            Invoke(
                                                method: () =>
                                                {
                                                    _databaseGridObject?.Reset();
                                                    _catalogTreeObject?.Reset();
                                                }
                                            );
                                            if (isOk)
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        statusCell.Value = "✔";
                                                        statusCell.ToolTipText = "OK";
                                                    }
                                                );
                                        }
                                        else
                                            Invoke(
                                                method: () =>
                                                {
                                                    statusCell.Value = "✘";
                                                    DatabaseLogAdd(input: statusCell.ToolTipText = treeResult.Message);
                                                }
                                            );
                                    }
                                    else
                                        Invoke(
                                            method: () =>
                                            {
                                                statusCell.Value = "?";
                                                DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                            }
                                        );
                                }
                                catch (Exception error)
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            statusCell.Value = "!";
                                            DatabaseLogAdd(input: statusCell.ToolTipText = error.Message);
                                        }
                                    );
                                }
                            }
                            break;
                        case ".geojson":
                            {
                                try
                                {
                                    string treePathString = null;
                                    XElement description = null;
                                    var canDo = true;
                                    if (!_noPromptLayersBuilder)
                                    {
                                        var getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: path).FullName);
                                        getTreeLayers.ShowDialog();
                                        if (getTreeLayers.Ok)
                                        {
                                            treePathString = getTreeLayers.TreePathString;
                                            description = getTreeLayers.Description;
                                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                                        }
                                        else
                                            canDo = false;
                                    }
                                    else
                                        treePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: path).FullName);
                                    if (canDo)
                                    {
                                        using var geoJsonObject = new GeositeXml.GeositeXml(projection: projectionX);
                                        geoJsonObject.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                                        {
                                            vectorBackgroundWorker.ReportProgress(
                                                percentProgress: thisEvent.Progress ?? -1,
                                                userState: thisEvent.Message ?? string.Empty
                                            );
                                        };
                                        geoJsonObject.Fire(message: $"[{path}] preprocessing ...", code: 0);
                                        var getGeositeXml = new StringBuilder();
                                        geoJsonObject.GeoJsonToGeositeXml(
                                            input: path,
                                            output: getGeositeXml,
                                            treePath: treePathString,
                                            extraDescription: description
                                        );
                                        if (getGeositeXml.Length > 0)
                                        {
                                            var featureCollectionX = XElement.Parse(text: getGeositeXml.ToString());
                                            featureCollectionX.Element(name: "name").Value = theme;
                                            var treeTimeStamp =
                                                $"{forest},{sequence},{DateTime.Parse(s: featureCollectionX?.Attribute(name: "timeStamp")?.Value ?? DateTime.Now.ToString(format: "s")): yyyyMMdd,HHmmss}";
                                            var treeResult =
                                                oneForest.Tree(
                                                    timestamp: treeTimeStamp,
                                                    treeX: featureCollectionX,
                                                    uri: path,
                                                    forestStatus: status
                                                );
                                            if (treeResult.Success)
                                            {
                                                var treeId = treeResult.Id;
                                                var treeType = new List<int>();
                                                var isOk = true;
                                                foreach
                                                (
                                                    var leafArray in new[] { "member", "Member", "MEMBER" }
                                                        .Select
                                                        (
                                                            selector: leafName => featureCollectionX.DescendantsAndSelf(name: leafName).ToList()
                                                        )
                                                        .Where
                                                        (
                                                            predicate: leafX => leafX.Any()
                                                        )
                                                )
                                                {
                                                    var leafCount = leafArray.Count;
                                                    if (leafCount > 0)
                                                    {
                                                        var leafPointer = 0;
                                                        var valid = 0;
                                                        var timeWatch = new Stopwatch();
                                                        timeWatch.Start();
                                                        foreach (var leafX in leafArray.AsParallel())
                                                        {
                                                            if (vectorBackgroundWorker.CancellationPending)
                                                            {
                                                                e.Cancel = true;
                                                                break;
                                                            }
                                                            var createRoute = oneForest.Branch(
                                                                forest: forest,
                                                                sequence: sequence,
                                                                tree: treeId,
                                                                leafX: leafX,
                                                                leafRootX: featureCollectionX
                                                            );
                                                            if (!createRoute.Success)
                                                            {
                                                                if (isOk)
                                                                    Invoke(
                                                                        method: () =>
                                                                        {
                                                                            statusCell.Value = "✘";
                                                                            DatabaseLogAdd(input: statusCell.ToolTipText = createRoute.Message);
                                                                        }
                                                                    );
                                                                isOk = false;
                                                            }
                                                            else
                                                            {
                                                                leafX.SetAttributeValue(name: "rank", value: rank);
                                                                var leafType = leafX.Attribute(name: "type")?.Value;
                                                                var createLeaf = oneForest.Leaf(
                                                                    route: createRoute.Route,
                                                                    leafX: leafX,
                                                                    timestamp:
                                                                    $"{DateTime.Parse(s: leafX?.Attribute(name: "timeStamp")?.Value ?? DateTime.Now.ToString(format: "s")): yyyyMMdd,HHmmss}",
                                                                    topology: doTopology
                                                                );
                                                                ++leafPointer;
                                                                if (createLeaf.Success)
                                                                {
                                                                    var theTreeType = createLeaf.Type;
                                                                    if (!treeType.Contains(item: theTreeType))
                                                                    {
                                                                        geoJsonObject.Fire(
                                                                            message:
                                                                            $"{leafType} [{leafPointer} / {leafCount}]",
                                                                            code: 1,
                                                                            progress: 100 * leafPointer / leafCount
                                                                        );
                                                                        treeType.Add(item: theTreeType);
                                                                        oneForest.Tree(
                                                                            enclosure:
                                                                            (
                                                                                treeId,
                                                                                treeType,
                                                                                false
                                                                            )
                                                                        );
                                                                    }
                                                                    valid++;
                                                                }
                                                                else
                                                                {
                                                                    if (isOk)
                                                                    {
                                                                        Invoke(
                                                                            method: () =>
                                                                            {
                                                                                statusCell.Value = "!";
                                                                                DatabaseLogAdd(input: statusCell.ToolTipText = createLeaf.Message);
                                                                            }
                                                                        );
                                                                    }
                                                                    isOk = false;
                                                                }
                                                            }
                                                        }
                                                        timeWatch.Stop();
                                                        var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
                                                        var message = $"[{valid} feature{(valid > 1 ? "s" : "")}] have been pushed - {duration}";
                                                        Invoke(
                                                            method: () =>
                                                            {
                                                                DatabaseLogAdd(input: statusCell.ToolTipText = message);
                                                            }
                                                        );
                                                        geoJsonObject.Fire(
                                                            message:
                                                            " " + message,
                                                            code: 200);
                                                    }
                                                    break;
                                                }
                                                oneForest.Tree(
                                                    enclosure:
                                                    (
                                                        treeId,
                                                        treeType,
                                                        isOk
                                                    )
                                                );
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        _databaseGridObject?.Reset();
                                                        _catalogTreeObject?.Reset();
                                                    }
                                                );
                                                if (isOk)
                                                    Invoke(
                                                        method: () =>
                                                        {
                                                            statusCell.Value = "✔";
                                                            statusCell.ToolTipText = "OK";
                                                        }
                                                    );
                                            }
                                            else
                                                Invoke(
                                                    method: () =>
                                                    {
                                                        statusCell.Value = "✘";
                                                        DatabaseLogAdd(input: statusCell.ToolTipText = treeResult.Message);
                                                    }
                                                );
                                        }
                                        else
                                            Invoke(
                                                method: () =>
                                                {
                                                    statusCell.Value = "✘";
                                                    DatabaseLogAdd(input: statusCell.ToolTipText = "Fail");
                                                }
                                            );
                                    }
                                    else
                                        Invoke(
                                            method: () =>
                                            {
                                                statusCell.Value = "?";
                                                DatabaseLogAdd(input: statusCell.ToolTipText = "Cancelled");
                                            }
                                        );
                                }
                                catch (Exception error)
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            statusCell.Value = "!";
                                            DatabaseLogAdd(input: statusCell.ToolTipText = error.Message);
                                        }
                                    );
                                }
                            }
                            break;
                        default:
                            Invoke(
                                method: () =>
                                {
                                    statusCell.Value = "?";
                                    DatabaseLogAdd(input: statusCell.ToolTipText = "Unknown");
                                }
                            );
                            break;
                    }
                }
            }
            return statusInfo;
        }

        private void VectorWorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var userState = (string)e.UserState;
            var progressPercentage = e.ProgressPercentage;
            if (statusText != null && statusProgress != null && statusBar != null)
            {
                statusText.Text = userState;
                var percent = statusProgress.Value = progressPercentage is >= 0 and <= 100 ? progressPercentage : 0;
                if (percent % 10 == 0)
                    statusBar.Refresh();
            }
        }

        private void VectorWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusProgress.Visible = false;
            if (e.Error != null)
                MessageBox.Show(text: e.Error.Message, caption: @"Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            else if (e.Cancelled)
                statusText.Text = @"Suspended!";
            else if (e.Result != null)
                statusText.Text = (string)e.Result;
            var serverUrl = GeositeServerUrl.Text.Trim();
            var serverUser = GeositeServerUser.Text.Trim();
            var serverPassword = GeositeServerPassword.Text.Trim();
            UpdateDatabaseSize(serverUrl: serverUrl, serverUser: serverUser, serverPassword: serverPassword);
            PostgresRun.BackgroundImage = Properties.Resources.linkpush;
            OGCtoolTip.SetToolTip(PostgresRun, "Start");
            _loading.Run(onOff: false);
        }

        private void VectorFilePool_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var colIndex = e.ColumnIndex;
            var rowIndex = e.RowIndex;
            if (colIndex == 0)
            {
                var dataGridView = (DataGridView)sender;
                var col = dataGridView.Rows[index: rowIndex].Cells[index: colIndex];
                _dataPoolGridCellValue = $"{col.Value}".Trim();
            }
        }

        private void VectorFilePool_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var colIndex = e.ColumnIndex;
            var rowIndex = e.RowIndex;
            if (colIndex == 0)
            {
                var row = ((DataGridView)sender).Rows[index: rowIndex];
                var col = row.Cells[index: colIndex];
                var newName = $"{col.Value}".Trim();
                var oldName = _dataPoolGridCellValue;
                if (string.IsNullOrWhiteSpace(value: newName))
                    col.Value = oldName;
                else
                {
                    try
                    {
                        col.Value = new XElement(name: newName).Name.LocalName;
                    }
                    catch
                    {
                        col.Value = oldName;
                    }
                }
            }
            _dataPoolGridCellValue = null;
        }

        private void VectorFilePool_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;
            if (rowIndex >= 0)
            {
                var colIndex = e.ColumnIndex;
                //0=Layer 1=URI 2=Projection 3=※（Status） 
                if (colIndex == 2)
                {
                    var senderGrid = (DataGridView)sender;
                    var row = senderGrid.Rows[index: rowIndex];
                    var col = row.Cells[index: colIndex];
                    var path = (string)row.Cells[index: 1].Value;

                    var tagX = (XElement)col.Tag;
                    var fromX = tagX?.Element("From")?.Elements().FirstOrDefault();
                    var fromXName = fromX?.Name.LocalName;

                    var projectionFrom = new XElement("From");
                    switch (Path.GetExtension(path: path)?.ToLower())
                    {
                        case ".kml":
                        case ".xml":
                        case ".geojson":
                            {
                                projectionFrom.Add(new XElement("Geography"));
                                break;
                            }
                        default:
                            {
                                projectionFrom.Add(fromXName != null
                                    ? new XAttribute(
                                        "Active",
                                        fromXName == "Geography" ? 0 :
                                        fromXName == "Gauss-Kruger" ? 1 :
                                        fromXName == "Lambert" ? 2 :
                                        fromXName == "Albers" ? 3 : 4
                                    )
                                    : null);
                                projectionFrom.Add(new XElement("Geography"));
                                projectionFrom.Add(fromXName == "Gauss-Kruger"
                                    ? fromX
                                    : new XElement("Gauss-Kruger", new XElement("Zone", 6)));
                                projectionFrom.Add(fromXName == "Lambert" ? fromX : new XElement("Lambert"));
                                projectionFrom.Add(fromXName == "Albers" ? fromX : new XElement("Albers"));
                                projectionFrom.Add(new XElement("Web-Mercator"));
                                break;
                            }
                    }
                    var projectionForm =
                        new ProjectionForm(
                            new XElement(
                                "Projection",
                                projectionFrom,
                                new XElement("To", new XElement("Geography"))
                            )
                        );
                    projectionForm.ShowDialog();
                    var getProjectionSame = projectionForm.GetProjectionSame();
                    var projectionX = projectionForm.Projection;
                    if (projectionX != null)
                    {
                        var from = projectionX.Element("From")?.Elements().FirstOrDefault()?.Name.LocalName;
                        var to = projectionX.Element("To")?.Elements().FirstOrDefault()?.Name.LocalName;
                        if (getProjectionSame)
                        {
                            foreach (var oneRow in senderGrid.Rows)
                            {
                                var oneCol = ((DataGridViewRow)oneRow).Cells[2];
                                oneCol.Tag = projectionX;
                                oneCol.Value = "*";
                                oneCol.ToolTipText = @$"[{from}] to [{to}]";
                            }
                        }
                        else
                        {
                            col.Tag = projectionX;
                            col.Value = "*";
                            col.ToolTipText = @$"[{from}] to [{to}]";
                        }
                    }
                    else
                    {
                        if (getProjectionSame)
                        {
                            foreach (var oneRow in senderGrid.Rows)
                            {
                                var oneCol = ((DataGridViewRow)oneRow).Cells[2];
                                oneCol.Tag = null;
                                oneCol.Value = "?";
                                oneCol.ToolTipText = @"Unknown";
                            }
                        }
                        else
                        {
                            col.Tag = null;
                            col.Value = "?";
                            col.ToolTipText = @"Unknown";
                        }
                    }
                }
            }
        }

        private void PostgresLight_CheckedChanged(object sender, EventArgs e)
        {
            if (!PostgresLight.Checked)
                MessageBox.Show(
                    text: @"Unchecked means that the data is only provided for background calculation without sharing.",
                    caption: @"Tip", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
        }

        private void LocalTileOpen_Click(object sender, EventArgs e)
        {
            var key = localTileOpen.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFolderDialog = new FolderBrowserDialog
            {
                Description = @"Please select a folder",
                ShowNewFolderButton = false
            };
            if (Directory.Exists(path: oldPath))
                openFolderDialog.SelectedPath = oldPath;
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                RegEdit.Setkey(keyname: path, defaultvalue: openFolderDialog.SelectedPath);
                localTileFolder.Text = openFolderDialog.SelectedPath;
            }
            else
                localTileFolder.Text = string.Empty;
        }

        private void ModelOpen_Click(object sender, EventArgs e)
        {
            var key = ModelOpen.Name;
            if (!int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex))
                filterIndex = 0;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFileDialog = new OpenFileDialog()
            {
                Title = @"Please select raster file[s]",
                Filter = @"Raster|*.tif;*.tiff;*.hgt;*.img;*.jp2;*.j2k;*.vrt;*.sid;*.ecw",
                FilterIndex = filterIndex,
                Multiselect = true
            };
            if (Directory.Exists(path: oldPath))
                openFileDialog.InitialDirectory = oldPath;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RegEdit.Setkey(keyname: key, defaultvalue: $"{openFileDialog.FilterIndex}");
                    RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: openFileDialog.FileName));
                    ModelOpenTextBox.Text = string.Join(separator: "|", value: openFileDialog.FileNames);
                    var rasterSourceFiles = Regex.Split(input: ModelOpenTextBox.Text.Trim(), pattern: @"[\s]*[|][\s]*")
                        .Where(predicate: s => !string.IsNullOrWhiteSpace(value: s)).ToArray();
                    if (rasterSourceFiles.Length > 0)
                        themeNameBox.Text = string.Join(separator: "|", value: rasterSourceFiles.Select(selector: Path.GetFileNameWithoutExtension).ToArray());
                }
                else
                    ModelOpenTextBox.Text = string.Empty;
            }
            catch (Exception error)
            {
                ModelOpenTextBox.Text = string.Empty;
                DatabaseLogAdd(input: statusText.Text = error.Message);
            }
        }

        private void ModelOpenTextBox_TextChanged(object sender, EventArgs e)
        {
            TileSource_SelectedIndexChanged(sender: sender, e: e);
            FormEventChanged(sender: sender);
            ModelSave.Enabled = !string.IsNullOrWhiteSpace(value: ModelOpenTextBox.Text);
        }

        private void ModelSave_Click(object sender, EventArgs e)
        {
            var rasterSourceFiles = Regex.Split(input: ModelOpenTextBox.Text.Trim(), pattern: @"[\s]*[|][\s]*").Where(predicate: s => !string.IsNullOrWhiteSpace(value: s)).ToArray();
            var rasterSourceFileCount = rasterSourceFiles.Length;
            if (rasterSourceFileCount > 0)
            {
                var key = ModelSave.Name;
                if (!int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex))
                    filterIndex = 0;
                var path = key + "_path";
                var oldPath = RegEdit.Getkey(keyname: path);
                string saveAs = null;
                if (rasterSourceFileCount == 1)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = @"Image(*.tif)|*.tif",
                        FilterIndex = filterIndex
                    };
                    if (Directory.Exists(path: oldPath))
                        saveFileDialog.InitialDirectory = oldPath;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        RegEdit.Setkey(keyname: key, defaultvalue: $"{saveFileDialog.FilterIndex}");
                        RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: saveFileDialog.FileName));
                        saveAs = saveFileDialog.FileName;
                    }
                }
                else
                {
                    var openFolderDialog = new FolderBrowserDialog()
                    {
                        Description = @"Please select a destination folder",
                        ShowNewFolderButton = true
                    };
                    if (Directory.Exists(path: oldPath))
                        openFolderDialog.SelectedPath = oldPath;
                    if (openFolderDialog.ShowDialog() == DialogResult.OK)
                    {
                        RegEdit.Setkey(keyname: path, defaultvalue: openFolderDialog.SelectedPath);
                        saveAs = openFolderDialog.SelectedPath;
                    }
                }
                if (saveAs != null)
                {
                    var isDirectory = Path.GetExtension(path: saveAs) == string.Empty;
                    statusText.Text = @"Saving ...";
                    var pointer = 0;
                    foreach (var rasterSourceFile in rasterSourceFiles)
                    {
                        if (File.Exists(path: rasterSourceFile))
                        {
                            var action = new Func<(bool Success, string Message)>(
                                () =>
                                {
                                    string targetFile;
                                    if (isDirectory)
                                    {
                                        var postfix = 0;
                                        do
                                        {
                                            targetFile = Path.Combine(
                                                path1: saveAs,
                                                path2: Path.GetFileNameWithoutExtension(path: rasterSourceFile) + (postfix == 0 ? "" : $"({postfix})") + ".tif");
                                            if (!File.Exists(path: targetFile))
                                                break;
                                            postfix++;
                                        } while (true);
                                    }
                                    else
                                        targetFile = saveAs;

                                    return GeositeTilePush.SaveAsGeoTiff(
                                        sourceFile: rasterSourceFile,
                                        targetFile: targetFile
                                    );
                                }
                            );
                            var result = Task.Run(function: action).Result;
                            statusText.Text = $@"[{++pointer} / {rasterSourceFileCount}] {(result.Success ? @"saved." : result.Message)}";
                        }
                    }
                }
            }
        }

        private void TileSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tilesource.SelectedIndex)
            {
                case 0:
                    {
                        if (FormatStandard.Checked)
                        {
                            EPSG4326.Enabled = true;
                            EPSG4326.ThreeState = EPSG4326.Checked = false;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = true;
                        }
                        else
                        {
                            if (FormatTMS.Checked || FormatMapcruncher.Checked || FormatArcGIS.Checked)
                            {
                                EPSG4326.Enabled = EPSG4326.ThreeState = EPSG4326.Checked = false;
                                tileLevels.Text = @"-1";
                                tileLevels.Enabled = true;
                            }
                            else
                            {
                                EPSG4326.Enabled = false;
                                EPSG4326.ThreeState = true;
                                EPSG4326.CheckState = CheckState.Indeterminate;
                                tileLevels.Text = @"-1";
                                tileLevels.Enabled = false;
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        EPSG4326.Enabled = true;
                        EPSG4326.ThreeState = EPSG4326.Checked = false;
                        tileLevels.Text = @"0";
                        tileLevels.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        EPSG4326.Enabled = true;
                        EPSG4326.ThreeState = false;
                        EPSG4326.Checked = true;
                        tileLevels.Text = @"-1";
                        tileLevels.Enabled = false;
                        break;
                    }
                default:
                    {
                        EPSG4326.Enabled = false;
                        EPSG4326.ThreeState = true;
                        EPSG4326.CheckState = CheckState.Indeterminate;
                        tileLevels.Text = @"-1";
                        tileLevels.Enabled = false;
                        break;
                    }
            }
        }

        private void TileFormatOpen_Click(object sender, EventArgs e)
        {
            var key = TileFormatOpen.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFolderDialog = new FolderBrowserDialog
            {
                Description = @"Please select a folder that contains tiles",
                ShowNewFolderButton = false
            };
            if (Directory.Exists(path: oldPath))
                openFolderDialog.SelectedPath = oldPath;
            var result = openFolderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                RegEdit.Setkey(keyname: path, defaultvalue: openFolderDialog.SelectedPath);
                TileFormatOpenBox.Text = openFolderDialog.SelectedPath;
            }
            else
                TileFormatOpenBox.Text = string.Empty;
        }

        private void TileFormatSave_Click(object sender, EventArgs e)
        {
            var key = TileFormatOpen.Name;
            var path = key + "_path";
            var oldPath = RegEdit.Getkey(keyname: path);
            var openFolderDialog = new FolderBrowserDialog()
            {
                Description = @"Please select a destination folder",
                ShowNewFolderButton = true
            };
            if (Directory.Exists(path: oldPath))
                openFolderDialog.SelectedPath = oldPath;
            var result = openFolderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                RegEdit.Setkey(keyname: path, defaultvalue: openFolderDialog.SelectedPath);
                TileFormatSaveBox.Text = openFolderDialog.SelectedPath;
            }
            else
                TileFormatSaveBox.Text = string.Empty;
        }

        private void TileConvert_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(path: TileFormatOpenBox.Text) && Directory.Exists(path: TileFormatSaveBox.Text))
            {
                var methodCode =
                    maptilertoogc.Checked ? 0 :
                    mapcrunchertoogc.Checked ? 3 :
                    ogctomapcruncher.Checked ? 2 :
                    ogctomaptiler.Checked ? 1 :
                    -1;
                if (methodCode > -1)
                {
                    var tileFormatTask = new TileConversion();
                    tileFormatTask.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                    {
                        switch (thisEvent.Code)
                        {
                            case 0:
                                _loading.Run();
                                break;
                            case 1:
                                _loading.Run(onOff: false);
                                break;
                            default:
                                _loading.Run(onOff: null);
                                statusText.Text = thisEvent.Message ?? string.Empty;
                                break;
                        }
                    };
                    statusText.Text = $@"{tileFormatTask.Convert(source: TileFormatOpenBox.Text, target: TileFormatSaveBox.Text, method: methodCode)} tiles were processed";
                }
            }
        }

        private void TileFormatChanged(object sender, EventArgs e)
        {
            tileconvert.Enabled = !string.IsNullOrWhiteSpace(value: TileFormatOpenBox.Text) && !string.IsNullOrWhiteSpace(value: TileFormatSaveBox.Text);
        }

        private void ThemeNameBox_TextChanged(object sender, EventArgs e)
        {
            TileSource_SelectedIndexChanged(sender: sender, e: e);
            FormEventChanged(sender: sender);
        }

        private void LocalTileFolder_TextChanged(object sender, EventArgs e)
        {
            TileSource_SelectedIndexChanged(sender: sender, e: e);
            FormEventChanged(sender: sender);
        }

        private void TileWebApi_TextChanged(object sender, EventArgs e)
        {
            TileSource_SelectedIndexChanged(sender: sender, e: e);
            FormEventChanged(sender: sender);
        }

        private void WmtsMinZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormEventChanged(sender: sender);
        }

        private void WmtsMaxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormEventChanged(sender: sender);
        }

        private void TileFormat_CheckedChanged(object sender, EventArgs e)
        {
            var tileFormatRadioButton = (RadioButton)sender;
            if (tileFormatRadioButton.Checked)
            {
                switch (tileFormatRadioButton.Text)
                {
                    case "TMS":
                        {
                            FormatStandard.Checked =
                                //FormatTMS.Checked =
                                FormatMapcruncher.Checked =
                                    FormatArcGIS.Checked =
                                        FormatDeepZoom.Checked =
                                            FormatRaster.Checked =
                                                false;
                            EPSG4326.Enabled =
                                EPSG4326.ThreeState =
                                    EPSG4326.Checked = false;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = true;
                            break;
                        }
                    case "MapCruncher":
                        {
                            FormatStandard.Checked =
                                FormatTMS.Checked =
                                    //FormatMapcruncher.Checked =
                                    FormatArcGIS.Checked =
                                        FormatDeepZoom.Checked =
                                            FormatRaster.Checked =
                                                false;
                            EPSG4326.Enabled =
                                EPSG4326.ThreeState =
                                    EPSG4326.Checked = false;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = true;
                            break;
                        }
                    case "ARCGIS":
                        {
                            FormatStandard.Checked =
                                FormatTMS.Checked =
                                    FormatMapcruncher.Checked =
                                        //FormatArcGIS.Checked =
                                        FormatDeepZoom.Checked =
                                            FormatRaster.Checked =
                                                false;
                            EPSG4326.Enabled =
                                EPSG4326.ThreeState =
                                    EPSG4326.Checked = false;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = true;
                            break;
                        }
                    case "DeepZoom":
                        {
                            FormatStandard.Checked =
                                FormatTMS.Checked =
                                    FormatMapcruncher.Checked =
                                        FormatArcGIS.Checked =
                                            //FormatDeepZoom.Checked =
                                            FormatRaster.Checked =
                                                false;
                            EPSG4326.Enabled = false;
                            EPSG4326.ThreeState = true;
                            EPSG4326.CheckState = CheckState.Indeterminate;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = false;
                            break;
                        }
                    case "Raster":
                        {
                            FormatStandard.Checked =
                                FormatTMS.Checked =
                                    FormatMapcruncher.Checked =
                                        FormatArcGIS.Checked =
                                            FormatDeepZoom.Checked =
                                                //FormatRaster.Checked = 
                                                false;
                            EPSG4326.Enabled = false;
                            EPSG4326.ThreeState = true;
                            EPSG4326.CheckState = CheckState.Indeterminate;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = false;
                            break;
                        }
                    //case "Standard":
                    default:
                        {
                            //FormatStandard.Checked =
                            FormatTMS.Checked =
                                FormatMapcruncher.Checked =
                                    FormatArcGIS.Checked =
                                        FormatDeepZoom.Checked =
                                            FormatRaster.Checked =
                                                false;
                            EPSG4326.Enabled = true;
                            EPSG4326.ThreeState =
                                EPSG4326.Checked = false;
                            tileLevels.Text = @"-1";
                            tileLevels.Enabled = true;
                            break;
                        }
                }
                FormEventChanged(sender: FormatStandard);
                FormEventChanged(sender: FormatTMS);
                FormEventChanged(sender: FormatMapcruncher);
                FormEventChanged(sender: FormatArcGIS);
                FormEventChanged(sender: FormatDeepZoom);
                FormEventChanged(sender: FormatRaster);
            }
        }

        private void WmtsSpider_CheckedChanged(object sender, EventArgs e)
        {
            wmtsMinZoom.Enabled = wmtsMaxZoom.Enabled = !wmtsSpider.Checked;
            FormEventChanged(sender: sender);
        }

        private void DeleteForest_Click(object sender, EventArgs e)
        {
            if (!_clusterUser.status)
                return;
            if (_administrator)
            {
                var result = PostgreSqlHelper.Scalar(
                    cmd: "SELECT id FROM forest WHERE id = @id LIMIT 1;",
                    parameters: new Dictionary<string, object>
                    {
                        { "id", _clusterUser.forest }
                    }
                );
                if (result == null)
                {
                    DatabaseLogAdd(input: statusText.Text = @"Nothing was found.");
                    return;
                }
                var random = new Random(Seed: (int)DateTime.Now.Ticks & 0x0000FFFF);
                var r1 = random.Next(minValue: 0, maxValue: 100);
                var r2 = random.Next(minValue: 0, maxValue: 100);
                {
                    var confirm = new InputForm(prompt: $"  For safety reasons, Please answer a question.\r\n\r\n  {r1} + {r2} = ?");
                    confirm.ShowDialog();
                    if (confirm.Yes is true && string.Equals(a: confirm.Result.Trim(), b: $"{r1 + r2}", comparisonType: StringComparison.Ordinal))
                        new Task(
                            action: () =>
                            {
                                try
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            _loading.Run();
                                            DatabaseLogAdd(input: statusText.Text = @"Deleting ...");
                                            dataGridPanel.Enabled = CatalogTreeView.Enabled = _clusterUser.status = false;
                                        }
                                    );
                                    var success = PostgreSqlHelper.NonQuery(
                                        cmd: "DELETE FROM forest WHERE id = @id;", // AND name = @name::text
                                        parameters: new Dictionary<string, object>
                                        {
                                            { "id", _clusterUser.forest }
                                            //, { "name", _clusterUser.name }
                                        }
                                    ) != null;
                                    if (success)
                                        Invoke(
                                            method: () =>
                                            {
                                                _databaseGridObject?.Reset();
                                                foreach (var statusCell in
                                                         vectorFilePool
                                                             .SelectedRows
                                                             .Cast<DataGridViewRow>()
                                                             .Where(predicate: row => !row.IsNewRow)
                                                             .Select(selector: row => vectorFilePool.CurrentCell = row.Cells[index: 3]))
                                                    statusCell.Value = statusCell.ToolTipText = "";
                                                _catalogTreeObject?.Reset();
                                                DatabaseLogAdd(input: statusText.Text = @"Delete succeeded.");
                                            }
                                        );
                                    else
                                    {
                                        var errorMessage = PostgreSqlHelper.ErrorMessage;
                                        Invoke(
                                            method: () =>
                                            {
                                                DatabaseLogAdd(input: statusText.Text = string.IsNullOrWhiteSpace(value: errorMessage) ? @"Delete failed." : errorMessage);
                                            }
                                        );
                                    }
                                }
                                finally
                                {
                                    Invoke(
                                        method: () =>
                                        {
                                            dataGridPanel.Enabled = CatalogTreeView.Enabled = true;
                                            _loading.Run(onOff: false);
                                        }
                                    );
                                }
                            }
                        ).Start();
                    else
                        Invoke(
                            method: () =>
                            {
                                DatabaseLogAdd(input: statusText.Text = @"The delete operation was not performed.");
                            }
                        );
                }
            }
            else
                Invoke(
                    method: () => { DatabaseLogAdd(input: statusText.Text = @"Administrator identity is required."); }
                );
        }

        private void OgcCard_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormEventChanged(sender: sender);
        }

        private void RasterRunClick()
        {
            string statusError = null;
            short.TryParse(s: tileLevels.Text, result: out var tileMatrix);
            var tileType = TileType.Standard;
            var typeCode = 0;
            XElement themeMetadataX = null;
            switch (tilesource.SelectedIndex)
            {
                case 0:
                    {
                        if (!Directory.Exists(path: localTileFolder.Text))
                            statusError = @"Folder does not exist";
                        else
                        {
                            if (FormatStandard.Checked)
                            {
                                tileType = TileType.Standard;
                                typeCode = EPSG4326.Checked ? 11001 : 11002;
                                if (!Directory.GetDirectories(path: localTileFolder.Text).Any(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir), pattern: @"^\d+$", options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline)))
                                    statusError = @"Folder does not meet the requirements";
                            }
                            else if (FormatTMS.Checked)
                            {
                                tileType = TileType.TMS;
                                typeCode = EPSG4326.Checked ? 11001 : 11002;
                                if (!Directory
                                        .GetDirectories(path: localTileFolder.Text)
                                        .Any(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir), pattern: @"^\d+$", options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline))
                                   )
                                    statusError = @"Folder does not meet the requirements";
                            }
                            else if (FormatMapcruncher.Checked)
                            {
                                tileType = TileType.MapCruncher;
                                typeCode = 11002;
                                if (!Directory
                                        .EnumerateFiles(path: localTileFolder.Text)
                                        .Any(predicate: file => Regex.IsMatch(input: Path.GetFileName(path: file),
                                            pattern: @"^[\d]+.png$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline))
                                   )
                                    statusError = @"Folder does not meet the requirements";
                            }
                            else if (FormatArcGIS.Checked)
                            {
                                tileType = TileType.ARCGIS;
                                typeCode = EPSG4326.Checked ? 11001 : 11002;
                                var tileFolder =
                                    Directory
                                        .GetDirectories(path: localTileFolder.Text)
                                        .FirstOrDefault(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir),
                                            pattern: @"^([\s\S]*?)(_alllayers)$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline));
                                if (tileFolder != null)
                                    localTileFolder.Text = tileFolder;
                                if (!Directory
                                        .GetDirectories(path: localTileFolder.Text)
                                        .Any(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir),
                                            pattern: "^L([0-9]+)$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline))
                                   )
                                    statusError = @"Folder does not meet the requirements.";
                            }
                            else if (FormatDeepZoom.Checked)
                            {
                                tileType = TileType.DeepZoom;
                                typeCode = 11000;
                                var tileFolder =
                                    Directory
                                        .GetDirectories(path: localTileFolder.Text)
                                        .FirstOrDefault(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir),
                                            pattern: @"^([\s\S]+)(_files)$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline));
                                if (tileFolder != null)
                                    localTileFolder.Text = tileFolder;
                                if (!Directory
                                        .GetDirectories(path: localTileFolder.Text)
                                        .Any(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir),
                                            pattern: "([0-9]+)$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline)))
                                    statusError = @"Folder does not meet the requirements.";
                                else
                                {
                                    var xmlName = Regex.Match(input: localTileFolder.Text, pattern: @"^([\s\S]+)(_files)$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline)
                                        .Groups[groupnum: 1].Value;
                                    if (!string.IsNullOrWhiteSpace(value: xmlName))
                                    {
                                        var xmlFile = $"{xmlName}.xml";
                                        if (File.Exists(path: xmlFile))
                                        {
                                            try
                                            {
                                                var metaDataX = XElement.Load(uri: xmlFile, options: LoadOptions.None);
                                                XNamespace ns = metaDataX.Attribute(name: "xmlns")?.Value;
                                                var sizeX = metaDataX.Element(name: ns + "Size");
                                                themeMetadataX = new XElement(
                                                    name: "property",
                                                    content: new object[]
                                                    {
                                                    new XElement(
                                                        name: "name", content: "deepzoom"
                                                    ),
                                                    new XElement(
                                                        name: "minZoom",
                                                        content: metaDataX.Attribute(name: "MinZoom")?.Value),
                                                    new XElement(
                                                        name: "maxZoom",
                                                        content: metaDataX.Attribute(name: "MaxZoom")?.Value),
                                                    new XElement(
                                                        name: "tileSize",
                                                        content: metaDataX.Attribute(name: "TileSize")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "overlap",
                                                        content: metaDataX.Attribute(name: "Overlap")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "type", content: metaDataX.Attribute(name: "Type")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "crs", content: metaDataX.Attribute(name: "CRS")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "format",
                                                        content: metaDataX.Attribute(name: "Format")?.Value),
                                                    new XElement(
                                                        name: "serverFormat",
                                                        content: metaDataX.Attribute(name: "ServerFormat")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "xmlns",
                                                        content: metaDataX.Attribute(name: "xmlns")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "size", content: new object[]
                                                        {
                                                            new XElement(
                                                                name: "width",
                                                                content: sizeX?.Attribute(name: "Width")?.Value),
                                                            new XElement(
                                                                name: "height",
                                                                content: sizeX?.Attribute(name: "Height")?.Value)
                                                        }
                                                    ),
                                                    new XElement(
                                                        name: "boundary", content: new object[]
                                                        {
                                                            new XElement(
                                                                name: "north",
                                                                content: sizeX?.Attribute(name: "Height")?.Value
                                                            ),
                                                            new XElement(
                                                                name: "south", content: 0
                                                            ),
                                                            new XElement(
                                                                name: "west", content: 0
                                                            ),
                                                            new XElement(
                                                                name: "east",
                                                                content: sizeX?.Attribute(name: "Width")?.Value
                                                            )
                                                        }
                                                    )
                                                    }
                                                );
                                            }
                                            catch (Exception xmlError)
                                            {
                                                statusError = xmlError.Message;
                                            }
                                        }
                                        else
                                            statusError = @$"[{xmlName}.xml] metadata file not found.";
                                    }
                                    else
                                        statusError = @"Folder does not meet the requirements.";
                                }
                            }
                            else
                            {
                                tileType = TileType.Raster;
                                typeCode = 11000;
                                if (!Directory
                                        .GetDirectories(path: localTileFolder.Text)
                                        .Any(predicate: dir => Regex.IsMatch(input: Path.GetFileName(path: dir),
                                            pattern: @"^\d+$",
                                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                     RegexOptions.Multiline))
                                   )
                                    statusError = @"Folder does not meet the requirements";
                                else
                                {
                                    var jsonFile = Path.Combine(path1: localTileFolder.Text, path2: "metadata.json");
                                    if (File.Exists(path: jsonFile))
                                    {
                                        using var sr = FreeText.FreeTextEncoding.OpenFreeTextFile(path: jsonFile);
                                        var metaDataX = JsonConvert.DeserializeXNode(value: sr.ReadToEnd(),
                                            deserializeRootElementName: "MapTiler")?.Root;
                                        if (metaDataX != null)
                                        {
                                            var extent = metaDataX.Elements(name: "extent").ToArray();
                                            if (extent.Length == 4)
                                            {
                                                themeMetadataX = new XElement(
                                                    name: "property",
                                                    content: new object[]
                                                    {
                                                    new XElement(
                                                        name: "name", content: metaDataX.Element(name: "name")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "minZoom",
                                                        content: metaDataX.Element(name: "minzoom")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "maxZoom",
                                                        content: metaDataX.Element(name: "maxzoom")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "tileSize",
                                                        content: metaDataX.Elements(name: "tile_matrix")
                                                            .FirstOrDefault()
                                                            ?.Element(name: "tile_size")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "overlap", content: 0
                                                    ),
                                                    new XElement(
                                                        name: "type", content: metaDataX.Element(name: "type")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "crs", content: metaDataX.Element(name: "crs")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "format",
                                                        content: metaDataX.Element(name: "Format")?.Value),
                                                    new XElement(
                                                        name: "scale", content: metaDataX.Element(name: "scale")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "profile",
                                                        content: metaDataX.Element(name: "profile")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "version",
                                                        content: metaDataX.Element(name: "version")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "attribution",
                                                        content: metaDataX.Element(name: "attribution")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "description",
                                                        content: metaDataX.Element(name: "description")?.Value
                                                    ),
                                                    new XElement(
                                                        name: "size", content: new object[]
                                                        {
                                                            new XElement(
                                                                name: "width",
                                                                content: Math.Abs(value: int.Parse(s: extent[2].Value))
                                                            ),
                                                            new XElement(
                                                                name: "height",
                                                                content: Math.Abs(value: int.Parse(s: extent[1].Value))
                                                            )
                                                        }
                                                    ),
                                                    new XElement(
                                                        name: "boundary", content: new object[]
                                                        {
                                                            new XElement(
                                                                name: "north",
                                                                content: Math.Abs(value: int.Parse(s: extent[1].Value))
                                                            ),
                                                            new XElement(
                                                                name: "south", content: 0
                                                            ),
                                                            new XElement(
                                                                name: "west", content: 0
                                                            ),
                                                            new XElement(
                                                                name: "east",
                                                                content: Math.Abs(value: int.Parse(s: extent[2].Value))
                                                            )
                                                        }
                                                    )
                                                    }
                                                );
                                            }
                                            else
                                                statusError = @"[metadata.json] metadata format is incorrect";
                                        }
                                        else
                                            statusError = @"[metadata.json] metadata format is incorrect";
                                    }
                                    else
                                        statusError = @"[metadata.json] metadata file not found";
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        var tilesWest = Map4326.Degree2DMS(DMS: wmtsWest.Text);
                        var tilesEast = Map4326.Degree2DMS(DMS: wmtsEast.Text);
                        var tilesSouth = Map4326.Degree2DMS(DMS: wmtsSouth.Text);
                        var tilesNorth = Map4326.Degree2DMS(DMS: wmtsNorth.Text);
                        if (tileMatrix < 0 && wmtsSpider.Checked)
                            statusError = @"Level should be >= 0";
                        else
                        {
                            if (!Regex.IsMatch(
                                    input: tilewebapi.Text,
                                    pattern: @"\b(https?|ftp|file)://[\s\S]+",
                                    options: RegexOptions.IgnoreCase | RegexOptions.Multiline))
                                statusError = @"URL template does not meet requirements.";
                            else
                            {
                                if (tilesWest == string.Empty || double.Parse(s: tilesWest) < -180 ||
                                    double.Parse(s: tilesWest) > 180)
                                    statusError = @"West Should be between [-180，180]";
                                else
                                {
                                    if (tilesEast == string.Empty || double.Parse(s: tilesEast) < -180 ||
                                        double.Parse(s: tilesEast) > 180)
                                        statusError = @"East Should be between [-180，180]";
                                    else
                                    {
                                        if (tilesSouth == string.Empty || double.Parse(s: tilesSouth) < -90 ||
                                            double.Parse(s: tilesSouth) > 90)
                                            statusError = @"South Should be between [-90，90]";
                                        else
                                        {
                                            if (tilesNorth == string.Empty || double.Parse(s: tilesNorth) < -90 ||
                                                double.Parse(s: tilesNorth) > 90)
                                                statusError = @"North Should be between [-90，90]";
                                            else
                                            {
                                                if (double.Parse(s: tilesWest) > double.Parse(s: tilesEast))
                                                    statusError = @"West should not exceed East";
                                                else
                                                {
                                                    if (double.Parse(s: tilesSouth) > double.Parse(s: tilesNorth))
                                                        statusError = @"South should not exceed North";
                                                    else
                                                    {
                                                        typeCode = EPSG4326.Checked ? 10001 : 10002;
                                                        if (!Regex.IsMatch(input: tilewebapi.Text,
                                                                pattern:
                                                                @".*?(?=.*?{x})(?=.*?{y})(?=.*?{([\d]+\s*[\+\-]\s*)?z(\s*[\+\-]\s*[\d]+)?}).*",
                                                                options: RegexOptions.IgnoreCase | RegexOptions.Multiline))
                                                        {
                                                            var foundBingmap = Regex.IsMatch(input: tilewebapi.Text,
                                                                pattern: ".*?{bingmap}.*",
                                                                options: RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                                            if (!foundBingmap)
                                                            {
                                                                var foundEsri = Regex.IsMatch(input: tilewebapi.Text,
                                                                    pattern: ".*?{esri}.*",
                                                                    options: RegexOptions.IgnoreCase |
                                                                             RegexOptions.Multiline);
                                                                if (!foundEsri)
                                                                    statusError =
                                                                        @"URL template does not meet requirements.";
                                                                else
                                                                    tileType = TileType.ARCGIS;
                                                            }
                                                            else
                                                                tileType = TileType.MapCruncher;
                                                        }
                                                        else
                                                            tileType = TileType.Standard;
                                                        if (string.IsNullOrWhiteSpace(value: statusError))
                                                        {
                                                            if (Regex.IsMatch(input: tilewebapi.Text, pattern: @"\{s\}", options: RegexOptions.IgnoreCase))
                                                            {
                                                                if (string.IsNullOrWhiteSpace(value: subdomainsBox.Text))
                                                                    statusError = @"Subdomains should be specified.";
                                                                else
                                                                {
                                                                    if (!Regex.IsMatch(input: subdomainsBox.Text,
                                                                            pattern: @"^[a-z\d]+$",
                                                                            options: RegexOptions.IgnoreCase))
                                                                        statusError = @"Subdomains does not meet requirements.";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!string.IsNullOrWhiteSpace(value: subdomainsBox.Text))
                                                                    statusError = @"Subdomains should be blank.";
                                                            }
                                                            if (wmtsSpider.Checked && int.Parse(s: wmtsMinZoom.Text) > int.Parse(s: wmtsMaxZoom.Text))
                                                                statusError = @"MinZoom Should be <= MaxZoom";
                                                            if (string.IsNullOrWhiteSpace(value: statusError) && !wmtsSpider.Checked)
                                                            {
                                                                themeMetadataX = new XElement(
                                                                    name: "property", content: new object[]
                                                                    {
                                                                        new XElement(name: "minZoom",
                                                                            content: wmtsMinZoom.Text),
                                                                        new XElement(name: "maxZoom",
                                                                            content: wmtsMaxZoom.Text),
                                                                        new XElement(name: "tileSize",
                                                                            content: wmtsSize.Text),
                                                                        new XElement(name: "format",
                                                                            content: MIMEBox.Text),
                                                                        new XElement(name: "boundary",
                                                                            content: new object[]
                                                                            {
                                                                                new XElement(name: "north",
                                                                                    content: wmtsNorth.Text),
                                                                                new XElement(name: "south",
                                                                                    content: wmtsSouth.Text),
                                                                                new XElement(name: "west",
                                                                                    content: wmtsWest.Text),
                                                                                new XElement(name: "east",
                                                                                    content: wmtsEast.Text)
                                                                            }
                                                                        )
                                                                    }
                                                                );
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (string.IsNullOrWhiteSpace(value: ModelOpenTextBox.Text))
                            statusError = @"File cannot be empty.";
                        else
                        {
                            rasterTileSize.Text = int.TryParse(s: rasterTileSize.Text, result: out var size)
                                ? size < 10
                                    ? @"10"
                                    : size > 1024
                                        ? "1024"
                                        : $"{size}"
                                : @"100";
                            tileType = TileType.Standard;
                            EPSG4326.Checked = true;
                            typeCode = EPSG4326.Checked ? 12001 : 12002;
                            themeMetadataX = new XElement(name: "property");
                        }
                        break;
                    }
                default:
                    return;
            }
            if (!string.IsNullOrWhiteSpace(value: statusError))
            {
                DatabaseLogAdd(input: statusText.Text = statusError);
                return;
            }
            if (themeMetadataX == null)
            {
                if (!_noPromptMetaData)
                {
                    var metaData = new MetaDataForm();
                    metaData.ShowDialog();
                    if (metaData.Ok)
                    {
                        themeMetadataX = metaData.MetaDataX;
                        _noPromptMetaData = metaData.DonotPrompt;
                    }
                }
            }
            if (themeMetadataX != null && themeMetadataX.Name != "property")
                themeMetadataX.Name = "property";
            _loading.Run();
            statusProgress.Visible = true;
            PostgresRun.BackgroundImage = Properties.Resources.linkcancel;
            OGCtoolTip.SetToolTip(PostgresRun, "Cancel");
            rasterWorker.RunWorkerAsync(
                argument: (
                    index: tilesource.SelectedIndex,
                    theme: themeNameBox.Text.Trim(),
                    type: tileType,
                    typeCode,
                    update: UpdateBox.Checked,
                    light: PostgresLight.Checked,
                    rank: rankList.Text,
                    metadata: themeMetadataX,
                    srid: tileType is TileType.DeepZoom or TileType.Raster ? 0 : tileType == TileType.Standard && EPSG4326.Checked ? 4326 : 3857,
                    tileMatrix,
                    tileSize: rasterTileSize.Text
                )
            );
        }

        private string RasterWorkStart(BackgroundWorker rasterBackgroundWorker, DoWorkEventArgs e)
        {
            var parameter = ((int index, string theme, TileType type, int typeCode, bool update, bool light, string rank, XElement metadata, int srid, short tileMatrix, string tileSize))e.Argument!;
            var oneForest = new GeositeXmlPush();
            var oneForestResult = oneForest.Forest(id: _clusterUser.forest, name: _clusterUser.name);
            if (!oneForestResult.Success)
            {
                Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = oneForestResult.Message); });
                return oneForestResult.Message;
            }
            var rank = parameter.rank;
            var tabIndex = parameter.index;
            var themeMetadataX = parameter.metadata;
            var typeCode = parameter.typeCode;
            var status = (short)(parameter.light ? 4 : 6);
            var forest = _clusterUser.forest;

            string[] themeNames;
            string[] rasterSourceFiles = null;
            if (tabIndex == 2)
            {
                rasterSourceFiles = Regex.Split(input: ModelOpenTextBox.Text.Trim(), pattern: @"[\s]*[|][\s]*").Where(predicate: s => !string.IsNullOrWhiteSpace(value: s)).ToArray();
                var rasterSourceFileCount = rasterSourceFiles.Length;
                themeNames = Regex.Split(input: parameter.theme, pattern: @"[\s]*[|][\s]*").Where(predicate: s => !string.IsNullOrWhiteSpace(value: s)).ToArray();
                var themeNameCount = themeNames.Length;
                if (rasterSourceFileCount != themeNameCount)
                {
                    const string message = "The number of files is inconsistent with the number of layers.";
                    Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = message); });
                    return message;
                }
            }
            else
                themeNames = new[] { parameter.theme };
            long total = 0;
            var timeWatch = new Stopwatch();
            timeWatch.Start();
            for (var pointer = 0; pointer < themeNames.Length; pointer++)
            {
                if (rasterBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                var themeName = themeNames[pointer];
                try
                {
                    var xmlNodeName = new XElement(name: themeName);
                    if (xmlNodeName.Name.LocalName != themeName || Regex.IsMatch(input: themeName, pattern: @"[\.]+"))
                        throw new Exception(message: $"[{themeName}] does not conform to XML naming rules.");
                }
                catch (Exception errorXml)
                {
                    Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = errorXml.Message); });
                    return errorXml.Message;
                }
                var oldTreeType = PostgreSqlHelper.Scalar(
                    cmd: "SELECT type FROM tree WHERE forest = @forest AND name ILIKE @name::text LIMIT 1;",
                    parameters: new Dictionary<string, object>
                    {
                        { "forest", forest },
                        { "name", themeName }
                    }
                );
                if (oldTreeType != null)
                {
                    if (oldTreeType.GetType().Name != "DBNull")
                    {
                        var typeArray = (int[])oldTreeType;
                        var tileArray = typeArray.Where(predicate: t => t is >= 10000 and <= 12002).Select(selector: t => t);
                        if (typeArray.Length != tileArray.Count())
                            return $"[{themeName}] is already used for vector theme.";
                    }
                }
                int tree;
                string[] routeName;
                int[] routeId;
                long leaf;
                XElement propertyX;
                var oldTree = PostgreSqlHelper.Scalar(
                    cmd: "WITH t1 AS" +
                         "(" +
                         "    SELECT branch.tree, branch.routename, branch.routeid, leaf.id, leaf.type FROM leaf," +
                         "    (" +
                         "        SELECT tree, array_agg(name) AS routename, array_agg(id) AS routeid FROM" +
                         "        (" +
                         "            SELECT * FROM branch WHERE tree IN" +
                         "            (" +
                         "                SELECT id FROM tree WHERE forest = @forest AND name ILIKE @name::text LIMIT 1" +
                         "            ) ORDER BY tree, level" +
                         "        ) AS branchtable" +
                         "        GROUP BY tree" +
                         "    ) AS branch" +
                         "    WHERE leaf.name ILIKE @name::text AND leaf.branch = branch.routeid[array_length(branch.routeid, 1)] LIMIT 1" +
                         ")" + "SELECT (t1.*, tt.description) FROM t1," +
                         "(" +
                         "    SELECT t2.leaf, array_agg((t2.name,t2.attribute,t2.level,t2.sequence,t2.parent,t2.flag,t2.type,t2.content)) AS description" +
                         "    FROM leaf_description AS t2, t1" +
                         "    WHERE t1.id = t2.leaf" +
                         "    GROUP BY t2.leaf" +
                         ") AS tt " +
                         "WHERE t1.id = tt.leaf LIMIT 1;",
                    parameters: new Dictionary<string, object>
                    {
                        { "forest", forest },
                        { "name", themeName }
                    }
                );
                if (oldTree != null)
                {
                    var oldTreeResult = (object[])oldTree;
                    tree = (int)oldTreeResult[0];
                    routeName = (string[])oldTreeResult[1];
                    routeId = (int[])oldTreeResult[2];
                    leaf = (long)oldTreeResult[3];
                    typeCode = (int)oldTreeResult[4];
                    propertyX = GeositeXmlFormatting.TableToXml(table: oldTreeResult[5]);
                }
                else
                {
                    var sequenceMax =
                        PostgreSqlHelper.Scalar(
                            cmd: "SELECT sequence FROM tree WHERE forest = @forest ORDER BY sequence DESC LIMIT 1;",
                            parameters: new Dictionary<string, object>
                            {
                                { "forest", forest }
                            }
                        );
                    var sequence = sequenceMax == null ? 0 : 1 + int.Parse(s: $"{sequenceMax}");
                    string getTreePathString = null;
                    XElement description = null;
                    var canDo = true;
                    timeWatch.Stop();
                    if (!_noPromptLayersBuilder)
                    {
                        LayersBuilderForm getTreeLayers;
                        switch (tabIndex)
                        {
                            case 0:
                                getTreeLayers = new LayersBuilderForm(treePathDefault: new DirectoryInfo(path: localTileFolder.Text).FullName);
                                break;
                            case 1:
                                getTreeLayers = new LayersBuilderForm(treePathDefault: "Untitled");
                                break;
                            case 2:
                                getTreeLayers = new LayersBuilderForm(treePathDefault: new FileInfo(fileName: rasterSourceFiles[pointer]).FullName);
                                break;
                            default:
                                return "This option is not supported.";
                        }
                        getTreeLayers.ShowDialog();
                        if (getTreeLayers.Ok)
                        {
                            getTreePathString = getTreeLayers.TreePathString;
                            description = getTreeLayers.Description;
                            _noPromptLayersBuilder = getTreeLayers.DonotPrompt;
                        }
                        else
                            canDo = false;
                    }
                    else
                    {
                        switch (tabIndex)
                        {
                            case 0:
                                getTreePathString = ConsoleIO.FilePathToXPath(path: new DirectoryInfo(path: localTileFolder.Text).FullName);
                                break;
                            case 1:
                                getTreePathString = "Untitled";
                                break;
                            case 2:
                                getTreePathString = ConsoleIO.FilePathToXPath(path: new FileInfo(fileName: rasterSourceFiles[pointer]).FullName);
                                break;
                            default:
                                return "This option is not supported.";
                        }
                    }
                    if (canDo)
                    {
                        timeWatch.Restart();
                        string treeUri;
                        DateTime treeLastWriteTime;
                        switch (tabIndex)
                        {
                            case 0:
                                {
                                    var getFolder = new DirectoryInfo(path: localTileFolder.Text);
                                    treeLastWriteTime = getFolder.LastWriteTime;
                                    treeUri = getFolder.FullName;
                                    break;
                                }
                            case 1:
                                {
                                    treeUri = tilewebapi.Text;
                                    treeLastWriteTime = DateTime.Now;
                                    break;
                                }
                            default:
                                {
                                    var fileInfo = new FileInfo(fileName: rasterSourceFiles[pointer]);
                                    treeUri = fileInfo.FullName;
                                    treeLastWriteTime = fileInfo.LastWriteTime;
                                    themeMetadataX = GeositeTilePush.GetRasterMetaData(sourceFile: treeUri, tileSize: parameter.tileSize);
                                    /*
                                        <property>
                                          <name>raster</name>
                                          <tileSize>256</tileSize>
                                          <overlap>0</overlap>
                                          <minZoom>-1</minZoom>
                                          <maxZoom>-1</maxZoom>
                                          <type>raster</type>
                                          <crs>3857</crs>
                                          <serverFormat>Default</serverFormat>
                                          <size>
                                            <width>22418</width>
                                            <height>23939</height>
                                          </size>
                                          <boundary>
                                            <north>21396567.2524</north>
                                            <south>-21397064.618899997</south>
                                            <west>-20037508.3428</west>
                                            <east>20037166.4261</east>
                                          </boundary>
                                        </property>                                     
                                     */
                                    if (!int.TryParse(themeMetadataX.Element("crs").Value, out var crs) || crs is not (4326 or 3857))
                                    {
                                        return $"The coordinate reference system [EPSG:{parameter.srid}] should be set to EPSG:4326 or 3857.";
                                    }
                                    if (parameter.srid != crs)
                                    {
                                        return $"The coordinate reference system [EPSG:{parameter.srid}] is inconsistent with the image [EPSG:{crs}].";
                                    }
                                    break;
                                }
                        }
                        var treePathString = getTreePathString;
                        var treeDescription = description;
                        var lastWriteTime = Regex.Split(
                            input: $"{treeLastWriteTime: yyyyMMdd,HHmmss}",
                            pattern: "[,]",
                            options: RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Multiline
                        );
                        int.TryParse(s: lastWriteTime[0], result: out var ymd);
                        int.TryParse(s: lastWriteTime[1], result: out var hms);
                        var timestamp = $"{forest},{sequence},{ymd},{hms}";
                        var treeXml = new XElement(
                            name: "FeatureCollection",
                            content: new object[]
                            {
                                new XAttribute(name: "timeStamp", value: DateTime.Now.ToString(format: "s")),
                                new XElement(name: "name", content: themeName)
                            }
                        );
                        if (treeDescription != null)
                            treeXml.Add(content: new XElement(name: "property", content: treeDescription.Elements().Select(selector: x => x)));
                        XElement layersX = null;
                        routeName = Regex.Split(input: treePathString, pattern: "[/]", options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                        for (var i = routeName.Length - 1; i >= 0; i--)
                            layersX = layersX == null
                                ? new XElement(
                                    name: "layer",
                                    content: new object[]
                                    {
                                        new XElement(name: "name", content: routeName[i]),
                                        themeMetadataX, new XElement(
                                            name: "member", content: new object[]
                                            {
                                                new XAttribute(name: "type", value: "Tile"),
                                                new XAttribute(name: "typeCode", value: typeCode),
                                                new XAttribute(name: "timeStamp", value: treeLastWriteTime.ToString(format: "s")),
                                                new XAttribute(name: "rank", value: rank),
                                                new XElement(name: "name", content: themeName),
                                                new XElement(name: "property",
                                                    content: new object[]
                                                    {
                                                        new XElement(name: "srid", content: parameter.srid),
                                                        tabIndex == 1 &&
                                                        !string.IsNullOrWhiteSpace(value: subdomainsBox.Text)
                                                            ? new XElement(name: "subdomains",
                                                                content: subdomainsBox.Text)
                                                            : null
                                                    }
                                                )
                                            }
                                        )
                                    }
                                )
                                : new XElement(
                                    name: "layer",
                                    content: new object[]
                                    {
                                        new XElement(name: "name", content: routeName[i]),
                                        layersX
                                    }
                                );
                        treeXml.Add(content: layersX);
                        var oneTree = oneForest.Tree(
                            timestamp: timestamp,
                            treeX: treeXml,
                            uri: treeUri,
                            forestStatus: status,
                            typeCode: typeCode
                        );
                        if (oneTree.Success)
                        {
                            Invoke(
                                method: () =>
                                {
                                    _databaseGridObject?.Reset();
                                    _catalogTreeObject?.Reset();
                                }
                            );
                            tree = oneTree.Id;
                            var leafX = treeXml.DescendantsAndSelf(name: "member").FirstOrDefault();
                            var oneBranch = oneForest.Branch(
                                forest: forest,
                                sequence: sequence,
                                tree: tree,
                                leafX: leafX,
                                leafRootX: treeXml
                            );
                            if (oneBranch.Success)
                            {
                                var routeArray = oneBranch.Route;
                                routeId = new ArraySegment<int>(array: routeArray, offset: 3, count: routeArray.Length - 3).ToArray();
                                var oneLeaf = oneForest.Leaf(route: routeArray, leafX: leafX);
                                if (oneLeaf.Success)
                                {
                                    leaf = oneLeaf.Id;
                                    propertyX = oneLeaf.Property;
                                }
                                else
                                    return oneLeaf.Message;
                            }
                            else
                                return oneBranch.Message;
                        }
                        else
                            return oneTree.Message;
                    }
                    else
                        return "Abort task";
                }

                var geositeTilePush = new GeositeTilePush(
                    forest: oneForest,
                    tree: tree,
                    routeName: routeName,
                    routeId: routeId,
                    leaf: leaf,
                    typeCode: typeCode,
                    propertyX: propertyX,
                    update: parameter.update,
                    backgroundWorker: rasterWorker,
                    eventParameter: e
                );
                var localI = pointer + 1;
                geositeTilePush.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                {
                    object userStatus = !string.IsNullOrWhiteSpace(value: thisEvent.Message)
                        ? themeNames.Length > 1
                            ? $"[{localI}/{themeNames.Length}] {thisEvent.Message}"
                            : thisEvent.Message
                        : null;
                    rasterWorker.ReportProgress(percentProgress: thisEvent.Progress ?? -1, userState: userStatus ?? string.Empty);
                };
                switch (tabIndex)
                {
                    case 0:
                        {
                            try
                            {
                                var result0 = geositeTilePush.TilePush(
                                    code: 0,
                                    tileUri: localTileFolder.Text,
                                    tileType: parameter.type,
                                    level: parameter.tileMatrix,
                                    epsg4326: EPSG4326.Checked
                                );
                                total += result0.total;
                            }
                            catch (Exception error)
                            {
                                Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = error.Message); });
                                return error.Message;
                            }
                            break;
                        }
                    case 1:
                        {
                            try
                            {
                                var result1 = geositeTilePush.TilePush(
                                    code: 1,
                                    tileUri: tilewebapi.Text,
                                    tileType: parameter.type,
                                    level: parameter.tileMatrix,
                                    epsg4326: EPSG4326.Checked,
                                    boundary: (wmtsNorth.Text, wmtsSouth.Text, wmtsWest.Text, wmtsEast.Text),
                                    isWMTS: wmtsSpider.Checked,
                                    subdomains: !string.IsNullOrWhiteSpace(value: subdomainsBox.Text)
                                        ? subdomainsBox.Text
                                        : null
                                );
                                total += result1.total;
                            }
                            catch (Exception error)
                            {
                                Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = error.Message); });
                                return error.Message;
                            }
                            break;
                        }
                    case 2:
                        {
                            try
                            {
                                var result2 = geositeTilePush.TilePush(
                                    code: 2,
                                    tileUri: rasterSourceFiles[pointer],
                                    tileType: TileType.Standard,
                                    level: -1,
                                    epsg4326: EPSG4326.Checked,
                                    boundary: (parameter.tileSize, parameter.tileSize, nodatabox.Text, null)
                                );
                                total += result2.total;
                            }
                            catch (Exception error)
                            {
                                Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = error.Message); });
                                return error.Message;
                            }
                            break;
                        }
                }
                oneForest.Tree(
                    enclosure:
                    (
                        tree,
                        new List<int> { typeCode },
                        true
                    )
                );
                Invoke(
                    method: () =>
                    {
                        _databaseGridObject?.Reset();
                        _catalogTreeObject?.Reset();
                    }
                );
            }
            timeWatch.Stop();
            var duration = timeWatch.Elapsed.ToString(format: @"d\.hh\:mm\:ss\.f");
            var errprMessage = total > 0 ? $"{$"Pushed {total} tile" + (total > 1 ? "s" : "")} - {duration}" : "No tile pushed.";
            Invoke(method: () => { DatabaseLogAdd(input: statusText.Text = errprMessage); });
            return errprMessage;
        }

        private void RasterWorkProgress(object sender, ProgressChangedEventArgs e)
        {
            var userState = (string)e.UserState;
            var progressPercentage = e.ProgressPercentage;
            var percent = statusProgress.Value = progressPercentage is >= 0 and <= 100 ? progressPercentage : 0;
            statusText.Text = userState;
            if (percent % 10 == 0)
                statusBar.Refresh();
        }

        private void RasterWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusProgress.Visible = false;
            if (e.Error != null)
                MessageBox.Show(text: e.Error.Message, caption: @"Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            else if (e.Cancelled)
                statusText.Text = @"Suspended!";
            else if (e.Result != null)
                statusText.Text = (string)e.Result;
            var serverUrl = GeositeServerUrl.Text.Trim();
            var serverUser = GeositeServerUser.Text.Trim();
            var serverPassword = GeositeServerPassword.Text.Trim();
            UpdateDatabaseSize(serverUrl: serverUrl, serverUser: serverUser, serverPassword: serverPassword);
            PostgresRun.BackgroundImage = Properties.Resources.linkpush;
            OGCtoolTip.SetToolTip(PostgresRun, "Start");
            _loading.Run(onOff: false);
        }

        private void RankList_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormEventChanged(sender: sender);
        }

        private void FileGridViewRowsClean()
        {
            vectorTargetFile.Text = string.Empty;
            FileSaveGroupBox.Enabled = FileGridView.Rows.Count > 0;
            if (FileGridView.Rows.Count == 0)
            {
                SaveAsFormat.Text = string.Empty;
                SaveAsFormat.Items.Clear();
            }
            SaveAsFormat.Enabled = FileGridView.Rows.Count > 1;
        }

        private void FileGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            FileGridViewRowsClean();
        }

        private void FileGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            FileGridViewRowsClean();
        }

        private void FileGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            var rowIndex = e.RowIndex;
            if (senderGrid != null && rowIndex >= 0)
            {
                var row = senderGrid.Rows[index: e.RowIndex];
                var columnIndex = e.ColumnIndex;
                var col = row.Cells[index: columnIndex];
                var path = row.Cells[index: 0].Value.ToString();
                switch (columnIndex)
                {
                    case 1: //坐标参照系按钮
                        {
                            var tagX = (XElement)col.Tag;
                            var fromX = tagX?.Element("From")?.Elements().FirstOrDefault();
                            var fromXName = fromX?.Name.LocalName;
                            var toX = tagX?.Element("To")?.Elements().FirstOrDefault();
                            var toXName = toX?.Name.LocalName;

                            var projectionFrom = new XElement("From");
                            switch (Path.GetExtension(path: path)?.ToLower())
                            {
                                case ".mpj":
                                    return;
                                case ".kml":
                                case ".xml":
                                case ".geojson":
                                    {
                                        projectionFrom.Add(new XElement("Geography"));
                                        break;
                                    }
                                default:
                                    {
                                        projectionFrom.Add(fromXName != null
                                            ? new XAttribute(
                                                "Active",
                                                fromXName == "Geography" ? 0 :
                                                fromXName == "Gauss-Kruger" ? 1 :
                                                fromXName == "Lambert" ? 2 :
                                                fromXName == "Albers" ? 3 : 4
                                            )
                                            : null);
                                        projectionFrom.Add(new XElement("Geography"));
                                        projectionFrom.Add(fromXName == "Gauss-Kruger"
                                            ? fromX
                                            : new XElement("Gauss-Kruger", new XElement("Zone", 6)));
                                        projectionFrom.Add(fromXName == "Lambert" ? fromX : new XElement("Lambert"));
                                        projectionFrom.Add(fromXName == "Albers" ? fromX : new XElement("Albers"));
                                        projectionFrom.Add(new XElement("Web-Mercator"));
                                        break;
                                    }
                            }
                            var projectionForm =
                                new ProjectionForm(
                                    new XElement(
                                        "Projection",
                                        projectionFrom,
                                        new XElement(
                                            "To",
                                            toXName != null
                                                ? new XAttribute(
                                                    "Active",
                                                    toXName == "Geography" ? 0 :
                                                    toXName == "Gauss-Kruger" ? 1 :
                                                    toXName == "Lambert" ? 2 :
                                                    toXName == "Albers" ? 3 : 4
                                                )
                                                : null,
                                            new XElement("Geography"),
                                            toXName == "Gauss-Kruger" ? toX : new XElement("Gauss-Kruger", new XElement("Zone", 6)),
                                            toXName == "Lambert" ? toX : new XElement("Lambert"),
                                            toXName == "Albers" ? toX : new XElement("Albers"),
                                            new XElement("Web-Mercator")
                                        )
                                    )
                                );
                            projectionForm.ShowDialog();
                            var getProjectionSame = projectionForm.GetProjectionSame();
                            var projectionX = projectionForm.Projection;
                            if (projectionX != null)
                            {
                                var from = projectionX.Element("From")?.Elements().FirstOrDefault()?.Name.LocalName;
                                var to = projectionX.Element("To")?.Elements().FirstOrDefault()?.Name.LocalName;
                                if (getProjectionSame)
                                {
                                    foreach (var oneRow in senderGrid.Rows)
                                    {
                                        var oneCol = ((DataGridViewRow)oneRow).Cells[1];
                                        oneCol.Tag = projectionX;
                                        oneCol.Value = "*";
                                        oneCol.ToolTipText = @$"[{from}] to [{to}]";
                                    }
                                }
                                else
                                {
                                    col.Tag = projectionX;
                                    col.Value = "*";
                                    col.ToolTipText = @$"[{from}] to [{to}]";
                                }
                            }
                            else
                            {
                                if (getProjectionSame)
                                {
                                    foreach (var oneRow in senderGrid.Rows)
                                    {
                                        var oneCol = ((DataGridViewRow)oneRow).Cells[1];
                                        oneCol.Tag = null;
                                        oneCol.Value = "?";
                                        oneCol.ToolTipText = @"Unknown";
                                    }
                                }
                                else
                                {
                                    col.Tag = null;
                                    col.Value = "?";
                                    col.ToolTipText = @"Unknown";
                                }
                            }
                            break;
                        }
                    case 2: //预览按钮
                        {
                            var property = (XElement)col.Tag;
                            var type = col.ToolTipText;
                            if (string.IsNullOrWhiteSpace(value: type))
                                MessageBox.Show(text: @"This type currently does not support preview.", caption: @"Tip");
                            else
                                Invoke(
                                    method: () =>
                                    {
                                        new MapView(
                                            mainForm: this,
                                            path: path,
                                            type: type,
                                            property: property,
                                            style: PreviewStyleForm.Style,
                                            projection: (XElement)row.Cells[index: 1].Tag
                                        ).View();
                                    }
                                );
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 图窗加载完成后的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapBox_Load(object sender, EventArgs e)
        {
            MapBox.OnMarkerClick += MapBox_OnMarkerClick;
            MapBox.OnPolygonClick += MapBox_OnPolygonClick;
            MapBox.OnRouteClick += MapBox_OnRouteClick;
            MapBox.OnRouteEnter += MapBox_OnRouteEnter;
            MapBox.OnRouteLeave += MapBox_OnRouteLeave;
            MapBox.OnMarkerEnter += MapBox_OnMarkerEnter;
            MapBox.OnMarkerLeave += MapBox_OnMarkerLeave;
            MapBox.OnPolygonEnter += MapBox_OnPolygonEnter;
            MapBox.OnPolygonLeave += MapBox_OnPolygonLeave;
            MapBox.OnTileLoadComplete += MapBox_OnTileLoadComplete;
            MapBox.OnTileLoadStart += MapBox_OnTileLoadStart;
            MapBox.OnMapZoomChanged += MapBox_OnMapZoomChanged;
            MapBox.OnMapTypeChanged += MapBox_OnMapTypeChanged;
            MapBox.MouseMove += MapBox_MouseMove;
            MapBox.MouseUp += MapBox_MouseUp;
            MapBox.OnMapDrag += MapBox_OnMapDrag;
            MapBox.DragButton = MouseButtons.Left;
            MapBox.DisableFocusOnMouseEnter = true;
            MapBox.ScaleMode = ScaleModes.Fractional;
            //MapBox.CacheLocation = Application.StartupPath;
            MapBox.MapScaleInfoPosition = MapScaleInfoPosition.Bottom;
            MapBox.ScalePen.Width = 1;
            MapBox.ScalePenBorder.Width = 3;
            MapBox.ShowCenter = //默认情况下，GMap.NET 控件在地图上显示一个【红十字】，以便准确显示中心的位置
                MapBox.MapScaleInfoEnabled = //线状比例尺
                                             //MapBox.ShowTileGridLines = //瓦片网格
            RegEdit.Getkey(keyname: ZoomLevelLabel.Name, defaultvalue: "0") == "1";
            TileLoading = new LoadingBar(bar: TileLoadProgressBar);
            FilePreviewLoading = new LoadingBar(bar: FilePreviewProgressBar);
            GMapProviderDictionary.Add(key: EmptyMapProviderKey, value: EmptyProvider.Instance);
            //if (new Ping().Send(hostNameOrAddress: (new UriBuilder(uri: "https://cn.bing.com/maps")).Host, timeout: 3000) is { Status: IPStatus.Success })
            {
                GMapProviderDictionary.Add(key: "Separator0", value: null);
                BingOSMapProvider.Instance.MinZoom = 1;
                GMapProviderDictionary.Add(key: "Bing Roads", value: BingOSMapProvider.Instance); //地理线划+注释
                BingSatelliteMapProvider.Instance.MinZoom = 1;
                GMapProviderDictionary.Add(key: "Bing Imagery", value: BingSatelliteMapProvider.Instance); //遥感
                BingHybridMapProvider.Instance.MinZoom = 1;
                GMapProviderDictionary.Add(key: "Bing Hybrid", value: BingHybridMapProvider.Instance); //遥感+注释
                BingMapProvider.Instance.MinZoom = 1;
                GMapProviderDictionary.Add(key: "Bing Terrain", value: BingMapProvider.Instance); //模型+注释
                GMapProviderDictionary.Add(key: "Separator1", value: null);
                GMapProviderDictionary.Add(key: "ArcGIS Imagery", value: ArcGISImageryProvider.Instance); //遥感
                //GMapProviderDictionary.Add(key: "ArcGIS Terrain", value: ArcGIS_World_Terrain_Base_MapProvider.Instance); //单色模型
                GMapProviderDictionary.Add(key: "Separator2", value: null);
                GMapProviderDictionary.Add(key: "Tianditu Roads", value: TiandituRoadsMapProvider.Instance); //地理线划+注释
                GMapProviderDictionary.Add(key: "Tianditu Imagery", value: TiandituImageryMapProvider.Instance); //遥感+注释
                GMapProviderDictionary.Add(key: "Tianditu Terrain", value: TiandituTerrainMapProvider.Instance); //模型+注释
                GMaps.Instance.Mode = AccessMode.ServerAndCache;
            }
            //else
            //    GMaps.Instance.Mode = AccessMode.CacheOnly;
            var mapProviderDropDownItem = RegEdit.Getkey(keyname: MapProviderDropDown.Name, defaultvalue: EmptyMapProviderKey);
            var mapProviderExist = false;
            foreach (var baseMapProvider in GMapProviderDictionary)
            {
                var providerKey = baseMapProvider.Key;
                var providerValue = baseMapProvider.Value;
                if (providerValue == null)
                    MapProviderDropDown.DropDownItems.Add(value: new ToolStripSeparator());
                else
                {
                    var isChecked = providerKey == mapProviderDropDownItem;
                    if (isChecked)
                        mapProviderExist = true;
                    var item = new ToolStripMenuItem
                    {
                        Text = providerKey,
                        Checked = isChecked,
                        CheckOnClick = true
                    };
                    item.Click += MapProviderMenuItem_Click;
                    MapProviderDropDown.DropDownItems.Add(value: item);
                }
            }
            if (!mapProviderExist)
                mapProviderDropDownItem = EmptyMapProviderKey;
            MapProviderDropDown.Text = mapProviderDropDownItem;
            MapBox.MapProvider = GMapProviderDictionary[key: mapProviderDropDownItem];
            MapBox.MinZoom = MapBox.MapProvider.MinZoom;
            MapBox.MaxZoom = MapBox.MapProvider.MaxZoom ?? 18;
            //MapBox.GrayScaleMode = true; //瓦片底图灰度模式
            //MapBox.NegativeMode = true;  //瓦片底图反相模式
            MapBox.Overlays.Add(item: MapView.Features = new GMapOverlay(id: "features"));
            MapBox.Overlays.Add(item: MapGrid.Features = new GMapOverlay(id: "features"));
            MapBox.Overlays.Add(item: TopologyCheckerForm.Features = new GMapOverlay(id: "features"));
            var zoomAndPosition = RegEdit.Getkey(keyname: MapBox.Name, defaultvalue: string.Empty);
            if (!string.IsNullOrWhiteSpace(value: zoomAndPosition))
            {
                var splitArray = Regex.Split(input: zoomAndPosition, pattern: @"[\s]*,[\s]*", options: RegexOptions.Singleline | RegexOptions.Multiline);
                if (splitArray.Length == 3)
                {
                    MapBox.Zoom = double.Parse(s: splitArray[0]);
                    MapBox.Position = new PointLatLng(lat: double.Parse(s: splitArray[2]), lng: double.Parse(s: splitArray[1]));
                }
                else
                    zoomAndPosition = null;
            }
            if (string.IsNullOrWhiteSpace(value: zoomAndPosition))
            {
                /*
	                西安 钟楼 (34.26098708, 108.94236401)              
                 */
                MapBox.Position = new PointLatLng(lat: 34.26098708, lng: 108.94236401);
                MapBox.Zoom = 5D;
            }
            var positionBoxItem = RegEdit.Getkey(keyname: PositionBox.Name, defaultvalue: "DEG");
            PositionBox.Tag = (positionBoxItem, ((double?)null, (double?)null));
            foreach (var item in PositionBox.DropDownItems)
                if (item.GetType().Name == "ToolStripMenuItem")
                {
                    var theItem = (ToolStripMenuItem)item;
                    theItem.Checked = theItem.Text == positionBoxItem;
                }
            var mapGridItem = RegEdit.Getkey(keyname: MapGrids.Name, defaultvalue: "None");
            foreach (var item in MapGrids.DropDownItems)
                if (item.GetType().Name == "ToolStripMenuItem")
                {
                    var theItem = (ToolStripMenuItem)item;
                    theItem.Checked = theItem.Text == mapGridItem;
                    if (theItem.Checked)
                        MapGrids.Tag = theItem.Tag;
                }
            var mapBearingItem = RegEdit.Getkey(keyname: MapBearingMenuItem.Name, defaultvalue: "0");
            foreach (var item in MapBearingMenuItem.DropDownItems)
                if (item.GetType().Name == "ToolStripMenuItem")
                {
                    var theItem = (ToolStripMenuItem)item;
                    theItem.Checked = (string)theItem.Tag == mapBearingItem;
                    if (theItem.Checked)
                    {
                        MapBearingMenuItem.Tag = theItem.Tag;
                        MapBox.Bearing = float.Parse((string)theItem.Tag);
                    }
                }
            ZoomLevelLabel.Text = $@"{MapBox.Zoom:#0.#} / {MapBox.MaxZoom:#0.#}";
        }

        /// <summary>
        /// 缩放级变更响应函数
        /// </summary>
        private void MapBox_OnMapZoomChanged()
        {
            BeginInvoke(
                method: () =>
                {
                    ZoomLevelLabel.Text = $@"{MapBox.Zoom:#0.#} / {MapBox.MaxZoom:#0.#}";
                    DrawMapGrid();
                }
            );
        }

        /// <summary>
        /// 图窗鼠标移动响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapBox_MouseMove(object sender, MouseEventArgs e = null)
        {
            BeginInvoke(
                method: () =>
                {
                    var positionBoxTag = ((string srid, (double? lng, double? lat) position))PositionBox.Tag;
                    double lng, lat;
                    if (e != null)
                    {
                        base.OnMouseMove(e: e);
                        var position = MapBox.FromLocalToLatLng(x: e.X, y: e.Y);
                        positionBoxTag.position.lng = lng = position.Lng;
                        positionBoxTag.position.lat = lat = position.Lat;
                    }
                    else
                    {
                        lng = positionBoxTag.position.lng ?? 0.0;
                        lat = positionBoxTag.position.lat ?? 0.0;
                    }
                    PositionBox.Text = positionBoxTag.srid switch
                    {
                        "DEG" => $@"{lng:###.0000000000} / {lat:##.0000000000}",
                        "DMS" => $@"{Ellipsoid.Degree2Dms(Degree: $"{lng}", Digit: "2")} / {Ellipsoid.Degree2Dms(Degree: $"{lat}", Digit: "2")}",
                        "Beijing 1954" => string.Join(separator: " / ",
                                values: Ellipsoid.GaussKruger(longitude: $"{lng}", latitude: $"{lat}", crs: "1954")
                                    .Split(separator: ',')
                                    .Select(selector: xy => $"{double.Parse(s: xy):#.000}")),
                        "Xian 1980" => string.Join(separator: " / ",
                                values: Ellipsoid.GaussKruger(longitude: $"{lng}", latitude: $"{lat}", crs: "1980")
                                    .Split(separator: ',')
                                    .Select(selector: xy => $"{double.Parse(s: xy):#.000}")),
                        "CGCS 2000" => string.Join(separator: " / ",
                                values: Ellipsoid.GaussKruger(longitude: $"{lng}", latitude: $"{lat}", crs: "2000")
                                    .Split(separator: ',')
                                    .Select(selector: xy => $"{double.Parse(s: xy):#.000}")),
                        "Web Mercator" => string.Join(separator: " / ",
                                values: Ellipsoid.WebMercator(lng: lng, lat: lat).Split(separator: ',')
                                    .Select(selector: xy => $"{double.Parse(s: xy):#.000}")),
                        _ => string.Empty
                    };
                    var scaleX = Models.MapGrids.Run(lamuda: lng, fai: lat, scale: MapGrid.AutoScale);
                    if (MapGrids.Tag.ToString() != "None")
                    {
                        MapGrids.Text = scaleX.DescendantsAndSelf(name: "new").FirstOrDefault()?.Value;
                        MapGrids.ToolTipText = scaleX.DescendantsAndSelf(name: "old").FirstOrDefault()?.Value;
                    }
                    else
                    {
                        MapGrids.Text = "";
                        MapGrids.ToolTipText = "";
                    }
                }
            );
        }

        private void MapBox_OnMapDrag()
        {
            _mapDrag = true;
        }

        private void MapBox_MouseUp(object sender, MouseEventArgs e = null)
        {
            if (_mapDrag)
            {
                _mapDrag = false;
                DrawMapGrid();
            }
        }

        private void MapBox_SizeChanged(object sender, EventArgs e)
        {
            DrawMapGrid();
        }

        private void DrawMapGrid()
        {
            BeginInvoke(
                method: () =>
                {
                    var scaleOption = MapGrids.Tag.ToString();
                    if (scaleOption != "None")
                    {
                        var viewArea = MapBox.ViewArea; //MapBox.ClientRectangle; --> 图窗尺寸变更事件发生后，此值并非调整后的范围！ 
                        MapGrid.View(
                            option: scaleOption,
                            zoom: (int)MapBox.Zoom,
                            boundary: (
                                TopLeft: (Latitude: viewArea.Lat, Longitude: viewArea.Lng),
                                BottomRight: (Latitude: viewArea.Lat - viewArea.HeightLat,
                                    Longitude: viewArea.Lng + viewArea.WidthLng)
                            ),
                            gridPen: PreviewStyleForm.Style == null
                                ? new Pen(Color.FromArgb(255, 255, 255), 1)
                                : PreviewStyleForm.Style.Value.mapGrid
                        );
                    }
                    else
                        MapGrid.View();
                }
            );
        }

        /// <summary>
        /// 国际图幅分幅网格计算方法切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapGridMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke(
                method: () =>
                {
                    var checkedItem = (ToolStripMenuItem)sender;
                    foreach (var item in MapGrids.DropDownItems)
                    {
                        if (item.GetType().Name == "ToolStripMenuItem")
                        {
                            var theItem = (ToolStripMenuItem)item;
                            theItem.Checked = theItem.Name == checkedItem.Name;
                            if (theItem.Checked)
                            {
                                MapGrids.Tag = theItem.Tag;
                                DrawMapGrid();
                            }
                        }
                    }
                    RegEdit.Setkey(keyname: MapGrids.Name, defaultvalue: checkedItem.Text);
                }
            );
        }

        /// <summary>
        /// 瓦片服务加载响应函数
        /// </summary>
        private void MapBox_OnTileLoadStart()
        {
            TileLoading.Count = 0;
            TileLoading.Run();
        }

        /// <summary>
        /// 瓦片服务加载完成函数
        /// </summary>
        /// <param name="elapsedMilliseconds"></param>
        private void MapBox_OnTileLoadComplete(long elapsedMilliseconds)
        {
            TileLoading.Run(onOff: false);
        }

        /// <summary>
        /// 底图类型变更响应函数
        /// </summary>
        /// <param name="type"></param>
        private void MapBox_OnMapTypeChanged(GMapProvider type)
        {
            MapBox.MinZoom = MapBox.MapProvider.MinZoom;
            MapBox.MaxZoom = MapBox.MapProvider.MaxZoom ?? 18;
            var zoom = MapBox.Zoom;
            if (zoom < MapBox.MinZoom)
            {
                MapBox.Zoom = MapBox.MinZoom;
                MapBox_OnMapZoomChanged();
            }
            else
            {
                if (zoom > MapBox.MaxZoom)
                {
                    MapBox.Zoom = MapBox.MaxZoom;
                    MapBox_OnMapZoomChanged();
                }
            }
            MapBox.ReloadMap();
            TileLoading.Run(onOff: null);
        }

        /// <summary>
        /// 点要素单击响应函数
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        private void MapBox_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            MapBox_FeatureClick(itemTag: item.Tag, e: e);
        }

        /// <summary>
        /// 点要素鼠标移入响应函数
        /// </summary>
        /// <param name="item"></param>
        private void MapBox_OnMarkerEnter(GMapMarker item)
        {
            BeginInvoke(method: () =>
                {
                    try
                    {
                        switch (item.GetType().Name)
                        {
                            case "GMapMarkerPushpin":
                                {
                                    var marker = (GMapMarkerPushpin)item;
                                    marker.IsSelected = true;
                                    marker.IsVisible = true;
                                    break;
                                }
                            case "GMapMarkerRect":
                                {
                                    var marker = (GMapMarkerRect)item;
                                    marker.IsSelected = true;
                                    marker.IsVisible = true;
                                    break;
                                }
                            case "GMapMarkerCircle":
                                {
                                    var marker = (GMapMarkerCircle)item;
                                    marker.IsSelected = true;
                                    marker.IsVisible = true;
                                    break;
                                }
                            case "GMapMarkerGround":
                                {
                                    var marker = (GMapMarkerGround)item;
                                    marker.IsSelected = true;
                                    marker.IsVisible = true;
                                    break;
                                }
                                //default:
                                //    break;
                        }
                    }
                    catch
                    {
                        //
                    }
                }
            );
        }

        /// <summary>
        /// 点要素鼠标移出响应函数
        /// </summary>
        /// <param name="item"></param>
        private void MapBox_OnMarkerLeave(GMapMarker item)
        {
            BeginInvoke(method: () =>
                {
                    try
                    {
                        switch (item.GetType().Name)
                        {
                            case "GMapMarkerPushpin":
                                {
                                    var marker = (GMapMarkerPushpin)item;
                                    marker.IsSelected = false;
                                    marker.IsVisible = true;
                                    break;
                                }
                            case "GMapMarkerRect":
                                {
                                    var marker = (GMapMarkerRect)item;
                                    marker.IsSelected = false;
                                    marker.IsVisible = true;
                                    break;
                                }
                            case "GMapMarkerCircle":
                                {
                                    var marker = (GMapMarkerCircle)item;
                                    marker.IsSelected = false;
                                    marker.IsVisible = true;
                                    break;
                                }
                            case "GMapMarkerGround":
                                {
                                    var marker = (GMapMarkerGround)item;
                                    marker.IsSelected = false;
                                    marker.IsVisible = true;
                                    break;
                                }
                                //default:
                                //    break;
                        }
                    }
                    catch
                    {
                        //
                    }
                }
            );
        }

        /// <summary>
        /// 线要素鼠标单击响应函数
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        private void MapBox_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            MapBox_FeatureClick(itemTag: item.Tag, e: e);
        }

        /// <summary>
        /// 线要素鼠标移入响应函数
        /// </summary>
        /// <param name="item"></param>
        private void MapBox_OnRouteEnter(GMapRoute item)
        {
            BeginInvoke(method: () =>
                {
                    var line = (GMapRouteLine)item;
                    line.IsSelected = true;
                    line.IsVisible = true;
                }
            );
        }

        /// <summary>
        /// 线要素鼠标移出响应函数
        /// </summary>
        /// <param name="item"></param>
        private void MapBox_OnRouteLeave(GMapRoute item)
        {
            BeginInvoke(
                method: () =>
                {
                    var line = (GMapRouteLine)item;
                    line.IsSelected = false;
                    line.IsVisible = true;
                }
            );
        }

        /// <summary>
        /// 面要素鼠标单击响应函数
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        private void MapBox_OnPolygonClick(GMapPolygon item, MouseEventArgs e)
        {
            MapBox_FeatureClick(itemTag: item.Tag, e: e);
        }

        /// <summary>
        /// 面要素鼠标移入响应函数
        /// </summary>
        /// <param name="item"></param>
        private void MapBox_OnPolygonLeave(GMapPolygon item)
        {
            BeginInvoke(method: () =>
                {
                    var polygon = (GMapPolygonArea)item;
                    polygon.IsSelected = false;
                    polygon.IsVisible = true;
                }
            );
        }

        /// <summary>
        /// 面要素鼠标移出响应函数
        /// </summary>
        /// <param name="item"></param>
        private void MapBox_OnPolygonEnter(GMapPolygon item)
        {
            BeginInvoke(method: () =>
                {
                    var polygon = (GMapPolygonArea)item;
                    polygon.IsSelected = true;
                    polygon.IsVisible = true;
                }
            );
        }

        /// <summary>
        /// 矢量要素单击事件处理函数
        /// </summary>
        /// <param name="itemTag"></param>
        /// <param name="e"></param>
        private void MapBox_FeatureClick(object itemTag, MouseEventArgs e)
        {
            BeginInvoke(
                method: () =>
                {
                    if (e.Button != MouseButtons.Left || itemTag == null)
                        return;
                    try
                    {
                        var descriptionJson = (((JObject property, JObject style))itemTag).property?.ToString(formatting: Formatting.Indented);
                        //var descriptionXml = JsonConvert.DeserializeXNode(descriptionJson, "property")?.Root?.ToString(SaveOptions.None);
                        MapBoxProperty.Text = descriptionJson; //descriptionXml

                        var styleJson = (((JObject property, JObject style))itemTag).style?.ToString(formatting: Formatting.Indented);
                        MapBoxStyle.Text = styleJson;
                    }
                    catch (Exception error)
                    {
                        DatabaseLogAdd(input: statusText.Text = error.Message);
                    }
                }
            );
        }

        private void MapBox_KeyUp(object sender, KeyEventArgs e)
        {
            //若图窗内按下了ESC键，中止绘图任务
            if (e.KeyData != Keys.Escape)
                return;
            BreakMapViewTask();
        }

        private void BreakMapViewTask()
        {
            foreach (var task in MapView.Tasks)
                task.CancelTask();
            DatabaseLogAdd(input: statusText.Text = @"Preview drawing task canceled.");
        }

        /// <summary>
        /// 实时坐标计算方法切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke(method: () =>
                {
                    var previousTag = ((string srid, (double? lng, double? lat) position))PositionBox.Tag;
                    var lng = previousTag.position.lng;
                    var lat = previousTag.position.lat;
                    var checkedItem = (ToolStripMenuItem)sender;
                    foreach (var item in PositionBox.DropDownItems)
                        if (item.GetType().Name == "ToolStripMenuItem")
                        {
                            var theItem = (ToolStripMenuItem)item;
                            theItem.Checked = theItem.Name == checkedItem.Name;
                        }
                    PositionBox.Tag = (checkedItem.Text, (lng, lat));
                    MapBox_MouseMove(sender: sender);
                    RegEdit.Setkey(keyname: PositionBox.Name, defaultvalue: checkedItem.Text);
                }
            );
        }

        /// <summary>
        /// 实时坐标文本复制到裁剪版
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionBox_DoubleClick(object sender, EventArgs e)
        {
            Invoke(method: () =>
                {
                    Clipboard.SetData(format: DataFormats.Text, data: PositionBox.Text);
                    MessageBox.Show(text: @"Successfully copied to Clipboard", caption: @"Tip");
                }
            );
        }

        /// <summary>
        /// 底图切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapProviderMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke(method: () =>
                {
                    var baseMap = (ToolStripMenuItem)sender;
                    MapProviderDropDown.Text = baseMap.Text;
                    foreach (var item in MapProviderDropDown.DropDownItems)
                    {
                        if (item.GetType().Name != "ToolStripMenuItem")
                            continue;
                        var theItem = (ToolStripMenuItem)item;
                        theItem.Checked = theItem.Text == baseMap.Text;
                    }
                    MapBox.MapProvider = GMapProviderDictionary[key: baseMap.Text];
                    RegEdit.Setkey(keyname: MapProviderDropDown.Name, defaultvalue: baseMap.Text);
                }
            );
        }

        /// <summary>
        /// 显示或隐藏视图组件（中心十字丝、线型比例尺）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomLevelLabel_Click(object sender, EventArgs e)
        {
            BeginInvoke(method: () =>
                {
                    var redCross = !MapBox.ShowCenter;
                    MapBox.ShowCenter = //默认情况下，GMap.NET 控件在地图上显示一个【红十字】，以便准确显示中心的位置
                        MapBox.MapScaleInfoEnabled =
                            //线状比例尺
                            //MapBox.ShowTileGridLines = //瓦片网格
                            redCross;
                    RegEdit.Setkey(keyname: ZoomLevelLabel.Name, defaultvalue: redCross ? "1" : "0");
                    MapBox.Refresh();
                }
            );
        }

        /// <summary>
        /// 生成视图快照（jpeg格式+世界文件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageMaker_Click(object sender, EventArgs e)
        {
            new MapSnapshot(mainForm: this)
            {
                Owner = this
            }.Show();
        }

        private void DatabaseLogIcon_Click(object sender, EventArgs e)
        {
            DatabaseLog.Items.Clear();
        }

        private void FileLoadLogIcon_Click(object sender, EventArgs e)
        {
            FileLoadLog.Items.Clear();
        }

        private void CatalogTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = e?.Node;
            if (node == null)
                return;
            if (node.Tag == null)
            {
                //处理因延迟渲染导致的异常情况
                if (node.Parent == null)
                    return;
                node = node.Parent;
                node.Tag = 1;
            }
            //1=需要异步通讯获取子节点; 0=不再需要执行异步通讯
            if (node.Tag?.ToString() != "1")
                return;
            var toolTipText = node.ToolTipText.Split(separator: '\n');
            /*  toolTipText包括：
                Layer - [?.?]                                  
            */
            _catalogTreeObject.InsertNodes(
                currentNode: node,
                typeName:
                (
                    toolTipText.Length >= 1
                        ? Regex.Replace(
                            input: toolTipText[0],
                            pattern: @"^Layer[^\[]*?\[([\s\S]*)\]$",
                            replacement: "$1",
                            options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline
                        ).Trim()
                        : "*"
                ) + ".*"
            );
        }

        private void CatalogTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            var node = e?.Node;
            var label = e?.Label;
            if (node == null || label == null)
                return; //如果取消了编辑态，控件自动恢复原始文本并直接返回
            string errorMessage = null;
            if (node.Level == 0)
                errorMessage = @"The root node name does not need to be modified.";
            else
            {
                if (_administrator)
                    try
                    {
                        //首先判断是否符合xml节点命名规范
                        var newLabel = new XElement(name: label).Name.ToString();
                        //然后识别节点名称里是否出现【./ \"'】字符
                        var foundMatch = Regex.IsMatch(
                            input: newLabel,
                            pattern: @"[\.\\/*\s\""\']+",
                            options: RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Multiline
                        );
                        if (foundMatch)
                            errorMessage = @"Special characters are not allowed.";
                        else
                        {
                            if (
                                MessageBox.Show(
                                    text: @$"Change from [{Regex.Replace(input: node.Text, pattern: @"(\s*-\s*\[[\s\S]+\]\s*)", replacement: "", options: RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Multiline)}] to [{newLabel}], are you sure?",
                                    caption: @"Confirm",
                                    buttons: MessageBoxButtons.OKCancel,
                                    icon: MessageBoxIcon.Question
                                ) == DialogResult.OK
                            )
                            {
                                DatabaseLogAdd(input: statusText.Text = @"Node name changing ...");
                                _catalogTreeObject.EditNodeLabel(node: node, label: newLabel);
                                DatabaseLogAdd(input: statusText.Text = @"Node name changed successfully.");
                                //_databaseGridObject?.Reset();
                                _catalogTreeObject?.Reset();
                                return;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        errorMessage = error.Message;
                    }
                else
                    errorMessage = @"Administrator identity is required.";
            }
            if (errorMessage != null)
            {
                DatabaseLogAdd(input: statusText.Text = errorMessage);
                MessageBox.Show(text: errorMessage, caption: @"Caution", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
            e.CancelEdit = true; //取消编辑更改，控件自动恢复原始文本
        }

        private void CatalogTreeNode_Remove(TreeNode node)
        {
            try
            {
                if (_administrator)
                {
                    if (node.Level == 0)
                        throw new Exception(message: @"The root node name does not need to be deleted.");
                    if (MessageBox.Show(
                            text: @$"[{node.Text}] node and all its child nodes will be deleted. Are you sure?",
                            caption: @"Confirm",
                            buttons: MessageBoxButtons.OKCancel,
                            icon: MessageBoxIcon.Question
                        ) != DialogResult.OK)
                        return;
                    DatabaseLogAdd(input: statusText.Text = @"Node is being deleted ...");
                    _catalogTreeObject?.DeleteNode(node: node);
                    _catalogTreeObject?.Reset();
                    DatabaseLogAdd(input: statusText.Text = @"Node deleted successfully.");
                    _databaseGridObject?.Reset();
                }
                else
                    throw new Exception(message: @"Administrator identity is required.");
            }
            catch (Exception error)
            {
                DatabaseLogAdd(input: statusText.Text = error.Message);
            }
        }

        private void CatalogTreeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e?.KeyCode != Keys.Delete)
                return;
            CatalogTreeNode_Remove(node: ((TreeView)sender).SelectedNode);
        }

        private void CatalogTreeMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var nodes = CatalogTreeView.SelectedNode;
                if (nodes == null)
                    return;
                var theSender = (ToolStripMenuItem)sender;
                switch (theSender.Text)
                {
                    case "Rename":
                        {
                            nodes.BeginEdit();
                            break;
                        }
                    case "Remove":
                        {
                            CatalogTreeNode_Remove(node: nodes);
                            break;
                        }
                    case "Refresh":
                        {
                            _catalogTreeObject?.Reset();
                            break;
                        }
                }
            }
            catch (Exception error)
            {
                DatabaseLogAdd(input: statusText.Text = error.Message);
            }
        }

        private void DatabaseTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormEventChanged(sender: sender);
        }

        private void MapBearing_Click(object sender, EventArgs e)
        {
            var checkedItem = (ToolStripMenuItem)sender;
            var bearing = (string)checkedItem.Tag;
            MapBox.Bearing = bearing switch
            {
                "90" => 90,
                "180" => 180,
                "270" => 270,
                _ => 0 //360
            };
            foreach (var item in MapBearingMenuItem.DropDownItems)
                if (item.GetType().Name == "ToolStripMenuItem")
                {
                    var theItem = (ToolStripMenuItem)item;
                    theItem.Checked = theItem.Name == checkedItem.Name;
                    if (!theItem.Checked)
                        continue;
                    MapBearingMenuItem.Tag = theItem.Tag;
                    RegEdit.Setkey(keyname: MapBearingMenuItem.Name, defaultvalue: bearing);
                }
        }

        private void GeositeServerPanel_Click(object sender, EventArgs e)
        {
            var serverUrl = GeositeServerUrl.Text.Trim();
            try
            {
                if (new Ping().Send(hostNameOrAddress: new UriBuilder(uri: serverUrl).Host, timeout: 3000) is { Status: IPStatus.Success })
                    Process.Start(startInfo: new ProcessStartInfo(fileName: serverUrl) { UseShellExecute = true });
            }
            catch (Exception error)
            {
                DatabaseLogAdd(input: statusText.Text = error.Message);
            }
        }

        private void FileListMenuItem_Click(object sender, EventArgs e)
        {
            var theSender = (ToolStripMenuItem)sender;
            var grid = ((ContextMenuStrip)theSender.Owner).SourceControl?.Name == "FileGridView" ? FileGridView : vectorFilePool;
            switch (theSender.Text)
            {
                case "Select All":
                    {
                        grid.SelectAll();
                        break;
                    }
                case "Remove Selected":
                    {
                        var rows = new List<int>();
                        foreach (var cell in grid.SelectedCells)
                        {
                            int? rowIndex = cell.GetType().Name switch
                            {
                                "DataGridViewTextBoxCell" => ((DataGridViewTextBoxCell)cell).RowIndex,
                                "DataGridViewButtonCell" => ((DataGridViewButtonCell)cell).RowIndex,
                                _ => null
                            };
                            if (rowIndex != null && !rows.Contains(item: rowIndex.Value))
                                rows.Add(item: rowIndex.Value);
                        }
                        foreach (var row in rows.OrderDescending())
                            grid.Rows.Remove(dataGridViewRow: grid.Rows[index: row]);
                        break;
                    }
            }
        }

        private void CleanFileGridView()
        {
            foreach (var row in FileGridView.SelectedRows.Cast<DataGridViewRow>().Where(predicate: row => !row.IsNewRow))
            {
                try
                {
                    FileGridView.Rows.Remove(dataGridViewRow: row);
                }
                catch
                {
                    //
                }
            }
            FileGridViewRowsClean();
        }

        private void TextBoxMenuItem_Click(object sender, EventArgs e)
        {
            var theSender = (ToolStripMenuItem)sender;
            var viewName = ((ContextMenuStrip)theSender.Owner).SourceControl?.Name;
            switch (viewName)
            {
                case "FileLoadLog" or "DatabaseLog":
                    {
                        var viewObject = viewName == "FileLoadLog" ? FileLoadLog : DatabaseLog;
                        switch (theSender.Text)
                        {
                            case "Select All":
                                {
                                    for (var i = 0; i < viewObject.Items.Count; i++)
                                        viewObject.SetSelected(index: i, value: true);
                                    break;
                                }
                            case "Copy to Clipboard":
                                {
                                    var sb = new StringBuilder();
                                    for (var index = 0; index < viewObject.SelectedItems.Count; index++)
                                    {
                                        var item = viewObject.SelectedItems[index: index];
                                        sb.Append(value: item);
                                        if (index != viewObject.SelectedItems.Count - 1)
                                            sb.AppendLine();
                                    }
                                    if (sb.Length > 0)
                                        Clipboard.SetData(format: DataFormats.Text, data: sb.ToString());
                                    break;
                                }
                        }
                        break;
                    }
                case "MapBoxProperty":
                    {
                        var viewObject = MapBoxProperty;
                        switch (theSender.Text)
                        {
                            case "Select All":
                                {
                                    viewObject.SelectAll();
                                    break;
                                }
                            case "Copy to Clipboard":
                                {
                                    if (viewObject.SelectionLength > 0)
                                        Clipboard.SetText(text: viewObject.SelectedText, format: TextDataFormat.Text);
                                    break;
                                }
                        }
                        break;
                    }
                case "MapBoxStyle":
                    {
                        var viewObject = MapBoxStyle;
                        switch (theSender.Text)
                        {
                            case "Select All":
                                {
                                    viewObject.SelectAll();
                                    break;
                                }
                            case "Copy to Clipboard":
                                {
                                    if (viewObject.SelectionLength > 0)
                                        Clipboard.SetText(text: viewObject.SelectedText, format: TextDataFormat.Text);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 地图窗口右键菜单处理函数，包括矢量图层充满窗口、移除矢量图层、 清理瓦片缓存和矢量要素存盘功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapBoxMenuItem_Click(object sender, EventArgs e)
        {
            var theSender = (ToolStripMenuItem)sender;
            switch (theSender.Text)
            {
                case "Zoom to Layer":
                    {
                        var left = double.MaxValue;
                        var top = double.MinValue;
                        var right = double.MinValue;
                        var bottom = double.MaxValue;
                        var hasFeature = false;
                        foreach (var rect in new List<RectLatLng?>
                             {
                                 MapBox.GetRectOfAllMarkers(overlayId: null),
                                 MapBox.GetRectOfAllRoutes(overlayId: null),
                                 Functions.GetRectOfAllPolygons(overlayId: null, mapControl: MapBox)
                             }.Where(predicate: rect => rect != null))
                        {
                            if (rect.Value.Left < left)
                                left = rect.Value.Left;
                            if (rect.Value.Top > top)
                                top = rect.Value.Top;
                            if (rect.Value.Right > right)
                                right = rect.Value.Right;
                            if (rect.Value.Bottom < bottom)
                                bottom = rect.Value.Bottom;
                            hasFeature = true;
                        }

                        if (hasFeature)
                            MapBox.SetZoomToFitRect(rect: RectLatLng.FromLTRB(leftLng: left, topLat: top, rightLng: right,
                                bottomLat: bottom));
                        break;
                    }
                case "Clear Layer":
                    {
                        MapView.Features.Markers.Clear();
                        MapView.Features.Routes.Clear();
                        MapView.Features.Polygons.Clear();
                        TopologyCheckerForm.Features.Markers.Clear();
                        TopologyCheckerForm.Features.Routes.Clear();
                        TopologyCheckerForm.Features.Polygons.Clear();
                        GMapProvider.OverlayTiles = new List<GMapProvider>();
                        MapBox.ReloadMap();
                        break;
                    }
                case "Clear Tiles Cache":
                    {
                        if (
                            MessageBox.Show(
                                text: @"Are You sure?",
                                caption: @"Clear tiles cache?",
                                buttons: MessageBoxButtons.OKCancel,
                                icon: MessageBoxIcon.Warning
                            ) != DialogResult.OK)
                            return;
                        try
                        {
                            BeginInvoke(
                                method: () =>
                                {
                                    FileLoadLogAdd(input: statusText.Text = @"Tiles cache cleaning up ...");
                                    Application.DoEvents();
                                    MapBox.Manager.PrimaryCache.DeleteOlderThan(date: DateTime.Now, type: null);
                                    FileLoadLogAdd(input: statusText.Text = @"Tiles cache cleaning completed.");
                                }
                            );
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(text: ex.Message);
                        }
                        break;
                    }
                case "Cancel Drawing (ESC)":
                    {
                        BeginInvoke(method: BreakMapViewTask);
                        break;
                    }
                case "Vector SaveAs ...":
                    {
                        var markers = MapView.Features.Markers;
                        var routes = MapView.Features.Routes;
                        var mapGrids = MapGrid.Features.Routes;
                        var polygons = MapView.Features.Polygons;
                        var markerCount = markers.Count;
                        var routeCount = routes.Count;
                        var mapGridCount = mapGrids.Count;
                        var polygonCount = polygons.Count;
                        var total = markerCount + routeCount + mapGridCount + polygonCount;
                        if (total > 0)
                        {
                            FileLoadLogAdd(input: statusText.Text =
                                @"SaveAs can be implemented using GetFeature / GeositeServer.");
                            Application.DoEvents();
                            var key = vectorSaveButton.Name;
                            var path = key + "_path";
                            var oldPath = RegEdit.Getkey(keyname: path);
                            int.TryParse(s: RegEdit.Getkey(keyname: key), result: out var filterIndex);
                            var saveFileDialog = new SaveFileDialog
                            {
                                Filter = @"ESRI ShapeFile(*.shp)|*.shp|GeoJSON(*.geojson)|*.geojson|GoogleEarth(*.kml)|*.kml|Gml(*.gml)|*.gml|GeositeXML(*.xml)|*.xml",
                                FilterIndex = filterIndex
                            };
                            if (Directory.Exists(path: oldPath))
                                saveFileDialog.InitialDirectory = oldPath;
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                RegEdit.Setkey(keyname: key, defaultvalue: $"{saveFileDialog.FilterIndex}");
                                RegEdit.Setkey(keyname: path, defaultvalue: Path.GetDirectoryName(path: saveFileDialog.FileName));
                                var saveAsFileName = saveFileDialog.FileName;
                                var fileType = Path.GetExtension(path: saveAsFileName).ToLower();
                                try
                                {
                                    var xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                                    var rootX = new XElement("FeatureCollection");
                                    rootX.Add(new XAttribute("timeStamp", DateTime.Now.ToString("s")));
                                    rootX.Add(new XElement("name", "Untitled"));
                                    var featureId = 0;
                                    var featureTimeStamp = DateTime.Now.ToString("s");
                                    if (markerCount > 0)
                                        lock (MapView.Features.Markers)
                                        {
                                            foreach (var marker in MapView.Features.Markers)
                                            {
                                                if (marker.IsVisible)
                                                {
                                                    var branchList =
                                                        Regex.Split("Markers", @"[\/\\\|\@]+")
                                                            .Where(layer => !string.IsNullOrWhiteSpace(layer))
                                                            .Select(layer => layer)
                                                            .ToArray();
                                                    var branchX = rootX;
                                                    foreach (var branch in branchList)
                                                    {
                                                        if (branchX.Elements("layer")
                                                            .All(x => x.Element("name")?.Value != branch))
                                                            branchX.Add(new XElement("layer",
                                                                new XElement("name", branch)));
                                                        branchX = branchX.Elements("layer")
                                                            .First(x => x.Element("name")?.Value == branch);
                                                    }
                                                    var vertex = marker.Position;
                                                    var geometry = new JArray { vertex.Lng, vertex.Lat };
                                                    XElement propertyX = null;
                                                    XElement styleX = null;
                                                    var tag = marker.Tag;
                                                    if (tag != null)
                                                    {
                                                        var (property, style) = ((JObject property, JObject style))tag;
                                                        propertyX = property != null
                                                            ? JsonConvert.DeserializeXNode(
                                                                property.ToString(Formatting.None),
                                                                "property")?.Root
                                                            : null;
                                                        styleX = style != null
                                                            ? JsonConvert.DeserializeXNode(style.ToString(Formatting.None),
                                                                "style")?.Root
                                                            : null;
                                                    }
                                                    branchX.Add(
                                                        new XElement(
                                                            "member",
                                                            new XAttribute("type", "Point"),
                                                            new XAttribute("typeCode", "1"),
                                                            new XAttribute("id", featureId),
                                                            new XAttribute("timeStamp", featureTimeStamp),
                                                            new XElement(
                                                                "geometry",
                                                                new XAttribute("format", "WKT"),
                                                                new XAttribute("type", "Point"),
                                                                new XAttribute("centroid",
                                                                    $"({geometry[0]} {geometry[1]})"),
                                                                new XAttribute("boundary",
                                                                    $"(({geometry[0]} {geometry[1]},{geometry[0]} {geometry[1]},{geometry[0]} {geometry[1]},{geometry[0]} {geometry[1]},{geometry[0]} {geometry[1]}))"),
                                                                $"({string.Join(" ", geometry)})"
                                                            ),
                                                            propertyX ?? new XElement("property", new XElement("id", featureId)),
                                                            styleX
                                                        )
                                                    );
                                                    featureId++;
                                                }
                                            }
                                        }
                                    if (routeCount > 0)
                                        lock (MapView.Features.Routes)
                                        {
                                            foreach (var route in MapView.Features.Routes)
                                            {
                                                if (route.IsVisible)
                                                {
                                                    var branchList =
                                                        Regex.Split("Routes", @"[\/\\\|\@]+")
                                                            .Where(layer => !string.IsNullOrWhiteSpace(layer))
                                                            .Select(layer => layer)
                                                            .ToArray();
                                                    var branchX = rootX;
                                                    foreach (var branch in branchList)
                                                    {
                                                        if (branchX.Elements("layer")
                                                            .All(x => x.Element("name")?.Value != branch))
                                                            branchX.Add(new XElement("layer",
                                                                new XElement("name", branch)));
                                                        branchX = branchX.Elements("layer")
                                                            .First(x => x.Element("name")?.Value == branch);
                                                    }
                                                    var verteics = route.Points;
                                                    var geometry = new JArray();
                                                    foreach (var vertex in verteics)
                                                        geometry.Add(new JArray { vertex.Lng, vertex.Lat });
                                                    (JArray Centroid, JArray BBox) lineStringTopology;
                                                    try
                                                    {
                                                        lineStringTopology = GeositeXML.Topology.GetTopology(geometry, "LineString");
                                                    }
                                                    catch
                                                    {
                                                        lineStringTopology = (null, null);
                                                    }
                                                    XElement propertyX = null;
                                                    XElement styleX = null;
                                                    var tag = route.Tag;
                                                    if (tag != null)
                                                    {
                                                        var (property, style) = ((JObject property, JObject style))tag;
                                                        propertyX = property != null
                                                            ? JsonConvert.DeserializeXNode(
                                                                property.ToString(Formatting.None),
                                                                "property")?.Root
                                                            : null;
                                                        styleX = style != null
                                                            ? JsonConvert.DeserializeXNode(style.ToString(Formatting.None),
                                                                "style")?.Root
                                                            : null;
                                                    }
                                                    branchX.Add(
                                                        new XElement
                                                        (
                                                            "member",
                                                            new XAttribute("type", "Line"),
                                                            new XAttribute("typeCode", "2"),
                                                            new XAttribute("id", featureId),
                                                            new XAttribute("timeStamp", featureTimeStamp),
                                                            new XElement(
                                                                "geometry",
                                                                new XAttribute("format", "WKT"),
                                                                new XAttribute("type", "LineString"),
                                                                lineStringTopology.Centroid != null
                                                                    ? new XAttribute("centroid",
                                                                        $"({lineStringTopology.Centroid[0]} {lineStringTopology.Centroid[1]})")
                                                                    : null,
                                                                lineStringTopology.BBox != null
                                                                    ? new XAttribute("boundary",
                                                                        $"(({lineStringTopology.BBox[0]} {lineStringTopology.BBox[1]},{lineStringTopology.BBox[2]} {lineStringTopology.BBox[1]},{lineStringTopology.BBox[2]} {lineStringTopology.BBox[3]},{lineStringTopology.BBox[0]} {lineStringTopology.BBox[3]},{lineStringTopology.BBox[0]} {lineStringTopology.BBox[1]}))")
                                                                    : null,
                                                                $"({string.Join(",", from vertex in geometry select $"{vertex[0]} {vertex[1]}")})"
                                                            ),
                                                            propertyX ?? new XElement("property", new XElement("id", featureId)),
                                                            styleX
                                                        )
                                                    );
                                                    featureId++;
                                                }
                                            }
                                        }
                                    if (mapGridCount > 0)
                                        lock (MapGrid.Features.Routes)
                                        {
                                            foreach (var route in MapGrid.Features.Routes)
                                            {
                                                if (route.IsVisible)
                                                {
                                                    var branchList =
                                                        Regex.Split("MapGridLines", @"[\/\\\|\@]+")
                                                            .Where(layer => !string.IsNullOrWhiteSpace(layer))
                                                            .Select(layer => layer)
                                                            .ToArray();
                                                    var branchX = rootX;
                                                    foreach (var branch in branchList)
                                                    {
                                                        if (branchX.Elements("layer")
                                                            .All(x => x.Element("name")?.Value != branch))
                                                            branchX.Add(new XElement("layer",
                                                                new XElement("name", branch)));
                                                        branchX = branchX.Elements("layer")
                                                            .First(x => x.Element("name")?.Value == branch);
                                                    }
                                                    var verteics = route.Points;
                                                    var geometry = new JArray();
                                                    foreach (var vertex in verteics)
                                                        geometry.Add(new JArray { vertex.Lng, vertex.Lat });
                                                    (JArray Centroid, JArray BBox) lineStringTopology;
                                                    try
                                                    {
                                                        lineStringTopology = GeositeXML.Topology.GetTopology(geometry, "LineString");
                                                    }
                                                    catch
                                                    {
                                                        lineStringTopology = (null, null);
                                                    }
                                                    XElement propertyX = null;
                                                    XElement styleX = null;
                                                    var tag = route.Tag;
                                                    if (tag != null)
                                                    {
                                                        var (property, style) = ((JObject property, JObject style))tag;
                                                        propertyX = property != null
                                                            ? JsonConvert.DeserializeXNode(
                                                                property.ToString(Formatting.None),
                                                                "property")?.Root
                                                            : null;
                                                        styleX = style != null
                                                            ? JsonConvert.DeserializeXNode(style.ToString(Formatting.None),
                                                                "style")?.Root
                                                            : null;
                                                    }
                                                    branchX.Add(
                                                        new XElement
                                                        (
                                                            "member",
                                                            new XAttribute("type", "Line"),
                                                            new XAttribute("typeCode", "2"),
                                                            new XAttribute("id", featureId),
                                                            new XAttribute("timeStamp", featureTimeStamp),
                                                            new XElement(
                                                                "geometry",
                                                                new XAttribute("format", "WKT"),
                                                                new XAttribute("type", "LineString"),
                                                                lineStringTopology.Centroid != null
                                                                    ? new XAttribute("centroid",
                                                                        $"({lineStringTopology.Centroid[0]} {lineStringTopology.Centroid[1]})")
                                                                    : null,
                                                                lineStringTopology.BBox != null
                                                                    ? new XAttribute("boundary",
                                                                        $"(({lineStringTopology.BBox[0]} {lineStringTopology.BBox[1]},{lineStringTopology.BBox[2]} {lineStringTopology.BBox[1]},{lineStringTopology.BBox[2]} {lineStringTopology.BBox[3]},{lineStringTopology.BBox[0]} {lineStringTopology.BBox[3]},{lineStringTopology.BBox[0]} {lineStringTopology.BBox[1]}))")
                                                                    : null,
                                                                $"({string.Join(",", from vertex in geometry select $"{vertex[0]} {vertex[1]}")})"
                                                            ),
                                                            propertyX ?? new XElement("property", new XElement("id", featureId)),
                                                            styleX
                                                        )
                                                    );
                                                    featureId++;
                                                }
                                            }
                                        }
                                    if (polygonCount > 0)
                                        lock (MapView.Features.Polygons)
                                        {
                                            foreach (var polygon in MapView.Features.Polygons)
                                            {
                                                if (polygon.IsVisible)
                                                {
                                                    var branchList =
                                                        Regex.Split("Polygons", @"[\/\\\|\@]+")
                                                            .Where(layer => !string.IsNullOrWhiteSpace(layer))
                                                            .Select(layer => layer)
                                                            .ToArray();
                                                    var branchX = rootX;
                                                    foreach (var branch in branchList)
                                                    {
                                                        if (branchX.Elements("layer")
                                                            .All(x => x.Element("name")?.Value != branch))
                                                            branchX.Add(new XElement("layer",
                                                                new XElement("name", branch)));
                                                        branchX = branchX.Elements("layer")
                                                            .First(x => x.Element("name")?.Value == branch);
                                                    }
                                                    var verteics = polygon.Points;
                                                    var geometry = new JArray();
                                                    foreach (var vertex in verteics)
                                                        geometry.Add(new JArray { vertex.Lng, vertex.Lat });
                                                    geometry = new JArray { geometry };
                                                    (JArray Centroid, JArray BBox) polygonTopology;
                                                    try
                                                    {
                                                        polygonTopology = GeositeXML.Topology.GetTopology(geometry, "Polygon");
                                                    }
                                                    catch
                                                    {
                                                        polygonTopology = (null, null);
                                                    }
                                                    XElement propertyX = null;
                                                    XElement styleX = null;
                                                    var tag = polygon.Tag;
                                                    if (tag != null)
                                                    {
                                                        var (property, style) = ((JObject property, JObject style))tag;
                                                        propertyX = property != null
                                                            ? JsonConvert.DeserializeXNode(
                                                                property.ToString(Formatting.None),
                                                                "property")?.Root
                                                            : null;
                                                        styleX = style != null
                                                            ? JsonConvert.DeserializeXNode(style.ToString(Formatting.None),
                                                                "style")?.Root
                                                            : null;
                                                    }
                                                    branchX.Add(
                                                        new XElement(
                                                            "member",
                                                            new XAttribute("type", "Polygon"),
                                                            new XAttribute("typeCode", "3"),
                                                            new XAttribute("id", featureId),
                                                            new XAttribute("timeStamp", featureTimeStamp),
                                                            new XElement(
                                                                "geometry",
                                                                new XAttribute("format", "WKT"),
                                                                new XAttribute("type", "Polygon"),
                                                                polygonTopology.Centroid != null
                                                                    ? new XAttribute("centroid",
                                                                        $"({polygonTopology.Centroid[0]} {polygonTopology.Centroid[1]})")
                                                                    : null,
                                                                polygonTopology.BBox != null
                                                                    ? new XAttribute("boundary",
                                                                        $"(({polygonTopology.BBox[0]} {polygonTopology.BBox[1]},{polygonTopology.BBox[2]} {polygonTopology.BBox[1]},{polygonTopology.BBox[2]} {polygonTopology.BBox[3]},{polygonTopology.BBox[0]} {polygonTopology.BBox[3]},{polygonTopology.BBox[0]} {polygonTopology.BBox[1]}))")
                                                                    : null
                                                                , $"(({string.Join(",", from vertex in geometry[0] select $"{vertex[0]} {vertex[1]}")}))"
                                                            ),
                                                            propertyX ?? new XElement("property",
                                                                new XElement("id", featureId)),
                                                            styleX
                                                        )
                                                    );
                                                    featureId++;
                                                }
                                            }
                                        }
                                    xDocument.Add(rootX);
                                    using var geositeXml =
                                        new GeositeXml.GeositeXml(
                                            new XElement(
                                                "Projection",
                                                new XElement(
                                                    "From",
                                                    new XElement("Geography")
                                                ),
                                                new XElement(
                                                    "To",
                                                    new XElement("Geography")
                                                )
                                            )
                                        );
                                    switch (fileType)
                                    {
                                        case ".shp":
                                            {
                                                geositeXml.GeositeXmlToShp(rootX, saveAsFileName);
                                                break;
                                            }
                                        case ".geojson":
                                            {
                                                var result = geositeXml.GeositeXmlToGeoJson(rootX.Descendants());
                                                if (result != null)
                                                {
                                                    using var w = new StreamWriter(saveAsFileName, false, Encoding.UTF8);
                                                    w.WriteLine(result);
                                                }
                                                break;
                                            }
                                        case ".kml":
                                            {
                                                geositeXml.GeositeXmlToKml(rootX.Descendants()).Save(saveAsFileName);
                                                break;
                                            }
                                        case ".gml":
                                            {
                                                geositeXml.GeositeXmlToGml(rootX.Descendants()).Save(saveAsFileName);
                                                break;
                                            }
                                        case ".xml":
                                            {
                                                geositeXml.GeositeXmlToGeositeXml(rootX.Descendants()).Save(saveAsFileName);
                                                break;
                                            }
                                    }
                                    FileLoadLogAdd(input: statusText.Text = @$"[{featureId} / {total}] feature{(featureId > 1 ? "s" : "")} saved.");
                                }
                                catch (Exception error)
                                {
                                    FileLoadLogAdd(input: statusText.Text = error.Message);
                                }
                            }
                        }
                        else
                            FileLoadLogAdd(input: statusText.Text = @"No vector data found.");
                        break;
                    }
            }
        }

        private void PreviewStyle_Click(object sender, EventArgs e)
        {
            new PreviewStyleForm().ShowDialog();
        }

        private void TopologyCheckerButton_Click(object sender, EventArgs e)
        {
            new TopologyCheckerForm(this).Show();
        }

        private void rasterTileSize_MouseLeave(object sender, EventArgs e)
        {
            rasterTileSize.Text = int.TryParse(s: rasterTileSize.Text, result: out var size)
                ? size < 10
                    ? @"10"
                    : size > 4096 //太大时容易导致内存溢出
                        ? "4096"
                        : $"{size}"
                : @"100"; //WPS推荐尺寸
            FormEventChanged(sender: sender);
        }

        private void nodatabox_MouseLeave(object sender, EventArgs e)
        {
            nodatabox.Text = double.TryParse(s: nodatabox.Text, result: out var i) ? $@"{i}" : @"";
            FormEventChanged(sender: sender);
        }

        private void DataConvert_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(path: TileFormatOpenBox.Text))
            {
                if (Directory.Exists(path: TileFormatSaveBox.Text))
                {
                    DataConvert.Enabled = false;
                    var quickCopyTask = new QuickCopy(TileFormatOpenBox.Text, TileFormatSaveBox.Text);
                    quickCopyTask.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
                    {
                        switch (thisEvent.Code)
                        {
                            case 0:
                                _loading.Run();
                                break;
                            case 1:
                                _loading.Run(onOff: false);
                                break;
                            default:
                                _loading.Run(onOff: null);
                                break;
                        }
                        statusText.Text = thisEvent.Message ?? string.Empty;
                    };
                    quickCopyTask.Run();
                    DataConvert.Enabled = true;
                }
                else
                {
                    statusText.Text = string.IsNullOrWhiteSpace(TileFormatSaveBox.Text)
                        ? @"Please select a target folder."
                        : @$"[{TileFormatSaveBox.Text}] does not exist.";
                }
            }
            else
            {
                statusText.Text = string.IsNullOrWhiteSpace(TileFormatOpenBox.Text)
                    ? @"Please select a source folder."
                    : @$"[{TileFormatOpenBox.Text}] does not exist.";
            }
        }
    }
}
