using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Data_Classes;
using Hero_Designer.Forms.Controls;
using Hero_Designer.My;
using midsControls;
using Newtonsoft.Json;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class FrmAttribModImporter : Form
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

		public FrmAttribModImporter(ref frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Load += frmAttribModImporter_Load;
            InitializeComponent();
            _myParent = iParent;
        }

        private void frmAttribModImporter_Load(object sender, EventArgs e)
        {
            CenterToParent();
            PopulateInfo();
        }

        private void PopulateInfo()
        {
	        lvModifiers.Items.Clear();
			lvModifiers.BeginUpdate();
            foreach (var mod in DatabaseAPI.Database.AttribMods.Modifier)
            {
	            var name = mod.ID;
	            var index = Convert.ToString(mod.BaseIndex);
	            lvModifiers.Items.Add(name).SubItems.Add(index);
            }

            lvModifiers.Columns[0].Text = @"Modifier ID";
            lvModifiers.Columns[0].Width = -2;
            lvModifiers.Columns[1].Text = @"Modifier BaseIndex";
            lvModifiers.Columns[1].Width = -2;
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

        private void btnImportMods_Click(object sender, EventArgs e)
        {
	        var fileImportDialog = new OpenFileDialog
            {
                InitialDirectory = $"{Application.StartupPath}\\Data\\",
                Title = @"Select JSON formatted Attribute Modifiers file",
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
	            BusyMsg("Importing Attribute Modifiers from JSON...");
	            lvModifiers.Items.Clear();
	            var jsonText = File.ReadAllText(fileImportDialog.FileName);
	            var settings = new JsonSerializerSettings
	            {
					TypeNameHandling = TypeNameHandling.Auto,
		            NullValueHandling = NullValueHandling.Ignore,
		            PreserveReferencesHandling = PreserveReferencesHandling.None,
		            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		            DefaultValueHandling = DefaultValueHandling.Ignore,
		            Formatting = Formatting.Indented,
					Converters = {
			            new AbstractConverter<Database, IDatabase>(),
		            }
                };
	            DatabaseAPI.Database.AttribMods = JsonConvert.DeserializeObject<Modifiers> (jsonText, settings);
            }
			PopulateInfo();
			BusyHide();
			if (DatabaseAPI.Database.AttribMods != null)
			{
				MessageBox.Show(
					@$"{DatabaseAPI.Database.AttribMods.Modifier.Length} modifier tables imported.",
					@"Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
        }

        private void btnExportMods_Click(object sender, EventArgs e)
        {
	        BusyMsg("Exporting Attribute Modifiers to JSON...");
	        var path = $"{Application.StartupPath}\\Data\\attribModTables.json";
	        var serializerSettings = new JsonSerializerSettings
	        {
		        NullValueHandling = NullValueHandling.Ignore,
		        PreserveReferencesHandling = PreserveReferencesHandling.None,
		        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		        DefaultValueHandling = DefaultValueHandling.Ignore,
		        Formatting = Formatting.Indented
	        };
	        File.WriteAllText(path, JsonConvert.SerializeObject(DatabaseAPI.Database.AttribMods, serializerSettings));
	        BusyHide();
        }

		private void btnSave_Click(object sender, EventArgs e)
		{
			BusyMsg("Saving newly imported Attribute Modifiers...");
			DatabaseAPI.Database.AttribMods?.Store(MyApplication.GetSerializer());
			BusyHide();
			MessageBox.Show($@"Attribute Modifiers have been saved.", @"Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
			DialogResult = DialogResult.OK;
			Hide();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}
    }
}
