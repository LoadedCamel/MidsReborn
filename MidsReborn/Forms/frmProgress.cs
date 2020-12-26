using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmProgress : Form
    {
        public frmProgress()
        {
            InitializeComponent();
            ShowInTaskbar = true;
        }

        public string WindowTitle
        {
            get => Text;
            set => Text = value;
        }

        public int Value
        {
            get => progressBar1.Value;
            set => progressBar1.Value = value;
        }

        public string OperationText
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        public ProgressBar Bar => progressBar1;
        public Label Label => label1;

        public override string Text
        {
            get => base.Text;
            set
            {
                //Debug.WriteLine("Value: " + string.Join(", ", value));
                if (!value.Contains("|"))
                {
                    base.Text = value;
                }

                string[] chunks = value.Split('|');
                //Debug.WriteLine("Chunks: " + string.Join(", ", chunks) + " (L: " + chunks.Length + ")");
                switch (chunks.Length)
                {
                    case 3:
                    {
                        if (!string.IsNullOrWhiteSpace(chunks[0])) base.Text = chunks[0];
                        progressBar1.Value = Convert.ToInt32(chunks[1], null);
                        label1.Text = chunks[2];
                        break;
                    }
                    case 2:
                    {
                        if (!string.IsNullOrWhiteSpace(chunks[0])) base.Text = chunks[0];
                        progressBar1.Value = Convert.ToInt32(chunks[1], null);
                        break;
                    }
                    default:
                    {
                        if (!string.IsNullOrWhiteSpace(chunks[0])) base.Text = chunks[0];
                        break;
                    }
                }
            }
        }
    }
}
