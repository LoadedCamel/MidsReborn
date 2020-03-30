
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Display;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmEnhEdit : Form
    {
        public frmEnhEdit()
        {
            Load += frmEnhEdit_Load;
            InitializeComponent();
            Name = nameof(frmEnhEdit);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmEnhEdit));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        }

        void AddListItem(int Index)
        {
            string[] items = new string[5];
            IEnhancement enhancement = DatabaseAPI.Database.Enhancements[Index];
            items[0] = enhancement.Name + " (" + enhancement.ShortName + ") - " + Convert.ToString(enhancement.StaticIndex);
            items[1] = Enum.GetName(enhancement.TypeID.GetType(), enhancement.TypeID);
            items[2] = Convert.ToString(enhancement.Effect.Length);
            items[3] = "";
            int num1 = enhancement.ClassID.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                if (items[3] != "")
                {
                    string[] strArray1 = items;
                    int num2 = 3;
                    string[] strArray2;
                    IntPtr index2;
                    (strArray2 = strArray1)[(int)(index2 = (IntPtr)num2)] = strArray2[(int)index2] + ",";
                }
                string[] strArray3 = items;
                int num3 = 3;
                string[] strArray4;
                IntPtr index3;
                (strArray4 = strArray3)[(int)(index3 = (IntPtr)num3)] = strArray4[(int)index3] + DatabaseAPI.Database.EnhancementClasses[enhancement.ClassID[index1]].ShortName;
            }
            if (enhancement.nIDSet > -1)
            {
                items[4] = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName;
                items[0] = items[4] + ": " + items[0];
            }
            else
                items[4] = "";
            lvEnh.Items.Add(new ListViewItem(items, Index));
            lvEnh.Items[lvEnh.Items.Count - 1].Selected = true;
            lvEnh.Items[lvEnh.Items.Count - 1].EnsureVisible();
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            IEnhancement iEnh = new Enhancement();
            frmEnhData frmEnhData = new frmEnhData(ref iEnh, DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1].StaticIndex + 1);
            int num = (int)frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            IEnhancement[] enhancementArray = (IEnhancement[])Utils.CopyArray(database.Enhancements, new IEnhancement[DatabaseAPI.Database.Enhancements.Length + 1]);
            database.Enhancements = enhancementArray;
            Enhancement newEnhancement = new Enhancement(frmEnhData.myEnh) { IsNew = true };
            DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1] = newEnhancement;
            if (newEnhancement.nIDSet > 0)
            {
                EnhancementSet es = DatabaseAPI.Database.EnhancementSets[newEnhancement.nIDSet];
                Array.Resize(ref es.Enhancements, es.Enhancements.Length + 1);
                es.Enhancements[es.Enhancements.Length - 1] = newEnhancement.StaticIndex;
            }
            ImageUpdate();
            AddListItem(DatabaseAPI.Database.Enhancements.Length - 1);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        void btnClone_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            IEnhancement oldEnhancement = DatabaseAPI.Database.Enhancements[lvEnh.SelectedIndices[0]];
            frmEnhData frmEnhData = new frmEnhData(ref oldEnhancement, DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1].StaticIndex + 1);
            int num = (int)frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            IEnhancement[] enhancementArray = (IEnhancement[])Utils.CopyArray(database.Enhancements, new IEnhancement[DatabaseAPI.Database.Enhancements.Length + 1]);
            database.Enhancements = enhancementArray;
            DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Enhancements.Length - 1] =
                new Enhancement(frmEnhData.myEnh) {IsNew = true, StaticIndex = -1};
            ImageUpdate();
            AddListItem(DatabaseAPI.Database.Enhancements.Length - 1);
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0 || Interaction.MsgBox(("Really delete enhancement: " + lvEnh.SelectedItems[0].Text + "?"), MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            Enhancement[] enhancementArray = new Enhancement[DatabaseAPI.Database.Enhancements.Length - 1 + 1];
            int selectedIndex = lvEnh.SelectedIndices[0];
            int index1 = 0;
            int num1 = DatabaseAPI.Database.Enhancements.Length - 1;
            IEnhancement enh = DatabaseAPI.Database.Enhancements[selectedIndex];
            if (enh.nIDSet > -1)
            {
                //Remove it from the enhancement set too.
                List<int> newEnhancementSet = new List<int>();
                for (int i = 0; i < DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements.Length; i++)
                {
                    int staticIndex = DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements[i];
                    if (staticIndex != enh.StaticIndex)
                        newEnhancementSet.Add(staticIndex);
                }
                DatabaseAPI.Database.EnhancementSets[enh.nIDSet].Enhancements = newEnhancementSet.ToArray();
            }
            for (int index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                enhancementArray[index1] = new Enhancement(DatabaseAPI.Database.Enhancements[index2]);
                ++index1;
            }
            DatabaseAPI.Database.Enhancements = new IEnhancement[DatabaseAPI.Database.Enhancements.Length - 2 + 1];
            int num2 = DatabaseAPI.Database.Enhancements.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
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

        void btnDown_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvEnh.SelectedIndices[0];
            if (selectedIndex >= lvEnh.Items.Count - 1)
                return;
            IEnhancement[] enhancementArray = {
                new Enhancement(DatabaseAPI.Database.Enhancements[selectedIndex]),
                new Enhancement(DatabaseAPI.Database.Enhancements[selectedIndex + 1])
            };
            DatabaseAPI.Database.Enhancements[selectedIndex + 1] = new Enhancement(enhancementArray[0]);
            DatabaseAPI.Database.Enhancements[selectedIndex] = new Enhancement(enhancementArray[1]);
            DisplayList();
            lvEnh.Items[selectedIndex + 1].Selected = true;
            lvEnh.Items[selectedIndex + 1].EnsureVisible();
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvEnh.SelectedIndices[0];
            frmEnhData frmEnhData = new frmEnhData(ref DatabaseAPI.Database.Enhancements[lvEnh.SelectedIndices[0]], 0);
            int num = (int)frmEnhData.ShowDialog();
            if (frmEnhData.DialogResult != DialogResult.OK)
                return;
            Enhancement newEnhancement = new Enhancement(frmEnhData.myEnh) { IsModified = true };
            DatabaseAPI.Database.Enhancements[lvEnh.SelectedIndices[0]] = newEnhancement;
            ImageUpdate();
            UpdateListItem(selectedIndex);
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            I9Gfx.LoadEnhancements();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, false);
            DatabaseAPI.AssignRecipeIDs();
            DatabaseAPI.SaveEnhancementDb(serializer);
            Hide();
        }

        void btnUp_Click(object sender, EventArgs e)
        {
            if (lvEnh.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvEnh.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            IEnhancement[] enhancementArray = {
                new Enhancement(DatabaseAPI.Database.Enhancements[selectedIndex]),
                new Enhancement(DatabaseAPI.Database.Enhancements[selectedIndex - 1])
            };
            DatabaseAPI.Database.Enhancements[selectedIndex - 1] = new Enhancement(enhancementArray[0]);
            DatabaseAPI.Database.Enhancements[selectedIndex] = new Enhancement(enhancementArray[1]);
            DisplayList();
            lvEnh.Items[selectedIndex - 1].Selected = true;
            lvEnh.Items[selectedIndex - 1].EnsureVisible();
        }

        void DisplayList()
        {
            ImageUpdate();
            lvEnh.BeginUpdate();
            lvEnh.Items.Clear();
            int num = DatabaseAPI.Database.Enhancements.Length - 1;
            for (int Index = 0; Index <= num; ++Index)
                AddListItem(Index);
            if (lvEnh.Items.Count > 0)
            {
                lvEnh.Items[0].Selected = true;
                lvEnh.Items[0].EnsureVisible();
            }
            lvEnh.EndUpdate();
        }

        public void FillImageList()
        {
            Size imageSize1 = ilEnh.ImageSize;
            int width1 = imageSize1.Width;
            imageSize1 = ilEnh.ImageSize;
            int height1 = imageSize1.Height;
            ExtendedBitmap extendedBitmap = new ExtendedBitmap(width1, height1);
            ilEnh.Images.Clear();
            int num = DatabaseAPI.Database.Enhancements.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                IEnhancement enhancement = DatabaseAPI.Database.Enhancements[index];
                if (enhancement.ImageIdx > -1)
                {
                    extendedBitmap.Graphics.Clear(Color.White);
                    Graphics graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancement(ref graphics, DatabaseAPI.Database.Enhancements[index].ImageIdx, I9Gfx.ToGfxGrade(enhancement.TypeID));
                    ilEnh.Images.Add(extendedBitmap.Bitmap);
                }
                else
                {
                    ImageList.ImageCollection images = ilEnh.Images;
                    Size imageSize2 = ilEnh.ImageSize;
                    int width2 = imageSize2.Width;
                    imageSize2 = ilEnh.ImageSize;
                    int height2 = imageSize2.Height;
                    Bitmap bitmap = new Bitmap(width2, height2);
                    images.Add(bitmap);
                }
            }
        }

        void frmEnhEdit_Load(object sender, EventArgs e)
        {
            Show();
            Refresh();
            DisplayList();
            lblLoading.Visible = false;
            lvEnh.Select();
        }

        public void ImageUpdate()
        {
            if (NoReload.Checked)
                return;
            I9Gfx.LoadEnhancements();
            FillImageList();
        }

        void lvEnh_DoubleClick(object sender, EventArgs e)
        {
            btnEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        void lvEnh_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        void NoReload_CheckedChanged(object sender, EventArgs e)
        {
            ImageUpdate();
        }

        void UpdateListItem(int Index)
        {
            string[] strArray1 = new string[5];
            IEnhancement enhancement = DatabaseAPI.Database.Enhancements[Index];
            strArray1[0] = enhancement.Name + " (" + enhancement.ShortName + ") - " + Convert.ToString(enhancement.StaticIndex);
            strArray1[1] = Enum.GetName(enhancement.TypeID.GetType(), enhancement.TypeID);
            strArray1[2] = Convert.ToString(enhancement.Effect.Length);
            strArray1[3] = "";
            int num1 = enhancement.ClassID.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                if (strArray1[3] != "")
                {
                    string[] strArray2 = strArray1;
                    int num2 = 3;
                    string[] strArray3;
                    IntPtr index2;
                    (strArray3 = strArray2)[(int)(index2 = (IntPtr)num2)] = strArray3[(int)index2] + ",";
                }
                string[] strArray4 = strArray1;
                int num3 = 3;
                string[] strArray5;
                IntPtr index3;
                (strArray5 = strArray4)[(int)(index3 = (IntPtr)num3)] = strArray5[(int)index3] + DatabaseAPI.Database.EnhancementClasses[enhancement.ClassID[index1]].ShortName;
            }
            if (enhancement.nIDSet > -1)
            {
                strArray1[4] = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName;
                strArray1[0] = strArray1[4] + ": " + strArray1[0];
            }
            else
                strArray1[4] = "";
            int num4 = strArray1.Length - 1;
            for (int index = 0; index <= num4; ++index)
                lvEnh.Items[Index].SubItems[index].Text = strArray1[index];
            lvEnh.Items[Index].ImageIndex = Index;
            lvEnh.Items[Index].EnsureVisible();
            lvEnh.Refresh();
        }
    }
}