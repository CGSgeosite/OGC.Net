using System.Net;
using GMap.NET.Projections;

namespace GMap.NET.MapProviders.TiandituProviders
{
    public abstract class TiandituProvidersBase : GMapProvider
    {
        private const string ServerUrl = "http://t{s}.tianditu.gov.cn/{c}_w/wmts" +
                                         "?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0" +
                                         "&LAYER={c}&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles" +
                                         "&TILEMATRIX={z}&TILECOL={x}&TILEROW={y}&tk={k}";

        private const string ServerLetters = "01234567";

        private readonly string _category;

        protected TiandituProvidersBase(string category, int maxZoom = 18, string apiKey = null)
        {
            //可通过访问 https://console.tianditu.gov.cn/api/key 申请【浏览器端】类型的 API Key
            //App.config 编译后将变为 OGC.Net.dll.config 
            ApiKey =
                apiKey
                ?? System.Configuration.ConfigurationManager.AppSettings["TIANDITU_APIKEY"]
                ?? "85c9d12d5d691d168ba5cb6ecaa749eb";

            RefererUrl = "https://www.tianditu.gov.cn/";

            _category = category;
            MinZoom = 1;
            MaxZoom = maxZoom;

            Copyright = $"© 天地图 - Map data ©{DateTime.Today.Year} TianDiTu";
        }

        public static string UrlFormat { get; set; } = ServerUrl
            .Replace("{s}", "{0}")
            .Replace("{c}", "{1}")
            .Replace("{x}", "{2}")
            .Replace("{y}", "{3}")
            .Replace("{z}", "{4}")
            .Replace("{k}", "{5}");

        public static string ApiKey { get; set; }

        public override PureProjection Projection => MercatorProjection.Instance;

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

        protected override void InitializeWebRequest(WebRequest request)
        {
            base.InitializeWebRequest(request);

            switch (request)
            {
                case HttpWebRequest httpWebRequest:
                    httpWebRequest.Accept = null;
                    break;
                default:
                    request.Headers.Remove("Accept");
                    break;
            }
        }

        protected virtual string MakeTileImageUrl(GPoint pos, int zoom)
        {
            var letter = ServerLetters[GetServerNum(pos, ServerLetters.Length)];
            return string.Format(UrlFormat, letter, _category, pos.X, pos.Y, zoom, ApiKey ?? "");
        }
    }

    public class TiandituImageProvider : TiandituProvidersBase
    {
        public static readonly TiandituImageProvider Instance = new();

        private TiandituImageProvider() : base("img")
        {
            //遥感影像层
        }

        public override Guid Id { get; } = Guid.NewGuid();

        public override string Name => nameof(TiandituImageProvider);

        public override GMapProvider[] Overlays => new GMapProvider[] { this };
    }

    public class TiandituTerrainProvider : TiandituProvidersBase
    {
        public static readonly TiandituTerrainProvider Instance = new();

        private TiandituTerrainProvider() : base("ter", 14)
        {
        }

        public override Guid Id { get; } = Guid.NewGuid();

        public override string Name => nameof(TiandituTerrainProvider);

        public override GMapProvider[] Overlays => new GMapProvider[] { this };
    }

    public class TiandituVectorProvider : TiandituProvidersBase
    {
        public static readonly TiandituVectorProvider Instance = new();

        private TiandituVectorProvider() : base("vec")
        {
        }

        public override Guid Id { get; } = Guid.NewGuid();

        public override string Name => nameof(TiandituVectorProvider);

        public override GMapProvider[] Overlays => new GMapProvider[] { this };
    }

    public class TiandituImageryMapProvider : TiandituProvidersBase
    {
        public static readonly TiandituImageryMapProvider Instance = new();

        private TiandituImageryMapProvider() : base("cia")
        {
            //注记层
        }

        public override Guid Id { get; } = Guid.NewGuid();

        public override string Name => "TianDiTuImageMap";

        public override GMapProvider[] Overlays => _overlays ??= new GMapProvider[]
        {
            TiandituImageProvider.Instance //最底层 - 遥感影像层
            ,this //上层 - 注记层
        };
    }

    public class TiandituTerrainMapProvider : TiandituProvidersBase
    {
        public static readonly TiandituTerrainMapProvider Instance = new();

        private TiandituTerrainMapProvider() : base("cia")
        {
            //注记层
        }

        public override Guid Id { get; } = Guid.NewGuid();

        public override string Name => "TianDiTuTerrainMap";

        public override GMapProvider[] Overlays =>
            _overlays ??= new GMapProvider[]
            {
                TiandituTerrainProvider.Instance //最底层 - 地势模型层
                , this //上层 - 注记层
            };
    }

    public class TiandituRoadsMapProvider : TiandituProvidersBase
    {
        public static readonly TiandituRoadsMapProvider Instance = new();

        private TiandituRoadsMapProvider() : base("cva")
        {
        }

        public override Guid Id { get; } = Guid.NewGuid();

        public override string Name => "TianDiTuVectorMap";

        public override GMapProvider[] Overlays => _overlays ??= new GMapProvider[]
        {
            TiandituVectorProvider.Instance //最底层 - 地理线划层
            , this //上层 - 区划标注层
        };
    }
}
