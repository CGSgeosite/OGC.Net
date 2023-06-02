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
    /// <summary>
    /// 文本输入窗体
    /// </summary>
    public sealed partial class InputForm : Form
    {
        /// <summary>
        /// 是否按下的是【Yes】按钮？
        /// 如果按下【Yes】就返回true
        /// 如果按下【No】就返回false
        /// 如果按下关闭窗体按钮返回null
        /// </summary>
        public bool? Yes;

        /// <summary>
        /// 结果文本
        /// </summary>
        public string Result;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prompt">提示文本</param>
        /// <param name="title">窗体标题文本，默认：Caution</param>
        /// <param name="defaultResponse">默认响应文本，默认：null</param>
        public InputForm(string prompt, string title = null, string defaultResponse = null)
        {
            InitializeComponent();
            tipTextBox.Text = prompt;
            Text = string.IsNullOrWhiteSpace(title) ? "Caution" : title;
            inputTextBox.Text = (Result = defaultResponse) ?? "";
        }

        private void Yes_Click(object sender, EventArgs e)
        {
            Yes = true;
            Result = inputTextBox.Text;
            Close();
        }

        private void No_Click(object sender, EventArgs e)
        {
            Yes = false;
            Close();
        }
    }
}
