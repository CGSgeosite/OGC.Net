namespace Geosite
{
    partial class FreeTextFieldForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FreeTextFieldForm));
            CoordinateComboBox = new ComboBox();
            label1 = new Label();
            no = new Button();
            panel1 = new Panel();
            yes = new Button();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // CoordinateComboBox
            // 
            CoordinateComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CoordinateComboBox.FormattingEnabled = true;
            CoordinateComboBox.Location = new Point(12, 117);
            CoordinateComboBox.Margin = new Padding(4);
            CoordinateComboBox.Name = "CoordinateComboBox";
            CoordinateComboBox.Size = new Size(458, 25);
            CoordinateComboBox.TabIndex = 2;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.BorderStyle = BorderStyle.Fixed3D;
            label1.Location = new Point(113, 13);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(357, 72);
            label1.TabIndex = 1;
            label1.Text = "Please select the geometric coordinate column.\r\n\r\n[No] can be selected for general data";
            // 
            // no
            // 
            no.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            no.DialogResult = DialogResult.Cancel;
            no.Location = new Point(339, 172);
            no.Margin = new Padding(4);
            no.Name = "no";
            no.Size = new Size(131, 33);
            no.TabIndex = 1;
            no.Text = "No";
            no.UseVisualStyleBackColor = true;
            no.Click += No_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = Color.Gray;
            panel1.Location = new Point(14, 156);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(459, 1);
            panel1.TabIndex = 2;
            // 
            // yes
            // 
            yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            yes.DialogResult = DialogResult.Cancel;
            yes.Location = new Point(187, 172);
            yes.Margin = new Padding(4);
            yes.Name = "yes";
            yes.Size = new Size(131, 33);
            yes.TabIndex = 0;
            yes.Text = "Yes";
            yes.UseVisualStyleBackColor = true;
            yes.Click += Yes_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(14, 13);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(81, 72);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.Gray;
            panel2.Location = new Point(12, 102);
            panel2.Margin = new Padding(4);
            panel2.Name = "panel2";
            panel2.Size = new Size(459, 1);
            panel2.TabIndex = 4;
            // 
            // FreeTextFieldForm
            // 
            AcceptButton = yes;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(483, 220);
            Controls.Add(panel2);
            Controls.Add(pictureBox1);
            Controls.Add(panel1);
            Controls.Add(yes);
            Controls.Add(no);
            Controls.Add(label1);
            Controls.Add(CoordinateComboBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FreeTextFieldForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Coordinate column";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox CoordinateComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button no;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button yes;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Panel panel2;
    }
}