using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Design.Extensions;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowerBrowser : Form
    {
        private const int BrowserGroupIconTargetPixels = 18;
        private const int BrowserSetIconTargetPixels = 24;
        private const int BrowserPowerIconTargetPixels = 20;
        private const int BrowserPowerFullNameSubItemIndex = 4;
        private const int FILTER_ALL_POWERS = 3;

        private const int FILTER_ALL_SETS = 2;

        private const int FILTER_CLASSES = 1;

        private const int FILTER_GROUPS = 0;

        private const int FILTER_ORPHAN_POWERS = 5;

        private const int FILTER_ORPHAN_SETS = 4;

        private frmBusy BusyForm { get; set; }

        private bool _updating;
        private int[] _selected;

        public frmPowerBrowser()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Load += frmPowerBrowser_Load;
            _updating = false;
            InitializeComponent();
            ApplyMinimumIconLayout();
            Name = nameof(frmPowerBrowser);
            var componentResourceManager = new ComponentResourceManager(typeof(frmPowerBrowser));
            Icon = Resources.MRB_Icon_Concept;
        }

        private void ApplyMinimumIconLayout()
        {
            ilAT.ImageSize = CreateBrowserImageListSize(BrowserGroupIconTargetPixels);
            ilPS.ImageSize = CreateBrowserImageListSize(BrowserSetIconTargetPixels);
            ilPower.ImageSize = CreateBrowserImageListSize(BrowserPowerIconTargetPixels);
            lvPower.SmallImageList = ilPower;
        }

        private Size CreateBrowserImageListSize(int targetPixels)
        {
            var dpi = DeviceDpi > 0 ? DeviceDpi : 96;
            var scale = dpi / 96f;
            var logicalSize = Math.Max(16, (int)Math.Round(targetPixels / scale));
            return new Size(logicalSize, logicalSize);
        }

        internal static string GetPowerEditorFlags(IPower? power)
        {
            return power?.HiddenPower == true ? "H" : string.Empty;
        }

        private static string GetPowerBrowserFullName(ListViewItem item)
        {
            return item.SubItems.Count > BrowserPowerFullNameSubItemIndex
                ? item.SubItems[BrowserPowerFullNameSubItemIndex].Text
                : string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BusyMsg("Discarding Changes...");
            DatabaseAPI.LoadMainDatabase(MidsContext.Config.DataPath);
            DatabaseAPI.MatchAllIDs();
            BusyHide();
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0) return;
            e.Cancel = true;
            e.Message = "Required";
        }

        private void btnClassAdd_Click(object sender, EventArgs e)
        {
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        var inputResult = InputBox.Show($"Enter a name for the Powerset Group.\nNote: Upon adding a group you must add a set in order for it to be saved.", "New Powerset Group", false, "NewPowersetGroup", InputBox.InputBoxIcon.Info, inputBox_Validating);
                        if (inputResult.OK)
                        {
                            var iPsg = new PowersetGroup(inputResult.Text);
                            DatabaseAPI.Database.PowersetGroups.Add(inputResult.Text, iPsg);
                        }

                        break;
                    }
                case 1:
                    {
                        var iAt = new Archetype
                        {
                            ClassName = "Class_New",
                            DisplayName = "New Class"
                        };
                        using var frmEditArchetype = new frmEditArchetype(ref iAt);
                        var num = (int)frmEditArchetype.ShowDialog();
                        if (frmEditArchetype.DialogResult != DialogResult.OK)
                            return;
                        var classes = DatabaseAPI.Database.Classes.ToList();
                        classes.Add(new Archetype(frmEditArchetype.MyAT) { IsNew = true });
                        DatabaseAPI.Database.Classes = classes.ToArray();
                        Sort(0);
                        break;
                    }
            }
            UpdateLists(lvGroup.Items.Count - 1);
        }

        private void btnClassClone_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            var index = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var iAt = new Archetype(DatabaseAPI.Database.Classes[index]);
                iAt.ClassName += "_Clone";
                iAt.DisplayName += " (Clone)";
                using var frmEditArchetype = new frmEditArchetype(ref iAt);
                var num2 = (int)frmEditArchetype.ShowDialog();
                if (frmEditArchetype.DialogResult != DialogResult.OK)
                    return;
                var database = DatabaseAPI.Database;
                Archetype?[] archetypeArray = Array.Empty<Archetype>();
                Array.Copy(database.Classes, archetypeArray, DatabaseAPI.Database.Classes.Length + 1);
                database.Classes = archetypeArray;
                DatabaseAPI.Database.Classes[^1] = new Archetype(frmEditArchetype.MyAT) { IsNew = true };
                UpdateLists(lvGroup.Items.Count - 1);
                Sort(0);
            }
        }

        private void btnClassDelete_Click(object sender, EventArgs e)
        {
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        if (lvGroup.SelectedIndices.Count <= 0)
                            return;
                        var selectedGroup = lvGroup.SelectedItems[0].Text;
                        DatabaseAPI.Database.PowersetGroups.Remove(selectedGroup);
                        UpdateLists(lvGroup.Items.Count - 1);
                        break;
                    }
                case 1:
                    {
                        if (lvGroup.SelectedIndices.Count <= 0)
                            return;
                        var index1 = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
                        if (index1 < 0)
                        {
                            MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else if (MessageBox.Show($@"Really delete Class: {DatabaseAPI.Database.Classes[index1].ClassName} ({DatabaseAPI.Database.Classes[index1].DisplayName})?", @"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            var archetypeArray = new Archetype?[DatabaseAPI.Database.Classes.Length];
                            var index2 = 0;
                            var num3 = DatabaseAPI.Database.Classes.Length - 1;
                            for (var index3 = 0; index3 <= num3; ++index3)
                            {
                                if (index3 == index1)
                                    continue;
                                archetypeArray[index2] = new Archetype(DatabaseAPI.Database.Classes[index3]);
                                ++index2;
                            }

                            DatabaseAPI.Database.Classes = new Archetype?[DatabaseAPI.Database.Classes.Length - 1];
                            var num4 = DatabaseAPI.Database.Classes.Length - 1;
                            for (var index3 = 0; index3 <= num4; ++index3)
                                DatabaseAPI.Database.Classes[index3] = new Archetype(archetypeArray[index3]);
                            var group = 0;
                            if (lvGroup.Items.Count > 0)
                            {
                                if (lvGroup.Items.Count > index1)
                                    group = index1;
                                else if (lvGroup.Items.Count == index1)
                                    group = index1 - 1;
                            }

                            BusyMsg("Re-Indexing...");
                            DatabaseAPI.MatchAllIDs();
                            RefreshLists(group, 0, 0);
                            BusyHide();
                        }

                        break;
                    }
            }
        }

        private void btnClassDown_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvGroup.SelectedIndices[0];
            if (selectedIndex >= lvGroup.Items.Count - 1)
                return;
            Archetype?[] archetypeArray =
            {
                new Archetype(DatabaseAPI.Database.Classes[selectedIndex]),
                new Archetype(DatabaseAPI.Database.Classes[selectedIndex + 1])
            };
            DatabaseAPI.Database.Classes[selectedIndex + 1] = new Archetype(archetypeArray[0]);
            DatabaseAPI.Database.Classes[selectedIndex] = new Archetype(archetypeArray[1]);
            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            List_Groups(selectedIndex + 1);
            BusyHide();
        }

        private void btnClassEdit_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            var index = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var className = DatabaseAPI.Database.Classes[index].ClassName;
                using var frmEditArchetype = new frmEditArchetype(ref DatabaseAPI.Database.Classes[index]);
                if (frmEditArchetype.ShowDialog() != DialogResult.OK)
                    return;
                DatabaseAPI.Database.Classes[index] = new Archetype(frmEditArchetype.MyAT) { IsModified = true };
                if (DatabaseAPI.Database.Classes[index].ClassName != className)
                    RefreshLists();
            }
        }

        private void Sort(int type)
        {
            BusyMsg("Re-Indexing...");
            switch (type)
            {
                case 0:
                    {
                        Array.Sort(DatabaseAPI.Database.Classes);
                        break;
                    }
                case 1:
                    {
                        Array.Sort(DatabaseAPI.Database.Powersets);
                        break;
                    }
                case 2:
                    {
                        Array.Sort(DatabaseAPI.Database.Power);
                        break;
                    }
            }
            DatabaseAPI.MatchAllIDs();
            UpdateLists();
            BusyHide();
        }

        private void btnClassUp_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvGroup.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            Archetype?[] archetypeArray =
            {
                new Archetype(DatabaseAPI.Database.Classes[selectedIndex]),
                new Archetype(DatabaseAPI.Database.Classes[selectedIndex - 1])
            };
            DatabaseAPI.Database.Classes[selectedIndex - 1] = new Archetype(archetypeArray[0]);
            DatabaseAPI.Database.Classes[selectedIndex] = new Archetype(archetypeArray[1]);
            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            List_Groups(selectedIndex - 1);
            BusyHide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            BusyMsg(@"Re-Indexing && Saving...");
            foreach (var power in DatabaseAPI.Database.Power)
            {
                power.BaseRechargeTime = power.RechargeTime;
            }

            //Array.Sort(DatabaseAPI.Database.Power);
            var serializer = Serializer.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.MatchAllIDs();

            // Uncomment below to update AT modifier columns if necessary
            // for (var index = 0; index < DatabaseAPI.Database.Classes.Length; index++)
            // {
            //     DatabaseAPI.Database.Classes[index].Column = index;
            // }

            DatabaseAPI.SaveMainDatabase(serializer, MidsContext.Config.DataPath);
            MainWindow.MainInstance?.UpdateTitle();
            BusyHide();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnPowerAdd_Click(object sender, EventArgs e)
        {
            IPower? iPower = new Power();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        if (lvGroup.SelectedItems.Count > 0 & lvSet.SelectedItems.Count > 0)
                            iPower.FullName =
                                $"{lvGroup.SelectedItems[0].SubItems[0].Text}{lvSet.SelectedItems[0].SubItems[0].Text}.New_Power";
                        break;
                    }
                case 1 when (lvGroup.SelectedItems.Count > 0) & (lvSet.SelectedItems.Count > 0):
                    iPower.FullName =
                        $"{DatabaseAPI.Database.Classes[lvGroup.SelectedIndices[0]].PrimaryGroup}{lvSet.SelectedItems[0].SubItems[0].Text}.New_Power";
                    break;
            }

            iPower.DisplayName = "New Power";
            using var frmEditPower = new frmEditPower(iPower);
            var ret = frmEditPower.ShowDialog();
            BringToFront();
            if (ret != DialogResult.OK)
            {
                return;
            }

            var database = DatabaseAPI.Database;

            var powerList = database.Power.ToList();
            powerList.Add(new Power(frmEditPower.myPower) { IsNew = true });
            database.Power = powerList.ToArray();
            UpdateLists();
            Sort(2);

            if (_selected[0] < lvGroup.Items.Count)
            {
                lvGroup.Items[_selected[0]].Selected = true;
                lvGroup.Items[_selected[0]].EnsureVisible();
            }

            if (_selected[1] < lvSet.Items.Count)
            {
                lvSet.Items[_selected[1]].Selected = true;
                lvSet.Items[_selected[1]].EnsureVisible();
            }

            if (_selected[2] < lvPower.Items.Count)
            {
                lvPower.Items[_selected[2]].Selected = true;
                lvPower.Items[_selected[2]].EnsureVisible();
            }
        }

        private void btnPowerClone_Click(object sender, EventArgs e)
        {
            var index = DatabaseAPI.NidFromUidPower(GetPowerBrowserFullName(lvPower.SelectedItems[0]));
            if (index < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var database = DatabaseAPI.Database;
                var powerList = new List<IPower?>(DatabaseAPI.Database.Power);
                var newPower = powerList.First(x => x.FullName == database.Power[index].FullName).Clone();
                newPower.StaticIndex = powerList.Last().StaticIndex++;
                newPower.FullName += "_Clone";
                newPower.DisplayName += " (Clone)";
                newPower.PowerName += "_Clone";
                newPower.IsNew = true;
                newPower.PowerIndex = powerList.Count - 1;

                using var frmEditPower = new frmEditPower(newPower);
                var ret = frmEditPower.ShowDialog();
                BringToFront();
                if (ret != DialogResult.OK)
                {
                    return;
                }

                newPower = frmEditPower.myPower;
                powerList.Add(newPower);
                DatabaseAPI.Database.Power = powerList.ToArray();

                //Add the power to the power set otherwise we'll get issues later when updating the UI.
                if (newPower.PowerSetID > -1)
                {
                    var powerSet = DatabaseAPI.GetPowersetByName(newPower.FullName);
                    var psPowerList = powerSet?.Powers.ToList();
                    psPowerList?.Add(newPower);
                    if (powerSet != null)
                    {
                        powerSet.Powers = psPowerList.ToArray();
                    }
                }

                UpdateLists(lvGroup.SelectedIndices[0], lvSet.SelectedIndices[0]);
                Sort(2);
            }
        }

        private void btnPowerDelete_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0 || MessageBox.Show($@"Really delete Power: {GetPowerBrowserFullName(lvPower.SelectedItems[0])}?", @"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            IPower?[] powerArray = new IPower[DatabaseAPI.Database.Power.Length];
            var num1 = DatabaseAPI.NidFromUidPower(GetPowerBrowserFullName(lvPower.SelectedItems[0]));
            if (num1 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var index1 = 0;
                var num3 = DatabaseAPI.Database.Power.Length - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                {
                    if (index2 == num1)
                        continue;
                    powerArray[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                    ++index1;
                }

                DatabaseAPI.Database.Power = new IPower[DatabaseAPI.Database.Power.Length - 1];
                var num4 = DatabaseAPI.Database.Power.Length - 1;
                for (var index2 = 0; index2 <= num4; ++index2)
                    DatabaseAPI.Database.Power[index2] = new Power(powerArray[index2]);
                var selIdx = -1;
                if (lvPower.Items.Count > 0)
                {
                    if (lvPower.Items.Count > num1)
                        selIdx = num1;
                    else if (lvPower.Items.Count == num1)
                        selIdx = num1 - 1;
                }

                List_Powers(selIdx);
                Sort(2);
            }
        }

        private void btnPowerDown_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
            {
                return;
            }

            var selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex >= lvPower.Items.Count - 1)
            {
                return;
            }

            var selIdx = lvPower.SelectedIndices[0] + 1;
            var index1 = DatabaseAPI.NidFromUidPower(GetPowerBrowserFullName(lvPower.Items[selectedIndex]));
            var index2 = DatabaseAPI.NidFromUidPower(GetPowerBrowserFullName(lvPower.Items[selIdx]));
            if (index1 < 0 | index2 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            IPower? template = new Power(DatabaseAPI.Database.Power[index1]);
            DatabaseAPI.Database.Power[index1] = new Power(DatabaseAPI.Database.Power[index2]);
            DatabaseAPI.Database.Power[index2] = new Power(template);
            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            List_Powers(selIdx);
            BusyHide();
        }

        private void btnPowerEdit_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
            {
                return;
            }

            var text = GetPowerBrowserFullName(lvPower.SelectedItems[0]);
            var index1 = DatabaseAPI.NidFromUidPower(text);
            if (index1 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            using var frmEditPower = new frmEditPower(DatabaseAPI.Database.Power[index1], true);
            var ret = frmEditPower.ShowDialog();
            BringToFront();
            if (ret != DialogResult.OK)
            {
                return;
            }

            IPower? newPower = new Power(frmEditPower.myPower) { IsModified = true };
            DatabaseAPI.Database.Power[index1] = newPower;
            if (text == DatabaseAPI.Database.Power[index1].FullName)
            {
                return;
            }

            //Update the full power name in the powerset array
            if (newPower.PowerSetID > -1)
            {
                DatabaseAPI.Database.Powersets[newPower.PowerSetID].Powers[newPower.PowerSetIndex].FullName = newPower.FullName;
            }

            foreach (var p in DatabaseAPI.Database.Power[index1].Effects)
            {
                p.PowerFullName = DatabaseAPI.Database.Power[index1].FullName;
            }

            var strArray = DatabaseAPI.UidReferencingPowerFix(text, DatabaseAPI.Database.Power[index1].FullName);
            var str1 = strArray.Aggregate("", (current, t) => $"{current}{t}\r\n");
            if (strArray.Length > 0)
            {
                var str2 = $"Power: {text} changed to {DatabaseAPI.Database.Power[index1].FullName}\r\nThe following powers referenced this power and were updated:\r\n{str1}\r\n\r\nThis list has been placed on the clipboard.";
                Clipboard.SetDataObject(str2, true);
                MessageBox.Show(str2);
            }

            RefreshLists();
        }

        private void btnPowerUp_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
            {
                return;
            }

            var selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex < 1)
            {
                return;
            }

            var selIdx = lvPower.SelectedIndices[0] - 1;
            var index1 = DatabaseAPI.NidFromUidPower(GetPowerBrowserFullName(lvPower.Items[selectedIndex]));
            var index2 = DatabaseAPI.NidFromUidPower(GetPowerBrowserFullName(lvPower.Items[selIdx]));
            if (index1 < 0 | index2 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            IPower? template = new Power(DatabaseAPI.Database.Power[index1]);
            DatabaseAPI.Database.Power[index1] = new Power(DatabaseAPI.Database.Power[index2]);
            DatabaseAPI.Database.Power[index2] = new Power(template);
            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            List_Powers(selIdx);
            BusyHide();
        }

        private void btnPSDown_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
            {
                return;
            }

            var selectedIndex = lvSet.SelectedIndices[0];
            if (selectedIndex >= lvSet.Items.Count - 1)
            {
                return;
            }

            var selIdx = lvSet.SelectedIndices[0] + 1;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selIdx].SubItems[3].Text);
            if (index1 < 0 | index2 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            IPowerset? template = new Powerset(DatabaseAPI.Database.Powersets[index1]);
            DatabaseAPI.Database.Powersets[index1] = new Powerset(DatabaseAPI.Database.Powersets[index2]);
            DatabaseAPI.Database.Powersets[index2] = new Powerset(template);
            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            List_Sets(selIdx);
            BusyHide();
        }

        private void btnPSUp_Click(object sender, EventArgs e)

        {
            if (lvSet.SelectedIndices.Count <= 0)
            {
                return;
            }

            var selectedIndex = lvSet.SelectedIndices[0];
            if (selectedIndex < 1)
            {
                return;
            }

            var selIdx = lvSet.SelectedIndices[0] - 1;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selIdx].SubItems[3].Text);
            if (index1 < 0 | index2 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            IPowerset? template = new Powerset(DatabaseAPI.Database.Powersets[index1]);
            DatabaseAPI.Database.Powersets[index1] = new Powerset(DatabaseAPI.Database.Powersets[index2]);
            DatabaseAPI.Database.Powersets[index2] = new Powerset(template);
            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            List_Sets(selIdx);
            BusyHide();
        }

        private void btnSetAdd_Click(object sender, EventArgs e)
        {
            IPowerset? iSet = new Powerset();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        if (lvGroup.SelectedItems.Count > 0)
                            iSet.FullName = $"{lvGroup.SelectedItems[0].SubItems[0].Text}.New_Set";
                        break;
                    }
                case 1 when lvGroup.SelectedItems.Count > 0:
                    iSet.FullName = $"{DatabaseAPI.Database.Classes[lvGroup.SelectedIndices[0]].PrimaryGroup}.New_Set";
                    break;
            }

            iSet.DisplayName = "New Set";
            using var frmEditPowerset = new frmEditPowerset(ref iSet);
            var ret = frmEditPowerset.ShowDialog();
            if (ret != DialogResult.OK)
            {
                return;
            }

            var database = DatabaseAPI.Database;
            var psList = database.Powersets.ToList();
            psList.Add(new Powerset(frmEditPowerset.MyPowerSet) { IsNew = true, nID = psList.Count + 1 });
            DatabaseAPI.Database.Powersets = psList.ToArray();
            UpdateLists();
            Sort(1);

            if (_selected[0] < lvGroup.Items.Count)
            {
                lvGroup.Items[_selected[0]].Selected = true;
                lvGroup.Items[_selected[0]].EnsureVisible();
            }

            if (_selected[1] < lvSet.Items.Count)
            {
                lvSet.Items[_selected[1]].Selected = true;
                lvSet.Items[_selected[1]].EnsureVisible();
            }

            if (_selected[2] < lvPower.Items.Count)
            {
                lvPower.Items[_selected[2]].Selected = true;
                lvPower.Items[_selected[2]].EnsureVisible();
            }
        }

        private void btnSetDelete_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
            {
                return;
            }

            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (index1 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            var str = "";
            if (DatabaseAPI.Database.Powersets[index1].Powers.Length > 0)
            {
                str =
                    $"{DatabaseAPI.Database.Powersets[index1].FullName} still has powers attached to it.\r\nThese powers will be orphaned if you remove the set.\r\n\r\n";
            }

            if (MessageBox.Show($@"{str} Really delete Powerset: {DatabaseAPI.Database.Powersets[index1].DisplayName}?", @"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            var powersetArray = new IPowerset?[DatabaseAPI.Database.Powersets.Length];
            var index2 = 0;
            for (var index3 = 0; index3 < DatabaseAPI.Database.Powersets.Length; index3++)
            {
                if (index3 == index1)
                {
                    continue;
                }

                powersetArray[index2] = new Powerset(DatabaseAPI.Database.Powersets[index3]);
                index2++;
            }

            DatabaseAPI.Database.Powersets = new IPowerset?[DatabaseAPI.Database.Powersets.Length - 1];
            for (var index3 = 0; index3 < DatabaseAPI.Database.Powersets.Length; index3++)
            {
                DatabaseAPI.Database.Powersets[index3] = new Powerset(powersetArray[index3]) { nID = index3 };
            }

            var powerset = -1;
            if (lvSet.Items.Count > 0)
            {
                if (lvSet.Items.Count > index1)
                {
                    powerset = index1;
                }
                else if (lvSet.Items.Count == index1)
                {
                    powerset = index1 - 1;
                }
            }

            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            RefreshLists(-1, powerset);
            BusyHide();
        }

        private void btnSetEdit_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
            {
                return;
            }

            var Powerset = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (Powerset < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            var powerset = DatabaseAPI.Database.Powersets[Powerset];
            var fullName = powerset.FullName;
            using var frmEditPowerset = new frmEditPowerset(ref powerset);
            if (frmEditPowerset.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            DatabaseAPI.Database.Powersets[Powerset] = new Powerset(frmEditPowerset.MyPowerSet) { IsModified = true };
            if (DatabaseAPI.Database.Powersets[Powerset].FullName == fullName)
            {
                return;
            }

            BusyMsg("Re-Indexing...");
            DatabaseAPI.MatchAllIDs();
            RefreshLists(-1, Powerset);
            BusyHide();
        }

        private async void BuildATImageList()

        {
            ilAT.Images.Clear();
            foreach (var kvp in AssetManager.Archetypes.OrderBy(x => x.Key))
            {
                var archetypeIcon = kvp.Value;
                if (archetypeIcon?.Bitmap != null)
                {
                    ilAT.Images.Add(new Bitmap(archetypeIcon.Bitmap));
                }
                else
                {
                    // Add a blank image as a fallback
                    ilAT.Images.Add(new Bitmap(ilAT.ImageSize.Width, ilAT.ImageSize.Height));
                }
            }
        }

        private void BuildPowersetImageList(IReadOnlyList<int> iSets)
        {
            ilPS.Images.Clear();
            using var canvasBitmap = new ExtendedBitmap(ilPS.ImageSize);
            if (canvasBitmap.Graphics == null) return;

            using var blackLabelBrush = new SolidBrush(Color.Black);
            using var whiteLabelBrush = new SolidBrush(Color.White);
            using var format = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using var font = new Font(Font.FontFamily, 6.75f, FontStyle.Bold, GraphicsUnit.Point);
            var iconBounds = new Rectangle(0, 0, ilPS.ImageSize.Width, ilPS.ImageSize.Height);
            var badgeSize = Math.Max(6, (int)Math.Round(ilPS.ImageSize.Width * 0.38f));
            var badgeBounds = new Rectangle(iconBounds.Right - badgeSize - 2, iconBounds.Bottom - badgeSize - 2, badgeSize, badgeSize);

            foreach (var setIndex in iSets)
            {
                var powerset = DatabaseAPI.Database.Powersets[setIndex];
                if (powerset == null) continue;

                // Use the helper to get the pre-cached image
                var powersetImage = AssetManager.GetPowersetImage(powerset);

                string label;
                SolidBrush labelBrush;
                Color backgroundColor;

                switch (powerset.SetType)
                {
                    case Enums.ePowerSetType.Primary:
                        backgroundColor = Color.Blue;
                        label = "1";
                        labelBrush = whiteLabelBrush;
                        break;
                    case Enums.ePowerSetType.Secondary:
                        backgroundColor = Color.Red;
                        label = "2";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Ancillary:
                        backgroundColor = Color.Green;
                        label = "A";
                        labelBrush = whiteLabelBrush;
                        break;
                    case Enums.ePowerSetType.Inherent:
                        backgroundColor = Color.Silver;
                        label = "I";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Pool:
                        backgroundColor = Color.Cyan;
                        label = "P";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Accolade:
                        backgroundColor = Color.Goldenrod;
                        label = "+";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Temp:
                        backgroundColor = Color.WhiteSmoke;
                        label = "T";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Pet:
                        backgroundColor = Color.Brown;
                        label = "x";
                        labelBrush = whiteLabelBrush;
                        break;
                    case Enums.ePowerSetType.Redirect:
                        backgroundColor = Color.BlueViolet;
                        label = "R";
                        labelBrush = whiteLabelBrush;
                        break;
                    case Enums.ePowerSetType.SetBonus:
                        backgroundColor = Color.LightSeaGreen;
                        label = "S";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Boost:
                        backgroundColor = Color.LightSeaGreen;
                        label = "B";
                        labelBrush = blackLabelBrush;
                        break;
                    case Enums.ePowerSetType.Incarnate:
                        backgroundColor = Color.SandyBrown;
                        label = "X";
                        labelBrush = blackLabelBrush;
                        break;
                    default:
                        backgroundColor = Color.White;
                        label = "";
                        labelBrush = blackLabelBrush;
                        break;
                }

                canvasBitmap.Graphics.Clear(Color.Transparent);
                if (powersetImage?.Bitmap != null)
                {
                    DbEditorIconLayout.DrawImageAspectFit(canvasBitmap.Graphics, powersetImage.Bitmap, iconBounds);
                }

                if (!string.IsNullOrWhiteSpace(label))
                {
                    using var badgeBrush = new SolidBrush(Color.FromArgb(224, backgroundColor));
                    canvasBitmap.Graphics.FillRectangle(badgeBrush, badgeBounds);
                    canvasBitmap.Graphics.DrawString(label, font, labelBrush, badgeBounds, format);
                }

                ilPS.Images.Add(new Bitmap(canvasBitmap.Bitmap));
            }
        }

        private void BusyMsg(string sMessage)
        {
            BusyForm = new frmBusy();
            BusyForm.SetTitle(@"Please wait");
            BusyForm.Show(this);
            BusyForm.SetMessage(sMessage);
        }

        private void BusyHide()
        {
            if (BusyForm == null)
                return;
            BusyForm.Completed();
            BusyForm = null;
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (_updating)
                return;
            var buttons = new List<Button>
            {
                btnClassClone,
                btnClassDown,
                btnClassEdit,
                btnClassUp
            };
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    foreach (Button btn in pnlGroup.Controls)
                    {
                        if (buttons.Any(b => b == btn))
                        {
                            btn.Enabled = false;
                        }
                    }

                    lvGroup.Sorting = SortOrder.Ascending;
                    break;
                case 1:
                    foreach (Button btn in pnlGroup.Controls)
                    {
                        if (buttons.Any(b => b == btn))
                        {
                            btn.Enabled = true;
                        }
                    }

                    lvGroup.Sorting = SortOrder.None;
                    break;
            }
            UpdateLists();
        }

        private void FillFilter()
        {
            cbFilter.BeginUpdate();
            cbFilter.Items.Clear();
            cbFilter.Items.Add("Groups");
            cbFilter.Items.Add("Archetype Classes");
            cbFilter.Items.Add("All Sets");
            cbFilter.Items.Add("All Powers");
            cbFilter.Items.Add("Orphan Sets");
            cbFilter.Items.Add("Orphan Powers");
            cbFilter.EndUpdate();
            cbFilter.SelectedIndex = cbFilter.Items.IndexOf("Groups");
        }

        private void frmPowerBrowser_Load(object sender, EventArgs e)
        {
            ApplyMinimumIconLayout();
            lvGroup.EnableDoubleBuffer();
            lvSet.EnableDoubleBuffer();
            lvPower.EnableDoubleBuffer();
            btnManageHiddenPowers.Visible = MidsContext.Config.MasterMode;
            btnDbQueries.Visible = MidsContext.Config.MasterMode;
            Text = $"Power Database Browser [{DatabaseAPI.DatabaseName} DB]";
            _selected = [0, 0, 0];

            try
            {
                FillFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            }
        }

        [DebuggerStepThrough]
        private void List_Groups(int selIdx)
        {
            _updating = true;
            lvGroup.BeginUpdate();
            lvGroup.Items.Clear();
            BuildATImageList();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    foreach (var powersetGroup in DatabaseAPI.Database.PowersetGroups.Values)
                    {
                        var imageIndex = -1;
                        for (var index = 0; index < DatabaseAPI.Database.Classes.Length; index++)
                        {
                            if (!(string.Equals(DatabaseAPI.Database.Classes[index].PrimaryGroup, powersetGroup.Name, StringComparison.OrdinalIgnoreCase) | string.Equals(DatabaseAPI.Database.Classes[index].SecondaryGroup, powersetGroup.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                continue;
                            }

                            imageIndex = index;
                            break;
                        }

                        if (imageIndex > -1)
                            lvGroup.Items.Add(new ListViewItem(powersetGroup.Name, imageIndex));
                        else
                            lvGroup.Items.Add(powersetGroup.Name);
                    }

                    lvGroup.Columns[0].Text = @"Group";
                    lvGroup.Columns[0].Width = -2;
                    lvGroup.Enabled = true;
                    pnlGroup.Enabled = true;
                    break;

                case 1:
                    for (var imageIndex = 0; imageIndex < DatabaseAPI.Database.Classes.Length; imageIndex++)
                    {
                        lvGroup.Items.Add(new ListViewItem(DatabaseAPI.Database.Classes[imageIndex].ClassName,
                            imageIndex));
                    }

                    lvGroup.Columns[0].Text = @"Class";
                    lvGroup.Columns[0].Width = -2;
                    lvGroup.Enabled = true;
                    pnlGroup.Enabled = true;
                    break;

                default:
                    lvGroup.Columns[0].Text = "";
                    lvGroup.Enabled = false;
                    pnlGroup.Enabled = false;
                    break;
            }

            if (lvGroup.Items.Count > 0)
            {
                if ((lvGroup.Items.Count > selIdx) & (selIdx > -1))
                {
                    lvGroup.Items[selIdx].Selected = true;
                    lvGroup.Items[selIdx].EnsureVisible();
                }
                else
                {
                    lvGroup.Items[0].Selected = true;
                    lvGroup.Items[0].EnsureVisible();
                }
            }

            lvGroup.EndUpdate();
            _updating = false;
        }

        private void List_Power_AddBlock(IReadOnlyList<int> iPowers, bool displayFullName)
        {
            if (iPowers.Count < 1)
            {
                return;
            }

            foreach (var p in iPowers)
            {
                if (p <= -1)
                {
                    continue;
                }

                AddPowerListItem(DatabaseAPI.Database.Power[p], p, displayFullName);
            }
        }

        private void List_Power_AddBlock(IReadOnlyList<string> iPowers, bool displayFullName)
        {
            if (iPowers.Count < 1)
            {
                return;
            }

            foreach (var p in iPowers)
            {
                var index2 = DatabaseAPI.NidFromUidPower(p);
                if (index2 <= -1)
                {
                    continue;
                }

                AddPowerListItem(DatabaseAPI.Database.Power[index2], index2, displayFullName);
            }
        }

        private void AddPowerListItem(IPower power, int powerIndex, bool displayFullName)
        {
            var items = new string[5];
            items[0] = !displayFullName ? power.PowerName : power.FullName;
            items[1] = power.DisplayName;
            items[2] = Convert.ToString(power.Level, CultureInfo.InvariantCulture);
            items[3] = GetPowerEditorFlags(power);
            items[4] = power.FullName;

            var item = new ListViewItem(items)
            {
                Tag = powerIndex
            };

            var powerImage = AssetManager.GetPowerImage(power);
            if (powerImage?.Bitmap != null)
            {
                ilPower.Images.Add(new Bitmap(powerImage.Bitmap));
                item.ImageIndex = ilPower.Images.Count - 1;
            }

            lvPower.Items.Add(item);
        }

        private void List_Powers(int selIdx)
        {
            var iPowers1 = Array.Empty<int>();
            var iPowers2 = Array.Empty<string>();
            var displayFullName = false;
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    if (lvSet.SelectedItems.Count > 0)
                        iPowers2 = DatabaseAPI.UidPowers(lvSet.SelectedItems[0].SubItems[3].Text);
                    break;
                case 1:
                    if (lvSet.SelectedItems.Count > 0)
                    {
                        var uidClass = "";
                        if (lvGroup.SelectedItems.Count > 0)
                            uidClass = lvGroup.SelectedItems[0].SubItems[0].Text;
                        iPowers2 = DatabaseAPI.UidPowers(lvSet.SelectedItems[0].SubItems[3].Text, uidClass);
                    }

                    break;
                case 2:
                    if (lvSet.SelectedItems.Count > 0)
                    {
                        if (lvSet.SelectedItems[0].SubItems[3].Text != "")
                            iPowers2 = DatabaseAPI.UidPowers(lvSet.SelectedItems[0].SubItems[3].Text);
                        else if (lvSet.SelectedItems[0].SubItems[4].Text != "")
                            iPowers1 = DatabaseAPI.NidPowers(
                                (int)Math.Round(Convert.ToDouble(lvSet.SelectedItems[0].SubItems[4].Text)));
                    }

                    break;
                case 4:
                    if (lvSet.SelectedItems.Count > 0)
                    {
                        int index;
                        if (lvSet.SelectedItems[0].SubItems[4].Text == "")
                        {
                            index = -1;
                        }
                        else
                        {
                            index = (int)Math.Round(Convert.ToDouble(lvSet.SelectedItems[0].SubItems[4].Text));
                        }

                        if (index > -1)
                        {
                            iPowers1 = new int[DatabaseAPI.Database.Powersets[index].Power.Length];
                            Array.Copy(DatabaseAPI.Database.Powersets[index].Power, iPowers1, iPowers1.Length);
                        }
                    }

                    break;
                case 5:
                    for (var index = 0; index < DatabaseAPI.Database.Power.Length; index++)
                    {
                        if (!((DatabaseAPI.Database.Power[index].GroupName == "") | (DatabaseAPI.Database.Power[index].SetName == "") | (DatabaseAPI.Database.Power[index].GetPowerSet() == null)))
                            continue;

                        Array.Resize(ref iPowers1, iPowers1.Length + 1);
                        iPowers1[^1] = index;
                    }

                    displayFullName = true;
                    break;
                case 3:
                    BusyMsg("Building List...");
                    iPowers1 = new int[DatabaseAPI.Database.Power.Length];
                    for (var index = 0; index < DatabaseAPI.Database.Power.Length; index++)
                    {
                        iPowers1[index] = index;
                    }

                    displayFullName = true;
                    break;
            }

            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            ilPower.Images.Clear();
            lblPower.Text = string.Empty;
            if (iPowers2.Length > 0)
            {
                List_Power_AddBlock(iPowers2, displayFullName);
            }
            else
            {
                List_Power_AddBlock(iPowers1, displayFullName);
            }

            BusyHide();
            if (lvPower.Items.Count > 0)
            {
                if ((selIdx > -1) & (selIdx < lvPower.Items.Count))
                {
                    lvPower.Items[selIdx].Selected = true;
                    lvPower.Items[selIdx].EnsureVisible();
                }
                else
                {
                    lvPower.Items[0].Selected = true;
                    lvPower.Items[0].EnsureVisible();
                }
            }

            lvPower.EndUpdate();
            pnlPower.Enabled = lvPower.Enabled;
        }

        private void List_Sets(int selIdx)
        {
            if (lvGroup.SelectedItems.Count == 0 & (cbFilter.SelectedIndex == 0 | cbFilter.SelectedIndex == 1))
                return;
            _updating = true;
            lvSet.BeginUpdate();
            lvSet.Items.Clear();
            if (cbFilter.SelectedIndex == 0 & lvGroup.SelectedItems.Count > 0)
            {
                var iSets = DatabaseAPI.NidSets(lvGroup.SelectedItems[0].SubItems[0].Text, "", Enums.ePowerSetType.None);
                BuildPowersetImageList(iSets);
                List_Sets_AddBlock(iSets);
                lvSet.Enabled = true;
            }
            else if (cbFilter.SelectedIndex == 1 & lvGroup.SelectedItems.Count > 0)
            {
                var iSets = DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Primary)
                    .Concat(DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Secondary))
                    .Concat(DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Ancillary))
                    .Concat(DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Inherent))
                    .Concat(DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Pool))
                    .ToArray();

                BuildPowersetImageList(iSets);
                List_Sets_AddBlock(iSets);
                lvSet.Enabled = true;
            }
            else
            {
                switch (cbFilter.SelectedIndex)
                {
                    case 4:
                        var ps = new List<int>();
                        for (var index = 0; index < DatabaseAPI.Database.Powersets.Length; index++)
                        {
                            if (!(DatabaseAPI.Database.Powersets[index].GetGroup() == null |
                                  string.IsNullOrEmpty(DatabaseAPI.Database.Powersets[index].GroupName)))
                                continue;
                            ps.Add(index);
                        }

                        var numArray3 = ps.ToArray();
                        BuildPowersetImageList(numArray3);
                        List_Sets_AddBlock(numArray3);
                        lvSet.Enabled = true;
                        break;
                    case 2:
                        BusyMsg("Building List...");
                        var iSets = DatabaseAPI.NidSets("", "", Enums.ePowerSetType.None);
                        BuildPowersetImageList(iSets);
                        List_Sets_AddBlock(iSets);
                        lvSet.Enabled = true;
                        break;
                    default:
                        lvSet.Enabled = false;
                        break;
                }
            }

            if (lvSet.Items.Count > 0)
            {
                if (lvSet.Items.Count > selIdx & selIdx > -1)
                {
                    lvSet.Items[selIdx].Selected = true;
                    lvSet.Items[selIdx].EnsureVisible();
                }
                else
                {
                    lvSet.Items[0].Selected = true;
                    lvSet.Items[0].EnsureVisible();
                }
            }

            lvSet.EndUpdate();
            BusyHide();
            pnlSet.Enabled = lvSet.Enabled;
            _updating = false;
            lvSet_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void List_Sets_AddBlock(IReadOnlyList<int> iSets)
        {
            var items = new string[5];
            if (iSets.Count < 1)
                return;
            for (var imageIndex = 0; imageIndex < iSets.Count; imageIndex++)
            {
                if (iSets[imageIndex] <= -1)
                    continue;
                items[0] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].SetName;
                items[1] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].DisplayName;
                items[2] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].SetType switch
                {
                    Enums.ePowerSetType.Primary => "Pri",
                    Enums.ePowerSetType.Secondary => "Sec",
                    Enums.ePowerSetType.Ancillary => "Epic",
                    Enums.ePowerSetType.Inherent => "Inh",
                    Enums.ePowerSetType.Pool => "Pool",
                    Enums.ePowerSetType.Accolade => "Acc",
                    _ => ""
                };
                items[3] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].FullName;
                items[4] = Convert.ToString(iSets[imageIndex]);
                lvSet.Items.Add(new ListViewItem(items, imageIndex));
            }
        }

        private void lvGroup_DoubleClick(object sender, EventArgs e)
        {
            if (cbFilter.SelectedIndex != 1)
                return;
            btnClassEdit_Click(this, EventArgs.Empty);
        }

        private void lvGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updating)
            {
                return;
            }

            if (lvGroup.SelectedIndices.Count > 0)
            {
                _selected[0] = lvGroup.SelectedIndices[0];
            }

            List_Sets(0);
            Application.DoEvents();
            List_Powers(0);
        }

        private void lvPower_DoubleClick(object sender, EventArgs e)
        {
            btnPowerEdit_Click(this, EventArgs.Empty);
        }

        private void lvPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPower.SelectedItems.Count <= 0)
            {
                lblPower.Text = string.Empty;
                _selected[2] = -1;
                return;
            }

            lblPower.Text = GetPowerBrowserFullName(lvPower.SelectedItems[0]);
            _selected[2] = lvPower.SelectedIndices[0];
        }

        private void lvSet_DoubleClick(object sender, EventArgs e)
        {
            btnSetEdit_Click(this, EventArgs.Empty);
        }

        private void lvSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updating)
            {
                return;
            }

            if (lvSet.SelectedItems.Count > 0)
            {
                var setArchetypes = "";
                lblSet.Text = lvSet.SelectedItems[0].SubItems[3].Text;
                _selected[1] = lvSet.SelectedIndices[0];

                if (DatabaseAPI.GetPowersetByFullname(lvSet.SelectedItems[0].SubItems[3].Text)?.SetType == Enums.ePowerSetType.Ancillary)
                {
                    var ps = DatabaseAPI.GetPowersetByFullname(lvSet.SelectedItems[0].SubItems[3].Text);
                    var powersetsArchetypes = DatabaseAPI.Database.Classes
                        .Where(t => t is { Playable: true })
                        .Where(t => DatabaseAPI.GetPowersetIndexes(t, Enums.ePowerSetType.Ancillary).Any(p => p.Equals(ps)))
                        .Select(t => t.DisplayName)
                        .OrderBy(t => t)
                        .ToList();

                    if (powersetsArchetypes.Count > 0)
                    {
                        lblSet.Text += $" [{string.Join(", ", powersetsArchetypes)}]";
                    }
                }
            }

            List_Powers(0);
        }

        private void RefreshLists(int group = -1, int powerset = -1, int power = -1)
        {
            var selectGroup = group;
            var selectSet = powerset;
            var selectPower = power;
            if (lvGroup.SelectedIndices.Count > 0 & selectGroup == -1)
            {
                selectGroup = lvGroup.SelectedIndices[0];
            }

            if (lvSet.SelectedIndices.Count > 0 & selectSet == -1)
            {
                selectSet = lvSet.SelectedIndices[0];
            }

            if (lvPower.SelectedIndices.Count > 0 & selectPower == -1)
            {
                selectPower = lvPower.SelectedIndices[0];
            }

            UpdateLists(selectGroup, selectSet, selectPower);
        }

        private void UpdateLists(int selectGroup = -1, int selectSet = -1, int selectPower = -1)
        {
            List_Groups(selectGroup);
            Application.DoEvents();
            List_Sets(selectSet);
            Application.DoEvents();
            List_Powers(selectPower);
        }

        private void btnManageHiddenPowers_Click(object sender, EventArgs e)
        {
            using var f = new frmRestoreHidden();
            var ret = f.ShowDialog();
            if (ret == DialogResult.OK)
            {
                RefreshLists();
            }
        }

        private void btnMassOp_Click(object sender, EventArgs e)
        {
            // [Zed] This is a "quick and dirty" mass processing routine.
            // Todo: turn this into a real search/process form.

            return;

        }

        private void btnDbQueries_Click(object sender, EventArgs e)
        {
            using var f = new frmDbQueries();

            f.ShowDialog();
        }

        private void btnDbDiff_Click(object sender, EventArgs e)
        {
            using var f = new frmDbDiff();

            f.ShowDialog();
        }
    }
}
