using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Hero_Designer.My;
using Microsoft.VisualBasic;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImport_Recipe : Form
    {
        private frmBusy bFrm;

        public frmImport_Recipe()
        {
            Load += frmImport_Recipe_Load;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_Recipe));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmImport_Recipe);
        }

        private void btnAttribIndex_Click(object sender, EventArgs e)
        {
            dlgBrowse.FileName = lblAttribIndex.Text;
            if (dlgBrowse.ShowDialog(this) != DialogResult.OK)
                return;
            lblAttribIndex.Text = dlgBrowse.FileName;
        }

        private void btnAttribLoad_Click(object sender, EventArgs e)
        {
            if ((lblAttribIndex.Text != "") & (lblAttribTables.Text != ""))
            {
                if (File.Exists(lblAttribIndex.Text) & File.Exists(lblAttribTables.Text))
                    ImportRecipeCSV(lblAttribIndex.Text, lblAttribTables.Text);
                else
                    MessageBox.Show("Files cannot be found!", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Files not selected!", "No Can Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnAttribTable_Click(object sender, EventArgs e)
        {
            dlgBrowse.FileName = lblAttribTables.Text;
            if (dlgBrowse.ShowDialog(this) != DialogResult.OK)
                return;
            lblAttribTables.Text = dlgBrowse.FileName;
        }

        private void BusyHide()
        {
            if (bFrm == null)
                return;
            bFrm.Close();
            bFrm = null;
        }

        private void BusyMsg(string sMessage)
        {
            if (bFrm == null)
            {
                bFrm = new frmBusy();
                bFrm.Show();
            }

            bFrm.SetMessage(sMessage);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmImport_Recipe_Load(object sender, EventArgs e)
        {
        }

        private bool ImportRecipeCSV(string iFName1, string iFName2)
        {
            StreamReader iStream1;
            StreamReader iStream2;
            try
            {
                iStream1 = new StreamReader(iFName1);
                iStream2 = new StreamReader(iFName2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Recipe CSVs Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            var num4 = 0;
            DatabaseAPI.Database.Recipes = new Recipe[0];
            int num5;
            try
            {
                string iLine1;
                do
                {
                    iLine1 = FileIO.ReadLineUnlimited(iStream1, char.MinValue);
                    if (iLine1 == null || iLine1.StartsWith("#"))
                        continue;
                    ++num4;
                    if (num4 >= 18)
                    {
                        BusyMsg(Strings.Format(num1, "###,##0") + " Recipes Created.");
                        num4 = 0;
                    }

                    var array = CSV.ToArray(iLine1);
                    if (array.Length < 18)
                        continue;
                    var flag = false;
                    var iName = array[0];
                    if (iName.Contains("_Memorized"))
                    {
                        iName = iName.Replace("_Memorized", "");
                        flag = true;
                    }

                    if (iName.LastIndexOf("_", StringComparison.Ordinal) > 0)
                        iName = iName.Substring(0, iName.LastIndexOf("_", StringComparison.Ordinal));
                    var num6 = (int) Math.Round(Conversion.Val(array[10]));
                    var recipe1 = DatabaseAPI.GetRecipeByName(iName);
                    if (recipe1 == null)
                    {
                        recipe1 = new Recipe
                        {
                            InternalName = iName,
                            ExternalName = array[1] == "" ? iName : array[1],
                            Rarity = (Recipe.RecipeRarity) Math.Round(Conversion.Val(array[9]) - 1.0)
                        };
                        DatabaseAPI.Database.Recipes = DatabaseAPI.Database.Recipes.Append(recipe1).ToArray();
                    }

                    var index1 = -1;
                    var num7 = recipe1.Item.Length - 1;
                    for (var index2 = 0; index2 <= num7; ++index2)
                        if (recipe1.Item[index2].Level == num6 - 1)
                            index1 = index2;
                    if (index1 < 0) recipe1.Item = recipe1.Item.Append(new Recipe.RecipeEntry()).ToArray();
                    recipe1.Item[index1].Level = num6 - 1;
                    if (flag)
                    {
                        recipe1.Item[index1].BuyCostM = (int) Math.Round(Conversion.Val(array[15]));
                        recipe1.Item[index1].CraftCostM = (int) Math.Round(Conversion.Val(array[11]));
                    }
                    else
                    {
                        recipe1.Item[index1].BuyCost = (int) Math.Round(Conversion.Val(array[15]));
                        recipe1.Item[index1].CraftCost = (int) Math.Round(Conversion.Val(array[11]));
                    }

                    if (array[7].Length > 0)
                    {
                        var index2 = DatabaseAPI.NidFromUidEnhExtended(array[7]);
                        if (index2 > -1)
                        {
                            recipe1.Enhancement = DatabaseAPI.Database.Enhancements[index2].UID;
                            DatabaseAPI.Database.Enhancements[index2].RecipeName = recipe1.InternalName;
                        }
                    }

                    ++num1;
                } while (iLine1 != null);

                iStream1.Close();
                num5 = 0;
                string iLine2;
                do
                {
                    iLine2 = FileIO.ReadLineUnlimited(iStream2, char.MinValue);
                    if (iLine2 == null || iLine2.StartsWith("#"))
                        continue;
                    ++num4;
                    if (num4 >= 18)
                    {
                        BusyMsg(Strings.Format(num5, "###,##0") + " Recipes Created.");
                        num4 = 0;
                    }

                    var array = CSV.ToArray(iLine2);
                    if (array.Length < 3)
                        continue;
                    var num6 = -1;
                    var flag = false;
                    var iName = array[0];
                    if (iName.Contains("_Memorized"))
                    {
                        iName = iName.Replace("_Memorized", "");
                        flag = true;
                    }

                    if (iName.LastIndexOf("_", StringComparison.Ordinal) > 0)
                    {
                        num6 = (int) Math.Round(
                            Conversion.Val(iName.Substring(iName.LastIndexOf("_", StringComparison.Ordinal) + 1)));
                        iName = iName.Substring(0, iName.LastIndexOf("_", StringComparison.Ordinal));
                    }

                    if (!((num6 > -1) & !flag))
                        continue;
                    var recipeByName = DatabaseAPI.GetRecipeByName(iName);
                    if (recipeByName == null)
                        continue;
                    var index1 = -1;
                    ++num5;
                    var recipe = recipeByName;
                    var num7 = recipe.Item.Length - 1;
                    for (var index2 = 0; index2 <= num7; ++index2)
                        if (recipe.Item[index2].Level == num6 - 1)
                            index1 = index2;

                    if (index1 <= -1)
                        continue;
                    {
                        var index2 = -1;
                        var num8 = recipe.Item[index1].Salvage.Length - 1;
                        for (var index3 = 0; index3 <= num8; ++index3)
                        {
                            if (recipe.Item[index1].Salvage[index3] != "")
                                continue;
                            index2 = index3;
                            break;
                        }

                        if (index2 <= -1)
                            continue;
                        recipe.Item[index1].Salvage[index2] = array[2];
                        recipe.Item[index1].Count[index2] = (int) Math.Round(Conversion.Val(array[1]));
                        recipe.Item[index1].SalvageIdx[index2] = -1;
                    }
                } while (iLine2 != null);

                DatabaseAPI.AssignRecipeSalvageIDs();
                DatabaseAPI.GuessRecipes();
                DatabaseAPI.AssignRecipeIDs();
                MessageBox.Show("Done, Recipe-Enhancement links have been guessed.", "Import", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                iStream1.Close();
                iStream2.Close();
                BusyHide();
                MessageBox.Show(ex.Message, "CSV Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            BusyHide();
            iStream2.Close();
            MessageBox.Show($"Parse Completed!\r\nTotal Records: {num5}\r\nGood: {num2}\r\nRejected: {num3}",
                "File Parsed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveRecipes(serializer);
            DatabaseAPI.SaveEnhancementDb(serializer);
            return true;
        }
    }
}