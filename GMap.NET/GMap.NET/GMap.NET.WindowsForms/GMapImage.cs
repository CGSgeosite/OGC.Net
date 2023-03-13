using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GMap.NET.Internals;
using GMap.NET.MapProviders;

namespace GMap.NET.WindowsForms
{
    /// <summary>
    ///     image abstraction
    /// </summary>
    public class GMapImage : PureImage
    {
        public Image Img;

        public override void Dispose()
        {
            if (Img != null)
            {
                Img.Dispose();
                Img = null;
            }

            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }
    }

    /// <summary>
    ///     image abstraction proxy
    /// </summary>
    public class GMapImageProxy : PureImageProxy
    {
        GMapImageProxy()
        {
        }

        public static void Enable()
        {
            GMapProvider.TileImageProxy = Instance;
        }

        public static readonly GMapImageProxy Instance = new();

        internal ColorMatrix ColorMatrix;

        static readonly bool Win7OrLater = Stuff.IsRunningOnWin7OrLater();

        private readonly ColorMatrix _colorMatrix = new(
            new[]
            {
                new[] { 1f, 0, 0, 0, 0 },
                new[] { 0, 1f, 0, 0, 0 },
                new[] { 0, 0, 1f, 0, 0 },
                new[] { 0, 0, 0, 1f, 0 },
                new[] { 0, 0, 0, 0, 1f }
            }
        );

        public override PureImage FromStream(Stream stream, float? alpha = null) //增补 透明度 alpha
        {
            try
            {
                var m = Image.FromStream(stream, true, !Win7OrLater);
                ////增补 透明度
                var image = ColorMatrix != null ? ApplyColorMatrix(m, ColorMatrix) : m;
                if (alpha >= 0 && 1f-alpha >= 1e-6)
                {
                    _colorMatrix[3, 3] = alpha.Value;
                    image = ApplyColorMatrix(m, _colorMatrix);
                }

                return new GMapImage { Img = image };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FromStream: " + ex);
            }

            return null;
        }

        public override bool Save(Stream stream, PureImage image)
        {
            var ret = image as GMapImage;
            var ok = true;

            if (ret?.Img != null)
            {
                // try png
                try
                {
                    ret.Img.Save(stream, ImageFormat.Png);
                }
                catch
                {
                    // try jpeg
                    try
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        ret.Img.Save(stream, ImageFormat.Jpeg);
                    }
                    catch
                    {
                        ok = false;
                    }
                }
            }
            else
            {
                ok = false;
            }

            return ok;
        }

        Bitmap ApplyColorMatrix(Image original, ColorMatrix matrix)
        {
            // create a blank bitmap the same size as original
            var newBitmap = new Bitmap(original.Width, original.Height);

            using (original) // destroy original
            {
                // get a graphics object from the new image
                using (var g = Graphics.FromImage(newBitmap))
                {
                    // set the color matrix attribute
                    using (var attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(matrix);
                        g.DrawImage(
                            original,
                            new Rectangle(0, 0, original.Width, original.Height),
                            0,
                            0,
                            original.Width,
                            original.Height,
                            GraphicsUnit.Pixel,
                            attributes);
                    }
                }
            }

            return newBitmap;
        }
    }
}
