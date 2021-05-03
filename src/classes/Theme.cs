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

        public void Load()
        {
            bool DarkMode = false;
            if (Main.FeatureConfig["AdaptiveDarkMode"]) DarkMode = FormMethods.IsWindowsInDarkMode();

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

        public void Apply(Control control)
        {
            // Don't apply the light theme to prevent visual glitches (it is already applied anyway)
            if (!Main.FeatureConfig["AdaptiveDarkMode"] || !FormMethods.IsWindowsInDarkMode()) return;

            control.ForeColor = ColorScheme["Fore"]["Text"];
            string[] controlName = control.Name.Split('_');
            if (string.IsNullOrWhiteSpace(controlName[0])) return;

            switch (controlName[0])
            {
                case "btn":
                    control.BackColor = ColorScheme["Back"]["Button"];
                    break;
                case "lst":
                case "num":
                case "txt":
                    control.BackColor = ColorScheme["Back"]["Window"];
                    break;
                default:
                    control.BackColor = ColorScheme["Back"]["Control"];
                    break;
            }

            if (control.HasChildren)
            {
                foreach (Control c in control.Controls)
                {
                    Apply(c);
                }
            }
        }
    }
}
