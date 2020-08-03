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
        public string OpenedFile = null;
        public string OpenedFileExtension = null;
        public string VGAudioCli = Path.GetFullPath("VGAudioCli.exe");

        public Main()
        {
            InitializeComponent();
            SetupControls();
            UpdateStatus();
            TestFeature();
        }

        private void OpenFileForm(object sender, EventArgs e)
        {
            OpenFile();
            /*
            if (OpenedFile == null)
            {
                OpenFile();
            }
            else
            {
                CloseFile();
            }
            */
        }

        private void OpenFile()
        {
            if (FileDialog())
            {
                FileLoaded();
            }
            else
            {
                if (OpenedFile == null)
                {
                    FileLoaded(false);
                }
            }
        }

        private void CloseFile()
        {
            FileLoaded(false);
            OpenedFileExtension = null;
            UpdateStatus(); // Comes before clearing OpenedFile and after clearing OpenedFileExtension intentionally
            OpenedFile = null;
        }

        private bool FileDialog()
        {
            if (OpenedFile == null)
            {
                lst_exportExtensions.SelectedIndex = 1;
            }

            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Open file",
                DefaultExt = "",
                Filter = "All Supported Audio Streams|*.wav;*.dsp;*.idsp;*.brstm;*.bcstm;*.bfstm;*.hps;*.adx;*.hca;*.genh;*.at9|"
                + "WAV|*.wav|DSP|*.dsp|IDSP|*.idsp|BRSTM|*.brstm|BCSTM|*.bcstm|BFSTM|*.bfstm|HPS|*.hps|ADX|*.adx|HCA|*.hca|GENH|*.genh|AT9|*.at9",
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                OpenedFile = Path.GetFullPath(openFile.FileName);
                OpenedFileExtension = Path.GetExtension(OpenedFile);
                string[] extsArray = {".wav", ".dsp", ".idsp", ".brstm", ".bcstm", ".bfstm", ".hps", ".adx", ".hca", ".genh", ".at9" };

                if (!extsArray.Contains(OpenedFileExtension))
                {
                    MessageBox.Show("The selected file is not supported!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseFile();
                    return false;
                }
                
                if (FormMethods.MassPathCheck(VGAudioCli, OpenedFile))
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo
                    {
                        FileName = VGAudioCli,
                        WorkingDirectory = Path.GetDirectoryName(OpenedFile),
                        Arguments = "-m " + "\"" + OpenedFile + "\"",
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
                        var mLoopStartVar = FormMethods.GetBetween(metadata, "Loop start: ", " samples");
                        var mLoopEndVar = FormMethods.GetBetween(metadata, "Loop end: ", " samples");

                        var mEncodingFormat = FormMethods.GetBetween(metadata, "Encoding format: ", "\r\n");
                        var mSampleRate = FormMethods.GetBetween(metadata, "Sample rate: ", "\r\n");
                        var mChannelCount = FormMethods.GetBetween(metadata, "Channel count: ", "\r\n");

                        txt_metadata.Text = "Encoding Format: " + mEncodingFormat + "\r\nSample Rate: " + mSampleRate + "\r\nChannel Count: " + mChannelCount;

                        if (int.TryParse(mLoopStartVar, out int mLoopStart) && int.TryParse(mLoopEndVar, out int mLoopEnd))
                        {
                            //txt_metadata.Text = "Loop information:\r\nLoop Start: " + mLoopStart + " samples\r\nLoop End: " + mLoopEnd + " samples" + metadata;

                            num_loopStart.Value = mLoopStart; // Sets the loop start at the current loop start
                            num_loopStart.Maximum = mLoopEnd - 1; // Makes sure the user can only input lower number than the loop's end
                            num_loopStart.Minimum = 0; // The loop start value cannot be lower than the beginning of the file

                            num_loopEnd.Value = mLoopEnd; // Sets the loop end at the current loop end
                            num_loopEnd.Maximum = mLoopEnd; // Makes sure the user doesn't input more samples than the file has
                            num_loopEnd.Minimum = num_loopStart.Value + 1; // Loop end has to be a bigger number than loop start

                            chk_loop.Text = "Loop the file";
                            chk_loop.Checked = true;
                        }
                        else
                        {
                            //txt_metadata.Text = "No loop information found." + metadata;
                            num_loopStart.Value = 0;
                            num_loopStart.Minimum = 0;

                            var mSampleCountVar = FormMethods.GetBetween(metadata, "Sample count: ", " (");
                            if (int.TryParse(mSampleCountVar, out int mSampleCount))
                            {
                                num_loopEnd.Maximum = mSampleCount;
                                num_loopEnd.Minimum = num_loopStart.Value + 1;
                                num_loopEnd.Value = mSampleCount;
                            }
                            else
                            {
                                // Should never occur - hopefully
                                num_loopEnd.Maximum = num_loopStart.Value + 9999999; // May break the program if number of samples is higher than the file has
                                num_loopEnd.Minimum = num_loopStart.Value + 1;
                                num_loopEnd.Value = num_loopStart.Value + 1;
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
            else
            {
                /*
                OpenedFile = null;
                OpenedFileExtension = null;
                */
            }
            return false;
        }

        private void FileLoaded(bool loaded = true)
        {
            btn_export.Visible = loaded;
            chk_loop.Visible = loaded;
            lbl_exportAs.Visible = loaded;
            lst_exportExtensions.Visible = loaded;

            if (loaded)
            {
                LoopTheFile("", new EventArgs());
                //btn_open.Text = "Close File";
            }
            else
            {
                lbl_loopStart.Visible = false;
                lbl_loopEnd.Visible = false;

                num_loopStart.Visible = false;
                num_loopEnd.Visible = false;

                txt_metadata.Visible = false;
                //btn_open.Text = "Open File";
            }
        }

        private void LoopTheFile(object sender, EventArgs e)
        {
            if (chk_loop.Checked)
            {
                lbl_loopStart.Visible = true;
                lbl_loopEnd.Visible = true;

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
            // TODO: export dialog - select destination of the file
            UpdateStatus("Verifying the file...");
            var importFile = OpenedFile;
            var importExtension = OpenedFileExtension;

            if (lst_exportExtensions.SelectedItem == null)
            {
                UpdateStatus();
                MessageBox.Show("Please select the exported file's extension!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var exportExtension = lst_exportExtensions.SelectedItem.ToString().ToLower();

            if (importExtension == exportExtension)
            {
                // TODO: implement the functionality
                DialogResult dialogResult = MessageBox.Show("The file you're trying to export has the same extension as the original file. The created file will have '_exported' suffix attached to it.\r\nContinue?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
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
            UpdateStatus("Verifying file...");

            // Select the save location
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(importFile),
                Title = "Export " + Path.GetFileNameWithoutExtension(importFile) + "." + exportExtension,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = exportExtension,
                Filter = exportExtension.ToUpper() + " audio file (*." + exportExtension + ")|*." + exportExtension,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //textBox1.Text = saveFileDialog.FileName;
                
                if (FormMethods.MassPathCheck(VGAudioCli, importFile))
                {
                    // Do stuff
                    UpdateStatus("Converting file...");
                    try
                    {
                        ProcessStartInfo procInfo = new ProcessStartInfo
                        {
                            FileName = VGAudioCli,
                            WorkingDirectory = Path.GetDirectoryName(importFile),
                            Arguments = "-i \"" + importFile + "\" -o " + "\"" + Path.GetFullPath(saveFileDialog.FileName) + "\"",
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
                                DialogResult dialogResult = MessageBox.Show("Loop information cannot be saved: The Loop Start value cannot exceed Loop End value.\r\nContinue the export without the loop information?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                if (dialogResult != DialogResult.Yes)
                                {
                                    UpdateStatus();
                                    return;
                                }
                                chk_loop.Checked = false;
                            }
                            procInfo.Arguments = procInfo.Arguments + " -l " + loopStart + "-" + loopEnd;
                        }
                        //MessageBox.Show(procInfo.Arguments);

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
            return;
        }

        public async void UpdateStatus(string message = "Ready")
        {
            if (message == "Ready" && OpenedFile != null)
            {
                if (OpenedFileExtension == null)
                {
                    slb_status.Text = "Closed file: " + Path.GetFileName(OpenedFile);
                    await Task.Delay(2000);
                }
                else
                {
                    slb_status.Text = "Opened file: " + Path.GetFileName(OpenedFile);
                    return;
                }
            }
            slb_status.Text = message;
        }

        private void NumLoopOnUpdate(object sender, EventArgs e)
        {
            num_loopStart.Minimum = 0;
            num_loopStart.Maximum = num_loopEnd.Value - 1;
            num_loopEnd.Minimum = num_loopStart.Value + 1;
        }
        private void SetupControls()
        {
            MinimumSize = Size;
            MaximumSize = Size;

            MaximizeBox = false;
            //MinimizeBox = false;
            //ControlBox = false;
        }

        private void TestFeature()
        {

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (OpenedFile != null)
            {
                e.Cancel = true;
                CloseFile();
            }
        }

        /*
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        */
    }
}
