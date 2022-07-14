using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Extensions;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDbQueries : Form
    {
        private class ListViewColumnSorter : IComparer
        {
            public int SortColumn { get; set; }

            public SortOrder Order { get; set; }

            public ListViewColumnSorter()
            {
                SortColumn = 0;
                Order = SortOrder.None;
            }

            public int Compare(object x, object y)
            {
                var listViewX = (ListViewItem) x;
                var listViewY = (ListViewItem) y;

                var compareResult = string.Compare(listViewX.SubItems[SortColumn].Text, listViewY.SubItems[SortColumn].Text, StringComparison.InvariantCultureIgnoreCase);

                return Order switch
                {
                    SortOrder.Ascending => compareResult,
                    SortOrder.Descending => -compareResult,
                    _ => 0
                };
            }
        }

        private enum QueryType
        {
            StaticIndex,
            PowerName,
            FirstAvailableIndex,
            HighestAvailableIndex,
            AllAvailableIndices
        }

        private List<string[]> LvItems;
        private QueryType CurrentQueryType;
        private ListViewColumnSorter LvColumnSorter;

        public frmDbQueries()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Icon = Resources.reborn;
            listView1.EnableDoubleBuffer();
            LvColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = LvColumnSorter;
        }

        private void AddLVPowersData(List<Power> pwList)
        {
            LvItems = new List<string[]>();

            listView1.BeginUpdate();
            listView1.Items.Clear();
            if (pwList.Count <= 0)
            {
                listView1.Items.Add(new ListViewItem(new[] { "", "Nothing found", "" }));
                listView1.EndUpdate();

                return;
            }

            foreach (var p in pwList)
            {
                var item = new[] {$"{p.StaticIndex}", p.DisplayName, p.FullName};
                LvItems.Add(item);
                listView1.Items.Add(new ListViewItem(item));
            }

            listView1.EndUpdate();
        }

        private void AddLVPowersData(List<string[]> items)
        {
            LvItems = new List<string[]>();

            listView1.BeginUpdate();
            listView1.Items.Clear();

            foreach (var p in items)
            {
                var item = new[] {$"{p[0]}", p[1], p[2]};
                LvItems.Add(item);
                listView1.Items.Add(new ListViewItem(item));
            }

            listView1.EndUpdate();
        }

        private void frmDbQueries_Load(object sender, EventArgs e)
        {
            LvItems = new List<string[]>();
        }

        private void btnSearchByIndex_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.StaticIndex;

            var ret = int.TryParse(tbStaticIndex.Text, out var staticIndex);
            if (!ret || staticIndex < 0)
            {
                MessageBox.Show("Static index must be a positive integer.", "Error");

                return;
            }

            var iPowers = DatabaseAPI.Database.Power.Where(pw => pw != null && pw.StaticIndex == staticIndex).ToList();
            var powers = iPowers.Select(pw => new Power(pw)).ToList();
            
            AddLVPowersData(powers);
        }

        private void btnSearchByName_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.PowerName;

            var pName = tbPowerName.Text.Trim();
            if (string.IsNullOrEmpty(pName))
            {
                MessageBox.Show("Power name must be non empty.", "Error");

                return;
            }

            var iPowers = DatabaseAPI.Database.Power.Where(pw => pw != null && string.Equals(pw.DisplayName, pName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            var powers = iPowers.Select(pw => new Power(pw)).ToList();
            
            AddLVPowersData(powers);
        }

        private void btnFirstAvailableIndex_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.FirstAvailableIndex;

            var dbIndices = DatabaseAPI.Database.Power.Select(pw => pw?.StaticIndex ?? 0);
            var indices = Enumerable.Range(0, dbIndices.Max() + 1);

            var availableIndices = indices.Except(dbIndices).ToList();
            availableIndices.Sort();

            AddLVPowersData(new List<string[]>
            {
                new[] {$"{availableIndices[0]}", "First available", ""},
                new[] {$"{DatabaseAPI.Database.Power.Length}", "Power DB Count", ""}
            });
        }

        private void btnHighestAvailableIndex_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.HighestAvailableIndex;

            var dbIndices = DatabaseAPI.Database.Power.Select(pw => pw?.StaticIndex ?? 0);

            AddLVPowersData(new List<string[]>
            {
                new[] {$"{dbIndices.Max() + 1}", "Highest available", ""},
                new[] {$"{DatabaseAPI.Database.Power.Length}", "Power DB Count", ""}
            });
        }

        private void btnAllAvailableIndices_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.AllAvailableIndices;

            var dbIndices = DatabaseAPI.Database.Power.Select(pw => pw?.StaticIndex ?? 0).ToList();
            var indices = Enumerable.Range(0, dbIndices.Max() + 1).ToList();

            var availableIndices = indices.Except(dbIndices).ToList();
            availableIndices.Sort();

            var lvItems = availableIndices.Select(d => new[] {$"{d}", "Available index", "In range" }).ToList();
            lvItems.Add(new[] {$"{dbIndices.Max() + 1}", "Available index", "Index is past DB maximum"});
            lvItems.Add(new[] {$"{DatabaseAPI.Database.Power.Length}", "Power DB Count", ""});

            AddLVPowersData(lvItems);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var text = string.Empty;
            if (CurrentQueryType is QueryType.PowerName or QueryType.StaticIndex)
            {
                text += $"Query: '{(CurrentQueryType == QueryType.PowerName ? tbPowerName.Text.Trim() : tbStaticIndex.Text.Trim())}' (by {(CurrentQueryType == QueryType.PowerName ? "power name" : "static index")})";
            }
            else
            {
                text += "Query: " + CurrentQueryType switch
                {
                    QueryType.FirstAvailableIndex => "First available index",
                    QueryType.HighestAvailableIndex => "Highest available index",
                    QueryType.AllAvailableIndices => "All available indices",
                    _ => ""
                };
            }

            text += "\r\n\r\n";
            text += string.Join("\r\n", LvItems.Select(it => string.Join("\t", it)));

            Clipboard.SetText(text);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbStaticIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            btnSearchByIndex.PerformClick();
        }

        private void tbPowerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            btnSearchByName.PerformClick();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == LvColumnSorter.SortColumn)
            {
                LvColumnSorter.Order = LvColumnSorter.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                LvColumnSorter.SortColumn = e.Column;
                LvColumnSorter.Order = SortOrder.Ascending;
            }

            listView1.Sort();
        }
    }
}