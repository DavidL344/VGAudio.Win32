﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VGAudio.Win32.Properties;

namespace VGAudio.Win32
{
    class FormMethods
    {
        private static readonly Main AppForm = new Main();
        public static bool MassPathCheck(string VGAudioCli, string inputFile)
        {
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

            if (!File.Exists(VGAudioCli))
            {
                try
                {
                    using (FileStream fsDst = new FileStream(VGAudioCli, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] bytes = Resources.VGAudioCli;
                        fsDst.Write(bytes, 0, bytes.Length);
                    }
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
            //MessageBox.Show("" + trimLength);

            if (fileNameWithExtension.Length > maxLength)
            {
                if (fileNameWithExtension.Length > trimLength)
                {
                    var truncate = Truncate(fileNameWithExtension, trimLength);
                    //MessageBox.Show(truncate);

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

            //MessageBox.Show(newFileName + " " + newFileName.Length);
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
    }
}
