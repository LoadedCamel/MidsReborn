using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Extensions;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDbQueries : Form
    {
        private class ListViewColumnSorter : IComparer
        {
            public int SortColumn { get; set; }

            public SortOrder Order { get; set; }

            private List<string[]> LvData;

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
            AllAvailableIndices,
            ListStaticIndices,
            OrphanEntities,
            FindDuplicateIndices
        }

        private List<string[]> LvItems;
        private QueryType CurrentQueryType;
        private ListViewColumnSorter LvColumnSorter;

        public frmDbQueries()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
            listView1.EnableDoubleBuffer();
            LvColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = LvColumnSorter;
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
            LvItems = iPowers.Select(pw => new Power(pw))
                .Select(pw => new[] { $"{pw.StaticIndex}", pw.DisplayName, pw.FullName })
                .ToList();
            listView1.VirtualListSize = 0;
            listView1.VirtualListSize = LvItems.Count;
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

            var iPowers = DatabaseAPI.Database.Power
                .Where(pw => pw != null && string.Equals(pw.DisplayName, pName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            LvItems = iPowers.Select(pw => new Power(pw))
                .Select(pw => new[] {$"{pw.StaticIndex}", pw.DisplayName, pw.FullName})
                .ToList();
            listView1.VirtualListSize = 0;
            listView1.VirtualListSize = LvItems.Count;
        }

        private void btnFirstAvailableIndex_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.FirstAvailableIndex;

            var dbIndices = DatabaseAPI.Database.Power.Select(pw => pw?.StaticIndex ?? 0);
            var indices = Enumerable.Range(0, dbIndices.Max() + 1);

            var availableIndices = indices.Except(dbIndices).ToList();
            availableIndices.Sort();

            LvItems = new List<string[]>
            {
                new[] {$"{availableIndices[0]}", "First available", ""},
                new[] {$"{DatabaseAPI.Database.Power.Length}", "Power DB Count", ""}
            };
            listView1.VirtualListSize = LvItems.Count;
        }

        private void btnHighestAvailableIndex_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.HighestAvailableIndex;

            var dbIndices = DatabaseAPI.Database.Power.Select(pw => pw?.StaticIndex ?? 0);

            LvItems = new List<string[]>
            {
                new[] {$"{dbIndices.Max() + 1}", "Highest available", ""},
                new[] {$"{DatabaseAPI.Database.Power.Length}", "Power DB Count", ""}
            };
            listView1.VirtualListSize = LvItems.Count;
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

            LvItems = lvItems;
            listView1.VirtualListSize = LvItems.Count;
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
                    QueryType.ListStaticIndices => "List static indices with matched powers",
                    QueryType.OrphanEntities => "Find orphan entities",
                    QueryType.FindDuplicateIndices => "Find duplicate indices",
                    _ => CurrentQueryType.ToString()
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

        private void btnListIndices_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.ListStaticIndices;
            var powersList = DatabaseAPI.Database.Power.Where(pw => pw != null).ToList();
            LvItems = powersList.Select(pw => new[] { $"{pw.StaticIndex}", pw.DisplayName, pw.FullName }).ToList();
            listView1.VirtualListSize = LvItems.Count;
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) => e.Item = new ListViewItem(LvItems[e.ItemIndex]);

        private void btnOrphanEntities_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.OrphanEntities;
            var itemsList = DatabaseAPI.Database.Power.Select(e => new KeyValuePair<IPower, List<IEffect>>(e,
                e.Effects.Where(e => e.EffectType == Enums.eEffectType.EntCreate & e.nSummon < 0 & !string.IsNullOrEmpty(e.Summon)).ToList()));

            LvItems = itemsList.SelectMany(e => e.Value, (k, v) => new[] {$"{k.Key.StaticIndex}", v.Summon, k.Key.FullName}).ToList();
            listView1.VirtualListSize = LvItems.Count;
        }

        private void btnDuplicateIndices_Click(object sender, EventArgs e)
        {
            CurrentQueryType = QueryType.FindDuplicateIndices;
            var indexList = DatabaseAPI.Database.Power
                .Select(e => e.StaticIndex)
                .GroupBy(e => e)
                .Where(e => e.Count() > 1)
                .Select(e => e.Key)
                .ToList();

            var itemsList = DatabaseAPI.Database.Power
                .Where(e => indexList.Contains(e.StaticIndex))
                .OrderBy(e => e.StaticIndex)
                .ToList();

            LvItems = itemsList.Count > 0
                ? itemsList.Select(e => new[] {$"{e.StaticIndex}", e.DisplayName, e.FullName}).ToList()
                : new List<string[]> {new[] {"", "Nothing found", ""}};
            listView1.VirtualListSize = LvItems.Count;
        }
    }
}