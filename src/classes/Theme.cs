using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public class Theme
    {
        public Dictionary<string, Dictionary<string, Color>> ColorScheme = new Dictionary<string, Dictionary<string, Color>>();
        
        public Theme()
        {
            ColorScheme.Add("Fore", new Dictionary<string, Color>());
            ColorScheme.Add("Back", new Dictionary<string, Color>());
            Load();
        }

        public bool Load(string theme = null)
        {
            if (theme != null) throw new NotImplementedException();
            SetDefault();
            return true;
        }

        public void SetDefault()
        {
            bool DarkMode;
            if (Main.FeatureConfig["AdaptiveDarkMode"])
            {
                DarkMode = FormMethods.IsWindowsInDarkMode();
            }
            else
            {
                DarkMode = false;
            }
            
            if (DarkMode)
            {
                ColorScheme["Fore"]["Text"] = SystemColors.Window;
                ColorScheme["Back"]["Button"] = SystemColors.ControlDarkDark;
                ColorScheme["Back"]["Control"] = SystemColors.ControlDarkDark;
                ColorScheme["Back"]["Window"] = SystemColors.ControlDarkDark;
            }
            else
            {
                ColorScheme["Fore"]["Text"] = SystemColors.ControlText;
                ColorScheme["Back"]["Button"] = SystemColors.ControlLight;
                ColorScheme["Back"]["Control"] = SystemColors.Control;
                ColorScheme["Back"]["Window"] = SystemColors.Window;
            }
        }

        public void Apply(Form form)
        {
            Dictionary<string, Control> Control = new Dictionary<string, Control>();

            foreach (Control c in form.Controls)
            {
                //Control btn_open = form.Controls[form.Controls.IndexOfKey("btn_open")];
                Control[c.Name] = c;
            }
            Control["form"] = form;
            Control["form"].BackColor = ColorScheme["Back"]["Control"];
            Control["form"].ForeColor = ColorScheme["Fore"]["Text"];

            /*
            string test = "";
            foreach (Control c in form.Controls)
            {
                test += " " + c.Name;
            }
            MessageBox.Show(test);
            */

            switch (form.GetType().Name)
            {
                case "AboutBox":
                    foreach (Control c in Control["tableLayoutPanel"].Controls)
                    {
                        c.BackColor = ColorScheme["Back"]["Control"];
                        c.ForeColor = ColorScheme["Fore"]["Text"];
                    }
                    break;
                case "Main":
                    Control["btn_open"].BackColor = Control["btn_advancedOptions"].BackColor = Control["btn_export"].BackColor = Control["btn_dump"].BackColor = ColorScheme["Back"]["Button"];
                    Control["statusStrip"].BackColor = Control["txt_metadata"].BackColor = ColorScheme["Back"]["Control"];
                    Control["statusStrip"].ForeColor = Control["txt_metadata"].ForeColor = ColorScheme["Fore"]["Text"];
                    Control["lst_exportExtensions"].ForeColor = ColorScheme["Fore"]["Text"];
                    Control["lst_exportExtensions"].BackColor = ColorScheme["Back"]["Window"];
                    Control["num_loopStart"].ForeColor = Control["num_loopEnd"].ForeColor = ColorScheme["Fore"]["Text"];
                    Control["num_loopStart"].BackColor = Control["num_loopEnd"].BackColor = ColorScheme["Back"]["Window"];
                    break;
                case "MainAdvanced":
                    foreach (Control ctrl in form.Controls)
                    {
                        if (!ctrl.Name.StartsWith("pnl_")) return;
                        foreach (Control c in ctrl.Controls)
                        {
                            c.BackColor = ColorScheme["Back"]["Control"];
                            c.ForeColor = ColorScheme["Fore"]["Text"];
                        }
                    }
                    break;
                case "MainDump":
                    Control["btn_options_saveExportInfo_fileLocation"].BackColor = Control["btn_options_dumpFileInfo_fileLocation"].BackColor = Control["btn_confirm"].BackColor = ColorScheme["Back"]["Button"];
                    Control["txt_options_dumpFileInfo_fileLocation"].BackColor = Control["txt_options_saveExportInfo_fileLocation"].BackColor = ColorScheme["Back"]["Control"];
                    Control["txt_options_dumpFileInfo_fileLocation"].ForeColor = Control["txt_options_saveExportInfo_fileLocation"].ForeColor = ColorScheme["Fore"]["Text"];
                    break;
                default:
                    break;
            }
        }
    }
}
