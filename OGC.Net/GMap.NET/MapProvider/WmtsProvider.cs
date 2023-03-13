using GMap.NET.Projections;

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
                var url = MakeTileImageUrl(pos, zoom);
                return GetTileImageUsingHttp(url);
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

            var url = UrlFormat;
            url = url.Replace("{s}", letter);
            url = url.Replace("{z}", "{0}");
            url = url.Replace("{x}", "{1}");
            url = url.Replace("{y}", "{2}");

            return string.Format(url, zoom, pos.X, pos.Y);
        }
        
        public override GMapProvider[] Overlays => _overlays ?? new GMapProvider[] { this };
    }
}
