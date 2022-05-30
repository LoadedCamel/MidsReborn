using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEnhData : Form
    {
        private readonly int ClassSize;
        private readonly int EnhAcross;
        private readonly int EnhPadding;
        public readonly IEnhancement myEnh;
        private ExtendedBitmap bxClass;
        private ExtendedBitmap bxClassList;
        private bool Loading;
        private bool EnhClassesSelectorOpen = false;

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
            btnImage.Image = Resources.enhData;
            typeSet.Image = Resources.enhData;
            typeIO.Image = Resources.enhData;
            typeRegular.Image = Resources.enhData;
            typeHO.Image = Resources.enhData;
            Icon = Resources.reborn;
            Name = nameof(frmEnhData);
            myEnh = new Enhancement(iEnh);
            if (newStaticIndex > 0)
                myEnh.StaticIndex = newStaticIndex;
            ClassSize = 22;
        }

        private int RecipeCbToIndex()
        {
            if (cbRecipe.SelectedIndex < 0) return -1;
            
            var activeItem = cbRecipe.SelectedItem.ToString();
            if (activeItem == "None") return -1;

            return Array.IndexOf(DatabaseAPI.Database.Recipes,
                DatabaseAPI.Database.Recipes.First(r => r.InternalName == activeItem));
        }

        private int RecipeIndexToCbSel(IEnhancement enh)
        {
            return cbRecipe.FindStringExact(enh.RecipeIDX < 0 ? "" : DatabaseAPI.Database.Recipes[enh.RecipeIDX].InternalName);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EffectList_Add();
        }

        private void btnAddFX_Click(object sender, EventArgs e)
        {
            IEffect iFx = new Effect();
            using var frmPowerEffect = new frmPowerEffect(iFx);
            if (frmPowerEffect.ShowDialog() != DialogResult.OK)
                return;
            var enh = myEnh;
  
            var effects = enh.Effect.ToList();
            effects.Add(new Enums.sEffect
            {
                Mode = Enums.eEffMode.FX,
                Enhance = new Enums.sTwinID { ID = -1, SubID = -1 },
                Multiplier = 1f,
                Schedule = Enums.eSchedule.A,
                FX = (IEffect)frmPowerEffect.MyFx.Clone()
            });
            effects[effects.Count - 1].FX.isEnhancementEffect = true;
            var sEffects = effects.ToArray();
            enh.Effect = sEffects;
            ListSelectedEffects();
            lstSelected.SelectedIndex = lstSelected.Items.Count - 1;
        }

        private void btnAutoFill_Click(object sender, EventArgs e)
        {
            var eEnhance = Enums.eEnhance.None;
            var eEnhanceShort = Enums.eEnhanceShort.None;
            var eMez = Enums.eMez.None;
            var eMezShort = Enums.eMezShort.None;
            var names1 = Enum.GetNames(eEnhance.GetType());
            var names2 = Enum.GetNames(eEnhanceShort.GetType());
            var names3 = Enum.GetNames(eMez.GetType());
            var names4 = Enum.GetNames(eMezShort.GetType());
            myEnh.Name = "";
            myEnh.ShortName = "";
            names1[4] = "Endurance";
            names1[18] = "Resistance";
            names1[5] = "EndMod";
            names2[18] = "ResDam";
            names3[2] = "Hold";
            names4[2] = "Hold";
            if ((myEnh.TypeID == Enums.eType.SetO) & (myEnh.nIDSet > -1) &
                (myEnh.nIDSet <= DatabaseAPI.Database.EnhancementSets.Count - 1))
                myEnh.UID = DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].DisplayName.Replace(" ", "_") + "_";
            var num1 = 0;
            var num2 = myEnh.Effect.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (myEnh.Effect[index].Mode != Enums.eEffMode.Enhancement)
                    continue;
                ++num1;
                var id = (Enums.eEnhance)myEnh.Effect[index].Enhance.ID;
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

            var num3 = num1 switch
            {
                2 => 0.625f,
                3 => 0.5f,
                4 => 7f / 16f,
                _ => 1f
            };
            var num4 = myEnh.Effect.Length - 1;
            for (var index = 0; index <= num4; ++index)
                if (myEnh.Effect[index].Mode == Enums.eEffMode.Enhancement)
                    myEnh.Effect[index].Multiplier = num3;
            DisplayAll();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lstSelected.SelectedIndices[0];
            if (selectedIndex >= lstSelected.Items.Count - 1)
                return;
            var sEffectArray = new Enums.sEffect[2];
            sEffectArray[0].Assign(myEnh.Effect[selectedIndex]);
            sEffectArray[1].Assign(myEnh.Effect[selectedIndex + 1]);
            myEnh.Effect[selectedIndex + 1].Assign(sEffectArray[0]);
            myEnh.Effect[selectedIndex].Assign(sEffectArray[1]);
            FillEffectList();
            ListSelectedEffects();
            lstSelected.SelectedIndex = selectedIndex + 1;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditClick();
        }

        private void btnEditPowerData_Click(object sender, EventArgs e)
        {
            var enh = myEnh;
            var power = enh.GetPower();
            var text = power.FullName;
            var index1 = DatabaseAPI.NidFromUidPower(power.FullName);
            if (index1 < 0)
            {
                MessageBox.Show("An unknown error caused an invalid PowerIndex return value.", "Wha?",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                using var frmEditPower = new frmEditPower(DatabaseAPI.Database.Power[index1]);
                if (frmEditPower.ShowDialog() != DialogResult.OK)
                    return;
                IPower? newPower = new Power(frmEditPower.myPower) { IsModified = true };
                DatabaseAPI.Database.Power[index1] = newPower;
                if (text == DatabaseAPI.Database.Power[index1].FullName)
                    return;
                //Update the full power name in the powerset array
                if (newPower.PowerSetID > -1)
                    DatabaseAPI.Database.Powersets[newPower.PowerSetID].Powers[newPower.PowerSetIndex].FullName =
                        newPower.FullName;

                var num2 = DatabaseAPI.Database.Power[index1].Effects.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                    DatabaseAPI.Database.Power[index1].Effects[index2].PowerFullName = DatabaseAPI.Database.Power[index1].FullName;
                var strArray =
                    DatabaseAPI.UidReferencingPowerFix(text, DatabaseAPI.Database.Power[index1].FullName);
                var str1 = "";
                var num3 = strArray.Length - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                    str1 = str1 + strArray[index2] + "\r\n";
                if (strArray.Length > 0)
                {
                    var str2 = "Power: " + text + " changed to " + DatabaseAPI.Database.Power[index1].FullName +
                               "\r\nThe following powers referenced this power and were updated:\r\n" + str1 +
                               "\r\n\r\nThis list has been placed on the clipboard.";
                    Clipboard.SetDataObject(str2, true);
                    MessageBox.Show(str2);
                }
            }
        }

        private void btnEditPowerData3_Click(object sender, EventArgs e)
        {
            var enh = myEnh;
            var power = enh.GetPower();
            using var frmEditPower = new frmEditPower(power);
            if (frmEditPower.ShowDialog() != DialogResult.OK)
                return;
            power = new Power(frmEditPower.myPower) { IsModified = true };
            // could really use structural equality here, but since we don't have it... we'll mark it as modified just because :/
            var num = power.Effects.Length - 1;
            for (var index = 0; index <= num; ++index)
                power.Effects[index].PowerFullName = power.FullName;
            myEnh.SetPower(power);
        }

        private void btnImage_Click(object sender, EventArgs e)
        {
            if (Loading)
                return;
            ImagePicker.InitialDirectory = I9Gfx.GetDbEnhancementsPath();
            ImagePicker.FileName = myEnh.Image;
            if (ImagePicker.ShowDialog(this) != DialogResult.OK) return;

            var imageFile = FileIO.StripPath(ImagePicker.FileName);
            if (!File.Exists(Path.Combine(I9Gfx.GetDbEnhancementsPath(), imageFile)) && !File.Exists(Path.Combine(I9Gfx.GetEnhancementsPath(), imageFile)))
            {
                MessageBox.Show($@"You must select an image from either the {I9Gfx.GetEnhancementsPath()} or the {I9Gfx.GetDbEnhancementsPath()} folder!\r\n\r\nIf you are adding a new image, you should copy it to the appropriate folder and then select it.", @"Select Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                myEnh.Image = imageFile;
                DisplayIcon();
                SetTypeIcons();
            }
        }

        private void btnNoImage_Click(object sender, EventArgs e)
        {
            myEnh.Image = "";
            SetTypeIcons();
            DisplayIcon();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            var sEffectArray = new Enums.sEffect[myEnh.Effect.Length - 1 + 1];
            var selectedIndex = lstSelected.SelectedIndex;
            var index1 = 0;
            var num1 = myEnh.Effect.Length - 1;
            for (var index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                sEffectArray[index1].Assign(myEnh.Effect[index2]);
                ++index1;
            }

            myEnh.Effect = new Enums.sEffect[myEnh.Effect.Length - 2 + 1];
            var num2 = myEnh.Effect.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
                myEnh.Effect[index2].Assign(sEffectArray[index2]);
            FillEffectList();
            ListSelectedEffects();
            if (lstSelected.Items.Count > selectedIndex)
                lstSelected.SelectedIndex = selectedIndex;
            else if (lstSelected.Items.Count == selectedIndex)
                lstSelected.SelectedIndex = selectedIndex - 1;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lstSelected.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            var sEffectArray = new Enums.sEffect[2];
            sEffectArray[0].Assign(myEnh.Effect[selectedIndex]);
            sEffectArray[1].Assign(myEnh.Effect[selectedIndex - 1]);
            myEnh.Effect[selectedIndex - 1].Assign(sEffectArray[0]);
            myEnh.Effect[selectedIndex].Assign(sEffectArray[1]);
            FillEffectList();
            ListSelectedEffects();
            lstSelected.SelectedIndex = selectedIndex - 1;
        }

        private void cbMutEx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.MutExID = (Enums.eEnhMutex)cbMutEx.SelectedIndex;
        }

        private void cbRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbRecipe.SelectedIndex > 0)
            {
                myEnh.RecipeName = cbRecipe.Text;
                myEnh.RecipeIDX = RecipeCbToIndex();
            }
            else
            {
                myEnh.RecipeName = "";
                myEnh.RecipeIDX = -1;
            }
        }

        private void cbSched_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            var selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode == Enums.eEffMode.Enhancement)
                myEnh.Effect[selectedIndex].Schedule = (Enums.eSchedule)cbSched.SelectedIndex;
        }

        private void cbSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.nIDSet = cbSet.SelectedIndex - 1;
            myEnh.UIDSet = myEnh.nIDSet <= -1 ? string.Empty : DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Uid;
            UpdateTitle();
            DisplaySetImage();
        }

        private void cbSubType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cb && cb.SelectedIndex != -1)
            {
                var selectedItem = cb.SelectedText;
                if (selectedItem != "None")
                {
                    var subType = DatabaseAPI.GetSpecialEnhByName(selectedItem).Index;
                    myEnh.SubTypeID = subType;
                }
            }
            else
            {
                myEnh.SubTypeID = cbSubType.SelectedIndex;
            }
        }

        private void chkSuperior_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.Superior = chkSuperior.Checked;
            if (txtInternal.Text.Contains("Attuned"))
            {
                myEnh.LevelMin = 0;
                myEnh.LevelMax = 0;
                udMinLevel.Minimum = 1;
                udMinLevel.Maximum = 53;
                udMinLevel.Value = 1;
                udMaxLevel.Minimum = 1;
                udMaxLevel.Maximum = 53;
                udMaxLevel.Value = 1;
            }
            else
            {
                myEnh.LevelMin = 9;
                myEnh.LevelMax = 49;
                udMinLevel.Minimum = 1;
                udMinLevel.Maximum = 53;
                udMinLevel.Value = 10;
                udMaxLevel.Minimum = 1;
                udMaxLevel.Maximum = 53;
                udMaxLevel.Value = 50;
            }

            chkUnique.Checked = true;
        }

        private void chkUnique_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            myEnh.Unique = chkUnique.Checked;
        }

        private void chkProc_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            myEnh.IsProc = chkProc.Checked;
        }

        private void chkScalable_CheckChanged(object sender, EventArgs e)
        {
            if (Loading) return;
            myEnh.IsScalable = chkScalable.Checked;
        }

        private void DisplayAll()
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
            chkProc.Checked = myEnh.IsProc;
            chkScalable.Checked = myEnh.IsScalable;
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
                    cbRecipe.SelectedIndex = RecipeIndexToCbSel(myEnh);
                    cbRecipe.Enabled = true;
                    break;
                case Enums.eType.SpecialO:
                    typeHO.Checked = true;
                    cbSubType.SelectedIndex = myEnh.SubTypeID;
                    cbSubType.Enabled = true;
                    cbRecipe.Enabled = false;
                    cbRecipe.SelectedIndex = 0;
                    break;
                case Enums.eType.SetO:
                    cbSubType.SelectedIndex = -1;
                    cbSubType.Enabled = false;
                    typeSet.Checked = true;
                    cbRecipe.SelectedIndex = RecipeIndexToCbSel(myEnh);
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

        private void DisplayEnhanceData()
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
                var selectedIndex = lstSelected.SelectedIndex;
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
                            txtModOther.Text = Convert.ToString(myEnh.Effect[selectedIndex].Multiplier,
                                CultureInfo.InvariantCulture);
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

        private void DisplayIcon()
        {
            if (!string.IsNullOrWhiteSpace(myEnh.Image))
            {
                var img = myEnh.Image;
                var path = Path.Combine(File.Exists(Path.Combine(I9Gfx.GetEnhancementsPath(), img)) ? I9Gfx.GetEnhancementsPath() : I9Gfx.GetDbEnhancementsPath(), img);
                using var extendedBitmap1 = new ExtendedBitmap(path);
                using var extendedBitmap2 = new ExtendedBitmap(30, 30);
                extendedBitmap2.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade(myEnh.TypeID)), GraphicsUnit.Pixel);
                extendedBitmap2.Graphics.DrawImage(extendedBitmap1.Bitmap, extendedBitmap2.ClipRect, extendedBitmap2.ClipRect, GraphicsUnit.Pixel);
                btnImage.Image = new Bitmap(extendedBitmap2.Bitmap);
                btnImage.Text = myEnh.Image;
            }
            else
            {
                btnImage.Image = myEnh.TypeID switch
                {
                    Enums.eType.Normal => typeRegular.Image,
                    Enums.eType.InventO => typeIO.Image,
                    Enums.eType.SpecialO => typeHO.Image,
                    Enums.eType.SetO => typeSet.Image,
                    _ => btnImage.Image
                };
                btnImage.Text = @"Select Image";
            }
        }

        private void DisplaySet()
        {
            gbSet.Enabled = myEnh.TypeID == Enums.eType.SetO;
            gbFlags.Enabled = myEnh.TypeID == Enums.eType.SetO;
            cbSet.SelectedIndex = myEnh.nIDSet + 1;
            DisplaySetImage();
        }

        private void DisplaySetImage()
        {
            if (myEnh.nIDSet > -1)
            {
                myEnh.Image = DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Image;
                DisplayIcon();
                SetTypeIcons();
                if (!string.IsNullOrWhiteSpace(DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Image))
                {
                    var img = DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].Image;
                    var path = Path.Combine(File.Exists(Path.Combine(I9Gfx.GetEnhancementsPath(), img)) ? I9Gfx.GetEnhancementsPath() : I9Gfx.GetDbEnhancementsPath(), img);
                    using var extendedBitmap1 = new ExtendedBitmap(path);
                    using var extendedBitmap2 = new ExtendedBitmap(30, 30);
                    extendedBitmap2.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap2.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), GraphicsUnit.Pixel);
                    extendedBitmap2.Graphics.DrawImage(extendedBitmap1.Bitmap, extendedBitmap2.ClipRect, extendedBitmap2.ClipRect, GraphicsUnit.Pixel);
                }
                else
                {
                    using var extendedBitmap = new ExtendedBitmap(30, 30);
                    extendedBitmap.Graphics.DrawImage(I9Gfx.Borders.Bitmap, extendedBitmap.ClipRect, I9Gfx.GetOverlayRect(Origin.Grade.SetO), GraphicsUnit.Pixel);
                }
            }
        }

        private void DrawClasses()
        {
            bxClass = new ExtendedBitmap(pnlClass.Width, pnlClass.Height);
            var enhPadding1 = EnhPadding;
            var enhPadding2 = EnhPadding;
            var num1 = 0;
            using var solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxClass.Graphics.FillRectangle(solidBrush, bxClass.ClipRect);
            var num2 = myEnh.ClassID.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var destRect = new Rectangle(enhPadding2, enhPadding1, ClassSize, ClassSize);
                bxClass.Graphics.DrawImage(I9Gfx.Classes.Bitmap, destRect, I9Gfx.GetImageRect(myEnh.ClassID[index]),
                    GraphicsUnit.Pixel);
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

        private void DrawClassList()
        {
            bxClassList = new ExtendedBitmap(pnlClassList.Width, pnlClassList.Height);
            var enhPadding1 = EnhPadding;
            var enhPadding2 = EnhPadding;
            var num1 = 0;
            using var solidBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            bxClassList.Graphics.FillRectangle(solidBrush, bxClassList.ClipRect);
            for (var index = 0; index < DatabaseAPI.Database.EnhancementClasses.Length; index++)
            {
                var destRect = new Rectangle(enhPadding2, enhPadding1, 30, 30);
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

        private void EditClick()
        {
            var flag = true;
            var num1 = -1;
            if (lstSelected.SelectedIndex <= -1)
                return;
            var selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode == Enums.eEffMode.Enhancement)
            {
                if (myEnh.Effect[selectedIndex].Enhance.ID == 12)
                {
                    var subId = myEnh.Effect[selectedIndex].Enhance.SubID;
                    num1 = MezPicker(subId);
                    if (num1 == subId)
                        return;
                    var num2 = myEnh.Effect.Length - 1;
                    for (var index1 = 0; index1 <= num2; ++index1)
                    {
                        var effect = myEnh.Effect;
                        var index2 = index1;
                        if ((effect[index2].Mode == Enums.eEffMode.Enhancement) & (effect[index2].Enhance.SubID == num1))
                        {
                            flag = false;
                        }
                    }
                }

                if (!flag)
                {
                    MessageBox.Show(@"This effect has already been added!", @"There can be only one.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                myEnh.Effect[selectedIndex].Enhance.SubID = num1;
            }
            else
            {
                using var frmPowerEffect = new frmPowerEffect(myEnh.Effect[selectedIndex].FX);
                if (frmPowerEffect.ShowDialog() == DialogResult.OK)
                {
                    var effect = myEnh.Effect;
                    var index = selectedIndex;
                    effect[index].Mode = Enums.eEffMode.FX;
                    effect[index].Enhance.ID = -1;
                    effect[index].Enhance.SubID = -1;
                    effect[index].Multiplier = 1f;
                    effect[index].Schedule = Enums.eSchedule.A;
                    effect[index].FX = (IEffect)frmPowerEffect.MyFx.Clone();
                }
            }

            ListSelectedEffects();
            lstSelected.SelectedIndex = selectedIndex;
        }

        private void EffectList_Add()
        {
            if (lstAvailable.SelectedIndex <= -1) return;

            var eEnhance = Enums.eEnhance.None;
            var flag = true;
            var tSub = -1;
            var integer = (Enums.eEnhance)Convert.ToInt32(
                Enum.Parse(eEnhance.GetType(), lstAvailable.Items[lstAvailable.SelectedIndex].ToString()),
                CultureInfo.InvariantCulture);
            if (integer == Enums.eEnhance.Mez)
            {
                tSub = MezPicker();
                var num = myEnh.Effect.Length - 1;
                for (var index1 = 0; index1 <= num; ++index1)
                {
                    var effect = myEnh.Effect;
                    var index2 = index1;
                    if ((effect[index2].Mode == Enums.eEffMode.Enhancement) & (effect[index2].Enhance.SubID == tSub))
                        flag = false;
                }
            }

            if (!flag)
            {
                MessageBox.Show(@"This effect has already been added!", @"There can be only one.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                IEnhancement enh = myEnh;

                var effects = enh.Effect.ToList();
                effects.Add(new Enums.sEffect
                {
                    Mode = Enums.eEffMode.Enhancement,
                    Enhance = new Enums.sTwinID { ID = (int)integer, SubID = tSub },
                    Multiplier = 1f,
                    Schedule = Enhancement.GetSchedule(integer, tSub)
                });
                enh.Effect = effects.ToArray();

                FillEffectList();
                ListSelectedEffects();
                lstSelected.SelectedIndex = lstSelected.Items.Count - 1;
            }
        }

        private void FillEffectList()
        {
            var eEnhance1 = Enums.eEnhance.None;
            lstAvailable.BeginUpdate();
            lstAvailable.Items.Clear();
            var names = Enum.GetNames(eEnhance1.GetType());
            var num1 = names.Length - 1;
            for (var index1 = 1; index1 <= num1; ++index1)
            {
                var eEnhance2 = (Enums.eEnhance)index1;
                var flag = false;
                var num2 = myEnh.Effect.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                    if (myEnh.Effect[index2].Mode == Enums.eEffMode.Enhancement &&
                        (myEnh.Effect[index2].Enhance.ID == index1) & (eEnhance2 != Enums.eEnhance.Mez))
                        flag = true;
                if (!flag)
                    lstAvailable.Items.Add(names[index1]);
            }

            btnAdd.Enabled = lstAvailable.Items.Count > 0;
            lstAvailable.EndUpdate();
        }

        private void FillMutExList()
        {
            var names = Enum.GetNames(Enums.eEnhMutex.None.GetType());
            cbMutEx.BeginUpdate();
            cbMutEx.Items.Clear();
            cbMutEx.Items.AddRange(names);
            cbMutEx.EndUpdate();
        }

        private void FillRecipeList()
        {
            cbRecipe.BeginUpdate();
            cbRecipe.Items.Clear();
            cbRecipe.Items.Add("None");
            var num = DatabaseAPI.Database.Recipes.Length;
            for (var index = 0; index < num; index++)
            {
                if (DatabaseAPI.Database.Recipes[index].IsHidden |
                    DatabaseAPI.Database.Recipes[index].IsVirtual) continue;
                cbRecipe.Items.Add(DatabaseAPI.Database.Recipes[index].InternalName);
            }

            cbRecipe.EndUpdate();
        }

        private void FillSchedules()
        {
            cbSched.BeginUpdate();
            cbSched.Items.Clear();
            cbSched.Items.Add($"A ({Convert.ToDecimal(DatabaseAPI.Database.MultSO[0][0] * 100f):0.##}%)");
            cbSched.Items.Add($"B ({Convert.ToDecimal(DatabaseAPI.Database.MultSO[0][1] * 100f):0.##}%)");
            cbSched.Items.Add($"C ({Convert.ToDecimal(DatabaseAPI.Database.MultSO[0][2] * 100f):0.##}%)");
            cbSched.Items.Add($"D ({Convert.ToDecimal(DatabaseAPI.Database.MultSO[0][3] * 100f):0.##}%)");
            cbSched.EndUpdate();
        }

        private void FillSetList()
        {
            cbSet.BeginUpdate();
            cbSet.Items.Clear();
            cbSet.Items.Add("None");
            var num = DatabaseAPI.Database.EnhancementSets.Count - 1;
            for (var index = 0; index <= num; ++index)
                cbSet.Items.Add(DatabaseAPI.Database.EnhancementSets[index].Uid);
            cbSet.EndUpdate();
        }

        private void FillSubTypeList()
        {
            var specEnhancements = DatabaseAPI.Database.SpecialEnhancements.Select(specEnh => specEnh.Name.Replace(" Origin", string.Empty)).ToList();
            cbSubType.BeginUpdate();
            cbSubType.Items.Clear();
            cbSubType.Items.AddRange(specEnhancements.ToArray<object>());
            cbSubType.EndUpdate();
        }

        private void frmEnhData_Load(object sender, EventArgs e)
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
        private void ListSelectedEffects()
        {
            var eEnhance = Enums.eEnhance.None;
            var eMez = Enums.eMez.None;
            var names1 = Enum.GetNames(eEnhance.GetType());
            var names2 = Enum.GetNames(eMez.GetType());
            lstSelected.BeginUpdate();
            lstSelected.Items.Clear();
            var num = myEnh.Effect.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (myEnh.Effect[index].Mode == Enums.eEffMode.Enhancement)
                {
                    var str = names1[myEnh.Effect[index].Enhance.ID];
                    if (myEnh.Effect[index].Enhance.SubID > -1)
                        str = str + ":" + names2[myEnh.Effect[index].Enhance.SubID];
                    lstSelected.Items.Add(str);
                }
                else
                {
                    lstSelected.Items.Add("Special: " + myEnh.Effect[index].FX.BuildEffectString());
                }

            lstSelected.EndUpdate();
        }

        private void lstAvailable_DoubleClick(object sender, EventArgs e)
        {
            EffectList_Add();
        }

        private void lstSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayEnhanceData();
            tTip.SetToolTip(lstSelected, Convert.ToString(lstSelected.SelectedItem, CultureInfo.InvariantCulture));
        }

        private static int MezPicker(int index = 1)
        {
            return frmEnhMiniPick.MezPicker(index);
        }

        private void PickerExpand()
        {
            if (!EnhClassesSelectorOpen)
            {
                btnEditPowerData.Visible = false;
                gbClass.Width = 272;
                gbClass.Left -= 188;
                lblClass.Width = 256;
                pnlClassList.Refresh();
                EnhClassesSelectorOpen = true;
            }
            else
            {
                btnEditPowerData.Visible = true;
                gbClass.Width = 84;
                gbClass.Left = 596;
                lblClass.Width = pnlClass.Width;
                EnhClassesSelectorOpen = false;
            }
        }

        private void pnlClass_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PickerExpand();
            }
            else
            {
                if (gbClass.Width <= 84 || Loading)
                    return;
                var num1 = -1;
                var num2 = -1;
                var num3 = 0;
                do
                {
                    if ((e.X > (EnhPadding + ClassSize) * num3) & (e.X < (EnhPadding + ClassSize) * (num3 + 1)))
                        num1 = num3;
                    ++num3;
                } while (num3 <= 1);

                var num4 = 0;
                do
                {
                    if ((e.Y > (EnhPadding + ClassSize) * num4) & (e.Y < (EnhPadding + ClassSize) * (num4 + 1)))
                        num2 = num4;
                    ++num4;
                } while (num4 <= 10);

                var num5 = num1 + num2 * 2;
                if (!((num5 < myEnh.ClassID.Length) & (num1 > -1) & (num2 > -1)))
                    return;
                var numArray = new int[myEnh.ClassID.Length - 1 + 1];
                var num6 = myEnh.ClassID.Length - 1;
                for (var index = 0; index <= num6; ++index)
                    numArray[index] = myEnh.ClassID[index];
                var index1 = 0;
                myEnh.ClassID = new int[myEnh.ClassID.Length - 2 + 1];
                var num7 = numArray.Length - 1;
                for (var index2 = 0; index2 <= num7; ++index2)
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

        private void pnlClass_MouseMove(object sender, MouseEventArgs e)
        {
            var num1 = -1;
            var num2 = -1;
            var num3 = 0;
            do
            {
                if ((e.X > (EnhPadding + ClassSize) * num3) & (e.X < (EnhPadding + ClassSize) * (num3 + 1)))
                    num1 = num3;
                ++num3;
            } while (num3 <= 1);

            var num4 = 0;
            do
            {
                if ((e.Y > (EnhPadding + ClassSize) * num4) & (e.Y < (EnhPadding + ClassSize) * (num4 + 1)))
                    num2 = num4;
                ++num4;
            } while (num4 <= 10);

            var index = num1 + num2 * 2;
            if ((index < myEnh.ClassID.Length) & (num1 > -1) & (num2 > -1))
                lblClass.Text = gbClass.Width < 100
                    ? DatabaseAPI.Database.EnhancementClasses[myEnh.ClassID[index]].ShortName
                    : DatabaseAPI.Database.EnhancementClasses[myEnh.ClassID[index]].Name;
            else
                lblClass.Text = "";
        }

        private void pnlClass_Paint(object sender, PaintEventArgs e)
        {
            if (bxClass == null)
                return;
            e.Graphics.DrawImageUnscaled(bxClass.Bitmap, 0, 0);
        }

        private void pnlClassList_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PickerExpand();
            }
            else
            {
                if (gbClass.Width <= 84 || Loading)
                    return;
                var num1 = -1;
                var num2 = -1;
                var num3 = EnhAcross - 1;
                for (var index = 0; index <= num3; ++index)
                    if ((e.X > (EnhPadding + 30) * index) & (e.X < (EnhPadding + 30) * (index + 1)))
                        num1 = index;
                var num4 = 0;
                do
                {
                    if ((e.Y > (EnhPadding + 30) * num4) & (e.Y < (EnhPadding + 30) * (num4 + 1)))
                        num2 = num4;
                    ++num4;
                } while (num4 <= 10);

                var num5 = num1 + num2 * EnhAcross;
                if (!((num5 < DatabaseAPI.Database.EnhancementClasses.Length) & (num1 > -1) & (num2 > -1)))
                    return;
                {
                    var flag = false;
                    var num6 = myEnh.ClassID.Length - 1;
                    for (var index = 0; index <= num6; ++index)
                        if (myEnh.ClassID[index] == num5)
                            flag = true;

                    if (flag)
                        return;

                    var classIdList = myEnh.ClassID.ToList();
                    classIdList.Add(num5);
                    myEnh.ClassID = classIdList.ToArray();
                    Array.Sort(myEnh.ClassID);
                    DrawClasses();
                }
            }
        }

        private void pnlClassList_MouseMove(object sender, MouseEventArgs e)
        {
            var num1 = -1;
            var num2 = -1;
            var num3 = EnhAcross - 1;
            for (var index = 0; index <= num3; ++index)
                if ((e.X > (EnhPadding + 30) * index) & (e.X < (EnhPadding + 30) * (index + 1)))
                    num1 = index;
            var num4 = 0;
            do
            {
                if ((e.Y > (EnhPadding + 30) * num4) & (e.Y < (EnhPadding + 30) * (num4 + 1)))
                    num2 = num4;
                ++num4;
            } while (num4 <= 10);

            var index1 = num1 + num2 * EnhAcross;
            if ((index1 < DatabaseAPI.Database.EnhancementClasses.Length) & (num1 > -1) & (num2 > -1))
                lblClass.Text = DatabaseAPI.Database.EnhancementClasses[index1].Name;
            else
                lblClass.Text = string.Empty;
        }

        private void pnlClassList_Paint(object sender, PaintEventArgs e)
        {
            if (bxClassList == null)
                return;
            e.Graphics.DrawImageUnscaled(bxClassList.Bitmap, 0, 0);
        }

        private void rbBuffDebuff_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading || lstSelected.SelectedIndex <= -1)
                return;
            var selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode != Enums.eEffMode.Enhancement)
                return;
            if (rbBuff.Checked)
                myEnh.Effect[selectedIndex].BuffMode = Enums.eBuffDebuff.BuffOnly;
            else if (rbDebuff.Checked)
                myEnh.Effect[selectedIndex].BuffMode = Enums.eBuffDebuff.DeBuffOnly;
            else if (rbBoth.Checked)
                myEnh.Effect[selectedIndex].BuffMode = Enums.eBuffDebuff.Any;
        }

        private void rbMod_CheckedChanged(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            var selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode != Enums.eEffMode.Enhancement)
                return;
            txtModOther.Enabled = false;
            if (rbModOther.Checked)
            {
                txtModOther.Enabled = true;
                txtModOther.Text = $"{myEnh.Effect[selectedIndex].Multiplier}";
                myEnh.Effect[selectedIndex].Multiplier = Convert.ToSingle(txtModOther.Text);
                txtModOther.SelectAll();
                txtModOther.Select();
            }
            else if (rbMod1.Checked)
            {
                myEnh.Effect[selectedIndex].Multiplier = 1f;
            }
            else if (rbMod2.Checked)
            {
                myEnh.Effect[selectedIndex].Multiplier = 0.625f;
            }
            else if (rbMod3.Checked)
            {
                myEnh.Effect[selectedIndex].Multiplier = 0.5f;
            }
            else if (rbMod4.Checked)
            {
                myEnh.Effect[selectedIndex].Multiplier = 7f / 16f;
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

        private void SetTypeIcons()
        {
            var img = myEnh.Image;
            var path = Path.Combine(File.Exists(Path.Combine(I9Gfx.GetEnhancementsPath(), img)) ? I9Gfx.GetEnhancementsPath() : I9Gfx.GetDbEnhancementsPath(), img);
            using var extendedBitmap1 = new ExtendedBitmap(30, 30);
            using var extendedBitmap2 = string.IsNullOrWhiteSpace(img)
                ? new ExtendedBitmap(30, 30)
                : new ExtendedBitmap(path);
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

        private void StaticIndex_TextChanged(object sender, EventArgs e)
        {
            myEnh.StaticIndex = Convert.ToInt32(StaticIndex.Text, CultureInfo.InvariantCulture);
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.Desc = txtDesc.Text;
        }

        private void txtInternal_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.UID = txtInternal.Text;
        }

        private void txtModOther_TextChanged(object sender, EventArgs e)
        {
            if (lstSelected.SelectedIndex <= -1)
                return;
            var selectedIndex = lstSelected.SelectedIndex;
            if (myEnh.Effect[selectedIndex].Mode == Enums.eEffMode.Enhancement && rbModOther.Checked)
                myEnh.Effect[selectedIndex].Multiplier = Convert.ToSingle(txtModOther.Text);
        }

        private void txtNameFull_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.Name = txtNameFull.Text;
            UpdateTitle();
        }

        private void txtNameShort_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.ShortName = txtNameShort.Text;
        }

        private void txtProb_Leave(object sender, EventArgs e)
        {
            if (Loading)
                return;
            txtProb.Text = Convert.ToString(myEnh.EffectChance, CultureInfo.InvariantCulture);
        }

        private void txtProb_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            var num = Convert.ToSingle(txtProb.Text);
            if (num > 1.0)
                num = 1f;
            if (num < 0.0)
                num = 0.0f;
            myEnh.EffectChance = num;
        }

        private void type_CheckedChanged(object sender, EventArgs e)
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

        private void udMaxLevel_Leave(object sender, EventArgs e)
        {
            SetMaxLevel(Convert.ToInt32(udMaxLevel.Text));
            myEnh.LevelMax = Convert.ToInt32(udMaxLevel.Value) - 1;
        }

        private void udMaxLevel_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.LevelMax = Convert.ToInt32(udMaxLevel.Value) - 1;
            udMinLevel.Maximum = udMaxLevel.Value;
        }

        private void udMinLevel_Leave(object sender, EventArgs e)
        {
            SetMinLevel(Convert.ToInt32(udMinLevel.Text));
            myEnh.LevelMin = Convert.ToInt32(udMinLevel.Value) - 1;
        }

        private void udMinLevel_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            myEnh.LevelMin = Convert.ToInt32(udMinLevel.Value) - 1;
            udMaxLevel.Minimum = udMinLevel.Value;
        }

        private void UpdateTitle()
        {
            var str1 = "Edit ";
            var str2 = myEnh.TypeID switch
            {
                Enums.eType.InventO => str1 + "Invention: ",
                Enums.eType.SpecialO => str1 + "HO: ",
                Enums.eType.SetO => myEnh.nIDSet > -1
                    ? str1 + DatabaseAPI.Database.EnhancementSets[myEnh.nIDSet].DisplayName + ": "
                    : str1 + "Set Invention: ",
                _ => str1 + "Enhancement: "
            };
            Text = str2 + myEnh.Name;
        }
    }
}