using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.My;
using mrbBase;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmSalvageEdit : Form
    {
        private bool Updating;

        public frmSalvageEdit()
        {
            Load += frmSalvageEdit_Load;
            Updating = true;
            InitializeComponent();
            Name = nameof(frmSalvageEdit);
            var componentResourceManager = new ComponentResourceManager(typeof(frmSalvageEdit));
            Icon = Resources.reborn;
        }

        private void AddListItem(int Index)
        {
            var items = new string[4];
            if ((Index > DatabaseAPI.Database.Salvage.Length - 1) | (Index < 0)) return;
            items[0] = DatabaseAPI.Database.Salvage[Index].ExternalName;
            items[1] = Enum.GetName(DatabaseAPI.Database.Salvage[Index].Origin.GetType(),
                DatabaseAPI.Database.Salvage[Index].Origin);
            items[2] = Enum.GetName(DatabaseAPI.Database.Salvage[Index].Rarity.GetType(),
                DatabaseAPI.Database.Salvage[Index].Rarity);
            items[3] = Convert.ToString(DatabaseAPI.Database.Salvage[Index].LevelMin + 1) + " - " +
                       Convert.ToString(DatabaseAPI.Database.Salvage[Index].LevelMax + 1);
            lvSalvage.Items.Add(new ListViewItem(items));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var database = DatabaseAPI.Database;
            var salvageArray =
                (Salvage[]) Utils.CopyArray(database.Salvage, new Salvage[DatabaseAPI.Database.Salvage.Length + 1]);
            database.Salvage = salvageArray;
            DatabaseAPI.Database.Salvage[DatabaseAPI.Database.Salvage.Length - 1] = new Salvage();
            AddListItem(DatabaseAPI.Database.Salvage.Length - 1);
            lvSalvage.Items[lvSalvage.Items.Count - 1].Selected = true;
            lvSalvage.Items[lvSalvage.Items.Count - 1].EnsureVisible();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DatabaseAPI.LoadSalvage();
            Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedItems.Count < 1 || Updating)
                return;
            var selectedIndex = lvSalvage.SelectedIndices[0];
            var salvageArray = new Salvage[DatabaseAPI.Database.Salvage.Length - 2 + 1];
            var index1 = -1;
            var num1 = DatabaseAPI.Database.Salvage.Length - 1;
            for (var index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                ++index1;
                salvageArray[index1] = new Salvage(ref DatabaseAPI.Database.Salvage[index2]);
            }

            DatabaseAPI.Database.Salvage = new Salvage[salvageArray.Length - 1 + 1];
            var num2 = DatabaseAPI.Database.Salvage.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
                DatabaseAPI.Database.Salvage[index2] = new Salvage(ref salvageArray[index2]);
            FillList();
            if (lvSalvage.Items.Count > selectedIndex)
                lvSalvage.Items[selectedIndex].Selected = true;
            else if (lvSalvage.Items.Count > selectedIndex - 1)
                lvSalvage.Items[selectedIndex - 1].Selected = true;
            else if (lvSalvage.Items.Count > 0)
                lvSalvage.Items[0].Selected = true;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            char[] chArray = {'\r'};
            var strArray1 = Clipboard.GetDataObject()?.GetData("System.String", true).ToString().Split(chArray);
            chArray[0] = '\t';
            DatabaseAPI.Database.Salvage = new Salvage[0];
            var num = strArray1.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var strArray2 = strArray1[index].Split(chArray);
                if (strArray2.Length <= 7) continue;
                var database = DatabaseAPI.Database;
                var salvageArray = (Salvage[]) Utils.CopyArray(database.Salvage,
                    new Salvage[DatabaseAPI.Database.Salvage.Length + 1]);
                database.Salvage = salvageArray;
                DatabaseAPI.Database.Salvage[DatabaseAPI.Database.Salvage.Length - 1] = new Salvage();
                var salvage = DatabaseAPI.Database.Salvage[DatabaseAPI.Database.Salvage.Length - 1];
                if (!strArray2[0].StartsWith("S") & (strArray2[0].Length > 2))
                    strArray2[0] = strArray2[0].Substring(1);
                salvage.InternalName = strArray2[0];
                salvage.ExternalName = strArray2[1];
                if (strArray2[10].StartsWith("10"))
                {
                    salvage.LevelMin = 9;
                    salvage.LevelMax = 24;
                }
                else if (strArray2[10].StartsWith("26"))
                {
                    salvage.LevelMin = 25;
                    salvage.LevelMax = 39;
                }
                else
                {
                    salvage.LevelMin = 40;
                    salvage.LevelMax = 52;
                }

                salvage.Origin = strArray2[9].IndexOf("Magic", StringComparison.Ordinal) <= -1
                    ? Salvage.SalvageOrigin.Tech
                    : Salvage.SalvageOrigin.Magic;
                salvage.Rarity = (Recipe.RecipeRarity) Math.Round(Conversion.Val(strArray2[6]) - 1.0);
            }

            FillList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DatabaseAPI.SaveSalvage(MyApplication.GetSerializer());
            Close();
        }

        private void cbLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedItems.Count < 1 || Updating) return;
            var selectedIndex = lvSalvage.SelectedIndices[0];
            switch (cbLevel.SelectedIndex)
            {
                case 0:
                    DatabaseAPI.Database.Salvage[selectedIndex].LevelMin = 9;
                    DatabaseAPI.Database.Salvage[selectedIndex].LevelMax = 24;
                    break;
                case 1:
                    DatabaseAPI.Database.Salvage[selectedIndex].LevelMin = 25;
                    DatabaseAPI.Database.Salvage[selectedIndex].LevelMax = 39;
                    break;
                default:
                    DatabaseAPI.Database.Salvage[selectedIndex].LevelMin = 40;
                    DatabaseAPI.Database.Salvage[selectedIndex].LevelMax = 52;
                    break;
            }

            UpdateListItem(selectedIndex);
        }

        private void cbOrigin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedItems.Count < 1 || Updating) return;
            var selectedIndex = lvSalvage.SelectedIndices[0];
            DatabaseAPI.Database.Salvage[selectedIndex].Origin = (Salvage.SalvageOrigin) cbOrigin.SelectedIndex;
            UpdateListItem(selectedIndex);
        }

        private void cbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedItems.Count < 1 || Updating) return;
            var selectedIndex = lvSalvage.SelectedIndices[0];
            DatabaseAPI.Database.Salvage[selectedIndex].Rarity = (Recipe.RecipeRarity) cbRarity.SelectedIndex;
            UpdateListItem(selectedIndex);
        }

        private void DisplayItem(int Index)
        {
            if ((Index > DatabaseAPI.Database.Salvage.Length - 1) | (Index < 0)) return;
            Updating = true;
            cbRarity.SelectedIndex = (int) DatabaseAPI.Database.Salvage[Index].Rarity;
            cbOrigin.SelectedIndex = (int) DatabaseAPI.Database.Salvage[Index].Origin;
            var levelMin = DatabaseAPI.Database.Salvage[Index].LevelMin;
            if (levelMin < 25)
                cbLevel.SelectedIndex = 0;
            else if (levelMin < 40)
                cbLevel.SelectedIndex = 1;
            else if (levelMin < 53)
                cbLevel.SelectedIndex = 2;
            txtExternal.Text = DatabaseAPI.Database.Salvage[Index].ExternalName;
            txtInternal.Text = DatabaseAPI.Database.Salvage[Index].InternalName;
            Updating = false;
        }

        private void FillList()
        {
            lvSalvage.BeginUpdate();
            lvSalvage.Items.Clear();
            var num = DatabaseAPI.Database.Salvage.Length - 1;
            for (var Index = 0; Index <= num; ++Index)
                AddListItem(Index);
            lvSalvage.EndUpdate();
        }

        private void frmSalvageEdit_Load(object sender, EventArgs e)
        {
            var salvageOrigin = Salvage.SalvageOrigin.Tech;
            var recipeRarity = Recipe.RecipeRarity.Common;
            FillList();
            cbRarity.Items.AddRange(Enum.GetNames(recipeRarity.GetType()));
            cbOrigin.Items.AddRange(Enum.GetNames(salvageOrigin.GetType()));
            cbLevel.Items.Add("10 - 25");
            cbLevel.Items.Add("26 - 40");
            cbLevel.Items.Add("41 - 53");
            Updating = false;
            if (lvSalvage.Items.Count <= 0) return;
            lvSalvage.Items[0].Selected = true;
        }

        [DebuggerStepThrough]
        private void lvSalvage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedIndices.Count <= 0) return;
            DisplayItem(lvSalvage.SelectedIndices[0]);
        }

        private void txtExternal_TextChanged(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedItems.Count < 1 || Updating) return;
            var selectedIndex = lvSalvage.SelectedIndices[0];
            DatabaseAPI.Database.Salvage[selectedIndex].ExternalName = txtExternal.Text;
            UpdateListItem(selectedIndex);
        }

        private void txtInternal_TextChanged(object sender, EventArgs e)
        {
            if (lvSalvage.SelectedItems.Count < 1 || Updating) return;
            var selectedIndex = lvSalvage.SelectedIndices[0];
            DatabaseAPI.Database.Salvage[selectedIndex].InternalName = txtInternal.Text;
            UpdateListItem(selectedIndex);
        }

        private void UpdateListItem(int Index)
        {
            if ((Index > DatabaseAPI.Database.Salvage.Length - 1) | (Index < 0)) return;
            lvSalvage.Items[Index].SubItems[0].Text = DatabaseAPI.Database.Salvage[Index].ExternalName;
            lvSalvage.Items[Index].SubItems[1].Text = Enum.GetName(DatabaseAPI.Database.Salvage[Index].Origin.GetType(),
                DatabaseAPI.Database.Salvage[Index].Origin);
            lvSalvage.Items[Index].SubItems[2].Text = Enum.GetName(DatabaseAPI.Database.Salvage[Index].Rarity.GetType(),
                DatabaseAPI.Database.Salvage[Index].Rarity);
            lvSalvage.Items[Index].SubItems[3].Text =
                Convert.ToString(DatabaseAPI.Database.Salvage[Index].LevelMin + 1) + " - " +
                Convert.ToString(DatabaseAPI.Database.Salvage[Index].LevelMax + 1);
        }
    }
}