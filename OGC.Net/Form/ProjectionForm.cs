using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Geosite
{
    /// <summary>
    /// 几何投影窗体类
    /// </summary>
    public partial class ProjectionForm : Form
    {
        /// <summary>
        /// 日志提示信息
        /// </summary>
        private string _tip;

        /// <summary>
        /// 投影参数初始化
        /// </summary>
        private XElement _projection;

        /// <summary>
        /// 投影参数配置结果
        /// </summary>
        public XElement Projection;

        /// <summary>
        /// 是否勾选了统改投影设置？
        /// </summary>
        public bool GetProjectionSame()
        {
            return ProjectionSame.Checked;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projection">预置投影参数，默认：null。如果为null或未设置任何有效卡片，将全部开启所有卡片并激活第一张（地理坐标系）卡片</param>
        /// <param name="tip">附加的日志提示信息，默认：null</param>
        public ProjectionForm(XElement projection, string tip = null)
        {
            InitializeComponent();
            InitializationProjection(projection: projection, tip: tip);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="boundary">预置源文件几何边界四元组参数（double north, double south, double west, double east）</param>
        /// <param name="targetProjection">预置目标投影参数，默认：null。如果为null或未设置任何有效卡片，将全部开启所有卡片并激活第一张（地理坐标系）卡片</param>
        /// <param name="tip">附加的日志提示信息，默认：null</param>
        public ProjectionForm((double north, double south, double west, double east) boundary, XElement targetProjection, string tip = null)
        {
            InitializeComponent();
            //依据边框信息推测投影参数
            XElement inferProjection;
            if (boundary is { north: <= 90, south: >= -90, west: >= -180, east: <= 180 })
            {
                inferProjection =
                    new XElement(
                        name: "Projection",
                        content: new object[]
                        {
                            new XElement(
                                name: "From",
                                content: new object[]
                                {
                                    new XAttribute(
                                        name: "Active",
                                        value: 0 //推测数据源最大可能属于【Geography】并可排除【Gauss-Kruger】 
                                    ),
                                    new XElement(name:
                                        "Geography"
                                    ),
                                    new XElement(name: "Lambert",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "OriginLatitude",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "Parallel1",
                                                content: 25
                                            ),
                                            new XElement(
                                                name: "Parallel2",
                                                content: 47
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }),
                                    new XElement(name: "Albers",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "OriginLatitude",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "Parallel1",
                                                content: 25
                                            ),
                                            new XElement(
                                                name: "Parallel2",
                                                content: 47
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }),
                                    new XElement(name: "Web-Mercator")
                                }
                            ),
                            targetProjection?.Element(name: "To")
                        }
                    );
            }
            else
            {
                var north = boundary.north;
                var south = boundary.south;
                var west = boundary.west;
                var east = boundary.east;
                var zoneWest = Math.Floor(d: west / 1000000);
                var zoneEast = Math.Floor(d: east / 1000000);
                //横向坐标通用值是否符合【带号+500000+自然值】格式？是否属于同一个投影带？
                if (west >= 1000000 && east >= 1000000 && north < 10005000 && south > -10005000 && zoneWest is >= 1 and <= 120 && zoneEast is >= 1 and <= 120 && Math.Abs(value: zoneWest - zoneEast) < 0.1)
                {
                    inferProjection = new XElement(
                        name: "Projection",
                        content: new object[]
                        {
                            new XElement(
                                name: "From",
                                content: new object[]
                                {
                                    new XAttribute(
                                        name: "Active",
                                        value: 1 //推测数据源最大可能属于【Gauss-Kruger】并可排除【Geography】 
                                    ),
                                    new XElement(
                                        name: "Gauss-Kruger",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: zoneWest > 60
                                                    ? GeositeServer.Vector.Ellipsoid.CentralMeridian3ByZone(int.Parse($"{zoneWest}"))
                                                    : GeositeServer.Vector.Ellipsoid.CentralMeridian6ByZone(int.Parse($"{zoneWest}"))
                                            ),
                                            new XElement(
                                                name: "Zone",
                                                content: zoneWest > 60 ? 3 : 6
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }
                                    ),
                                    new XElement(name: "Lambert",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: zoneWest * 6 - 3
                                            ),
                                            new XElement(
                                                name: "OriginLatitude",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "Parallel1",
                                                content: 25
                                            ),
                                            new XElement(
                                                name: "Parallel2",
                                                content: 47
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }),
                                    new XElement(name: "Albers",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: zoneWest * 6 - 3
                                            ),
                                            new XElement(
                                                name: "OriginLatitude",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "Parallel1",
                                                content: 25
                                            ),
                                            new XElement(
                                                name: "Parallel2",
                                                content: 47
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }),
                                    new XElement(name: "Web-Mercator")
                                }
                            ),
                            targetProjection?.Element(name: "To")
                        }
                    );
                }
                else
                {
                    inferProjection = new XElement(
                        name: "Projection",
                        content: new object[]
                        {
                            new XElement(
                                name: "From",
                                content: new object[]
                                {
                                    new XAttribute(
                                        name: "Active",
                                        value: 2 //推测数据源最大可能属于【Lambert】并可排除【Geography、Gauss-Kruger】 
                                    ),
                                    new XElement(name: "Lambert",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "OriginLatitude",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "Parallel1",
                                                content: 25
                                            ),
                                            new XElement(
                                                name: "Parallel2",
                                                content: 47
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }),
                                    new XElement(name: "Albers",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "OriginLatitude",
                                                content: 0
                                            ),
                                            new XElement(
                                                name: "Parallel1",
                                                content: 25
                                            ),
                                            new XElement(
                                                name: "Parallel2",
                                                content: 47
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: 2000
                                            )
                                        }),
                                    new XElement(name: "Web-Mercator")
                                }
                            ),
                            targetProjection?.Element(name: "To")
                        }
                    );
                }
            }
            InitializationProjection(projection: inferProjection, tip: tip);
        }

        /// <summary>
        /// 设置初始化投影参数
        /// </summary>
        /// <param name="projection">投影参数</param>
        /// <param name="tip">附加的提示信息，默认：null</param>
        private void InitializationProjection(XElement projection, string tip = null)
        {
            // projection 如果设为null或未设置任何有效卡片，将全部开启所有卡片并激活第一张卡片
            // 预置投影参数xml包括两个节，分别是【From】和【To】，样例如下（提示：如果某标签未出现，便禁止选择此卡片）：
            // new XElement(
            //    "Projection",
            //    new XElement(
            //        "From",
            //        //new XElement("Geography"),
            //        new XElement(
            //            "Gauss-Kruger",
            //            new XElement(
            //                "CentralMeridian",
            //                99
            //            ),
            //            new XElement(
            //                "Zone",
            //                3
            //            ),
            //            new XElement(
            //                "Srid",
            //                1954
            //            )
            //        ),
            //        new XElement("Lambert",
            //            new XElement(
            //                "CentralMeridian",
            //                105
            //            ),
            //            new XElement(
            //                "OriginLatitude",
            //                0
            //            ),
            //            new XElement(
            //                "Parallel1",
            //                25
            //            ),
            //            new XElement(
            //                "Parallel2",
            //                47
            //            ),
            //            new XElement(
            //                "Srid",
            //                1980
            //            )),
            //        new XElement("Albers",
            //            new XElement(
            //                "CentralMeridian",
            //                105
            //            ),
            //            new XElement(
            //                "OriginLatitude",
            //                0
            //            ),
            //            new XElement(
            //                "Parallel1",
            //                25
            //            ),
            //            new XElement(
            //                "Parallel2",
            //                47
            //            ),
            //            new XElement(
            //                "Srid",
            //                1980
            //            )),
            //        new XElement("Web-Mercator")
            //    ),
            //    new XElement(
            //        "To",
            //        new XElement("Geography"),
            //        new XElement("Web-Mercator")
            //    )
            // );
            _tip = tip;
            _projection = projection;
        }

        /// <summary>
        /// 追加提示信息
        /// </summary>
        /// <param name="tip">提示信息</param>
        private void AppendProjectionTip(string tip)
        {
            if (string.IsNullOrWhiteSpace(value: tip))
                return;
            ProjectionTipBox.AppendText(text: Environment.NewLine);
            ProjectionTipBox.AppendText(text: tip);
            ProjectionTipBox.ScrollToCaret();
        }

        /// <summary>
        /// 窗体加载成功后的回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectionForm_Load(object sender, EventArgs e)
        {
            AppendProjectionTip(tip: _tip);
            var selectedIndexFrom = -1;
            var selectedIndexTo = -1;
            if (_projection != null)
            {
                // 原始投影
                var projectionFrom = _projection.Element(name: "From");
                if (projectionFrom != null)
                {
                    var webMercatorX = projectionFrom.Element(name: "Web-Mercator");
                    if (webMercatorX != null)
                        selectedIndexFrom = 4;
                    else
                        WebMercatorPageFrom.Tag = false;
                    var albersX = projectionFrom.Element(name: "Albers");
                    if (albersX != null)
                    {
                        selectedIndexFrom = 3;
                        var centralMeridian = albersX.Element(name: "CentralMeridian")?.Value;
                        if (centralMeridian != null)
                            AlbersPageCentralMeridianFrom.Text = centralMeridian;
                        var originLatitude = albersX.Element(name: "OriginLatitude")?.Value;
                        if (originLatitude != null)
                            AlbersPageOriginLatitudeFrom.Text = originLatitude;
                        var parallel1 = albersX.Element(name: "Parallel1")?.Value;
                        if (parallel1 != null)
                            AlbersPageParallel1From.Text = parallel1;
                        var parallel2 = albersX.Element(name: "Parallel2")?.Value;
                        if (parallel2 != null)
                            AlbersPageParallel2From.Text = parallel2;
                        var srid = albersX.Element(name: "Srid")?.Value;
                        if (srid != null)
                            switch (srid)
                            {
                                case "1954":
                                    {
                                        AlbersPage1954From.Checked = true;
                                        break;
                                    }
                                case "1980":
                                    {
                                        AlbersPage1980From.Checked = true;
                                        break;
                                    }
                                case "1984":
                                    {
                                        AlbersPage1984From.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        AlbersPage2000From.Checked = true;
                                        break;
                                    }
                            }
                        else
                            AlbersPage2000From.Checked = true;
                    }
                    else
                        AlbersPageFrom.Tag = false;
                    var lambertX = projectionFrom.Element(name: "Lambert");
                    if (lambertX != null)
                    {
                        selectedIndexFrom = 2;
                        var centralMeridian = lambertX.Element(name: "CentralMeridian")?.Value;
                        if (centralMeridian != null)
                            LambertPageCentralMeridianFrom.Text = centralMeridian;
                        var originLatitude = lambertX.Element(name: "OriginLatitude")?.Value;
                        if (originLatitude != null)
                            LambertPageOriginLatitudeFrom.Text = originLatitude;
                        var parallel1 = lambertX.Element(name: "Parallel1")?.Value;
                        if (parallel1 != null)
                            LambertPageParallel1From.Text = parallel1;
                        var parallel2 = lambertX.Element(name: "Parallel2")?.Value;
                        if (parallel2 != null)
                            LambertPageParallel2From.Text = parallel2;
                        var srid = lambertX.Element(name: "Srid")?.Value;
                        if (srid != null)
                            switch (srid)
                            {
                                case "1954":
                                    {
                                        LambertPage1954From.Checked = true;
                                        break;
                                    }
                                case "1980":
                                    {
                                        LambertPage1980From.Checked = true;
                                        break;
                                    }
                                case "1984":
                                    {
                                        LambertPage1984From.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        LambertPage2000From.Checked = true;
                                        break;
                                    }
                            }
                        else
                            LambertPage2000From.Checked = true;
                    }
                    else
                        LambertPageFrom.Tag = false;
                    var gaussKrugerX = projectionFrom.Element(name: "Gauss-Kruger");
                    if (gaussKrugerX != null)
                    {
                        selectedIndexFrom = 1;
                        var centralMeridian = gaussKrugerX.Element(name: "CentralMeridian")?.Value;
                        if (centralMeridian != null)
                            GaussKrugerPageCentralMeridianFrom.Text = centralMeridian;
                        var zone = gaussKrugerX.Element(name: "Zone")?.Value.FirstOrDefault();
                        if (zone != null)
                            switch (zone.Value)
                            {
                                case '6':
                                    {
                                        GaussKrugerPage6DegreeFrom.Checked = true;
                                        break;
                                    }
                                case '3':
                                    {
                                        GaussKrugerPage3DegreeFrom.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        GaussKrugerPageUnknownFrom.Checked = true;
                                        break;
                                    }
                            }
                        else
                            GaussKrugerPage6DegreeFrom.Checked = true;
                        var srid = gaussKrugerX.Element(name: "Srid")?.Value;
                        if (srid != null)
                            switch (srid)
                            {
                                case "1954":
                                    {
                                        GaussKrugerPage1954From.Checked = true;
                                        break;
                                    }
                                case "1980":
                                    {
                                        GaussKrugerPage1980From.Checked = true;
                                        break;
                                    }
                                case "1984":
                                    {
                                        GaussKrugerPage1984From.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        GaussKrugerPage2000From.Checked = true;
                                        break;
                                    }
                            }
                        else
                            GaussKrugerPage2000From.Checked = true;
                    }
                    else
                        GaussKrugerPageFrom.Tag = false;
                    var geographyX = projectionFrom.Element(name: "Geography");
                    if (geographyX != null)
                        selectedIndexFrom = 0;
                    else
                        GeographyPageFrom.Tag = false;
                    var active = projectionFrom.Attribute(name: "Active")?.Value;
                    if (active != null)
                    {
                        if (int.TryParse(s: active, result: out var index))
                        {
                            if (index is >= 0 and <= 4)
                            {
                                selectedIndexFrom = index;
                            }
                        }
                    }
                }
                // 目的投影
                var projectionTo = _projection.Element(name: "To");
                if (projectionTo != null)
                {
                    var webMercatorX = projectionTo.Element(name: "Web-Mercator");
                    if (webMercatorX != null)
                        selectedIndexTo = 4;
                    else
                        WebMercatorPageTo.Tag = false;
                    var albersX = projectionTo.Element(name: "Albers");
                    if (albersX != null)
                    {
                        selectedIndexTo = 3;
                        var centralMeridian = albersX.Element(name: "CentralMeridian")?.Value;
                        if (centralMeridian != null)
                            AlbersPageCentralMeridianTo.Text = centralMeridian;
                        var originLatitude = albersX.Element(name: "OriginLatitude")?.Value;
                        if (originLatitude != null)
                            AlbersPageOriginLatitudeTo.Text = originLatitude;
                        var parallel1 = albersX.Element(name: "Parallel1")?.Value;
                        if (parallel1 != null)
                            AlbersPageParallel1To.Text = parallel1;
                        var parallel2 = albersX.Element(name: "Parallel2")?.Value;
                        if (parallel2 != null)
                            AlbersPageParallel2To.Text = parallel2;
                        var srid = albersX.Element(name: "Srid")?.Value;
                        if (srid != null)
                            switch (srid)
                            {
                                case "1954":
                                    {
                                        AlbersPage1954To.Checked = true;
                                        break;
                                    }
                                case "1980":
                                    {
                                        AlbersPage1980To.Checked = true;
                                        break;
                                    }
                                case "1984":
                                    {
                                        AlbersPage1984To.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        AlbersPage2000To.Checked = true;
                                        break;
                                    }
                            }
                        else
                            AlbersPage2000To.Checked = true;
                    }
                    else
                        AlbersPageTo.Tag = false;
                    var lambertX = projectionTo.Element(name: "Lambert");
                    if (lambertX != null)
                    {
                        selectedIndexTo = 2;
                        var centralMeridian = lambertX.Element(name: "CentralMeridian")?.Value;
                        if (centralMeridian != null)
                            LambertPageCentralMeridianTo.Text = centralMeridian;
                        var originLatitude = lambertX.Element(name: "OriginLatitude")?.Value;
                        if (originLatitude != null)
                            LambertPageOriginLatitudeTo.Text = originLatitude;
                        var parallel1 = lambertX.Element(name: "Parallel1")?.Value;
                        if (parallel1 != null)
                            LambertPageParallel1To.Text = parallel1;
                        var parallel2 = lambertX.Element(name: "Parallel2")?.Value;
                        if (parallel2 != null)
                            LambertPageParallel2To.Text = parallel2;
                        var srid = lambertX.Element(name: "Srid")?.Value;
                        if (srid != null)
                            switch (srid)
                            {
                                case "1954":
                                    {
                                        LambertPage1954To.Checked = true;
                                        break;
                                    }
                                case "1980":
                                    {
                                        LambertPage1980To.Checked = true;
                                        break;
                                    }
                                case "1984":
                                    {
                                        LambertPage1984To.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        LambertPage2000To.Checked = true;
                                        break;
                                    }
                            }
                        else
                            LambertPage2000To.Checked = true;
                    }
                    else
                        LambertPageTo.Tag = false;
                    var gaussKrugerX = projectionTo.Element(name: "Gauss-Kruger");
                    if (gaussKrugerX != null)
                    {
                        selectedIndexTo = 1;
                        var centralMeridian = gaussKrugerX.Element(name: "CentralMeridian")?.Value;
                        if (centralMeridian != null)
                            GaussKrugerPageCentralMeridianTo.Text = centralMeridian;
                        var zone = gaussKrugerX.Element(name: "Zone")?.Value.FirstOrDefault();
                        if (zone != null)
                            switch (zone.Value)
                            {
                                case '6':
                                    {
                                        GaussKrugerPage6DegreeTo.Checked = true;
                                        break;
                                    }
                                case '3':
                                    {
                                        GaussKrugerPage3DegreeTo.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        GaussKrugerPageUnknownTo.Checked = true;
                                        break;
                                    }
                            }
                        else
                            GaussKrugerPage6DegreeTo.Checked = true;
                        var srid = gaussKrugerX.Element(name: "Srid")?.Value;
                        if (srid != null)
                            switch (srid)
                            {
                                case "1954":
                                    {
                                        GaussKrugerPage1954To.Checked = true;
                                        break;
                                    }
                                case "1980":
                                    {
                                        GaussKrugerPage1980To.Checked = true;
                                        break;
                                    }
                                case "1984":
                                    {
                                        GaussKrugerPage1984To.Checked = true;
                                        break;
                                    }
                                default:
                                    {
                                        GaussKrugerPage2000To.Checked = true;
                                        break;
                                    }
                            }
                        else
                            GaussKrugerPage2000To.Checked = true;
                    }
                    else
                        GaussKrugerPageTo.Tag = false;
                    var geographyX = projectionTo.Element(name: "Geography");
                    if (geographyX != null)
                        selectedIndexTo = 0;
                    else
                        GeographyPageTo.Tag = false;
                    var active = projectionTo.Attribute(name: "Active")?.Value;
                    if (active != null)
                    {
                        if (int.TryParse(s: active, result: out var index))
                        {
                            if (index is >= 0 and <= 4)
                            {
                                selectedIndexTo = index;
                            }
                        }
                    }
                }
            }
            ProjectionTabControlFrom.SelectedIndex = selectedIndexFrom == -1 ? 0 : selectedIndexFrom;
            ProjectionTabControlTo.SelectedIndex = selectedIndexTo == -1 ? 0 : selectedIndexTo;
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            Projection = new XElement(
                name: "Projection",
                content: new object[]
                {
                    new XElement(
                        name: "From"
                    ),
                    new XElement(
                        name: "To"
                    )
                }
            );
            var fromX = Projection.Element(name: "From");
            var toX = Projection.Element(name: "To");
            string errorMessage = null;
            // 原始投影
            switch (ProjectionTabControlFrom.SelectedIndex)
            {
                case 0: //Geography
                    {
                        fromX!.Add(content: new XElement(name: "Geography"));
                        break;
                    }
                case 1: //Gauss-Kruger
                    {
                        if (double.TryParse(s: GaussKrugerPageCentralMeridianFrom.Text, result: out var gaussKrugerPageCentralMeridian))
                        {
                            if (gaussKrugerPageCentralMeridian is >= -180 and <= 180)
                                fromX!.Add(
                                    content: new XElement(
                                        name: "Gauss-Kruger",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: gaussKrugerPageCentralMeridian
                                            ),
                                            new XElement(
                                                name: "Zone",
                                                content: GaussKrugerPage6DegreeFrom.Checked ? 6 :
                                                GaussKrugerPage3DegreeFrom.Checked ? 3 : -1
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: GaussKrugerPage1954From.Checked ? 1954 :
                                                GaussKrugerPage1980From.Checked ? 1980 :
                                                GaussKrugerPage1984From.Checked ? 1984 : 2000
                                            )
                                        }
                                    )
                                );
                            else
                                errorMessage = "From: CentralMeridian should be >= -180 and <= 180.";
                        }
                        else
                            errorMessage = "From: CentralMeridian should be of double type.";
                        break;
                    }
                case 2: //Lambert
                    {
                        if (double.TryParse(s: LambertPageCentralMeridianFrom.Text, result: out var lambertPageCentralMeridian))
                        {
                            if (lambertPageCentralMeridian is >= -180 and <= 180)
                            {
                                if (double.TryParse(s: LambertPageOriginLatitudeFrom.Text, result: out var lambertPageOriginLatitude))
                                {
                                    if (lambertPageOriginLatitude is >= -90 and <= 90)
                                    {
                                        if (double.TryParse(s: LambertPageParallel1From.Text, result: out var lambertPageParallel1))
                                        {
                                            if (lambertPageParallel1 is >= -90 and <= 90)
                                            {
                                                if (double.TryParse(s: LambertPageParallel2From.Text, result: out var lambertPageParallel2))
                                                {
                                                    if (lambertPageParallel2 is >= -90 and <= 90)
                                                    {
                                                        if (lambertPageParallel2 > lambertPageParallel1)
                                                            fromX!.Add(
                                                                content: new XElement(name: "Lambert",
                                                                    content: new object[]
                                                                    {
                                                                        new XElement(
                                                                            name: "CentralMeridian",
                                                                            content: lambertPageCentralMeridian
                                                                        ),
                                                                        new XElement(
                                                                            name: "OriginLatitude",
                                                                            content: lambertPageOriginLatitude
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel1",
                                                                            content: lambertPageParallel1
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel2",
                                                                            content: lambertPageParallel2
                                                                        ),
                                                                        new XElement(
                                                                            name: "Srid",
                                                                            content: LambertPage1954From.Checked
                                                                                ? 1954
                                                                                : LambertPage1980From.Checked
                                                                                    ? 1980
                                                                                    : LambertPage1984From.Checked
                                                                                        ? 1984
                                                                                        : 2000
                                                                        )
                                                                    }
                                                                )
                                                            );
                                                        else
                                                            errorMessage = "From: Parallel2 should be > Parallel1.";
                                                    }
                                                    else
                                                        errorMessage = "From: Parallel2 should be >= -90 and <= 90.";
                                                }
                                                else
                                                    errorMessage = "From: Parallel2 should be of double type.";
                                            }
                                            else
                                                errorMessage = "From: Parallel1 should be >= -90 and <= 90.";
                                        }
                                        else
                                            errorMessage = "From: Parallel1 should be of double type.";
                                    }
                                    else
                                        errorMessage = "From: OriginLatitude should be >= -90 and <= 90.";
                                }
                                else
                                    errorMessage = "From: OriginLatitude should be of double type.";
                            }
                            else
                                errorMessage = "From: CentralMeridian should be >= -180 and <= 180.";
                        }
                        else
                            errorMessage = "From: CentralMeridian should be of double type.";
                        break;
                    }
                case 3: //Albers
                    {
                        if (double.TryParse(s: AlbersPageCentralMeridianFrom.Text, result: out var albersPageCentralMeridian))
                        {
                            if (albersPageCentralMeridian is >= -180 and <= 180)
                            {
                                if (double.TryParse(s: AlbersPageOriginLatitudeFrom.Text, result: out var albersPageOriginLatitude))
                                {
                                    if (albersPageOriginLatitude is >= -90 and <= 90)
                                    {
                                        if (double.TryParse(s: AlbersPageParallel1From.Text, result: out var albersPageParallel1))
                                        {
                                            if (albersPageParallel1 is >= -90 and <= 90)
                                            {
                                                if (double.TryParse(s: AlbersPageParallel2From.Text, result: out var albersPageParallel2))
                                                {
                                                    if (albersPageParallel2 is >= -90 and <= 90)
                                                    {
                                                        if (albersPageParallel2 > albersPageParallel1)
                                                            fromX!.Add(
                                                                content: new XElement(name: "Albers",
                                                                    content: new object[]
                                                                    {
                                                                        new XElement(
                                                                            name: "CentralMeridian",
                                                                            content: albersPageCentralMeridian
                                                                        ),
                                                                        new XElement(
                                                                            name: "OriginLatitude",
                                                                            content: albersPageOriginLatitude
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel1",
                                                                            content: albersPageParallel1
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel2",
                                                                            content: albersPageParallel2
                                                                        ),
                                                                        new XElement(
                                                                            name: "Srid",
                                                                            content: AlbersPage1954From.Checked ? 1954 :
                                                                            AlbersPage1980From.Checked ? 1980 :
                                                                            AlbersPage1984From.Checked ? 1984 : 2000
                                                                        )
                                                                    })
                                                            );
                                                        else
                                                            errorMessage = "From: Parallel2 should be > Parallel1.";
                                                    }
                                                    else
                                                        errorMessage = "From: Parallel2 should be >= -90 and <= 90.";
                                                }
                                                else
                                                    errorMessage = "From: Parallel2 should be of double type.";
                                            }
                                            else
                                                errorMessage = "From: Parallel1 should be >= -90 and <= 90.";
                                        }
                                        else
                                            errorMessage = "From: Parallel1 should be of double type.";
                                    }
                                    else
                                        errorMessage = "From: OriginLatitude should be >= -90 and <= 90.";
                                }
                                else
                                    errorMessage = "From: OriginLatitude should be of double type.";
                            }
                            else
                                errorMessage = "From: CentralMeridian should be >= -180 and <= 180.";
                        }
                        else
                            errorMessage = "From: CentralMeridian should be of double type.";
                        break;
                    }
                case 4: //Web-Mercator
                    {
                        fromX!.Add(content: new XElement(name: "Web-Mercator"));
                        break;
                    }
            }
            // 目的投影
            switch (ProjectionTabControlTo.SelectedIndex)
            {
                case 0: //Geography
                    {
                        toX!.Add(content: new XElement(name: "Geography"));
                        break;
                    }
                case 1: //Gauss-Kruger
                    {
                        if (double.TryParse(s: GaussKrugerPageCentralMeridianTo.Text, result: out var gaussKrugerPageCentralMeridian))
                        {
                            if (gaussKrugerPageCentralMeridian is >= -180 and <= 180)
                                toX!.Add(
                                    content: new XElement(
                                        name: "Gauss-Kruger",
                                        content: new object[]
                                        {
                                            new XElement(
                                                name: "CentralMeridian",
                                                content: gaussKrugerPageCentralMeridian
                                            ),
                                            new XElement(
                                                name: "Zone",
                                                content: GaussKrugerPage6DegreeTo.Checked ? 6 :
                                                GaussKrugerPage3DegreeTo.Checked ? 3 : -1
                                            ),
                                            new XElement(
                                                name: "Srid",
                                                content: GaussKrugerPage1954To.Checked ? 1954 :
                                                GaussKrugerPage1980To.Checked ? 1980 :
                                                GaussKrugerPage1984To.Checked ? 1984 : 2000
                                            )
                                        }
                                    )
                                );
                            else
                                errorMessage = "To: CentralMeridian should be >= -180 and <= 180.";
                        }
                        else
                            errorMessage = "To: CentralMeridian should be of double type.";
                        break;
                    }
                case 2: //Lambert
                    {
                        if (double.TryParse(s: LambertPageCentralMeridianTo.Text, result: out var lambertPageCentralMeridian))
                        {
                            if (lambertPageCentralMeridian is >= -180 and <= 180)
                            {
                                if (double.TryParse(s: LambertPageOriginLatitudeTo.Text, result: out var lambertPageOriginLatitude))
                                {
                                    if (lambertPageOriginLatitude is >= -90 and <= 90)
                                    {
                                        if (double.TryParse(s: LambertPageParallel1To.Text, result: out var lambertPageParallel1))
                                        {
                                            if (lambertPageParallel1 is >= -90 and <= 90)
                                            {
                                                if (double.TryParse(s: LambertPageParallel2To.Text, result: out var lambertPageParallel2))
                                                {
                                                    if (lambertPageParallel2 is >= -90 and <= 90)
                                                    {
                                                        if (lambertPageParallel2 > lambertPageParallel1)
                                                            toX!.Add(
                                                                content: new XElement(name: "Lambert",
                                                                    content: new object[]
                                                                    {
                                                                        new XElement(
                                                                            name: "CentralMeridian",
                                                                            content: lambertPageCentralMeridian
                                                                        ),
                                                                        new XElement(
                                                                            name: "OriginLatitude",
                                                                            content: lambertPageOriginLatitude
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel1",
                                                                            content: lambertPageParallel1
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel2",
                                                                            content: lambertPageParallel2
                                                                        ),
                                                                        new XElement(
                                                                            name: "Srid",
                                                                            content: LambertPage1954To.Checked
                                                                                ? 1954
                                                                                : LambertPage1980To.Checked
                                                                                    ? 1980
                                                                                    : LambertPage1984To.Checked
                                                                                        ? 1984
                                                                                        : 2000
                                                                        )
                                                                    }
                                                                )
                                                            );
                                                        else
                                                            errorMessage = "To: Parallel2 should be > Parallel1.";
                                                    }
                                                    else
                                                        errorMessage = "To: Parallel2 should be >= -90 and <= 90.";
                                                }
                                                else
                                                    errorMessage = "To: Parallel2 should be of double type.";
                                            }
                                            else
                                                errorMessage = "To: Parallel1 should be >= -90 and <= 90.";
                                        }
                                        else
                                            errorMessage = "To: Parallel1 should be of double type.";
                                    }
                                    else
                                        errorMessage = "To: OriginLatitude should be >= -90 and <= 90.";
                                }
                                else
                                    errorMessage = "To: OriginLatitude should be of double type.";
                            }
                            else
                                errorMessage = "To: CentralMeridian should be >= -180 and <= 180.";
                        }
                        else
                            errorMessage = "To: CentralMeridian should be of double type.";
                        break;
                    }
                case 3: //Albers
                    {
                        if (double.TryParse(s: AlbersPageCentralMeridianTo.Text, result: out var albersPageCentralMeridian))
                        {
                            if (albersPageCentralMeridian is >= -180 and <= 180)
                            {
                                if (double.TryParse(s: AlbersPageOriginLatitudeTo.Text, result: out var albersPageOriginLatitude))
                                {
                                    if (albersPageOriginLatitude is >= -90 and <= 90)
                                    {
                                        if (double.TryParse(s: AlbersPageParallel1To.Text, result: out var albersPageParallel1))
                                        {
                                            if (albersPageParallel1 is >= -90 and <= 90)
                                            {
                                                if (double.TryParse(s: AlbersPageParallel2To.Text, result: out var albersPageParallel2))
                                                {
                                                    if (albersPageParallel2 is >= -90 and <= 90)
                                                    {
                                                        if (albersPageParallel2 > albersPageParallel1)
                                                            toX!.Add(
                                                                content: new XElement(name: "Albers",
                                                                    content: new object[]
                                                                    {
                                                                        new XElement(
                                                                            name: "CentralMeridian",
                                                                            content: albersPageCentralMeridian
                                                                        ),
                                                                        new XElement(
                                                                            name: "OriginLatitude",
                                                                            content: albersPageOriginLatitude
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel1",
                                                                            content: albersPageParallel1
                                                                        ),
                                                                        new XElement(
                                                                            name: "Parallel2",
                                                                            content: albersPageParallel2
                                                                        ),
                                                                        new XElement(
                                                                            name: "Srid",
                                                                            content: AlbersPage1954To.Checked ? 1954 :
                                                                            AlbersPage1980To.Checked ? 1980 :
                                                                            AlbersPage1984To.Checked ? 1984 : 2000
                                                                        )
                                                                    })
                                                            );
                                                        else
                                                            errorMessage = "To: Parallel2 should be > Parallel1.";
                                                    }
                                                    else
                                                        errorMessage = "To: Parallel2 should be >= -90 and <= 90.";
                                                }
                                                else
                                                    errorMessage = "To: Parallel2 should be of double type.";
                                            }
                                            else
                                                errorMessage = "To: Parallel1 should be >= -90 and <= 90.";
                                        }
                                        else
                                            errorMessage = "To: Parallel1 should be of double type.";
                                    }
                                    else
                                        errorMessage = "To: OriginLatitude should be >= -90 and <= 90.";
                                }
                                else
                                    errorMessage = "To: OriginLatitude should be of double type.";
                            }
                            else
                                errorMessage = "To: CentralMeridian should be >= -180 and <= 180.";
                        }
                        else
                            errorMessage = "To: CentralMeridian should be of double type.";
                        break;
                    }
                case 4: //Web-Mercator
                    {
                        toX!.Add(content: new XElement(name: "Web-Mercator"));
                        break;
                    }
            }
            if (errorMessage == null)
            {
                if (
                    MessageBox.Show(
                        text: string.Join(
                            separator: Environment.NewLine,
                            value: Regex.Split(
                                input: Projection.ToString(options: SaveOptions.None),
                                pattern: @"[\r\n]+",
                                options: RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace |
                                         RegexOptions.Singleline |
                                         RegexOptions.Multiline
                            )
                        ),
                        caption: @"Are you sure ?",
                        buttons: MessageBoxButtons.YesNo,
                        icon: MessageBoxIcon.Question
                    ) == DialogResult.Yes
                )
                    Close();
            }
            else
            {
                Projection = null;
                AppendProjectionTip(tip: errorMessage);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Projection = null;
            Close();
        }

        private void GaussKrugerPageZoneFrom_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "6 Degree":
                    {
                        GaussKrugerPage3DegreeFrom.Checked =
                            //GaussKrugerPage6DegreeFrom.Checked =
                            GaussKrugerPageUnknownFrom.Checked =
                                false;
                        break;
                    }
                case "3 Degree":
                    {
                        //GaussKrugerPage3DegreeFrom.Checked =
                        GaussKrugerPage6DegreeFrom.Checked =
                            GaussKrugerPageUnknownFrom.Checked =
                                false;
                        break;
                    }
                case "Unknown":
                    {
                        GaussKrugerPage3DegreeFrom.Checked =
                            GaussKrugerPage6DegreeFrom.Checked =
                                //GaussKrugerPageUnknownFrom.Checked =
                                false;
                        break;
                    }
            }
            GaussKrugerPageCentralMeridianFrom_MouseLeave(sender, e);
        }

        private void GaussKrugerPageZoneTo_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "6 Degree":
                    {
                        GaussKrugerPage3DegreeTo.Checked =
                            //GaussKrugerPage6DegreeTo.Checked =
                            GaussKrugerPageUnknownTo.Checked =
                                false;
                        break;
                    }
                case "3 Degree":
                    {
                        //GaussKrugerPage3DegreeTo.Checked =
                        GaussKrugerPage6DegreeTo.Checked =
                            GaussKrugerPageUnknownTo.Checked =
                                false;
                        break;
                    }
                case "Unknown":
                    {
                        GaussKrugerPage3DegreeTo.Checked =
                            GaussKrugerPage6DegreeTo.Checked =
                                //GaussKrugerPageUnknownTo.Checked =
                                false;
                        break;
                    }
            }
            GaussKrugerPageCentralMeridianTo_MouseLeave(sender, e);
        }

        private void GaussKrugerPageCRSFrom_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "1954 Beijing":
                    {
                        //GaussKrugerPage1954From.Checked =
                        GaussKrugerPage1980From.Checked =
                            GaussKrugerPage1984From.Checked =
                                GaussKrugerPage2000From.Checked =
                                    false;
                        break;
                    }
                case "1980 Xi'an":
                    {
                        GaussKrugerPage1954From.Checked =
                            //GaussKrugerPage1980From.Checked =
                            GaussKrugerPage1984From.Checked =
                                GaussKrugerPage2000From.Checked =
                                    false;
                        break;
                    }
                case "1984 UTM":
                    {
                        GaussKrugerPage1954From.Checked =
                            GaussKrugerPage1980From.Checked =
                                //GaussKrugerPage1984From.Checked =
                                GaussKrugerPage2000From.Checked =
                                    false;
                        break;
                    }
                case "2000 CGCS":
                    {
                        GaussKrugerPage1954From.Checked =
                            GaussKrugerPage1980From.Checked =
                                GaussKrugerPage1984From.Checked =
                                    //GaussKrugerPage2000From.Checked =
                                    false;
                        break;
                    }
            }
        }

        private void GaussKrugerPageCRSTo_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "1954 Beijing":
                    {
                        //GaussKrugerPage1954To.Checked =
                        GaussKrugerPage1980To.Checked =
                            GaussKrugerPage1984To.Checked =
                                GaussKrugerPage2000To.Checked =
                                    false;
                        break;
                    }
                case "1980 Xi'an":
                    {
                        GaussKrugerPage1954To.Checked =
                            //GaussKrugerPage1980To.Checked =
                            GaussKrugerPage1984To.Checked =
                                GaussKrugerPage2000To.Checked =
                                    false;
                        break;
                    }
                case "1984 UTM":
                    {
                        GaussKrugerPage1954To.Checked =
                            GaussKrugerPage1980To.Checked =
                                //GaussKrugerPage1984To.Checked =
                                GaussKrugerPage2000To.Checked =
                                    false;
                        break;
                    }
                case "2000 CGCS":
                    {
                        GaussKrugerPage1954To.Checked =
                            GaussKrugerPage1980To.Checked =
                                GaussKrugerPage1984To.Checked =
                                    //GaussKrugerPage2000To.Checked =
                                    false;
                        break;
                    }
            }
        }

        private void LambertPageCRSFrom_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "1954 Beijing":
                    {
                        //LambertPage1954From.Checked =
                        LambertPage1980From.Checked =
                            LambertPage1984From.Checked =
                                LambertPage2000From.Checked =
                                    false;
                        break;
                    }
                case "1980 Xi'an":
                    {
                        LambertPage1954From.Checked =
                            //LambertPage1980From.Checked =
                            LambertPage1984From.Checked =
                                LambertPage2000From.Checked =
                                    false;
                        break;
                    }
                case "1984 UTM":
                    {
                        LambertPage1954From.Checked =
                            LambertPage1980From.Checked =
                                //LambertPage1984From.Checked =
                                LambertPage2000From.Checked =
                                    false;
                        break;
                    }
                case "2000 CGCS":
                    {
                        LambertPage1954From.Checked =
                            LambertPage1980From.Checked =
                                LambertPage1984From.Checked =
                                    //LambertPage2000From.Checked =
                                    false;
                        break;
                    }
            }
        }

        private void LambertPageCRSTo_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "1954 Beijing":
                    {
                        //LambertPage1954To.Checked =
                        LambertPage1980To.Checked =
                            LambertPage1984To.Checked =
                                LambertPage2000To.Checked =
                                    false;
                        break;
                    }
                case "1980 Xi'an":
                    {
                        LambertPage1954To.Checked =
                            //LambertPage1980To.Checked =
                            LambertPage1984To.Checked =
                                LambertPage2000To.Checked =
                                    false;
                        break;
                    }
                case "1984 UTM":
                    {
                        LambertPage1954To.Checked =
                            LambertPage1980To.Checked =
                                //LambertPage1984To.Checked =
                                LambertPage2000To.Checked =
                                    false;
                        break;
                    }
                case "2000 CGCS":
                    {
                        LambertPage1954To.Checked =
                            LambertPage1980To.Checked =
                                LambertPage1984To.Checked =
                                    //LambertPage2000To.Checked =
                                    false;
                        break;
                    }
            }
        }

        private void AlbersPageCRSFrom_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "1954 Beijing":
                    {
                        //AlbersPage1954From.Checked =
                        AlbersPage1980From.Checked =
                            AlbersPage1984From.Checked =
                                AlbersPage2000From.Checked =
                                    false;
                        break;
                    }
                case "1980 Xi'an":
                    {
                        AlbersPage1954From.Checked =
                            //AlbersPage1980From.Checked =
                            AlbersPage1984From.Checked =
                                AlbersPage2000From.Checked =
                                    false;
                        break;
                    }
                case "1984 UTM":
                    {
                        AlbersPage1954From.Checked =
                            AlbersPage1980From.Checked =
                                //AlbersPage1984From.Checked =
                                AlbersPage2000From.Checked =
                                    false;
                        break;
                    }
                case "2000 CGCS":
                    {
                        AlbersPage1954From.Checked =
                            AlbersPage1980From.Checked =
                                AlbersPage1984From.Checked =
                                    //AlbersPage2000From.Checked =
                                    false;
                        break;
                    }
            }
        }

        private void AlbersPageCRSTo_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (!radioButton.Checked)
                return;
            switch (radioButton.Text)
            {
                case "1954 Beijing":
                    {
                        //AlbersPage1954To.Checked =
                        AlbersPage1980To.Checked =
                            AlbersPage1984To.Checked =
                                AlbersPage2000To.Checked =
                                    false;
                        break;
                    }
                case "1980 Xi'an":
                    {
                        AlbersPage1954To.Checked =
                            //AlbersPage1980To.Checked =
                            AlbersPage1984To.Checked =
                                AlbersPage2000To.Checked =
                                    false;
                        break;
                    }
                case "1984 UTM":
                    {
                        AlbersPage1954To.Checked =
                            AlbersPage1980To.Checked =
                                //AlbersPage1984To.Checked =
                                AlbersPage2000To.Checked =
                                    false;
                        break;
                    }
                case "2000 CGCS":
                    {
                        AlbersPage1954To.Checked =
                            AlbersPage1980To.Checked =
                                AlbersPage1984To.Checked =
                                    //AlbersPage2000To.Checked =
                                    false;
                        break;
                    }
            }
        }

        private void ProjectionPurposeTabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var tag = e.TabPage?.Tag?.ToString();
            if (tag != null && !bool.Parse(value: tag))
                e.Cancel = true;
        }

        private void ProjectionTabControlFrom_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var tag = e.TabPage?.Tag?.ToString();
            if (tag != null && !bool.Parse(value: tag))
                e.Cancel = true;
        }

        private void ProjectionTabControlTo_Selecting(object sender, TabControlCancelEventArgs e)
        {
            var tag = e.TabPage?.Tag?.ToString();
            if (tag != null && !bool.Parse(value: tag))
                e.Cancel = true;
        }

        private void GaussKrugerPageCentralMeridianFrom_MouseLeave(object sender, EventArgs e)
        {
            var longitudeString =
                GeositeServer.Vector.Ellipsoid.Degree2Dms(DMS: GaussKrugerPageCentralMeridianFrom.Text);
            if (longitudeString != null)
            {
                if (GaussKrugerPage6DegreeFrom.Checked)
                {
                    GaussKrugerPageCentralMeridianFrom.Text = $@"{GeositeServer.Vector.Ellipsoid.CentralMeridian6(double.Parse(longitudeString))}";
                    GaussKrugerPageCentralMeridianZoneFrom.Text = $@"{GeositeServer.Vector.Ellipsoid.Zone6(double.Parse(longitudeString))}";
                }
                else
                {
                    if (GaussKrugerPage3DegreeFrom.Checked)
                    {
                        GaussKrugerPageCentralMeridianFrom.Text = $@"{GeositeServer.Vector.Ellipsoid.CentralMeridian3(double.Parse(longitudeString))}";
                        GaussKrugerPageCentralMeridianZoneFrom.Text = $@"{GeositeServer.Vector.Ellipsoid.Zone3(double.Parse(longitudeString))}";
                    }
                    else
                    {
                        GaussKrugerPageCentralMeridianZoneFrom.Text = "";
                    }
                }
            }
            else
            {
                GaussKrugerPageCentralMeridianZoneFrom.Text = "";
            }
        }

        private void GaussKrugerPageCentralMeridianTo_MouseLeave(object sender, EventArgs e)
        {
            var longitudeString =
                GeositeServer.Vector.Ellipsoid.Degree2Dms(DMS: GaussKrugerPageCentralMeridianTo.Text);
            if (longitudeString != null)
            {
                if (GaussKrugerPage6DegreeTo.Checked)
                {
                    GaussKrugerPageCentralMeridianTo.Text = $@"{GeositeServer.Vector.Ellipsoid.CentralMeridian6(double.Parse(longitudeString))}";
                    GaussKrugerPageCentralMeridianZoneTo.Text = $@"{GeositeServer.Vector.Ellipsoid.Zone6(double.Parse(longitudeString))}";
                }
                else
                {
                    if (GaussKrugerPage3DegreeTo.Checked)
                    {
                        GaussKrugerPageCentralMeridianTo.Text = $@"{GeositeServer.Vector.Ellipsoid.CentralMeridian3(double.Parse(longitudeString))}";
                        GaussKrugerPageCentralMeridianZoneTo.Text = $@"{GeositeServer.Vector.Ellipsoid.Zone3(double.Parse(longitudeString))}";
                    }
                    else
                    {
                        GaussKrugerPageCentralMeridianTo.Text = "";
                    }
                }
            }
            else
            {
                GaussKrugerPageCentralMeridianTo.Text = "";
            }
        }

        private void Degree_MouseLeave(object sender, EventArgs e)
        {
            var theSender = (TextBox)sender;
            if (theSender == null)
                return;
            var degree = GeositeServer.Vector.Ellipsoid.Degree2Dms(DMS: theSender.Text);
            if (degree != null)
                theSender.Text = degree;
        }
    }
}
