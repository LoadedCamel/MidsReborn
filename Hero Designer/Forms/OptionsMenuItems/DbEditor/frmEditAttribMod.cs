using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Base;
using Base.Data_Classes;
using Hero_Designer.My;
using midsControls;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditAttribMod : Form
    {
        private Modifiers TempAttribMods = new Modifiers();
        private DataGraph GraphViewer;

        public frmEditAttribMod()
        {
            InitializeComponent();
            GraphViewer = new DataGraph(new Size(pbGraph.Width, pbGraph.Height));
        }

        private void frmEditAttribMod_Load(object sender, EventArgs e)
        {
            TempAttribMods = (Modifiers)Database.Instance.AttribMods.Clone(); // Safer to work on a temp copy...

            lblRevision.Text = Convert.ToString(TempAttribMods.Revision, null);
            lblRevisionDate.Text = TempAttribMods.RevisionDate.ToString("dd/MM/yyyy", null);

            UpdateModifiersList();
            UpdateClassesList();

            InitializeDataGrid();
            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
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
            string tableName = "";
            DialogResult r = InputBox("Add modifier table", "New modifier table name:", ref tableName);
            if (r == DialogResult.Cancel) return;

            Modifiers.ModifierTable table = new Modifiers.ModifierTable(Database.Instance.Classes.Length)
            {
                BaseIndex = TempAttribMods.Modifier.Length - 1,
                ID = tableName
            };
            _ = TempAttribMods.Modifier.Append(table);

            UpdateModifiersList(table.BaseIndex);
        }

        private void bnRemoveTable_Click(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex >= 0 && listBoxTables.SelectedIndex < listBoxTables.Items.Count - 1)
            {
                MessageBox.Show("You can only remove the last element of the list.", "Dang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TempAttribMods.Modifier.RemoveLast();
            UpdateClassesList(listBoxTables.Items.Count - 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Database.Instance.AttribMods = (Modifiers)TempAttribMods.Clone();
            Database.Instance.AttribMods.Store(MyApplication.GetSerializer());
            
            Close();
        }

        private void btnImportCsv_Click(object sender, EventArgs e)
        {
            // Todo
        }

        private void btnImportJson_Click(object sender, EventArgs e)
        {
            // Todo
        }

        private void btnImportDef_Click(object sender, EventArgs e)
        {
            using OpenFileDialog f = new OpenFileDialog()
            {
                Title = "Select DEF source",
                DefaultExt = "txt",
                Filter = "Text files(*.txt)|*.txt|DEF files(*.def)|*.def|All files(*.*)|*.*",
                FilterIndex = 0,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            DialogResult r = f.ShowDialog();
            if (r != DialogResult.OK) return;

            AttribModDefParser defParser = new AttribModDefParser(f.FileNames[0]);
            Dictionary<string, Dictionary<string, float[]>> parsedDefs = defParser.ParseMods();

            if (parsedDefs.Count == 0) return;

            int nAt = parsedDefs.Count;
            int nTables = 0;
            foreach (KeyValuePair<string, Dictionary<string, float[]>> el in parsedDefs)
            {
                foreach (var _ in parsedDefs[el.Key]) nTables++;
            }

            r = MessageBox.Show(
                Convert.ToString(nAt, null) + " Archetype" + (nAt != 1 ? "s" : "") + " found,\n" +
                Convert.ToString(nTables, null) + " Tables found.\n\nProceed to import? This will overwrite current values.",
                "DEF tables import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            Dictionary<string, int> modsIndexDictionary = new Dictionary<string, int>();
            int i;
            for (i = 0; i < TempAttribMods.Modifier.Length; i++)
            {
                modsIndexDictionary.Add(TempAttribMods.Modifier[i].ID, i);
            }

            Dictionary<string, int> atIndexDictionary = new Dictionary<string, int>();
            for (i = 0; i < Database.Instance.Classes.Length; i++)
            {
                atIndexDictionary.Add(Database.Instance.Classes[i].ClassName, i);
            }

            foreach (KeyValuePair<string, Dictionary<string, float[]>> el in parsedDefs)
            {
                int atIdx = atIndexDictionary.ContainsKey(el.Key) ? atIndexDictionary[el.Key] : -1;
                if (atIdx == -1) continue;

                foreach (KeyValuePair<string, float[]> tData in parsedDefs[el.Key])
                {
                    int modIdx = modsIndexDictionary.ContainsKey(tData.Key) ? modsIndexDictionary[tData.Key] : -1;
                    if (modIdx == -1) continue; // Todo: create mod table if new
                    
                    for (int k = 0; k < Math.Min(tData.Value.Length, TempAttribMods.Modifier[modIdx].Table.Length); k++)
                    {   
                        TempAttribMods.Modifier[modIdx].Table[k][atIdx] = parsedDefs[el.Key][tData.Key][k];
                    }
                }
            }
        }

        private void dgCellsLabel_MouseEnter(object sender, EventArgs e)
        {
            // http://csharphelper.com/blog/2014/09/use-an-event-handler-for-multiple-controls-in-c/
            if (!(sender is Label lbl)) return;
            lbl.BackColor = SystemColors.Highlight;
        }

        private void dgCellsLabel_MouseLeave(object sender, EventArgs e)
        {
            if (!(sender is Label lbl)) return;
            lbl.BackColor = SystemColors.Control;
        }

        private void dgCellsLabel_Click(object sender, EventArgs e)
        {
            if (!(sender is Label lbl)) return;
            MouseEventArgs? ev = e as MouseEventArgs;
            using EditableLabel editor = new EditableLabel
            {
                Location = lbl.PointToScreen(new Point(ev.X + 13, ev.Y + 5)),
                Text = lbl.Text
            };

            if (editor.ShowDialog() != DialogResult.OK) return;

            lbl.Text = editor.Text;
            if (!float.TryParse(lbl.Text, out float dummy)) return;
            TempAttribMods
                .Modifier[listBoxTables.SelectedIndex]
                .Table[Convert.ToInt32(lbl.Name.Substring(6), null)][cbArchetype.SelectedIndex] =
                (float)Convert.ToDecimal(lbl.Text, null);
        }

        private void lblRevision_ValueChanged(object sender, EventArgs e)
        {
            bool res = int.TryParse(lblRevision.Text.Trim(), out TempAttribMods.Revision);
            lblRevision.Text = res ? lblRevision.Text.Trim() : Convert.ToString(TempAttribMods.Revision, null);
        }

        private void lblRevisionDate_TextChanged(object sender, EventArgs e)
        {
            bool res = DateTime.TryParseExact(
                lblRevisionDate.Text.Trim(),
                "dd/mm/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out TempAttribMods.RevisionDate);
            lblRevisionDate.Text = res ? lblRevisionDate.Text.Trim() : TempAttribMods.RevisionDate.ToString("dd/MM/yyyy", null);
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
            using Label label = new Label()
            {
                Text = promptText,
                AutoSize = true,
                Bounds = new Rectangle(9, 10, 372, 13)
            };

            using TextBox textBox = new TextBox()
            {
                Text = value,
                TextAlign = HorizontalAlignment.Left,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Bounds = new Rectangle(12, 26, 372, 20)
            };

            using Button buttonOk = new Button()
            {
                Text = "OK",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Bounds = new Rectangle(228, 62, 75, 23),
                DialogResult = DialogResult.OK,
                TextAlign = ContentAlignment.MiddleCenter
            };

            using Button buttonCancel = new Button()
            {
                Text = "Cancel",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Bounds = new Rectangle(309, 62, 75, 23),
                DialogResult = DialogResult.Cancel,
                TextAlign = ContentAlignment.MiddleCenter
            };

            using Form inputBoxFrm = new Form()
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

            DialogResult dialogResult = inputBoxFrm.ShowDialog();
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
            cbArchetype.Items.AddRange(Database.Instance.Classes.Select(e => e.ClassName).ToArray() as object[]);
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
            int i; int l;

            SuspendLayout();
            l = Convert.ToInt32(Math.Ceiling(TempAttribMods.Modifier[modIdx].Table.Length / 10d)) * 10;
            for (i = 0; i < l; i++)
            {
                if (i < TempAttribMods.Modifier[modIdx].Table.Length &&
                TempAttribMods.Modifier[modIdx].Table[i].Length > 0) {
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
            
            List<float> dataSeries = new List<float>();
            for (i = 0; i < TempAttribMods.Modifier[modIdx].Table.Length; i++)
            {
                if (TempAttribMods.Modifier[modIdx].Table[i].Length > 0)
                {
                    dataSeries.Add(TempAttribMods.Modifier[modIdx].Table[i][atIdx]);
                }
            }

            GraphViewer.SetOption("SmoothDraw", smoothDraw);
            GraphViewer.SetOption("DetectPlateau", detectPlateau);
            GraphViewer.SetOption("IgnoreValue", ignoreValue);

            GraphViewer.SetDataPoints(dataSeries);
            pbGraph.Image = GraphViewer.Draw();
        }
    }
}