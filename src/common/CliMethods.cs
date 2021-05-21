using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGAudio.Win32
{
    class CliMethods
    {
        public static bool ExecuteCommand(string[] args)
        {
            if (args.Length == 0) return false;
            switch (args[0])
            {
                case "--extract":
                    FormMethods.ExtractCli(true);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
