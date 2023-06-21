using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MRBUpdater.Utils;

namespace MRBUpdater
{
    public partial class Update : Form
    {
        private PatchDecompressor? _patchDecompressor;
        private List<PatchDecompressor.FileData>? _decompressedData;
        private readonly int _parentPiD;
        private string PackedUpdate { get; set; } = string.Empty;
        private string ExtractPath { get; set; } = string.Empty;
        private string TempFile { get; }
        private readonly Queue<UpdateDetails>? _downloadQueue;
        private readonly Queue<InstallDetails> _installQueue;
        private InstallDetails _installDetails;
        private int _timeToRestart = 5;

        public Update(IReadOnlyList<string> passedArgs)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            TempFile = passedArgs[0];
            _installQueue = new Queue<InstallDetails>();
            _downloadQueue = JsonSerializer.Deserialize<Queue<UpdateDetails>>(File.ReadAllText(TempFile));
            _parentPiD = int.Parse(passedArgs[1]);
            Load += OnLoad;
            Shown += OnShown;
            InitializeComponent();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToScreen();
        }

        private async void OnShown(object? sender, EventArgs e)
        {
            await KillProcess();
            await Updater_DownloadUpdate();
        }

        private async Task KillProcess()
        {
            Process.GetProcessById(_parentPiD).Kill();
            await Process.GetProcessById(_parentPiD).WaitForExitAsync();
        }

        private async Task Updater_DownloadUpdate()
        {
            if (_downloadQueue != null)
            {
                if (_downloadQueue.Any())
                {
                    var client = new WebClient();
                    client.DownloadProgressChanged += Updater_DownloadProgressChanged;
                    client.DownloadFileCompleted += Updater_DownloadFileCompleted;
                    lblStatus.Text = @"Downloading Update(s)...";
                    var item = _downloadQueue.Dequeue();
                    var lastSeg = new Uri(item.Uri).Segments.Last();
                    var updFile = new Uri($"{item.Uri.Replace(lastSeg, $"{item.UpdateFile}")}");
                    PackedUpdate = Path.Combine(Path.GetTempPath(), $"{item.UpdateFile}");
                    ExtractPath = item.ExtractTo;
                    lblFileName.Text = $@"{item.UpdateFile}";
                    await client.DownloadFileTaskAsync(updFile, PackedUpdate);
                }
                else
                {
                    lblStatus.Text = @"Please Wait...";
                    await Task.Delay(1500);
                    await Updater_BeginInstall();
                }
            }
        }

        private async void Updater_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            await Task.Delay(50);
            if (e.ProgressPercentage > 99)
            {
                ctlProgressBar1.Value = e.ProgressPercentage - 1;
            }
            else
            {
                ctlProgressBar1.Value = e.ProgressPercentage;
            }
        }

        private async void Updater_DownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                lblStatus.Text = @"Download Error: Aborting";
            }
            else
            {
                _installQueue.Enqueue(new InstallDetails(PackedUpdate, ExtractPath));
                await Updater_DownloadUpdate();
            }
        }

        private async Task Updater_BeginInstall()
        {
            if (_installQueue.Any())
            {
                while (_installQueue.Count > 0)
                {
                    _installDetails = _installQueue.Dequeue();
                    ctlProgressBar1.Value = 0;
                    lblStatus.Text = @"Installing Update(s)...";
                    _patchDecompressor = new PatchDecompressor();
                    _decompressedData = PatchDecompressor.DecompressData(_installDetails.File);
                    if (_decompressedData == null)
                    {
                        lblStatus.Text = @"Installation Aborted: Expected data was null.";
                        return;
                    }

                    _patchDecompressor.ProgressUpdate += PatchDecompressor_OnProgressUpdate;
                    _patchDecompressor.ErrorUpdate += PatchDecompressor_OnErrorUpdate;
                    _patchDecompressor.Completed += PatchDecompressor_OnCompleted;
                    await _patchDecompressor.RecompileFileEntries(_installDetails.ExtractPath, _decompressedData);
                }
            }

            await Task.Delay(1500);
            await UpdaterFinished();
        }

        private async void PatchDecompressor_OnCompleted(object? sender, bool e)
        {
            if (!e) return;
            File.Delete(_installDetails.File);
            await Task.Delay(1000);
            await Updater_BeginInstall();
        }

        private void PatchDecompressor_OnErrorUpdate(object? sender, ErrorEventArgs e)
        {
            lblStatus.Text = @"Installation Aborted: Decompression Error.";
            MessageBox.Show(e.GetException().Message);
        }

        private async void PatchDecompressor_OnProgressUpdate(object? sender, ProgressEventArgs e)
        {
            lblFileName.Text = e.Name;
            await Task.Delay(250);
            ctlProgressBar1.Value = e.PercentComplete;
        }

        private async Task UpdaterFinished()
        {
            ctlProgressBar1.Value = 99;
            lblStatus.Text = @"Update Complete!";
            File.Delete(TempFile);
            await AsyncRestart();
            Process.Start(Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe"));
            Application.Exit();
        }

        private async Task AsyncRestart()
        {
            while (_timeToRestart != 0)
            {
                lblFileName.Text = $@"Restarting in {_timeToRestart}...";
                _timeToRestart -= 1;
                await Task.Delay(1250);
            }
            lblFileName.Text = @"Restarting MRB...";
            await Task.Delay(1500);
        }
    }
}
