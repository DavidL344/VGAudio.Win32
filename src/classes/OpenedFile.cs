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
        public Dictionary<string, string> ExportResult = new Dictionary<string, string>();
        public Dictionary<string, object> AdvancedExportInfo = new Dictionary<string, object>();
        private List<string> Warnings = new List<string>();
        private FileStream FileLock = null;
        public bool Initialized = false;

        // An equivalent of FileDialogProcess
        public OpenedFile(string filePath = null)
        {
            if (filePath != null) Open(filePath);
        }

        public bool Open(string filePath = null)
        {
            if (filePath == null) return false;
            if (!File.Exists(filePath)) throw new FileNotFoundException("The selected file no longer exists!");

            if (File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                throw new IOException("Batch conversions are not supported!");
            }
            if (!IsSupported(Path.GetExtension(filePath).Substring(1))) throw new IOException("The selected file is not supported!");

            // The file path has to be explicitly stated here - the entry Info["Path"] is defined upon successfully loading the file
            // If the program were to load the new Info["Path"] to memory, it would mismatch the currently opened file in the app
            if (IsLocked(filePath))
            {
                // Check if the locked file is in use by the app
                if (Info.ContainsKey("Path") && filePath == Info["Path"])
                {
                    return false;
                }
                else
                {
                    throw new IOException("The selected file is already in use by another process.");
                }
            }

            // Check if the file is valid
            string filePathEscaped = String.Format("\"{0}\"", filePath);
            string metadata = LoadCommand("-m " + filePathEscaped);
            if (metadata == null) throw new IOException("Unable to read the file!");
            if (Initialized) Close(Main.FeatureConfig["ResetExportOptionsOnNewFile"]);

            // File path information
            Info["Path"] = filePath;
            Info["PathEscaped"] = filePathEscaped;

            // File extension information
            Info["Extension"] = Path.GetExtension(filePath);
            Info["ExtensionNoDot"] = Info["Extension"].Substring(1);

            // File name information
            Info["Name"] = Path.GetFileName(filePath);
            Info["NameNoExtension"] = Path.GetFileNameWithoutExtension(filePath);
            Info["NameShort"] = TruncateName();
            Info["NameShort_OnError"] = TruncateName(16);

            // File Metadata
            Metadata["Full"] = null;
            Metadata["Short"] = null;
            Metadata["Status"] = null;
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
            ParseMetadata(metadata);

            // File export information
            ExportInfo["Path"] = null;
            ExportInfo["PathEscaped"] = null;
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

            // Result upon file export
            ExportResult["TimeElapsed"] = null;

            // Check again if the file is accessible
            // It could've been locked or deleted by something else in the meantime
            if (!IsLocked())
            {
                Lock(true);
                Initialized = true;
            }
            else
            {
                throw new EndOfStreamException("The selected file is inaccessible!");
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

            string channelSetup;
            switch (int.Parse(Metadata["ChannelCount"]))
            {
                case 0:
                    channelSetup = "no channels";
                    break;
                case 1:
                    channelSetup = "mono";
                    break;
                case 2:
                    channelSetup = "stereo";
                    break;
                default:
                    channelSetup = Metadata["ChannelCount"];
                    break;
            }

            Metadata["Status"] = String.Format("{0} | {1} | {2}", Metadata["EncodingFormat"], Metadata["SampleRate"], channelSetup);

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

        public string[] DumpInformation(Dictionary<string, Dictionary<string, object>> Options)
        {
            bool dumpExportInfo = (bool)Options["DumpFileInfo"]["IncludeExportInformation"];

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
                lineList.Add("Target file: " + ExportInfo["ExtensionNoDot"]);

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

                string conversionCommand = GenerateConversionParams(null, false);
                if (conversionCommand == null)
                {
                    conversionCommand = "(unable to generate)";
                }
                else
                {
                    conversionCommand = "VGAudioCli.exe " + conversionCommand;
                }
                lineList.Add("\r\nConversion command:\r\n" + conversionCommand);
            }
            lines = lineList.ToArray();
            return lines;
        }

        public bool IsSupported(string fileExtension = null)
        {
            if (fileExtension == null) fileExtension = Info["Extension"];
            return Main.extsArray.Contains(fileExtension.ToLower());
        }

        public string LoadCommand(string arguments = null, string workingDirectory = null)
        {
            if (!FormMethods.VerifyIntegrity()) return null;

            // If there's an opened file, use its location as a working directory
            if (workingDirectory == null)
            {
                if (Info.ContainsKey("Path"))
                {
                    workingDirectory = Path.GetDirectoryName(Info["Path"]);
                }
                else
                {
                    workingDirectory = Path.GetDirectoryName(Main.VGAudioCli);
                }
            }
            else
            {
                if (!Directory.Exists(workingDirectory)) workingDirectory = Path.GetDirectoryName(Main.VGAudioCli);
            }

            ProcessStartInfo procInfo = new ProcessStartInfo
            {
                FileName = Main.VGAudioCli,
                WorkingDirectory = workingDirectory,
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

        public string GenerateConversionParams(string exportLocation = null, bool silent = true, bool batchFormat = false)
        {
            if (exportLocation == null) exportLocation = String.Format("\"{0}\"", Path.ChangeExtension(Info["Path"], ExportInfo["Extension"]));
            string arguments = String.Format("-i {0} -o {1}", Info["PathEscaped"], exportLocation);
            GetWarnings(); // This won't be necessary once the controls use GetWarnings() on value change

            if (ExportLoop["Enabled"] == 1)
            {
                if (ExportLoop["Start"] > ExportLoop["End"])
                {
                    // The loop information is invalid - the file would contain a negative number of samples
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

            int chcp = 65001;
            if (!silent)
            {
                string commentCharacter = "";
                if (batchFormat) commentCharacter = ":: ";
                string warnings = "";
                if (Warnings.Count > 0)
                {
                    foreach (var warning in Warnings)
                    {
                        warnings += String.Format("\r\n{0}[WARNING] {1}", commentCharacter, warning.ToString());
                    }
                }
                else
                {
                    warnings = null;
                }

                string title = FormMethods.GetAppInfo("name") + " (" + FormMethods.GetAppInfo("version") + ")";
                if (batchFormat)
                {
                    arguments = String.Format(":: {0}\r\n:: Original file: {1}\r\n:: The converted file: {2}{3}\r\n\r\n@echo off\r\nchcp {4}>nul\r\nVGAudioCli.exe {5}\r\npause>nul", title, Info["Path"], exportLocation, warnings, chcp, arguments);
                }
                else
                {
                    if (warnings != null) warnings = String.Format("\r\n{0}", warnings);
                    arguments += warnings;
                }
            }
            else
            {
                if (batchFormat)
                {
                    arguments = String.Format("@echo off\r\nchcp {0}>nul\r\nVGAudioCli.exe {1}\r\npause>nul", chcp, arguments);
                }
            }
            return arguments;
        }

        public List<string> GetWarnings()
        {
            Warnings.Clear();
            if (ExportInfo["ExtensionNoDot"] == Info["ExtensionNoDot"])
                Warnings.Add("You're exporting to the original file extension. Some of the changes might not be applied.");
            
            switch (ExportInfo["ExtensionNoDot"])
            {
                case "bcstm":
                    Warnings.Add("The created BCSTM will most likely not work on a 3DS (ex. HOME Menu).");
                    break;
                case "wav":
                    if (ExportLoop["Enabled"] == 1)
                        Warnings.Add("While the wave file can hold loop information, it won't be read by most media players.");
                    break;
                default:
                    break;
            }

            if (ExportLoop["Enabled"] == 1)
            {
                if (ExportLoop["Start"] > ExportLoop["End"])
                {
                    int? sampleLength = (int)ExportLoop["End"] - (int)ExportLoop["Start"];
                    Warnings.Add(String.Format("The resulting file length is {0} samples due to an invalid loop points.", sampleLength));
                }
            }

            // Always up-to-date
            return Warnings;
        }

        public bool IsLocked(string filePath = null)
        {
            if (filePath == null) filePath = Info["Path"]; // Only used for early verification in Open()
            try
            {
                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                // https://stackoverflow.com/a/937558
                // If the file is unavailable, it is because:
                // - It's still being written to
                // - It's being processed by another thread
                // - It doesn't exist
                return true;
            }
            return false;
        }

        public bool Lock(bool lockTheFile)
        {
            if (Main.FeatureConfig.ContainsKey("LockOpenedFile") && Main.FeatureConfig["LockOpenedFile"])
            {
                if (Info.ContainsKey("Path") && File.Exists(Info["Path"]))
                {
                    if (lockTheFile)
                    {
                        if ((!IsLocked()) && (FileLock == null))
                        {
                            // https://stackoverflow.com/a/3279183
                            FileLock = File.Open(Info["Path"], FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                            return true;
                        }
                    }
                    else
                    {
                        if ((IsLocked()) && (FileLock != null))
                        {
                            FileLock.Close();
                            FileLock = null;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string TruncateName(int maxLength = 20)
        {
            string fileExtInfo = "... (" + Info["Extension"] + ")";

            // The trim length shouldn't be bigger than the length of the file name itself
            if (fileExtInfo.Length > maxLength) return fileExtInfo;
            int trimLength = maxLength - fileExtInfo.Length;

            if (Info["Name"].Length > maxLength)
            {
                if (Info["Name"].Length > trimLength)
                {
                    string truncate = FormMethods.Truncate(Info["Name"], trimLength);
                    if (Info["Name"] != truncate)
                    {
                        return truncate + fileExtInfo;
                    }
                }
            }
            return Info["Name"];
        }

        public bool Convert()
        {
            if (!File.Exists(Info["Path"])) throw new FileNotFoundException("The opened file no longer exists!");

            // Check if the export file extension is the correct one
            if (Path.GetExtension(ExportInfo["Path"]).ToLower() != ExportInfo["Extension"].ToLower())
            {
                // Error occurs when the user replaces an existing file
                // with invalid export extension through the dialog box
                throw new ArgumentException("The file extension selected is invalid!");
            }

            string arguments = GenerateConversionParams(ExportInfo["PathEscaped"]);
            if (string.IsNullOrEmpty(arguments)) throw new Exception("Internal Error: No parameters!");
            if (!FormMethods.VerifyIntegrity()) return false;
            Lock(false); // Unlock the file

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

            string line = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                line += proc.StandardOutput.ReadLine() + "\r\n";
            }
            string[] standardConsoleOutput = line.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            Lock(true); // Relock the file

            // Invalid parameter passed to the CLI
            if (proc.ExitCode != 0) throw new ArgumentException(standardConsoleOutput[0]);

            // Progress bar [####   ] starts with '[' and success starts with, well, 'Success!'
            if (!standardConsoleOutput[0].StartsWith("[") && !standardConsoleOutput[0].StartsWith("Success!"))
                throw new NotSupportedException(standardConsoleOutput[0]);

            // Get the time elapsed during the conversion
            string timeElapsed = standardConsoleOutput[1].Substring(standardConsoleOutput[1].IndexOf(":") + 1);
            ExportResult["TimeElapsed"] = TimeSpan.FromSeconds(System.Convert.ToDouble(timeElapsed)).ToString(@"hh\:mm\:ss\.fff");
            return true;
        }

        public void Close(bool resetExportOptions = true)
        {
            Lock(false);

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
        }
    }
}
