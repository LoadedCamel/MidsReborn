using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmSetEditNEW : Form
    {
        private bool Loading;
        public readonly EnhancementSet mySet;
        private int[] SetBonusList;

        public frmSetEditNEW(ref EnhancementSet iSet)
        {
            Load += frmSetEdit_Load;
            SetBonusList = new int[0];
            Loading = true;
            InitializeComponent();
            Name = nameof(frmSetEditNEW);
            var componentResourceManager = new ComponentResourceManager(typeof(frmSetEditNEW));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            btnImage.Image = (Image) componentResourceManager.GetObject("btnImage.Image");
            mySet = new EnhancementSet(iSet);
        }

        private int BonusID()
        {
            return cbSlotCount.SelectedIndex;
        }

        void btnImage_Click(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetEnhancementsPath();
            ImagePicker.FileName = mySet.Image;
            if (ImagePicker.ShowDialog() == DialogResult.OK)
            {
                string str = FileIO.StripPath(ImagePicker.FileName);
                if (!File.Exists(FileIO.AddSlash(ImagePicker.InitialDirectory) + str))
                {
                    MessageBox.Show(
                        $"You must select an image from the {I9Gfx.GetEnhancementsPath()} folder!\r\n\r\nIf you are adding a new image, you should copy it to the folder and then select it.",
                        @"Select Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    mySet.Image = str;
                    DisplayIcon();
                }
            }
        }

        void btnNoImage_Click(object sender, EventArgs e)
        {
            mySet.Image = "";
            DisplayIcon();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            mySet.LevelMin = Convert.ToInt32(decimal.Subtract(udMinLevel.Value, new decimal(1)));
            mySet.LevelMax = Convert.ToInt32(decimal.Subtract(udMaxLevel.Value, new decimal(1)));
            DialogResult = DialogResult.OK;
            Hide();
        }

        void cbSetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.SetType = (Enums.eSetType) cbSetType.SelectedIndex;
        }

        void cbSlotX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            DisplayBonus();
            DisplayBonusText();
        }

        void udMaxLevel_Leave(object sender, EventArgs e)
        {
            SetMaxLevel((int)Math.Round(Conversion.Val(udMaxLevel.Text)));
            mySet.LevelMax = Convert.ToInt32(decimal.Subtract(udMaxLevel.Value, new decimal(1)));
        }

        void udMaxLevel_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.LevelMax = Convert.ToInt32(decimal.Subtract(udMaxLevel.Value, new decimal(1)));
            udMinLevel.Maximum = udMaxLevel.Value;
        }

        void udMinLevel_Leave(object sender, EventArgs e)
        {
            SetMinLevel((int) Math.Round(Conversion.Val(udMinLevel.Text)));
            mySet.LevelMin = Convert.ToInt32(decimal.Subtract(udMinLevel.Value, new decimal(1)));
        }

        void udMinLevel_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.LevelMin = Convert.ToInt32(Decimal.Subtract(udMinLevel.Value, new Decimal(1)));
            udMaxLevel.Minimum = udMinLevel.Value;
        }

        void txtAlternate_TextChanged(object sender, EventArgs e)
        {
            if (isBonus())
                mySet.Bonus[BonusID()].AltString = txtAlternate.Text;
            else if (isSpecial())
                mySet.SpecialBonus[SpecialID()].AltString = txtAlternate.Text;
            DisplayBonusText();
        }

        void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.Desc = txtDesc.Text;
        }

        void txtInternal_TextChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            mySet.Uid = txtInternal.Text;
        }

        void txtNameFull_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.DisplayName = txtNameFull.Text;
        }

        void txtNameShort_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.ShortName = txtNameShort.Text;
        }

        void lstBonus_DoubleClick(object sender, EventArgs e)
        {
            if (lstBonus.SelectedIndex < 0)
                return;
            int selectedIndex = lstBonus.SelectedIndex;
            int[] numArray1 = Array.Empty<int>();
            string[] strArray1 = Array.Empty<string>();
            int index1 = 0;
            if (isBonus())
            {
                int[] numArray2 = new int[mySet.Bonus[BonusID()].Index.Length - 2 + 1];
                string[] strArray2 = new string[mySet.Bonus[BonusID()].Name.Length - 2 + 1];
                int num1 = mySet.Bonus[BonusID()].Index.Length - 1;
                for (int index2 = 0; index2 <= num1; ++index2)
                {
                    if (index2 != selectedIndex)
                    {
                        numArray2[index1] = mySet.Bonus[BonusID()].Index[index2];
                        strArray2[index1] = mySet.Bonus[BonusID()].Name[index2];
                        ++index1;
                    }
                }
                mySet.Bonus[BonusID()].Name = new string[numArray2.Length - 1 + 1];
                mySet.Bonus[BonusID()].Index = new int[strArray2.Length - 1 + 1];
                int num2 = numArray2.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    mySet.Bonus[BonusID()].Index[index2] = numArray2[index2];
                    mySet.Bonus[BonusID()].Name[index2] = strArray2[index2];
                }
            }
            else if (isSpecial())
            {
                int[] numArray2 = new int[mySet.SpecialBonus[SpecialID()].Index.Length - 2 + 1];
                string[] strArray2 = new string[mySet.SpecialBonus[SpecialID()].Name.Length - 2 + 1];
                int num1 = mySet.SpecialBonus[SpecialID()].Index.Length - 1;
                for (int index2 = 0; index2 <= num1; ++index2)
                {
                    if (index2 != selectedIndex)
                    {
                        numArray2[index1] = mySet.SpecialBonus[SpecialID()].Index[index2];
                        strArray2[index1] = mySet.SpecialBonus[SpecialID()].Name[index2];
                        ++index1;
                    }
                }
                mySet.SpecialBonus[SpecialID()].Name = new string[numArray2.Length - 1 + 1];
                mySet.SpecialBonus[SpecialID()].Index = new int[strArray2.Length - 1 + 1];
                int num2 = numArray2.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    mySet.SpecialBonus[SpecialID()].Index[index2] = numArray2[index2];
                    mySet.SpecialBonus[SpecialID()].Name[index2] = strArray2[index2];
                }
                if (mySet.SpecialBonus[SpecialID()].Index.Length < 1)
                    mySet.SpecialBonus[SpecialID()].Special = -1;
            }
            DisplayBonus();
            DisplayBonusText();
        }

        void lvBonusList_DoubleClick(object sender, EventArgs e)
        {
            if (lvBonusList.SelectedIndices.Count < 1)
                return;
            int index = (int)Math.Round(Conversion.Val(RuntimeHelpers.GetObjectValue(lvBonusList.SelectedItems[0].Tag)));
            if (index < 0)
            {
                MessageBox.Show(@"Tag was less than 0!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (isBonus())
                {
                    mySet.Bonus[BonusID()].Name = (string[])Utils.CopyArray(mySet.Bonus[BonusID()].Name, (Array)new string[mySet.Bonus[BonusID()].Name.Length + 1]);
                    mySet.Bonus[BonusID()].Index = (int[])Utils.CopyArray(mySet.Bonus[BonusID()].Index, (Array)new int[mySet.Bonus[BonusID()].Index.Length + 1]);
                    mySet.Bonus[BonusID()].Name[mySet.Bonus[BonusID()].Name.Length - 1] = DatabaseAPI.Database.Power[index].FullName;
                    mySet.Bonus[BonusID()].Index[mySet.Bonus[BonusID()].Index.Length - 1] = index;
                    mySet.Bonus[BonusID()].Slotted = cbSlotCount.SelectedIndex + 2;
                }
                else if (isSpecial())
                {
                    mySet.SpecialBonus[SpecialID()].Special = SpecialID();
                    mySet.SpecialBonus[SpecialID()].Name = (string[])Utils.CopyArray(mySet.SpecialBonus[SpecialID()].Name, (Array)new string[mySet.SpecialBonus[SpecialID()].Name.Length + 1]);
                    mySet.SpecialBonus[SpecialID()].Index = (int[])Utils.CopyArray(mySet.SpecialBonus[SpecialID()].Index, (Array)new int[mySet.SpecialBonus[SpecialID()].Index.Length + 1]);
                    mySet.SpecialBonus[SpecialID()].Name[mySet.SpecialBonus[SpecialID()].Name.Length - 1] = DatabaseAPI.Database.Power[index].FullName;
                    mySet.SpecialBonus[SpecialID()].Index[mySet.SpecialBonus[SpecialID()].Index.Length - 1] = index;
                }
                DisplayBonus();
                DisplayBonusText();
            }
        }

        private void SetMaxLevel(int iValue)
        {
            if (decimal.Compare(new decimal(iValue), udMaxLevel.Minimum) < 0)
                iValue = Convert.ToInt32(udMaxLevel.Minimum);
            if (decimal.Compare(new decimal(iValue), udMaxLevel.Maximum) > 0)
                iValue = Convert.ToInt32(udMaxLevel.Maximum);
            udMaxLevel.Value = new decimal(iValue);
        }

        private void SetMinLevel(int iValue)
        {
            if (decimal.Compare(new decimal(iValue), udMinLevel.Minimum) < 0)
                iValue = Convert.ToInt32(udMinLevel.Minimum);
            if (decimal.Compare(new decimal(iValue), udMinLevel.Maximum) > 0)
                iValue = Convert.ToInt32(udMinLevel.Maximum);
            udMinLevel.Value = new decimal(iValue);
        }

        private int SpecialID()
        {
            return cbSlotCount.SelectedIndex - (mySet.Enhancements.Length - 1);
        }

        private void DisplayBonus()
        {
            try
            {
                lstBonus.BeginUpdate();
                lstBonus.Items.Clear();
                if (isBonus())
                {
                    int index1 = BonusID();
                    int num = mySet.Bonus[index1].Index.Length - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
                        lstBonus.Items.Add(DatabaseAPI.Database.Power[mySet.Bonus[index1].Index[index2]].PowerName);
                    txtAlternate.Text = mySet.Bonus[index1].AltString;
                }
                else if (isSpecial())
                {
                    int index1 = SpecialID();
                    int num = mySet.SpecialBonus[index1].Index.Length - 1;
                    for (int index2 = 0; index2 <= num; ++index2)
                        lstBonus.Items.Add(DatabaseAPI.Database.Power[mySet.SpecialBonus[index1].Index[index2]]
                            .PowerName);
                    txtAlternate.Text = mySet.SpecialBonus[index1].AltString;
                }

                lstBonus.EndUpdate();
                cbSlotCount.Enabled = mySet.Enhancements.Length > 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n\n{ex.StackTrace}");
                //ProjectData.SetProjectError(ex);
                //ProjectData.ClearProjectError();
            }
        }

        private void DisplayBonusText()
        {
            string str1 = RTF.StartRTF();
            int num1 = mySet.Bonus.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                if (mySet.Bonus[index1].Index.Length > 0)
                    str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                           RTF.Bold($"{mySet.Bonus[index1].Slotted} Enhancements: ");
                int num2 = mySet.Bonus[index1].Index.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (mySet.Bonus[index1].Index[index2] > -1)
                    {
                        if (index2 > 0)
                            str1 += ", ";
                        str1 = str1 + RTF.Color(RTF.ElementID.InentionInvert) +
                               DatabaseAPI.Database.Power[mySet.Bonus[index1].Index[index2]].PowerName;
                    }
                }

                if (mySet.Bonus[index1].Index.Length > 0)
                    str1 = str1 + RTF.Crlf() + "   " + RTF.Italic(mySet.GetEffectString(index1, false));
                if (mySet.Bonus[index1].PvMode == Enums.ePvX.PvP)
                    str1 += "(PvP)";
                if (mySet.Bonus[index1].Index.Length > 0)
                    str1 += RTF.Crlf();
            }

            int num3 = mySet.SpecialBonus.Length - 1;
            for (int index1 = 0; index1 <= num3; ++index1)
            {
                if (mySet.SpecialBonus[index1].Special > -1)
                {
                    string str2 = str1 + RTF.Color(RTF.ElementID.Black) + RTF.Bold("Special Case Enhancement: ") +
                                  RTF.Color(RTF.ElementID.InentionInvert);
                    if (mySet.Enhancements[mySet.SpecialBonus[index1].Special] > -1)
                        str2 += DatabaseAPI.Database
                            .Enhancements[mySet.Enhancements[mySet.SpecialBonus[index1].Special]].Name;
                    string str3 = str2 + RTF.Crlf();
                    int num2 = mySet.SpecialBonus[index1].Index.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (mySet.SpecialBonus[index1].Index[index2] > -1)
                        {
                            if (index2 > 0)
                                str3 += ", ";
                            str3 = str3 + RTF.Color(RTF.ElementID.InentionInvert) + DatabaseAPI.Database
                                       .Power[mySet.SpecialBonus[index1].Index[index2]].PowerName;
                        }
                    }

                    str1 = str3 + RTF.Crlf() + "   " + RTF.Italic(mySet.GetEffectString(index1, true)) +
                           RTF.Crlf();
                }

                if (mySet.SpecialBonus[index1].Index.Length > 0)
                    str1 += RTF.Crlf();
            }

            rtbBonus.Rtf = str1 + RTF.EndRTF();
        }

        void DisplayIcon()
        {
            if (!string.IsNullOrWhiteSpace(mySet.Image))
            {
                using ExtendedBitmap extendedBitmap1 = new ExtendedBitmap($"{I9Gfx.GetEnhancementsPath()}{mySet.Image}");
                using ExtendedBitmap extendedBitmap2 = new ExtendedBitmap(30, 30);
                extendedBitmap2.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), System.Drawing.GraphicsUnit.Pixel);
                extendedBitmap2.Graphics.DrawImage(extendedBitmap1.Bitmap, extendedBitmap2.ClipRect, extendedBitmap2.ClipRect, System.Drawing.GraphicsUnit.Pixel);
                btnImage.Image = new Bitmap(extendedBitmap2.Bitmap);
                btnImage.Text = mySet.Image;
            }
            else
            {
                using ExtendedBitmap extendedBitmap = new ExtendedBitmap(30, 30);
                extendedBitmap.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), System.Drawing.GraphicsUnit.Pixel);
                btnImage.Image = new Bitmap(extendedBitmap.Bitmap);
                btnImage.Text = @"Select Image";
            }
        }

        private void DisplaySetData()
        {
            DisplaySetIcons();
            DisplayIcon();
            txtNameFull.Text = mySet.DisplayName;
            txtNameShort.Text = mySet.ShortName;
            txtDesc.Text = mySet.Desc;
            txtInternal.Text = mySet.Uid;
            SetMinLevel(mySet.LevelMin + 1);
            SetMaxLevel(mySet.LevelMax + 1);
            udMaxLevel.Minimum = udMinLevel.Value;
            udMinLevel.Maximum = udMaxLevel.Value;
            cbSetType.SelectedIndex = (int)mySet.SetType;
            btnImage.Text = mySet.Image;
            DisplayBonusText();
            DisplayBonus();
        }

        private void DisplaySetIcons()
        {
            FillImageList();
            string[] items = new string[2];
            lvEnh.BeginUpdate();
            lvEnh.Items.Clear();
            FillImageList();
            int num1 = mySet.Enhancements.Length - 1;
            for (int imageIndex = 0; imageIndex <= num1; ++imageIndex)
            {
                IEnhancement enhancement = DatabaseAPI.Database.Enhancements[mySet.Enhancements[imageIndex]];
                items[0] = enhancement.Name + " (" + enhancement.ShortName + ")";
                items[1] = "";
                int num2 = enhancement.ClassID.Length - 1;
                for (int index1 = 0; index1 <= num2; ++index1)
                {
                    if (!string.IsNullOrWhiteSpace(items[1]))
                    {
                        string[] strArray1 = items;
                        int num3 = 1;
                        string[] strArray2;
                        IntPtr index2;
                        (strArray2 = strArray1)[(int)(index2 = (IntPtr)num3)] = strArray2[(int)index2] + ",";
                    }
                    string[] strArray3 = items;
                    int num4 = 1;
                    string[] strArray4;
                    IntPtr index3;
                    (strArray4 = strArray3)[(int)(index3 = (IntPtr)num4)] = strArray4[(int)index3] + DatabaseAPI.Database.EnhancementClasses[enhancement.ClassID[index1]].ShortName;
                }
                lvEnh.Items.Add(new ListViewItem(items, imageIndex));
            }
            lvEnh.EndUpdate();
        }

        private void FillBonusCombos()
        {
            cbSlotCount.BeginUpdate();
            cbSlotCount.Items.Clear();
            int num1 = mySet.Enhancements.Length - 2;
            for (int index = 0; index <= num1; ++index)
                cbSlotCount.Items.Add($"{index + 2} Enhancements");
            int num2 = mySet.Enhancements.Length - 1;
            for (int index = 0; index <= num2; ++index)
                cbSlotCount.Items.Add(DatabaseAPI.Database.Enhancements[mySet.Enhancements[index]].Name);
            if (cbSlotCount.Items.Count > 0)
                cbSlotCount.SelectedIndex = 0;
            cbSlotCount.EndUpdate();
        }

        private void FillBonusList()
        {
            lvBonusList.BeginUpdate();
            lvBonusList.Items.Clear();
            string[] items = new string[2];
            int num1 = SetBonusList.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                items[1] = "";
                if (DatabaseAPI.Database.Power[SetBonusList[index]].Effects.Length > 0)
                    items[1] = DatabaseAPI.Database.Power[SetBonusList[index]].Effects[0].BuildEffectStringShort(false, true);
                items[0] = DatabaseAPI.Database.Power[SetBonusList[index]].PowerName;
                if (items[0].ToUpper(CultureInfo.InvariantCulture).Contains(txtBonusFilter.Text.ToUpper(CultureInfo.InvariantCulture)))
                {
                    lvBonusList.Items.Add(new ListViewItem(items)
                    {
                        Tag = SetBonusList[index]
                    });
                }
            }
            lvBonusList.Sort();
            lvBonusList.EndUpdate();
        }

        private void FillComboBoxes()
        {
            string[] names = Enum.GetNames(Enums.eSetType.Untyped.GetType());
            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange(names.ToArray<object>());
            cbSetType.EndUpdate();
        }

        private void FillImageList()
        {
            Size imageSize1 = ilEnh.ImageSize;
            int width1 = imageSize1.Width;
            imageSize1 = ilEnh.ImageSize;
            int height1 = imageSize1.Height;
            using ExtendedBitmap extendedBitmap = new ExtendedBitmap(width1, height1);
            ilEnh.Images.Clear();
            int num = mySet.Enhancements.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                IEnhancement enhancement = DatabaseAPI.Database.Enhancements[mySet.Enhancements[index]];
                if (enhancement.ImageIdx > -1)
                {
                    Origin.Grade gfxGrade = I9Gfx.ToGfxGrade(enhancement.TypeID);
                    extendedBitmap.Graphics.Clear(Color.White);
                    Graphics graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancement(ref graphics, DatabaseAPI.Database.Enhancements[mySet.Enhancements[index]].ImageIdx, gfxGrade);
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

        void frmSetEdit_Load(object sender, EventArgs e)
        {
            SetBonusList = DatabaseAPI.NidPowers("set_bonus.set_bonus");
            if (mySet.Bonus.Length < 1)
                mySet.InitBonus();
            FillComboBoxes();
            FillBonusCombos();
            FillBonusList();
            DisplaySetData();
            Loading = false;
            DisplayBonus();
        }

        private bool isBonus()
        {
            return cbSlotCount.SelectedIndex > -1 & cbSlotCount.SelectedIndex < mySet.Enhancements.Length - 1;
        }

        private bool isSpecial()
        {
            return cbSlotCount.SelectedIndex >= mySet.Enhancements.Length - 1 & cbSlotCount.SelectedIndex < mySet.Enhancements.Length + mySet.Enhancements.Length - 1;
        }

        private void TxtBonusFilter_TextChanged(object sender, EventArgs e)
        {
            FillBonusList();
        }
    }
}
