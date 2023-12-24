using Geosite.GeositeServer.PostgreSQL;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Geosite.GeositeServer.Vector;
using Geosite.GeositeXML;
using Newtonsoft.Json;
using System.Text;
using Geosite.Messager;

namespace Geosite
{
    /// <summary>
    /// Vector Feature Export Class (Based on Database Layer Content)
    /// </summary>
    public partial class ExportForm : Form
    {
        /// <summary>
        /// Main window object
        /// </summary>
        private readonly MainForm _mainForm;

        /// <summary>
        /// Thematic Layer Name
        /// </summary>
        private readonly string _layerName;

        /// <summary>
        /// Thematic layer routing information
        /// </summary>
        private readonly string _layerRoute;

        /// <summary>
        /// Layer belonging to [tree root] identifier code
        /// </summary>
        private readonly int _treeId;

        /// <summary>
        /// Layered type code array
        /// </summary>
        private readonly string[] _types;

        /// <summary>
        /// TreeTop ID
        /// </summary>
        private int _layerId;

        /// <summary>
        /// The number of vector features contained in the layer
        /// </summary>
        private long _featureCount;

        /// <summary>
        /// Background Task Worker
        /// </summary>
        private readonly BackgroundWorker _backgroundWorker = new();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mainForm">Main window object</param>
        /// <param name="treeId">Layer belonging to [tree root] identifier code</param>
        /// <param name="types">Layered type code array</param>
        /// <param name="layerName">Thematic Layer Name</param>
        /// <param name="layerRoute">Thematic layer routing information</param>
        /// <param name="layerId">TreeTop ID</param>
        public ExportForm(MainForm mainForm, int treeId, string[] types, string layerName, string layerRoute, int layerId)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _treeId = treeId;
            _types = types;
            _layerName = layerName;
            _layerRoute = layerRoute;
            _layerId = layerId;
        }

        /// <summary>
        /// Form Load Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportForm_Load(object sender, EventArgs e)
        {
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            ExportStatusLabel.Text = _mainForm.GetCopyright;
            ExportLogAdd("Retrieving vector features ...");
            BeginInvoke(
                method: () =>
                {
                    var featureCount = PostgreSqlHelper.Scalar(
                        //1：Point点、2：Line线、3：Polygon面、4：Image贴图
                        cmd: $"SELECT count(*) FROM leaf WHERE branch = {_layerId} AND type BETWEEN 1 AND 4;",
                        timeout: 0
                    );
                    if (!long.TryParse($"{featureCount}", out _featureCount))
                        _featureCount = 0L;
                    if (_featureCount == 0)
                    {
                        ExportSaveButton.Enabled = ExportStartButton.Enabled = false;
                        ExportLogAdd("No vector features found.");
                    }
                    else
                    {
                        ExportSaveButton.Enabled = true;
                        ExportLogAdd($"{_featureCount} vector feature{(_featureCount > 1 ? "s" : "")} found.");
                    }
                }
            );
        }

