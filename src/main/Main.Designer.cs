namespace VGAudio.Win32
{
    partial class Main
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.slb_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.btn_open = new System.Windows.Forms.Button();
            this.chk_loop = new System.Windows.Forms.CheckBox();
            this.num_loopStart = new System.Windows.Forms.NumericUpDown();
            this.num_loopEnd = new System.Windows.Forms.NumericUpDown();
            this.lbl_loopStart = new System.Windows.Forms.Label();
            this.lbl_loopEnd = new System.Windows.Forms.Label();
            this.lbl_exportAs = new System.Windows.Forms.Label();
            this.lst_exportExtensions = new System.Windows.Forms.ComboBox();
            this.btn_export = new System.Windows.Forms.Button();
            this.txt_metadata = new System.Windows.Forms.TextBox();
            this.btn_dump = new System.Windows.Forms.Button();
            this.btn_advancedOptions = new System.Windows.Forms.Button();
            this.lbl_dnd = new System.Windows.Forms.Label();
            this.prg_main = new System.Windows.Forms.ProgressBar();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_loopStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_loopEnd)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slb_status});
            this.statusStrip.Location = new System.Drawing.Point(0, 396);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 54);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip";
            // 
            // slb_status
            // 
            this.slb_status.Name = "slb_status";
            this.slb_status.Size = new System.Drawing.Size(707, 41);
            this.slb_status.Spring = true;
            this.slb_status.Text = "Loading...";
            this.slb_status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(299, 12);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(172, 54);
            this.btn_open.TabIndex = 0;
            this.btn_open.Text = "Open File";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.OpenFileForm);
            // 
            // chk_loop
            // 
            this.chk_loop.AutoSize = true;
            this.chk_loop.Location = new System.Drawing.Point(276, 157);
            this.chk_loop.Name = "chk_loop";
            this.chk_loop.Size = new System.Drawing.Size(209, 36);
            this.chk_loop.TabIndex = 1;
            this.chk_loop.Text = "Loop the file";
            this.chk_loop.UseVisualStyleBackColor = true;
            this.chk_loop.Visible = false;
            this.chk_loop.CheckedChanged += new System.EventHandler(this.LoopTheFile);
            // 
            // num_loopStart
            // 
            this.num_loopStart.Location = new System.Drawing.Point(324, 211);
            this.num_loopStart.Maximum = new decimal(new int[] {
            -1486618625,
            232830643,
            0,
            0});
            this.num_loopStart.Name = "num_loopStart";
            this.num_loopStart.Size = new System.Drawing.Size(281, 38);
            this.num_loopStart.TabIndex = 3;
            this.num_loopStart.Visible = false;
            this.num_loopStart.ValueChanged += new System.EventHandler(this.NumLoopOnUpdate);
            // 
            // num_loopEnd
            // 
            this.num_loopEnd.Location = new System.Drawing.Point(324, 263);
            this.num_loopEnd.Maximum = new decimal(new int[] {
            -1486618625,
            232830643,
            0,
            0});
            this.num_loopEnd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_loopEnd.Name = "num_loopEnd";
            this.num_loopEnd.Size = new System.Drawing.Size(281, 38);
            this.num_loopEnd.TabIndex = 4;
            this.num_loopEnd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_loopEnd.Visible = false;
            this.num_loopEnd.ValueChanged += new System.EventHandler(this.NumLoopOnUpdate);
            // 
            // lbl_loopStart
            // 
            this.lbl_loopStart.AutoSize = true;
            this.lbl_loopStart.Location = new System.Drawing.Point(164, 211);
            this.lbl_loopStart.Name = "lbl_loopStart";
            this.lbl_loopStart.Size = new System.Drawing.Size(154, 32);
            this.lbl_loopStart.TabIndex = 5;
            this.lbl_loopStart.Text = "Loop Start:";
            this.lbl_loopStart.Visible = false;
            // 
            // lbl_loopEnd
            // 
            this.lbl_loopEnd.AutoSize = true;
            this.lbl_loopEnd.Location = new System.Drawing.Point(173, 263);
            this.lbl_loopEnd.Name = "lbl_loopEnd";
            this.lbl_loopEnd.Size = new System.Drawing.Size(145, 32);
            this.lbl_loopEnd.TabIndex = 6;
            this.lbl_loopEnd.Text = "Loop End:";
            this.lbl_loopEnd.Visible = false;
            // 
            // lbl_exportAs
            // 
            this.lbl_exportAs.AutoSize = true;
            this.lbl_exportAs.Location = new System.Drawing.Point(227, 92);
            this.lbl_exportAs.Name = "lbl_exportAs";
            this.lbl_exportAs.Size = new System.Drawing.Size(145, 32);
            this.lbl_exportAs.TabIndex = 7;
            this.lbl_exportAs.Text = "Export As:";
            this.lbl_exportAs.Visible = false;
            // 
            // lst_exportExtensions
            // 
            this.lst_exportExtensions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lst_exportExtensions.FormattingEnabled = true;
            this.lst_exportExtensions.Items.AddRange(new object[] {
            "WAV",
            "DSP",
            "IDSP",
            "BRSTM",
            "BCSTM",
            "BFSTM",
            "HPS",
            "ADX",
            "HCA"});
            this.lst_exportExtensions.Location = new System.Drawing.Point(389, 89);
            this.lst_exportExtensions.Name = "lst_exportExtensions";
            this.lst_exportExtensions.Size = new System.Drawing.Size(161, 39);
            this.lst_exportExtensions.TabIndex = 8;
            this.lst_exportExtensions.Visible = false;
            this.lst_exportExtensions.SelectedIndexChanged += new System.EventHandler(this.ExportExtensionUpdater);
            // 
            // btn_export
            // 
            this.btn_export.Location = new System.Drawing.Point(200, 322);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(172, 54);
            this.btn_export.TabIndex = 9;
            this.btn_export.Text = "Export";
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Visible = false;
            this.btn_export.Click += new System.EventHandler(this.FileExport);
            // 
            // txt_metadata
            // 
            this.txt_metadata.Location = new System.Drawing.Point(170, 199);
            this.txt_metadata.Multiline = true;
            this.txt_metadata.Name = "txt_metadata";
            this.txt_metadata.ReadOnly = true;
            this.txt_metadata.Size = new System.Drawing.Size(446, 108);
            this.txt_metadata.TabIndex = 0;
            this.txt_metadata.Visible = false;
            // 
            // btn_dump
            // 
            this.btn_dump.Location = new System.Drawing.Point(389, 322);
            this.btn_dump.Name = "btn_dump";
            this.btn_dump.Size = new System.Drawing.Size(172, 54);
            this.btn_dump.TabIndex = 10;
            this.btn_dump.Text = "Dump Info";
            this.btn_dump.UseVisualStyleBackColor = true;
            this.btn_dump.Visible = false;
            this.btn_dump.Click += new System.EventHandler(this.FileDump);
            // 
            // btn_advancedOptions
            // 
            this.btn_advancedOptions.Location = new System.Drawing.Point(628, 396);
            this.btn_advancedOptions.Name = "btn_advancedOptions";
            this.btn_advancedOptions.Size = new System.Drawing.Size(172, 54);
            this.btn_advancedOptions.TabIndex = 11;
            this.btn_advancedOptions.Text = "Advanced";
            this.btn_advancedOptions.UseVisualStyleBackColor = true;
            this.btn_advancedOptions.Visible = false;
            this.btn_advancedOptions.Click += new System.EventHandler(this.OpenAdvancedOptions);
            // 
            // lbl_dnd
            // 
            this.lbl_dnd.AutoSize = true;
            this.lbl_dnd.Location = new System.Drawing.Point(164, 199);
            this.lbl_dnd.Name = "lbl_dnd";
            this.lbl_dnd.Size = new System.Drawing.Size(455, 32);
            this.lbl_dnd.TabIndex = 12;
            this.lbl_dnd.Text = "...or just drag and drop the file here";
            // 
            // prg_main
            // 
            this.prg_main.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.prg_main.Location = new System.Drawing.Point(0, -2);
            this.prg_main.MarqueeAnimationSpeed = 30;
            this.prg_main.Name = "prg_main";
            this.prg_main.Size = new System.Drawing.Size(801, 10);
            this.prg_main.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prg_main.TabIndex = 0;
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.prg_main);
            this.Controls.Add(this.btn_advancedOptions);
            this.Controls.Add(this.btn_dump);
            this.Controls.Add(this.btn_export);
            this.Controls.Add(this.lst_exportExtensions);
            this.Controls.Add(this.lbl_exportAs);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.chk_loop);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.lbl_loopEnd);
            this.Controls.Add(this.lbl_loopStart);
            this.Controls.Add(this.num_loopEnd);
            this.Controls.Add(this.num_loopStart);
            this.Controls.Add(this.txt_metadata);
            this.Controls.Add(this.lbl_dnd);
            this.Name = "Main";
            this.Text = "VGAudio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DragDropFile);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DragDropEffects);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_loopStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_loopEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel slb_status;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.CheckBox chk_loop;
        private System.Windows.Forms.NumericUpDown num_loopStart;
        private System.Windows.Forms.NumericUpDown num_loopEnd;
        private System.Windows.Forms.Label lbl_loopStart;
        private System.Windows.Forms.Label lbl_loopEnd;
        private System.Windows.Forms.Label lbl_exportAs;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.TextBox txt_metadata;
        private System.Windows.Forms.Button btn_dump;
        private System.Windows.Forms.Button btn_advancedOptions;
        private System.Windows.Forms.ComboBox lst_exportExtensions;
        private System.Windows.Forms.Label lbl_dnd;
        private System.Windows.Forms.ProgressBar prg_main;
    }
}

