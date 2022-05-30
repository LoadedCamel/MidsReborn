using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class FrmSetEditPvP : Form
    {
        public readonly EnhancementSet MySet;
        private bool _loading;
        private int[] _setBonusList;
        private int[] _setBonusListPvp;

        public FrmSetEditPvP(ref EnhancementSet iSet)
        {
            Load += frmSetEdit_Load;
            _setBonusList = Array.Empty<int>();
            _loading = true;
            InitializeComponent();
            Name = nameof(FrmSetEditPvP);
            var componentResourceManager = new ComponentResourceManager(typeof(FrmSetEditPvP));
            Icon = Resources.reborn;
            btnImage.Image = Resources.enhData;
            MySet = new EnhancementSet(iSet);
        }

        private int BonusId()
        {
            return cbSlotCount.SelectedIndex;
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (_loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetDbEnhancementsPath();
            ImagePicker.FileName = MySet.Image;
            if (ImagePicker.ShowDialog(this) != DialogResult.OK) return;

            var imageFile = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(Path.Combine(I9Gfx.GetDbEnhancementsPath(), imageFile)) && !File.Exists(Path.Combine(I9Gfx.GetEnhancementsPath(), imageFile)))
            {
                MessageBox.Show($@"You must select an image from either the {I9Gfx.GetEnhancementsPath()} or the {I9Gfx.GetDbEnhancementsPath()} folder!\r\n\r\nIf you are adding a new image, you should copy it to the appropriate folder and then select it.", @"Select Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MySet.Image = imageFile;
                DisplayIcon();
            }
        }

        private void btnNoImage_Click(object sender, EventArgs e)
        {
            MySet.Image = "";
            DisplayIcon();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MySet.LevelMin = Convert.ToInt32(decimal.Subtract(udMinLevel.Value, new decimal(1)));
            MySet.LevelMax = Convert.ToInt32(decimal.Subtract(udMaxLevel.Value, new decimal(1)));
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void cbSetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MySet.SetType = cbSetType.SelectedIndex;
        }

        private void cbSlotX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            DisplayBonus();
            DisplayBonusText();
        }

        private void udMaxLevel_Leave(object sender, EventArgs e)
        {
            SetMaxLevel((int) Math.Round(Convert.ToDecimal(udMaxLevel.Text)));
            MySet.LevelMax = Convert.ToInt32(decimal.Subtract(udMaxLevel.Value, new decimal(1)));
        }

        private void udMaxLevel_ValueChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MySet.LevelMax = Convert.ToInt32(decimal.Subtract(udMaxLevel.Value, new decimal(1)));
            udMinLevel.Maximum = udMaxLevel.Value;
        }

        private void udMinLevel_Leave(object sender, EventArgs e)
        {
            SetMinLevel((int) Math.Round(Convert.ToDecimal(udMinLevel.Text)));
            MySet.LevelMin = Convert.ToInt32(decimal.Subtract(udMinLevel.Value, new decimal(1)));
        }

        private void udMinLevel_ValueChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MySet.LevelMin = Convert.ToInt32(decimal.Subtract(udMinLevel.Value, new decimal(1)));
            udMaxLevel.Minimum = udMinLevel.Value;
        }

        private void txtAlternate_TextChanged(object sender, EventArgs e)
        {
            if (IsBonus())
                MySet.Bonus[BonusId()].AltString = txtAlternate.Text;
            else if (IsSpecial())
                MySet.SpecialBonus[SpecialId()].AltString = txtAlternate.Text;
            DisplayBonusText();
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MySet.Desc = txtDesc.Text;
        }

        private void txtInternal_TextChanged(object sender, EventArgs e)

        {
            if (_loading)
                return;
            MySet.Uid = txtInternal.Text;
        }

        private void txtNameFull_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MySet.DisplayName = txtNameFull.Text;
        }

        private void txtNameShort_TextChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            MySet.ShortName = txtNameShort.Text;
        }

        private void lstBonus_DoubleClick(object sender, EventArgs e)
        {
            if (lstBonus.SelectedIndex < 0)
                return;
            var selectedIndex = lstBonus.SelectedIndex;
            var index1 = 0;
            if (IsBonus())
            {
                var numArray2 = new int[MySet.Bonus[BonusId()].Index.Length - 2 + 1];
                var strArray2 = new string[MySet.Bonus[BonusId()].Name.Length - 2 + 1];
                var num1 = MySet.Bonus[BonusId()].Index.Length - 1;
                for (var index2 = 0; index2 <= num1; ++index2)
                {
                    if (index2 == selectedIndex)
                        continue;
                    numArray2[index1] = MySet.Bonus[BonusId()].Index[index2];
                    strArray2[index1] = MySet.Bonus[BonusId()].Name[index2];
                    ++index1;
                }

                MySet.Bonus[BonusId()].Name = new string[numArray2.Length - 1 + 1];
                MySet.Bonus[BonusId()].Index = new int[strArray2.Length - 1 + 1];
                var num2 = numArray2.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    MySet.Bonus[BonusId()].Index[index2] = numArray2[index2];
                    MySet.Bonus[BonusId()].Name[index2] = strArray2[index2];
                }
            }
            else if (IsSpecial())
            {
                var numArray2 = new int[MySet.SpecialBonus[SpecialId()].Index.Length - 2 + 1];
                var strArray2 = new string[MySet.SpecialBonus[SpecialId()].Name.Length - 2 + 1];
                var num1 = MySet.SpecialBonus[SpecialId()].Index.Length - 1;
                for (var index2 = 0; index2 <= num1; ++index2)
                {
                    if (index2 == selectedIndex)
                        continue;
                    numArray2[index1] = MySet.SpecialBonus[SpecialId()].Index[index2];
                    strArray2[index1] = MySet.SpecialBonus[SpecialId()].Name[index2];
                    ++index1;
                }

                MySet.SpecialBonus[SpecialId()].Name = new string[numArray2.Length - 1 + 1];
                MySet.SpecialBonus[SpecialId()].Index = new int[strArray2.Length - 1 + 1];
                var num2 = numArray2.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    MySet.SpecialBonus[SpecialId()].Index[index2] = numArray2[index2];
                    MySet.SpecialBonus[SpecialId()].Name[index2] = strArray2[index2];
                }

                if (MySet.SpecialBonus[SpecialId()].Index.Length < 1)
                    MySet.SpecialBonus[SpecialId()].Special = -1;
            }

            DisplayBonus();
            DisplayBonusText();
        }

        private void lvBonusList_DoubleClick(object sender, EventArgs e)
        {
            if (lvBonusList.SelectedIndices.Count < 1) return;

            var index = Convert.ToInt32(lvBonusList.SelectedItems[0].Tag);
            if (index < 0)
            {
                MessageBox.Show(@"Tag was < 0!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (IsBonus())
                {
                    var bonusPower = lvBonusList.SelectedItems[0].Text;
                    var bonusMode = bonusPower.Contains("(PVP Only)") ? Enums.ePvX.PvP : Enums.ePvX.Any;
                    MySet.Bonus[BonusId()].PvMode = bonusMode;
                    Array.Resize(ref MySet.Bonus[BonusId()].Name, MySet.Bonus[BonusId()].Name.Length + 1);
                    Array.Resize(ref MySet.Bonus[BonusId()].Index, MySet.Bonus[BonusId()].Index.Length + 1);
                    MySet.Bonus[BonusId()].Name[MySet.Bonus[BonusId()].Name.Length - 1] = DatabaseAPI.Database.Power[index].FullName;
                    MySet.Bonus[BonusId()].Index[MySet.Bonus[BonusId()].Index.Length - 1] = index;
                }
                else if (IsSpecial())
                {
                    MySet.SpecialBonus[SpecialId()].Special = SpecialId();
                    Array.Resize(ref MySet.SpecialBonus[SpecialId()].Name, MySet.SpecialBonus[SpecialId()].Name.Length + 1);
                    Array.Resize(ref MySet.SpecialBonus[SpecialId()].Index, MySet.SpecialBonus[SpecialId()].Index.Length + 1);
                    MySet.SpecialBonus[SpecialId()].Name[MySet.SpecialBonus[SpecialId()].Name.Length - 1] = DatabaseAPI.Database.Power[index].FullName;
                    MySet.SpecialBonus[SpecialId()].Index[MySet.SpecialBonus[SpecialId()].Index.Length - 1] = index;
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

        private int SpecialId()
        {
            var num1 = MySet.Enhancements.Length;
            var num2 = num1 + num1 - 2;
            return cbSlotCount.SelectedIndex - num2;
        }

        private void DisplayBonus()
        {
            try
            {
                lstBonus.BeginUpdate();
                lstBonus.Items.Clear();
                if (IsBonus())
                {
                    var index1 = BonusId();
                    var num = MySet.Bonus[index1].Index.Length - 1;
                    for (var index2 = 0; index2 <= num; ++index2)
                    {
                        lstBonus.Items.Add(DatabaseAPI.Database.Power[MySet.Bonus[index1].Index[index2]].PowerName);
                        txtAlternate.Text = MySet.Bonus[index1].AltString;
                    }
                }
                else if (IsSpecial())
                {
                    var index1 = SpecialId();
                    var num = MySet.SpecialBonus[index1].Index.Length - 1;
                    for (var index2 = 0; index2 <= num; ++index2)
                        lstBonus.Items.Add(DatabaseAPI.Database.Power[MySet.SpecialBonus[index1].Index[index2]]
                            .PowerName);
                    txtAlternate.Text = MySet.SpecialBonus[index1].AltString;
                }

                lstBonus.EndUpdate();
                cbSlotCount.Enabled = MySet.Enhancements.Length > 1;
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
            var str1 = RTF.StartRTF();
            var num1 = MySet.Bonus.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                switch (index1)
                {
                    case 0:
                        MySet.Bonus[index1].Slotted = 2;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 1:
                        MySet.Bonus[index1].Slotted = 2;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 2:
                        MySet.Bonus[index1].Slotted = 3;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 3:
                        MySet.Bonus[index1].Slotted = 3;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 4:
                        MySet.Bonus[index1].Slotted = 4;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 5:
                        MySet.Bonus[index1].Slotted = 4;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 6:
                        MySet.Bonus[index1].Slotted = 5;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 7:
                        MySet.Bonus[index1].Slotted = 5;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 8:
                        MySet.Bonus[index1].Slotted = 6;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                    case 9:
                        MySet.Bonus[index1].Slotted = 6;
                        if (MySet.Bonus[index1].Index.Length > 0)
                            str1 = str1 + RTF.Color(RTF.ElementID.Black) +
                                   RTF.Bold(Convert.ToString(MySet.Bonus[index1].Slotted) + " Enhancements: ");
                        break;
                }

                var num2 = MySet.Bonus[index1].Index.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    if (MySet.Bonus[index1].Index[index2] <= -1)
                        continue;
                    if (index2 > 0)
                        str1 += ", ";
                    str1 = str1 + RTF.Color(RTF.ElementID.InventionInvert) +
                           DatabaseAPI.Database.Power[MySet.Bonus[index1].Index[index2]].PowerName;
                }

                if (MySet.Bonus[index1].Index.Length > 0)
                    str1 = str1 + RTF.Crlf() + "   " + RTF.Italic(MySet.GetEffectString(index1, false));
                if (MySet.Bonus[index1].PvMode == Enums.ePvX.PvP)
                    str1 += " (PvP)";
                if (MySet.Bonus[index1].Index.Length > 0)
                    str1 += RTF.Crlf();
            }

            var num3 = MySet.SpecialBonus.Length - 1;
            for (var index1 = 0; index1 <= num3; ++index1)
            {
                if (MySet.SpecialBonus[index1].Special > -1)
                {
                    var str2 = str1 + RTF.Color(RTF.ElementID.Black) + RTF.Bold("Special Case Enhancement: ") +
                               RTF.Color(RTF.ElementID.InventionInvert);
                    if (MySet.Enhancements[MySet.SpecialBonus[index1].Special] > -1)
                        str2 += DatabaseAPI.Database
                            .Enhancements[MySet.Enhancements[MySet.SpecialBonus[index1].Special]].Name;
                    var str3 = str2 + RTF.Crlf();
                    var num2 = MySet.SpecialBonus[index1].Index.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
                    {
                        if (MySet.SpecialBonus[index1].Index[index2] <= -1)
                            continue;
                        if (index2 > 0)
                            str3 += ", ";
                        str3 = str3 + RTF.Color(RTF.ElementID.InventionInvert) +
                               DatabaseAPI.Database.Power[MySet.SpecialBonus[index1].Index[index2]].PowerName;
                    }

                    str1 = str3 + RTF.Crlf() + "   " + RTF.Italic(MySet.GetEffectString(index1, true)) + RTF.Crlf();
                }

                if (MySet.SpecialBonus[index1].Index.Length > 0)
                    str1 += RTF.Crlf();
            }

            rtbBonus.Rtf = str1 + RTF.EndRTF();
        }

        private void DisplayIcon()
        {
            if (!string.IsNullOrWhiteSpace(MySet.Image))
            {
                var img = MySet.Image;
                var path = Path.Combine(File.Exists(Path.Combine(I9Gfx.GetEnhancementsPath(), img)) ? I9Gfx.GetEnhancementsPath() : I9Gfx.GetDbEnhancementsPath(), img);
                using var extendedBitmap1 = new ExtendedBitmap(path);
                using var extendedBitmap2 = new ExtendedBitmap(30, 30);
                extendedBitmap2.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect,
                    I9Gfx.GetOverlayRect(Origin.Grade.SetO), GraphicsUnit.Pixel);
                extendedBitmap2.Graphics.DrawImage(extendedBitmap1.Bitmap, extendedBitmap2.ClipRect,
                    extendedBitmap2.ClipRect, GraphicsUnit.Pixel);
                btnImage.Image = new Bitmap(extendedBitmap2.Bitmap);
                btnImage.Text = MySet.Image;
            }
            else
            {
                using var extendedBitmap = new ExtendedBitmap(30, 30);
                extendedBitmap.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap.ClipRect,
                    I9Gfx.GetOverlayRect(Origin.Grade.SetO), GraphicsUnit.Pixel);
                btnImage.Image = new Bitmap(extendedBitmap.Bitmap);
                btnImage.Text = @"Select Image";
            }
        }

        private void DisplaySetData()
        {
            DisplaySetIcons();
            DisplayIcon();
            txtNameFull.Text = MySet.DisplayName;
            txtNameShort.Text = MySet.ShortName;
            txtDesc.Text = MySet.Desc;
            txtInternal.Text = MySet.Uid;
            SetMinLevel(MySet.LevelMin + 1);
            SetMaxLevel(MySet.LevelMax + 1);
            udMaxLevel.Minimum = udMinLevel.Value;
            udMinLevel.Maximum = udMaxLevel.Value;
            cbSetType.SelectedIndex = (int) MySet.SetType;
            btnImage.Text = MySet.Image;
            DisplayBonusText();
            DisplayBonus();
        }

        private void DisplaySetIcons()
        {
            FillImageList();
            var items = new string[2];
            lvEnh.BeginUpdate();
            lvEnh.Items.Clear();
            FillImageList();
            var num1 = MySet.Enhancements.Length - 1;
            for (var imageIndex = 0; imageIndex <= num1; ++imageIndex)
            {
                var enhancement = DatabaseAPI.Database.Enhancements[MySet.Enhancements[imageIndex]];
                items[0] = enhancement.Name + " (" + enhancement.ShortName + ")";
                items[1] = "";
                var num2 = enhancement.ClassID.Length - 1;
                for (var index1 = 0; index1 <= num2; ++index1)
                {
                    if (!string.IsNullOrWhiteSpace(items[1]))
                    {
                        var strArray1 = items;
                        var num3 = 1;
                        string[] strArray2;
                        IntPtr index2;
                        (strArray2 = strArray1)[(int) (index2 = (IntPtr) num3)] = strArray2[(int) index2] + ",";
                    }

                    var strArray3 = items;
                    var num4 = 1;
                    string[] strArray4;
                    IntPtr index3;
                    (strArray4 = strArray3)[(int) (index3 = (IntPtr) num4)] = strArray4[(int) index3] +
                                                                              DatabaseAPI.Database
                                                                                  .EnhancementClasses[
                                                                                      enhancement.ClassID[index1]]
                                                                                  .ShortName;
                }

                lvEnh.Items.Add(new ListViewItem(items, imageIndex));
            }

            lvEnh.EndUpdate();
        }

        private void FillBonusCombos()
        {
            cbSlotCount.BeginUpdate();
            cbSlotCount.Items.Clear();
            var num0 = MySet.Enhancements.Length;
            var num1 = num0 + num0 - 1;
            for (var index = 0; index <= num1; ++index)
                switch (index)
                {
                    case 2:
                        cbSlotCount.Items.Add(Convert.ToString(index) + " Enhancements");
                        break;
                    case 3:
                        cbSlotCount.Items.Add(Convert.ToString(index - 1) + " Enhancements (PVP Effect)");
                        break;
                    case 4:
                        cbSlotCount.Items.Add(Convert.ToString(index - 1) + " Enhancements");
                        break;
                    case 5:
                        cbSlotCount.Items.Add(Convert.ToString(index - 2) + " Enhancements (PVP Effect)");
                        break;
                    case 6:
                        cbSlotCount.Items.Add(Convert.ToString(index - 2) + " Enhancements");
                        break;
                    case 7:
                        cbSlotCount.Items.Add(Convert.ToString(index - 3) + " Enhancements (PVP Effect)");
                        break;
                    case 8:
                        cbSlotCount.Items.Add(Convert.ToString(index - 3) + " Enhancements");
                        break;
                    case 9:
                        cbSlotCount.Items.Add(Convert.ToString(index - 4) + " Enhancements (PVP Effect)");
                        break;
                    case 10:
                        cbSlotCount.Items.Add(Convert.ToString(index - 4) + " Enhancements");
                        break;
                    case 11:
                        cbSlotCount.Items.Add(Convert.ToString(index - 5) + " Enhancements (PVP Effect)");
                        break;
                }

            var num2 = MySet.Enhancements.Length - 1;
            for (var index = 0; index <= num2; ++index)
                cbSlotCount.Items.Add(DatabaseAPI.Database.Enhancements[MySet.Enhancements[index]].Name);
            if (cbSlotCount.Items.Count > 0)
                cbSlotCount.SelectedIndex = 0;
            cbSlotCount.EndUpdate();
        }

        private void FillBonusList()
        {
            lvBonusList.BeginUpdate();
            lvBonusList.Items.Clear();
            var items = new string[2];
            var num1 = _setBonusList.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                items[1] = "";
                if (DatabaseAPI.Database.Power[_setBonusList[index]].Effects.Length > 0)
                    items[1] = DatabaseAPI.Database.Power[_setBonusList[index]].Effects[0]
                        .BuildEffectStringShort(false, true);
                items[0] = DatabaseAPI.Database.Power[_setBonusList[index]].PowerName;
                lvBonusList.Items.Add(new ListViewItem(items)
                {
                    Tag = _setBonusList[index]
                });
            }

            var num2 = _setBonusListPvp.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                items[1] = "";
                if (DatabaseAPI.Database.Power[_setBonusListPvp[index]].Effects.Length > 0)
                    items[1] = DatabaseAPI.Database.Power[_setBonusListPvp[index]].Effects[0]
                        .BuildEffectStringShort(false, true);
                items[0] = DatabaseAPI.Database.Power[_setBonusListPvp[index]].PowerName + " (PVP Only)";
                lvBonusList.Items.Add(new ListViewItem(items)
                {
                    Tag = _setBonusListPvp[index]
                });
            }

            lvBonusList.Sort();
            lvBonusList.EndUpdate();
        }

        private void FillComboBoxes()
        {
            var names = DatabaseAPI.Database.SetTypes.Select(setType => setType.ShortName).ToList();

            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange(names.ToArray<object>());
            cbSetType.EndUpdate();
        }

        private void FillImageList()
        {
            var imageSize1 = ilEnh.ImageSize;
            var width1 = imageSize1.Width;
            imageSize1 = ilEnh.ImageSize;
            var height1 = imageSize1.Height;
            using var extendedBitmap = new ExtendedBitmap(width1, height1);
            ilEnh.Images.Clear();
            var num = MySet.Enhancements.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var enhancement = DatabaseAPI.Database.Enhancements[MySet.Enhancements[index]];
                if (enhancement.ImageIdx > -1)
                {
                    var gfxGrade = I9Gfx.ToGfxGrade(enhancement.TypeID);
                    extendedBitmap.Graphics.Clear(Color.White);
                    var graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancement(ref graphics,
                        DatabaseAPI.Database.Enhancements[MySet.Enhancements[index]].ImageIdx, gfxGrade);
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

        private void frmSetEdit_Load(object sender, EventArgs e)
        {
            _setBonusList = DatabaseAPI.NidPowers("set_bonus.set_bonus");
            _setBonusListPvp = DatabaseAPI.NidPowers("set_bonus.pvp_set_bonus");
            if (MySet.Bonus.Length < 1 || MySet.Bonus.Length < 6)
                MySet.InitBonusPvP();
            FillComboBoxes();
            FillBonusCombos();
            FillBonusList();
            DisplaySetData();
            _loading = false;
            DisplayBonus();
        }

        private bool IsBonus()
        {
            var num0 = MySet.Enhancements.Length;
            var num1 = num0 + num0 - 2;
            return (cbSlotCount.SelectedIndex > -1) & (cbSlotCount.SelectedIndex < num1);
        }

        private bool IsSpecial()
        {
            var num0 = MySet.Enhancements.Length;
            var num1 = num0 + num0 - 1;
            var num2 = num1 - 1;
            var num3 = num1 + num0 - 1;
            return (cbSlotCount.SelectedIndex >= num2) & (cbSlotCount.SelectedIndex < num3);
        }

        private void TxtBonusFilter_TextChanged(object sender, EventArgs e)
        {
            FillBonusList();
        }
    }
}