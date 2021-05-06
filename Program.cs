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

            OptionsRun optionsRun = new OptionsRun();
            optionsRun.Go();

            return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
