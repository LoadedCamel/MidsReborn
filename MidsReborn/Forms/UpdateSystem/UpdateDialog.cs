using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mids_Reborn.Forms.UpdateSystem.Models;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class UpdateDialog : Form
    {
        private List<string> Updates { get; }

        public UpdateDialog(UpdateCheckResult result)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            Updates = [];

            if (result.IsAppUpdateAvailable)
            {
                Updates.Add($"* {result.AppName} v{result.AppVersion}");
            }

            if (result.IsDbUpdateAvailable)
            {
                Updates.Add($"* {result.DbName} Database ({result.DbVersion})");
            }
            Load += OnQuery_Load;
            InitializeComponent();
        }

        private void OnQuery_Load(object? sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var update in Updates)
            {
                sb.AppendLine(update);
            }
            updatesLabel.Text =  sb.ToString();
            CenterToParent();
        }
    }
}
