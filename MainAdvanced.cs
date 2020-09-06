using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public partial class MainAdvanced : Form
    {
        private string exportExtension;
        public static string brstm_audioFormat;
        public MainAdvanced(string exportExtension)
        {
            InitializeComponent();
            this.exportExtension = exportExtension.ToLower();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            MinimumSize = Size;
            MaximumSize = Size;
            MaximizeBox = false;

            lbl_brstm_audioFormat.Visible = false;
            lst_brstm_audioFormat.Visible = false;

            switch (exportExtension)
            {
                case "brstm":
                    lbl_brstm_audioFormat.Visible = true;
                    lst_brstm_audioFormat.Visible = true;
                    if (lst_brstm_audioFormat.SelectedItem == null)
                    {
                        lst_brstm_audioFormat.SelectedIndex = 0;
                    }
                    break;
                default:
                    break;
            }
        }

        private void lst_brstm_audioFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            brstm_audioFormat = lst_brstm_audioFormat.SelectedItem.ToString();
        }
    }
}
