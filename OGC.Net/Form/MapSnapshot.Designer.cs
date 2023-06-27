namespace Geosite
{
    partial class MapSnapshot
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapSnapshot));
            SnapshotToolStrip = new ToolStrip();
            SnapshotSave = new ToolStripButton();
            SnapshotFileTextBox = new ToolStripSpringTextBox();
            toolStripSeparator1 = new ToolStripSeparator();
            SnapshotZoomLabel = new ToolStripLabel();
            SnapshotZoom = new ToolStripDropDownButton();
            toolStripSeparator2 = new ToolStripSeparator();
            SnapshotRun = new ToolStripButton();
            SnapshotPicture = new PictureBox();
            panel1 = new Panel();
            panel2 = new Panel();
            SnapshotPanel = new Panel();
            panel4 = new Panel();
            panel5 = new Panel();
            SnapshotTools = new ToolStrip();
            toolStripLabel2 = new ToolStripLabel();
            toolStripLabel1 = new ToolStripLabel();
            toolStripSeparator4 = new ToolStripSeparator();
            EPSG3857Switch = new ToolStripLabel();
            EPSG4326Switch = new ToolStripLabel();
            SnapshotRefresh = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            statusStrip1 = new StatusStrip();
            SnapshotProgressBar = new ToolStripProgressBar();
            SnapshotStatusLabel = new ToolStripStatusLabel();
            panel3 = new Panel();
            SnapshotTop = new TextBox();
            SnapshotLeft = new TextBox();
            toolTip1 = new ToolTip(components);
            SnapshotBottom = new TextBox();
            SnapshotRight = new TextBox();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            SnapshotAreaPanel = new Panel();
            SnapshotToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SnapshotPicture).BeginInit();
            SnapshotPanel.SuspendLayout();
            panel5.SuspendLayout();
            SnapshotTools.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SnapshotAreaPanel.SuspendLayout();
            SuspendLayout();
            // 
            // SnapshotToolStrip
            // 
            resources.ApplyResources(SnapshotToolStrip, "SnapshotToolStrip");
            SnapshotToolStrip.BackColor = Color.Transparent;
            SnapshotToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            SnapshotToolStrip.Items.AddRange(new ToolStripItem[] { SnapshotSave, SnapshotFileTextBox, toolStripSeparator1, SnapshotZoomLabel, SnapshotZoom, toolStripSeparator2, SnapshotRun });
            SnapshotToolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            SnapshotToolStrip.Name = "SnapshotToolStrip";
            SnapshotToolStrip.RenderMode = ToolStripRenderMode.System;
            // 
            // SnapshotSave
            // 
            resources.ApplyResources(SnapshotSave, "SnapshotSave");
            SnapshotSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SnapshotSave.Margin = new Padding(2);
            SnapshotSave.Name = "SnapshotSave";
            SnapshotSave.Padding = new Padding(6, 0, 6, 0);
            SnapshotSave.Click += SnapshotSave_Click;
            // 
            // SnapshotFileTextBox
            // 
            SnapshotFileTextBox.BackColor = Color.White;
            SnapshotFileTextBox.BorderStyle = BorderStyle.FixedSingle;
            SnapshotFileTextBox.Margin = new Padding(1, 0, 6, 0);
            SnapshotFileTextBox.Name = "SnapshotFileTextBox";
            SnapshotFileTextBox.ReadOnly = true;
            resources.ApplyResources(SnapshotFileTextBox, "SnapshotFileTextBox");
            SnapshotFileTextBox.Click += SnapshotSave_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // SnapshotZoomLabel
            // 
            SnapshotZoomLabel.Name = "SnapshotZoomLabel";
            resources.ApplyResources(SnapshotZoomLabel, "SnapshotZoomLabel");
            // 
            // SnapshotZoom
            // 
            SnapshotZoom.AutoToolTip = false;
            SnapshotZoom.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(SnapshotZoom, "SnapshotZoom");
            SnapshotZoom.Name = "SnapshotZoom";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // SnapshotRun
            // 
            SnapshotRun.Alignment = ToolStripItemAlignment.Right;
            resources.ApplyResources(SnapshotRun, "SnapshotRun");
            SnapshotRun.AutoToolTip = false;
            SnapshotRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SnapshotRun.Image = Properties.Resources.run;
            SnapshotRun.Name = "SnapshotRun";
            SnapshotRun.Padding = new Padding(6, 0, 6, 0);
            SnapshotRun.Click += SnapshotRun_Click;
            // 
            // SnapshotPicture
            // 
            SnapshotPicture.BackColor = Color.Transparent;
            resources.ApplyResources(SnapshotPicture, "SnapshotPicture");
            SnapshotPicture.Name = "SnapshotPicture";
            SnapshotPicture.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.BackColor = Color.FromArgb(189, 189, 189);
            panel1.Name = "panel1";
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.BackColor = Color.FromArgb(189, 189, 189);
            panel2.Name = "panel2";
            // 
            // SnapshotPanel
            // 
            resources.ApplyResources(SnapshotPanel, "SnapshotPanel");
            SnapshotPanel.BackColor = Color.Transparent;
            SnapshotPanel.BorderStyle = BorderStyle.FixedSingle;
            SnapshotPanel.Controls.Add(SnapshotPicture);
            SnapshotPanel.Name = "SnapshotPanel";
            // 
            // panel4
            // 
            resources.ApplyResources(panel4, "panel4");
            panel4.BackColor = Color.FromArgb(189, 189, 189);
            panel4.Name = "panel4";
            // 
            // panel5
            // 
            resources.ApplyResources(panel5, "panel5");
            panel5.Controls.Add(SnapshotToolStrip);
            panel5.Name = "panel5";
            // 
            // SnapshotTools
            // 
            SnapshotTools.BackColor = Color.Transparent;
            SnapshotTools.GripStyle = ToolStripGripStyle.Hidden;
            SnapshotTools.Items.AddRange(new ToolStripItem[] { toolStripLabel2, toolStripLabel1, toolStripSeparator4, EPSG3857Switch, EPSG4326Switch, SnapshotRefresh, toolStripSeparator3 });
            resources.ApplyResources(SnapshotTools, "SnapshotTools");
            SnapshotTools.Name = "SnapshotTools";
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(toolStripLabel2, "toolStripLabel2");
            toolStripLabel2.Name = "toolStripLabel2";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(toolStripLabel1, "toolStripLabel1");
            toolStripLabel1.Name = "toolStripLabel1";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Margin = new Padding(0, 0, 6, 0);
            toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(toolStripSeparator4, "toolStripSeparator4");
            // 
            // EPSG3857Switch
            // 
            resources.ApplyResources(EPSG3857Switch, "EPSG3857Switch");
            EPSG3857Switch.Image = Properties.Resources.checkedbox;
            EPSG3857Switch.IsLink = true;
            EPSG3857Switch.LinkBehavior = LinkBehavior.NeverUnderline;
            EPSG3857Switch.LinkColor = Color.Black;
            EPSG3857Switch.Name = "EPSG3857Switch";
            EPSG3857Switch.Tag = "1";
            // 
            // EPSG4326Switch
            // 
            EPSG4326Switch.Image = Properties.Resources.checkedbox;
            resources.ApplyResources(EPSG4326Switch, "EPSG4326Switch");
            EPSG4326Switch.IsLink = true;
            EPSG4326Switch.LinkBehavior = LinkBehavior.NeverUnderline;
            EPSG4326Switch.LinkColor = Color.Black;
            EPSG4326Switch.Name = "EPSG4326Switch";
            EPSG4326Switch.Tag = "1";
            EPSG4326Switch.Click += EPSG4326Switch_Click;
            // 
            // SnapshotRefresh
            // 
            SnapshotRefresh.Alignment = ToolStripItemAlignment.Right;
            resources.ApplyResources(SnapshotRefresh, "SnapshotRefresh");
            SnapshotRefresh.AutoToolTip = false;
            SnapshotRefresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SnapshotRefresh.Name = "SnapshotRefresh";
            SnapshotRefresh.Padding = new Padding(6, 0, 6, 0);
            SnapshotRefresh.Click += SnapshotRefresh_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = Color.Transparent;
            statusStrip1.Items.AddRange(new ToolStripItem[] { SnapshotProgressBar, SnapshotStatusLabel });
            resources.ApplyResources(statusStrip1, "statusStrip1");
            statusStrip1.Name = "statusStrip1";
            statusStrip1.SizingGrip = false;
            // 
            // SnapshotProgressBar
            // 
            resources.ApplyResources(SnapshotProgressBar, "SnapshotProgressBar");
            SnapshotProgressBar.Name = "SnapshotProgressBar";
            SnapshotProgressBar.Step = 1;
            // 
            // SnapshotStatusLabel
            // 
            SnapshotStatusLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(SnapshotStatusLabel, "SnapshotStatusLabel");
            SnapshotStatusLabel.Name = "SnapshotStatusLabel";
            SnapshotStatusLabel.Spring = true;
            // 
            // panel3
            // 
            resources.ApplyResources(panel3, "panel3");
            panel3.BackColor = Color.FromArgb(189, 189, 189);
            panel3.Name = "panel3";
            // 
            // SnapshotTop
            // 
            resources.ApplyResources(SnapshotTop, "SnapshotTop");
            SnapshotTop.Name = "SnapshotTop";
            toolTip1.SetToolTip(SnapshotTop, resources.GetString("SnapshotTop.ToolTip"));
            // 
            // SnapshotLeft
            // 
            resources.ApplyResources(SnapshotLeft, "SnapshotLeft");
            SnapshotLeft.Name = "SnapshotLeft";
            toolTip1.SetToolTip(SnapshotLeft, resources.GetString("SnapshotLeft.ToolTip"));
            // 
            // SnapshotBottom
            // 
            resources.ApplyResources(SnapshotBottom, "SnapshotBottom");
            SnapshotBottom.Name = "SnapshotBottom";
            toolTip1.SetToolTip(SnapshotBottom, resources.GetString("SnapshotBottom.ToolTip"));
            // 
            // SnapshotRight
            // 
            resources.ApplyResources(SnapshotRight, "SnapshotRight");
            SnapshotRight.Name = "SnapshotRight";
            toolTip1.SetToolTip(SnapshotRight, resources.GetString("SnapshotRight.ToolTip"));
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            toolTip1.SetToolTip(pictureBox1, resources.GetString("pictureBox1.ToolTip"));
            // 
            // pictureBox2
            // 
            resources.ApplyResources(pictureBox2, "pictureBox2");
            pictureBox2.Name = "pictureBox2";
            pictureBox2.TabStop = false;
            toolTip1.SetToolTip(pictureBox2, resources.GetString("pictureBox2.ToolTip"));
            // 
            // pictureBox3
            // 
            resources.ApplyResources(pictureBox3, "pictureBox3");
            pictureBox3.Name = "pictureBox3";
            pictureBox3.TabStop = false;
            toolTip1.SetToolTip(pictureBox3, resources.GetString("pictureBox3.ToolTip"));
            // 
            // pictureBox4
            // 
            resources.ApplyResources(pictureBox4, "pictureBox4");
            pictureBox4.Name = "pictureBox4";
            pictureBox4.TabStop = false;
            toolTip1.SetToolTip(pictureBox4, resources.GetString("pictureBox4.ToolTip"));
            // 
            // SnapshotAreaPanel
            // 
            SnapshotAreaPanel.Controls.Add(SnapshotLeft);
            SnapshotAreaPanel.Controls.Add(pictureBox3);
            SnapshotAreaPanel.Controls.Add(pictureBox4);
            SnapshotAreaPanel.Controls.Add(SnapshotBottom);
            SnapshotAreaPanel.Controls.Add(pictureBox2);
            SnapshotAreaPanel.Controls.Add(SnapshotTop);
            SnapshotAreaPanel.Controls.Add(SnapshotRight);
            SnapshotAreaPanel.Controls.Add(pictureBox1);
            resources.ApplyResources(SnapshotAreaPanel, "SnapshotAreaPanel");
            SnapshotAreaPanel.Name = "SnapshotAreaPanel";
            // 
            // MapSnapshot
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(SnapshotAreaPanel);
            Controls.Add(panel3);
            Controls.Add(statusStrip1);
            Controls.Add(SnapshotTools);
            Controls.Add(panel5);
            Controls.Add(panel4);
            Controls.Add(SnapshotPanel);
            Controls.Add(panel2);
            Controls.Add(panel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MapSnapshot";
            ShowInTaskbar = false;
            TopMost = true;
            FormClosing += MapSnapshot_FormClosing;
            Load += MapSnapshot_Load;
            Enter += MapSnapshot_Enter;
            MouseEnter += MapSnapshot_Enter;
            SnapshotToolStrip.ResumeLayout(false);
            SnapshotToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SnapshotPicture).EndInit();
            SnapshotPanel.ResumeLayout(false);
            panel5.ResumeLayout(false);
            SnapshotTools.ResumeLayout(false);
            SnapshotTools.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            SnapshotAreaPanel.ResumeLayout(false);
            SnapshotAreaPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolStrip SnapshotToolStrip;
        private ToolStripButton SnapshotSave;
        private ToolStripSpringTextBox SnapshotFileTextBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton SnapshotRun;
        private PictureBox SnapshotPicture;
        private ToolStripLabel SnapshotZoomLabel;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripDropDownButton SnapshotZoom;
        private Panel panel1;
        private Panel panel2;
        private Panel SnapshotPanel;
        private Panel panel4;
        private Panel panel5;
        private ToolStrip SnapshotTools;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton SnapshotRefresh;
        private ToolStripSeparator toolStripSeparator3;
        private StatusStrip statusStrip1;
        private ToolStripLabel toolStripLabel2;
        private ToolStripProgressBar SnapshotProgressBar;
        private ToolStripStatusLabel SnapshotStatusLabel;
        private Panel panel3;
        private TextBox SnapshotTop;
        private ToolTip toolTip1;
        private TextBox SnapshotLeft;
        private TextBox SnapshotBottom;
        private TextBox SnapshotRight;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private Panel SnapshotAreaPanel;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripLabel EPSG3857Switch;
        private ToolStripLabel EPSG4326Switch;
    }
}