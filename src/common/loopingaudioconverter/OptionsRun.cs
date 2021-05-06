using LoopingAudioConverter;
using LoopingAudioConverter.Brawl;
using RSTMLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGAudio.Win32
{
		class OptionsRun
		{
			public IAudioImporter importer;
			public IAudioExporter exporter;
			public void Go()
			{
			
			



				exporter = new RSTMExporter();

			PCM16Audio w = new PCM16Audio(2, 44100, new short[] { -32, -1 });
			byte[] data = RSTMConverter.EncodeToByteArray(new PCM16AudioStream(w), null);
			File.WriteAllBytes(Path.Combine(@"C:\Users\David\Desktop", "test.brstm"), data);
		}
		}

}
