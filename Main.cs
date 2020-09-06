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
        
        public Dictionary<string, string> OpenedFileRemake = new Dictionary<string, string>();
        public Dictionary<string, int> OpenedFileLoop = new Dictionary<string, int>();
        public Dictionary<string, bool> FeatureConfig = new Dictionary<string, bool>();
        //public Dictionary<string, string> PreviousOpenedFile = new Dictionary<string, string>();
        public readonly string[] extsArray = { "wav", "dsp", "idsp", "brstm", "bcstm", "bfstm", "hps", "adx", "hca", "genh", "at9" };
        public readonly string extsFilter = "All Supported Audio Streams|*.wav;*.dsp;*.idsp;*.brstm;*.bcstm;*.bfstm;*.hps;*.adx;*.hca;*.genh;*.at9|"
                                            + "WAV|*.wav|DSP|*.dsp|IDSP|*.idsp|BRSTM|*.brstm|BCSTM|*.bcstm|BFSTM|*.bfstm|HPS|*.hps|ADX|*.adx|HCA|*.hca|GENH|*.genh|AT9|*.at9";

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
                if (!OpenedFileRemake.ContainsKey("FileName"))
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
                if (!OpenedFileRemake.ContainsKey("FileName"))
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
                if (!OpenedFileRemake.ContainsKey("FileName"))
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
            OpenedFileRemake.Clear();
            OpenedFileLoop.Clear();
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
            // Select the second export extension (DSP)
            if (!OpenedFileRemake.ContainsKey("FilePath"))
            {
                lst_exportExtensions.SelectedIndex = 1;
            }

            // Check if the selected path is not a directory
            // https://stackoverflow.com/a/1395226
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                // TODO: add support for directories to enable batch conversions
                MessageBox.Show("Batch conversions are currently not supported!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Get the file extension without the '.' early for the file extension verification
            var FileExtensionWithDot = Path.GetExtension(filePath);
            var FileExtensionWithoutDot = FileExtensionWithDot.Substring(1);

            // Check if the file extension is valid
            if (!extsArray.Contains(FileExtensionWithoutDot.ToLower()))
            {
                MessageBox.Show("The selected file is not supported!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (FormMethods.IsFileLocked(filePath))
            {
                if (OpenedFileRemake.ContainsKey("FilePath"))
                {
                    // Check if the locked file is in use by the app
                    if (filePath == OpenedFileRemake["FilePath"])
                    {
                        if (FeatureConfig.ContainsKey("LockOpenedFile") && FeatureConfig["LockOpenedFile"])
                        {
                            MessageBox.Show("The selected file is already loaded.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("The selected file is already in use by another process.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The selected file is already in use by another process.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            // Clear the dictionaries if not empty
            if (OpenedFileRemake.Count != 0)
            {
                // Remember the previous file path and metadata for checking if the locked file is in use by the app - replaced
                // TODO (IDEA): remember everything - PreviousOpenedFile = OpenedFileRemake (could load previously closed file)
                // TODO (IDEA): if loading previously closed file, save the current one (OpenedFileRemake) to NextOpenedFile
                // if (OpenedFileRemake.ContainsKey("FilePath")) PreviousOpenedFile["FilePath"] = OpenedFileRemake["FilePath"];
                // if (OpenedFileRemake.ContainsKey("Metadata")) PreviousOpenedFile["Metadata"] = OpenedFileRemake["Metadata"];
                OpenedFileRemake.Clear();
            }
            if (OpenedFileLoop.Count != 0) OpenedFileLoop.Clear();

            // Load file information
            OpenedFileRemake.Add("FileName", Path.GetFileName(filePath));
            OpenedFileRemake.Add("FilePath", filePath);
            OpenedFileRemake.Add("FilePathEscaped", "\"" + OpenedFileRemake["FilePath"] + "\"");
            OpenedFileRemake.Add("FileNoExtension", Path.GetFileNameWithoutExtension(OpenedFileRemake["FilePath"]));

            // Load file extension without the '.' at the beginning
            OpenedFileRemake.Add("FileExtension", FileExtensionWithoutDot);

            // Shorten the file name if it's too long and add file extension that wouldn't be otherwise seen
            OpenedFileRemake.Add("FileNameShort", FormMethods.TruncateFileName(OpenedFileRemake["FileName"], OpenedFileRemake["FileExtension"]));

            if (FormMethods.VerifyIntegrity(OpenedFileRemake["FilePath"]))
            {
                ProcessStartInfo procInfo = new ProcessStartInfo
                {
                    FileName = VGAudioCli,
                    WorkingDirectory = Path.GetDirectoryName(OpenedFileRemake["FilePath"]),
                    Arguments = "-m " + OpenedFileRemake["FilePathEscaped"],
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var proc = Process.Start(procInfo);
                proc.WaitForExit();
                if (proc.ExitCode == 0)
                {
                    OpenedFileRemake.Add("Metadata", proc.StandardOutput.ReadToEnd());

                    // Vars that are later converted to int
                    var mLoopStartVar = FormMethods.GetBetween(OpenedFileRemake["Metadata"], "Loop start: ", " samples");
                    var mLoopEndVar = FormMethods.GetBetween(OpenedFileRemake["Metadata"], "Loop end: ", " samples");

                    OpenedFileRemake.Add("EncodingFormat", FormMethods.GetBetween(OpenedFileRemake["Metadata"], "Encoding format: ", "\r\n"));
                    OpenedFileRemake.Add("SampleRate", FormMethods.GetBetween(OpenedFileRemake["Metadata"], "Sample rate: ", "\r\n"));
                    OpenedFileRemake.Add("ChannelCount", FormMethods.GetBetween(OpenedFileRemake["Metadata"], "Channel count: ", "\r\n"));

                    txt_metadata.Text = OpenedFileRemake["EncodingFormat"] + "\r\nSample Rate: " + OpenedFileRemake["SampleRate"] + "\r\nChannel Count: " + OpenedFileRemake["ChannelCount"];

                    if (int.TryParse(mLoopStartVar, out int mLoopStart) && int.TryParse(mLoopEndVar, out int mLoopEnd))
                    {
                        OpenedFileLoop.Add("StartLoaded", mLoopStart); // Save the loop start 
                        OpenedFileLoop.Add("EndLoaded", mLoopEnd); // Save the loop end

                        OpenedFileLoop.Add("Start", OpenedFileLoop["StartLoaded"]); // Sets the loop start at the current loop start
                        OpenedFileLoop.Add("End", OpenedFileLoop["EndLoaded"]); // Sets the loop end at the current loop end

                        OpenedFileLoop.Add("StartMax", OpenedFileLoop["End"] - 1); // Makes sure the user can only input lower number than the loop's end
                        OpenedFileLoop.Add("StartMin", 0); // The loop start value cannot be lower than the beginning of the file

                        OpenedFileLoop.Add("EndMax", mLoopEnd); // Makes sure the user doesn't input more samples than the file has
                        OpenedFileLoop.Add("EndMin", OpenedFileLoop["Start"] + 1); // Loop end has to be a bigger number than loop start

                        chk_loop.Text = "Keep the loop";
                        chk_loop.Checked = true;
                    }
                    else
                    {
                        OpenedFileLoop.Add("Start", 0);
                        OpenedFileLoop.Add("StartMin", 0);

                        var mSampleCountVar = FormMethods.GetBetween(OpenedFileRemake["Metadata"], "Sample count: ", " (");
                        if (int.TryParse(mSampleCountVar, out int mSampleCount))
                        {
                            // If there's no loop, the new loop end is end of the file by default
                            OpenedFileLoop.Add("EndMax", mSampleCount);
                            OpenedFileLoop.Add("EndMin", OpenedFileLoop["Start"] + 1);
                            OpenedFileLoop.Add("End", OpenedFileLoop["EndMax"]);
                            OpenedFileLoop.Add("StartMax", OpenedFileLoop["EndMax"] - 1);

                            chk_loop.Text = "Create a loop";
                            chk_loop.Checked = false;
                        }
                        else
                        {
                            // Should never occur - hopefully
                            MessageBox.Show("File contains invalid header data!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            CloseFile();
                            return false;
                        }
                    }
                    num_loopStart.Maximum = OpenedFileLoop["StartMax"];
                    num_loopStart.Minimum = OpenedFileLoop["StartMin"];
                    num_loopStart.Value = OpenedFileLoop["Start"];

                    num_loopEnd.Maximum = OpenedFileLoop["EndMax"];
                    num_loopEnd.Minimum = OpenedFileLoop["EndMin"];
                    num_loopEnd.Value = OpenedFileLoop["End"];

                    // Check again if the file is accessible
                    // It could've been locked or deleted by something else in the meantime
                    if (!FormMethods.IsFileLocked(OpenedFileRemake["FilePath"]))
                    {
                        FormMethods.FileLock(OpenedFileRemake["FilePath"]);
                        UpdateStatus();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("The selected file is inaccessible!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Should never happen - hopefully
                    MessageBox.Show("Unable to read the file!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            CloseFile();
            UpdateStatus();
            return false;
        }

        private void FileLoaded(bool loaded = true)
        {
            btn_export.Visible = loaded;
            btn_dump.Visible = loaded;
            chk_loop.Visible = loaded;
            lbl_exportAs.Visible = loaded;
            lst_exportExtensions.Visible = loaded;
            btn_advancedOptions.Visible = false;

            if (loaded)
            {
                LoopTheFile("", new EventArgs());
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

        private void ExportExtensionUpdater(object sender, EventArgs e)
        {
            switch (lst_exportExtensions.SelectedItem.ToString().ToLower())
            {
                case "brstm":
                case "bcstm":
                    btn_advancedOptions.Visible = true;
                    break;
                default:
                    btn_advancedOptions.Visible = false;
                    break;
            }
        }

        private void OpenAdvancedOptions(object sender, EventArgs e)
        {
            MainAdvanced mainAdvanced = new MainAdvanced(lst_exportExtensions.SelectedItem.ToString())
            {
                StartPosition = FormStartPosition.Manual,
                Left = this.Left,
                Top = this.Top,
                Text = String.Format("Advanced options | {0}", Text)
            };
            mainAdvanced.ShowDialog();
        }

        private void LoopTheFile(object sender, EventArgs e)
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
        }

        private void FileExport(object sender, EventArgs e)
        {
            // TODO: advanced settings (BRSTM, BCSTM) + audio format (BRSTM)
            UpdateStatus("Verifying the file...");

            // If the file was missing or inaccessible, but suddenly is, relock it again
            FormMethods.FileLock(OpenedFileRemake["FilePath"]);


            if (lst_exportExtensions.SelectedItem == null)
            {
                UpdateStatus();
                MessageBox.Show("Please select the exported file's extension!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var exportExtension = lst_exportExtensions.SelectedItem.ToString().ToLower();

            if (OpenedFileRemake["FileExtension"] == exportExtension)
            {
                DialogResult dialogResult = MessageBox.Show("The file you're trying to export has the same extension as the original file.\r\nContinue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dialogResult != DialogResult.Yes)
                {
                    UpdateStatus();
                    return;
                }
            }

            if (chk_loop.Checked)
            {
                if (exportExtension == "wav")
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
                if (OpenedFileLoop.ContainsKey("StartLoaded") && OpenedFileLoop.ContainsKey("EndLoaded"))
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
                InitialDirectory = Path.GetDirectoryName(OpenedFileRemake["FilePath"]),
                Title = "Export " + OpenedFileRemake["FileNoExtension"] + "." + exportExtension,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = exportExtension,
                Filter = exportExtension.ToUpper() + " audio file (*." + exportExtension + ")|*." + exportExtension,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (FeatureConfig["PrefillExportFileName"])
            {
                saveFileDialog.FileName = OpenedFileRemake["FileNoExtension"] + "." + exportExtension;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (FormMethods.VerifyIntegrity(OpenedFileRemake["FilePath"]))
                {
                    // Do stuff
                    UpdateStatus("Converting the file...");
                    FormMethods.EnableCloseButton(this, false);
                    try
                    {
                        ProcessStartInfo procInfo = new ProcessStartInfo
                        {
                            FileName = VGAudioCli,
                            WorkingDirectory = Path.GetDirectoryName(OpenedFileRemake["FilePath"]),
                            Arguments = "-i " + OpenedFileRemake["FilePathEscaped"] + " -o " + "\"" + Path.GetFullPath(saveFileDialog.FileName) + "\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };

                        if (chk_loop.Checked)
                        {
                            var loopStart = num_loopStart.Value;
                            var loopEnd = num_loopEnd.Value;

                            if (loopStart > loopEnd)
                            {
                                DialogResult dialogResult = MessageBox.Show("Loop information cannot be saved: The Loop Start value cannot exceed the Loop End value.\r\nContinue the export without the loop information?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                if (dialogResult != DialogResult.Yes)
                                {
                                    UpdateStatus();
                                    return;
                                }
                                chk_loop.Checked = false;
                            }
                            procInfo.Arguments = procInfo.Arguments + " -l " + loopStart + "-" + loopEnd;
                        }
                        else
                        {
                            procInfo.Arguments += " --no-loop";
                        }

                        switch (exportExtension)
                        {
                            case "brstm":
                                switch (MainAdvanced.brstm_audioFormat)
                                {
                                    case "DSP-ADPCM":
                                        // If not specified, the file is converted to DSP-ADPCM audio format
                                        break;
                                    case "16-bit PCM":
                                        procInfo.Arguments += " -f pcm16";
                                        break;
                                    case "8-bit PCM":
                                        procInfo.Arguments += " -f pcm8";
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "bcstm":
                                break;
                            default:
                                break;
                        }

                        FormMethods.FileLock(null); // Unlock the file
                        var proc = Process.Start(procInfo); // Process the file
                        proc.WaitForExit();
                        FormMethods.FileLock(OpenedFileRemake["FilePath"]); // Relock the file
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
                        FormMethods.FileLock(OpenedFileRemake["FilePath"]); // Relock the file after an unsuccessful attempt to convert it
                        UpdateStatus();
                        MessageBox.Show(ex.Message, "Fatal Error | " + Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            UpdateStatus();
            return;
        }

        private void FileDump(object sender, EventArgs e)
        {
            // Dump export information?
            DialogResult result = MessageBox.Show("Include export information set in the app?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            bool dumpExportInfo;
            switch (result)
            {
                case DialogResult.Yes:
                    dumpExportInfo = true;
                    break;
                case DialogResult.No:
                    dumpExportInfo = false;
                    break;
                default:
                    return;
            }

            UpdateStatus("Dumping info...");
            FormMethods.EnableCloseButton(this, false);
            var path = OpenedFileRemake["FilePath"] + ".dump";

            var exportExtension = lst_exportExtensions.SelectedItem.ToString().ToLower();
            var convertCommand = "VGAudioCli.exe -i " + OpenedFileRemake["FilePathEscaped"] + " -o " + "output." + exportExtension;

            List<string> lineList = new List<string>
            {
                FormMethods.GetAppInfo("name") + " (" + FormMethods.GetAppInfo("version") + ")",
                "Dumped Info (" + OpenedFileRemake["FileName"] + ")\r\n",
                OpenedFileRemake["Metadata"]
            };

            if (dumpExportInfo)
            {
                lineList.Add("----------\r\n");
                lineList.Add("Custom Export Info:");
                lineList.Add("Target file: " + exportExtension);
                if (chk_loop.Checked)
                {
                    if (exportExtension != "wav")
                    {
                        lineList.Add("Loop start: " + num_loopStart.Value);
                        lineList.Add("Loop end: " + num_loopEnd.Value);
                        convertCommand += " -l " + num_loopStart.Value + "-" + num_loopEnd.Value;
                    }
                }
                lineList.Add("\r\nConversion command (defined from the UI):\r\n" + convertCommand);
            }

            string[] lines = lineList.ToArray();
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
                    if (OpenedFileRemake.ContainsKey("FileName"))
                    {
                        if (File.Exists(OpenedFileRemake["FilePath"]))
                        {
                            slb_status.Text = "Opened the file: " + OpenedFileRemake["FileNameShort"];
                        }
                        else
                        {
                            // Makes sure the file name isn't too long
                            var invalidFileNameShort = FormMethods.TruncateFileName(OpenedFileRemake["FileName"], OpenedFileRemake["FileExtension"], 20);
                            slb_status.Text = "Please check the path of \"" + invalidFileNameShort + "\"";
                        }
                        return;
                    }
                    slb_status.Text = message;
                    break;
                case "Close":
                    // The key won't be set if user tries to drag and drop
                    // an invalid file while no other file is opened
                    if (OpenedFileRemake.ContainsKey("FileNameShort"))
                    {
                        slb_status.Text = "Closed the file: " + OpenedFileRemake["FileNameShort"];
                        await Task.Delay(2000);
                    }

                    // Another file might've been opened in the meantime when the previous file was closed
                    // Was another file opened during the Task.Delay?
                    if (OpenedFileRemake.ContainsKey("FileExtension"))
                    {
                        slb_status.Text = "Opened the file: " + OpenedFileRemake["FileNameShort"];
                        return;
                    }
                    slb_status.Text = "Ready";
                    break;
                default:
                    slb_status.Text = message;
                    break;
            }
        }

        private void NumLoopOnUpdate(object sender, EventArgs e)
        {
            OpenedFileLoop["StartMin"] = 0;
            OpenedFileLoop["StartMax"] = OpenedFileLoop["End"] - 1;
            OpenedFileLoop["EndMin"] = OpenedFileLoop["Start"] + 1;

            num_loopStart.Minimum = OpenedFileLoop["StartMin"];
            num_loopStart.Maximum = OpenedFileLoop["StartMax"];
            num_loopEnd.Minimum = OpenedFileLoop["EndMin"];
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

            // Listen to pressed keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormMethods.ActionListener);

            // Feature Config
            // A button that requires an extra click to close the file first before opening a new one
            FeatureConfig.Add("OpenCloseWinformsButton", true);

            // Close button closes the file on the first click instead of closing the app
            FeatureConfig.Add("CloseButtonClosesFile", true);

            // Prefill export file name and extension
            FeatureConfig.Add("PrefillExportFileName", true);

            // Lock the opened file so that it can't be moved or deleted
            FeatureConfig.Add("LockOpenedFile", true);
        }

        private void TestFeature()
        {

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FeatureConfig["CloseButtonClosesFile"])
            {
                if (OpenedFileRemake.ContainsKey("FilePath"))
                {
                    e.Cancel = true;
                    CloseFile();
                }
            }
            else
            {
                FormMethods.FileLock(null);
            }
        }
    }
}
