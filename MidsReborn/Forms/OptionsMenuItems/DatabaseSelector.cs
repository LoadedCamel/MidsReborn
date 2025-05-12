using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Utils;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems
{
    public partial class DatabaseSelector : Form
    {
        private List<DatabaseItems> DatabaseData { get; set; } = [];
        private bool ShowCurrent;
        private bool EnableBrowse;
        public string? SelectedDatabase;

        public DatabaseSelector(bool showCurrent = true, bool enableBrowse = false)
        {
            Load += OnLoad;
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
            ShowCurrent = showCurrent;
            EnableBrowse = enableBrowse;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToScreen();
            var databases = Directory.EnumerateDirectories(Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder));
            var index = 0;
            var currentDbIndex = -1;
            foreach (var database in databases)
            {
                var databaseData = new DirectoryInfo(database);
                if (databaseData.Name == DatabaseAPI.DatabaseName)
                {
                    if (ShowCurrent)
                    {
                        currentDbIndex = index;
                        DatabaseData.Add(new DatabaseItems { Name = $"{databaseData.Name} [current]", Path = databaseData.FullName });
                    }
                }
                else
                {
                    DatabaseData.Add(new DatabaseItems { Name = databaseData.Name, Path = databaseData.FullName });
                }

                index++;
            }

            if (EnableBrowse)
            {
                DatabaseData.Add(new DatabaseItems { Name = "Browse...", Path = null});
            }

            dbDropdown.DataSource = DatabaseData;
            dbDropdown.DisplayMember = "Name";
            dbDropdown.ValueMember = "Path";
            if (currentDbIndex > -1)
            {
                dbDropdown.SelectedIndex = currentDbIndex;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            SelectedDatabase = dbDropdown.SelectedValue?.ToString();
            if (string.IsNullOrWhiteSpace(SelectedDatabase))
            {
                var dirSelector = new FolderBrowserDialog
                {
                    Description = @"Select your secondary database directory:",
                    ShowNewFolderButton = false
                };
                var dsr = dirSelector.ShowDialog();
                if (dsr == DialogResult.Cancel)
                {
                    DialogResult = DialogResult.Cancel;
                    Close();

                    return;
                }

                SelectedDatabase = dirSelector.SelectedPath;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
