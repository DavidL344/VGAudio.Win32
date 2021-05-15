namespace VGAudio.Win32
{
    partial class MainDump
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
            this.btn_confirm = new System.Windows.Forms.Button();
            this.chk_options_dumpFileInfo_exportInfomation = new System.Windows.Forms.CheckBox();
            this.lbl_options_saveExportInfo = new System.Windows.Forms.Label();
            this.lbl_options_dumpFileInfo = new System.Windows.Forms.Label();
            this.lbl_options_saveExportInfo_fileLocation = new System.Windows.Forms.Label();
            this.txt_options_saveExportInfo_fileLocation = new System.Windows.Forms.TextBox();
            this.btn_options_saveExportInfo_fileLocation = new System.Windows.Forms.Button();
            this.chk_file_dumpFileInfo = new System.Windows.Forms.CheckBox();
            this.chk_file_saveExportInfo = new System.Windows.Forms.CheckBox();
            this.btn_options_dumpFileInfo_fileLocation = new System.Windows.Forms.Button();
            this.txt_options_dumpFileInfo_fileLocation = new System.Windows.Forms.TextBox();
            this.lbl_options_dumpFileInfo_fileLocation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_confirm
            // 
            this.btn_confirm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_confirm.Location = new System.Drawing.Point(295, 384);
            this.btn_confirm.Name = "btn_confirm";
            this.btn_confirm.Size = new System.Drawing.Size(172, 54);
            this.btn_confirm.TabIndex = 12;
            this.btn_confirm.Text = "OK";
            this.btn_confirm.UseVisualStyleBackColor = true;
            this.btn_confirm.Click += new System.EventHandler(this.OnConfirm);
            // 
            // chk_options_dumpFileInfo_exportInfomation
            // 
            this.chk_options_dumpFileInfo_exportInfomation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chk_options_dumpFileInfo_exportInfomation.AutoSize = true;
            this.chk_options_dumpFileInfo_exportInfomation.Location = new System.Drawing.Point(52, 209);
            this.chk_options_dumpFileInfo_exportInfomation.Name = "chk_options_dumpFileInfo_exportInfomation";
            this.chk_options_dumpFileInfo_exportInfomation.Size = new System.Drawing.Size(380, 36);
            this.chk_options_dumpFileInfo_exportInfomation.TabIndex = 14;
            this.chk_options_dumpFileInfo_exportInfomation.Text = "Include export information";
            this.chk_options_dumpFileInfo_exportInfomation.UseVisualStyleBackColor = true;
            this.chk_options_dumpFileInfo_exportInfomation.CheckedChanged += new System.EventHandler(this.OnUpdate);
            // 
            // lbl_options_saveExportInfo
            // 
            this.lbl_options_saveExportInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_options_saveExportInfo.AutoSize = true;
            this.lbl_options_saveExportInfo.Location = new System.Drawing.Point(30, 272);
            this.lbl_options_saveExportInfo.Name = "lbl_options_saveExportInfo";
            this.lbl_options_saveExportInfo.Size = new System.Drawing.Size(487, 32);
            this.lbl_options_saveExportInfo.TabIndex = 18;
            this.lbl_options_saveExportInfo.Text = "Options for saving export information:";
            // 
            // lbl_options_dumpFileInfo
            // 
            this.lbl_options_dumpFileInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_options_dumpFileInfo.AutoSize = true;
            this.lbl_options_dumpFileInfo.Location = new System.Drawing.Point(30, 118);
            this.lbl_options_dumpFileInfo.Name = "lbl_options_dumpFileInfo";
            this.lbl_options_dumpFileInfo.Size = new System.Drawing.Size(473, 32);
            this.lbl_options_dumpFileInfo.TabIndex = 19;
            this.lbl_options_dumpFileInfo.Text = "Options for dumping file information:";
            // 
            // lbl_options_saveExportInfo_fileLocation
            // 
            this.lbl_options_saveExportInfo_fileLocation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_options_saveExportInfo_fileLocation.AutoSize = true;
            this.lbl_options_saveExportInfo_fileLocation.Location = new System.Drawing.Point(46, 316);
            this.lbl_options_saveExportInfo_fileLocation.Name = "lbl_options_saveExportInfo_fileLocation";
            this.lbl_options_saveExportInfo_fileLocation.Size = new System.Drawing.Size(177, 32);
            this.lbl_options_saveExportInfo_fileLocation.TabIndex = 20;
            this.lbl_options_saveExportInfo_fileLocation.Text = "File location:";
            // 
            // txt_options_saveExportInfo_fileLocation
            // 
            this.txt_options_saveExportInfo_fileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_options_saveExportInfo_fileLocation.Location = new System.Drawing.Point(236, 316);
            this.txt_options_saveExportInfo_fileLocation.Name = "txt_options_saveExportInfo_fileLocation";
            this.txt_options_saveExportInfo_fileLocation.Size = new System.Drawing.Size(365, 38);
            this.txt_options_saveExportInfo_fileLocation.TabIndex = 21;
            this.txt_options_saveExportInfo_fileLocation.TextChanged += new System.EventHandler(this.OnUpdate);
            // 
            // btn_options_saveExportInfo_fileLocation
            // 
            this.btn_options_saveExportInfo_fileLocation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_options_saveExportInfo_fileLocation.Location = new System.Drawing.Point(613, 307);
            this.btn_options_saveExportInfo_fileLocation.Name = "btn_options_saveExportInfo_fileLocation";
            this.btn_options_saveExportInfo_fileLocation.Size = new System.Drawing.Size(172, 54);
            this.btn_options_saveExportInfo_fileLocation.TabIndex = 22;
            this.btn_options_saveExportInfo_fileLocation.Text = "Browse";
            this.btn_options_saveExportInfo_fileLocation.UseVisualStyleBackColor = true;
            this.btn_options_saveExportInfo_fileLocation.Click += new System.EventHandler(this.FileBrowse);
            // 
            // chk_file_dumpFileInfo
            // 
            this.chk_file_dumpFileInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chk_file_dumpFileInfo.AutoSize = true;
            this.chk_file_dumpFileInfo.Location = new System.Drawing.Point(12, 13);
            this.chk_file_dumpFileInfo.Name = "chk_file_dumpFileInfo";
            this.chk_file_dumpFileInfo.Size = new System.Drawing.Size(426, 36);
            this.chk_file_dumpFileInfo.TabIndex = 23;
            this.chk_file_dumpFileInfo.Text = "Dump file information (.dump)";
            this.chk_file_dumpFileInfo.UseVisualStyleBackColor = true;
            this.chk_file_dumpFileInfo.CheckedChanged += new System.EventHandler(this.OnUpdate);
            // 
            // chk_file_saveExportInfo
            // 
            this.chk_file_saveExportInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chk_file_saveExportInfo.AutoSize = true;
            this.chk_file_saveExportInfo.Location = new System.Drawing.Point(12, 55);
            this.chk_file_saveExportInfo.Name = "chk_file_saveExportInfo";
            this.chk_file_saveExportInfo.Size = new System.Drawing.Size(426, 36);
            this.chk_file_saveExportInfo.TabIndex = 24;
            this.chk_file_saveExportInfo.Text = "Save export information (.bat)";
            this.chk_file_saveExportInfo.UseVisualStyleBackColor = true;
            this.chk_file_saveExportInfo.CheckedChanged += new System.EventHandler(this.OnUpdate);
            // 
            // btn_options_dumpFileInfo_fileLocation
            // 
            this.btn_options_dumpFileInfo_fileLocation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_options_dumpFileInfo_fileLocation.Location = new System.Drawing.Point(613, 152);
            this.btn_options_dumpFileInfo_fileLocation.Name = "btn_options_dumpFileInfo_fileLocation";
            this.btn_options_dumpFileInfo_fileLocation.Size = new System.Drawing.Size(172, 54);
            this.btn_options_dumpFileInfo_fileLocation.TabIndex = 27;
            this.btn_options_dumpFileInfo_fileLocation.Text = "Browse";
            this.btn_options_dumpFileInfo_fileLocation.UseVisualStyleBackColor = true;
            this.btn_options_dumpFileInfo_fileLocation.Click += new System.EventHandler(this.FileBrowse);
            // 
            // txt_options_dumpFileInfo_fileLocation
            // 
            this.txt_options_dumpFileInfo_fileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_options_dumpFileInfo_fileLocation.Location = new System.Drawing.Point(236, 161);
            this.txt_options_dumpFileInfo_fileLocation.Name = "txt_options_dumpFileInfo_fileLocation";
            this.txt_options_dumpFileInfo_fileLocation.Size = new System.Drawing.Size(365, 38);
            this.txt_options_dumpFileInfo_fileLocation.TabIndex = 26;
            // 
            // lbl_options_dumpFileInfo_fileLocation
            // 
            this.lbl_options_dumpFileInfo_fileLocation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_options_dumpFileInfo_fileLocation.AutoSize = true;
            this.lbl_options_dumpFileInfo_fileLocation.Location = new System.Drawing.Point(46, 161);
            this.lbl_options_dumpFileInfo_fileLocation.Name = "lbl_options_dumpFileInfo_fileLocation";
            this.lbl_options_dumpFileInfo_fileLocation.Size = new System.Drawing.Size(177, 32);
            this.lbl_options_dumpFileInfo_fileLocation.TabIndex = 25;
            this.lbl_options_dumpFileInfo_fileLocation.Text = "File location:";
            // 
            // MainDump
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_options_dumpFileInfo_fileLocation);
            this.Controls.Add(this.txt_options_dumpFileInfo_fileLocation);
            this.Controls.Add(this.lbl_options_dumpFileInfo_fileLocation);
            this.Controls.Add(this.chk_file_saveExportInfo);
            this.Controls.Add(this.chk_file_dumpFileInfo);
            this.Controls.Add(this.btn_options_saveExportInfo_fileLocation);
            this.Controls.Add(this.txt_options_saveExportInfo_fileLocation);
            this.Controls.Add(this.lbl_options_saveExportInfo_fileLocation);
            this.Controls.Add(this.lbl_options_dumpFileInfo);
            this.Controls.Add(this.lbl_options_saveExportInfo);
            this.Controls.Add(this.chk_options_dumpFileInfo_exportInfomation);
            this.Controls.Add(this.btn_confirm);
            this.Name = "MainDump";
            this.Text = "MainDump";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_confirm;
        private System.Windows.Forms.CheckBox chk_options_dumpFileInfo_exportInfomation;
        private System.Windows.Forms.Label lbl_options_saveExportInfo;
        private System.Windows.Forms.Label lbl_options_dumpFileInfo;
        private System.Windows.Forms.Label lbl_options_saveExportInfo_fileLocation;
        private System.Windows.Forms.TextBox txt_options_saveExportInfo_fileLocation;
        private System.Windows.Forms.Button btn_options_saveExportInfo_fileLocation;
        private System.Windows.Forms.CheckBox chk_file_dumpFileInfo;
        private System.Windows.Forms.CheckBox chk_file_saveExportInfo;
        private System.Windows.Forms.Button btn_options_dumpFileInfo_fileLocation;
        private System.Windows.Forms.TextBox txt_options_dumpFileInfo_fileLocation;
        private System.Windows.Forms.Label lbl_options_dumpFileInfo_fileLocation;
    }
}