        /// <summary>
        /// Form Closing Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// Clear LOG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ExportLog.Items.Clear();
        }

        /// <summary>
        /// Add a line of text to log list
        /// </summary>
        /// <param name="input">input string</param>
        private void ExportLogAdd(string input)
        {
            BeginInvoke(
                method: () =>
                {
                    ExportLog.Items.Add(item: input);
                    ExportLog.SelectedIndex = ExportLog.Items.Count - 1;
                    ExportLog.SelectedIndex = -1;
                }
            );
        }

        /// <summary>
        /// Export - Save Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportSaveButton_Click(object sender, EventArgs e)
        {
            var key = ExportSaveButton.Name;
            var path = key + "_path";
            var oldPath = RegEdit.GetKey(key: path);
            int.TryParse(s: RegEdit.GetKey(key: key), result: out var filterIndex);
            var saveFileDialog = new SaveFileDialog
            {
                Filter = @"ESRI ShapeFile(*.shp)|*.shp|GeoJSON(*.geojson)|*.geojson|GoogleEarth(*.kml)|*.kml|Gml(*.gml)|*.gml|GeositeXML(*.xml)|*.xml",
                FileName = _layerName,
                FilterIndex = filterIndex
            };
            if (Directory.Exists(path: oldPath))
                saveFileDialog.InitialDirectory = oldPath;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                RegEdit.SetKey(key: key, defaultValue: $"{saveFileDialog.FilterIndex}");
                RegEdit.SetKey(key: path, defaultValue: Path.GetDirectoryName(path: saveFileDialog.FileName));
                ExportPathTextBox.Text = saveFileDialog.FileName;
            }
            ExportStartButton.Enabled = !string.IsNullOrWhiteSpace(value: ExportPathTextBox.Text);
        }

        /// <summary>
        /// Start Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportStartButton_Click(object sender, EventArgs e)
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
                ExportStartButton.Image = Properties.Resources.run;
                ExportStartButton.ToolTipText = @"Start";
                ExportProgressBar.Value = 0;
                ExportProgressBar.Visible = false;
            }
            else
            {
                ExportStartButton.Image = Properties.Resources.stop;
                ExportStartButton.ToolTipText = @"Cancel";
                ExportProgressBar.Value = 0;
                ExportProgressBar.Visible = true;
                Application.DoEvents();
                _backgroundWorker.RunWorkerAsync(argument: (int.Parse(s: RegEdit.GetKey(key: ExportSaveButton.Name)), ExportPathTextBox.Text));
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressPercentage = e.ProgressPercentage;
            if (progressPercentage >= 0)
                ExportProgressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
                ExportLogAdd(ExportStatusLabel.Text = $@"{e.UserState}");
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ExportLogAdd(ExportStatusLabel.Text = e.Cancelled ? @"Cancelled." : e.Error == null ? @"Done." : $@"Error:{e.Error}");
            ExportProgressBar.Value = 0;
            ExportProgressBar.Visible = false;
            ExportStartButton.Image = Properties.Resources.run;
            ExportStartButton.ToolTipText = @"Start";
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            (XElement xml, string minId, string maxId, int numberMatched, int numberReturned)? GeositeXmlBuilder(
            DataTable leaves,
            int epsg = 4326,
            int format = 2
            )
            {
                static string ToGmlNodeName(string xmlNodeName)
                {
                    do
                        try
                        {
                            xmlNodeName = new XElement(name: string.IsNullOrWhiteSpace(value: xmlNodeName) ? "Untitled" : xmlNodeName).Name.LocalName;
                            break;
                        }
                        catch (XmlException xmlError)
                        {
                            xmlNodeName = xmlNodeName!.Replace(oldChar: xmlNodeName[index: xmlError.LinePosition - 1], newChar: '_');
                        }
                    while (true);
                    return xmlNodeName;
                }

                static string ToTimeStamp(string yyyyMMddHHmmss)
                {
                    var timeStampArray = Regex.Split
                    (
                        //年月日（yyyyMMdd）,时分秒（HHmmss）
                        yyyyMMddHHmmss,
                        @"[,\s]+"
                    );
                    var yyyyMMdd = ("00000000" + timeStampArray[0])[^8..];
                    var hhmmss = ("000000" + timeStampArray[1])[^6..];
                    return new DateTime(
                            int.Parse(yyyyMMdd[..4]),
                            //MM
                            int.Parse(yyyyMMdd.Substring(4, 2)),
                            //dd
                            int.Parse(yyyyMMdd.Substring(6, 2)),
                            //HH 
                            int.Parse(hhmmss[..2]),
                            //mm
                            int.Parse(hhmmss.Substring(2, 2)),
                            //ss
                            int.Parse(hhmmss.Substring(4, 2))
                        )
                        .ToString("s");
                }

                var members = leaves?.Rows;
                if (members == null)
                    return null;
                var columns = leaves.Columns;

                XNamespace wfsPrefix = "http://www.opengis.net/wfs/2.0";
                XNamespace gmlPrefix = "http://www.opengis.net/gml/3.2";
                XNamespace xsiPrefix = "http://www.w3.org/2001/XMLSchema-instance";

                var featureCollectionX = new XElement
                (
                    name: format == 0 ? "FeatureCollection" : wfsPrefix + "FeatureCollection",
                    new XAttribute(
                        name: "timeStamp",
                        value: DateTime.Now.ToString(format: "s")
                    ),
                    new XElement(
                        name: "name",
                        content: "Data"
                    ),
                    new XAttribute(name: "status", value: 0),
                    new XAttribute(name: "creator", value: "GeositeServer")
                );

                if (format == 0)
                    featureCollectionX.Add(
                        new XAttribute(
                            name: XNamespace.Xmlns + "wfs",
                            value: wfsPrefix
                        ),
                        new XAttribute(
                            name: XNamespace.Xmlns + "gml",
                            value: gmlPrefix
                        ),
                        new XAttribute(
                            name: XNamespace.Xmlns + "xlink",
                            value: "http://www.w3.org/1999/xlink"
                        ),
                        new XAttribute(
                            name: XNamespace.Xmlns + "xsi",
                            value: xsiPrefix
                        ),
                        new XAttribute(
                            name: xsiPrefix + "schemaLocation",
                            value: "http://www.opengis.net/wfs/2.0 http://schemas.opengis.net/wfs/2.0/wfs.xsd http://www.opengis.net/gml/3.2 http://schemas.opengis.net/gml/3.2.1/gml.xsd"
                        )
                    );

                var srsName = $"EPSG:{epsg}";

                featureCollectionX.Add(
                    content: format == 0
                        ? new XElement(
                            name: wfsPrefix + "boundedBy",
                            content: new XElement(
                                name: gmlPrefix + "Envelope",
                                new XAttribute(name: "srsDimension", value: "2"),
                                new XAttribute(name: "srsName", value: srsName)
                            )
                        )
                        : new XElement(
                            name: format is 3 or 4 ? "bbox" : "boundary",
                            content: new XAttribute(name: "srsName", value: srsName)
                        )
                );

                var numberMatched = members.Count;
                var numberReturned = 0;

                double? north = null;
                double? south = null;
                double? west = null;
                double? east = null;

                var idMin = long.MaxValue;
                var idMax = long.MinValue;

                foreach (DataRow member in members) // members.AsParallel()      
                {
                    try
                    {
                        var type = (int)member[columnName: "type"]; //int
                        if (format is 0 or 4 or 5 && type is < 0 or > 3)
                            continue;

                        var id = (long)member[columnName: "id"];
                        if (id < idMin)
                            idMin = id;
                        if (id > idMax)
                            idMax = id;

                        var name = member[columnName: "name"]?.ToString(); //string null
                        var timestamp = (int[])member[columnName: "timestamp"];
                        var rank = (short)member[columnName: "rank"];
                        var frequency = (long)member[columnName: "frequency"];
                        var tree = (int)member[columnName: "tree"];

                        var inputLayer = (string[])member[columnName: "layer"];
                        var sordedLevels =
                            ((short[])member[columnName: "levels"])
                            .Select(selector: (x, i) => new KeyValuePair<int, int>(key: x, value: i))
                            .OrderBy(keySelector: x => x.Key);
                        var layer = new List<string>();
                        var inputLayerDetail = member.IsNull(columnName: "layerdetail") ? null : (string[])member[columnName: "layerdetail"]; //string[] xml null
                        var layerDetail = new List<string>();
                        var inputLayerProperty = member.IsNull(columnName: "layerproperty") ? null : (string[])member[columnName: "layerproperty"]; //string[] json null
                        var layerProperty = new List<string>();
                        foreach (var index in sordedLevels.Select(selector: level => level.Value))
                        {
                            layer.Add(item: inputLayer[index]);
                            if (inputLayerDetail != null)
                                layerDetail.Add(item: inputLayerDetail[index]);
                            if (inputLayerProperty != null)
                                layerProperty.Add(item: inputLayerProperty[index]);
                        }

                        var description = columns.Contains(name: "description")
                            ? member.IsNull(columnName: "description") ? null : member[columnName: "description"]
                            : null; //string[][] = object[][] null
                        var coordinate = columns.Contains(name: "coordinate")
                            ? member.IsNull(columnName: "coordinate") ? null : member[columnName: "coordinate"]?.ToString()
                            : null; //string wkt / geojson / kml
                        var boundary = columns.Contains(name: "boundary")
                            ? member.IsNull(columnName: "boundary") ? null : member[columnName: "boundary"]?.ToString()
                            : null; //string wkt / geojson / kml
                        var centroid = columns.Contains(name: "centroid")
                            ? member.IsNull(columnName: "centroid") ? null : member[columnName: "centroid"]?.ToString()
                            : null; //string wkt / geojson / kml
                        var style = columns.Contains(name: "style")
                            ? member.IsNull(columnName: "style") ? null : member[columnName: "style"].ToString()
                            : null; //string json null 

                        var rootX = featureCollectionX;
                        List<(string name, XElement property, XElement relation)> layerList = null;
                        if (format is 0 or 4)
                        {
                            layerList = new List<(string name, XElement property, XElement relation)>();
                            for (var i = 0; i < layer.Count; i++)
                            {
                                var layerProperties = layerProperty.Any() ? layerProperty[index: i] : null; //null json
                                var layerDetails = layerDetail.Any() ? layerDetail[index: i] : null; //null xml
                                layerList.Add(
                                    item: (
                                        layer[index: i],
                                        layerProperties != null
                                            ? XElement.Load(
                                                reader: new XmlNodeReader(
                                                    node: JsonConvert.DeserializeXmlNode(value: layerProperties, deserializeRootElementName: "property")
                                                )
                                            )
                                            : null,
                                        layerDetails != null
                                            ? new XElement(name: "relation", content: XElement.Parse(text: layerDetails))
                                            : null
                                    )
                                );
                            }
                            if (!layerList.Any())
                                continue;
                        }
                        else
                        {
                            for (var i = 0; i < layer.Count; i++)
                            {
                                var layerName = layer[index: i];
                                var layerDetails = layerDetail.Any() ? layerDetail[index: i] : null; //null xml
                                var layerProperties = layerProperty.Any() ? layerProperty[index: i] : null; //null json
                                var layerX =
                                    rootX
                                        .Elements(name: "layer")
                                        .FirstOrDefault(predicate: theLayer => theLayer.Element(name: "name")?.Value == layerName);
                                if (layerX == null)
                                {
                                    layerX = new XElement(
                                        name: "layer",
                                        new XElement(
                                            name: "name",
                                            content: layerName
                                        ),
                                        layerProperties != null
                                            ? XElement.Load(
                                                reader: new XmlNodeReader(
                                                    node: JsonConvert.DeserializeXmlNode(
                                                        value: layerProperties,
                                                        deserializeRootElementName: format is 3 ? "properties" : "property")
                                                )
                                            )
                                            : null,
                                        layerDetails != null
                                            ? new XElement(
                                                name: "relation",
                                                content: XElement.Parse(text: layerDetails)
                                            )
                                            : null
                                    );
                                    rootX.Add(content: layerX);
                                }
                                rootX = layerX;
                            }
                        }

                        var leafX = new XElement(
                            name: format switch
                            {
                                0 => wfsPrefix + "member",
                                4 => "feature",
                                _ => "member"
                            },
                            new XAttribute(
                                name: "type",
                                value: format is 0 or 4 or 5
                                    //gml geojson shape
                                    ? type switch
                                    {
                                        1 =>
                                            "Point",
                                        2 =>
                                            "Line",
                                        3 =>
                                            "Polygon",
                                        _ => ""
                                    }
                                    //kml geositeXml geositeJson
                                    : type switch
                                    {
                                        1 =>
                                            "Point",
                                        2 =>
                                            "Line",
                                        3 =>
                                            "Polygon",
                                        4 =>
                                            "Image",
                                        _ => "" //视为非空间类型
                                    }
                            ),
                            new XAttribute(name: "id", value: id),
                            new XAttribute(name: "frequency", value: frequency),
                            new XAttribute(name: "timeStamp", value: ToTimeStamp(yyyyMMddHHmmss: $"{timestamp[0]},{timestamp[1]}")),
                            new XAttribute(name: "rank", value: rank),
                            new XAttribute(name: "tree", value: tree)
                        );

                        XElement leafNodeX = null; //GML
                        if (format is 0 or 4)
                        {
                            var tailArray = layerList[^1];
                            switch (format)
                            {
                                case 0:
                                    {
                                        leafNodeX = new XElement(
                                            name: string.Join(separator: ".", values: layerList.Select(selector: theLayer => ToGmlNodeName(xmlNodeName: theLayer.name))),
                                            new XAttribute(name: gmlPrefix + "id", value: $"{tailArray.name}.{id}"),
                                            tailArray.property,
                                            tailArray.relation
                                        );
                                        break;
                                    }
                                case 4:
                                    {
                                        leafX.Add(
                                            new XElement(
                                                name: "layer",
                                                tailArray.name
                                                , tailArray.property
                                                , tailArray.relation
                                            ),
                                            new XElement(
                                                name: "route",
                                                content: string.Join(separator: ".", values: layerList.Select(selector: theLayer => theLayer.name))
                                            )
                                        );
                                        break;
                                    }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(value: name))
                        {
                            var nameX = new XElement(
                                name: "name",
                                content: name
                            );
                            if (format == 0)
                                leafNodeX.Add(content: nameX);
                            else
                                leafX.Add(content: nameX);
                        }

                        if (coordinate != null)
                        {
                            var gmlX = new XElement(
                                name: "geometry",
                                content: XElement.Parse(text: $"<Shell xmlns:gml=\"{gmlPrefix}\">{coordinate}</Shell>")
                                    .Elements()
                                    .FirstOrDefault()
                            );
                            switch (format)
                            {
                                case 0:
                                    //gml
                                    leafNodeX.Add(
                                        content: gmlX
                                    );
                                    break;
                                case 1:
                                    //kml
                                    leafX.Add(
                                        content: new XElement(
                                            name: "geometry",
                                            content: OGCformat.GmlToKml(geometryCode: type - 1, geometry: gmlX)
                                        )
                                    );
                                    break;
                                case 2 or 3 or 4 or 5:
                                    leafX.Add(
                                        content: new XElement(
                                            name: "geometry",
                                            content: OGCformat.GmlToGeoJson(geometryCode: type - 1, geometry: gmlX, simplify: format is 2 or 3)
                                        )
                                    );
                                    break;
                            }
                        }

                        if (description != null)
                        {
                            var propertyX = GeositeXmlFormatting.TableToXml(
                                table: description,
                                rootName: format is 3 or 4 ? "properties" : "property"
                            );

                            if (format == 0)
                            {
                                leafNodeX.Add
                                (
                                    content: new XElement
                                    (
                                        name: "property",
                                        content: GeositeXmlFormatting
                                            .XmlToTable(xml: propertyX)
                                            .Value.field?
                                            .Where(
                                                predicate: field => field.flag
                                            )
                                            .Select(
                                                selector: field =>
                                                {
                                                    var x = new XElement(
                                                        name: ToGmlNodeName(xmlNodeName: field.name),
                                                        field.attribute?.Select(selector: a => new XAttribute(name: a.Name, value: a.Value)),
                                                        field.content
                                                    );
                                                    if (field.flag)
                                                        x.SetAttributeValue(
                                                            name: "type",
                                                            value: field.type switch
                                                            {
                                                                0 => "string",
                                                                1 => "integer",
                                                                2 => "decimal",
                                                                3 => "hybrid",
                                                                4 => "boolean",
                                                                _ => string.Empty
                                                            }
                                                        );
                                                    x.SetAttributeValue(name: "level", value: field.level);
                                                    x.SetAttributeValue(name: "sequence", value: field.sequence);
                                                    x.SetAttributeValue(name: "parent", value: field.parent);
                                                    return x;
                                                }
                                            )
                                    )
                                );
                            }
                            else
                                leafX.Add(content: propertyX);
                        }

                        if (format != 0 && style != null)
                            leafX.Add(
                                content: XElement.Load(
                                    reader: new XmlNodeReader(
                                        node: JsonConvert.DeserializeXmlNode(
                                            value: style,
                                            deserializeRootElementName: "style"
                                        )
                                    )
                                )
                            );

                        if (boundary != null)
                        {
                            double theWest, theEast;
                            var theNorth = theEast = double.MinValue;
                            var theSouth = theWest = double.MaxValue;
                            var posListX = new XElement(
                                    name: "geometry",
                                    content: XElement
                                        .Parse(text: $"<Shell xmlns:gml=\"{gmlPrefix}\">{boundary}</Shell>")
                                        .Elements()
                                        .FirstOrDefault()
                                )
                                .Descendants(name: gmlPrefix + "posList")
                                .FirstOrDefault();
                            var srsDimension = int.Parse(s: posListX?.Attribute(name: "srsDimension").Value ?? "2");
                            var posListArray = Regex.Split(input: posListX.Value, pattern: @"[\s]+");
                            for (var i = 0; i < posListArray.Length; i += srsDimension)
                            {
                                var x = double.Parse(s: posListArray[i]);
                                var y = double.Parse(s: posListArray[i + 1]);
                                if (i == 0)
                                {
                                    theWest = theEast = x;
                                    theNorth = theSouth = y;
                                }
                                else
                                {
                                    if (theWest > x)
                                        theWest = x;
                                    if (theEast < x)
                                        theEast = x;
                                    if (theNorth < y)
                                        theNorth = y;
                                    if (theSouth > y)
                                        theSouth = y;
                                }
                            }
                            if (west == null || west > theWest)
                                west = theWest;
                            if (east == null || east < theEast)
                                east = theEast;
                            if (north == null || north < theNorth)
                                north = theNorth;
                            if (south == null || south > theSouth)
                                south = theSouth;

                            string[] centroidArray = null;
                            XElement posX = null;
                            if (!string.IsNullOrWhiteSpace(value: centroid))
                            {
                                posX = new XElement(
                                    name: "geometry",
                                    content: XElement
                                        .Parse(text: $"<Shell xmlns:gml=\"{gmlPrefix}\">{centroid}</Shell>")
                                        .Elements()
                                        .FirstOrDefault()
                                ).Descendants(name: gmlPrefix + "pos").FirstOrDefault();
                                centroidArray = Regex.Split(input: posX.Value, pattern: @"[\s]+");
                            }

                            var leafBoundaryX = new XElement(
                                name: format switch
                                {
                                    0 => gmlPrefix + "boundedBy",
                                    3 or 4 => "bbox",
                                    _ => "boundary"
                                },
                                new XAttribute(name: "srsName", value: srsName),
                                centroidArray == null
                                    ? null
                                    : new XAttribute(
                                        name: "centroid",
                                        value: format switch
                                        {
                                            0 => posX.Value,
                                            3 or 4 => $"[{string.Join(separator: ",", values: centroidArray.Reverse())}]",
                                            _ => string.Join(separator: ",", value: centroidArray)
                                        }
                                    )
                            );
                            switch (format)
                            {
                                case 0:
                                    {
                                        leafBoundaryX.Add(
                                            content: new XElement(
                                                name: gmlPrefix + "Envelope",
                                                new XAttribute(name: "srsDimension", value: "2"),
                                                new XAttribute(name: "srsName", value: srsName),
                                                new XElement(
                                                    name: gmlPrefix + "lowerCorner",
                                                    new XAttribute(name: "remarks", value: "WestSouth"),
                                                    $"{theWest} {theSouth}"
                                                ),
                                                new XElement(
                                                    name: gmlPrefix + "upperCorner",
                                                    new XAttribute(name: "remarks", value: "EastNorth"),
                                                    $"{theEast} {theNorth}"
                                                )
                                            )
                                        );
                                        break;
                                    }
                                case 3 or 4:
                                    {
                                        leafBoundaryX.Value =
                                            $"[{theSouth},{theWest},{theNorth},{theEast}]";
                                        break;
                                    }
                                default:
                                    {
                                        leafBoundaryX.Add(
                                            new XElement(
                                                name: "north",
                                                content: theNorth
                                            ),
                                            new XElement(
                                                name: "south",
                                                content: theSouth
                                            ),
                                            new XElement(
                                                name: "west",
                                                content: theWest
                                            ),
                                            new XElement(
                                                name: "east",
                                                content: theEast
                                            )
                                        );
                                        break;
                                    }
                            }

                            if (format == 0)
                                leafNodeX.Add(content: leafBoundaryX);
                            else
                                leafX.Add(content: leafBoundaryX);
                        }

                        if (format == 0)
                            leafX.Add(content: leafNodeX);
                        rootX.Add(content: leafX);
                        numberReturned++;
                    }
                    catch
                    {
                        //
                    }
                }

                if (
                    north.HasValue &&
                    south.HasValue &&
                    west.HasValue &&
                    east.HasValue
                )
                {
                    switch (format)
                    {
                        case 0:
                            {
                                featureCollectionX
                                    .Element(name: wfsPrefix + "boundedBy")
                                    .Element(name: gmlPrefix + "Envelope")
                                    .Add(
                                        new XElement(
                                            name: gmlPrefix + "lowerCorner",
                                            new XAttribute(name: "remarks", value: "WestSouth"),
                                            $"{west} {south}"
                                        ),
                                        new XElement(
                                            name: gmlPrefix + "upperCorner",
                                            new XAttribute(name: "remarks", value: "EastNorth"),
                                            $"{east} {north}"
                                        )
                                    );
                                break;
                            }
                        case 3 or 4:
                            {
                                //southwesterly northeasterly
                                featureCollectionX.Element(name: "bbox").Value = $"[{south},{west},{north},{east}]";
                                break;
                            }
                        default:
                            {
                                featureCollectionX.Element(name: "boundary").Add(
                                    new XElement(name: "north", content: north),
                                    new XElement(name: "south", content: south),
                                    new XElement(name: "west", content: west),
                                    new XElement(name: "east", content: east)
                                );
                                break;
                            }
                    }
                }

                return (featureCollectionX, $"{idMin}", $"{idMax}", numberMatched, numberReturned);
            }

            if (e.Argument == null)
                return;

            _backgroundWorker.ReportProgress(percentProgress: -1, userState: @"Acquiring vector features ...");

            var sqlCmd =
                " SELECT * FROM (" +
                "     SELECT * FROM (" +
                "         SELECT * FROM (" +
                "             SELECT * FROM (" +
                $"                SELECT * FROM leaf WHERE branch = {_layerId} AND type BETWEEN 1 AND 4" +
                "             ) AS leaves LEFT JOIN LATERAL (SELECT * FROM ogc_branch(branch)) AS linktable ON TRUE" +
                "         ) AS leaves LEFT JOIN LATERAL (" +
                "             SELECT ARRAY_AGG((name,attribute,level,sequence,parent,flag,type,content)) AS description FROM leaf_description WHERE leaf = id GROUP BY leaf" +
                "         ) AS linktable ON TRUE" +
                "     ) AS leaves LEFT JOIN LATERAL (" +
                "         SELECT style FROM leaf_style WHERE leaf = id" +
                "     ) AS linktable ON TRUE" +
                " ) AS leaves LEFT JOIN LATERAL (" +
                "     SELECT ST_AsGML(3,coordinate,8) AS coordinate, ST_AsGML(3,boundary,8) AS boundary, ST_AsGML(3,centroid,8) AS centroid FROM leaf_geometry WHERE leaf = id " +
                " ) AS linktable ON TRUE";

            var leavesTable = PostgreSqlHelper.DataTableReader(
                cmd: sqlCmd,
                timeout: 0
            );
            if (leavesTable == null || leavesTable.Rows.Count == 0)
            {
                _backgroundWorker.ReportProgress(percentProgress: -1, userState: @"No vector features found.");
                return;
            }

            var getResultXml = GeositeXmlBuilder(leaves: leavesTable);
            var xml = getResultXml?.xml;
            if (xml == null)
            {
                _backgroundWorker.ReportProgress(percentProgress: -1, userState: @"No vector features found.");
                return;
            }
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
            geositeXml.OnMessagerEvent += delegate (object _, MessagerEventArgs thisEvent)
            {
                var progress = thisEvent.Progress;
                if (progress == null)
                {
                    var progressPercentage = thisEvent.Progress;
                    _backgroundWorker.ReportProgress(percentProgress: progressPercentage is >= 0 ? progressPercentage.Value : -1, userState: thisEvent.Message ?? string.Empty);
                }
            };
            try
            {
                var exportInfo = ((int fileType, string filePath))e.Argument;
                switch (exportInfo.fileType)
                {
                    case 1: //ShapeFile
                        {
                            geositeXml.GeositeXmlToShp(xml, exportInfo.filePath);
                            break;
                        }
                    case 2: //GeoJSON
                        {
                            var result = geositeXml.GeositeXmlToGeoJson(xml.Descendants());
                            if (result != null)
                            {
                                using var writer = new StreamWriter(exportInfo.filePath, false, Encoding.UTF8);
                                writer.WriteLine(result);
                            }
                            break;
                        }
                    case 3: //KML
                        {
                            geositeXml.GeositeXmlToKml(xml.Descendants()).Save(exportInfo.filePath);
                            break;
                        }
                    case 4: //GML
                        {
                            geositeXml.GeositeXmlToGml(xml.Descendants()).Save(exportInfo.filePath);
                            break;
                        }
                    case 5: //GeositeXml
                        {
                            geositeXml.GeositeXmlToGeositeXml(xml.Descendants()).Save(exportInfo.filePath);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _backgroundWorker.ReportProgress(percentProgress: -1, userState: ex.Message);
            }
        }
    }
}
