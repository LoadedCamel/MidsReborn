using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using mrbBase;

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
                Debug.WriteLine(databaseData.FullName);
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
