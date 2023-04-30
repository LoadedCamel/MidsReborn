using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Utils;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems
{
    public partial class DatabaseSelector : Form
    {
        private List<DatabaseItems> DatabaseData { get; set; } = new();
        public string? SelectedDatabase;

        public DatabaseSelector()
        {
            Load += OnLoad;
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
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
                    currentDbIndex = index;
                    DatabaseData.Add(new DatabaseItems { Name = $"{databaseData.Name} [current]", Path = databaseData.FullName });
                }
                else
                {
                    DatabaseData.Add(new DatabaseItems { Name = databaseData.Name, Path = databaseData.FullName });
                }

                index++;
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
            SelectedDatabase = dbDropdown.SelectedValue.ToString();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
