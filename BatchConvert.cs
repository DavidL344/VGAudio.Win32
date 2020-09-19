using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VGAudio.Win32
{
    public partial class BatchConvert : Form
    {
        private string[] files;
        public BatchConvert(string[] files)
        {
            InitializeComponent();
            this.files = files;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            MinimumSize = Size;
            MaximumSize = Size;
            MaximizeBox = false;
        }
    }
}
