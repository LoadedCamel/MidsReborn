using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Hero_Designer.My;
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
            Icon = Resources.reborn;
        }

        private void AddListItem(int Index)
        {
            if (!((Index > -1) & (Index < DatabaseAPI.Database.Recipes.Length))) return;
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
            for (var index = 0; index < DatabaseAPI.Database.Recipes.Length; index++)
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
                lstItems.SelectedIndex = lstItems.Items.Count - 1;
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
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0) return;
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
                for (var recipeIdx = 0; recipeIdx < DatabaseAPI.Database.Recipes.Length; recipeIdx++)
                {
                    if (recipeIdx == recipeId)
                        continue;
                    ++recipeCount;
                    recipeArray[recipeCount] = new Recipe(ref DatabaseAPI.Database.Recipes[recipeIdx]);
                }

                DatabaseAPI.Database.Recipes = new Recipe[recipeArray.Length - 1 + 1];
                for (var recipeIdx = 0; recipeIdx < DatabaseAPI.Database.Recipes.Length; recipeIdx++)
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
                for (var index = enhIdx + 1; index < DatabaseAPI.Database.Enhancements.Length; index++)
                {
                    if (DatabaseAPI.Database.Enhancements[index].TypeID != Enums.eType.SetO) continue;
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
                if (NoUpdate || RecipeID() <= -1 || cbEnh.SelectedIndex <= -1) return;
                var recipe = DatabaseAPI.Database.Recipes[RecipeID()];
                recipe.EnhIdx = (cbEnh.SelectedText.ToLower() == "none") ? -1 : cbEnh.SelectedIndex - 1;
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
                if (NoUpdate || RecipeID() <= -1 || cbRarity.SelectedIndex <= -1) return;
                DatabaseAPI.Database.Recipes[RecipeID()].Rarity = (Recipe.RecipeRarity) cbRarity.SelectedIndex;
                UpdateListItem(RecipeID());
            });
        }

        private void cbSalX_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0) return;
                if (cbIsRecipe0.Checked)
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[0] = cbSal0.SelectedIndex - 1;
                }
                else
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[0] = cbSal0.SelectedIndex - 1;
                }

                if (cbIsRecipe1.Checked)
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[1] = cbSal1.SelectedIndex - 1;
                }
                else
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[1] = cbSal1.SelectedIndex - 1;
                }

                if (cbIsRecipe2.Checked)
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[2] = cbSal2.SelectedIndex - 1;
                }
                else
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[2] = cbSal2.SelectedIndex - 1;
                }

                if (cbIsRecipe3.Checked)
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[3] = cbSal3.SelectedIndex - 1;
                }
                else
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[3] = cbSal3.SelectedIndex - 1;
                }

                if (cbIsRecipe4.Checked)
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[4] = cbSal4.SelectedIndex - 1;
                }
                else
                {
                    DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[4] = cbSal4.SelectedIndex - 1;
                }
                /*if ((cbSal0.SelectedIndex > 0) & (decimal.Compare(udSal0.Value, new decimal(1)) < 0))
                    udSal0.Value = new decimal(1);
                if ((cbSal1.SelectedIndex > 0) & (decimal.Compare(udSal1.Value, new decimal(1)) < 0))
                    udSal1.Value = new decimal(1);
                if ((cbSal2.SelectedIndex > 0) & (decimal.Compare(udSal2.Value, new decimal(1)) < 0))
                    udSal2.Value = new decimal(1);
                if ((cbSal3.SelectedIndex > 0) & (decimal.Compare(udSal3.Value, new decimal(1)) < 0))
                    udSal3.Value = new decimal(1);
                if ((cbSal4.SelectedIndex > 0) & (decimal.Compare(udSal4.Value, new decimal(1)) < 0))
                    udSal4.Value = new decimal(1);
                */
                udSal0.Value = (cbSal0.SelectedIndex > 0) ? Math.Min(udSal0.Value, 1) : udSal0.Value;
                udSal1.Value = (cbSal1.SelectedIndex > 0) ? Math.Min(udSal1.Value, 1) : udSal1.Value;
                udSal2.Value = (cbSal2.SelectedIndex > 0) ? Math.Min(udSal2.Value, 1) : udSal2.Value;
                udSal3.Value = (cbSal3.SelectedIndex > 0) ? Math.Min(udSal3.Value, 1) : udSal3.Value;
                udSal4.Value = (cbSal4.SelectedIndex > 0) ? Math.Min(udSal4.Value, 1) : udSal4.Value;
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 0);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 1);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 2);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 3);
                SetSalvageStringFromIDX(RecipeID(), EntryID(), 4);
            });
        }

        private void ClearEntryInfo()
        {
            udLevel.Value = 1;
            udLevel.Enabled = false;
            udBuy.Value = 1;
            udBuy.Enabled = false;
            udBuyM.Value = 1;
            udBuyM.Enabled = false;
            udCraft.Value = 1;
            udCraft.Enabled = false;
            udCraftM.Value = 1;
            udCraftM.Enabled = false;
            cbSal0.SelectedIndex = -1;
            cbSal0.Enabled = false;
            udSal0.Value = 0;
            udSal0.Enabled = false;
            cbSal1.SelectedIndex = -1;
            cbSal1.Enabled = false;
            udSal1.Value = 0;
            udSal1.Enabled = false;
            cbSal2.SelectedIndex = -1;
            cbSal2.Enabled = false;
            udSal2.Value = 0;
            udSal2.Enabled = false;
            cbSal3.SelectedIndex = -1;
            cbSal3.Enabled = false;
            udSal3.Value = 0;
            udSal3.Enabled = false;
            cbSal4.SelectedIndex = -1;
            cbSal4.Enabled = false;
            udSal4.Value = 0;
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
            cbIsGeneric.Checked = false;
            cbIsVirtual.Checked = false;
            cbIsHidden.Checked = false;
            Label2.Visible = true;
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
            for (var index = 0; index < DatabaseAPI.Database.Recipes.Length; index++)
            {
                AddListItem(index);
            }

            lvDPA.EndUpdate();
            if (lvDPA.SelectedItems.Count <= 0) return;
            lvDPA.Items[0].Selected = true;
        }

        private void frmRecipeEdit_Load(object sender, EventArgs e)
        {
            var recipeRarity = Recipe.RecipeRarity.Common;
            cbRarity.BeginUpdate();
            cbRarity.Items.Clear();
            cbRarity.Items.AddRange(Enum.GetNames(recipeRarity.GetType()).Cast<object>().ToArray());
            cbRarity.EndUpdate();
            cbEnh.BeginUpdate();
            cbEnh.Items.Clear();
            cbEnh.Items.Add("None");
            foreach (var enh in DatabaseAPI.Database.Enhancements)
            {
                cbEnh.Items.Add(enh.UID != "" ? enh.UID : "X - " + enh.Name);
            }

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
            foreach (var slv in DatabaseAPI.Database.Salvage)
            {
                var salvageName = slv.ExternalName;
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
            return iLevelZB >= 0 ? iLevelZB < numArray.Length ? numArray[iLevelZB] : 0 : 0;
        }

        private void IncrementX(int nMax)
        {
            if (RecipeID() < 0 || (DatabaseAPI.Database.Recipes[RecipeID()].Item.Length < 1) |
                (DatabaseAPI.Database.Recipes[RecipeID()].Item.Length > 53))
                return;
            var num = DatabaseAPI.Database.Recipes[RecipeID()]
                .Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1].Level + 1;
            if (num >= nMax) return;
            for (var index = num; index < nMax; index++)
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
                {
                    var recipeItem = lvDPA.SelectedItems[0].Text;
                    var rIndex = Array.FindIndex(DatabaseAPI.Database.Recipes, 0, x => x.InternalName == recipeItem);
                    ShowEntryInfo(rIndex, lstItems.SelectedIndex);
                }
                else
                {
                    ClearEntryInfo();
                }
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
            /*if (decimal.Compare(new decimal(iValue), iControl.Minimum) < 0)
                iValue = Convert.ToInt32(iControl.Minimum);
            if (decimal.Compare(new decimal(iValue), iControl.Maximum) > 0)
                iValue = Convert.ToInt32(iControl.Maximum);
            */
            return (int) Math.Max(iControl.Minimum, Math.Min(iControl.Maximum, iValue));
        }

        private int RecipeID()
        {
            if (lvDPA.SelectedIndices.Count <= 0)
            {
                return -1;
            }

            var recipeItem = lvDPA.SelectedItems[0].Text;
            var rIndex = Array.FindIndex(DatabaseAPI.Database.Recipes, 0, x => x.InternalName == recipeItem);
            return lvDPA.SelectedIndices[0];
        }

        private void SetSalvageStringFromIDX(int iRecipe, int iItem, int iIndex)
        {
            if (DatabaseAPI.Database.Recipes[iRecipe].Item[iItem].SalvageIdx[iIndex] <= -1)
            {
                DatabaseAPI.Database.Recipes[iRecipe].Item[iItem].Salvage[iIndex] = "";
            }
            else
            {
                DatabaseAPI.Database.Recipes[iRecipe].Item[iItem].Salvage[iIndex] = DatabaseAPI.Database.Salvage[DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[iIndex]].InternalName;
                Console.WriteLine(DatabaseAPI.Database.Salvage[DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[iIndex]].InternalName);
            }
        }

        private void ShowEntryInfo(int rIDX, int iIDX) // edit this
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
                udLevel.Value = recipeEntry.Level + 1;
                udLevel.Enabled = true;
                udBuy.Value = recipeEntry.BuyCost;
                udBuy.Enabled = true;
                udBuyM.Value = recipeEntry.BuyCostM;
                udBuyM.Enabled = true;
                udCraft.Value = recipeEntry.CraftCost;
                udCraft.Enabled = true;
                udCraftM.Value = recipeEntry.CraftCostM;
                udCraftM.Enabled = true;
                cbSal0.SelectedIndex = recipeEntry.SalvageIdx[0] + 1;
                cbSal0.Enabled = true;
                udSal0.Value = recipeEntry.Count[0];
                udSal0.Enabled = true;
                cbSal1.SelectedIndex = recipeEntry.SalvageIdx[1] + 1;
                cbSal1.Enabled = true;
                udSal1.Value = recipeEntry.Count[1];
                udSal1.Enabled = true;
                cbSal2.SelectedIndex = recipeEntry.SalvageIdx[2] + 1;
                cbSal2.Enabled = true;
                udSal2.Value = recipeEntry.Count[2];
                udSal2.Enabled = true;
                cbSal3.SelectedIndex = recipeEntry.SalvageIdx[3] + 1;
                cbSal3.Enabled = true;
                udSal3.Value = recipeEntry.Count[3];
                udSal3.Enabled = true;
                cbSal4.SelectedIndex = recipeEntry.SalvageIdx[4] + 1;
                cbSal4.Enabled = true;
                udSal4.Value = recipeEntry.Count[4];
                udSal4.Enabled = true;
                NoUpdate = false;
            }
        }

        private void ShowRecipeInfo(int index)
        {
            var recipeItem = lvDPA.SelectedItems[0].Text;
            var rIndex = Array.FindIndex(DatabaseAPI.Database.Recipes, 0, x => x.InternalName == recipeItem);
            if ((rIndex < 0) | (rIndex > DatabaseAPI.Database.Recipes.Length - 1))
            {
                ClearInfo();
            }
            else
            {
                NoUpdate = true;
                txtRecipeName.Text = DatabaseAPI.Database.Recipes[rIndex].InternalName;
                var enhIdx = Array.FindIndex(DatabaseAPI.Database.Enhancements, 0,
                    e => e.RecipeName == DatabaseAPI.Database.Recipes[rIndex].InternalName);
                if (enhIdx > -1)
                {
                    var recipeEnhIdx = cbEnh.Items.IndexOf(DatabaseAPI.Database.Enhancements[enhIdx].UID);
                    cbEnh.SelectedIndex = recipeEnhIdx;
                    lblEnh.Text = DatabaseAPI.Database.Enhancements[enhIdx].LongName;
                }
                else
                {
                    cbEnh.SelectedIndex = -1;
                    lblEnh.Text = "";
                }

                cbRarity.SelectedIndex = (int) DatabaseAPI.Database.Recipes[rIndex].Rarity;
                txtExtern.Text = DatabaseAPI.Database.Recipes[rIndex].ExternalName;
                cbIsGeneric.Checked = DatabaseAPI.Database.Recipes[rIndex].IsGeneric;
                cbIsVirtual.Checked = DatabaseAPI.Database.Recipes[rIndex].IsVirtual;
                cbIsHidden.Checked = DatabaseAPI.Database.Recipes[rIndex].IsHidden;
                Label2.Visible = !cbIsGeneric.Checked;
                cbEnh.Visible = !cbIsGeneric.Checked;
                lblEnh.Visible = !cbIsGeneric.Checked & enhIdx > -1;

                lstItems.Items.Clear();
                foreach (var r in DatabaseAPI.Database.Recipes[rIndex].Item)
                {
                    lstItems.Items.Add($"Level: {r.Level + 1}");
                }

                if (lstItems.Items.Count > 0) lstItems.SelectedIndex = 0;
                NoUpdate = false;
            }
        }

        private void txtExtern_TextChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() <= -1) return;
                DatabaseAPI.Database.Recipes[RecipeID()].ExternalName = txtExtern.Text;
            });
        }

        private void txtRecipeName_TextChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() <= -1) return;
                DatabaseAPI.Database.Recipes[RecipeID()].InternalName = txtRecipeName.Text;
                UpdateListItem(RecipeID());
            });
        }

        private void udCostX_Leave(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0) return;
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                recipeItem.Level = MinMax(Convert.ToInt32(Regex.Replace(udLevel.Text, @"[\.\,]", "")), udLevel) - 1;
                recipeItem.BuyCost = MinMax(Convert.ToInt32(Regex.Replace(udBuy.Text, @"[\.\,]", "")), udBuy);
                recipeItem.BuyCostM = MinMax(Convert.ToInt32(Regex.Replace(udBuyM.Text, @"[\.\,]", "")), udBuyM);
                recipeItem.CraftCost = MinMax(Convert.ToInt32(Regex.Replace(udBuy.Text, @"[\.\,]", "")), udCraft);
                recipeItem.CraftCostM = MinMax(Convert.ToInt32(Regex.Replace(udBuy.Text, @"[\.\,]", "")), udCraftM);
            });
        }

        private void udCostX_ValueChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0) return;
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                var level = Convert.ToInt32(udLevel.Value);
                recipeItem.Level = level - 1;
                recipeItem.BuyCost = Convert.ToInt32(udBuy.Value);
                recipeItem.BuyCostM = Convert.ToInt32(udBuyM.Value);
                recipeItem.CraftCost = Convert.ToInt32(udCraft.Value);
                recipeItem.CraftCostM = Convert.ToInt32(udCraftM.Value);

                if (level >= 0 & level <= 50) lstItems.SelectedItem = $"Level: {level}";
            });
        }

        private void udSalX_Leave(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
                recipeItem.Count[0] = MinMax(Convert.ToInt32(udSal0.Text), udSal0);
                recipeItem.Count[1] = MinMax(Convert.ToInt32(udSal1.Text), udSal1);
                recipeItem.Count[2] = MinMax(Convert.ToInt32(udSal2.Text), udSal2);
                recipeItem.Count[3] = MinMax(Convert.ToInt32(udSal3.Text), udSal3);
                recipeItem.Count[4] = MinMax(Convert.ToInt32(udSal4.Text), udSal4);
            });
        }

        private void udSalX_ValueChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (NoUpdate || RecipeID() < 0 || EntryID() < 0) return;
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
            if (!((index > -1) & (index < DatabaseAPI.Database.Recipes.Length))) return;
            lvDPA.Items[index].SubItems[0].Text = DatabaseAPI.Database.Recipes[index].InternalName;
            lvDPA.Items[index].SubItems[1].Text = DatabaseAPI.Database.Recipes[index].EnhIdx <= -1
                ? "None"
                : $"{DatabaseAPI.Database.Recipes[index].Enhancement} ({DatabaseAPI.Database.Recipes[index].EnhIdx})";
            lvDPA.Items[index].SubItems[2].Text = Enum.GetName(DatabaseAPI.Database.Recipes[index].Rarity.GetType(),
                DatabaseAPI.Database.Recipes[index].Rarity);
            lvDPA.Items[index].SubItems[3].Text = Convert.ToString(DatabaseAPI.Database.Recipes[index].Item.Length,
                CultureInfo.InvariantCulture);
        }

        private void cbIsRecipe_CheckedChanged(object sender, EventArgs e)
        {
            var target = sender as CheckBox;
            var state = target != null && target.Checked;

            switch (target.Name)
            {
                case "cbIsRecipe0":
                    PopulateComboBoxList(ref cbSal0, !state);
                    cbSal0.SelectedIndex = 0;
                    udSal0.Visible = !state;
                    if (state)
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[0] = 0;
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[0] = 0;
                    }

                    break;

                case "cbIsRecipe1":
                    PopulateComboBoxList(ref cbSal1, !state);
                    cbSal1.SelectedIndex = 0;
                    udSal1.Visible = !state;
                    if (state)
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[1] = 0;
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[1] = 0;
                    }

                    break;

                case "cbIsRecipe2":
                    PopulateComboBoxList(ref cbSal2, !state);
                    cbSal2.SelectedIndex = 0;
                    udSal2.Visible = !state;
                    if (state)
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[2] = 0;
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[2] = 0;
                    }

                    break;

                case "cbIsRecipe3":
                    PopulateComboBoxList(ref cbSal3, !state);
                    cbSal3.SelectedIndex = 0;
                    udSal3.Visible = !state;
                    if (state)
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[3] = 0;
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[3] = 0;
                    }

                    break;

                case "cbIsRecipe4":
                    PopulateComboBoxList(ref cbSal4, !state);
                    cbSal4.SelectedIndex = 0;
                    udSal4.Visible = !state;
                    if (state)
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].SalvageIdx[4] = 0;
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[4] = 0;
                    }

                    break;
            }
        }

        private void PopulateComboBoxList(ref ComboBox ctl, bool withSalvage = true)
        {
            ctl.BeginUpdate();
            ctl.Items.Clear();
            ctl.Items.Add("None");
            if (withSalvage)
            {
                foreach (var slv in DatabaseAPI.Database.Salvage)
                {
                    ctl.Items.Add(slv.ExternalName);
                }
            }
            else
            {
                foreach (var re in DatabaseAPI.Database.Recipes)
                {
                    ctl.Items.Add(re.ExternalName.Trim() == "Nothing" ? re.InternalName : $"{re.ExternalName.Trim()} [{re.InternalName}]");
                }
            }
            ctl.EndUpdate();
        }

        private void cbIsGeneric_CheckedChanged(object sender, EventArgs e)
        {
            var state = sender is CheckBox target && target.Checked;
            DatabaseAPI.Database.Recipes[RecipeID()].IsGeneric = state;
            cbEnh.Visible = !state;
            lblEnh.Visible = !state;
            cbEnh.SelectedIndex = state ? -1 : cbEnh.SelectedIndex;
            Label2.Visible = !state;
        }

        private void cbIsVirtual_CheckedChanged(object sender, EventArgs e)
        {
            var state = sender is CheckBox target && target.Checked;
            DatabaseAPI.Database.Recipes[RecipeID()].IsVirtual = state;
        }

        private void cbIsHidden_CheckedChanged(object sender, EventArgs e)
        {
            var state = sender is CheckBox target && target.Checked;
            DatabaseAPI.Database.Recipes[RecipeID()].IsHidden = state;
        }
    }
}