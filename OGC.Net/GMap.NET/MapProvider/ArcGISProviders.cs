namespace GMap.NET.MapProviders.ArcGISProviders
{
    /// <summary>
    ///     ArcGIS COSMO Imagery provider, http://server.arcgisonline.com
    /// </summary>
    public class ArcGISImageryProvider : ArcGISMapMercatorProviderBase
    {
        public static readonly ArcGISImageryProvider Instance;

        ArcGISImageryProvider()
        {
        }

        static ArcGISImageryProvider()
        {
            Instance = new ArcGISImageryProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = Guid.NewGuid();

        public override string Name => "ArcGISImagery";

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                return GetTileImageUsingHttp(MakeTileImageUrl(pos, zoom, LanguageStr)); //Ôö²¹ Í¸Ã÷¶È alpha
            }
            catch 
            {
                return null;
            }
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            return string.Format(UrlFormat, zoom, pos.Y, pos.X);
        }

        public string UrlFormat =
            "https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{0}/{1}/{2}";

        public override GMapProvider[] Overlays => _overlays ?? new GMapProvider[] { this };
    }
}
