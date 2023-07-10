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

using Geosite.GeositeServer.PostgreSQL;
using Geosite.GeositeServer.Vector;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Geosite
{
    /// <summary>
    /// 分类树处理类
    /// </summary>
    public class CatalogTree
    {
        /// <summary>
        /// 森林号（群编号）
        /// </summary>
        private readonly int _forest;

        /// <summary>
        /// 分类树控件对象
        /// </summary>
        private readonly TreeView _catalogTreeView;

        /// <summary>
        /// 根数据名称
        /// </summary>
        private readonly string _rootName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="catalogTreeView">分类树对象</param>
        /// <param name="forest">森林编号</param>
        /// <param name="rootName">根数据名称，默认：Root</param>
        public CatalogTree(TreeView catalogTreeView, int forest, string rootName = "Root")
        {
            _catalogTreeView = catalogTreeView;
            _forest = forest;
            _rootName= rootName;

            _catalogTreeView.Invoke(
                method: () =>
                {
                    var imageList = new ImageList();
                    imageList.Images.Add(Properties.Resources.house);       //0 - root 
                    imageList.Images.Add(Properties.Resources.point);       //1 - point
                    imageList.Images.Add(Properties.Resources.line);        //2 - line
                    imageList.Images.Add(Properties.Resources.polygon);     //3 - polygon
                    imageList.Images.Add(Properties.Resources.image);       //4 - image
                    imageList.Images.Add(Properties.Resources.wms);         //5 - wms 服务型
                    imageList.Images.Add(Properties.Resources.wmts);        //6 - Wmts 金字塔 tile
                    imageList.Images.Add(Properties.Resources.rastertile);  //7 - WPS 平铺 tile
                    imageList.Images.Add(Properties.Resources.guide);       //8 - table
                    imageList.Images.Add(Properties.Resources.hybrid);      //9 - hybrid
                    imageList.Images.Add(Properties.Resources.loading);     //10 - loading
                    _catalogTreeView.ImageList = imageList;
                    Reset();
                }
            );
        }

        /// <summary>
        /// 分类树重置
        /// </summary>
        public void Reset()
        {
            new Thread(
                start: () =>
                {
                    InsertNodes();
                }
            )
            {
                IsBackground = true
            }.Start();
        }

        /// <summary>
        /// 清除分类树所有节点
        /// </summary>
        public void Clear()
        {
            _catalogTreeView.Invoke(
                method: () =>
                {
                    _catalogTreeView.Nodes.Clear();
                }
            );
        }

        /// <summary>
        /// 逐层获取指定专题及其子级分类描述，工作原理类似于【Windows-资源管理器】
        /// </summary>
        /// <param name="typeName">专题层名称（省略时取默认值：星号），指定专题层名称时应从根节点开始逐级指定（可采用星号进行模糊匹配），逐级之间需采用[.]分隔</param>
        /// <returns>返回指定层级以及所属子级分类的摘要信息（包括专题层名称、所处层级、是否包含子级专题层）</returns>
        public XElement GetNodes(string typeName = null)
        {
            var layerArray = Regex.Split
                (
                    input: Regex.Replace
                    (
                        input: typeName?.Trim() ?? "*",
                        pattern: "&(amp;)+", replacement: "&",
                        options: RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Multiline),
                    pattern: @"\s*\.+\s*",
                    options: RegexOptions.Singleline | RegexOptions.Multiline
                )
                .Select(
                    selector: x =>
                    {
                        var xTrim = x.Trim();
                        return string.IsNullOrWhiteSpace(value: xTrim) ? "*" : xTrim;
                    }
                )
                .ToList();
            if (layerArray.Count == 0)
                layerArray.Add(item: "*");
            var level = layerArray.Count; 
            var withParameters = new Dictionary<string, object>();
            var layerName = layerArray[index: level - 1];
            var routeArray = new List<string>();
            for (var i = 1; i <= level; i++)
            {
                var name = layerArray[index: i - 1];
                if (name != "*" && name != "＊")
                    withParameters.Add(key: $"name{i}", value: name);
                routeArray.Add(item: $"level{i}.name");
            }
            var withHeader =
                $"SELECT level{level}.id, level{level}.tree, level{level}.level, level{level}.children, level{level}.property, level{level}.detail, level{level}.name, ARRAY[{string.Join(separator: ",", values: routeArray)}] AS layer" +
                " FROM" +
                " (" +
                "   SELECT outerlevel.*, CASE WHEN branch.parent IS null THEN 0 ELSE 1 END AS children FROM" +
                "   (" +
                "    SELECT branch.*, branch_relation.detail FROM branch LEFT JOIN branch_relation ON branch.id = branch_relation.branch" +
                $"    WHERE branch.level = {level}" + 
                (layerName != "*" && layerName != "＊" ? $" AND name ILIKE @name{level}::TEXT" : string.Empty) +
                "   ) AS outerlevel LEFT JOIN branch ON outerlevel.id = branch.parent" +
                $" ) AS level{level}";
            var afterSelect = string.Empty;
            var afterWhere = new List<string>();
            for (var i = level - 1; i > 0; i--)
            {
                layerName = layerArray[index: i - 1];
                afterSelect +=
                    ",(" +
                    $"SELECT * FROM branch WHERE level = {i}" +
                    (layerName != "*" && layerName != "＊" ? $" AND name ILIKE @name{i}::TEXT" : string.Empty) +
                    $") AS level{i}";
                afterWhere.Add(item: $"level{i + 1}.parent = level{i}.id");
            }

            if (!string.IsNullOrEmpty(value: afterSelect))
                afterSelect += " WHERE " + string.Join(separator: " AND ", values: afterWhere);
            var sql = "SET enable_seqscan = off;" +
                      $"WITH levels AS ({withHeader}{afterSelect})," +
                      "  themes AS" +
                      "   (" +
                      "       SELECT" +
                      "       levels.name AS name," +
                      "       count(tree.name) AS trees," + //被合并的专题文档树个数
                      "       first(tree.name) AS theme," +
                      "       first(tree.uri) AS uri," +
                      "       first(levels.level) AS level," +
                      "       ARRAY(SELECT unnest(array_agg(CASE WHEN array_length(tree.type, 1) > 0 THEN tree.type ELSE '{0}' END))) AS type," + //暂将推送异常的文档树类型标志数组强制为表格类型【{0}】
                      "       first(tree.status) AS status," +
                      "       first(tree.id) AS tree," +
                      "       sum(levels.children) AS children," +
                      "       first(levels.id) AS branch," +
                      "       first(levels.layer) AS layer," +
                      "       first(levels.property) AS property," +
                      "       first(levels.detail) AS detail" +
                      "       FROM tree, levels" +
                      $"      WHERE tree.forest = {_forest} AND tree.id = levels.tree" +
                      "       GROUP BY levels.name" +
                      "       ORDER BY levels.name" +
                      "   )" +
                      //  针对金字塔栅格瓦片类型，可利用此处返回的某专题层首个枝干对应的第一片叶子id提升效率，基于此因，约定某专题瓦片必须存储在一片叶子中
                      "   SELECT themes.*, firstleaf.leaf" +
                      "   FROM themes" +
                      "   LEFT JOIN LATERAL" +
                      "   (" +
                      "       SELECT branch, id AS leaf" +
                      "       FROM leaf" +
                      "       WHERE themes.branch = branch" +
                      "       LIMIT 1" +
                      "   ) AS firstleaf" +
                      "   ON TRUE;";
            var kvps = PostgreSqlHelper.DataTableReader(
                //"SET enable_seqscan = off; SET enable_bitmapscan = on; SET enable_indexscan = on;" +  
                cmd: sql,
                parameters: withParameters
            );
            if (kvps == null)
                throw new Exception(message: PostgreSqlHelper.ErrorMessage);
            var rows = kvps.Rows;
            var numberMatched = rows.Count;
            if (numberMatched == 0)
                throw new Exception(message: "Nothing was found.");
            var layersX = new XElement(
                name: "DescribeLayerResponse",
                new XAttribute(name: "version", value: "1.0.0"),
                new XAttribute(name: "forest", value: _forest),
                new XAttribute(name: "numberReturned", value: numberMatched),
                new XAttribute(name: "numberMatched", value: numberMatched)
            );
            foreach (DataRow kvpValue in rows)
            {
                var property = $"{kvpValue[columnName: "property"]}".Trim();
                var detail = $"{kvpValue[columnName: "detail"]}".Trim();
                var layer = string.Join(separator: ".", value: (string[])kvpValue[columnName: "layer"]);
                var name = $"{kvpValue[columnName: "name"]}";
                var type = string.Join(separator: ",", values: (int[])kvpValue[columnName: "type"]); 
                var owsType = new List<string>();
                foreach (
                    var theType in 
                    Regex.Split(
                        input: type, 
                        pattern: @"[,\s]+",
                             options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline
                        )
                    )
                {
                    if (int.TryParse(s: theType, result: out var typeCode))
                    {
                        switch (typeCode)
                        {
                            case >= 1 and <= 4:
                            {
                                if (!owsType.Contains(item: "WFS"))
                                    owsType.Add(item: "WFS");
                                break;
                            }
                            case >= 10000 and <= 10002:
                            {
                                if (!owsType.Contains(item: "WMS"))
                                    owsType.Add(item: "WMS");
                                break;
                            }
                            case >= 11000 and <= 11002:
                            {
                                if (!owsType.Contains(item: "WMTS"))
                                    owsType.Add(item: "WMTS");
                                break;
                            }
                            case >= 12000 and <= 12002 when !owsType.Contains(item: "WPS"):
                                owsType.Add(item: "WPS");
                                break;
                        }
                    }
                }
                layersX.Add(
                    content: new XElement(
                        name: "LayerDescription",
                        new XAttribute(name: "name", value: name),
                        new XAttribute(name: "layer", value: layer),
                        new XAttribute(name: "tree", value: kvpValue[columnName: "tree"]),
                        new XAttribute(name: "trees", value: kvpValue[columnName: "trees"]),
                        new XAttribute(name: "theme", value: kvpValue[columnName: "theme"]),
                        new XAttribute(name: "uri", value: kvpValue[columnName: "uri"]),
                        new XAttribute(name: "branch", value: kvpValue[columnName: "branch"]),
                        new XAttribute(name: "leaf", value: kvpValue[columnName: "leaf"]),
                        new XAttribute(name: "type", value: type),
                        new XAttribute(name: "level", value: kvpValue[columnName: "level"]),
                        new XAttribute(name: "children", value: kvpValue[columnName: "children"]),
                        new XAttribute(name: "status", value: kvpValue[columnName: "status"]),
                        new XElement(name: "typeName", content: name),
                        new XElement(
                            name: "owsType",
                            content: string.Join(separator: ",", values: owsType) //wfs wcf wms wps wmts
                        ),
                        string.IsNullOrWhiteSpace(value: property)
                            ? null
                            : GeositeXmlFormatting.JsonStringToXElement(
                                json: property,
                                rootName: "property"),
                        string.IsNullOrWhiteSpace(value: detail)
                            ? null
                            : new XElement(
                                name: "relation",
                                content: XElement.Parse(text: detail)
                            )
                    )
                );
            }
            return layersX;
        }

        /// <summary>
        /// 依据专题层名称在当前节点下面插入若干新节点
        /// </summary>
        /// <param name="currentNode">当前节点，默认：null=根节点</param>
        /// <param name="typeName">专题层名称，默认：null</param>
        public void InsertNodes(TreeNode currentNode = null, string typeName = null)
        {
            try
            {
                _catalogTreeView.Invoke(
                    method: () =>
                    {
                        if (currentNode == null)
                        {
                            _catalogTreeView.Nodes.Clear();
                            _catalogTreeView.Nodes.Add(currentNode = new TreeNode(_rootName)
                            {
                                ToolTipText = $"Forest - [{_forest}]",
                                ImageIndex = 0,
                                SelectedImageIndex = 0,
                                Tag = 1 //1=需要异步通讯获取子节点; 0=不再需要执行异步通讯
                            });
                        }
                        //先移除当前节点下的已有的全部子节点？
                        currentNode.Nodes.Clear();
                        //获取当前节点下欲新建的全部子节点信息
                        var describeLayerResponseX = GetNodes(typeName);
                        /*
                            <DescribeLayerResponse version="1.0.0" forest="-1" numberReturned="2" numberMatched="2">
                            </DescribeLayerResponse>           
                         */
                        var numberReturned = int.Parse(describeLayerResponseX.Attribute("numberReturned")?.Value ?? "0");
                        if (numberReturned > 0)
                        {
                            foreach (var layerDescription in describeLayerResponseX.Elements(name: "LayerDescription"))
                            {
                                var typeArray = Regex.Split(
                                    layerDescription.Attribute("type")?.Value ?? "0",
                                    @"[\s,]+",
                                    RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline
                                );
                                var typeIndex = typeArray.Length == 1
                                    ? typeArray[0] switch
                                    {
                                        "0" => 8,
                                        "1" => 1,
                                        "2" => 2,
                                        "3" => 3,
                                        "4" => 4,
                                        "10000" or
                                            "10001" or
                                            "10002" => 5,
                                        "11000" or
                                            "11001" or
                                            "11002" => 6,
                                        "12000" or
                                            "12001" or
                                            "12002" => 7,
                                        _ => 9
                                    }
                                    : 9;
                                /*
                                      <LayerDescription name="test" layer="test" teees="4" tree="1" theme="googlemap" uri="E:\test\googlemap" branch="1" leaf="" type="11002" level="1" children="1" status="5">
                                        <typeName>test</typeName>
                                        <owsType>WMTS</owsType>
                                      </LayerDescription>          
                                 */
                                var trees= int.Parse(layerDescription.Attribute("trees")?.Value ?? "0");
                                //本级节点下的各级嵌套子节点个数，只要不为0，就表明有子节点
                                var children = int.Parse(layerDescription.Attribute("children")?.Value ?? "0");
                                var theNode = new TreeNode(
                                    (
                                        layerDescription.Attribute("name")?.Value ?? "unknown"
                                    ) +
                                    (
                                        //为方便与数据表中的专题名称对照，特追加【theme】后缀
                                        children > 0
                                            ? ""
                                            : $" - [{layerDescription.Attribute("theme")?.Value}{(trees > 1 ? " ..." : "")}]"
                                    )
                                )
                                {
                                    ImageIndex = typeIndex,
                                    SelectedImageIndex = typeIndex,
                                    ToolTipText = $"Layer - [{layerDescription.Attribute("layer")?.Value}]\nTree - [{layerDescription.Attribute("tree")?.Value}]\nTrees - [{layerDescription.Attribute("trees")?.Value}]\nBranch - [{layerDescription.Attribute("branch")?.Value}]\nLeaf - [{layerDescription.Attribute("leaf")?.Value}]\nLevel - [{layerDescription.Attribute("level")?.Value}]\nStatus - [{layerDescription.Attribute("status")?.Value}]\nChildren - [{children}]\nUri - [{layerDescription.Attribute("uri")?.Value}]",
                                    Tag = children > 0 ? 1 : 0
                                };
                                if (children > 0)
                                    theNode.Nodes.Add(
                                        new TreeNode("loading ...")
                                        {
                                            ImageIndex = 10,
                                            SelectedImageIndex = 10
                                        }
                                    );
                                theNode.Collapse();
                                currentNode.Nodes.Add(theNode);
                            }
                            currentNode.Tag = 0;
                            currentNode.Expand();
                        }
                    }
                );
            }
            catch
            {
                //
            }
        }

        /// <summary>
        /// 分类树节点标签名称编辑更改，如果更新失败，抛出异常
        /// </summary>
        /// <param name="node">欲编辑的节点对象</param>
        /// <param name="label">新标签</param>
        public void EditNodeLabel(TreeNode node, string label)
        {
            var level = node.Level; 
            var toolTipText = node.ToolTipText.Split(separator: '\n');
            var layers = Regex.Split(
                Regex.Replace(
                    input: toolTipText[0],
                    pattern: @"^Layer[^\[]*?\[([\s\S]*)\]$",
                    replacement: "$1",
                    options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline
                ).Trim(),
                @"[\.]",
                RegexOptions.Singleline | RegexOptions.Multiline
            );
            if (layers.Length == level)
                _catalogTreeView.Invoke(
                    () =>
                    {
                        var updateInner = string.Empty;
                        var parameters = new Dictionary<string, object> { { "name", label } };
                        for (var i = 1; i <= level; i++)
                        {
                            updateInner =
                                $"(SELECT id FROM branch WHERE level={i} AND name=@name{i}::TEXT{(i == 1 ? "" : " AND parent IN " + updateInner)})";
                            parameters.Add($"name{i}", layers[i - 1]);
                        }

                        var result = PostgreSqlHelper.NonQuery(
                            cmd: $"UPDATE branch SET name=@name::TEXT WHERE id IN ({updateInner});",
                            parameters: parameters
                        );
                        if (result == null)
                            throw new Exception(PostgreSqlHelper.ErrorMessage);
                    }
                );
        }

        /// <summary>
        /// 分类树节点删除
        /// </summary>
        /// <param name="node">欲删除的节点对象</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int DeleteNode(TreeNode node)
        {
            var flag = 0;
            var level = node.Level;
            var toolTipText = node.ToolTipText.Split(separator: '\n');
            var layers = Regex.Split(
                Regex.Replace(
                    input: toolTipText[0],
                    pattern: @"^Layer[^\[]*?\[([\s\S]*)\]$",
                    replacement: "$1",
                    options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline
                ).Trim(),
                @"[\.]",
                RegexOptions.Singleline | RegexOptions.Multiline
            );
            if (layers.Length == level)
            {
                var updateInner = string.Empty;
                var parameters = new Dictionary<string, object>();
                for (var i = 1; i <= level; i++)
                {
                    updateInner = $"(SELECT id FROM branch WHERE level={i} AND name=@name{i}::TEXT{(i == 1 ? "" : " AND parent IN " + updateInner)})";
                    parameters.Add($"name{i}", layers[i - 1]);
                }
                //删除本节点及其全部子节点（枝干表、叶子表和挂接表）记录
                var sql = "WITH branchtable AS (" +
                          "    SELECT id FROM branch WHERE id IN (" +
                          "        SELECT id FROM branch, (" +
                          $"           SELECT tree, level FROM branch WHERE id IN ({updateInner})" +
                          "        ) AS branches" +
                          "        WHERE branch.tree = branches.tree AND branch.level >= branches.level" +
                          "    )" +
                          " )," +
                          " deleted AS (" +
                          "    DELETE FROM branch WHERE id IN (SELECT id FROM branchtable) RETURNING tree" +
                          " )" +
                          " SELECT DISTINCT tree FROM deleted;";
                var result = PostgreSqlHelper.DataTableReader(
                    cmd: sql,
                    parameters: parameters
                );
                if (result == null)
                    throw new Exception(PostgreSqlHelper.ErrorMessage);
                flag |= 1;
                var treeIds = string.Join(",", from DataRow row in result.Rows select row[0]);
                //继续判断树表内是否有记录，若无，也顺便删除树表对应的记录，也就是说，不允许出现没有枝干的树记录
                sql = @$"DELETE FROM tree WHERE id IN (SELECT * from UNNEST(ARRAY[{treeIds}]) AS tree EXCEPT (SELECT tree FROM branch WHERE tree IN ({treeIds})));";
                if (PostgreSqlHelper.NonQuery(cmd: sql) == null)
                    throw new Exception(PostgreSqlHelper.ErrorMessage);
                flag |= 2;
            }
            return flag; //3
        }
    }
}
