using System;
using System.Drawing;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
using midsControls;

/*
Regen (cap): 28;111;28 (L=44%)
Regen (base): 51;204;51 (L=80%)
Regen: 64;255;64 (L=100%)
Max HP (cap): 10;38;10 (L=15%)
Max HP (base): 31;130;31 (L=51%, -20)
Max HP: 44;180;44 (L=71%, -29)
Absorb: Gainsboro

EndRec (cap): 13;63;112 (-56)
EndRec (base): 24;114;204 (-20)
EndRec: 30;144;255 (DodgerBlue)

EndUse: 149;203;255

MaxEnd (base): 47;125;204 (-20)
MaxEnd: 59;158;255


Inner Labels: 192;192;255
Resistance Caps: 255;128;128

Movement (cap): 0;48;32 (L=19%)
Movement (base): 0;140;94 (L=55%)
Movement: 0;192;128 (L=75%)

Haste (cap): 112;56;0 (L=44%)
Haste (base): 204;102;0 (L=80%)
Haste: 255;128;0

ToHit: 255;255;128

Damage (base): 204;0;0(L=80%)
Damage: 255;0;0

EndRdx: 65;105;225

Threat (base): 113;86;168
Threat: 147;112;219

Elusivity:
panel88

Threat:
panel84

Threat (base):
panel83

EndRdx:
panel80

Damage (base):
panel76

Damage:
panel77

Accuracy:
panel70

ToHit:
panel67

Haste (cap):
panel74

Haste (base):
panel54

Haste:
panel55

----------------------

Stealth (PvE):
panel65

Stealth (PvP):
panel62

Perception:
panel59

----------------------

Run speed (cap):
panel51

Run speed (base):
panel42

Run speed:
panel46

Jump speed (cap):
panel50

Jump speed (base):
panel43

Jump speed:
panel47

Jump height (cap):
panel49

Jump height:
panel52

Jump height (base):
panel44

Fly (cap):
panel48

Fly (base):
panel45

Fly:
panel53

---------------------------------

EndRec (cap):
panel16

EndRec (base):
panel39

EndRec:
panel40

EndUse:
panel17

MaxEnd (base):
panel18

MaxEnd:
panel41


defense:
panel3 - panel12

Resistance (caps):
panel13, 19-25

Resistance (main):
panel26-33

Regen (cap):
panel14

Regen (base):
panel34

Regen:
panel36

Max HP (cap):
panel15

Max HP (base):
panel35

Max HP:
panel37

Absorb:
panel38

------------------

Values:

Defense:
label15-24

Resistance:
label33-40

Regen:
label43

Max HP:
label44

EndRec:
label48

EndUse:
label49

MaxEnd:
label50

Run:
label70

Jump speed:
label59

Jump height:
label58

Fly:
label57

Stealth (PvE):
label66

Stealth (PvP):
label54

Perception:
label53

Haste:
label74

ToHit:
label67

Accuracy:
label71

Damage:
label73

EndRdx:
label76

Threat:
label78

Elusivity:
label80
*/

namespace Hero_Designer.Forms.WindowMenuItems
{
    public partial class frmTotalTest : Form
    {
        private readonly frmMain _myParent;
        private bool _keepOnTop;

        public frmTotalTest(ref frmMain iParent)
        {
            Load += OnLoad;
            _keepOnTop = true;
            InitializeComponent();
            _myParent = iParent;
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
            if (_myParent?.Drawing == null)
                return;
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
            var height2 = bFont.GetHeight(e.Graphics) + 2f;
            var Bounds = new RectangleF(0.0f, (float)((22 - (double)height2) / 2.0), 105, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
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
            if (_myParent?.Drawing == null)
                return;
            var index = 2;
            if (_keepOnTop)
                index = 3;
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
            var height = bFont.GetHeight(e.Graphics) + 2f;
            var Bounds = new RectangleF(0.0f, (float)((22 - (double)height) / 2.0), 105, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }
    }
}