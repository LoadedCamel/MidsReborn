using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmRecipeEdit : Form
    {
        private bool NoUpdate;

        public frmRecipeEdit()
        {
            Load += frmRecipeEdit_Load;
            NoUpdate = true;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmRecipeEdit));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
        }

        private void AddListItem(int Index)
        {
            if (!((Index > -1) & (Index < DatabaseAPI.Database.Recipes.Length)))
                return;
            var recipe = DatabaseAPI.Database.Recipes[Index];
            lvDPA.Items.Add(new ListViewItem(new[]
            {
                recipe.InternalName,
                recipe.EnhIdx <= -1 ? "None" : recipe.Enhancement + " (" + Convert.ToString(recipe.EnhIdx) + ")",
                Enum.GetName(recipe.Rarity.GetType(), recipe.Rarity),
                Convert.ToString(recipe.Item.Length)
            }));
        }

        private static void AssignNewRecipes()
        {
            for (var index = 0; index <= DatabaseAPI.Database.Recipes.Length - 1; ++index)
            {
                var recipe = DatabaseAPI.Database.Recipes[index];
                if (!((recipe.EnhIdx > -1) & (recipe.EnhIdx <= DatabaseAPI.Database.Enhancements.Length - 1)) ||
                    DatabaseAPI.Database.Enhancements[recipe.EnhIdx].RecipeIDX >= 0)
                    continue;
                DatabaseAPI.Database.Enhancements[recipe.EnhIdx].RecipeIDX = index;
                DatabaseAPI.Database.Enhancements[recipe.EnhIdx].RecipeName = recipe.InternalName;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (RecipeID() < 0)
                    return;
                var recipe = DatabaseAPI.Database.Recipes[RecipeID()];
                recipe.Item = recipe.Item.Append(new Recipe.RecipeEntry
                {
                    Level = 9
                }).ToArray();
                ShowRecipeInfo(RecipeID());
                UpdateListItem(RecipeID());
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                DatabaseAPI.LoadRecipes();
                Close();
            });
        }

        // this delete button is supposed to remove a specific level of recipe, not the whole recipe.
        private void btnDel_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                var recipeid = RecipeID();
                if (recipeid < 0 || lstItems.SelectedIndex < 0 || lstItems.Items.Count < 2)
                    return;
                var recipe = DatabaseAPI.Database.Recipes[recipeid];
                var recipeItemId = lstItems.SelectedIndex;
                recipe.Item = recipe.Item.Where((ri, i) => i != recipeItemId).ToArray();
                ShowRecipeInfo(RecipeID());
            });
        }

        private void btnGuessCost_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0)
                    return;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].CraftCost =
                    GetCostByLevel(DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].Level);
                udCraft.Value = new decimal(DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].CraftCost);
            });
        }

        private void btnI20_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() => IncrementX(19));
        }

        private void btnI25_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() => IncrementX(24));
        }

        private void btnI40_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() => IncrementX(39));
        }

        private void btnI50_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() => IncrementX(49));
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (Interaction.MsgBox("Really erase all stored recipes and attempt import?",
                    MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Careful...") != MsgBoxResult.Yes)
                    return;
                char[] delimiter = {'\r'};
                var strArray1 = Clipboard.GetDataObject()?.GetData("System.String", true).ToString().Split(delimiter);
                delimiter[0] = '\t';
                DatabaseAPI.Database.Recipes = new Recipe[0];
                var num1 = strArray1.Length - 1;
                for (var index1 = 0; index1 <= num1; ++index1)
                {
                    var strArray2 = strArray1[index1].Split(delimiter);
                    if (strArray2.Length <= 7)
                        continue;
                    var iName = strArray2[0].Replace("_Memorized", "").LastIndexOf("_", StringComparison.Ordinal) <= 0
                        ? strArray2[0]
                        : strArray2[0].Substring(0,
                            strArray2[0].Replace("_Memorized", "").LastIndexOf("_", StringComparison.Ordinal));
                    if (!char.IsLetterOrDigit(iName[0]))
                        iName = iName.Substring(1);
                    var recipe1 = DatabaseAPI.GetRecipeByName(iName);
                    if (recipe1 == null)
                    {
                        recipe1 =
                            new Recipe
                            {
                                InternalName = iName,
                                ExternalName = strArray2[14] != "" ? "" : strArray2[1],
                                Rarity = (Recipe.RecipeRarity) Math.Round(Conversion.Val(strArray2[15]) - 1.0)
                            };
                        DatabaseAPI.Database.Recipes = DatabaseAPI.Database.Recipes.Append(recipe1).ToArray();
                    }

                    var index2 = -1;
                    var num2 = recipe1.Item.Length - 1;
                    for (var index3 = 0; index3 <= num2; ++index3)
                        if (Math.Abs(recipe1.Item[index3].Level - (Conversion.Val(strArray2[16]) - 1.0)) <
                            float.Epsilon)
                            index2 = index3;
                    if (index2 < 0)
                    {
                        recipe1.Item = (Recipe.RecipeEntry[]) Utils.CopyArray(recipe1.Item,
                            new Recipe.RecipeEntry[recipe1.Item.Length + 1]);
                        recipe1.Item[recipe1.Item.Length - 1] = new Recipe.RecipeEntry();
                        index2 = recipe1.Item.Length - 1;
                    }

                    recipe1.Item[index2].Level = (int) Math.Round(Conversion.Val(strArray2[16]) - 1.0);
                    if (strArray2[0].IndexOf("Memorized", StringComparison.Ordinal) > -1)
                    {
                        recipe1.Item[index2].BuyCostM = (int) Math.Round(Conversion.Val(strArray2[19]) - 1.0);
                        recipe1.Item[index2].CraftCostM = (int) Math.Round(Conversion.Val(strArray2[17]) - 1.0);
                    }
                    else
                    {
                        recipe1.Item[index2].BuyCost = (int) Math.Round(Conversion.Val(strArray2[19]) - 1.0);
                        recipe1.Item[index2].CraftCost = (int) Math.Round(Conversion.Val(strArray2[17]) - 1.0);
                    }

                    var index4 = 0;
                    do
                    {
                        if (Conversion.Val(strArray2[4 + index4 * 2]) > 0.0)
                        {
                            recipe1.Item[index2].Count[index4] =
                                (int) Math.Round(Conversion.Val(strArray2[4 + index4 * 2]));
                            recipe1.Item[index2].Salvage[index4] = strArray2[5 + index4 * 2];
                            recipe1.Item[index2].SalvageIdx[index4] = -1;
                        }

                        ++index4;
                    } while (index4 <= 4);
                }

                DatabaseAPI.AssignRecipeSalvageIDs();
                DatabaseAPI.GuessRecipes();
                DatabaseAPI.AssignRecipeIDs();
                FillList();
                Interaction.MsgBox("Done. Recipe-Enhancement links have been guessed.", MsgBoxStyle.Information,
                    "Import");
            });
        }

        private void btnIncrement_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (RecipeID() < 0 || (DatabaseAPI.Database.Recipes[RecipeID()].Item.Length < 1) |
                    (DatabaseAPI.Database.Recipes[RecipeID()].Item.Length > 53))
                    return;
                DatabaseAPI.Database.Recipes[RecipeID()].Item = (Recipe.RecipeEntry[]) Utils.CopyArray(
                    DatabaseAPI.Database.Recipes[RecipeID()].Item,
                    new Recipe.RecipeEntry[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length + 1]);
                DatabaseAPI.Database.Recipes[RecipeID()].Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1]
                    = new Recipe.RecipeEntry(DatabaseAPI.Database.Recipes[RecipeID()]
                        .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 2]);
                ++DatabaseAPI.Database.Recipes[RecipeID()]
                    .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1].Level;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1]
                    .CraftCost = GetCostByLevel(DatabaseAPI.Database.Recipes[RecipeID()]
                    .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1].Level);
                ShowRecipeInfo(RecipeID());
                lstItems.SelectedIndex = lstItems.Items.Count - 1;
            });
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                AssignNewRecipes();
                DatabaseAPI.AssignRecipeSalvageIDs();
                DatabaseAPI.AssignRecipeIDs();
                var serializer = MyApplication.GetSerializer();
                DatabaseAPI.SaveRecipes(serializer);
                DatabaseAPI.SaveEnhancementDb(serializer);
                Close();
            });
        }

        private void btnRAdd_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                var recipe = new Recipe();
                DatabaseAPI.Database.Recipes = DatabaseAPI.Database.Recipes.Append(recipe).ToArray();
                //IDatabase database = DatabaseAPI.Database;
                //Recipe[] recipeArray = (Recipe[])Utils.CopyArray(database.Recipes, new Recipe[DatabaseAPI.Database.Recipes.Length + 1]);
                //database.Recipes = recipeArray;
                //DatabaseAPI.Database.Recipes[DatabaseAPI.Database.Recipes.Length - 1] = new Recipe();
                AddListItem(DatabaseAPI.Database.Recipes.Length - 1);
                lvDPA.Items[lvDPA.Items.Count - 1].Selected = true;
                lvDPA.Items[lvDPA.Items.Count - 1].EnsureVisible();
                cbEnh.Select();
                cbEnh.SelectAll();
            });
        }

        private void btnRDel_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (RecipeID() < 0)
                    return;
                var recipeId = RecipeID();
                var recipeArray = new Recipe[DatabaseAPI.Database.Recipes.Length - 1];
                var recipeCount = -1;
                for (var recipeIdx = 0; recipeIdx <= DatabaseAPI.Database.Recipes.Length - 1; ++recipeIdx)
                {
                    if (recipeIdx == recipeId)
                        continue;
                    ++recipeCount;
                    recipeArray[recipeCount] = new Recipe(ref DatabaseAPI.Database.Recipes[recipeIdx]);
                }

                DatabaseAPI.Database.Recipes = new Recipe[recipeArray.Length - 1 + 1];
                for (var recipeIdx = 0; recipeIdx <= DatabaseAPI.Database.Recipes.Length - 1; ++recipeIdx)
                    DatabaseAPI.Database.Recipes[recipeIdx] = new Recipe(ref recipeArray[recipeIdx]);
                FillList();
                if (lvDPA.Items.Count > recipeId)
                    lvDPA.Items[recipeId].Selected = true;
                else if (lvDPA.Items.Count > recipeId - 1)
                    lvDPA.Items[recipeId - 1].Selected = true;
                else if (lvDPA.Items.Count > 0)
                    lvDPA.Items[0].Selected = true;
            });
        }

        private void btnRunSeq_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                var enhIdx = DatabaseAPI.Database.Recipes[DatabaseAPI.Database.Recipes.Length - 1].EnhIdx;
                for (var index = enhIdx + 1; index <= DatabaseAPI.Database.Enhancements.Length - 1; ++index)
                {
                    if (DatabaseAPI.Database.Enhancements[index].TypeID != Enums.eType.SetO)
                        continue;
                    DatabaseAPI.Database.Recipes = DatabaseAPI.Database.Recipes.Append(new Recipe
                        {
                            EnhIdx = index,
                            Enhancement = DatabaseAPI.Database.Enhancements[index].UID,
                            InternalName = DatabaseAPI.Database.Enhancements[index].UID
                        }
                    ).ToArray();
                    AddListItem(DatabaseAPI.Database.Recipes.Length - 1);
                }

                lvDPA.Items[lvDPA.Items.Count - 1].Selected = true;
                lvDPA.Items[lvDPA.Items.Count - 1].EnsureVisible();
                cbEnh.Select();
                cbEnh.SelectAll();
            });
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                DatabaseAPI.GuessRecipes();
                DatabaseAPI.AssignRecipeIDs();
            });
        }

        private void cbEnh_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() <= -1 || cbEnh.SelectedIndex <= -1)
                    return;
                var recipe = DatabaseAPI.Database.Recipes[RecipeID()];
                recipe.EnhIdx = cbEnh.SelectedIndex - 1;
                if (recipe.EnhIdx > -1)
                {
                    recipe.Enhancement = cbEnh.Text;
                    recipe.InternalName = cbEnh.Text;
                    txtRecipeName.Text = cbEnh.Text;
                    try
                    {
                        lblEnh.Text = DatabaseAPI.Database.Enhancements[recipe.EnhIdx].LongName;
                    }
                    catch (Exception)
                    {
                        lblEnh.Text = string.Empty;
                    }
                }
                else
                {
                    recipe.Enhancement = "";
                }

                UpdateListItem(RecipeID());
            });
        }

        private void cbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() <= -1 || cbRarity.SelectedIndex <= -1)
                    return;
                DatabaseAPI.Database.Recipes[RecipeID()].Rarity = (Recipe.RecipeRarity) cbRarity.SelectedIndex;
                UpdateListItem(RecipeID());
            });
        }

        private void cbSalX_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0)
                    return;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[0] = cbSal0.SelectedIndex - 1;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[1] = cbSal1.SelectedIndex - 1;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[2] = cbSal2.SelectedIndex - 1;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[3] = cbSal3.SelectedIndex - 1;
                DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[4] = cbSal4.SelectedIndex - 1;
                if ((cbSal0.SelectedIndex > 0) & (decimal.Compare(udSal0.Value, new decimal(1)) < 0))
                    udSal0.Value = new decimal(1);
                if ((cbSal1.SelectedIndex > 0) & (decimal.Compare(udSal1.Value, new decimal(1)) < 0))
                    udSal1.Value = new decimal(1);
                if ((cbSal2.SelectedIndex > 0) & (decimal.Compare(udSal2.Value, new decimal(1)) < 0))
                    udSal2.Value = new decimal(1);
                if ((cbSal3.SelectedIndex > 0) & (decimal.Compare(udSal3.Value, new decimal(1)) < 0))
                    udSal3.Value = new decimal(1);
                if ((cbSal4.SelectedIndex > 0) & (decimal.Compare(udSal4.Value, new decimal(1)) < 0))
                    udSal4.Value = new decimal(1);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 0);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 1);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 2);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 3);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 4);
            });
        }

        private void ClearEntryInfo()
        {
            udLevel.Value = new decimal(1);
            udLevel.Enabled = false;
            udBuy.Value = new decimal(1);
            udBuy.Enabled = false;
            udBuyM.Value = new decimal(1);
            udBuyM.Enabled = false;
            udCraft.Value = new decimal(1);
            udCraft.Enabled = false;
            udCraftM.Value = new decimal(1);
            udCraftM.Enabled = false;
            cbSal0.SelectedIndex = -1;
            cbSal0.Enabled = false;
            udSal0.Value = new decimal(0);
            udSal0.Enabled = false;
            cbSal1.SelectedIndex = -1;
            cbSal1.Enabled = false;
            udSal1.Value = new decimal(0);
            udSal1.Enabled = false;
            cbSal2.SelectedIndex = -1;
            cbSal2.Enabled = false;
            udSal2.Value = new decimal(0);
            udSal2.Enabled = false;
            cbSal3.SelectedIndex = -1;
            cbSal3.Enabled = false;
            udSal3.Value = new decimal(0);
            udSal3.Enabled = false;
            cbSal4.SelectedIndex = -1;
            cbSal4.Enabled = false;
            udSal4.Value = new decimal(0);
            udSal4.Enabled = false;
        }

        private void ClearInfo()
        {
            txtRecipeName.Text = "";
            cbEnh.SelectedIndex = -1;
            cbRarity.SelectedIndex = -1;
            lstItems.Items.Clear();
            lblEnh.Text = "";
            txtExtern.Text = "";
            ClearEntryInfo();
        }

        private int EntryID()
        {
            return RecipeID() <= -1 ? -1 : lstItems.SelectedIndex;
        }

        private void FillList()
        {
            lvDPA.BeginUpdate();
            lvDPA.Items.Clear();
            var num = DatabaseAPI.Database.Recipes.Length - 1;
            for (var Index = 0; Index <= num; ++Index)
                AddListItem(Index);
            lvDPA.EndUpdate();
            if (lvDPA.SelectedItems.Count <= 0)
                return;
            lvDPA.Items[0].Selected = true;
        }

        private void frmRecipeEdit_Load(object sender, EventArgs e)
        {
            var recipeRarity = Recipe.RecipeRarity.Common;
            cbRarity.BeginUpdate();
            cbRarity.Items.Clear();
            cbRarity.Items.AddRange(Enum.GetNames(recipeRarity.GetType()));
            cbRarity.EndUpdate();
            cbEnh.BeginUpdate();
            cbEnh.Items.Clear();
            cbEnh.Items.Add("None");
            var num1 = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index = 0; index <= num1; ++index)
                if (DatabaseAPI.Database.Enhancements[index].UID != "")
                    cbEnh.Items.Add(DatabaseAPI.Database.Enhancements[index].UID);
                else
                    cbEnh.Items.Add("X - " + DatabaseAPI.Database.Enhancements[index].Name);
            cbEnh.EndUpdate();
            cbSal0.BeginUpdate();
            cbSal1.BeginUpdate();
            cbSal2.BeginUpdate();
            cbSal3.BeginUpdate();
            cbSal4.BeginUpdate();
            cbSal0.Items.Clear();
            cbSal1.Items.Clear();
            cbSal2.Items.Clear();
            cbSal3.Items.Clear();
            cbSal4.Items.Clear();
            cbSal0.Items.Add("None");
            cbSal1.Items.Add("None");
            cbSal2.Items.Add("None");
            cbSal3.Items.Add("None");
            cbSal4.Items.Add("None");
            var num2 = DatabaseAPI.Database.Salvage.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var salvageName = DatabaseAPI.Database.Salvage[index].ExternalName;
                cbSal0.Items.Add(salvageName);
                cbSal1.Items.Add(salvageName);
                cbSal2.Items.Add(salvageName);
                cbSal3.Items.Add(salvageName);
                cbSal4.Items.Add(salvageName);
            }

            cbSal0.EndUpdate();
            cbSal1.EndUpdate();
            cbSal2.EndUpdate();
            cbSal3.EndUpdate();
            cbSal4.EndUpdate();
            NoUpdate = false;
            FillList();
        }

        private int GetCostByLevel(int iLevelZB)
        {
            int[] numArray =
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 3600, 4380, 5160,
                5940, 6720, 7500, 12900,
                18300, 23700, 29100, 34500,
                35080, 35660, 36240, 36820,
                37400, 38660, 39920, 41180,
                42440, 43700, 48200, 52700,
                57200, 61700, 66200, 73920,
                81640, 89360, 97080, 104800,
                121260, 137720, 154180, 170640,
                187100, 198720, 210340, 221960,
                233580, 490400, 513640, 536880,
                560120
            };
            return iLevelZB >= 0 ? iLevelZB <= numArray.Length - 1 ? numArray[iLevelZB] : 0 : 0;
        }

        private void IncrementX(int nMax)
        {
            if (RecipeID() < 0 || (DatabaseAPI.Database.Recipes[RecipeID()].Item.Length < 1) |
                (DatabaseAPI.Database.Recipes[RecipeID()].Item.Length > 53))
                return;
            var num = DatabaseAPI.Database.Recipes[RecipeID()]
                .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1].Level + 1;
            if (num >= nMax)
                return;
            for (var index = num; index <= nMax; ++index)
            {
                DatabaseAPI.Database.Recipes[RecipeID()].Item = (Recipe.RecipeEntry[]) Utils.CopyArray(
                    DatabaseAPI.Database.Recipes[RecipeID()].Item,
                    new Recipe.RecipeEntry[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length + 1]);
                DatabaseAPI.Database.Recipes[RecipeID()]
                        .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1] =
                    new Recipe.RecipeEntry(DatabaseAPI.Database.Recipes[RecipeID()]
                        .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 2]) {Level = index};
                DatabaseAPI.Database.Recipes[RecipeID()].Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1]
                    .CraftCost = GetCostByLevel(DatabaseAPI.Database.Recipes[RecipeID()]
                    .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1].Level);
            }

            ShowRecipeInfo(RecipeID());
            lstItems.SelectedIndex = lstItems.Items.Count - 1;
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (lvDPA.SelectedIndices.Count > 0)
                    ShowEntryInfo(lvDPA.SelectedIndices[0], lstItems.SelectedIndex);
                else
                    ClearEntryInfo();
            });
        }

        private void lvDPA_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (lvDPA.SelectedIndices.Count > 0)
                    ShowRecipeInfo(lvDPA.SelectedIndices[0]);
                else
                    ClearInfo();
            });
        }

        private int MinMax(int iValue, NumericUpDown iControl)
        {
            if (decimal.Compare(new decimal(iValue), iControl.Minimum) < 0)
                iValue = Convert.ToInt32(iControl.Minimum);
            if (decimal.Compare(new decimal(iValue), iControl.Maximum) > 0)
                iValue = Convert.ToInt32(iControl.Maximum);
            return iValue;
        }

        private int RecipeID()
        {
            return lvDPA.SelectedIndices.Count <= 0 ? -1 : lvDPA.SelectedIndices[0];
        }

        private void SetSalvageStringFromIDX(int iRecipe, int iItem, int iIndex)
        {
            if (DatabaseAPI.Database.Recipes[iRecipe].Item[iItem].SalvageIdx[iIndex] > -1)
                DatabaseAPI.Database.Recipes[iRecipe].Item[iItem].Salvage[iIndex] = DatabaseAPI.Database
                    .Salvage[DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[iIndex]].InternalName;
            else
                DatabaseAPI.Database.Recipes[iRecipe].Item[iItem].Salvage[iIndex] = "";
        }

        private void ShowEntryInfo(int rIDX, int iIDX)
        {
            if ((rIDX < 0) | (rIDX > DatabaseAPI.Database.Recipes.Length - 1))
            {
                ClearEntryInfo();
            }
            else if ((iIDX < 0) | (iIDX > DatabaseAPI.Database.Recipes[rIDX].Item.Length - 1))
            {
                ClearEntryInfo();
            }
            else
            {
                NoUpdate = true;
                var recipeEntry = DatabaseAPI.Database.Recipes[rIDX].Item[iIDX];
                udLevel.Value = new decimal(recipeEntry.Level + 1);
                udLevel.Enabled = true;
                udBuy.Value = new decimal(recipeEntry.BuyCost);
                udBuy.Enabled = true;
                udBuyM.Value = new decimal(recipeEntry.BuyCostM);
                udBuyM.Enabled = true;
                udCraft.Value = new decimal(recipeEntry.CraftCost);
                udCraft.Enabled = true;
                udCraftM.Value = new decimal(recipeEntry.CraftCostM);
                udCraftM.Enabled = true;
                cbSal0.SelectedIndex = recipeEntry.SalvageIdx[0] + 1;
                cbSal0.Enabled = true;
                udSal0.Value = new decimal(recipeEntry.Count[0]);
                udSal0.Enabled = true;
                cbSal1.SelectedIndex = recipeEntry.SalvageIdx[1] + 1;
                cbSal1.Enabled = true;
                udSal1.Value = new decimal(recipeEntry.Count[1]);
                udSal1.Enabled = true;
                cbSal2.SelectedIndex = recipeEntry.SalvageIdx[2] + 1;
                cbSal2.Enabled = true;
                udSal2.Value = new decimal(recipeEntry.Count[2]);
                udSal2.Enabled = true;
                cbSal3.SelectedIndex = recipeEntry.SalvageIdx[3] + 1;
                cbSal3.Enabled = true;
                udSal3.Value = new decimal(recipeEntry.Count[3]);
                udSal3.Enabled = true;
                cbSal4.SelectedIndex = recipeEntry.SalvageIdx[4] + 1;
                cbSal4.Enabled = true;
                udSal4.Value = new decimal(recipeEntry.Count[4]);
                udSal4.Enabled = true;
                NoUpdate = false;
            }
        }

        private void ShowRecipeInfo(int Index)
        {
            if ((Index < 0) | (Index > DatabaseAPI.Database.Recipes.Length - 1))
            {
                ClearInfo();
            }
            else
            {
                NoUpdate = true;
                txtRecipeName.Text = DatabaseAPI.Database.Recipes[Index].InternalName;
                cbEnh.SelectedIndex = DatabaseAPI.Database.Recipes[Index].EnhIdx + 1;
                try
                {
                    lblEnh.Text = DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.Recipes[Index].EnhIdx]
                        .LongName;
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    lblEnh.Text = string.Empty;
                    ProjectData.ClearProjectError();
                }

                cbRarity.SelectedIndex = (int) DatabaseAPI.Database.Recipes[Index].Rarity;
                txtExtern.Text = DatabaseAPI.Database.Recipes[Index].ExternalName;
                lstItems.Items.Clear();
                var num = DatabaseAPI.Database.Recipes[Index].Item.Length - 1;
                for (var index = 0; index <= num; ++index)
                    lstItems.Items.Add("Level: " +
                                       Convert.ToString(DatabaseAPI.Database.Recipes[Index].Item[index].Level + 1));
                if (lstItems.Items.Count > 0)
                    lstItems.SelectedIndex = 0;
                NoUpdate = false;
            }
        }

        private void txtExtern_TextChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() <= -1)
                    return;
                DatabaseAPI.Database.Recipes[RecipeID()].ExternalName = txtExtern.Text;
            });
        }

        private void txtRecipeName_TextChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() <= -1)
                    return;
                DatabaseAPI.Database.Recipes[RecipeID()].InternalName = txtRecipeName.Text;
                UpdateListItem(RecipeID());
            });
        }

        private void udCostX_Leave(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0)
                    return;
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                recipeItem.Level =
                    MinMax((int) Math.Round(Conversion.Val(udLevel.Text.Replace(",", "").Replace(".", ""))), udLevel) -
                    1;
                recipeItem.BuyCost =
                    MinMax((int) Math.Round(Conversion.Val(udBuy.Text.Replace(",", "").Replace(".", ""))), udBuy);
                recipeItem.BuyCostM =
                    MinMax((int) Math.Round(Conversion.Val(udBuyM.Text.Replace(",", "").Replace(".", ""))), udBuyM);
                recipeItem.CraftCost =
                    MinMax((int) Math.Round(Conversion.Val(udCraft.Text.Replace(",", "").Replace(".", ""))), udCraft);
                recipeItem.CraftCostM =
                    MinMax((int) Math.Round(Conversion.Val(udCraftM.Text.Replace(",", "").Replace(".", ""))), udCraftM);
            });
        }

        private void udCostX_ValueChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0)
                    return;
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                recipeItem.Level = Convert.ToInt32(decimal.Subtract(udLevel.Value, new decimal(1)));
                recipeItem.BuyCost = Convert.ToInt32(udBuy.Value);
                recipeItem.BuyCostM = Convert.ToInt32(udBuyM.Value);
                recipeItem.CraftCost = Convert.ToInt32(udCraft.Value);
                recipeItem.CraftCostM = Convert.ToInt32(udCraftM.Value);
            });
        }

        private void udSalX_Leave(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                recipeItem.Count[0] = MinMax((int) Math.Round(Conversion.Val(udSal0.Text)), udSal0);
                recipeItem.Count[1] = MinMax((int) Math.Round(Conversion.Val(udSal1.Text)), udSal1);
                recipeItem.Count[2] = MinMax((int) Math.Round(Conversion.Val(udSal2.Text)), udSal2);
                recipeItem.Count[3] = MinMax((int) Math.Round(Conversion.Val(udSal3.Text)), udSal3);
                recipeItem.Count[4] = MinMax((int) Math.Round(Conversion.Val(udSal4.Text)), udSal4);
            });
        }

        private void udSalX_ValueChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0)
                    return;
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                recipeItem.Count[0] = Convert.ToInt32(udSal0.Value);
                recipeItem.Count[1] = Convert.ToInt32(udSal1.Value);
                recipeItem.Count[2] = Convert.ToInt32(udSal2.Value);
                recipeItem.Count[3] = Convert.ToInt32(udSal3.Value);
                recipeItem.Count[4] = Convert.ToInt32(udSal4.Value);
            });
        }

        private void UpdateListItem(int index)
        {
            if (!((index > -1) & (index < DatabaseAPI.Database.Recipes.Length)))
                return;
            lvDPA.Items[index].SubItems[0].Text = DatabaseAPI.Database.Recipes[index].InternalName;
            lvDPA.Items[index].SubItems[1].Text = DatabaseAPI.Database.Recipes[index].EnhIdx <= -1
                ? "None"
                : DatabaseAPI.Database.Recipes[index].Enhancement + " (" +
                  Convert.ToString(DatabaseAPI.Database.Recipes[index].EnhIdx) + ")";
            lvDPA.Items[index].SubItems[2].Text = Enum.GetName(DatabaseAPI.Database.Recipes[index].Rarity.GetType(),
                DatabaseAPI.Database.Recipes[index].Rarity);
            lvDPA.Items[index].SubItems[3].Text = Convert.ToString(DatabaseAPI.Database.Recipes[index].Item.Length,
                CultureInfo.InvariantCulture);
        }
    }
}