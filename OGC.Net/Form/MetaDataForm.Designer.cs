
namespace Geosite
{
    partial class MetaDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaDataForm));
            metaBox = new GroupBox();
            themeMetadata = new TextBox();
            OKbutton = new Button();
            Info = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            donotPrompt = new CheckBox();
            metaBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // metaBox
            // 
            metaBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            metaBox.BackColor = Color.Transparent;
            metaBox.Controls.Add(themeMetadata);
            metaBox.Location = new Point(8, 65);
            metaBox.Margin = new Padding(4);
            metaBox.Name = "metaBox";
            metaBox.Padding = new Padding(4);
            metaBox.Size = new Size(461, 143);
            metaBox.TabIndex = 19;
            metaBox.TabStop = false;
            metaBox.Text = "Metadata (XML)";
            // 
            // themeMetadata
            // 
            themeMetadata.AcceptsReturn = true;
            themeMetadata.BackColor = Color.White;
            themeMetadata.BorderStyle = BorderStyle.None;
            themeMetadata.Dock = DockStyle.Fill;
            themeMetadata.Location = new Point(4, 20);
            themeMetadata.Margin = new Padding(4);
            themeMetadata.MaxLength = 327670;
            themeMetadata.Multiline = true;
            themeMetadata.Name = "themeMetadata";
            themeMetadata.ScrollBars = ScrollBars.Both;
            themeMetadata.Size = new Size(453, 119);
            themeMetadata.TabIndex = 12;
            themeMetadata.WordWrap = false;
            themeMetadata.KeyPress += ThemeMetadata_KeyPress;
            // 
            // OKbutton
            // 
            OKbutton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OKbutton.Location = new Point(338, 278);
            OKbutton.Margin = new Padding(4);
            OKbutton.Name = "OKbutton";
            OKbutton.Size = new Size(131, 33);
            OKbutton.TabIndex = 20;
            OKbutton.Text = "OK";
            OKbutton.UseVisualStyleBackColor = true;
            OKbutton.Click += OKbutton_Click;
            // 
            // Info
            // 
            Info.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Info.BorderStyle = BorderStyle.Fixed3D;
            Info.FlatStyle = FlatStyle.Flat;
            Info.ForeColor = Color.Red;
            Info.Location = new Point(8, 218);
            Info.Margin = new Padding(4, 0, 4, 0);
            Info.Name = "Info";
            Info.Size = new Size(461, 50);
            Info.TabIndex = 21;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.Location = new Point(66, 17);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(399, 41);
            label1.TabIndex = 22;
            label1.Text = "The metadata is in XML format and will be attached to the last layer";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Location = new Point(15, 10);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(44, 47);
            pictureBox1.TabIndex = 23;
            pictureBox1.TabStop = false;
            // 
            // donotPrompt
            // 
            donotPrompt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            donotPrompt.AutoSize = true;
            donotPrompt.Location = new Point(13, 285);
            donotPrompt.Margin = new Padding(4);
            donotPrompt.Name = "donotPrompt";
            donotPrompt.Size = new Size(145, 21);
            donotPrompt.TabIndex = 24;
            donotPrompt.Text = "Don't prompt again.";
            donotPrompt.UseVisualStyleBackColor = true;
            donotPrompt.CheckedChanged += DoNotPrompt_CheckedChanged;
            // 
            // MetaDataForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(483, 320);
            Controls.Add(donotPrompt);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(Info);
            Controls.Add(OKbutton);
            Controls.Add(metaBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "MetaDataForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MetaData";
            TopMost = true;
            metaBox.ResumeLayout(false);
            metaBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox metaBox;
        private TextBox themeMetadata;
        private Button OKbutton;
        private Label Info;
        private Label label1;
        private PictureBox pictureBox1;
        private CheckBox donotPrompt;
    }
}