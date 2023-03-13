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

using System.Xml.Linq;
using Newtonsoft.Json;

namespace Geosite
{
    public partial class MetaDataForm : Form
    {
        public bool Ok;
        public XElement MetaDataX;
        public bool DonotPrompt;
        

        public MetaDataForm(string metaDataString = null)
        {
            InitializeComponent();
            themeMetadata.Text = metaDataString ?? "";
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            string error = null;
            var themeMetadataText = themeMetadata.Text;
            if (themeMetadataText.Length > 0)
            {
                try
                {
                    MetaDataX = XElement.Parse(themeMetadataText);
                }
                catch(Exception xmlError)
                {
                    error = xmlError.Message;
                    try
                    {
                        var x = JsonConvert.DeserializeXNode(themeMetadataText, "property")?.ToString();
                        if (x != null)
                        {
                            MetaDataX = XElement.Parse(x);
                            error = null;
                        }
                    }
                    catch (Exception jsonError)
                    {
                        error = jsonError.Message;
                    }
                }
                if (MetaDataX != null)
                {
                    if (MetaDataX.Name != "property") 
                        MetaDataX = new XElement("property", MetaDataX);
                }
            }
            else 
                MetaDataX = null;

            if (error == null)
            {
                Ok = true;
                Close();
            }
            else
            {
                Info.Text = error;
                MetaDataX = null;
            }
        }

        private void ThemeMetadata_KeyPress(object sender, KeyPressEventArgs e)
        {
            //解决当TextBox控件在设置了MultiLine=True之后，Ctrl+A 无法全选的尴尬问题！
            if (e.KeyChar == '\x1')
                ((TextBox)sender).SelectAll();
        }

        private void DoNotPrompt_CheckedChanged(object sender, EventArgs e)
        {
            DonotPrompt = donotPrompt.Checked;
        }
    }
}
