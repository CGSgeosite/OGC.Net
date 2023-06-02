namespace Geosite
{
    partial class TopologyCheckerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TopologyCheckerForm));
            TopologyRun = new Button();
            statusStrip1 = new StatusStrip();
            TopologyProgressBar = new ToolStripProgressBar();
            TopologyStatusLabel = new ToolStripStatusLabel();
            panel1 = new Panel();
            toolTip1 = new ToolTip(components);
            label7 = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            label11 = new Label();
            label9 = new Label();
            label5 = new Label();
            label3 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label15 = new Label();
            CheckBoxDangle = new CheckBox();
            panel8 = new Panel();
            panel7 = new Panel();
            panel6 = new Panel();
            panel5 = new Panel();
            panel4 = new Panel();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            CheckBoxPseudo = new CheckBox();
            CheckBoxCoincide = new CheckBox();
            CheckBoxOverlay = new CheckBox();
            CheckBoxIntersection = new CheckBox();
            label2 = new Label();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox6 = new PictureBox();
            LabelDangle = new Label();
            LabelPseudo = new Label();
            LabelCoincide = new Label();
            LabelOverlay = new Label();
            LabelIntersection = new Label();
            panel3 = new Panel();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // TopologyRun
            // 
            TopologyRun.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TopologyRun.Image = Properties.Resources.run;
            TopologyRun.Location = new Point(23, 62);
            TopologyRun.Name = "TopologyRun";
            TopologyRun.Size = new Size(42, 67);
            TopologyRun.TabIndex = 0;
            toolTip1.SetToolTip(TopologyRun, "Start");
            TopologyRun.UseVisualStyleBackColor = true;
            TopologyRun.Click += TopologyRun_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = Color.Transparent;
            statusStrip1.Items.AddRange(new ToolStripItem[] { TopologyProgressBar, TopologyStatusLabel });
            statusStrip1.Location = new Point(0, 293);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(503, 22);
            statusStrip1.SizingGrip = false;
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // TopologyProgressBar
            // 
            TopologyProgressBar.AutoSize = false;
            TopologyProgressBar.MarqueeAnimationSpeed = 0;
            TopologyProgressBar.Name = "TopologyProgressBar";
            TopologyProgressBar.Size = new Size(100, 16);
            TopologyProgressBar.Step = 1;
            TopologyProgressBar.Visible = false;
            // 
            // TopologyStatusLabel
            // 
            TopologyStatusLabel.Name = "TopologyStatusLabel";
            TopologyStatusLabel.Size = new Size(488, 17);
            TopologyStatusLabel.Spring = true;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.FromArgb(189, 189, 189);
            panel1.Location = new Point(0, 286);
            panel1.Name = "panel1";
            panel1.Size = new Size(503, 1);
            panel1.TabIndex = 9;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Dock = DockStyle.Fill;
            label7.Location = new Point(78, 98);
            label7.Name = "label7";
            label7.Size = new Size(94, 29);
            label7.TabIndex = 1;
            label7.Text = "Coincide";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            toolTip1.SetToolTip(label7, "Two nodes have the same geometry.");
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.Location = new Point(61, 9);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(429, 74);
            label1.TabIndex = 27;
            label1.Text = "Topology check only applies to features within MapView:\r\n1. Point: overlap.\r\n2. Polyline: dangle pseudo coincide overlap intersection.\r\n3. Polygon: dangle pseudo coincide overlap intersection.";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Location = new Point(9, 9);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(44, 31);
            pictureBox1.TabIndex = 26;
            pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.FromArgb(189, 189, 189);
            panel2.Location = new Point(0, 86);
            panel2.Name = "panel2";
            panel2.Size = new Size(503, 1);
            panel2.TabIndex = 28;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Dock = DockStyle.Fill;
            label11.Location = new Point(78, 158);
            label11.Name = "label11";
            label11.Size = new Size(94, 30);
            label11.TabIndex = 1;
            label11.Text = "Intersection";
            label11.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Location = new Point(78, 128);
            label9.Name = "label9";
            label9.Size = new Size(94, 29);
            label9.TabIndex = 1;
            label9.Text = "Overlay";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(78, 68);
            label5.Name = "label5";
            label5.Size = new Size(94, 29);
            label5.TabIndex = 1;
            label5.Text = "Pseudo";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(78, 38);
            label3.Name = "label3";
            label3.Size = new Size(94, 29);
            label3.TabIndex = 1;
            label3.Text = "Dangle";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 87F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(panel3, 1, 0);
            tableLayoutPanel1.Location = new Point(9, 89);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(488, 195);
            tableLayoutPanel1.TabIndex = 30;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(label15, 0, 0);
            tableLayoutPanel2.Controls.Add(CheckBoxDangle, 0, 1);
            tableLayoutPanel2.Controls.Add(label11, 2, 5);
            tableLayoutPanel2.Controls.Add(panel8, 1, 5);
            tableLayoutPanel2.Controls.Add(label3, 2, 1);
            tableLayoutPanel2.Controls.Add(panel7, 1, 4);
            tableLayoutPanel2.Controls.Add(panel6, 1, 3);
            tableLayoutPanel2.Controls.Add(panel5, 1, 2);
            tableLayoutPanel2.Controls.Add(panel4, 1, 1);
            tableLayoutPanel2.Controls.Add(label12, 1, 0);
            tableLayoutPanel2.Controls.Add(label9, 2, 4);
            tableLayoutPanel2.Controls.Add(label13, 2, 0);
            tableLayoutPanel2.Controls.Add(label14, 3, 0);
            tableLayoutPanel2.Controls.Add(label5, 2, 2);
            tableLayoutPanel2.Controls.Add(label7, 2, 3);
            tableLayoutPanel2.Controls.Add(CheckBoxPseudo, 0, 2);
            tableLayoutPanel2.Controls.Add(CheckBoxCoincide, 0, 3);
            tableLayoutPanel2.Controls.Add(CheckBoxOverlay, 0, 4);
            tableLayoutPanel2.Controls.Add(CheckBoxIntersection, 0, 5);
            tableLayoutPanel2.Controls.Add(label2, 4, 0);
            tableLayoutPanel2.Controls.Add(pictureBox2, 3, 1);
            tableLayoutPanel2.Controls.Add(pictureBox3, 3, 2);
            tableLayoutPanel2.Controls.Add(pictureBox4, 3, 3);
            tableLayoutPanel2.Controls.Add(pictureBox5, 3, 4);
            tableLayoutPanel2.Controls.Add(pictureBox6, 3, 5);
            tableLayoutPanel2.Controls.Add(LabelDangle, 4, 1);
            tableLayoutPanel2.Controls.Add(LabelPseudo, 4, 2);
            tableLayoutPanel2.Controls.Add(LabelCoincide, 4, 3);
            tableLayoutPanel2.Controls.Add(LabelOverlay, 4, 4);
            tableLayoutPanel2.Controls.Add(LabelIntersection, 4, 5);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16F));
            tableLayoutPanel2.Size = new Size(395, 189);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.BackColor = Color.WhiteSmoke;
            label15.Dock = DockStyle.Fill;
            label15.Location = new Point(4, 1);
            label15.Name = "label15";
            label15.Size = new Size(26, 36);
            label15.TabIndex = 31;
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CheckBoxDangle
            // 
            CheckBoxDangle.AutoSize = true;
            CheckBoxDangle.CheckAlign = ContentAlignment.MiddleCenter;
            CheckBoxDangle.Dock = DockStyle.Fill;
            CheckBoxDangle.Location = new Point(4, 41);
            CheckBoxDangle.Name = "CheckBoxDangle";
            CheckBoxDangle.Size = new Size(26, 23);
            CheckBoxDangle.TabIndex = 31;
            CheckBoxDangle.TextAlign = ContentAlignment.MiddleCenter;
            CheckBoxDangle.UseVisualStyleBackColor = true;
            CheckBoxDangle.CheckedChanged += TopologyCheck_Click;
            // 
            // panel8
            // 
            panel8.BackColor = Color.Aqua;
            panel8.Dock = DockStyle.Fill;
            panel8.Location = new Point(37, 161);
            panel8.Name = "panel8";
            panel8.Size = new Size(34, 24);
            panel8.TabIndex = 31;
            // 
            // panel7
            // 
            panel7.BackColor = Color.Fuchsia;
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(37, 131);
            panel7.Name = "panel7";
            panel7.Size = new Size(34, 23);
            panel7.TabIndex = 31;
            // 
            // panel6
            // 
            panel6.BackColor = Color.Yellow;
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(37, 101);
            panel6.Name = "panel6";
            panel6.Size = new Size(34, 23);
            panel6.TabIndex = 31;
            // 
            // panel5
            // 
            panel5.BackColor = Color.Green;
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(37, 71);
            panel5.Name = "panel5";
            panel5.Size = new Size(34, 23);
            panel5.TabIndex = 31;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Red;
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(37, 41);
            panel4.Name = "panel4";
            panel4.Size = new Size(34, 23);
            panel4.TabIndex = 31;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.BackColor = Color.WhiteSmoke;
            label12.Dock = DockStyle.Fill;
            label12.Location = new Point(37, 1);
            label12.Name = "label12";
            label12.Size = new Size(34, 36);
            label12.TabIndex = 32;
            label12.Text = "Icon";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.BackColor = Color.WhiteSmoke;
            label13.Dock = DockStyle.Fill;
            label13.Location = new Point(78, 1);
            label13.Name = "label13";
            label13.Size = new Size(94, 36);
            label13.TabIndex = 33;
            label13.Text = "Type";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.BackColor = Color.WhiteSmoke;
            label14.Dock = DockStyle.Fill;
            label14.Location = new Point(179, 1);
            label14.Name = "label14";
            label14.Size = new Size(52, 36);
            label14.TabIndex = 34;
            label14.Text = "Sample";
            label14.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CheckBoxPseudo
            // 
            CheckBoxPseudo.AutoSize = true;
            CheckBoxPseudo.CheckAlign = ContentAlignment.MiddleCenter;
            CheckBoxPseudo.Dock = DockStyle.Fill;
            CheckBoxPseudo.Location = new Point(4, 71);
            CheckBoxPseudo.Name = "CheckBoxPseudo";
            CheckBoxPseudo.Size = new Size(26, 23);
            CheckBoxPseudo.TabIndex = 35;
            CheckBoxPseudo.TextAlign = ContentAlignment.MiddleCenter;
            CheckBoxPseudo.UseVisualStyleBackColor = true;
            CheckBoxPseudo.CheckedChanged += TopologyCheck_Click;
            // 
            // CheckBoxCoincide
            // 
            CheckBoxCoincide.AutoSize = true;
            CheckBoxCoincide.CheckAlign = ContentAlignment.MiddleCenter;
            CheckBoxCoincide.Dock = DockStyle.Fill;
            CheckBoxCoincide.Location = new Point(4, 101);
            CheckBoxCoincide.Name = "CheckBoxCoincide";
            CheckBoxCoincide.Size = new Size(26, 23);
            CheckBoxCoincide.TabIndex = 36;
            CheckBoxCoincide.TextAlign = ContentAlignment.MiddleCenter;
            CheckBoxCoincide.UseVisualStyleBackColor = true;
            CheckBoxCoincide.CheckedChanged += TopologyCheck_Click;
            // 
            // CheckBoxOverlay
            // 
            CheckBoxOverlay.AutoSize = true;
            CheckBoxOverlay.CheckAlign = ContentAlignment.MiddleCenter;
            CheckBoxOverlay.Dock = DockStyle.Fill;
            CheckBoxOverlay.Location = new Point(4, 131);
            CheckBoxOverlay.Name = "CheckBoxOverlay";
            CheckBoxOverlay.Size = new Size(26, 23);
            CheckBoxOverlay.TabIndex = 37;
            CheckBoxOverlay.TextAlign = ContentAlignment.MiddleCenter;
            CheckBoxOverlay.UseVisualStyleBackColor = true;
            CheckBoxOverlay.CheckedChanged += TopologyCheck_Click;
            // 
            // CheckBoxIntersection
            // 
            CheckBoxIntersection.AutoSize = true;
            CheckBoxIntersection.CheckAlign = ContentAlignment.MiddleCenter;
            CheckBoxIntersection.Dock = DockStyle.Fill;
            CheckBoxIntersection.Location = new Point(4, 161);
            CheckBoxIntersection.Name = "CheckBoxIntersection";
            CheckBoxIntersection.Size = new Size(26, 24);
            CheckBoxIntersection.TabIndex = 38;
            CheckBoxIntersection.TextAlign = ContentAlignment.MiddleCenter;
            CheckBoxIntersection.UseVisualStyleBackColor = true;
            CheckBoxIntersection.CheckedChanged += TopologyCheck_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.WhiteSmoke;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(238, 1);
            label2.Name = "label2";
            label2.Size = new Size(153, 36);
            label2.TabIndex = 39;
            label2.Text = "Count";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Center;
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Location = new Point(179, 41);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(52, 23);
            pictureBox2.TabIndex = 40;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Center;
            pictureBox3.Dock = DockStyle.Fill;
            pictureBox3.Location = new Point(179, 71);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(52, 23);
            pictureBox3.TabIndex = 41;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.BackgroundImage = (Image)resources.GetObject("pictureBox4.BackgroundImage");
            pictureBox4.BackgroundImageLayout = ImageLayout.Center;
            pictureBox4.Dock = DockStyle.Fill;
            pictureBox4.Location = new Point(179, 101);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(52, 23);
            pictureBox4.TabIndex = 42;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.BackgroundImage = (Image)resources.GetObject("pictureBox5.BackgroundImage");
            pictureBox5.BackgroundImageLayout = ImageLayout.Center;
            pictureBox5.Dock = DockStyle.Fill;
            pictureBox5.Location = new Point(179, 131);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(52, 23);
            pictureBox5.TabIndex = 43;
            pictureBox5.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.BackgroundImage = (Image)resources.GetObject("pictureBox6.BackgroundImage");
            pictureBox6.BackgroundImageLayout = ImageLayout.Center;
            pictureBox6.Dock = DockStyle.Fill;
            pictureBox6.Location = new Point(179, 161);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(52, 24);
            pictureBox6.TabIndex = 44;
            pictureBox6.TabStop = false;
            // 
            // LabelDangle
            // 
            LabelDangle.AutoSize = true;
            LabelDangle.Dock = DockStyle.Fill;
            LabelDangle.Location = new Point(238, 38);
            LabelDangle.Name = "LabelDangle";
            LabelDangle.Size = new Size(153, 29);
            LabelDangle.TabIndex = 45;
            LabelDangle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelPseudo
            // 
            LabelPseudo.AutoSize = true;
            LabelPseudo.Dock = DockStyle.Fill;
            LabelPseudo.Location = new Point(238, 68);
            LabelPseudo.Name = "LabelPseudo";
            LabelPseudo.Size = new Size(153, 29);
            LabelPseudo.TabIndex = 45;
            LabelPseudo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelCoincide
            // 
            LabelCoincide.AutoSize = true;
            LabelCoincide.Dock = DockStyle.Fill;
            LabelCoincide.Location = new Point(238, 98);
            LabelCoincide.Name = "LabelCoincide";
            LabelCoincide.Size = new Size(153, 29);
            LabelCoincide.TabIndex = 45;
            LabelCoincide.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelOverlay
            // 
            LabelOverlay.AutoSize = true;
            LabelOverlay.Dock = DockStyle.Fill;
            LabelOverlay.Location = new Point(238, 128);
            LabelOverlay.Name = "LabelOverlay";
            LabelOverlay.Size = new Size(153, 29);
            LabelOverlay.TabIndex = 45;
            LabelOverlay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LabelIntersection
            // 
            LabelIntersection.AutoSize = true;
            LabelIntersection.Dock = DockStyle.Fill;
            LabelIntersection.Location = new Point(238, 158);
            LabelIntersection.Name = "LabelIntersection";
            LabelIntersection.Size = new Size(153, 30);
            LabelIntersection.TabIndex = 45;
            LabelIntersection.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            panel3.Controls.Add(TopologyRun);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(404, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(81, 189);
            panel3.TabIndex = 30;
            // 
            // TopologyCheckerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(503, 315);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(panel2);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(panel1);
            Controls.Add(statusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TopologyCheckerForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "Topology Checker";
            TopMost = true;
            FormClosing += TopologyCheckerForm_FormClosing;
            Load += TopologyCheckerForm_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            panel3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button TopologyRun;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar TopologyProgressBar;
        private ToolStripStatusLabel TopologyStatusLabel;
        private Panel panel1;
        private ToolTip toolTip1;
        private Label label1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Label label5;
        private Label label3;
        private Label label7;
        private Label label9;
        private Label label11;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel3;
        private TableLayoutPanel tableLayoutPanel2;
        private CheckBox CheckBoxDangle;
        private Panel panel4;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Panel panel5;
        private Panel panel6;
        private Panel panel8;
        private Panel panel7;
        private CheckBox CheckBoxPseudo;
        private CheckBox CheckBoxCoincide;
        private CheckBox CheckBoxOverlay;
        private CheckBox CheckBoxIntersection;
        private Label label2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
        private Label LabelDangle;
        private Label LabelPseudo;
        private Label LabelCoincide;
        private Label LabelOverlay;
        private Label LabelIntersection;
    }
}