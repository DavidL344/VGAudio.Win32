using Microsoft.Win32;
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
        public static string AppName = GetAppInfo("name");
        public static bool VerifyIntegrity()
        {
            if (!File.Exists(Main.VGAudioCli))
            {
                try
                {
                    return ExtractCli();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to verify integrity: " + e.Message, "Error | " + AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public static string CreateExtensionFilter(string[] extensionArray)
        {
            // Add every supported file extension as one entry
            string filter = "All Supported Audio Streams|";
            bool firstEntry = true;

            foreach (string ext in extensionArray)
            {
                if (firstEntry)
                {
                    firstEntry = false;
                }
                else
                {
                    filter += ";";
                }
                filter += String.Format("*.{0}", ext.ToLower());
            }

            // Add supported file extensions as separate entries
            foreach (string ext in extensionArray)
            {
                filter += String.Format("|{0}|*.{1}", ext.ToUpper(), ext.ToLower());
            }
            return filter;
        }

        public static bool IsWindowsInDarkMode()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("AppsUseLightTheme");
                        if (o != null)
                        {
                            if (int.TryParse(o.ToString(), out int lightTheme))
                            {
                                return (lightTheme == 0);
                            }
                        }
                    }
                }
            }
            finally{}
            return false;
        }
    }
}
