using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Forms.Controls;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;
using mrbBase.Base.Extensions;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmPowerBrowser : Form
    {
        private const int FILTER_ALL_POWERS = 3;

        private const int FILTER_ALL_SETS = 2;

        private const int FILTER_CLASSES = 1;

        private const int FILTER_GROUPS = 0;

        private const int FILTER_ORPHAN_POWERS = 5;

        private const int FILTER_ORPHAN_SETS = 4;

        private frmBusy BusyForm { get; set; }

        private bool _updating;

        public frmPowerBrowser()
        {
            Load += frmPowerBrowser_Load;
            _updating = false;
            InitializeComponent();
            Name = nameof(frmPowerBrowser);
            var componentResourceManager = new ComponentResourceManager(typeof(frmPowerBrowser));
            Icon = Resources.reborn;
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
                    var database = DatabaseAPI.Database;
                    var archetypeArray = Array.Empty<Archetype>();
                    Array.Copy(database.Classes, archetypeArray, DatabaseAPI.Database.Classes.Length + 1);
                    database.Classes = archetypeArray;
                    DatabaseAPI.Database.Classes[DatabaseAPI.Database.Classes.Length - 1] = new Archetype(frmEditArchetype.MyAT) { IsNew = true };
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
                var archetypeArray = Array.Empty<Archetype>();
                Array.Copy(database.Classes, archetypeArray, DatabaseAPI.Database.Classes.Length + 1);
                database.Classes = archetypeArray;
                DatabaseAPI.Database.Classes[DatabaseAPI.Database.Classes.Length - 1] = new Archetype(frmEditArchetype.MyAT) { IsNew = true };
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
                        var archetypeArray = new Archetype[DatabaseAPI.Database.Classes.Length - 1 + 1];
                        var index2 = 0;
                        var num3 = DatabaseAPI.Database.Classes.Length - 1;
                        for (var index3 = 0; index3 <= num3; ++index3)
                        {
                            if (index3 == index1)
                                continue;
                            archetypeArray[index2] = new Archetype(DatabaseAPI.Database.Classes[index3]);
                            ++index2;
                        }

                        DatabaseAPI.Database.Classes = new Archetype[DatabaseAPI.Database.Classes.Length - 2 + 1];
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
            Archetype[] archetypeArray =
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
            Archetype[] archetypeArray =
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

            Array.Sort(DatabaseAPI.Database.Power);
            var serializer = Serializer.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.MatchAllIDs();

            // Uncomment below to update AT modifier columns if necessary
            // for (var index = 0; index < DatabaseAPI.Database.Classes.Length - 1; index++)
            // {
            //     DatabaseAPI.Database.Classes[index].Column = index;
            // }
            DatabaseAPI.SaveMainDatabase(serializer, MidsContext.Config.DataPath);
            frmMain.MainInstance.UpdateTitle();
            BusyHide();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnPowerAdd_Click(object sender, EventArgs e)
        {
            IPower iPower = new Power();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        if ((lvGroup.SelectedItems.Count > 0) & (lvSet.SelectedItems.Count > 0))
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
            if (frmEditPower.ShowDialog() != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;

            var powerList = database.Power.ToList();
            powerList.Add(new Power(frmEditPower.myPower) { IsNew = true });
            database.Power = powerList.ToArray();
            UpdateLists();
            Sort(2);
        }

        private void btnPowerClone_Click(object sender, EventArgs e)
        {
            var index = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
            if (index < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var database = DatabaseAPI.Database;
                var powerList = new List<IPower>(DatabaseAPI.Database.Power);
                var newPower = powerList.First(x => x.FullName == database.Power[index].FullName).Clone();
                newPower.StaticIndex = powerList.Last().StaticIndex++;
                newPower.FullName += "_Clone";
                newPower.DisplayName += " (Clone)";
                newPower.PowerName += "_Clone";
                newPower.IsNew = true;
                newPower.PowerIndex = powerList.Count - 1;

                using var frmEditPower = new frmEditPower(newPower);
                if (frmEditPower.ShowDialog() != DialogResult.OK) return;
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
            if (lvPower.SelectedIndices.Count <= 0 || MessageBox.Show($@"Really delete Power: {lvPower.SelectedItems[0].SubItems[3].Text}?", @"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var powerArray = new IPower[DatabaseAPI.Database.Power.Length - 1 + 1];
            var num1 = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
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

                DatabaseAPI.Database.Power = new IPower[DatabaseAPI.Database.Power.Length - 2 + 1];
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
                return;
            var selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex >= lvPower.Items.Count - 1)
                return;
            var selIdx = lvPower.SelectedIndices[0] + 1;
            var index1 = DatabaseAPI.NidFromUidPower(lvPower.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPower(lvPower.Items[selIdx].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPower template = new Power(DatabaseAPI.Database.Power[index1]);
                DatabaseAPI.Database.Power[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                DatabaseAPI.Database.Power[index2] = new Power(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Powers(selIdx);
                BusyHide();
            }
        }

        private void btnPowerEdit_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            var text = lvPower.SelectedItems[0].SubItems[3].Text;
            var index1 = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
            if (index1 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                using var frmEditPower = new frmEditPower(DatabaseAPI.Database.Power[index1], true);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                IPower newPower = new Power(frmEditPower.myPower) { IsModified = true };
                DatabaseAPI.Database.Power[index1] = newPower;
                if (text == DatabaseAPI.Database.Power[index1].FullName)
                {
                    return;
                }

                //Update the full power name in the powerset array
                if (newPower.PowerSetID > -1)
                {
                    DatabaseAPI.Database.Powersets[newPower.PowerSetID].Powers[newPower.PowerSetIndex].FullName =
                        newPower.FullName;
                }

                foreach (var p in DatabaseAPI.Database.Power[index1].Effects)
                {
                    p.PowerFullName = DatabaseAPI.Database.Power[index1].FullName;
                }

                var strArray = DatabaseAPI.UidReferencingPowerFix(text, DatabaseAPI.Database.Power[index1].FullName);
                var str1 = strArray.Aggregate("", (current, t) => $"{current}{t}\r\n");
                if (strArray.Length > 0)
                {
                    var str2 = "Power: " + text + " changed to " + DatabaseAPI.Database.Power[index1].FullName +
                               "\r\nThe following powers referenced this power and were updated:\r\n" + str1 +
                               "\r\n\r\nThis list has been placed on the clipboard.";
                    Clipboard.SetDataObject(str2, true);
                    MessageBox.Show(str2);
                }

                RefreshLists();
            }
        }

        private void btnPowerUp_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            var selIdx = lvPower.SelectedIndices[0] - 1;
            var index1 = DatabaseAPI.NidFromUidPower(lvPower.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPower(lvPower.Items[selIdx].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPower template = new Power(DatabaseAPI.Database.Power[index1]);
                DatabaseAPI.Database.Power[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                DatabaseAPI.Database.Power[index2] = new Power(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Powers(selIdx);
                BusyHide();
            }
        }

        private void btnPSDown_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvSet.SelectedIndices[0];
            if (selectedIndex >= lvSet.Items.Count - 1)
                return;
            var selIdx = lvSet.SelectedIndices[0] + 1;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selIdx].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPowerset template = new Powerset(DatabaseAPI.Database.Powersets[index1]);
                DatabaseAPI.Database.Powersets[index1] = new Powerset(DatabaseAPI.Database.Powersets[index2]);
                DatabaseAPI.Database.Powersets[index2] = new Powerset(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Sets(selIdx);
                BusyHide();
            }
        }

        private void btnPSUp_Click(object sender, EventArgs e)

        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvSet.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            var selIdx = lvSet.SelectedIndices[0] - 1;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selIdx].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPowerset template = new Powerset(DatabaseAPI.Database.Powersets[index1]);
                DatabaseAPI.Database.Powersets[index1] = new Powerset(DatabaseAPI.Database.Powersets[index2]);
                DatabaseAPI.Database.Powersets[index2] = new Powerset(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Sets(selIdx);
                BusyHide();
            }
        }

        private void btnSetAdd_Click(object sender, EventArgs e)
        {
            IPowerset iSet = new Powerset();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        if (lvGroup.SelectedItems.Count > 0)
                            iSet.FullName = lvGroup.SelectedItems[0].SubItems[0].Text + ".New_Set";
                        break;
                    }
                case 1 when lvGroup.SelectedItems.Count > 0:
                    iSet.FullName = DatabaseAPI.Database.Classes[lvGroup.SelectedIndices[0]].PrimaryGroup + ".New_Set";
                    break;
            }

            iSet.DisplayName = "New Set";
            using var frmEditPowerset = new frmEditPowerset(ref iSet);
            var num = (int)frmEditPowerset.ShowDialog();
            if (frmEditPowerset.DialogResult != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;
            var psList = database.Powersets.ToList();
            psList.Add(new Powerset(frmEditPowerset.myPS) { IsNew = true, nID = psList.Count + 1 });
            DatabaseAPI.Database.Powersets = psList.ToArray();
            UpdateLists();
            Sort(1);
        }

        private void btnSetDelete_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (index1 < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var str = "";
                if (DatabaseAPI.Database.Powersets[index1].Powers.Length > 0)
                    str = DatabaseAPI.Database.Powersets[index1].FullName +
                          " still has powers attached to it.\r\nThese powers will be orphaned if you remove the set.\r\n\r\n";
                if (MessageBox.Show($@"{str} Really delete Powerset: {DatabaseAPI.Database.Powersets[index1].DisplayName}?", @"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                var powersetArray = new IPowerset[DatabaseAPI.Database.Powersets.Length - 1 + 1];
                var index2 = 0;
                var num2 = DatabaseAPI.Database.Powersets.Length - 1;
                for (var index3 = 0; index3 <= num2; ++index3)
                {
                    if (index3 == index1)
                        continue;
                    powersetArray[index2] = new Powerset(DatabaseAPI.Database.Powersets[index3]);
                    ++index2;
                }

                DatabaseAPI.Database.Powersets = new IPowerset[DatabaseAPI.Database.Powersets.Length - 2 + 1];
                var num3 = DatabaseAPI.Database.Powersets.Length - 1;
                for (var index3 = 0; index3 <= num3; ++index3)
                    DatabaseAPI.Database.Powersets[index3] = new Powerset(powersetArray[index3]) { nID = index3 };
                var powerset = -1;
                if (lvSet.Items.Count > 0)
                {
                    if (lvSet.Items.Count > index1)
                        powerset = index1;
                    else if (lvSet.Items.Count == index1)
                        powerset = index1 - 1;
                }

                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                RefreshLists(-1, powerset);
                BusyHide();
            }
        }

        private void btnSetEdit_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            var Powerset = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (Powerset < 0)
            {
                MessageBox.Show(@"An unknown error caused an invalid PowerIndex return value.", @"Wha?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var powerset = DatabaseAPI.Database.Powersets[Powerset];
                var fullName = powerset.FullName;
                using var frmEditPowerset = new frmEditPowerset(ref powerset);
                if (frmEditPowerset.ShowDialog() != DialogResult.OK)
                    return;
                DatabaseAPI.Database.Powersets[Powerset] = new Powerset(frmEditPowerset.myPS) { IsModified = true };
                if (DatabaseAPI.Database.Powersets[Powerset].FullName == fullName)
                    return;
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                RefreshLists(-1, Powerset);
                BusyHide();
            }
        }

        private async void BuildATImageList()

        {
            ilAT.Images.Clear();
            var loadedArchetypes = await I9Gfx.LoadArchetypes();
            foreach (var archetype in loadedArchetypes)
            {
                using var extendedBitmap = new ExtendedBitmap(archetype);
                ilAT.Images.Add(new Bitmap(extendedBitmap.Bitmap));
            }
        }

        private void BuildPowersetImageList(int[] iSets)

        {
            ilPS.Images.Clear();
            var imageSize = ilPS.ImageSize;
            var width = imageSize.Width;
            imageSize = ilPS.ImageSize;
            var height = imageSize.Height;
            using var extendedBitmap1 = new ExtendedBitmap(width, height);
            using var solidBrush1 = new SolidBrush(Color.Black);
            using var solidBrush2 = new SolidBrush(Color.White);
            using var solidBrush3 = new SolidBrush(Color.Transparent);
            using var format = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            using var font = new Font(Font, FontStyle.Bold);
            var layoutRectangle = new RectangleF(17f, 0.0f, 16f, 18f);
            var num = iSets.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var str = I9Gfx.GetPowersetsPath() + DatabaseAPI.Database.Powersets[iSets[index]].ImageName;
                if (!File.Exists(str))
                    str = I9Gfx.ImagePath() + "\\Unknown.png";
                using var extendedBitmap2 = new ExtendedBitmap(str);
                string s;
                SolidBrush solidBrush4;
                switch (DatabaseAPI.Database.Powersets[iSets[index]].SetType)
                {
                    case Enums.ePowerSetType.Primary:
                        extendedBitmap1.Graphics.Clear(Color.Blue);
                        s = "1";
                        solidBrush4 = solidBrush2;
                        break;
                    case Enums.ePowerSetType.Secondary:
                        extendedBitmap1.Graphics.Clear(Color.Red);
                        s = "2";
                        solidBrush4 = solidBrush1;
                        break;
                    case Enums.ePowerSetType.Ancillary:
                        extendedBitmap1.Graphics.Clear(Color.Green);
                        s = "A";
                        solidBrush4 = solidBrush2;
                        break;
                    case Enums.ePowerSetType.Inherent:
                        extendedBitmap1.Graphics.Clear(Color.Silver);
                        s = "I";
                        solidBrush4 = solidBrush1;
                        break;
                    case Enums.ePowerSetType.Pool:
                        extendedBitmap1.Graphics.Clear(Color.Cyan);
                        s = "P";
                        solidBrush4 = solidBrush1;
                        break;
                    case Enums.ePowerSetType.Accolade:
                        extendedBitmap1.Graphics.Clear(Color.Goldenrod);
                        s = "+";
                        solidBrush4 = solidBrush1;
                        break;
                    case Enums.ePowerSetType.Temp:
                        extendedBitmap1.Graphics.Clear(Color.WhiteSmoke);
                        s = "T";
                        solidBrush4 = solidBrush1;
                        break;
                    case Enums.ePowerSetType.Pet:
                        extendedBitmap1.Graphics.Clear(Color.Brown);
                        s = "x";
                        solidBrush4 = solidBrush2;
                        break;
                    default:
                        extendedBitmap1.Graphics.Clear(Color.White);
                        s = "";
                        solidBrush4 = solidBrush1;
                        break;
                }

                extendedBitmap1.Graphics.DrawImageUnscaled(extendedBitmap2.Bitmap, new Point(1, 1));
                extendedBitmap1.Graphics.DrawString(s, font, solidBrush4, layoutRectangle, format);
                ilPS.Images.Add(new Bitmap(extendedBitmap1.Bitmap));
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
                {
                    foreach (Button btn in pnlGroup.Controls)
                    {
                        if (buttons.Any(b => b == btn))
                        {
                            btn.Enabled = false;
                        }
                    }

                    lvGroup.Sorting = SortOrder.Ascending;
                    break;
                }
                case 1:
                {
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
            }
            UpdateLists();
        }

        private static int[] ConcatArray(IReadOnlyList<int> iArray1, IReadOnlyList<int> iArray2)
        {
            var numArray = Array.Empty<int>();
            if (iArray1 == null || iArray2 == null) return numArray;
            var length = iArray1.Count;
            numArray = new int[iArray1.Count + iArray2.Count - 1 + 1];
            var num1 = length - 1;
            for (var index = 0; index <= num1; ++index)
                numArray[index] = iArray1[index];
            var num2 = iArray2.Count - 1;
            for (var index = 0; index <= num2; ++index)
                numArray[length + index] = iArray2[index];

            return numArray;
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
            lvGroup.EnableDoubleBuffer();
            lvSet.EnableDoubleBuffer();
            lvPower.EnableDoubleBuffer();
            btnManageHiddenPowers.Visible = MidsContext.Config.MasterMode;

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
                    {
                        foreach (var powersetGroup in DatabaseAPI.Database.PowersetGroups.Values)
                        {
                            var imageIndex = -1;
                            var num = DatabaseAPI.Database.Classes.Length - 1;
                            for (var index = 0; index <= num; ++index)
                            {
                                if (!(string.Equals(DatabaseAPI.Database.Classes[index].PrimaryGroup, powersetGroup.Name, StringComparison.OrdinalIgnoreCase) | string.Equals(DatabaseAPI.Database.Classes[index].SecondaryGroup, powersetGroup.Name, StringComparison.OrdinalIgnoreCase)))
                                    continue;
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
                    }
                case 1:
                    {
                        var num = DatabaseAPI.Database.Classes.Length - 1;
                        for (var imageIndex = 0; imageIndex <= num; ++imageIndex)
                            lvGroup.Items.Add(new ListViewItem(DatabaseAPI.Database.Classes[imageIndex].ClassName,
                                imageIndex));
                        lvGroup.Columns[0].Text = @"Class";
                        lvGroup.Columns[0].Width = -2;
                        lvGroup.Enabled = true;
                        pnlGroup.Enabled = true;
                        break;
                    }
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
            var items = new string[4];
            if (iPowers.Count < 1)
                return;
            var num = iPowers.Count - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (iPowers[index] <= -1 || DatabaseAPI.Database.Power[iPowers[index]].HiddenPower)
                    continue;
                items[0] = !displayFullName
                    ? DatabaseAPI.Database.Power[iPowers[index]].PowerName
                    : DatabaseAPI.Database.Power[iPowers[index]].FullName;
                items[1] = DatabaseAPI.Database.Power[iPowers[index]].DisplayName;
                items[2] = Convert.ToString(DatabaseAPI.Database.Power[iPowers[index]].Level,
                    CultureInfo.InvariantCulture);
                items[3] = DatabaseAPI.Database.Power[iPowers[index]].FullName;
                lvPower.Items.Add(new ListViewItem(items)
                {
                    Tag = iPowers[index]
                });
            }
        }

        private void List_Power_AddBlock(IReadOnlyList<string> iPowers, bool displayFullName)
        {
            var items = new string[4];
            if (iPowers.Count < 1)
                return;
            var num = iPowers.Count - 1;
            for (var index1 = 0; index1 <= num; ++index1)
            {
                var index2 = DatabaseAPI.NidFromUidPower(iPowers[index1]);
                if (index2 <= -1 || DatabaseAPI.Database.Power[index2].HiddenPower)
                    continue;
                items[0] = !displayFullName
                    ? DatabaseAPI.Database.Power[index2].PowerName
                    : DatabaseAPI.Database.Power[index2].FullName;
                items[1] = DatabaseAPI.Database.Power[index2].DisplayName;
                items[2] = Convert.ToString(DatabaseAPI.Database.Power[index2].Level, CultureInfo.InvariantCulture);
                items[3] = DatabaseAPI.Database.Power[index2].FullName;
                lvPower.Items.Add(new ListViewItem(items));
            }
        }

        private void List_Powers(int selIdx)
        {
            var iPowers1 = Array.Empty<int>();
            var iPowers2 = Array.Empty<string>();
            var displayFullName = false;
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                    {
                        if (lvSet.SelectedItems.Count > 0)
                            iPowers2 = DatabaseAPI.UidPowers(lvSet.SelectedItems[0].SubItems[3].Text);
                        break;
                    }
                case 1:
                    {
                        if (lvSet.SelectedItems.Count > 0)
                        {
                            var uidClass = "";
                            if (lvGroup.SelectedItems.Count > 0)
                                uidClass = lvGroup.SelectedItems[0].SubItems[0].Text;
                            iPowers2 = DatabaseAPI.UidPowers(lvSet.SelectedItems[0].SubItems[3].Text, uidClass);
                        }

                        break;
                    }
                case 2:
                    {
                        if (lvSet.SelectedItems.Count > 0)
                        {
                            if (lvSet.SelectedItems[0].SubItems[3].Text != "")
                                iPowers2 = DatabaseAPI.UidPowers(lvSet.SelectedItems[0].SubItems[3].Text);
                            else if (lvSet.SelectedItems[0].SubItems[4].Text != "")
                                iPowers1 = DatabaseAPI.NidPowers(
                                    (int)Math.Round(Convert.ToDouble(lvSet.SelectedItems[0].SubItems[4].Text)));
                        }

                        break;
                    }
                case 4:
                    {
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
                                iPowers1 = new int[DatabaseAPI.Database.Powersets[index].Power.Length - 1 + 1];
                                Array.Copy(DatabaseAPI.Database.Powersets[index].Power, iPowers1, iPowers1.Length);
                            }
                        }

                        break;
                    }
                case 5:
                    {
                        var num = DatabaseAPI.Database.Power.Length - 1;
                        for (var index = 0; index <= num; ++index)
                        {
                            if (!((DatabaseAPI.Database.Power[index].GroupName == "") | (DatabaseAPI.Database.Power[index].SetName == "") | (DatabaseAPI.Database.Power[index].GetPowerSet() == null)))
                                continue;

                            Array.Resize(ref iPowers1, iPowers1.Length + 1);
                            iPowers1[iPowers1.Length - 1] = index;
                        }

                        displayFullName = true;
                        break;
                    }
                case 3:
                    {
                        BusyMsg("Building List...");
                        iPowers1 = new int[DatabaseAPI.Database.Power.Length - 1 + 1];
                        var num = DatabaseAPI.Database.Power.Length - 1;
                        for (var index = 0; index <= num; ++index)
                            iPowers1[index] = index;
                        displayFullName = true;
                        break;
                    }
            }

            lvPower.BeginUpdate();
            lvPower.Items.Clear();
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
            var numArray1 = Array.Empty<int>();
            var numArray2 = Array.Empty<int>();
            if ((lvGroup.SelectedItems.Count == 0) & ((cbFilter.SelectedIndex == 0) | (cbFilter.SelectedIndex == 1)))
                return;
            _updating = true;
            lvSet.BeginUpdate();
            lvSet.Items.Clear();
            if ((cbFilter.SelectedIndex == 0) & (lvGroup.SelectedItems.Count > 0))
            {
                var iSets = DatabaseAPI.NidSets(lvGroup.SelectedItems[0].SubItems[0].Text, "",
                    Enums.ePowerSetType.None);
                BuildPowersetImageList(iSets);
                List_Sets_AddBlock(iSets);
                lvSet.Enabled = true;
            }
            else if ((cbFilter.SelectedIndex == 1) & (lvGroup.SelectedItems.Count > 0))
            {
                var iSets = ConcatArray(
                    ConcatArray(
                        ConcatArray(
                            ConcatArray(
                                DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text,
                                    Enums.ePowerSetType.Primary),
                                DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text,
                                    Enums.ePowerSetType.Secondary)),
                            DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text,
                                Enums.ePowerSetType.Ancillary)),
                        DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text,
                            Enums.ePowerSetType.Inherent)),
                    DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Pool));
                BuildPowersetImageList(iSets);
                List_Sets_AddBlock(iSets);
                lvSet.Enabled = true;
            }
            else
            {
                switch (cbFilter.SelectedIndex)
                {
                    case 4:
                        {
                            var numArray3 = Array.Empty<int>();
                            var num = DatabaseAPI.Database.Powersets.Length - 1;
                            for (var index = 0; index <= num; ++index)
                            {
                                if (!((DatabaseAPI.Database.Powersets[index].GetGroup() == null) |
                                      string.IsNullOrEmpty(DatabaseAPI.Database.Powersets[index].GroupName)))
                                    continue;
                                int[] iArray2 = { index };
                                numArray3 = ConcatArray(numArray3, iArray2);
                            }

                            BuildPowersetImageList(numArray3);
                            List_Sets_AddBlock(numArray3);
                            lvSet.Enabled = true;
                            break;
                        }
                    case 2:
                        {
                            BusyMsg("Building List...");
                            var iSets = DatabaseAPI.NidSets("", "", Enums.ePowerSetType.None);
                            BuildPowersetImageList(iSets);
                            List_Sets_AddBlock(iSets);
                            lvSet.Enabled = true;
                            break;
                        }
                    default:
                        lvSet.Enabled = false;
                        break;
                }
            }

            if (lvSet.Items.Count > 0)
            {
                if ((lvSet.Items.Count > selIdx) & (selIdx > -1))
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
        }

        private void List_Sets_AddBlock(IReadOnlyList<int> iSets)
        {
            var items = new string[5];
            if (iSets.Count < 1)
                return;
            var num = iSets.Count - 1;
            for (var imageIndex = 0; imageIndex <= num; ++imageIndex)
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
                return;
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
                return;
            lblPower.Text = lvPower.SelectedItems[0].SubItems[3].Text;
        }

        private void lvSet_DoubleClick(object sender, EventArgs e)
        {
            btnSetEdit_Click(this, EventArgs.Empty);
        }

        private void lvSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            if (lvSet.SelectedItems.Count > 0)
                lblSet.Text = lvSet.SelectedItems[0].SubItems[3].Text;
            List_Powers(0);
        }

        private void RefreshLists(int @group = -1, int powerset = -1, int power = -1)
        {
            var selectGroup = @group;
            var selectSet = powerset;
            var selectPower = power;
            if ((lvGroup.SelectedIndices.Count > 0) & (selectGroup == -1))
                selectGroup = lvGroup.SelectedIndices[0];
            if ((lvSet.SelectedIndices.Count > 0) & (selectSet == -1))
                selectSet = lvSet.SelectedIndices[0];
            if ((lvPower.SelectedIndices.Count > 0) & (selectPower == -1))
                selectPower = lvPower.SelectedIndices[0];
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

            BusyMsg("Fetching Scrapper powers...");
            //Debug.WriteLine("Fetching powers");
            var powers = DatabaseAPI.Database.Power.Where(p => p.FullName.StartsWith("Scrapper_Melee."));
            //Debug.WriteLine("Counting effects");
            var fxNb = powers.Sum(p => p.Effects.Length);

            BusyHide();
            BusyMsg($"Updating {powers.Count()} powers ({fxNb} effects)...");
            foreach (var p in powers)
            {
                foreach (var fx in p.Effects)
                {
                    if (fx.EffectId == "MLCrit" | fx.EffectId == "CritSmall")
                    {
                        fx.ActiveConditionals.Clear();
                        fx.SpecialCase = Enums.eSpecialCase.CriticalMinion;
                        fx.EffectId = "MLCrit";
                        //Debug.WriteLine($"Updated MLCrit for {p.FullName}");
                    }
                    else if (fx.EffectId == "BossCrit" | fx.EffectId == "CritLarge" | fx.SpecialCase == Enums.eSpecialCase.CriticalBoss)
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.EffectId = "BossCrit";
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        //Debug.WriteLine($"Updated BossCrit for {p.FullName}");
                    }
                    else if (fx.EffectId == "PlayerCrit" | fx.EffectId == "CritPlayer") // Not found in fx.SpecialCase
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.EffectId = "PlayerCrit";
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        //Debug.WriteLine($"Updated PlayerCrit for {p.FullName}");
                    }
                    else if (fx.EffectId == "ECCritModSmall")
                    {
                        fx.ActiveConditionals.Clear();
                        fx.SpecialCase = Enums.eSpecialCase.CriticalMinion;
                        //Debug.WriteLine($"Updated ECCritModSmall for {p.FullName}");
                    }
                    else if (fx.EffectId == "ECCritModLarge")
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        //Debug.WriteLine($"Updated ECCritModLarge for {p.FullName}");
                    }
                    else if (fx.EffectId == "ECCritModPlayer")
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        //Debug.WriteLine($"Updated ECCritModPlayer for {p.FullName}");
                    }
                }
            }

            BusyHide();
        }
    }
}