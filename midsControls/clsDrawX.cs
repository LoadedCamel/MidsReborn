using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
//using Microsoft.VisualBasic;
//using Microsoft.VisualBasic.CompilerServices;

namespace midsControls
{
    public class clsDrawX
    {
        private const string GfxPath = "images\\";

        private const string GfxPowerFn = "pSlot";

        private const string GfxFileExt = ".png";

        private const string NewSlotName = "Newslot.png";

        private const int PaddingX = 7;

        private const int PaddingY = 17;

        public const int OffsetY = 20;

        private const int OffsetInherent = 4;

        private const int vcPowers = 24;

        public static readonly float[][] heroMatrix =
        {
            new[]
            {
                1f, 0f, 0f, 0f, 0f
            },
            new[]
            {
                0f, 1f, 0f, 0f, 0f
            },
            new[]
            {
                0f, 0f, 1f, 0f, 0f
            },
            new[]
            {
                0f, 0f, 0f, 1f, 0f
            },
            new[]
            {
                0f, 0f, 0f, 0f, 1f
            }
        };

        private static readonly float[][] villainMatrix =
        {
            new[]
            {
                0.45f, 0, 0, 0, 0
            },
            new[]
            {
                0, 0.35f, 0, 0, 0
            },
            new[]
            {
                0.75f, 0, 0, 0.175f, 0, 0
            },
            new[]
            {
                0, 0, 0, 1f, 0
            },
            new[]
            {
                0, 0, 0, 0, 1f
            }
        };

        private readonly ExtendedBitmap bxNewSlot;

        public readonly ExtendedBitmap[] bxPower;

        private Color BackColor;

        public ExtendedBitmap bxBuffer;

        private Control cTarget;

        private Font DefaultFont;

        private Graphics gTarget;

        public int Highlight;

        public Enums.eInterfaceMode InterfaceMode;

        private ColorMatrix pColorMatrix;

        public ImageAttributes pImageAttributes;

        //bool VillainColor;

        private float ScaleValue;

        private Size szBuffer;

        public Size szSlot;

        private int vcCols;
        private int vcRowsPowers;

