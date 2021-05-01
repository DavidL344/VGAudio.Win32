namespace VGAudio.Win32
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.lbl_productName = new System.Windows.Forms.Label();
            this.lbl_productAuthor = new System.Windows.Forms.Label();
            this.lbl_ogProjectAuthor = new System.Windows.Forms.Label();
            this.lbl_cliVersion = new System.Windows.Forms.Label();
            this.txt_license = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lbl_productName, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.lbl_productAuthor, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.lbl_ogProjectAuthor, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.lbl_cliVersion, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.txt_license, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.okButton, 1, 5);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(24, 21);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1112, 633);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(8, 7);
            this.logoPictureBox.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
            this.logoPictureBox.Size = new System.Drawing.Size(350, 619);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 12;
            this.logoPictureBox.TabStop = false;
            // 
            // lbl_productName
            // 
            this.lbl_productName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_productName.Location = new System.Drawing.Point(382, 0);
            this.lbl_productName.Margin = new System.Windows.Forms.Padding(16, 0, 8, 0);
            this.lbl_productName.MaximumSize = new System.Drawing.Size(0, 41);
            this.lbl_productName.Name = "lbl_productName";
            this.lbl_productName.Size = new System.Drawing.Size(722, 41);
            this.lbl_productName.TabIndex = 19;
            this.lbl_productName.Text = "Product Name";
            this.lbl_productName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_productAuthor
            // 
            this.lbl_productAuthor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_productAuthor.Location = new System.Drawing.Point(382, 63);
            this.lbl_productAuthor.Margin = new System.Windows.Forms.Padding(16, 0, 8, 0);
            this.lbl_productAuthor.MaximumSize = new System.Drawing.Size(0, 41);
            this.lbl_productAuthor.Name = "lbl_productAuthor";
            this.lbl_productAuthor.Size = new System.Drawing.Size(722, 41);
            this.lbl_productAuthor.TabIndex = 0;
            this.lbl_productAuthor.Text = "Product Author";
            this.lbl_productAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_ogProjectAuthor
            // 
            this.lbl_ogProjectAuthor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_ogProjectAuthor.Location = new System.Drawing.Point(382, 126);
            this.lbl_ogProjectAuthor.Margin = new System.Windows.Forms.Padding(16, 0, 8, 0);
            this.lbl_ogProjectAuthor.MaximumSize = new System.Drawing.Size(0, 41);
            this.lbl_ogProjectAuthor.Name = "lbl_ogProjectAuthor";
            this.lbl_ogProjectAuthor.Size = new System.Drawing.Size(722, 41);
            this.lbl_ogProjectAuthor.TabIndex = 21;
            this.lbl_ogProjectAuthor.Text = "Original Project Author";
            this.lbl_ogProjectAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_cliVersion
            // 
            this.lbl_cliVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_cliVersion.Location = new System.Drawing.Point(382, 189);
            this.lbl_cliVersion.Margin = new System.Windows.Forms.Padding(16, 0, 8, 0);
            this.lbl_cliVersion.MaximumSize = new System.Drawing.Size(0, 41);
            this.lbl_cliVersion.Name = "lbl_cliVersion";
            this.lbl_cliVersion.Size = new System.Drawing.Size(722, 41);
            this.lbl_cliVersion.TabIndex = 22;
            this.lbl_cliVersion.Text = "Command-line Interface Version";
            this.lbl_cliVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_license
            // 
            this.txt_license.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_license.Location = new System.Drawing.Point(382, 259);
            this.txt_license.Margin = new System.Windows.Forms.Padding(16, 7, 8, 7);
            this.txt_license.Multiline = true;
            this.txt_license.Name = "txt_license";
            this.txt_license.ReadOnly = true;
            this.txt_license.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_license.Size = new System.Drawing.Size(722, 302);
            this.txt_license.TabIndex = 23;
            this.txt_license.TabStop = false;
            this.txt_license.Text = "License";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(904, 575);
            this.okButton.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(200, 51);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 675);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(24, 21, 24, 21);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutBox";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label lbl_productName;
        private System.Windows.Forms.Label lbl_productAuthor;
        private System.Windows.Forms.Label lbl_ogProjectAuthor;
        private System.Windows.Forms.Label lbl_cliVersion;
        private System.Windows.Forms.TextBox txt_license;
        private System.Windows.Forms.Button okButton;
    }
}
