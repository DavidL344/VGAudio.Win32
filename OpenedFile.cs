using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public class OpenedFile
    {
        public Dictionary<string, string> Info = new Dictionary<string, string>();
        public Dictionary<string, string> Metadata = new Dictionary<string, string>();
        public Dictionary<string, int?> Loop = new Dictionary<string, int?>();
        public Dictionary<string, string> ExportInfo = new Dictionary<string, string>();
        public Dictionary<string, int?> ExportLoop = new Dictionary<string, int?>();
        public Dictionary<string, object> AdvancedExportInfo = new Dictionary<string, object>();
        public bool Initialized = false;

        // An equivalent of FileDialogProcess
        public OpenedFile(string filePath = null)
        {
            if (filePath != null) Open(filePath);
        }

        public bool Open(string filePath = null)
        {
            if (filePath == null) return false;
            if (!File.Exists(filePath)) throw new Exception("The selected file no longer exists!");

            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                throw new Exception("Batch conversions are not supported!");
            }
            if (!IsSupported(Path.GetExtension(filePath).Substring(1))) throw new Exception("The selected file is not supported!");

            // TODO: Change to IsLocked()
            if (FormMethods.IsFileLocked(filePath))
            {
                // Check if the locked file is in use by the app
                if (Info.ContainsKey("Path") && filePath == Info["Path"])
                {
                    if (Main.FeatureConfig.ContainsKey("LockOpenedFile") && Main.FeatureConfig["LockOpenedFile"])
                    {
                        MessageBox.Show("The selected file is already loaded.", FormMethods.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("The selected file is already in use by another process.", FormMethods.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            Initialized = false;

            // File path information
            Info["Path"] = filePath;
            Info["PathEscaped"] = String.Format("\"{0}\"", filePath);

            // File extension information
            Info["Extension"] = Path.GetExtension(filePath);
            Info["ExtensionNoDot"] = Info["Extension"].Substring(1);

            // File name information
            Info["Name"] = Path.GetFileName(filePath);
            Info["NameNoExtension"] = Path.GetFileNameWithoutExtension(filePath);
            Info["NameShort"] = FormMethods.TruncateFileName(Info["Name"], Info["ExtensionNoDot"]);

            // File Metadata
            Metadata["Full"] = null;
            Metadata["Short"] = null;
            Metadata["EncodingFormat"] = null;
            Metadata["SampleCount"] = null;
            Metadata["SampleRate"] = null;
            Metadata["ChannelCount"] = null;

            // File loop information
            Loop["Enabled"] = null;
            Loop["StartMin"] = null;
            Loop["Start"] = null;
            Loop["StartMax"] = null;
            Loop["EndMin"] = null;
            Loop["End"] = null;
            Loop["EndMax"] = null;

            // File export loop information
            ExportLoop["Enabled"] = null;
            ExportLoop["StartMin"] = null;
            ExportLoop["Start"] = null;
            ExportLoop["StartMax"] = null;
            ExportLoop["EndMin"] = null;
            ExportLoop["End"] = null;
            ExportLoop["EndMax"] = null;

            // Parse file metadata and loop information
            string metadata = LoadCommand("-m " + Info["PathEscaped"]);
            if (metadata == null) throw new Exception("Unable to read the file!");
            ParseMetadata(metadata);

            // File export information
            ExportInfo["Extension"] = null;
            ExportInfo["ExtensionNoDot"] = null;

            // Advanced file export information
            AdvancedExportInfo["Apply"] = false;
            AdvancedExportInfo["ADX_encrypt"] = false;
            AdvancedExportInfo["ADX_type"] = null;
            AdvancedExportInfo["ADX_keystring_use"] = false;
            AdvancedExportInfo["ADX_keystring"] = null;
            AdvancedExportInfo["ADX_keycode_use"] = false;
            AdvancedExportInfo["ADX_keycode"] = null;
            AdvancedExportInfo["ADX_filter_use"] = false;
            AdvancedExportInfo["ADX_filter"] = null;
            AdvancedExportInfo["ADX_version_use"] = false;
            AdvancedExportInfo["ADX_version"] = null;
            AdvancedExportInfo["BRSTM_audioFormat"] = null;
            AdvancedExportInfo["HCA_audioRadioButtonSelector"] = null;
            AdvancedExportInfo["HCA_audioQuality"] = null;
            AdvancedExportInfo["HCA_audioBitrate"] = null;
            AdvancedExportInfo["HCA_limitBitrate"] = null;

            // Check again if the file is accessible
            // It could've been locked or deleted by something else in the meantime
            if (!FormMethods.IsFileLocked(Info["Path"]))
            {
                FormMethods.FileLock(Info["Path"]);
                Initialized = true;
            }
            else
            {
                throw new Exception("The selected file is inaccessible!");
            }
            return true;
        }

        public bool UpdateExportInformation(string key, string value)
        {
            ExportInfo[key] = value;

            switch (key)
            {
                case "Extension":
                    ExportInfo["ExtensionNoDot"] = ExportInfo[key].Substring(1);
                    break;
                default:
                    break;
            }
            return true;
        }

        public bool ParseMetadata(string metadata)
        {
            // Parse the file information - if it fails, close the file
            Metadata["Full"] = metadata;
            Metadata["EncodingFormat"] = FormMethods.GetBetween(metadata, "Encoding format: ", "\r\n");
            Metadata["SampleCount"] = FormMethods.GetBetween(metadata, "Sample count: ", " (");
            Metadata["SampleRate"] = FormMethods.GetBetween(metadata, "Sample rate: ", "\r\n");
            Metadata["ChannelCount"] = FormMethods.GetBetween(metadata, "Channel count: ", "\r\n");
            Metadata["Short"] = Metadata["EncodingFormat"] + "\r\nSample Rate: " + Metadata["SampleRate"] + "\r\nChannel Count: " + Metadata["ChannelCount"];

            // Check if the file has a valid sample count
            if (!int.TryParse(Metadata["SampleCount"], out int mSampleCount))
            {
                // The file contains invalid sample count
                return false;
            }

            // Parse the loop information
            string mLoopStartVar = FormMethods.GetBetween(metadata, "Loop start: ", " samples");
            string mLoopEndVar = FormMethods.GetBetween(metadata, "Loop end: ", " samples");

            // Note: Loop["StartMax"] and Loop["EndMin"] are valid only upon loading the file
            // These two variables should be updated dynamically directly in the form to prevent overlapping
            if (int.TryParse(mLoopStartVar, out int mLoopStart) && int.TryParse(mLoopEndVar, out int mLoopEnd))
            {
                Loop["Enabled"] = 1;

                Loop["Start"] = mLoopStart; // Save the loop start
                Loop["End"] = mLoopEnd; // Save the loop start
                Loop["EndMax"] = mSampleCount; // Makes sure the user doesn't input more samples than the file has
            }
            else
            {
                Loop["Enabled"] = 0;
                Loop["Start"] = 0;

                // If there's no loop, the loop end is set as the end of the file by default
                Loop["EndMax"] = Loop["End"] = mSampleCount;
            }

            // Set default loop information for the imported file
            Loop["StartMin"] = 0; // The loop start value cannot be lower than the beginning of the file
            Loop["StartMax"] = Loop["End"] - 1; // Makes sure the user can only input lower number than the loop's end
            Loop["EndMin"] = Loop["Start"] + 1; // Loop end has to be a bigger number than loop start

            // These values can be modified and will be used upon export
            ExportLoop["Enabled"] = Loop["Enabled"];
            ExportLoop["StartMin"] = Loop["StartMin"];
            ExportLoop["Start"] = Loop["Start"];
            ExportLoop["StartMax"] = Loop["StartMax"];
            ExportLoop["EndMin"] = Loop["EndMin"];
            ExportLoop["End"] = Loop["End"];
            ExportLoop["EndMax"] = Loop["EndMax"];
            return true;
        }

        public string[] DumpInformation(bool dumpExportInfo)
        {
            string[] lines;
            int? exportLoop = ExportLoop["Enabled"];

            List<string> lineList = new List<string>
            {
                FormMethods.GetAppInfo("name") + " (" + FormMethods.GetAppInfo("version") + ")",
                "Dumped Info (" + Info["Name"] + ")\r\n",
                Metadata["Full"]
            };

            if (dumpExportInfo)
            {
                lineList.Add("----------\r\n");
                lineList.Add("Custom Export Info:");
                lineList.Add("Target file: " + ExportInfo["Extension"]);

                if (exportLoop == 1)
                {
                    lineList.Add("Loop start: " + ExportLoop["Start"]);
                    lineList.Add("Loop end: " + ExportLoop["End"]);
                }

                if ((bool)AdvancedExportInfo["Apply"])
                {
                    switch (ExportInfo["ExtensionNoDot"])
                    {
                        case "adx":
                            if ((bool)AdvancedExportInfo["ADX_encrypt"])
                            {
                                lineList.Add("Encoding type: " + AdvancedExportInfo["ADX_type"]);
                                if ((bool)AdvancedExportInfo["ADX_keystring_use"])
                                    lineList.Add("Keystring: " + AdvancedExportInfo["ADX_keystring"]);
                                if ((bool)AdvancedExportInfo["ADX_keycode_use"])
                                    lineList.Add("Keycode: " + AdvancedExportInfo["ADX_keycode"]);
                                if ((bool)AdvancedExportInfo["ADX_filter_use"])
                                    lineList.Add("Encoding filter: " + AdvancedExportInfo["ADX_filter"]);
                                if ((bool)AdvancedExportInfo["ADX_version_use"])
                                    lineList.Add("Header version: " + AdvancedExportInfo["ADX_version"]);
                            }
                            break;
                        case "brstm":
                            lineList.Add("Audio format: " + AdvancedExportInfo["BRSTM_audioFormat"]);
                            break;
                        case "hca":
                            switch (AdvancedExportInfo["HCA_audioRadioButtonSelector"])
                            {
                                case "quality":
                                    lineList.Add("Audio quality: " + AdvancedExportInfo["HCA_audioQuality"]);
                                    break;
                                case "bitrate":
                                    lineList.Add("Audio bitrate: " + AdvancedExportInfo["HCA_audioBitrate"]);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }

                string conversionCommand = GenerateConversionParams();
                if (conversionCommand == null)
                {
                    conversionCommand = "(unable to generate)";
                }
                else
                {
                    conversionCommand = "VGAudioCli.exe " + conversionCommand;
                }

                if (exportLoop == 1 && ExportInfo["ExtensionNoDot"] == "wav")
                {
                    lineList.Add("\r\nConversion command (see the warning below):\r\n" + conversionCommand);
                    lineList.Add("\r\n[WARNING] While the wave file can hold loop information, it won't be read by most media players.");
                }
                else
                {
                    lineList.Add("\r\nConversion command:\r\n" + conversionCommand);
                }
            }
            lines = lineList.ToArray();
            return lines;
        }

        public bool IsSupported(string fileExtension = null)
        {
            if (fileExtension == null) fileExtension = Info["Extension"];
            return Main.extsArray.Contains(fileExtension.ToLower());
        }

        public string LoadCommand(string arguments = null)
        {
            if (!FormMethods.VerifyIntegrity()) return null;
            ProcessStartInfo procInfo = new ProcessStartInfo
            {
                FileName = Main.VGAudioCli,
                WorkingDirectory = Path.GetDirectoryName(Info["Path"]),
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var proc = Process.Start(procInfo);
            proc.WaitForExit();
            if (proc.ExitCode == 0)
            {
                return proc.StandardOutput.ReadToEnd();
            }
            else
            {
                return null;
            }
        }
        public string GenerateConversionParams(string exportLocation = null, bool silent = false)
        {
            if (exportLocation == null) exportLocation = "output." + ExportInfo["ExtensionNoDot"];
            string arguments = String.Format("-i {0} -o {1}", Info["PathEscaped"], exportLocation);
            
            if (ExportLoop["Enabled"] == 1)
            {
                if (ExportLoop["Start"] > ExportLoop["End"])
                {
                    // TODO: ask to continue since the export information cannot be saved
                    arguments += " --no-loop";
                }
                else
                {
                    arguments += String.Format(" -l {0}-{1}", ExportLoop["Start"], ExportLoop["End"]);
                }
            }
            else
            {
                arguments += " --no-loop";
            }

            if ((bool)AdvancedExportInfo["Apply"])
            {
                switch (ExportInfo["ExtensionNoDot"])
                {
                    case "adx":
                        if ((bool)AdvancedExportInfo["ADX_encrypt"])
                        {
                            switch (AdvancedExportInfo["ADX_type"])
                            {
                                case "Linear":
                                case "Fixed":
                                    arguments += " --adxtype " + AdvancedExportInfo["ADX_type"];
                                    break;
                                case "Exponential":
                                    arguments += " --adxtype Exp";
                                    break;
                                default:
                                    break;
                            }

                            if ((bool)AdvancedExportInfo["ADX_keystring_use"])
                            {
                                if (AdvancedExportInfo.TryGetValue("ADX_keystring", out object keystring))
                                {
                                    arguments += " --keystring " + keystring;
                                }
                            }

                            if ((bool)AdvancedExportInfo["ADX_keycode_use"])
                            {
                                if (AdvancedExportInfo.TryGetValue("ADX_keycode", out object keycode))
                                {
                                    arguments += " --keycode " + keycode;
                                }
                            }

                            switch (AdvancedExportInfo["ADX_filter"])
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                    arguments += " --filter " + AdvancedExportInfo["ADX_filter"];
                                    break;
                                default:
                                    break;
                            }

                            switch (AdvancedExportInfo["ADX_version"])
                            {
                                case 3:
                                case 4:
                                    arguments += " --version " + AdvancedExportInfo["ADX_version"];
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "brstm":
                        switch (AdvancedExportInfo["BRSTM_audioFormat"])
                        {
                            case "DSP-ADPCM":
                                // If not specified, the file is converted to DSP-ADPCM audio format
                                break;
                            case "16-bit PCM":
                                arguments += " -f pcm16";
                                break;
                            case "8-bit PCM":
                                arguments += " -f pcm8";
                                break;
                            default:
                                break;
                        }
                        break;
                    case "hca":
                        switch (AdvancedExportInfo["HCA_audioRadioButtonSelector"])
                        {
                            case "quality":
                                arguments += " --hcaquality " + AdvancedExportInfo["HCA_audioQuality"];
                                break;
                            case "bitrate":
                                arguments += " --bitrate " + AdvancedExportInfo["HCA_audioBitrate"];
                                break;
                            default:
                                break;
                        }
                        if ((bool)AdvancedExportInfo["HCA_limitBitrate"]) arguments += " --limit-bitrate";
                        break;
                    default:
                        break;
                }
            }
            return arguments;
        }

        public void LoadAdvancedExportInfo()
        {
            AdvancedExportInfo["Apply"] = Main.AdvancedSettings["Apply"];
            AdvancedExportInfo["ADX_encrypt"] = Main.AdvancedSettings["ADX_encrypt"];
            AdvancedExportInfo["ADX_type"] = Main.AdvancedSettings["ADX_type"];
            AdvancedExportInfo["ADX_keystring_use"] = Main.AdvancedSettings["ADX_keystring_use"];
            AdvancedExportInfo["ADX_keystring"] = Main.AdvancedSettings["ADX_keystring"];
            AdvancedExportInfo["ADX_keycode_use"] = Main.AdvancedSettings["ADX_keycode_use"];
            AdvancedExportInfo["ADX_keycode"] = Main.AdvancedSettings["ADX_keycode"];
            AdvancedExportInfo["ADX_filter_use"] = Main.AdvancedSettings["ADX_filter_use"];
            AdvancedExportInfo["ADX_filter"] = Main.AdvancedSettings["ADX_filter"];
            AdvancedExportInfo["ADX_version_use"] = Main.AdvancedSettings["ADX_version_use"];
            AdvancedExportInfo["ADX_version"] = Main.AdvancedSettings["ADX_version"];
            AdvancedExportInfo["BRSTM_audioFormat"] = Main.AdvancedSettings["BRSTM_audioFormat"];
            AdvancedExportInfo["HCA_audioRadioButtonSelector"] = Main.AdvancedSettings["HCA_audioRadioButtonSelector"];
            AdvancedExportInfo["HCA_audioQuality"] = Main.AdvancedSettings["HCA_audioQuality"];
            AdvancedExportInfo["HCA_audioBitrate"] = Main.AdvancedSettings["HCA_audioBitrate"];
            AdvancedExportInfo["HCA_limitBitrate"] = Main.AdvancedSettings["HCA_limitBitrate"];
        }

        public void Close(bool resetExportOptions = true)
        {
            Info.Clear();
            Metadata.Clear();
            Loop.Clear();

            if (resetExportOptions)
            {
                ExportLoop.Clear();
                ExportInfo.Clear();
                AdvancedExportInfo.Clear();
            }
            Initialized = false;

            /*
            // File path information
            Info.Add("Path", null);
            Info.Add("PathEscaped", null);

            // File extension information
            Info.Add("Extension", null);
            Info.Add("ExtensionNoDot", null);

            // File name information
            Info.Add("Name", null);
            Info.Add("NameNoExtension", null);
            Info.Add("NameShort", null);

            // File Metadata
            Metadata.Add("Full", null);
            Metadata.Add("Short", null);
            Metadata.Add("EncodingFormat", null);
            Metadata.Add("SampleCount", null);
            Metadata.Add("SampleRate", null);
            Metadata.Add("ChannelCount", null);

            // File loop information
            Loop.Add("Enabled", null);
            Loop.Add("StartMin", null);
            Loop.Add("Start", null);
            Loop.Add("StartMax", null);
            Loop.Add("EndMin", null);
            Loop.Add("End", null);
            Loop.Add("EndMax", null);

            // File export loop information
            ExportLoop.Add("Enabled", null);
            ExportLoop.Add("StartMin", null);
            ExportLoop.Add("Start", null);
            ExportLoop.Add("StartMax", null);
            ExportLoop.Add("EndMin", null);
            ExportLoop.Add("End", null);
            ExportLoop.Add("EndMax", null);

            // File export information
            ExportInfo.Add("Extension", null);
            ExportInfo.Add("ExtensionNoDot", null);

            // Advanced file export information
            AdvancedExportInfo.Add("Apply", false);
            AdvancedExportInfo.Add("ADX_encrypt", false);
            AdvancedExportInfo.Add("ADX_type", null);
            AdvancedExportInfo.Add("ADX_keystring_use", false);
            AdvancedExportInfo.Add("ADX_keystring", null);
            AdvancedExportInfo.Add("ADX_keycode_use", false);
            AdvancedExportInfo.Add("ADX_keycode", null);
            AdvancedExportInfo.Add("ADX_filter_use", false);
            AdvancedExportInfo.Add("ADX_filter", null);
            AdvancedExportInfo.Add("ADX_version_use", false);
            AdvancedExportInfo.Add("ADX_version", null);
            AdvancedExportInfo.Add("BRSTM_audioFormat", null);
            AdvancedExportInfo.Add("HCA_audioRadioButtonSelector", null);
            AdvancedExportInfo.Add("HCA_audioQuality", null);
            AdvancedExportInfo.Add("HCA_audioBitrate", null);
            AdvancedExportInfo.Add("HCA_limitBitrate", null);
            */
        }
    }
}
