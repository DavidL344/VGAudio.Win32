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
        public string VGAudioCli = Path.GetFullPath("VGAudioCli.exe");
        
        public Dictionary<string, string> OpenedFileRemake = new Dictionary<string, string>();
        public Dictionary<string, int> OpenedFileLoop = new Dictionary<string, int>();
        public readonly string[] extsArray = { "wav", "dsp", "idsp", "brstm", "bcstm", "bfstm", "hps", "adx", "hca", "genh", "at9" };
        public readonly string extsFilter = "All Supported Audio Streams|*.wav;*.dsp;*.idsp;*.brstm;*.bcstm;*.bfstm;*.hps;*.adx;*.hca;*.genh;*.at9|"
                                            + "WAV|*.wav|DSP|*.dsp|IDSP|*.idsp|BRSTM|*.brstm|BCSTM|*.bcstm|BFSTM|*.bfstm|HPS|*.hps|ADX|*.adx|HCA|*.hca|GENH|*.genh|AT9|*.at9";

        // A button that requires an extra click to close the file first before opening a new one
        public bool OpenCloseWinformsButton = true;

        // Close button closes the file on the first click instead of closing the app
        public bool CloseButtonClosesFile = true;

        public Main()
        {
            InitializeComponent();
            OnStart();
            UpdateStatus();
            TestFeature();
        }

        private void OpenFileForm(object sender, EventArgs e)
        {
            if (OpenCloseWinformsButton)
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

            // Clear dictionary if not empty
            if (OpenedFileRemake.Count != 0) OpenedFileRemake.Clear();
            if (OpenedFileLoop.Count != 0) OpenedFileLoop.Clear();

            // Load file information
            OpenedFileRemake.Add("FileName", Path.GetFileName(filePath));
            OpenedFileRemake.Add("FilePath", filePath);
            OpenedFileRemake.Add("FilePathEscaped", "\"" + OpenedFileRemake["FilePath"] + "\"");
            OpenedFileRemake.Add("FileNoExtension", Path.GetFileNameWithoutExtension(OpenedFileRemake["FilePath"]));

            // Load file extension without the '.' at the beginning
            var FileExtensionWithDot = Path.GetExtension(OpenedFileRemake["FilePath"]);
            OpenedFileRemake.Add("FileExtension", FileExtensionWithDot.Substring(1));

            // Shorten the file name if it's too long and add file extension that wouldn't be otherwise seen
            OpenedFileRemake.Add("FileNameShort", FormMethods.Truncate(OpenedFileRemake["FileName"]));
            if (OpenedFileRemake["FileName"] != OpenedFileRemake["FileNameShort"])
            {
                OpenedFileRemake["FileNameShort"] += "... (." + OpenedFileRemake["FileExtension"] + ")";
            }

            if (!extsArray.Contains(OpenedFileRemake["FileExtension"]))
            {
                MessageBox.Show("The selected file is not supported!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (FormMethods.MassPathCheck(VGAudioCli, OpenedFileRemake["FilePath"]))
            {
                ProcessStartInfo procInfo = new ProcessStartInfo
                {
                    FileName = VGAudioCli,
                    WorkingDirectory = Path.GetDirectoryName(OpenedFileRemake["FilePath"]),
                    Arguments = "-m " + "\"" + OpenedFileRemake["FilePath"] + "\"", // TODO: escape?
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
                        OpenedFileLoop.Add("Start", mLoopStart); // Sets the loop start at the current loop start
                        OpenedFileLoop.Add("End", mLoopEnd); // Sets the loop end at the current loop end

                        OpenedFileLoop.Add("StartMax", OpenedFileLoop["End"] - 1); // Makes sure the user can only input lower number than the loop's end
                        OpenedFileLoop.Add("StartMin", 0); // The loop start value cannot be lower than the beginning of the file

                        OpenedFileLoop.Add("EndMax", mLoopEnd); // Makes sure the user doesn't input more samples than the file has
                        OpenedFileLoop.Add("EndMin", OpenedFileLoop["Start"] + 1); // Loop end has to be a bigger number than loop start

                        chk_loop.Text = "Loop the file";
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

                    UpdateStatus();
                    return true;
                }
            }
            return false;
        }

        private void FileLoaded(bool loaded = true)
        {
            btn_export.Visible = loaded;
            btn_dump.Visible = loaded;
            chk_loop.Visible = loaded;
            lbl_exportAs.Visible = loaded;
            lst_exportExtensions.Visible = loaded;

            if (loaded)
            {
                LoopTheFile("", new EventArgs());
                if (OpenCloseWinformsButton)
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
                if (OpenCloseWinformsButton)
                {
                    btn_open.Text = "Open File";
                }
            }
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
            // TODO: BRSTM - advanced settings + audio format
            UpdateStatus("Verifying the file...");

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

            if (chk_loop.Checked && exportExtension == "wav")
            {
                DialogResult dialogResult = MessageBox.Show("Loop information cannot be saved into the wave file and will be lost upon export.\r\nContinue the export without the loop information?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                if (dialogResult != DialogResult.Yes)
                {
                    UpdateStatus();
                    return;
                }
                chk_loop.Checked = false;
            }
            UpdateStatus("Verifying the file...");

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

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (FormMethods.MassPathCheck(VGAudioCli, OpenedFileRemake["FilePath"]))
                {
                    // Do stuff
                    UpdateStatus("Converting the file...");
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

                        var proc = Process.Start(procInfo);
                        proc.WaitForExit();
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
                                MessageBox.Show(standardConsoleOutput, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            // Should never occur
                            MessageBox.Show(standardConsoleOutput, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
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
                            slb_status.Text = "Please check the path of \"" + OpenedFileRemake["FileNameShort"] + "\"";
                        }
                        return;
                    }
                    slb_status.Text = message;
                    break;
                case "Close":
                    slb_status.Text = "Closed the file: " + OpenedFileRemake["FileNameShort"];
                    await Task.Delay(2000);

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
        }

        private void TestFeature()
        {

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseButtonClosesFile)
            {
                if (OpenedFileRemake.ContainsKey("FilePath"))
                {
                    e.Cancel = true;
                    CloseFile();
                }
            }
        }
    }
}
