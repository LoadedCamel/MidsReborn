using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class PatchGen : Form
    {
        private PatchCompressor? _compressor;

        public PatchGen()
        {
            Load += OnLoad;
            InitializeComponent();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
            formPages1.SelectedIndex = MidsContext.Config.Mode switch
            {
                ConfigData.Modes.AppAdmin => 0,
                ConfigData.Modes.DbAdmin => 1,
                _ => formPages1.SelectedIndex
            };
        }

        private async void App_Click(object? sender, EventArgs e)
        {
            formPages1.SelectedIndex = 2;
            await RenameBootstrapperFiles();
            await Task.Delay(100);
            _compressor = PatchCompressor.AppPatchCompressor;
            _compressor.ProgressChanged += CompressorOnProgressChanged;
            await Task.Delay(100);
            await StartProcess();
        }

        private async void Database_Click(object? sender, EventArgs e)
        {
            formPages1.SelectedIndex = 2;
            _compressor = PatchCompressor.DbPatchCompressor;
            _compressor.ProgressChanged += CompressorOnProgressChanged;
            await Task.Delay(100);
            await StartProcess();
        }

        private async Task RenameBootstrapperFiles()
        {
            processLabel.Text = "Renaming Bootstrapper files...";
            await Task.Delay(100);
            var filesDir = Path.GetDirectoryName(Application.ExecutablePath);
            var files = Directory.EnumerateFiles(filesDir, "MRBBootstrap*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                try
                {
                    File.Move(file, file.Replace("MRBBootstrap", "New_MRBBootstrap"), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming file from {file}:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            processLabel.Text = "Done preparing Bootstrapper files";
            await Task.Delay(150);
        }


        private async Task StartProcess()
        {
            var generated = _compressor != null && await _compressor.CreatePatchFile();
            if (generated)
            {
                processLabel.Text = @"Completed";
                await Task.Delay(250);
            }
            Completed();
        }

        private async void CompressorOnProgressChanged(object? sender, ProgressEventArgs e)
        {
            if (e.Text != null) processLabel.Text = $@"{e.Text}";
            await Task.Delay(250);
            progressBar.Value = e.PercentComplete;
        }


        private void Completed() => Close();

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
