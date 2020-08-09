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
        // TODO: convert vars to a single dictionary
        //public string OpenedFile = null;
        //public string OpenedFileExtension = null;

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

        private void CloseFile()
        {
            FileLoaded(false);
            //OpenedFileExtension = null;
            UpdateStatus("Close"); // OLD: Comes before clearing OpenedFile and after clearing OpenedFileExtension intentionally
            //OpenedFile = null;

            OpenedFileRemake.Clear();
            OpenedFileLoop.Clear();
        }

        private bool FileDialog()
        {
            if (!OpenedFileRemake.ContainsKey("FilePath"))
            {
                lst_exportExtensions.SelectedIndex = 1;
            }

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
                // Load file information
                OpenedFileRemake.Add("FileName", Path.GetFileName(openFile.FileName));
                OpenedFileRemake.Add("FilePath", openFile.FileName);
                OpenedFileRemake.Add("FilePathEscaped", "\"" + OpenedFileRemake["FilePath"] + "\"");
                OpenedFileRemake.Add("FileNoExtension", Path.GetFileNameWithoutExtension(OpenedFileRemake["FilePath"]));

                // Load file extension without the '.' at the beginning
                var FileExtensionWithDot = Path.GetExtension(OpenedFileRemake["FilePath"]);
                OpenedFileRemake.Add("FileExtension", FileExtensionWithDot.Substring(1));


                // TODO: testing only
                /*
                var test = OpenedFileRemake["FilePath"];
                MessageBox.Show(test);
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(0);
                */

                //OpenedFile = OpenedFileRemake["FilePath"];
                //OpenedFileExtension = OpenedFileRemake["FileExtension"];

                if (!extsArray.Contains(OpenedFileRemake["FileExtension"]))
                {
                    MessageBox.Show("The selected file is not supported!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseFile();
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
                        var metadata = proc.StandardOutput.ReadToEnd();

                        // Vars that are later converted to int
                        var mLoopStartVar = FormMethods.GetBetween(metadata, "Loop start: ", " samples");
                        var mLoopEndVar = FormMethods.GetBetween(metadata, "Loop end: ", " samples");

                        OpenedFileRemake.Add("EncodingFormat", FormMethods.GetBetween(metadata, "Encoding format: ", "\r\n"));
                        OpenedFileRemake.Add("SampleRate", FormMethods.GetBetween(metadata, "Sample rate: ", "\r\n"));
                        OpenedFileRemake.Add("ChannelCount", FormMethods.GetBetween(metadata, "Channel count: ", "\r\n"));

                        txt_metadata.Text = OpenedFileRemake["EncodingFormat"] + "\r\nSample Rate: " + OpenedFileRemake["SampleRate"] + "\r\nChannel Count: " + OpenedFileRemake["ChannelCount"];

                        if (int.TryParse(mLoopStartVar, out int mLoopStart) && int.TryParse(mLoopEndVar, out int mLoopEnd))
                        {
                            OpenedFileLoop.Add("Start", mLoopStart); // Sets the loop start at the current loop start
                            OpenedFileLoop.Add("End", mLoopEnd); // Sets the loop end at the current loop end

                            OpenedFileLoop.Add("StartMax", OpenedFileLoop["End"] - 1); // Makes sure the user can only input lower number than the loop's end
                            OpenedFileLoop.Add("StartMin", 0); // The loop start value cannot be lower than the beginning of the file

                            OpenedFileLoop.Add("EndMax", mLoopEnd); // Makes sure the user doesn't input more samples than the file has
                            OpenedFileLoop.Add("EndMin", OpenedFileLoop["Start"] + 1); // Loop end has to be a bigger number than loop start

                            num_loopStart.Value = OpenedFileLoop["Start"];
                            num_loopStart.Maximum = OpenedFileLoop["StartMax"];
                            num_loopStart.Minimum = OpenedFileLoop["StartMin"];

                            num_loopEnd.Value = OpenedFileLoop["End"];
                            num_loopEnd.Maximum = OpenedFileLoop["EndMax"];
                            num_loopEnd.Minimum = OpenedFileLoop["EndMin"];

                            chk_loop.Text = "Loop the file";
                            chk_loop.Checked = true;
                        }
                        else
                        {
                            // TODO: where is StartMax?
                            OpenedFileLoop.Add("Start", 0);
                            OpenedFileLoop.Add("StartMin", 0);

                            var mSampleCountVar = FormMethods.GetBetween(metadata, "Sample count: ", " (");
                            if (int.TryParse(mSampleCountVar, out int mSampleCount))
                            {
                                // If there's no loop, the new loop end is end of the file by default
                                OpenedFileLoop.Add("EndMax", mSampleCount);
                                OpenedFileLoop.Add("EndMin", OpenedFileLoop["Start"] + 1);
                                OpenedFileLoop.Add("End", OpenedFileLoop["EndMax"]);

                                num_loopEnd.Maximum = OpenedFileLoop["EndMax"];
                                num_loopEnd.Minimum = OpenedFileLoop["EndMin"];
                                num_loopEnd.Value = OpenedFileLoop["End"];
                            }
                            else
                            {
                                // Should never occur - hopefully
                                OpenedFileLoop.Add("EndMax", OpenedFileLoop["Start"] + 9999999); // May break the program if number of samples is higher than the file has
                                OpenedFileLoop.Add("EndMin", OpenedFileLoop["Start"] + 1);
                                OpenedFileLoop.Add("End", OpenedFileLoop["Start"] + 1);

                                num_loopEnd.Maximum = OpenedFileLoop["EndMax"];
                                num_loopEnd.Minimum = OpenedFileLoop["EndMin"];
                                num_loopEnd.Value = OpenedFileLoop["End"];
                            }
                            num_loopStart.Maximum = num_loopEnd.Maximum - 1;

                            chk_loop.Text = "Create a loop";
                            chk_loop.Checked = false;
                        }
                        UpdateStatus();
                    }
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
            var importFile = OpenedFileRemake["FilePath"]; //OpenedFile
            var importExtension = OpenedFileRemake["FileExtension"]; //OpenedFileExtension.Remove(0, 1)

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
            UpdateStatus("Dumping info...");
            var path = OpenedFileRemake["FilePath"] + ".dump";
            string[] lines = { "dw", "de" }; // TODO: actually dump stuff
            File.WriteAllLines(path, lines);
            UpdateStatus();
            MessageBox.Show("Info dumped to " + path + "!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public async void UpdateStatus(string message = "Ready")
        {
            /*
            foreach (KeyValuePair<string, string> kvp in OpenedFileRemake)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                //Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                MessageBox.Show("Key = " + kvp.Key + "\r\nValue = " + kvp.Value);
            }
            */

            switch(message)
            {
                case "Ready":
                    if (OpenedFileRemake.ContainsKey("FileName"))
                    {
                        slb_status.Text = "Opened the file: " + OpenedFileRemake["FileName"];
                        return;
                    }
                    slb_status.Text = message;
                    break;
                case "Close":
                    slb_status.Text = "Closed the file: " + OpenedFileRemake["FileName"];
                    await Task.Delay(2000);

                    // Another file might've been opened in the meantime when the previous file was closed
                    // Was another file opened during the Task.Delay?
                    if (OpenedFileRemake.ContainsKey("FileExtension"))
                    {
                        slb_status.Text = "Opened the file: " + OpenedFileRemake["FileName"];
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
            OpenedFileLoop["StartMax"] = (int)num_loopEnd.Value - 1;
            OpenedFileLoop["EndMin"] = (int)num_loopStart.Value + 1;

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
            /* Changed!
            OpenedFileRemake.Add("File", null);
            OpenedFileRemake.Add("FileNoExtension", null);
            OpenedFileRemake.Add("FileExtension", null);
            OpenedFileRemake.Add("LoopStart", null);
            OpenedFileRemake.Add("LoopEnd", null);
            OpenedFileRemake.Add("EncodingFormat", null);
            OpenedFileRemake.Add("SampleRate", null);
            OpenedFileRemake.Add("ChannelCount", null);
            */
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
