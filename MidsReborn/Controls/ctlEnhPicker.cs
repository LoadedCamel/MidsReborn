using System.Diagnostics;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.Controls
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
