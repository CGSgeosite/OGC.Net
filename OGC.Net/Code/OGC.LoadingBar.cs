/******************************************************************************
 *
 * Name: OGC.net
 * Purpose: A free tool for reading ShapeFile, MapGIS, Excel/TXT/CSV, converting
 *          into GML, GeoJSON, ShapeFile, KML and GeositeXML, and pushing vector
 *          or raster to PostgreSQL database.
 *
 ******************************************************************************
 * (C) 2019-2024 Geosite Development Team of CGS (R)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

namespace Geosite
{
    public class LoadingBar
    {
        public int Count;

        private readonly ProgressBar _bar;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bar">ProgressBar 类型对象</param>
        public LoadingBar(ProgressBar bar)
        {
            _bar = bar;
            _bar.BeginInvoke(() =>
                {
                    _bar.MarqueeAnimationSpeed = 0;
                    _bar.Refresh();
                }
            );
        }

        /// <summary>
        /// 开启或关闭等待效果
        /// </summary>
        /// <param name="onOff">true（默认）=开启；false=仅关闭前次开启；null=彻底关闭</param>
        public void Run(bool? onOff = true)
        {
            if (onOff == true)
            {
                Count++;
                if (Count == 1)
                    try
                    {
                        _bar.BeginInvoke(() =>
                            {
                                _bar.MarqueeAnimationSpeed = 1;
                                _bar.Refresh();
                            }
                        );
                    }
                    catch
                    {
                        //
                    }
            }
            else
            {
                if (onOff == false)
                {
                    Count--;
                    if (Count < 0)
                        Count = 0;
                }
                else
                    Count = 0;
                try
                {
                    if (Count == 0)
                        _bar.BeginInvoke(() =>
                            {
                                _bar.MarqueeAnimationSpeed = 0;
                                _bar.Refresh();
                            }
                        );
                }
                catch
                {
                    //
                }
            }
        }
    }
}
