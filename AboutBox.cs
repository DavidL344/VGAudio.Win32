using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.lbl_productName.Text = String.Format("{0} {1}", AssemblyProduct, AssemblyVersion);
            this.lbl_productAuthor.Text = String.Format("Created by {0}", AssemblyCompany);
            this.lbl_ogProjectAuthor.Text = String.Format("Original project by Alex Barney");
            this.lbl_cliVersion.Text = String.Format("Running on VGAudioCli {0}", GetCliVersion());
            this.txt_license.Text = ReadLicense();
        }

        private string ReadLicense()
        {
            string Win32License;
            string VGAudioLicense;

            byte[] win32_license = VGAudio.Win32.Properties.Resources.LICENSE;
            using (MemoryStream stream = new MemoryStream(win32_license))
            using (StreamReader reader = new StreamReader(stream))
            {
                Win32License = reader.ReadToEnd();
            }

            byte[] vgaudio_license = VGAudio.Win32.Properties.Resources.LICENSE_VGAUDIO;
            using (MemoryStream stream = new MemoryStream(vgaudio_license))
            using (StreamReader reader = new StreamReader(stream))
            {
                VGAudioLicense = reader.ReadToEnd();
            }

            return String.Format("License (VGAudio.Win32):\r\n{0}\r\nLicense (VGAudio):\r\n{1}", Win32License, VGAudioLicense);
        }

        private string GetCliVersion()
        {
            if (FormMethods.MassPathCheck(Main.VGAudioCli))
            {
                ProcessStartInfo procInfo = new ProcessStartInfo
                {
                    FileName = Main.VGAudioCli,
                    WorkingDirectory = Path.GetDirectoryName(Main.VGAudioCli),
                    Arguments = "--version ",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var proc = Process.Start(procInfo);
                proc.WaitForExit();
                if (proc.ExitCode == 1)
                {
                    return FormMethods.GetBetween(proc.StandardOutput.ReadToEnd(), "VGAudio v", "\r\n");
                }
            }
            return "(Unknown)";
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
