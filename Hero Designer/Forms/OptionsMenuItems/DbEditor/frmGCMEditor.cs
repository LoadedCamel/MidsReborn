using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Forms.Controls;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbControls;
using Newtonsoft.Json;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class FrmGCMEditor : Form
    {
        private readonly frmMain _myParent;
        private frmBusy _bFrm;

		private void BusyHide()
		{
	        if (_bFrm == null)
		        return;
	        _bFrm.Close();
	        _bFrm = null;
        }

        private void BusyMsg(string sMessage)

        {
	        using var bFrm = new frmBusy();
	        bFrm.Show(this);
	        bFrm.SetMessage(sMessage);
        }

		public FrmGCMEditor(ref frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Load += frmGCMEditor_Load;
            InitializeComponent();
            _myParent = iParent;
        }

        private void frmGCMEditor_Load(object sender, EventArgs e)
        {
            CenterToParent();
            PopulateInfo();
        }

        private void PopulateInfo()
        {
            lvModifiers.BeginUpdate();
            lvModifiers.Items.Clear();
            foreach (var effectId in DatabaseAPI.Database.EffectIds)
            {
                lvModifiers.Items.Add(effectId);
            }

            lvModifiers.Columns[0].Text = @"Current Modifiers";
            lvModifiers.Columns[0].Width = -2;
            lvModifiers.EndUpdate();
        }

        private void ListView_Leave(object sender, EventArgs e)
        {
	        var lvControl = (ctlListViewColored)sender;
	        lvControl.LostFocusItem = lvControl.FocusedItem.Index;
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
	        e.DrawDefault = true;
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
	        var lvControl = (ctlListViewColored)sender;
	        if (e.Item.Selected)
	        {
		        if (lvControl.LostFocusItem == e.Item.Index)
		        {
			        e.Item.BackColor = Color.Goldenrod;
			        e.Item.ForeColor = Color.Black;
			        lvControl.LostFocusItem = -1;
		        }
		        else if (lvControl.Focused)
		        {
			        e.Item.ForeColor = SystemColors.HighlightText;
			        e.Item.BackColor = SystemColors.Highlight;
		        }
	        }
	        else
	        {
		        e.Item.BackColor = lvControl.BackColor;
		        e.Item.ForeColor = lvControl.ForeColor;
	        }
	        e.DrawBackground();
	        e.DrawText();
        }

        private void btnRemoveMod_Click(object sender, EventArgs e)
        {
	        DatabaseAPI.Database.EffectIds.Remove(lvModifiers.SelectedItems[0].Text);
            lvModifiers.SelectedItems[0].Remove();
        }

        private void btnAddMod_Click(object sender, EventArgs e)
        {
	        var newGCM = string.Empty;
	        InputBoxResult result = InputBox.Show("Enter the modifier you wish to add.", "Add Modifier", "Enter the modifier here", InputBox.InputBoxIcon.Info, inputBox_Validating);
	        if (result.OK) { newGCM = result.Text; }
            DatabaseAPI.Database.EffectIds.Add(newGCM);
            lvModifiers.Items.Add(newGCM);
        }

        private void btnImportMods_Click(object sender, EventArgs e)
        {
	        BusyMsg("Importing Global Chance Modifiers from JSON...");
            lvModifiers.Items.Clear();
	        var effects = new List<string>();
            var fileImportDialog = new OpenFileDialog
            {
                InitialDirectory = $"{Application.StartupPath}\\Data\\",
                Title = @"Select JSON formatted GCM file",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "json",
                Filter = @"JSON files (*.json)|*.json",
                FilterIndex = 1,
                RestoreDirectory = true,
                ShowReadOnly = true
            };
            if (fileImportDialog.ShowDialog() == DialogResult.OK)
            {
	            var jsonText = File.ReadAllText(fileImportDialog.FileName);
	            var settings = new JsonSerializerSettings
	            {
		            TypeNameHandling = TypeNameHandling.Auto,
		            Converters = {
			            new AbstractConverter<Database, IDatabase>()
		            }
                };
                effects = JsonConvert.DeserializeObject<List<string>>(jsonText, settings);
            }
            DatabaseAPI.Database.EffectIds = effects;
			PopulateInfo();
			BusyHide();
        }

        private void btnExportMods_Click(object sender, EventArgs e)
        {
	        BusyMsg("Exporting Global Chance Modifiers to JSON...");
            var path = $"{Application.StartupPath}\\Data\\GlobalMods.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(DatabaseAPI.Database.EffectIds, Formatting.Indented));
            BusyHide();
        }

		private void btnSave_Click(object sender, EventArgs e)
		{
			BusyMsg("Saving EffectIds...");
			DatabaseAPI.SaveEffectIdsDatabase();
			BusyHide();
			DialogResult = DialogResult.OK;
			Hide();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Hide();
        }

		private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
		{
			if (e.Text.Trim().Length != 0) return;
			e.Cancel = true;
			e.Message = "Required";
		}
    }

    public class AbstractConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract
    {
	    public override Boolean CanConvert(Type objectType)
		    => objectType == typeof(TAbstract);

	    public override Object ReadJson(JsonReader reader, Type type, Object value, JsonSerializer jser)
		    => jser.Deserialize<TReal>(reader);

	    public override void WriteJson(JsonWriter writer, Object value, JsonSerializer jser)
		    => jser.Serialize(writer, value);
    }
}
