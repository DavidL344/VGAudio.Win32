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
        public Dictionary<string, Dictionary<string, object>> Options = new Dictionary<string, Dictionary<string, object>>();
        public bool Confirmed = false;
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
            txt_options_dumpFileInfo_fileLocation.Enabled = btn_options_dumpFileInfo_fileLocation.Enabled = chk_options_dumpFileInfo_exportInfomation.Enabled = chk_file_dumpFileInfo.Checked;
            txt_options_saveExportInfo_fileLocation.Enabled = btn_options_saveExportInfo_fileLocation.Enabled = chk_file_saveExportInfo.Checked;
            btn_confirm.Enabled = !(!chk_file_dumpFileInfo.Checked && !chk_file_saveExportInfo.Checked);
        }

        private void FileBrowse(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TextBox activeTextbox;
            string title = null;
            string setPath;
            string fileExtension;
            string fileExtensionNoDot;

            switch (button.Name)
            {
                case "btn_options_dumpFileInfo_fileLocation":
                    activeTextbox = txt_options_dumpFileInfo_fileLocation;
                    title = "Dump file information";
                    break;
                case "btn_options_saveExportInfo_fileLocation":
                    activeTextbox = txt_options_saveExportInfo_fileLocation;
                    title = "Save export information";
                    break;
                default:
                    activeTextbox = txt_options_dumpFileInfo_fileLocation;
                    break;
            }
            setPath = activeTextbox.Text;
            fileExtension = Path.GetExtension(activeTextbox.Text);
            fileExtensionNoDot = fileExtension.Substring(1);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(setPath),
                Title = title,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = fileExtensionNoDot,
                Filter = fileExtensionNoDot.ToUpper() + " file (*." + fileExtensionNoDot + ")|*." + fileExtensionNoDot,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK) activeTextbox.Text = saveFileDialog.FileName;
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

            Confirmed = true;
            this.Close();
        }
    }
}
