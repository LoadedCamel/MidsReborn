
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmEnhData : Form
    {

        protected ExtendedBitmap bxClass;
        protected ExtendedBitmap bxClassList;
        protected int ClassSize;

        protected int EnhAcross;
        protected int EnhPadding;
        protected bool Loading;
        public IEnhancement myEnh;

        public frmEnhData(ref IEnhancement iEnh, int newStaticIndex)
        {
            Load += frmEnhData_Load;
            ClassSize = 15;
            EnhPadding = 3;
            EnhAcross = 5;
            Loading = true;
            InitializeComponent();
            pnlClass.MouseMove += pnlClass_MouseMove;
            pnlClass.Paint += pnlClass_Paint;
            pnlClass.MouseDown += pnlClass_MouseDown;
            pnlClassList.MouseMove += pnlClassList_MouseMove;
            pnlClassList.Paint += pnlClassList_Paint;
            pnlClassList.MouseDown += pnlClassList_MouseDown;
            var componentResourceManager = new ComponentResourceManager(typeof(frmEnhData));
            btnImage.Image = (Image)componentResourceManager.GetObject("btnImage.Image", CultureInfo.InvariantCulture);
            typeSet.Image = (Image)componentResourceManager.GetObject("typeSet.Image", CultureInfo.InvariantCulture);
            typeIO.Image = (Image)componentResourceManager.GetObject("typeIO.Image", CultureInfo.InvariantCulture);
            typeRegular.Image = (Image)componentResourceManager.GetObject("typeRegular.Image", CultureInfo.InvariantCulture);
            typeHO.Image = (Image)componentResourceManager.GetObject("typeHO.Image", CultureInfo.InvariantCulture);
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon", CultureInfo.InvariantCulture);
            Name = nameof(frmEnhData);
            myEnh = new Enhancement(iEnh);
            if (newStaticIndex > 0)
                myEnh.StaticIndex = newStaticIndex;
            ClassSize = 22;
        }

        void btnAdd_Click(object sender, EventArgs e)

        {
            EffectList_Add();
        }

        void btnAddFX_Click(object sender, EventArgs e)

        {
            IEffect iFX = new Effect();
            using frmPowerEffect frmPowerEffect = new frmPowerEffect(iFX);
            if (frmPowerEffect.ShowDialog() != DialogResult.OK)
                return;
            IEnhancement enh = myEnh;
            Enums.sEffect[] sEffectArray = (Enums.sEffect[])Utils.CopyArray(enh.Effect, new Enums.sEffect[myEnh.Effect.Length + 1]);
            enh.Effect = sEffectArray;
            Enums.sEffect[] effect = myEnh.Effect;
            int index = myEnh.Effect.Length - 1;
            effect[index].Mode = Enums.eEffMode.FX;
            effect[index].Enhance.ID = -1;
            effect[index].Enhance.SubID = -1;
            effect[index].Multiplier = 1f;
            effect[index].Schedule = Enums.eSchedule.A;
            effect[index].FX = (IEffect)frmPowerEffect.myFX.Clone();
            effect[index].FX.isEnhancementEffect = true;
            ListSelectedEffects();
            lstSelected.SelectedIndex = lstSelected.Items.Count - 1;
        }

        void btnAutoFill_Click(object sender, EventArgs e)

        {
            Enums.eEnhance eEnhance = Enums.eEnhance.None;
            Enums.eEnhanceShort eEnhanceShort = Enums.eEnhanceShort.None;
            Enums.eMez eMez = Enums.eMez.None;
            Enums.eMezShort eMezShort = Enums.eMezShort.None;
            string[] names1 = Enum.GetNames(eEnhance.GetType());
            string[] names2 = Enum.GetNames(eEnhanceShort.GetType());
            string[] names3 = Enum.GetNames(eMez.GetType());
            string[] names4 = Enum.GetNames(eMezShort.GetType());
            myEnh.Name = "";
            myEnh.ShortName = "";
            names1[4] = "Endurance";
            names1[18] = "Resistance";
            names1[5] = "EndMod";
            names2[18] = "ResDam";
            names3[2] = "Hold";
            names4[2] = "Hold";
            if (myEnh.TypeID == Enums.eType.SetO & myEnh.nIDSet > -1 & myEnh.nIDSet <= DatabaseAPI.Database.EnhancementSets.Count - 1)
                myEnh.UID = DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].DisplayName.Replace(" ", "_") + "_";
            int num1 = 0;
            int num2 = myEnh.Effect.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                if (myEnh.Effect[index].Mode != Enums.eEffMode.Enhancement)
                    continue;
                ++num1;
                Enums.eEnhance id = (Enums.eEnhance)myEnh.Effect[index].Enhance.ID;
                if (id != Enums.eEnhance.Mez)
                {
                    if (!string.IsNullOrWhiteSpace(myEnh.Name))
                        myEnh.Name += "/";
                    myEnh.Name += names1[(int)id];
                    if (!string.IsNullOrWhiteSpace(myEnh.ShortName))
                        myEnh.ShortName += "/";
                    myEnh.ShortName += names2[(int)id];
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(myEnh.Name))
                        myEnh.Name += "/";
                    myEnh.Name += names3[myEnh.Effect[index].Enhance.SubID];
                    if (!string.IsNullOrWhiteSpace(myEnh.ShortName))
                        myEnh.ShortName += "/";
                    myEnh.ShortName += names4[myEnh.Effect[index].Enhance.SubID];
                }
            }
            myEnh.UID += myEnh.Name.Replace("/", "_");

            float num3 = 1f;
            switch (num1)
            {
                case 2:
                    num3 = 0.625f;
                    break;
                case 3:
                    num3 = 0.5f;
                    break;
                case 4:
                    num3 = 7f / 16f;
                    break;
            }
            int num4 = myEnh.Effect.Length - 1;
            for (int index = 0; index <= num4; ++index)
            {
                if (myEnh.Effect[index].Mode == Enums.eEffMode.Enhancement)
                    myEnh.Effect[index].Multiplier = num3;
            }
            DisplayAll();
        }

        void btnCancel_Click(object sender, EventArgs e)

        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnDown_Click(object sender, EventArgs e)

        {
            if (lstSelected.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lstSelected.SelectedIndices[0];
            if (selectedIndex >= lstSelected.Items.Count - 1)
                return;
            Enums.sEffect[] sEffectArray = new Enums.sEffect[2];
            sEffectArray[0].Assign(myEnh.Effect[selectedIndex]);
            sEffectArray[1].Assign(myEnh.Effect[selectedIndex + 1]);
            myEnh.Effect[selectedIndex + 1].Assign(sEffectArray[0]);
            myEnh.Effect[selectedIndex].Assign(sEffectArray[1]);
            FillEffectList();
            ListSelectedEffects();
            lstSelected.SelectedIndex = selectedIndex + 1;
        }

        void btnEdit_Click(object sender, EventArgs e)

        {
            EditClick();
        }

        void btnEditPowerData_Click(object sender, EventArgs e)
        {
            IEnhancement enh = myEnh;
            IPower power = enh.GetPower();
            string text = power.FullName;
            int index1 = DatabaseAPI.NidFromUidPower(power.FullName);
            if (index1 < 0)
            {
                int num1 = (int) Interaction.MsgBox("Unknown error caused an invalid PowerIndex return value.",
                    MsgBoxStyle.Exclamation, "Wha?");
            }
            else
            {
                using frmEditPower frmEditPower = new frmEditPower(DatabaseAPI.Database.Power[index1]);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                    return;
                IPower newPower = new Power(frmEditPower.myPower) {IsModified = true};
                DatabaseAPI.Database.Power[index1] = newPower;
                if (text == DatabaseAPI.Database.Power[index1].FullName)
                    return;
                //Update the full power name in the powerset array
                if (newPower.PowerSetID > -1)
                    DatabaseAPI.Database.Powersets[newPower.PowerSetID].Powers[newPower.PowerSetIndex].FullName =
                        newPower.FullName;

                int num2 = DatabaseAPI.Database.Power[index1].Effects.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                    DatabaseAPI.Database.Power[index1].Effects[index2].PowerFullName =
                        DatabaseAPI.Database.Power[index1].FullName;
                string[] strArray =
                    DatabaseAPI.UidReferencingPowerFix(text, DatabaseAPI.Database.Power[index1].FullName);
                string str1 = "";
                int num3 = strArray.Length - 1;
                for (int index2 = 0; index2 <= num3; ++index2)
                    str1 = str1 + strArray[index2] + "\r\n";
                if (strArray.Length > 0)
                {
                    string str2 = "Power: " + text + " changed to " + DatabaseAPI.Database.Power[index1].FullName +
                                  "\r\nThe following powers referenced this power and were updated:\r\n" + str1 +
                                  "\r\n\r\nThis list has been placed on the clipboard.";
                    Clipboard.SetDataObject(str2, true);
                    int num4 = (int) Interaction.MsgBox(str2);
                }
            }
        }

        void btnEditPowerData3_Click(object sender, EventArgs e)

        {
            IEnhancement enh = myEnh;
            IPower power = enh.GetPower();
            using frmEditPower frmEditPower = new frmEditPower(power);
            if (frmEditPower.ShowDialog() != DialogResult.OK)
                return;
            power = new Power(frmEditPower.myPower) {IsModified = true};
            // could really use structural equality here, but since we don't have it... we'll mark it as modified just because :/
            int num = power.Effects.Length - 1;
            for (int index = 0; index <= num; ++index)
                power.Effects[index].PowerFullName = power.FullName;
            myEnh.SetPower(power);
        }

        void btnImage_Click(object sender, EventArgs e)

        {
            if (Loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetEnhancementsPath();
            ImagePicker.FileName = myEnh.Image;
            if (ImagePicker.ShowDialog() != DialogResult.OK)
                return;
            string str = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(FileIO.AddSlash(ImagePicker.InitialDirectory) + str))
            {
                int num = (int)Interaction.MsgBox(("You must select an image from the " + I9Gfx.GetEnhancementsPath() + " folder!\r\n\r\nIf you are adding a new image, you should copy it to the folder and then select it."), MsgBoxStyle.Information, "Ah...");
            }
            else
            {
                myEnh.Image = str;
                DisplayIcon();
                SetTypeIcons();
            }
        }

        void btnNoImage_Click(object sender, EventArgs e)

        {
            myEnh.Image = "";
            SetTypeIcons();
            DisplayIcon();
        }

        void btnOK_Click(object sender, EventArgs e)

        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        void btnRemove_Click(object sender, EventArgs e)

        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            Enums.sEffect[] sEffectArray = new Enums.sEffect[myEnh.Effect.Length - 1 + 1];
            int selectedIndex = lstSelected.SelectedIndex;
            int index1 = 0;
            int num1 = myEnh.Effect.Length - 1;
            for (int index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                sEffectArray[index1].Assign(myEnh.Effect[index2]);
                ++index1;
            }
            myEnh.Effect = new Enums.sEffect[myEnh.Effect.Length - 2 + 1];
            int num2 = myEnh.Effect.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
                myEnh.Effect[index2].Assign(sEffectArray[index2]);
            FillEffectList();
            ListSelectedEffects();
            if (lstSelected.Items.Count > selectedIndex)
                lstSelected.SelectedIndex = selectedIndex;
            else if (lstSelected.Items.Count == selectedIndex)
                lstSelected.SelectedIndex = selectedIndex - 1;
        }

        void btnUp_Click(object sender, EventArgs e)

        {
            if (lstSelected.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lstSelected.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            Enums.sEffect[] sEffectArray = new Enums.sEffect[2];
            sEffectArray[0].Assign(myEnh.Effect[selectedIndex]);
            sEffectArray[1].Assign(myEnh.Effect[selectedIndex - 1]);
            myEnh.Effect[selectedIndex - 1].Assign(sEffectArray[0]);
            myEnh.Effect[selectedIndex].Assign(sEffectArray[1]);
            FillEffectList();
            ListSelectedEffects();
            lstSelected.SelectedIndex = selectedIndex - 1;
        }

        void cbMutEx_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.MutExID = (Enums.eEnhMutex)cbMutEx.SelectedIndex;
        }

        void cbRecipe_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (cbRecipe.SelectedIndex > 0)
            {
                myEnh.RecipeName = cbRecipe.Text;
                myEnh.RecipeIDX = cbRecipe.SelectedIndex - 1;
            }
            else
            {
                myEnh.RecipeName = "";
                myEnh.RecipeIDX = -1;
            }
        }

        void cbSched_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            int selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode == Enums.eEffMode.Enhancement)
                myEnh.Effect[selectedIndex].Schedule = (Enums.eSchedule)cbSched.SelectedIndex;
        }

        void cbSet_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.nIDSet = cbSet.SelectedIndex - 1;
            myEnh.UIDSet = myEnh.nIDSet <= -1 ? string.Empty : DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Uid;
            UpdateTitle();
            DisplaySetImage();
        }

        void cbSubType_SelectedIndexChanged(object sender, EventArgs e)

        {
            myEnh.SubTypeID = (Enums.eSubtype)cbSubType.SelectedIndex;
        }

        void chkSuperior_CheckedChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.Superior = chkSuperior.Checked;
            if (chkSuperior.Checked)
            {
                myEnh.LevelMin = 49;
                myEnh.LevelMax = 49;
                udMinLevel.Value = new Decimal(50);
                udMaxLevel.Value = new Decimal(50);
            }
            chkUnique.Checked = true;
        }

        void chkUnique_CheckedChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.Unique = chkUnique.Checked;
        }

        public void DisplayAll()
        {
            txtNameFull.Text = myEnh.Name;
            txtNameShort.Text = myEnh.ShortName;
            txtDesc.Text = myEnh.Desc;
            txtProb.Text = Convert.ToString(myEnh.EffectChance, CultureInfo.InvariantCulture);
            txtInternal.Text = myEnh.UID;
            StaticIndex.Text = Convert.ToString(myEnh.StaticIndex, CultureInfo.InvariantCulture);
            SetMinLevel(myEnh.LevelMin + 1);
            SetMaxLevel(myEnh.LevelMax + 1);
            udMaxLevel.Minimum = udMinLevel.Value;
            udMinLevel.Maximum = udMaxLevel.Value;
            chkUnique.Checked = myEnh.Unique;
            cbMutEx.SelectedIndex = (int)myEnh.MutExID;
            chkSuperior.Checked = myEnh.Superior;
            switch (myEnh.TypeID)
            {
                case Enums.eType.Normal:
                    typeRegular.Checked = true;
                    cbSubType.SelectedIndex = -1;
                    cbSubType.Enabled = false;
                    cbRecipe.SelectedIndex = 0;
                    cbRecipe.Enabled = false;
                    break;
                case Enums.eType.InventO:
                    typeIO.Checked = true;
                    cbSubType.SelectedIndex = -1;
                    cbSubType.Enabled = false;
                    cbRecipe.SelectedIndex = myEnh.RecipeIDX + 1;
                    cbRecipe.Enabled = true;
                    break;
                case Enums.eType.SpecialO:
                    typeHO.Checked = true;
                    cbSubType.SelectedIndex = (int)myEnh.SubTypeID;
                    cbSubType.Enabled = true;
                    cbRecipe.Enabled = false;
                    cbRecipe.SelectedIndex = 0;
                    break;
                case Enums.eType.SetO:
                    cbSubType.SelectedIndex = -1;
                    cbSubType.Enabled = false;
                    typeSet.Checked = true;
                    cbRecipe.SelectedIndex = myEnh.RecipeIDX + 1;
                    cbRecipe.Enabled = true;
                    break;
                default:
                    typeRegular.Checked = true;
                    cbSubType.SelectedIndex = -1;
                    cbSubType.Enabled = false;
                    cbRecipe.Enabled = false;
                    break;
            }
            DisplaySet();
            btnImage.Text = myEnh.Image;
            DisplayIcon();
            DisplaySetImage();
            DrawClasses();
            ListSelectedEffects();
            DisplayEnhanceData();
        }

        public void DisplayEnhanceData()
        {
            if (lstSelected.SelectedIndex <= -1)
            {
                btnRemove.Enabled = false;
                gbMod.Enabled = false;
                cbSched.Enabled = false;
                btnEdit.Enabled = false;
            }
            else
            {
                btnRemove.Enabled = true;
                int selectedIndex = lstSelected.SelectedIndex;
                if (myEnh.Effect[selectedIndex].Mode != Enums.eEffMode.Enhancement)
                {
                    btnEdit.Enabled = true;
                    gbMod.Enabled = false;
                    cbSched.Enabled = false;
                }
                else
                {
                    btnEdit.Enabled = myEnh.Effect[selectedIndex].Enhance.ID == 12;
                    gbMod.Enabled = true;
                    cbSched.Enabled = true;
                    switch (myEnh.Effect[selectedIndex].Multiplier.ToString(CultureInfo.InvariantCulture))
                    {
                        case "1":
                            rbMod1.Checked = true;
                            txtModOther.Text = "";
                            txtModOther.Enabled = false;
                            break;
                        case "0.625":
                            rbMod2.Checked = true;
                            txtModOther.Text = "";
                            txtModOther.Enabled = false;
                            break;
                        case "0.5":
                            rbMod3.Checked = true;
                            txtModOther.Text = "";
                            txtModOther.Enabled = false;
                            break;
                        default:
                            txtModOther.Text = Convert.ToString(myEnh.Effect[selectedIndex].Multiplier, CultureInfo.InvariantCulture);
                            rbModOther.Checked = true;
                            txtModOther.Enabled = true;
                            break;
                    }
                    switch (myEnh.Effect[selectedIndex].BuffMode)
                    {
                        case Enums.eBuffDebuff.BuffOnly:
                            rbBuff.Checked = true;
                            break;
                        case Enums.eBuffDebuff.DeBuffOnly:
                            rbDebuff.Checked = true;
                            break;
                        default:
                            rbBoth.Checked = true;
                            break;
                    }
                    cbSched.SelectedIndex = (int)myEnh.Effect[selectedIndex].Schedule;
                }
            }
        }

        public void DisplayIcon()
        {
            if (!string.IsNullOrWhiteSpace(myEnh.Image))
            {
                using ExtendedBitmap extendedBitmap1 = new ExtendedBitmap(I9Gfx.GetEnhancementsPath() + myEnh.Image);
                using ExtendedBitmap extendedBitmap2 = new ExtendedBitmap(30, 30);
                extendedBitmap2.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade(myEnh.TypeID)), GraphicsUnit.Pixel);
                extendedBitmap2.Graphics.DrawImage(extendedBitmap1.Bitmap, extendedBitmap2.ClipRect, extendedBitmap2.ClipRect, GraphicsUnit.Pixel);
                btnImage.Image = new Bitmap(extendedBitmap2.Bitmap);
                btnImage.Text = myEnh.Image;
            }
            else
            {
                switch (myEnh.TypeID)
                {
                    case Enums.eType.Normal:
                        btnImage.Image = typeRegular.Image;
                        break;
                    case Enums.eType.InventO:
                        btnImage.Image = typeIO.Image;
                        break;
                    case Enums.eType.SpecialO:
                        btnImage.Image = typeHO.Image;
                        break;
                    case Enums.eType.SetO:
                        btnImage.Image = typeSet.Image;
                        break;
                }
                btnImage.Text = "Select Image";
            }
        }

        public void DisplaySet()
        {
            gbSet.Enabled = myEnh.TypeID == Enums.eType.SetO;
            cbSet.SelectedIndex = myEnh.nIDSet + 1;
            DisplaySetImage();
        }

        public void DisplaySetImage()
        {
            if (myEnh.nIDSet > -1)
            {
                myEnh.Image = DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Image;
                DisplayIcon();
                SetTypeIcons();
                if (!string.IsNullOrWhiteSpace(DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Image))
                {
                    using ExtendedBitmap extendedBitmap1 = new ExtendedBitmap(I9Gfx.GetEnhancementsPath() + DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Image);
                    using ExtendedBitmap extendedBitmap2 = new ExtendedBitmap(30, 30);
                    extendedBitmap2.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), GraphicsUnit.Pixel);
                    extendedBitmap2.Graphics.DrawImage(extendedBitmap1.Bitmap, extendedBitmap2.ClipRect, extendedBitmap2.ClipRect, GraphicsUnit.Pixel);
                    pbSet.Image = new Bitmap(extendedBitmap2.Bitmap);
                }
                else
                {
                    using ExtendedBitmap extendedBitmap = new ExtendedBitmap(30, 30);
                    extendedBitmap.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), GraphicsUnit.Pixel);
                    pbSet.Image = new Bitmap(extendedBitmap.Bitmap);
                }
            }
            else
                pbSet.Image = new Bitmap(pbSet.Width, pbSet.Height);
        }

        public void DrawClasses()
        {
            bxClass = new ExtendedBitmap(pnlClass.Width, pnlClass.Height);
            int enhPadding1 = EnhPadding;
            int enhPadding2 = EnhPadding;
            int num1 = 0;
            using SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0,0,0));
            bxClass.Graphics.FillRectangle(solidBrush, bxClass.ClipRect);
            int num2 = myEnh.ClassID.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                Rectangle destRect = new Rectangle(enhPadding2, enhPadding1, ClassSize, ClassSize);
                bxClass.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(myEnh.ClassID[index]), GraphicsUnit.Pixel);
                enhPadding2 += ClassSize + EnhPadding;
                ++num1;
                if (num1 != 2)
                    continue;
                num1 = 0;
                enhPadding2 = EnhPadding;
                enhPadding1 += ClassSize + EnhPadding;
            }
            pnlClass.CreateGraphics().DrawImageUnscaled(bxClass.Bitmap, 0, 0);
        }

        public void DrawClassList()
        {
            using ExtendedBitmap bxClassList = new ExtendedBitmap(pnlClassList.Width, pnlClassList.Height);
            int enhPadding1 = EnhPadding;
            int enhPadding2 = EnhPadding;
            int num1 = 0;
            using SolidBrush solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxClassList.Graphics.FillRectangle(solidBrush, bxClassList.ClipRect);
            int num2 = DatabaseAPI.Database.EnhancementClasses.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                Rectangle destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
                bxClassList.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(index), GraphicsUnit.Pixel);
                enhPadding2 += 30 + EnhPadding;
                ++num1;
                if (num1 != EnhAcross)
                    continue;
                num1 = 0;
                enhPadding2 = EnhPadding;
                enhPadding1 += 30 + EnhPadding;
            }
            pnlClassList.CreateGraphics().DrawImageUnscaled(bxClassList.Bitmap, 0, 0);
        }

        public void EditClick()
        {
            bool flag = true;
            int num1 = -1;
            if (lstSelected.SelectedIndex <= -1)
                return;
            int selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode == Enums.eEffMode.Enhancement)
            {
                if (myEnh.Effect[selectedIndex].Enhance.ID == 12)
                {
                    int subId = myEnh.Effect[selectedIndex].Enhance.SubID;
                    num1 = MezPicker(subId);
                    if (num1 == subId)
                        return;
                    int num2 = myEnh.Effect.Length - 1;
                    for (int index1 = 0; index1 <= num2; ++index1)
                    {
                        Enums.sEffect[] effect = myEnh.Effect;
                        int index2 = index1;
                        if (effect[index2].Mode == Enums.eEffMode.Enhancement & effect[index2].Enhance.SubID == num1)
                            flag = false;
                    }
                }
                if (!flag)
                {
                    int num2 = (int)Interaction.MsgBox("This effect has already been added!", MsgBoxStyle.Information, "There can be only one.");
                    return;
                }
                myEnh.Effect[selectedIndex].Enhance.SubID = num1;
            }
            else
            {
                using frmPowerEffect frmPowerEffect = new frmPowerEffect(myEnh.Effect[selectedIndex].FX);
                if (frmPowerEffect.ShowDialog() == DialogResult.OK)
                {
                    Enums.sEffect[] effect = myEnh.Effect;
                    int index = selectedIndex;
                    effect[index].Mode = Enums.eEffMode.FX;
                    effect[index].Enhance.ID = -1;
                    effect[index].Enhance.SubID = -1;
                    effect[index].Multiplier = 1f;
                    effect[index].Schedule = Enums.eSchedule.A;
                    effect[index].FX = (IEffect)frmPowerEffect.myFX.Clone();
                }
            }
            ListSelectedEffects();
            lstSelected.SelectedIndex = selectedIndex;
        }

        public void EffectList_Add()
        {
            if (lstAvailable.SelectedIndex <= -1)
                return;
            Enums.eEnhance eEnhance = Enums.eEnhance.None;
            bool flag = true;
            int tSub = -1;
            Enums.eEnhance integer = (Enums.eEnhance)Convert.ToInt32(Enum.Parse(eEnhance.GetType(), lstAvailable.Items[lstAvailable.SelectedIndex].ToString()), CultureInfo.InvariantCulture);
            if (integer == Enums.eEnhance.Mez)
            {
                tSub = MezPicker();
                int num = myEnh.Effect.Length - 1;
                for (int index1 = 0; index1 <= num; ++index1)
                {
                    Enums.sEffect[] effect = myEnh.Effect;
                    int index2 = index1;
                    if (effect[index2].Mode == Enums.eEffMode.Enhancement & effect[index2].Enhance.SubID == tSub)
                        flag = false;
                }
            }
            if (!flag)
            {
                int num1 = (int)Interaction.MsgBox("This effect has already been added!", MsgBoxStyle.Information, "There can be only one.");
            }
            else
            {
                IEnhancement enh = myEnh;
                Enums.sEffect[] sEffectArray = (Enums.sEffect[])Utils.CopyArray(enh.Effect, new Enums.sEffect[myEnh.Effect.Length + 1]);
                enh.Effect = sEffectArray;
                Enums.sEffect[] effect = myEnh.Effect;
                int index = myEnh.Effect.Length - 1;
                effect[index].Mode = Enums.eEffMode.Enhancement;
                effect[index].Enhance.ID = (int)integer;
                effect[index].Enhance.SubID = tSub;
                effect[index].Multiplier = 1f;
                effect[index].Schedule = Enhancement.GetSchedule(integer, tSub);
                FillEffectList();
                ListSelectedEffects();
                lstSelected.SelectedIndex = lstSelected.Items.Count - 1;
            }
        }

        public void FillEffectList()
        {
            Enums.eEnhance eEnhance1 = Enums.eEnhance.None;
            lstAvailable.BeginUpdate();
            lstAvailable.Items.Clear();
            string[] names = Enum.GetNames(eEnhance1.GetType());
            int num1 = names.Length - 1;
            for (int index1 = 1; index1 <= num1; ++index1)
            {
                Enums.eEnhance eEnhance2 = (Enums.eEnhance)index1;
                bool flag = false;
                int num2 = myEnh.Effect.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (myEnh.Effect[index2].Mode == Enums.eEffMode.Enhancement && myEnh.Effect[index2].Enhance.ID == index1 & eEnhance2 != Enums.eEnhance.Mez)
                        flag = true;
                }
                if (!flag)
                    lstAvailable.Items.Add(names[index1]);
            }
            btnAdd.Enabled = lstAvailable.Items.Count > 0;
            lstAvailable.EndUpdate();
        }

        public void FillMutExList()
        {
            string[] names = Enum.GetNames(Enums.eEnhMutex.None.GetType());
            cbMutEx.BeginUpdate();
            cbMutEx.Items.Clear();
            cbMutEx.Items.AddRange(names);
            cbMutEx.EndUpdate();
        }

        public void FillRecipeList()
        {
            cbRecipe.BeginUpdate();
            cbRecipe.Items.Clear();
            cbRecipe.Items.Add("None");
            int num = DatabaseAPI.Database.Recipes.Length - 1;
            for (int index = 0; index <= num; ++index)
                cbRecipe.Items.Add(DatabaseAPI.Database.Recipes[index].InternalName);
            cbRecipe.EndUpdate();
        }

        public void FillSchedules()
        {
            cbSched.BeginUpdate();
            cbSched.Items.Clear();
            string Style = "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##";
            cbSched.Items.Add(("A (" + Strings.Format((float)(DatabaseAPI.Database.MultSO[0][0] * 100.0), Style) + "%)"));
            cbSched.Items.Add(("B (" + Strings.Format((float)(DatabaseAPI.Database.MultSO[0][1] * 100.0), Style) + "%)"));
            cbSched.Items.Add(("C (" + Strings.Format((float)(DatabaseAPI.Database.MultSO[0][2] * 100.0), Style) + "%)"));
            cbSched.Items.Add(("D (" + Strings.Format((float)(DatabaseAPI.Database.MultSO[0][3] * 100.0), Style) + "%)"));
            cbSched.EndUpdate();
        }

        public void FillSetList()
        {
            cbSet.BeginUpdate();
            cbSet.Items.Clear();
            cbSet.Items.Add("None");
            int num = DatabaseAPI.Database.EnhancementSets.Count - 1;
            for (int index = 0; index <= num; ++index)
                cbSet.Items.Add(DatabaseAPI.Database.EnhancementSets[index].Uid);
            cbSet.EndUpdate();
        }

        public void FillSubTypeList()
        {
            string[] names = Enum.GetNames(Enums.eSubtype.None.GetType());
            cbSubType.BeginUpdate();
            cbSubType.Items.Clear();
            cbSubType.Items.AddRange(names);
            cbSubType.EndUpdate();
        }

        void frmEnhData_Load(object sender, EventArgs e)

        {
            FillSetList();
            FillEffectList();
            FillMutExList();
            FillSubTypeList();
            FillRecipeList();
            DisplayAll();
            SetTypeIcons();
            DrawClassList();
            FillSchedules();
            UpdateTitle();
            Loading = false;
        }

        [DebuggerStepThrough]

        public void ListSelectedEffects()
        {
            Enums.eEnhance eEnhance = Enums.eEnhance.None;
            Enums.eMez eMez = Enums.eMez.None;
            string[] names1 = Enum.GetNames(eEnhance.GetType());
            string[] names2 = Enum.GetNames(eMez.GetType());
            lstSelected.BeginUpdate();
            lstSelected.Items.Clear();
            int num = myEnh.Effect.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (myEnh.Effect[index].Mode == Enums.eEffMode.Enhancement)
                {
                    string str = names1[myEnh.Effect[index].Enhance.ID];
                    if (myEnh.Effect[index].Enhance.SubID > -1)
                        str = str + ":" + names2[myEnh.Effect[index].Enhance.SubID];
                    lstSelected.Items.Add(str);
                }
                else
                    lstSelected.Items.Add(("Special: " + myEnh.Effect[index].FX.BuildEffectString()));
            }
            lstSelected.EndUpdate();
        }

        void lstAvailable_DoubleClick(object sender, EventArgs e)

        {
            EffectList_Add();
        }

        void lstSelected_SelectedIndexChanged(object sender, EventArgs e)

        {
            DisplayEnhanceData();
            tTip.SetToolTip(lstSelected, Convert.ToString(lstSelected.SelectedItem, CultureInfo.InvariantCulture));
        }

        public static int MezPicker(int index = 1)
        => frmEnhMiniPick.MezPicker(index);

        void PickerExpand()

        {
            if (gbClass.Width == 84)
            {
                gbClass.Width = 272;
                gbClass.Left -= 188;
                lblClass.Width = 256;
            }
            else
            {
                gbClass.Width = 84;
                gbClass.Left = 596;
                lblClass.Width = pnlClass.Width;
            }
        }

        void pnlClass_MouseDown(object sender, MouseEventArgs e)

        {
            if (e.Button == MouseButtons.Right)
            {
                PickerExpand();
            }
            else
            {
                if (gbClass.Width <= 84 || Loading)
                    return;
                int num1 = -1;
                int num2 = -1;
                int num3 = 0;
                do
                {
                    if (e.X > (EnhPadding + ClassSize) * num3 & e.X < (EnhPadding + ClassSize) * (num3 + 1))
                        num1 = num3;
                    ++num3;
                }
                while (num3 <= 1);
                int num4 = 0;
                do
                {
                    if (e.Y > (EnhPadding + ClassSize) * num4 & e.Y < (EnhPadding + ClassSize) * (num4 + 1))
                        num2 = num4;
                    ++num4;
                }
                while (num4 <= 10);
                int num5 = num1 + num2 * 2;
                if (!(num5 < myEnh.ClassID.Length & num1 > -1 & num2 > -1))
                    return;
                int[] numArray = new int[myEnh.ClassID.Length - 1 + 1];
                int num6 = myEnh.ClassID.Length - 1;
                for (int index = 0; index <= num6; ++index)
                    numArray[index] = myEnh.ClassID[index];
                int index1 = 0;
                myEnh.ClassID = new int[myEnh.ClassID.Length - 2 + 1];
                int num7 = numArray.Length - 1;
                for (int index2 = 0; index2 <= num7; ++index2)
                {
                    if (index2 == num5)
                        continue;
                    myEnh.ClassID[index1] = numArray[index2];
                    ++index1;
                }
                Array.Sort(myEnh.ClassID);
                DrawClasses();
            }
        }

        void pnlClass_MouseMove(object sender, MouseEventArgs e)

        {
            int num1 = -1;
            int num2 = -1;
            int num3 = 0;
            do
            {
                if (e.X > (EnhPadding + ClassSize) * num3 & e.X < (EnhPadding + ClassSize) * (num3 + 1))
                    num1 = num3;
                ++num3;
            }
            while (num3 <= 1);
            int num4 = 0;
            do
            {
                if (e.Y > (EnhPadding + ClassSize) * num4 & e.Y < (EnhPadding + ClassSize) * (num4 + 1))
                    num2 = num4;
                ++num4;
            }
            while (num4 <= 10);
            int index = num1 + num2 * 2;
            if (index < myEnh.ClassID.Length & num1 > -1 & num2 > -1)
            {
                lblClass.Text = gbClass.Width < 100 ? DatabaseAPI.Database.EnhancementClasses[myEnh.ClassID[index]].ShortName : DatabaseAPI.Database.EnhancementClasses[myEnh.ClassID[index]].Name;
            }
            else
                lblClass.Text = "";
        }

        void pnlClass_Paint(object sender, PaintEventArgs e)

        {
            if (bxClass == null)
                return;
            e.Graphics.DrawImageUnscaled(bxClass.Bitmap, 0, 0);
        }

        void pnlClassList_MouseDown(object sender, MouseEventArgs e)

        {
            if (e.Button == MouseButtons.Right)
            {
                PickerExpand();
            }
            else
            {
                if (gbClass.Width <= 84 || Loading)
                    return;
                int num1 = -1;
                int num2 = -1;
                int num3 = EnhAcross - 1;
                for (int index = 0; index <= num3; ++index)
                {
                    if (e.X > (EnhPadding + 30) * index & e.X < (EnhPadding + 30) * (index + 1))
                        num1 = index;
                }
                int num4 = 0;
                do
                {
                    if (e.Y > (EnhPadding + 30) * num4 & e.Y < (EnhPadding + 30) * (num4 + 1))
                        num2 = num4;
                    ++num4;
                }
                while (num4 <= 10);
                int num5 = num1 + num2 * EnhAcross;
                if (!(num5 < DatabaseAPI.Database.EnhancementClasses.Length & num1 > -1 & num2 > -1))
                    return;
                {
                    bool flag = false;
                    int num6 = myEnh.ClassID.Length - 1;
                    for (int index = 0; index <= num6; ++index)
                    {
                        if (myEnh.ClassID[index] == num5)
                            flag = true;
                    }

                    if (flag)
                        return;
                    IEnhancement enh = myEnh;
                    int[] numArray = (int[])Utils.CopyArray(enh.ClassID, new int[myEnh.ClassID.Length + 1]);
                    enh.ClassID = numArray;
                    myEnh.ClassID[myEnh.ClassID.Length - 1] = num5;
                    Array.Sort(myEnh.ClassID);
                    DrawClasses();
                }
            }
        }

        void pnlClassList_MouseMove(object sender, MouseEventArgs e)

        {
            int num1 = -1;
            int num2 = -1;
            int num3 = EnhAcross - 1;
            for (int index = 0; index <= num3; ++index)
            {
                if (e.X > (EnhPadding + 30) * index & e.X < (EnhPadding + 30) * (index + 1))
                    num1 = index;
            }
            int num4 = 0;
            do
            {
                if (e.Y > (EnhPadding + 30) * num4 & e.Y < (EnhPadding + 30) * (num4 + 1))
                    num2 = num4;
                ++num4;
            }
            while (num4 <= 10);
            int index1 = num1 + num2 * EnhAcross;
            if (index1 < DatabaseAPI.Database.EnhancementClasses.Length & num1 > -1 & num2 > -1)
                lblClass.Text = DatabaseAPI.Database.EnhancementClasses[index1].Name;
            else
                lblClass.Text = string.Empty;
        }

        void pnlClassList_Paint(object sender, PaintEventArgs e)

        {
            if (bxClassList == null)
                return;
            e.Graphics.DrawImageUnscaled(bxClassList.Bitmap, 0, 0);
        }

        void rbBuffDebuff_CheckedChanged(object sender, EventArgs e)

        {
            if (Loading || lstSelected.SelectedIndex <= -1)
                return;
            int selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode != Enums.eEffMode.Enhancement)
                return;
            if (rbBuff.Checked)
                myEnh.Effect[selectedIndex].BuffMode = Enums.eBuffDebuff.BuffOnly;
            else if (rbDebuff.Checked)
                myEnh.Effect[selectedIndex].BuffMode = Enums.eBuffDebuff.DeBuffOnly;
            else if (rbBoth.Checked)
                myEnh.Effect[selectedIndex].BuffMode = Enums.eBuffDebuff.Any;
        }

        void rbMod_CheckedChanged(object sender, EventArgs e)

        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            int selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode != Enums.eEffMode.Enhancement)
                return;
            txtModOther.Enabled = false;
            if (rbModOther.Checked)
            {
                txtModOther.Enabled = true;
                myEnh.Effect[selectedIndex].Multiplier = (float)Conversion.Val(txtModOther.Text);
                txtModOther.SelectAll();
                txtModOther.Select();
            }
            else if (rbMod1.Checked)
                myEnh.Effect[selectedIndex].Multiplier = 1f;
            else if (rbMod2.Checked)
                myEnh.Effect[selectedIndex].Multiplier = 0.625f;
            else if (rbMod3.Checked)
                myEnh.Effect[selectedIndex].Multiplier = 0.5f;
            else if (rbMod4.Checked)
                myEnh.Effect[selectedIndex].Multiplier = 7f / 16f;
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

        public void SetTypeIcons()
        {
            using ExtendedBitmap extendedBitmap1 = new ExtendedBitmap(30, 30);
            using ExtendedBitmap extendedBitmap2 = string.IsNullOrWhiteSpace(myEnh.Image) ? new ExtendedBitmap(30, 30) : new ExtendedBitmap(I9Gfx.GetEnhancementsPath() + myEnh.Image);
            extendedBitmap1.Graphics.Clear(Color.Transparent);
            extendedBitmap1.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade(Enums.eType.Normal)), GraphicsUnit.Pixel);
            extendedBitmap1.Graphics.DrawImage(extendedBitmap2.Bitmap, 0, 0);
            typeRegular.Image = new Bitmap(extendedBitmap1.Bitmap);
            extendedBitmap1.Graphics.Clear(Color.Transparent);
            extendedBitmap1.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade(Enums.eType.InventO)), GraphicsUnit.Pixel);
            extendedBitmap1.Graphics.DrawImage(extendedBitmap2.Bitmap, 0, 0);
            typeIO.Image = new Bitmap(extendedBitmap1.Bitmap);
            extendedBitmap1.Graphics.Clear(Color.Transparent);
            extendedBitmap1.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade(Enums.eType.SpecialO)), GraphicsUnit.Pixel);
            extendedBitmap1.Graphics.DrawImage(extendedBitmap2.Bitmap, 0, 0);
            typeHO.Image = new Bitmap(extendedBitmap1.Bitmap);
            extendedBitmap1.Graphics.Clear(Color.Transparent);
            extendedBitmap1.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade(Enums.eType.SetO)), GraphicsUnit.Pixel);
            extendedBitmap1.Graphics.DrawImage(extendedBitmap2.Bitmap, 0, 0);
            typeSet.Image = new Bitmap(extendedBitmap1.Bitmap);
        }

        void StaticIndex_TextChanged(object sender, EventArgs e)

        {
            myEnh.StaticIndex = Convert.ToInt32(StaticIndex.Text, CultureInfo.InvariantCulture);
        }

        void txtDesc_TextChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.Desc = txtDesc.Text;
        }

        void txtInternal_TextChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.UID = txtInternal.Text;
        }

        void txtModOther_TextChanged(object sender, EventArgs e)

        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            int selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode == Enums.eEffMode.Enhancement && rbModOther.Checked)
                myEnh.Effect[selectedIndex].Multiplier = (float)Conversion.Val(txtModOther.Text);
        }

        void txtNameFull_TextChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.Name = txtNameFull.Text;
            UpdateTitle();
        }

        void txtNameShort_TextChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.ShortName = txtNameShort.Text;
        }

        void txtProb_Leave(object sender, EventArgs e)

        {
            if (Loading)
                return;
            txtProb.Text = Convert.ToString(myEnh.EffectChance, CultureInfo.InvariantCulture);
        }

        void txtProb_TextChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            float num = (float)Conversion.Val(txtProb.Text);
            if (num > 1.0)
                num = 1f;
            if (num < 0.0)
                num = 0.0f;
            myEnh.EffectChance = num;
        }

        void type_CheckedChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            if (typeRegular.Checked)
            {
                myEnh.TypeID = Enums.eType.Normal;
                chkUnique.Checked = false;
                cbSubType.Enabled = false;
                cbSubType.SelectedIndex = -1;
                cbRecipe.SelectedIndex = -1;
                cbRecipe.Enabled = false;
            }
            else if (typeIO.Checked)
            {
                myEnh.TypeID = Enums.eType.InventO;
                chkUnique.Checked = false;
                cbSubType.Enabled = false;
                cbSubType.SelectedIndex = -1;
                cbRecipe.SelectedIndex = 0;
                cbRecipe.Enabled = true;
            }
            else if (typeHO.Checked)
            {
                myEnh.TypeID = Enums.eType.SpecialO;
                chkUnique.Checked = false;
                cbSubType.Enabled = true;
                cbSubType.SelectedIndex = 0;
                cbRecipe.SelectedIndex = -1;
                cbRecipe.Enabled = false;
            }
            else if (typeSet.Checked)
            {
                myEnh.TypeID = Enums.eType.SetO;
                cbSet.Select();
                cbSubType.Enabled = false;
                cbSubType.SelectedIndex = -1;
                cbRecipe.SelectedIndex = 0;
                cbRecipe.Enabled = true;
            }
            DisplaySet();
            UpdateTitle();
            DisplayIcon();
        }

        void udMaxLevel_Leave(object sender, EventArgs e)

        {
            SetMaxLevel((int)Math.Round(Conversion.Val(udMaxLevel.Text)));
            myEnh.LevelMax = Convert.ToInt32(Decimal.Subtract(udMaxLevel.Value, new Decimal(1)));
        }

        void udMaxLevel_ValueChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.LevelMax = Convert.ToInt32(Decimal.Subtract(udMaxLevel.Value, new Decimal(1)));
            udMinLevel.Maximum = udMaxLevel.Value;
        }

        void udMinLevel_Leave(object sender, EventArgs e)

        {
            SetMinLevel((int)Math.Round(Conversion.Val(udMinLevel.Text)));
            myEnh.LevelMin = Convert.ToInt32(Decimal.Subtract(udMinLevel.Value, new Decimal(1)));
        }

        void udMinLevel_ValueChanged(object sender, EventArgs e)

        {
            if (Loading)
                return;
            myEnh.LevelMin = Convert.ToInt32(Decimal.Subtract(udMinLevel.Value, new Decimal(1)));
            udMaxLevel.Minimum = udMinLevel.Value;
        }

        public void UpdateTitle()
        {
            string str1 = "Edit ";
            string str2;
            switch (myEnh.TypeID)
            {
                case Enums.eType.InventO:
                    str2 = str1 + "Invention: ";
                    break;
                case Enums.eType.SpecialO:
                    str2 = str1 + "HO: ";
                    break;
                case Enums.eType.SetO:
                    str2 = myEnh.nIDSet > -1 ? str1 + DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].DisplayName + ": " : str1 + "Set Invention: ";
                    break;
                default:
                    str2 = str1 + "Enhancement: ";
                    break;
            }
            Text = str2 + myEnh.Name;
        }
    }
}