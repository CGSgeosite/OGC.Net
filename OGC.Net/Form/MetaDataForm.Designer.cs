
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
            this.metaBox = new System.Windows.Forms.GroupBox();
            this.themeMetadata = new System.Windows.Forms.TextBox();
            this.OKbutton = new System.Windows.Forms.Button();
            this.Info = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.donotPrompt = new System.Windows.Forms.CheckBox();
            this.metaBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // metaBox
            // 
            this.metaBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metaBox.BackColor = System.Drawing.Color.Transparent;
            this.metaBox.Controls.Add(this.themeMetadata);
            this.metaBox.Location = new System.Drawing.Point(8, 65);
            this.metaBox.Margin = new System.Windows.Forms.Padding(4);
            this.metaBox.Name = "metaBox";
            this.metaBox.Padding = new System.Windows.Forms.Padding(4);
            this.metaBox.Size = new System.Drawing.Size(461, 143);
            this.metaBox.TabIndex = 19;
            this.metaBox.TabStop = false;
            this.metaBox.Text = "Metadata (XML)";
            // 
            // themeMetadata
            // 
            this.themeMetadata.AcceptsReturn = true;
            this.themeMetadata.BackColor = System.Drawing.Color.White;
            this.themeMetadata.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.themeMetadata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.themeMetadata.Location = new System.Drawing.Point(4, 20);
            this.themeMetadata.Margin = new System.Windows.Forms.Padding(4);
            this.themeMetadata.MaxLength = 327670;
            this.themeMetadata.Multiline = true;
            this.themeMetadata.Name = "themeMetadata";
            this.themeMetadata.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.themeMetadata.Size = new System.Drawing.Size(453, 119);
            this.themeMetadata.TabIndex = 12;
            this.themeMetadata.WordWrap = false;
            this.themeMetadata.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ThemeMetadata_KeyPress);
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.Location = new System.Drawing.Point(338, 278);
            this.OKbutton.Margin = new System.Windows.Forms.Padding(4);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(131, 33);
            this.OKbutton.TabIndex = 20;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // Info
            // 
            this.Info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Info.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Info.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Info.ForeColor = System.Drawing.Color.Red;
            this.Info.Location = new System.Drawing.Point(8, 218);
            this.Info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Info.Name = "Info";
            this.Info.Size = new System.Drawing.Size(461, 50);
            this.Info.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(66, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(399, 41);
            this.label1.TabIndex = 22;
            this.label1.Text = "The metadata is in XML format and will be attached to the last layer";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Location = new System.Drawing.Point(15, 10);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(44, 47);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // donotPrompt
            // 
            this.donotPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.donotPrompt.AutoSize = true;
            this.donotPrompt.Location = new System.Drawing.Point(13, 285);
            this.donotPrompt.Margin = new System.Windows.Forms.Padding(4);
            this.donotPrompt.Name = "donotPrompt";
            this.donotPrompt.Size = new System.Drawing.Size(142, 21);
            this.donotPrompt.TabIndex = 24;
            this.donotPrompt.Text = "Don\'t prompt again";
            this.donotPrompt.UseVisualStyleBackColor = true;
            this.donotPrompt.CheckedChanged += new System.EventHandler(this.DoNotPrompt_CheckedChanged);
            // 
            // MetaData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 320);
            this.Controls.Add(this.donotPrompt);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Info);
            this.Controls.Add(this.OKbutton);
            this.Controls.Add(this.metaBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MetaData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MetaData";
            this.TopMost = true;
            this.metaBox.ResumeLayout(false);
            this.metaBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox metaBox;
        private System.Windows.Forms.TextBox themeMetadata;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Label Info;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox donotPrompt;
    }
}