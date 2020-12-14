using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using Microsoft.VisualBasic.CompilerServices;
using midsControls;

namespace Hero_Designer.Forms
{
    public partial class frmRecipeViewer : Form
    {
        #region BuildSavageSummary sub-class
        private static class BuildSalvageSummary
        {
            public static int EnhObtained { get; private set; }
            public static int EnhCatalysts { get; private set; }
            public static int EnhBoosters { get; private set; }
            public static int TotalEnhancements { get; private set; }

            public static void CalcAll()
            {
                TotalEnhancements = 0;
                EnhObtained = 0;
                EnhCatalysts = 0;
                EnhBoosters = 0;

                foreach (var p in MidsContext.Character.CurrentBuild.Powers)
                {
                    for (var j = 0; j < p.Slots.Length; j++)
                    {
                        var enhIdx = p.Slots[j].Enhancement.Enh;

                        if (enhIdx > -1) TotalEnhancements++;
                        if (p.Slots[j].Enhancement.Obtained & enhIdx > -1) EnhObtained++;
                        if (enhIdx == -1) continue;

                        var enhName = Database.Instance.Enhancements[enhIdx].UID;
                        if (DatabaseAPI.EnhHasCatalyst(enhName) && DatabaseAPI.EnhIsSuperior(enhIdx)) EnhCatalysts++;

                        var relativeLevel = p.Slots[j].Enhancement.RelativeLevel;
                        if (DatabaseAPI.EnhIsIO(enhIdx))
                        {
                            EnhBoosters += relativeLevel switch
                            {
                                Enums.eEnhRelative.PlusOne => 1,
                                Enums.eEnhRelative.PlusTwo => 2,
                                Enums.eEnhRelative.PlusThree => 3,
                                Enums.eEnhRelative.PlusFour => 4,
                                Enums.eEnhRelative.PlusFive => 5,
                                _ => 0
                            };
                        }
                    }
                }
            }

