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

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Geosite.Messager;

namespace Geosite
{
    public partial class LayersBuilderForm : Form
    {
        public string TreePathString;

        public XElement Description;

        public bool Ok;

        public bool DonotPrompt;

        public LayersBuilderForm(string treePathDefault = null)
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(treePathDefault))
            {
                treePathTab.TabPages[0].Enabled = false;
                treePathTab.SelectedIndex = 1;
            }
            else
            {
                treePathTab.SelectedIndex = 0;
                //尽可能从文件夹或文件路径中提取分类树
                treePathBox.Text = ConsoleIO.FilePathToXPath(treePathDefault);
                treePathBox.Focus();
            }
        }
        
        private void OKbutton_Click(object sender, EventArgs e)
        {
            var canExit = true;

            TreePathString = treePathBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(TreePathString))
            {
                var layers = new List<string>();
                foreach (var layer in Regex.Split(
                        TreePathString,
                        @"[/\\]+", //约定为正斜杠【/】或者反斜杠【\】分隔，尤其不能出现小数点【.】，因为在后期服务中，小数点有特殊含义！
                        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline)
                    .Select(layer => layer.Trim())
                    .Where(layer => layer.Length > 0)
                )
                {
                    try
                    {
                        //GML-layer名称需符合xml元素命名规则，至少不能出现特殊字符、括号、纯数字 ...,也不允许出现小数点，因为有特殊含义
                        var xmlNodeName = new XElement(layer);
                        if (xmlNodeName.Name.LocalName != layer || Regex.IsMatch(layer, @"[\.]+", RegexOptions.IgnoreCase))
                        {
                            throw new Exception($"[{layer}] does not conform to XML naming rules");
                        }
                        layers.Add(new XElement(layer).Name.LocalName);
                    }
                    catch (Exception error)
                    {
                        canExit = false;
                        tipsBox.Text = error.Message;
                        break;
                    }
                }

                if (layers.Count == 0)
                {
                    canExit = false;
                    tipsBox.Text = @"Incorrect input";
                }
                else
                {
                    TreePathString = string.Join("/", layers);
                }
            }

            if (!string.IsNullOrWhiteSpace(downloadBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("download", downloadBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(legendBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("legend", legendBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(thumbnailBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("thumbnail", thumbnailBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(authorBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("author", authorBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(contactBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("contact", contactBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(keywordBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("keyword", keywordBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(abstractBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("abstract", abstractBox.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(remarksBox.Text))
            {
                Description ??= new XElement("property");
                Description.Add(new XElement("remarks", remarksBox.Text.Trim()));
            }

            if (canExit)
            {
                Ok = true;
                Close();
            } else 
                Ok = false;
        }

        private void DoNotPrompt_CheckedChanged(object sender, EventArgs e)
        {
            DonotPrompt = donotPrompt.Checked;
        }

        private void TreePathBox_TextChanged(object sender, EventArgs e)
        {
            tipsBox.Text = string.Empty;
        }
    }
}
