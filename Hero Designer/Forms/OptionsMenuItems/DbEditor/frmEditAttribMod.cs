using Base.Data_Classes;
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
            cbArchetype.Items.AddRange(Database.Instance.Classes.Select(e => { return e.DisplayName; }).ToArray());
            cbArchetype.SelectedIndex = 0;
            cbArchetype.EndUpdate();

            PopulateTableGrid(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
            DrawDataGraph(listBoxTables.SelectedIndex, cbArchetype.SelectedIndex);
        }

        // Prevent selecting invalid cells
        private void dgTableData_SelectionChanged(object sender, EventArgs e)
        {
            if (dgTableData.SelectedCells.Count == 0) return;

            if (dgTableData.SelectedCells[0].ReadOnly)
            {
                dgTableData.SelectedCells[0].Selected = false;
            }
        }

        // //////////////////////////////////// //

        private void PopulateTableGrid(int modIdx, int atIdx)
        {
            int i; int l;
            int x; int y;

            // Cells dimensions
            int h = Convert.ToInt32(Math.Floor(dgTableData.Height / 6d));

            dgTableData.Rows.Clear();

            l = Convert.ToInt32(Math.Ceiling(Database.Instance.AttribMods.Modifier[modIdx].Table.Length / 10d)) * 10;
            for (i = 0; i < l; i++)
            {
                x = i % 10;
                y = Convert.ToInt32(Math.Floor(i / 10d));

                if (i % 10 == 0)
                {
                    dgTableData.Rows.Add();
                    dgTableData.Rows[y].Height = h - ((y == 5) ? 2 : 0);
                    dgTableData.Rows[y].Resizable = DataGridViewTriState.False;
                }

                if (i < Database.Instance.AttribMods.Modifier[modIdx].Table.Length)
                {
                    dgTableData.Rows[y].Cells[x].Value = Convert.ToString(Database.Instance.AttribMods.Modifier[modIdx].Table[i][atIdx], null);
                    dgTableData.Rows[y].Cells[x].ReadOnly = false;
                    dgTableData.Rows[y].Cells[x].Style.BackColor = SystemColors.Control;
                }
                else
                {
                    dgTableData.Rows[y].Cells[x].Value = "";
                    dgTableData.Rows[y].Cells[x].ReadOnly = true;
                    dgTableData.Rows[y].Cells[x].Style.BackColor = SystemColors.ControlDark;
                }
            }

            dgTableData.Rows[0].Cells[0].Selected = false;
        }

        private void DrawDataGraph(int modIdx, int atIdx, bool smoothDraw = true, bool detectPlateau = true, float ignoreValue = 0)
        {
            Color lineColor = Color.Yellow;
            Color dotsColor = Color.DarkSeaGreen;
            Color bgColor = Color.Black;
            Color axisColor = Color.Silver;
            Color innerGridColor = Color.DarkGray;
            Color axisValuesColor = Color.Gainsboro;
            int[] padding = new int[4] { 8, 8, 15, 15 }; // CSS order: top, right, bottom, left

            int i;
            int w = pbGraph.Width - padding[1] - padding[3];
            int h = pbGraph.Height - padding[0] - padding[2];
            int dotRadius = 1;

            float xMin = 0;
            float xMax = Database.Instance.AttribMods.Modifier[modIdx].Table.Length;
            float yMin;
            float yMax;
            float xPlateau = float.NegativeInfinity;
            float v;

            float[] dataSeries = new float[Database.Instance.AttribMods.Modifier[modIdx].Table.Length];
            for (i = 0; i < Database.Instance.AttribMods.Modifier[modIdx].Table.Length; i++)
            {
                dataSeries[i] = Database.Instance.AttribMods.Modifier[modIdx].Table[i][atIdx];
            }

            v = dataSeries[0]; // Only because it needs to be forcibly initialized.
            yMin = dataSeries.Min();
            yMax = dataSeries.Max();

            for (i = dataSeries.Length - 1; i > 0; i--)
            {
                if (dataSeries[i] != ignoreValue)
                {
                    v = dataSeries[i];
                    break;
                }
            }

            for (i = dataSeries.Length - 1; i > 0; i--)
            {
                if (dataSeries[i] != v && i < dataSeries.Length - 2)
                {
                    xPlateau = i + 1;
                    break;
                }
            }

            Pen p;
            Bitmap res = new Bitmap(pbGraph.Width, pbGraph.Height);
            using (var g = Graphics.FromImage(res))
            {
                // Zed: I kinda want to use Anti-aliasing here but it also turns the dotted lines into plain ones,
                // and the general apperance is pretty bad.
                //g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Black);

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
                        g.DrawLine(p, new Point((int)Math.Round(i * xScale) + padding[3], padding[0]), new Point((int)Math.Round(i * xScale) + padding[3], h + padding[0]));
                    }

                    // Horizontals
                    for (i = padding[0]; i <= h + padding[0]; i += h / 5)
                    {
                        g.DrawLine(p, new Point(padding[3], i), new Point(w + padding[3], i));
                    }
                }

                // /////////////////////// //
                // Axis
                using (p = new Pen(color: axisColor) { DashStyle = DashStyle.Solid })
                {
                    g.DrawLine(p, new Point(padding[3], h + padding[0]), new Point(w + padding[3], h + padding[0]));
                    g.DrawLine(p, new Point(padding[3], padding[0]), new Point(padding[3], h + padding[0]));
                    // May need to draw the axis for value 0 here, if needed.
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
                    for (i = 0; i < dataSeries.Length; i++)
                    {
                        if (dataSeries[i] == ignoreValue) continue;
                        val = dataSeries[i] * (reverseValues ? -1 : 1);
                        val = h + padding[0] - val * yScale;
                        dataPoints.Add(new Point((int)(i * xScale + padding[3]), (int)val));
                    }

                    if (smoothDraw)
                    {
                        g.DrawCurve(p, dataPoints.ToArray());
                    }
                    else
                    {
                        g.DrawLines(p, dataPoints.ToArray());
                    }
                }
            }

            pbGraph.Image = res;
        }

        private static bool CheckAllNegativeValues(float[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > 0) return false;
            }

            return true;
        }
    }
}