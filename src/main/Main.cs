﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public partial class Main : Form
    {
        public static string VGAudioCli = Path.GetFullPath("VGAudioCli.exe");
        private OpenedFile OpenedFile = new OpenedFile();
        public static Dictionary<string, bool> FeatureConfig = Config.FeatureConfig.GetValues();
        public static Theme AppTheme;
        public static readonly string[] extsArray = { "wav", "dsp", "idsp", "brstm", "bcstm", "bfstm", "hps", "adx", "hca", "genh", "at9" };
        public static string extsFilter;

        public Main()
        {
            InitializeComponent();
            OnStart();
            UpdateStatus();
            TestFeature();
        }

        private void OpenFileForm(object sender, EventArgs e)
        {
            if (FeatureConfig["OpenCloseWinformsButton"])
            {
                if (!OpenedFile.Initialized)
                {
                    OpenFile();
                }
                else
                {
                    CloseFile();
                }
            }
            else
            {
                OpenFile();
            }
        }

        private void OpenFile()
        {
            if (FileDialog())
            {
                FileLoaded();
            }
            else
            {
                if (!OpenedFile.Initialized)
                {
                    FileLoaded(false);
                }
            }
        }

        private void DragDropFile(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (fileList == null)
            {
                MessageBox.Show("The selected file is not supported!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (!OpenedFile.Initialized)
                {
                    CloseFile();
                }
            }
            else
            {
                if (fileList.Length == 1)
                {
                    var file = fileList[0];
                    if (FileDialogProcess(file))
                    {
                        FileLoaded();
                    }
                }
                else
                {
                    // TODO: add support for batch conversions
                    MessageBox.Show("Only one file is supported at a time.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DragDropEffects(object sender, DragEventArgs e)
        {
            e.Effect = System.Windows.Forms.DragDropEffects.Move;
        }

        private void CloseFile()
        {
            FileLoaded(false);
            UpdateStatus("Close");
            OpenedFile.Close(FeatureConfig["ResetExportOptionsOnNewFile"]);

            if (FeatureConfig["ResetExportOptionsOnNewFile"])
            {
                lst_exportExtensions.SelectedIndex = default;
            }
        }

        private bool FileDialog()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Open file",
                DefaultExt = "",
                Filter = extsFilter,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                return FileDialogProcess(openFile.FileName);
            }
            return false;
        }

        private bool FileDialogProcess(string filePath)
        {
            bool noPreviousFile; // Saves the value of whether there was previously a file opened or not
            UpdateStatus("Reading the file...");

            if (!OpenedFile.Initialized)
            {
                // Select the second export extension (DSP)
                lst_exportExtensions.SelectedIndex = 1;
                noPreviousFile = true;
            }
            else
            {
                noPreviousFile = false;
            }

            try
            {
                // If the app doesn't open the new file but still has one in memory, keep it
                if (!OpenedFile.Open(filePath) && OpenedFile.Initialized)
                {
                    UpdateStatus();
                    if (FeatureConfig.ContainsKey("LockOpenedFile") && FeatureConfig["LockOpenedFile"])
                    {
                        MessageBox.Show("The selected file is already loaded.", FormMethods.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                UpdateStatus();
                MessageBox.Show(e.Message, "Error | " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (e is EndOfStreamException) CloseFile();
                return false;
            }

            if (!OpenedFile.Initialized)
            {
                CloseFile();
                return false;
            }

            txt_metadata.Text = OpenedFile.Metadata["Short"];
            if (OpenedFile.Loop["Enabled"] == 1)
            {
                chk_loop.Text = "Keep the loop";
                if (FeatureConfig["ResetExportOptionsOnNewFile"]) chk_loop.Checked = true;
            }
            else
            {
                chk_loop.Text = "Create a loop";
                if (FeatureConfig["ResetExportOptionsOnNewFile"]) chk_loop.Checked = false;
            }

            num_loopStart.Maximum = (decimal)OpenedFile.Loop["StartMax"];
            num_loopStart.Minimum = (decimal)OpenedFile.Loop["StartMin"];

            num_loopEnd.Maximum = (decimal)OpenedFile.Loop["EndMax"];
            num_loopEnd.Minimum = (decimal)OpenedFile.Loop["EndMin"];

            // Load the loop values only if it's the first file loaded
            // Overwrite the loop values only if specified in the config
            if (FeatureConfig["ResetExportOptionsOnNewFile"] || noPreviousFile)
            {
                num_loopStart.Value = (decimal)OpenedFile.Loop["Start"];
                num_loopEnd.Value = (decimal)OpenedFile.Loop["End"];
            }
            UpdateStatus();
            return true;
        }

        private void FileLoaded(bool loaded = true)
        {
            if (loaded)
            {
                LoopTheFile();
                ExportExtensionUpdater();
                if (FeatureConfig["OpenCloseWinformsButton"])
                {
                    btn_open.Text = "Close File";
                }
            }
            else
            {
                lbl_loopStart.Visible = false;
                lbl_loopEnd.Visible = false;

                num_loopStart.Visible = false;
                num_loopEnd.Visible = false;

                txt_metadata.Visible = false;
                btn_advancedOptions.Visible = false;

                OpenedFile.Lock(false);
                if (FeatureConfig["OpenCloseWinformsButton"])
                {
                    btn_open.Text = "Open File";
                }
            }

            btn_export.Visible = loaded;
            btn_dump.Visible = loaded;
            chk_loop.Visible = loaded;
            lbl_exportAs.Visible = loaded;
            lst_exportExtensions.Visible = loaded;
            lbl_dnd.Visible = !loaded;
        }

        private void ExportExtensionUpdater(object sender = null, EventArgs e = null)
        {
            OpenedFile.ExportInfo["ExtensionNoDot"] = lst_exportExtensions.SelectedItem.ToString().ToLower();
            OpenedFile.ExportInfo["Extension"] = String.Format(".{0}", OpenedFile.ExportInfo["ExtensionNoDot"]);
            switch (OpenedFile.ExportInfo["ExtensionNoDot"])
            {
                case "adx":
                case "brstm":
                case "hca":
                    btn_advancedOptions.Visible = true;
                    break;
                default:
                    btn_advancedOptions.Visible = false;
                    break;
            }
        }

        private void OpenAdvancedOptions(object sender, EventArgs e)
        {
            MainAdvanced mainAdvanced = new MainAdvanced(OpenedFile)
            {
                StartPosition = FormStartPosition.CenterParent,
                Text = String.Format("Advanced options | {0}", Text)
            };
            mainAdvanced.ShowDialog();
        }

        private void LoopTheFile(object sender = null, EventArgs e = null)
        {
            if (chk_loop.Checked)
            {
                lbl_loopStart.Visible = true;
                lbl_loopEnd.Visible = true;

                num_loopStart.Visible = true;
                num_loopStart.Visible = true;
                num_loopEnd.Visible = true;

                txt_metadata.Visible = false;
            }
            else
            {
                lbl_loopStart.Visible = false;
                lbl_loopEnd.Visible = false;

                num_loopStart.Visible = false;
                num_loopEnd.Visible = false;

                txt_metadata.Visible = true;
            }

            OpenedFile.ExportLoop["Enabled"] = Convert.ToInt32(chk_loop.Checked);
        }

        private void FileExport(object sender, EventArgs e)
        {
            UpdateStatus("Converting the file...");

            // If the file was missing or inaccessible, but suddenly is, relock it again
            OpenedFile.Lock(true);

            if (OpenedFile.ExportInfo["ExtensionNoDot"] == null)
            {
                UpdateStatus();
                MessageBox.Show("Please select the exported file's extension!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (OpenedFile.Info["ExtensionNoDot"] == OpenedFile.ExportInfo["ExtensionNoDot"])
            {
                DialogResult dialogResult = MessageBox.Show("The file you're trying to export has the same extension as the original file. Some of the changes might not be applied.\r\nContinue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dialogResult != DialogResult.Yes)
                {
                    UpdateStatus();
                    return;
                }
            }

            if (OpenedFile.ExportLoop["Enabled"] == 1)
            {
                if (OpenedFile.ExportInfo["ExtensionNoDot"] == "wav")
                {
                    // Encoding the loop information into the wave file
                    // is useful only for conversions done later on (to a format that supports it)
                    DialogResult dialogResult = MessageBox.Show("While the wave file can hold loop information, it won't be read by most media players.\r\nExport the loop information anyway?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            break;
                        case DialogResult.No:
                            chk_loop.Checked = false;
                            break;
                        case DialogResult.Cancel:
                        default:
                            UpdateStatus();
                            return;
                    }
                }
            }
            else
            {
                if (OpenedFile.ExportLoop["Enabled"] == 1)
                {
                    DialogResult dialogResult = MessageBox.Show("The imported file has loop information, which will be lost upon export.\r\nContinue the export without the loop information?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (dialogResult != DialogResult.Yes)
                    {
                        UpdateStatus();
                        return;
                    }
                }
            }

            // Select the save location
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(OpenedFile.Info["Path"]),
                Title = "Export " + OpenedFile.Info["NameNoExtension"] + "." + OpenedFile.ExportInfo["ExtensionNoDot"],
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = OpenedFile.ExportInfo["ExtensionNoDot"],
                Filter = OpenedFile.ExportInfo["ExtensionNoDot"].ToUpper() + " audio file (*." + OpenedFile.ExportInfo["ExtensionNoDot"] + ")|*." + OpenedFile.ExportInfo["ExtensionNoDot"],
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (FeatureConfig["PrefillExportFileName"])
            {
                saveFileDialog.FileName = OpenedFile.Info["NameNoExtension"] + "." + OpenedFile.ExportInfo["ExtensionNoDot"];
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenedFile.ExportInfo["Path"] = saveFileDialog.FileName;
                OpenedFile.ExportInfo["PathEscaped"] = String.Format("\"{0}\"", Path.GetFullPath(OpenedFile.ExportInfo["Path"]));
                FormMethods.EnableCloseButton(this, false);
                try
                {
                    if (OpenedFile.Convert())
                    {
                        UpdateStatus();
                        string successMessage = "Task performed successfully.";
                        if (FeatureConfig["ShowTimeElapsed"]) successMessage += String.Format(" Time elapsed: {0}", OpenedFile.ExportResult["TimeElapsed"]);
                        MessageBox.Show(successMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    OpenedFile.Lock(true);
                    UpdateStatus();

                    string exceptionTitle = "Fatal Error";
                    if (ex is NotSupportedException) exceptionTitle = "Conversion Error";
                    if (ex is ArgumentException) exceptionTitle = "Error";
                    MessageBox.Show(ex.Message, String.Format("{0} | {1}", exceptionTitle, Text), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    FormMethods.EnableCloseButton(this);
                }
            }
            UpdateStatus();
            return;
        }

        private void FileDump(object sender, EventArgs e)
        {
            MainDump mainDump = new MainDump(OpenedFile.Info["Path"], lst_exportExtensions.SelectedItem.ToString().ToLower())
            {
                StartPosition = FormStartPosition.CenterParent,
                Text = String.Format("Dump Info | {0}", Text)
            };
            mainDump.Size = (this.Width / 2 > 832 && this.Height / 2 > 538) ? new Size(this.Width / 2, 538) : new Size(this.Width, 538);
            mainDump.ShowDialog();

            if (mainDump.Confirmed)
            {
                UpdateStatus("Dumping info...");
                FormMethods.EnableCloseButton(this, false);
                Dictionary<string, Dictionary<string, object>> Options = mainDump.Options;

                try
                {
                    if ((bool)Options["DumpFileInfo"]["Use"])
                    {
                        string path = (string)Options["DumpFileInfo"]["FileLocation"];
                        string[] lines = OpenedFile.DumpInformation(Options);
                        File.WriteAllLines(path, lines);
                    }

                    if ((bool)Options["SaveExportInfo"]["Use"])
                    {
                        string path = (string)Options["SaveExportInfo"]["FileLocation"];
                        string line = OpenedFile.GenerateConversionParams(null, false, true);
                        File.WriteAllText(path, String.Format("{0}\r\n", line));
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus();
                    MessageBox.Show(ex.Message, "Error dumping file information | " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UpdateStatus();
                MessageBox.Show("Info dumped successfully!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public async void UpdateStatus(string message = "Ready")
        {
            FormMethods.EnableCloseButton(this);
            switch (message)
            {
                case "Ready":
                    if (OpenedFile.Initialized)
                    {
                        if (File.Exists(OpenedFile.Info["Path"]))
                        {
                            slb_status.Text = OpenedFile.Metadata["Status"];
                        }
                        else
                        {
                            slb_status.Text = "Please check the path of " + OpenedFile.Info["NameShort_OnError"] + "!";
                        }
                        return;
                    }
                    slb_status.Text = message;
                    break;
                case "Close":
                    // The key won't be set if user tries to drag and drop
                    // an invalid file while no other file is opened
                    if (OpenedFile.Info.ContainsKey("NameShort"))
                    {
                        slb_status.Text = "Closed the file: " + OpenedFile.Info["NameShort"];
                        await Task.Delay(2000);
                    }

                    // Another file might've been opened in the meantime when the previous file was closed
                    // Was another file opened during the Task.Delay?
                    if (OpenedFile.Initialized)
                    {
                        slb_status.Text = "Opened the file: " + OpenedFile.Info["NameShort"];
                        return;
                    }
                    else
                    {
                        slb_status.Text = "Ready";
                    }
                    break;
                default:
                    slb_status.Text = message;
                    break;
            }
        }

        private void NumLoopOnUpdate(object sender, EventArgs e)
        {
            OpenedFile.ExportLoop["StartMin"] = 0;
            OpenedFile.ExportLoop["StartMax"] = (int)num_loopEnd.Value - 1;
            OpenedFile.ExportLoop["EndMin"] = (int)num_loopStart.Value + 1;

            num_loopStart.Minimum = (decimal)OpenedFile.ExportLoop["StartMin"];
            num_loopStart.Maximum = (decimal)OpenedFile.ExportLoop["StartMax"];
            num_loopEnd.Minimum = (decimal)OpenedFile.ExportLoop["EndMin"];

            OpenedFile.ExportLoop["Start"] = (int)num_loopStart.Value;
            OpenedFile.ExportLoop["End"] = (int)num_loopEnd.Value;
        }
        private void OnStart()
        {
            // Form size
            MinimumSize = Size;
            if (!FeatureConfig["AllowWindowResize"])
            {
                MaximumSize = Size;
                MaximizeBox = false;
            }

            // Create the supported extensions to a filter
            extsFilter = FormMethods.CreateExtensionFilter(extsArray);

            // Listen to pressed keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormMethods.ActionListener);

            // Set the app's theme
            AppTheme = new Theme();
            AppTheme.Apply(this);

            // Properly render the status strip in dark mode
            statusStrip.Renderer = new FormMethods.ToolStripLightRenderer();
            if (FormMethods.IsAppInDarkMode()) statusStrip.Renderer = new FormMethods.ToolStripDarkRenderer();
        }

        private void TestFeature()
        {

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FeatureConfig["CloseButtonClosesFile"] && OpenedFile.Initialized) e.Cancel = true;
            CloseFile();
        }
    }
}
