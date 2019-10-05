
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmPowerBrowser : Form
    {
        const int FILTER_ALL_POWERS = 3;

        const int FILTER_ALL_SETS = 2;

        const int FILTER_CLASSES = 1;

        const int FILTER_GROUPS = 0;

        const int FILTER_ORPHAN_POWERS = 5;

        const int FILTER_ORPHAN_SETS = 4;

        frmBusy bFrm;

        protected bool Updating;

        public frmPowerBrowser()
        {
            Load += frmPowerBrowser_Load;
            Updating = false;
            InitializeComponent();
            Name = nameof(frmPowerBrowser);
            var componentResourceManager = new ComponentResourceManager(typeof(frmPowerBrowser));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            BusyMsg("Discarding Changes...");
            DatabaseAPI.LoadMainDatabase();
            DatabaseAPI.MatchAllIDs();
            BusyHide();
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnClassAdd_Click(object sender, EventArgs e)

        {
            Archetype iAT = new Archetype
            {
                ClassName = "Class_New",
                DisplayName = "New Class"
            };
            frmEditArchetype frmEditArchetype = new frmEditArchetype(ref iAT);
            int num = (int)frmEditArchetype.ShowDialog();
            if (frmEditArchetype.DialogResult != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            Archetype[] archetypeArray = (Archetype[])Utils.CopyArray(database.Classes, new Archetype[DatabaseAPI.Database.Classes.Length + 1]);
            database.Classes = archetypeArray;
            DatabaseAPI.Database.Classes[DatabaseAPI.Database.Classes.Length - 1] =
                new Archetype(frmEditArchetype.MyAT) {IsNew = true};
            UpdateLists(lvGroup.Items.Count - 1);
        }

        void btnClassClone_Click(object sender, EventArgs e)

        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            int index = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                Archetype iAT = new Archetype(DatabaseAPI.Database.Classes[index]);
                iAT.ClassName += "_Clone";
                iAT.DisplayName += " (Clone)";
                frmEditArchetype frmEditArchetype = new frmEditArchetype(ref iAT);
                int num2 = (int)frmEditArchetype.ShowDialog();
                if (frmEditArchetype.DialogResult != DialogResult.OK)
                    return;
                IDatabase database = DatabaseAPI.Database;
                Archetype[] archetypeArray = (Archetype[])Utils.CopyArray(database.Classes, new Archetype[DatabaseAPI.Database.Classes.Length + 1]);
                database.Classes = archetypeArray;
                DatabaseAPI.Database.Classes[DatabaseAPI.Database.Classes.Length - 1] =
                    new Archetype(frmEditArchetype.MyAT) {IsNew = true};
                UpdateLists(lvGroup.Items.Count - 1);
            }
        }

        void btnClassDelete_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            int index1 = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index1 < 0)
            {
                int num1 = (int)Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else if (Interaction.MsgBox(("Really delete Class: " + DatabaseAPI.Database.Classes[index1].ClassName + " (" + DatabaseAPI.Database.Classes[index1].DisplayName + ")?"), MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") == MsgBoxResult.Yes)
            {
                Archetype[] archetypeArray = new Archetype[DatabaseAPI.Database.Classes.Length - 1 + 1];
                int num2 = index1;
                int index2 = 0;
                int num3 = DatabaseAPI.Database.Classes.Length - 1;
                for (int index3 = 0; index3 <= num3; ++index3)
                {
                    if (index3 == num2)
                        continue;
                    archetypeArray[index2] = new Archetype(DatabaseAPI.Database.Classes[index3]);
                    ++index2;
                }
                DatabaseAPI.Database.Classes = new Archetype[DatabaseAPI.Database.Classes.Length - 2 + 1];
                int num4 = DatabaseAPI.Database.Classes.Length - 1;
                for (int index3 = 0; index3 <= num4; ++index3)
                    DatabaseAPI.Database.Classes[index3] = new Archetype(archetypeArray[index3]);
                int Group = 0;
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

        void btnClassDown_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvGroup.SelectedIndices[0];
            if (selectedIndex >= lvGroup.Items.Count - 1)
                return;
            Archetype[] archetypeArray = {
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

        void btnClassEdit_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            int index = DatabaseAPI.NidFromUidClass(lvGroup.SelectedItems[0].SubItems[0].Text);
            if (index < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                string className = DatabaseAPI.Database.Classes[index].ClassName;
                frmEditArchetype frmEditArchetype = new frmEditArchetype(ref DatabaseAPI.Database.Classes[index]);
                if (frmEditArchetype.ShowDialog() != DialogResult.OK)
                    return;
                DatabaseAPI.Database.Classes[index] = new Archetype(frmEditArchetype.MyAT) {IsModified = true};
                if (DatabaseAPI.Database.Classes[index].ClassName != className)
                    RefreshLists();
            }
        }

        void btnClassSort_Click(object sender, EventArgs e)
        {
            BusyMsg("Discarding Changes...");
            Array.Sort(DatabaseAPI.Database.Classes);
            DatabaseAPI.MatchAllIDs();
            UpdateLists();
            BusyHide();
        }

        void btnClassUp_Click(object sender, EventArgs e)
        {
            if (lvGroup.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvGroup.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            Archetype[] archetypeArray = {
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

        void btnOK_Click(object sender, EventArgs e)
        {
            BusyMsg("Re-Indexing && Saving...");
            foreach (var power in DatabaseAPI.Database.Power)
            {
                power.BaseRechargeTime = power.RechargeTime;
            }
            Array.Sort(DatabaseAPI.Database.Power);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.MatchAllIDs();
            DatabaseAPI.SaveMainDatabase(serializer);
            BusyHide();
            DialogResult = DialogResult.OK;
            Hide();
        }

        void btnPowerAdd_Click(object sender, EventArgs e)
        {
            IPower iPower = new Power();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                {
                    if (lvGroup.SelectedItems.Count > 0 & lvSet.SelectedItems.Count > 0)
                        iPower.FullName = lvGroup.SelectedItems[0].SubItems[0].Text + lvSet.SelectedItems[0].SubItems[0].Text + ".New_Power";
                    break;
                }
                case 1 when lvGroup.SelectedItems.Count > 0 & lvSet.SelectedItems.Count > 0:
                    iPower.FullName = DatabaseAPI.Database.Classes[lvGroup.SelectedIndices[0]].PrimaryGroup + lvSet.SelectedItems[0].SubItems[0].Text + ".New_Power";
                    break;
            }
            iPower.DisplayName = "New Power";
            frmEditPower frmEditPower = new frmEditPower(iPower);
            if (frmEditPower.ShowDialog() != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            IPower[] powerArray = (IPower[])Utils.CopyArray(database.Power, new IPower[DatabaseAPI.Database.Power.Length + 1]);
            database.Power = powerArray;
            DatabaseAPI.Database.Power[DatabaseAPI.Database.Power.Length - 1] =
                new Power(frmEditPower.myPower) {IsNew = true};
            UpdateLists();
        }

        void btnPowerClone_Click(object sender, EventArgs e)
        {
            IPower iPower = new Power();
            if (DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text) < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                iPower.DisplayName = "New Power";
                iPower.FullName += "_Clone";
                iPower.DisplayName += " (Clone)";
                frmEditPower frmEditPower = new frmEditPower(iPower);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                    return;
                IDatabase database = DatabaseAPI.Database;
                IPower[] powerArray = (IPower[])Utils.CopyArray(database.Power, new IPower[DatabaseAPI.Database.Power.Length + 1]);
                database.Power = powerArray;
                DatabaseAPI.Database.Power[DatabaseAPI.Database.Power.Length - 1] =
                    new Power(frmEditPower.myPower) {IsNew = true};
                UpdateLists();
            }
        }

        void btnPowerDelete_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0 || Interaction.MsgBox(("Really delete Power: " + lvPower.SelectedItems[0].SubItems[3].Text + "?"), MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            IPower[] powerArray = new IPower[DatabaseAPI.Database.Power.Length - 1 + 1];
            int num1 = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
            if (num1 < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                int index1 = 0;
                int num3 = DatabaseAPI.Database.Power.Length - 1;
                for (int index2 = 0; index2 <= num3; ++index2)
                {
                    if (index2 == num1)
                        continue;
                    powerArray[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                    ++index1;
                }
                DatabaseAPI.Database.Power = new IPower[DatabaseAPI.Database.Power.Length - 2 + 1];
                int num4 = DatabaseAPI.Database.Power.Length - 1;
                for (int index2 = 0; index2 <= num4; ++index2)
                    DatabaseAPI.Database.Power[index2] = new Power(powerArray[index2]);
                int SelIDX = -1;
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

        void btnPowerDown_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex >= lvPower.Items.Count - 1)
                return;
            int SelIDX = lvPower.SelectedIndices[0] + 1;
            int index1 = DatabaseAPI.NidFromUidPower(lvPower.Items[selectedIndex].SubItems[3].Text);
            int index2 = DatabaseAPI.NidFromUidPower(lvPower.Items[SelIDX].SubItems[3].Text);
            if (index1 < 0 | index2 < 0)
            {
                int num = (int)Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
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

        void btnPowerEdit_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            string text = lvPower.SelectedItems[0].SubItems[3].Text;
            int index1 = DatabaseAPI.NidFromUidPower(lvPower.SelectedItems[0].SubItems[3].Text);
            if (index1 < 0)
            {
                int num1 = (int)Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                frmEditPower frmEditPower = new frmEditPower(DatabaseAPI.Database.Power[index1]);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                    return;
                DatabaseAPI.Database.Power[index1] = new Power(frmEditPower.myPower) {IsModified = true};
                if (text == DatabaseAPI.Database.Power[index1].FullName)
                    return;
                int num2 = DatabaseAPI.Database.Power[index1].Effects.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                    DatabaseAPI.Database.Power[index1].Effects[index2].PowerFullName = DatabaseAPI.Database.Power[index1].FullName;
                string[] strArray = DatabaseAPI.UidReferencingPowerFix(text, DatabaseAPI.Database.Power[index1].FullName);
                string str1 = "";
                int num3 = strArray.Length - 1;
                for (int index2 = 0; index2 <= num3; ++index2)
                    str1 = str1 + strArray[index2] + "\r\n";
                if (strArray.Length > 0)
                {
                    string str2 = "Power: " + text + " changed to " + DatabaseAPI.Database.Power[index1].FullName + "\r\nThe following powers referenced this power and were updated:\r\n" + str1 + "\r\n\r\nThis list has been placed on the clipboard.";
                    Clipboard.SetDataObject(str2, true);
                    int num4 = (int)Interaction.MsgBox(str2);
                }
                RefreshLists();
            }
        }

        void btnPowerSort_Click(object sender, EventArgs e)
        {
            BusyMsg("Re-Indexing...");
            Array.Sort(DatabaseAPI.Database.Power);
            DatabaseAPI.MatchAllIDs();
            UpdateLists();
            BusyHide();
        }

        void btnPowerUp_Click(object sender, EventArgs e)
        {
            if (lvPower.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvPower.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            int SelIDX = lvPower.SelectedIndices[0] - 1;
            int index1 = DatabaseAPI.NidFromUidPower(lvPower.Items[selectedIndex].SubItems[3].Text);
            int index2 = DatabaseAPI.NidFromUidPower(lvPower.Items[SelIDX].SubItems[3].Text);
            if (index1 < 0 | index2 < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
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

        void btnPSDown_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvSet.SelectedIndices[0];
            if (selectedIndex >= lvSet.Items.Count - 1)
                return;
            int SelIDX = lvSet.SelectedIndices[0] + 1;
            int index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            int index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[SelIDX].SubItems[3].Text);
            if (index1 < 0 | index2 < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
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

        void btnPSUp_Click(object sender, EventArgs e)

        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvSet.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            int SelIDX = lvSet.SelectedIndices[0] - 1;
            int index1 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[selectedIndex].SubItems[3].Text);
            int index2 = DatabaseAPI.NidFromUidPowerset(lvSet.Items[SelIDX].SubItems[3].Text);
            if (index1 < 0 | index2 < 0)
            {
                Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
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

        void btnSetAdd_Click(object sender, EventArgs e)
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
            frmEditPowerset frmEditPowerset = new frmEditPowerset(ref iSet);
            int num = (int)frmEditPowerset.ShowDialog();
            if (frmEditPowerset.DialogResult != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            IPowerset[] powersetArray = (IPowerset[])Utils.CopyArray(database.Powersets, new IPowerset[DatabaseAPI.Database.Powersets.Length + 1]);
            database.Powersets = powersetArray;
            DatabaseAPI.Database.Powersets[DatabaseAPI.Database.Powersets.Length - 1] =
                new Powerset(frmEditPowerset.myPS) {IsNew = true, nID = DatabaseAPI.Database.Powersets.Length - 1};
            UpdateLists();
        }

        void btnSetDelete_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            int index1 = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (index1 < 0)
            {
                int num1 = (int)Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                string str = "";
                if (DatabaseAPI.Database.Powersets[index1].Powers.Length > 0)
                    str = DatabaseAPI.Database.Powersets[index1].FullName + " still has powers attached to it.\r\nThese powers will be orphaned if you remove the set.\r\n\r\n";
                if (Interaction.MsgBox((str + "Really delete Powerset: " + DatabaseAPI.Database.Powersets[index1].DisplayName + "?"),
                        MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                    return;
                IPowerset[] powersetArray = new IPowerset[DatabaseAPI.Database.Powersets.Length - 1 + 1];
                int index2 = 0;
                int num2 = DatabaseAPI.Database.Powersets.Length - 1;
                for (int index3 = 0; index3 <= num2; ++index3)
                {
                    if (index3 == index1)
                        continue;
                    powersetArray[index2] = new Powerset(DatabaseAPI.Database.Powersets[index3]);
                    ++index2;
                }
                DatabaseAPI.Database.Powersets = new IPowerset[DatabaseAPI.Database.Powersets.Length - 2 + 1];
                int num3 = DatabaseAPI.Database.Powersets.Length - 1;
                for (int index3 = 0; index3 <= num3; ++index3)
                {
                    DatabaseAPI.Database.Powersets[index3] = new Powerset(powersetArray[index3]) {nID = index3};
                }
                int Powerset = -1;
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

        void btnSetEdit_Click(object sender, EventArgs e)
        {
            if (lvSet.SelectedIndices.Count <= 0)
                return;
            int Powerset = DatabaseAPI.NidFromUidPowerset(lvSet.SelectedItems[0].SubItems[3].Text);
            if (Powerset < 0)
            {
                int num = (int)Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.", MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                IPowerset powerset = DatabaseAPI.Database.Powersets[Powerset];
                string fullName = powerset.FullName;
                frmEditPowerset frmEditPowerset = new frmEditPowerset(ref powerset);
                if (frmEditPowerset.ShowDialog() != DialogResult.OK)
                    return;
                DatabaseAPI.Database.Powersets[Powerset] = new Powerset(frmEditPowerset.myPS) {IsModified = true};
                if (DatabaseAPI.Database.Powersets[Powerset].FullName == fullName)
                    return;
                BusyMsg("Re-Indexing...");
                DatabaseAPI.MatchAllIDs();
                RefreshLists(-1, Powerset);
                BusyHide();
            }
        }

        void btnSetSort_Click(object sender, EventArgs e)

        {
            BusyMsg("Re-Indexing...");
            Array.Sort(DatabaseAPI.Database.Powersets);
            DatabaseAPI.MatchAllIDs();
            UpdateLists();
            BusyHide();
        }

        void BuildATImageList()

        {
            ilAT.Images.Clear();
            int num = DatabaseAPI.Database.Classes.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                string str = I9Gfx.GetOriginsPath() + DatabaseAPI.Database.Classes[index].ClassName + ".png";
                if (!File.Exists(str))
                    str = I9Gfx.ImagePath() + "Unknown.png";
                ilAT.Images.Add(new Bitmap(new ExtendedBitmap(str).Bitmap));
            }
        }

        void BuildPowersetImageList(int[] iSets)

        {
            ilPS.Images.Clear();
            Size imageSize = ilPS.ImageSize;
            int width = imageSize.Width;
            imageSize = ilPS.ImageSize;
            int height = imageSize.Height;
            ExtendedBitmap extendedBitmap1 = new ExtendedBitmap(width, height);
            SolidBrush solidBrush1 = new SolidBrush(Color.Black);
            SolidBrush solidBrush2 = new SolidBrush(Color.White);
            SolidBrush solidBrush3 = new SolidBrush(Color.Transparent);
            StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            Font font = new Font(Font, FontStyle.Bold);
            RectangleF layoutRectangle = new RectangleF(17f, 0.0f, 16f, 18f);
            int num = iSets.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                string str = I9Gfx.GetPowersetsPath() + DatabaseAPI.Database.Powersets[iSets[index]].ImageName;
                if (!File.Exists(str))
                    str = I9Gfx.ImagePath() + "Unknown.png";
                ExtendedBitmap extendedBitmap2 = new ExtendedBitmap(str);
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

        void BusyHide()

        {
            if (bFrm == null)
                return;
            bFrm.Close();
            bFrm = null;
        }

        void BusyMsg(string sMessage)

        {
            if (bFrm == null)
            {
                bFrm = new frmBusy();
                bFrm.Show(this);
            }
            bFrm.SetMessage(sMessage);
        }

        void cbFilter_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (Updating)
                return;
            UpdateLists();
        }

        public int[] ConcatArray(int[] iArray1, int[] iArray2)
        {
            int length = iArray1.Length;
            int[] numArray = new int[iArray1.Length + iArray2.Length - 1 + 1];
            int num1 = length - 1;
            for (int index = 0; index <= num1; ++index)
                numArray[index] = iArray1[index];
            int num2 = iArray2.Length - 1;
            for (int index = 0; index <= num2; ++index)
                numArray[length + index] = iArray2[index];
            return numArray;
        }

        public void FillFilter()
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

        void frmPowerBrowser_Load(object sender, EventArgs e)

        {
            try
            {
                FillFilter();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num = (int)MessageBox.Show(ex.Message);
                ProjectData.ClearProjectError();
            }
        }

        [DebuggerStepThrough]

        public void List_Groups(int SelIDX)
        {
            Updating = true;
            lvGroup.BeginUpdate();
            lvGroup.Items.Clear();
            BuildATImageList();
            switch (cbFilter.SelectedIndex)
            {
                case 0:
                {
                    foreach (PowersetGroup powersetGroup in DatabaseAPI.Database.PowersetGroups.Values)
                    {
                        int imageIndex = -1;
                        int num = DatabaseAPI.Database.Classes.Length - 1;
                        for (int index = 0; index <= num; ++index)
                        {
                            if (!(string.Equals(DatabaseAPI.Database.Classes[index].PrimaryGroup, powersetGroup.Name,
                                      StringComparison.OrdinalIgnoreCase) | string.Equals(DatabaseAPI.Database.Classes[index].SecondaryGroup,
                                      powersetGroup.Name, StringComparison.OrdinalIgnoreCase)))
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
                    int num = DatabaseAPI.Database.Classes.Length - 1;
                    for (int imageIndex = 0; imageIndex <= num; ++imageIndex)
                        lvGroup.Items.Add(new ListViewItem(DatabaseAPI.Database.Classes[imageIndex].ClassName, imageIndex));
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
                if (lvGroup.Items.Count > SelIDX & SelIDX > -1)
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

        public void List_Power_AddBlock(int[] iPowers, bool DisplayFullName)
        {
            string[] items = new string[4];
            if (iPowers.Length < 1)
                return;
            int num = iPowers.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (iPowers[index] <= -1 || DatabaseAPI.Database.Power[iPowers[index]].HiddenPower)
                    continue;
                items[0] = !DisplayFullName ? DatabaseAPI.Database.Power[iPowers[index]].PowerName : DatabaseAPI.Database.Power[iPowers[index]].FullName;
                items[1] = DatabaseAPI.Database.Power[iPowers[index]].DisplayName;
                items[2] = Convert.ToString(DatabaseAPI.Database.Power[iPowers[index]].Level);
                items[3] = DatabaseAPI.Database.Power[iPowers[index]].FullName;
                lvPower.Items.Add(new ListViewItem(items)
                {
                    Tag = iPowers[index]
                });
            }
        }

        public void List_Power_AddBlock(string[] iPowers, bool DisplayFullName)
        {
            string[] items = new string[4];
            if (iPowers.Length < 1)
                return;
            int num = iPowers.Length - 1;
            for (int index1 = 0; index1 <= num; ++index1)
            {
                int index2 = DatabaseAPI.NidFromUidPower(iPowers[index1]);
                if (index2 <= -1 || DatabaseAPI.Database.Power[index2].HiddenPower)
                    continue;
                items[0] = !DisplayFullName ? DatabaseAPI.Database.Power[index2].PowerName : DatabaseAPI.Database.Power[index2].FullName;
                items[1] = DatabaseAPI.Database.Power[index2].DisplayName;
                items[2] = Convert.ToString(DatabaseAPI.Database.Power[index2].Level);
                items[3] = DatabaseAPI.Database.Power[index2].FullName;
                lvPower.Items.Add(new ListViewItem(items));
            }
        }

        public void List_Powers(int SelIDX)
        {
            int[] iPowers1 = new int[0];
            string[] iPowers2 = new string[0];
            bool DisplayFullName = false;
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
                        string uidClass = "";
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
                            iPowers1 = DatabaseAPI.NidPowers((int)Math.Round(Conversion.Val(lvSet.SelectedItems[0].SubItems[4].Text)));
                    }

                    break;
                }
                case 4:
                {
                    if (lvSet.SelectedItems.Count > 0)
                    {
                        int index = lvSet.SelectedItems[0].SubItems[4].Text == "" ? -1 : (int)Math.Round(Conversion.Val(lvSet.SelectedItems[0].SubItems[4].Text));
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
                    int num = DatabaseAPI.Database.Power.Length - 1;
                    for (int index = 0; index <= num; ++index)
                    {
                        if (!(DatabaseAPI.Database.Power[index].GroupName == "" | DatabaseAPI.Database.Power[index].SetName == "" |
                              DatabaseAPI.Database.Power[index].GetPowerSet() == null))
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
                    int num = DatabaseAPI.Database.Power.Length - 1;
                    for (int index = 0; index <= num; ++index)
                        iPowers1[index] = index;
                    DisplayFullName = true;
                    break;
                }
            }
            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            if (iPowers2.Length > 0)
                List_Power_AddBlock(iPowers2, DisplayFullName);
            else
                List_Power_AddBlock(iPowers1, DisplayFullName);
            BusyHide();
            if (lvPower.Items.Count > 0)
            {
                if (SelIDX > -1 & SelIDX < lvPower.Items.Count)
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

        public void List_Sets(int SelIDX)
        {
            int[] numArray1 = new int[0];
            int[] numArray2 = new int[0];
            if (lvGroup.SelectedItems.Count == 0 & (cbFilter.SelectedIndex == 0 | cbFilter.SelectedIndex == 1))
                return;
            Updating = true;
            lvSet.BeginUpdate();
            lvSet.Items.Clear();
            if (cbFilter.SelectedIndex == 0 & lvGroup.SelectedItems.Count > 0)
            {
                int[] iSets = DatabaseAPI.NidSets(lvGroup.SelectedItems[0].SubItems[0].Text, "", Enums.ePowerSetType.None);
                BuildPowersetImageList(iSets);
                List_Sets_AddBlock(iSets);
                lvSet.Enabled = true;
            }
            else if (cbFilter.SelectedIndex == 1 & lvGroup.SelectedItems.Count > 0)
            {
                int[] iSets = ConcatArray(ConcatArray(ConcatArray(ConcatArray(DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Primary), DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Secondary)), DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Ancillary)), DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Inherent)), DatabaseAPI.NidSets("", lvGroup.SelectedItems[0].SubItems[0].Text, Enums.ePowerSetType.Pool));
                BuildPowersetImageList(iSets);
                List_Sets_AddBlock(iSets);
                lvSet.Enabled = true;
            }
            else switch (cbFilter.SelectedIndex)
            {
                case 4:
                {
                    int[] numArray3 = new int[0];
                    int num = DatabaseAPI.Database.Powersets.Length - 1;
                    for (int index = 0; index <= num; ++index)
                    {
                        if (!(DatabaseAPI.Database.Powersets[index].GetGroup() == null |
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
                    int[] iSets = DatabaseAPI.NidSets("", "", Enums.ePowerSetType.None);
                    BuildPowersetImageList(iSets);
                    List_Sets_AddBlock(iSets);
                    lvSet.Enabled = true;
                    break;
                }
                default:
                    lvSet.Enabled = false;
                    break;
            }
            if (lvSet.Items.Count > 0)
            {
                if (lvSet.Items.Count > SelIDX & SelIDX > -1)
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

        public void List_Sets_AddBlock(int[] iSets)
        {
            string[] items = new string[5];
            if (iSets.Length < 1)
                return;
            int num = iSets.Length - 1;
            for (int imageIndex = 0; imageIndex <= num; ++imageIndex)
            {
                if (iSets[imageIndex] <= -1)
                    continue;
                items[0] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].SetName;
                items[1] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].DisplayName;
                switch (DatabaseAPI.Database.Powersets[iSets[imageIndex]].SetType)
                {
                    case Enums.ePowerSetType.Primary:
                        items[2] = "Pri";
                        break;
                    case Enums.ePowerSetType.Secondary:
                        items[2] = "Sec";
                        break;
                    case Enums.ePowerSetType.Ancillary:
                        items[2] = "Epic";
                        break;
                    case Enums.ePowerSetType.Inherent:
                        items[2] = "Inh";
                        break;
                    case Enums.ePowerSetType.Pool:
                        items[2] = "Pool";
                        break;
                    case Enums.ePowerSetType.Accolade:
                        items[2] = "Acc";
                        break;
                    default:
                        items[2] = "";
                        break;
                }
                items[3] = DatabaseAPI.Database.Powersets[iSets[imageIndex]].FullName;
                items[4] = Convert.ToString(iSets[imageIndex]);
                lvSet.Items.Add(new ListViewItem(items, imageIndex));
            }
        }

        void lvGroup_DoubleClick(object sender, EventArgs e)

        {
            if (cbFilter.SelectedIndex != 1)
                return;
            btnClassEdit_Click(this, new EventArgs());
        }

        void lvGroup_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (Updating)
                return;
            List_Sets(0);
            Application.DoEvents();
            List_Powers(0);
        }

        void lvPower_DoubleClick(object sender, EventArgs e)

        {
            btnPowerEdit_Click(this, new EventArgs());
        }

        void lvPower_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (lvPower.SelectedItems.Count <= 0)
                return;
            lblPower.Text = lvPower.SelectedItems[0].SubItems[3].Text;
        }

        void lvSet_DoubleClick(object sender, EventArgs e)

        {
            btnSetEdit_Click(this, new EventArgs());
        }

        void lvSet_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (Updating)
                return;
            if (lvSet.SelectedItems.Count > 0)
                lblSet.Text = lvSet.SelectedItems[0].SubItems[3].Text;
            List_Powers(0);
        }

        void RefreshLists(int Group = -1, int Powerset = -1, int Power = -1)

        {
            int SelectGroup = Group;
            int SelectSet = Powerset;
            int SelectPower = Power;
            if (lvGroup.SelectedIndices.Count > 0 & SelectGroup == -1)
                SelectGroup = lvGroup.SelectedIndices[0];
            if (lvSet.SelectedIndices.Count > 0 & SelectSet == -1)
                SelectSet = lvSet.SelectedIndices[0];
            if (lvPower.SelectedIndices.Count > 0 & SelectPower == -1)
                SelectPower = lvPower.SelectedIndices[0];
            UpdateLists(SelectGroup, SelectSet, SelectPower);
        }

        void UpdateLists(int SelectGroup = -1, int SelectSet = -1, int SelectPower = -1)

        {
            List_Groups(SelectGroup);
            Application.DoEvents();
            List_Sets(SelectSet);
            Application.DoEvents();
            List_Powers(SelectPower);
        }
    }
}