using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public partial class MainDump : Form
    {
        public Dictionary<string, object> Options = new Dictionary<string, object>();
        public MainDump(string filePath, string newFileExtension)
        {
            InitializeComponent();

            MinimumSize = Size;
            MaximumSize = Size;
            MaximizeBox = false;

            string fileLocationNewExtension = Path.ChangeExtension(filePath, newFileExtension);
            chk_file_dumpFileInfo.Checked = true;
            chk_file_saveExportInfo.Checked = false;
            chk_options_dumpFileInfo_exportInfomation.Checked = false;
            txt_options_dumpFileInfo_fileLocation.Text = filePath + ".dump";
            txt_options_saveExportInfo_fileLocation.Text = fileLocationNewExtension + ".bat";
        }

        private void OnUpdate(object sender, EventArgs e)
        {
            // TODO: implement the feature so that this can be enabled
            /*
            txt_options_dumpFileInfo_fileLocation.Enabled = btn_options_dumpFileInfo_fileLocation.Enabled = chk_file_dumpFileInfo.Checked;
            txt_options_saveExportInfo_fileLocation.Enabled = btn_options_saveExportInfo_fileLocation.Enabled = chk_file_saveExportInfo.Checked;
            */
            
            txt_options_dumpFileInfo_fileLocation.Enabled = btn_options_dumpFileInfo_fileLocation.Enabled = false;
            txt_options_saveExportInfo_fileLocation.Enabled = btn_options_saveExportInfo_fileLocation.Enabled = chk_file_saveExportInfo.Enabled = false;
        }

        private void OnConfirm(object sender, EventArgs e)
        {
            Options["DumpFileInfo"] = new Dictionary<string, object> {
                { "Use", chk_file_dumpFileInfo.Checked },
                { "FileLocation", txt_options_dumpFileInfo_fileLocation.Text },
                { "IncludeExportInformation", chk_options_dumpFileInfo_exportInfomation.Checked }
            };

            Options["SaveExportInfo"] = new Dictionary<string, object> {
                { "Use", chk_file_saveExportInfo.Checked },
                { "FileLocation", txt_options_saveExportInfo_fileLocation.Text }
            };

            Options["Confirmed"] = true;
            this.Close();
        }
    }
}
