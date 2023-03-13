namespace Geosite
{
    partial class LayersBuilderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayersBuilderForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.treePathBox = new System.Windows.Forms.TextBox();
            this.OKbutton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.treePathTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tipsBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.remarksBox = new System.Windows.Forms.TextBox();
            this.abstractBox = new System.Windows.Forms.TextBox();
            this.keywordBox = new System.Windows.Forms.TextBox();
            this.contactBox = new System.Windows.Forms.TextBox();
            this.authorBox = new System.Windows.Forms.TextBox();
            this.thumbnailBox = new System.Windows.Forms.TextBox();
            this.legendBox = new System.Windows.Forms.TextBox();
            this.downloadBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.donotPrompt = new System.Windows.Forms.CheckBox();
            this.treePathTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Note: the layers should be separated by 【/】,for example:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 69);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "● Level 2：【A/B】";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "● Level 3：【A/B/C】";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // treePathBox
            // 
            this.treePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treePathBox.Location = new System.Drawing.Point(7, 196);
            this.treePathBox.Margin = new System.Windows.Forms.Padding(4);
            this.treePathBox.Name = "treePathBox";
            this.treePathBox.Size = new System.Drawing.Size(442, 23);
            this.treePathBox.TabIndex = 1;
            this.treePathBox.WordWrap = false;
            this.treePathBox.TextChanged += new System.EventHandler(this.TreePathBox_TextChanged);
            // 
            // OKbutton
            // 
            this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKbutton.Location = new System.Drawing.Point(344, 280);
            this.OKbutton.Margin = new System.Windows.Forms.Padding(4);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(131, 33);
            this.OKbutton.TabIndex = 2;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(113, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "● Level 1：【A】";
            // 
            // treePathTab
            // 
            this.treePathTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treePathTab.Controls.Add(this.tabPage1);
            this.treePathTab.Controls.Add(this.tabPage2);
            this.treePathTab.Location = new System.Drawing.Point(12, 10);
            this.treePathTab.Margin = new System.Windows.Forms.Padding(4);
            this.treePathTab.Name = "treePathTab";
            this.treePathTab.SelectedIndex = 0;
            this.treePathTab.Size = new System.Drawing.Size(465, 264);
            this.treePathTab.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.tipsBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.treePathBox);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(457, 234);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Classification";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(8, 41);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(98, 101);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // tipsBox
            // 
            this.tipsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tipsBox.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tipsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tipsBox.ForeColor = System.Drawing.Color.Red;
            this.tipsBox.Location = new System.Drawing.Point(9, 165);
            this.tipsBox.Margin = new System.Windows.Forms.Padding(4);
            this.tipsBox.Name = "tipsBox";
            this.tipsBox.ReadOnly = true;
            this.tipsBox.Size = new System.Drawing.Size(440, 16);
            this.tipsBox.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(113, 126);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(159, 17);
            this.label10.TabIndex = 0;
            this.label10.Text = "● Level n：【A/B/C/.../N】";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.remarksBox);
            this.tabPage2.Controls.Add(this.abstractBox);
            this.tabPage2.Controls.Add(this.keywordBox);
            this.tabPage2.Controls.Add(this.contactBox);
            this.tabPage2.Controls.Add(this.authorBox);
            this.tabPage2.Controls.Add(this.thumbnailBox);
            this.tabPage2.Controls.Add(this.legendBox);
            this.tabPage2.Controls.Add(this.downloadBox);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(457, 234);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Supplementary";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // remarksBox
            // 
            this.remarksBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.remarksBox.Location = new System.Drawing.Point(338, 190);
            this.remarksBox.Margin = new System.Windows.Forms.Padding(4);
            this.remarksBox.Name = "remarksBox";
            this.remarksBox.Size = new System.Drawing.Size(110, 23);
            this.remarksBox.TabIndex = 10;
            // 
            // abstractBox
            // 
            this.abstractBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.abstractBox.Location = new System.Drawing.Point(142, 152);
            this.abstractBox.Margin = new System.Windows.Forms.Padding(4);
            this.abstractBox.Name = "abstractBox";
            this.abstractBox.Size = new System.Drawing.Size(306, 23);
            this.abstractBox.TabIndex = 9;
            // 
            // keywordBox
            // 
            this.keywordBox.Location = new System.Drawing.Point(142, 187);
            this.keywordBox.Margin = new System.Windows.Forms.Padding(4);
            this.keywordBox.Name = "keywordBox";
            this.keywordBox.Size = new System.Drawing.Size(110, 23);
            this.keywordBox.TabIndex = 8;
            // 
            // contactBox
            // 
            this.contactBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contactBox.Location = new System.Drawing.Point(338, 116);
            this.contactBox.Margin = new System.Windows.Forms.Padding(4);
            this.contactBox.Name = "contactBox";
            this.contactBox.Size = new System.Drawing.Size(110, 23);
            this.contactBox.TabIndex = 7;
            // 
            // authorBox
            // 
            this.authorBox.Location = new System.Drawing.Point(142, 116);
            this.authorBox.Margin = new System.Windows.Forms.Padding(4);
            this.authorBox.Name = "authorBox";
            this.authorBox.Size = new System.Drawing.Size(110, 23);
            this.authorBox.TabIndex = 6;
            // 
            // thumbnailBox
            // 
            this.thumbnailBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.thumbnailBox.Location = new System.Drawing.Point(142, 81);
            this.thumbnailBox.Margin = new System.Windows.Forms.Padding(4);
            this.thumbnailBox.Name = "thumbnailBox";
            this.thumbnailBox.Size = new System.Drawing.Size(306, 23);
            this.thumbnailBox.TabIndex = 5;
            // 
            // legendBox
            // 
            this.legendBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.legendBox.Location = new System.Drawing.Point(142, 45);
            this.legendBox.Margin = new System.Windows.Forms.Padding(4);
            this.legendBox.Name = "legendBox";
            this.legendBox.Size = new System.Drawing.Size(306, 23);
            this.legendBox.TabIndex = 4;
            // 
            // downloadBox
            // 
            this.downloadBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadBox.Location = new System.Drawing.Point(142, 10);
            this.downloadBox.Margin = new System.Windows.Forms.Padding(4);
            this.downloadBox.Name = "downloadBox";
            this.downloadBox.Size = new System.Drawing.Size(306, 23);
            this.downloadBox.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(80, 194);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 17);
            this.label11.TabIndex = 0;
            this.label11.Text = "Keyword";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(276, 194);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 17);
            this.label13.TabIndex = 0;
            this.label13.Text = "Remarks";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(74, 156);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 17);
            this.label12.TabIndex = 0;
            this.label12.Text = "Abstract";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(88, 120);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "Author";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(276, 120);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 17);
            this.label8.TabIndex = 0;
            this.label8.Text = "Contact";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 85);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Thumbnail（URL）";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 17);
            this.label6.TabIndex = 0;
            this.label6.Text = "Legend（URL）";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 14);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Download（URL）";
            // 
            // donotPrompt
            // 
            this.donotPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.donotPrompt.AutoSize = true;
            this.donotPrompt.Location = new System.Drawing.Point(16, 286);
            this.donotPrompt.Margin = new System.Windows.Forms.Padding(4);
            this.donotPrompt.Name = "donotPrompt";
            this.donotPrompt.Size = new System.Drawing.Size(142, 21);
            this.donotPrompt.TabIndex = 25;
            this.donotPrompt.Text = "Don\'t prompt again";
            this.donotPrompt.UseVisualStyleBackColor = true;
            this.donotPrompt.CheckedChanged += new System.EventHandler(this.DoNotPrompt_CheckedChanged);
            // 
            // LayersBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(483, 320);
            this.Controls.Add(this.donotPrompt);
            this.Controls.Add(this.treePathTab);
            this.Controls.Add(this.OKbutton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LayersBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Classification & Supplementary information";
            this.TopMost = true;
            this.treePathTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox treePathBox;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl treePathTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox abstractBox;
        private System.Windows.Forms.TextBox keywordBox;
        private System.Windows.Forms.TextBox contactBox;
        private System.Windows.Forms.TextBox authorBox;
        private System.Windows.Forms.TextBox thumbnailBox;
        private System.Windows.Forms.TextBox legendBox;
        private System.Windows.Forms.TextBox downloadBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox remarksBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tipsBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox donotPrompt;
    }
}