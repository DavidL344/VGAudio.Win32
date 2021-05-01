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
            this.lbl_options = new System.Windows.Forms.Label();
            this.chk_advanced = new System.Windows.Forms.CheckBox();
            this.pnl_brstm = new System.Windows.Forms.Panel();
            this.rb_hca_audioBitrate = new System.Windows.Forms.RadioButton();
            this.rb_hca_audioQuality = new System.Windows.Forms.RadioButton();
            this.lst_hca_audioQuality = new System.Windows.Forms.ComboBox();
            this.pnl_hca = new System.Windows.Forms.Panel();
            this.num_hca_audioBitrate = new System.Windows.Forms.NumericUpDown();
            this.lbl_hca_conversion = new System.Windows.Forms.Label();
            this.chk_hca_limitBitrate = new System.Windows.Forms.CheckBox();
            this.pnl_adx = new System.Windows.Forms.Panel();
            this.num_adx_encrypt_version = new System.Windows.Forms.NumericUpDown();
            this.chk_adx_encrypt_version = new System.Windows.Forms.CheckBox();
            this.num_adx_encrypt_filter = new System.Windows.Forms.NumericUpDown();
            this.chk_adx_encrypt_filter = new System.Windows.Forms.CheckBox();
            this.txt_adx_encrypt_keycode = new System.Windows.Forms.TextBox();
            this.chk_adx_encrypt_keycode = new System.Windows.Forms.CheckBox();
            this.txt_adx_encrypt_keystring = new System.Windows.Forms.TextBox();
            this.chk_adx_encrypt_keystring = new System.Windows.Forms.CheckBox();
            this.chk_adx_encrypt = new System.Windows.Forms.CheckBox();
            this.lst_adx_type = new System.Windows.Forms.ComboBox();
            this.pnl_brstm.SuspendLayout();
            this.pnl_hca.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_hca_audioBitrate)).BeginInit();
            this.pnl_adx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_adx_encrypt_version)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_adx_encrypt_filter)).BeginInit();
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
            this.lst_brstm_audioFormat.Location = new System.Drawing.Point(193, 11);
            this.lst_brstm_audioFormat.Name = "lst_brstm_audioFormat";
            this.lst_brstm_audioFormat.Size = new System.Drawing.Size(216, 39);
            this.lst_brstm_audioFormat.TabIndex = 13;
            // 
            // lbl_brstm_audioFormat
            // 
            this.lbl_brstm_audioFormat.AutoSize = true;
            this.lbl_brstm_audioFormat.Location = new System.Drawing.Point(3, 11);
            this.lbl_brstm_audioFormat.Name = "lbl_brstm_audioFormat";
            this.lbl_brstm_audioFormat.Size = new System.Drawing.Size(184, 32);
            this.lbl_brstm_audioFormat.TabIndex = 14;
            this.lbl_brstm_audioFormat.Text = "Audio format:";
            // 
            // lbl_options
            // 
            this.lbl_options.AutoSize = true;
            this.lbl_options.Location = new System.Drawing.Point(23, 66);
            this.lbl_options.Name = "lbl_options";
            this.lbl_options.Size = new System.Drawing.Size(264, 32);
            this.lbl_options.TabIndex = 15;
            this.lbl_options.Text = "(extension) options:";
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
            // pnl_brstm
            // 
            this.pnl_brstm.Controls.Add(this.lbl_brstm_audioFormat);
            this.pnl_brstm.Controls.Add(this.lst_brstm_audioFormat);
            this.pnl_brstm.Location = new System.Drawing.Point(29, 110);
            this.pnl_brstm.Name = "pnl_brstm";
            this.pnl_brstm.Size = new System.Drawing.Size(421, 62);
            this.pnl_brstm.TabIndex = 24;
            // 
            // rb_hca_audioBitrate
            // 
            this.rb_hca_audioBitrate.AutoSize = true;
            this.rb_hca_audioBitrate.Location = new System.Drawing.Point(240, 12);
            this.rb_hca_audioBitrate.Name = "rb_hca_audioBitrate";
            this.rb_hca_audioBitrate.Size = new System.Drawing.Size(213, 36);
            this.rb_hca_audioBitrate.TabIndex = 22;
            this.rb_hca_audioBitrate.TabStop = true;
            this.rb_hca_audioBitrate.Text = "Audio bitrate";
            this.rb_hca_audioBitrate.UseVisualStyleBackColor = true;
            this.rb_hca_audioBitrate.CheckedChanged += new System.EventHandler(this.HcaCheckboxToggle);
            // 
            // rb_hca_audioQuality
            // 
            this.rb_hca_audioQuality.AutoSize = true;
            this.rb_hca_audioQuality.Location = new System.Drawing.Point(8, 12);
            this.rb_hca_audioQuality.Name = "rb_hca_audioQuality";
            this.rb_hca_audioQuality.Size = new System.Drawing.Size(217, 36);
            this.rb_hca_audioQuality.TabIndex = 21;
            this.rb_hca_audioQuality.Text = "Audio quality";
            this.rb_hca_audioQuality.UseVisualStyleBackColor = true;
            this.rb_hca_audioQuality.CheckedChanged += new System.EventHandler(this.HcaCheckboxToggle);
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
            this.lst_hca_audioQuality.Location = new System.Drawing.Point(9, 67);
            this.lst_hca_audioQuality.Name = "lst_hca_audioQuality";
            this.lst_hca_audioQuality.Size = new System.Drawing.Size(216, 39);
            this.lst_hca_audioQuality.TabIndex = 17;
            this.lst_hca_audioQuality.Visible = false;
            this.lst_hca_audioQuality.SelectedValueChanged += new System.EventHandler(this.UpdateApproxKbps);
            // 
            // pnl_hca
            // 
            this.pnl_hca.Controls.Add(this.lst_hca_audioQuality);
            this.pnl_hca.Controls.Add(this.num_hca_audioBitrate);
            this.pnl_hca.Controls.Add(this.lbl_hca_conversion);
            this.pnl_hca.Controls.Add(this.chk_hca_limitBitrate);
            this.pnl_hca.Controls.Add(this.rb_hca_audioBitrate);
            this.pnl_hca.Controls.Add(this.rb_hca_audioQuality);
            this.pnl_hca.Location = new System.Drawing.Point(29, 110);
            this.pnl_hca.Name = "pnl_hca";
            this.pnl_hca.Size = new System.Drawing.Size(549, 220);
            this.pnl_hca.TabIndex = 26;
            // 
            // num_hca_audioBitrate
            // 
            this.num_hca_audioBitrate.Location = new System.Drawing.Point(9, 68);
            this.num_hca_audioBitrate.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.num_hca_audioBitrate.Minimum = new decimal(new int[] {
            8958,
            0,
            0,
            0});
            this.num_hca_audioBitrate.Name = "num_hca_audioBitrate";
            this.num_hca_audioBitrate.Size = new System.Drawing.Size(216, 38);
            this.num_hca_audioBitrate.TabIndex = 23;
            this.num_hca_audioBitrate.Value = new decimal(new int[] {
            235000,
            0,
            0,
            0});
            this.num_hca_audioBitrate.Visible = false;
            this.num_hca_audioBitrate.ValueChanged += new System.EventHandler(this.UpdateKbpsConversion);
            // 
            // lbl_hca_conversion
            // 
            this.lbl_hca_conversion.AutoSize = true;
            this.lbl_hca_conversion.Location = new System.Drawing.Point(232, 68);
            this.lbl_hca_conversion.Name = "lbl_hca_conversion";
            this.lbl_hca_conversion.Size = new System.Drawing.Size(281, 32);
            this.lbl_hca_conversion.TabIndex = 25;
            this.lbl_hca_conversion.Text = "bps to Kbps / approx.";
            this.lbl_hca_conversion.Visible = false;
            // 
            // chk_hca_limitBitrate
            // 
            this.chk_hca_limitBitrate.AutoSize = true;
            this.chk_hca_limitBitrate.Location = new System.Drawing.Point(9, 125);
            this.chk_hca_limitBitrate.Name = "chk_hca_limitBitrate";
            this.chk_hca_limitBitrate.Size = new System.Drawing.Size(201, 36);
            this.chk_hca_limitBitrate.TabIndex = 24;
            this.chk_hca_limitBitrate.Text = "Limit bitrate";
            this.chk_hca_limitBitrate.UseVisualStyleBackColor = true;
            // 
            // pnl_adx
            // 
            this.pnl_adx.Controls.Add(this.num_adx_encrypt_version);
            this.pnl_adx.Controls.Add(this.chk_adx_encrypt_version);
            this.pnl_adx.Controls.Add(this.num_adx_encrypt_filter);
            this.pnl_adx.Controls.Add(this.chk_adx_encrypt_filter);
            this.pnl_adx.Controls.Add(this.txt_adx_encrypt_keycode);
            this.pnl_adx.Controls.Add(this.chk_adx_encrypt_keycode);
            this.pnl_adx.Controls.Add(this.txt_adx_encrypt_keystring);
            this.pnl_adx.Controls.Add(this.chk_adx_encrypt_keystring);
            this.pnl_adx.Controls.Add(this.chk_adx_encrypt);
            this.pnl_adx.Controls.Add(this.lst_adx_type);
            this.pnl_adx.Location = new System.Drawing.Point(29, 110);
            this.pnl_adx.Name = "pnl_adx";
            this.pnl_adx.Size = new System.Drawing.Size(565, 328);
            this.pnl_adx.TabIndex = 26;
            // 
            // num_adx_encrypt_version
            // 
            this.num_adx_encrypt_version.Location = new System.Drawing.Point(240, 235);
            this.num_adx_encrypt_version.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.num_adx_encrypt_version.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.num_adx_encrypt_version.Name = "num_adx_encrypt_version";
            this.num_adx_encrypt_version.Size = new System.Drawing.Size(120, 38);
            this.num_adx_encrypt_version.TabIndex = 23;
            this.num_adx_encrypt_version.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // chk_adx_encrypt_version
            // 
            this.chk_adx_encrypt_version.AutoSize = true;
            this.chk_adx_encrypt_version.Location = new System.Drawing.Point(54, 235);
            this.chk_adx_encrypt_version.Name = "chk_adx_encrypt_version";
            this.chk_adx_encrypt_version.Size = new System.Drawing.Size(158, 36);
            this.chk_adx_encrypt_version.TabIndex = 22;
            this.chk_adx_encrypt_version.Text = "Version:";
            this.chk_adx_encrypt_version.UseVisualStyleBackColor = true;
            this.chk_adx_encrypt_version.CheckedChanged += new System.EventHandler(this.AdxCheckboxToggle);
            // 
            // num_adx_encrypt_filter
            // 
            this.num_adx_encrypt_filter.Location = new System.Drawing.Point(240, 179);
            this.num_adx_encrypt_filter.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.num_adx_encrypt_filter.Name = "num_adx_encrypt_filter";
            this.num_adx_encrypt_filter.Size = new System.Drawing.Size(120, 38);
            this.num_adx_encrypt_filter.TabIndex = 21;
            // 
            // chk_adx_encrypt_filter
            // 
            this.chk_adx_encrypt_filter.AutoSize = true;
            this.chk_adx_encrypt_filter.Location = new System.Drawing.Point(54, 179);
            this.chk_adx_encrypt_filter.Name = "chk_adx_encrypt_filter";
            this.chk_adx_encrypt_filter.Size = new System.Drawing.Size(125, 36);
            this.chk_adx_encrypt_filter.TabIndex = 20;
            this.chk_adx_encrypt_filter.Text = "Filter:";
            this.chk_adx_encrypt_filter.UseVisualStyleBackColor = true;
            this.chk_adx_encrypt_filter.CheckedChanged += new System.EventHandler(this.AdxCheckboxToggle);
            // 
            // txt_adx_encrypt_keycode
            // 
            this.txt_adx_encrypt_keycode.Location = new System.Drawing.Point(240, 123);
            this.txt_adx_encrypt_keycode.Name = "txt_adx_encrypt_keycode";
            this.txt_adx_encrypt_keycode.Size = new System.Drawing.Size(309, 38);
            this.txt_adx_encrypt_keycode.TabIndex = 19;
            // 
            // chk_adx_encrypt_keycode
            // 
            this.chk_adx_encrypt_keycode.AutoSize = true;
            this.chk_adx_encrypt_keycode.Location = new System.Drawing.Point(54, 123);
            this.chk_adx_encrypt_keycode.Name = "chk_adx_encrypt_keycode";
            this.chk_adx_encrypt_keycode.Size = new System.Drawing.Size(172, 36);
            this.chk_adx_encrypt_keycode.TabIndex = 18;
            this.chk_adx_encrypt_keycode.Text = "Keycode:";
            this.chk_adx_encrypt_keycode.UseVisualStyleBackColor = true;
            this.chk_adx_encrypt_keycode.CheckedChanged += new System.EventHandler(this.AdxCheckboxToggle);
            // 
            // txt_adx_encrypt_keystring
            // 
            this.txt_adx_encrypt_keystring.Location = new System.Drawing.Point(240, 68);
            this.txt_adx_encrypt_keystring.Name = "txt_adx_encrypt_keystring";
            this.txt_adx_encrypt_keystring.Size = new System.Drawing.Size(309, 38);
            this.txt_adx_encrypt_keystring.TabIndex = 17;
            // 
            // chk_adx_encrypt_keystring
            // 
            this.chk_adx_encrypt_keystring.AutoSize = true;
            this.chk_adx_encrypt_keystring.Location = new System.Drawing.Point(54, 68);
            this.chk_adx_encrypt_keystring.Name = "chk_adx_encrypt_keystring";
            this.chk_adx_encrypt_keystring.Size = new System.Drawing.Size(180, 36);
            this.chk_adx_encrypt_keystring.TabIndex = 16;
            this.chk_adx_encrypt_keystring.Text = "Keystring:";
            this.chk_adx_encrypt_keystring.UseVisualStyleBackColor = true;
            this.chk_adx_encrypt_keystring.CheckedChanged += new System.EventHandler(this.AdxCheckboxToggle);
            // 
            // chk_adx_encrypt
            // 
            this.chk_adx_encrypt.AutoSize = true;
            this.chk_adx_encrypt.Location = new System.Drawing.Point(18, 14);
            this.chk_adx_encrypt.Name = "chk_adx_encrypt";
            this.chk_adx_encrypt.Size = new System.Drawing.Size(149, 36);
            this.chk_adx_encrypt.TabIndex = 15;
            this.chk_adx_encrypt.Text = "Encrypt";
            this.chk_adx_encrypt.UseVisualStyleBackColor = true;
            this.chk_adx_encrypt.CheckedChanged += new System.EventHandler(this.AdxCheckboxToggle);
            // 
            // lst_adx_type
            // 
            this.lst_adx_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lst_adx_type.FormattingEnabled = true;
            this.lst_adx_type.Items.AddRange(new object[] {
            "Linear",
            "Fixed",
            "Exponential"});
            this.lst_adx_type.Location = new System.Drawing.Point(281, 14);
            this.lst_adx_type.Name = "lst_adx_type";
            this.lst_adx_type.Size = new System.Drawing.Size(216, 39);
            this.lst_adx_type.TabIndex = 13;
            // 
            // MainAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnl_adx);
            this.Controls.Add(this.pnl_hca);
            this.Controls.Add(this.pnl_brstm);
            this.Controls.Add(this.chk_advanced);
            this.Controls.Add(this.lbl_options);
            this.Name = "MainAdvanced";
            this.Text = "VGAudio";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClose);
            this.Load += new System.EventHandler(this.OnLoad);
            this.pnl_brstm.ResumeLayout(false);
            this.pnl_brstm.PerformLayout();
            this.pnl_hca.ResumeLayout(false);
            this.pnl_hca.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_hca_audioBitrate)).EndInit();
            this.pnl_adx.ResumeLayout(false);
            this.pnl_adx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_adx_encrypt_version)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_adx_encrypt_filter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox lst_brstm_audioFormat;
        private System.Windows.Forms.Label lbl_brstm_audioFormat;
        private System.Windows.Forms.Label lbl_options;
        private System.Windows.Forms.CheckBox chk_advanced;
        private System.Windows.Forms.Panel pnl_brstm;
        private System.Windows.Forms.RadioButton rb_hca_audioBitrate;
        private System.Windows.Forms.RadioButton rb_hca_audioQuality;
        private System.Windows.Forms.ComboBox lst_hca_audioQuality;
        private System.Windows.Forms.Panel pnl_hca;
        private System.Windows.Forms.NumericUpDown num_hca_audioBitrate;
        private System.Windows.Forms.CheckBox chk_hca_limitBitrate;
        private System.Windows.Forms.Label lbl_hca_conversion;
        private System.Windows.Forms.Panel pnl_adx;
        private System.Windows.Forms.TextBox txt_adx_encrypt_keycode;
        private System.Windows.Forms.CheckBox chk_adx_encrypt_keycode;
        private System.Windows.Forms.TextBox txt_adx_encrypt_keystring;
        private System.Windows.Forms.CheckBox chk_adx_encrypt_keystring;
        private System.Windows.Forms.CheckBox chk_adx_encrypt;
        private System.Windows.Forms.ComboBox lst_adx_type;
        private System.Windows.Forms.CheckBox chk_adx_encrypt_filter;
        private System.Windows.Forms.NumericUpDown num_adx_encrypt_filter;
        private System.Windows.Forms.NumericUpDown num_adx_encrypt_version;
        private System.Windows.Forms.CheckBox chk_adx_encrypt_version;
    }
}