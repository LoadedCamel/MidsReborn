using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class PatchNotes : Form
    {
        private readonly frmMain _parent;
        private bool ReadNotes { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        private string ChangeLog { get; set; }
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
            if (stream == null) return 0;
            using var reader = new StreamReader(stream);
            var i = 0;
            string result;
            while ((result = reader.ReadLine()) != null)
            {
                i++;
                if (!result.Contains($"## {Type} [{Version}]")) continue;
                var startLine = i + 1;
                return startLine;
            }

            return 0;
        }

        private void PatchNotes_LoadEvent(object sender, EventArgs e)
        {
            Console.WriteLine(Type);
            switch (Type)
            {
                case "App":
                    ChangeLog = MidsContext.Config.AppChangeLog;
                    Text = $@"Viewing ""MRB {Version}"" Patch Notes";
                    break;
                case "Database":
                    ChangeLog = MidsContext.Config.DbChangeLog;
                    Text = $@"Viewing ""{Type} {Version}"" Patch Notes";
                    break;
            }
            if (ReadNotes)
            {
                closeUpdate.Text = @"UPDATE";
            }

            var startLine = GetStartingLine(ChangeLog);
            richTextBox1.ForeColor = Color.AliceBlue;
            using var client = new WebClient();
            var stream = client.OpenRead(ChangeLog);
            if (stream == null) return;
            using var reader = new StreamReader(stream);
            var builder = new StringBuilder(richTextBox1.Text);
            var i = 0;
            string result;

            while ((result = reader.ReadLine()) != null)
            {
                i++;
                if (i < startLine) continue;
                var regMatch = new Regex("(##.*\\[.*\\])");
                if (regMatch.IsMatch(result))
                {
                    break;
                }

                builder.AppendFormat($"{result}\r\n");
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
                switch(Type)
                {
                    case "App":
                        clsXMLUpdate.Update(MidsContext.Config.UpdatePath, Version);
                        break;
                    case "Database":
                        clsXMLUpdate.Update(DatabaseAPI.Database.UpdateManifest, Version);
                        break;
                }
                Close();
            }
            else
            {
                Close();
            }
        }
    }
}
