using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public partial class BatchConvert : Form
    {
        private string[] importedFiles;
        public Dictionary<string, int> fileStatus = new Dictionary<string, int>();
        public BatchConvert(string[] files)
        {
            InitializeComponent();
            LoadFiles(files);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            MinimumSize = Size;
            MaximumSize = Size;
            MaximizeBox = false;
        }

        private void LoadFiles(string[] importedItems)
        {
            lb_importedFiles.Items.Clear();
            fileStatus.Clear();
            fileStatus.Add("added", 0);
            fileStatus.Add("failed", 0);

            foreach (string item in importedItems)
            {
                string[] importedDirContents;
                if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                {
                    importedDirContents = LoadDirectory(item);
                    
                    foreach (string file in importedDirContents)
                    {
                        lb_importedFiles.Items.Add(file);
                    }
                }
                else
                {
                    if (CheckFile(item)) lb_importedFiles.Items.Add(item);
                }
                
            }
            this.importedFiles = lb_importedFiles.Items.OfType<string>().ToArray();

            string fileSuccessText;
            string fileFailText;

            if (fileStatus["added"] == 1)
            {
                fileSuccessText = "file";
            }
            else
            {
                fileSuccessText = "files";
            }

            if (fileStatus["failed"] == 1)
            {
                fileFailText = "file";
            }
            else
            {
                fileFailText = "files";
            }

            string importStatusMessage;
            MessageBoxIcon importStatusIcon = MessageBoxIcon.Information;
            if (fileStatus["added"] > 0)
            {
                importStatusMessage = String.Format("Successfully imported {0} {1}!", "" + fileStatus["added"], fileSuccessText);
                if (fileStatus["failed"] > 0)
                {
                    importStatusMessage += String.Format("\r\nFailed to import {0} {1}.", "" + fileStatus["failed"], fileFailText);
                    importStatusIcon = MessageBoxIcon.Warning;
                }
            }
            else
            {
                importStatusMessage = "Unable to load any of the selected files!";
                importStatusIcon = MessageBoxIcon.Error;
            }
            MessageBox.Show(importStatusMessage, Text, MessageBoxButtons.OK, importStatusIcon);
        }

        private string[] LoadDirectory(string directory, bool recursive = true)
        {
            List<string> dirContents = new List<string>();
            string[] getDirContents = Directory.GetFiles(directory);

            foreach (string item in getDirContents)
            {
                if (CheckFile(item)) dirContents.Add(item);
            }

            if (recursive)
            {
                string[] getDirDirectories = Directory.GetDirectories(directory);
                string[] nestedDirContents;

                foreach (string item in getDirDirectories)
                {
                    if (File.GetAttributes(item).HasFlag(FileAttributes.Directory))
                    {
                        nestedDirContents = LoadDirectory(item, recursive);
                        foreach (string nestedItem in nestedDirContents)
                        {
                            // Each file here doesn't need to be checked
                            // It already was through the recursive directory scanning
                            dirContents.Add(nestedItem);
                        }
                    }
                }
            }
            return dirContents.ToArray();
        }

        private bool CheckFile(string file)
        {
            // TODO: don't open the form if the number of successfully imported files is 0
            // TODO: check if the file is valid, has the valid extension and doesn't return null like recycle bin does
            // TODO: if the MainAdvanced form is called from here, it'll save advanced options here, to the future-existing dictionary, instead

            var FileExtensionWithDot = Path.GetExtension(file);
            var FileExtensionWithoutDot = FileExtensionWithDot.Substring(1);
            if (Main.extsArray.Contains(FileExtensionWithoutDot.ToLower()))
            {
                if (!FormMethods.IsFileLocked(file))
                {
                    if (FormMethods.VerifyIntegrity(file))
                    {
                        ProcessStartInfo procInfo = new ProcessStartInfo
                        {
                            FileName = Main.VGAudioCli,
                            WorkingDirectory = Path.GetDirectoryName(file),
                            Arguments = "-m " + String.Format("\"{0}\"", file),
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        var proc = Process.Start(procInfo);
                        proc.WaitForExit();
                        if (proc.ExitCode == 0)
                        {
                            fileStatus["added"]++;
                            return true;
                        }
                    }
                }
            }
            fileStatus["failed"]++;
            return false;
        }
    }
}
