/******************************************************************************
 *
 * Name: OGC.net - DatabaseGrid
 * Purpose: A free tool for reading ShapeFile, MapGIS, Excel/TXT/CSV, converting
 *          into GML, GeoJSON, ShapeFile, KML and GeositeXML, and pushing vector
 *          or raster to PostgreSQL database.
 *
 ******************************************************************************
 * (C) 2019-2024 Geosite Development Team of CGS (R)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to use, copy,
 * modify, and distribute the Software for any purpose, without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE GEOSITE DEVELOPMENT TEAM OF CGS
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using Geosite.GeositeServer.PostgreSQL;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Geosite
{
    /// <summary>
    /// 专题表处理类
    /// </summary>
    class DatabaseGrid
    {
        private readonly DataGridView _databaseGridView;
        private readonly Button _firstPage;
        private readonly Button _previousPage;
        private readonly Button _nextPage;
        private readonly Button _lastPage;
        private readonly TextBox _pageBox;
        private readonly Button _deleteTree;
        private readonly int _forest; //森林编号
        private int _totel; //总记录数
        private readonly int _limit; //每页多个条记录
        private int _page; //当前页码

        public DatabaseGrid(
            DataGridView dataGridView,
            Button firstPage, Button previousPage, Button nextPage, Button lastPage,
            TextBox pageBox,
            Button deleteTree,
            int forest,
            int page = -1,
            int limit = 100
            )
        {
            _databaseGridView = dataGridView;
            _firstPage = firstPage;
            _previousPage = previousPage;
            _nextPage = nextPage;
            _lastPage = lastPage;
            _pageBox = pageBox;
            _deleteTree = deleteTree;
            _forest = forest; // = 0; 
            _page = page;
            _limit = limit <= 0 ? 100 : limit;
            Reset();
        }

        public void Reset()
        {
            var treeCount = PostgreSqlHelper.Scalar(
                "SELECT COUNT(*) FROM tree WHERE forest = @forest;",
                new Dictionary<string, object>
                {
                    { "forest", _forest }
                }
            );
            int.TryParse($"{treeCount}", out _totel);
            Show(_page);
        }

        private void Show(int page = 0)
        {
            _page = page;
            new Task(
                () =>
                {
                    Clear();
                    var pages = (int)Math.Ceiling(1.0 * _totel / _limit);
                    if (pages == 0)
                        pages = 1;
                    if (_page >= pages)
                        _page = pages - 1;
                    else
                    {
                        if (_page < 0)
                            _page = 0;
                    }
                    _pageBox.Invoke(
                        () =>
                        {
                            _pageBox.Text = $@"{_page + 1} / {pages}";
                        }
                    );
                    var offset = _page * _limit;
                    var sql =
                        "WITH treeTable AS (" +
                        "    SELECT id AS tree, name, uri, timestamp, type, status FROM tree WHERE forest = @forest ORDER BY timestamp[3] DESC, timestamp[4] DESC OFFSET @offset LIMIT @limit" +
                        "), branchTable AS (" +
                        "    SELECT branch1.* FROM treeTable, LATERAL (" +
                        "        SELECT tree, id AS branch, property FROM branch WHERE tree = treeTable.tree ORDER BY level DESC LIMIT 1" +
                        "    ) AS branch1" +
                        "), layerTable AS (" +
                        "    SELECT layer1.*, treeTable.tree FROM treeTable, LATERAL (" +
                        "        SELECT array_to_string(array_agg(name), '.') AS layer FROM (SELECT distinct on(level) name FROM branch WHERE tree = treeTable.tree ORDER BY level,id) AS route" +
                        "    ) AS layer1" +
                        "), leafTable AS (" +
                        "    SELECT leaf1.*, branchTable.tree FROM branchTable, LATERAL (" +
                        // ----------------------------------------------------------------------------------- 如果未发现实体要素，特采用【UNION ALL】取默认值，以便正常返回完整的图层结构
                        "        (SELECT rank, id AS leaf FROM leaf WHERE branch = branchTable.branch limit 1) UNION ALL SELECT -1 AS rank, -1 AS leaf WHERE NOT EXISTS (SELECT 1 FROM leaf WHERE branch = branchTable.branch limit 1)" +
                        "    ) AS leaf1" +
                        ")" +
                        "SELECT treeTable.*, branchTable.branch, branchTable.property, layerTable.layer, leafTable.rank, leafTable.leaf " +
                        "FROM treeTable, branchTable, layerTable, leafTable " +
                        "WHERE treeTable.tree = branchTable.tree AND treeTable.tree = layerTable.tree AND treeTable.tree = leafTable.tree;";
                    var trees = PostgreSqlHelper.XElementReader(
                        sql,
                        new Dictionary<string, object>
                        {
                            { "forest", _forest },
                            { "offset", offset },
                            { "limit", _limit }
                        }
                    );
                    if (trees != null)
                    {
                        var rows = trees.Elements("row");
                        foreach (var row in rows)
                        {
                            var tree = row.Element("tree")?.Value;
                            var name = row.Element("name")?.Value.Trim();
                            var uri = row.Element("uri")?.Value;
                            var timestamp = string.Join(",", row.Elements("timestamp").Select(x => x.Value).ToArray());
                            var type = row.Element("type")?.Value ?? "0";
                            var status = row.Element("status")?.Value;
                            if (string.IsNullOrWhiteSpace(status))
                                status = "4";  //如果仅存在【森林、树和枝干】，未发现叶子的话，强行取【-1】级别
                            var branch = row.Element("branch")?.Value;
                            var layer = row.Element("layer")?.Value;
                            var rank = row.Element("rank")?.Value;
                            var leaf = row.Element("leaf")?.Value;
                            if (string.IsNullOrWhiteSpace(rank))
                                rank = "-1";  //如果仅存在【森林、树和枝干】，未发现叶子的话，强行取【-1】级别
                            // tree：
                            // name：文档树根节点简要名称，通常是入库文件基本名
                            // uri：文档树数据来源（存放路径及文件名）
                            // timestamp：文档树编码印章，采用[节点森林序号,文档树序号,年月日（yyyyMMdd）,时分秒（HHmmss）]四元整型数组编码方式
                            // type：文档树要素类型码
                            //      0：非空间数据【默认】
                            //      1：Point点、2：Line线、3：Polygon面、4：Image地理贴图
                            //      10000：Tile栅格  金字塔瓦片     wms服务类型     [epsg:0       无投影瓦片]
                            //      10001：Tile栅格  金字塔瓦片     wms服务类型     [epsg:4326    地理坐标系瓦片]
                            //      10002：Tile栅格  金字塔瓦片     wms服务类型     [epsg:3857    球体墨卡托瓦片]
                            //      11000：Tile栅格  金字塔瓦片     wmts服务类型    [epsg:0       无投影瓦片]
                            //      11001：Tile栅格  金字塔瓦片     wmts服务类型    [epsg:4326    地理坐标系瓦片]
                            //      11002：Tile栅格  金字塔瓦片     wmts服务类型    [epsg:3857    球体墨卡托瓦片]
                            //      12000：Tile栅格  平铺式瓦片     wps服务类型     [epsg:0       无投影瓦片]
                            //      12001：Tile栅格  平铺式瓦片     wps服务类型     [epsg:4326    地理坐标系瓦片]
                            //      12002：Tile栅格  平铺式瓦片     wps服务类型     [epsg:3857    球体墨卡托瓦片]
                            //status 森林状态码（介于0～7之间）,提示：文档树推送成功后，status为奇数, 约定如下：
                            //      持久化位 暗数据位 完整性位  |  值：含义
                            //      ======== ======== ========  |  ======================================
                            //      ----------------对等体-------
                            //      0        0        0         |  0=非持久化明数据（参与对等）无值或失败    
                            //      0        0        1         |  1=非持久化明数据（参与对等）正常          
                            //      0        1        0         |  2=非持久化暗数据（参与对等）失败          
                            //      0        1        1         |  3=非持久化暗数据（参与对等）正常
                            //      ----------------OGC.net------
                            //      1        0        0         |  4=持久化明数据（不参与对等）失败          
                            //      1        0        1         |  5=持久化明数据（不参与对等）正常          
                            //      1        1        0         |  6=持久化暗数据（不参与对等）失败          
                            //      1        1        1         |  7=持久化暗数据（不参与对等）正常 
                            var statusBitmap = status switch
                            {
                                "1" => Properties.Resources.lock001,
                                "2" => Properties.Resources.lock010,
                                "3" => Properties.Resources.lock011,
                                "4" => Properties.Resources.lock100,
                                "5" => Properties.Resources.lock101,
                                "6" => Properties.Resources.lock110,
                                "7" => Properties.Resources.lock111,
                                _ => Properties.Resources.lock000
                            };

                            var typeArray = Regex.Split(
                                type,
                                @"[\s,]+",
                                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline
                            );

                            var propertyX = row.Element("property");
                            propertyX?.Add(
                                new XElement(
                                    "type",
                                    string.Join(",", typeArray)
                                )
                            );

                            Bitmap typeBitmap;
                            if (typeArray.Length == 1)
                            {
                                typeBitmap = typeArray[0] switch
                                {
                                    "0" => Properties.Resources.guide,
                                    "1" => Properties.Resources.point,
                                    "2" => Properties.Resources.line,
                                    "3" => Properties.Resources.polygon,
                                    "4" => Properties.Resources.image,
                                    "10000" or
                                    "10001" or
                                    "10002" => Properties.Resources.wms,
                                    "11000" or
                                    "11001" or
                                    "11002" => Properties.Resources.wmts,
                                    "12000" or
                                    "12001" or
                                    "12002" => Properties.Resources.rastertile,
                                    _ => Properties.Resources.hybrid
                                };
                            }
                            else
                                typeBitmap = Properties.Resources.hybrid;

                            _databaseGridView.Invoke(
                                () =>
                                {
                                    var index = _databaseGridView.Rows.Add(name, rank, statusBitmap, typeBitmap);
                                    _databaseGridView.Rows[index].Cells[0].ToolTipText = uri;
                                    _databaseGridView.Rows[index].Cells[1].ToolTipText = $"{tree}\b{type}\b{layer}\b{branch}\b{leaf}\b{status}\b{timestamp}";
                                    _databaseGridView.Rows[index].Cells[1].Tag = propertyX;
                                }
                            );
                        }
                    }
                    _databaseGridView.Invoke(
                        () =>
                        {
                            _deleteTree.Enabled = _databaseGridView?.RowCount > 0;
                        }
                    );
                    if (pages == 1)
                        _databaseGridView.Invoke(
                            () =>
                            {
                                _firstPage.Enabled =
                                    _previousPage.Enabled =
                                        _nextPage.Enabled =
                                            _lastPage.Enabled = false;
                            }
                        );
                    else
                    {
                        if (_page == 0)
                            _databaseGridView.Invoke(
                                () =>
                                {
                                    _firstPage.Enabled =
                                        _previousPage.Enabled = false;
                                    _nextPage.Enabled =
                                        _lastPage.Enabled = true;
                                }
                            );
                        else
                        {
                            _databaseGridView.Invoke(
                                () =>
                                {
                                    if (_page == pages - 1)
                                    {
                                        _firstPage.Enabled =
                                            _previousPage.Enabled = true;
                                        _nextPage.Enabled =
                                            _lastPage.Enabled = false;
                                    }
                                    else
                                    {
                                        _firstPage.Enabled =
                                            _previousPage.Enabled =
                                                _nextPage.Enabled =
                                                    _lastPage.Enabled = true;
                                    }
                                }
                            );
                        }
                    }
                }
            ).Start();
        }

        public void Clear()
        {
            _databaseGridView.Invoke(
                () =>
                {
                    _databaseGridView?.Rows.Clear();
                }
            );
        }

        public void First()
        {
            if (_page != 0)
                Show();
        }

        public void Previous()
        {
            if (_page > 0)
                Show(_page - 1);
        }

        public void Next()
        {
            var pages = (int)Math.Ceiling(1.0 * _totel / _limit);
            if (_page < pages - 1)
                Show(_page + 1);
        }

        public void Last()
        {
            var pages = (int)Math.Ceiling(1.0 * _totel / _limit);
            if (_page != pages - 1)
                Show(pages - 1);
        }
    }
}
