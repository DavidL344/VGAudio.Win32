using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                AppForm.UpdateStatus();
                return false;
            }

            if (!File.Exists(VGAudioCli))
            {
                try
                {
                    using (FileStream fsDst = new FileStream(VGAudioCli, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] bytes = Resources.GetVGAudioCli();
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
    }
}
