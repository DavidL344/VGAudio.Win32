using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGAudio.Win32
{
    class OpenedFile
    {
        public Dictionary<string, string> Info = new Dictionary<string, string>();
        public Dictionary<string, string> DisplayInfo = new Dictionary<string, string>();
        public Dictionary<string, string> Metadata = new Dictionary<string, string>();
        public Dictionary<string, int?> Loop = new Dictionary<string, int?>();
        public Dictionary<string, string> ExportInfo = new Dictionary<string, string>();
        public Dictionary<string, int?> ExportLoop = new Dictionary<string, int?>();
        public Dictionary<string, object> AdvancedExportInfo = new Dictionary<string, object>();
        public bool Initialized = false;

        // An equivalent of FileDialogProcess
        public OpenedFile(string filePath)
        {
            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                throw new Exception("Batch conversions are not supported!");
            }
            if (!IsSupported()) throw new Exception("The selected file is not supported!");

            // TODO: Change to IsLocked()
            if (FormMethods.IsFileLocked(filePath))
            {
                // Check if the locked file is in use by the app
                if (filePath == Info["Path"])
                {
                    if (Main.FeatureConfig.ContainsKey("LockOpenedFile") && Main.FeatureConfig["LockOpenedFile"])
                    {
                        // TODO: "The selected file is already loaded."
                    }
                    return;
                }
                else
                {
                    throw new Exception("The selected file is already in use by another process.");
                }
            }
            Initialized = false;

            // File path information
            Info.Add("Path", filePath);
            Info.Add("PathEscaped", String.Format("\"{0}\"", filePath));

            // File name information
            Info.Add("Name", Path.GetFileName(filePath));
            Info.Add("NameNoExtension", Path.GetFileNameWithoutExtension(filePath));

            // File extension information
            Info.Add("Extension", Path.GetExtension(filePath));
            Info.Add("ExtensionNoDot", Info["Extension"].Substring(1));

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

            // Parse file metadata and loop information
            ParseMetadata(LoadCommand("-m " + Info["PathEscaped"]));

            // File export information
            ExportInfo.Add("Extension", null);
            ExportInfo.Add("ExtensionNoDot", null);

            // File export loop information
            ExportLoop.Add("LoopEnabled", null);
            ExportLoop.Add("StartMin", null);
            ExportLoop.Add("Start", null);
            ExportLoop.Add("StartMax", null);
            ExportLoop.Add("EndMin", null);
            ExportLoop.Add("End", null);
            ExportLoop.Add("EndMax", null);

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

            Initialized = true;
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
            bool exportLoop = bool.Parse(ExportLoop["Enabled"].ToString());

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

                if (exportLoop)
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

                if (exportLoop && ExportInfo["ExtensionNoDot"] == "wav")
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

        public bool IsSupported()
        {
            return Main.extsArray.Contains(Info["Extension"].ToLower());
        }

        public string LoadCommand(string arguments = null)
        {
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
        public string GenerateConversionParams(bool silent = false)
        {
            string arguments = String.Format("-i {0} -o {1}", Info["PathEscaped"], "output." + ExportInfo["ExtensionNoDot"]);
            
            if (Loop["Enabled"] == 1)
            {
                if (Loop["Start"] > Loop["End"])
                {
                    // TODO: ask to continue since the export information cannot be saved
                    arguments += " --no-loop";
                }
                else
                {
                    arguments += String.Format(" -l {0}-{1}", Loop["Start"], Loop["End"]);
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
    }
}