            public static void CalcTotalEnhancements()
            {
                TotalEnhancements = 0;
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    for (var j = 0; j < MidsContext.Character.CurrentBuild.Powers[i].Slots.Length; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i].Slots[j].Enhancement.Enh > -1)
                            TotalEnhancements++;
                    }
                }
            }

            public static void CalcEnhObtained()
            {
                EnhObtained = 0;
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    for (var j = 0; j < MidsContext.Character.CurrentBuild.Powers[i].Slots.Length; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i].Slots[j].Enhancement.Obtained &
                            MidsContext.Character.CurrentBuild.Powers[i].Slots[j].Enhancement.Enh > -1)
                        {
                            EnhObtained++;
                        }

                    }
                }
            }

            public static void CalcEnhCatalysts()
            {
                EnhCatalysts = 0;
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    for (var j = 0; j < MidsContext.Character.CurrentBuild.Powers[i].Slots.Length; i++)
                    {
                        var enhIdx = MidsContext.Character.CurrentBuild.Powers[i].Slots[j].Enhancement.Enh;
                        if (enhIdx == -1) continue;
                        var enhName = Database.Instance.Enhancements[enhIdx].UID;

                        if (DatabaseAPI.EnhHasCatalyst(enhName) && DatabaseAPI.EnhIsSuperior(enhIdx)) EnhCatalysts++;
                    }
                }
            }

            public static void CalcEnhBoosters()
            {
                EnhBoosters = 0;
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    for (var j = 0; j < MidsContext.Character.CurrentBuild.Powers[i].Slots.Length; i++)
                    {
                        var enhIdx = MidsContext.Character.CurrentBuild.Powers[i].Slots[j].Enhancement.Enh;
                        if (enhIdx == -1) continue;

                        var relativeLevel = MidsContext.Character.CurrentBuild.Powers[i].Slots[j].Enhancement.RelativeLevel;
                        if (DatabaseAPI.EnhIsIO(enhIdx) &
                            relativeLevel != Enums.eEnhRelative.Even &
                            relativeLevel != Enums.eEnhRelative.None)
                        {
                            EnhBoosters += relativeLevel switch
                            {
                                Enums.eEnhRelative.PlusOne => 1,
                                Enums.eEnhRelative.PlusTwo => 2,
                                Enums.eEnhRelative.PlusThree => 3,
                                Enums.eEnhRelative.PlusFour => 4,
                                Enums.eEnhRelative.PlusFive => 5,
                                _ => 0
                            };
                        }
                    }
                }
            }
        }
        #endregion

        private readonly ExtendedBitmap bxRecipe;
        private readonly frmMain myParent;

        private ImageButton ibClipboard;
        private ImageButton ibClose;
        private ImageButton ibMiniList;
        private ImageButton ibTopmost;

        private bool Loading;
        private int nonRecipeCount;
        private ctlPopUp RecipeInfo;

        public frmRecipeViewer(frmMain iParent)
        {
            FormClosed += frmRecipeViewer_FormClosed;
            Load += frmRecipeViewer_Load;
            Loading = true;
            InitializeComponent();
            Name = nameof(frmRecipeViewer);
            var componentResourceManager = new ComponentResourceManager(typeof(frmRecipeViewer));
            Icon = Resources.reborn;
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
            ibEnhCheckMode.ButtonClicked += ibEnhCheckMode_ButtonClicked;
            myParent = iParent;
            bxRecipe = new ExtendedBitmap(I9Gfx.GetRecipeName());
        }

        public void RecalcSalvage()
        {
            BuildSalvageSummary.CalcAll();
            lblEnhObtained.Text = $"Obtained: {BuildSalvageSummary.EnhObtained}/{BuildSalvageSummary.TotalEnhancements}";
            lblCatalysts.Text = $"x{BuildSalvageSummary.EnhCatalysts}";
            lblBoosters.Text = $"x{BuildSalvageSummary.EnhBoosters}";
        }

        public void UpdateEnhObtained()
        {
            BuildSalvageSummary.CalcEnhObtained();
            lblEnhObtained.Text = $"Obtained: {BuildSalvageSummary.EnhObtained}/{BuildSalvageSummary.TotalEnhancements}";
        }

        private void AddToImageList(int eIDX)
        {
            var imageSize = ilSets.ImageSize;
            var width = imageSize.Width;
            imageSize = ilSets.ImageSize;
            var height = imageSize.Height;
            var extendedBitmap = new ExtendedBitmap(width, height);
            var enhancement = DatabaseAPI.Database.Enhancements[eIDX];
            if (enhancement.ImageIdx > -1)
            {
                extendedBitmap.Graphics.Clear(Color.White);
                var graphics = extendedBitmap.Graphics;
                I9Gfx.DrawEnhancement(ref graphics, enhancement.ImageIdx, Origin.Grade.IO);
                ilSets.Images.Add(extendedBitmap.Bitmap);
            }
            else
            {
                ilSets.Images.Add(new Bitmap(ilSets.ImageSize.Width, ilSets.ImageSize.Height));
            }
        }

        private PopUp.PopupData BuildList(bool Mini)
        {
            var iIndent = 1;
            var popupData = new PopUp.PopupData();
            var tl = new CountingList[0];
            if (lvDPA.SelectedIndices.Count < 1)
                return popupData;
            if (lvDPA.SelectedIndices[0] == 0)
            {
                var numArray1 = new int[DatabaseAPI.Database.Salvage.Length - 1 + 1];
                var num1 = 0;
                var num2 = 0;
                var num3 = 0;
                var num4 = 0;
                DrawIcon(-1);
                var numArray2 = new int[DatabaseAPI.Database.Recipes.Length - 1 + 1][];
                var num5 = numArray2.Length - 1;
                for (var index = 0; index <= num5; ++index)
                {
                    var numArray3 = new int[DatabaseAPI.Database.Recipes[index].Item.Length - 1 + 1];
                    numArray2[index] = numArray3;
                }

                var num6 = lvDPA.Items.Count - 1;
                for (var index1 = 1; index1 <= num6; ++index1)
                {
                    var rIDX = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.Items[index1].Tag)].RecipeIDX;
                    if (lvDPA.Items[index1].SubItems[1].Text == "*")
                    {
                        rIDX = -1;
                        putInList(ref tl, lvDPA.Items[index1].Text);
                    }

                    if (rIDX <= -1)
                        continue;
                    var iLevel = Convert.ToInt32(lvDPA.Items[index1].SubItems[1].Text) - 1;
                    var itemId = FindItemID(rIDX, iLevel);
                    if (itemId <= -1)
                        continue;
                    if (chkRecipe.Checked)
                        ++numArray2[rIDX][itemId];
                    var recipeEntry = DatabaseAPI.Database.Recipes[rIDX].Item[itemId];
                    var num7 = recipeEntry.SalvageIdx.Length - 1;
                    for (var index2 = 0; index2 <= num7; ++index2)
                    {
                        if (!((recipeEntry.SalvageIdx[index2] > -1) & (recipeEntry.Count[index2] > 0)))
                            continue;
                        if (!((index2 != 0) & (recipeEntry.SalvageIdx[index2] == recipeEntry.SalvageIdx[0])))
                        {
                            numArray1[recipeEntry.SalvageIdx[index2]] += recipeEntry.Count[index2];
                            num4 += recipeEntry.Count[index2];
                        }
                        else
                        {
                            break;
                        }
                    }

                    num1 += recipeEntry.CraftCost;
                    if (recipeEntry.CraftCostM > 0)
                        num3 += recipeEntry.CraftCostM;
                    else if (DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.Items[index1].Tag)].TypeID ==
                             Enums.eType.SetO)
                        num3 += recipeEntry.CraftCost;
                    num2 += recipeEntry.BuyCost;
                }

                var index3 = popupData.Add();
                if (Mini)
                    iIndent = 0;
                lblHeader.Text = "Shopping List";
                if (lvPower.CheckedIndices.Count == 1)
                    popupData.Sections[index3].Add(
                        !lvPower.Items[0].Checked
                            ? DatabaseAPI.Database
                                .Power[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[Convert.ToInt32(lvPower.CheckedItems[0].Tag)].NIDPower]
                                .DisplayName
                            : "All Powers", PopUp.Colors.Title);
                else
                    popupData.Sections[index3].Add(Convert.ToString(lvPower.CheckedIndices.Count) + " Powers",
                        PopUp.Colors.Title);
                if (!chkRecipe.Checked)
                    popupData.Sections[index3].Add(Convert.ToString(lvDPA.Items.Count - nonRecipeCount) + " Recipes:",
                        PopUp.Colors.Title);
                if (Mini)
                {
                    var str = "Buy:";
                    if (num2 > 0)
                        popupData.Sections[index3].Add($"{str} {num2:###,###,##0}",
                            PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }
                else
                {
                    var iText = "Buy Cost:";
                    if (num2 > 0)
                        popupData.Sections[index3].Add(iText, PopUp.Colors.Invention,
                            $"{num2:###,###,##0}", PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }

                if (Mini)
                {
                    var str = "Craft:";
                    if (num1 > 0)
                        popupData.Sections[index3].Add(str + " " + $"{num1:###,###,##0}",
                            PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }
                else
                {
                    var iText = "Craft Cost:";
                    if (num1 > 0)
                        popupData.Sections[index3].Add(iText, PopUp.Colors.Invention,
                            $"{num1:###,###,##0}", PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }

                if (Mini)
                {
                    var str = "Craft (Mem'd):";
                    if ((num3 > 0) & (num3 != num1))
                        popupData.Sections[index3].Add($"{str} {num3:###,###,##0}",
                            PopUp.Colors.Effect, 0.9f, FontStyle.Bold, iIndent);
                }
                else
                {
                    var iText = "Craft Cost (Memorized Common):";
                    if ((num3 > 0) & (num3 != num1))
                        popupData.Sections[index3].Add(iText, PopUp.Colors.Effect, $"{num3:###,###,##0}",
                            PopUp.Colors.Effect, 0.9f, FontStyle.Bold, iIndent);
                }

                if (chkRecipe.Checked)
                {
                    RecipeInfo.ColumnPosition = 0.75f;
                    var index1 = popupData.Add();
                    popupData.Sections[index1].Add(Convert.ToString(lvDPA.Items.Count - nonRecipeCount) + " Recipes:",
                        PopUp.Colors.Title);
                    var num7 = numArray2.Length - 1;
                    for (var index2 = 0; index2 <= num7; ++index2)
                    {
                        var num8 = numArray2[index2].Length - 1;
                        for (var index4 = 0; index4 <= num8; ++index4)
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
                                popupData.Sections[index1].Add(" " + Convert.ToString(numArray2[index2][index4]) + " x",
                                    color,
                                    DatabaseAPI.GetEnhancementNameShortWSet(DatabaseAPI.Database.Recipes[index2]
                                        .EnhIdx) + " (" +
                                    Convert.ToString(DatabaseAPI.Database.Recipes[index2].Item[index4].Level + 1) + ")",
                                    color, 0.9f, FontStyle.Bold, iIndent);
                            else
                                popupData.Sections[index1].Add(
                                    DatabaseAPI.GetEnhancementNameShortWSet(DatabaseAPI.Database.Recipes[index2]
                                        .EnhIdx) + " (" +
                                    Convert.ToString(DatabaseAPI.Database.Recipes[index2].Item[index4].Level + 1) + ")",
                                    color, Convert.ToString(numArray2[index2][index4]), color, 0.9f, FontStyle.Bold,
                                    iIndent);
                        }
                    }

                    popupData.Sections[index1].Content = sortPopupStrings(Mini, 2, popupData.Sections[index1].Content);
                }
                else
                {
                    RecipeInfo.ColumnPosition = 0.5f;
                }

                if (Mini)
                {
                    popupData.ColPos = 0.15f;
                    popupData.ColRight = false;
                }

                var index5 = popupData.Add();
                var iText1 = !Mini ? Convert.ToString(num4) + " Salvage Items:" : Convert.ToString(num4) + " Items:";
                popupData.Sections[index5].Add(iText1, PopUp.Colors.Title);
                var num9 = numArray1.Length - 1;
                for (var index1 = 0; index1 <= num9; ++index1)
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
                        popupData.Sections[index5].Add(" " + Convert.ToString(numArray1[index1]) + " x", color,
                            DatabaseAPI.Database.Salvage[index1].ExternalName, color, 0.9f);
                    else
                        popupData.Sections[index5].Add(DatabaseAPI.Database.Salvage[index1].ExternalName, color,
                            Convert.ToString(numArray1[index1]), color, 0.9f, FontStyle.Bold, 1);
                }

                popupData.Sections[index5].Content = sortPopupStrings(Mini, 1, popupData.Sections[index5].Content);
                if (nonRecipeCount == 1)
                    return popupData;
                {
                    var index1 = popupData.Add();
                    var iText2 = !Mini
                        ? Convert.ToString(nonRecipeCount - 1) + " Non-Crafted Enhancements:"
                        : Convert.ToString(nonRecipeCount - 1) + " Enhs:";
                    popupData.Sections[index1].Add(iText2, PopUp.Colors.Title);
                    var num7 = tl.Length - 1;
                    for (var index2 = 0; index2 <= num7; ++index2)
                    {
                        var common = PopUp.Colors.Common;
                        if (Mini)
                            popupData.Sections[index1].Add(" " + Convert.ToString(tl[index2].Count) + " x", common,
                                tl[index2].Text, common, 0.9f);
                        else
                            popupData.Sections[index1].Add(tl[index2].Text, common, Convert.ToString(tl[index2].Count),
                                common, 0.9f, FontStyle.Bold, 1);
                    }

                    popupData.Sections[index1].Content = sortPopupStrings(Mini, 1, popupData.Sections[index1].Content);
                }
                return popupData;
            }

            lblHeader.Text = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.SelectedItems[0].Tag)].LongName +
                             " (" + lvDPA.SelectedItems[0].SubItems[1].Text + ")";
            var rIdx = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.SelectedItems[0].Tag)].RecipeIDX;
            if (lvDPA.SelectedItems[0].SubItems[1].Text == "*")
                rIdx = -1;
            DrawIcon(Convert.ToInt32(lvDPA.SelectedItems[0].Tag));
            if (rIdx <= -1)
                return popupData;
            {
                var index1 = popupData.Add();
                popupData.Sections[index1] =
                    Character.PopRecipeInfo(rIdx, Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text) - 1);
                if (popupData.Sections[index1].Content != null && popupData.Sections[index1].Content.Length > 0)
                {
                    var content = popupData.Sections[index1].Content;
                    var index2 = 0;
                    content[index2].Text = content[index2].Text + " (" + lvDPA.SelectedItems[0].SubItems[1].Text + ")";
                    return popupData;
                }

                popupData.Sections[index1].Content[0].Text = "";
            }
            return popupData;
        }

        private void ChangedRecipeInfoElements()
        {
            VScrollBar1.Value = 0;
            VScrollBar1.Maximum =
                (int) Math.Round(RecipeInfo.lHeight * (VScrollBar1.LargeChange / (double) Panel1.Height));
        }

        private void chkRecipe_CheckedChanged(object sender, EventArgs e)
        {
            lvDPA_SelectedIndexChanged(this, new EventArgs());
            MidsContext.Config.ShoppingListIncludesRecipes = chkRecipe.Checked;
        }

        private void chkSortByLevel_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePowerList();
        }

        private static int colorRarityCompare(Color t1, Color t2)
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
                    if (t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare) |
                        t2.Equals(PopUp.Colors.UltraRare))
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
                else if (t1.Equals(PopUp.Colors.UltraRare) && t2.Equals(PopUp.Colors.Common) |
                    t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare))
                {
                    return 1;
                }

                num = -1;
            }

            return num;
        }

        private static int colorRarityCompareB(Color t1, Color t2)
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
                    if (t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare) |
                        t2.Equals(PopUp.Colors.UltraRare))
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
                else if (t1.Equals(PopUp.Colors.UltraRare) && t2.Equals(PopUp.Colors.Common) |
                    t2.Equals(PopUp.Colors.Uncommon) | t2.Equals(PopUp.Colors.Rare))
                {
                    return 1;
                }

                num = -1;
            }

            return num;
        }

        private void DrawIcon(int Index)
        {
            var extendedBitmap = new ExtendedBitmap(bxRecipe.Size);
            extendedBitmap.Graphics.Clear(Color.Black);
            extendedBitmap.Graphics.DrawImageUnscaled(bxRecipe.Bitmap, 0, 0);
            if (Index > -1)
                extendedBitmap.Graphics.DrawImageUnscaled(I9Gfx.Enhancements[Index], 0, 0);
            pbRecipe.Image = new Bitmap(extendedBitmap.Bitmap);
        }

        private void FillEnhList()
        {
            if (lvPower.CheckedIndices.Count < 1)
            {
                lvDPA.Items.Clear();
            }
            else
            {
                lvDPA.BeginUpdate();
                lvDPA.Items.Clear();
                var items = new string[3];
                var flag = false;
                var num1 = lvPower.CheckedIndices.Count - 1;
                if (lvPower.Items[0].Checked)
                {
                    flag = true;
                    num1 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                }

                ilSets.Images.Clear();
                nonRecipeCount = 1;
                lvDPA.Items.Add(" - All Recipes - ");
                var num2 = num1;
                for (var index1 = 0; index1 <= num2; ++index1)
                {
                    var hIDX = flag ? index1 : Convert.ToInt32(lvPower.CheckedItems[index1].Tag);
                    if (!((MidsContext.Character.CurrentBuild.Powers[hIDX].NIDPowerset > -1) & HasIOs(hIDX)))
                        continue;
                    var num3 = MidsContext.Character.CurrentBuild.Powers[hIDX].Slots.Length - 1;
                    for (var index2 = 0; index2 <= num3; ++index2)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh <= -1)
                            continue;
                        switch (DatabaseAPI.Database
                            .Enhancements[MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh]
                            .TypeID)
                        {
                            case Enums.eType.SetO:
                            {
                                items[0] = DatabaseAPI.Database
                                    .EnhancementSets[
                                        DatabaseAPI.Database
                                            .Enhancements[
                                                MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2]
                                                    .Enhancement.Enh].nIDSet].DisplayName + ": ";
                                var strArray1 = items;
                                var num4 = 0;
                                string[] strArray2;
                                IntPtr index3;
                                (strArray2 = strArray1)[(int) (index3 = (IntPtr) num4)] = strArray2[(int) index3] +
                                    DatabaseAPI.Database
                                        .Enhancements[
                                            MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement
                                                .Enh].Name;
                                items[1] = Convert.ToString(MidsContext.Character.CurrentBuild.Powers[hIDX]
                                    .Slots[index2].Enhancement.IOLevel + 1);
                                break;
                            }
                            case Enums.eType.InventO:
                                items[0] = DatabaseAPI.Database
                                    .Enhancements[
                                        MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh]
                                    .Name + " (Common)";
                                items[1] = Convert.ToString(MidsContext.Character.CurrentBuild.Powers[hIDX]
                                    .Slots[index2].Enhancement.IOLevel + 1);
                                break;
                            default:
                                items[0] = DatabaseAPI.Database
                                    .Enhancements[
                                        MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh]
                                    .Name;
                                ++nonRecipeCount;
                                items[1] = "*";
                                break;
                        }

                        AddToImageList(MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index2].Enhancement.Enh);
                        items[2] = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDX].NIDPower]
                            .DisplayName;
                        lvDPA.Items.Add(new ListViewItem(items, ilSets.Images.Count - 1));
                        lvDPA.Items[lvDPA.Items.Count - 1].Tag = MidsContext.Character.CurrentBuild.Powers[hIDX]
                            .Slots[index2].Enhancement.Enh;
                    }
                }

                lvDPA.EndUpdate();
                if (lvDPA.Items.Count > 0)
                    lvDPA.Items[0].Selected = true;
            }
        }

        private void FillPowerList()
        {
            lvPower.BeginUpdate();
            lvPower.Items.Clear();
            lvPower.Sorting = SortOrder.None;
            lvPower.Items.Add(" - All Powers - ");
            lvPower.Items[lvPower.Items.Count - 1].Tag = -1;
            var num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (var hIdx = 0; hIdx <= num; ++hIdx)
            {
                if (!((MidsContext.Character.CurrentBuild.Powers[hIdx].NIDPower > -1) & HasIOs(hIdx)))
                    continue;
                var text = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIdx].NIDPower]
                    .DisplayName;
                if (chkSortByLevel.Checked)
                    text = $"{MidsContext.Character.CurrentBuild.Powers[hIdx].Level + 1:00} - {text}";
                lvPower.Items.Add(text);
                lvPower.Items[lvPower.Items.Count - 1].Tag = hIdx;
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

        private static int FindItemID(int rIDX, int iLevel)
        {
            var num1 = -1;
            var num2 = 52;
            var num3 = 0;
            var num4 = DatabaseAPI.Database.Recipes[rIDX].Item.Length - 1;
            for (var index = 0; index <= num4; ++index)
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
                var num5 = DatabaseAPI.Database.Recipes[rIDX].Item.Length - 1;
                for (var index = 0; index <= num5; ++index)
                {
                    if (DatabaseAPI.Database.Recipes[rIDX].Item[index].Level != iLevel)
                        continue;
                    num1 = index;
                    break;
                }
            }
            return num1 >= 0 ? num1 : -1;
        }

        private void frmRecipeViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            ibEnhCheckMode.Checked = false;
            pSalvageSummary.Visible = false;
            MidsContext.EnhCheckMode = false;
            StoreLocation();
            myParent.DoRedraw();
            myParent.FloatRecipe(false);
        }

        private void frmRecipeViewer_Load(object sender, EventArgs e)
        {
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;
            RecipeInfo.SetPopup(new PopUp.PopupData());
            ChangedRecipeInfoElements();
            chkRecipe.Checked = MidsContext.Config.ShoppingListIncludesRecipes;
            RecalcSalvage();
            ibEnhCheckMode.IA = myParent.Drawing.pImageAttributes;
            ibEnhCheckMode.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibEnhCheckMode.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            MidsContext.EnhCheckMode = false;
            ibEnhCheckMode.Checked = false;
            pSalvageSummary.Visible = false;
            Loading = false;
        }

        private bool HasIOs(int hIDX)
        {
            if (hIDX < 0)
                return false;
            var num = MidsContext.Character.CurrentBuild.Powers[hIDX].Slots.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index].Enhancement.Enh > -1)
                    return true;
            return false;
        }

        private void CopyToClipboard()
        {
            var str1 = "";
            var popupData = BuildList(true);
            var num1 = RecipeInfo.pData.Sections.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var num2 = RecipeInfo.pData.Sections[index1].Content.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    var content = popupData.Sections[index1].Content;
                    var index3 = index2;
                    var str2 = str1 + content[index3].Text;
                    if (content[index3].TextColumn != "")
                        str2 = str2 + "  " + content[index3].TextColumn;
                    str1 = str2 + "\r\n";
                }

                str1 += "\r\n";
            }

            Clipboard.SetDataObject(str1, true);
            MessageBox.Show("Data copied to clipboard!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ibClipboard_ButtonClicked()
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

        private void ibClose_ButtonClicked()
        {
            Close();
        }

        private void ibMiniList_ButtonClicked()
        {
            myParent.SetMiniList(BuildList(true), "Shopping List");
        }

        private void ibTopmost_ButtonClicked()
        {
            TopMost = ibTopmost.Checked;
            if (!TopMost)
                return;
            BringToFront();
        }

        private void ibEnhCheckMode_ButtonClicked()
        {
            Debug.WriteLine($"Check mode (before): {ibEnhCheckMode.Checked}");
            ibEnhCheckMode.Checked = !ibEnhCheckMode.Checked;
            MidsContext.EnhCheckMode = ibEnhCheckMode.Checked;
            Debug.WriteLine($"Check mode (after): {ibEnhCheckMode.Checked}");
            pSalvageSummary.Visible = MidsContext.EnhCheckMode;
            myParent.DoRedraw();
        }

        [DebuggerStepThrough]
        private void lvPower_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Index == 0)
            {
                if (Operators.ConditionalCompareObjectLess(e.Item.Tag, 0, false) && e.Item.Checked)
                {
                    var num = lvPower.Items.Count - 1;
                    for (var index = 1; index <= num; ++index)
                        lvPower.Items[index].Checked = false;
                }
            }
            else if (e.Item.Checked)
            {
                lvPower.Items[0].Checked = false;
            }

            FillEnhList();
        }

        private void lvPower_MouseEnter(object sender, EventArgs e)
        {
            lvPower.Focus();
        }

        private void lvDPA_MouseEnter(object sender, EventArgs e)
        {
            lvDPA.Focus();
        }

        private void lvDPA_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecipeInfo.ScrollY = 0.0f;
            RecipeInfo.SetPopup(BuildList(false));
            ChangedRecipeInfoElements();
        }

        private static void putInList(ref CountingList[] tl, string item)
        {
            var num = tl.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (tl[index].Text != item)
                    continue;
                ++tl[index].Count;
                return;
            }

            tl = (CountingList[]) Utils.CopyArray(tl, new CountingList[tl.Length + 1]);
            tl[tl.Length - 1].Count = 1;
            tl[tl.Length - 1].Text = item;
        }

        private void RecipeInfo_MouseEnter(object sender, EventArgs e)
        {
            VScrollBar1.Focus();
        }

        private void RecipeInfo_MouseWheel(object sender, MouseEventArgs e)
        {
            VScrollBar1.Value =
                Convert.ToInt32(Operators.AddObject(VScrollBar1.Value, e.Delta > 0 ? -1 : 1)); // Interaction.IIf(e.Delta > 0, -1, 1)
            if (VScrollBar1.Value > VScrollBar1.Maximum - 9)
                VScrollBar1.Value = VScrollBar1.Maximum - 9;
            VScrollBar1_Scroll(RuntimeHelpers.GetObjectValue(sender),
                new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
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
                rectangle.X = (int) Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2.0);
            if (rectangle.Y < 32)
                rectangle.Y = (int) Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2.0);
            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
        }

        private static PopUp.StringValue[] sortPopupStrings(
            bool Mini,
            int colorSortMode,
            PopUp.StringValue[] inStrs)
        {
            var num1 = 0;
            var numArray = new int[inStrs.Length - 1 + 1];
            var num2 = numArray.Length - 1;
            for (var index1 = 0; index1 <= num2; ++index1)
            {
                var flag = false;
                var num3 = index1 - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                {
                    flag = true;
                    num1 = colorSortMode switch
                    {
                        1 => colorRarityCompare(inStrs[numArray[index2]].tColor, inStrs[index1].tColor),
                        2 => colorRarityCompareB(inStrs[numArray[index2]].tColor, inStrs[index1].tColor),
                        _ => num1
                    };

                    if ((num1 != 0 || string.CompareOrdinal(
                        Convert.ToString(Mini ? inStrs[index1].TextColumn : inStrs[index1].Text), // Interaction.IIf(Mini, inStrs[index1].TextColumn, inStrs[index1].Text)
                        Convert.ToString(Mini ? inStrs[numArray[index2]].TextColumn : inStrs[numArray[index2]].Text)) >= 0) && num1 <= 0) // Interaction.IIf(Mini, inStrs[numArray[index2]].TextColumn, inStrs[numArray[index2]].Text)
                        continue;
                    var num4 = index2;
                    for (var index3 = index1 - 1; index3 >= num4; index3 += -1)
                        numArray[index3 + 1] = numArray[index3];
                    numArray[index2] = index1;
                    flag = false;
                    break;
                }

                if (flag)
                    numArray[index1] = index1;
            }

            var stringValueArray = new PopUp.StringValue[inStrs.Length - 1 + 1];
            var num5 = inStrs.Length - 1;
            for (var index = 0; index <= num5; ++index)
                stringValueArray[index] = inStrs[numArray[index]];
            return stringValueArray;
        }

        private void StoreLocation()
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
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibTopmost.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibClipboard.IA = myParent.Drawing.pImageAttributes;
            ibClipboard.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClipboard.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            ibMiniList.IA = myParent.Drawing.pImageAttributes;
            ibMiniList.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibMiniList.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            FillPowerList();
        }

        private void UpdatePowerList()
        {
            if (Loading)
                return;
            FillPowerList();
        }

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            RecipeInfo.ScrollY = VScrollBar1.Value / (float) (VScrollBar1.Maximum - VScrollBar1.LargeChange) *
                                 (RecipeInfo.lHeight - Panel1.Height);
        }

        private struct CountingList
        {
            public string Text;
            public int Count;
        }
    }
}