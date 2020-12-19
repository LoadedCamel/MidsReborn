using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Display;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEnhEdit : Form
    {
        public frmEnhEdit()
        {
            Load += frmEnhEdit_Load;
            InitializeComponent();
            Name = nameof(frmEnhEdit);
            var componentResourceManager = new ComponentResourceManager(typeof(frmEnhEdit));
            Icon = Resources.reborn;
        }

        private void AddListItem(int Index)
        {
            var items = new string[6];
            var enhancement = DatabaseAPI.Database.Enhancements[Index];
            items[0] = enhancement.Name + " (" + enhancement.ShortName + ") - " +
                       Convert.ToString(enhancement.StaticIndex, CultureInfo.InvariantCulture);
            items[1] = Enum.GetName(enhancement.TypeID.GetType(), enhancement.TypeID);
            items[2] = Convert.ToString(enhancement.Effect.Length, CultureInfo.InvariantCulture);
            items[3] = "";
            items[5] = enhancement.UID;
            var num1 = enhancement.ClassID.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                if (!string.IsNullOrWhiteSpace(items[3]))
                {
                    var strArray1 = items;
                    var num2 = 3;
                    string[] strArray2;
                    IntPtr index2;
                    (strArray2 = strArray1)[(int)(index2 = (IntPtr)num2)] = strArray2[(int)index2] + ",";
                }

                var strArray3 = items;
                var num3 = 3;
                string[] strArray4;
                IntPtr index3;
                (strArray4 = strArray3)[(int)(index3 = (IntPtr)num3)] = strArray4[(int)index3] +
                                                                          DatabaseAPI.Database
                                                                              .EnhancementClasses[
                                                                                  enhancement.ClassID[index1]]
                                                                              .ShortName;
            }

            if (enhancement.nIDSet > -1)
            {
                items[4] = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName;
                items[0] = items[4] + ": " + items[0];
            }
            else
            {
                items[4] = "";
            }

            lvEnh.Items.Add(new ListViewItem(items, Index));
            lvEnh.Items[lvEnh.Items.Count - 1].Selected = true;
            lvEnh.Items[lvEnh.Items.Count - 1].EnsureVisible();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            IEnhancement iEnh = new Enhancement();
            using var frmEnhData = new frmEnhData(ref iEnh,
                DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1].StaticIndex + 1);
            var num = (int)frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;
            var enhancementArray = (IEnhancement[])Utils.CopyArray(database.Enhancements,
                new IEnhancement[DatabaseAPI.Database.Enhancements.Length + 1]);
            database.Enhancements = enhancementArray;
            var newEnhancement = new Enhancement(frmEnhData.myEnh) { IsNew = true };
            DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1] = newEnhancement;
            if (newEnhancement.nIDSet > 0)
            {
                var es = DatabaseAPI.Database.EnhancementSets[newEnhancement.nIDSet];
                Array.Resize(ref es.Enhancements, es.Enhancements.Length + 1);
                es.Enhancements[es.Enhancements.Length - 1] = newEnhancement.StaticIndex;
            }

            ImageUpdate();
            AddListItem(DatabaseAPI.Database.Enhancements.Length - 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            var oldEnhancement =
                DatabaseAPI.Database.Enhancements[
                    DatabaseAPI.GetEnhancementByUIDName(lvEnh.SelectedItems[0].SubItems[5].Text)];
            using var frmEnhData = new frmEnhData(ref oldEnhancement,
                DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1].StaticIndex + 1);
            var num = (int)frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;
            var enhancementArray = (IEnhancement[])Utils.CopyArray(database.Enhancements,
                new IEnhancement[DatabaseAPI.Database.Enhancements.Length + 1]);
            database.Enhancements = enhancementArray;
            DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1] =
                new Enhancement(frmEnhData.myEnh) { IsNew = true, StaticIndex = -1 };
            ImageUpdate();
            AddListItem(DatabaseAPI.Database.Enhancements.Length - 1);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0 || MessageBox.Show($"Really delete enhancement: {lvEnh.SelectedItems[0].Text}?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            var enhancementArray = new Enhancement[DatabaseAPI.Database.Enhancements.Length - 1 + 1];
            var selectedIndex = DatabaseAPI.GetEnhancementByUIDName(lvEnh.SelectedItems[0].SubItems[5].Text);
            var index1 = 0;
            var num1 = DatabaseAPI.Database.Enhancements.Length - 1;
            var enh = DatabaseAPI.Database.Enhancements[selectedIndex];
            if (enh.nIDSet > -1)
            {
                //Remove it from the enhancement set too.

                DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements = DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements.Where(staticIndex => staticIndex != enh.StaticIndex).ToArray();
            }

            for (var index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                enhancementArray[index1] = new Enhancement(DatabaseAPI.Database.Enhancements[index2]);
                ++index1;
            }

            DatabaseAPI.Database.Enhancements = new IEnhancement[DatabaseAPI.Database.Enhancements.Length - 2 + 1];
            var num2 = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
                DatabaseAPI.Database.Enhancements[index2] = new Enhancement(enhancementArray[index2]);

            DisplayList();
            if (lvEnh.Items.Count <= 0)
                return;
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
                return;
            var enhIndex = DatabaseAPI.GetEnhancementByUIDName(lvEnh.SelectedItems[0].SubItems[5].Text);
            var selectedIndex = DatabaseAPI.GetEnhancementByUIDName(lvEnh.SelectedItems[0].SubItems[5].Text);
            if (selectedIndex >= lvEnh.Items.Count - 1)
                return;
            IEnhancement[] enhancementArray =
            {
                new Enhancement(DatabaseAPI.Database.Enhancements[enhIndex]),
                new Enhancement(DatabaseAPI.Database.Enhancements[enhIndex + 1])
            };
            DatabaseAPI.Database.Enhancements[enhIndex + 1] = new Enhancement(enhancementArray[0]);
            DatabaseAPI.Database.Enhancements[enhIndex] = new Enhancement(enhancementArray[1]);
            DisplayList();
            lvEnh.Items[selectedIndex + 1].Selected = true;
            lvEnh.Items[selectedIndex + 1].EnsureVisible();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = DatabaseAPI.GetEnhancementByUIDName(lvEnh.SelectedItems[0].SubItems[5].Text);
            using var frmEnhData = new frmEnhData(ref DatabaseAPI.Database.Enhancements[selectedIndex], 0);
            var num = (int)frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
                return;
            var newEnhancement = new Enhancement(frmEnhData.myEnh) { IsModified = true };
            DatabaseAPI.Database.Enhancements[lvEnh.SelectedIndices[0]] = newEnhancement;
            ImageUpdate();
            UpdateListItem(selectedIndex);
        }

        private static void BusyHide()
        {
            using var bFrm = new frmBusy();
            bFrm.Close();
        }

        private void BusyMsg(string sMessage)
        {
            using var bFrm = new frmBusy();
            bFrm.Show(this);
            bFrm.SetMessage(sMessage);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BusyMsg("Saving please wait...");
            I9Gfx.LoadEnhancements();
            foreach (var power in DatabaseAPI.Database.Power) power.BaseRechargeTime = power.RechargeTime;
            Array.Sort(DatabaseAPI.Database.Power);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.AssignRecipeIDs();
            DatabaseAPI.SaveEnhancementDb(serializer);
            DatabaseAPI.MatchAllIDs();
            Task.Delay(1000).Wait();
            DatabaseAPI.SaveMainDatabase(serializer);
            BusyHide();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            var enhIndex = DatabaseAPI.GetEnhancementByUIDName(lvEnh.SelectedItems[0].SubItems[5].Text);
            var selectedIndex = lvEnh.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            IEnhancement[] enhancementArray =
            {
                new Enhancement(DatabaseAPI.Database.Enhancements[enhIndex]),
                new Enhancement(DatabaseAPI.Database.Enhancements[enhIndex - 1])
            };
            DatabaseAPI.Database.Enhancements[enhIndex - 1] = new Enhancement(enhancementArray[0]);
            DatabaseAPI.Database.Enhancements[enhIndex] = new Enhancement(enhancementArray[1]);
            DisplayList();
            lvEnh.Items[selectedIndex - 1].Selected = true;
            lvEnh.Items[selectedIndex - 1].EnsureVisible();
        }

        private void DisplayList()
        {
            ImageUpdate();
            lvEnh.BeginUpdate();
            lvEnh.Items.Clear();
            var num = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var Index = 0; Index <= num; ++Index)
            {
                if (string.IsNullOrEmpty(txtFilter.Text) || DatabaseAPI.Database.Enhancements[Index].Name
                    .ToUpper(CultureInfo.InvariantCulture)
                    .Contains(txtFilter.Text.ToUpper(CultureInfo.InvariantCulture)))
                {
                    AddListItem(Index);
                    continue;
                }

                if (DatabaseAPI.Database.Enhancements[Index].nIDSet > -1)
                    if (DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[Index].nIDSet]
                        .DisplayName.ToUpper(CultureInfo.InvariantCulture)
                        .Contains(txtFilter.Text.ToUpper(CultureInfo.InvariantCulture)))
                        AddListItem(Index);
            }

            if (lvEnh.Items.Count > 0)
            {
                lvEnh.Items[0].Selected = true;
                lvEnh.Items[0].EnsureVisible();
            }

            lvEnh.EndUpdate();
        }

        private void FillImageList()
        {
            var imageSize1 = ilEnh.ImageSize;
            var width1 = imageSize1.Width;
            imageSize1 = ilEnh.ImageSize;
            var height1 = imageSize1.Height;
            using var extendedBitmap = new ExtendedBitmap(width1, height1);
            ilEnh.Images.Clear();
            var num = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var enhancement = DatabaseAPI.Database.Enhancements[index];
                if (enhancement.ImageIdx > -1)
                {
                    extendedBitmap.Graphics.Clear(Color.White);
                    var graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancement(ref graphics, DatabaseAPI.Database.Enhancements[index].ImageIdx,
                        I9Gfx.ToGfxGrade(enhancement.TypeID));
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
            Show();
            Refresh();
            DisplayList();
            lblLoading.Visible = false;
            lvEnh.Select();
        }

        private void ImageUpdate()
        {
            if (NoReload.Checked)
                return;
            I9Gfx.LoadEnhancements();
            FillImageList();
        }

        private void lvEnh_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void lvEnh_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void NoReload_CheckedChanged(object sender, EventArgs e)
        {
            ImageUpdate();
        }

        private void UpdateListItem(int Index)
        {
            var strArray1 = new string[6];
            var enhancement = DatabaseAPI.Database.Enhancements[Index];
            strArray1[0] = enhancement.Name + " (" + enhancement.ShortName + ") - " +
                           Convert.ToString(enhancement.StaticIndex, CultureInfo.InvariantCulture);
            strArray1[1] = Enum.GetName(enhancement.TypeID.GetType(), enhancement.TypeID);
            strArray1[2] = Convert.ToString(enhancement.Effect.Length, CultureInfo.InvariantCulture);
            strArray1[3] = "";
            strArray1[5] = enhancement.UID;
            var num1 = enhancement.ClassID.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                if (!string.IsNullOrWhiteSpace(strArray1[3]))
                {
                    var strArray2 = strArray1;
                    var num2 = 3;
                    string[] strArray3;
                    IntPtr index2;
                    (strArray3 = strArray2)[(int)(index2 = (IntPtr)num2)] = strArray3[(int)index2] + ",";
                }

                var strArray4 = strArray1;
                var num3 = 3;
                string[] strArray5;
                IntPtr index3;
                (strArray5 = strArray4)[(int)(index3 = (IntPtr)num3)] = strArray5[(int)index3] +
                                                                          DatabaseAPI.Database
                                                                              .EnhancementClasses[
                                                                                  enhancement.ClassID[index1]]
                                                                              .ShortName;
            }

            if (enhancement.nIDSet > -1)
            {
                strArray1[4] = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName;
                strArray1[0] = strArray1[4] + ": " + strArray1[0];
            }
            else
            {
                strArray1[4] = "";
            }

            var num4 = strArray1.Length - 1;
            for (var index = 0; index <= num4; ++index)
                lvEnh.Items[Index].SubItems[index].Text = strArray1[index];
            lvEnh.Items[Index].ImageIndex = Index;
            lvEnh.Items[Index].EnsureVisible();
            lvEnh.Refresh();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            DisplayList();
            btnUp.Enabled = string.IsNullOrEmpty(txtFilter.Text);
            btnDown.Enabled = string.IsNullOrEmpty(txtFilter.Text);
        }
    }
}