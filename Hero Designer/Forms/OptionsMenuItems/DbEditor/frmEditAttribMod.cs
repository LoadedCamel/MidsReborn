using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Base;
using Base.Data_Classes;
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

            listBoxTables.BeginUpdate();
            listBoxTables.Items.Clear();
            listBoxTables.Items.AddRange(TempAttribMods.Modifier.Select(e => e.ID).ToArray());
            listBoxTables.SelectedIndex = 0;
            listBoxTables.EndUpdate();

            cbArchetype.BeginUpdate();
            cbArchetype.Items.Clear();
            cbArchetype.Items.AddRange(Database.Instance.Classes.Select(e => e.ClassName).ToArray()); // e.DisplayName + e.ClassName is too long!
            cbArchetype.SelectedIndex = 0;
            cbArchetype.EndUpdate();

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

            listBoxTables.BeginUpdate();
            listBoxTables.Items.Clear();
            listBoxTables.Items.AddRange(TempAttribMods.Modifier.Select(e => e.ID).ToArray());
            listBoxTables.SelectedIndex = table.BaseIndex;
            listBoxTables.EndUpdate();
        }

        private void bnRemoveTable_Click(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex >= 0 && listBoxTables.SelectedIndex < listBoxTables.Items.Count - 1)
            {
                MessageBox.Show("You can only remove the last element of the list.", "Dang", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TempAttribMods.Modifier.RemoveLast();

            listBoxTables.BeginUpdate();
            listBoxTables.Items.Clear();
            listBoxTables.Items.AddRange(TempAttribMods.Modifier.Select(e => e.ID).ToArray());
            listBoxTables.SelectedIndex = listBoxTables.Items.Count - 1;
            listBoxTables.EndUpdate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnImportCsv_Click(object sender, EventArgs e)
        {

        }

        private void btnImportJson_Click(object sender, EventArgs e)
        {

        }

        private void btnImportDef_Click(object sender, EventArgs e)
        {

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

        #endregion

        // VB's InputBox() clone
        // To be done later: move this to a separate class/user control
        private static DialogResult InputBox(string title, string promptText, ref string value)
        {
            using Label label = new Label()
            {
                Text = promptText,
                AutoSize = true,
                Bounds = new Rectangle(9, 20, 372, 13)
            };

            using TextBox textBox = new TextBox()
            {
                Text = value,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Bounds = new Rectangle(12, 36, 372, 20)
            };

            using Button buttonOk = new Button()
            {
                Text = "OK",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Bounds = new Rectangle(228, 72, 75, 23),
                DialogResult = DialogResult.OK
            };

            using Button buttonCancel = new Button()
            {
                Text = "Cancel",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Bounds = new Rectangle(309, 72, 75, 23),
                DialogResult = DialogResult.Cancel
            };

            using Form inputBoxFrm = new Form()
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                Text = title
            };

            inputBoxFrm.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            inputBoxFrm.ClientSize = new Size(Math.Max(300, label.Right + 10), inputBoxFrm.ClientSize.Height);

            DialogResult dialogResult = inputBoxFrm.ShowDialog();
            value = textBox.Text;

            return dialogResult;
        }

        private void UpdateDataDisplay()
        {
            if (listBoxTables.SelectedIndex <= -1 || cbArchetype.SelectedIndex <= -1) return;
            
            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

        // 'Cause Designer is not happy if one touch his things. Boo.
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