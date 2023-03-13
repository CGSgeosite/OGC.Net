using System;
using System.IO;

namespace GMap.NET
{
    /// <summary>
    ///     image abstraction proxy
    /// </summary>
    public abstract class PureImageProxy
    {
        public abstract PureImage FromStream(Stream stream, float? alpha = null); //增补 透明度 alpha

        public abstract bool Save(Stream stream, PureImage image);

        public PureImage FromArray(byte[] data, float? alpha = null) //增补 透明度 alpha
        {
            var m = new MemoryStream(data, 0, data.Length, false, true);
            var pi = FromStream(m, alpha);
            if (pi != null)
            {
                m.Position = 0;
                pi.Data = m;
            }

            return pi;
        }
    }

    /// <summary>
    ///     image abstraction
    /// </summary>
    public abstract class PureImage : IDisposable
    {
        public MemoryStream Data;

        internal bool IsParent;
        internal long Ix;
        internal long Xoff;
        internal long Yoff;

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }

    public class DefaultImageProxy : PureImageProxy
    {
        public static readonly DefaultImageProxy Instance = new DefaultImageProxy();

        private DefaultImageProxy()
        {
        }

        public override PureImage FromStream(Stream stream, float? alpha = null) //增补 透明度 alpha
        {
            return new DefaultImage();
        }

        public override bool Save(Stream stream, PureImage image)
        {
            if (image.Data != null)
            {
                image.Data.WriteTo(stream);
                image.Data.Position = 0;
                return true;
            }

            return false;
        }

        private class DefaultImage : PureImage
        {
            public override void Dispose()
            {
            }
        }
    }
}
