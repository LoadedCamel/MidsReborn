using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase;

namespace mrbControls
{
    public partial class ctlEnhPicker : UserControl
    {
        public ctlEnhPicker()
        {
            InitializeComponent();
        }


        internal class PickerEnhImage : PictureBox
        {
            public bool IsPlaced { get; set; }
            public Enhancement Enhancement { get; set; }
        }

        private void setsType_Clicked(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Hello");
        }
    }
}
