using System;
using System.Drawing;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
using midsControls;

namespace Hero_Designer.Forms.WindowMenuItems
{
    public partial class frmTotalsV2 : Form
    {
        private class LayeredBar
        {
            private Panel? BaseVal;
            private Panel? OverCap;
            private Panel Bar;
            private Panel? Overlay1;
            private Panel? Overlay2;
            private Label Value;
            private int MaxWidth;

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;

                BaseVal = null;
                OverCap = null;
                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel overCap)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                OverCap = overCap;

                BaseVal = null;
                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth, int dummmy,
                ref Panel baseVal)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;

                OverCap = null;
                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel baseVal, ref Panel overCap)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;
                OverCap = overCap;

                Overlay1 = null;
                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel baseVal, ref Panel overCap, ref Panel overlay1)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;
                OverCap = overCap;
                Overlay1 = overlay1;

                Overlay2 = null;
            }

            public LayeredBar(ref Panel bar, ref Label value, int barMaxWidth,
                ref Panel baseVal, ref Panel overCap, ref Panel overlay1, ref Panel overlay2)
            {
                Bar = bar;
                Value = value;
                MaxWidth = barMaxWidth;
                BaseVal = baseVal;
                OverCap = overCap;
                Overlay1 = overlay1;
                Overlay2 = overlay2;
            }
        }

        private readonly frmMain _myParent;
        private bool _keepOnTop;
        private LayeredBar[] Bars;
        private readonly int BarMaxWidth = 277;

        private void InitBars()
        {
            Bars = new []
            {
                #region LayeredBars declaration
                // Defense
                new LayeredBar(ref panel3, ref label15, BarMaxWidth),
                new LayeredBar(ref panel4, ref label16, BarMaxWidth),
                new LayeredBar(ref panel5, ref label17, BarMaxWidth),
                new LayeredBar(ref panel6, ref label18, BarMaxWidth),
                new LayeredBar(ref panel7, ref label19, BarMaxWidth),
                new LayeredBar(ref panel8, ref label20, BarMaxWidth),
                new LayeredBar(ref panel9, ref label21, BarMaxWidth),
                new LayeredBar(ref panel10, ref label22, BarMaxWidth),
                new LayeredBar(ref panel11, ref label23, BarMaxWidth),
                new LayeredBar(ref panel12, ref label24, BarMaxWidth),

                // Resistance
                new LayeredBar(ref panel26, ref label33, BarMaxWidth, ref panel13),
                new LayeredBar(ref panel27, ref label34, BarMaxWidth, ref panel19),
                new LayeredBar(ref panel28, ref label35, BarMaxWidth, ref panel20),
                new LayeredBar(ref panel29, ref label36, BarMaxWidth, ref panel21),
                new LayeredBar(ref panel30, ref label37, BarMaxWidth, ref panel22),
                new LayeredBar(ref panel31, ref label38, BarMaxWidth, ref panel23),
                new LayeredBar(ref panel32, ref label39, BarMaxWidth, ref panel24),
                new LayeredBar(ref panel33, ref label40, BarMaxWidth, ref panel25),

                // Regen/HP
                new LayeredBar(ref panel36, ref label43, BarMaxWidth, ref panel34, ref panel14),
                new LayeredBar(ref panel37, ref label44, BarMaxWidth, ref panel35, ref panel15, ref panel38),

                // Endurance
                new LayeredBar(ref panel40, ref label48, BarMaxWidth, ref panel39, ref panel16),
                new LayeredBar(ref panel17, ref label49, BarMaxWidth),
                new LayeredBar(ref panel41, ref label50, BarMaxWidth, 0, ref panel18),

                // Movement
                new LayeredBar(ref panel46, ref label70, BarMaxWidth, ref panel42, ref panel51),
                new LayeredBar(ref panel47, ref label59, BarMaxWidth, ref panel43, ref panel50),
                new LayeredBar(ref panel52, ref label58, BarMaxWidth, ref panel44, ref panel49),
                new LayeredBar(ref panel53, ref label57, BarMaxWidth, ref panel45, ref panel48),

                // Stealth/Perception
                new LayeredBar(ref panel65, ref label66, BarMaxWidth),
                new LayeredBar(ref panel52, ref label54, BarMaxWidth),
                new LayeredBar(ref panel59, ref label53, BarMaxWidth),

                // Haste
                new LayeredBar(ref panel55, ref label74, BarMaxWidth, ref panel54, ref panel74),

                // ToHit
                new LayeredBar(ref panel67, ref label67, BarMaxWidth),
                
                // Accuracy
                new LayeredBar(ref panel70, ref label71, BarMaxWidth),
                
                // Damage
                new LayeredBar(ref panel77, ref label73, BarMaxWidth, 0, ref panel76),
                
                // EndRdx
                new LayeredBar(ref panel80, ref label76, BarMaxWidth),
                
                // Threat Level
                new LayeredBar(ref panel84, ref label78, BarMaxWidth, 0, ref panel83),
                
                // Elusivity
                new LayeredBar(ref panel88, ref label80, BarMaxWidth),

                // Status protection
                new LayeredBar(ref panel97, ref label89, BarMaxWidth),
                new LayeredBar(ref panel94, ref label84, BarMaxWidth),
                new LayeredBar(ref panel91, ref label83, BarMaxWidth),
                new LayeredBar(ref panel88, ref label82, BarMaxWidth),
                new LayeredBar(ref panel89, ref label97, BarMaxWidth),
                new LayeredBar(ref panel90, ref label98, BarMaxWidth),
                new LayeredBar(ref panel92, ref label99, BarMaxWidth),
                new LayeredBar(ref panel93, ref label100, BarMaxWidth),
                new LayeredBar(ref panel95, ref label101, BarMaxWidth),
                new LayeredBar(ref panel96, ref label102, BarMaxWidth),
                new LayeredBar(ref panel98, ref label103, BarMaxWidth),

                // Status resistance
                new LayeredBar(ref panel102, ref label126, BarMaxWidth),
                new LayeredBar(ref panel101, ref label121, BarMaxWidth),
                new LayeredBar(ref panel100, ref label120, BarMaxWidth),
                new LayeredBar(ref panel99, ref label119, BarMaxWidth),
                new LayeredBar(ref panel103, ref label127, BarMaxWidth),
                new LayeredBar(ref panel104, ref label111, BarMaxWidth),
                new LayeredBar(ref panel105, ref label110, BarMaxWidth),
                new LayeredBar(ref panel106, ref label109, BarMaxWidth),
                new LayeredBar(ref panel107, ref label108, BarMaxWidth),
                new LayeredBar(ref panel108, ref label107, BarMaxWidth),
                new LayeredBar(ref panel109, ref label106, BarMaxWidth),

                // Debuff resistance
                new LayeredBar(ref panel113, ref label149, BarMaxWidth),
                new LayeredBar(ref panel112, ref label144, BarMaxWidth),
                new LayeredBar(ref panel111, ref label143, BarMaxWidth),
                new LayeredBar(ref panel110, ref label142, BarMaxWidth),
                new LayeredBar(ref panel114, ref label150, BarMaxWidth),
                new LayeredBar(ref panel115, ref label134, BarMaxWidth),
                new LayeredBar(ref panel116, ref label133, BarMaxWidth)
                #endregion
            };

        }

        private void SetTabPanelColorScheme()
        {
            if (MidsContext.Character.IsHero())
            {
                tabControlAdv2.InactiveTabColor = Color.DodgerBlue;
                tabControlAdv2.TabPanelBackColor = Color.DodgerBlue;
                tabControlAdv2.FixedSingleBorderColor = Color.Goldenrod;
                tabControlAdv2.ActiveTabColor = Color.Goldenrod;
            }
            else
            {
                tabControlAdv2.InactiveTabColor = Color.FromArgb(193, 23, 23);
                tabControlAdv2.TabPanelBackColor = Color.FromArgb(193, 23, 23);
                tabControlAdv2.FixedSingleBorderColor = Color.FromArgb(198, 128, 29);
                tabControlAdv2.ActiveTabColor = Color.FromArgb(198, 128, 29);
            }
        }

        public frmTotalsV2(ref frmMain iParent)
        {
            Load += OnLoad;
            _keepOnTop = true;
            InitializeComponent();
            _myParent = iParent;

            // Tab Foreground Colors don't stick in the designer.
            // Note: default colors will be used anyway.
            // This may only cause issues if a custom
            // Windows theme is in use.
            tabControlAdv2.ActiveTabForeColor = Color.White;
            tabControlAdv2.InActiveTabForeColor = Color.Black;

            InitBars();
            SetTabPanelColorScheme();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();
        }

        private void PbCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PbClosePaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null) return;

            var iStr = "Close";
            var rectangle = new Rectangle();
            ref var local = ref rectangle;
            var size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var width = size.Width;
            size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            extendedBitmap.Graphics.DrawImage(
                MidsContext.Character.IsHero()
                    ? _myParent.Drawing.bxPower[2].Bitmap
                    : _myParent.Drawing.bxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height2 = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height2) / 2, 105, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private void PbTopMostClick(object sender, EventArgs e)
        {
            _keepOnTop = !_keepOnTop;
            TopMost = _keepOnTop;
            pbTopMost.Refresh();
        }

        private void PbTopMostPaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null) return;

            var index = 2;
            if (_keepOnTop) index = 3;
            var iStr = "Keep On top";
            var rectangle = new Rectangle(0, 0, _myParent.Drawing.bxPower[index].Size.Width,
                _myParent.Drawing.bxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            if (index == 3)
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
            else
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height) / 2, 105, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }
    }
}