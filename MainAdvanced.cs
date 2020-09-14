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
        private readonly Dictionary<string, int> BitrateValue = new Dictionary<string, int>();
        public MainAdvanced(string exportExtension)
        {
            InitializeComponent();
            this.exportExtension = exportExtension.ToLower();

            BitrateValue.Add("Highest", 353);
            BitrateValue.Add("High", 235);
            BitrateValue.Add("Middle", 176);
            BitrateValue.Add("Low", 117);
            BitrateValue.Add("Lowest", 88);
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
            HcaCheckboxToggle();
        }

        private void UpdateValuesFromFields()
        {
            chk_advanced.Checked = (bool)Main.AdvancedSettings["Apply"];
            var brstm_audioFormat = (string)Main.AdvancedSettings["BRSTM_audioFormat"];
            var hca_audioQuality = (string)Main.AdvancedSettings["HCA_audioQuality"];

            decimal hca_audioBitrate;
            if (Main.AdvancedSettings["HCA_audioBitrate"] != null)
            {
                hca_audioBitrate = (decimal)Main.AdvancedSettings["HCA_audioBitrate"];
                if (hca_audioBitrate < num_hca_audioBitrate.Minimum || hca_audioBitrate > num_hca_audioBitrate.Maximum)
                {
                    // Set the default bitrate value from the form
                    hca_audioBitrate = (int)num_hca_audioBitrate.Value;
                }
            }
            else
            {
                hca_audioBitrate = num_hca_audioBitrate.Value;
            }

            bool hca_limitBitrate;
            if (Main.AdvancedSettings["HCA_limitBitrate"] != null)
            {
                hca_limitBitrate = (bool)Main.AdvancedSettings["HCA_limitBitrate"];
            }
            else
            {
                hca_limitBitrate = false;
            }

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

            num_hca_audioBitrate.Value = hca_audioBitrate;
            chk_hca_limitBitrate.Checked = hca_limitBitrate;

            switch (Main.AdvancedSettings["HCA_audioRadioButtonSelector"])
            {
                case "bitrate":
                    rb_hca_audioQuality.Checked = false;
                    rb_hca_audioBitrate.Checked = true;
                    break;
                case "quality":
                case null:
                default:
                    rb_hca_audioQuality.Checked = true;
                    rb_hca_audioBitrate.Checked = false;
                    break;
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
                        pnl_brstm.Visible = true;
                        pnl_hca.Visible = false;
                        break;
                    case "hca":
                        pnl_brstm.Visible = false;
                        pnl_hca.Visible = true;
                        break;
                    default:
                        Main.AdvancedSettings["Apply"] = false;
                        pnl_brstm.Visible = false;
                        pnl_hca.Visible = false;
                        break;
                }
            }
            else
            {
                pnl_brstm.Visible = false;
                pnl_hca.Visible = false;
            }
            lbl_options.Visible = (bool)Main.AdvancedSettings["Apply"];
        }

        private void HcaCheckboxToggle(object sender = null, EventArgs e = null)
        {
            lst_hca_audioQuality.Visible = rb_hca_audioQuality.Checked;
            num_hca_audioBitrate.Visible = rb_hca_audioBitrate.Checked;

            if (rb_hca_audioQuality.Checked) UpdateApproxKbps();
            if (rb_hca_audioBitrate.Checked) UpdateKbpsConversion();
        }

        private void UpdateKbpsConversion(object sender = null, EventArgs e = null)
        {
            lbl_hca_conversion.Text = String.Format("bps = {0} Kbps", num_hca_audioBitrate.Value / 1000);
        }
        private void UpdateApproxKbps(object sender = null, EventArgs e = null)
        {
            int approxValue = BitrateValue[lst_hca_audioQuality.SelectedItem.ToString()];
            lbl_hca_conversion.Text = String.Format("= approx. {0} Kbps", approxValue);
        }

        private void OnClose(object sender, FormClosedEventArgs e)
        {
            // Apply the changes made
            Apply();
        }

        private void Apply()
        {
            switch (exportExtension)
            {
                case "brstm":
                    Main.AdvancedSettings["BRSTM_audioFormat"] = lst_brstm_audioFormat.SelectedItem.ToString();
                    break;
                case "hca":
                    if (rb_hca_audioQuality.Checked)
                    {
                        Main.AdvancedSettings["HCA_audioQuality"] = lst_hca_audioQuality.SelectedItem.ToString();
                    }
                    if (rb_hca_audioBitrate.Checked) Main.AdvancedSettings["HCA_audioBitrate"] = num_hca_audioBitrate.Value;
                    Main.AdvancedSettings["HCA_limitBitrate"] = chk_hca_limitBitrate.Checked;

                    if (rb_hca_audioBitrate.Checked) Main.AdvancedSettings["HCA_audioRadioButtonSelector"] = "bitrate";
                    if (rb_hca_audioQuality.Checked) Main.AdvancedSettings["HCA_audioRadioButtonSelector"] = "quality";
                    break;
                default:
                    Main.AdvancedSettings["Apply"] = false;
                    return;
            }
            Main.AdvancedSettings["Apply"] = chk_advanced.Checked;
        }

        public static void Reset()
        {
            Main.AdvancedSettings.Clear();
            Main.AdvancedSettings.Add("Apply", false);
            Main.AdvancedSettings.Add("BRSTM_audioFormat", null);
            Main.AdvancedSettings.Add("HCA_audioRadioButtonSelector", null);
            Main.AdvancedSettings.Add("HCA_audioQuality", null);
            Main.AdvancedSettings.Add("HCA_audioBitrate", null);
            Main.AdvancedSettings.Add("HCA_limitBitrate", null);
        }
    }
}
