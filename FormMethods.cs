using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VGAudio.Win32.Properties;

namespace VGAudio.Win32
{
    static class FormMethods
    {
        private static readonly Main AppForm = new Main();
        private static FileStream FileStreamLock;
        public static bool VerifyIntegrity(string inputFile = null)
        {
            if (inputFile != null)
            {
                // Try to unlock the file to check if it exists
                if (!File.Exists(inputFile))
                {
                    MessageBox.Show("The selected file no longer exists!", "Error | " + AppForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Closing the file throws an error, because OpenedFileRemake is a part of another class and is called from here
                    // Leaving CloseFile() as a private method for now
                    // AppForm.CloseFile();

                    // Status is overridden by another call of UpdateStatus directly in Main
                    // AppForm.UpdateStatus("Force closed the file: " + inputFile);
                    return false;
                }
            }

            if (!File.Exists(Main.VGAudioCli))
            {
                try
                {
                    return ExtractCli();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to verify integrity: " + e.Message, "Error | " + AppForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppForm.UpdateStatus();
                    return false;
                }
            }
            return true;
        }

        public static bool ExtractCli()
        {
            if (!File.Exists(Main.VGAudioCli))
            {
                try
                {
                    using (FileStream fsDst = new FileStream(Main.VGAudioCli, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] bytes = Resources.VGAudioCli;
                        fsDst.Write(bytes, 0, bytes.Length);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Unable to extract VGAudioCli: {0}", e.Message), "VGAudio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        // https://stackoverflow.com/a/10709874
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            return "";
        }

        // https://stackoverflow.com/a/2776689
        public static string Truncate(string value, int maxLength = 15)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string TruncateFileName(string fileNameWithExtension, string fileExtension, int maxLength = 25)
        {
            var newFileName = fileNameWithExtension;
            var fileExtInfo = "... (." + fileExtension + ")";
            int trimLength = maxLength - fileExtInfo.Length;

            if (fileNameWithExtension.Length > maxLength)
            {
                if (fileNameWithExtension.Length > trimLength)
                {
                    var truncate = Truncate(fileNameWithExtension, trimLength);
                    if (fileNameWithExtension != truncate)
                    {
                        newFileName = truncate + fileExtInfo;
                    }
                }
                else
                {
                    // Should never happen
                    MessageBox.Show("Internal Error: The trim length is bigger than the file length itself!", AppForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return newFileName;
        }

        public static string GetAppInfo(string type = "name")
        {
            switch (type)
            {
                case "name":
                    // https://stackoverflow.com/a/4266261
                    return Assembly.GetExecutingAssembly().GetName().Name;
                case "version":
                    // https://stackoverflow.com/a/909583
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    return fvi.FileVersion;
                default:
                    return "";
            }
        }

        // https://stackoverflow.com/a/15937460
        [DllImport("user32")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint itemId, uint uEnable);

        public static void EnableCloseButton(this Form form, bool enabled = true)
        {
            if (enabled)
            {
                // The zero parameter means to enable. 0xF060 is SC_CLOSE.
                EnableMenuItem(GetSystemMenu(form.Handle, false), 0xF060, 0);
            }
            else
            {
                // The 1 parameter means to gray out. 0xF060 is SC_CLOSE.
                EnableMenuItem(GetSystemMenu(form.Handle, false), 0xF060, 1);
            }
        }
        
        public static bool FileLock(string file = null)
        {
            if (AppForm.FeatureConfig.ContainsKey("LockOpenedFile") && AppForm.FeatureConfig["LockOpenedFile"])
            {
                if (file != null)
                {
                    if (File.Exists(file))
                    {
                        if (!IsFileLocked(file))
                        {
                            if (FileStreamLock != null)
                            {
                                FileLock(null);
                            }

                            // https://stackoverflow.com/a/3279183
                            FileStreamLock = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // https://stackoverflow.com/a/872328
                    FileStreamLock?.Close();
                    return true;
                }
            }
            return false;
        }

        // https://stackoverflow.com/a/937558
        public static bool IsFileLocked(string file)
        {
            try
            {
                using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                // The file is unavailable because it is:
                // - still being written to
                // - being processed by another thread
                // - does not exist (has already been processed)
                return true;
            }

            // The file is not locked
            return false;
        }

        // https://stackoverflow.com/a/5048766
        public static void ActionListener(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                AboutBox aboutBox = new AboutBox();
                aboutBox.ShowDialog();
                e.SuppressKeyPress = true; // Stops other controls on the form receiving event.
            }
        }
    }
}
