using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.My;
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
        private frmDBDiffing _diffFrm;

        private bool Updating;

        public frmPowerBrowser()
        {
            Load += frmPowerBrowser_Load;
            Updating = false;
            InitializeComponent();
            Name = nameof(frmPowerBrowser);
            var componentResourceManager = new ComponentResourceManager(typeof(frmPowerBrowser));
            Icon = Resources.reborn;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BusyMsg("Discarding Changes...");
            DatabaseAPI.LoadMainDatabase();
            DatabaseAPI.MatchAllIDs();
            BusyHide();
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnClassAdd_Click(object sender, EventArgs e)

        {
            var iAT = new Archetype
            {
                ClassName = "Class_New",
                DisplayName = "New Class"
            };
            using var frmEditArchetype = new frmEditArchetype(ref iAT);
            var num = (int)frmEditArchetype.ShowDialog();
            if (frmEditArchetype.DialogResult != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;
            var archetypeArray = (Archetype[])Utils.CopyArray(database.Classes,
                new Archetype[DatabaseAPI.Database.Classes.Length + 1]);
            database.Classes = archetypeArray;
            DatabaseAPI.Database.Classes[DatabaseAPI.Database.Classes.Length - 1] =
                new Archetype(frmEditArchetype.MyAT) { IsNew = true };
            UpdateLists(lvGroup.Items.Count - 1);
        }

        private void btnClassClone_Click(object sender, EventArgs e)

        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            var index = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index < 0)
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var iAT = new Archetype(DatabaseAPI.Database.Classes[index]);
                iAT.ClassName += "_Clone";
                iAT.DisplayName += " (Clone)";
                using var frmEditArchetype = new frmEditArchetype(ref iAT);
                var num2 = (int)frmEditArchetype.ShowDialog();
                if (frmEditArchetype.DialogResult != DialogResult.OK)
                    return;
                var database = DatabaseAPI.Database;
                var archetypeArray = (Archetype[])Utils.CopyArray(database.Classes,
                    new Archetype[DatabaseAPI.Database.Classes.Length + 1]);
                database.Classes = archetypeArray;
                DatabaseAPI.Database.Classes[DatabaseAPI.Database.Classes.Length - 1] =
                    new Archetype(frmEditArchetype.MyAT) { IsNew = true };
                UpdateLists(lvGroup.Items.Count - 1);
            }
        }

        private void btnClassDelete_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            var index1 = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index1 < 0)
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (MessageBox.Show($"Really delete Class: {DatabaseAPI.Database.Classes[index1].ClassName} ({DatabaseAPI.Database.Classes[index1].DisplayName})?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var archetypeArray = new Archetype[DatabaseAPI.Database.Classes.Length - 1 + 1];
                var num2 = index1;
                var index2 = 0;
                var num3 = DatabaseAPI.Database.Classes.Length - 1;
                for (var index3 = 0; index3 <= num3; ++index3)
                {
                    if (index3 == num2)
                        continue;
                    archetypeArray[index2] = new Archetype(DatabaseAPI.Database.Classes[index3]);
                    ++index2;
                }

                DatabaseAPI.Database.Classes = new Archetype[DatabaseAPI.Database.Classes.Length - 2 + 1];
                var num4 = DatabaseAPI.Database.Classes.Length - 1;
                for (var index3 = 0; index3 <= num4; ++index3)
                    DatabaseAPI.Database.Classes[index3] = new Archetype(archetypeArray[index3]);
                var Group = 0;
                if (lvGroup.Items.Count > 0)
                {
                    if (lvGroup.Items.Count > num2)
                        Group = num2;
                    else if (lvGroup.Items.Count == num2)
                        Group = num2 - 1;
                }

                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                RefreshLists(Group, 0, 0);
                BusyHide();
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
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void btnClassSort_Click(object sender, EventArgs e)
        {
            BusyMsg("Discarding Changes...");
            Array.Sort(DatabaseAPI.Database.Classes);
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
            // Zed - 04/11/21 - disabled diffing
            /*if (!Debugger.IsAttached)
            {
                BusyMsg(@"Re-Indexing && Saving...");
                foreach (var power in DatabaseAPI.Database.Power) power.BaseRechargeTime = power.RechargeTime;
                Array.Sort(DatabaseAPI.Database.Power);
                var serializer = MyApplication.GetSerializer();
                DatabaseAPI.AssignStaticIndexValues(serializer, false);
                DatabaseAPI.MatchAllIDs();
                DatabaseAPI.SaveMainDatabase(serializer, MidsContext.Config.DataPath);
                BusyHide();
                DialogResult = DialogResult.OK;
                Hide();
            }
            else
            {
                foreach (var power in DatabaseAPI.Database.Power) power.BaseRechargeTime = power.RechargeTime;
                Array.Sort(DatabaseAPI.Database.Power);
                var serializer = MyApplication.GetSerializer();
                DatabaseAPI.AssignStaticIndexValues(serializer, false);
                DatabaseAPI.MatchAllIDs();
                var iParent = this;
                using var diffForm = new frmDBDiffing(serializer, iParent);
                diffForm.Closed += diffForm_Closed;
                diffForm.ShowDialog(this);
            }*/

            BusyMsg(@"Re-Indexing && Saving...");
            foreach (var power in DatabaseAPI.Database.Power) power.BaseRechargeTime = power.RechargeTime;
            Array.Sort(DatabaseAPI.Database.Power);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.MatchAllIDs();

            // Uncomment below to update AT modifier columns if necessary
            // for (var index = 0; index < DatabaseAPI.Database.Classes.Length - 1; index++)
            // {
            //     DatabaseAPI.Database.Classes[index].Column = index;
            // }

            DatabaseAPI.SaveMainDatabase(serializer, MidsContext.Config.DataPath);
            BusyHide();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void diffForm_Closed(object sender, EventArgs e)
        {
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
            var powerArray =
                (IPower[])Utils.CopyArray(database.Power, new IPower[DatabaseAPI.Database.Power.Length + 1]);
            database.Power = powerArray;
            DatabaseAPI.Database.Power[DatabaseAPI.Database.Power.Length - 1] =
                new Power(frmEditPower.myPower) { IsNew = true };
            UpdateLists();
        }

        private void btnPowerClone_Click(object sender, EventArgs e)
        {
            var index = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
            if (index < 0)
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPower newPower = new Power(DatabaseAPI.Database.Power[index]);
                //Get the StaticIndex from the last power and add 1
                var powerList = new List<IPower>(DatabaseAPI.Database.Power);
                var newStaticIndex = powerList.Max(x => x.StaticIndex) + 1;
                newPower.StaticIndex = newStaticIndex;
                newPower.FullName += "_Clone";
                newPower.DisplayName += " (Clone)";
                newPower.PowerName += "_Clone";
                newPower.IsNew = true;
                newPower.PowerIndex = DatabaseAPI.Database.Power.Length;

                using var frmEditPower = new frmEditPower(newPower);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                    return;
                newPower = frmEditPower.myPower;
                var database = DatabaseAPI.Database;
                var powerArray =
                    (IPower[])Utils.CopyArray(database.Power, new IPower[DatabaseAPI.Database.Power.Length + 1]);
                database.Power = powerArray;
                DatabaseAPI.Database.Power[DatabaseAPI.Database.Power.Length - 1] = newPower;
                //Add the power to the power set otherwise we'll get issues later when upting the UI.
                if (newPower.PowerSetID > -1)
                {
                    var powerSet = DatabaseAPI.GetPowersetByName(newPower.FullName);
                    powerArray = (IPower[])Utils.CopyArray(powerSet.Powers, new IPower[powerSet.Powers.Length + 1]);
                    powerSet.Powers = powerArray;
                    powerArray[powerSet.Powers.Length - 1] = newPower;
                }

                UpdateLists(lvGroup.SelectedIndices[0], lvSet.SelectedIndices[0]);
            }
        }

        private void btnPowerDelete_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0 || MessageBox.Show($"Really delete Power: {lvPower.SelectedItems[0].SubItems[3].Text}?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var powerArray = new IPower[DatabaseAPI.Database.Power.Length - 1 + 1];
            var num1 = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
            if (num1 < 0)
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                var SelIDX = -1;
                if (lvPower.Items.Count > 0)
                {
                    if (lvPower.Items.Count > num1)
                        SelIDX = num1;
                    else if (lvPower.Items.Count == num1)
                        SelIDX = num1 - 1;
                }

                List_Powers(SelIDX);
            }
        }

        private void btnPowerDown_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex >= lvPower.Items.Count - 1)
                return;
            var SelIDX = lvPower.SelectedIndices[0] + 1;
            var index1 = DatabaseAPI.NidFromUidPower(lvPower.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPower(lvPower.Items[SelIDX].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPower template = new Power(DatabaseAPI.Database.Power[index1]);
                DatabaseAPI.Database.Power[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                DatabaseAPI.Database.Power[index2] = new Power(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Powers(SelIDX);
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
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                using var frmEditPower = new frmEditPower(DatabaseAPI.Database.Power[index1]);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                    return;
                IPower newPower = new Power(frmEditPower.myPower) { IsModified = true };
                DatabaseAPI.Database.Power[index1] = newPower;
                if (text == DatabaseAPI.Database.Power[index1].FullName)
                    return;
                //Update the full power name in the powerset array
                if (newPower.PowerSetID > -1)
                    DatabaseAPI.Database.Powersets[newPower.PowerSetID].Powers[newPower.PowerSetIndex].FullName =
                        newPower.FullName;

                var num2 = DatabaseAPI.Database.Power[index1].Effects.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                    DatabaseAPI.Database.Power[index1].Effects[index2].PowerFullName =
                        DatabaseAPI.Database.Power[index1].FullName;
                var strArray = DatabaseAPI.UidReferencingPowerFix(text, DatabaseAPI.Database.Power[index1].FullName);
                var str1 = "";
                var num3 = strArray.Length - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                    str1 = str1 + strArray[index2] + "\r\n";
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

        private void btnPowerSort_Click(object sender, EventArgs e)
        {
            BusyMsg("Re-Indexing...");
            Array.Sort(DatabaseAPI.Database.Power);
            DatabaseAPI.MatchAllIDs();
            UpdateLists();
            BusyHide();
        }

        private void btnPowerUp_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            var SelIDX = lvPower.SelectedIndices[0] - 1;
            var index1 = DatabaseAPI.NidFromUidPower(lvPower.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPower(lvPower.Items[SelIDX].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPower template = new Power(DatabaseAPI.Database.Power[index1]);
                DatabaseAPI.Database.Power[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                DatabaseAPI.Database.Power[index2] = new Power(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Powers(SelIDX);
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
            var SelIDX = lvSet.SelectedIndices[0] + 1;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[SelIDX].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPowerset template = new Powerset(DatabaseAPI.Database.Powersets[index1]);
                DatabaseAPI.Database.Powersets[index1] = new Powerset(DatabaseAPI.Database.Powersets[index2]);
                DatabaseAPI.Database.Powersets[index2] = new Powerset(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Sets(SelIDX);
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
            var SelIDX = lvSet.SelectedIndices[0] - 1;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            var index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[SelIDX].SubItems[3].Text);
            if ((index1 < 0) | (index2 < 0))
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IPowerset template = new Powerset(DatabaseAPI.Database.Powersets[index1]);
                DatabaseAPI.Database.Powersets[index1] = new Powerset(DatabaseAPI.Database.Powersets[index2]);
                DatabaseAPI.Database.Powersets[index2] = new Powerset(template);
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                List_Sets(SelIDX);
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
            var powersetArray = (IPowerset[])Utils.CopyArray(database.Powersets,
                new IPowerset[DatabaseAPI.Database.Powersets.Length + 1]);
            database.Powersets = powersetArray;
            DatabaseAPI.Database.Powersets[DatabaseAPI.Database.Powersets.Length - 1] =
                new Powerset(frmEditPowerset.myPS) { IsNew = true, nID = DatabaseAPI.Database.Powersets.Length - 1 };
            UpdateLists();
        }

        private void btnSetDelete_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            var index1 = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (index1 < 0)
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var str = "";
                if (DatabaseAPI.Database.Powersets[index1].Powers.Length > 0)
                    str = DatabaseAPI.Database.Powersets[index1].FullName +
                          " still has powers attached to it.\r\nThese powers will be orphaned if you remove the set.\r\n\r\n";
                if (MessageBox.Show(
                    $"{str} Really delete Powerset: {DatabaseAPI.Database.Powersets[index1].DisplayName}?",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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
                var Powerset = -1;
                if (lvSet.Items.Count > 0)
                {
                    if (lvSet.Items.Count > index1)
                        Powerset = index1;
                    else if (lvSet.Items.Count == index1)
                        Powerset = index1 - 1;
                }

                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                RefreshLists(-1, Powerset);
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
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        private void btnSetSort_Click(object sender, EventArgs e)

        {
            BusyMsg("Re-Indexing...");
            Array.Sort(DatabaseAPI.Database.Powersets);
            DatabaseAPI.MatchAllIDs();
            UpdateLists();
            BusyHide();
        }

        private void BuildATImageList()

        {
            ilAT.Images.Clear();
            var num = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var str = I9Gfx.GetOriginsPath() + DatabaseAPI.Database.Classes[index].ClassName + ".png";
                if (!File.Exists(str)) str = I9Gfx.ImagePath() + "Unknown.png";
                using var extendedBitmap = new ExtendedBitmap(str);
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
                    str = I9Gfx.ImagePath() + "Unknown.png";
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
            if (Updating)
                return;
            UpdateLists();
        }

        private static int[] ConcatArray(int[] iArray1, int[] iArray2)
        {
            var numArray = Array.Empty<int>();
            if (iArray1 != null && iArray2 != null)
            {
                var length = iArray1.Length;
                numArray = new int[iArray1.Length + iArray2.Length - 1 + 1];
                var num1 = length - 1;
                for (var index = 0; index <= num1; ++index)
                    numArray[index] = iArray1[index];
                var num2 = iArray2.Length - 1;
                for (var index = 0; index <= num2; ++index)
                    numArray[length + index] = iArray2[index];
            }

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
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
                ProjectData.SetProjectError(ex);
                var num = (int)MessageBox.Show(ex.Message);
                ProjectData.ClearProjectError();
            }
        }

        [DebuggerStepThrough]
        private void List_Groups(int SelIDX)
        {
            Updating = true;
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

                        lvGroup.Columns[0].Text = "Group";
                        lvGroup.Enabled = true;
                        pnlGroup.Enabled = false;
                        break;
                    }
                case 1:
                    {
                        var num = DatabaseAPI.Database.Classes.Length - 1;
                        for (var imageIndex = 0; imageIndex <= num; ++imageIndex)
                            lvGroup.Items.Add(new ListViewItem(DatabaseAPI.Database.Classes[imageIndex].ClassName,
                                imageIndex));
                        lvGroup.Columns[0].Text = "Class";
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
                if ((lvGroup.Items.Count > SelIDX) & (SelIDX > -1))
                {
                    lvGroup.Items[SelIDX].Selected = true;
                    lvGroup.Items[SelIDX].EnsureVisible();
                }
                else
                {
                    lvGroup.Items[0].Selected = true;
                    lvGroup.Items[0].EnsureVisible();
                }
            }

            lvGroup.EndUpdate();
            Updating = false;
        }

        private void List_Power_AddBlock(int[] iPowers, bool DisplayFullName)
        {
            var items = new string[4];
            if (iPowers.Length < 1)
                return;
            var num = iPowers.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (iPowers[index] <= -1 || DatabaseAPI.Database.Power[iPowers[index]].HiddenPower)
                    continue;
                items[0] = !DisplayFullName
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

        private void List_Power_AddBlock(string[] iPowers, bool DisplayFullName)
        {
            var items = new string[4];
            if (iPowers.Length < 1)
                return;
            var num = iPowers.Length - 1;
            for (var index1 = 0; index1 <= num; ++index1)
            {
                var index2 = DatabaseAPI.NidFromUidPower(iPowers[index1]);
                if (index2 <= -1 || DatabaseAPI.Database.Power[index2].HiddenPower)
                    continue;
                items[0] = !DisplayFullName
                    ? DatabaseAPI.Database.Power[index2].PowerName
                    : DatabaseAPI.Database.Power[index2].FullName;
                items[1] = DatabaseAPI.Database.Power[index2].DisplayName;
                items[2] = Convert.ToString(DatabaseAPI.Database.Power[index2].Level, CultureInfo.InvariantCulture);
                items[3] = DatabaseAPI.Database.Power[index2].FullName;
                lvPower.Items.Add(new ListViewItem(items));
            }
        }

        private void List_Powers(int SelIDX)
        {
            var iPowers1 = Array.Empty<int>();
            var iPowers2 = Array.Empty<string>();
            var DisplayFullName = false;
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
                                    (int)Math.Round(Conversion.Val(lvSet.SelectedItems[0].SubItems[4].Text)));
                        }

                        break;
                    }
                case 4:
                    {
                        if (lvSet.SelectedItems.Count > 0)
                        {
                            var index = lvSet.SelectedItems[0].SubItems[4].Text == ""
                                ? -1
                                : (int)Math.Round(Conversion.Val(lvSet.SelectedItems[0].SubItems[4].Text));
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
                            if (!((DatabaseAPI.Database.Power[index].GroupName == "") |
                                  (DatabaseAPI.Database.Power[index].SetName == "") |
                                  (DatabaseAPI.Database.Power[index].GetPowerSet() == null)))
                                continue;
                            iPowers1 = (int[])Utils.CopyArray(iPowers1, new int[iPowers1.Length + 1]);
                            iPowers1[iPowers1.Length - 1] = index;
                        }

                        DisplayFullName = true;
                        break;
                    }
                case 3:
                    {
                        BusyMsg("Building List...");
                        iPowers1 = new int[DatabaseAPI.Database.Power.Length - 1 + 1];
                        var num = DatabaseAPI.Database.Power.Length - 1;
                        for (var index = 0; index <= num; ++index)
                            iPowers1[index] = index;
                        DisplayFullName = true;
                        break;
                    }
            }

            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            if (iPowers2.Length > 0)
            {
                List_Power_AddBlock(iPowers2, DisplayFullName);
            }
            else
            {
                List_Power_AddBlock(iPowers1, DisplayFullName);
            }

            BusyHide();
            if (lvPower.Items.Count > 0)
            {
                if ((SelIDX > -1) & (SelIDX < lvPower.Items.Count))
                {
                    lvPower.Items[SelIDX].Selected = true;
                    lvPower.Items[SelIDX].EnsureVisible();
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

        private void List_Sets(int SelIDX)
        {
            var numArray1 = Array.Empty<int>();
            var numArray2 = Array.Empty<int>();
            if ((lvGroup.SelectedItems.Count == 0) & ((cbFilter.SelectedIndex == 0) | (cbFilter.SelectedIndex == 1)))
                return;
            Updating = true;
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
                if ((lvSet.Items.Count > SelIDX) & (SelIDX > -1))
                {
                    lvSet.Items[SelIDX].Selected = true;
                    lvSet.Items[SelIDX].EnsureVisible();
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
            Updating = false;
        }

        private void List_Sets_AddBlock(int[] iSets)
        {
            var items = new string[5];
            if (iSets.Length < 1)
                return;
            var num = iSets.Length - 1;
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
            btnClassEdit_Click(this, new EventArgs());
        }

        private void lvGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            List_Sets(0);
            Application.DoEvents();
            List_Powers(0);
        }

        private void lvPower_DoubleClick(object sender, EventArgs e)
        {
            btnPowerEdit_Click(this, new EventArgs());
        }

        private void lvPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPower.SelectedItems.Count <= 0)
                return;
            lblPower.Text = lvPower.SelectedItems[0].SubItems[3].Text;
        }

        private void lvSet_DoubleClick(object sender, EventArgs e)
        {
            btnSetEdit_Click(this, new EventArgs());
        }

        private void lvSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            if (lvSet.SelectedItems.Count > 0)
                lblSet.Text = lvSet.SelectedItems[0].SubItems[3].Text;
            List_Powers(0);
        }

        private void RefreshLists(int Group = -1, int Powerset = -1, int Power = -1)
        {
            var SelectGroup = Group;
            var SelectSet = Powerset;
            var SelectPower = Power;
            if ((lvGroup.SelectedIndices.Count > 0) & (SelectGroup == -1))
                SelectGroup = lvGroup.SelectedIndices[0];
            if ((lvSet.SelectedIndices.Count > 0) & (SelectSet == -1))
                SelectSet = lvSet.SelectedIndices[0];
            if ((lvPower.SelectedIndices.Count > 0) & (SelectPower == -1))
                SelectPower = lvPower.SelectedIndices[0];
            UpdateLists(SelectGroup, SelectSet, SelectPower);
        }

        private void UpdateLists(int SelectGroup = -1, int SelectSet = -1, int SelectPower = -1)
        {
            List_Groups(SelectGroup);
            Application.DoEvents();
            List_Sets(SelectSet);
            Application.DoEvents();
            List_Powers(SelectPower);
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
            Debug.WriteLine("Fetching powers");
            var powers = DatabaseAPI.Database.Power.Where(p => p.FullName.StartsWith("Scrapper_Melee."));
            Debug.WriteLine("Counting effects");
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
                        Debug.WriteLine($"Updated MLCrit for {p.FullName}");
                    }
                    else if (fx.EffectId == "BossCrit" | fx.EffectId == "CritLarge" | fx.SpecialCase == Enums.eSpecialCase.CriticalBoss)
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.EffectId = "BossCrit";
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        Debug.WriteLine($"Updated BossCrit for {p.FullName}");
                    }
                    else if (fx.EffectId == "PlayerCrit" | fx.EffectId == "CritPlayer") // Not found in fx.SpecialCase
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.EffectId = "PlayerCrit";
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        Debug.WriteLine($"Updated PlayerCrit for {p.FullName}");
                    }
                    else if (fx.EffectId == "ECCritModSmall")
                    {
                        fx.ActiveConditionals.Clear();
                        fx.SpecialCase = Enums.eSpecialCase.CriticalMinion;
                        Debug.WriteLine($"Updated ECCritModSmall for {p.FullName}");
                    }
                    else if (fx.EffectId == "ECCritModLarge")
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        Debug.WriteLine($"Updated ECCritModLarge for {p.FullName}");
                    }
                    else if (fx.EffectId == "ECCritModPlayer")
                    {
                        fx.SpecialCase = Enums.eSpecialCase.None;
                        fx.ActiveConditionals.Clear();
                        fx.ActiveConditionals.Add(new KeyValue<string, string>("Active:Inherent.Inherent.Critical_Hit", "True"));
                        Debug.WriteLine($"Updated ECCritModPlayer for {p.FullName}");
                    }
                }
            }

            BusyHide();
        }
    }
}