using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Extensions;
using MRBResourceLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDbDiff : Form
    {
        private class DiffItem(string[]? textData, bool selected = false) : ICloneable
        {
            public string[]? TextData = textData;
            public bool Selected = selected;

            // Deep clone object
            public object Clone()
            {
                return new DiffItem(
                    (string[]?)TextData?.Clone(),
                    Selected
                );
            }
        }

        private IDatabase? SecDb;
        private string? SecDbPath;
        private List<DiffItem>? DiffData;
        private List<string>? DbInfo;
        private string? ExportDir;

        public frmDbDiff()
        {
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
        }

        // Get DB name (last chunk of path)
        private static string GetDbName(string path)
        {
            return Path.GetFileName(path);
        }

        // Force refreshing virtual listview.
        private void RefreshLv()
        {
            if (DiffData == null)
            {
                return;
            }

            listView1.VirtualListSize = 0;
            listView1.VirtualListSize = DiffData.Count;
        }

        // Power.GetPowerset()?.GetGroupName() doesn't work.
        // Pick first two chunks of power's full name.
        private static string GetPowerGroup(string? pwName, bool fullName = true)
        {
            if (string.IsNullOrWhiteSpace(pwName))
            {
                return "";
            }

            var chunks = pwName.Split('.');

            return chunks.Length switch
            {
                <= 1 => pwName,
                >= 2 when !fullName => chunks[1],
                _ => $"{chunks[0]}.{chunks[1]}"
            };
        }

        private void UpdateData()
        {
            // Left box, main db info
            DbInfo =
            [
                $"Current: {DatabaseAPI.DatabaseName}",
                $"Rev. {DatabaseAPI.Database.Version.ToString()} for I{DatabaseAPI.Database.Issue} {DatabaseAPI.Database.PageVolText} {DatabaseAPI.Database.PageVol}",
                $"Powersets: {DatabaseAPI.Database.Powersets.Length}",
                $"Powers: {DatabaseAPI.Database.Power.Length}"
            ];

            if (SecDbPath != null && SecDb != null)
            {
                // Auxiliary db info
                DbInfo.AddRange([
                    "",
                    $"Secondary: {GetDbName(SecDbPath)}",
                    $"Rev. {SecDb.Version.ToString()} for I{SecDb.Issue} {SecDb.PageVolText} {SecDb.PageVol}",
                    $"Powersets: {SecDb.Powersets.Length}",
                    $"Powers: {SecDb.Power.Length}"
                ]);
            }

            listBox1.DataSource = new BindingList<string>(DbInfo);

            if (SecDb == null)
            {
                return;
            }

            DiffData = [new DiffItem(["", "", "Processing diff...", "", ""])];
            RefreshLv();

            DiffData = [];

            // New powers
            DiffData.AddRange(SecDb.Power
                .Where(e => e != null)
                .Where(e => !DatabaseAPI.Database.Power.Any(f => f != null && f.FullName == e?.FullName))
                .Select(e => new DiffItem(["New", "Power", e?.DisplayName ?? "--", GetPowerGroup(e?.FullName), e?.FullName ?? "--"], true)));

            // Removed powers
            DiffData.AddRange(DatabaseAPI.Database.Power
                .Where(e => e != null)
                .Where(e => !SecDb.Power.Any(f => f != null && f.FullName == e?.FullName))
                .Select(e => new DiffItem(["Rem", "Power", e?.DisplayName ?? "--", GetPowerGroup(e?.FullName), e?.FullName ?? "--"], true)));

            // Edited powers
            // Not working.
            /*DiffData.AddRange(SecDb.Power
                .Where(e => e != null)
                .Where(e => DatabaseAPI.Database.Power.Any(f => f != null && f.FullName == e?.FullName) && e != DatabaseAPI.GetPowerByFullName(e.FullName))
                .Select(e => new DiffItem(["Mod", "Power", e?.DisplayName ?? "--", GetPowerGroup(e?.FullName), e?.FullName ?? "--"], true)));*/

            // New powersets
            DiffData.AddRange(SecDb.Powersets
                .Where(e => e != null)
                .Where(e => !DatabaseAPI.Database.Powersets.Any(f => f != null && f.FullName == e?.FullName))
                .Select(e => new DiffItem(["New", "Powerset", e?.DisplayName ?? "--", e?.GroupName ?? "(none)", e?.FullName ?? "--"], true)));

            // Removed powersets
            DiffData.AddRange(DatabaseAPI.Database.Powersets
                .Where(e => e != null)
                .Where(e => !SecDb.Powersets.Any(f => f != null && f.FullName == e?.FullName))
                .Select(e => new DiffItem(["Rem", "Powerset", e?.DisplayName ?? "--", e?.GroupName ?? "(none)", e?.FullName ?? "--"], true)));

            RefreshLv();

            btnSelectExportDir.Visible = true;
            label2.Visible = true;
        }

        private void frmDbDiff_Load(object sender, EventArgs e)
        {
            listView1.EnableDoubleBuffer();
            DiffData = [];
            DbInfo = [];
            listBox1.DataSource = new BindingList<string>();
            listBox1.BindingContext = new BindingContext();
            btnSelectExportDir.Visible = false;
            label2.Visible = false;
            label2.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSelectDb_Click(object sender, EventArgs e)
        {
            using var dbSelector = new DatabaseSelector(false, true);
            var result = dbSelector.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            var dbSelected = dbSelector.SelectedDatabase;
            if (dbSelected == null)
            {
                return;
            }

            DiffData = [new DiffItem(["", "", "Loading DB...", "", ""])];
            RefreshLv();

            SecDbPath = dbSelected;
            SecDb = new Database();
            var ret = DatabaseAPI.LoadMainDatabase(dbSelected, ref SecDb);
            if (!ret)
            {
                DiffData = [];
                RefreshLv();
                MessageBox.Show($"Unable to load database from {dbSelected}!", "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            UpdateData();
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // Detect system dark theme
            var isDarkTheme = SystemColors.Window.GetBrightness() <= 0.5f;

            var item = new ListViewItem
            {
                BackColor = DiffData?[e.ItemIndex].TextData?[0] switch
                {
                    _ when DiffData?[e.ItemIndex].Selected != true & isDarkTheme => Color.FromArgb(0x40, 0x40, 0x55),
                    _ when DiffData?[e.ItemIndex].Selected != true => Color.FromArgb(0xBB, 0xBB, 0xFF),
                    "New" when isDarkTheme => Color.FromArgb(0x00, 0x40, 0x00),
                    "Rem" when isDarkTheme => Color.FromArgb(0x40, 0x00, 0x00),
                    "Mod" when isDarkTheme => Color.FromArgb(0x47, 0x32, 0x82),
                    "New" => Color.FromArgb(0x00, 0xBB, 0x00),
                    "Rem" => Color.FromArgb(0xBB, 0x00, 0x00),
                    "Mod" => Color.FromArgb(0x86, 0x66, 0xe0),
                    _ => Color.Transparent
                },
                Tag = e.ItemIndex
            };

            if (DiffData?[e.ItemIndex].TextData != null)
            {
                item.Text = DiffData?[e.ItemIndex].TextData?[0];
                if (DiffData?[e.ItemIndex].TextData?.Length > 1)
                {
                    for (var i = 1; i < (DiffData?[e.ItemIndex].TextData).Length; i++)
                    {
                        var t = DiffData?[e.ItemIndex].TextData?[i];
                        var s = new ListViewItem.ListViewSubItem
                        {
                            Text = t
                        };

                        item.SubItems.Add(s);
                    }
                }
            }
            else
            {
                item.Text = "";
                for (var i = 0; i < Math.Max(0, listView1.Columns.Count - 1); i++)
                {
                    var s = new ListViewItem.ListViewSubItem
                    {
                        Text = ""
                    };

                    item.SubItems.Add(s);
                }
            }

            e.Item = item;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            var lv = (ListView)sender;
            var lvi = lv.GetItemAt(e.X, e.Y);

            if (lvi?.Tag == null)
            {
                return;
            }

            if (DiffData == null)
            {
                return;
            }

            DiffData[(int)lvi.Tag].Selected = !DiffData[(int)lvi.Tag].Selected;
        }

        private void btnOpRun_Click(object sender, EventArgs e)
        {
            if (SecDb == null)
            {
                MessageBox.Show("No secondary database is loaded.", "Whoops");

                return;
            }

            if (ExportDir == null)
            {
                MessageBox.Show("Export directory is not set.", "Whoops");

                return;
            }

            var newPowers = SecDb.Power
                .Where(e => e != null)
                .Where(e => !DatabaseAPI.Database.Power.Any(f => f != null && f.FullName == e?.FullName))
                .ToList();


            progressBar1.Minimum = 0;
            progressBar1.Maximum = newPowers.Count + 1;
            progressBar1.Value = 0;
            var k = 0;

            foreach (var p in newPowers)
            {
                if (p == null)
                {
                    continue;
                }

                File.WriteAllText($"{ExportDir}{Path.DirectorySeparatorChar}{p.FullName}.json", p.ExportToJson());
                progressBar1.Value = k++;
            }

            var removedPowers = DatabaseAPI.Database.Power
                .Where(e => e != null)
                .Where(e => !SecDb.Power.Any(f => f != null && f.FullName == e?.FullName))
                .Select(e => e?.FullName)
                .ToList();

            File.WriteAllText($"{ExportDir}{Path.DirectorySeparatorChar}removed_powers.txt", string.Join("\r\n", removedPowers));
            progressBar1.Value = ++k;
            progressBar1.Visible = false;

            MessageBox.Show($"Exported {newPowers.Count} new powers, {removedPowers.Count} removed powers to {ExportDir}.", "Export complete");
        }

        private void btnSelectExportDir_Click(object sender, EventArgs e)
        {
            var dirSelector = new FolderBrowserDialog
            {
                Description = @"Select directory where to export powers:",
                ShowNewFolderButton = false
            };

            var dsr = dirSelector.ShowDialog();
            if (dsr == DialogResult.Cancel)
            {
                return;
            }

            ExportDir = dirSelector.SelectedPath;
            label2.Text = $"Export to: {ExportDir}";
            panel1.Visible = true;
        }
    }
}
