using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Hero_Designer
{
    public partial class frmSetEdit : Form
    {

        protected bool Loading;
        public EnhancementSet mySet;
        protected int[] SetBonusList;

        ListView lvBonusList
        {
            get
            {
                return _lvBonusList;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler eventHandler2 = new EventHandler(lvBonusList_DoubleClick);
                if (_lvBonusList != null)
                {
                    _lvBonusList.DoubleClick -= eventHandler2;
                }
                _lvBonusList = value;
                if (_lvBonusList == null)
                    return;
                _lvBonusList.DoubleClick += eventHandler2;
            }
        }

        NumericUpDown udMaxLevel
        {
            get
            {
                return _udMaxLevel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler eventHandler1 = new EventHandler(udMaxLevel_Leave);
                EventHandler eventHandler2 = new EventHandler(udMaxLevel_ValueChanged);
                if (_udMaxLevel != null)
                {
                    _udMaxLevel.Leave -= eventHandler1;
                    _udMaxLevel.ValueChanged -= eventHandler2;
                }
                _udMaxLevel = value;
                if (_udMaxLevel == null)
                    return;
                _udMaxLevel.Leave += eventHandler1;
                _udMaxLevel.ValueChanged += eventHandler2;
            }
        }

        NumericUpDown udMinLevel
        {
            get
            {
                return _udMinLevel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler eventHandler1 = new EventHandler(udMinLevel_Leave);
                EventHandler eventHandler2 = new EventHandler(udMinLevel_ValueChanged);
                if (_udMinLevel != null)
                {
                    _udMinLevel.Leave -= eventHandler1;
                    _udMinLevel.ValueChanged -= eventHandler2;
                }
                _udMinLevel = value;
                if (_udMinLevel == null)
                    return;
                _udMinLevel.Leave += eventHandler1;
                _udMinLevel.ValueChanged += eventHandler2;
            }
        }

        public frmSetEdit(ref EnhancementSet iSet)
        {
            Load += new EventHandler(frmSetEdit_Load);
            SetBonusList = new int[0];
            Loading = true;
            InitializeComponent();
            Name = nameof(frmSetEdit);
            var componentResourceManager = new ComponentResourceManager(typeof(frmSetEdit));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            btnImage.Image = (Image)componentResourceManager.GetObject("btnImage.Image");
            mySet = new EnhancementSet(iSet);
        }

        public int BonusID()
        {
            return cbSlotCount.SelectedIndex;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
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
                    int num = (int)Interaction.MsgBox(("You must select an image from the " + I9Gfx.GetEnhancementsPath() + " folder!\r\n\r\nIf you are adding a new image, you should copy it to the folder and then select it."), MsgBoxStyle.Information, "Ah...");
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

        void btnOK_Click(object sender, EventArgs e)
        {
            mySet.LevelMin = Convert.ToInt32(Decimal.Subtract(udMinLevel.Value, new Decimal(1)));
            mySet.LevelMax = Convert.ToInt32(Decimal.Subtract(udMaxLevel.Value, new Decimal(1)));
            DialogResult = DialogResult.OK;
            Hide();
        }

        void btnPaste_Click(object sender, EventArgs e)
        {
            string str = Convert.ToString(Clipboard.GetData("System.String"));
            char[] chArray = new char[1] { '^' };
            string[] strArray1 = str.Replace("\r\n", Convert.ToString(chArray[0])).Split(chArray);
            chArray[0] = '\t';
            mySet.InitBonus();
            int num1 = strArray1.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                string[] strArray2 = strArray1[index1].Split(chArray);
                if (strArray2.Length > 3)
                {
                    int num2 = (int)Math.Round(Conversion.Val(strArray2[0]));
                    int index2 = DatabaseAPI.NidFromUidPower(strArray2[3]);
                    int num3 = num2 - 2;
                    if (num3 > -1 & index2 > -1)
                    {
                        EnhancementSet.BonusItem[] bonus = mySet.Bonus;
                        int index3 = num3;
                        bonus[index3].Name = (string[])Utils.CopyArray(bonus[index3].Name, (Array)new string[bonus[index3].Name.Length + 1]);
                        bonus[index3].Index = (int[])Utils.CopyArray(bonus[index3].Index, (Array)new int[bonus[index3].Index.Length + 1]);
                        bonus[index3].Index[bonus[index3].Index.Length - 1] = index2;
                        bonus[index3].Name[bonus[index3].Name.Length - 1] = DatabaseAPI.Database.Power[index2].FullName;
                    }
                }
            }
            DisplayBonus();
            DisplayBonusText();
        }

        void cbSetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.SetType = (Enums.eSetType)cbSetType.SelectedIndex;
        }

        void cbSlotX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            DisplayBonus();
            DisplayBonusText();
        }

        public void DisplayBonus()
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
                        lstBonus.Items.Add(DatabaseAPI.Database.Power[mySet.SpecialBonus[index1].Index[index2]].PowerName);
                    txtAlternate.Text = mySet.SpecialBonus[index1].AltString;
                }
                lstBonus.EndUpdate();
                cbSlotCount.Enabled = mySet.Enhancements.Length > 1;
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
        }

        public void DisplayBonusText()
        {
            string str1 = RTF.StartRTF();
            int num1 = mySet.Bonus.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                if (mySet.Bonus[index1].Index.Length > 0)
                    str1 = str1 + RTF.Color(RTF.ElementID.Black) + RTF.Bold(Convert.ToString(mySet.Bonus[index1].Slotted) + " Enhancements: ");
                int num2 = mySet.Bonus[index1].Index.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (mySet.Bonus[index1].Index[index2] > -1)
                    {
                        if (index2 > 0)
                            str1 += ", ";
                        str1 = str1 + RTF.Color(RTF.ElementID.InentionInvert) + DatabaseAPI.Database.Power[mySet.Bonus[index1].Index[index2]].PowerName;
                    }
                }
                if (mySet.Bonus[index1].Index.Length > 0)
                    str1 = str1 + RTF.Crlf() + "   " + RTF.Italic(mySet.GetEffectString(index1, false, false));
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
                    string str2 = str1 + RTF.Color(RTF.ElementID.Black) + RTF.Bold("Special Case Enhancement: ") + RTF.Color(RTF.ElementID.InentionInvert);
                    if (mySet.Enhancements[mySet.SpecialBonus[index1].Special] > -1)
                        str2 += DatabaseAPI.Database.Enhancements[mySet.Enhancements[mySet.SpecialBonus[index1].Special]].Name;
                    string str3 = str2 + RTF.Crlf();
                    int num2 = mySet.SpecialBonus[index1].Index.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (mySet.SpecialBonus[index1].Index[index2] > -1)
                        {
                            if (index2 > 0)
                                str3 += ", ";
                            str3 = str3 + RTF.Color(RTF.ElementID.InentionInvert) + DatabaseAPI.Database.Power[mySet.SpecialBonus[index1].Index[index2]].PowerName;
                        }
                    }
                    str1 = str3 + RTF.Crlf() + "   " + RTF.Italic(mySet.GetEffectString(index1, true, false)) + RTF.Crlf();
                }
                if (mySet.SpecialBonus[index1].Index.Length > 0)
                    str1 += RTF.Crlf();
            }
            rtbBonus.Rtf = str1 + RTF.EndRTF();
        }

        void DisplayIcon()
        {
            if (mySet.Image != "")
            {
                ExtendedBitmap extendedBitmap1 = new ExtendedBitmap(I9Gfx.GetEnhancementsPath() + mySet.Image);
                ExtendedBitmap extendedBitmap2 = new ExtendedBitmap(30, 30);
                extendedBitmap2.Graphics.DrawImage((Image)I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), System.Drawing.GraphicsUnit.Pixel);
                extendedBitmap2.Graphics.DrawImage((Image)extendedBitmap1.Bitmap, extendedBitmap2.ClipRect, extendedBitmap2.ClipRect, System.Drawing.GraphicsUnit.Pixel);
                btnImage.Image = (Image)new Bitmap((Image)extendedBitmap2.Bitmap);
                btnImage.Text = mySet.Image;
            }
            else
            {
                ExtendedBitmap extendedBitmap = new ExtendedBitmap(30, 30);
                extendedBitmap.Graphics.DrawImage((Image)I9Gfx.Borders.Bitmap, extendedBitmap.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), System.Drawing.GraphicsUnit.Pixel);
                btnImage.Image = (Image)new Bitmap((Image)extendedBitmap.Bitmap);
                btnImage.Text = "Select Image";
            }
        }

        public void DisplaySetData()
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

        public void DisplaySetIcons()
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
                    if (items[1] != "")
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

        public void FillBonusCombos()
        {
            cbSlotCount.BeginUpdate();
            cbSlotCount.Items.Clear();
            int num1 = mySet.Enhancements.Length - 2;
            for (int index = 0; index <= num1; ++index)
                cbSlotCount.Items.Add((Convert.ToString(index + 2) + " Enhancements"));
            int num2 = mySet.Enhancements.Length - 1;
            for (int index = 0; index <= num2; ++index)
                cbSlotCount.Items.Add(DatabaseAPI.Database.Enhancements[mySet.Enhancements[index]].Name);
            if (cbSlotCount.Items.Count > 0)
                cbSlotCount.SelectedIndex = 0;
            cbSlotCount.EndUpdate();
        }

        public void FillBonusList()
        {
            lvBonusList.BeginUpdate();
            lvBonusList.Items.Clear();
            string[] items = new string[2];
            int num1 = SetBonusList.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                items[1] = "";
                if (DatabaseAPI.Database.Power[SetBonusList[index]].Effects.Length > 0)
                    items[1] = DatabaseAPI.Database.Power[SetBonusList[index]].Effects[0].BuildEffectStringShort(false, true, false);
                items[0] = DatabaseAPI.Database.Power[SetBonusList[index]].PowerName;
                if (items[0].ToUpper().Contains(this.txtBonusFilter.Text.ToUpper()))
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

        public void FillComboBoxes()
        {
            string[] names = Enum.GetNames(Enums.eSetType.Untyped.GetType());
            cbSetType.BeginUpdate();
            cbSetType.Items.Clear();
            cbSetType.Items.AddRange((object[])names);
            cbSetType.EndUpdate();
        }

        public void FillImageList()
        {
            Size imageSize1 = ilEnh.ImageSize;
            int width1 = imageSize1.Width;
            imageSize1 = ilEnh.ImageSize;
            int height1 = imageSize1.Height;
            ExtendedBitmap extendedBitmap = new ExtendedBitmap(width1, height1);
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
                    ilEnh.Images.Add((Image)extendedBitmap.Bitmap);
                }
                else
                {
                    ImageList.ImageCollection images = ilEnh.Images;
                    Size imageSize2 = ilEnh.ImageSize;
                    int width2 = imageSize2.Width;
                    imageSize2 = ilEnh.ImageSize;
                    int height2 = imageSize2.Height;
                    Bitmap bitmap = new Bitmap(width2, height2);
                    images.Add((Image)bitmap);
                }
            }
        }

        void frmSetEdit_Load(object sender, EventArgs e)
        {
            if (MidsContext.Config.MasterMode)
                btnPaste.Visible = true;
            SetBonusList = DatabaseAPI.NidPowers("set_bonus.set_bonus", "");
            if (mySet.Bonus.Length < 1)
                mySet.InitBonus();
            FillComboBoxes();
            FillBonusCombos();
            FillBonusList();
            DisplaySetData();
            Loading = false;
            DisplayBonus();
        }

        public bool isBonus()
        {
            return cbSlotCount.SelectedIndex > -1 & cbSlotCount.SelectedIndex < mySet.Enhancements.Length - 1;
        }

        public bool isSpecial()
        {
            return cbSlotCount.SelectedIndex >= mySet.Enhancements.Length - 1 & cbSlotCount.SelectedIndex < mySet.Enhancements.Length + mySet.Enhancements.Length - 1;
        }

        void lstBonus_DoubleClick(object sender, EventArgs e)
        {
            if (lstBonus.SelectedIndex < 0)
                return;
            int selectedIndex = lstBonus.SelectedIndex;
            int[] numArray1 = new int[0];
            string[] strArray1 = new string[0];
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
                int num = (int)Interaction.MsgBox("Tag was < 0!", MsgBoxStyle.OkOnly, null);
            }
            else
            {
                if (isBonus())
                {
                    mySet.Bonus[BonusID()].Name = (string[])Utils.CopyArray(mySet.Bonus[BonusID()].Name, (Array)new string[mySet.Bonus[BonusID()].Name.Length + 1]);
                    mySet.Bonus[BonusID()].Index = (int[])Utils.CopyArray(mySet.Bonus[BonusID()].Index, (Array)new int[mySet.Bonus[BonusID()].Index.Length + 1]);
                    mySet.Bonus[BonusID()].Name[mySet.Bonus[BonusID()].Name.Length - 1] = DatabaseAPI.Database.Power[index].FullName;
                    mySet.Bonus[BonusID()].Index[mySet.Bonus[BonusID()].Index.Length - 1] = index;
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

        public void SetMaxLevel(int iValue)
        {
            if (Decimal.Compare(new Decimal(iValue), udMaxLevel.Minimum) < 0)
                iValue = Convert.ToInt32(udMaxLevel.Minimum);
            if (Decimal.Compare(new Decimal(iValue), udMaxLevel.Maximum) > 0)
                iValue = Convert.ToInt32(udMaxLevel.Maximum);
            udMaxLevel.Value = new Decimal(iValue);
        }

        public void SetMinLevel(int iValue)
        {
            if (Decimal.Compare(new Decimal(iValue), udMinLevel.Minimum) < 0)
                iValue = Convert.ToInt32(udMinLevel.Minimum);
            if (Decimal.Compare(new Decimal(iValue), udMinLevel.Maximum) > 0)
                iValue = Convert.ToInt32(udMinLevel.Maximum);
            udMinLevel.Value = new Decimal(iValue);
        }

        public int SpecialID()
        {
            return cbSlotCount.SelectedIndex - (mySet.Enhancements.Length - 1);
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

        void udMaxLevel_Leave(object sender, EventArgs e)
        {
            SetMaxLevel((int)Math.Round(Conversion.Val(udMaxLevel.Text)));
            mySet.LevelMax = Convert.ToInt32(Decimal.Subtract(udMaxLevel.Value, new Decimal(1)));
        }

        void udMaxLevel_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.LevelMax = Convert.ToInt32(Decimal.Subtract(udMaxLevel.Value, new Decimal(1)));
            udMinLevel.Maximum = udMaxLevel.Value;
        }

        void udMinLevel_Leave(object sender, EventArgs e)
        {
            SetMinLevel((int)Math.Round(Conversion.Val(udMinLevel.Text)));
            mySet.LevelMin = Convert.ToInt32(Decimal.Subtract(udMinLevel.Value, new Decimal(1)));
        }

        void udMinLevel_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            mySet.LevelMin = Convert.ToInt32(Decimal.Subtract(udMinLevel.Value, new Decimal(1)));
            udMaxLevel.Minimum = udMinLevel.Value;
        }

        private void TxtBonusFilter_TextChanged(object sender, EventArgs e)
        {
            FillBonusList();
        }
    }
}