using GMap.NET.Projections;
using System.Text;
using System.Text.RegularExpressions;

//GMap.net 默认使用 Web Mercator 投影（EPSG：3857）显示地图瓦片

namespace GMap.NET.MapProviders.WmtsProvider
{
    public class WmtsProvider : GMapProvider
    {
        public static readonly WmtsProvider Instance;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WmtsProvider()
        {
            //Copyright = ""
            //MinZoom = 1;
            //MaxZoom = 18;
            //ServerLetters = ""
            //Area ==>> RectLatLng
            //Alpha
        }

        static WmtsProvider()
        {
            Instance = new WmtsProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = Guid.NewGuid();

        public override string Name => "WmtsProvider";

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                return GetTileImageUsingHttp(MakeTileImageUrl(pos, zoom));
            }
            catch
            {
                return null;
            }
        }

        public override PureProjection Projection => MercatorProjection.Instance;

        public string UrlFormat = string.Empty; //在调用实例中应对其赋值 微软必应地图占位符为【{bingmap}】

        #endregion

        public string ServerLetters = ""; //子域占位符 在调用实例中可对其赋值 天地图为："01234567" 

        string MakeTileImageUrl(GPoint pos, int zoom)
        {
            var index = GetServerNum(pos, Math.Max(ServerLetters?.Length ?? 0, 1));
            var letter = (ServerLetters?.Length > index ? ServerLetters?[index].ToString() : "") ?? "";
            return string.Format(
                Regex.Replace(
                    Regex.Replace(
                        Regex.Replace(
                            Regex.Replace(
                                Regex.Replace(UrlFormat, "{bingmap}", TileXYToQuadKey(pos.X, pos.Y, zoom), RegexOptions.IgnoreCase),
                                "{subdomains}|{subdomain}|{s}",
                                letter,
                                RegexOptions.IgnoreCase
                            ),
                            "{z}",
                            "{0}",
                            RegexOptions.IgnoreCase
                        ),
                        "{x}",
                        "{1}",
                        RegexOptions.IgnoreCase
                    ),
                    "{y}",
                    "{2}",
                    RegexOptions.IgnoreCase
                ),
                zoom,
                pos.X,
                pos.Y
            );
        }

        /// <summary>
        ///     Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">
        ///     Level of detail, from 1 (lowest detail)
        ///     to 23 (highest detail).
        /// </param>
        /// <returns>A string containing the QuadKey.</returns>
        private string TileXYToQuadKey(long tileX, long tileY, int levelOfDetail)
        {
            //var key = TileXYToQuadKey(pos.X, pos.Y, zoom);

            var quadKey = new StringBuilder();
            for (var i = levelOfDetail; i > 0; i--)
            {
                var digit = '0';
                var mask = 1 << (i - 1);
                if ((tileX & mask) != 0) 
                    digit++;
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }

        public override GMapProvider[] Overlays => _overlays ?? new GMapProvider[] { this };
    }
}
