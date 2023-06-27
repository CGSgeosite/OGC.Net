namespace Geosite
{
    partial class PreviewStyleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewStyleForm));
            pictureBox1 = new PictureBox();
            label1 = new Label();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            panel4 = new Panel();
            PinRadioPicture = new PictureBox();
            PinRadioButton = new RadioButton();
            CircleRadioButton = new RadioButton();
            panel3 = new Panel();
            PointColorPanel = new Panel();
            groupBox2 = new GroupBox();
            LineColorPanel = new Panel();
            groupBox3 = new GroupBox();
            PolygonColorPanel = new Panel();
            OKbutton = new Button();
            Cancelbutton = new Button();
            panel2 = new Panel();
            ColorDialog = new ColorDialog();
            MapGridGroupBox = new GroupBox();
            MapGridColorPanel = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PinRadioPicture).BeginInit();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            MapGridGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Location = new Point(4, 4);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(44, 31);
            pictureBox1.TabIndex = 24;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.Location = new Point(56, 4);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(313, 37);
            label1.TabIndex = 25;
            label1.Text = "The rendering style of vector features can be set, \r\nand currently supports stroke color.";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.Gray;
            panel1.Location = new Point(4, 42);
            panel1.Name = "panel1";
            panel1.Size = new Size(365, 1);
            panel1.TabIndex = 26;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(panel4);
            groupBox1.Controls.Add(panel3);
            groupBox1.Controls.Add(PointColorPanel);
            groupBox1.Location = new Point(4, 44);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(365, 77);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Point";
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel4.Controls.Add(PinRadioPicture);
            panel4.Controls.Add(PinRadioButton);
            panel4.Controls.Add(CircleRadioButton);
            panel4.Location = new Point(8, 16);
            panel4.Name = "panel4";
            panel4.Size = new Size(351, 33);
            panel4.TabIndex = 2;
            // 
            // PinRadioPicture
            // 
            PinRadioPicture.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PinRadioPicture.BackgroundImage = (Image)resources.GetObject("PinRadioPicture.BackgroundImage");
            PinRadioPicture.BackgroundImageLayout = ImageLayout.Stretch;
            PinRadioPicture.Location = new Point(316, 6);
            PinRadioPicture.Name = "PinRadioPicture";
            PinRadioPicture.Size = new Size(22, 20);
            PinRadioPicture.TabIndex = 1;
            PinRadioPicture.TabStop = false;
            PinRadioPicture.Click += PinRadioPicture_Click;
            // 
            // PinRadioButton
            // 
            PinRadioButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            PinRadioButton.AutoSize = true;
            PinRadioButton.Location = new Point(269, 6);
            PinRadioButton.Name = "PinRadioButton";
            PinRadioButton.Size = new Size(43, 21);
            PinRadioButton.TabIndex = 0;
            PinRadioButton.Text = "Pin";
            PinRadioButton.UseVisualStyleBackColor = true;
            // 
            // CircleRadioButton
            // 
            CircleRadioButton.AutoSize = true;
            CircleRadioButton.Checked = true;
            CircleRadioButton.Location = new Point(11, 6);
            CircleRadioButton.Name = "CircleRadioButton";
            CircleRadioButton.Size = new Size(77, 21);
            CircleRadioButton.TabIndex = 0;
            CircleRadioButton.TabStop = true;
            CircleRadioButton.Text = "Circle (○)";
            CircleRadioButton.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel3.BackColor = Color.Gray;
            panel3.Location = new Point(8, 51);
            panel3.Name = "panel3";
            panel3.Size = new Size(351, 1);
            panel3.TabIndex = 1;
            // 
            // PointColorPanel
            // 
            PointColorPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PointColorPanel.BackColor = Color.FromArgb(13, 110, 253);
            PointColorPanel.BorderStyle = BorderStyle.Fixed3D;
            PointColorPanel.Cursor = Cursors.Hand;
            PointColorPanel.Location = new Point(8, 54);
            PointColorPanel.Name = "PointColorPanel";
            PointColorPanel.Size = new Size(351, 16);
            PointColorPanel.TabIndex = 0;
            PointColorPanel.Click += ColorPanel_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(LineColorPanel);
            groupBox2.Location = new Point(4, 128);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(365, 40);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Line";
            // 
            // LineColorPanel
            // 
            LineColorPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LineColorPanel.BackColor = Color.Black;
            LineColorPanel.BorderStyle = BorderStyle.Fixed3D;
            LineColorPanel.Cursor = Cursors.Hand;
            LineColorPanel.Location = new Point(8, 17);
            LineColorPanel.Name = "LineColorPanel";
            LineColorPanel.Size = new Size(351, 16);
            LineColorPanel.TabIndex = 0;
            LineColorPanel.Click += ColorPanel_Click;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(PolygonColorPanel);
            groupBox3.Location = new Point(4, 176);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(365, 40);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Polygon";
            // 
            // PolygonColorPanel
            // 
            PolygonColorPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PolygonColorPanel.BackColor = Color.FromArgb(255, 63, 34);
            PolygonColorPanel.BorderStyle = BorderStyle.Fixed3D;
            PolygonColorPanel.Cursor = Cursors.Hand;
            PolygonColorPanel.Location = new Point(8, 17);
            PolygonColorPanel.Name = "PolygonColorPanel";
            PolygonColorPanel.Size = new Size(351, 16);
            PolygonColorPanel.TabIndex = 0;
            PolygonColorPanel.Click += ColorPanel_Click;
            // 
            // OKbutton
            // 
            OKbutton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OKbutton.Location = new Point(237, 281);
            OKbutton.Margin = new Padding(4);
            OKbutton.Name = "OKbutton";
            OKbutton.Size = new Size(131, 33);
            OKbutton.TabIndex = 0;
            OKbutton.Text = "OK";
            OKbutton.UseVisualStyleBackColor = true;
            OKbutton.Click += OKbutton_Click;
            // 
            // Cancelbutton
            // 
            Cancelbutton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancelbutton.Location = new Point(98, 281);
            Cancelbutton.Margin = new Padding(4);
            Cancelbutton.Name = "Cancelbutton";
            Cancelbutton.Size = new Size(131, 33);
            Cancelbutton.TabIndex = 4;
            Cancelbutton.Text = "Cancel";
            Cancelbutton.UseVisualStyleBackColor = true;
            Cancelbutton.Click += Cancelbutton_Click;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.Gray;
            panel2.Location = new Point(4, 273);
            panel2.Name = "panel2";
            panel2.Size = new Size(365, 1);
            panel2.TabIndex = 26;
            // 
            // MapGridGroupBox
            // 
            MapGridGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MapGridGroupBox.Controls.Add(MapGridColorPanel);
            MapGridGroupBox.Location = new Point(4, 225);
            MapGridGroupBox.Name = "MapGridGroupBox";
            MapGridGroupBox.Size = new Size(365, 40);
            MapGridGroupBox.TabIndex = 3;
            MapGridGroupBox.TabStop = false;
            MapGridGroupBox.Text = "Map Grid";
            // 
            // MapGridColorPanel
            // 
            MapGridColorPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MapGridColorPanel.BackColor = Color.White;
            MapGridColorPanel.BorderStyle = BorderStyle.Fixed3D;
            MapGridColorPanel.Cursor = Cursors.Hand;
            MapGridColorPanel.Location = new Point(8, 17);
            MapGridColorPanel.Name = "MapGridColorPanel";
            MapGridColorPanel.Size = new Size(351, 16);
            MapGridColorPanel.TabIndex = 0;
            MapGridColorPanel.Click += ColorPanel_Click;
            // 
            // PreviewStyleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 320);
            Controls.Add(Cancelbutton);
            Controls.Add(OKbutton);
            Controls.Add(MapGridGroupBox);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PreviewStyleForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Preview Style";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PinRadioPicture).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            MapGridGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Panel panel1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Button OKbutton;
        private Button Cancelbutton;
        private Panel panel2;
        private Panel PointColorPanel;
        private Panel LineColorPanel;
        private Panel PolygonColorPanel;
        private ColorDialog ColorDialog;
        private Panel panel3;
        private Panel panel4;
        private RadioButton PinRadioButton;
        private RadioButton CircleRadioButton;
        private PictureBox PinRadioPicture;
        private GroupBox MapGridGroupBox;
        private Panel MapGridColorPanel;
    }
}