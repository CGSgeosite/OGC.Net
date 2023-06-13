namespace GMap.NET.Extend
{
    /* 增补：
     * 点要素类（二维单点）启用Gmap.net提供的【PointLatLng】类，目的是加速处理可视化要素
     */

    /// <summary>
    /// 线段类（由首尾端点要素构成的单条线段）
    /// </summary>
    public class Segment
    {
        public Segment(PointLatLng startPoint, PointLatLng endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Segment(Segment segment)
        {
            StartPoint = segment.StartPoint;
            EndPoint = segment.EndPoint;
        }

        private (double north, double south, double west, double east)? _boundary;

        public (double north, double south, double west, double east) Boundary(bool update = false)
        {
            if (update || _boundary == null)
            {
                _boundary =
                (
                    Math.Max(StartPoint.Lat, EndPoint.Lat), //north
                    Math.Min(StartPoint.Lat, EndPoint.Lat), //south
                    Math.Min(StartPoint.Lng, EndPoint.Lng), //west
                    Math.Max(StartPoint.Lng, EndPoint.Lng) //east
                );
            }

            return _boundary.Value;
        }

        public PointLatLng StartPoint { get; set; }

        public PointLatLng EndPoint { get; set; }

        /// <summary>
        /// 求取两个线段的交点坐标或重叠点坐标
        /// </summary>
        /// <param name="segment">线段</param>
        /// <param name="sameLine">两线段是否属于同一条线？默认：false</param>
        /// <returns>如果不相交不重叠，返回null，否则返回交点或重叠点</returns>
        public (PointLatLng point, int type)? GetIntersection(Segment segment, bool sameLine = false)
        {
            var x1 = StartPoint.Lng;
            var y1 = StartPoint.Lat;
            var x2 = EndPoint.Lng;
            var y2 = EndPoint.Lat;
            if (Equals(StartPoint, EndPoint))
                return (StartPoint, 0b1000);

            var x3 = segment.StartPoint.Lng;
            var y3 = segment.StartPoint.Lat;
            var x4 = segment.EndPoint.Lng;
            var y4 = segment.EndPoint.Lat;
            if (Equals(segment.StartPoint, segment.EndPoint))
                return (segment.StartPoint, 0b1000);

            if (sameLine)
            {
                if (Equals(StartPoint, segment.StartPoint) && Equals(EndPoint, segment.EndPoint))
                    return null;
            }
            else
            {
                if (Equals(StartPoint, segment.StartPoint) && Equals(EndPoint, segment.EndPoint) ||
                    Equals(StartPoint, segment.EndPoint) && Equals(EndPoint, segment.StartPoint))
                    return (segment.EndPoint, 0b100);
            }

            var box12 = Boundary();
            var box34 = segment.Boundary();

            if (box12.west <= box34.east &&
                box12.east >= box34.west
                && box12.south <= box34.north &&
                box12.north >= box34.south)
            {
                var delta = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                if (!(Math.Abs(delta) <= 1e-15))
                {
                    var s = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / delta;
                    var t = ((y1 - y2) * (x1 - x3) - (x1 - x2) * (y1 - y3)) / delta;
                    if (s is > 0 and < 1 && t is > 0 and < 1)
                        return (new PointLatLng(y1 + s * (y2 - y1), x1 + s * (x2 - x1)), 0b10000);
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 线条要素类（由若干个点要素构成的单条几何线条）
    /// </summary>
    public class Polyline : MapRoute
    {
        public Polyline(List<PointLatLng> points) : base(points, null)
        { }

        private (double north, double south, double west, double east)? _boundary;


        public (double north, double south, double west, double east) Boundary(bool update = false)
        {
            if (update || _boundary == null)
            {
                var north = double.MinValue;
                var south = double.MaxValue;
                var west = double.MaxValue;
                var east = double.MinValue;
                foreach (var vertex in Points)
                {
                    if (vertex.Lat > north)
                        north = vertex.Lat;
                    if (vertex.Lat < south)
                        south = vertex.Lat;
                    if (vertex.Lng > east)
                        east = vertex.Lng;
                    if (vertex.Lng < west)
                        west = vertex.Lng;
                }
                _boundary = (north, south, west, east);
            }
            return _boundary.Value;
        }
    }
}
