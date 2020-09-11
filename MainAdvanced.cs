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
        private readonly string exportExtension;
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
            lbl_options.Text = String.Format("{0} options:", exportExtension.ToUpper());

            // Load if the advanced settings should be applied
            UpdateValuesFromFields();
            AdvancedToggle();
        }

        private void UpdateValuesFromFields()
        {
            chk_advanced.Checked = (bool)Main.AdvancedSettings["Apply"];
            var brstm_audioFormat = (string)Main.AdvancedSettings["BRSTM_audioFormat"];
            var hca_audioQuality = (string)Main.AdvancedSettings["HCA_audioQuality"];

            if (brstm_audioFormat != null)
            {
                lst_brstm_audioFormat.SelectedItem = brstm_audioFormat;
            }
            else
            {
                // Default value: DSP-ADPCM
                lst_brstm_audioFormat.SelectedIndex = 0;
            }

            if (hca_audioQuality != null)
            {
                lst_hca_audioQuality.SelectedItem = hca_audioQuality;
            }
            else
            {
                // Default value: High
                lst_hca_audioQuality.SelectedIndex = 1;
            }
        }

        private void AdvancedToggle(object sender = null, EventArgs e = null)
        {
            Main.AdvancedSettings["Apply"] = chk_advanced.Checked;
            if ((bool)Main.AdvancedSettings["Apply"])
            {
                // TODO:
                // - ADX options (type, framesize, keystring, keycode, filter, version)
                // - HCA options (quality, bitrate, limit bitrate)
                switch (exportExtension)
                {
                    case "brstm":
                        lbl_brstm_audioFormat.Visible = true;
                        lst_brstm_audioFormat.Visible = true;
                        lbl_hca_audioQuality.Visible = false;
                        lst_hca_audioQuality.Visible = false;
                        break;
                    case "hca":
                        lbl_brstm_audioFormat.Visible = false;
                        lst_brstm_audioFormat.Visible = false;
                        lbl_hca_audioQuality.Visible = true;
                        lst_hca_audioQuality.Visible = true;
                        break;
                    default:
                        Main.AdvancedSettings["Apply"] = false;
                        lbl_brstm_audioFormat.Visible = false;
                        lst_brstm_audioFormat.Visible = false;
                        lbl_hca_audioQuality.Visible = false;
                        lst_hca_audioQuality.Visible = false;
                        break;
                }
            }
            else
            {
                lbl_brstm_audioFormat.Visible = false;
                lst_brstm_audioFormat.Visible = false;
                lbl_hca_audioQuality.Visible = false;
                lst_hca_audioQuality.Visible = false;
            }
            lbl_options.Visible = (bool)Main.AdvancedSettings["Apply"];
        }

        private void OnClose(object sender, FormClosedEventArgs e)
        {
            // Apply the changes made
            Apply();
        }

        private void Apply()
        {
            Main.AdvancedSettings["BRSTM_audioFormat"] = lst_brstm_audioFormat.SelectedItem.ToString();
            Main.AdvancedSettings["HCA_audioQuality"] = lst_hca_audioQuality.SelectedItem.ToString();
        }

        public static void Reset()
        {
            Main.AdvancedSettings.Clear();
            Main.AdvancedSettings.Add("Apply", false);
            Main.AdvancedSettings.Add("BRSTM_audioFormat", null);
            Main.AdvancedSettings.Add("HCA_audioQuality", null);
        }
    }
}
