using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public partial class MainAdvanced : Form
    {
        private readonly string exportExtension;
        private readonly Dictionary<string, int> BitrateValue = new Dictionary<string, int>();
        private readonly OpenedFile OpenedFile;
        public MainAdvanced(OpenedFile OpenedFile)
        {
            InitializeComponent();
            this.exportExtension = OpenedFile.ExportInfo["ExtensionNoDot"].ToLower();
            this.OpenedFile = OpenedFile;
            if (!OpenedFile.AdvancedExportInfo.ContainsKey("Apply")) Reset();

            // Add samples from 44100 Hz sample CRI HCA file
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
            Main.AppTheme.Apply(this);

            // Load if the advanced settings should be applied
            UpdateValuesFromFields();
            AdvancedToggle();
        }

        private void UpdateValuesFromFields()
        {
            chk_advanced.Checked = (bool)OpenedFile.AdvancedExportInfo["Apply"];

            // Update values for ADX
            var adx_encrypt = (bool)OpenedFile.AdvancedExportInfo["ADX_encrypt"];
            var adx_type = (string)OpenedFile.AdvancedExportInfo["ADX_type"];
            var adx_keystring_use = (bool)OpenedFile.AdvancedExportInfo["ADX_keystring_use"];
            var adx_keystring = (string)OpenedFile.AdvancedExportInfo["ADX_keystring"];
            var adx_keycode_use = (bool)OpenedFile.AdvancedExportInfo["ADX_keycode_use"];
            var adx_keycode = (string)OpenedFile.AdvancedExportInfo["ADX_keycode"];
            var adx_filter_use = (bool)OpenedFile.AdvancedExportInfo["ADX_filter_use"];
            var adx_filter = (int?)OpenedFile.AdvancedExportInfo["ADX_filter"];
            var adx_version_use = (bool)OpenedFile.AdvancedExportInfo["ADX_version_use"];
            var adx_version = (int?)OpenedFile.AdvancedExportInfo["ADX_version"];

            if (adx_type != null)
            {
                lst_adx_type.SelectedItem = adx_type;
            }
            else
            {
                // Default value: Linear
                lst_adx_type.SelectedIndex = 0;
            }

            if (adx_keystring != null) txt_adx_encrypt_keystring.Text = adx_keystring;
            if (adx_keycode != null) txt_adx_encrypt_keycode.Text = adx_keycode;
            if (adx_filter != null) num_adx_encrypt_filter.Value = (Decimal)adx_filter;
            if (adx_version != null) num_adx_encrypt_version.Value = (Decimal)adx_version;

            chk_adx_encrypt.Checked = adx_encrypt;
            chk_adx_encrypt_keystring.Checked = adx_keystring_use;
            chk_adx_encrypt_keycode.Checked = adx_keycode_use;
            chk_adx_encrypt_filter.Checked = adx_filter_use;
            chk_adx_encrypt_version.Checked = adx_version_use;

            // Update values for BRSTM
            var brstm_audioFormat = (string)OpenedFile.AdvancedExportInfo["BRSTM_audioFormat"];

            if (brstm_audioFormat != null)
            {
                lst_brstm_audioFormat.SelectedItem = brstm_audioFormat;
            }
            else
            {
                // Default value: DSP-ADPCM
                lst_brstm_audioFormat.SelectedIndex = 0;
            }

            // Update values for HCA
            var hca_audioQuality = (string)OpenedFile.AdvancedExportInfo["HCA_audioQuality"];

            decimal hca_audioBitrate;
            if (OpenedFile.AdvancedExportInfo["HCA_audioBitrate"] != null)
            {
                // Check if the bitrate saved in the dictionary is valid
                hca_audioBitrate = (decimal)OpenedFile.AdvancedExportInfo["HCA_audioBitrate"];
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
            if (OpenedFile.AdvancedExportInfo["HCA_limitBitrate"] != null)
            {
                hca_limitBitrate = (bool)OpenedFile.AdvancedExportInfo["HCA_limitBitrate"];
            }
            else
            {
                hca_limitBitrate = false;
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

            switch (OpenedFile.AdvancedExportInfo["HCA_audioRadioButtonSelector"])
            {
                case "bitrate":
                    rb_hca_audioBitrate.Checked = true;
                    break;
                case "quality":
                case null:
                default:
                    rb_hca_audioQuality.Checked = true;
                    break;
            }
            lbl_hca_conversion.Visible = true;
        }

        private void AdvancedToggle(object sender = null, EventArgs e = null)
        {
            OpenedFile.AdvancedExportInfo["Apply"] = chk_advanced.Checked;
            if ((bool)OpenedFile.AdvancedExportInfo["Apply"])
            {
                pnl_adx.Visible = false;
                pnl_brstm.Visible = false;
                pnl_hca.Visible = false;

                switch (exportExtension)
                {
                    case "adx":
                        AdxCheckboxToggle();
                        pnl_adx.Visible = true;
                        break;
                    case "brstm":
                        pnl_brstm.Visible = true;
                        break;
                    case "hca":
                        pnl_hca.Visible = true;
                        break;
                    default:
                        OpenedFile.AdvancedExportInfo["Apply"] = false;
                        break;
                }
            }
            else
            {
                pnl_adx.Visible = false;
                pnl_brstm.Visible = false;
                pnl_hca.Visible = false;
            }
            lbl_options.Visible = (bool)OpenedFile.AdvancedExportInfo["Apply"];
        }

        private void AdxCheckboxToggle(object sender = null, EventArgs e = null)
        {
            lst_adx_type.Visible = chk_adx_encrypt.Checked;
            chk_adx_encrypt_keystring.Visible = chk_adx_encrypt.Checked;
            chk_adx_encrypt_keycode.Visible = chk_adx_encrypt.Checked;
            chk_adx_encrypt_filter.Visible = chk_adx_encrypt.Checked;
            chk_adx_encrypt_version.Visible = chk_adx_encrypt.Checked;

            txt_adx_encrypt_keystring.Enabled = chk_adx_encrypt_keystring.Checked;
            txt_adx_encrypt_keycode.Enabled = chk_adx_encrypt_keycode.Checked;
            num_adx_encrypt_filter.Enabled = chk_adx_encrypt_filter.Checked;
            num_adx_encrypt_version.Enabled = chk_adx_encrypt_version.Checked;

            if (chk_adx_encrypt.Checked)
            {
                chk_adx_encrypt.Text = "Encryption type:";
                txt_adx_encrypt_keystring.Visible = chk_adx_encrypt_keystring.Checked;
                txt_adx_encrypt_keycode.Visible = chk_adx_encrypt_keycode.Checked;
                num_adx_encrypt_filter.Visible = chk_adx_encrypt_filter.Checked;
                num_adx_encrypt_version.Visible = chk_adx_encrypt_version.Checked;
            }
            else
            {
                chk_adx_encrypt.Text = "Encrypt";
                txt_adx_encrypt_keystring.Visible = false;
                txt_adx_encrypt_keycode.Visible = false;
                num_adx_encrypt_filter.Visible = false;
                num_adx_encrypt_version.Visible = false;
            }
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
            // Get the approximate value from 44100 Hz sample
            int approxValue = BitrateValue[lst_hca_audioQuality.SelectedItem.ToString()];

            // Strip down the string to just the integer part, then try to parse it as a one
            var sampleRateSplit = Regex.Split(OpenedFile.Metadata["SampleRate"], " Hz")[0];
            if (int.TryParse(sampleRateSplit, out int sampleRate))
            {
                // Make an approximate calculation from the file's sample rate
                approxValue /= 44100 / sampleRate;
            }
            else
            {
                MessageBox.Show("Sample rate calculation failed: " + OpenedFile.Metadata["SampleRate"]);
            }

            if (Main.FeatureConfig["SyncBitrateTabs"]) num_hca_audioBitrate.Value = approxValue * 1000;
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
                case "adx":
                    OpenedFile.AdvancedExportInfo["ADX_encrypt"] = chk_adx_encrypt.Checked;
                    OpenedFile.AdvancedExportInfo["ADX_type"] = lst_adx_type.SelectedItem.ToString();

                    OpenedFile.AdvancedExportInfo["ADX_keystring_use"] = chk_adx_encrypt_keystring.Checked;
                    OpenedFile.AdvancedExportInfo["ADX_keystring"] = txt_adx_encrypt_keystring.Text;

                    OpenedFile.AdvancedExportInfo["ADX_keycode_use"] = chk_adx_encrypt_keycode.Checked;
                    OpenedFile.AdvancedExportInfo["ADX_keycode"] = txt_adx_encrypt_keycode.Text;

                    OpenedFile.AdvancedExportInfo["ADX_filter_use"] = chk_adx_encrypt_filter.Checked;
                    OpenedFile.AdvancedExportInfo["ADX_filter"] = (int?)num_adx_encrypt_filter.Value;

                    OpenedFile.AdvancedExportInfo["ADX_version_use"] = chk_adx_encrypt_version.Checked;
                    OpenedFile.AdvancedExportInfo["ADX_version"] = (int?)num_adx_encrypt_version.Value;
                    break;
                case "brstm":
                    OpenedFile.AdvancedExportInfo["BRSTM_audioFormat"] = lst_brstm_audioFormat.SelectedItem.ToString();
                    break;
                case "hca":
                    if (rb_hca_audioQuality.Checked)
                    {
                        OpenedFile.AdvancedExportInfo["HCA_audioQuality"] = lst_hca_audioQuality.SelectedItem.ToString();
                    }
                    if (rb_hca_audioBitrate.Checked) OpenedFile.AdvancedExportInfo["HCA_audioBitrate"] = num_hca_audioBitrate.Value;
                    OpenedFile.AdvancedExportInfo["HCA_limitBitrate"] = chk_hca_limitBitrate.Checked;

                    if (rb_hca_audioBitrate.Checked) OpenedFile.AdvancedExportInfo["HCA_audioRadioButtonSelector"] = "bitrate";
                    if (rb_hca_audioQuality.Checked) OpenedFile.AdvancedExportInfo["HCA_audioRadioButtonSelector"] = "quality";
                    break;
                default:
                    OpenedFile.AdvancedExportInfo["Apply"] = false;
                    return;
            }
            OpenedFile.AdvancedExportInfo["Apply"] = chk_advanced.Checked;
        }

        public void Reset()
        {
            OpenedFile.AdvancedExportInfo.Clear();
            OpenedFile.AdvancedExportInfo.Add("Apply", false);
            OpenedFile.AdvancedExportInfo.Add("ADX_encrypt", false);
            OpenedFile.AdvancedExportInfo.Add("ADX_type", null);
            OpenedFile.AdvancedExportInfo.Add("ADX_keystring_use", false);
            OpenedFile.AdvancedExportInfo.Add("ADX_keystring", null);
            OpenedFile.AdvancedExportInfo.Add("ADX_keycode_use", false);
            OpenedFile.AdvancedExportInfo.Add("ADX_keycode", null);
            OpenedFile.AdvancedExportInfo.Add("ADX_filter_use", false);
            OpenedFile.AdvancedExportInfo.Add("ADX_filter", null);
            OpenedFile.AdvancedExportInfo.Add("ADX_version_use", false);
            OpenedFile.AdvancedExportInfo.Add("ADX_version", null);
            OpenedFile.AdvancedExportInfo.Add("BRSTM_audioFormat", null);
            OpenedFile.AdvancedExportInfo.Add("HCA_audioRadioButtonSelector", null);
            OpenedFile.AdvancedExportInfo.Add("HCA_audioQuality", null);
            OpenedFile.AdvancedExportInfo.Add("HCA_audioBitrate", null);
            OpenedFile.AdvancedExportInfo.Add("HCA_limitBitrate", null);
        }
    }
}
