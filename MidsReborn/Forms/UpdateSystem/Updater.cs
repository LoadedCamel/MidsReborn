using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class Updater : Form
    {
        private BackgroundWorker _zipExtractor;

        private Progress<ZipProgress> _zProgress;
        public string VersionText { get; set; }
        public string Type { get; set; }
        private static Uri UpdateFile { get; set; }
        private static string TempFile { get; set; }
        private Thread DownloadThread { get; set; }

        public Updater(frmMain parent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            Location = new Point((parent.pnlGFX.Right - Width) * 2, (parent.pnlGFX.Height - Height) / 2);
            InitializeComponent();
            Begin_Update();
        }

        private void Begin_Update()
        {
            DownloadThread = new Thread(() =>
            {
                var client = new WebClient();
                circularProgressBar1.Text = @"Downloading";
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                UpdateFile = new Uri($"{Path.GetDirectoryName(MidsContext.Config.UpdatePath)?.Replace("https:\\", "https://").Replace("\\", "/")}/{VersionText}.zip");
                Directory.CreateDirectory($"{Path.GetTempPath()}\\MidsTemp\\");
                TempFile = $"{Path.GetTempPath()}\\MidsTemp\\{VersionText}.zip";
                client.DownloadFileAsync(UpdateFile, TempFile);
            });
            DownloadThread.Start();
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                circularProgressBar1.Text = @"Please Wait";
                Task.Delay(2000);
                BeginInstall(true);
            });
        }

        private void BeginInstall(bool completed)
        {
            if (completed) InstallUpdate(Type);
        }

        private void InstallUpdate(string updateType)
        {
            var asmLoc = Assembly.GetExecutingAssembly().Location;
            var dirLoc = $"{Directory.GetParent(asmLoc)}";
            switch (updateType)
            {
                case "App":
                    circularProgressBar1.Text = @"Installing";
                    foreach (var filename in Directory.GetFiles(dirLoc, "*.dll"))
                        File.Move(filename, $"{filename}.bak");

                    File.Move(Assembly.GetExecutingAssembly().Location,
                        $"{Assembly.GetExecutingAssembly().Location}.bak");
                    break;
                case "Database":
                    circularProgressBar1.Text = @"Installing";
                    break;
            }

            RunZipExtractor();
        }

        private void RunZipExtractor()
        {
            _zipExtractor = new BackgroundWorker();
            _zipExtractor.DoWork += zipExtractor_DoWork;
            _zipExtractor.RunWorkerCompleted += zipExtractor_RunWorkerCompleted;
            _zipExtractor.WorkerReportsProgress = true;
            _zipExtractor.RunWorkerAsync();
        }

        private void zipExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            _zProgress = new Progress<ZipProgress>();
            _zProgress.ProgressChanged += Report;
            TempFile = $"{Path.GetTempPath()}MidsTemp\\{VersionText}.zip";
            var loc = Assembly.GetExecutingAssembly().Location;
            var dirLoc = Directory.GetParent(loc);
            var dirTmp = $"{Path.GetTempPath()}MidsTemp\\";
            using var zip = ZipFile.OpenRead(TempFile);
            zip.ExtractToDirectory(dirTmp, _zProgress, true);
            zip.ExtractToDirectory(dirLoc.ToString(), null, true);
        }

        private void Report(object sender, ZipProgress zipProgress)
        {
            var percentCompleted = zipProgress.Processed * 100 / zipProgress.Total;
            _zipExtractor.ReportProgress(percentCompleted);
        }

        private void zipExtractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var asmLoc = Assembly.GetExecutingAssembly().Location;
            var dirLoc = $"{Directory.GetParent(asmLoc)}";
            circularProgressBar1.Text = @"Complete";
            TransferFiles($"{Path.GetTempPath()}MidsTemp\\", dirLoc);
            Task.Delay(2000);
            Directory.Delete($"{Path.GetTempPath()}MidsTemp\\", true);
            File.Delete($"{dirLoc}\\{VersionText}.zip");
            circularProgressBar1.Text = @"Restarting";
            Task.Delay(1000);
            Application.Restart();
        }
        private static void TransferFiles(string sourcePath, string destinationPath)
        {
            foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var dirInfo = new DirectoryInfo(file);
                var fileInfo = new FileInfo(file);
                var parentDir = dirInfo.Parent?.Name;
                if (parentDir == "MidsTemp")
                {
                    var destFile = $"{destinationPath}\\{fileInfo.Name}";
                    File.Copy(fileInfo.FullName, destFile, true);
                }
            }
        }
    }
}
