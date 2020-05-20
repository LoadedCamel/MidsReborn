using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;
using midsControls;


namespace Hero_Designer.Forms
{
    public partial class frmUpdate : Form
    {
        public string VersionText { get; set; }
        public string Type { get; set; }
        public static Uri UpdateFile { get; set; }
        public static string TempFile { get; set; }

        private Progress<ZipProgress> ZProgress;
        public Thread thread { get; set; }

        private bool DLComplete;

        private BackgroundWorker zipExtractor;

        public string RichText
        {
            get => richTextBox1.Text;
            set => richTextBox1.Text = value;
        }

        public frmUpdate()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            DownloadUpdate();
        }

        public void DownloadUpdate()
        {
            thread = new Thread(() =>
            {
                var client = new WebClient();
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                UpdateFile =
                    new Uri(
                        $"{Path.GetDirectoryName(MidsContext.Config.UpdatePath)?.Replace("https:\\", "https://").Replace("\\", "/")}/{VersionText}.zip");
                Directory.CreateDirectory($"{Path.GetTempPath()}\\MidsTemp\\");
                TempFile = $"{Path.GetTempPath()}\\MidsTemp\\{VersionText}.zip";
                client.DownloadFileAsync(UpdateFile, TempFile);
            });
            thread.Start();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _ = BeginInvoke((MethodInvoker) delegate
            {
                var bytesIn = double.Parse(e.BytesReceived.ToString(CultureInfo.InvariantCulture),
                    CultureInfo.InvariantCulture);
                var totalBytes = double.Parse(e.TotalBytesToReceive.ToString(CultureInfo.InvariantCulture),
                    CultureInfo.InvariantCulture);
                var percentage = bytesIn / totalBytes * 100;
                ctlProgressBar1.Value = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture),
                    CultureInfo.InvariantCulture);
                ctlProgressBar1.Text = $@"{ctlProgressBar1.StatusText} - {ctlProgressBar1.Value}%";
            });
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                ctlProgressBar1.Text = @"Download Complete";
                DLComplete = true;
                BeginInstall();
            });
        }

        public void BeginInstall()
        {
            if (DLComplete)
            {
                InstallUpdate(Type);
            }
        }
        public void InstallUpdate(string updateType)
        {
            ctlProgressBar2.Value = 0;
            switch (updateType)
            {
                case "App":
                    ctlProgressBar2.StatusText = $"Installing: Mids {VersionText}";
                    foreach (var filename in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll"))
                    {
                        File.Move(filename, $"{filename}.bak");
                    }

                    File.Move(Assembly.GetExecutingAssembly().Location,
                        $"{Assembly.GetExecutingAssembly().Location}.bak");
                    break;
                case "Database":
                    ctlProgressBar2.StatusText = $"Installing: {updateType} {VersionText}";
                    break;
            }

            RunZipExtractor();
        }

        public void RunZipExtractor()
        {
            ctlProgressBar2.Maximum = 100;
            zipExtractor = new BackgroundWorker();
            zipExtractor.DoWork += zipExtractor_DoWork;
            zipExtractor.ProgressChanged += zipExtractor_ProgressChanged;
            zipExtractor.RunWorkerCompleted += zipExtractor_RunWorkerCompleted;
            zipExtractor.WorkerReportsProgress = true;
            zipExtractor.RunWorkerAsync();
        }

        private void zipExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            ZProgress = new Progress<ZipProgress>();
            ZProgress.ProgressChanged += Report;
            TempFile = $"{Path.GetTempPath()}\\MidsTemp\\{VersionText}.zip";
            var loc = Assembly.GetExecutingAssembly().Location;
            var dirloc = Directory.GetParent(loc);
            using ZipArchive zip = ZipFile.OpenRead(TempFile);
            zip.ExtractToDirectory(dirloc.ToString(), ZProgress, true);
        }

        private void Report(object sender, ZipProgress zipProgress)
        {
            var percentCompleted = zipProgress.Processed * 100 / zipProgress.Total;
            zipExtractor.ReportProgress(percentCompleted);
        }

        private void zipExtractor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var totalPercent = e.ProgressPercentage;
            if (totalPercent > ctlProgressBar2.Maximum)
            {
                totalPercent = 100;
            }

            ctlProgressBar2.Value = totalPercent;
            ctlProgressBar2.Text = $@"{ctlProgressBar2.StatusText}";
        }

        private void zipExtractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ctlProgressBar2.Value = ctlProgressBar2.Maximum;
            ctlProgressBar2.Text = "Installation Complete!";
            ctlProgressBar2.StatusText = "Install Complete!";
            var completed = MessageBox.Show(@"The update was successfully applied, Mids will now restart.",
                @"Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (completed == DialogResult.OK)
            {
                File.Delete(TempFile);
                Directory.Delete($"{Path.GetTempPath()}\\MidsTemp\\");
                Application.Restart();
            }
        }
    }
}
