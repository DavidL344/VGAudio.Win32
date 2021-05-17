using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGAudio.Win32.Config
{
    class FeatureConfig
    {
        public static Dictionary<string, bool> GetValues()
        {
            Dictionary<string, bool> Values = new Dictionary<string, bool>();
            Values.Clear();

            // A button that requires an extra click to close the file first before opening a new one
            Values.Add("OpenCloseWinformsButton", true);

            // Close button closes the file on the first click instead of closing the app
            Values.Add("CloseButtonClosesFile", true);

            // Prefill export file name and extension
            Values.Add("PrefillExportFileName", true);

            // Lock the opened file so that it can't be moved or deleted
            Values.Add("LockOpenedFile", true);

            // Reset the loop information and the advanced options after the file is closed
            Values.Add("ResetExportOptionsOnNewFile", true);

            // Show the time elapsed after finishing the conversion
            Values.Add("ShowTimeElapsed", true);

            // Sync the audio bitrate between tabs in advanced settings
            Values.Add("SyncBitrateTabs", true);

            // Enable adaptive dark mode
            Values.Add("AdaptiveDarkMode", true);

            // Enable window resizing
            Values.Add("AllowWindowResize", true);

            return Values;
        }
    }
}
