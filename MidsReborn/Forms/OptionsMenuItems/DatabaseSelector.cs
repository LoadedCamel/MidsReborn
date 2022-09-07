using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Core;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems
{
    public partial class DatabaseSelector : Form
    {
        private List<DatabaseItems> DatabaseData { get; set; } = new();
        public string SelectedDatabase;

        public DatabaseSelector()
        {
            Load += OnLoad;
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
        }

        private struct DatabaseItems
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToScreen();
            var databases = Directory.EnumerateDirectories(Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder));
            foreach (var database in databases)
            {
                var databaseData = new DirectoryInfo(database);
                DatabaseData.Add(new DatabaseItems { Name = databaseData.Name, Path = databaseData.FullName });
            }

            dbDropdown.DataSource = DatabaseData;
            dbDropdown.DisplayMember = "Name";
            dbDropdown.ValueMember = "Path";
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
