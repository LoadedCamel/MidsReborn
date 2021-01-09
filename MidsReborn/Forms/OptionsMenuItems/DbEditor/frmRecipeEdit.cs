using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.Forms.WindowMenuItems;
using Mids_Reborn.My;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmRecipeEdit : Form
    {
        private bool _noUpdate;
        private ListViewColumnSorter _lvColumnSorter;
        private int _noEnhancementIdx;

        #region ListView column sorter

        // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/sort-listview-by-column
        private class ListViewColumnSorter : IComparer
        {
            public int Column;
            public SortOrder SortDir;
            private CaseInsensitiveComparer ObjectCompare;
            private string[] _recipeRaritiesWithBlank;

            public ListViewColumnSorter()
            {
                Column = 1;
                SortDir = SortOrder.Ascending;
                ObjectCompare = new CaseInsensitiveComparer();
                var raritiesList = new List<string> { "" };
                raritiesList.AddRange(Enum.GetNames(typeof(Recipe.RecipeRarity)));
                _recipeRaritiesWithBlank = raritiesList.ToArray();
            }

            public int Compare(object a, object b)
            {
                if (a == null) return -1;
                if (b == null) return 1;

                var lvA = (ListViewItem) a;
                var lvB = (ListViewItem) b;
                
                var compareResult = Column switch
                {
                    1 => ObjectCompare.Compare(
                        Convert.ToInt32(lvA.SubItems[Column].Text),
                        Convert.ToInt32(lvB.SubItems[Column].Text)),
                    3 => ObjectCompare.Compare(
                        Array.IndexOf(_recipeRaritiesWithBlank, lvA.SubItems[Column].Text),
                        Array.IndexOf(_recipeRaritiesWithBlank, lvB.SubItems[Column].Text)),
                    4 => ObjectCompare.Compare(
                        Convert.ToInt32(lvA.SubItems[Column].Text),
                        Convert.ToInt32(lvB.SubItems[Column].Text)),
                    _ => ObjectCompare.Compare(lvA.SubItems[Column].Text, lvB.SubItems[Column].Text),
                };

                return SortDir switch
                {
                    SortOrder.Ascending => compareResult,
                    SortOrder.Descending => -compareResult,
                    _ => 0
                };
            }
        }

        #endregion ListView column sorter

        public frmRecipeEdit()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Load += frmRecipeEdit_Load;
            _noUpdate = true;
            InitializeComponent();
            //var componentResourceManager = new ComponentResourceManager(typeof(frmRecipeEdit));
            Icon = Resources.reborn;
            FillList();
        }

        private void AddListItem(int index)
        {
            if (!((index > -1) & (index < DatabaseAPI.Database.Recipes.Length))) return;
            var recipe = DatabaseAPI.Database.Recipes[index];
            lvDPA.Items.Add(new ListViewItem(new[]
            {
                recipe.InternalName,
                Convert.ToString(index),
                recipe.EnhIdx <= -1 ? "None" : $"{recipe.Enhancement} ({recipe.EnhIdx})",
                Enum.GetName(recipe.Rarity.GetType(), recipe.Rarity),
                Convert.ToString(recipe.Item.Length),
                GetRecipeFlags(index)
            }));
        }

        private bool CheckIndexesConsistency(bool silent = false)
        {
            var k = 0;
            var idxList = new List<int>();

            for (var i = 0; i < lvDPA.Items.Count; i++)
            {
                idxList.Add(Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text));
            }
            
            idxList.Sort();
            for (var i = 0; i < idxList.Count; i++)
            {
                if (i == k++) continue;
                if (!silent)
                {
                    MessageBox.Show(
                        $"Warning: Recipe {DatabaseAPI.Database.Recipes[i].InternalName} has index {i} (should be {k})",
                        "Inconsistent index detected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return false;
            }

            return true;
        }

        private void AssignRecipeIndexes(bool async = true)
        {
            var t = new Task(() =>
            {
                var recipesArr = new Recipe[DatabaseAPI.Database.Recipes.Length];
                for (var i = 0; i < lvDPA.Items.Count; i++)
                {
                    var index = Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text);
                    recipesArr[index] = DatabaseAPI.Database.Recipes.First(r => r.InternalName == lvDPA.Items[i].SubItems[0].Text);
                }

                DatabaseAPI.Database.Recipes = (Recipe[]) recipesArr.Clone();
            });
            
            t.Start();
            if (!async) t.Wait();
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
            if (RecipeID() < 0) return;
            var recipe = DatabaseAPI.Database.Recipes[RecipeID()];
            recipe.Item = recipe.Item.Append(new Recipe.RecipeEntry
            {
                Level = 9
            }).ToArray();
            ShowRecipeInfo(RecipeID());
            UpdateListItem(RecipeID());
            lstItems.SelectedIndex = lstItems.Items.Count - 1;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DatabaseAPI.LoadRecipes();
            Close();
        }

        // This delete button is supposed to remove a specific level of recipe, not the whole recipe.
        private void btnDel_Click(object sender, EventArgs e)
        {
            var recipeId = RecipeID();
            if (recipeId < 0 || lstItems.SelectedIndex < 0 || lstItems.Items.Count < 2)
                return;
            var recipe = DatabaseAPI.Database.Recipes[recipeId];
            var recipeItemId = lstItems.SelectedIndex;
            recipe.Item = recipe.Item.Where((ri, i) => i != recipeItemId).ToArray();
            ShowRecipeInfo(RecipeID());
        }

        private void btnGuessCost_Click(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() < 0 || EntryID() < 0) return;
            DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].CraftCost =
                GetCostByLevel(DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].Level);
            udCraft.Value = new decimal(DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].CraftCost);
        }

        private void btnI20_Click(object sender, EventArgs e)
        {
            IncrementX(19);
        }

        private void btnI25_Click(object sender, EventArgs e)
        {
            IncrementX(24);
        }

        private void btnI40_Click(object sender, EventArgs e)
        {
            IncrementX(39);
        }

        private void btnI50_Click(object sender, EventArgs e)
        {
            IncrementX(49);
        }

        private void btnIncrement_Click(object sender, EventArgs e)
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
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckIndexesConsistency()) return;
            
            AssignRecipeIndexes(false);
            AssignNewRecipes();
            DatabaseAPI.AssignRecipeSalvageIDs();
            DatabaseAPI.AssignRecipeIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveRecipes(serializer);
            DatabaseAPI.SaveEnhancementDb(serializer);
            Close();
        }

        private void btnRAdd_Click(object sender, EventArgs e)
        {
            var recipe = new Recipe();
            recipe.Item = recipe.Item.Append(new Recipe.RecipeEntry
            {
                Level = 9
            }).ToArray();
            DatabaseAPI.Database.Recipes = DatabaseAPI.Database.Recipes.Append(recipe).ToArray();
            var rIndex = DatabaseAPI.Database.Recipes.Length - 1;
            Debug.WriteLine($"New recipe index: {rIndex}");
            AddListItem(rIndex);
            UpdateListItem(rIndex);
            udStaticIndex.Value = rIndex;
            lvDPA.Items[lvDPA.Items.Count - 1].Selected = true;
            lvDPA.Items[lvDPA.Items.Count - 1].EnsureVisible();
            cbRarity.SelectedIndex = 0;
            cbEnh.SelectedIndex = _noEnhancementIdx;
            lblEnh.Visible = false;
            
            cbEnh.Select();
        }

        private void btnRDel_Click(object sender, EventArgs e)
        {
            if (RecipeID() < 0) return;
            var recipeId = RecipeID();
            var recipeArray = new Recipe[DatabaseAPI.Database.Recipes.Length - 1];
            var recipeCount = 0;
            for (var recipeIdx = 0; recipeIdx < DatabaseAPI.Database.Recipes.Length; recipeIdx++)
            {
                if (recipeIdx == recipeId) continue;
                recipeArray[recipeCount++] = new Recipe(ref DatabaseAPI.Database.Recipes[recipeIdx]);
            }

            /*DatabaseAPI.Database.Recipes = new Recipe[recipeArray.Length];
            for (var recipeIdx = 0; recipeIdx < DatabaseAPI.Database.Recipes.Length; recipeIdx++)
            {
                DatabaseAPI.Database.Recipes[recipeIdx] = new Recipe(ref recipeArray[recipeIdx]);
            }*/

            DatabaseAPI.Database.Recipes = (Recipe[]) recipeArray.Clone();

            FillList();
            if (lvDPA.Items.Count > recipeId)
            {
                lvDPA.Items[recipeId].Selected = true;
            }
            else if (lvDPA.Items.Count > recipeId - 1)
            {
                lvDPA.Items[recipeId - 1].Selected = true;
            }
            else if (lvDPA.Items.Count > 0)
            {
                lvDPA.Items[0].Selected = true;
            }
        }

        private void btnRunSeq_Click(object sender, EventArgs e)
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
        }

        private void btnReGuess_Click(object sender, EventArgs e)
        {
            DatabaseAPI.GuessRecipes();
            DatabaseAPI.AssignRecipeIDs();
        }

        private void cbEnh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() <= -1 || cbEnh.SelectedIndex <= -1) return;
            var recipe = DatabaseAPI.Database.Recipes[RecipeID()];
            recipe.EnhIdx = cbEnh.SelectedText.ToLower() == "none"
                ? -1
                : DatabaseAPI.Database.Enhancements.TryFindIndex(e => e.UID == cbEnh.SelectedText);
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
        }

        private void cbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() <= -1 || cbRarity.SelectedIndex <= -1) return;
            DatabaseAPI.Database.Recipes[RecipeID()].Rarity = (Recipe.RecipeRarity) cbRarity.SelectedIndex;
            UpdateListItem(RecipeID());
        }

        private void cbSalX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() < 0 || EntryID() < 0) return;
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
            //lvDPA.Items[0].Selected = true;
        }

        private void frmRecipeEdit_Load(object sender, EventArgs e)
        {
            // Mitigate flickering on the ListView control.
            // https://stackoverflow.com/a/42389596
            lvDPA
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(lvDPA, true, null);
            const Recipe.RecipeRarity recipeRarity = Recipe.RecipeRarity.Common;
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

            for (var i = 0; i < cbEnh.Items.Count; i++)
            {
                if (cbEnh.Items[i].ToString() != "None") continue;
                _noEnhancementIdx = i;
                break;
            }
            
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
            _lvColumnSorter = new ListViewColumnSorter();
            lvDPA.ListViewItemSorter = _lvColumnSorter;
            btnPrepareTags.Visible = MidsContext.Config.MasterMode;
            ClearInfo();
            _noUpdate = false;
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
            var num = DatabaseAPI.Database.Recipes[RecipeID()].Item[DatabaseAPI.Database.Recipes[RecipeID()].Item.Length - 1].Level + 1;
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
            if (lvDPA.SelectedItems.Count > 0)
            {
                //var recipeItem = lvDPA.SelectedItems[0].Text;
                //var rIndex = Array.FindIndex(DatabaseAPI.Database.Recipes, 0, x => x.InternalName == recipeItem);
                var rIndex = Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text);
                ShowEntryInfo(rIndex, lstItems.SelectedIndex);
            }
            else
            {
                ClearEntryInfo();
            }
        }

        private void lvDPA_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDPA.SelectedItems.Count <= 0) return;
            udStaticIndex.Enabled = true;
            ShowRecipeInfo(Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text));
            
            /*else
            {
                udStaticIndex.Enabled = false;
                ClearInfo();
            }*/
        }

        private int MinMax(int iValue, NumericUpDown iControl)
        {
            return (int) Math.Max(iControl.Minimum, Math.Min(iControl.Maximum, iValue));
        }

        private int RecipeID(int item = 0)
        {
            if (lvDPA.SelectedItems.Count <= 0) return -1;

            //var recipeItem = lvDPA.SelectedItems[0].Text;
            //var rIndex = Array.FindIndex(DatabaseAPI.Database.Recipes, 0, x => x.InternalName == recipeItem);
            //return lvDPA.SelectedIndices[0];

            return Convert.ToInt32(lvDPA.SelectedItems[item].SubItems[1].Text);
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
                _noUpdate = true;
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
                _noUpdate = false;
            }
        }

        private void ShowRecipeInfo(int index)
        {
            //var recipeItem = lvDPA.SelectedItems[0].Text;
            //var rIndex = Array.FindIndex(DatabaseAPI.Database.Recipes, 0, x => x.InternalName == recipeItem);
            var rIndex = Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text);
            if ((rIndex < 0) | (rIndex > DatabaseAPI.Database.Recipes.Length - 1))
            {
                ClearInfo();
            }
            else
            {
                _noUpdate = true;
                txtRecipeName.Text = DatabaseAPI.Database.Recipes[rIndex].InternalName;
                var enhIdx = -1;
                try
                {
                    enhIdx = Array.FindIndex(DatabaseAPI.Database.Enhancements, 0,
                        e => e.RecipeName == DatabaseAPI.Database.Recipes[rIndex].InternalName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ShowRecipeInfo({index}) #2:\r\n{ex.Message}\r\n{ex.StackTrace}");
                    enhIdx = -1;
                }

                if (enhIdx > -1)
                {
                    var recipeEnhIdx = cbEnh.Items.IndexOf(DatabaseAPI.Database.Enhancements[enhIdx].UID);
                    cbEnh.SelectedIndex = recipeEnhIdx;
                    lblEnh.Text = DatabaseAPI.Database.Enhancements[enhIdx].LongName;
                }
                else
                {
                    cbEnh.SelectedIndex = _noEnhancementIdx;
                    lblEnh.Text = "";
                }
                
                try
                {
                    cbRarity.SelectedIndex = (int) DatabaseAPI.Database.Recipes[rIndex].Rarity;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ShowRecipeInfo({index}) #3:\r\n{ex.Message}\r\n{ex.StackTrace}");
                    DatabaseAPI.Database.Recipes[rIndex].Rarity = Recipe.RecipeRarity.Common;
                }

                udStaticIndex.Value = rIndex;
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
                _noUpdate = false;
            }
        }

        private void txtExtern_TextChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            if (_noUpdate || rId <= -1) return;
            DatabaseAPI.Database.Recipes[rId].ExternalName = txtExtern.Text;
        }

        private void txtRecipeName_TextChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            if (_noUpdate || rId <= -1) return;
            DatabaseAPI.Database.Recipes[rId].InternalName = txtRecipeName.Text;
            UpdateListItem(rId);
        }

        private void udCostX_Leave(object sender, EventArgs e)
        {
            var rId = RecipeID();
            var eId = EntryID();
            if (_noUpdate || rId < 0 || eId < 0) return;
            var recipeItem = DatabaseAPI.Database.Recipes[rId].Item[eId];
            recipeItem.Level = MinMax(Convert.ToInt32(Regex.Replace(udLevel.Text, @"[\.\,]", "")), udLevel) - 1;
            recipeItem.BuyCost = MinMax(Convert.ToInt32(Regex.Replace(udBuy.Text, @"[\.\,]", "")), udBuy);
            recipeItem.BuyCostM = MinMax(Convert.ToInt32(Regex.Replace(udBuyM.Text, @"[\.\,]", "")), udBuyM);
            recipeItem.CraftCost = MinMax(Convert.ToInt32(Regex.Replace(udBuy.Text, @"[\.\,]", "")), udCraft);
            recipeItem.CraftCostM = MinMax(Convert.ToInt32(Regex.Replace(udBuy.Text, @"[\.\,]", "")), udCraftM);
        }

        private void udCostX_ValueChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            var eId = EntryID();
            
            if (_noUpdate || rId < 0 || eId < 0) return;
            var recipeItem = DatabaseAPI.Database.Recipes[rId].Item[eId];
            var level = Convert.ToInt32(udLevel.Value);
            recipeItem.Level = level - 1;
            recipeItem.BuyCost = Convert.ToInt32(udBuy.Value);
            recipeItem.BuyCostM = Convert.ToInt32(udBuyM.Value);
            recipeItem.CraftCost = Convert.ToInt32(udCraft.Value);
            recipeItem.CraftCostM = Convert.ToInt32(udCraftM.Value);

            if (level >= 0 & level <= 50) lstItems.SelectedItem = $"Level: {level}";
            if ((sender as NumericUpDown)?.Name == "udLevel")
            {
                UpdateListItem(rId);
            }
        }

        private void udSalX_Leave(object sender, EventArgs e)
        {
            var recipeItem = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()];
            recipeItem.Count[0] = MinMax(Convert.ToInt32(udSal0.Text), udSal0);
            recipeItem.Count[1] = MinMax(Convert.ToInt32(udSal1.Text), udSal1);
            recipeItem.Count[2] = MinMax(Convert.ToInt32(udSal2.Text), udSal2);
            recipeItem.Count[3] = MinMax(Convert.ToInt32(udSal3.Text), udSal3);
            recipeItem.Count[4] = MinMax(Convert.ToInt32(udSal4.Text), udSal4);
        }

        private void udSalX_ValueChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            var eId = EntryID();
            if (_noUpdate || rId < 0 || eId < 0) return;
            var recipeItem = DatabaseAPI.Database.Recipes[rId].Item[eId];
            recipeItem.Count[0] = Convert.ToInt32(udSal0.Value);
            recipeItem.Count[1] = Convert.ToInt32(udSal1.Value);
            recipeItem.Count[2] = Convert.ToInt32(udSal2.Value);
            recipeItem.Count[3] = Convert.ToInt32(udSal3.Value);
            recipeItem.Count[4] = Convert.ToInt32(udSal4.Value);
        }

        private void UpdateListItem(int index)
        {
            if (!((index > -1) & (index < DatabaseAPI.Database.Recipes.Length))) return;
            lvDPA.Items[index].SubItems[0].Text = DatabaseAPI.Database.Recipes[index].InternalName;
            lvDPA.Items[index].SubItems[1].Text = Convert.ToString(index);
            lvDPA.Items[index].SubItems[2].Text = DatabaseAPI.Database.Recipes[index].EnhIdx <= -1
                ? "None"
                : $"{DatabaseAPI.Database.Recipes[index].Enhancement} ({DatabaseAPI.Database.Recipes[index].EnhIdx})";
            lvDPA.Items[index].SubItems[3].Text = Enum.GetName(DatabaseAPI.Database.Recipes[index].Rarity.GetType(),
                DatabaseAPI.Database.Recipes[index].Rarity);
            lvDPA.Items[index].SubItems[4].Text = Convert.ToString(DatabaseAPI.Database.Recipes[index].Item.Length,
                CultureInfo.InvariantCulture);
            lvDPA.Items[index].SubItems[5].Text = GetRecipeFlags(index);
        }

        private void cbIsRecipe_CheckedChanged(object sender, EventArgs e)
        {
            var target = sender as CheckBox;
            var state = target != null && target.Checked;
            var rId = RecipeID();
            var eId = EntryID();
            if (target == null) return;
            switch (target.Name)
            {
                case "cbIsRecipe0":
                    PopulateComboBoxList(ref cbSal0, !state);
                    cbSal0.SelectedIndex = 0;
                    udSal0.Visible = !state;
                    if (!state)
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].SalvageIdx[0] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #1):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].RecipeIdx[0] = 0;
                    }

                    break;

                case "cbIsRecipe1":
                    PopulateComboBoxList(ref cbSal1, !state);
                    cbSal1.SelectedIndex = 0;
                    udSal1.Visible = !state;
                    if (!state)
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].SalvageIdx[1] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #2):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].RecipeIdx[1] = 0;
                    }

                    break;

                case "cbIsRecipe2":
                    PopulateComboBoxList(ref cbSal2, !state);
                    cbSal2.SelectedIndex = 0;
                    udSal2.Visible = !state;
                    if (!state)
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].SalvageIdx[2] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #3):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].RecipeIdx[2] = 0;
                    }

                    break;

                case "cbIsRecipe3":
                    PopulateComboBoxList(ref cbSal3, !state);
                    cbSal3.SelectedIndex = 0;
                    udSal3.Visible = !state;
                    if (!state)
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].SalvageIdx[3] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #4):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].RecipeIdx[3] = 0;
                    }

                    break;

                case "cbIsRecipe4":
                    PopulateComboBoxList(ref cbSal4, !state);
                    cbSal4.SelectedIndex = 0;
                    udSal4.Visible = !state;
                    if (!state)
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].SalvageIdx[4] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #5):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        DatabaseAPI.Database.Recipes[rId].Item[eId].RecipeIdx[4] = 0;
                    }

                    break;
            }
            groupBox4.Visible = !state;
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

        private string GetRecipeFlags(int idx)
        {
            var recipe = DatabaseAPI.Database.Recipes[idx];
            return $"{(recipe.IsGeneric ? "G" : " ")}{(recipe.IsVirtual ? "V" : " ")}{(recipe.IsHidden ? "H" : "")}";
        }

        private void cbIsGeneric_Click(object sender, EventArgs e)
        {
            if (lvDPA.SelectedItems.Count == 0) return;
            
            var control = (CheckBox) sender;
            var state = control?.Checked == true;

            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            for (var i = 0; i < lvDPA.SelectedItems.Count; i++)
            {
                var rId = RecipeID(i);
                DatabaseAPI.Database.Recipes[rId].IsGeneric = state;
                lvDPA.SelectedItems[i].SubItems[5].Text = GetRecipeFlags(rId);
                if (lvDPA.SelectedItems.Count > 1) continue;
                cbEnh.Visible = !state;
                lblEnh.Visible = !state;
                cbEnh.SelectedIndex = state ? -1 : cbEnh.SelectedIndex;
                Label2.Visible = !state;
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();
        }

        private void cbIsVirtual_Click(object sender, EventArgs e)
        {
            if (lvDPA.SelectedItems.Count == 0) return;
            
            var control = (CheckBox) sender;
            var state = control?.Checked == true;

            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            for (var i = 0; i < lvDPA.SelectedItems.Count; i++)
            {
                var rId = RecipeID(i);
                DatabaseAPI.Database.Recipes[rId].IsVirtual = state;
                lvDPA.SelectedItems[i].SubItems[5].Text = GetRecipeFlags(rId);
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();
        }

        private void cbIsHidden_Click(object sender, EventArgs e)
        {
            if (lvDPA.SelectedItems.Count == 0) return;
            
            var control = (CheckBox) sender;
            var state = control?.Checked == true;

            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            for (var i = 0; i < lvDPA.SelectedItems.Count; i++)
            {
                var rId = RecipeID(i);
                DatabaseAPI.Database.Recipes[rId].IsHidden = state;
                lvDPA.SelectedItems[i].SubItems[5].Text = GetRecipeFlags(rId);
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();
        }

        private void cbSal_Enter_UpdateSubRecipe(int idx)
        {
            groupBox4.Visible = cbIsRecipe0.Checked;
            label4.Text = $"Sub-recipe components(Ingredient #{idx + 1}):";
            var subRecipe = DatabaseAPI.Database.Recipes[DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].RecipeIdx[idx]];
            var mainLevel = DatabaseAPI.Database.Recipes[RecipeID()].Item[EntryID()].Level;
            var subRecipeEntry = subRecipe.Item.Where(item => item.Level == mainLevel).DefaultIfEmpty(subRecipe.Item[0]).First();
            lstSubRecipeComponents.Items.Clear();
            for (var i = 0; i < subRecipeEntry.Salvage.Length; i++)
            {
                if (subRecipeEntry.SalvageIdx[i] <= -1) continue;
                lstSubRecipeComponents.Items.Add(DatabaseAPI.Database.Salvage[subRecipeEntry.SalvageIdx[i]]);
            }
        }

        private void cbSal0_Enter(object sender, EventArgs e)
        {
            if (!cbIsRecipe0.Checked) return;
            cbSal_Enter_UpdateSubRecipe(0);
        }

        private void cbSal1_Enter(object sender, EventArgs e)
        {
            if (!cbIsRecipe1.Checked) return;
            cbSal_Enter_UpdateSubRecipe(1);
        }

        private void cbSal2_Enter(object sender, EventArgs e)
        {
            if (!cbIsRecipe2.Checked) return;
            cbSal_Enter_UpdateSubRecipe(2);
        }

        private void cbSal3_Enter(object sender, EventArgs e)
        {
            if (!cbIsRecipe3.Checked) return;
            cbSal_Enter_UpdateSubRecipe(3);
        }

        private void cbSal4_Enter(object sender, EventArgs e)
        {
            if (!cbIsRecipe4.Checked) return;
            cbSal_Enter_UpdateSubRecipe(4);
        }

        private void cbSalX_Leave(object sender, EventArgs e)
        {
            groupBox4.Visible = false;
        }

        private void lvDPA_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lvColumnSorter.Column)
            {
                _lvColumnSorter.SortDir = _lvColumnSorter.SortDir == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _lvColumnSorter.Column = e.Column;
                _lvColumnSorter.SortDir = SortOrder.Ascending;
            }
            
            lvDPA.Sort();
        }

        private void udStaticIndex_ValueChanged(object sender, EventArgs e)
        {
            if (sender is NumericUpDown ud)
            {
                if (lvDPA.SelectedItems.Count > 0)
                {
                    lvDPA.SelectedItems[0].SubItems[1].Text = Convert.ToString(ud.Value, CultureInfo.InvariantCulture);
                }
            }
        }

        private void udStaticIndex_Leave(object sender, EventArgs e)
        {
            if (!CheckIndexesConsistency()) return;
            
            AssignRecipeIndexes();
            if (_lvColumnSorter.Column == 1) lvDPA.Sort();
        }

        private void btnAutoMarkGeneric_Click(object sender, EventArgs e)
        {
            btnMassUpdateTags.Enabled = false;
            btnAutoMarkGeneric.Enabled = false;
            btnPrepareTags.Enabled = false;
            label17.Text = "Updating tags... 0%";
            panel1.Visible = true;
            label17.Refresh();
            
            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            for (var i = 0; i < lvDPA.Items.Count; i++)
            {
                var index = Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text);
                var recipe = DatabaseAPI.Database.Recipes[index];
                recipe.IsGeneric = recipe.EnhIdx == -1;
                lvDPA.Items[i].SubItems[5].Text = GetRecipeFlags(index);
                if (i <= 0 || i % 10 != 0) continue;
                
                var vp = (int)Math.Round((float) i / lvDPA.Items.Count * 100);
                label17.Text = $"Updating tags... {vp}%";
                label17.Refresh();
                progressBar1.Value = vp;
            }

            if (lvDPA.SelectedItems.Count > 0)
            {
                cbIsGeneric.Checked = DatabaseAPI.Database.Recipes[Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text)].IsGeneric;
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();

            panel1.Visible = false;
            btnMassUpdateTags.Enabled = true;
            btnAutoMarkGeneric.Enabled = true;
            btnPrepareTags.Enabled = true;
        }

        private void btnMassUpdateTags_Click(object sender, EventArgs e)
        {
            var frmFilter = new frmRecipeEditorBulkFilter();
            var ret = frmFilter.ShowDialog(this);
            if (ret != DialogResult.OK) return;

            btnMassUpdateTags.Enabled = false;
            btnAutoMarkGeneric.Enabled = false;
            btnPrepareTags.Enabled = false;
            var filterParams = frmFilter.FilterResult;
            if (filterParams.G == frmRecipeEditorBulkFilter.FlagAction.Ignore &
                filterParams.V == frmRecipeEditorBulkFilter.FlagAction.Ignore &
                filterParams.H == frmRecipeEditorBulkFilter.FlagAction.Ignore)
            {
                // Do not run any action if no action has been set
                return;
            }

            label17.Text = "Updating tags... 0%";
            panel1.Visible = true;
            label17.Refresh();

            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            for (var i = 0; i < lvDPA.Items.Count; i++)
            {
                var index = Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text);
                var recipe = DatabaseAPI.Database.Recipes[index];
                var testCond = false;
                if (!(filterParams.UseFilter & filterParams.Column != -1 &
                      filterParams.FlagConditionType != frmRecipeEditorBulkFilter.FlagConditionType.None &
                      !string.IsNullOrWhiteSpace(filterParams.ConditionText)))
                {
                    testCond = true;
                }
                else
                {
                    var lvColumn = filterParams.Column switch
                    {
                        1 => 2,
                        2 => 3,
                        3 => 5,
                        _ => 0
                    };

                    testCond = filterParams.FlagConditionType switch
                    {
                        frmRecipeEditorBulkFilter.FlagConditionType.Is => lvDPA.Items[i]
                            .SubItems[lvColumn].Text == filterParams.ConditionText,
                        frmRecipeEditorBulkFilter.FlagConditionType.Contains => lvDPA.Items[i]
                            .SubItems[lvColumn]
                            .Text.Contains(filterParams.ConditionText),
                        frmRecipeEditorBulkFilter.FlagConditionType.StartsWith => lvDPA.Items[i]
                            .SubItems[lvColumn]
                            .Text.StartsWith(filterParams.ConditionText),
                        frmRecipeEditorBulkFilter.FlagConditionType.DoesNotContain => !lvDPA.Items[i]
                            .SubItems[lvColumn]
                            .Text.Contains(filterParams.ConditionText),
                        frmRecipeEditorBulkFilter.FlagConditionType.DoesNotStartWith => !lvDPA.Items[i]
                            .SubItems[lvColumn]
                            .Text.StartsWith(filterParams.ConditionText),
                        _ => false
                    };
                }

                if (filterParams.G != frmRecipeEditorBulkFilter.FlagAction.Ignore & testCond)
                {
                    recipe.IsGeneric = filterParams.G == frmRecipeEditorBulkFilter.FlagAction.Enable;
                }

                if (filterParams.V != frmRecipeEditorBulkFilter.FlagAction.Ignore & testCond)
                {
                    recipe.IsVirtual = filterParams.V == frmRecipeEditorBulkFilter.FlagAction.Enable;
                }

                if (filterParams.H != frmRecipeEditorBulkFilter.FlagAction.Ignore & testCond)
                {
                    recipe.IsHidden = filterParams.H == frmRecipeEditorBulkFilter.FlagAction.Enable;
                }

                lvDPA.Items[i].SubItems[5].Text = GetRecipeFlags(index);

                var vp = (int)Math.Round((float)i / lvDPA.Items.Count * 100);
                label17.Text = $"Updating tags... {vp}%";
                label17.Refresh();
                progressBar1.Value = vp;
            }

            if (lvDPA.SelectedItems.Count > 0)
            {
                var selectedRecipeIdx = Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text);
                cbIsGeneric.Checked = DatabaseAPI.Database.Recipes[selectedRecipeIdx].IsGeneric;
                cbIsVirtual.Checked = DatabaseAPI.Database.Recipes[selectedRecipeIdx].IsVirtual;
                cbIsHidden.Checked = DatabaseAPI.Database.Recipes[selectedRecipeIdx].IsHidden;
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();

            panel1.Visible = false;
            btnMassUpdateTags.Enabled = true;
            btnAutoMarkGeneric.Enabled = true;
            btnPrepareTags.Enabled = true;
            frmFilter.Dispose();
        }

        // Internal use only.
        private void btnPrepareTags_Click(object sender, EventArgs e)
        {
            btnMassUpdateTags.Enabled = false;
            btnAutoMarkGeneric.Enabled = false;
            btnPrepareTags.Enabled = false;

            label17.Text = "Updating tags... 0%";
            panel1.Visible = true;
            label17.Refresh();

            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            for (var i = 0; i < lvDPA.Items.Count; i++)
            {
                var index = Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text);
                var recipe = DatabaseAPI.Database.Recipes[index];
                var updated = false;
                if (index <= 824 |
                    index == 955 |
                    index >= 1008 & index <= 1019 |
                    index >= 1043 & index <= 1059 |
                    index >= 1063 & index <= 1072 |
                    index >= 1079 & index <= 1127 |
                    index >= 1908 & index <= 2308)
                {
                    recipe.IsHidden = true;
                    updated = true;
                }

                if (index == 0 |
                    index >= 2092 & index <= 2176 |
                    index >= 2227 & index <= 2283 |
                    index >= 2339 & index <= 2343)
                {
                    recipe.IsVirtual = true;
                    updated = true;
                }

                recipe.IsGeneric = recipe.EnhIdx == -1;

                if (updated | recipe.EnhIdx == -1)
                {
                    lvDPA.Items[i].SubItems[5].Text = GetRecipeFlags(index);
                }
                
                if (i <= 0 || i % 10 != 0) continue;

                var vp = (int)Math.Round((float)i / lvDPA.Items.Count * 100);
                label17.Text = $"Updating tags... {vp}%";
                label17.Refresh();
                progressBar1.Value = vp;
            }

            if (lvDPA.SelectedItems.Count > 0)
            {
                cbIsGeneric.Checked = DatabaseAPI.Database.Recipes[Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text)].IsGeneric;
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();

            panel1.Visible = false;
            btnMassUpdateTags.Enabled = true;
            btnAutoMarkGeneric.Enabled = true;
            btnPrepareTags.Enabled = true;
        }
    }
}