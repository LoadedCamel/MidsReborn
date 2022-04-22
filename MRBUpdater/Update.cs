using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase.Utils;

namespace MRBUpdater
{
    public partial class Update : Form
    {
        private BackgroundWorker UpdateWorker;
        private PatchDecompressor PatchDecompressor;
        private List<FileData> DecompressedData;
        private string UpdatePath { get; set; }
        private string VersionText { get; set; }
        private int ParentPid { get; set; }
        private string ExtractPath { get; set; }
        private static Uri UpdateFile { get; set; }
        private static string PackedUpdate { get; set; }

        public Update(IReadOnlyList<string> args)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            UpdatePath = args[0];
            VersionText = args[1];
            ParentPid = int.Parse(args[2]);
            ExtractPath = args[3];
            Load += OnLoad;
            Shown += OnShown;
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void OnShown(object sender, EventArgs e)
        {
            Updater_DownloadUpdate();
        }

        private void Updater_DownloadUpdate()
        {
            var client = new WebClient();
            lblStatus.Text = @"Downloading Update...";
            lblFileName.Text = $@"{VersionText}.mru";
            client.DownloadProgressChanged += Updater_DownloadProgressChanged;
            client.DownloadFileCompleted += Updater_DownloadFileCompleted;
            var lastSeg = new Uri(UpdatePath).Segments.Last();
            UpdateFile = new Uri($"{UpdatePath.Replace(lastSeg, $"{VersionText}.mru")}");
            PackedUpdate = Path.Combine(AppContext.BaseDirectory, $"{VersionText}.mru");
            client.DownloadFileAsync(UpdateFile, PackedUpdate);
        }

        private void Updater_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 99)
            {
                ctlProgressBar1.Value = e.ProgressPercentage - 1;
            }
            else
            {
                ctlProgressBar1.Value = e.ProgressPercentage;
            }
            Thread.Sleep(50);
        }

        private async void Updater_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                lblStatus.Text = @"Download Error: Aborting";
            }
            else
            {
                lblStatus.Text = @"Please Wait...";
                await Task.Delay(1500);
                Updater_BeginInstall();
            }
        }

        private void Updater_BeginInstall()
        {
            ctlProgressBar1.Value = 0;
            lblStatus.Text = @"Installing Update...";
            UpdateWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            PatchDecompressor = new PatchDecompressor();
            DecompressedData = PatchDecompressor.DecompressData(PackedUpdate);
            UpdateWorker.DoWork += UpdateWorker_DoWork;
            UpdateWorker.RunWorkerCompleted += UpdateWorker_RunWorkerCompleted;
            UpdateWorker.RunWorkerAsync();
        }

        private void UpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Process.GetProcessById(ParentPid).Kill();
            Process.GetProcessById(ParentPid).WaitForExit();
            if (DecompressedData == null)
            {
                lblStatus.Text = @"Install Error: Aborting";
                MessageBox.Show("Decompressed Data is NULL");
            }
            else
            {
                //PatchDecompressor.CleanOldEntries(ExtractPath);
                PatchDecompressor.ProgressUpdate += PatchDecompressor_OnProgressUpdate;
                PatchDecompressor.ErrorUpdate += PatchDecompressor_OnErrorUpdate;
                PatchDecompressor.RecompileFileEntries(ExtractPath, DecompressedData);
            }
        }

        private void PatchDecompressor_OnErrorUpdate(object sender, ErrorEventArgs e)
        {
            lblStatus.Text = @"Decompression Error: Aborting";
            MessageBox.Show($"{e.GetException().Message}\r\r\n{e.GetException().StackTrace}");
            UpdateWorker.CancelAsync();
        }

        private void PatchDecompressor_OnProgressUpdate(object sender, ProgressEventArgs e)
        {
            lblFileName.Text = e.Name;
            ctlProgressBar1.Value = e.PercentComplete;
            UpdateWorker.ReportProgress(e.PercentComplete);
        }

        private void UpdateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ctlProgressBar1.Value = 99;
            lblStatus.Text = @"Update Complete!";
            lblFileName.Text = string.Empty;
            Application.DoEvents();
            File.Delete(PackedUpdate);
            Thread.Sleep(1000);
            lblStatus.Text = @"Restarting...";
            Application.DoEvents();
            Thread.Sleep(1000);
            Process.Start(Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe"));
            Application.Exit();
        }
    }
}
