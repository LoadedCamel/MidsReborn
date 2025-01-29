using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Controls.Extensions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEnhEdit : Form
    {
        private frmBusy _bFrm;
        private frmMain _frmMain;
        private List<string> SpecialEnhTypes;
        private List<string[]> LvItems;
        private ListViewExt.LvKeyboardNavHandler LvKbHandler;

        public frmEnhEdit()
        {
            Load += frmEnhEdit_Load;
            InitializeComponent();
            Name = nameof(frmEnhEdit);
            Icon = Resources.MRB_Icon_Concept;
            SpecialEnhTypes = DatabaseAPI.Database.SpecialEnhancements.Select(specEnh => specEnh.Name.Replace(" Origin", string.Empty)).ToList();
            LvKbHandler = lvEnh.AssignKeyboardNavHandler();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            IEnhancement iEnh = new Enhancement();
            using var frmEnhData = new frmEnhData(ref iEnh, DatabaseAPI.Database.Enhancements[^1].StaticIndex + 1);
            frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
            {
                return;
            }
            
            var dbEnhancements = DatabaseAPI.Database.Enhancements.ToList();
            var newEnhancement = new Enhancement(frmEnhData.myEnh) { IsNew = true };
            dbEnhancements.Add(newEnhancement);
            DatabaseAPI.Database.Enhancements = dbEnhancements.ToArray();
            if (newEnhancement.nIDSet > 0)
            {
                var es = DatabaseAPI.Database.EnhancementSets[newEnhancement.nIDSet];
                var setEnhancements = es.Enhancements.ToList();
                setEnhancements.Add(newEnhancement.StaticIndex);
                es.Enhancements = setEnhancements.ToArray();
            }

            DisplayList(true);
            lvEnh.Items[^1].Selected = true;
            lvEnh.Items[^1].EnsureVisible();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
            {
                return;
            }

            var oldEnhancement = DatabaseAPI.Database.Enhancements[DatabaseAPI.GetEnhancementByUIDName(LvItems[lvEnh.SelectedIndices[0]][6])];
            var maxStaticIndex = DatabaseAPI.Database.Enhancements.Max(e => e.StaticIndex);
            using var frmEnhData = new frmEnhData(ref oldEnhancement, maxStaticIndex + 1);
            frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
            {
                return;
            }

            var dbEnhancements = DatabaseAPI.Database.Enhancements.ToList();
            var newEnhancement = new Enhancement(frmEnhData.myEnh) { IsNew = true, StaticIndex = -1 };
            dbEnhancements.Add(newEnhancement);
            DatabaseAPI.Database.Enhancements = dbEnhancements.ToArray();
            ImageUpdate();
            lvEnh.VirtualListSize = 0;
            lvEnh.VirtualListSize = DatabaseAPI.Database.Enhancements.Length - 1;
            lvEnh.Invalidate();
            //AddListItem(DatabaseAPI.Database.Enhancements.Length - 1);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0 || MessageBox.Show($"Really delete enhancement: {LvItems[lvEnh.SelectedIndices[0]][0]}?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            var enhancementArray = new Enhancement[DatabaseAPI.Database.Enhancements.Length];
            var selectedIndex = DatabaseAPI.GetEnhancementByUIDName(LvItems[lvEnh.SelectedIndices[0]][6]);
            var index1 = 0;
            var enh = DatabaseAPI.Database.Enhancements[selectedIndex];
            if (enh.nIDSet > -1)
            {
                //Remove it from the enhancement set too.

                DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements = DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements.Where(staticIndex => staticIndex != enh.StaticIndex).ToArray();
            }

            for (var index2 = 0; index2 < DatabaseAPI.Database.Enhancements.Length; index2++)
            {
                if (index2 == selectedIndex)
                {
                    continue;
                }

                enhancementArray[index1++] = new Enhancement(DatabaseAPI.Database.Enhancements[index2]);
            }

            DatabaseAPI.Database.Enhancements = new IEnhancement[DatabaseAPI.Database.Enhancements.Length - 1];
            for (var index2 = 0; index2 < DatabaseAPI.Database.Enhancements.Length; index2++)
            {
                DatabaseAPI.Database.Enhancements[index2] = new Enhancement(enhancementArray[index2]);
            }

            DisplayList(true);
            if (lvEnh.Items.Count <= 0)
            {
                return;
            }

            if (lvEnh.Items.Count > selectedIndex)
            {
                lvEnh.Items[selectedIndex].Selected = true;
                lvEnh.Items[selectedIndex].EnsureVisible();
            }
            else if (lvEnh.Items.Count == selectedIndex)
            {
                lvEnh.Items[selectedIndex - 1].Selected = true;
                lvEnh.Items[selectedIndex - 1].EnsureVisible();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
            {
                return;
            }

            var enhIndex = DatabaseAPI.GetEnhancementByUIDName(LvItems[lvEnh.SelectedIndices[0]][6]);
            var selectedIndex = lvEnh.SelectedIndices[0];
            if (selectedIndex >= lvEnh.Items.Count - 1)
            {
                return;
            }

            var enhList = DatabaseAPI.Database.Enhancements.ToList();
            enhList.Reverse(enhIndex, 2);
            DatabaseAPI.Database.Enhancements = enhList.ToArray();
            DisplayList(true);
            lvEnh.Items[selectedIndex + 1].Selected = true;
            lvEnh.Items[selectedIndex + 1].EnsureVisible();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
            {
                return;
            }

            var selectedIndex = DatabaseAPI.GetEnhancementByUIDName(LvItems[lvEnh.SelectedIndices[0]][6]);
            using var frmEnhData = new frmEnhData(ref DatabaseAPI.Database.Enhancements[selectedIndex], 0);
            frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
            {
                return;
            }

            var newEnhancement = new Enhancement(frmEnhData.myEnh) { IsModified = true };
            DatabaseAPI.Database.Enhancements[lvEnh.SelectedIndices[0]] = newEnhancement;
            UpdateListItem(selectedIndex);
            DisplayList();
        }

        private void BusyHide()
        {
            _bFrm.Close();
        }

        private void BusyMsg(string sMessage)
        {
            _bFrm.SetMessage(sMessage);
            _bFrm.Show(this);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            BusyMsg("Saving, please wait...");
            await I9Gfx.LoadEnhancements();
            foreach (var power in DatabaseAPI.Database.Power)
            {
                power.BaseRechargeTime = power.RechargeTime;
            }

            Array.Sort(DatabaseAPI.Database.Power);
            var serializer = Serializer.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.AssignRecipeIDs();
            DatabaseAPI.SaveEnhancementDb(serializer, MidsContext.Config.DataPath);
            DatabaseAPI.MatchAllIDs();
            Task.Delay(1000).Wait();
            DatabaseAPI.SaveMainDatabase(serializer, MidsContext.Config.DataPath);
            frmMain.MainInstance?.UpdateTitle();
            BusyHide();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
            {
                return;
            }

            var enhIndex = DatabaseAPI.GetEnhancementByUIDName(LvItems[lvEnh.SelectedIndices[0]][6]);
            var selectedIndex = lvEnh.SelectedIndices[0];
            if (selectedIndex < 1)
            {
                return;
            }

            var enhList = DatabaseAPI.Database.Enhancements.ToList();
            enhList.Reverse(enhIndex - 1, 2);
            DatabaseAPI.Database.Enhancements = enhList.ToArray();
            DisplayList(true);
            lvEnh.Items[selectedIndex - 1].Selected = true;
            lvEnh.Items[selectedIndex - 1].EnsureVisible();
        }

        private void DisplayList(bool forceUpdate = false)
        {
            ImageUpdate();

            LvItems = [];
            for (var index = 0; index < DatabaseAPI.Database.Enhancements.Length; index++)
            {
                if (!string.IsNullOrWhiteSpace(txtFilter.Text) &&
                    !DatabaseAPI.Database.Enhancements[index].LongName.ToLowerInvariant().Contains(txtFilter.Text.ToLowerInvariant()))
                {
                    continue;
                }

                LvItems.Add(GetEnhancementData(index));
            }

            if (forceUpdate)
            {
                lvEnh.VirtualListSize = 0;
            }

            lvEnh.VirtualListSize = LvItems.Count;
        }

        private void FillImageList()
        {
            var imageSize1 = ilEnh.ImageSize;
            var width1 = imageSize1.Width;
            imageSize1 = ilEnh.ImageSize;
            var height1 = imageSize1.Height;
            using var extendedBitmap = new ExtendedBitmap(width1, height1);
            ilEnh.Images.Clear();
            foreach (var enh in DatabaseAPI.Database.Enhancements)
            {
                if (!string.IsNullOrWhiteSpace(txtFilter.Text) &&
                    !enh.LongName.ToLowerInvariant().Contains(txtFilter.Text.ToLowerInvariant()))
                {
                    continue;
                }

                if (enh.ImageIdx > -1)
                {
                    extendedBitmap.Graphics.Clear(Color.Transparent);
                    var graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancement(ref graphics, enh.ImageIdx, I9Gfx.ToGfxGrade(enh.TypeID));
                    ilEnh.Images.Add(extendedBitmap.Bitmap);
                }
                else
                {
                    var images = ilEnh.Images;
                    var imageSize2 = ilEnh.ImageSize;
                    var width2 = imageSize2.Width;
                    imageSize2 = ilEnh.ImageSize;
                    var height2 = imageSize2.Height;
                    var bitmap = new Bitmap(width2, height2);
                    images.Add(bitmap);
                }
            }
        }

        private void frmEnhEdit_Load(object sender, EventArgs e)
        {
            _bFrm = new frmBusy();
            lvEnh.EnableDoubleBuffer();
            Show();
            Refresh();
            DisplayList();
            lblLoading.Visible = false;
            lvEnh.Select();
        }

        private async void ImageUpdate()
        {
            if (NoReload.Checked)
            {
                return;
            }

            await I9Gfx.LoadEnhancements();
            FillImageList();
        }

        private void lvEnh_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void NoReload_CheckedChanged(object sender, EventArgs e)
        {
            ImageUpdate();
        }

        private string[] GetEnhancementData(int index)
        {
            var item = new string[7];
            var enhancement = DatabaseAPI.Database.Enhancements[index];
            item[0] = $"{enhancement.Name} ({enhancement.ShortName}) - {enhancement.StaticIndex}";
            item[1] = Enum.GetName(typeof(Enums.eType), enhancement.TypeID);
            item[2] = $"{enhancement.LevelMin + 1}-{enhancement.LevelMax + 1}";
            item[3] = $"{enhancement.Effect.Length}";
            item[4] = string.Join(", ", enhancement.ClassID.Select(c => DatabaseAPI.Database.EnhancementClasses[c].ShortName));
            item[6] = enhancement.UID;

            if (enhancement.nIDSet > -1)
            {
                item[5] = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName;
                item[0] = $"{item[5]}: {item[0]}";
            }
            else
            {
                item[5] = "";
            }

            return item;
        }

        private void UpdateListItem(int index) => LvItems[index] = GetEnhancementData(index);

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            DisplayList();
            btnUp.Enabled = string.IsNullOrEmpty(txtFilter.Text);
            btnDown.Enabled = string.IsNullOrEmpty(txtFilter.Text);
        }

        private void lvEnh_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= LvItems.Count)
            {
                //Debug.WriteLine($"frmEnhEdit.lvEnh_RetrieveVirtualItem() out of range exception - trying to get index {e.ItemIndex} / max index: {(LvItems.Count == 0 ? "(no items)" : $"{LvItems.Count - 1}")}");
                e.Item = new ListViewItem(LvItems[0], 0);
                
                return;
            }

            e.Item = new ListViewItem(LvItems[e.ItemIndex], e.ItemIndex);
        }

        private void lvEnh_KeyPress(object sender, KeyPressEventArgs e)
        {
            LvKbHandler.ProcessInput(e.KeyChar.ToString(), LvItems, 0);
        }
    }
}