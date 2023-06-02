using GMap.NET.Projections;

//gmap.net 默认使用Web Mercator投影（EPSG：3857）显示地图瓦片

namespace GMap.NET.MapProviders.WmtsProvider
{
    public class WmtsProvider : GMapProvider
    {
        public static readonly WmtsProvider Instance;

        WmtsProvider()
        {

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

        public string UrlFormat = string.Empty;

        #endregion

        public string ServerLetters = "";

        string MakeTileImageUrl(GPoint pos, int zoom)
        {
            var index = GetServerNum(pos, Math.Max(ServerLetters?.Length ?? 0, 1));
            var letter = (ServerLetters?.Length > index ? ServerLetters?[index].ToString() : "") ?? "";
            return string.Format(
                UrlFormat
                    .Replace("{s}", letter)
                    .Replace("{subdomains}", letter)
                    .Replace("{z}", "{0}")
                    .Replace("{x}", "{1}")
                    .Replace("{y}", "{2}"),
                zoom,
                pos.X,
                pos.Y
            );
        }
        
        public override GMapProvider[] Overlays => _overlays ?? new GMapProvider[] { this };
    }
}
