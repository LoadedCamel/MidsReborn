
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using midsControls;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmRecipeViewer : Form
    {

        ImageButton ibClipboard;
        ImageButton ibClose;
        ImageButton ibMiniList;
        ImageButton ibTopmost;
        ctlPopUp RecipeInfo;
        readonly ExtendedBitmap bxRecipe;

        bool Loading;
        readonly frmMain myParent;
        int nonRecipeCount;

        public frmRecipeViewer(frmMain iParent)
        {
            FormClosed += frmRecipeViewer_FormClosed;
            Load += frmRecipeViewer_Load;
            Loading = true;
            InitializeComponent();
            Name = nameof(frmRecipeViewer);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmRecipeViewer));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            RecipeInfo.MouseWheel += RecipeInfo_MouseWheel;
            RecipeInfo.MouseEnter += RecipeInfo_MouseEnter;
            lvPower.MouseEnter += lvPower_MouseEnter;
            lvPower.ItemChecked += lvPower_ItemChecked;
            lvDPA.SelectedIndexChanged += lvDPA_SelectedIndexChanged;
            lvDPA.MouseEnter += lvDPA_MouseEnter;
            VScrollBar1.Scroll += VScrollBar1_Scroll;
            chkRecipe.CheckedChanged += chkRecipe_CheckedChanged;
            chkSortByLevel.CheckedChanged += chkSortByLevel_CheckedChanged;
            ibClipboard.ButtonClicked += ibClipboard_ButtonClicked;
            ibClose.ButtonClicked += ibClose_ButtonClicked;
            ibMiniList.ButtonClicked += ibMiniList_ButtonClicked;
            ibTopmost.ButtonClicked += ibTopmost_ButtonClicked;
            myParent = iParent;
            bxRecipe = new ExtendedBitmap(I9Gfx.GetRecipeName());
        }

        void AddToImageList(int eIDX)

        {
            Size imageSize = ilSets.ImageSize;
            int width = imageSize.Width;
            imageSize = ilSets.ImageSize;
            int height = imageSize.Height;
            ExtendedBitmap extendedBitmap = new ExtendedBitmap(width, height);
            IEnhancement enhancement = DatabaseAPI.Database.Enhancements[eIDX];
            if (enhancement.ImageIdx > -1)
            {
                extendedBitmap.Graphics.Clear(Color.White);
                Graphics graphics = extendedBitmap.Graphics;
                I9Gfx.DrawEnhancement(ref graphics, enhancement.ImageIdx, Origin.Grade.IO);
                ilSets.Images.Add(extendedBitmap.Bitmap);
            }
            else
                ilSets.Images.Add(new Bitmap(ilSets.ImageSize.Width, ilSets.ImageSize.Height));
        }

        PopUp.PopupData BuildList(bool Mini)

        {
            int iIndent = 1;
            PopUp.PopupData popupData = new PopUp.PopupData();
            CountingList[] tl = new CountingList[0];
            if (lvDPA.SelectedIndices.Count < 1)
                return popupData;
            if (lvDPA.SelectedIndices[0] == 0)
            {
                int[] numArray1 = new int[DatabaseAPI.Database.Salvage.Length - 1 + 1];
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                DrawIcon(-1);
                int[][] numArray2 = new int[DatabaseAPI.Database.Recipes.Length - 1 + 1][];
                int num5 = numArray2.Length - 1;
                for (int index = 0; index <= num5; ++index)
                {
                    int[] numArray3 = new int[DatabaseAPI.Database.Recipes[index].Item.Length - 1 + 1];
                    numArray2[index] = numArray3;
                }
                int num6 = lvDPA.Items.Count - 1;
                for (int index1 = 1; index1 <= num6; ++index1)
                {
                    int rIDX = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.Items[index1].Tag)].RecipeIDX;
                    if (lvDPA.Items[index1].SubItems[1].Text == "*")
                    {
                        rIDX = -1;
                        putInList(ref tl, lvDPA.Items[index1].Text);
                    }

                    if (rIDX <= -1)
                        continue;
                    int iLevel = Convert.ToInt32(lvDPA.Items[index1].SubItems[1].Text) - 1;
                    int itemId = FindItemID(rIDX, iLevel);
                    if (itemId <= -1)
                        continue;
                    if (chkRecipe.Checked)
                        ++numArray2[rIDX][itemId];
                    Recipe.RecipeEntry recipeEntry = DatabaseAPI.Database.Recipes[rIDX].Item[itemId];
                    int num7 = recipeEntry.SalvageIdx.Length - 1;
                    for (int index2 = 0; index2 <= num7; ++index2)
                    {
                        if (!(recipeEntry.SalvageIdx[index2] > -1 & recipeEntry.Count[index2] > 0))
                            continue;
                        if (!(index2 != 0 & recipeEntry.SalvageIdx[index2] == recipeEntry.SalvageIdx[0]))
                        {
                            numArray1[recipeEntry.SalvageIdx[index2]] += recipeEntry.Count[index2];
                            num4 += recipeEntry.Count[index2];
                        }
                        else
                            break;
                    }
                    num1 += recipeEntry.CraftCost;
                    if (recipeEntry.CraftCostM > 0)
                        num3 += recipeEntry.CraftCostM;
                    else if (DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.Items[index1].Tag)].TypeID == Enums.eType.SetO)
                        num3 += recipeEntry.CraftCost;
                    num2 += recipeEntry.BuyCost;
                }
                int index3 = popupData.Add();
                if (Mini)
                    iIndent = 0;
                lblHeader.Text = "Shopping List";
                if (lvPower.CheckedIndices.Count == 1)
                {
                    popupData.Sections[index3].Add(
                        !lvPower.Items[0].Checked
                            ? DatabaseAPI.Database
                                .Power[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[Convert.ToInt32(lvPower.CheckedItems[0].Tag)].NIDPower]
                                .DisplayName
                            : "All Powers", PopUp.Colors.Title);
                }
                else
                    popupData.Sections[index3].Add(Convert.ToString(lvPower.CheckedIndices.Count) + " Powers", PopUp.Colors.Title);
                if (!chkRecipe.Checked)
                    popupData.Sections[index3].Add(Convert.ToString(lvDPA.Items.Count - nonRecipeCount) + " Recipes:", PopUp.Colors.Title);
                if (Mini)
                {
                    string str = "Buy:";
                    if (num2 > 0)
                        popupData.Sections[index3].Add(str + " " + Strings.Format(num2, "###,###,##0"), PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }
                else
                {
                    string iText = "Buy Cost:";
                    if (num2 > 0)
                        popupData.Sections[index3].Add(iText, PopUp.Colors.Invention, Strings.Format(num2, "###,###,##0"), PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }
                if (Mini)
                {
                    string str = "Craft:";
                    if (num1 > 0)
                        popupData.Sections[index3].Add(str + " " + Strings.Format(num1, "###,###,##0"), PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }
                else
                {
                    string iText = "Craft Cost:";
                    if (num1 > 0)
                        popupData.Sections[index3].Add(iText, PopUp.Colors.Invention, Strings.Format(num1, "###,###,##0"), PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }
                if (Mini)
                {
                    string str = "Craft (Mem'd):";
                    if (num3 > 0 & num3 != num1)
                        popupData.Sections[index3].Add(str + " " + Strings.Format(num3, "###,###,##0"), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, iIndent);
                }
                else
                {
                    string iText = "Craft Cost (Memorized Common):";
                    if (num3 > 0 & num3 != num1)
                        popupData.Sections[index3].Add(iText, PopUp.Colors.Effect, Strings.Format(num3, "###,###,##0"), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, iIndent);
                }
                if (chkRecipe.Checked)
                {
                    RecipeInfo.ColumnPosition = 0.75f;
                    int index1 = popupData.Add();
                    popupData.Sections[index1].Add(Convert.ToString(lvDPA.Items.Count - nonRecipeCount) + " Recipes:", PopUp.Colors.Title);
                    int num7 = numArray2.Length - 1;
                    for (int index2 = 0; index2 <= num7; ++index2)
                    {
                        int num8 = numArray2[index2].Length - 1;
                        for (int index4 = 0; index4 <= num8; ++index4)
                        {
                            if (numArray2[index2][index4] <= 0)
                                continue;
                            var color = DatabaseAPI.Database.Recipes[index2].Rarity switch
                            {
                                Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                                Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                                Recipe.RecipeRarity.UltraRare => PopUp.Colors.UltraRare,
                                _ => PopUp.Colors.Text
                            };
                            if (Mini)
                                popupData.Sections[index1].Add(" " + Convert.ToString(numArray2[index2][index4]) + " x", color, DatabaseAPI.GetEnhancementNameShortWSet(DatabaseAPI.Database.Recipes[index2].EnhIdx) + " (" + Convert.ToString(DatabaseAPI.Database.Recipes[index2].Item[index4].Level + 1) + ")", color, 0.9f, FontStyle.Bold, iIndent);
                            else
                                popupData.Sections[index1].Add(DatabaseAPI.GetEnhancementNameShortWSet(DatabaseAPI.Database.Recipes[index2].EnhIdx) + " (" + Convert.ToString(DatabaseAPI.Database.Recipes[index2].Item[index4].Level + 1) + ")", color, Convert.ToString(numArray2[index2][index4]), color, 0.9f, FontStyle.Bold, iIndent);
                        }
                    }
                    popupData.Sections[index1].Content = sortPopupStrings(Mini, 2, popupData.Sections[index1].Content);
                }
                else
                    RecipeInfo.ColumnPosition = 0.5f;
                if (Mini)
                {
                    popupData.ColPos = 0.15f;
                    popupData.ColRight = false;
                }
                int index5 = popupData.Add();
                string iText1 = !Mini ? Convert.ToString(num4) + " Salvage Items:" : Convert.ToString(num4) + " Items:";
                popupData.Sections[index5].Add(iText1, PopUp.Colors.Title);
                int num9 = numArray1.Length - 1;
                for (int index1 = 0; index1 <= num9; ++index1)
                {
                    if (numArray1[index1] <= 0)
                        continue;
                    var color = DatabaseAPI.Database.Salvage[index1].Rarity switch
                    {
                        Recipe.RecipeRarity.Common => PopUp.Colors.Common,
                        Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                        Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                        _ => Color.White
                    };
                    if (Mini)
                        popupData.Sections[index5].Add(" " + Convert.ToString(numArray1[index1]) + " x", color, DatabaseAPI.Database.Salvage[index1].ExternalName, color, 0.9f);
                    else
                        popupData.Sections[index5].Add(DatabaseAPI.Database.Salvage[index1].ExternalName, color, Convert.ToString(numArray1[index1]), color, 0.9f, FontStyle.Bold, 1);
                }
                popupData.Sections[index5].Content = sortPopupStrings(Mini, 1, popupData.Sections[index5].Content);
                if (nonRecipeCount == 1)
                    return popupData;
                {
                    int index1 = popupData.Add();
                    string iText2 = !Mini ? Convert.ToString(nonRecipeCount - 1) + " Non-Crafted Enhancements:" : Convert.ToString(nonRecipeCount - 1) + " Enhs:";
                    popupData.Sections[index1].Add(iText2, PopUp.Colors.Title);
                    int num7 = tl.Length - 1;
                    for (int index2 = 0; index2 <= num7; ++index2)
                    {
                        Color common = PopUp.Colors.Common;
                        if (Mini)
                            popupData.Sections[index1].Add(" " + Convert.ToString(tl[index2].Count) + " x", common, tl[index2].Text, common, 0.9f);
                        else
                            popupData.Sections[index1].Add(tl[index2].Text, common, Convert.ToString(tl[index2].Count), common, 0.9f, FontStyle.Bold, 1);
                    }
                    popupData.Sections[index1].Content = sortPopupStrings(Mini, 1, popupData.Sections[index1].Content);
                }
                return popupData;
            }
            lblHeader.Text = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.SelectedItems[0].Tag)].LongName + " (" + lvDPA.SelectedItems[0].SubItems[1].Text + ")";
            int rIdx = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.SelectedItems[0].Tag)].RecipeIDX;
            if (lvDPA.SelectedItems[0].SubItems[1].Text == "*")
                rIdx = -1;
            DrawIcon(Convert.ToInt32(lvDPA.SelectedItems[0].Tag));
            if (rIdx <= -1)
                return popupData;
            {
                int index1 = popupData.Add();
                popupData.Sections[index1] = Character.PopRecipeInfo(rIdx, Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text) - 1);
                if (popupData.Sections[index1].Content != null && popupData.Sections[index1].Content.Length > 0)
                {
                    PopUp.StringValue[] content = popupData.Sections[index1].Content;
                    int index2 = 0;
                    content[index2].Text = content[index2].Text + " (" + lvDPA.SelectedItems[0].SubItems[1].Text + ")";
                    return popupData;
                }
                popupData.Sections[index1].Content[0].Text = "";
            }
            return popupData;
        }

        void ChangedRecipeInfoElements()

        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum = (int)Math.Round(RecipeInfo.lHeight * (VScrollBar1.LargeChange / (double)Panel1.Height));
        }

        void chkRecipe_CheckedChanged(object sender, EventArgs e)

        {
            lvDPA_SelectedIndexChanged(this, new EventArgs());
            MidsContext.Config.ShoppingListIncludesRecipes = chkRecipe.Checked;
        }

        void chkSortByLevel_CheckedChanged(object sender, EventArgs e)

        {
            UpdatePowerList();
        }

        static int colorRarityCompare(Color t1, Color t2)

        {
            int num;
            if (t1.Equals(t2))
            {
                num = 0;
            }
            else
            {
                if (t1.Equals(PopUp.Colors.Common))
                {
                    if (t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare) | t2.Equals(PopUp.Colors.UltraRare))
                        return -1;
                }
                else if (t1.Equals(PopUp.Colors.Uncommon))
                {
                    if (t2.Equals(PopUp.Colors.Common))
                        return 1;
                    if (t2.Equals(PopUp.Colors.Rare) | t2.Equals(PopUp.Colors.UltraRare))
                        return -1;
                }
                else if (t1.Equals(PopUp.Colors.Rare))
                {
                    if (t2.Equals(PopUp.Colors.Common) | t2.Equals(PopUp.Colors.Uncommon))
                        return 1;
                    if (t2.Equals(PopUp.Colors.UltraRare))
                        return -1;
                }
                else if (t1.Equals(PopUp.Colors.UltraRare) && t2.Equals(PopUp.Colors.Common) | t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare))
                    return 1;
                num = -1;
            }
            return num;
        }

        static int colorRarityCompareB(Color t1, Color t2)

        {
            int num;
            if (t1.Equals(t2))
            {
                num = 0;
            }
            else
            {
                if (t1.Equals(PopUp.Colors.Common))
                {
                    if (t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare) | t2.Equals(PopUp.Colors.UltraRare))
                        return -1;
                }
                else if (t1.Equals(PopUp.Colors.Uncommon) | t1.Equals(PopUp.Colors.Rare))
                {
                    if (t2.Equals(PopUp.Colors.Common))
                        return 1;
                    if (t2.Equals(PopUp.Colors.Rare))
                        return 0;
                    if (t2.Equals(PopUp.Colors.UltraRare))
                        return -1;
                }
                else if (t1.Equals(PopUp.Colors.UltraRare) && t2.Equals(PopUp.Colors.Common) | t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare))
                    return 1;
                num = -1;
            }
            return num;
        }

        void DrawIcon(int Index)

        {
            ExtendedBitmap extendedBitmap = new ExtendedBitmap(bxRecipe.Size);
            extendedBitmap.Graphics.Clear(Color.Black);
            extendedBitmap.Graphics.DrawImageUnscaled(bxRecipe.Bitmap, 0, 0);
            if (Index > -1)
                extendedBitmap.Graphics.DrawImageUnscaled(I9Gfx.Enhancements[Index], 0, 0);
            pbRecipe.Image = new Bitmap(extendedBitmap.Bitmap);
        }

        void FillEnhList()

        {
            if (lvPower.CheckedIndices.Count < 1)
            {
                lvDPA.Items.Clear();
            }
            else
            {
                lvDPA.BeginUpdate();
                lvDPA.Items.Clear();
                string[] items = new string[3];
                bool flag = false;
                int num1 = lvPower.CheckedIndices.Count - 1;
                if (lvPower.Items[0].Checked)
                {
                    flag = true;
                    num1 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                }
                ilSets.Images.Clear();
                nonRecipeCount = 1;
                lvDPA.Items.Add(" - All Recipes - ");
                int num2 = num1;
                for (int index1 = 0; index1 <= num2; ++index1)
                {
                    int hIDX = flag ? index1 : Convert.ToInt32(lvPower.CheckedItems[index1].Tag);
                    if (!(MidsContext.Character.CurrentBuild.Powers[hIDX].NIDPowerset > -1 & HasIOs(hIDX)))
                        continue;
                    int num3 = MidsContext.Character.CurrentBuild.Powers[hIDX].Slots.Length - 1;
                    for (int index2 = 0; index2 <= num3; ++index2)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh <= -1)
                            continue;
                        switch (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh].TypeID)
                        {
                            case Enums.eType.SetO:
                            {
                                items[0] = DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh].nIDSet].DisplayName + ": ";
                                string[] strArray1 = items;
                                int num4 = 0;
                                string[] strArray2;
                                IntPtr index3;
                                (strArray2 = strArray1)[(int)(index3 = (IntPtr)num4)] = strArray2[(int)index3] + DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh].Name;
                                items[1] = Convert.ToString(MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.IOLevel + 1);
                                break;
                            }
                            case Enums.eType.InventO:
                                items[0] = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh].Name + " (Common)";
                                items[1] = Convert.ToString(MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.IOLevel + 1);
                                break;
                            default:
                                items[0] = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh].Name;
                                ++nonRecipeCount;
                                items[1] = "*";
                                break;
                        }
                        AddToImageList(MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh);
                        items[2] = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDX].NIDPower].DisplayName;
                        lvDPA.Items.Add(new ListViewItem(items, ilSets.Images.Count - 1));
                        lvDPA.Items[lvDPA.Items.Count - 1].Tag = MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh;
                    }
                }
                lvDPA.EndUpdate();
                if (lvDPA.Items.Count > 0)
                    lvDPA.Items[0].Selected = true;
            }
        }

        void FillPowerList()

        {
            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            lvPower.Sorting = SortOrder.None;
            lvPower.Items.Add(" - All Powers - ");
            lvPower.Items[lvPower.Items.Count - 1].Tag = -1;
            int num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (int hIDX = 0; hIDX <= num; ++hIDX)
            {
                if (!(MidsContext.Character.CurrentBuild.Powers[hIDX].NIDPower > -1 & HasIOs(hIDX)))
                    continue;
                string text = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDX].NIDPower].DisplayName;
                if (chkSortByLevel.Checked)
                    text = Strings.Format(MidsContext.Character.CurrentBuild.Powers[hIDX].Level + 1, "00") + " - " + text;
                lvPower.Items.Add(text);
                lvPower.Items[lvPower.Items.Count - 1].Tag = hIDX;
            }
            lvPower.Sorting = SortOrder.Ascending;
            lvPower.Sort();
            if (lvPower.Items.Count > 0)
            {
                lvPower.Items[0].Selected = true;
                lvPower.Items[0].Checked = true;
            }
            lvPower.EndUpdate();
        }

        static int FindItemID(int rIDX, int iLevel)

        {
            int num1 = -1;
            int num2 = 52;
            int num3 = 0;
            int num4 = DatabaseAPI.Database.Recipes[rIDX].Item.Length - 1;
            for (int index = 0; index <= num4; ++index)
            {
                if (DatabaseAPI.Database.Recipes[rIDX].Item[index].Level > num3)
                    num3 = DatabaseAPI.Database.Recipes[rIDX].Item[index].Level;
                if (DatabaseAPI.Database.Recipes[rIDX].Item[index].Level < num2)
                    num2 = DatabaseAPI.Database.Recipes[rIDX].Item[index].Level;
                if (DatabaseAPI.Database.Recipes[rIDX].Item[index].Level != iLevel)
                    continue;
                num1 = index;
                break;
            }

            if (num1 >= 0)
                return num1 >= 0 ? num1 : -1;
            {
                iLevel = Enhancement.GranularLevelZb(iLevel, 0, 49);
                int num5 = DatabaseAPI.Database.Recipes[rIDX].Item.Length - 1;
                for (int index = 0; index <= num5; ++index)
                {
                    if (DatabaseAPI.Database.Recipes[rIDX].Item[index].Level != iLevel)
                        continue;
                    num1 = index;
                    break;
                }
            }
            return num1 >= 0 ? num1 : -1;
        }

        void frmRecipeViewer_FormClosed(object sender, FormClosedEventArgs e)

        {
            StoreLocation();
            myParent.FloatRecipe(false);
        }

        void frmRecipeViewer_Load(object sender, EventArgs e)

        {
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            RecipeInfo.SetPopup(new PopUp.PopupData());
            ChangedRecipeInfoElements();
            chkRecipe.Checked = MidsContext.Config.ShoppingListIncludesRecipes;
            Loading = false;
        }

        bool HasIOs(int hIDX)

        {
            if (hIDX < 0)
                return false;
            int num = MidsContext.Character.CurrentBuild.Powers[hIDX].Slots.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index].Enhancement.Enh > -1)
                    return true;
            }
            return false;
        }

        void CopyToClipboard()
        {
            string str1 = "";
            PopUp.PopupData popupData = BuildList(true);
            int num1 = RecipeInfo.pData.Sections.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                int num2 = RecipeInfo.pData.Sections[index1].Content.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    PopUp.StringValue[] content = popupData.Sections[index1].Content;
                    int index3 = index2;
                    string str2 = str1 + content[index3].Text;
                    if (content[index3].TextColumn != "")
                        str2 = str2 + "  " + content[index3].TextColumn;
                    str1 = str2 + "\r\n";
                }
                str1 += "\r\n";
            }
            Clipboard.SetDataObject(str1, true);
            int num3 = (int)Interaction.MsgBox("Data copied to clipboard!", MsgBoxStyle.Information, "Done");
        }
        void ibClipboard_ButtonClicked()
        {
            try
            {
                CopyToClipboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Clipboard copy failed:" + ex.Message);
            }
        }

        void ibClose_ButtonClicked()

        {
            Close();
        }

        void ibMiniList_ButtonClicked()

        {
            myParent.SetMiniList(BuildList(true), "Shopping List");
        }

        void ibTopmost_ButtonClicked()

        {
            TopMost = ibTopmost.Checked;
            if (!TopMost)
                return;
            BringToFront();
        }

        [DebuggerStepThrough]

        void lvPower_ItemChecked(object sender, ItemCheckedEventArgs e)

        {
            if (e.Item.Index == 0)
            {
                if (Operators.ConditionalCompareObjectLess(e.Item.Tag, 0, false) && e.Item.Checked)
                {
                    int num = lvPower.Items.Count - 1;
                    for (int index = 1; index <= num; ++index)
                        lvPower.Items[index].Checked = false;
                }
            }
            else if (e.Item.Checked)
                lvPower.Items[0].Checked = false;
            FillEnhList();
        }

        void lvPower_MouseEnter(object sender, EventArgs e)

        {
            lvPower.Focus();
        }

        void lvDPA_MouseEnter(object sender, EventArgs e)

        {
            lvDPA.Focus();
        }

        void lvDPA_SelectedIndexChanged(object sender, EventArgs e)

        {
            RecipeInfo.ScrollY = 0.0f;
            RecipeInfo.SetPopup(BuildList(false));
            ChangedRecipeInfoElements();
        }

        static void putInList(ref CountingList[] tl, string item)

        {
            int num = tl.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (tl[index].Text != item)
                    continue;
                ++tl[index].Count;
                return;
            }
            tl = (CountingList[])Utils.CopyArray(tl, new CountingList[tl.Length + 1]);
            tl[tl.Length - 1].Count = 1;
            tl[tl.Length - 1].Text = item;
        }

        void RecipeInfo_MouseEnter(object sender, EventArgs e)

        {
            VScrollBar1.Focus();
        }

        void RecipeInfo_MouseWheel(object sender, MouseEventArgs e)

        {
            VScrollBar1.Value = Convert.ToInt32(Operators.AddObject(VScrollBar1.Value, Interaction.IIf(e.Delta > 0, -1, 1)));
            if (VScrollBar1.Value > VScrollBar1.Maximum - 9)
                VScrollBar1.Value = VScrollBar1.Maximum - 9;
            VScrollBar1_Scroll(RuntimeHelpers.GetObjectValue(sender), new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        public void SetLocation()
        {
            Rectangle rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmRecipe.X,
                Y = MainModule.MidsController.SzFrmRecipe.Y,
                Width = MainModule.MidsController.SzFrmRecipe.Width,
                Height = MainModule.MidsController.SzFrmRecipe.Height
            };
            if (rectangle.Width < 1)
                rectangle.Width = Width;
            if (rectangle.Height < 1)
                rectangle.Height = Height;
            if (rectangle.Width < MinimumSize.Width)
                rectangle.Width = MinimumSize.Width;
            if (rectangle.Height < MinimumSize.Height)
                rectangle.Height = MinimumSize.Height;
            if (rectangle.X < 1)
                rectangle.X = (int)Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2.0);
            if (rectangle.Y < 32)
                rectangle.Y = (int)Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2.0);
            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
        }

        static PopUp.StringValue[] sortPopupStrings(

          bool Mini,
          int colorSortMode,
          PopUp.StringValue[] inStrs)
        {
            int num1 = 0;
            int[] numArray = new int[inStrs.Length - 1 + 1];
            int num2 = numArray.Length - 1;
            for (int index1 = 0; index1 <= num2; ++index1)
            {
                bool flag = false;
                int num3 = index1 - 1;
                for (int index2 = 0; index2 <= num3; ++index2)
                {
                    flag = true;
                    num1 = colorSortMode switch
                    {
                        1 => colorRarityCompare(inStrs[numArray[index2]].tColor, inStrs[index1].tColor),
                        2 => colorRarityCompareB(inStrs[numArray[index2]].tColor, inStrs[index1].tColor),
                        _ => num1
                    };

                    if ((num1 != 0 || string.CompareOrdinal(
                             Convert.ToString(Interaction.IIf(Mini, inStrs[index1].TextColumn, inStrs[index1].Text)),
                             Convert.ToString(Interaction.IIf(Mini, inStrs[numArray[index2]].TextColumn,
                                 inStrs[numArray[index2]].Text))) >= 0) && num1 <= 0)
                        continue;
                    int num4 = index2;
                    for (int index3 = index1 - 1; index3 >= num4; index3 += -1)
                        numArray[index3 + 1] = numArray[index3];
                    numArray[index2] = index1;
                    flag = false;
                    break;
                }
                if (flag)
                    numArray[index1] = index1;
            }
            PopUp.StringValue[] stringValueArray = new PopUp.StringValue[inStrs.Length - 1 + 1];
            int num5 = inStrs.Length - 1;
            for (int index = 0; index <= num5; ++index)
                stringValueArray[index] = inStrs[numArray[index]];
            return stringValueArray;
        }

        void StoreLocation()

        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.SzFrmRecipe.X = Left;
            MainModule.MidsController.SzFrmRecipe.Y = Top;
            MainModule.MidsController.SzFrmRecipe.Width = Width;
            MainModule.MidsController.SzFrmRecipe.Height = Height;
        }

        public void UpdateData()
        {
            BackColor = myParent.BackColor;
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibClipboard.IA = myParent.Drawing.pImageAttributes;
            ibClipboard.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibClipboard.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibMiniList.IA = myParent.Drawing.pImageAttributes;
            ibMiniList.ImageOff = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[2].Bitmap : myParent.Drawing.bxPower[4].Bitmap;
            ibMiniList.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            FillPowerList();
        }

        void UpdatePowerList()

        {
            if (Loading)
                return;
            FillPowerList();
        }

        void VScrollBar1_Scroll(object sender, ScrollEventArgs e)

        {
            RecipeInfo.ScrollY = (float)(VScrollBar1.Value / (double)(VScrollBar1.Maximum - VScrollBar1.LargeChange) * (RecipeInfo.lHeight - (double)Panel1.Height));
        }

        struct CountingList

        {
            public string Text;
            public int Count;
        }
    }
}