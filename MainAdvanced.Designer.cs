namespace VGAudio.Win32
{
    partial class MainAdvanced
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
            this.lst_brstm_audioFormat = new System.Windows.Forms.ComboBox();
            this.lbl_brstm_audioFormat = new System.Windows.Forms.Label();
            this.lbl_brstm_settings = new System.Windows.Forms.Label();
            this.lbl_hca_audioQuality = new System.Windows.Forms.Label();
            this.lst_hca_audioQuality = new System.Windows.Forms.ComboBox();
            this.chk_advanced = new System.Windows.Forms.CheckBox();
            this.lbl_hca_settings = new System.Windows.Forms.Label();
            this.lbl_brstm_line = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lst_brstm_audioFormat
            // 
            this.lst_brstm_audioFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lst_brstm_audioFormat.FormattingEnabled = true;
            this.lst_brstm_audioFormat.Items.AddRange(new object[] {
            "DSP-ADPCM",
            "16-bit PCM",
            "8-bit PCM"});
            this.lst_brstm_audioFormat.Location = new System.Drawing.Point(209, 100);
            this.lst_brstm_audioFormat.Name = "lst_brstm_audioFormat";
            this.lst_brstm_audioFormat.Size = new System.Drawing.Size(216, 39);
            this.lst_brstm_audioFormat.TabIndex = 13;
            // 
            // lbl_brstm_audioFormat
            // 
            this.lbl_brstm_audioFormat.AutoSize = true;
            this.lbl_brstm_audioFormat.Location = new System.Drawing.Point(19, 100);
            this.lbl_brstm_audioFormat.Name = "lbl_brstm_audioFormat";
            this.lbl_brstm_audioFormat.Size = new System.Drawing.Size(184, 32);
            this.lbl_brstm_audioFormat.TabIndex = 14;
            this.lbl_brstm_audioFormat.Text = "Audio format:";
            // 
            // lbl_brstm_settings
            // 
            this.lbl_brstm_settings.AutoSize = true;
            this.lbl_brstm_settings.Location = new System.Drawing.Point(17, 56);
            this.lbl_brstm_settings.Name = "lbl_brstm_settings";
            this.lbl_brstm_settings.Size = new System.Drawing.Size(227, 32);
            this.lbl_brstm_settings.TabIndex = 15;
            this.lbl_brstm_settings.Text = "BRSTM settings:";
            // 
            // lbl_hca_audioQuality
            // 
            this.lbl_hca_audioQuality.AutoSize = true;
            this.lbl_hca_audioQuality.Location = new System.Drawing.Point(21, 217);
            this.lbl_hca_audioQuality.Name = "lbl_hca_audioQuality";
            this.lbl_hca_audioQuality.Size = new System.Drawing.Size(188, 32);
            this.lbl_hca_audioQuality.TabIndex = 18;
            this.lbl_hca_audioQuality.Text = "Audio quality:";
            // 
            // lst_hca_audioQuality
            // 
            this.lst_hca_audioQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lst_hca_audioQuality.FormattingEnabled = true;
            this.lst_hca_audioQuality.Items.AddRange(new object[] {
            "Highest",
            "High",
            "Middle",
            "Low",
            "Lowest"});
            this.lst_hca_audioQuality.Location = new System.Drawing.Point(211, 217);
            this.lst_hca_audioQuality.Name = "lst_hca_audioQuality";
            this.lst_hca_audioQuality.Size = new System.Drawing.Size(216, 39);
            this.lst_hca_audioQuality.TabIndex = 17;
            // 
            // chk_advanced
            // 
            this.chk_advanced.AutoSize = true;
            this.chk_advanced.Location = new System.Drawing.Point(13, 13);
            this.chk_advanced.Name = "chk_advanced";
            this.chk_advanced.Size = new System.Drawing.Size(334, 36);
            this.chk_advanced.TabIndex = 20;
            this.chk_advanced.Text = "Use advanced options";
            this.chk_advanced.UseVisualStyleBackColor = true;
            this.chk_advanced.CheckedChanged += new System.EventHandler(this.AdvancedToggle);
            // 
            // lbl_hca_settings
            // 
            this.lbl_hca_settings.AutoSize = true;
            this.lbl_hca_settings.Location = new System.Drawing.Point(19, 173);
            this.lbl_hca_settings.Name = "lbl_hca_settings";
            this.lbl_hca_settings.Size = new System.Drawing.Size(188, 32);
            this.lbl_hca_settings.TabIndex = 19;
            this.lbl_hca_settings.Text = "HCA settings:";
            // 
            // lbl_brstm_line
            // 
            this.lbl_brstm_line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_brstm_line.Location = new System.Drawing.Point(0, 150);
            this.lbl_brstm_line.Name = "lbl_brstm_line";
            this.lbl_brstm_line.Size = new System.Drawing.Size(801, 2);
            this.lbl_brstm_line.TabIndex = 16;
            // 
            // MainAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chk_advanced);
            this.Controls.Add(this.lbl_hca_settings);
            this.Controls.Add(this.lbl_hca_audioQuality);
            this.Controls.Add(this.lst_hca_audioQuality);
            this.Controls.Add(this.lbl_brstm_line);
            this.Controls.Add(this.lbl_brstm_settings);
            this.Controls.Add(this.lbl_brstm_audioFormat);
            this.Controls.Add(this.lst_brstm_audioFormat);
            this.Name = "MainAdvanced";
            this.Text = "VGAudio";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClose);
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox lst_brstm_audioFormat;
        private System.Windows.Forms.Label lbl_brstm_audioFormat;
        private System.Windows.Forms.Label lbl_brstm_settings;
        private System.Windows.Forms.Label lbl_hca_audioQuality;
        private System.Windows.Forms.ComboBox lst_hca_audioQuality;
        private System.Windows.Forms.CheckBox chk_advanced;
        private System.Windows.Forms.Label lbl_hca_settings;
        private System.Windows.Forms.Label lbl_brstm_line;
    }
}