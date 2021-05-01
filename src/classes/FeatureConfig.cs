using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGAudio.Win32
{
    class FeatureConfig
    {
        public Dictionary<string, bool> Value = new Dictionary<string, bool>();
        public Dictionary<string, string[]> Info = new Dictionary<string, string[]>();

        public FeatureConfig(string file = null)
        {
            if (!string.IsNullOrWhiteSpace(file))
                Load(file);
            else
                SetDefaults();
        }

        public bool Load(string file)
        {
            if (File.Exists(file))
            {
                Reset();
                throw new NotImplementedException();
            }
            return false;
        }

        public void Reset()
        {
            Value.Clear();
            Info.Clear();
        }

        public void SetDefaults()
        {
            Reset();

            // A button that requires an extra click to close the file first before opening a new one
            AddEntry("OpenCloseWinformsButton", "Import button closes files", "When importing a file using a dialog box, require closing any opened files first.", true);

            // Close button closes the file on the first click instead of closing the app
            AddEntry("CloseButtonClosesFile", "Close button closes files", "Use the close button of the app to close any opened files.", true);

            // Prefill export file name and extension
            AddEntry("PrefillExportFileName", "Prefill export info", "Prefill file name and extension in the export dialog box.", true);

            // Lock the opened file so that it can't be moved or deleted
            AddEntry("LockOpenedFile", "Lock the opened file", "Prevent the opened file from getting deleted.", true);

            // Reset the loop information and the advanced options after the file is closed
            AddEntry("ResetExportOptionsOnNewFile", "Always reset export options", "Reset export information upon opening a new file.", true);

            // Show the time elapsed after finishing the conversion
            AddEntry("ShowTimeElapsed", "Show time elapsed", "Show the time elapsed after finishing the conversion.", true);

            // Sync the audio bitrate between tabs in advanced settings
            AddEntry("SyncBitrateTabs", "Sync bitrate between tabs", "Sync the audio bitrate between tabs in advanced settings", true);
        }

        private void AddEntry(string variable, string name, string description = null, bool defaultValue = true)
        {
            Value.Add(variable, defaultValue);
            Info.Add(variable, new string[] { name, description });
        }
    }
}