        public clsDrawX(Control iTarget)
        {
            InterfaceMode = 0;
            //VillainColor = false;
            ScaleValue = 2f;
            Scaling = true;
            vcCols = 6;
            vcRowsPowers = 16;
            bxPower = new ExtendedBitmap[5];
            checked
            {
                var num2 = bxPower.Length - 1;
                for (var i = 0; i <= num2; i++)
                {
                    bxPower[i] = new ExtendedBitmap($"{FileIO.AddSlash(Application.StartupPath)}{GfxPath}{GfxPowerFn}{Convert.ToString(i).Trim()}{GfxFileExt}");
                }

                ColorSwitch();
                InitColumns = MidsContext.Config.Columns; 
                SzPower = bxPower[0].Size;

                szSlot = new Size(40, 40);
                szBuffer = GetMaxDrawingArea();
                var size = new Size(szBuffer.Width, szBuffer.Height);
                bxBuffer = new ExtendedBitmap(size);
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                bxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                bxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                szBuffer = GetRequiredDrawingArea();
                bxNewSlot = new ExtendedBitmap(FileIO.AddSlash(Application.StartupPath) + GfxPath + NewSlotName);
                gTarget = iTarget.CreateGraphics();
                cTarget = iTarget;
                gTarget.CompositingMode = CompositingMode.SourceCopy;
                gTarget.CompositingQuality = CompositingQuality.HighQuality;
                gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gTarget.SmoothingMode = SmoothingMode.HighQuality;
                gTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
                DefaultFont = new Font("Arial", 12.5f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
                BackColor = iTarget.BackColor;
                if (szBuffer.Height < cTarget.Height)
                {
                    gTarget.FillRectangle(new SolidBrush(BackColor), 0, szBuffer.Height, cTarget.Width, cTarget.Height - szBuffer.Height);
                }
            }
        }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600001B RID: 27 RVA: 0x000022E0 File Offset: 0x000004E0
        public bool EpicColumns => MidsContext.Character != null && MidsContext.Character.Archetype != null &&
                                   MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic;

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600001C RID: 28 RVA: 0x0000231C File Offset: 0x0000051C
        public bool Scaling { get; private set; }

        public int Columns
        {
            set
            {
                MiniSetCol(value);
                Blank();
                szBuffer = GetRequiredDrawingArea();
                SetScaling(cTarget.Size);
            }
        }

        private int InitColumns
        {
            set
            {
                if (value == vcCols)
                    return;
                if ((value < 2) | (value > 6))
                    return;
                vcCols = value;
                vcRowsPowers = checked((int) Math.Round(36.0 / vcCols));
            }
        }

        public Size SzPower { get; }

        public void ReInit(Control iTarget)
        {
            gTarget = iTarget.CreateGraphics();
            cTarget = iTarget;
            //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
            DefaultFont = new Font("Arial", 12.5f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            BackColor = iTarget.BackColor;
        }

        private void DrawSplit()
        {
            var pen = new Pen(Color.Goldenrod, 2f);
            checked
            {
                int iValue;
                switch (vcCols)
                {
                    case 2:
                        iValue = 4 + vcRowsPowers * (SzPower.Height + 27);
                        bxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(PowerPosition(15).X + SzPower.Width), ScaleDown(iValue));
                        bxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font("Arial", 13f, FontStyle.Bold, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(PowerPosition(15).X + SzPower.Width) / 2 - 35,
                            ScaleDown(iValue));
                        break;
                    case 3:
                        iValue = 4 + vcRowsPowers * (SzPower.Height + 28);
                        bxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(PowerPosition(15).X + SzPower.Width + 275), ScaleDown(iValue));
                        bxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font("Arial", 13f, FontStyle.Bold, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(PowerPosition(15).X + SzPower.Width + 275) / 2 - 50,
                            ScaleDown(iValue));
                        break;
                    case 4:
                        iValue = 4 + vcRowsPowers * (SzPower.Height + 30);
                        bxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(PowerPosition(15).X + SzPower.Width + 300), ScaleDown(iValue));
                        bxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font("Arial", 13f, FontStyle.Bold, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(PowerPosition(15).X + SzPower.Width + 300) / 2 - 75,
                            ScaleDown(iValue));
                        break;
                    case 5:
                        iValue = 4 + vcRowsPowers * (SzPower.Height + 30);
                        bxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(PowerPosition(15).X + SzPower.Width + 300), ScaleDown(iValue));
                        bxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font("Arial", 13f, FontStyle.Bold, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(PowerPosition(15).X + SzPower.Width + 300) / 2 - 75,
                            ScaleDown(iValue));
                        break;
                    case 6:
                        iValue = 4 + vcRowsPowers * (SzPower.Height + 30);
                        bxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(PowerPosition(15).X + SzPower.Width + 550), ScaleDown(iValue));
                        bxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font("Arial", 13f, FontStyle.Bold, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(PowerPosition(15).X + SzPower.Width + 300) / 2 - 75,
                            ScaleDown(iValue));
                        break;
                }
            }
        }

        private void DrawPowers()
        {
            checked
            {
                for (var i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                    if (MidsContext.Character.CanPlaceSlot & (Highlight == i))
                    {
                        var currentBuild = MidsContext.Character.CurrentBuild;
                        var powers = currentBuild.Powers;
                        var index = i;
                        var value = powers[index];
                        DrawPowerSlot(ref value, true);
                        currentBuild.Powers[index] = value;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                    {
                        var currentBuild = MidsContext.Character.CurrentBuild;
                        var powers2 = currentBuild.Powers;
                        var index = i;
                        var value = powers2[index];
                        DrawPowerSlot(ref value);
                        currentBuild.Powers[index] = value;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[i].Power != null && (string.CompareOrdinal(MidsContext.Character.CurrentBuild.Powers[i].Power.GroupName, "Incarnate") == 0) | MidsContext.Character.CurrentBuild.Powers[i].Power.IncludeFlag)
                    {
                        var currentBuild = MidsContext.Character.CurrentBuild;
                        var powers3 = currentBuild.Powers;
                        var index = i;
                        var value = powers3[index];
                        DrawPowerSlot(ref value);
                        currentBuild.Powers[index] = value;
                    }

                Application.DoEvents();
                DrawSplit();
            }
        }

        private float FontScale(float iSZ)
        {
            return Math.Min(ScaleDown(iSZ) * 1.1f, iSZ);
        }

        private static int IndexFromLevel()
        {
            return MidsContext.Character.CurrentBuild.Powers.FindIndex(pow =>
                pow?.Level == MidsContext.Character.RequestedLevel);
        }

        private void DrawEnhancementLevel(SlotEntry slot, Font font, Graphics g, ref RectangleF rect)
        {
            unchecked
            {
                var enhType = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID;
                var catalystSet = false;
                if (enhType == Enums.eType.SetO || enhType == Enums.eType.InventO)
                {
                    var iValue2 = rect;
                    iValue2.Y -= 5f;
                    iValue2.X -= 4f;
                    iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                    string relativeLevelNumeric;
                    var enhInternalName = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].UID;
                    catalystSet = DatabaseAPI.EnhHasCatalyst(enhInternalName) ||
                                  DatabaseAPI.EnhIsNaturallyAttuned(slot.Enhancement.Enh);
                    // Catalysed enhancements take character level no matter what.
                    // Game does not allow boosters over enhancement catalysts.
                    // [Zed] Note: I am not sure yet if catalysed enhancements are considered having the level of the user.
                    // Meaning a max level 30 IO --MAY-- have the stats of a level 50 one, even if it doesn't exist.
                    if (catalystSet || slot.Enhancement.RelativeLevel <= Enums.eEnhRelative.Even)
                        relativeLevelNumeric = string.Empty;
                    else
                        relativeLevelNumeric = Enums.GetRelativeString(slot.Enhancement.RelativeLevel, false);

                    // If enhancement has boosters, need to stretch the level drawing zone a little,
                    // or relative level doesn't fit in.
                    if (!string.IsNullOrEmpty(relativeLevelNumeric))
                    {
                        iValue2.Width += 3f;
                        iValue2.X -= 3f;
                    }

                    var iStr = (MidsContext.Config.I9.HideIOLevels
                        ? string.Empty
                        : Convert.ToString(checked(slot.Enhancement.IOLevel + 1))) + relativeLevelNumeric;
                    if (!catalystSet)
                    {
                        var bounds = ScaleDown(iValue2);
                        var cyan = Color.Cyan;
                        var outline = Color.FromArgb(128, 0, 0, 0);
                        var outlineSpace = 1f;
                        g = bxBuffer.Graphics;

                        DrawOutlineText(iStr, bounds, cyan, outline, font, outlineSpace, g);
                    }
                }
                else if (enhType == Enums.eType.Normal || enhType == Enums.eType.SpecialO)
                {
                    var iValue2 = rect;
                    iValue2.Y -= 3f;
                    iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                    Color color;

                    if (slot.Enhancement.RelativeLevel == 0)
                        color = Color.FromArgb(172, 67, 50);
                    else if (slot.Enhancement.RelativeLevel < Enums.eEnhRelative.Even)
                        color = Color.Yellow;
                    else if (slot.Enhancement.RelativeLevel > Enums.eEnhRelative.Even)
                        //color = Color.FromArgb(0, 255, 255);
                        color = Color.FromArgb(81, 120, 169);
                    else
                        color = Color.White;

                    var relativeString = string.Empty;
                    // Always display relative level if present
                    //if (MidsContext.Config.ShowEnhRel)
                    relativeString = Enums.GetRelativeString(slot.Enhancement.RelativeLevel,
                        MidsContext.Config.ShowRelSymbols);

                    if (MidsContext.Config.ShowSOLevels)
                        // It isn't slot.Enhancement.IOLevel... always set to 0
                        // Improvisation it is...
                        // I dunno. MidsContext.Character.Level == 48 ?
                        relativeString = Convert.ToString(Math.Max(50, MidsContext.Character.Level + 2), null) +
                                         relativeString;

                    if (
                        //MidsContext.Config.ShowEnhRel &&
                        MidsContext.Config.ShowSOLevels &&
                        slot.Enhancement.RelativeLevel != Enums.eEnhRelative.None &&
                        slot.Enhancement.RelativeLevel != Enums.eEnhRelative.Even
                    )
                    {
                        iValue2.Width += 5f;
                        iValue2.X -= 5f;
                    }

                    if (!string.IsNullOrEmpty(relativeString))
                    {
                        var bounds2 = ScaleDown(iValue2);
                        var outline2 = Color.FromArgb(128, 0, 0, 0);
                        var outlineSpace2 = 1f;

                        DrawOutlineText(relativeString, bounds2, color, outline2, font, outlineSpace2, g);
                    }
                }
            }
        }

        public Point DrawPowerSlot(ref PowerEntry iSlot, bool singleDraw = false)
        {
            var pen = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            var text = string.Empty;
            var text2 = string.Empty;
            var rectangleF = new RectangleF(0f, 0f, 0f, 0f);
            var stringFormat = new StringFormat(StringFormatFlags.NoClip)
            {
                Trimming = StringTrimming.None
            };
            var pen2 = new Pen(Color.Black);
            Rectangle rectangle = default;
            //Font font = new Font(DefaultFont.FontFamily, FontScale(DefaultFont.SizeInPoints), DefaultFont.Style, GraphicsUnit.Point);
            var font = new Font("Arial", FontScale(DefaultFont.Size), FontStyle.Bold, GraphicsUnit.Pixel, 0);
            var slotChk = MidsContext.Character.SlotCheck(iSlot);
            var ePowerState = iSlot.State;
            var canPlaceSlot = MidsContext.Character.CanPlaceSlot;
            var drawNewSlot = iSlot.Power != null && iSlot.State != Enums.ePowerState.Empty && canPlaceSlot &&
                              iSlot.Slots.Length < 6 &&
                              singleDraw && iSlot.Power.Slottable & (InterfaceMode != Enums.eInterfaceMode.PowerToggle);
            var result = PowerPosition(iSlot);
            Point point = default;
            checked
            {
                //position of enh rectangle
                point.X = (int) Math.Round(result.X + checked(SzPower.Width - szSlot.Width * 6) / 2.0);
                point.Y = result.Y + 22;
                var graphics = bxBuffer.Graphics;
                Brush brush = new SolidBrush(BackColor);
                var clipRect = new Rectangle(point.X, point.Y, SzPower.Width, SzPower.Height);
                graphics.FillRectangle(brush, clipRect);
                var toggling = InterfaceMode == Enums.eInterfaceMode.PowerToggle;
                if (!toggling)
                {
                    if (iSlot.Power != null)
                    {
                        if (singleDraw
                            && slotChk > -1
                            && canPlaceSlot
                            && InterfaceMode != Enums.eInterfaceMode.PowerToggle
                            && iSlot.PowerSet != null
                            && iSlot.Slots.Length < 6
                            && iSlot.Power.Slottable)
                            ePowerState = Enums.ePowerState.Open;
                        else if (iSlot.Chosen & !canPlaceSlot & (InterfaceMode != Enums.eInterfaceMode.PowerToggle) &
                                 (Highlight == MidsContext.Character.CurrentBuild.Powers.IndexOf(iSlot)))
                            ePowerState = Enums.ePowerState.Open;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers.IndexOf(iSlot) == IndexFromLevel())
                    {
                        ePowerState = Enums.ePowerState.Open;
                    }
                }

                rectangleF.Height = szSlot.Height;
                rectangleF.Width = szSlot.Width;
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;
                stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip;
                bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                bool grey;
                ImageAttributes imageAttr;
                if (toggling)
                {
                    if (ePowerState == Enums.ePowerState.Open) ePowerState = Enums.ePowerState.Empty;

                    if (iSlot.StatInclude & (ePowerState == Enums.ePowerState.Used))
                    {
                        ePowerState = Enums.ePowerState.Open;
                        grey = iSlot.Level >= MidsContext.Config.ForceLevel;
                        imageAttr = GreySlot(grey, true);
                    }
                    else if (iSlot.CanIncludeForStats())
                    {
                        grey = iSlot.Level >= MidsContext.Config.ForceLevel;
                        imageAttr = GreySlot(grey);
                    }
                    else
                    {
                        imageAttr = GreySlot(true);
                        grey = true;
                    }
                }
                else
                {
                    grey = iSlot.Level >= MidsContext.Config.ForceLevel;
                    imageAttr = GreySlot(grey);
                }
                //appears to be power size
                var iValue = new Rectangle(result.X, result.Y, bxPower[(int) ePowerState].Size.Width,
                    bxPower[(int) ePowerState].Size.Height);
                if (ePowerState == Enums.ePowerState.Used || toggling)
                {
                    if (!MidsContext.Config.DisableDesaturateInherent & !iSlot.Chosen)
                        imageAttr = Desaturate(grey, ePowerState == Enums.ePowerState.Open);

                    var graphics2 = bxBuffer.Graphics;
                    Image bitmap = !MidsContext.Character.IsHero()
                        ? bxPower[4].Bitmap
                        : bxPower[(int) ePowerState].Bitmap;
                    var destRect = ScaleDown(iValue);
                    var srcX = 0;
                    var srcY = 0;
                    var width = bxPower[(int) ePowerState].ClipRect.Width;
                    var clipRect2 = bxPower[(int) ePowerState].ClipRect;
                    graphics2.DrawImage(bitmap, destRect, srcX, srcY, width, clipRect2.Height, GraphicsUnit.Pixel,
                        imageAttr);
                }
                else
                {
                    var graphics3 = bxBuffer.Graphics;
                    Image bitmap2 = bxPower[(int) ePowerState].Bitmap;
                    var destRect2 = ScaleDown(iValue);
                    var srcX2 = 0;
                    var srcY2 = 0;
                    var width2 = bxPower[(int) ePowerState].ClipRect.Width;
                    clipRect = bxPower[(int) ePowerState].ClipRect;
                    graphics3.DrawImage(bitmap2, destRect2, srcX2, srcY2, width2, clipRect.Height, GraphicsUnit.Pixel);
                }
                //Toggle Button on powers
                if (iSlot.CanIncludeForStats())
                {
                    rectangle.Height = 15;
                    rectangle.Width = rectangle.Height;
                    rectangle.Y = (int) Math.Round(iValue.Top + checked(iValue.Height - rectangle.Height) / 2.0);
                    rectangle.X = (int) Math.Round(iValue.Right -
                                                   (rectangle.Width + checked(iValue.Height - rectangle.Height) / 2.0));
                    rectangle = ScaleDown(rectangle);
                    PathGradientBrush brush2;
                    if (iSlot.StatInclude)
                    {
                        var iRect = rectangle;
                        var iCenter = new PointF(-0.25f, -0.33f);
                        brush2 = MakePathBrush(iRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0));
                    }
                    else
                    {
                        var iRect2 = rectangle;
                        var iCenter = new PointF(-0.25f, -0.33f);
                        brush2 = MakePathBrush(iRect2, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                    }

                    bxBuffer.Graphics.FillEllipse(brush2, rectangle);
                    bxBuffer.Graphics.DrawEllipse(pen2, rectangle);
                }

                SolidBrush solidBrush;
                //if (!System.Diagnostics.Debugger.IsAttached || !this.IsInDesignMode() || !System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
                var inDesigner = Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv");
                for (var i = 0; i <= iSlot.Slots.Length - 1; i++)
                {
                    var slot = iSlot.Slots[i];
                    //Controls the spacing between enh
                    rectangleF.X = point.X + szSlot.Width * i / (float) 1.2;
                    rectangleF.Y = point.Y;
                    if (slot.Enhancement.Enh < 0)
                    {
                        var clipRect2 = new Rectangle((int) Math.Round(rectangleF.X), point.Y, 30, 30);
                        bxBuffer.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, ScaleDown(clipRect2), 0, 0, 30, 30,
                            GraphicsUnit.Pixel,
                            pImageAttributes);
                        if ((MidsContext.Config.CalcEnhLevel == 0) | (slot.Level >= MidsContext.Config.ForceLevel) |
                            ((InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !iSlot.StatInclude) |
                            (!iSlot.AllowFrontLoading & (slot.Level < iSlot.Level)))
                        {
                            solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                            bxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(rectangleF));
                            bxBuffer.Graphics.DrawEllipse(pen, ScaleDown(rectangleF));
                        }
                    }
                    else
                    {
                        if (inDesigner) continue;
                        var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                        var graphics5 = bxBuffer.Graphics;
                        var clipRect2 = new Rectangle((int) Math.Round(rectangleF.X), point.Y, 40, 40);
                        I9Gfx.DrawEnhancementAt(ref graphics5, ScaleDown(clipRect2), enhancement.ImageIdx,
                            I9Gfx.ToGfxGrade(enhancement.TypeID, slot.Enhancement.Grade));
                        if ((slot.Enhancement.RelativeLevel == 0) | (slot.Level >= MidsContext.Config.ForceLevel) |
                            ((InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !iSlot.StatInclude) |
                            (!iSlot.AllowFrontLoading & (slot.Level < iSlot.Level)))
                        {
                            solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                            var iValue2 = rectangleF;
                            iValue2.Inflate(1f, 1f);
                            bxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(iValue2));
                        }

                        if (slot.Enhancement.Enh > -1)
                            DrawEnhancementLevel(slot, font, graphics, ref rectangleF);
                    }

                    if (!MidsContext.Config.ShowSlotLevels)
                        continue;
                    {
                        var iValue2 = rectangleF;
                        unchecked
                        {
                            //Controls placement of slot levels
                            iValue2.Y += iValue2.Height;
                            iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                            iValue2.Y -= iValue2.Height - 1;
                            iValue2.X += iValue2.Width - 5;
                            iValue2.X -= iValue2.Width - 1;
                        }

                        var graphics5 = bxBuffer.Graphics;
                        DrawOutlineText(
                            Convert.ToString(slot.Level + 1),
                            ScaleDown(iValue2),
                            Color.FromArgb(0, 255, 0),
                            Color.FromArgb(192, 0, 0, 0),
                            font,
                            1f,
                            graphics5);
                    }
                }

                if (slotChk > -1 && ePowerState != Enums.ePowerState.Empty && drawNewSlot)
                {
                    //var clipRect2 = new Rectangle(point.X + szSlot.Width * iSlot.Slots.Length, point.Y, 30, 30);
                    var clipRect2 = new Rectangle(point.X + (szSlot.Width - 5) * iSlot.Slots.Length, point.Y, 30, 30);
                    RectangleF iValue2 = clipRect2;
                    bxBuffer.Graphics.DrawImage(bxNewSlot.Bitmap, ScaleDown(iValue2));
                    iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                    iValue2.Y += (szSlot.Height - iValue2.Height) / 2f;
                    DrawOutlineText(Convert.ToString(slotChk + 1), ScaleDown(iValue2),
                        Color.FromArgb(0, 255, 255), Color.FromArgb(192, 0, 0, 0), font, 1f, bxBuffer.Graphics);
                }

                solidBrush = new SolidBrush(Color.Black);
                stringFormat = new StringFormat();
                //Text Positioning of Power Names
                rectangleF.X = result.X + 10;
                rectangleF.Y = result.Y + 4;
                rectangleF.Width = SzPower.Width;
                rectangleF.Height = DefaultFont.GetHeight() * 2f;
                var ePowerState2 = iSlot.State;
                if ((ePowerState2 == Enums.ePowerState.Empty) & (ePowerState == Enums.ePowerState.Open))
                    ePowerState2 = ePowerState;

                switch (ePowerState2)
                {
                    case 0:
                        solidBrush = new SolidBrush(Color.Transparent);
                        text = "";
                        break;
                    case Enums.ePowerState.Empty:
                        solidBrush = new SolidBrush(Color.WhiteSmoke);
                        text = "(" + Convert.ToString(iSlot.Level + 1) + ")";
                        break;
                    case Enums.ePowerState.Used:
                        switch (iSlot.PowerSet.SetType)
                        {
                            case Enums.ePowerSetType.Primary:
                                text2 = "";
                                break;
                            case Enums.ePowerSetType.Secondary:
                                text2 = "";
                                break;
                            case Enums.ePowerSetType.Ancillary:
                                text2 = "";
                                break;
                            case Enums.ePowerSetType.Inherent:
                                text2 = "";
                                break;
                            case Enums.ePowerSetType.Pool:
                                text2 = "";
                                break;
                        }

                        solidBrush = new SolidBrush(Color.Black);
                        text = iSlot.Virtual
                            ? iSlot.Name
                            : string.Concat("(", Convert.ToString(iSlot.Level + 1), ") ", iSlot.Name, " ", text2);
                        break;
                    case Enums.ePowerState.Open:
                        solidBrush = new SolidBrush(Color.WhiteSmoke);
                        text = "(" + Convert.ToString(iSlot.Level + 1) + ")";
                        break;
                }

                if ((ePowerState == Enums.ePowerState.Empty) & (iSlot.State == Enums.ePowerState.Used))
                    solidBrush = new SolidBrush(Color.WhiteSmoke);

                if (InterfaceMode == Enums.eInterfaceMode.PowerToggle && solidBrush.Color == Color.Black &&
                    !iSlot.CanIncludeForStats()) solidBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));

                stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                if (MidsContext.Config.EnhanceVisibility)
                {
                    var iStr4 = text;
                    var bounds5 = ScaleDown(rectangleF);
                    var whiteSmoke = Color.WhiteSmoke;
                    var outline5 = Color.FromArgb(192, 0, 0, 0);
                    var bFont5 = font;
                    var outlineSpace5 = 1f;
                    var graphics5 = bxBuffer.Graphics;
                    graphics5.CompositingQuality = CompositingQuality.HighQuality;
                    graphics5.SmoothingMode = SmoothingMode.HighQuality;
                    graphics5.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    graphics5.PageUnit = GraphicsUnit.Pixel;
                    graphics5.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    DrawOutlineText(iStr4, bounds5, whiteSmoke, outline5, bFont5, outlineSpace5, graphics5, false,
                        true);
                }
                else
                {
                    bxBuffer.Graphics.DrawString(text, font, solidBrush, ScaleDown(rectangleF));
                }

                return result;
            }
        }

        private PathGradientBrush MakePathBrush(Rectangle iRect, PointF iCenter, Color iColor1, Color icolor2)
        {
            var num = (float) (iRect.Left + iRect.Width * 0.5);
            var num2 = (float) (iRect.Top + iRect.Height * 0.5);
            var graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(iRect);
            PathGradientBrush pathGradientBrush;
            PathGradientBrush pathGradientBrush2;
            checked
            {
                var array = new Color[graphicsPath.PathPoints.GetUpperBound(0) + 1];
                var lowerBound = graphicsPath.PathPoints.GetLowerBound(0);
                var upperBound = graphicsPath.PathPoints.GetUpperBound(0);
                for (var i = lowerBound; i <= upperBound; i++) array[i] = icolor2;

                pathGradientBrush = new PathGradientBrush(graphicsPath)
                {
                    CenterColor = iColor1,
                    SurroundColors = array
                };
                pathGradientBrush2 = pathGradientBrush;
            }

            var centerPoint = new PointF((float) (num + (iCenter.X + iCenter.X * (iRect.Width * 0.5))),
                (float) (num2 + (iCenter.Y + iCenter.Y * (iRect.Height * 0.5))));
            pathGradientBrush2.CenterPoint = centerPoint;
            return pathGradientBrush;
        }

        public void FullRedraw()
        {
            ColorSwitch();
            BackColor = cTarget.BackColor;
            bxBuffer.Graphics.Clear(BackColor);
            DrawPowers();
            var location = new Point(0, 0);
            OutputUnscaled(ref bxBuffer, location);
            GC.Collect();
        }

        public void Refresh(Rectangle Clip)
        {
            OutputRefresh(Clip, Clip, GraphicsUnit.Pixel);
        }

        private int GetVisualIDX(int PowerIndex)
        {
            var nidpowerset = MidsContext.Character.CurrentBuild.Powers[PowerIndex].NIDPowerset;
            var idxpower = MidsContext.Character.CurrentBuild.Powers[PowerIndex].IDXPower;
            checked
            {
                if (nidpowerset > -1)
                {
                    int vIdx;
                    if (DatabaseAPI.Database.Powersets[nidpowerset].SetType == Enums.ePowerSetType.Inherent)
                    {
                        vIdx = DatabaseAPI.Database.Powersets[nidpowerset].Powers[idxpower].LocationIndex;
                    }
                    else
                    {
                        vIdx = -1;
                        for (var i = 0; i <= PowerIndex; i++)
                            if (MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset > -1)
                            {
                                if (DatabaseAPI.Database
                                        .Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType !=
                                    Enums.ePowerSetType.Inherent)
                                    vIdx++;
                            }
                            else
                            {
                                vIdx++;
                            }
                    }

                    return vIdx;
                }
                else
                {
                    var vIdx = -1;
                    for (var i = 0; i <= PowerIndex; i++)
                        if (MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset > -1)
                        {
                            if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset]
                                    .SetType !=
                                Enums.ePowerSetType.Inherent)
                                vIdx++;
                        }
                        else
                        {
                            vIdx++;
                        }

                    return vIdx;
                }
            }
        }

        public static void DrawOutlineText(string iStr, RectangleF bounds, Color textColor, Color outlineColor,
            Font bFont, float outlineSpace,
            Graphics g, bool smallMode = false, bool leftAlign = false)
        {
            var stringFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Near,
                Alignment = leftAlign ? StringAlignment.Near : StringAlignment.Center
            };

            var brush = new SolidBrush(outlineColor);
            var layoutRectangle = bounds;
            var layoutRectangle2 = new RectangleF(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width,
                bFont.GetHeight(g));
            layoutRectangle2.X -= outlineSpace;
            if (!smallMode)
                g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y -= outlineSpace;
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X += outlineSpace;
            if (!smallMode)
                g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X += outlineSpace;
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y += outlineSpace;
            if (!smallMode)
                g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y += outlineSpace;
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X -= outlineSpace;
            if (!smallMode)
                g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.X -= outlineSpace;
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            layoutRectangle2.Y -= outlineSpace;
            if (!smallMode)
                g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
            g.DrawString(iStr, bFont, new SolidBrush(textColor), layoutRectangle, stringFormat);
        }

        public int WhichSlot(int iX, int iY)
        {
            checked
            {
                for (var i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    Point point;
                    if (MidsContext.Character.CurrentBuild.Powers[i].Power == null ||
                        MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                        point = PowerPosition(GetVisualIDX(i));
                    else
                        point = PowerPosition(i);
                    if (iX >= point.X && iY >= point.Y && iX < SzPower.Width + point.X &&
                        iY < point.Y + SzPower.Height + 17)
                        return i;
                }

                return -1;
            }
        }

        public int WhichEnh(int iX, int iY)
        {
            var oPower = -1;
            checked
            {
                Point point = default;
                for (var i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i].Power == null ||
                        MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                        point = PowerPosition(GetVisualIDX(i));
                    else
                        point = PowerPosition(i);

                    if (iX < point.X || iY < point.Y || iX >= SzPower.Width + point.X ||
                        iY >= point.Y + SzPower.Height + 17)
                        continue;
                    oPower = i;
                    break;
                }

                if (oPower <= -1)
                    return -1;
                {
                    var isValid = false;
                    if (iY >= point.Y + 18)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[oPower].NIDPowerset > -1)
                        {
                            if (DatabaseAPI.Database
                                .Powersets[MidsContext.Character.CurrentBuild.Powers[oPower].NIDPowerset]
                                .Powers[MidsContext.Character.CurrentBuild.Powers[oPower].IDXPower].Slottable)
                                isValid = true;
                        }
                        else
                        {
                            isValid = true;
                        }
                    }

                    if (!isValid)
                        return -1;
                    iX = (int) Math.Round(iX - (point.X + checked(SzPower.Width - szSlot.Width * 6) / 2.0));
                    for (var i = 0; i <= MidsContext.Character.CurrentBuild.Powers[oPower].Slots.Length - 1; i++)
                        if (iX <= (i + 1) * szSlot.Width)
                            return i;

                    return -1;
                }
            }
        }

        public bool HighlightSlot(int idx, bool Force = false)
        {
            checked
            {
                if (MidsContext.Character.CurrentBuild.Powers.Count >= 1)
                {
                    if (Highlight == idx && !Force) return false;
                    if (idx != -1)
                    {
                        Build currentBuild;
                        PowerEntry value;
                        Point point2;
                        Rectangle iValue;
                        Rectangle rectangle;
                        if (Highlight != -1 && Highlight < MidsContext.Character.CurrentBuild.Powers.Count)
                        {
                            currentBuild = MidsContext.Character.CurrentBuild;
                            var powers = currentBuild.Powers;
                            var highlight = Highlight;
                            value = powers[highlight];
                            var point = DrawPowerSlot(ref value);
                            currentBuild.Powers[highlight] = value;
                            point2 = point;
                            iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + 17);
                            rectangle = ScaleDown(iValue);
                            DrawSplit();
                            Output(ref bxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
                        }

                        Highlight = idx;
                        currentBuild = MidsContext.Character.CurrentBuild;
                        value = currentBuild.Powers[idx];
                        var point3 = DrawPowerSlot(ref value, true);
                        currentBuild.Powers[idx] = value;
                        point2 = point3;
                        iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + 17);
                        rectangle = ScaleDown(iValue);
                        DrawSplit();
                        Output(ref bxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
                    }
                    else if (Highlight != -1)
                    {
                        var currentBuild = MidsContext.Character.CurrentBuild;
                        var powers2 = currentBuild.Powers;
                        var highlight = Highlight;
                        var value = powers2[highlight];
                        var point4 = DrawPowerSlot(ref value);
                        currentBuild.Powers[highlight] = value;
                        var point2 = point4;
                        var iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + 17);
                        var rectangle = ScaleDown(iValue);
                        DrawSplit();
                        Output(ref bxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
                        Highlight = idx;
                    }
                }
                else
                {
                    return false;
                }

                return false;
            }
        }

        private void Blank()
        {
            bxBuffer?.Graphics.Clear(BackColor);
        }

        public void SetScaling(Size iSize)
        {
            var scaleEnabled = Scaling;
            var scaleValue = ScaleValue;
            if ((iSize.Width < 10) | (iSize.Height < 10))
                return;
            var drawingArea = GetDrawingArea();
            if ((drawingArea.Width > iSize.Width) | (drawingArea.Height > iSize.Height))
            {
                Scaling = true;
                ScaleValue = (double) drawingArea.Width / iSize.Width > drawingArea.Height / (double) iSize.Height
                    ? (float) (drawingArea.Width / (double) iSize.Width)
                    : (float) (drawingArea.Height / (double) iSize.Height);
                ResetTarget();
                bxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                bxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                bxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                if (!(Math.Abs(ScaleValue - scaleValue) > float.Epsilon))
                    return;
                FullRedraw();
                bxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                bxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                bxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            }
            else
            {
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                ScaleValue = 1f;
                ResetTarget();
                Scaling = false;
                if ((scaleEnabled != Scaling) | (Math.Abs(scaleValue - ScaleValue) > float.Epsilon)) FullRedraw();
            }
        }

        private void ResetTarget()
        {
            bxBuffer.Graphics.TextRenderingHint =
                ScaleValue > 1.125 ? TextRenderingHint.ClearTypeGridFit : TextRenderingHint.ClearTypeGridFit;
            gTarget.Dispose();
            gTarget = cTarget.CreateGraphics();
            gTarget.CompositingQuality = CompositingQuality.HighQuality;
            gTarget.CompositingMode = CompositingMode.SourceCopy;
            gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gTarget.SmoothingMode = SmoothingMode.HighQuality;
        }

        public int ScaleDown(int iValue)
        {
            int result;
            if (!Scaling)
            {
                result = iValue;
            }
            else
            {
                iValue = checked((int) Math.Round(iValue / ScaleValue));
                result = iValue;
            }

            return result;
        }

        public int ScaleUp(int iValue)
        {
            int result;
            if (!Scaling)
            {
                result = iValue;
            }
            else
            {
                iValue = checked((int) Math.Round(iValue * ScaleValue));
                result = iValue;
            }

            return result;
        }

        private float ScaleDown(float iValue)
        {
            float result;
            if (!Scaling)
            {
                result = iValue;
            }
            else
            {
                iValue /= ScaleValue;
                result = iValue;
            }

            return result;
        }

        public Rectangle ScaleDown(Rectangle iValue)
        {
            checked
            {
                Rectangle result;
                if (!Scaling)
                {
                    result = iValue;
                }
                else
                {
                    iValue.X = (int) Math.Round(iValue.X / ScaleValue);
                    iValue.Y = (int) Math.Round(iValue.Y / ScaleValue);
                    iValue.Width = (int) Math.Round(iValue.Width / ScaleValue);
                    iValue.Height = (int) Math.Round(iValue.Height / ScaleValue);
                    result = iValue;
                }

                return result;
            }
        }

        private RectangleF ScaleDown(RectangleF iValue)
        {
            checked
            {
                if (!Scaling)
                    return iValue;
                iValue.X = (int) Math.Round(iValue.X / ScaleValue);
                iValue.Y = (int) Math.Round(iValue.Y / ScaleValue);
                iValue.Width = (int) Math.Round(iValue.Width / ScaleValue);
                iValue.Height = (int) Math.Round(iValue.Height / ScaleValue);
                return iValue;
            }
        }

        private void Output(ref ExtendedBitmap Buffer, Rectangle DestRect, Rectangle SrcRect, GraphicsUnit iUnit)
        {
            gTarget.DrawImage(Buffer.Bitmap, DestRect, SrcRect, iUnit);
        }

        private void OutputRefresh(Rectangle DestRect, Rectangle SrcRect, GraphicsUnit iUnit)
        {
            gTarget.DrawImage(bxBuffer.Bitmap, DestRect, SrcRect, iUnit);
        }

        private void OutputUnscaled(ref ExtendedBitmap Buffer, Point Location)
        {
            gTarget.DrawImageUnscaled(Buffer.Bitmap, Location);
        }

        public void ColorSwitch()
        {
            /*bool useHeroColors = true;
            if (MidsContext.Character != null)
                useHeroColors = MidsContext.Character.IsHero();
            if (MidsContext.Config.DisableVillainColors)
                useHeroColors = true;
            VillainColor = !useHeroColors;*/
            pColorMatrix = new ColorMatrix(heroMatrix);
            if (pImageAttributes == null)
                pImageAttributes = new ImageAttributes();
            pImageAttributes.SetColorMatrix(pColorMatrix);
        }

        public static ImageAttributes GetRecolorIa(bool hero)
        {
            var colorMatrix = new ColorMatrix(heroMatrix);
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            return imageAttributes;
        }

        private ImageAttributes GreySlot(bool grey, bool bypassIa = false)
        {
            if (!grey) return bypassIa ? new ImageAttributes() : pImageAttributes;

            checked
            {
                var colorMatrix = new ColorMatrix(heroMatrix);
                var r = 0;
                do
                {
                    var c = 0;
                    do
                    {
                        if (!bypassIa) colorMatrix[r, c] = pColorMatrix[r, c];

                        if (r != 4) colorMatrix[r, c] = (float) (colorMatrix[r, c] / 1.5);

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                return imageAttributes;
            }
        }

        private ImageAttributes Desaturate(bool Grey, bool BypassIA = false)
        {
            var tMM = new ColorMatrix(new[]
            {
                new[]
                {
                    0.299f, 0.299f, 0.299f, 0f, 0f
                },
                new[]
                {
                    0.587f, 0.587f, 0.587f, 0f, 0f
                },
                new[]
                {
                    0.114f, 0.114f, 0.114f, 0f, 0f
                },
                new[]
                {
                    0, 0, 0, 1f, 0
                },
                new[]
                {
                    0, 0, 0, 0, 1f
                }
            });
            var tCM = new ColorMatrix(heroMatrix);
            var r = 0;
            checked
            {
                do
                {
                    var c = 0;
                    do
                    {
                        if (!BypassIA) tCM[r, c] = (pColorMatrix[r, c] + tMM[r, c]) / 2f;

                        if (Grey && r != 4) tCM[r, c] = (float) (tCM[r, c] / 1.5);

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(tCM);
                return imageAttributes;
            }
        }

        public Rectangle PowerBoundsUnScaled(int hIdx)
        {
            var rectangle = new Rectangle(0, 0, 1, 1);
            checked
            {
                Rectangle result;
                if ((hIdx < 0) | (hIdx > MidsContext.Character.CurrentBuild.Powers.Count - 1))
                {
                    result = rectangle;
                }
                else
                {
                    if (!MidsContext.Character.CurrentBuild.Powers[hIdx].Chosen &&
                        MidsContext.Character.CurrentBuild.Powers[hIdx].Power != null)
                        rectangle.Location = PowerPosition(hIdx);
                    else
                        rectangle.Location = PowerPosition(GetVisualIDX(hIdx));

                    rectangle.Width = SzPower.Width;
                    var num = rectangle.Y + 18;
                    num += szSlot.Height;
                    rectangle.Height = num - rectangle.Y;
                    result = rectangle;
                }

                return result;
            }
        }

        public bool WithinPowerBar(Rectangle pBounds, Point e)
        {
            pBounds.Height = SzPower.Height + 5;
            return e.X >= pBounds.Left && e.X < pBounds.Right && e.Y >= pBounds.Top && e.Y < pBounds.Bottom;
        }

        private Point PowerPosition(int powerEntryIdx)
        {
            return PowerPosition(MidsContext.Character.CurrentBuild.Powers[powerEntryIdx]);
        }

        private int[][] GetInherentGrid()
        {
            switch (vcCols)
            {
                case 2:
                    if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                        return new[]
                        {
                            new[]
                            {
                                0, 1
                            },
                            new[]
                            {
                                2, 3
                            },
                            new[]
                            {
                                4, 5
                            },
                            new[]
                            {
                                6, 7
                            },
                            new[]
                            {
                                8, 9
                            },
                            new[]
                            {
                                10, 11
                            },
                            new[]
                            {
                                12, 13
                            },
                            new[]
                            {
                                14, 15
                            },
                            new[]
                            {
                                16, 17
                            },
                            new[]
                            {
                                18, 19
                            },
                            new[]
                            {
                                20, 21
                            },
                            new[]
                            {
                                22, 23
                            },
                            new[]
                            {
                                24, 25
                            },
                            new[]
                            {
                                26, 27
                            },
                            new[]
                            {
                                28, 29
                            },
                            new[]
                            {
                                30, 31
                            },
                            new[]
                            {
                                32, 33
                            },
                            new[]
                            {
                                34, 35
                            },
                            new[]
                            {
                                36, 37
                            },
                            new[]
                            {
                                38, 39
                            },
                            new[]
                            {
                                40, 41
                            },
                            new[]
                            {
                                42, 43
                            },
                            new[]
                            {
                                44, 45
                            },
                            new[]
                            {
                                46, 47
                            },
                            new[]
                            {
                                48, 49
                            },
                            new[]
                            {
                                50, 51
                            },
                            new[]
                            {
                                52, 53
                            },
                            new[]
                            {
                                54, 55
                            },
                            new[]
                            {
                                56, 57
                            },
                            new[]
                            {
                                58, 59
                            }
                        };

                    return new[]
                    {
                        new[]
                        {
                            0, 1
                        },
                        new[]
                        {
                            2, 3
                        },
                        new[]
                        {
                            4, 5
                        },
                        new[]
                        {
                            6, 7
                        },
                        new[]
                        {
                            8, 9
                        },
                        new[]
                        {
                            10, 11
                        },
                        new[]
                        {
                            12, 13
                        },
                        new[]
                        {
                            14, 15
                        },
                        new[]
                        {
                            16, 17
                        },
                        new[]
                        {
                            18, 19
                        },
                        new[]
                        {
                            20, 21
                        },
                        new[]
                        {
                            22, 23
                        },
                        new[]
                        {
                            24, 25
                        },
                        new[]
                        {
                            26, 27
                        },
                        new[]
                        {
                            28, 29
                        },
                        new[]
                        {
                            30, 31
                        },
                        new[]
                        {
                            32, 33
                        },
                        new[]
                        {
                            34, 35
                        },
                        new[]
                        {
                            36, 37
                        },
                        new[]
                        {
                            38, 39
                        },
                        new[]
                        {
                            40, 41
                        },
                        new[]
                        {
                            42, 43
                        },
                        new[]
                        {
                            44, 45
                        },
                        new[]
                        {
                            46, 47
                        },
                        new[]
                        {
                            48, 49
                        },
                        new[]
                        {
                            50, 51
                        },
                        new[]
                        {
                            52, 53
                        },
                        new[]
                        {
                            54, 55
                        },
                        new[]
                        {
                            56, 57
                        },
                        new[]
                        {
                            58, 59
                        }
                    };
                case 4:
                    if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                        return new[]
                        {
                            new[]
                            {
                                0, 1, 2, 3
                            },
                            new[]
                            {
                                4, 5, 6, 7
                            },
                            new[]
                            {
                                8, 9, 10, 11
                            },
                            new[]
                            {
                                12, 13, 14, 15
                            },
                            new[]
                            {
                                16, 17, 18, 19
                            },
                            new[]
                            {
                                20, 21, 22, 23
                            },
                            new[]
                            {
                                24, 25, 26, 27
                            },
                            new[]
                            {
                                28, 29, 30, 31
                            },
                            new[]
                            {
                                32, 33, 34, 35
                            },
                            new[]
                            {
                                36, 37, 38, 39
                            },
                            new[]
                            {
                                40, 41, 42, 43
                            },
                            new[]
                            {
                                44, 45, 46, 47
                            },
                            new[]
                            {
                                48, 49, 50, 51
                            },
                            new[]
                            {
                                52, 53, 54, 55
                            },
                            new[]
                            {
                                56, 57, 58, 59
                            }
                        };

                    return new[]
                    {
                        new[]
                        {
                            0, 1, 2, 3
                        },
                        new[]
                        {
                            4, 5, 6, 7
                        },
                        new[]
                        {
                            8, 9, 10, 11
                        },
                        new[]
                        {
                            12, 13, 14, 15
                        },
                        new[]
                        {
                            16, 17, 18, 19
                        },
                        new[]
                        {
                            20, 21, 22, 23
                        },
                        new[]
                        {
                            24, 25, 26, 27
                        },
                        new[]
                        {
                            28, 29, 30, 31
                        },
                        new[]
                        {
                            32, 33, 34, 35
                        },
                        new[]
                        {
                            36, 37, 38, 39
                        },
                        new[]
                        {
                            40, 41, 42, 43
                        },
                        new[]
                        {
                            44, 45, 46, 47
                        },
                        new[]
                        {
                            48, 49, 50, 51
                        },
                        new[]
                        {
                            52, 53, 54, 55
                        },
                        new[]
                        {
                            56, 57, 58, 59
                        }
                    };
                case 5:
                    if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                        return new[]
                        {
                            new[]
                            {
                                0, 1, 2, 3, 4
                            },
                            new[]
                            {
                                5, 6, 7, 8, 9
                            },
                            new[]
                            {
                                10, 11, 12, 13, 14
                            },
                            new[]
                            {
                                15, 16, 17, 18, 19
                            },
                            new[]
                            {
                                20, 21, 22, 23, 24
                            },
                            new[]
                            {
                                25, 26, 27, 28, 29
                            },
                            new[]
                            {
                                30, 31, 32, 33, 34
                            },
                            new[]
                            {
                                35, 36, 37, 38, 39
                            },
                            new[]
                            {
                                40, 41, 42, 43, 44
                            },
                            new[]
                            {
                                45, 46, 47, 48, 49
                            },
                            new[]
                            {
                                50, 51, 52, 53, 54
                            },
                            new[]
                            {
                                55, 56, 57, 58, 59
                            }
                        };

                    return new[]
                    {
                        new[]
                        {
                            0, 1, 2, 3, 4
                        },
                        new[]
                        {
                            5, 6, 7, 8, 9
                        },
                        new[]
                        {
                            10, 11, 12, 13, 14
                        },
                        new[]
                        {
                            15, 16, 17, 18, 19
                        },
                        new[]
                        {
                            20, 21, 22, 23, 24
                        },
                        new[]
                        {
                            25, 26, 27, 28, 29
                        },
                        new[]
                        {
                            30, 31, 32, 33, 34
                        },
                        new[]
                        {
                            35, 36, 37, 38, 39
                        },
                        new[]
                        {
                            40, 41, 42, 43, 44
                        },
                        new[]
                        {
                            45, 46, 47, 48, 49
                        },
                        new[]
                        {
                            50, 51, 52, 53, 54
                        },
                        new[]
                        {
                            55, 56, 57, 58, 59
                        }
                    };
                case 6:
                    if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                        return new[]
                        {
                            new[]
                            {
                                0, 1, 2, 3, 4, 5
                            },
                            new[]
                            {
                                6, 7, 8, 9, 10, 11
                            },
                            new[]
                            {
                                12, 13, 14, 15, 16, 17
                            },
                            new[]
                            {
                                18, 19, 20, 21, 22, 23
                            },
                            new[]
                            {
                                24, 25, 26, 27, 28, 29
                            },
                            new[]
                            {
                                30, 31, 32, 33, 34, 35
                            },
                            new[]
                            {
                                36, 37, 38, 39, 40, 41
                            },
                            new[]
                            {
                                42, 43, 44, 45, 46, 47
                            },
                            new[]
                            {
                                48, 49, 50, 51, 52, 53
                            },
                            new[]
                            {
                                54, 55, 56, 57, 58, 59
                            }
                        };

                    return new[]
                    {
                        new[]
                        {
                            0, 1, 2, 3, 4, 5
                        },
                        new[]
                        {
                            6, 7, 8, 9, 10, 11
                        },
                        new[]
                        {
                            12, 13, 14, 15, 16, 17
                        },
                        new[]
                        {
                            18, 19, 20, 21, 22, 23
                        },
                        new[]
                        {
                            24, 25, 26, 27, 28, 29
                        },
                        new[]
                        {
                            30, 31, 32, 33, 34, 35
                        },
                        new[]
                        {
                            36, 37, 38, 39, 40, 41
                        },
                        new[]
                        {
                            42, 43, 44, 45, 46, 47
                        },
                        new[]
                        {
                            48, 49, 50, 51, 52, 53
                        },
                        new[]
                        {
                            54, 55, 56, 57, 58, 59
                        }
                    };
            }

            if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                return new[]
                {
                    new[]
                    {
                        0, 1, 2
                    },
                    new[]
                    {
                        3, 4, 5
                    },
                    new[]
                    {
                        6, 7, 8
                    },
                    new[]
                    {
                        9, 10, 11
                    },
                    new[]
                    {
                        12, 13, 14
                    },
                    new[]
                    {
                        15, 16, 17
                    },
                    new[]
                    {
                        18, 19, 20
                    },
                    new[]
                    {
                        21, 22, 23
                    },
                    new[]
                    {
                        24, 25, 26
                    },
                    new[]
                    {
                        27, 28, 29
                    },
                    new[]
                    {
                        30, 31, 32
                    },
                    new[]
                    {
                        33, 34, 35
                    },
                    new[]
                    {
                        36, 37, 38
                    },
                    new[]
                    {
                        39, 40, 41
                    },
                    new[]
                    {
                        42, 43, 44
                    },
                    new[]
                    {
                        45, 46, 47
                    },
                    new[]
                    {
                        48, 49, 50
                    },
                    new[]
                    {
                        51, 52, 53
                    },
                    new[]
                    {
                        54, 55, 56
                    },
                    new[]
                    {
                        57, 58, 59
                    }
                };

            return new[]
            {
                new[]
                {
                    0, 1, 2
                },
                new[]
                {
                    3, 4, 5
                },
                new[]
                {
                    6, 7, 8
                },
                new[]
                {
                    9, 10, 11
                },
                new[]
                {
                    12, 13, 14
                },
                new[]
                {
                    15, 16, 17
                },
                new[]
                {
                    18, 19, 20
                },
                new[]
                {
                    21, 22, 23
                },
                new[]
                {
                    24, 25, 26
                },
                new[]
                {
                    27, 28, 29
                },
                new[]
                {
                    30, 31, 32
                },
                new[]
                {
                    33, 34, 35
                },
                new[]
                {
                    36, 37, 38
                },
                new[]
                {
                    39, 40, 41
                },
                new[]
                {
                    42, 43, 44
                },
                new[]
                {
                    45, 46, 47
                },
                new[]
                {
                    48, 49, 50
                },
                new[]
                {
                    51, 52, 53
                },
                new[]
                {
                    54, 55, 56
                },
                new[]
                {
                    57, 58, 59
                }
            };
        }


        public Point PowerPosition(PowerEntry powerEntry, int displayLocation = -1)
        {
            var powerIdx = MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry);
            checked
            {
                if (powerIdx == -1)
                {
                    var num2 = 0;
                    var num3 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                    for (var i = num2; i <= num3; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i].Power.PowerIndex !=
                            powerEntry.Power.PowerIndex ||
                            MidsContext.Character.CurrentBuild.Powers[i].Level != powerEntry.Level)
                            continue;
                        powerIdx = i;
                        break;
                    }
                }

                var inherentGrid = GetInherentGrid();
                var flag = false;
                var iRow = 0;
                var iCol = 0;

                if (!powerEntry.Chosen)
                {
                    if (displayLocation == -1 && powerEntry.Power != null)
                        displayLocation = powerEntry.Power.DisplayLocation;

                    if (displayLocation <= -1) return CRtoXY(iCol, iRow);
                    iRow = vcRowsPowers;
                    for (var i = 0; i <= inherentGrid.Length - 1; i++)
                    {
                        for (var k = 0; k <= inherentGrid[i].Length - 1; k++)
                        {
                            if (displayLocation != inherentGrid[i][k]) continue;
                            iRow += i + 1;
                            iCol = k;
                            flag = true;
                            break;
                        }

                        if (flag) break;
                    }
                }

                else if (powerIdx > -1)
                {
                    for (var i = 1; i <= vcCols; i++)
                    {
                        if (powerIdx >= vcRowsPowers * i)
                            continue;
                        iCol = i - 1;
                        iRow = powerIdx - vcRowsPowers * iCol;
                        break;
                    }
                }

                return CRtoXY(iCol, iRow);
            }
        }

        private Point CRtoXY(int iCol, int iRow)
        {
            var result = new Point(0, 0);
            checked
            {
                if (iRow >= vcRowsPowers)
                {
                    result.X = iCol * (SzPower.Width + 13);
                    result.Y = 6 + iRow * (SzPower.Height + 25);
                }
                else
                {
                    result.X = iCol * (SzPower.Width + 15);
                    result.Y = iRow * (SzPower.Height + 25);
                }

                return result;
            }
        }

        public Size GetDrawingArea()
        {
            var result = (Size) PowerPosition(23);
            checked
            {
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + 20;
                for (var i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i].Power == null ||
                        MidsContext.Character.CurrentBuild.Powers[i].Chosen &&
                        i > MidsContext.Character.CurrentBuild.LastPower)
                        continue;
                    var size = new Size(result.Width, PowerPosition(i).Y + SzPower.Height + 20);
                    if (size.Height > result.Height) result.Height = size.Height + 20;

                    if (size.Width > result.Width) result.Width = size.Width + 20;
                }

                return result;
            }
        }

        private Size GetMaxDrawingArea()
        {
            var cols = vcCols;
            MiniSetCol(6);
            var result = (Size) PowerPosition(23);
            MiniSetCol(2);
            var inherentGrid = GetInherentGrid();
            checked
            {
                var size = (Size) CRtoXY(inherentGrid[inherentGrid.Length - 1].Length - 1, inherentGrid.Length - 1);
                if (size.Height > result.Height) result.Height = size.Height + 20;

                if (size.Width > result.Width) result.Width = size.Width;

                MiniSetCol(cols);
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + 20;
                return result;
            }
        }

        private void MiniSetCol(int cols)
        {
            if (cols == vcCols)
                return;
            if ((cols < 2) | (cols > 6))
                return;
            vcCols = cols;
            vcRowsPowers = checked((int) Math.Round(24.0 / vcCols));
        }

        public Size GetRequiredDrawingArea()
        {
            var maxY = -1;
            var maxX = -1;
            checked
            {
                var num4 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                for (var i = 0; i <= num4; i++)
                {
                    if (!((MidsContext.Character.CurrentBuild.Powers[i].IDXPower > -1) |
                          MidsContext.Character.CurrentBuild.Powers[i].Chosen))
                        continue;
                    var point = PowerPosition(i);
                    if (point.X > maxX) maxX = point.X;

                    if (point.Y > maxY) maxY = point.Y;
                }

                Size result;
                if ((maxX > -1) & (maxY > -1))
                {
                    var size = new Size(maxX + SzPower.Width, maxY + SzPower.Height + 20);
                    result = size;
                }
                else
                {
                    var point2 = PowerPosition(MidsContext.Character.CurrentBuild.LastPower);
                    var size = new Size(point2.X + SzPower.Width, point2.Y + SzPower.Height + 20 + 4);
                    result = size;
                }

                return result;
            }
        }
    }
}