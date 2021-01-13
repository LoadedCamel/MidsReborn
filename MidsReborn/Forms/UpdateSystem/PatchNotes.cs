using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class PatchNotes : Form
    {
        private frmMain _parent;
        private bool ReadNotes { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public PatchNotes(frmMain parent, bool notes)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            ReadNotes = notes;
            Load += PatchNotes_LoadEvent;
            InitializeComponent();
            _parent = parent;
        }

        private int GetStartingLine(string src)
        {
            using var client = new WebClient();
            var stream = client.OpenRead(src);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                int i = 0;
                string result;
                while ((result = reader.ReadLine()) != null)
                {
                    i++;
                    if (result == "# [Released]")
                    {
                        var startLine = i + 1;
                        return startLine;
                    }
                }
            }

            return 0;
        }

        private void PatchNotes_LoadEvent(object sender, EventArgs e)
        {
            if (ReadNotes)
            {
                closeUpdate.Text = @"UPDATE";
            }

            int startLine = GetStartingLine("https://midsreborn.com/mids_updates/changelog.txt");
            richTextBox1.ForeColor = Color.AliceBlue;
            using var client = new WebClient();
            var stream = client.OpenRead("https://midsreborn.com/mids_updates/changelog.txt");
            if (stream == null) return;
            using var reader = new StreamReader(stream);
            var builder = new StringBuilder(richTextBox1.Text);
            int i = 0;
            string result;

            while ((result = reader.ReadLine()) != null)
            {
                i++;
                if (i > startLine)
                {
                    if (result.Contains($"## {Type} [{Version}]"))
                    {
                        builder.AppendFormat($"{result}\r\n");
                    }
                    else if (result.Contains($"## {Type}") && !result.Contains($"[{Version}]"))
                    {
                        break;
                    }
                    else
                    {
                        builder.AppendFormat($"{result}\r\n");
                    }
                }
            }

            richTextBox1.Text = builder.ToString();
        }

        private void closeUpdate_Click(object sender, EventArgs e)
        {
            Close_Update(ReadNotes);
        }

        private void Close_Update(bool update)
        {
            if (update)
            {
                Hide();
                var updater = new Updater(_parent);
                updater.ShowDialog(this);
                Close();
            }
            else
            {
                Close();
            }
        }
    }
}
