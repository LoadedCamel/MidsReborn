using Base.Data_Classes;
using midsControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditAttribMod : Form
    {

        public frmEditAttribMod()
        {
            InitializeComponent();
        }

        private void frmEditAttribMod_Load(object sender, EventArgs e)
        {
            lblRevision.Text = Convert.ToString(Database.Instance.AttribMods.Revision, null);
            lblRevisionDate.Text = Database.Instance.AttribMods.RevisionDate.ToString("dd/MM/yyyy", null);

            listBoxTables.BeginUpdate();
            listBoxTables.Items.Clear();
            listBoxTables.Items.AddRange(Database.Instance.AttribMods.Modifier.Select(e => { return e.ID; }).ToArray());
            listBoxTables.SelectedIndex = 0;
            listBoxTables.EndUpdate();

            // Zed: This list is not subject to change any time soon.
            // Maybe worth the shot not to clear it every time the form is loaded.
            cbArchetype.BeginUpdate();
            cbArchetype.Items.Clear();
            cbArchetype.Items.AddRange(Database.Instance.Classes.Select(e => { return e.ClassName; }).ToArray()); // e.DisplayName + e.ClassName is too long!
            cbArchetype.SelectedIndex = 0;
            cbArchetype.EndUpdate();

            InitializeDataGrid();
            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

        // //////////////////////////////////// //

        private void listBoxTables_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex > -1 &&
                cbArchetype.SelectedIndex > -1 &&
                Database.Instance.AttribMods.Modifier.ElementAtOrDefault(listBoxTables.SelectedIndex) != null)
            {
                PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
                DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            }
        }

        private void cbArchetype_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateDataDisplay();
        }

        private void btnAddNewTable_Click(object sender, EventArgs e)
        {

        }

        private void bnRemoveTable_Click(object sender, EventArgs e)
        {

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
            Label lbl = sender as Label;
            lbl.BackColor = SystemColors.Highlight;
        }

        private void dgCellsLabel_MouseLeave(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            lbl.BackColor = SystemColors.Control;
        }

        private void dgCellsLabel_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            MouseEventArgs ev = e as MouseEventArgs;

            if (lbl != null)
            {
                using EditableLabel editor = new EditableLabel
                {
                    Location = lbl.PointToScreen(new Point(ev.X + 13, ev.Y + 5)),
                    Text = lbl.Text
                };

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    lbl.Text = editor.Text;
                }
            }
        }

        // //////////////////////////////////// //

        private void UpdateDataDisplay()
        {
            if (listBoxTables.SelectedIndex > -1 && cbArchetype.SelectedIndex > -1)
            {
                PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
                DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            }
        }

        // 'Cause Designer is not happy if one touch his things. Boo.
        private void InitializeDataGrid()
        {
            SuspendLayout();

            dgCells = new Label[60];
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

                dgCells[i].MouseEnter += new EventHandler(dgCellsLabel_MouseEnter);
                dgCells[i].MouseLeave += new EventHandler(dgCellsLabel_MouseLeave);
                dgCells[i].Click += new EventHandler(dgCellsLabel_Click);
            }
            Controls.AddRange(dgCells);

            ResumeLayout();
        }

        private void PopulateTableGrid(int modIdx, int atIdx)
        {
            int i; int l;

            SuspendLayout();
            l = Convert.ToInt32(Math.Ceiling(Database.Instance.AttribMods.Modifier[modIdx].Table.Length / 10d)) * 10;
            for (i = 0; i < l; i++)
            {
                if (i < Database.Instance.AttribMods.Modifier[modIdx].Table.Length &&
                Database.Instance.AttribMods.Modifier[modIdx].Table[i].Length > 0) {
                    dgCells[i].Text = Convert.ToString(Database.Instance.AttribMods.Modifier[modIdx].Table[i][atIdx], null);
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

        private void DrawDataGraph(int modIdx, int atIdx, bool smoothDraw = true, bool detectPlateau = true, float ignoreValue = 0)
        {
            Color lineColor = Color.Yellow;
            Color dotsColor = Color.FromArgb(Convert.ToInt32(0.66 * 256), Color.LightGreen); // Alpha = 90%
            Color bgColor = Color.Black;
            Color axisColor = Color.Silver;
            Color innerGridColor = Color.DarkGray;
            Color axisValuesColor = Color.Gainsboro;
            Padding padding = new Padding(15, 8, 8, 15);

            bool drawDots = false;

            int i;
            int w = pbGraph.Width - padding.Horizontal;
            int h = pbGraph.Height - padding.Vertical;
            int dotRadius = 1;

            float xMin = 0;
            float xMax = Database.Instance.AttribMods.Modifier[modIdx].Table.Length;
            float yMin;
            float yMax;
            float xPlateau = float.NegativeInfinity;
            float v;

            List<float> dataSeries = new List<float>();
            for (i = 0; i < Database.Instance.AttribMods.Modifier[modIdx].Table.Length; i++)
            {
                if (Database.Instance.AttribMods.Modifier[modIdx].Table[i].Length > 0)
                {
                    dataSeries.Add(Database.Instance.AttribMods.Modifier[modIdx].Table[i][atIdx]);
                }
            }

            v = dataSeries[0]; // Only because it needs to be forcibly initialized.
            yMin = dataSeries.Min();
            yMax = dataSeries.Max();

            for (i = dataSeries.Count - 1; i > 0; i--)
            {
                if (dataSeries[i] != ignoreValue)
                {
                    v = dataSeries[i];
                    break;
                }
            }

            for (i = dataSeries.Count - 1; i > 0; i--)
            {
                if (dataSeries[i] != v && i < dataSeries.Count - 2)
                {
                    xPlateau = i + 1;
                    break;
                }
            }

            pbGraph.SuspendLayout();

            Pen p;
            Bitmap res = new Bitmap(pbGraph.Width, pbGraph.Height);
            using (var g = Graphics.FromImage(res))
            {
                // Zed: I kinda want to use Anti-aliasing here but it also turns the dotted lines into plain ones,
                // and the general apperance is pretty bad.
                //g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(bgColor);

                /*
                 * Layers:
                 * Inner grid (dotted)
                 * Axis
                 * Lines
                 * Dots
                 * Plateau secondary X axis
                 * Axis Values
                 */

                // Warning CA2000 - call Dispose() on object before all references to it are out of scope
                // https://stackoverflow.com/questions/16920386/call-system-idisposable-dispose-on-object-emailform-before-all-references-to-i

                float xScale = w / Math.Max(xMax - xMin, 1);
                float yScale = h / Math.Max(yMax - yMin, 1);

                // /////////////////////// //
                // Inner Grid
                using (p = new Pen(color: innerGridColor) { DashStyle = DashStyle.Dot })
                {
                    // Verticals
                    for (i = (int)xMin; i <= xMax; i += 5)
                    {
                        g.DrawLine(p, new Point((int)Math.Round(i * xScale) + padding.Left, padding.Top), new Point((int)Math.Round(i * xScale) + padding.Left, h + padding.Top));
                    }

                    // Horizontals
                    for (i = padding.Top; i <= h + padding.Top; i += h / 5)
                    {
                        g.DrawLine(p, new Point(padding.Left, i), new Point(w + padding.Left, i));
                    }
                }

                // /////////////////////// //
                // Axis
                using (p = new Pen(color: axisColor) { DashStyle = DashStyle.Solid })
                {
                    g.DrawLine(p, new Point(padding.Left, h + padding.Top), new Point(w + padding.Left, h + padding.Top));
                    g.DrawLine(p, new Point(padding.Left, padding.Top), new Point(padding.Left, h + padding.Top));
                    if (DataSetCrossZero(dataSeries)) // May need to draw the axis for value 0 here, if needed.
                    {
                        g.DrawLine(p, new Point(), new Point());
                    }
                }

                // /////////////////////// //
                // Lines
                // Note: negate values if they are all negative, e.g. for damage tables.
                bool reverseValues = CheckAllNegativeValues(dataSeries);
                float val;

                // Not using Array for graph data points.
                // It may not have the same lengh as the source dataSeries.
                List<Point> dataPoints = new List<Point>();

                using (p = new Pen(color: lineColor) { DashStyle = DashStyle.Solid, Width = 1 })
                {
                    for (i = 0; i < Math.Min(50, dataSeries.Count); i++)
                    {
                        if (dataSeries[i] == ignoreValue) continue;
                        val = dataSeries[i] * (reverseValues ? -1 : 1);
                        val = h + padding.Top - val * yScale;
                        dataPoints.Add(new Point((int)(i * xScale + padding.Left), (int)val));
                    }

                    if (dataPoints.Count < 2) return;

                    if (smoothDraw)
                    {
                        g.DrawCurve(p, dataPoints.ToArray());
                    }
                    else
                    {
                        g.DrawLines(p, dataPoints.ToArray());
                    }
                }

                // /////////////////////// //
                // Dots
                if (drawDots)
                {
                    using (p = new Pen(color: dotsColor))
                    {
                        for (i = 0; i < Math.Min(50, dataSeries.Count); i++)
                        {
                            if (dataSeries[i] == ignoreValue) continue;
                            val = dataSeries[i] * (reverseValues ? -1 : 1);
                            val = h + padding.Top - val * yScale;
                            g.DrawEllipse(p, new Rectangle(new Point((int)(i * xScale + padding.Left) - dotRadius, (int)val - dotRadius), new Size(dotRadius * 2 + 1, dotRadius * 2 + 1)));
                        }
                    }
                }
            }

            pbGraph.Image = res;
            pbGraph.ResumeLayout();
        }

        private static bool CheckAllNegativeValues(List<float> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] > 0) return false;
            }

            return true;
        }

        private static bool DataSetCrossZero(List<float> values)
        {
            bool vPositive = false;
            bool vNegative = false;
            for (int i = 0; i<values.Count; i++)
            {
                if (values[i] > 0) vPositive = true;
                if (values[i] < 0) vNegative = true;
            }

            return (vPositive & vNegative);
        }
    }
}