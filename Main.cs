using System;
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

        public static Dictionary<string, string> OpenedFileRemake = new Dictionary<string, string>();
        public Dictionary<string, int> OpenedFileLoop = new Dictionary<string, int>();
        public static Dictionary<string, bool> FeatureConfig = new Dictionary<string, bool>();
        public static Dictionary<string, object> AdvancedSettings = new Dictionary<string, object>();
        //public Dictionary<string, string> PreviousOpenedFile = new Dictionary<string, string>();
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
            OpenedFileRemake.Clear();
            OpenedFileLoop.Clear();

            if (FeatureConfig["ResetExportOptionsOnNewFile"])
            {
                lst_exportExtensions.SelectedIndex = default;
                MainAdvanced.Reset();
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
                    UpdateStatus(); // TODO: necessary?
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseFile();
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
            btn_export.Visible = loaded;
            btn_dump.Visible = loaded;
            chk_loop.Visible = loaded;
            lbl_exportAs.Visible = loaded;
            lst_exportExtensions.Visible = loaded;
            btn_advancedOptions.Visible = false;
            lbl_dnd.Visible = !loaded;

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

                FormMethods.FileLock(null);
                if (FeatureConfig["OpenCloseWinformsButton"])
                {
                    btn_open.Text = "Open File";
                }
            }
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
                StartPosition = FormStartPosition.Manual,
                Left = this.Left,
                Top = this.Top,
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
            UpdateStatus("Verifying the file...");

            // If the file was missing or inaccessible, but suddenly is, relock it again
            FormMethods.FileLock(OpenedFile.Info["Path"]);

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
                if (FormMethods.VerifyIntegrity(OpenedFile.Info["Path"]))
                {
                    // Prepare the arguments
                    UpdateStatus("Converting the file...");
                    FormMethods.EnableCloseButton(this, false);
                    string exportLocation = String.Format("\"{0}\"", Path.GetFullPath(saveFileDialog.FileName));
                    string arguments = OpenedFile.GenerateConversionParams(exportLocation);

                    // Cancelling the operation before it starts results in null
                    if (arguments != null)
                    {
                        try
                        {
                            ProcessStartInfo procInfo = new ProcessStartInfo
                            {
                                FileName = VGAudioCli,
                                WorkingDirectory = Path.GetDirectoryName(OpenedFile.Info["Path"]),
                                Arguments = arguments,
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            };

                            FormMethods.FileLock(null); // Unlock the file
                            var proc = Process.Start(procInfo); // Process the file - BUG: WAV to DSP/IDSP with a loop hangs here - TODO: fix

                            proc.WaitForExit();
                            FormMethods.FileLock(OpenedFile.Info["Path"]); // Relock the file
                            UpdateStatus();
                            var standardConsoleOutput = proc.StandardOutput.ReadToEnd();
                            if (proc.ExitCode == 0)
                            {
                                // Progress bar [####   ] starts with '[' and success starts with, well, 'Success!'
                                if (standardConsoleOutput.StartsWith("[") || standardConsoleOutput.StartsWith("Success!"))
                                {
                                    MessageBox.Show("Task performed successfully.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    // Should happen only if the operation is not supported by the CLI
                                    MessageBox.Show(standardConsoleOutput, "Conversion Error | " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                // Error occurs when the user replaces an existing file
                                // with invalid export extension through the dialog box
                                string[] errorString = standardConsoleOutput.Split(
                                    new[] { "\r\n", "\r", "\n" },
                                    StringSplitOptions.None
                                );

                                // Returns the error message without the usage of VGAudioCli
                                MessageBox.Show(errorString[0], "Error | " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            FormMethods.FileLock(OpenedFile.Info["Path"]); // Relock the file after an unsuccessful attempt to convert it
                            UpdateStatus();
                            MessageBox.Show(ex.Message, "Fatal Error | " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            UpdateStatus();
            return;
        }

        private void FileDump(object sender, EventArgs e)
        {
            MainDump mainDump = new MainDump(OpenedFile.Info["Path"], lst_exportExtensions.SelectedItem.ToString().ToLower())
            {
                StartPosition = FormStartPosition.Manual,
                Left = this.Left,
                Top = this.Top,
                Text = String.Format("Dump Info | {0}", Text)
            };
            mainDump.ShowDialog();

            if (!mainDump.Options.ContainsKey("Confirmed") || !(bool)mainDump.Options["Confirmed"]) return;
            Dictionary<string, object> Options = mainDump.Options;

            // For temporary compatibility - TODO: replace
            bool dumpExportInfo = (bool)((Dictionary<string, object>)Options["DumpFileInfo"])["IncludeExportInformation"];

            UpdateStatus("Dumping info...");
            FormMethods.EnableCloseButton(this, false);

            string[] lines = OpenedFile.DumpInformation(dumpExportInfo);
            var path = OpenedFile.Info["Path"] + ".dump";

            File.WriteAllLines(path, lines);
            UpdateStatus();
            MessageBox.Show("Info dumped to " + path + "!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            slb_status.Text = "Opened the file: " + OpenedFile.Info["NameShort"];
                        }
                        else
                        {
                            // Makes sure the file name isn't too long
                            var invalidFileNameShort = FormMethods.TruncateFileName(OpenedFile.Info["Name"], OpenedFile.Info["ExtensionNoDot"], 20);
                            slb_status.Text = "Please check the path of \"" + invalidFileNameShort + "\"";
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
            MaximumSize = Size;
            MaximizeBox = false;

            // Init OpenedFile
            OpenedFileRemake.Clear();
            OpenedFileLoop.Clear();

            // Init the advanced settings dictionary
            MainAdvanced.Reset();

            // Create the supported extensions to a filter
            extsFilter = FormMethods.CreateExtensionFilter(extsArray);

            // Listen to pressed keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormMethods.ActionListener);

            // Feature Config
            FeatureConfig.Clear();

            // A button that requires an extra click to close the file first before opening a new one
            FeatureConfig.Add("OpenCloseWinformsButton", true);

            // Close button closes the file on the first click instead of closing the app
            FeatureConfig.Add("CloseButtonClosesFile", true);

            // Prefill export file name and extension
            FeatureConfig.Add("PrefillExportFileName", true);

            // Lock the opened file so that it can't be moved or deleted
            FeatureConfig.Add("LockOpenedFile", true);

            // Reset the loop information and the advanced options after the file is closed
            FeatureConfig.Add("ResetExportOptionsOnNewFile", true);
        }

        private void TestFeature()
        {
            
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FeatureConfig["CloseButtonClosesFile"])
            {
                if (OpenedFile.Initialized)
                {
                    e.Cancel = true;
                    CloseFile();
                }
            }
            // Unlock the file before exiting
            FormMethods.FileLock(null);
        }
    }
}
