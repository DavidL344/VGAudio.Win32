using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    switch (args[0])
                    {
                        case "--extract":
                            FormMethods.ExtractCli();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            string[] extsArray = { "wav", "dsp", "idsp", "brstm", "bcstm", "bfstm", "hps", "adx", "hca", "genh", "at9" };
            string extsFilter = FormMethods.CreateExtensionFilter(extsArray);
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Open file",
                DefaultExt = "",
                Filter = extsFilter,
                CheckFileExists = true,
                CheckPathExists = true,
                DereferenceLinks = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                OptionsRun optionsRun = new OptionsRun();
                optionsRun.Go(openFile.FileName);
                return;
            }
            Main(null);

            

            return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
