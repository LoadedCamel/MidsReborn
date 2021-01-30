using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.My;
using mrbBase;
using mrbBase.Base.Master_Classes;
using Syncfusion.Windows.Forms.Tools;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDBConvert : Form
    {
        private readonly frmMain _myParent;
        private string SourcePath { get; set; }
        private string DestinationPath { get; set; }
        private List<string> SourceFiles { get; set; }
        private List<string> DestinationFiles { get; set; }


        public frmDBConvert(ref frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            Load += On_Load;
            InitializeComponent();
            _myParent = iParent;
            SourcePath = MidsContext.Config.SourceDataPath;
            DestinationPath = MidsContext.Config.ConversionDataPath;
            SourceFiles = new List<string>();
            DestinationFiles = new List<string>();
        }

        private void On_Load(object sender, EventArgs e)
        {
            Text = @"Database Converter";
            CenterToParent();
        }

        private bool GenerateDestinationFiles()
        {
            DestinationFiles = new List<string>();
            if (SourceFiles != null)
            {
                foreach (var file in SourceFiles)
                {
                    var path = Regex.Replace(file, @"(.*)\\", $"{DestinationPath}\\");
                    if (!path.Contains("Levels"))
                    {
                        if (path.Contains("AttribMod"))
                        {
                            path = path.Replace(".mhd", ".json");
                        }
                        DestinationFiles.Add(path);
                    }
                }

                if (!DestinationFiles.Contains("GlobalMods") && DestinationFiles.Count > 0)
                {
                    DestinationFiles.Add($"{DestinationPath}\\GlobalMods.mhd");
                }
            }

            return DestinationFiles.Count > 0;
        }

        private void srcBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog sourceDialog = new FolderBrowserDialog {SelectedPath = Application.StartupPath};
            DialogResult result = sourceDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(sourceDialog.SelectedPath))
            {
                SourcePath = sourceDialog.SelectedPath;
                sourceFolder.Text = SourcePath;
                MidsContext.Config.SourceDataPath = SourcePath;
                SourceFiles = Directory.GetFiles(SourcePath, "*.mhd").ToList();
            }
        }

        private void destBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog destinationDialog = new FolderBrowserDialog {SelectedPath = Application.StartupPath};
            DialogResult result = destinationDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(destinationDialog.SelectedPath))
            {
                DestinationPath = destinationDialog.SelectedPath;
                destinationFolder.Text = DestinationPath;
                MidsContext.Config.ConversionDataPath = DestinationPath;
            }
        }

        private void convertBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SourcePath) && !string.IsNullOrWhiteSpace(DestinationPath))
            {
                MidsContext.Config.ConvertOldDb = true;
                MessageBox.Show(@"The application will now restart in order to prep conversion.");
                MidsContext.Config.SaveConfig(MyApplication.GetSerializer());
                Application.Restart();
            }
        }
    }
}
