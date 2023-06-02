/******************************************************************************
 *
 * Name: OGC.net
 * Purpose: A free tool for reading ShapeFile, MapGIS, Excel/TXT/CSV, converting
 *          into GML, GeoJSON, ShapeFile, KML and GeositeXML, and pushing vector
 *          or raster to PostgreSQL database.
 *
 ******************************************************************************
 * (C) 2019-2023 Geosite Development Team of CGS (R)
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
    public class ToolStripSpringTextBox : ToolStripTextBox
    {
        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (IsOnOverflow || Owner.Orientation == Orientation.Vertical)
                return DefaultSize;

            var width = Owner.DisplayRectangle.Width;

            if (Owner.OverflowButton.Visible)
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;

            var springBoxCount = 0;

            foreach (ToolStripItem item in Owner.Items)
            {
                if (item.IsOnOverflow) 
                    continue;
                if (item is ToolStripSpringTextBox)
                {
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                    width = width - item.Width - item.Margin.Horizontal;
            }

            if (springBoxCount > 1) 
                width /= springBoxCount;

            if (width < DefaultSize.Width) 
                width = DefaultSize.Width;

            var size = base.GetPreferredSize(constrainingSize);
            size.Width = width;
            return size;
        }
    }
}
