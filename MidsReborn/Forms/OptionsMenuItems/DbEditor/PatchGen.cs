using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
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
