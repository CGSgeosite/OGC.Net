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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            treePathBox = new TextBox();
            OKbutton = new Button();
            label4 = new Label();
            treePathTab = new TabControl();
            tabPage1 = new TabPage();
            pictureBox1 = new PictureBox();
            tipsBox = new TextBox();
            label10 = new Label();
            tabPage2 = new TabPage();
            remarksBox = new TextBox();
            abstractBox = new TextBox();
            keywordBox = new TextBox();
            contactBox = new TextBox();
            authorBox = new TextBox();
            thumbnailBox = new TextBox();
            legendBox = new TextBox();
            downloadBox = new TextBox();
            label11 = new Label();
            label13 = new Label();
            label12 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            donotPrompt = new CheckBox();
            treePathTab.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 10);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(352, 17);
            label1.TabIndex = 0;
            label1.Text = "Note: the layers should be separated by 【/】,for example:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(113, 69);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(117, 17);
            label2.TabIndex = 0;
            label2.Text = "● Level 2：【A/B】";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(113, 98);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(130, 17);
            label3.TabIndex = 0;
            label3.Text = "● Level 3：【A/B/C】";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // treePathBox
            // 
            treePathBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treePathBox.Location = new Point(7, 196);
            treePathBox.Margin = new Padding(4);
            treePathBox.Name = "treePathBox";
            treePathBox.Size = new Size(442, 23);
            treePathBox.TabIndex = 3;
            treePathBox.WordWrap = false;
            treePathBox.TextChanged += TreePathBox_TextChanged;
            // 
            // OKbutton
            // 
            OKbutton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            OKbutton.Location = new Point(344, 280);
            OKbutton.Margin = new Padding(4);
            OKbutton.Name = "OKbutton";
            OKbutton.Size = new Size(131, 33);
            OKbutton.TabIndex = 0;
            OKbutton.Text = "OK";
            OKbutton.UseVisualStyleBackColor = true;
            OKbutton.Click += OKbutton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(113, 41);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(104, 17);
            label4.TabIndex = 0;
            label4.Text = "● Level 1：【A】";
            // 
            // treePathTab
            // 
            treePathTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treePathTab.Controls.Add(tabPage1);
            treePathTab.Controls.Add(tabPage2);
            treePathTab.Location = new Point(12, 10);
            treePathTab.Margin = new Padding(4);
            treePathTab.Name = "treePathTab";
            treePathTab.SelectedIndex = 0;
            treePathTab.Size = new Size(465, 264);
            treePathTab.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(pictureBox1);
            tabPage1.Controls.Add(tipsBox);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(treePathBox);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label10);
            tabPage1.Controls.Add(label3);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Margin = new Padding(4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(4);
            tabPage1.Size = new Size(457, 234);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Classification";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.White;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(8, 41);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(98, 101);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // tipsBox
            // 
            tipsBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tipsBox.BackColor = SystemColors.ControlLight;
            tipsBox.BorderStyle = BorderStyle.None;
            tipsBox.ForeColor = Color.Red;
            tipsBox.Location = new Point(9, 165);
            tipsBox.Margin = new Padding(4);
            tipsBox.Name = "tipsBox";
            tipsBox.ReadOnly = true;
            tipsBox.Size = new Size(440, 16);
            tipsBox.TabIndex = 4;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(113, 126);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(159, 17);
            label10.TabIndex = 0;
            label10.Text = "● Level n：【A/B/C/.../N】";
            label10.TextAlign = ContentAlignment.TopCenter;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(remarksBox);
            tabPage2.Controls.Add(abstractBox);
            tabPage2.Controls.Add(keywordBox);
            tabPage2.Controls.Add(contactBox);
            tabPage2.Controls.Add(authorBox);
            tabPage2.Controls.Add(thumbnailBox);
            tabPage2.Controls.Add(legendBox);
            tabPage2.Controls.Add(downloadBox);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(label13);
            tabPage2.Controls.Add(label12);
            tabPage2.Controls.Add(label9);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(label5);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Margin = new Padding(4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(4);
            tabPage2.Size = new Size(457, 234);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Supplementary";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // remarksBox
            // 
            remarksBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            remarksBox.Location = new Point(338, 190);
            remarksBox.Margin = new Padding(4);
            remarksBox.Name = "remarksBox";
            remarksBox.Size = new Size(110, 23);
            remarksBox.TabIndex = 11;
            // 
            // abstractBox
            // 
            abstractBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            abstractBox.Location = new Point(142, 152);
            abstractBox.Margin = new Padding(4);
            abstractBox.Name = "abstractBox";
            abstractBox.Size = new Size(306, 23);
            abstractBox.TabIndex = 9;
            // 
            // keywordBox
            // 
            keywordBox.Location = new Point(142, 187);
            keywordBox.Margin = new Padding(4);
            keywordBox.Name = "keywordBox";
            keywordBox.Size = new Size(110, 23);
            keywordBox.TabIndex = 10;
            // 
            // contactBox
            // 
            contactBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            contactBox.Location = new Point(338, 116);
            contactBox.Margin = new Padding(4);
            contactBox.Name = "contactBox";
            contactBox.Size = new Size(110, 23);
            contactBox.TabIndex = 8;
            // 
            // authorBox
            // 
            authorBox.Location = new Point(142, 116);
            authorBox.Margin = new Padding(4);
            authorBox.Name = "authorBox";
            authorBox.Size = new Size(110, 23);
            authorBox.TabIndex = 7;
            // 
            // thumbnailBox
            // 
            thumbnailBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            thumbnailBox.Location = new Point(142, 81);
            thumbnailBox.Margin = new Padding(4);
            thumbnailBox.Name = "thumbnailBox";
            thumbnailBox.Size = new Size(306, 23);
            thumbnailBox.TabIndex = 6;
            // 
            // legendBox
            // 
            legendBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            legendBox.Location = new Point(142, 45);
            legendBox.Margin = new Padding(4);
            legendBox.Name = "legendBox";
            legendBox.Size = new Size(306, 23);
            legendBox.TabIndex = 5;
            // 
            // downloadBox
            // 
            downloadBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            downloadBox.Location = new Point(142, 10);
            downloadBox.Margin = new Padding(4);
            downloadBox.Name = "downloadBox";
            downloadBox.Size = new Size(306, 23);
            downloadBox.TabIndex = 4;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(80, 194);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(59, 17);
            label11.TabIndex = 0;
            label11.Text = "Keyword";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(276, 194);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(59, 17);
            label13.TabIndex = 0;
            label13.Text = "Remarks";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(74, 156);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(56, 17);
            label12.TabIndex = 0;
            label12.Text = "Abstract";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(88, 120);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(47, 17);
            label9.TabIndex = 0;
            label9.Text = "Author";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(276, 120);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(52, 17);
            label8.TabIndex = 0;
            label8.Text = "Contact";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(18, 85);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(115, 17);
            label7.TabIndex = 0;
            label7.Text = "Thumbnail（URL）";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(38, 50);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(98, 17);
            label6.TabIndex = 0;
            label6.Text = "Legend（URL）";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(24, 14);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(114, 17);
            label5.TabIndex = 0;
            label5.Text = "Download（URL）";
            // 
            // donotPrompt
            // 
            donotPrompt.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            donotPrompt.AutoSize = true;
            donotPrompt.Location = new Point(16, 286);
            donotPrompt.Margin = new Padding(4);
            donotPrompt.Name = "donotPrompt";
            donotPrompt.Size = new Size(145, 21);
            donotPrompt.TabIndex = 1;
            donotPrompt.Text = "Don't prompt again.";
            donotPrompt.UseVisualStyleBackColor = true;
            donotPrompt.CheckedChanged += DoNotPrompt_CheckedChanged;
            // 
            // LayersBuilderForm
            // 
            AcceptButton = OKbutton;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(483, 320);
            Controls.Add(donotPrompt);
            Controls.Add(treePathTab);
            Controls.Add(OKbutton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LayersBuilderForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Classification & Supplementary information";
            TopMost = true;
            treePathTab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox treePathBox;
        private Button OKbutton;
        private Label label4;
        private TabControl treePathTab;
        private TabPage tabPage1;
        private Label label10;
        private TabPage tabPage2;
        private TextBox abstractBox;
        private TextBox keywordBox;
        private TextBox contactBox;
        private TextBox authorBox;
        private TextBox thumbnailBox;
        private TextBox legendBox;
        private TextBox downloadBox;
        private Label label11;
        private Label label12;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private TextBox remarksBox;
        private Label label13;
        private TextBox tipsBox;
        private PictureBox pictureBox1;
        private CheckBox donotPrompt;
    }
}