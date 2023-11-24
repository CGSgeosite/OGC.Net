using GMap.NET.Projections;
using System.Text;
using System.Text.RegularExpressions;

//GMap.net 默认使用 Web Mercator 投影（EPSG：3857）显示地图瓦片

namespace GMap.NET.MapProviders.GeositeMapProvider
{
    public class MapProvider : GMapProvider
    {
        public static readonly MapProvider Instance;

        /// <summary>
        /// 投影系代号投影系代号，默认：3857，支持：3857/4326
        /// </summary>
        private readonly int _srid;

        /// <summary>
        /// 瓦片尺寸，默认：256，支持：256/512
        /// </summary>
        private readonly int _tileSize;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="srid">投影系代号，默认：3857，支持：3857/4326</param>
        /// <param name="tileSize">瓦片尺寸，默认：256，支持：256/512</param>
        public MapProvider(int srid = 3857, int tileSize = 256)
        {
            _srid = srid switch
            {
                4326 => 4326,
                _ => 3857
            };

            _tileSize = tileSize switch
            {
                512 => 512,
                _ => 256
            };

            MaxZoom = 24;

            //Copyright = ""
            //MinZoom = 1;
            //MaxZoom = 18;
            //ServerLetters = ""
            //Area ==>> RectLatLng
            //Alpha
        }

        static MapProvider()
        {
            Instance = new MapProvider();
        }

        #region GMapProvider Members


        /// <summary>
        /// 图层标识码
        /// </summary>
        public override Guid Id
        {
            get;
        } = Guid.NewGuid();

        /// <summary>
        /// 图层名称
        /// </summary>
        public override string Name => "GeositeMapProvider";

        /// <summary>
        /// 依据位置和缩放级获取瓦片影像
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 投影方式
        /// </summary>
        public override PureProjection Projection => _srid == 3857 ? MercatorProjection.Instance : PlateCarreeProjection.Instance;

        /// <summary>
        /// 访问地址
        /// </summary>
        public string UrlFormat = string.Empty; //在调用实例中应对其赋值 微软必应地图占位符为【{bingmap}】

        #endregion

        /// <summary>
        /// 访问地址中的子域占位符
        /// </summary>
        public string ServerLetters = ""; //子域占位符 在调用实例中可对其赋值 天地图为："01234567" 

        /// <summary>
        /// 依据位置和缩放级获取访问地址
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private string MakeTileImageUrl(GPoint pos, int zoom)
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
