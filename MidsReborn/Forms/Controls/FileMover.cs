using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class FileMover : Form
    {
        private readonly BackgroundWorker _progressWorker = new();
        private bool _moveExceptionRequested;
        private readonly string _sourceDirectory;
        private readonly string _destinationDirectory;
        private List<string> Items { get; set; }

        public FileMover(string source, string destination)
        {
            _sourceDirectory = source;
            _destinationDirectory = destination;
            Load += OnLoad;
            InitializeComponent();

        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();
            sourceLabel.Text = _sourceDirectory;
            destLabel.Text = _destinationDirectory;
            Items = Directory.GetFiles(_sourceDirectory).ToList();
            ctlProgressBar1.Maximum = Items.Count;
            ctlProgressBar1.Value = 0;
            ctlProgressBar1.Step = 1;
            _progressWorker.WorkerReportsProgress = true;
            _progressWorker.DoWork += ProgressWorker_DoWork;
            _progressWorker.ProgressChanged += ProgressWorker_ProgressChanged;
            _progressWorker.RunWorkerCompleted += ProgressWorker_RunWorkerCompleted;
            Start();
        }

        private void Start()
        {
            _progressWorker.RunWorkerAsync();
        }

        private void ProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ctlProgressBar1.StatusText = @"Moving...";
            ctlProgressBar1.ItemCount = Items.Count;
            for (var index = 0; index < Items.Count; index++)
            {
                var item = Items[index];
                var fInfo = new FileInfo(item);
                try
                {
                    File.Move(fInfo.FullName, Path.Combine(_destinationDirectory, fInfo.Name));
                }
                catch (Exception)
                {
                    _moveExceptionRequested = true;
                }

                var percentage = (index * ctlProgressBar1.Maximum) / Items.Count;
                _progressWorker.ReportProgress(percentage);
                Thread.Sleep(50);
            }
        }

        private void ProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ctlProgressBar1.Value = e.ProgressPercentage;
        }

        private void ProgressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ctlProgressBar1.StatusText = string.Empty;
            if (!_moveExceptionRequested)
            {
                ResultYes();
            }
            else
            {
                ResultNo();
            }
        }

        private void ResultYes()
        {
            DialogResult = DialogResult.Yes;
        }

        private void ResultNo()
        {
            DialogResult = DialogResult.No;
        }
    }
}
