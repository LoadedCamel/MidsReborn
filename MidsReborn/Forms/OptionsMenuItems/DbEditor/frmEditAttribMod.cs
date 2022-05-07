#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbControls;
using Newtonsoft.Json;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditAttribMod : Form
    {
        private Modifiers TempAttribMods = new Modifiers();
        private frmBusy? _bFrm;

        public frmEditAttribMod()
        {
            InitializeComponent();
        }

        private void frmEditAttribMod_Load(object sender, EventArgs e)
        {
            CenterToParent();
            // Safer to work on a temp copy...
            TempAttribMods = (Modifiers)Database.Instance.AttribMods.Clone();

            lblRevision.Text = Convert.ToString(TempAttribMods.Revision, null);
            lblRevisionDate.Text = TempAttribMods.RevisionDate.ToString("MM/dd/yyyy", null);

            UpdateModifiersList();
            UpdateClassesList();

            InitializeDataGrid();
            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

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

        #region Event Handlers

        private void listBoxTables_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex <= -1 || cbArchetype.SelectedIndex <= -1 ||
                TempAttribMods.Modifier.ElementAtOrDefault(listBoxTables.SelectedIndex) == null) return;
            
            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

        private void cbArchetype_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateDataDisplay();
        }

        private void btnAddNewTable_Click(object sender, EventArgs e)
        {
            var tableName = "";
            var r = InputBox("Add modifier table", "New modifier table name:", ref tableName);
            if (r == DialogResult.Cancel) return;

            var table = new Modifiers.ModifierTable(Database.Instance.Classes.Length)
            {
                BaseIndex = TempAttribMods.Modifier.Count - 1, // Zed: This may not be correct, but this field is not used anywhere.
                ID = tableName
            };
            _ = TempAttribMods.Modifier.Append(table);

            UpdateModifiersList(TempAttribMods.Modifier.Count - 1);
        }

        private void bnRemoveTable_Click(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex >= 0 && listBoxTables.SelectedIndex < listBoxTables.Items.Count - 1)
            {
                MessageBox.Show("You can only remove the last element of the list.", "Dang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TempAttribMods.Modifier.RemoveAt(TempAttribMods.Modifier.Count - 1);
            UpdateClassesList(listBoxTables.Items.Count - 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TempAttribMods.Revision = Convert.ToInt32(lblRevision.Text);
            TempAttribMods.RevisionDate = Convert.ToDateTime(lblRevisionDate.Text);
            Database.Instance.AttribMods = (Modifiers)TempAttribMods.Clone();
            //Database.Instance.AttribMods.Store(Serializer.GetSerializer());
            //DatabaseAPI.UpdateModifiersDict(Database.Instance.AttribMods.Modifier);

            // This one barely shows up.
            // May only be needed if one has Mids on a ZIP drive!
            BusyMsg("Saving newly imported Attribute Modifiers...");
            Database.Instance.AttribMods = (Modifiers)TempAttribMods.Clone();
            DatabaseAPI.Database.AttribMods?.Store(Serializer.GetSerializer());
            BusyHide();
            //MessageBox.Show($@"Attribute Modifiers have been saved.", @"Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnImportJson_Click(object sender, EventArgs e)
        {
            using var f = new OpenFileDialog()
            {
                Title = "Select JSON source",
                DefaultExt = "json",
                Filter = "JSON files(*.json)|*.json|All files(*.*)|*.*",
                FilterIndex = 0,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            var r = f.ShowDialog();
            if (r != DialogResult.OK) return;

            var m = new Modifiers();
            var src = File.ReadAllText(f.FileName);

            try
            {
                if (src.TrimStart(' ', '\t', '\r', '\n', '\0').StartsWith("["))
                {
                    //Modifiers.ModifierTable[]? tables = JsonConvert.DeserializeObject<Modifiers.ModifierTable[]>(src, jsonOpt);
                    var tables = JsonConvert.DeserializeObject<List<Modifiers.ModifierTable>>(src, Serializer.SerializerSettings);
                    if (tables == null) throw new FormatException("JSON file contains no modifier tables.");

                    m.Modifier = (List<Modifiers.ModifierTable>) tables.Clone();
                    m.Revision = Database.Instance.AttribMods.Revision + 1;
                    m.RevisionDate = DateTime.Now;
                    m.SourceIndex = string.Empty;
                    m.SourceTables = f.FileName;
                }
                else if (src.TrimStart(' ', '\t', '\r', '\n', '\0').StartsWith("{"))
                {
                    m = JsonConvert.DeserializeObject<Modifiers>(src, Serializer.SerializerSettings);
                    if (m == null) throw new FormatException("JSON file contains no usable data.");
                }
                else
                {
                    throw new FormatException("Malformed JSON file.");
                }
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show("[JsonSerializationException] Error deserializing JSON.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }
            catch (JsonReaderException ex)
            {
                MessageBox.Show("[JsonReaderException] Error reading JSON.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("[FormatException] JSON file format exception.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }

            var nAt = m.Modifier[0].Table.Count;
            var nMods = m.Modifier.Count;

            r = MessageBox.Show(
                Convert.ToString(nAt, null) + " Archetype" + (nAt != 1 ? "s" : "") + "/Entit" + (nAt != 1 ? "ies" : "y") + " found,\n" +
                Convert.ToString(nMods, null) + " Mod" + (nMods!= 1 ? "s" : "") + " found.\n\nProceed to import? This will overwrite current values.\n",
                "JSON tables import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            TempAttribMods = (Modifiers)m.Clone();

            lblRevision.Text = Convert.ToString(TempAttribMods.Revision, null);
            lblRevisionDate.Text = TempAttribMods.RevisionDate.ToString("dd/MM/yyyy", null);

            UpdateModifiersList();
            UpdateClassesList();
        }

        private void dgCellsLabel_MouseEnter(object sender, EventArgs e)
        {
            // http://csharphelper.com/blog/2014/09/use-an-event-handler-for-multiple-controls-in-c/
            if (sender is not Label lbl) return;
            lbl.BackColor = SystemColors.Highlight;
        }

        private void dgCellsLabel_MouseLeave(object sender, EventArgs e)
        {
            if (sender is not Label lbl) return;
            lbl.BackColor = SystemColors.Control;
        }

        private void dgCellsLabel_Click(object sender, EventArgs e)
        {
            if (sender is not Label lbl) return;
            var ev = e as MouseEventArgs;
            using var editor = new EditableLabel
            {
                Location = lbl.PointToScreen(new Point(ev.X + 13, ev.Y + 5)),
                Text = lbl.Text
            };

            if (editor.ShowDialog() != DialogResult.OK) return;

            lbl.Text = editor.Text;
            if (!float.TryParse(lbl.Text, out var dummy)) return;
            TempAttribMods
                .Modifier[listBoxTables.SelectedIndex]
                .Table[Convert.ToInt32(lbl.Name.Substring(6), null)][cbArchetype.SelectedIndex] =
                (float)Convert.ToDecimal(lbl.Text, null);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

        private void lblRevision_ValueChanged(object sender, EventArgs e)
        {
            var res = int.TryParse(lblRevision.Text.Trim(), out TempAttribMods.Revision);
            lblRevision.Text = res ? lblRevision.Text.Trim() : Convert.ToString(TempAttribMods.Revision, null);
        }

        private void lblRevisionDate_Leave(object sender, EventArgs e)
        {
            var res = DateTime.TryParseExact(
                lblRevisionDate.Text.Trim(),
                "MM/dd/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out TempAttribMods.RevisionDate);
            lblRevisionDate.Text = res ? lblRevisionDate.Text.Trim() : TempAttribMods.RevisionDate.ToString("MM/dd/yyyy", null);
        }

        private void lblRevisionDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*
             https://docs.microsoft.com/en-us/dotnet/framework/winforms/user-input-validation-in-windows-forms?redirectedfrom=MSDN
             The Validating event is supplied an object of type CancelEventArgs.
             If you determine that the control's data is not valid,
             you can cancel the Validating event by setting this object's Cancel property to true.
             If you do not set the Cancel property,
             Windows Forms will assume that validation succeeded for that control,
             and raise the Validated event.
            */
        }

        private void lblRevision_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        #endregion

        // VB's InputBox() clone
        // To be done later: move this to a separate class/user control
        private static DialogResult InputBox(string title, string promptText, ref string value)
        {
            using var label = new Label()
            {
                Text = promptText,
                AutoSize = true,
                Bounds = new Rectangle(9, 10, 372, 13)
            };

            using var textBox = new TextBox()
            {
                Text = value,
                TextAlign = HorizontalAlignment.Left,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Bounds = new Rectangle(12, 26, 372, 20)
            };

            using var buttonOk = new Button()
            {
                Text = "OK",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Bounds = new Rectangle(228, 62, 75, 23),
                DialogResult = DialogResult.OK,
                TextAlign = ContentAlignment.MiddleCenter
            };

            using var buttonCancel = new Button()
            {
                Text = "Cancel",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Bounds = new Rectangle(309, 62, 75, 23),
                DialogResult = DialogResult.Cancel,
                TextAlign = ContentAlignment.MiddleCenter
            };

            using var inputBoxFrm = new Form()
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                Text = title,
                ClientSize = new Size(390, 90)
            };

            inputBoxFrm.Controls.AddRange(new Control[] {label, textBox, buttonOk, buttonCancel});

            var dialogResult = inputBoxFrm.ShowDialog();
            value = textBox.Text;

            return dialogResult;
        }

        private void UpdateModifiersList(int selectedIndex = 0)
        {
            listBoxTables.BeginUpdate();
            listBoxTables.Items.Clear();
            // as object[] required to soft-cast from string[] to object[]
            // (VS may object this)
            listBoxTables.Items.AddRange(TempAttribMods.Modifier.Select(e => e.ID).ToArray() as object[]);
            listBoxTables.SelectedIndex = selectedIndex;
            listBoxTables.EndUpdate();
        }

        private void UpdateClassesList(int selectedIndex = 0)
        {
            cbArchetype.BeginUpdate();
            cbArchetype.Items.Clear();
            // e.DisplayName + e.ClassName is too long!
            cbArchetype.Items.AddRange(DatabaseAPI.Database.Classes.Select(e => e.ClassName).ToArray() as object[]);
            cbArchetype.SelectedIndex = selectedIndex;
            cbArchetype.EndUpdate();
        }

        private void UpdateDataDisplay()
        {
            if (listBoxTables.SelectedIndex <= -1 || cbArchetype.SelectedIndex <= -1) return;
            
            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

        // 'Cause Designer is not happy if one touch his things. Boo.
        // So instead of putting the labels array initialize routine in the form designer it goes here.
        // Different ways, same result, I guess...
        private void InitializeDataGrid()
        {
            SuspendLayout();

            dgCells = new Control[60];
            int i;
            for (i = 0; i < dgCells.Length; i++)
            {
                dgCells[i] = new Label()
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(257 + 62 * (i % 10), 32 + 50 * Convert.ToInt32(Math.Floor(i / 10d))),
                    Margin = new Padding(0),
                    Name = "dgCell" + Convert.ToString(i, null),
                    Size = new Size(62, 50),
                    TabIndex = 29 + i,
                    Text = "0",
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };

                dgCells[i].MouseEnter += dgCellsLabel_MouseEnter;
                dgCells[i].MouseLeave += dgCellsLabel_MouseLeave;
                dgCells[i].Click += dgCellsLabel_Click;
            }
            Controls.AddRange(dgCells);

            ResumeLayout();
        }

        private void PopulateTableGrid(int modIdx, int atIdx)
        {
            int i;

            SuspendLayout();
            var l = Convert.ToInt32(Math.Ceiling(TempAttribMods.Modifier[modIdx].Table.Count / 10d)) * 10;
            for (i = 0; i < l; i++)
            {
                if (i < TempAttribMods.Modifier[modIdx].Table.Count && TempAttribMods.Modifier[modIdx].Table[i].Count > 0) 
                {
                    dgCells[i].Text = Convert.ToString(TempAttribMods.Modifier[modIdx].Table[i][atIdx], null);
                    dgCells[i].Enabled = true;
                    dgCells[i].BackColor = SystemColors.Control;
                }
                else
                {
                    dgCells[i].Text = "";
                    dgCells[i].Enabled = false;
                    dgCells[i].BackColor = SystemColors.ControlDark;
                }
            }

            ResumeLayout();
        }

        private void DrawDataGraph(int modIdx, int atIdx, bool smoothDraw = true, bool detectPlateau = true, float? ignoreValue = 0)
        {
            int i;
            
            var dataSeries = new List<float>();
            for (i = 0; i < TempAttribMods.Modifier[modIdx].Table.Count; i++)
            {
                if (TempAttribMods.Modifier[modIdx].Table[i].Count > 0)
                {
                    dataSeries.Add(TempAttribMods.Modifier[modIdx].Table[i][atIdx]);
                }
            }

            pbGraph.Graph.SetSize(new Size(pbGraph.Width, pbGraph.Height));
            pbGraph.Graph.SmoothDraw = smoothDraw;
            pbGraph.Graph.DetectPlateau = detectPlateau;
            pbGraph.Graph.IgnoreValue = ignoreValue;
            pbGraph.Graph.ForceInvertValues = TempAttribMods.Modifier[modIdx].ID.ToLower().Contains("damage");

            pbGraph.Graph.SetDataPoints(dataSeries);
            pbGraph.Refresh();
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            DatabaseAPI.ExportAttribMods();
            MessageBox.Show("Export complete.", "Woop", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}