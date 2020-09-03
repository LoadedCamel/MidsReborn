using Base.Data_Classes;
using Base.Master_Classes;
using Hero_Designer.Forms.ImportExportItems;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmCSV : Form
    {
        private frmBusy bFrm;

        public frmCSV()
        {
            Load += frmCSV_Load;
            InitializeComponent();
            Name = nameof(frmCSV);
            var componentResourceManager = new ComponentResourceManager(typeof(frmCSV));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
        }

        private void frmCSV_Load(object sender, EventArgs e)
        {
            DisplayInfo();
        }

        private void at_Import_Click(object sender, EventArgs e)
        {
            using var frmImportArchetype = new frmImport_Archetype();
            frmImportArchetype.ShowDialog();
            DisplayInfo();
        }

        private void btnBonusLookup_Click(object sender, EventArgs e)
        {
            using var frmImportSetBonusAssignment = new frmImport_SetBonusAssignment();
            frmImportSetBonusAssignment.ShowDialog();
            DisplayInfo();
        }

        private void btnClearSI_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really set all StaticIndex values to -1?\r\nIf not using qualified names for Save/Load, files will be un-openable until Statics are re-indexed. Full Re-Indexing may result in changed index assignments.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            var num1 = DatabaseAPI.Database.Power.Length - 1;
            for (var index = 0; index <= num1; ++index)
                DatabaseAPI.Database.Power[index].StaticIndex = -1;
            var num2 = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index = 0; index <= num2; ++index)
                DatabaseAPI.Database.Enhancements[index].StaticIndex = -1;
            MessageBox.Show("Static Index values cleared.", "De-Indexing Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDefiance_Click(object sender, EventArgs e)
        {
            BusyMsg("Working...");
            var num1 = DatabaseAPI.Database.Powersets.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                if (!string.Equals(DatabaseAPI.Database.Powersets[index1].ATClass, "CLASS_BLASTER",
                    StringComparison.OrdinalIgnoreCase))
                    continue;
                var num2 = DatabaseAPI.Database.Powersets[index1].Powers.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    var num3 = DatabaseAPI.Database.Powersets[index1].Powers[index2].Effects.Length - 1;
                    for (var index3 = 0; index3 <= num3; ++index3)
                    {
                        var effect = DatabaseAPI.Database.Powersets[index1].Powers[index2].Effects[index3];
                        if (effect.EffectType == Enums.eEffectType.DamageBuff && (effect.Mag < 0.4) &
                            (effect.Mag > 0.0) & (effect.ToWho == Enums.eToWho.Self) &
                            (effect.SpecialCase == Enums.eSpecialCase.None))
                            effect.SpecialCase = Enums.eSpecialCase.Defiance;
                    }
                }
            }

            BusyMsg("Re-Indexing && Saving...");
            DatabaseAPI.MatchAllIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            BusyHide();
        }

        private void btnEnhEffects_Click(object sender, EventArgs e)
        {
            using var frmImportEnhancementEffects = new frmImport_EnhancementEffects();
            frmImportEnhancementEffects.ShowDialog();
            DisplayInfo();
        }

        private void btnEntities_Click(object sender, EventArgs e)
        {
            using var frmImportEntities = new frmImport_Entities();
            frmImportEntities.ShowDialog();
            DisplayInfo();
        }

        private void btnImportRecipes_Click(object sender, EventArgs e)
        {
            using var frmImportRecipe = new frmImport_Recipe();
            frmImportRecipe.ShowDialog();
            DisplayInfo();
        }

        private void btnIOLevels_Click(object sender, EventArgs e)
        {
            BusyMsg("Working...");
            SetEnhancementLevels();
            BusyMsg("Saving...");
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveEnhancementDb(serializer);
            BusyHide();
        }

        private void btnSalvageUpdate_Click(object sender, EventArgs e)
        {
            new frmImport_SalvageReq().ShowDialog();
            DisplayInfo();
        }

        private static void btnStaticExport_Click(object sender, EventArgs e)
        {
            var str1 = "Static Indexes, App version " + MidsContext.AppVersion + ", database version " +
                       Convert.ToString(DatabaseAPI.Database.Version, CultureInfo.InvariantCulture) + ":\r\n";
            str1 = (from Power power in DatabaseAPI.Database.Power
                where power.GetPowerSet().SetType != Enums.ePowerSetType.Boost
                select Convert.ToString(power.StaticIndex, CultureInfo.InvariantCulture) + "\t" + power.FullName +
                       "\r\n").Aggregate(str1, (current, str2) => current + str2);
            var text = str1 + "Enhancements\r\n";
            foreach (var enhancement1 in DatabaseAPI.Database.Enhancements)
            {
                var enhancement = (Enhancement) enhancement1;
                string str2;
                var power = enhancement.GetPower();
                if (power != null)
                    str2 = Convert.ToString(enhancement.StaticIndex, CultureInfo.InvariantCulture) + "\t" +
                           power.FullName + "\r\n";
                else
                    str2 = "THIS ONE IS NULL  " +
                           Convert.ToString(enhancement.StaticIndex, CultureInfo.InvariantCulture) + "\t" +
                           enhancement.Name + "\r\n";
                text += str2;
            }

            Clipboard.SetText(text);
            try
            {
                using StreamWriter sw = File.AppendText("StaticIndexes.txt");
                sw.WriteLine(text);
                MessageBox.Show(@"Copied to clipboard and saved in StaticIndexes.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Copied to clipboard only");
            }
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

        private void Button2_Click(object sender, EventArgs e)
        {
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.AssignStaticIndexValues(serializer, true);
            MessageBox.Show(@"Static Index values assigned.", @"Indexing Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DisplayInfo()
        {
            mod_Date.Text = Strings.Format(DatabaseAPI.Database.AttribMods.RevisionDate, "dd/MMM/yy HH:mm:ss");
            mod_Revision.Text = Convert.ToString(DatabaseAPI.Database.AttribMods.Revision);
            mod_Count.Text = Convert.ToString(DatabaseAPI.Database.AttribMods.Modifier.Length);
            at_Date.Text = Strings.Format(DatabaseAPI.Database.ArchetypeVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            at_Revision.Text = Convert.ToString(DatabaseAPI.Database.ArchetypeVersion.Revision);
            at_Count.Text = Convert.ToString(DatabaseAPI.Database.Classes.Length);
            set_Date.Text = Strings.Format(DatabaseAPI.Database.PowersetVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            set_Revision.Text = Convert.ToString(DatabaseAPI.Database.PowersetVersion.Revision);
            set_Count.Text = Convert.ToString(DatabaseAPI.Database.Powersets.Length);
            pow_Date.Text = Strings.Format(DatabaseAPI.Database.PowerVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            pow_Revision.Text = Convert.ToString(DatabaseAPI.Database.PowerVersion.Revision);
            pow_Count.Text = Convert.ToString(DatabaseAPI.Database.Power.Length);
            lev_date.Text = Strings.Format(DatabaseAPI.Database.PowerLevelVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            lev_Revision.Text = Convert.ToString(DatabaseAPI.Database.PowerLevelVersion.Revision);
            lev_Count.Text = Convert.ToString(DatabaseAPI.Database.Power.Length);
            fx_Date.Text = Strings.Format(DatabaseAPI.Database.PowerEffectVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            fx_Revision.Text = Convert.ToString(DatabaseAPI.Database.PowerEffectVersion.Revision);
            fx_Count.Text = "Many Lots";
            invent_Date.Text =
                Strings.Format(DatabaseAPI.Database.IOAssignmentVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            invent_Revision.Text = Convert.ToString(DatabaseAPI.Database.IOAssignmentVersion.Revision);
            invent_RecipeDate.Text = Strings.Format(DatabaseAPI.Database.RecipeRevisionDate, "dd/MMM/yy HH:mm:ss");
        }

        private void fx_Import_Click(object sender, EventArgs e)
        {
            var num = (int) new frmImportEffects().ShowDialog();
            DisplayInfo();
        }

        private void invent_Import_Click(object sender, EventArgs e)

        {
            var num = (int) new frmImport_SetAssignments().ShowDialog();
            DisplayInfo();
        }

        private void inventSetImport_Click(object sender, EventArgs e)

        {
            var num = (int) new frmImportEnhSets().ShowDialog();
            DisplayInfo();
        }

        private void level_import_Click(object sender, EventArgs e)

        {
            var num = (int) new frmImportPowerLevels().ShowDialog();
            DisplayInfo();
        }

        private void mod_Import_Click(object sender, EventArgs e)

        {
            var num = (int) new frmImport_mod().ShowDialog();
            DisplayInfo();
        }

        private void pow_Import_Click(object sender, EventArgs e)

        {
            var num = (int) new frmImport_Power().ShowDialog();
            DisplayInfo();
        }

        private void set_Import_Click(object sender, EventArgs e)

        {
            var num = (int) new frmImport_Powerset().ShowDialog();
            DisplayInfo();
        }

        private static void SetEnhancementLevels()

        {
            var num = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (DatabaseAPI.Database.Enhancements[index].TypeID != Enums.eType.SetO ||
                    DatabaseAPI.Database.Enhancements[index].RecipeIDX <= -1 ||
                    DatabaseAPI.Database.Recipes.Length <= DatabaseAPI.Database.Enhancements[index].RecipeIDX ||
                    DatabaseAPI.Database.Recipes[DatabaseAPI.Database.Enhancements[index].RecipeIDX].Item.Length <= 0)
                    continue;
                DatabaseAPI.Database.Enhancements[index].LevelMin = DatabaseAPI.Database
                    .Recipes[DatabaseAPI.Database.Enhancements[index].RecipeIDX].Item[0].Level;
                DatabaseAPI.Database.Enhancements[index].LevelMax = DatabaseAPI.Database
                    .Recipes[DatabaseAPI.Database.Enhancements[index].RecipeIDX].Item[
                        DatabaseAPI.Database.Recipes[DatabaseAPI.Database.Enhancements[index].RecipeIDX].Item.Length -
                        1].Level;
                if (DatabaseAPI.Database.Enhancements[index].nIDSet <= -1)
                    continue;
                DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[index].nIDSet].LevelMin =
                    DatabaseAPI.Database.Enhancements[index].LevelMin;
                DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[index].nIDSet].LevelMax =
                    DatabaseAPI.Database.Enhancements[index].LevelMax;
            }
        }
    }
}