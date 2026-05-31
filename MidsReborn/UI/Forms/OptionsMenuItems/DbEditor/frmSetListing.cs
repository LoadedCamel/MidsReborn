using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Design.Extensions;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmSetListing : Form
    {
        public frmSetListing()
        {
            Load += frmSetListing_Load;
            InitializeComponent();
            ApplyMinimumIconLayout();
            Name = nameof(frmSetListing);
            Icon = Resources.MRB_Icon_Concept;
        }

        private void ApplyMinimumIconLayout()
        {
            var iconSize = Math.Max(DbEditorIconLayout.MinimumIconSize, Math.Max(ilSets.ImageSize.Width, ilSets.ImageSize.Height));
            ilSets.ImageSize = new Size(iconSize, iconSize);
        }

        private void AddListItem(int idx)
        {
            var items = new string[7];
            var enhancementSet = DatabaseAPI.Database.EnhancementSets[idx];
            items[0] = $"{enhancementSet.DisplayName} ({enhancementSet.ShortName})";
            items[1] = DatabaseAPI.GetSetTypeByIndex(enhancementSet.SetType).ShortName;
            items[2] = $"{enhancementSet.LevelMin + 1}";
            items[3] = $"{enhancementSet.LevelMax + 1}";
            items[4] = $"{enhancementSet.Enhancements.Length}";
            var num1 = 0;
            for (var index = 0; index < enhancementSet.Bonus.Length; index++)
            {
                if (enhancementSet.Bonus[index].Index.Length > 0)
                {
                    ++num1;
                }
            }

            items[5] = $"{num1}";

            items[6] = SetContainsPvPfx(enhancementSet) ? "X" : "";
            lvSets.Items.Add(new ListViewItem(items, idx));
            lvSets.Items[^1].Selected = true;
            lvSets.Items[^1].EnsureVisible();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var iSet = new EnhancementSet();
            var isPvP = MessageBox.Show(@"Will this set be used with PvP?", "", MessageBoxButtons.YesNo);
            switch (isPvP)
            {
                case DialogResult.Yes:
                {
                    using var frmSetEdit = new FrmSetEditPvP(ref iSet);
                    var num = (int)frmSetEdit.ShowDialog();
                    if (frmSetEdit.DialogResult != DialogResult.OK)
                        return;
                    DatabaseAPI.Database.EnhancementSets.Add(new EnhancementSet(frmSetEdit.MySet));
                    break;
                }
                case DialogResult.No:
                {
                    using var frmSetEdit = new FrmSetEdit(ref iSet);
                    var num = (int)frmSetEdit.ShowDialog();
                    if (frmSetEdit.DialogResult != DialogResult.OK)
                        return;
                    DatabaseAPI.Database.EnhancementSets.Add(new EnhancementSet(frmSetEdit.MySet));
                    break;
                }
            }
            ImageUpdate();
            AddListItem(DatabaseAPI.Database.EnhancementSets.Count - 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (lvSets.SelectedIndices.Count <= 0)
                return;
            var iSet = new EnhancementSet(DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]]);
            iSet.DisplayName += " Copy";
            var setContainsPvPfx = SetContainsPvPfx(iSet);

            if (setContainsPvPfx)
            {
                using var frmSetEdit = new FrmSetEditPvP(ref iSet);
                frmSetEdit.ShowDialog();
                if (frmSetEdit.DialogResult != DialogResult.OK)
                    return;
                DatabaseAPI.Database.EnhancementSets.Add(new EnhancementSet(frmSetEdit.MySet));
            }
            else
            {
                using var frmSetEdit = new FrmSetEdit(ref iSet);
                frmSetEdit.ShowDialog();
                if (frmSetEdit.DialogResult != DialogResult.OK)
                    return;
                DatabaseAPI.Database.EnhancementSets.Add(new EnhancementSet(frmSetEdit.MySet));
            }
            ImageUpdate();
            AddListItem(DatabaseAPI.Database.EnhancementSets.Count - 1);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvSets.SelectedIndices.Count <= 0 || MessageBox.Show($@"Really delete Set: {lvSets.SelectedItems[0].Text}?", @"Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var selectedIndex = lvSets.SelectedIndices[0];
            if (DatabaseAPI.Database.EnhancementSets[selectedIndex].Enhancements.Length > 0)
            {
                MessageBox.Show(@"You need to remove all enhancements from this set before you can delete the set!", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            DatabaseAPI.Database.EnhancementSets.RemoveAt(selectedIndex);
            DatabaseAPI.MatchEnhancementIDs();
            DisplayList();
            if (lvSets.Items.Count <= 0)
                return;
            if (lvSets.Items.Count > selectedIndex)
                lvSets.Items[selectedIndex].Selected = true;
            else if (lvSets.Items.Count == selectedIndex)
                lvSets.Items[selectedIndex - 1].Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lvSets.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvSets.SelectedIndices[0];
            if (selectedIndex >= lvSets.Items.Count - 1)
                return;
            EnhancementSet[] enhancementSetArray =
            {
                new EnhancementSet(DatabaseAPI.Database.EnhancementSets[selectedIndex]),
                new EnhancementSet(DatabaseAPI.Database.EnhancementSets[selectedIndex + 1])
            };
            DatabaseAPI.Database.EnhancementSets[selectedIndex + 1] = new EnhancementSet(enhancementSetArray[0]);
            DatabaseAPI.Database.EnhancementSets[selectedIndex] = new EnhancementSet(enhancementSetArray[1]);
            DatabaseAPI.MatchEnhancementIDs();
            var listViewItem1 = (ListViewItem)lvSets.Items[selectedIndex].Clone();
            var listViewItem2 = (ListViewItem)lvSets.Items[selectedIndex + 1].Clone();
            lvSets.Items[selectedIndex] = listViewItem2;
            lvSets.Items[selectedIndex + 1] = listViewItem1;
            lvSets.Items[selectedIndex + 1].Selected = true;
            lvSets.Items[selectedIndex + 1].EnsureVisible();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvSets.SelectedIndices.Count <= 0)
                return;
            var flag = false;
            var uidOld = "";
            var selectedIndex1 = lvSets.SelectedIndices[0];
            var enhancementSets = DatabaseAPI.Database.EnhancementSets;
            var selectedIndex2 = lvSets.SelectedIndices[0];
            /*string setName = Regex.Replace(lvSets.SelectedItems[0].Text, @"( \(\w+\))", "");
            var setNameIndex = enhancementSets.FindIndex(x => x.DisplayName == setName);*/
            var iSet = enhancementSets[selectedIndex2];
            enhancementSets[selectedIndex2] = iSet;


            var setContainsPvPfx = SetContainsPvPfx(iSet);

            if (setContainsPvPfx)
            {
                using var frmSetEdit = new FrmSetEditPvP(ref iSet);
                frmSetEdit.ShowDialog();
                if (frmSetEdit.DialogResult != DialogResult.OK)
                    return;
                if (frmSetEdit.MySet.Uid != DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]].Uid)
                {
                    flag = true;
                    uidOld = DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]].Uid;
                }

                DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]] = new EnhancementSet(frmSetEdit.MySet);
                ImageUpdate();
                UpdateListItem(selectedIndex1);
                if (!flag)
                    return;
                RenameIOSet(uidOld, frmSetEdit.MySet.Uid);
            }
            else
            {
                using var frmSetEdit = new FrmSetEdit(ref iSet);
                frmSetEdit.ShowDialog();
                if (frmSetEdit.DialogResult != DialogResult.OK)
                    return;
                if (frmSetEdit.MySet.Uid != DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]].Uid)
                {
                    flag = true;
                    uidOld = DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]].Uid;
                }

                DatabaseAPI.Database.EnhancementSets[lvSets.SelectedIndices[0]] = new EnhancementSet(frmSetEdit.MySet);
                ImageUpdate();
                UpdateListItem(selectedIndex1);
                if (!flag)
                    return;
                RenameIOSet(uidOld, frmSetEdit.MySet.Uid);
            }
            DatabaseAPI.MatchEnhancementIDs();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var serializer = Serializer.GetSerializer();
            DatabaseAPI.SaveEnhancementDb(serializer, MidsContext.Config.SavePath);
            Hide();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lvSets.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvSets.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            EnhancementSet[] enhancementSetArray =
            {
                new EnhancementSet(DatabaseAPI.Database.EnhancementSets[selectedIndex]),
                new EnhancementSet(DatabaseAPI.Database.EnhancementSets[selectedIndex - 1])
            };
            DatabaseAPI.Database.EnhancementSets[selectedIndex - 1] = new EnhancementSet(enhancementSetArray[0]);
            DatabaseAPI.Database.EnhancementSets[selectedIndex] = new EnhancementSet(enhancementSetArray[1]);
            DatabaseAPI.MatchEnhancementIDs();
            var listViewItem1 = (ListViewItem)lvSets.Items[selectedIndex].Clone();
            var listViewItem2 = (ListViewItem)lvSets.Items[selectedIndex - 1].Clone();
            lvSets.Items[selectedIndex] = listViewItem2;
            lvSets.Items[selectedIndex - 1] = listViewItem1;
            lvSets.Items[selectedIndex - 1].Selected = true;
            lvSets.Items[selectedIndex - 1].EnsureVisible();
        }

        private void DisplayList()
        {
            lvSets.BeginUpdate();
            lvSets.Items.Clear();
            ImageUpdate();

            for (var index = 0; index < DatabaseAPI.Database.EnhancementSets.Count; index++)
            {
                AddListItem(index);
            }

            if (lvSets.Items.Count > 0)
            {
                lvSets.Items[0].Selected = true;
                lvSets.Items[0].EnsureVisible();
            }

            lvSets.EndUpdate();
        }

        private void FillImageList()
        {
            using var extendedBitmap = new ExtendedBitmap(ilSets.ImageSize);
            ilSets.Images.Clear();

            for (var index = 0; index < DatabaseAPI.Database.EnhancementSets.Count; index++)
            {
                var enhSet = DatabaseAPI.Database.EnhancementSets[index];
                if (enhSet.ImageIdx > -1)
                {
                    extendedBitmap.Graphics?.Clear(Color.Transparent);
                    AssetManager.DrawEnhancementSet(extendedBitmap.Graphics, index);
                    ilSets.Images.Add(extendedBitmap.Bitmap);
                }
                else
                {
                    var images = ilSets.Images;
                    var bitmap = new Bitmap(ilSets.ImageSize.Width, ilSets.ImageSize.Height);
                    images.Add(bitmap);
                }
            }
        }

        private void frmSetListing_Load(object? sender, EventArgs e)
        {
            lvSets.EnableDoubleBuffer();
            DisplayList();
        }

        private void ImageUpdate()
        {
            if (NoReload.Checked)
                return;
            
            FillImageList();
        }

        [DebuggerStepThrough]
        private void lvSets_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void lvSets_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void NoReload_CheckedChanged(object sender, EventArgs e)
        {
            ImageUpdate();
        }

        private static bool SetContainsPvPfx(EnhancementSet enhancementSet)
        {
            foreach (var bonus in enhancementSet.Bonus)
            {
                foreach (var powerIndex in bonus.Index)
                {
                    if (!IsValidPowerIndex(powerIndex))
                    {
                        continue;
                    }

                    var fullName = DatabaseAPI.Database.Power[powerIndex]?.FullName;
                    if (!string.IsNullOrEmpty(fullName) &&
                        fullName.Contains("pvp", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsValidPowerIndex(int powerIndex)
        {
            return DatabaseAPI.Database?.Power != null &&
                   powerIndex >= 0 &&
                   powerIndex < DatabaseAPI.Database.Power.Length &&
                   DatabaseAPI.Database.Power[powerIndex] != null;
        }

        private static void RenameIOSet(string uidOld, string uidNew)
        {
            var num = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (DatabaseAPI.Database.Enhancements[index].UIDSet == uidOld)
                    DatabaseAPI.Database.Enhancements[index].UIDSet = uidNew;
        }

        private void UpdateListItem(int Index)
        {
            var strArray = new string[7];
            var enhancementSet = DatabaseAPI.Database.EnhancementSets[Index];
            strArray[0] = enhancementSet.DisplayName + " (" + enhancementSet.ShortName + ")";
            strArray[1] = DatabaseAPI.GetSetTypeByIndex(enhancementSet.SetType).ShortName;
            strArray[2] = Convert.ToString(enhancementSet.LevelMin + 1, CultureInfo.InvariantCulture);
            strArray[3] = Convert.ToString(enhancementSet.LevelMax + 1, CultureInfo.InvariantCulture);
            strArray[4] = Convert.ToString(enhancementSet.Enhancements.Length, CultureInfo.InvariantCulture);
            var num1 = 0;
            var num2 = enhancementSet.Bonus.Length - 1;
            for (var index = 0; index <= num2; ++index)
                if (enhancementSet.Bonus[index].Index.Length > 0)
                    ++num1;
            strArray[5] = Convert.ToString(num1);

            strArray[6] = SetContainsPvPfx(enhancementSet) ? "X" : "";
            var num3 = strArray.Length - 1;
            for (var index = 0; index <= num3; ++index) lvSets.Items[Index].SubItems[index].Text = strArray[index];
            lvSets.Items[Index].ImageIndex = Index;
            lvSets.Refresh();
        }
    }
}
