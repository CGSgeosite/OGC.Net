namespace Geosite
{
    sealed partial class InputForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputForm));
            inputTextBox = new TextBox();
            panel1 = new Panel();
            yes = new Button();
            no = new Button();
            tipTextBox = new TextBox();
            SuspendLayout();
            // 
            // inputTextBox
            // 
            inputTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            inputTextBox.Location = new Point(12, 134);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(459, 23);
            inputTextBox.TabIndex = 3;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.BackColor = SystemColors.ButtonShadow;
            panel1.Location = new Point(12, 179);
            panel1.Name = "panel1";
            panel1.Size = new Size(459, 1);
            panel1.TabIndex = 2;
            // 
            // yes
            // 
            yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            yes.Location = new Point(189, 197);
            yes.Name = "yes";
            yes.Size = new Size(131, 33);
            yes.TabIndex = 0;
            yes.Text = "Yes";
            yes.UseVisualStyleBackColor = true;
            yes.Click += Yes_Click;
            // 
            // no
            // 
            no.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            no.Location = new Point(340, 197);
            no.Name = "no";
            no.Size = new Size(131, 33);
            no.TabIndex = 1;
            no.Text = "No";
            no.UseVisualStyleBackColor = true;
            no.Click += No_Click;
            // 
            // tipTextBox
            // 
            tipTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tipTextBox.BackColor = SystemColors.ButtonHighlight;
            tipTextBox.Location = new Point(12, 12);
            tipTextBox.Multiline = true;
            tipTextBox.Name = "tipTextBox";
            tipTextBox.ReadOnly = true;
            tipTextBox.ScrollBars = ScrollBars.Vertical;
            tipTextBox.Size = new Size(459, 103);
            tipTextBox.TabIndex = 2;
            // 
            // InputForm
            // 
            AcceptButton = yes;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(483, 246);
            Controls.Add(tipTextBox);
            Controls.Add(no);
            Controls.Add(yes);
            Controls.Add(panel1);
            Controls.Add(inputTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Caution";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox inputTextBox;
        private Panel panel1;
        private Button yes;
        private Button no;
        private TextBox tipTextBox;
    }
}