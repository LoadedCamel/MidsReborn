using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms
{
    public partial class frmTimelineColorRefTable : Form
    {
        private frmRotationHelper MyParent;
        private ctlCombatTimeline.ColorTheme ColorTheme;

        public frmTimelineColorRefTable(frmRotationHelper myParent)
        {
            MyParent = myParent;
            ColorTheme = new ctlCombatTimeline.ColorTheme();

            InitializeComponent();
            TopMost = true;
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
        }

        private void frmTimelineColorRefTable_Load(object sender, System.EventArgs e)
        {
            var colorDict = ColorTheme.BuildColorDictionary();
            const int cellH = 48;
            const int cellHsmall = 16;
            const int cellHgap = 4;
            const int cellV = 16;
            const int cellInterline = 10;
            const int labelSpacing = 8;
            const int padding = 8;
            const int columnWidth = 300;
            var cellBorderColor = Color.FromArgb(64, 64, 64);

            var maxH = borderPanel1.Height;

            var x = padding;
            var y = padding;

            SuspendLayout();

            foreach (var colorRef in colorDict)
            {
                for (var i = 0; i < colorRef.Value.Count; i++)
                {
                    var cell = new BorderPanel
                    {
                        Location = new Point(x + i * (cellHsmall + cellHgap), y),
                        Size = new Size(colorRef.Value.Count == 1 ? cellH : cellHsmall, cellV),
                        Border = new BorderPanel.PanelBorder
                        {
                            Color = cellBorderColor,
                            Thickness = 1,
                            Which = BorderPanel.PanelBorder.BorderToDraw.All
                        },
                        BackColor = colorRef.Value[i],
                        ForeColor = Color.WhiteSmoke
                    };

                    borderPanel1.Controls.Add(cell);

                    if (i != colorRef.Value.Count - 1)
                    {
                        continue;
                    }

                    var cellLabel = new Label
                    {
                        Location = new Point(x + (i == 0 ? cellH : (i + 1) * cellHsmall + i * cellHgap) + labelSpacing, y),
                        AutoSize = true,
                        ForeColor = Color.WhiteSmoke,
                        Text = @$"{colorRef.Key.EffectType}{(colorRef.Key.EffectType == Enums.eEffectType.Enhancement & colorRef.Key.ETModifies is null or Enums.eEffectType.None ? "(Generic 1-8)" : colorRef.Key.ETModifies is null or Enums.eEffectType.None ? "" : $"({colorRef.Key.ETModifies})")}"
                    };

                    borderPanel1.Controls.Add(cellLabel);
                }

                y += cellV + cellInterline;
                if (y < maxH - padding - cellV)
                {
                    continue;
                }

                x += columnWidth;
                y = padding;
            }

            ResumeLayout(true);
        }

        private void ibClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
