using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        private Recipe[] _tempRecipes;
        private IEnhancement[] _tempEnhancements;
        private int _recipeListPrevSelected;

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
            _tempRecipes = (Recipe[])DatabaseAPI.Database.Recipes.Clone();
            _tempEnhancements = (IEnhancement[])DatabaseAPI.Database.Enhancements.Clone();
            FillList();
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
            _recipeListPrevSelected = -1;
            _noUpdate = false;
        }

        private void AddListItem(int index)
        {
            if (!((index > -1) & (index < _tempRecipes.Length))) return;
            var recipe = _tempRecipes[index];
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
                        $"Warning: Recipe {_tempRecipes[i].InternalName} has index {i} (should be {k})",
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
                var recipesArr = new Recipe[_tempRecipes.Length];
                for (var i = 0; i < lvDPA.Items.Count; i++)
                {
                    var index = Convert.ToInt32(lvDPA.Items[i].SubItems[1].Text);
                    recipesArr[index] = _tempRecipes.First(r => r.InternalName == lvDPA.Items[i].SubItems[0].Text);
                }

                _tempRecipes = (Recipe[]) recipesArr.Clone();
            });
            
            t.Start();
            if (!async) t.Wait();
        }

        private void AssignNewRecipes()
        {
            for (var index = 0; index < _tempRecipes.Length; index++)
            {
                var recipe = _tempRecipes[index];
                if (!((recipe.EnhIdx > -1) & (recipe.EnhIdx <= _tempEnhancements.Length - 1)) ||
                    _tempEnhancements[recipe.EnhIdx].RecipeIDX >= 0)
                    continue;
                _tempEnhancements[recipe.EnhIdx].RecipeIDX = index;
                _tempEnhancements[recipe.EnhIdx].RecipeName = recipe.InternalName;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var rId = RecipeID();
            var eId = EntryID();
            if (rId < 0) return;
            var recipe = _tempRecipes[rId];
            var entryLevel = 9;
            if (eId > - 1)
            {
                entryLevel = Math.Min(49, recipe.Item[eId].Level + 1);
            }

            var lItems = recipe.Item.ToList();
            lItems.Add(new Recipe.RecipeEntry {Level = entryLevel});
            recipe.Item = lItems.ToArray();
            ShowRecipeInfo(rId);
            UpdateListItem(rId);
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
            var recipe = _tempRecipes[recipeId];
            var recipeItemId = lstItems.SelectedIndex;
            recipe.Item = recipe.Item.Where((ri, i) => i != recipeItemId).ToArray();
            ShowRecipeInfo(RecipeID());
        }

        private void btnGuessCost_Click(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() < 0 || EntryID() < 0) return;
            _tempRecipes[RecipeID()].Item[EntryID()].CraftCost =
                GetCostByLevel(_tempRecipes[RecipeID()].Item[EntryID()].Level);
            udCraft.Value = new decimal(_tempRecipes[RecipeID()].Item[EntryID()].CraftCost);
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
            if (RecipeID() < 0 || (_tempRecipes[RecipeID()].Item.Length < 1) | (_tempRecipes[RecipeID()].Item.Length > 53))
                return;
            _tempRecipes[RecipeID()].Item = (Recipe.RecipeEntry[]) Utils.CopyArray(
                _tempRecipes[RecipeID()].Item,
                new Recipe.RecipeEntry[_tempRecipes[RecipeID()].Item.Length + 1]);
            _tempRecipes[RecipeID()].Item[_tempRecipes[RecipeID()].Item.Length - 1] = new Recipe.RecipeEntry(_tempRecipes[RecipeID()]
                    .Item[_tempRecipes[RecipeID()].Item.Length - 2]);
            ++_tempRecipes[RecipeID()].Item[_tempRecipes[RecipeID()].Item.Length - 1].Level;
            _tempRecipes[RecipeID()].Item[_tempRecipes[RecipeID()].Item.Length - 1]
                .CraftCost = GetCostByLevel(_tempRecipes[RecipeID()]
                .Item[_tempRecipes[RecipeID()].Item.Length - 1].Level);
            ShowRecipeInfo(RecipeID());
            lstItems.SelectedIndex = lstItems.Items.Count - 1;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckIndexesConsistency()) return;
            
            AssignRecipeIndexes(false);
            AssignNewRecipes();
            DatabaseAPI.Database.Recipes = (Recipe[]) _tempRecipes.Clone();
            DatabaseAPI.Database.Enhancements = (IEnhancement[]) _tempEnhancements.Clone();
            DatabaseAPI.AssignRecipeSalvageIDs();
            DatabaseAPI.AssignRecipeIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveRecipes(serializer);
            DatabaseAPI.SaveEnhancementDb(serializer);
            Close();
        }

        private void btnRAdd_Click(object sender, EventArgs e)
        {
            _noUpdate = true;
            var recipe = new Recipe
            {
                EnhIdx = -1,
                Enhancement = "",
                IsGeneric = true,
                Item = new[] {new Recipe.RecipeEntry {Level = 9}}
            };
            //_tempRecipes = _tempRecipes.Append(recipe).ToArray();
            var lRecipes = _tempRecipes.ToList();
            lRecipes.Add(recipe);
            _tempRecipes = lRecipes.ToArray();
            var rIndex = _tempRecipes.Length - 1;
            AddListItem(rIndex);
            UpdateListItem(rIndex);
            if (_recipeListPrevSelected == -1)
            {
                EnableRecipeEntryControls();
                _recipeListPrevSelected = lvDPA.SelectedItems[0].Index;
            }
            udStaticIndex.Value = rIndex;
            lvDPA.Items[lvDPA.Items.Count - 1].Selected = true;
            lvDPA.Items[lvDPA.Items.Count - 1].EnsureVisible();
            cbRarity.SelectedIndex = 0;
            cbEnh.SelectedIndex = _noEnhancementIdx;
            lblEnh.Visible = false;
            cbIsGeneric.Checked = true;
            cbEnh.Select();
            _noUpdate = false;

            //Debug.WriteLine($"RAdd: rIndex: {rIndex}\r\nEnhIdx: {_tempRecipes[rIndex].EnhIdx}\r\nEnhancement: '{_tempRecipes[rIndex].Enhancement}'");
        }

        private void btnRDel_Click(object sender, EventArgs e)
        {
            var recipeId = RecipeID();
            if (recipeId < 0) return;

            _tempRecipes = _tempRecipes.Where((_, idx) => idx != recipeId).ToArray();

            /*var recipeArray = new Recipe[_tempRecipes.Length - 1];
            var recipeCount = 0;
            for (var recipeIdx = 0; recipeIdx < _tempRecipes.Length; recipeIdx++)
            {
                if (recipeIdx == recipeId) continue;
                recipeArray[recipeCount++] = new Recipe(ref _tempRecipes[recipeIdx]);
            }

            _tempRecipes = new Recipe[recipeArray.Length];
            for (var recipeIdx = 0; recipeIdx < _tempRecipes.Length; recipeIdx++)
            {
                _tempRecipes[recipeIdx] = new Recipe(ref recipeArray[recipeIdx]);
            }

            _tempRecipes = (Recipe[]) recipeArray.Clone();*/

            FillList();
            if (lvDPA.Items.Count > recipeId)
            {
                lvDPA.Items[recipeId].Selected = true;
                lvDPA.Items[recipeId].EnsureVisible();
            }
            else if (lvDPA.Items.Count > recipeId - 1)
            {
                lvDPA.Items[recipeId - 1].Selected = true;
                lvDPA.Items[recipeId - 1].EnsureVisible();
            }
            else if (lvDPA.Items.Count > 0)
            {
                lvDPA.Items[0].Selected = true;
                lvDPA.Items[0].EnsureVisible();
            }
        }

        private void btnRunSeq_Click(object sender, EventArgs e)
        {
            var enhIdx = _tempRecipes[_tempRecipes.Length - 1].EnhIdx;
            for (var index = enhIdx + 1; index < _tempEnhancements.Length; index++)
            {
                if (_tempEnhancements[index].TypeID != Enums.eType.SetO) continue;
                _tempRecipes = _tempRecipes.Append(new Recipe
                    {
                        EnhIdx = index,
                        Enhancement = _tempEnhancements[index].UID,
                        InternalName = _tempEnhancements[index].UID
                    }
                ).ToArray();
                AddListItem(_tempRecipes.Length - 1);
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
            var recipe = _tempRecipes[RecipeID()];
            recipe.EnhIdx = cbEnh.SelectedText.ToLower() == "none" | cbEnh.SelectedIndex == _noEnhancementIdx
                ? -1
                : _tempEnhancements.TryFindIndex(e => e.UID == cbEnh.SelectedText);
            if (recipe.EnhIdx > -1)
            {
                recipe.Enhancement = cbEnh.Text;
                recipe.InternalName = cbEnh.Text;
                txtRecipeName.Text = cbEnh.Text;
                try
                {
                    lblEnh.Text = _tempEnhancements[recipe.EnhIdx].LongName;
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

            //Debug.WriteLine($"cbEnh_cb: rIndex: {RecipeID()}\r\nrecipe.EnhIdx: {recipe.EnhIdx}");

            UpdateListItem(RecipeID());
        }

        private void cbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() <= -1 || cbRarity.SelectedIndex <= -1) return;
            _tempRecipes[RecipeID()].Rarity = (Recipe.RecipeRarity) cbRarity.SelectedIndex;
            UpdateListItem(RecipeID());
        }

        private void cbSalX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_noUpdate || RecipeID() < 0 || EntryID() < 0) return;
            if (cbIsRecipe0.Checked)
            {
                _tempRecipes[RecipeID()].Item[EntryID()].RecipeIdx[0] = cbSal0.SelectedIndex - 1;
            }
            else
            {
                _tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[0] = cbSal0.SelectedIndex - 1;
            }

            if (cbIsRecipe1.Checked)
            {
                _tempRecipes[RecipeID()].Item[EntryID()].RecipeIdx[1] = cbSal1.SelectedIndex - 1;
            }
            else
            {
                _tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[1] = cbSal1.SelectedIndex - 1;
            }

            if (cbIsRecipe2.Checked)
            {
                _tempRecipes[RecipeID()].Item[EntryID()].RecipeIdx[2] = cbSal2.SelectedIndex - 1;
            }
            else
            {
                _tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[2] = cbSal2.SelectedIndex - 1;
            }

            if (cbIsRecipe3.Checked)
            {
                _tempRecipes[RecipeID()].Item[EntryID()].RecipeIdx[3] = cbSal3.SelectedIndex - 1;
            }
            else
            {
                _tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[3] = cbSal3.SelectedIndex - 1;
            }

            if (cbIsRecipe4.Checked)
            {
                _tempRecipes[RecipeID()].Item[EntryID()].RecipeIdx[4] = cbSal4.SelectedIndex - 1;
            }
            else
            {
                _tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[4] = cbSal4.SelectedIndex - 1;
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
            cbIsRecipe0.Enabled = false;
            cbIsRecipe1.Enabled = false;
            cbIsRecipe2.Enabled = false;
            cbIsRecipe3.Enabled = false;
            cbIsRecipe4.Enabled = false;
        }

        private void EnableRecipeEntryControls()
        {
            udLevel.Enabled = true;
            udBuy.Enabled = true;
            udBuyM.Enabled = true;
            udCraft.Enabled = true;
            udCraftM.Enabled = true;
            cbSal0.Enabled = true;
            udSal0.Enabled = true;
            cbSal1.Enabled = true;
            udSal1.Enabled = true;
            cbSal2.Enabled = true;
            udSal2.Enabled = true;
            cbSal3.Enabled = true;
            udSal3.Enabled = true;
            cbSal4.Enabled = true;
            udSal4.Enabled = true;
            cbIsRecipe0.Enabled = true;
            cbIsRecipe1.Enabled = true;
            cbIsRecipe2.Enabled = true;
            cbIsRecipe3.Enabled = true;
            cbIsRecipe4.Enabled = true;
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
            lvDPA.SuspendLayout();
            lvDPA.BeginUpdate();
            lvDPA.Items.Clear();
            for (var index = 0; index < _tempRecipes.Length; index++)
            {
                AddListItem(index);
            }

            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();
            //lvDPA.Items[0].Selected = true;
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
            var rId = RecipeID();
            if (rId < 0) return;

            var itemCount = _tempRecipes[rId].Item.Length;
            if (_tempRecipes[rId].Item.Length < 1) return;
            if (_tempRecipes[rId].Item.Length > 53) return;
            
            var num = _tempRecipes[rId].Item[_tempRecipes[rId].Item.Length - 1].Level + 1;
            if (num >= nMax) return;
            for (var index = num; index < nMax; index++)
            {
                _tempRecipes[rId].Item = (Recipe.RecipeEntry[]) Utils.CopyArray(
                    _tempRecipes[rId].Item,
                    new Recipe.RecipeEntry[_tempRecipes[rId].Item.Length + 1]);
                _tempRecipes[rId]
                        .Item[_tempRecipes[rId].Item.Length - 1] =
                    new Recipe.RecipeEntry(_tempRecipes[rId]
                        .Item[_tempRecipes[rId].Item.Length - 2]) {Level = index};
                _tempRecipes[rId].Item[_tempRecipes[rId].Item.Length - 1]
                    .CraftCost = GetCostByLevel(_tempRecipes[rId]
                    .Item[_tempRecipes[rId].Item.Length - 1].Level);
            }

            ShowRecipeInfo(rId);
            lstItems.SelectedIndex = lstItems.Items.Count - 1;
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDPA.SelectedItems.Count > 0)
            {
                //var recipeItem = lvDPA.SelectedItems[0].Text;
                //var rIndex = Array.FindIndex(_tempRecipes, 0, x => x.InternalName == recipeItem);
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
            var rIndex = Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text);
            ShowRecipeInfo(rIndex);
            
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

        private int MinMax(NumericUpDown iControl)
        {
            var v = (int) Math.Floor(iControl.Value);

            return (int) Math.Max(iControl.Minimum, Math.Min(iControl.Maximum, v));
        }

        private int RecipeID(int item = 0)
        {
            if (lvDPA.SelectedItems.Count <= 0) return -1;

            //var recipeItem = lvDPA.SelectedItems[0].Text;
            //var rIndex = Array.FindIndex(_tempRecipes, 0, x => x.InternalName == recipeItem);
            //return lvDPA.SelectedIndices[0];

            return Convert.ToInt32(lvDPA.SelectedItems[item].SubItems[1].Text);
        }

        private void SetSalvageStringFromIDX(int iRecipe, int iItem, int iIndex)
        {
            if (_tempRecipes[iRecipe].Item[iItem].SalvageIdx[iIndex] <= -1)
            {
                _tempRecipes[iRecipe].Item[iItem].Salvage[iIndex] = "";
            }
            else
            {
                _tempRecipes[iRecipe].Item[iItem].Salvage[iIndex] = DatabaseAPI.Database.Salvage[_tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[iIndex]].InternalName;
                Debug.WriteLine($"SetSalvageStringFromIDX: {DatabaseAPI.Database.Salvage[_tempRecipes[RecipeID()].Item[EntryID()].SalvageIdx[iIndex]].InternalName}");
            }
        }

        private void ShowEntryInfo(int rIdx, int iIdx)
        {
            if (rIdx < 0 |
                rIdx > _tempRecipes.Length - 1 |
                iIdx < 0 | iIdx > _tempRecipes[rIdx].Item.Length - 1)
            {
                ClearEntryInfo();
                return;
            }
            
            _noUpdate = true;
            
            var recipeEntry = _tempRecipes[rIdx].Item[iIdx];
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

            if (recipeEntry.RecipeIdx[0] > -1)
            {
                if (!cbIsRecipe0.Checked) PopulateComboBoxList(ref cbSal0, false);
                cbIsRecipe0.Checked = true;
                udSal0.Value = 1;
                udSal0.Visible = false;
                cbSal0.SelectedIndex = recipeEntry.RecipeIdx[0] + 1;
            }
            else
            {
                if (cbIsRecipe0.Checked) PopulateComboBoxList(ref cbSal0);
                cbIsRecipe0.Checked = false;
                udSal0.Value = recipeEntry.Count[0];
                udSal0.Visible = true;
                cbSal0.SelectedIndex = recipeEntry.SalvageIdx[0] + 1;
            }

            if (recipeEntry.RecipeIdx[1] > -1)
            {
                if (!cbIsRecipe1.Checked) PopulateComboBoxList(ref cbSal1, false);
                cbIsRecipe1.Checked = true;
                udSal1.Value = 1;
                udSal1.Visible = false;
                cbSal1.SelectedIndex = recipeEntry.RecipeIdx[1] + 1;
            }
            else
            {
                if (cbIsRecipe1.Checked) PopulateComboBoxList(ref cbSal1, false);
                cbIsRecipe1.Checked = false;
                udSal1.Value = recipeEntry.Count[1];
                udSal1.Visible = true;
                cbSal1.SelectedIndex = recipeEntry.SalvageIdx[1] + 1;
            }

            if (recipeEntry.RecipeIdx[2] > -1)
            {
                if (!cbIsRecipe2.Checked) PopulateComboBoxList(ref cbSal2, false);
                cbIsRecipe2.Checked = true;
                udSal2.Value = 1;
                udSal2.Visible = false;
                cbSal2.SelectedIndex = recipeEntry.RecipeIdx[2] + 1;
            }
            else
            {
                if (cbIsRecipe2.Checked) PopulateComboBoxList(ref cbSal2, false);
                cbIsRecipe2.Checked = false;
                udSal2.Value = recipeEntry.Count[2];
                udSal2.Visible = true;
                cbSal2.SelectedIndex = recipeEntry.SalvageIdx[2] + 1;
            }

            if (recipeEntry.RecipeIdx[3] > -1)
            {
                if (!cbIsRecipe3.Checked) PopulateComboBoxList(ref cbSal3, false);
                cbIsRecipe3.Checked = true;
                udSal3.Value = 1;
                udSal3.Visible = false;
                cbSal3.SelectedIndex = recipeEntry.RecipeIdx[3] + 1;
            }
            else
            {
                if (cbIsRecipe3.Checked) PopulateComboBoxList(ref cbSal3, false);
                cbIsRecipe3.Checked = false;
                udSal3.Value = recipeEntry.Count[3];
                udSal3.Visible = true;
                PopulateComboBoxList(ref cbSal3);
                cbSal3.SelectedIndex = recipeEntry.SalvageIdx[3] + 1;
            }

            if (recipeEntry.RecipeIdx[4] > -1)
            {
                if (!cbIsRecipe4.Checked) PopulateComboBoxList(ref cbSal4, false);
                cbIsRecipe4.Checked = true;
                udSal4.Value = 1;
                udSal4.Visible = false;
                cbSal4.SelectedIndex = recipeEntry.RecipeIdx[4] + 1;
            }
            else
            {
                if (cbIsRecipe4.Checked) PopulateComboBoxList(ref cbSal4, false);
                cbIsRecipe4.Checked = false;
                udSal4.Value = recipeEntry.Count[4];
                udSal4.Visible = true;
                cbSal4.SelectedIndex = recipeEntry.SalvageIdx[4] + 1;
            }

            _noUpdate = false;
        }

        private void PopulateRecipeEntries(int rIndex, int eIndex = -1)
        {
            if (rIndex <= -1 | rIndex >= _tempRecipes.Length) return;

            var dupIndex = -1; 
            if (eIndex > -1)
            {
                for (var i = 0; i < _tempRecipes[rIndex].Item.Length; i++)
                {
                    if (i == eIndex) continue;
                    var re = _tempRecipes[rIndex].Item[i];
                    if (re.Level != _tempRecipes[rIndex].Item[eIndex].Level) continue;
                    
                    _tempRecipes[rIndex].Item[i] = (Recipe.RecipeEntry) _tempRecipes[rIndex].Item[eIndex].Clone();
                    dupIndex = i;
                    break;
                }

                if (dupIndex > -1)
                {
                    _tempRecipes[rIndex].Item = _tempRecipes[rIndex].Item
                        .Where((_, idx) => idx != eIndex)
                        .ToArray();
                }
            }

            var si = lstItems.SelectedIndex;
            lstItems.SuspendLayout();
            lstItems.BeginUpdate();
            lstItems.Items.Clear();
            foreach (var r in _tempRecipes[rIndex].Item)
            {
                lstItems.Items.Add($"Level: {r.Level + 1}");
            }
            lstItems.EndUpdate();
            lstItems.ResumeLayout();

            if (dupIndex > -1 & eIndex > -1)
            {
                lstItems.SelectedIndex = dupIndex;
            }
            else
            {
                lstItems.SelectedIndex = si;
            }
        }

        private void ShowRecipeInfo(int index)
        {
            //var recipeItem = lvDPA.SelectedItems[0].Text;
            //var rIndex = Array.FindIndex(_tempRecipes, 0, x => x.InternalName == recipeItem);
            var rIndex = Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text);
            if ((rIndex < 0) | (rIndex > _tempRecipes.Length - 1))
            {
                ClearInfo();
            }
            else
            {
                if (_recipeListPrevSelected == -1)
                {
                    EnableRecipeEntryControls();
                    _recipeListPrevSelected = lvDPA.SelectedItems[0].Index;
                }

                _noUpdate = true;
                txtRecipeName.Text = _tempRecipes[rIndex].InternalName;
                var enhIdx = -1;
                try
                {
                    enhIdx = Array.FindIndex(_tempEnhancements, 0,
                        e => e.RecipeName == _tempRecipes[rIndex].InternalName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ShowRecipeInfo({index}) #2:\r\n{ex.Message}\r\n{ex.StackTrace}");
                    enhIdx = -1;
                }

                if (enhIdx > -1)
                {
                    var recipeEnhIdx = cbEnh.Items.IndexOf(_tempEnhancements[enhIdx].UID);
                    cbEnh.SelectedIndex = recipeEnhIdx;
                    lblEnh.Text = _tempEnhancements[enhIdx].LongName;
                }
                else
                {
                    cbEnh.SelectedIndex = _noEnhancementIdx;
                    lblEnh.Text = "";
                }
                
                try
                {
                    cbRarity.SelectedIndex = (int) _tempRecipes[rIndex].Rarity;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ShowRecipeInfo({index}) #3:\r\n{ex.Message}\r\n{ex.StackTrace}");
                    _tempRecipes[rIndex].Rarity = Recipe.RecipeRarity.Common;
                }

                udStaticIndex.Value = rIndex;
                txtExtern.Text = _tempRecipes[rIndex].ExternalName;
                cbIsGeneric.Checked = _tempRecipes[rIndex].IsGeneric;
                cbIsVirtual.Checked = _tempRecipes[rIndex].IsVirtual;
                cbIsHidden.Checked = _tempRecipes[rIndex].IsHidden;
                Label2.Visible = !cbIsGeneric.Checked;
                cbEnh.Visible = !cbIsGeneric.Checked;
                lblEnh.Visible = !cbIsGeneric.Checked & enhIdx > -1;

                PopulateRecipeEntries(rIndex);
                
                if (lstItems.Items.Count > 0) lstItems.SelectedIndex = 0;
                _noUpdate = false;
            }
        }

        private void txtExtern_TextChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            if (_noUpdate || rId <= -1) return;
            _tempRecipes[rId].ExternalName = txtExtern.Text;
        }

        private void txtRecipeName_TextChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            if (_noUpdate || rId <= -1) return;
            _tempRecipes[rId].InternalName = txtRecipeName.Text;
            UpdateListItem(rId);
        }

        private void udCostX_Leave(object sender, EventArgs e)
        {
            var rId = RecipeID();
            var eId = EntryID();
            //Debug.WriteLine($"udCostX_Leave: noUpdate: {_noUpdate}, rId: {rId}, eId: {eId}");
            if (_noUpdate || rId < 0 || eId < 0) return;
            var recipeItem = _tempRecipes[rId].Item[eId];
            recipeItem.Level = MinMax(udLevel) - 1;
            recipeItem.BuyCost = MinMax(udBuy);
            recipeItem.BuyCostM = MinMax(udBuyM);
            recipeItem.CraftCost = MinMax(udCraft);
            recipeItem.CraftCostM = MinMax(udCraftM);
            if ((sender as NumericUpDown)?.Name == "udLevel")
            {
                PopulateRecipeEntries(rId, eId);
            }
            UpdateListItem(rId);
        }

        private void udCostX_ValueChanged(object sender, EventArgs e)
        {
            var rId = RecipeID();
            var eId = EntryID();
            
            if (_noUpdate || rId < 0 || eId < 0) return;
            var recipeItem = _tempRecipes[rId].Item[eId];
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
            var recipeItem = _tempRecipes[RecipeID()].Item[EntryID()];
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
            var recipeItem = _tempRecipes[rId].Item[eId];
            recipeItem.Count[0] = Convert.ToInt32(udSal0.Value);
            recipeItem.Count[1] = Convert.ToInt32(udSal1.Value);
            recipeItem.Count[2] = Convert.ToInt32(udSal2.Value);
            recipeItem.Count[3] = Convert.ToInt32(udSal3.Value);
            recipeItem.Count[4] = Convert.ToInt32(udSal4.Value);
        }

        private void UpdateListItem(int index)
        {
            if (index <= -1) return;
            if (index >= _tempRecipes.Length) return;

            lvDPA.BeginUpdate();
            lvDPA.Items[index].SubItems[0].Text = _tempRecipes[index].InternalName;
            lvDPA.Items[index].SubItems[1].Text = Convert.ToString(index);
            lvDPA.Items[index].SubItems[2].Text = _tempRecipes[index].EnhIdx <= -1
                ? "None"
                : $"{_tempRecipes[index].Enhancement} ({_tempRecipes[index].EnhIdx})";
            lvDPA.Items[index].SubItems[3].Text = Enum.GetName(_tempRecipes[index].Rarity.GetType(), _tempRecipes[index].Rarity);
            lvDPA.Items[index].SubItems[4].Text = Convert.ToString(_tempRecipes[index].Item.Length, CultureInfo.InvariantCulture);
            lvDPA.Items[index].SubItems[5].Text = GetRecipeFlags(index);
            lvDPA.EndUpdate();
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
                        _tempRecipes[rId].Item[eId].SalvageIdx[0] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #1):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        _tempRecipes[rId].Item[eId].RecipeIdx[0] = 0;
                    }

                    break;

                case "cbIsRecipe1":
                    PopulateComboBoxList(ref cbSal1, !state);
                    cbSal1.SelectedIndex = 0;
                    udSal1.Visible = !state;
                    if (!state)
                    {
                        _tempRecipes[rId].Item[eId].SalvageIdx[1] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #2):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        _tempRecipes[rId].Item[eId].RecipeIdx[1] = 0;
                    }

                    break;

                case "cbIsRecipe2":
                    PopulateComboBoxList(ref cbSal2, !state);
                    cbSal2.SelectedIndex = 0;
                    udSal2.Visible = !state;
                    if (!state)
                    {
                        _tempRecipes[rId].Item[eId].SalvageIdx[2] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #3):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        _tempRecipes[rId].Item[eId].RecipeIdx[2] = 0;
                    }

                    break;

                case "cbIsRecipe3":
                    PopulateComboBoxList(ref cbSal3, !state);
                    cbSal3.SelectedIndex = 0;
                    udSal3.Visible = !state;
                    if (!state)
                    {
                        _tempRecipes[rId].Item[eId].SalvageIdx[3] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #4):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        _tempRecipes[rId].Item[eId].RecipeIdx[3] = 0;
                    }

                    break;

                case "cbIsRecipe4":
                    PopulateComboBoxList(ref cbSal4, !state);
                    cbSal4.SelectedIndex = 0;
                    udSal4.Visible = !state;
                    if (!state)
                    {
                        _tempRecipes[rId].Item[eId].SalvageIdx[4] = 0;
                        Label2.Text = "Sub-recipe components (Ingredient #5):";
                        lstSubRecipeComponents.Items.Clear();
                    }
                    else
                    {
                        _tempRecipes[rId].Item[eId].RecipeIdx[4] = 0;
                    }

                    break;
            }
            groupBox4.Visible = state;
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
                foreach (var re in _tempRecipes)
                {
                    ctl.Items.Add(re.ExternalName.Trim() == "Nothing" ? re.InternalName : $"{re.ExternalName.Trim()} [{re.InternalName}]");
                }
            }
            ctl.EndUpdate();
        }

        private string GetRecipeFlags(int idx)
        {
            var recipe = _tempRecipes[idx];
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
                _tempRecipes[rId].IsGeneric = state;
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
                _tempRecipes[rId].IsVirtual = state;
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
                _tempRecipes[rId].IsHidden = state;
                lvDPA.SelectedItems[i].SubItems[5].Text = GetRecipeFlags(rId);
            }
            lvDPA.EndUpdate();
            lvDPA.ResumeLayout();
        }

        private void cbSal_Enter_UpdateSubRecipe(int idx)
        {
            var rId = RecipeID();
            var eId = EntryID();
            groupBox4.Visible = cbIsRecipe0.Checked;
            label4.Text = $"Sub-recipe components(Ingredient #{idx + 1}):";
            var subRecipe = _tempRecipes[_tempRecipes[rId].Item[eId].RecipeIdx[idx]];
            Debug.WriteLine($"cbSal: subRecipe: {subRecipe.ExternalName} ({idx})");
            var mainLevel = _tempRecipes[rId].Item[eId].Level;
            var subRecipeEntry = subRecipe.Item.Where(item => item.Level == mainLevel).DefaultIfEmpty(subRecipe.Item[0]).First();
            
            lstSubRecipeComponents.BeginUpdate();
            lstSubRecipeComponents.Items.Clear();
            for (var i = 0; i < subRecipeEntry.Salvage.Length; i++)
            {
                if (subRecipeEntry.SalvageIdx[i] <= -1) continue;
                lstSubRecipeComponents.Items.Add(DatabaseAPI.Database.Salvage[subRecipeEntry.SalvageIdx[i]]);
            }
            lstSubRecipeComponents.EndUpdate();
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
            if (_noUpdate) return;
            if (lvDPA.SelectedItems.Count <= 0) return;

            if (sender is NumericUpDown ud)
            {
                lvDPA.SelectedItems[0].SubItems[1].Text = Convert.ToString(ud.Value, CultureInfo.InvariantCulture);
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
                var recipe = _tempRecipes[index];
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
                cbIsGeneric.Checked = _tempRecipes[Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text)].IsGeneric;
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
                var recipe = _tempRecipes[index];
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
                cbIsGeneric.Checked = _tempRecipes[selectedRecipeIdx].IsGeneric;
                cbIsVirtual.Checked = _tempRecipes[selectedRecipeIdx].IsVirtual;
                cbIsHidden.Checked = _tempRecipes[selectedRecipeIdx].IsHidden;
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
                var recipe = _tempRecipes[index];
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
                cbIsGeneric.Checked = _tempRecipes[Convert.ToInt32(lvDPA.SelectedItems[0].SubItems[1].Text)].IsGeneric;
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