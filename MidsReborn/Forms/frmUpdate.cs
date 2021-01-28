using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;
using Mids_Reborn.Forms.UpdateSystem;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmUpdate : Form
    {
        private bool DLComplete;

        private BackgroundWorker zipExtractor;

        private Progress<ZipProgress> ZProgress;

        public frmUpdate()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            DownloadUpdate();
        }

        public string VersionText { get; set; }
        public string Type { get; set; }
        private static Uri UpdateFile { get; set; }
        private static string TempFile { get; set; }
        private Thread thread { get; set; }

        public string RichText
        {
            get => richTextBox1.Text;
            set => richTextBox1.Text = value;
        }

        private void DownloadUpdate()
        {
            thread = new Thread(() =>
            {
                var client = new WebClient();
                TempFile = $"{Path.GetTempPath()}\\MidsTemp\\{VersionText}.zip";
                if (File.Exists(TempFile)) File.Delete(TempFile);

                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                UpdateFile = new Uri($"{Path.GetDirectoryName(MidsContext.Config.UpdatePath)?.Replace("https:\\", "https://").Replace("\\", "/")}/{VersionText}.zip");
                Directory.CreateDirectory($"{Path.GetTempPath()}\\MidsTemp\\");
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
                //ctlProgressBar1.Value = int.Parse(Math.Truncate(percentage).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                //ctlProgressBar1.Text = $@"{ctlProgressBar1.StatusText} - {ctlProgressBar1.Value}%";
            });
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                //ctlProgressBar1.Text = @"Download Complete";
                DLComplete = true;
                BeginInstall();
            });
        }

        private void BeginInstall()
        {
            if (DLComplete) InstallUpdate(Type);
        }

        // https://stackoverflow.com/a/1281638
        private bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(path);
            var accessRules = accessControlList?.GetAccessRules(true, true,
                typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null) return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write) continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    writeAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    writeDeny = true;
                }
            }

            return writeAllow && !writeDeny;
        }

        private (int returnCode, string msg) SafeCreateBackup(string filename, bool overwrite=true)
        {
            var backupName = $"{filename}.bak";
            var ofExists = File.Exists(filename);
            var bfExists = File.Exists(backupName);

            if (ofExists & !bfExists)
            {
                File.Move(filename, backupName);
                return (returnCode: 0, msg: "");
            }

            if (!ofExists)
            {
                return (returnCode: -1, msg: $"{Path.GetFileName(filename)} backup cannot be created because this file cannot be reached.");
            }

            if (!overwrite)
            {
                return (returnCode: -2, msg: $"{Path.GetFileName(filename)} backup file already exists (won't overwrite).");
            }

            File.Delete(backupName);
            File.Move(filename, backupName);

            return (returnCode: 0, msg: "");
        }

        private bool SafeCreateBackupWMsg(string filename)
        {
            var (returnCode, msg) = SafeCreateBackup(filename);
            if (returnCode >= 0) return true;

            MessageBox.Show(
                $"Cannot create backup file for {Path.GetFileName(filename)}.\r\n{msg}",
                "Dang", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }

        private void InstallUpdate(string updateType)
        {
            var asmLoc = Assembly.GetExecutingAssembly().Location;
            var dirLoc = $"{Directory.GetParent(asmLoc)}";
            if (!HasWritePermissionOnDir(dirLoc))
            {
                MessageBox.Show(
                    $"Cannot start update process: no write permission on Mids Reborn install folder.\r\n({dirLoc})",
                    "Dang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                Close();
                return;
            }

            //ctlProgressBar2.Value = 0;
            switch (updateType)
            {
                case "App":
                    //ctlProgressBar2.StatusText = $"Installing: Mids {VersionText}";
                    foreach (var filename in Directory.GetFiles(dirLoc, "*.dll").Append(asmLoc))
                    {
                        if (SafeCreateBackupWMsg(filename)) continue;
                        
                        Close();
                        return;
                    }

                    break;
                case "Database":
                    //ctlProgressBar2.StatusText = $"Installing: {updateType} {VersionText}";
                    break;
            }

            RunZipExtractor();
        }

        private void RunZipExtractor()
        {
            //ctlProgressBar2.Maximum = 100;
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
            TempFile = $"{Path.GetTempPath()}MidsTemp\\{VersionText}.zip";
            var loc = Assembly.GetExecutingAssembly().Location;
            var dirLOC = Directory.GetParent(loc);
            var dirTmp = $"{Path.GetTempPath()}MidsTemp\\";
            using var zip = ZipFile.OpenRead(TempFile);
            zip.ExtractToDirectory(dirTmp, ZProgress, true);
            zip.ExtractToDirectory(dirLOC.ToString(), null, true);
        }

        private void Report(object sender, ZipProgress zipProgress)
        {
            var percentCompleted = zipProgress.Processed * 100 / zipProgress.Total;
            zipExtractor.ReportProgress(percentCompleted);
        }

        private void zipExtractor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var totalPercent = e.ProgressPercentage;
            //if (totalPercent > ctlProgressBar2.Maximum) totalPercent = 100;

            //ctlProgressBar2.Value = totalPercent;
            //ctlProgressBar2.Text = $@"{ctlProgressBar2.StatusText}";
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

                /*else
                {
                    var subPath = $"{destinationPath}\\{parentDir}\\{fileInfo.Name}";
                    File.Copy(fileInfo.FullName, subPath, true);
                }*/
            }
        }

        private void zipExtractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var asmLOC = Assembly.GetExecutingAssembly().Location;
            var dirLOC = $"{Directory.GetParent(asmLOC)}";
            //ctlProgressBar2.Value = ctlProgressBar2.Maximum;
            //ctlProgressBar2.Text = "Installation Complete!";
            //ctlProgressBar2.StatusText = "Install Complete!";
            var completed = MessageBox.Show(@"The update was successfully applied, Mids will now restart.",
                @"Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (completed == DialogResult.OK)
            {
                TransferFiles($"{Path.GetTempPath()}MidsTemp\\", dirLOC);
                Thread.Sleep(2000);
                Directory.Delete($"{Path.GetTempPath()}MidsTemp\\", true);
                File.Delete($"{dirLOC}\\{VersionText}.zip");
                Application.Restart();
            }
        }
    }
}