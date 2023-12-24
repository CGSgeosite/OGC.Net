namespace Geosite
{
    partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            statusStrip1 = new StatusStrip();
            ExportProgressBar = new ToolStripProgressBar();
            ExportStatusLabel = new ToolStripStatusLabel();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            ExportLog = new ListBox();
            toolStrip1 = new ToolStrip();
            ExportSaveButton = new ToolStripButton();
            ExportPathTextBox = new ToolStripSpringTextBox();
            toolStripSeparator1 = new ToolStripSeparator();
            ExportStartButton = new ToolStripButton();
            panel2 = new Panel();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            toolStrip1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { ExportProgressBar, ExportStatusLabel });
            statusStrip1.Location = new Point(0, 138);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(432, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // ExportProgressBar
            // 
            ExportProgressBar.Name = "ExportProgressBar";
            ExportProgressBar.Size = new Size(100, 16);
            ExportProgressBar.Style = ProgressBarStyle.Continuous;
            ExportProgressBar.Visible = false;
            // 
            // ExportStatusLabel
            // 
            ExportStatusLabel.BackColor = Color.WhiteSmoke;
            ExportStatusLabel.Name = "ExportStatusLabel";
            ExportStatusLabel.Size = new Size(284, 17);
            ExportStatusLabel.Spring = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(6, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = SystemColors.ControlLight;
            panel1.Location = new Point(6, 86);
            panel1.Name = "panel1";
            panel1.Size = new Size(420, 1);
            panel1.TabIndex = 3;
            // 
            // ExportLog
            // 
            ExportLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ExportLog.BackColor = Color.WhiteSmoke;
            ExportLog.FormattingEnabled = true;
            ExportLog.ItemHeight = 17;
            ExportLog.Items.AddRange(new object[] { "Export a vector layer from Database to a file." });
            ExportLog.Location = new Point(60, 5);
            ExportLog.Name = "ExportLog";
            ExportLog.SelectionMode = SelectionMode.MultiExtended;
            ExportLog.Size = new Size(366, 72);
            ExportLog.TabIndex = 4;
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.BackColor = Color.Transparent;
            toolStrip1.Dock = DockStyle.Fill;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { ExportSaveButton, ExportPathTextBox, toolStripSeparator1, ExportStartButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Margin = new Padding(2);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(420, 43);
            toolStrip1.TabIndex = 5;
            toolStrip1.Text = "toolStrip1";
            // 
            // ExportSaveButton
            // 
            ExportSaveButton.AutoSize = false;
            ExportSaveButton.BackgroundImageLayout = ImageLayout.Center;
            ExportSaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ExportSaveButton.Image = (Image)resources.GetObject("ExportSaveButton.Image");
            ExportSaveButton.ImageScaling = ToolStripItemImageScaling.None;
            ExportSaveButton.ImageTransparentColor = Color.Magenta;
            ExportSaveButton.Margin = new Padding(2);
            ExportSaveButton.Name = "ExportSaveButton";
            ExportSaveButton.Padding = new Padding(6, 0, 6, 0);
            ExportSaveButton.Size = new Size(37, 41);
            ExportSaveButton.Text = "toolStripButton1";
            ExportSaveButton.ToolTipText = "Export";
            ExportSaveButton.Click += ExportSaveButton_Click;
            // 
            // ExportPathTextBox
            // 
            ExportPathTextBox.BackColor = Color.White;
            ExportPathTextBox.BorderStyle = BorderStyle.FixedSingle;
            ExportPathTextBox.Margin = new Padding(1, 0, 6, 0);
            ExportPathTextBox.Name = "ExportPathTextBox";
            ExportPathTextBox.ReadOnly = true;
            ExportPathTextBox.Size = new Size(285, 43);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 43);
            // 
            // ExportStartButton
            // 
            ExportStartButton.Alignment = ToolStripItemAlignment.Right;
            ExportStartButton.AutoSize = false;
            ExportStartButton.AutoToolTip = false;
            ExportStartButton.BackgroundImageLayout = ImageLayout.Center;
            ExportStartButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ExportStartButton.Enabled = false;
            ExportStartButton.Image = Properties.Resources.run;
            ExportStartButton.ImageScaling = ToolStripItemImageScaling.None;
            ExportStartButton.ImageTransparentColor = Color.Magenta;
            ExportStartButton.Margin = new Padding(6, 0, 6, 0);
            ExportStartButton.Name = "ExportStartButton";
            ExportStartButton.Size = new Size(37, 41);
            ExportStartButton.TextImageRelation = TextImageRelation.Overlay;
            ExportStartButton.ToolTipText = "Start";
            ExportStartButton.Click += ExportStartButton_Click;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(toolStrip1);
            panel2.Location = new Point(6, 92);
            panel2.Name = "panel2";
            panel2.Size = new Size(420, 43);
            panel2.TabIndex = 6;
            // 
            // ExportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(432, 160);
            Controls.Add(panel2);
            Controls.Add(ExportLog);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(statusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExportForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Vector Feature Export";
            TopMost = true;
            FormClosing += ExportForm_FormClosing;
            Load += ExportForm_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStripProgressBar ExportProgressBar;
        private ToolStripStatusLabel ExportStatusLabel;
        private PictureBox pictureBox1;
        private Panel panel1;
        private ListBox ExportLog;
        private ToolStrip toolStrip1;
        private ToolStripButton ExportSaveButton;
        private ToolStripSpringTextBox ExportPathTextBox;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton ExportStartButton;
        private Panel panel2;
    }
}