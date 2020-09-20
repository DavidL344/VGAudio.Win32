namespace VGAudio.Win32
{
    partial class BatchConvert
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
            this.lb_importedFiles = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lb_importedFiles
            // 
            this.lb_importedFiles.DisplayMember = "l";
            this.lb_importedFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_importedFiles.FormattingEnabled = true;
            this.lb_importedFiles.ItemHeight = 31;
            this.lb_importedFiles.Location = new System.Drawing.Point(0, 0);
            this.lb_importedFiles.Name = "lb_importedFiles";
            this.lb_importedFiles.Size = new System.Drawing.Size(800, 128);
            this.lb_importedFiles.TabIndex = 0;
            // 
            // BatchConvert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lb_importedFiles);
            this.Name = "BatchConvert";
            this.Text = "VGAudio";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lb_importedFiles;
    }
}