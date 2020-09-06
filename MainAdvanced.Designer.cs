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
            this.lst_brstm_audioFormat.Location = new System.Drawing.Point(203, 13);
            this.lst_brstm_audioFormat.Name = "lst_brstm_audioFormat";
            this.lst_brstm_audioFormat.Size = new System.Drawing.Size(216, 39);
            this.lst_brstm_audioFormat.TabIndex = 13;
            this.lst_brstm_audioFormat.Visible = false;
            this.lst_brstm_audioFormat.SelectedIndexChanged += new System.EventHandler(this.lst_brstm_audioFormat_SelectedIndexChanged);
            // 
            // lbl_brstm_audioFormat
            // 
            this.lbl_brstm_audioFormat.AutoSize = true;
            this.lbl_brstm_audioFormat.Location = new System.Drawing.Point(13, 13);
            this.lbl_brstm_audioFormat.Name = "lbl_brstm_audioFormat";
            this.lbl_brstm_audioFormat.Size = new System.Drawing.Size(184, 32);
            this.lbl_brstm_audioFormat.TabIndex = 14;
            this.lbl_brstm_audioFormat.Text = "Audio format:";
            this.lbl_brstm_audioFormat.Visible = false;
            // 
            // MainAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lbl_brstm_audioFormat);
            this.Controls.Add(this.lst_brstm_audioFormat);
            this.Name = "MainAdvanced";
            this.Text = "VGAudio";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox lst_brstm_audioFormat;
        private System.Windows.Forms.Label lbl_brstm_audioFormat;
    }
}