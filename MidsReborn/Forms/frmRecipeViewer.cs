using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;
using mrbBase.Base.Extensions;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms
{
    public partial class frmRecipeViewer : Form
    {
        private readonly ExtendedBitmap bxRecipe;
        private readonly frmMain myParent;

        private ImageButton ibClipboard;
        private ImageButton ibClose;
        private ImageButton ibMiniList;
        private ImageButton ibTopmost;

        private bool Loading;
        private bool SalvageHudVisible;
        private int nonRecipeCount;
        private ctlPopUp RecipeInfo;

        public frmRecipeViewer(frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FormClosed += frmRecipeViewer_FormClosed;
            Load += frmRecipeViewer_Load;
            Loading = true;
            InitializeComponent();
            Name = nameof(frmRecipeViewer);
            //var componentResourceManager = new ComponentResourceManager(typeof(frmRecipeViewer));
            Icon = Resources.reborn;
            lvPower.MouseEnter += lvPower_MouseEnter;
            lvPower.ItemChecked += lvPower_ItemChecked;
            lvDPA.SelectedIndexChanged += lvDPA_SelectedIndexChanged;
            lvDPA.MouseEnter += lvDPA_MouseEnter;
            chkRecipe.CheckedChanged += chkRecipe_CheckedChanged;
            chkSortByLevel.CheckedChanged += chkSortByLevel_CheckedChanged;
            ibClipboard.ButtonClicked += ibClipboard_ButtonClicked;
            ibClose.ButtonClicked += ibClose_ButtonClicked;
            ibMiniList.ButtonClicked += ibMiniList_ButtonClicked;
            ibTopmost.ButtonClicked += ibTopmost_ButtonClicked;
            ibEnhCheckMode.ButtonClicked += ibEnhCheckMode_ButtonClicked;
            myParent = iParent;
            bxRecipe = new ExtendedBitmap(I9Gfx.GetRecipeTransparentName());
        }

        public void RecalcSalvage()
        {
            BuildSalvageSummary.UpdateAllSalvage(lblEnhObtained, lblCatalysts, lblBoosters);
        }

        public void UpdateEnhObtained()
        {
            BuildSalvageSummary.UpdateEnhObtained(lblEnhObtained);
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
                extendedBitmap.Graphics.Clear(Color.Transparent);
                var graphics = extendedBitmap.Graphics;
                I9Gfx.DrawEnhancement(ref graphics, enhancement.ImageIdx, Origin.Grade.IO);
                ilSets.Images.Add(extendedBitmap.Bitmap);
            }
            else
            {
                ilSets.Images.Add(new Bitmap(ilSets.ImageSize.Width, ilSets.ImageSize.Height));
            }
        }

        private Font GetBestFitFont(Label lbl, string txt, float minSize, float maxSize, FontStyle fontStyle = FontStyle.Regular)
        {
            // http://csharphelper.com/blog/2015/04/size-a-font-to-fit-a-label-in-c/
            if (txt.Length <= 0) return new Font(lbl.Font.FontFamily, maxSize, fontStyle, GraphicsUnit.Pixel, 0);

            var bestSize = maxSize;
            var w = lbl.DisplayRectangle.Width - 3;
            var h = lbl.DisplayRectangle.Height - 3;
            using (var g = lbl.CreateGraphics())
            {
                for (var i = minSize; i <= maxSize; i += 0.5f)
                {
                    using var f = new Font(lbl.Font.FontFamily, i, fontStyle, GraphicsUnit.Pixel, 0);
                    var txtSize = g.MeasureString(txt, f);
                    if (txtSize.Width <= w & txtSize.Height <= h) continue;

                    bestSize = i - 1;
                    break;
                }
            }

            return new Font(lbl.Font.FontFamily, bestSize, fontStyle, GraphicsUnit.Pixel, 0);
        }

        private int GetNumBoostersFromLv(ListViewItem lvItem)
        {
            var linkedEnhIdx = Convert.ToInt32(lvItem.Tag);
            var lvPowerName = lvItem.SubItems[2].Text;
            var powerEntry = MidsContext.Character.CurrentBuild.Powers
                .Where(p => p.Power != null)
                .First(p => p.Power.DisplayName == lvPowerName);
            //var linkedEnh = DatabaseAPI.Database.Enhancements[linkedEnhIdx];
            var powerSlot = powerEntry.Slots.First(ps => ps.Enhancement.Enh == linkedEnhIdx);
            var numBoosters = powerSlot.Enhancement.RelativeLevel switch
            {
                Enums.eEnhRelative.PlusOne => 1,
                Enums.eEnhRelative.PlusTwo => 2,
                Enums.eEnhRelative.PlusThree => 3,
                Enums.eEnhRelative.PlusFour => 4,
                Enums.eEnhRelative.PlusFive => 5,
                _ => 0
            };

            return numBoosters;
        }

        private int GetPopupDataLines(PopUp.PopupData popupData)
        {
            return popupData.Sections?.Sum(s => s.Content.Length) ?? 0;
        }

        private PopUp.PopupData BuildList(bool mini)
        {
            var iIndent = 1;
            var popupData = new PopUp.PopupData();
            var tl = new List<CountingList>();
            if (lvDPA.SelectedIndices.Count < 1)
            {
                return popupData;
            }

            var boosterSalvageIdx = Array.IndexOf(DatabaseAPI.Database.Salvage,
                DatabaseAPI.Database.Salvage.First(s => s.ExternalName == "Enhancement Booster"));
            
            RecipeInfo.SuspendLayout();
            var numBoosters = 0;
            if (lvDPA.SelectedIndices[0] == 0)
            {
                var salvageTotalCount = new Dictionary<int, int>();
                var recipeTotalCount = new Dictionary<KeyValuePair<int, int>, int>();
                var num1 = 0;
                var num2 = 0;
                var num3 = 0;
                var num4 = 0;
                DrawIcon(-1);

                var lvRecipesItems = lvDPA.Items.Count;
                for (var i = 1; i < lvRecipesItems; i++)
                {
                    var rIDX = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.Items[i].Tag)].RecipeIDX;
                    if (lvDPA.Items[i].SubItems[1].Text == "*")
                    {
                        rIDX = -1;
                        PutInList(ref tl, lvDPA.Items[i].Text);
                    }

                    if (rIDX <= -1)
                    {
                        continue;
                    }

                    var iLevel = Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text) - 1;
                    var itemId = FindItemID(rIDX, iLevel);
                    if (itemId <= -1)
                    {
                        continue;
                    }

                    if (chkRecipe.Checked)
                    {
                        var rk = new KeyValuePair<int, int>(rIDX, itemId);
                        if (!recipeTotalCount.ContainsKey(rk))
                        {
                            recipeTotalCount.Add(rk, 1);
                        }
                        else
                        {
                            recipeTotalCount[rk]++;
                        }
                    }

                    var recipeEntry = DatabaseAPI.Database.Recipes[rIDX].Item[itemId];
                    for (var j = 0 ; j < recipeEntry.SalvageIdx.Length; j++)
                    {
                        var cs = recipeEntry.SalvageIdx[j];
                        var csc = recipeEntry.Count[j];
                        if (cs <= -1 || csc <= 0)
                        {
                            continue;
                        }

                        if (!salvageTotalCount.ContainsKey(cs))
                        {
                            salvageTotalCount.Add(cs, csc);
                        }
                        else
                        {
                            salvageTotalCount[cs] += csc;
                        }

                        num4 += csc;
                    }

                    numBoosters = GetNumBoostersFromLv(lvDPA.Items[i]);
                    if (!salvageTotalCount.ContainsKey(boosterSalvageIdx))
                    {
                        salvageTotalCount.Add(boosterSalvageIdx, numBoosters);
                    }
                    else
                    {
                        salvageTotalCount[boosterSalvageIdx] += numBoosters;
                    }

                    num1 += recipeEntry.CraftCost;
                    if (recipeEntry.CraftCostM > 0)
                    {
                        num3 += recipeEntry.CraftCostM;
                    }
                    else if (DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.Items[i].Tag)].TypeID == Enums.eType.SetO)
                    {
                        num3 += recipeEntry.CraftCost;
                    }

                    num2 += recipeEntry.BuyCost;
                }

                var index3 = popupData.Add();
                if (mini)
                {
                    iIndent = 0;
                }

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
                {
                    popupData.Sections[index3].Add($"{lvPower.CheckedIndices.Count} Powers",
                        PopUp.Colors.Title);
                }

                if (!chkRecipe.Checked)
                {
                    popupData.Sections[index3].Add($"{lvDPA.Items.Count - nonRecipeCount} Recipes:",
                        PopUp.Colors.Title);
                }

                if (num2 > 0)
                {
                    popupData.Sections[index3].Add($"Buy{(mini ? "" : " Cost")}: {num2:###,###,##0}",
                        PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }

                if (num1 > 0)
                {
                    popupData.Sections[index3].Add($"Craft{(mini ? "" : " Cost")}: {num1:###,###,##0}",
                        PopUp.Colors.Invention, 0.9f, FontStyle.Bold, iIndent);
                }

                if ((num3 > 0) & (num3 != num1))
                {
                    popupData.Sections[index3].Add($"Craft ({(mini ? "Mem'd" : "Memorized Common")}): {num3:###,###,##0}",
                        PopUp.Colors.Effect, 0.9f, FontStyle.Bold, iIndent);
                }

                if (chkRecipe.Checked)
                {
                    RecipeInfo.ColumnPosition = 0.75f;
                    var index1 = popupData.Add();
                    popupData.Sections[index1].Add($"{lvDPA.Items.Count - nonRecipeCount} Recipes:", PopUp.Colors.Title);
                    foreach (var ri in recipeTotalCount)
                    {
                        var rId = ri.Key.Key;
                        var rEntryId = ri.Key.Value;
                        var rAmt = ri.Value;
                        var color = DatabaseAPI.Database.Recipes[rId].Rarity switch
                        {
                            Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                            Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                            Recipe.RecipeRarity.UltraRare => PopUp.Colors.UltraRare,
                            _ => PopUp.Colors.Text
                        };

                        var craftedEnhName = DatabaseAPI.Database.Recipes[rId].IsGeneric | DatabaseAPI.Database.Recipes[rId].IsVirtual
                            ? DatabaseAPI.Database.Recipes[rId].ExternalName
                            : DatabaseAPI.GetEnhancementNameShortWSet(DatabaseAPI.Database.Recipes[rId].EnhIdx);

                        if (craftedEnhName.Contains("Merits Recipe")) continue;
                        
                        if (mini)
                        {
                            popupData.Sections[index1].Add($" {rAmt} x",
                                color,
                                $"{craftedEnhName} ({DatabaseAPI.Database.Recipes[rId].Item[rEntryId].Level + 1})",
                                color, 0.9f, FontStyle.Bold, iIndent);
                        }
                        else
                        {
                            popupData.Sections[index1].Add(
                                $"{craftedEnhName} ({DatabaseAPI.Database.Recipes[rId].Item[rEntryId].Level + 1})",
                                color, Convert.ToString(rAmt), color, 0.9f, FontStyle.Bold, iIndent);
                        }
                    }

                    popupData.Sections[index1].Content = sortPopupStrings(mini, 2, popupData.Sections[index1].Content);
                }
                else
                {
                    RecipeInfo.ColumnPosition = 0.5f;
                }

                if (mini)
                {
                    popupData.ColPos = 0.15f;
                    popupData.ColRight = false;
                }

                var index5 = popupData.Add();
                var iText1 = mini ? $"{num4} Items:" : $"{num4} Salvage Items:";
                popupData.Sections[index5].Add(iText1, PopUp.Colors.Title);
                foreach (var sl in salvageTotalCount)
                {
                    var color = DatabaseAPI.Database.Salvage[sl.Key].Rarity switch
                    {
                        Recipe.RecipeRarity.Common => PopUp.Colors.Common,
                        Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                        Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                        _ => Color.White
                    };

                    if (mini)
                    {
                        popupData.Sections[index5].Add($" {sl.Value} x", color,
                            DatabaseAPI.Database.Salvage[sl.Key].ExternalName, color, 0.9f);
                    }
                    else
                    {
                        popupData.Sections[index5].Add(DatabaseAPI.Database.Salvage[sl.Key].ExternalName, color,
                            $"{sl.Value}", color, 0.9f, FontStyle.Bold, 1);
                    }
                }

                popupData.Sections[index5].Content = sortPopupStrings(mini, 1, popupData.Sections[index5].Content);
                if (nonRecipeCount == 1)
                    return popupData;
                {
                    var index1 = popupData.Add();
                    var iText2 = mini
                        ? $"{nonRecipeCount - 1} Enhs:"
                        : $"{nonRecipeCount - 1} Non-Crafted Enhancements:";

                    popupData.Sections[index1].Add(iText2, PopUp.Colors.Title);
                    for (var index2 = 0; index2 < tl.Count; index2++)
                    {
                        if (mini)
                        {
                            popupData.Sections[index1].Add($" {tl[index2].Count} x", PopUp.Colors.Common,
                                tl[index2].Text, PopUp.Colors.Common, 0.9f);
                        }
                        else
                        {
                            popupData.Sections[index1].Add(tl[index2].Text, PopUp.Colors.Common, Convert.ToString(tl[index2].Count),
                                PopUp.Colors.Common, 0.9f, FontStyle.Bold, 1);
                        }
                    }

                    popupData.Sections[index1].Content = sortPopupStrings(mini, 1, popupData.Sections[index1].Content);
                }

                RecipeInfo.Height = (int) Math.Round(RecipeInfo.lHeight) + Panel1.Height + 16;
                RecipeInfo.ResumeLayout();

                return popupData;
            }

            numBoosters = GetNumBoostersFromLv(lvDPA.SelectedItems[0]);
            var boostersRelLevel = numBoosters switch
            {
                1 => Enums.eEnhRelative.PlusOne,
                2 => Enums.eEnhRelative.PlusTwo,
                3 => Enums.eEnhRelative.PlusThree,
                4 => Enums.eEnhRelative.PlusFour,
                5 => Enums.eEnhRelative.PlusFive,
                _ => Enums.eEnhRelative.Even
            };

            var headerText = $"{DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.SelectedItems[0].Tag)].LongName} ({lvDPA.SelectedItems[0].SubItems[1].Text}{(numBoosters > 0 ? $"+{numBoosters}" : "")})";
            lblHeader.SuspendLayout();
            lblHeader.Font = GetBestFitFont(lblHeader, headerText, 10f, 18f, FontStyle.Bold);
            lblHeader.Text = headerText;
            lblHeader.ResumeLayout();
            var rIdx = DatabaseAPI.Database.Enhancements[Convert.ToInt32(lvDPA.SelectedItems[0].Tag)].RecipeIDX;
            if (lvDPA.SelectedItems[0].SubItems[1].Text == "*")
            {
                rIdx = -1;
            }

            DrawIcon(Convert.ToInt32(lvDPA.SelectedItems[0].Tag));
            if (rIdx <= -1)
            {
                return popupData;
            }

            var index4 = popupData.Add();
            popupData.Sections[index4] = Character.PopRecipeInfo(rIdx,
                Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text) - 1,
                boostersRelLevel);
            if (popupData.Sections[index4].Content != null && popupData.Sections[index4].Content.Length > 0)
            {
                var content = popupData.Sections[index4].Content;
                content[0].Text = $"{content[0].Text} ({lvDPA.SelectedItems[0].SubItems[1].Text})";

                return popupData;
            }

            var sContent = popupData.Sections[index4].Content;
            if (sContent != null) sContent[0].Text = "";

            RecipeInfo.ResumeLayout();

            return popupData;
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

        private void DrawIcon(int index)
        {
            var extendedBitmap = new ExtendedBitmap(bxRecipe.Size);
            extendedBitmap.Graphics.Clear(Color.Black);
            extendedBitmap.Graphics.DrawImageUnscaled(index > -1 ? I9Gfx.Enhancements[index] : Resources.Icon_AncientMemories, 0, 0);
            //extendedBitmap.Graphics.DrawImageUnscaled(index > -1 ? I9Gfx.Enhancements[index] : Resources.Icon_DisorientingField, 0, 0);
            extendedBitmap.Graphics.DrawImageUnscaled(bxRecipe.Bitmap, 0, 0);
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
            var num = MidsContext.Character.CurrentBuild.Powers.Count;
            for (var hIdx = 0; hIdx < num; hIdx++)
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
            var num4 = DatabaseAPI.Database.Recipes[rIDX].Item.Length;
            for (var index = 0; index < num4; index++)
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
                var num5 = DatabaseAPI.Database.Recipes[rIDX].Item.Length;
                for (var index = 0; index < num5; index++)
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
            if (SalvageHudVisible)
            {
                myParent.FloatBuildSalvageHud(true);
            }
            else
            {
                MidsContext.EnhCheckMode = false;
                ibEnhCheckMode.Checked = false;
                pSalvageSummary.Visible = false;
                myParent.UpdateEnhCheckModeToolStrip();
                myParent.DoRedraw();
            }

            StoreLocation();
            myParent.FloatRecipe(false);
        }

        private void frmRecipeViewer_Load(object sender, EventArgs e)
        {
            // Bug: open dps calc form then the recipe viewer, form size differs
            Size = new Size(738, 543);
            lvPower.EnableDoubleBuffer();
            lvDPA.EnableDoubleBuffer();
            UpdateColorTheme();
            RecipeInfo.SetPopup(new PopUp.PopupData());
            chkRecipe.Checked = MidsContext.Config.ShoppingListIncludesRecipes;
            RecalcSalvage();
            SalvageHudVisible = myParent.IsSalvageHudVisible();
            if (SalvageHudVisible)
            {
                myParent.SetSalvageHudOnCloseExecution(false);
                myParent.FloatBuildSalvageHud(false);
            }
            else
            {
                MidsContext.EnhCheckMode = false;
                myParent.UpdateEnhCheckModeToolStrip();
            }

            ibEnhCheckMode.Checked = false;
            pSalvageSummary.Visible = false;
            if (SalvageHudVisible)
            {
                myParent.Activate();
            }

            Loading = false;
        }

        public void UpdateColorTheme()
        {
            BackColor = myParent.BackColor;
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
            
            ibEnhCheckMode.IA = myParent.Drawing.pImageAttributes;
            ibEnhCheckMode.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibEnhCheckMode.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;

            ibClipboard.IA = myParent.Drawing.pImageAttributes;
            ibClipboard.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClipboard.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;

            ibMiniList.IA = myParent.Drawing.pImageAttributes;
            ibMiniList.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibMiniList.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;
        }

        private bool HasIOs(int hIDX)
        {
            if (hIDX < 0)
                return false;
            for (var index = 0; index < MidsContext.Character.CurrentBuild.Powers[hIDX].Slots.Length; index++)
                if (MidsContext.Character.CurrentBuild.Powers[hIDX].Slots[index].Enhancement.Enh > -1)
                    return true;
            return false;
        }

        private void CopyToClipboard()
        {
            var ret = "";
            var popupData = BuildList(true);
            for (var index1 = 0; index1 < RecipeInfo.pData.Sections.Length; index1++)
            {
                for (var index2 = 0; index2 < RecipeInfo.pData.Sections[index1].Content.Length; index2++)
                {
                    var content = popupData.Sections[index1].Content;
                    ret += $"{content[index2].Text}{(content[index2].TextColumn != "" ? $"  {content[index2].TextColumn}" : "")}\r\n";
                }

                ret += "\r\n";
            }

            Clipboard.SetDataObject(ret, true);
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
            MidsContext.EnhCheckMode = ibEnhCheckMode.Checked;
            RecalcSalvage();
            pSalvageSummary.Visible = MidsContext.EnhCheckMode;
            myParent.DoRedraw();
        }

        [DebuggerStepThrough]
        private void lvPower_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item == null) return;
            if (e.Item.Index == 0)
            {
                if (lvPower.Items.Count > 0 &&
                    e.Item.Tag != null &&
                    !e.Item.Tag.Equals(0)
                    && e.Item.Checked)
                {
                    foreach (var o in lvPower.Items)
                    {
                        var el = (ListViewItem) o;
                        if (el.SubItems[0].ToString().Contains("All Powers")) continue;
                        el.Checked = false;
                    }
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
            RecipeInfo.SetPopup(BuildList(false));
        }

        private static void PutInList(ref List<CountingList> tl, string item)
        {
            for (var index = 0; index < tl.Count; index++)
            {
                var e = tl[index];
                if (e.Text != item) continue;
                
                e.Count++;
                
                return;
            }

            tl.Add(new CountingList { Count = 1, Text = item });
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
            var numArray = new int[inStrs.Length];
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
                        Convert.ToString(Mini ? inStrs[index1].TextColumn : inStrs[index1].Text), // Interaction.IIf(mini, inStrs[index1].TextColumn, inStrs[index1].Text)
                        Convert.ToString(Mini ? inStrs[numArray[index2]].TextColumn : inStrs[numArray[index2]].Text)) >= 0) && num1 <= 0) // Interaction.IIf(mini, inStrs[numArray[index2]].TextColumn, inStrs[numArray[index2]].Text)
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

            var stringValueArray = new PopUp.StringValue[inStrs.Length];
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
            UpdateColorTheme();
            FillPowerList();
            ibEnhCheckMode.Checked = MidsContext.EnhCheckMode;
            pSalvageSummary.Visible = MidsContext.EnhCheckMode;
        }

        private void UpdatePowerList()
        {
            if (Loading)
                return;
            FillPowerList();
        }

        private struct CountingList
        {
            public string Text;
            public int Count;
        }
    }
}