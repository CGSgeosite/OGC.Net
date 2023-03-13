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
    public partial class FreeTextFieldForm : Form
    {
        public string CoordinateFieldName;
        public bool? Ok;

        public FreeTextFieldForm(IEnumerable<string> fieldNames)
        {
            InitializeComponent();
            foreach (var name in fieldNames) 
                CoordinateComboBox.Items.Add(name);
            CoordinateFieldName =
                CoordinateComboBox.Text =
                    CoordinateComboBox.SelectedText =
                        $"{CoordinateComboBox.Items[0]}";
        }
        
        private void Yes_Click(object sender, EventArgs e)
        {
            CoordinateFieldName = CoordinateComboBox.Text;
            Ok = true;
            Close();
        }

        private void No_Click(object sender, EventArgs e)
        {
            CoordinateFieldName = null;
            Ok = false;
            Close();
        }
    }
}
