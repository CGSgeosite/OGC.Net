namespace Geosite
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            OGCtoolTip = new ToolTip(components);
            deleteTree = new Button();
            lastPage = new Button();
            nextPage = new Button();
            previousPage = new Button();
            firstPage = new Button();
            pagesBox = new TextBox();
            GeositeServerUrl = new TextBox();
            wmtsSouth = new TextBox();
            wmtsWest = new TextBox();
            wmtsEast = new TextBox();
            wmtsNorth = new TextBox();
            rasterTileSize = new TextBox();
            wmtsSize = new TextBox();
            deleteForest = new Button();
            FormatRaster = new RadioButton();
            FormatStandard = new RadioButton();
            FormatDeepZoom = new RadioButton();
            FormatTMS = new RadioButton();
            FormatArcGIS = new RadioButton();
            FormatMapcruncher = new RadioButton();
            label6 = new Label();
            label7 = new Label();
            GeositeServerUser = new TextBox();
            GeositeServerPassword = new TextBox();
            ModelSave = new Button();
            ModelOpen = new Button();
            tileconvert = new Button();
            MIMEBox = new ComboBox();
            rankList = new ComboBox();
            groupBox7 = new GroupBox();
            PostgresLight = new CheckBox();
            UpdateBox = new CheckBox();
            EPSG4326 = new CheckBox();
            wmtsSpider = new CheckBox();
            pictureBox7 = new PictureBox();
            pictureBox5 = new PictureBox();
            VectorFileClear = new Button();
            VectorOpen = new Button();
            Reindex = new Button();
            ReClean = new Button();
            GeositeServerLink = new Button();
            pictureBox4 = new PictureBox();
            DatabaseLogIcon = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            PostgresRun = new Button();
            localTileOpen = new Button();
            TileFormatSave = new Button();
            TileFormatOpen = new Button();
            pictureBox6 = new PictureBox();
            pictureBox10 = new PictureBox();
            pictureBox13 = new PictureBox();
            pictureBox15 = new PictureBox();
            pictureBox16 = new PictureBox();
            FileLoadLogIcon = new PictureBox();
            FileLoadLog = new ListBox();
            DatabaseLog = new ListBox();
            tilewebapi = new TextBox();
            FileSaveGroupBox = new GroupBox();
            toolStrip4 = new ToolStrip();
            vectorSaveButton = new ToolStripButton();
            toolStripTextBox2 = new ToolStripTextBox();
            FileRunButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            SaveAsFormat = new ToolStripComboBox();
            vectorTargetFile = new ToolStripSpringTextBox();
            groupBox4 = new GroupBox();
            panel44 = new Panel();
            FileSplitContainer = new SplitContainer();
            panel45 = new Panel();
            FileTabControl = new TabControl();
            FileTabPage = new TabPage();
            tableLayoutPanel11 = new TableLayoutPanel();
            FilePreviewProgressBar = new ProgressBar();
            panel3 = new Panel();
            FileGridView = new DataGridView();
            FilePath = new DataGridViewTextBoxColumn();
            FilePreview = new DataGridViewButtonColumn();
            panel46 = new Panel();
            PreviewTabControl = new TabControl();
            MapTabPage = new TabPage();
            tableLayoutPanel12 = new TableLayoutPanel();
            FileMapSplitContainer = new SplitContainer();
            panel12 = new Panel();
            panel21 = new Panel();
            panel18 = new Panel();
            toolStrip1 = new ToolStrip();
            MapProviderDropDown = new ToolStripDropDownButton();
            ZoomLevelLabel = new ToolStripLabel();
            toolStripSeparator8 = new ToolStripSeparator();
            ZoomToLayer = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            ClearLayers = new ToolStripButton();
            ImageMaker = new ToolStripButton();
            toolStripSeparator7 = new ToolStripSeparator();
            PositionBox = new ToolStripDropDownButton();
            DegMenuItem = new ToolStripMenuItem();
            DmsMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            BeijingMenuItem = new ToolStripMenuItem();
            XianMenuItem = new ToolStripMenuItem();
            CGCS2000MenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            WebMercatorMenuItem = new ToolStripMenuItem();
            gMapPanel = new Panel();
            MapBox = new GMap.NET.WindowsForms.GMapControl();
            propertyPanel = new Panel();
            MapBoxProperty = new RichTextBox();
            panel22 = new Panel();
            label8 = new Label();
            pictureBox8 = new PictureBox();
            TileLoadProgressBar = new ProgressBar();
            fileToolStrip = new ToolStrip();
            vectorOpenButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            mapgisButton = new ToolStripButton();
            arcgisIconButton = new ToolStripButton();
            tableIconButton = new ToolStripButton();
            gmlIconButton = new ToolStripButton();
            geojsonIconButton = new ToolStripButton();
            geositeIconButton = new ToolStripButton();
            kmlIconButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            cleanIconButton = new ToolStripButton();
            fileWorker = new System.ComponentModel.BackgroundWorker();
            box = new Panel();
            panel1 = new Panel();
            panel20 = new Panel();
            panel23 = new Panel();
            ogcCard = new TabControl();
            fileCard = new TabPage();
            databaseCard = new TabPage();
            PostgresLinkSplitContainer = new SplitContainer();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel25 = new Panel();
            panel24 = new Panel();
            panel9 = new Panel();
            panel13 = new Panel();
            DatabaseTabControl = new TabControl();
            DatabasePage = new TabPage();
            dataGridPanel = new Panel();
            tableLayoutPanel2 = new TableLayoutPanel();
            panel31 = new Panel();
            panel30 = new Panel();
            panel29 = new Panel();
            panel28 = new Panel();
            panel26 = new Panel();
            panel27 = new Panel();
            panel2 = new Panel();
            DatabaseGridView = new DataGridView();
            ThemeName = new DataGridViewTextBoxColumn();
            ThemeRank = new DataGridViewTextBoxColumn();
            ThemeStatus = new DataGridViewImageColumn();
            ThemeType = new DataGridViewImageColumn();
            DatabaseProgressBar = new ProgressBar();
            CatalogPage = new TabPage();
            CatalogTreeView = new TreeView();
            groupBox9 = new GroupBox();
            panel7 = new Panel();
            dataCards = new TabControl();
            RasterPage = new TabPage();
            PushPanel = new Panel();
            groupBox1 = new GroupBox();
            themeNameBox = new TextBox();
            method = new GroupBox();
            label1 = new Label();
            tileLevels = new ComboBox();
            tilesource = new TabControl();
            LocalTilePage = new TabPage();
            FormatGroupBox = new GroupBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            panel32 = new Panel();
            panel33 = new Panel();
            panel34 = new Panel();
            panel35 = new Panel();
            panel36 = new Panel();
            panel37 = new Panel();
            FormatStandardBox = new RichTextBox();
            FormatTMSBox = new RichTextBox();
            FormatMapcruncherBox = new RichTextBox();
            FormatArcGISBox = new RichTextBox();
            FormatDeepZoomBox = new RichTextBox();
            FormatRasterBox = new RichTextBox();
            localTileFolder = new TextBox();
            RemoteTilePage = new TabPage();
            tableLayoutPanel10 = new TableLayoutPanel();
            wmtsTipBox = new RichTextBox();
            panel42 = new Panel();
            groupBox5 = new GroupBox();
            tableLayoutPanel6 = new TableLayoutPanel();
            panel38 = new Panel();
            panel39 = new Panel();
            panel40 = new Panel();
            panel41 = new Panel();
            subdomainsBox = new TextBox();
            label5 = new Label();
            wmtsMaxZoom = new ComboBox();
            wmtsMinZoom = new ComboBox();
            label4 = new Label();
            ModelPage = new TabPage();
            tableLayoutPanel9 = new TableLayoutPanel();
            modelTipBox = new RichTextBox();
            tableLayoutPanel7 = new TableLayoutPanel();
            groupBox3 = new GroupBox();
            groupBox2 = new GroupBox();
            nodatabox = new TextBox();
            panel8 = new Panel();
            ModelOpenTextBox = new TextBox();
            TileConvertPage = new TabPage();
            panel16 = new Panel();
            panel15 = new Panel();
            tableLayoutPanel8 = new TableLayoutPanel();
            convertTipBox = new RichTextBox();
            panel14 = new Panel();
            panel6 = new Panel();
            maptilertoogc = new RadioButton();
            mapcrunchertoogc = new RadioButton();
            ogctomapcruncher = new RadioButton();
            ogctomaptiler = new RadioButton();
            TileFormatSaveBox = new TextBox();
            TileFormatOpenBox = new TextBox();
            pictureBox9 = new PictureBox();
            VectorPage = new TabPage();
            panel4 = new Panel();
            vectorFilePool = new DataGridView();
            VectorTheme = new DataGridViewTextBoxColumn();
            VectorURI = new DataGridViewTextBoxColumn();
            VectorStatus = new DataGridViewTextBoxColumn();
            helpCard = new TabPage();
            tabControl2 = new TabControl();
            tabPage3 = new TabPage();
            panel17 = new Panel();
            readmeTextBox = new RichTextBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            richTextBox1 = new RichTextBox();
            panel10 = new Panel();
            pictureBox11 = new PictureBox();
            tabPage4 = new TabPage();
            panel43 = new Panel();
            apiTextBox = new RichTextBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            richTextBox2 = new RichTextBox();
            panel11 = new Panel();
            pictureBox12 = new PictureBox();
            vectorWorker = new System.ComponentModel.BackgroundWorker();
            rasterWorker = new System.ComponentModel.BackgroundWorker();
            statusBar = new StatusStrip();
            statusProgress = new ToolStripProgressBar();
            statusText = new ToolStripStatusLabel();
            dataGridViewImageColumn1 = new DataGridViewImageColumn();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DatabaseLogIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox13).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox15).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FileLoadLogIcon).BeginInit();
            FileSaveGroupBox.SuspendLayout();
            toolStrip4.SuspendLayout();
            groupBox4.SuspendLayout();
            panel44.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FileSplitContainer).BeginInit();
            FileSplitContainer.Panel1.SuspendLayout();
            FileSplitContainer.Panel2.SuspendLayout();
            FileSplitContainer.SuspendLayout();
            panel45.SuspendLayout();
            FileTabControl.SuspendLayout();
            FileTabPage.SuspendLayout();
            tableLayoutPanel11.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FileGridView).BeginInit();
            panel46.SuspendLayout();
            PreviewTabControl.SuspendLayout();
            MapTabPage.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FileMapSplitContainer).BeginInit();
            FileMapSplitContainer.Panel1.SuspendLayout();
            FileMapSplitContainer.Panel2.SuspendLayout();
            FileMapSplitContainer.SuspendLayout();
            panel12.SuspendLayout();
            panel18.SuspendLayout();
            toolStrip1.SuspendLayout();
            gMapPanel.SuspendLayout();
            propertyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
            fileToolStrip.SuspendLayout();
            box.SuspendLayout();
            panel1.SuspendLayout();
            panel20.SuspendLayout();
            ogcCard.SuspendLayout();
            fileCard.SuspendLayout();
            databaseCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PostgresLinkSplitContainer).BeginInit();
            PostgresLinkSplitContainer.Panel1.SuspendLayout();
            PostgresLinkSplitContainer.Panel2.SuspendLayout();
            PostgresLinkSplitContainer.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel25.SuspendLayout();
            panel24.SuspendLayout();
            DatabaseTabControl.SuspendLayout();
            DatabasePage.SuspendLayout();
            dataGridPanel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            panel31.SuspendLayout();
            panel30.SuspendLayout();
            panel29.SuspendLayout();
            panel28.SuspendLayout();
            panel26.SuspendLayout();
            panel27.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DatabaseGridView).BeginInit();
            CatalogPage.SuspendLayout();
            groupBox9.SuspendLayout();
            dataCards.SuspendLayout();
            RasterPage.SuspendLayout();
            PushPanel.SuspendLayout();
            groupBox1.SuspendLayout();
            method.SuspendLayout();
            tilesource.SuspendLayout();
            LocalTilePage.SuspendLayout();
            FormatGroupBox.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            panel32.SuspendLayout();
            panel33.SuspendLayout();
            panel34.SuspendLayout();
            panel35.SuspendLayout();
            panel36.SuspendLayout();
            panel37.SuspendLayout();
            RemoteTilePage.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            groupBox5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            panel38.SuspendLayout();
            panel39.SuspendLayout();
            panel40.SuspendLayout();
            panel41.SuspendLayout();
            ModelPage.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            TileConvertPage.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox9).BeginInit();
            VectorPage.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)vectorFilePool).BeginInit();
            helpCard.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage3.SuspendLayout();
            panel17.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
            tabPage4.SuspendLayout();
            panel43.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox12).BeginInit();
            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // deleteTree
            // 
            deleteTree.BackColor = Color.White;
            deleteTree.Cursor = Cursors.Hand;
            deleteTree.Dock = DockStyle.Fill;
            deleteTree.Enabled = false;
            deleteTree.Font = new Font("宋体", 9F, FontStyle.Bold, GraphicsUnit.Point);
            deleteTree.ForeColor = Color.Black;
            deleteTree.Location = new Point(0, 0);
            deleteTree.Margin = new Padding(4);
            deleteTree.Name = "deleteTree";
            deleteTree.Size = new Size(54, 37);
            deleteTree.TabIndex = 3;
            deleteTree.Text = "×";
            OGCtoolTip.SetToolTip(deleteTree, "Delete selected themes");
            deleteTree.UseVisualStyleBackColor = false;
            deleteTree.Click += DeleteTree_Click;
            // 
            // lastPage
            // 
            lastPage.BackColor = Color.White;
            lastPage.Cursor = Cursors.Hand;
            lastPage.Dock = DockStyle.Fill;
            lastPage.Enabled = false;
            lastPage.Font = new Font("宋体", 8F, FontStyle.Bold, GraphicsUnit.Point);
            lastPage.Location = new Point(0, 0);
            lastPage.Margin = new Padding(4);
            lastPage.Name = "lastPage";
            lastPage.Size = new Size(54, 37);
            lastPage.TabIndex = 3;
            lastPage.Text = ">|";
            OGCtoolTip.SetToolTip(lastPage, "Last Page");
            lastPage.UseVisualStyleBackColor = false;
            lastPage.Click += LastPage_Click;
            // 
            // nextPage
            // 
            nextPage.BackColor = Color.White;
            nextPage.Cursor = Cursors.Hand;
            nextPage.Dock = DockStyle.Fill;
            nextPage.Enabled = false;
            nextPage.Font = new Font("宋体", 8F, FontStyle.Bold, GraphicsUnit.Point);
            nextPage.Location = new Point(0, 0);
            nextPage.Margin = new Padding(4);
            nextPage.Name = "nextPage";
            nextPage.Size = new Size(54, 37);
            nextPage.TabIndex = 3;
            nextPage.Text = ">";
            OGCtoolTip.SetToolTip(nextPage, "Next Page");
            nextPage.UseVisualStyleBackColor = false;
            nextPage.Click += NextPage_Click;
            // 
            // previousPage
            // 
            previousPage.BackColor = Color.White;
            previousPage.Cursor = Cursors.Hand;
            previousPage.Dock = DockStyle.Fill;
            previousPage.Enabled = false;
            previousPage.Font = new Font("宋体", 8F, FontStyle.Bold, GraphicsUnit.Point);
            previousPage.Location = new Point(0, 0);
            previousPage.Margin = new Padding(4);
            previousPage.Name = "previousPage";
            previousPage.Size = new Size(54, 37);
            previousPage.TabIndex = 3;
            previousPage.Text = "<";
            OGCtoolTip.SetToolTip(previousPage, "Previous Page");
            previousPage.UseVisualStyleBackColor = false;
            previousPage.Click += PreviousPage_Click;
            // 
            // firstPage
            // 
            firstPage.BackColor = Color.White;
            firstPage.Cursor = Cursors.Hand;
            firstPage.Dock = DockStyle.Fill;
            firstPage.Enabled = false;
            firstPage.Font = new Font("宋体", 8F, FontStyle.Bold, GraphicsUnit.Point);
            firstPage.Location = new Point(0, 0);
            firstPage.Margin = new Padding(4);
            firstPage.Name = "firstPage";
            firstPage.Size = new Size(54, 37);
            firstPage.TabIndex = 3;
            firstPage.Text = "|<";
            OGCtoolTip.SetToolTip(firstPage, "First Page");
            firstPage.UseVisualStyleBackColor = false;
            firstPage.Click += FirstPage_Click;
            // 
            // pagesBox
            // 
            pagesBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pagesBox.BackColor = Color.WhiteSmoke;
            pagesBox.Enabled = false;
            pagesBox.Location = new Point(0, 6);
            pagesBox.Margin = new Padding(4);
            pagesBox.Name = "pagesBox";
            pagesBox.ReadOnly = true;
            pagesBox.Size = new Size(264, 23);
            pagesBox.TabIndex = 7;
            pagesBox.TextAlign = HorizontalAlignment.Center;
            OGCtoolTip.SetToolTip(pagesBox, "10 entries per page");
            // 
            // GeositeServerUrl
            // 
            GeositeServerUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GeositeServerUrl.Location = new Point(33, 7);
            GeositeServerUrl.Margin = new Padding(4);
            GeositeServerUrl.Name = "GeositeServerUrl";
            GeositeServerUrl.PlaceholderText = "e.g. http://localhost:5000";
            GeositeServerUrl.Size = new Size(225, 23);
            GeositeServerUrl.TabIndex = 1;
            OGCtoolTip.SetToolTip(GeositeServerUrl, "GeositeServer Url");
            GeositeServerUrl.TextChanged += GeositeServer_LinkChanged;
            // 
            // wmtsSouth
            // 
            wmtsSouth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            wmtsSouth.Location = new Point(26, 2);
            wmtsSouth.Margin = new Padding(4);
            wmtsSouth.Name = "wmtsSouth";
            wmtsSouth.Size = new Size(180, 23);
            wmtsSouth.TabIndex = 34;
            wmtsSouth.Text = "-90";
            wmtsSouth.TextAlign = HorizontalAlignment.Center;
            OGCtoolTip.SetToolTip(wmtsSouth, "South");
            wmtsSouth.TextChanged += FormEventChanged;
            // 
            // wmtsWest
            // 
            wmtsWest.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            wmtsWest.Location = new Point(26, 3);
            wmtsWest.Margin = new Padding(4);
            wmtsWest.Name = "wmtsWest";
            wmtsWest.Size = new Size(179, 23);
            wmtsWest.TabIndex = 32;
            wmtsWest.Text = "-180";
            wmtsWest.TextAlign = HorizontalAlignment.Center;
            OGCtoolTip.SetToolTip(wmtsWest, "West");
            wmtsWest.TextChanged += FormEventChanged;
            // 
            // wmtsEast
            // 
            wmtsEast.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            wmtsEast.Location = new Point(26, 3);
            wmtsEast.Margin = new Padding(4);
            wmtsEast.Name = "wmtsEast";
            wmtsEast.Size = new Size(179, 23);
            wmtsEast.TabIndex = 33;
            wmtsEast.Text = "180";
            wmtsEast.TextAlign = HorizontalAlignment.Center;
            OGCtoolTip.SetToolTip(wmtsEast, "East");
            wmtsEast.TextChanged += FormEventChanged;
            // 
            // wmtsNorth
            // 
            wmtsNorth.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            wmtsNorth.Location = new Point(26, 3);
            wmtsNorth.Margin = new Padding(4);
            wmtsNorth.Name = "wmtsNorth";
            wmtsNorth.Size = new Size(180, 23);
            wmtsNorth.TabIndex = 31;
            wmtsNorth.Text = "90";
            wmtsNorth.TextAlign = HorizontalAlignment.Center;
            OGCtoolTip.SetToolTip(wmtsNorth, "North");
            wmtsNorth.TextChanged += FormEventChanged;
            // 
            // rasterTileSize
            // 
            rasterTileSize.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rasterTileSize.BackColor = Color.WhiteSmoke;
            rasterTileSize.BorderStyle = BorderStyle.None;
            rasterTileSize.Location = new Point(9, 17);
            rasterTileSize.Margin = new Padding(4);
            rasterTileSize.Name = "rasterTileSize";
            rasterTileSize.Size = new Size(312, 16);
            rasterTileSize.TabIndex = 20;
            rasterTileSize.Text = "100";
            OGCtoolTip.SetToolTip(rasterTileSize, "width");
            rasterTileSize.TextChanged += RasterTileSize_TextChanged;
            // 
            // wmtsSize
            // 
            wmtsSize.BackColor = Color.WhiteSmoke;
            wmtsSize.Enabled = false;
            wmtsSize.Location = new Point(142, 87);
            wmtsSize.Margin = new Padding(4);
            wmtsSize.Name = "wmtsSize";
            wmtsSize.ReadOnly = true;
            wmtsSize.Size = new Size(68, 23);
            wmtsSize.TabIndex = 36;
            wmtsSize.Text = "256";
            OGCtoolTip.SetToolTip(wmtsSize, "Size");
            // 
            // deleteForest
            // 
            deleteForest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            deleteForest.BackColor = Color.White;
            deleteForest.Cursor = Cursors.Hand;
            deleteForest.Enabled = false;
            deleteForest.Font = new Font("宋体", 9F, FontStyle.Bold, GraphicsUnit.Point);
            deleteForest.ForeColor = Color.Black;
            deleteForest.Location = new Point(225, 38);
            deleteForest.Margin = new Padding(4);
            deleteForest.Name = "deleteForest";
            deleteForest.Size = new Size(33, 54);
            deleteForest.TabIndex = 20;
            deleteForest.Text = "×";
            OGCtoolTip.SetToolTip(deleteForest, "Delete user data");
            deleteForest.UseVisualStyleBackColor = false;
            deleteForest.Click += DeleteForest_Click;
            // 
            // FormatRaster
            // 
            FormatRaster.AutoSize = true;
            FormatRaster.Cursor = Cursors.Hand;
            FormatRaster.Dock = DockStyle.Fill;
            FormatRaster.Location = new Point(0, 0);
            FormatRaster.Margin = new Padding(4);
            FormatRaster.Name = "FormatRaster";
            FormatRaster.Size = new Size(126, 26);
            FormatRaster.TabIndex = 11;
            FormatRaster.Text = "Raster";
            OGCtoolTip.SetToolTip(FormatRaster, "EPSG:0");
            FormatRaster.UseVisualStyleBackColor = true;
            FormatRaster.CheckedChanged += TileFormat_CheckedChanged;
            // 
            // FormatStandard
            // 
            FormatStandard.AutoSize = true;
            FormatStandard.Checked = true;
            FormatStandard.Cursor = Cursors.Hand;
            FormatStandard.Dock = DockStyle.Fill;
            FormatStandard.Location = new Point(0, 0);
            FormatStandard.Margin = new Padding(4);
            FormatStandard.Name = "FormatStandard";
            FormatStandard.Size = new Size(159, 26);
            FormatStandard.TabIndex = 6;
            FormatStandard.TabStop = true;
            FormatStandard.Text = "Standard";
            OGCtoolTip.SetToolTip(FormatStandard, "EPSG:3857/4326");
            FormatStandard.UseVisualStyleBackColor = true;
            FormatStandard.CheckedChanged += TileFormat_CheckedChanged;
            // 
            // FormatDeepZoom
            // 
            FormatDeepZoom.AutoSize = true;
            FormatDeepZoom.Cursor = Cursors.Hand;
            FormatDeepZoom.Dock = DockStyle.Fill;
            FormatDeepZoom.Location = new Point(0, 0);
            FormatDeepZoom.Margin = new Padding(4);
            FormatDeepZoom.Name = "FormatDeepZoom";
            FormatDeepZoom.Size = new Size(126, 26);
            FormatDeepZoom.TabIndex = 10;
            FormatDeepZoom.Text = "DeepZoom";
            OGCtoolTip.SetToolTip(FormatDeepZoom, "EPSG:0");
            FormatDeepZoom.UseVisualStyleBackColor = true;
            FormatDeepZoom.CheckedChanged += TileFormat_CheckedChanged;
            // 
            // FormatTMS
            // 
            FormatTMS.AutoSize = true;
            FormatTMS.Cursor = Cursors.Hand;
            FormatTMS.Dock = DockStyle.Fill;
            FormatTMS.Location = new Point(0, 0);
            FormatTMS.Margin = new Padding(4);
            FormatTMS.Name = "FormatTMS";
            FormatTMS.Size = new Size(126, 26);
            FormatTMS.TabIndex = 7;
            FormatTMS.Text = "TMS";
            OGCtoolTip.SetToolTip(FormatTMS, "EPSG:3857\r\nY from large to small");
            FormatTMS.UseVisualStyleBackColor = true;
            FormatTMS.CheckedChanged += TileFormat_CheckedChanged;
            // 
            // FormatArcGIS
            // 
            FormatArcGIS.AutoSize = true;
            FormatArcGIS.Cursor = Cursors.Hand;
            FormatArcGIS.Dock = DockStyle.Fill;
            FormatArcGIS.Location = new Point(0, 0);
            FormatArcGIS.Margin = new Padding(4);
            FormatArcGIS.Name = "FormatArcGIS";
            FormatArcGIS.Size = new Size(126, 26);
            FormatArcGIS.TabIndex = 9;
            FormatArcGIS.Text = "ARCGIS";
            OGCtoolTip.SetToolTip(FormatArcGIS, "EPSG:3857");
            FormatArcGIS.UseVisualStyleBackColor = true;
            FormatArcGIS.CheckedChanged += TileFormat_CheckedChanged;
            // 
            // FormatMapcruncher
            // 
            FormatMapcruncher.AutoSize = true;
            FormatMapcruncher.Cursor = Cursors.Hand;
            FormatMapcruncher.Dock = DockStyle.Fill;
            FormatMapcruncher.Location = new Point(0, 0);
            FormatMapcruncher.Margin = new Padding(4);
            FormatMapcruncher.Name = "FormatMapcruncher";
            FormatMapcruncher.Size = new Size(126, 26);
            FormatMapcruncher.TabIndex = 8;
            FormatMapcruncher.Text = "MapCruncher";
            OGCtoolTip.SetToolTip(FormatMapcruncher, "EPSG:3857");
            FormatMapcruncher.UseVisualStyleBackColor = true;
            FormatMapcruncher.CheckedChanged += TileFormat_CheckedChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(112, 44);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(23, 17);
            label6.TabIndex = 35;
            label6.Text = "{S}";
            OGCtoolTip.SetToolTip(label6, "Subdomains replacer");
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(107, 90);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(31, 17);
            label7.TabIndex = 35;
            label7.Text = "Size";
            OGCtoolTip.SetToolTip(label7, "Tile size");
            // 
            // GeositeServerUser
            // 
            GeositeServerUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GeositeServerUser.Location = new Point(33, 38);
            GeositeServerUser.Margin = new Padding(4);
            GeositeServerUser.Name = "GeositeServerUser";
            GeositeServerUser.PlaceholderText = "e.g. user1";
            GeositeServerUser.Size = new Size(184, 23);
            GeositeServerUser.TabIndex = 1;
            OGCtoolTip.SetToolTip(GeositeServerUser, "GeositeServer user name");
            GeositeServerUser.TextChanged += GeositeServer_LinkChanged;
            // 
            // GeositeServerPassword
            // 
            GeositeServerPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GeositeServerPassword.Location = new Point(33, 69);
            GeositeServerPassword.Margin = new Padding(4);
            GeositeServerPassword.Name = "GeositeServerPassword";
            GeositeServerPassword.PlaceholderText = "e.g. xxxxxx";
            GeositeServerPassword.Size = new Size(184, 23);
            GeositeServerPassword.TabIndex = 1;
            OGCtoolTip.SetToolTip(GeositeServerPassword, "GeositeServer user password");
            GeositeServerPassword.UseSystemPasswordChar = true;
            GeositeServerPassword.TextChanged += GeositeServer_LinkChanged;
            // 
            // ModelSave
            // 
            ModelSave.BackColor = Color.White;
            ModelSave.BackgroundImage = (Image)resources.GetObject("ModelSave.BackgroundImage");
            ModelSave.BackgroundImageLayout = ImageLayout.Center;
            ModelSave.Cursor = Cursors.Hand;
            ModelSave.Enabled = false;
            ModelSave.Location = new Point(108, 61);
            ModelSave.Margin = new Padding(0);
            ModelSave.Name = "ModelSave";
            ModelSave.Size = new Size(59, 45);
            ModelSave.TabIndex = 11;
            OGCtoolTip.SetToolTip(ModelSave, "Can be saved as GeoTIFF");
            ModelSave.UseVisualStyleBackColor = false;
            ModelSave.Click += ModelSave_Click;
            // 
            // ModelOpen
            // 
            ModelOpen.BackColor = Color.White;
            ModelOpen.BackgroundImage = (Image)resources.GetObject("ModelOpen.BackgroundImage");
            ModelOpen.BackgroundImageLayout = ImageLayout.Center;
            ModelOpen.Cursor = Cursors.Hand;
            ModelOpen.Location = new Point(108, 10);
            ModelOpen.Margin = new Padding(0);
            ModelOpen.Name = "ModelOpen";
            ModelOpen.Size = new Size(59, 43);
            ModelOpen.TabIndex = 11;
            OGCtoolTip.SetToolTip(ModelOpen, "Only geographic coordinate systems are supported");
            ModelOpen.UseVisualStyleBackColor = false;
            ModelOpen.Click += ModelOpen_Click;
            // 
            // tileconvert
            // 
            tileconvert.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tileconvert.BackgroundImageLayout = ImageLayout.Center;
            tileconvert.Cursor = Cursors.Hand;
            tileconvert.Enabled = false;
            tileconvert.Font = new Font("宋体", 14F, FontStyle.Bold, GraphicsUnit.Point);
            tileconvert.ForeColor = Color.Black;
            tileconvert.Location = new Point(807, 11);
            tileconvert.Margin = new Padding(4);
            tileconvert.Name = "tileconvert";
            tileconvert.Size = new Size(53, 99);
            tileconvert.TabIndex = 20;
            tileconvert.Text = ">";
            OGCtoolTip.SetToolTip(tileconvert, "Run");
            tileconvert.UseVisualStyleBackColor = true;
            tileconvert.Click += TileConvert_Click;
            // 
            // MIMEBox
            // 
            MIMEBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MIMEBox.AutoCompleteCustomSource.AddRange(new string[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" });
            MIMEBox.BackColor = Color.White;
            MIMEBox.DropDownStyle = ComboBoxStyle.DropDownList;
            MIMEBox.FormattingEnabled = true;
            MIMEBox.Items.AddRange(new object[] { "png", "jpeg", "gif", "bmp", "tiff", "webp" });
            MIMEBox.Location = new Point(793, 41);
            MIMEBox.Margin = new Padding(4);
            MIMEBox.Name = "MIMEBox";
            MIMEBox.Size = new Size(67, 25);
            MIMEBox.TabIndex = 38;
            OGCtoolTip.SetToolTip(MIMEBox, "MIME type");
            MIMEBox.SelectedIndexChanged += FormEventChanged;
            // 
            // rankList
            // 
            rankList.BackColor = Color.WhiteSmoke;
            rankList.DropDownStyle = ComboBoxStyle.DropDownList;
            rankList.FlatStyle = FlatStyle.Flat;
            rankList.FormattingEnabled = true;
            rankList.Items.AddRange(new object[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            rankList.Location = new Point(7, 22);
            rankList.Margin = new Padding(4);
            rankList.Name = "rankList";
            rankList.RightToLeft = RightToLeft.No;
            rankList.Size = new Size(57, 25);
            rankList.TabIndex = 22;
            OGCtoolTip.SetToolTip(rankList, "[-1] for all users, Other escalation");
            rankList.SelectedIndexChanged += RankList_SelectedIndexChanged;
            // 
            // groupBox7
            // 
            groupBox7.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox7.Controls.Add(rankList);
            groupBox7.Location = new Point(907, 445);
            groupBox7.Margin = new Padding(4);
            groupBox7.Name = "groupBox7";
            groupBox7.Padding = new Padding(4);
            groupBox7.Size = new Size(72, 58);
            groupBox7.TabIndex = 23;
            groupBox7.TabStop = false;
            groupBox7.Text = "Rank";
            OGCtoolTip.SetToolTip(groupBox7, "Access level");
            // 
            // PostgresLight
            // 
            PostgresLight.BackColor = Color.WhiteSmoke;
            PostgresLight.BackgroundImage = (Image)resources.GetObject("PostgresLight.BackgroundImage");
            PostgresLight.BackgroundImageLayout = ImageLayout.Zoom;
            PostgresLight.Checked = true;
            PostgresLight.CheckState = CheckState.Checked;
            PostgresLight.Cursor = Cursors.Hand;
            PostgresLight.ImageAlign = ContentAlignment.MiddleRight;
            PostgresLight.Location = new Point(7, 23);
            PostgresLight.Margin = new Padding(4);
            PostgresLight.Name = "PostgresLight";
            PostgresLight.RightToLeft = RightToLeft.Yes;
            PostgresLight.Size = new Size(57, 23);
            PostgresLight.TabIndex = 18;
            OGCtoolTip.SetToolTip(PostgresLight, "Open sharing permission?");
            PostgresLight.UseVisualStyleBackColor = false;
            PostgresLight.CheckedChanged += PostgresLight_CheckedChanged;
            // 
            // UpdateBox
            // 
            UpdateBox.AutoSize = true;
            UpdateBox.BackgroundImage = (Image)resources.GetObject("UpdateBox.BackgroundImage");
            UpdateBox.BackgroundImageLayout = ImageLayout.None;
            UpdateBox.CheckAlign = ContentAlignment.MiddleRight;
            UpdateBox.Checked = true;
            UpdateBox.CheckState = CheckState.Checked;
            UpdateBox.Cursor = Cursors.Hand;
            UpdateBox.Location = new Point(208, 26);
            UpdateBox.Margin = new Padding(4);
            UpdateBox.Name = "UpdateBox";
            UpdateBox.Size = new Size(35, 21);
            UpdateBox.TabIndex = 19;
            UpdateBox.Text = "  ";
            OGCtoolTip.SetToolTip(UpdateBox, "Update ?");
            UpdateBox.UseVisualStyleBackColor = true;
            UpdateBox.CheckedChanged += FormEventChanged;
            // 
            // EPSG4326
            // 
            EPSG4326.AutoSize = true;
            EPSG4326.BackgroundImage = (Image)resources.GetObject("EPSG4326.BackgroundImage");
            EPSG4326.BackgroundImageLayout = ImageLayout.None;
            EPSG4326.CheckAlign = ContentAlignment.MiddleRight;
            EPSG4326.Cursor = Cursors.Hand;
            EPSG4326.ForeColor = SystemColors.ControlText;
            EPSG4326.Location = new Point(151, 26);
            EPSG4326.Margin = new Padding(4);
            EPSG4326.Name = "EPSG4326";
            EPSG4326.Size = new Size(35, 21);
            EPSG4326.TabIndex = 14;
            EPSG4326.Text = "  ";
            EPSG4326.ThreeState = true;
            OGCtoolTip.SetToolTip(EPSG4326, "EPSG:4326 ?\r\nGeographic coordinate system");
            EPSG4326.UseVisualStyleBackColor = true;
            EPSG4326.CheckedChanged += FormEventChanged;
            // 
            // wmtsSpider
            // 
            wmtsSpider.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            wmtsSpider.BackgroundImage = (Image)resources.GetObject("wmtsSpider.BackgroundImage");
            wmtsSpider.BackgroundImageLayout = ImageLayout.Zoom;
            wmtsSpider.CheckAlign = ContentAlignment.MiddleRight;
            wmtsSpider.Cursor = Cursors.Hand;
            wmtsSpider.ImageAlign = ContentAlignment.MiddleRight;
            wmtsSpider.Location = new Point(793, 76);
            wmtsSpider.Margin = new Padding(4);
            wmtsSpider.Name = "wmtsSpider";
            wmtsSpider.Size = new Size(67, 34);
            wmtsSpider.TabIndex = 18;
            OGCtoolTip.SetToolTip(wmtsSpider, "Spider\r\nPush to database?");
            wmtsSpider.UseVisualStyleBackColor = true;
            wmtsSpider.CheckedChanged += WmtsSpider_CheckedChanged;
            // 
            // pictureBox7
            // 
            pictureBox7.BackgroundImage = (Image)resources.GetObject("pictureBox7.BackgroundImage");
            pictureBox7.BackgroundImageLayout = ImageLayout.Center;
            pictureBox7.Location = new Point(111, 6);
            pictureBox7.Margin = new Padding(4);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(24, 23);
            pictureBox7.TabIndex = 0;
            pictureBox7.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox7, "URI\r\nSubstitution: {s} {z} {x} {y} {BingMap} {ESRI}");
            // 
            // pictureBox5
            // 
            pictureBox5.BackgroundImage = (Image)resources.GetObject("pictureBox5.BackgroundImage");
            pictureBox5.BackgroundImageLayout = ImageLayout.Center;
            pictureBox5.Location = new Point(9, 10);
            pictureBox5.Margin = new Padding(4);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(82, 89);
            pictureBox5.TabIndex = 25;
            pictureBox5.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox5, "Geographic Reference System");
            // 
            // VectorFileClear
            // 
            VectorFileClear.BackColor = Color.White;
            VectorFileClear.BackgroundImage = (Image)resources.GetObject("VectorFileClear.BackgroundImage");
            VectorFileClear.BackgroundImageLayout = ImageLayout.Center;
            VectorFileClear.Cursor = Cursors.Hand;
            VectorFileClear.Enabled = false;
            VectorFileClear.Font = new Font("宋体", 10F, FontStyle.Bold, GraphicsUnit.Point);
            VectorFileClear.ForeColor = Color.Black;
            VectorFileClear.Location = new Point(7, 76);
            VectorFileClear.Margin = new Padding(4);
            VectorFileClear.Name = "VectorFileClear";
            VectorFileClear.Size = new Size(64, 60);
            VectorFileClear.TabIndex = 16;
            OGCtoolTip.SetToolTip(VectorFileClear, "Clear selected");
            VectorFileClear.UseVisualStyleBackColor = false;
            VectorFileClear.Click += VectorFileClear_Click;
            // 
            // VectorOpen
            // 
            VectorOpen.BackColor = Color.White;
            VectorOpen.BackgroundImage = (Image)resources.GetObject("VectorOpen.BackgroundImage");
            VectorOpen.BackgroundImageLayout = ImageLayout.Center;
            VectorOpen.Cursor = Cursors.Hand;
            VectorOpen.Location = new Point(7, 8);
            VectorOpen.Margin = new Padding(4);
            VectorOpen.Name = "VectorOpen";
            VectorOpen.Size = new Size(64, 58);
            VectorOpen.TabIndex = 15;
            OGCtoolTip.SetToolTip(VectorOpen, "Open vector files");
            VectorOpen.UseVisualStyleBackColor = false;
            VectorOpen.Click += VectorOpen_Click;
            // 
            // Reindex
            // 
            Reindex.BackColor = Color.White;
            Reindex.BackgroundImage = (Image)resources.GetObject("Reindex.BackgroundImage");
            Reindex.BackgroundImageLayout = ImageLayout.Center;
            Reindex.Cursor = Cursors.Hand;
            Reindex.Dock = DockStyle.Fill;
            Reindex.Enabled = false;
            Reindex.Location = new Point(0, 0);
            Reindex.Margin = new Padding(0);
            Reindex.Name = "Reindex";
            Reindex.Size = new Size(144, 37);
            Reindex.TabIndex = 24;
            OGCtoolTip.SetToolTip(Reindex, "Reindex");
            Reindex.UseVisualStyleBackColor = false;
            Reindex.Click += ReIndex_Click;
            // 
            // ReClean
            // 
            ReClean.BackColor = Color.White;
            ReClean.BackgroundImage = (Image)resources.GetObject("ReClean.BackgroundImage");
            ReClean.BackgroundImageLayout = ImageLayout.Center;
            ReClean.Cursor = Cursors.Hand;
            ReClean.Dock = DockStyle.Fill;
            ReClean.Enabled = false;
            ReClean.Location = new Point(0, 0);
            ReClean.Margin = new Padding(0);
            ReClean.Name = "ReClean";
            ReClean.Size = new Size(144, 37);
            ReClean.TabIndex = 24;
            OGCtoolTip.SetToolTip(ReClean, "ReClean");
            ReClean.UseVisualStyleBackColor = false;
            ReClean.Click += ReClean_Click;
            // 
            // GeositeServerLink
            // 
            GeositeServerLink.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            GeositeServerLink.BackColor = Color.White;
            GeositeServerLink.BackgroundImage = (Image)resources.GetObject("GeositeServerLink.BackgroundImage");
            GeositeServerLink.BackgroundImageLayout = ImageLayout.Center;
            GeositeServerLink.Cursor = Cursors.Hand;
            GeositeServerLink.Location = new Point(263, 7);
            GeositeServerLink.Margin = new Padding(4);
            GeositeServerLink.Name = "GeositeServerLink";
            GeositeServerLink.Size = new Size(51, 85);
            GeositeServerLink.TabIndex = 2;
            OGCtoolTip.SetToolTip(GeositeServerLink, "Connect to GeositeServer");
            GeositeServerLink.UseVisualStyleBackColor = false;
            GeositeServerLink.Click += GeositeServerLink_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.BackgroundImage = (Image)resources.GetObject("pictureBox4.BackgroundImage");
            pictureBox4.BackgroundImageLayout = ImageLayout.Center;
            pictureBox4.Location = new Point(4, 65);
            pictureBox4.Margin = new Padding(4);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(26, 30);
            pictureBox4.TabIndex = 0;
            pictureBox4.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox4, "GeositeServer Password");
            // 
            // DatabaseLogIcon
            // 
            DatabaseLogIcon.BackgroundImage = (Image)resources.GetObject("DatabaseLogIcon.BackgroundImage");
            DatabaseLogIcon.BackgroundImageLayout = ImageLayout.Center;
            DatabaseLogIcon.Location = new Point(4, 102);
            DatabaseLogIcon.Margin = new Padding(4);
            DatabaseLogIcon.Name = "DatabaseLogIcon";
            DatabaseLogIcon.Size = new Size(26, 23);
            DatabaseLogIcon.TabIndex = 0;
            DatabaseLogIcon.TabStop = false;
            OGCtoolTip.SetToolTip(DatabaseLogIcon, "Clear Database Log");
            DatabaseLogIcon.Click += DatabaseLogIcon_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Center;
            pictureBox3.Location = new Point(4, 34);
            pictureBox3.Margin = new Padding(4);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(26, 30);
            pictureBox3.TabIndex = 0;
            pictureBox3.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox3, "GeositeServer User");
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Center;
            pictureBox2.Location = new Point(4, 3);
            pictureBox2.Margin = new Padding(4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(26, 30);
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox2, "GeositeServer URL");
            // 
            // PostgresRun
            // 
            PostgresRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            PostgresRun.BackColor = Color.White;
            PostgresRun.BackgroundImage = (Image)resources.GetObject("PostgresRun.BackgroundImage");
            PostgresRun.BackgroundImageLayout = ImageLayout.Center;
            PostgresRun.Cursor = Cursors.Hand;
            PostgresRun.Enabled = false;
            PostgresRun.Font = new Font("Arial", 18F, FontStyle.Bold, GraphicsUnit.Point);
            PostgresRun.ForeColor = Color.Black;
            PostgresRun.Location = new Point(907, 512);
            PostgresRun.Margin = new Padding(4);
            PostgresRun.Name = "PostgresRun";
            PostgresRun.Size = new Size(72, 54);
            PostgresRun.TabIndex = 14;
            OGCtoolTip.SetToolTip(PostgresRun, "Start");
            PostgresRun.UseVisualStyleBackColor = false;
            PostgresRun.Click += PostgresRun_Click;
            // 
            // localTileOpen
            // 
            localTileOpen.BackColor = Color.White;
            localTileOpen.BackgroundImage = (Image)resources.GetObject("localTileOpen.BackgroundImage");
            localTileOpen.BackgroundImageLayout = ImageLayout.Center;
            localTileOpen.Cursor = Cursors.Hand;
            localTileOpen.Location = new Point(7, 5);
            localTileOpen.Margin = new Padding(4);
            localTileOpen.Name = "localTileOpen";
            localTileOpen.Size = new Size(57, 51);
            localTileOpen.TabIndex = 5;
            OGCtoolTip.SetToolTip(localTileOpen, "Open a folder");
            localTileOpen.UseVisualStyleBackColor = false;
            localTileOpen.Click += LocalTileOpen_Click;
            // 
            // TileFormatSave
            // 
            TileFormatSave.BackColor = Color.White;
            TileFormatSave.BackgroundImage = (Image)resources.GetObject("TileFormatSave.BackgroundImage");
            TileFormatSave.BackgroundImageLayout = ImageLayout.Center;
            TileFormatSave.Cursor = Cursors.Hand;
            TileFormatSave.FlatAppearance.MouseDownBackColor = Color.Transparent;
            TileFormatSave.FlatAppearance.MouseOverBackColor = Color.Transparent;
            TileFormatSave.Location = new Point(108, 62);
            TileFormatSave.Margin = new Padding(4);
            TileFormatSave.Name = "TileFormatSave";
            TileFormatSave.Size = new Size(52, 48);
            TileFormatSave.TabIndex = 28;
            OGCtoolTip.SetToolTip(TileFormatSave, "Save to");
            TileFormatSave.UseVisualStyleBackColor = false;
            TileFormatSave.Click += TileFormatSave_Click;
            // 
            // TileFormatOpen
            // 
            TileFormatOpen.BackColor = Color.White;
            TileFormatOpen.BackgroundImage = (Image)resources.GetObject("TileFormatOpen.BackgroundImage");
            TileFormatOpen.BackgroundImageLayout = ImageLayout.Center;
            TileFormatOpen.Cursor = Cursors.Hand;
            TileFormatOpen.FlatAppearance.MouseDownBackColor = Color.White;
            TileFormatOpen.FlatAppearance.MouseOverBackColor = Color.White;
            TileFormatOpen.Location = new Point(108, 11);
            TileFormatOpen.Margin = new Padding(4);
            TileFormatOpen.Name = "TileFormatOpen";
            TileFormatOpen.Size = new Size(52, 48);
            TileFormatOpen.TabIndex = 27;
            OGCtoolTip.SetToolTip(TileFormatOpen, "Open a folder");
            TileFormatOpen.UseVisualStyleBackColor = false;
            TileFormatOpen.Click += TileFormatOpen_Click;
            // 
            // pictureBox6
            // 
            pictureBox6.Image = (Image)resources.GetObject("pictureBox6.Image");
            pictureBox6.ImeMode = ImeMode.NoControl;
            pictureBox6.Location = new Point(3, 6);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(16, 16);
            pictureBox6.TabIndex = 39;
            pictureBox6.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox6, "Bottom");
            // 
            // pictureBox10
            // 
            pictureBox10.Image = (Image)resources.GetObject("pictureBox10.Image");
            pictureBox10.ImeMode = ImeMode.NoControl;
            pictureBox10.Location = new Point(3, 6);
            pictureBox10.Name = "pictureBox10";
            pictureBox10.Size = new Size(16, 16);
            pictureBox10.TabIndex = 40;
            pictureBox10.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox10, "Right");
            // 
            // pictureBox13
            // 
            pictureBox13.Image = (Image)resources.GetObject("pictureBox13.Image");
            pictureBox13.ImeMode = ImeMode.NoControl;
            pictureBox13.Location = new Point(3, 6);
            pictureBox13.Name = "pictureBox13";
            pictureBox13.Size = new Size(16, 16);
            pictureBox13.TabIndex = 41;
            pictureBox13.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox13, "Left");
            // 
            // pictureBox15
            // 
            pictureBox15.Image = (Image)resources.GetObject("pictureBox15.Image");
            pictureBox15.ImeMode = ImeMode.NoControl;
            pictureBox15.Location = new Point(3, 6);
            pictureBox15.Name = "pictureBox15";
            pictureBox15.Size = new Size(16, 16);
            pictureBox15.TabIndex = 42;
            pictureBox15.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox15, "Top");
            // 
            // pictureBox16
            // 
            pictureBox16.BackgroundImage = (Image)resources.GetObject("pictureBox16.BackgroundImage");
            pictureBox16.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox16.Location = new Point(9, 10);
            pictureBox16.Margin = new Padding(4);
            pictureBox16.Name = "pictureBox16";
            pictureBox16.Size = new Size(82, 89);
            pictureBox16.TabIndex = 46;
            pictureBox16.TabStop = false;
            OGCtoolTip.SetToolTip(pictureBox16, "Geographic Reference System");
            // 
            // FileLoadLogIcon
            // 
            FileLoadLogIcon.BackgroundImage = (Image)resources.GetObject("FileLoadLogIcon.BackgroundImage");
            FileLoadLogIcon.BackgroundImageLayout = ImageLayout.Center;
            FileLoadLogIcon.Location = new Point(8, 6);
            FileLoadLogIcon.Name = "FileLoadLogIcon";
            FileLoadLogIcon.Size = new Size(29, 32);
            FileLoadLogIcon.TabIndex = 17;
            FileLoadLogIcon.TabStop = false;
            OGCtoolTip.SetToolTip(FileLoadLogIcon, "Clear FileLoad Log");
            FileLoadLogIcon.Click += FileLoadLogIcon_Click;
            // 
            // FileLoadLog
            // 
            FileLoadLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            FileLoadLog.BackColor = Color.WhiteSmoke;
            FileLoadLog.BorderStyle = BorderStyle.None;
            FileLoadLog.FormattingEnabled = true;
            FileLoadLog.ItemHeight = 17;
            FileLoadLog.Items.AddRange(new object[] { "Data format converter.", "Before conversion, Verify the data with the help of the Preview and should be in Longitude / Latitude." });
            FileLoadLog.Location = new Point(44, 6);
            FileLoadLog.Name = "FileLoadLog";
            FileLoadLog.SelectionMode = SelectionMode.MultiExtended;
            FileLoadLog.Size = new Size(926, 34);
            FileLoadLog.TabIndex = 18;
            OGCtoolTip.SetToolTip(FileLoadLog, "FileLoad Log");
            FileLoadLog.KeyDown += FileLoadLog_KeyDown;
            FileLoadLog.MouseLeave += FileLoadLog_MouseLeave;
            // 
            // DatabaseLog
            // 
            DatabaseLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            DatabaseLog.BackColor = Color.WhiteSmoke;
            DatabaseLog.FormattingEnabled = true;
            DatabaseLog.ItemHeight = 17;
            DatabaseLog.Items.AddRange(new object[] { "Database connection.", "GeositeServer bridging is required." });
            DatabaseLog.Location = new Point(33, 103);
            DatabaseLog.Name = "DatabaseLog";
            DatabaseLog.SelectionMode = SelectionMode.MultiExtended;
            DatabaseLog.Size = new Size(280, 72);
            DatabaseLog.TabIndex = 28;
            OGCtoolTip.SetToolTip(DatabaseLog, "Database Log");
            DatabaseLog.KeyDown += DatabaseLog_KeyDown;
            DatabaseLog.MouseLeave += DatabaseLog_MouseLeave;
            // 
            // tilewebapi
            // 
            tilewebapi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tilewebapi.BackColor = Color.White;
            tilewebapi.Location = new Point(141, 6);
            tilewebapi.Margin = new Padding(4);
            tilewebapi.Name = "tilewebapi";
            tilewebapi.Size = new Size(719, 23);
            tilewebapi.TabIndex = 30;
            tilewebapi.TextChanged += TileWebApi_TextChanged;
            // 
            // FileSaveGroupBox
            // 
            FileSaveGroupBox.BackColor = Color.White;
            FileSaveGroupBox.Controls.Add(toolStrip4);
            FileSaveGroupBox.Dock = DockStyle.Bottom;
            FileSaveGroupBox.Enabled = false;
            FileSaveGroupBox.Location = new Point(0, 501);
            FileSaveGroupBox.Margin = new Padding(4);
            FileSaveGroupBox.Name = "FileSaveGroupBox";
            FileSaveGroupBox.Padding = new Padding(4);
            FileSaveGroupBox.Size = new Size(974, 70);
            FileSaveGroupBox.TabIndex = 15;
            FileSaveGroupBox.TabStop = false;
            FileSaveGroupBox.Text = "Save";
            // 
            // toolStrip4
            // 
            toolStrip4.AllowDrop = true;
            toolStrip4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            toolStrip4.AutoSize = false;
            toolStrip4.BackColor = Color.Transparent;
            toolStrip4.BackgroundImageLayout = ImageLayout.None;
            toolStrip4.Dock = DockStyle.None;
            toolStrip4.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip4.ImageScalingSize = new Size(32, 32);
            toolStrip4.Items.AddRange(new ToolStripItem[] { vectorSaveButton, toolStripTextBox2, FileRunButton, toolStripSeparator3, SaveAsFormat, vectorTargetFile });
            toolStrip4.Location = new Point(4, 12);
            toolStrip4.Name = "toolStrip4";
            toolStrip4.Size = new Size(966, 56);
            toolStrip4.TabIndex = 18;
            toolStrip4.Text = "toolStrip4";
            // 
            // vectorSaveButton
            // 
            vectorSaveButton.AutoSize = false;
            vectorSaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            vectorSaveButton.Image = (Image)resources.GetObject("vectorSaveButton.Image");
            vectorSaveButton.ImageScaling = ToolStripItemImageScaling.None;
            vectorSaveButton.ImageTransparentColor = Color.Magenta;
            vectorSaveButton.Name = "vectorSaveButton";
            vectorSaveButton.Size = new Size(50, 46);
            vectorSaveButton.Text = "Save AS";
            vectorSaveButton.TextImageRelation = TextImageRelation.ImageAboveText;
            vectorSaveButton.Click += VectorSaveFile_Click;
            // 
            // toolStripTextBox2
            // 
            toolStripTextBox2.AutoCompleteMode = AutoCompleteMode.Append;
            toolStripTextBox2.BackColor = Color.WhiteSmoke;
            toolStripTextBox2.BorderStyle = BorderStyle.FixedSingle;
            toolStripTextBox2.Name = "toolStripTextBox2";
            toolStripTextBox2.RightToLeft = RightToLeft.No;
            toolStripTextBox2.Size = new Size(0, 56);
            // 
            // FileRunButton
            // 
            FileRunButton.Alignment = ToolStripItemAlignment.Right;
            FileRunButton.AutoSize = false;
            FileRunButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FileRunButton.Image = (Image)resources.GetObject("FileRunButton.Image");
            FileRunButton.ImageTransparentColor = Color.Magenta;
            FileRunButton.Name = "FileRunButton";
            FileRunButton.Size = new Size(50, 46);
            FileRunButton.Text = "Run";
            FileRunButton.TextImageRelation = TextImageRelation.ImageAboveText;
            FileRunButton.ToolTipText = "Run";
            FileRunButton.Click += FileRun_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator3.AutoSize = false;
            toolStripSeparator3.Margin = new Padding(12, 0, 0, 0);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 58);
            // 
            // SaveAsFormat
            // 
            SaveAsFormat.Alignment = ToolStripItemAlignment.Right;
            SaveAsFormat.BackColor = Color.White;
            SaveAsFormat.FlatStyle = FlatStyle.Standard;
            SaveAsFormat.Name = "SaveAsFormat";
            SaveAsFormat.Size = new Size(162, 56);
            // 
            // vectorTargetFile
            // 
            vectorTargetFile.BackColor = Color.White;
            vectorTargetFile.BorderStyle = BorderStyle.FixedSingle;
            vectorTargetFile.Name = "vectorTargetFile";
            vectorTargetFile.ReadOnly = true;
            vectorTargetFile.Size = new Size(648, 56);
            vectorTargetFile.DoubleClick += VectorTargetFile_DoubleClick;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.BackColor = Color.White;
            groupBox4.Controls.Add(panel44);
            groupBox4.Controls.Add(fileToolStrip);
            groupBox4.FlatStyle = FlatStyle.System;
            groupBox4.Location = new Point(0, 44);
            groupBox4.Margin = new Padding(4);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4);
            groupBox4.Size = new Size(974, 455);
            groupBox4.TabIndex = 14;
            groupBox4.TabStop = false;
            groupBox4.Text = "Open";
            // 
            // panel44
            // 
            panel44.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel44.Controls.Add(FileSplitContainer);
            panel44.Location = new Point(4, 102);
            panel44.Name = "panel44";
            panel44.Size = new Size(966, 348);
            panel44.TabIndex = 19;
            // 
            // FileSplitContainer
            // 
            FileSplitContainer.BackColor = Color.Transparent;
            FileSplitContainer.Dock = DockStyle.Fill;
            FileSplitContainer.Location = new Point(0, 0);
            FileSplitContainer.Margin = new Padding(0);
            FileSplitContainer.Name = "FileSplitContainer";
            // 
            // FileSplitContainer.Panel1
            // 
            FileSplitContainer.Panel1.BackColor = Color.Transparent;
            FileSplitContainer.Panel1.Controls.Add(panel45);
            // 
            // FileSplitContainer.Panel2
            // 
            FileSplitContainer.Panel2.BackColor = Color.White;
            FileSplitContainer.Panel2.Controls.Add(panel46);
            FileSplitContainer.Size = new Size(966, 348);
            FileSplitContainer.SplitterDistance = 278;
            FileSplitContainer.TabIndex = 18;
            // 
            // panel45
            // 
            panel45.Controls.Add(FileTabControl);
            panel45.Dock = DockStyle.Fill;
            panel45.Location = new Point(0, 0);
            panel45.Margin = new Padding(0);
            panel45.Name = "panel45";
            panel45.Size = new Size(278, 348);
            panel45.TabIndex = 0;
            // 
            // FileTabControl
            // 
            FileTabControl.Alignment = TabAlignment.Bottom;
            FileTabControl.Controls.Add(FileTabPage);
            FileTabControl.Dock = DockStyle.Fill;
            FileTabControl.Location = new Point(0, 0);
            FileTabControl.Margin = new Padding(0);
            FileTabControl.Name = "FileTabControl";
            FileTabControl.Padding = new Point(0, 0);
            FileTabControl.SelectedIndex = 0;
            FileTabControl.Size = new Size(278, 348);
            FileTabControl.TabIndex = 1;
            // 
            // FileTabPage
            // 
            FileTabPage.BackColor = Color.White;
            FileTabPage.Controls.Add(tableLayoutPanel11);
            FileTabPage.Location = new Point(4, 4);
            FileTabPage.Margin = new Padding(0);
            FileTabPage.Name = "FileTabPage";
            FileTabPage.Size = new Size(270, 318);
            FileTabPage.TabIndex = 0;
            FileTabPage.Text = "Files";
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.ColumnCount = 1;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel11.Controls.Add(FilePreviewProgressBar, 0, 0);
            tableLayoutPanel11.Controls.Add(panel3, 0, 1);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(0, 0);
            tableLayoutPanel11.Margin = new Padding(0);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 2;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Absolute, 3F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle());
            tableLayoutPanel11.Size = new Size(270, 318);
            tableLayoutPanel11.TabIndex = 20;
            // 
            // FilePreviewProgressBar
            // 
            FilePreviewProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            FilePreviewProgressBar.BackColor = Color.White;
            FilePreviewProgressBar.Location = new Point(0, 0);
            FilePreviewProgressBar.Margin = new Padding(0);
            FilePreviewProgressBar.MarqueeAnimationSpeed = 0;
            FilePreviewProgressBar.Name = "FilePreviewProgressBar";
            FilePreviewProgressBar.Size = new Size(270, 1);
            FilePreviewProgressBar.Step = 100;
            FilePreviewProgressBar.Style = ProgressBarStyle.Marquee;
            FilePreviewProgressBar.TabIndex = 18;
            FilePreviewProgressBar.Value = 100;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(FileGridView);
            panel3.Location = new Point(0, 3);
            panel3.Margin = new Padding(0);
            panel3.Name = "panel3";
            panel3.Size = new Size(270, 315);
            panel3.TabIndex = 19;
            // 
            // FileGridView
            // 
            FileGridView.AllowUserToAddRows = false;
            FileGridView.AllowUserToDeleteRows = false;
            FileGridView.AllowUserToOrderColumns = true;
            FileGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FileGridView.BackgroundColor = Color.WhiteSmoke;
            FileGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.WhiteSmoke;
            dataGridViewCellStyle6.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.True;
            FileGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            FileGridView.ColumnHeadersHeight = 39;
            FileGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            FileGridView.Columns.AddRange(new DataGridViewColumn[] { FilePath, FilePreview });
            FileGridView.Location = new Point(-1, -1);
            FileGridView.Name = "FileGridView";
            FileGridView.ReadOnly = true;
            FileGridView.RowHeadersWidth = 36;
            FileGridView.RowTemplate.Height = 25;
            FileGridView.Size = new Size(270, 315);
            FileGridView.TabIndex = 17;
            FileGridView.CellContentClick += FileGridView_CellContentClick;
            FileGridView.RowsAdded += FileGridView_RowsAdded;
            FileGridView.RowsRemoved += FileGridView_RowsRemoved;
            FileGridView.SelectionChanged += FileGridView_SelectionChanged;
            // 
            // FilePath
            // 
            FilePath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleLeft;
            FilePath.DefaultCellStyle = dataGridViewCellStyle7;
            FilePath.HeaderText = "File";
            FilePath.Name = "FilePath";
            FilePath.ReadOnly = true;
            FilePath.ToolTipText = "文件路径";
            // 
            // FilePreview
            // 
            FilePreview.HeaderText = "Preview";
            FilePreview.Name = "FilePreview";
            FilePreview.ReadOnly = true;
            FilePreview.Text = "Preview";
            FilePreview.ToolTipText = "文件预览";
            FilePreview.UseColumnTextForButtonValue = true;
            FilePreview.Width = 66;
            // 
            // panel46
            // 
            panel46.Controls.Add(PreviewTabControl);
            panel46.Dock = DockStyle.Fill;
            panel46.Location = new Point(0, 0);
            panel46.Margin = new Padding(0);
            panel46.Name = "panel46";
            panel46.Size = new Size(684, 348);
            panel46.TabIndex = 1;
            // 
            // PreviewTabControl
            // 
            PreviewTabControl.Alignment = TabAlignment.Bottom;
            PreviewTabControl.Controls.Add(MapTabPage);
            PreviewTabControl.Dock = DockStyle.Fill;
            PreviewTabControl.Location = new Point(0, 0);
            PreviewTabControl.Margin = new Padding(0);
            PreviewTabControl.Name = "PreviewTabControl";
            PreviewTabControl.Padding = new Point(0, 0);
            PreviewTabControl.SelectedIndex = 0;
            PreviewTabControl.Size = new Size(684, 348);
            PreviewTabControl.TabIndex = 0;
            // 
            // MapTabPage
            // 
            MapTabPage.BackColor = Color.White;
            MapTabPage.Controls.Add(tableLayoutPanel12);
            MapTabPage.Location = new Point(4, 4);
            MapTabPage.Margin = new Padding(0);
            MapTabPage.Name = "MapTabPage";
            MapTabPage.Size = new Size(676, 318);
            MapTabPage.TabIndex = 0;
            MapTabPage.Text = "Preview";
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 1;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.Controls.Add(FileMapSplitContainer, 0, 1);
            tableLayoutPanel12.Controls.Add(TileLoadProgressBar, 0, 0);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(0, 0);
            tableLayoutPanel12.Margin = new Padding(0);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 2;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Absolute, 3F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle());
            tableLayoutPanel12.Size = new Size(676, 318);
            tableLayoutPanel12.TabIndex = 22;
            // 
            // FileMapSplitContainer
            // 
            FileMapSplitContainer.Dock = DockStyle.Fill;
            FileMapSplitContainer.Location = new Point(0, 3);
            FileMapSplitContainer.Margin = new Padding(0);
            FileMapSplitContainer.Name = "FileMapSplitContainer";
            FileMapSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // FileMapSplitContainer.Panel1
            // 
            FileMapSplitContainer.Panel1.Controls.Add(panel12);
            // 
            // FileMapSplitContainer.Panel2
            // 
            FileMapSplitContainer.Panel2.Controls.Add(propertyPanel);
            FileMapSplitContainer.Size = new Size(676, 315);
            FileMapSplitContainer.SplitterDistance = 206;
            FileMapSplitContainer.TabIndex = 0;
            // 
            // panel12
            // 
            panel12.BackColor = Color.Transparent;
            panel12.BorderStyle = BorderStyle.FixedSingle;
            panel12.Controls.Add(panel21);
            panel12.Controls.Add(panel18);
            panel12.Controls.Add(gMapPanel);
            panel12.Dock = DockStyle.Fill;
            panel12.Location = new Point(0, 0);
            panel12.Name = "panel12";
            panel12.Size = new Size(676, 206);
            panel12.TabIndex = 1;
            // 
            // panel21
            // 
            panel21.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel21.BackColor = SystemColors.WindowFrame;
            panel21.Location = new Point(-1, 37);
            panel21.Name = "panel21";
            panel21.Size = new Size(675, 1);
            panel21.TabIndex = 2;
            // 
            // panel18
            // 
            panel18.Controls.Add(toolStrip1);
            panel18.Dock = DockStyle.Top;
            panel18.Location = new Point(0, 0);
            panel18.Name = "panel18";
            panel18.Size = new Size(674, 37);
            panel18.TabIndex = 1;
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = Color.White;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { MapProviderDropDown, ZoomLevelLabel, toolStripSeparator8, ZoomToLayer, toolStripSeparator6, ClearLayers, ImageMaker, toolStripSeparator7, PositionBox });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(674, 36);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // MapProviderDropDown
            // 
            MapProviderDropDown.AutoToolTip = false;
            MapProviderDropDown.BackColor = Color.White;
            MapProviderDropDown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            MapProviderDropDown.ImageScaling = ToolStripItemImageScaling.None;
            MapProviderDropDown.ImageTransparentColor = Color.Magenta;
            MapProviderDropDown.Margin = new Padding(2, 1, 0, 2);
            MapProviderDropDown.Name = "MapProviderDropDown";
            MapProviderDropDown.Size = new Size(65, 33);
            MapProviderDropDown.Text = "(Empty)";
            MapProviderDropDown.ToolTipText = "BaseMap Provider";
            // 
            // ZoomLevelLabel
            // 
            ZoomLevelLabel.Name = "ZoomLevelLabel";
            ZoomLevelLabel.Size = new Size(35, 33);
            ZoomLevelLabel.Text = "0 / 0";
            ZoomLevelLabel.ToolTipText = "Zoom Level";
            ZoomLevelLabel.Click += ZoomLevelLabel_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(6, 36);
            // 
            // ZoomToLayer
            // 
            ZoomToLayer.AutoSize = false;
            ZoomToLayer.AutoToolTip = false;
            ZoomToLayer.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ZoomToLayer.Image = (Image)resources.GetObject("ZoomToLayer.Image");
            ZoomToLayer.ImageScaling = ToolStripItemImageScaling.None;
            ZoomToLayer.ImageTransparentColor = Color.Magenta;
            ZoomToLayer.Margin = new Padding(2);
            ZoomToLayer.Name = "ZoomToLayer";
            ZoomToLayer.Padding = new Padding(2);
            ZoomToLayer.Size = new Size(32, 32);
            ZoomToLayer.Text = "Zoom to Layer";
            ZoomToLayer.ToolTipText = "Zoom to Layer";
            ZoomToLayer.Click += ZoomToLayer_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 36);
            // 
            // ClearLayers
            // 
            ClearLayers.AutoSize = false;
            ClearLayers.AutoToolTip = false;
            ClearLayers.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ClearLayers.DoubleClickEnabled = true;
            ClearLayers.Image = (Image)resources.GetObject("ClearLayers.Image");
            ClearLayers.ImageScaling = ToolStripItemImageScaling.None;
            ClearLayers.ImageTransparentColor = Color.Magenta;
            ClearLayers.Name = "ClearLayers";
            ClearLayers.Padding = new Padding(2);
            ClearLayers.Size = new Size(32, 32);
            ClearLayers.Text = "Clear Layers";
            ClearLayers.ToolTipText = "Clear Layers (click)\r\nClear Tiles Catch (Right click)";
            ClearLayers.MouseUp += ClearLayers_MouseUp;
            // 
            // ImageMaker
            // 
            ImageMaker.Alignment = ToolStripItemAlignment.Right;
            ImageMaker.AutoSize = false;
            ImageMaker.AutoToolTip = false;
            ImageMaker.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ImageMaker.Image = (Image)resources.GetObject("ImageMaker.Image");
            ImageMaker.ImageScaling = ToolStripItemImageScaling.None;
            ImageMaker.ImageTransparentColor = Color.Magenta;
            ImageMaker.Name = "ImageMaker";
            ImageMaker.Padding = new Padding(2);
            ImageMaker.Size = new Size(32, 32);
            ImageMaker.Text = "Image Maker";
            ImageMaker.ToolTipText = "Map Snapshot";
            ImageMaker.Click += ImageMaker_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 36);
            // 
            // PositionBox
            // 
            PositionBox.Alignment = ToolStripItemAlignment.Right;
            PositionBox.AutoToolTip = false;
            PositionBox.DisplayStyle = ToolStripItemDisplayStyle.Text;
            PositionBox.DoubleClickEnabled = true;
            PositionBox.DropDownItems.AddRange(new ToolStripItem[] { DegMenuItem, DmsMenuItem, toolStripSeparator4, BeijingMenuItem, XianMenuItem, CGCS2000MenuItem, toolStripSeparator5, WebMercatorMenuItem });
            PositionBox.ImageTransparentColor = Color.Magenta;
            PositionBox.Name = "PositionBox";
            PositionBox.Size = new Size(13, 33);
            PositionBox.Tag = "";
            PositionBox.TextAlign = ContentAlignment.MiddleRight;
            PositionBox.DoubleClick += PositionBox_DoubleClick;
            // 
            // DegMenuItem
            // 
            DegMenuItem.Checked = true;
            DegMenuItem.CheckOnClick = true;
            DegMenuItem.CheckState = CheckState.Checked;
            DegMenuItem.Name = "DegMenuItem";
            DegMenuItem.Size = new Size(161, 22);
            DegMenuItem.Tag = "DEG";
            DegMenuItem.Text = "DEG";
            DegMenuItem.Click += PositionMenuItem_Click;
            // 
            // DmsMenuItem
            // 
            DmsMenuItem.CheckOnClick = true;
            DmsMenuItem.Name = "DmsMenuItem";
            DmsMenuItem.Size = new Size(161, 22);
            DmsMenuItem.Tag = "DMS";
            DmsMenuItem.Text = "DMS";
            DmsMenuItem.Click += PositionMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(158, 6);
            // 
            // BeijingMenuItem
            // 
            BeijingMenuItem.CheckOnClick = true;
            BeijingMenuItem.Name = "BeijingMenuItem";
            BeijingMenuItem.Size = new Size(161, 22);
            BeijingMenuItem.Tag = "Beijing 1954";
            BeijingMenuItem.Text = "Beijing 1954";
            BeijingMenuItem.Click += PositionMenuItem_Click;
            // 
            // XianMenuItem
            // 
            XianMenuItem.CheckOnClick = true;
            XianMenuItem.Name = "XianMenuItem";
            XianMenuItem.Size = new Size(161, 22);
            XianMenuItem.Tag = "Xian 1980";
            XianMenuItem.Text = "Xian 1980";
            XianMenuItem.Click += PositionMenuItem_Click;
            // 
            // CGCS2000MenuItem
            // 
            CGCS2000MenuItem.CheckOnClick = true;
            CGCS2000MenuItem.Name = "CGCS2000MenuItem";
            CGCS2000MenuItem.Size = new Size(161, 22);
            CGCS2000MenuItem.Tag = "CGCS 2000";
            CGCS2000MenuItem.Text = "CGCS 2000";
            CGCS2000MenuItem.Click += PositionMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(158, 6);
            // 
            // WebMercatorMenuItem
            // 
            WebMercatorMenuItem.CheckOnClick = true;
            WebMercatorMenuItem.Name = "WebMercatorMenuItem";
            WebMercatorMenuItem.Size = new Size(161, 22);
            WebMercatorMenuItem.Tag = "Web Mercator";
            WebMercatorMenuItem.Text = "Web Mercator";
            WebMercatorMenuItem.Click += PositionMenuItem_Click;
            // 
            // gMapPanel
            // 
            gMapPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gMapPanel.BackColor = Color.Transparent;
            gMapPanel.Controls.Add(MapBox);
            gMapPanel.Location = new Point(-1, 37);
            gMapPanel.Name = "gMapPanel";
            gMapPanel.Size = new Size(676, 168);
            gMapPanel.TabIndex = 0;
            // 
            // MapBox
            // 
            MapBox.BackColor = Color.White;
            MapBox.Bearing = 0F;
            MapBox.CanDragMap = true;
            MapBox.Dock = DockStyle.Fill;
            MapBox.EmptyTileColor = Color.Transparent;
            MapBox.GrayScaleMode = false;
            MapBox.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            MapBox.LevelsKeepInMemory = 5;
            MapBox.Location = new Point(0, 0);
            MapBox.Margin = new Padding(0);
            MapBox.MarkersEnabled = true;
            MapBox.MaxZoom = 18;
            MapBox.MinZoom = 0;
            MapBox.MouseWheelZoomEnabled = true;
            MapBox.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            MapBox.Name = "MapBox";
            MapBox.NegativeMode = false;
            MapBox.PolygonsEnabled = true;
            MapBox.RetryLoadTile = 0;
            MapBox.RoutesEnabled = true;
            MapBox.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            MapBox.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
            MapBox.ShowTileGridLines = false;
            MapBox.Size = new Size(676, 168);
            MapBox.TabIndex = 0;
            MapBox.Zoom = 5D;
            MapBox.Load += MapBox_Load;
            // 
            // propertyPanel
            // 
            propertyPanel.BorderStyle = BorderStyle.FixedSingle;
            propertyPanel.Controls.Add(MapBoxProperty);
            propertyPanel.Controls.Add(panel22);
            propertyPanel.Controls.Add(label8);
            propertyPanel.Controls.Add(pictureBox8);
            propertyPanel.Dock = DockStyle.Fill;
            propertyPanel.Location = new Point(0, 0);
            propertyPanel.Margin = new Padding(0);
            propertyPanel.Name = "propertyPanel";
            propertyPanel.Size = new Size(676, 105);
            propertyPanel.TabIndex = 0;
            // 
            // MapBoxProperty
            // 
            MapBoxProperty.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MapBoxProperty.BackColor = Color.White;
            MapBoxProperty.BorderStyle = BorderStyle.None;
            MapBoxProperty.Location = new Point(3, 29);
            MapBoxProperty.Name = "MapBoxProperty";
            MapBoxProperty.ReadOnly = true;
            MapBoxProperty.Size = new Size(668, 71);
            MapBoxProperty.TabIndex = 20;
            MapBoxProperty.Text = "";
            // 
            // panel22
            // 
            panel22.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel22.BackColor = Color.LightGray;
            panel22.Location = new Point(0, 25);
            panel22.Name = "panel22";
            panel22.Size = new Size(673, 1);
            panel22.TabIndex = 19;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(27, 3);
            label8.Name = "label8";
            label8.Size = new Size(53, 17);
            label8.TabIndex = 18;
            label8.Text = "Content";
            // 
            // pictureBox8
            // 
            pictureBox8.BackgroundImage = (Image)resources.GetObject("pictureBox8.BackgroundImage");
            pictureBox8.BackgroundImageLayout = ImageLayout.Center;
            pictureBox8.Location = new Point(5, 3);
            pictureBox8.Name = "pictureBox8";
            pictureBox8.Size = new Size(18, 18);
            pictureBox8.TabIndex = 17;
            pictureBox8.TabStop = false;
            // 
            // TileLoadProgressBar
            // 
            TileLoadProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TileLoadProgressBar.BackColor = Color.White;
            TileLoadProgressBar.Location = new Point(0, 0);
            TileLoadProgressBar.Margin = new Padding(0);
            TileLoadProgressBar.MarqueeAnimationSpeed = 0;
            TileLoadProgressBar.Name = "TileLoadProgressBar";
            TileLoadProgressBar.Size = new Size(676, 1);
            TileLoadProgressBar.Step = 100;
            TileLoadProgressBar.Style = ProgressBarStyle.Marquee;
            TileLoadProgressBar.TabIndex = 20;
            TileLoadProgressBar.Value = 100;
            // 
            // fileToolStrip
            // 
            fileToolStrip.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileToolStrip.AutoSize = false;
            fileToolStrip.BackColor = Color.Transparent;
            fileToolStrip.BackgroundImageLayout = ImageLayout.None;
            fileToolStrip.Dock = DockStyle.None;
            fileToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            fileToolStrip.ImageScalingSize = new Size(45, 43);
            fileToolStrip.Items.AddRange(new ToolStripItem[] { vectorOpenButton, toolStripSeparator1, mapgisButton, arcgisIconButton, tableIconButton, gmlIconButton, geojsonIconButton, geositeIconButton, kmlIconButton, toolStripSeparator2, cleanIconButton });
            fileToolStrip.Location = new Point(4, 17);
            fileToolStrip.Name = "fileToolStrip";
            fileToolStrip.Padding = new Padding(0);
            fileToolStrip.Size = new Size(966, 78);
            fileToolStrip.TabIndex = 13;
            fileToolStrip.Text = "toolStrip1";
            // 
            // vectorOpenButton
            // 
            vectorOpenButton.AutoSize = false;
            vectorOpenButton.Image = (Image)resources.GetObject("vectorOpenButton.Image");
            vectorOpenButton.ImageTransparentColor = Color.Magenta;
            vectorOpenButton.Name = "vectorOpenButton";
            vectorOpenButton.Size = new Size(80, 64);
            vectorOpenButton.Text = "Files";
            vectorOpenButton.TextImageRelation = TextImageRelation.ImageAboveText;
            vectorOpenButton.Click += VectorOpenFile_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 78);
            // 
            // mapgisButton
            // 
            mapgisButton.AutoSize = false;
            mapgisButton.Image = Properties.Resources.MapGIS_logo;
            mapgisButton.ImageTransparentColor = Color.Magenta;
            mapgisButton.Name = "mapgisButton";
            mapgisButton.Size = new Size(80, 64);
            mapgisButton.Text = "MapGIS";
            mapgisButton.TextImageRelation = TextImageRelation.ImageAboveText;
            mapgisButton.ToolTipText = "MapGIS - wt wl wp mpj";
            mapgisButton.Click += MapGisIcon_Click;
            // 
            // arcgisIconButton
            // 
            arcgisIconButton.AutoSize = false;
            arcgisIconButton.Image = Properties.Resources.ArcGIS_logo;
            arcgisIconButton.ImageTransparentColor = Color.Magenta;
            arcgisIconButton.Name = "arcgisIconButton";
            arcgisIconButton.Size = new Size(80, 64);
            arcgisIconButton.Text = "ArcGIS";
            arcgisIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            arcgisIconButton.ToolTipText = "ArcGIS - ShapeFile";
            arcgisIconButton.Click += ArcGisIcon_Click;
            // 
            // tableIconButton
            // 
            tableIconButton.AutoSize = false;
            tableIconButton.Image = Properties.Resources.Table_logo;
            tableIconButton.ImageTransparentColor = Color.Magenta;
            tableIconButton.Name = "tableIconButton";
            tableIconButton.Size = new Size(80, 64);
            tableIconButton.Text = "Table";
            tableIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            tableIconButton.ToolTipText = "Excel TXT CSV";
            tableIconButton.Click += TabTextIcon_Click;
            // 
            // gmlIconButton
            // 
            gmlIconButton.AutoSize = false;
            gmlIconButton.Enabled = false;
            gmlIconButton.Image = Properties.Resources.GML_logo;
            gmlIconButton.ImageTransparentColor = Color.Magenta;
            gmlIconButton.Name = "gmlIconButton";
            gmlIconButton.Size = new Size(80, 64);
            gmlIconButton.Text = "GML";
            gmlIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            gmlIconButton.ToolTipText = "GML - OGC";
            // 
            // geojsonIconButton
            // 
            geojsonIconButton.AutoSize = false;
            geojsonIconButton.Image = Properties.Resources.GeoJSON_logo;
            geojsonIconButton.ImageTransparentColor = Color.Magenta;
            geojsonIconButton.Name = "geojsonIconButton";
            geojsonIconButton.Size = new Size(80, 64);
            geojsonIconButton.Text = "GeoJSON";
            geojsonIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            geojsonIconButton.Click += GeoJsonIcon_Click;
            // 
            // geositeIconButton
            // 
            geositeIconButton.AutoSize = false;
            geositeIconButton.Image = Properties.Resources.GeositeXML_logo;
            geositeIconButton.ImageTransparentColor = Color.Magenta;
            geositeIconButton.Name = "geositeIconButton";
            geositeIconButton.Size = new Size(80, 64);
            geositeIconButton.Text = "GeositeXML";
            geositeIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            geositeIconButton.ToolTipText = "GeositeXML - XML";
            geositeIconButton.Click += GeositeIcon_Click;
            // 
            // kmlIconButton
            // 
            kmlIconButton.AutoSize = false;
            kmlIconButton.Image = Properties.Resources.KML_logo;
            kmlIconButton.ImageTransparentColor = Color.Magenta;
            kmlIconButton.Name = "kmlIconButton";
            kmlIconButton.Size = new Size(80, 64);
            kmlIconButton.Text = "KML";
            kmlIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            kmlIconButton.ToolTipText = "KML - GoogleEarth";
            kmlIconButton.Click += KmlIcon_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 78);
            // 
            // cleanIconButton
            // 
            cleanIconButton.AutoSize = false;
            cleanIconButton.Enabled = false;
            cleanIconButton.Image = (Image)resources.GetObject("cleanIconButton.Image");
            cleanIconButton.ImageTransparentColor = Color.Magenta;
            cleanIconButton.Name = "cleanIconButton";
            cleanIconButton.Size = new Size(80, 64);
            cleanIconButton.Text = "Clean";
            cleanIconButton.TextImageRelation = TextImageRelation.ImageAboveText;
            cleanIconButton.ToolTipText = "Clear selected files from the list";
            cleanIconButton.Click += CleanIconButton_Click;
            // 
            // fileWorker
            // 
            fileWorker.WorkerReportsProgress = true;
            fileWorker.WorkerSupportsCancellation = true;
            // 
            // box
            // 
            box.AutoSize = true;
            box.Controls.Add(panel1);
            box.Dock = DockStyle.Fill;
            box.Location = new Point(4, 4);
            box.Margin = new Padding(0);
            box.Name = "box";
            box.Size = new Size(974, 571);
            box.TabIndex = 16;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(panel20);
            panel1.Controls.Add(groupBox4);
            panel1.Controls.Add(FileSaveGroupBox);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(974, 571);
            panel1.TabIndex = 10;
            // 
            // panel20
            // 
            panel20.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel20.BackColor = Color.WhiteSmoke;
            panel20.Controls.Add(panel23);
            panel20.Controls.Add(FileLoadLogIcon);
            panel20.Controls.Add(FileLoadLog);
            panel20.Location = new Point(0, 0);
            panel20.Name = "panel20";
            panel20.Size = new Size(974, 46);
            panel20.TabIndex = 19;
            // 
            // panel23
            // 
            panel23.BackColor = Color.White;
            panel23.Location = new Point(41, 2);
            panel23.Name = "panel23";
            panel23.Size = new Size(1, 42);
            panel23.TabIndex = 19;
            // 
            // ogcCard
            // 
            ogcCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ogcCard.Controls.Add(fileCard);
            ogcCard.Controls.Add(databaseCard);
            ogcCard.Controls.Add(helpCard);
            ogcCard.Location = new Point(6, 6);
            ogcCard.Margin = new Padding(0);
            ogcCard.Name = "ogcCard";
            ogcCard.Padding = new Point(3, 3);
            ogcCard.SelectedIndex = 0;
            ogcCard.Size = new Size(990, 609);
            ogcCard.TabIndex = 17;
            ogcCard.SelectedIndexChanged += OgcCard_SelectedIndexChanged;
            // 
            // fileCard
            // 
            fileCard.Controls.Add(box);
            fileCard.Location = new Point(4, 26);
            fileCard.Margin = new Padding(4);
            fileCard.Name = "fileCard";
            fileCard.Padding = new Padding(4);
            fileCard.Size = new Size(982, 579);
            fileCard.TabIndex = 0;
            fileCard.Text = "File";
            fileCard.UseVisualStyleBackColor = true;
            // 
            // databaseCard
            // 
            databaseCard.Controls.Add(PostgresLinkSplitContainer);
            databaseCard.Controls.Add(groupBox9);
            databaseCard.Controls.Add(panel7);
            databaseCard.Controls.Add(groupBox7);
            databaseCard.Controls.Add(dataCards);
            databaseCard.Controls.Add(PostgresRun);
            databaseCard.Location = new Point(4, 26);
            databaseCard.Margin = new Padding(4);
            databaseCard.Name = "databaseCard";
            databaseCard.Size = new Size(982, 579);
            databaseCard.TabIndex = 2;
            databaseCard.Text = "Database";
            databaseCard.UseVisualStyleBackColor = true;
            // 
            // PostgresLinkSplitContainer
            // 
            PostgresLinkSplitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PostgresLinkSplitContainer.Location = new Point(7, 6);
            PostgresLinkSplitContainer.Name = "PostgresLinkSplitContainer";
            // 
            // PostgresLinkSplitContainer.Panel1
            // 
            PostgresLinkSplitContainer.Panel1.Controls.Add(tabControl1);
            // 
            // PostgresLinkSplitContainer.Panel2
            // 
            PostgresLinkSplitContainer.Panel2.Controls.Add(DatabaseTabControl);
            PostgresLinkSplitContainer.Size = new Size(972, 259);
            PostgresLinkSplitContainer.SplitterDistance = 324;
            PostgresLinkSplitContainer.SplitterWidth = 6;
            PostgresLinkSplitContainer.TabIndex = 31;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(324, 259);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tableLayoutPanel1);
            tabPage1.Controls.Add(DatabaseLog);
            tabPage1.Controls.Add(panel9);
            tabPage1.Controls.Add(pictureBox3);
            tabPage1.Controls.Add(GeositeServerUrl);
            tabPage1.Controls.Add(DatabaseLogIcon);
            tabPage1.Controls.Add(GeositeServerPassword);
            tabPage1.Controls.Add(panel13);
            tabPage1.Controls.Add(pictureBox4);
            tabPage1.Controls.Add(GeositeServerLink);
            tabPage1.Controls.Add(deleteForest);
            tabPage1.Controls.Add(pictureBox2);
            tabPage1.Controls.Add(GeositeServerUser);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(316, 229);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "GeositeServer";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panel25, 2, 0);
            tableLayoutPanel1.Controls.Add(panel24, 0, 0);
            tableLayoutPanel1.Location = new Point(3, 183);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(310, 43);
            tableLayoutPanel1.TabIndex = 27;
            // 
            // panel25
            // 
            panel25.Controls.Add(Reindex);
            panel25.Dock = DockStyle.Fill;
            panel25.Location = new Point(163, 3);
            panel25.Name = "panel25";
            panel25.Size = new Size(144, 37);
            panel25.TabIndex = 31;
            // 
            // panel24
            // 
            panel24.Controls.Add(ReClean);
            panel24.Dock = DockStyle.Fill;
            panel24.Location = new Point(3, 3);
            panel24.Name = "panel24";
            panel24.Size = new Size(144, 37);
            panel24.TabIndex = 31;
            // 
            // panel9
            // 
            panel9.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel9.BackColor = Color.DarkGray;
            panel9.Location = new Point(4, 179);
            panel9.Name = "panel9";
            panel9.Size = new Size(310, 1);
            panel9.TabIndex = 26;
            // 
            // panel13
            // 
            panel13.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel13.BackColor = Color.DarkGray;
            panel13.Location = new Point(4, 98);
            panel13.Name = "panel13";
            panel13.Size = new Size(310, 1);
            panel13.TabIndex = 26;
            // 
            // DatabaseTabControl
            // 
            DatabaseTabControl.Controls.Add(DatabasePage);
            DatabaseTabControl.Controls.Add(CatalogPage);
            DatabaseTabControl.Dock = DockStyle.Fill;
            DatabaseTabControl.Location = new Point(0, 0);
            DatabaseTabControl.Multiline = true;
            DatabaseTabControl.Name = "DatabaseTabControl";
            DatabaseTabControl.SelectedIndex = 0;
            DatabaseTabControl.Size = new Size(642, 259);
            DatabaseTabControl.TabIndex = 0;
            // 
            // DatabasePage
            // 
            DatabasePage.Controls.Add(dataGridPanel);
            DatabasePage.Location = new Point(4, 26);
            DatabasePage.Name = "DatabasePage";
            DatabasePage.Padding = new Padding(3);
            DatabasePage.Size = new Size(634, 229);
            DatabasePage.TabIndex = 0;
            DatabasePage.Text = "Database";
            DatabasePage.UseVisualStyleBackColor = true;
            // 
            // dataGridPanel
            // 
            dataGridPanel.Controls.Add(tableLayoutPanel2);
            dataGridPanel.Controls.Add(panel2);
            dataGridPanel.Controls.Add(DatabaseProgressBar);
            dataGridPanel.Dock = DockStyle.Fill;
            dataGridPanel.Enabled = false;
            dataGridPanel.Location = new Point(3, 3);
            dataGridPanel.Margin = new Padding(4);
            dataGridPanel.Name = "dataGridPanel";
            dataGridPanel.Size = new Size(628, 223);
            dataGridPanel.TabIndex = 23;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 11;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel2.Controls.Add(panel31, 10, 0);
            tableLayoutPanel2.Controls.Add(panel30, 8, 0);
            tableLayoutPanel2.Controls.Add(panel29, 6, 0);
            tableLayoutPanel2.Controls.Add(panel28, 4, 0);
            tableLayoutPanel2.Controls.Add(panel26, 0, 0);
            tableLayoutPanel2.Controls.Add(panel27, 2, 0);
            tableLayoutPanel2.Location = new Point(4, 180);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(620, 43);
            tableLayoutPanel2.TabIndex = 20;
            // 
            // panel31
            // 
            panel31.Controls.Add(deleteTree);
            panel31.Dock = DockStyle.Fill;
            panel31.Location = new Point(563, 3);
            panel31.Name = "panel31";
            panel31.Size = new Size(54, 37);
            panel31.TabIndex = 32;
            // 
            // panel30
            // 
            panel30.Controls.Add(pagesBox);
            panel30.Dock = DockStyle.Fill;
            panel30.Location = new Point(283, 3);
            panel30.Name = "panel30";
            panel30.Padding = new Padding(3, 0, 3, 0);
            panel30.Size = new Size(264, 37);
            panel30.TabIndex = 32;
            // 
            // panel29
            // 
            panel29.Controls.Add(lastPage);
            panel29.Dock = DockStyle.Fill;
            panel29.Location = new Point(213, 3);
            panel29.Name = "panel29";
            panel29.Size = new Size(54, 37);
            panel29.TabIndex = 32;
            // 
            // panel28
            // 
            panel28.Controls.Add(nextPage);
            panel28.Dock = DockStyle.Fill;
            panel28.Location = new Point(143, 3);
            panel28.Name = "panel28";
            panel28.Size = new Size(54, 37);
            panel28.TabIndex = 32;
            // 
            // panel26
            // 
            panel26.Controls.Add(firstPage);
            panel26.Dock = DockStyle.Fill;
            panel26.Location = new Point(3, 3);
            panel26.Name = "panel26";
            panel26.Size = new Size(54, 37);
            panel26.TabIndex = 32;
            // 
            // panel27
            // 
            panel27.Controls.Add(previousPage);
            panel27.Dock = DockStyle.Fill;
            panel27.Location = new Point(73, 3);
            panel27.Name = "panel27";
            panel27.Size = new Size(54, 37);
            panel27.TabIndex = 32;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(DatabaseGridView);
            panel2.Location = new Point(4, 5);
            panel2.Margin = new Padding(4);
            panel2.Name = "panel2";
            panel2.Size = new Size(620, 167);
            panel2.TabIndex = 5;
            // 
            // DatabaseGridView
            // 
            DatabaseGridView.AllowUserToAddRows = false;
            DatabaseGridView.AllowUserToDeleteRows = false;
            DatabaseGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            DatabaseGridView.BackgroundColor = SystemColors.ControlLightLight;
            DatabaseGridView.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle8.BackColor = SystemColors.Control;
            dataGridViewCellStyle8.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle8.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            DatabaseGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            DatabaseGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DatabaseGridView.Columns.AddRange(new DataGridViewColumn[] { ThemeName, ThemeRank, ThemeStatus, ThemeType });
            DatabaseGridView.Location = new Point(-1, -1);
            DatabaseGridView.Margin = new Padding(4);
            DatabaseGridView.Name = "DatabaseGridView";
            DatabaseGridView.RowTemplate.Height = 23;
            DatabaseGridView.RowTemplate.Resizable = DataGridViewTriState.False;
            DatabaseGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DatabaseGridView.ShowCellToolTips = false;
            DatabaseGridView.Size = new Size(620, 167);
            DatabaseGridView.TabIndex = 20;
            DatabaseGridView.CellBeginEdit += DataPool_CellBeginEdit;
            DatabaseGridView.CellClick += DataPool_CellClick;
            DatabaseGridView.CellEndEdit += DataPool_CellEndEdit;
            // 
            // ThemeName
            // 
            ThemeName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ThemeName.HeaderText = "Theme";
            ThemeName.Name = "ThemeName";
            // 
            // ThemeRank
            // 
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ThemeRank.DefaultCellStyle = dataGridViewCellStyle9;
            ThemeRank.HeaderText = "Rank";
            ThemeRank.MinimumWidth = 36;
            ThemeRank.Name = "ThemeRank";
            ThemeRank.Resizable = DataGridViewTriState.True;
            ThemeRank.SortMode = DataGridViewColumnSortMode.NotSortable;
            ThemeRank.ToolTipText = "[-1] for all users, Other escalation";
            ThemeRank.Width = 48;
            // 
            // ThemeStatus
            // 
            ThemeStatus.HeaderText = "Status";
            ThemeStatus.MinimumWidth = 16;
            ThemeStatus.Name = "ThemeStatus";
            ThemeStatus.ReadOnly = true;
            ThemeStatus.Resizable = DataGridViewTriState.False;
            ThemeStatus.Width = 48;
            // 
            // ThemeType
            // 
            ThemeType.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ThemeType.HeaderText = "Type";
            ThemeType.MinimumWidth = 16;
            ThemeType.Name = "ThemeType";
            ThemeType.ReadOnly = true;
            ThemeType.Resizable = DataGridViewTriState.False;
            ThemeType.Width = 48;
            // 
            // DatabaseProgressBar
            // 
            DatabaseProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            DatabaseProgressBar.BackColor = Color.White;
            DatabaseProgressBar.Location = new Point(4, 176);
            DatabaseProgressBar.Margin = new Padding(4);
            DatabaseProgressBar.MarqueeAnimationSpeed = 0;
            DatabaseProgressBar.Name = "DatabaseProgressBar";
            DatabaseProgressBar.Size = new Size(620, 1);
            DatabaseProgressBar.Step = 50;
            DatabaseProgressBar.Style = ProgressBarStyle.Marquee;
            DatabaseProgressBar.TabIndex = 19;
            DatabaseProgressBar.Value = 100;
            // 
            // CatalogPage
            // 
            CatalogPage.Controls.Add(CatalogTreeView);
            CatalogPage.Location = new Point(4, 26);
            CatalogPage.Name = "CatalogPage";
            CatalogPage.Padding = new Padding(3);
            CatalogPage.Size = new Size(634, 229);
            CatalogPage.TabIndex = 1;
            CatalogPage.Text = "Catalog";
            CatalogPage.UseVisualStyleBackColor = true;
            // 
            // CatalogTreeView
            // 
            CatalogTreeView.BorderStyle = BorderStyle.None;
            CatalogTreeView.Dock = DockStyle.Fill;
            CatalogTreeView.Location = new Point(3, 3);
            CatalogTreeView.Name = "CatalogTreeView";
            CatalogTreeView.ShowNodeToolTips = true;
            CatalogTreeView.Size = new Size(628, 223);
            CatalogTreeView.TabIndex = 0;
            CatalogTreeView.BeforeExpand += CatalogTreeView_BeforeExpand;
            // 
            // groupBox9
            // 
            groupBox9.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox9.Controls.Add(PostgresLight);
            groupBox9.Location = new Point(907, 384);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(70, 54);
            groupBox9.TabIndex = 30;
            groupBox9.TabStop = false;
            groupBox9.Text = "Share";
            // 
            // panel7
            // 
            panel7.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel7.BackColor = Color.LightGray;
            panel7.Location = new Point(7, 270);
            panel7.Name = "panel7";
            panel7.Size = new Size(972, 1);
            panel7.TabIndex = 29;
            // 
            // dataCards
            // 
            dataCards.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataCards.Controls.Add(RasterPage);
            dataCards.Controls.Add(VectorPage);
            dataCards.Location = new Point(7, 276);
            dataCards.Margin = new Padding(4);
            dataCards.Name = "dataCards";
            dataCards.SelectedIndex = 0;
            dataCards.Size = new Size(892, 300);
            dataCards.TabIndex = 20;
            dataCards.SelectedIndexChanged += DataCards_SelectedIndexChanged;
            // 
            // RasterPage
            // 
            RasterPage.Controls.Add(PushPanel);
            RasterPage.Controls.Add(tilesource);
            RasterPage.Location = new Point(4, 26);
            RasterPage.Margin = new Padding(4);
            RasterPage.Name = "RasterPage";
            RasterPage.Size = new Size(884, 270);
            RasterPage.TabIndex = 2;
            RasterPage.Text = "Raster";
            RasterPage.UseVisualStyleBackColor = true;
            // 
            // PushPanel
            // 
            PushPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PushPanel.Controls.Add(groupBox1);
            PushPanel.Controls.Add(method);
            PushPanel.Location = new Point(0, 193);
            PushPanel.Margin = new Padding(4);
            PushPanel.Name = "PushPanel";
            PushPanel.Size = new Size(880, 71);
            PushPanel.TabIndex = 19;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(themeNameBox);
            groupBox1.Location = new Point(265, 10);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(611, 61);
            groupBox1.TabIndex = 18;
            groupBox1.TabStop = false;
            groupBox1.Text = "Theme Name";
            // 
            // themeNameBox
            // 
            themeNameBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            themeNameBox.BackColor = Color.White;
            themeNameBox.Location = new Point(8, 26);
            themeNameBox.Margin = new Padding(4);
            themeNameBox.Name = "themeNameBox";
            themeNameBox.Size = new Size(595, 23);
            themeNameBox.TabIndex = 30;
            themeNameBox.TextChanged += ThemeNameBox_TextChanged;
            // 
            // method
            // 
            method.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            method.Controls.Add(UpdateBox);
            method.Controls.Add(label1);
            method.Controls.Add(tileLevels);
            method.Controls.Add(EPSG4326);
            method.FlatStyle = FlatStyle.Flat;
            method.Location = new Point(4, 10);
            method.Margin = new Padding(4);
            method.Name = "method";
            method.Padding = new Padding(4);
            method.Size = new Size(254, 61);
            method.TabIndex = 21;
            method.TabStop = false;
            method.Text = "Mode";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 27);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(37, 17);
            label1.TabIndex = 17;
            label1.Text = "Level";
            // 
            // tileLevels
            // 
            tileLevels.AutoCompleteCustomSource.AddRange(new string[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" });
            tileLevels.BackColor = Color.White;
            tileLevels.DropDownStyle = ComboBoxStyle.DropDownList;
            tileLevels.FormattingEnabled = true;
            tileLevels.Items.AddRange(new object[] { "-1", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" });
            tileLevels.Location = new Point(44, 23);
            tileLevels.Margin = new Padding(4);
            tileLevels.Name = "tileLevels";
            tileLevels.Size = new Size(85, 25);
            tileLevels.TabIndex = 13;
            tileLevels.SelectedIndexChanged += FormEventChanged;
            // 
            // tilesource
            // 
            tilesource.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tilesource.Controls.Add(LocalTilePage);
            tilesource.Controls.Add(RemoteTilePage);
            tilesource.Controls.Add(ModelPage);
            tilesource.Controls.Add(TileConvertPage);
            tilesource.Location = new Point(4, 4);
            tilesource.Margin = new Padding(4);
            tilesource.Name = "tilesource";
            tilesource.SelectedIndex = 0;
            tilesource.Size = new Size(876, 186);
            tilesource.TabIndex = 18;
            tilesource.SelectedIndexChanged += TileSource_SelectedIndexChanged;
            // 
            // LocalTilePage
            // 
            LocalTilePage.Controls.Add(localTileOpen);
            LocalTilePage.Controls.Add(FormatGroupBox);
            LocalTilePage.Controls.Add(localTileFolder);
            LocalTilePage.Location = new Point(4, 26);
            LocalTilePage.Margin = new Padding(4);
            LocalTilePage.Name = "LocalTilePage";
            LocalTilePage.Padding = new Padding(4);
            LocalTilePage.Size = new Size(868, 156);
            LocalTilePage.TabIndex = 0;
            LocalTilePage.Text = "Folder";
            LocalTilePage.UseVisualStyleBackColor = true;
            // 
            // FormatGroupBox
            // 
            FormatGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FormatGroupBox.Controls.Add(tableLayoutPanel3);
            FormatGroupBox.Location = new Point(7, 58);
            FormatGroupBox.Margin = new Padding(4);
            FormatGroupBox.Name = "FormatGroupBox";
            FormatGroupBox.Padding = new Padding(4);
            FormatGroupBox.Size = new Size(853, 98);
            FormatGroupBox.TabIndex = 23;
            FormatGroupBox.TabStop = false;
            FormatGroupBox.Text = "Format";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel3.ColumnCount = 6;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            tableLayoutPanel3.Controls.Add(panel32, 0, 0);
            tableLayoutPanel3.Controls.Add(panel33, 1, 0);
            tableLayoutPanel3.Controls.Add(panel34, 2, 0);
            tableLayoutPanel3.Controls.Add(panel35, 3, 0);
            tableLayoutPanel3.Controls.Add(panel36, 4, 0);
            tableLayoutPanel3.Controls.Add(panel37, 5, 0);
            tableLayoutPanel3.Controls.Add(FormatStandardBox, 0, 1);
            tableLayoutPanel3.Controls.Add(FormatTMSBox, 1, 1);
            tableLayoutPanel3.Controls.Add(FormatMapcruncherBox, 2, 1);
            tableLayoutPanel3.Controls.Add(FormatArcGISBox, 3, 1);
            tableLayoutPanel3.Controls.Add(FormatDeepZoomBox, 4, 1);
            tableLayoutPanel3.Controls.Add(FormatRasterBox, 5, 1);
            tableLayoutPanel3.Location = new Point(7, 17);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(839, 75);
            tableLayoutPanel3.TabIndex = 12;
            // 
            // panel32
            // 
            panel32.Controls.Add(FormatStandard);
            panel32.Dock = DockStyle.Fill;
            panel32.Location = new Point(5, 5);
            panel32.Name = "panel32";
            panel32.Size = new Size(159, 26);
            panel32.TabIndex = 0;
            // 
            // panel33
            // 
            panel33.Controls.Add(FormatTMS);
            panel33.Dock = DockStyle.Fill;
            panel33.Location = new Point(172, 5);
            panel33.Name = "panel33";
            panel33.Size = new Size(126, 26);
            panel33.TabIndex = 1;
            // 
            // panel34
            // 
            panel34.Controls.Add(FormatMapcruncher);
            panel34.Dock = DockStyle.Fill;
            panel34.Location = new Point(306, 5);
            panel34.Name = "panel34";
            panel34.Size = new Size(126, 26);
            panel34.TabIndex = 2;
            // 
            // panel35
            // 
            panel35.Controls.Add(FormatArcGIS);
            panel35.Dock = DockStyle.Fill;
            panel35.Location = new Point(440, 5);
            panel35.Name = "panel35";
            panel35.Size = new Size(126, 26);
            panel35.TabIndex = 3;
            // 
            // panel36
            // 
            panel36.Controls.Add(FormatDeepZoom);
            panel36.Dock = DockStyle.Fill;
            panel36.Location = new Point(574, 5);
            panel36.Name = "panel36";
            panel36.Size = new Size(126, 26);
            panel36.TabIndex = 4;
            // 
            // panel37
            // 
            panel37.Controls.Add(FormatRaster);
            panel37.Dock = DockStyle.Fill;
            panel37.Location = new Point(708, 5);
            panel37.Name = "panel37";
            panel37.Size = new Size(126, 26);
            panel37.TabIndex = 5;
            // 
            // FormatStandardBox
            // 
            FormatStandardBox.BackColor = Color.White;
            FormatStandardBox.BorderStyle = BorderStyle.None;
            FormatStandardBox.Dock = DockStyle.Fill;
            FormatStandardBox.Location = new Point(5, 39);
            FormatStandardBox.Name = "FormatStandardBox";
            FormatStandardBox.ReadOnly = true;
            FormatStandardBox.Size = new Size(159, 31);
            FormatStandardBox.TabIndex = 6;
            FormatStandardBox.Text = "Loading ...";
            FormatStandardBox.WordWrap = false;
            // 
            // FormatTMSBox
            // 
            FormatTMSBox.BackColor = Color.White;
            FormatTMSBox.BorderStyle = BorderStyle.None;
            FormatTMSBox.Dock = DockStyle.Fill;
            FormatTMSBox.Location = new Point(172, 39);
            FormatTMSBox.Name = "FormatTMSBox";
            FormatTMSBox.ReadOnly = true;
            FormatTMSBox.Size = new Size(126, 31);
            FormatTMSBox.TabIndex = 7;
            FormatTMSBox.Text = "Loading ...";
            FormatTMSBox.WordWrap = false;
            // 
            // FormatMapcruncherBox
            // 
            FormatMapcruncherBox.BackColor = Color.White;
            FormatMapcruncherBox.BorderStyle = BorderStyle.None;
            FormatMapcruncherBox.Dock = DockStyle.Fill;
            FormatMapcruncherBox.Location = new Point(306, 39);
            FormatMapcruncherBox.Name = "FormatMapcruncherBox";
            FormatMapcruncherBox.ReadOnly = true;
            FormatMapcruncherBox.Size = new Size(126, 31);
            FormatMapcruncherBox.TabIndex = 8;
            FormatMapcruncherBox.Text = "Loading ...";
            FormatMapcruncherBox.WordWrap = false;
            // 
            // FormatArcGISBox
            // 
            FormatArcGISBox.BackColor = Color.White;
            FormatArcGISBox.BorderStyle = BorderStyle.None;
            FormatArcGISBox.Dock = DockStyle.Fill;
            FormatArcGISBox.Location = new Point(440, 39);
            FormatArcGISBox.Name = "FormatArcGISBox";
            FormatArcGISBox.ReadOnly = true;
            FormatArcGISBox.Size = new Size(126, 31);
            FormatArcGISBox.TabIndex = 9;
            FormatArcGISBox.Text = "Loading ...";
            FormatArcGISBox.WordWrap = false;
            // 
            // FormatDeepZoomBox
            // 
            FormatDeepZoomBox.BackColor = Color.White;
            FormatDeepZoomBox.BorderStyle = BorderStyle.None;
            FormatDeepZoomBox.Dock = DockStyle.Fill;
            FormatDeepZoomBox.Location = new Point(574, 39);
            FormatDeepZoomBox.Name = "FormatDeepZoomBox";
            FormatDeepZoomBox.ReadOnly = true;
            FormatDeepZoomBox.Size = new Size(126, 31);
            FormatDeepZoomBox.TabIndex = 10;
            FormatDeepZoomBox.Text = "Loading ...";
            FormatDeepZoomBox.WordWrap = false;
            // 
            // FormatRasterBox
            // 
            FormatRasterBox.BackColor = Color.White;
            FormatRasterBox.BorderStyle = BorderStyle.None;
            FormatRasterBox.Dock = DockStyle.Fill;
            FormatRasterBox.Location = new Point(708, 39);
            FormatRasterBox.Name = "FormatRasterBox";
            FormatRasterBox.ReadOnly = true;
            FormatRasterBox.Size = new Size(126, 31);
            FormatRasterBox.TabIndex = 11;
            FormatRasterBox.Text = "Loading ...";
            FormatRasterBox.WordWrap = false;
            // 
            // localTileFolder
            // 
            localTileFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            localTileFolder.BackColor = SystemColors.Window;
            localTileFolder.Font = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point);
            localTileFolder.Location = new Point(72, 21);
            localTileFolder.Margin = new Padding(4);
            localTileFolder.MaxLength = 3276700;
            localTileFolder.Name = "localTileFolder";
            localTileFolder.ReadOnly = true;
            localTileFolder.Size = new Size(788, 21);
            localTileFolder.TabIndex = 10;
            localTileFolder.TextChanged += LocalTileFolder_TextChanged;
            localTileFolder.DoubleClick += LocalTileOpen_Click;
            // 
            // RemoteTilePage
            // 
            RemoteTilePage.Controls.Add(tableLayoutPanel10);
            RemoteTilePage.Controls.Add(panel42);
            RemoteTilePage.Controls.Add(pictureBox16);
            RemoteTilePage.Controls.Add(groupBox5);
            RemoteTilePage.Controls.Add(MIMEBox);
            RemoteTilePage.Controls.Add(tilewebapi);
            RemoteTilePage.Controls.Add(subdomainsBox);
            RemoteTilePage.Controls.Add(wmtsSize);
            RemoteTilePage.Controls.Add(label5);
            RemoteTilePage.Controls.Add(label6);
            RemoteTilePage.Controls.Add(wmtsMaxZoom);
            RemoteTilePage.Controls.Add(wmtsMinZoom);
            RemoteTilePage.Controls.Add(label7);
            RemoteTilePage.Controls.Add(label4);
            RemoteTilePage.Controls.Add(wmtsSpider);
            RemoteTilePage.Controls.Add(pictureBox7);
            RemoteTilePage.Location = new Point(4, 26);
            RemoteTilePage.Margin = new Padding(4);
            RemoteTilePage.Name = "RemoteTilePage";
            RemoteTilePage.Padding = new Padding(4);
            RemoteTilePage.Size = new Size(868, 156);
            RemoteTilePage.TabIndex = 1;
            RemoteTilePage.Text = "WM[T]S";
            RemoteTilePage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel10.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel10.ColumnCount = 1;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Controls.Add(wmtsTipBox, 0, 0);
            tableLayoutPanel10.Location = new Point(108, 118);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Size = new Size(752, 32);
            tableLayoutPanel10.TabIndex = 48;
            // 
            // wmtsTipBox
            // 
            wmtsTipBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            wmtsTipBox.BackColor = Color.White;
            wmtsTipBox.BorderStyle = BorderStyle.None;
            wmtsTipBox.Location = new Point(5, 5);
            wmtsTipBox.Name = "wmtsTipBox";
            wmtsTipBox.ReadOnly = true;
            wmtsTipBox.Size = new Size(742, 22);
            wmtsTipBox.TabIndex = 45;
            wmtsTipBox.Text = "Loading ...";
            wmtsTipBox.WordWrap = false;
            // 
            // panel42
            // 
            panel42.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel42.BackColor = Color.DarkGray;
            panel42.Location = new Point(100, 10);
            panel42.Name = "panel42";
            panel42.Size = new Size(1, 140);
            panel42.TabIndex = 47;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox5.Controls.Add(tableLayoutPanel6);
            groupBox5.Location = new Point(349, 30);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(437, 87);
            groupBox5.TabIndex = 44;
            groupBox5.TabStop = false;
            groupBox5.Text = "Boundary";
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(panel38, 0, 0);
            tableLayoutPanel6.Controls.Add(panel39, 1, 0);
            tableLayoutPanel6.Controls.Add(panel40, 0, 1);
            tableLayoutPanel6.Controls.Add(panel41, 1, 1);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 19);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(431, 65);
            tableLayoutPanel6.TabIndex = 43;
            // 
            // panel38
            // 
            panel38.Controls.Add(pictureBox13);
            panel38.Controls.Add(wmtsWest);
            panel38.Dock = DockStyle.Fill;
            panel38.Location = new Point(3, 3);
            panel38.Name = "panel38";
            panel38.Size = new Size(209, 26);
            panel38.TabIndex = 0;
            // 
            // panel39
            // 
            panel39.Controls.Add(pictureBox15);
            panel39.Controls.Add(wmtsNorth);
            panel39.Dock = DockStyle.Fill;
            panel39.Location = new Point(218, 3);
            panel39.Name = "panel39";
            panel39.Size = new Size(210, 26);
            panel39.TabIndex = 1;
            // 
            // panel40
            // 
            panel40.Controls.Add(pictureBox10);
            panel40.Controls.Add(wmtsEast);
            panel40.Dock = DockStyle.Fill;
            panel40.Location = new Point(3, 35);
            panel40.Name = "panel40";
            panel40.Size = new Size(209, 27);
            panel40.TabIndex = 2;
            // 
            // panel41
            // 
            panel41.Controls.Add(pictureBox6);
            panel41.Controls.Add(wmtsSouth);
            panel41.Dock = DockStyle.Fill;
            panel41.Location = new Point(218, 35);
            panel41.Name = "panel41";
            panel41.Size = new Size(210, 27);
            panel41.TabIndex = 3;
            // 
            // subdomainsBox
            // 
            subdomainsBox.Location = new Point(142, 41);
            subdomainsBox.Margin = new Padding(4);
            subdomainsBox.Name = "subdomainsBox";
            subdomainsBox.Size = new Size(68, 23);
            subdomainsBox.TabIndex = 37;
            subdomainsBox.TextChanged += FormEventChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(217, 89);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(67, 17);
            label5.TabIndex = 35;
            label5.Text = "MaxZoom";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // wmtsMaxZoom
            // 
            wmtsMaxZoom.AutoCompleteCustomSource.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" });
            wmtsMaxZoom.BackColor = Color.White;
            wmtsMaxZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            wmtsMaxZoom.Enabled = false;
            wmtsMaxZoom.FormattingEnabled = true;
            wmtsMaxZoom.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" });
            wmtsMaxZoom.Location = new Point(287, 85);
            wmtsMaxZoom.Margin = new Padding(4);
            wmtsMaxZoom.Name = "wmtsMaxZoom";
            wmtsMaxZoom.Size = new Size(55, 25);
            wmtsMaxZoom.TabIndex = 13;
            wmtsMaxZoom.Tag = "18";
            wmtsMaxZoom.SelectedIndexChanged += WmtsMaxZoom_SelectedIndexChanged;
            // 
            // wmtsMinZoom
            // 
            wmtsMinZoom.AutoCompleteCustomSource.AddRange(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" });
            wmtsMinZoom.BackColor = Color.White;
            wmtsMinZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            wmtsMinZoom.Enabled = false;
            wmtsMinZoom.FormattingEnabled = true;
            wmtsMinZoom.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" });
            wmtsMinZoom.Location = new Point(287, 41);
            wmtsMinZoom.Margin = new Padding(4);
            wmtsMinZoom.Name = "wmtsMinZoom";
            wmtsMinZoom.Size = new Size(55, 25);
            wmtsMinZoom.TabIndex = 13;
            wmtsMinZoom.Tag = "0";
            wmtsMinZoom.SelectedIndexChanged += WmtsMinZoom_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(217, 44);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(64, 17);
            label4.TabIndex = 35;
            label4.Text = "MinZoom";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ModelPage
            // 
            ModelPage.Controls.Add(tableLayoutPanel9);
            ModelPage.Controls.Add(tableLayoutPanel7);
            ModelPage.Controls.Add(panel8);
            ModelPage.Controls.Add(ModelSave);
            ModelPage.Controls.Add(ModelOpen);
            ModelPage.Controls.Add(ModelOpenTextBox);
            ModelPage.Controls.Add(pictureBox5);
            ModelPage.Location = new Point(4, 26);
            ModelPage.Margin = new Padding(4);
            ModelPage.Name = "ModelPage";
            ModelPage.Size = new Size(868, 156);
            ModelPage.TabIndex = 4;
            ModelPage.Text = "Model";
            ModelPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel9.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel9.ColumnCount = 1;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Controls.Add(modelTipBox, 0, 0);
            tableLayoutPanel9.Location = new Point(108, 111);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Size = new Size(752, 39);
            tableLayoutPanel9.TabIndex = 34;
            // 
            // modelTipBox
            // 
            modelTipBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modelTipBox.BackColor = Color.White;
            modelTipBox.BorderStyle = BorderStyle.None;
            modelTipBox.Location = new Point(5, 5);
            modelTipBox.Name = "modelTipBox";
            modelTipBox.ReadOnly = true;
            modelTipBox.Size = new Size(742, 29);
            modelTipBox.TabIndex = 33;
            modelTipBox.Text = "Loading ...";
            modelTipBox.WordWrap = false;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel7.ColumnCount = 3;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Controls.Add(groupBox3, 0, 0);
            tableLayoutPanel7.Controls.Add(groupBox2, 2, 0);
            tableLayoutPanel7.Location = new Point(176, 56);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.Size = new Size(684, 53);
            tableLayoutPanel7.TabIndex = 32;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(rasterTileSize);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(4, 4);
            groupBox3.Margin = new Padding(4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4);
            groupBox3.Size = new Size(329, 45);
            groupBox3.TabIndex = 26;
            groupBox3.TabStop = false;
            groupBox3.Text = "Tile size";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(nodatabox);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(351, 4);
            groupBox2.Margin = new Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4);
            groupBox2.Size = new Size(329, 45);
            groupBox2.TabIndex = 26;
            groupBox2.TabStop = false;
            groupBox2.Text = "Nodata mask";
            // 
            // nodatabox
            // 
            nodatabox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            nodatabox.BackColor = Color.WhiteSmoke;
            nodatabox.BorderStyle = BorderStyle.None;
            nodatabox.Location = new Point(9, 17);
            nodatabox.Margin = new Padding(4);
            nodatabox.Name = "nodatabox";
            nodatabox.Size = new Size(312, 16);
            nodatabox.TabIndex = 2;
            nodatabox.Text = "-32768";
            nodatabox.TextChanged += NoDataBox_TextChanged;
            // 
            // panel8
            // 
            panel8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel8.BackColor = Color.DarkGray;
            panel8.Location = new Point(100, 10);
            panel8.Name = "panel8";
            panel8.Size = new Size(1, 140);
            panel8.TabIndex = 31;
            // 
            // ModelOpenTextBox
            // 
            ModelOpenTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ModelOpenTextBox.BackColor = Color.WhiteSmoke;
            ModelOpenTextBox.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
            ModelOpenTextBox.Location = new Point(179, 10);
            ModelOpenTextBox.Margin = new Padding(4);
            ModelOpenTextBox.MaxLength = 3276700;
            ModelOpenTextBox.Multiline = true;
            ModelOpenTextBox.Name = "ModelOpenTextBox";
            ModelOpenTextBox.ReadOnly = true;
            ModelOpenTextBox.ScrollBars = ScrollBars.Vertical;
            ModelOpenTextBox.Size = new Size(677, 43);
            ModelOpenTextBox.TabIndex = 12;
            ModelOpenTextBox.TextChanged += ModelOpenTextBox_TextChanged;
            ModelOpenTextBox.DoubleClick += ModelOpen_Click;
            // 
            // TileConvertPage
            // 
            TileConvertPage.Controls.Add(panel16);
            TileConvertPage.Controls.Add(panel15);
            TileConvertPage.Controls.Add(tableLayoutPanel8);
            TileConvertPage.Controls.Add(panel14);
            TileConvertPage.Controls.Add(TileFormatSave);
            TileConvertPage.Controls.Add(TileFormatOpen);
            TileConvertPage.Controls.Add(panel6);
            TileConvertPage.Controls.Add(tileconvert);
            TileConvertPage.Controls.Add(TileFormatSaveBox);
            TileConvertPage.Controls.Add(TileFormatOpenBox);
            TileConvertPage.Controls.Add(pictureBox9);
            TileConvertPage.Location = new Point(4, 26);
            TileConvertPage.Margin = new Padding(4);
            TileConvertPage.Name = "TileConvertPage";
            TileConvertPage.Size = new Size(868, 156);
            TileConvertPage.TabIndex = 3;
            TileConvertPage.Text = "Convert";
            TileConvertPage.UseVisualStyleBackColor = true;
            // 
            // panel16
            // 
            panel16.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel16.BackColor = Color.LightGray;
            panel16.Location = new Point(800, 11);
            panel16.Name = "panel16";
            panel16.Size = new Size(1, 99);
            panel16.TabIndex = 38;
            // 
            // panel15
            // 
            panel15.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel15.BackColor = Color.LightGray;
            panel15.Location = new Point(608, 11);
            panel15.Name = "panel15";
            panel15.Size = new Size(1, 99);
            panel15.TabIndex = 37;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel8.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel8.ColumnCount = 1;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Controls.Add(convertTipBox, 0, 0);
            tableLayoutPanel8.Location = new Point(108, 115);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(752, 34);
            tableLayoutPanel8.TabIndex = 36;
            // 
            // convertTipBox
            // 
            convertTipBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            convertTipBox.BackColor = Color.White;
            convertTipBox.BorderStyle = BorderStyle.None;
            convertTipBox.Location = new Point(5, 5);
            convertTipBox.Name = "convertTipBox";
            convertTipBox.ReadOnly = true;
            convertTipBox.Size = new Size(742, 24);
            convertTipBox.TabIndex = 35;
            convertTipBox.Text = "Loading ...";
            convertTipBox.WordWrap = false;
            // 
            // panel14
            // 
            panel14.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel14.BackColor = Color.DarkGray;
            panel14.Location = new Point(100, 10);
            panel14.Name = "panel14";
            panel14.Size = new Size(1, 140);
            panel14.TabIndex = 34;
            // 
            // panel6
            // 
            panel6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel6.Controls.Add(maptilertoogc);
            panel6.Controls.Add(mapcrunchertoogc);
            panel6.Controls.Add(ogctomapcruncher);
            panel6.Controls.Add(ogctomaptiler);
            panel6.Location = new Point(611, 11);
            panel6.Margin = new Padding(4);
            panel6.Name = "panel6";
            panel6.Size = new Size(187, 99);
            panel6.TabIndex = 21;
            // 
            // maptilertoogc
            // 
            maptilertoogc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            maptilertoogc.CheckAlign = ContentAlignment.MiddleRight;
            maptilertoogc.Checked = true;
            maptilertoogc.Location = new Point(4, 4);
            maptilertoogc.Margin = new Padding(4);
            maptilertoogc.Name = "maptilertoogc";
            maptilertoogc.Size = new Size(179, 21);
            maptilertoogc.TabIndex = 2;
            maptilertoogc.TabStop = true;
            maptilertoogc.Text = "Maptiler ----> Standard";
            maptilertoogc.TextAlign = ContentAlignment.MiddleRight;
            maptilertoogc.UseVisualStyleBackColor = true;
            maptilertoogc.CheckedChanged += FormEventChanged;
            // 
            // mapcrunchertoogc
            // 
            mapcrunchertoogc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            mapcrunchertoogc.CheckAlign = ContentAlignment.MiddleRight;
            mapcrunchertoogc.Location = new Point(4, 27);
            mapcrunchertoogc.Margin = new Padding(4);
            mapcrunchertoogc.Name = "mapcrunchertoogc";
            mapcrunchertoogc.Size = new Size(179, 21);
            mapcrunchertoogc.TabIndex = 3;
            mapcrunchertoogc.Text = "Mapcruncher -> Standard";
            mapcrunchertoogc.TextAlign = ContentAlignment.MiddleRight;
            mapcrunchertoogc.UseVisualStyleBackColor = true;
            mapcrunchertoogc.CheckedChanged += FormEventChanged;
            // 
            // ogctomapcruncher
            // 
            ogctomapcruncher.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ogctomapcruncher.CheckAlign = ContentAlignment.MiddleRight;
            ogctomapcruncher.Location = new Point(4, 51);
            ogctomapcruncher.Margin = new Padding(4);
            ogctomapcruncher.Name = "ogctomapcruncher";
            ogctomapcruncher.Size = new Size(179, 21);
            ogctomapcruncher.TabIndex = 4;
            ogctomapcruncher.Text = "Standard -> Mapcruncher";
            ogctomapcruncher.TextAlign = ContentAlignment.MiddleRight;
            ogctomapcruncher.UseVisualStyleBackColor = true;
            ogctomapcruncher.CheckedChanged += FormEventChanged;
            // 
            // ogctomaptiler
            // 
            ogctomaptiler.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ogctomaptiler.CheckAlign = ContentAlignment.MiddleRight;
            ogctomaptiler.Location = new Point(4, 76);
            ogctomaptiler.Margin = new Padding(4);
            ogctomaptiler.Name = "ogctomaptiler";
            ogctomaptiler.Size = new Size(179, 21);
            ogctomaptiler.TabIndex = 5;
            ogctomaptiler.Text = "Standard ----> Maptiler";
            ogctomaptiler.TextAlign = ContentAlignment.MiddleRight;
            ogctomaptiler.UseVisualStyleBackColor = true;
            ogctomaptiler.CheckedChanged += FormEventChanged;
            // 
            // TileFormatSaveBox
            // 
            TileFormatSaveBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TileFormatSaveBox.BackColor = Color.White;
            TileFormatSaveBox.Location = new Point(168, 75);
            TileFormatSaveBox.Margin = new Padding(4);
            TileFormatSaveBox.Name = "TileFormatSaveBox";
            TileFormatSaveBox.ReadOnly = true;
            TileFormatSaveBox.Size = new Size(433, 23);
            TileFormatSaveBox.TabIndex = 0;
            TileFormatSaveBox.TextChanged += TileFormatChanged;
            TileFormatSaveBox.DoubleClick += TileFormatSave_Click;
            // 
            // TileFormatOpenBox
            // 
            TileFormatOpenBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TileFormatOpenBox.BackColor = Color.White;
            TileFormatOpenBox.Location = new Point(168, 24);
            TileFormatOpenBox.Margin = new Padding(4);
            TileFormatOpenBox.MaxLength = 3276700;
            TileFormatOpenBox.Name = "TileFormatOpenBox";
            TileFormatOpenBox.ReadOnly = true;
            TileFormatOpenBox.Size = new Size(433, 23);
            TileFormatOpenBox.TabIndex = 0;
            TileFormatOpenBox.TextChanged += TileFormatChanged;
            TileFormatOpenBox.DoubleClick += TileFormatOpen_Click;
            // 
            // pictureBox9
            // 
            pictureBox9.BackgroundImage = (Image)resources.GetObject("pictureBox9.BackgroundImage");
            pictureBox9.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox9.Location = new Point(9, 10);
            pictureBox9.Margin = new Padding(4);
            pictureBox9.Name = "pictureBox9";
            pictureBox9.Size = new Size(86, 80);
            pictureBox9.TabIndex = 25;
            pictureBox9.TabStop = false;
            // 
            // VectorPage
            // 
            VectorPage.Controls.Add(panel4);
            VectorPage.Controls.Add(VectorFileClear);
            VectorPage.Controls.Add(VectorOpen);
            VectorPage.Location = new Point(4, 26);
            VectorPage.Margin = new Padding(4);
            VectorPage.Name = "VectorPage";
            VectorPage.Padding = new Padding(4);
            VectorPage.Size = new Size(884, 270);
            VectorPage.TabIndex = 0;
            VectorPage.Text = "Vector";
            VectorPage.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(vectorFilePool);
            panel4.Location = new Point(78, 8);
            panel4.Margin = new Padding(4);
            panel4.Name = "panel4";
            panel4.Size = new Size(802, 258);
            panel4.TabIndex = 4;
            // 
            // vectorFilePool
            // 
            vectorFilePool.AllowUserToAddRows = false;
            vectorFilePool.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            vectorFilePool.BackgroundColor = SystemColors.ControlLightLight;
            vectorFilePool.BorderStyle = BorderStyle.None;
            vectorFilePool.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            vectorFilePool.Columns.AddRange(new DataGridViewColumn[] { VectorTheme, VectorURI, VectorStatus });
            vectorFilePool.Location = new Point(-1, -1);
            vectorFilePool.Margin = new Padding(4);
            vectorFilePool.Name = "vectorFilePool";
            vectorFilePool.RowTemplate.Height = 23;
            vectorFilePool.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            vectorFilePool.Size = new Size(802, 258);
            vectorFilePool.TabIndex = 3;
            vectorFilePool.CellBeginEdit += VectorFilePool_CellBeginEdit;
            vectorFilePool.CellEndEdit += VectorFilePool_CellEndEdit;
            vectorFilePool.RowsAdded += VectorFilePool_RowsAdded;
            vectorFilePool.RowsRemoved += VectorFilePool_RowsRemoved;
            // 
            // VectorTheme
            // 
            VectorTheme.HeaderText = "Theme";
            VectorTheme.Name = "VectorTheme";
            VectorTheme.Width = 80;
            // 
            // VectorURI
            // 
            VectorURI.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            VectorURI.HeaderText = "URI";
            VectorURI.Name = "VectorURI";
            VectorURI.ReadOnly = true;
            // 
            // VectorStatus
            // 
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.MiddleCenter;
            VectorStatus.DefaultCellStyle = dataGridViewCellStyle10;
            VectorStatus.HeaderText = "※";
            VectorStatus.Name = "VectorStatus";
            VectorStatus.ReadOnly = true;
            VectorStatus.ToolTipText = "status";
            VectorStatus.Width = 48;
            // 
            // helpCard
            // 
            helpCard.AutoScroll = true;
            helpCard.Controls.Add(tabControl2);
            helpCard.Location = new Point(4, 26);
            helpCard.Margin = new Padding(4);
            helpCard.Name = "helpCard";
            helpCard.Padding = new Padding(4);
            helpCard.Size = new Size(982, 579);
            helpCard.TabIndex = 1;
            helpCard.Text = "Help";
            helpCard.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            tabControl2.Alignment = TabAlignment.Bottom;
            tabControl2.Controls.Add(tabPage3);
            tabControl2.Controls.Add(tabPage4);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new Point(4, 4);
            tabControl2.Margin = new Padding(4);
            tabControl2.Multiline = true;
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(974, 571);
            tabControl2.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel17);
            tabPage3.Controls.Add(tableLayoutPanel4);
            tabPage3.Controls.Add(panel10);
            tabPage3.Controls.Add(pictureBox11);
            tabPage3.Location = new Point(4, 4);
            tabPage3.Margin = new Padding(4);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(4);
            tabPage3.Size = new Size(966, 541);
            tabPage3.TabIndex = 0;
            tabPage3.Text = "Introduction";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel17
            // 
            panel17.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel17.BackColor = Color.White;
            panel17.BorderStyle = BorderStyle.FixedSingle;
            panel17.Controls.Add(readmeTextBox);
            panel17.Location = new Point(4, 108);
            panel17.Name = "panel17";
            panel17.Padding = new Padding(3);
            panel17.Size = new Size(958, 429);
            panel17.TabIndex = 6;
            // 
            // readmeTextBox
            // 
            readmeTextBox.BackColor = Color.White;
            readmeTextBox.BorderStyle = BorderStyle.None;
            readmeTextBox.Dock = DockStyle.Fill;
            readmeTextBox.Location = new Point(3, 3);
            readmeTextBox.Name = "readmeTextBox";
            readmeTextBox.ReadOnly = true;
            readmeTextBox.Size = new Size(950, 421);
            readmeTextBox.TabIndex = 5;
            readmeTextBox.Text = "Loading ...";
            readmeTextBox.WordWrap = false;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel4.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(richTextBox1, 0, 0);
            tableLayoutPanel4.Location = new Point(87, 8);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(875, 82);
            tableLayoutPanel4.TabIndex = 4;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.White;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(6, 6);
            richTextBox1.Margin = new Padding(4);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.Size = new Size(863, 70);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "OGC.net is a free tool for reading ShapeFile, MapGIS, Excel/TXT/CSV,  converting them into GML, GeoJSON, ShapeFile, KML and GeositeXML, and pushing vector/raster to PostgreSQL database.";
            // 
            // panel10
            // 
            panel10.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel10.BackColor = Color.Gainsboro;
            panel10.Location = new Point(7, 99);
            panel10.Margin = new Padding(4);
            panel10.Name = "panel10";
            panel10.Size = new Size(955, 1);
            panel10.TabIndex = 3;
            // 
            // pictureBox11
            // 
            pictureBox11.BackgroundImage = (Image)resources.GetObject("pictureBox11.BackgroundImage");
            pictureBox11.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox11.Location = new Point(7, 8);
            pictureBox11.Margin = new Padding(4);
            pictureBox11.Name = "pictureBox11";
            pictureBox11.Size = new Size(72, 82);
            pictureBox11.TabIndex = 2;
            pictureBox11.TabStop = false;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(panel43);
            tabPage4.Controls.Add(tableLayoutPanel5);
            tabPage4.Controls.Add(panel11);
            tabPage4.Controls.Add(pictureBox12);
            tabPage4.Location = new Point(4, 4);
            tabPage4.Margin = new Padding(4);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(4);
            tabPage4.Size = new Size(966, 541);
            tabPage4.TabIndex = 1;
            tabPage4.Text = "Interface";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel43
            // 
            panel43.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel43.BackColor = Color.White;
            panel43.BorderStyle = BorderStyle.FixedSingle;
            panel43.Controls.Add(apiTextBox);
            panel43.Location = new Point(4, 108);
            panel43.Name = "panel43";
            panel43.Padding = new Padding(3);
            panel43.Size = new Size(958, 429);
            panel43.TabIndex = 8;
            // 
            // apiTextBox
            // 
            apiTextBox.BackColor = Color.White;
            apiTextBox.BorderStyle = BorderStyle.None;
            apiTextBox.Dock = DockStyle.Fill;
            apiTextBox.Location = new Point(3, 3);
            apiTextBox.Name = "apiTextBox";
            apiTextBox.ReadOnly = true;
            apiTextBox.Size = new Size(950, 421);
            apiTextBox.TabIndex = 7;
            apiTextBox.Text = "Loading ...";
            apiTextBox.WordWrap = false;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel5.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(richTextBox2, 0, 0);
            tableLayoutPanel5.Location = new Point(87, 8);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(875, 82);
            tableLayoutPanel5.TabIndex = 6;
            // 
            // richTextBox2
            // 
            richTextBox2.BackColor = Color.White;
            richTextBox2.BorderStyle = BorderStyle.None;
            richTextBox2.Dock = DockStyle.Fill;
            richTextBox2.Location = new Point(6, 6);
            richTextBox2.Margin = new Padding(4);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.ReadOnly = true;
            richTextBox2.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox2.Size = new Size(863, 70);
            richTextBox2.TabIndex = 4;
            richTextBox2.Text = "OGC.net provides a dynamic link library for embedded development.\nTips: https://github.com/CGSgeosite/OGC.net";
            // 
            // panel11
            // 
            panel11.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel11.BackColor = Color.Gainsboro;
            panel11.Location = new Point(4, 99);
            panel11.Margin = new Padding(4);
            panel11.Name = "panel11";
            panel11.Size = new Size(958, 1);
            panel11.TabIndex = 5;
            // 
            // pictureBox12
            // 
            pictureBox12.BackgroundImage = (Image)resources.GetObject("pictureBox12.BackgroundImage");
            pictureBox12.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox12.Location = new Point(7, 8);
            pictureBox12.Margin = new Padding(4);
            pictureBox12.Name = "pictureBox12";
            pictureBox12.Size = new Size(72, 82);
            pictureBox12.TabIndex = 3;
            pictureBox12.TabStop = false;
            // 
            // vectorWorker
            // 
            vectorWorker.WorkerReportsProgress = true;
            vectorWorker.WorkerSupportsCancellation = true;
            // 
            // rasterWorker
            // 
            rasterWorker.WorkerReportsProgress = true;
            rasterWorker.WorkerSupportsCancellation = true;
            // 
            // statusBar
            // 
            statusBar.BackColor = Color.WhiteSmoke;
            statusBar.GripStyle = ToolStripGripStyle.Visible;
            statusBar.Items.AddRange(new ToolStripItem[] { statusProgress, statusText });
            statusBar.Location = new Point(0, 621);
            statusBar.Name = "statusBar";
            statusBar.Padding = new Padding(1, 0, 16, 0);
            statusBar.ShowItemToolTips = true;
            statusBar.Size = new Size(1003, 22);
            statusBar.SizingGrip = false;
            statusBar.TabIndex = 18;
            // 
            // statusProgress
            // 
            statusProgress.AutoToolTip = true;
            statusProgress.Name = "statusProgress";
            statusProgress.Size = new Size(117, 23);
            statusProgress.Style = ProgressBarStyle.Continuous;
            statusProgress.Visible = false;
            // 
            // statusText
            // 
            statusText.DoubleClickEnabled = true;
            statusText.Name = "statusText";
            statusText.Size = new Size(986, 17);
            statusText.Spring = true;
            // 
            // dataGridViewImageColumn1
            // 
            dataGridViewImageColumn1.HeaderText = "Column1";
            dataGridViewImageColumn1.Image = (Image)resources.GetObject("dataGridViewImageColumn1.Image");
            dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1003, 643);
            Controls.Add(statusBar);
            Controls.Add(ogcCard);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "MainForm";
            Opacity = 0D;
            StartPosition = FormStartPosition.Manual;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)DatabaseLogIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox10).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox13).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox15).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox16).EndInit();
            ((System.ComponentModel.ISupportInitialize)FileLoadLogIcon).EndInit();
            FileSaveGroupBox.ResumeLayout(false);
            toolStrip4.ResumeLayout(false);
            toolStrip4.PerformLayout();
            groupBox4.ResumeLayout(false);
            panel44.ResumeLayout(false);
            FileSplitContainer.Panel1.ResumeLayout(false);
            FileSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FileSplitContainer).EndInit();
            FileSplitContainer.ResumeLayout(false);
            panel45.ResumeLayout(false);
            FileTabControl.ResumeLayout(false);
            FileTabPage.ResumeLayout(false);
            tableLayoutPanel11.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FileGridView).EndInit();
            panel46.ResumeLayout(false);
            PreviewTabControl.ResumeLayout(false);
            MapTabPage.ResumeLayout(false);
            tableLayoutPanel12.ResumeLayout(false);
            FileMapSplitContainer.Panel1.ResumeLayout(false);
            FileMapSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FileMapSplitContainer).EndInit();
            FileMapSplitContainer.ResumeLayout(false);
            panel12.ResumeLayout(false);
            panel18.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            gMapPanel.ResumeLayout(false);
            propertyPanel.ResumeLayout(false);
            propertyPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
            fileToolStrip.ResumeLayout(false);
            fileToolStrip.PerformLayout();
            box.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel20.ResumeLayout(false);
            ogcCard.ResumeLayout(false);
            fileCard.ResumeLayout(false);
            fileCard.PerformLayout();
            databaseCard.ResumeLayout(false);
            PostgresLinkSplitContainer.Panel1.ResumeLayout(false);
            PostgresLinkSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PostgresLinkSplitContainer).EndInit();
            PostgresLinkSplitContainer.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            panel25.ResumeLayout(false);
            panel24.ResumeLayout(false);
            DatabaseTabControl.ResumeLayout(false);
            DatabasePage.ResumeLayout(false);
            dataGridPanel.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            panel31.ResumeLayout(false);
            panel30.ResumeLayout(false);
            panel30.PerformLayout();
            panel29.ResumeLayout(false);
            panel28.ResumeLayout(false);
            panel26.ResumeLayout(false);
            panel27.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DatabaseGridView).EndInit();
            CatalogPage.ResumeLayout(false);
            groupBox9.ResumeLayout(false);
            dataCards.ResumeLayout(false);
            RasterPage.ResumeLayout(false);
            PushPanel.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            method.ResumeLayout(false);
            method.PerformLayout();
            tilesource.ResumeLayout(false);
            LocalTilePage.ResumeLayout(false);
            LocalTilePage.PerformLayout();
            FormatGroupBox.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            panel32.ResumeLayout(false);
            panel32.PerformLayout();
            panel33.ResumeLayout(false);
            panel33.PerformLayout();
            panel34.ResumeLayout(false);
            panel34.PerformLayout();
            panel35.ResumeLayout(false);
            panel35.PerformLayout();
            panel36.ResumeLayout(false);
            panel36.PerformLayout();
            panel37.ResumeLayout(false);
            panel37.PerformLayout();
            RemoteTilePage.ResumeLayout(false);
            RemoteTilePage.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            panel38.ResumeLayout(false);
            panel38.PerformLayout();
            panel39.ResumeLayout(false);
            panel39.PerformLayout();
            panel40.ResumeLayout(false);
            panel40.PerformLayout();
            panel41.ResumeLayout(false);
            panel41.PerformLayout();
            ModelPage.ResumeLayout(false);
            ModelPage.PerformLayout();
            tableLayoutPanel9.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            TileConvertPage.ResumeLayout(false);
            TileConvertPage.PerformLayout();
            tableLayoutPanel8.ResumeLayout(false);
            panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox9).EndInit();
            VectorPage.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)vectorFilePool).EndInit();
            helpCard.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel17.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
            tabPage4.ResumeLayout(false);
            panel43.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox12).EndInit();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ToolTip OGCtoolTip;
        private System.ComponentModel.BackgroundWorker fileWorker;
        private System.Windows.Forms.Panel box;
        private System.Windows.Forms.TabControl ogcCard;
        private System.Windows.Forms.TabPage fileCard;
        private System.Windows.Forms.TabPage helpCard;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TabPage databaseCard;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox GeositeServerPassword;
        private System.Windows.Forms.TextBox GeositeServerUser;
        private System.Windows.Forms.TextBox GeositeServerUrl;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button GeositeServerLink;
        private System.Windows.Forms.ProgressBar DatabaseProgressBar;
        private System.Windows.Forms.Button deleteTree;
        private System.Windows.Forms.Button lastPage;
        private System.Windows.Forms.Button nextPage;
        private System.Windows.Forms.Button previousPage;
        private System.Windows.Forms.Button firstPage;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView DatabaseGridView;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.TabControl dataCards;
        private System.Windows.Forms.TabPage VectorPage;
        private System.Windows.Forms.TabPage RasterPage;
        private System.Windows.Forms.TextBox pagesBox;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button PostgresRun;
        private System.Windows.Forms.Button VectorOpen;
        private System.Windows.Forms.DataGridView vectorFilePool;
        private System.Windows.Forms.Button VectorFileClear;
        private System.ComponentModel.BackgroundWorker vectorWorker;
        private System.Windows.Forms.DataGridViewTextBoxColumn VectorTheme;
        private System.Windows.Forms.DataGridViewTextBoxColumn VectorURI;
        private System.Windows.Forms.DataGridViewTextBoxColumn VectorStatus;
        private System.Windows.Forms.TabControl tilesource;
        private System.Windows.Forms.TabPage LocalTilePage;
        private System.Windows.Forms.Button localTileOpen;
        private System.Windows.Forms.GroupBox FormatGroupBox;
        private System.Windows.Forms.RadioButton FormatRaster;
        private System.Windows.Forms.RadioButton FormatStandard;
        private System.Windows.Forms.RadioButton FormatDeepZoom;
        private System.Windows.Forms.RadioButton FormatTMS;
        private System.Windows.Forms.RadioButton FormatArcGIS;
        private System.Windows.Forms.RadioButton FormatMapcruncher;
        private System.Windows.Forms.TextBox localTileFolder;
        private System.Windows.Forms.TabPage RemoteTilePage;
        private System.Windows.Forms.TextBox tilewebapi;
        private System.Windows.Forms.TextBox wmtsSouth;
        private System.Windows.Forms.TextBox wmtsWest;
        private System.Windows.Forms.TextBox wmtsEast;
        private System.Windows.Forms.TextBox wmtsNorth;
        private System.Windows.Forms.TabPage TileConvertPage;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.RadioButton maptilertoogc;
        private System.Windows.Forms.RadioButton mapcrunchertoogc;
        private System.Windows.Forms.RadioButton ogctomapcruncher;
        private System.Windows.Forms.RadioButton ogctomaptiler;
        private System.Windows.Forms.Button tileconvert;
        private System.Windows.Forms.TextBox TileFormatSaveBox;
        private System.Windows.Forms.TextBox TileFormatOpenBox;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.Button TileFormatSave;
        private System.Windows.Forms.Button TileFormatOpen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox themeNameBox;
        private System.Windows.Forms.GroupBox method;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox tileLevels;
        private System.Windows.Forms.CheckBox EPSG4326;
        private System.Windows.Forms.CheckBox PostgresLight;
        private System.Windows.Forms.TabPage ModelPage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox rasterTileSize;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox nodatabox;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.TextBox ModelOpenTextBox;
        private System.ComponentModel.BackgroundWorker rasterWorker;
        private System.Windows.Forms.Panel PushPanel;
        private System.Windows.Forms.Button ModelOpen;
        private System.Windows.Forms.ComboBox wmtsMinZoom;
        private System.Windows.Forms.ComboBox wmtsMaxZoom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox wmtsSpider;
        private System.Windows.Forms.TextBox wmtsSize;
        private System.Windows.Forms.Button deleteForest;
        private System.Windows.Forms.CheckBox UpdateBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox subdomainsBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.Button ModelSave;
        private System.Windows.Forms.Panel dataGridPanel;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripProgressBar statusProgress;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.GroupBox FileSaveGroupBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button ReClean;
        private System.Windows.Forms.PictureBox DatabaseLogIcon;
        private System.Windows.Forms.ComboBox MIMEBox;
        private System.Windows.Forms.ComboBox rankList;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button Reindex;
        private Panel panel13;
        private GroupBox groupBox9;
        private Panel panel7;
        private Panel panel8;
        private ToolStrip fileToolStrip;
        private ToolStripButton vectorOpenButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton mapgisButton;
        private ToolStripButton arcgisIconButton;
        private ToolStripButton tableIconButton;
        private ToolStripButton gmlIconButton;
        private ToolStripButton geojsonIconButton;
        private ToolStripButton geositeIconButton;
        private ToolStripButton kmlIconButton;
        private DataGridView FileGridView;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton cleanIconButton;
        private ToolStrip toolStrip4;
        private ToolStripButton vectorSaveButton;
        private ToolStripTextBox toolStripTextBox2;
        //private ToolStripSpringTextBox vectorTargetFile;
        private ToolStripButton FileRunButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripComboBox SaveAsFormat;
        private SplitContainer FileSplitContainer;
        private SplitContainer FileMapSplitContainer;
        private Panel gMapPanel;
        public GMap.NET.WindowsForms.GMapControl MapBox;
        private Panel propertyPanel;
        private Panel panel12;
        private Panel panel18;
        private ToolStrip toolStrip1;
        private DataGridViewTextBoxColumn FilePath;
        private DataGridViewButtonColumn FilePreview;
        private PictureBox FileLoadLogIcon;
        private ToolStripButton ZoomToLayer;
        private TabControl FileTabControl;
        private TabPage FileTabPage;
        private TabControl PreviewTabControl;
        private TabPage MapTabPage;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripLabel ZoomLevelLabel;
        private ToolStripDropDownButton PositionBox;
        private ToolStripMenuItem DegMenuItem;
        private ToolStripMenuItem DmsMenuItem;
        private ToolStripMenuItem CGCS2000MenuItem;
        private ToolStripDropDownButton MapProviderDropDown;
        private ToolStripSeparator toolStripSeparator4;
        private ProgressBar TileLoadProgressBar;
        private ToolStripMenuItem BeijingMenuItem;
        private ToolStripMenuItem XianMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem WebMercatorMenuItem;
        private ToolStripButton ImageMaker;
        private ToolStripButton ClearLayers;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripSeparator toolStripSeparator8;
        private Panel panel22;
        private Label label8;
        private PictureBox pictureBox8;
        private RichTextBox MapBoxProperty;
        private Panel panel3;
        private ProgressBar FilePreviewProgressBar;
        private Panel panel21;
        private ListBox FileLoadLog;
        private Panel panel20;
        private Panel panel23;
        private SplitContainer PostgresLinkSplitContainer;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel31;
        private Panel panel30;
        private Panel panel29;
        private Panel panel28;
        private Panel panel26;
        private Panel panel27;
        private TableLayoutPanel tableLayoutPanel3;
        private Panel panel32;
        private Panel panel33;
        private Panel panel34;
        private Panel panel35;
        private Panel panel36;
        private Panel panel37;
        private TableLayoutPanel tableLayoutPanel4;
        private TableLayoutPanel tableLayoutPanel5;
        private RichTextBox FormatStandardBox;
        private RichTextBox FormatTMSBox;
        private RichTextBox FormatMapcruncherBox;
        private RichTextBox FormatArcGISBox;
        private RichTextBox FormatDeepZoomBox;
        private RichTextBox FormatRasterBox;
        private RichTextBox wmtsTipBox;
        private GroupBox groupBox5;
        private TableLayoutPanel tableLayoutPanel6;
        private Panel panel38;
        private PictureBox pictureBox13;
        private Panel panel39;
        private PictureBox pictureBox15;
        private Panel panel40;
        private PictureBox pictureBox10;
        private Panel panel41;
        private PictureBox pictureBox6;
        private TableLayoutPanel tableLayoutPanel7;
        private Panel panel42;
        private PictureBox pictureBox16;
        private RichTextBox convertTipBox;
        private Panel panel14;
        private RichTextBox modelTipBox;
        private TableLayoutPanel tableLayoutPanel10;
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel8;
        private ToolStripSpringTextBox vectorTargetFile;
        private Panel panel15;
        private Panel panel16;
        private RichTextBox readmeTextBox;
        private RichTextBox apiTextBox;
        private Panel panel17;
        private Panel panel43;
        private DataGridViewTextBoxColumn ThemeName;
        private DataGridViewTextBoxColumn ThemeRank;
        private DataGridViewImageColumn ThemeStatus;
        private DataGridViewImageColumn ThemeType;
        private Panel panel44;
        private Panel panel45;
        private TableLayoutPanel tableLayoutPanel11;
        private Panel panel46;
        private TableLayoutPanel tableLayoutPanel12;
        private ListBox DatabaseLog;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabControl DatabaseTabControl;
        private TabPage DatabasePage;
        private TabPage CatalogPage;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel25;
        private Panel panel24;
        private Panel panel9;
        private TreeView CatalogTreeView;
    }
}

