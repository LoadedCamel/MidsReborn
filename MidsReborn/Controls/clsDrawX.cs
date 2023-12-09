using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls
{
    public class ClsDrawX
    {
        // Horizontal space between power slots
        private const int PaddingX = 15;

        // Vertical space between power slots
        private const int PaddingY = 25;

        // Vertical offset for enhancement slots
        public const int OffsetY = 23;

        // Horizontal offset for enhancement slots
        private const int OffsetX = 30;

        private const int OffsetInherent = 10;

        // Same size as target drawing area
        private Size _szBuffer;

        // Size of a power slot
        public Size SzPower { get; set; }

        // Size of an enhancement slot
        public Size SzSlot;

        // Surface to draw on before combining to display
        public ExtendedBitmap? BxBuffer;

        // List of disabled, empty, filled, waiting
        public readonly List<ExtendedBitmap> BxPower;

        // The unplaced enhancement slot image
        public ExtendedBitmap? BxNewSlot;

        // Graphics object of target drawing surface (The panel)
        private Graphics _gTarget;


        // Column variables
        private const int VcPowers = 24;
        private int _vcCols;
        private int _vcRowsPowers;

        // Recoloring variables
        private ColorMatrix? _pColorMatrix;
        public ImageAttributes? PImageAttributes;

        // Scaling variables
        private float ScaleValue { get; set; }
        private bool Scaling { get; set; }

        public static readonly float[][] HeroMatrix =
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
        private static readonly float[][] VillainMatrix =
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
                0.75f, 0, 0, 0.175f, 0
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
        private const int IcoOffset = 32;
        private Color _backColor;
        private Control _cTarget;
        private Font _defaultFont;
        public int Highlight;
        public Enums.eInterfaceMode InterfaceMode;
        private readonly bool _inDesigner = AppDomain.CurrentDomain.FriendlyName.Contains("devenv");

        //bool VillainColor;

        public ClsDrawX(Control iTarget)
        {
            InterfaceMode = 0;
            //VillainColor = false;
            ScaleValue = 2f;
            Scaling = true;
            _vcCols = 6;
            _vcRowsPowers = 24;
            BxPower = new List<ExtendedBitmap>();
            checked
            {
                ColorSwitch();
                InitColumns = MidsContext.Config.Columns;
                InitializeAsync();
                _gTarget = iTarget.CreateGraphics();
                _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
                _gTarget.CompositingQuality = CompositingQuality.HighQuality;
                _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _gTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                _gTarget.SmoothingMode = SmoothingMode.HighQuality;
                _gTarget.CompositingMode = CompositingMode.SourceOver;
                _gTarget.PageUnit = GraphicsUnit.Pixel;
                _cTarget = iTarget;
                _defaultFont = new Font(Fonts.Family("Noto Sans"), 12.25f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
                _backColor = iTarget.BackColor;
                if (_szBuffer.Height < _cTarget.Height)
                {
                    _gTarget.FillRectangle(new SolidBrush(_backColor), 0, _szBuffer.Height, _cTarget.Width, _cTarget.Height - _szBuffer.Height);
                }
            }
        }

        private async void InitializeAsync()
        {
            var gfxImages = await I9Gfx.LoadButtons();
            var buttonPaths = gfxImages.Where(gfxImage => !string.IsNullOrWhiteSpace(gfxImage)).ToList();
            var firstPath = buttonPaths.First();
            if (string.IsNullOrWhiteSpace(firstPath)) throw new ArgumentException("Image path cannot be null or empty");
            foreach (var buttonPath in buttonPaths.OfType<string>())
            {
                BxPower.Add(new ExtendedBitmap(Image.FromFile(buttonPath)));
            }

            SzPower = BxPower[0].Size;


            var slotImagePath = await I9Gfx.LoadNewSlot();
            if (slotImagePath != null)
            {
                var slotImage = Image.FromFile(slotImagePath);
                BxNewSlot = new ExtendedBitmap(slotImage);
            }

            if (BxNewSlot != null) SzSlot = BxNewSlot.Size;

            _szBuffer = GetMaxDrawingArea();
            BxBuffer = new ExtendedBitmap(_szBuffer.Width, _szBuffer.Height);
            if (BxBuffer.Graphics != null)
            {
                BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
                BxBuffer.Graphics.PageUnit = GraphicsUnit.Pixel;
            }

            _szBuffer = GetRequiredDrawingArea();
        }

        public static bool EpicColumns => MidsContext.Character is { Archetype.ClassType: Enums.eClassType.HeroEpic };

        public int Columns
        {
            set
            {
                MiniSetCol(value);
                Blank();
                _szBuffer = GetRequiredDrawingArea();
                SetScaling(_cTarget.Size);
            }
        }

        private int InitColumns
        {
            init
            {
                if (value == _vcCols)
                    return;
                if ((value < 2) | (value > 6))
                    return;
                _vcCols = value;
                _vcRowsPowers = VcPowers / _vcCols;
            }
        }

        public void ReInit(Control iTarget)
        {
            if (iTarget.IsDisposed) return;
            _gTarget = iTarget.CreateGraphics();
            _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _gTarget.CompositingQuality = CompositingQuality.HighQuality;
            _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _gTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _gTarget.SmoothingMode = SmoothingMode.HighQuality;
            _gTarget.CompositingMode = CompositingMode.SourceOver;
            _gTarget.PageUnit = GraphicsUnit.Pixel;
            _cTarget = iTarget;
            //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
            _defaultFont = new Font(Fonts.Family("Noto Sans"), 12.25f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            _backColor = iTarget.BackColor;
        }

        private void DrawSplit()
        {
            var pen = new Pen(Color.Goldenrod, 2f);
            checked
            {
                int iValue;
                switch (_vcCols)
                {
                    case 2:
                        iValue = OffsetInherent + _vcRowsPowers * (SzPower.Height + 27);
                        BxBuffer.Graphics?.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(GetDrawingArea().Width), ScaleDown(iValue));
                        BxBuffer.Graphics?.DrawString("Inherent Powers", new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel), MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(GetDrawingArea().Width / 2 - 50), ScaleDown(iValue));
                        break;
                    case 3:
                        iValue = OffsetInherent + _vcRowsPowers * (SzPower.Height + 27);
                        BxBuffer.Graphics?.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(GetDrawingArea().Width), ScaleDown(iValue));
                        BxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(GetDrawingArea().Width / 2 - 50),
                            ScaleDown(iValue));

                        break;
                    case 4:
                        iValue = OffsetInherent + _vcRowsPowers * (SzPower.Height + 27);
                        BxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(GetDrawingArea().Width), ScaleDown(iValue));
                        BxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(GetDrawingArea().Width / 2 - 50),
                            ScaleDown(iValue));
                        break;
                    case 5:
                        iValue = OffsetInherent + _vcRowsPowers * (SzPower.Height + 48);
                        BxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(GetDrawingArea().Width), ScaleDown(iValue));
                        BxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(GetDrawingArea().Width / 2 - 50),
                            ScaleDown(iValue));
                        break;
                    case 6:
                        iValue = OffsetInherent + _vcRowsPowers * (SzPower.Height + 27);
                        BxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(GetDrawingArea().Width), ScaleDown(iValue));
                        BxBuffer.Graphics.DrawString("Inherent Powers",
                            new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                            MidsContext.Character.IsHero()
                                ? new SolidBrush(Color.DodgerBlue)
                                : new SolidBrush(Color.Red), ScaleDown(GetDrawingArea().Width / 2 - 50),
                            ScaleDown(iValue));
                        break;
                }
            }
        }

        private void DrawPowers()
        {
            checked
            {
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    if (MidsContext.Character.CanPlaceSlot & Highlight == i)
                    {
                        var value = MidsContext.Character.CurrentBuild.Powers[i];
                        DrawPowerSlot(ref value, true);
                        MidsContext.Character.CurrentBuild.Powers[i] = value;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[i] != null && MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                    {
                        var value = MidsContext.Character.CurrentBuild.Powers[i];
                        DrawPowerSlot(ref value);
                        MidsContext.Character.CurrentBuild.Powers[i] = value;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[i] != null && MidsContext.Character.CurrentBuild.Powers[i].Power != null && MidsContext.Character.CurrentBuild.Powers[i].Power.GroupName == "Incarnate" | MidsContext.Character.CurrentBuild.Powers[i].Power.IncludeFlag)
                    {
                        var value = MidsContext.Character.CurrentBuild.Powers[i];
                        DrawPowerSlot(ref value);
                        MidsContext.Character.CurrentBuild.Powers[i] = value;
                    }
                }

                Application.DoEvents();
                DrawSplit();
            }
        }

        private float FontScale(float iSz)
        {
            return Math.Min(ScaleDown(iSz) * 1.1f, iSz);
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
                if (enhType == Enums.eType.SetO || enhType == Enums.eType.InventO)
                {
                    var iValue2 = rect;
                    iValue2.Y -= 5f;
                    if (BxBuffer.Graphics != null)
                    {
                        iValue2.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                        var relativeLevelNumeric = "";
                        var enhInternalName = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].UID;
                        var catalystSet = DatabaseAPI.EnhHasCatalyst(enhInternalName) ||
                                          enhInternalName.ToLowerInvariant().Contains("overwhelming_force");
                        // Catalysed enhancements take character level no matter what.
                        // Game does not allow boosters over enhancement catalysts.
                        if (!catalystSet & slot.Enhancement.RelativeLevel > Enums.eEnhRelative.Even &
                            MidsContext.Config.ShowEnhRel)
                        {
                            relativeLevelNumeric = Enums.GetRelativeString(slot.Enhancement.RelativeLevel, false);
                        }

                        // If enhancement has boosters, need to stretch the level drawing zone a little,
                        // or relative level doesn't fit in.
                        if (!string.IsNullOrEmpty(relativeLevelNumeric))
                        {
                            iValue2.Width += 10f;
                            iValue2.X -= 5f;
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
                            g = BxBuffer.Graphics;

                            DrawOutlineText(iStr, bounds, cyan, outline, font, outlineSpace, g);
                        }
                    }
                }
                else if (enhType == Enums.eType.Normal || enhType == Enums.eType.SpecialO)
                {
                    var iValue2 = rect;
                    iValue2.Y -= 5f;
                    iValue2.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                    Color color;

                    if (slot.Enhancement.RelativeLevel == 0)
                    {
                        color = Color.Red;
                    }
                    else if (slot.Enhancement.RelativeLevel < Enums.eEnhRelative.Even)
                    {
                        color = Color.Yellow;
                    }
                    else if (slot.Enhancement.RelativeLevel > Enums.eEnhRelative.Even)
                    {
                        color = Color.FromArgb(0, 255, 0);
                    }
                    else
                    {
                        color = Color.White;
                    }

                    string relativeString;
                    // Always display relative level if present
                    //if (MidsContext.Config.ShowEnhRel)
                    relativeString = Enums.GetRelativeString(slot.Enhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols);

                    // +3 SO do not exist ingame, at least not through combinations.
                    // Display flat level instead.
                    if (slot.Enhancement.RelativeLevel == Enums.eEnhRelative.PlusThree)
                    {
                        // I dunno. MidsContext.Character.Level == 48 ?
                        // Zed 07/07 - Having issues using MidsContext.Character.Level
                        //relativeString = Convert.ToString(Math.Max(53, MidsContext.Character.Level + 5), null);
                        relativeString = "53";
                    }
                    else if (MidsContext.Config.ShowSoLevels)
                    {
                        // It isn't slot.Enhancement.IOLevel... always set to 0
                        // Improvisation it is...
                        //relativeString = Convert.ToString(Math.Max(50, MidsContext.Character.Level + 2), null) + relativeString;
                        relativeString = "50" + relativeString;
                    }

                    if (
                        //MidsContext.Config.ShowEnhRel &&
                        MidsContext.Config.ShowSoLevels &&
                        slot.Enhancement.RelativeLevel != Enums.eEnhRelative.None &&
                        slot.Enhancement.RelativeLevel != Enums.eEnhRelative.Even &&
                        slot.Enhancement.RelativeLevel != Enums.eEnhRelative.PlusThree
                    )
                    {
                        iValue2.Width += 10f;
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

        public Point DrawPowerSlot(ref PowerEntry? iSlot, bool singleDraw = false)
        {
            var pen = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            var text = string.Empty;
            var text2 = string.Empty;
            var rectangleF = new RectangleF(0f, 0f, 0f, 0f);
            var stringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
            {
                Trimming = StringTrimming.None
            };
            var pen2 = new Pen(Color.Black);
            Rectangle toggleRect = default;
            Rectangle procRect = default;
            var style = !MidsContext.Config.RtFont.PowersBold ? FontStyle.Regular : FontStyle.Bold;
            var font = MidsContext.Config.RtFont.PowersBase > 0 ? new Font(_defaultFont.FontFamily, FontScale(MidsContext.Config.RtFont.PowersBase), style, GraphicsUnit.Point, 0) : new Font(_defaultFont.FontFamily, FontScale(8), style, GraphicsUnit.Point, 0);
            var slotChk = MidsContext.Character.SlotCheck(iSlot);
            var ePowerState = iSlot.State;
            var canPlaceSlot = MidsContext.Character.CanPlaceSlot;
            var drawNewSlot = iSlot.Power != null && (iSlot.State != Enums.ePowerState.Empty && canPlaceSlot) && iSlot.Slots.Length < 6 && singleDraw && iSlot.Power.Slottable & InterfaceMode != Enums.eInterfaceMode.PowerToggle;
            var location = PowerPosition(iSlot);
            Point slotLocation = default;
            checked
            {
                //Position of enhancements drawing
                slotLocation.X = (int)Math.Round(location.X - OffsetX + checked(SzPower.Width - SzSlot.Width * 6) / 2.0);
                slotLocation.Y = location.Y + OffsetY;
                //
                Brush brush = new SolidBrush(_backColor);
                var clipRect = new Rectangle(slotLocation.X, slotLocation.Y, SzPower.Width, SzPower.Height);
                BxBuffer.Graphics.FillRectangle(brush, ScaleDown(clipRect));
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
                        {
                            ePowerState = Enums.ePowerState.Open;
                        }
                        else if (iSlot.Chosen & !canPlaceSlot & InterfaceMode != Enums.eInterfaceMode.PowerToggle & Highlight == MidsContext.Character.CurrentBuild.Powers.IndexOf(iSlot))
                        {
                            ePowerState = Enums.ePowerState.Open;
                        }
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers.IndexOf(iSlot) == IndexFromLevel())
                    {
                        ePowerState = Enums.ePowerState.Open;
                    }
                }

                rectangleF.Height = SzSlot.Height;
                rectangleF.Width = SzSlot.Width;
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                bool grey;
                ImageAttributes? imageAttr;
                if (toggling)
                {
                    if (ePowerState == Enums.ePowerState.Open)
                    {
                        ePowerState = Enums.ePowerState.Empty;
                    }

                    if (iSlot.StatInclude & ePowerState == Enums.ePowerState.Used)
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
                    grey = (iSlot.Level >= MidsContext.Config.ForceLevel);
                    imageAttr = GreySlot(grey);
                }
                // Power position/sizing?
                var powerRect = new Rectangle(location.X, location.Y, BxPower[(int)ePowerState].Size.Width, BxPower[(int)ePowerState].Size.Height);
                //

                var destRect = ScaleDown(powerRect);
                var width = BxPower[(int)ePowerState].ClipRect.Width;
                var clipRect2 = BxPower[(int)ePowerState].ClipRect;
                if (ePowerState == Enums.ePowerState.Used || toggling)
                {
                    if (!MidsContext.Config.DisableDesaturateInherent & !iSlot.Chosen)
                    {
                        imageAttr = Desaturate(grey, ePowerState == Enums.ePowerState.Open);
                    }

                    Image bitmap = !MidsContext.Character.IsHero() ? BxPower[4].Bitmap : BxPower[2].Bitmap;
                    BxBuffer.Graphics.DrawImage(bitmap, destRect, 0, 0, width, clipRect2.Height, GraphicsUnit.Pixel, imageAttr);
                }
                else if (ePowerState == Enums.ePowerState.Open)
                {
                    Image bitmap2 = !MidsContext.Character.IsHero() ? BxPower[5].Bitmap : BxPower[3].Bitmap;
                    //Image bitmap2 = bxPower[(int)ePowerState].Bitmap;
                    BxBuffer.Graphics.DrawImage(bitmap2, destRect, 0, 0, width, clipRect2.Height, GraphicsUnit.Pixel);
                }
                else
                {
                    Image bitmap2 = BxPower[(int)ePowerState].Bitmap;
                    BxBuffer.Graphics.DrawImage(bitmap2, destRect, 0, 0, width, clipRect2.Height, GraphicsUnit.Pixel);
                }
                // Toggle button on powers
                if (iSlot.CanIncludeForStats() && !iSlot.HasProc())
                {
                    var toggleGraphics = BxBuffer.Graphics;
                    toggleRect.Height = 15;
                    toggleRect.Width = toggleRect.Height;
                    toggleRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - toggleRect.Height) / 2.0);
                    toggleRect.X = (int)Math.Round(powerRect.Right - (toggleRect.Width + checked(powerRect.Height - toggleRect.Height) / 2.0));
                    toggleRect = ScaleDown(toggleRect);
                    var iCenter = new PointF(-0.25f, -0.33f);
                    var brush2 = iSlot.StatInclude
                        ? MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                        : MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

                    toggleGraphics.FillEllipse(brush2, toggleRect);
                    toggleGraphics.DrawEllipse(pen2, toggleRect);
                }
                if (iSlot.HasProc() && !iSlot.CanIncludeForStats())
                {
                    //draw proc toggle
                    var procGraphics = BxBuffer.Graphics;
                    procRect.Height = 15;
                    procRect.Width = procRect.Height;
                    procRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - procRect.Height) / 2.0);
                    procRect.X = (int)Math.Round(powerRect.Right - (procRect.Width + checked(powerRect.Height - procRect.Height) / 2.0));
                    procRect = ScaleDown(procRect);
                    var pCenter = new PointF(-0.25f, -0.33f);
                    var brush3 = !iSlot.ProcInclude
                        ? MakePathBrush(procRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                        : MakePathBrush(procRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

                    procGraphics.FillEllipse(brush3, procRect);
                    procGraphics.DrawEllipse(pen2, procRect);
                }
                if (iSlot.HasProc() && iSlot.CanIncludeForStats())
                {
                    //draw power toggle
                    var toggleGraphics = BxBuffer.Graphics;
                    toggleRect.Height = 15;
                    toggleRect.Width = toggleRect.Height;
                    toggleRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - toggleRect.Height) / 2.0);
                    toggleRect.X = (int)Math.Round(powerRect.Right - (toggleRect.Width + checked(powerRect.Height - toggleRect.Height) / 3.0));
                    toggleRect = ScaleDown(toggleRect);
                    var iCenter = new PointF(-0.25f, -0.33f);
                    var brush2 = iSlot.StatInclude
                        ? MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                        : MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

                    toggleGraphics.FillEllipse(brush2, toggleRect);
                    toggleGraphics.DrawEllipse(pen2, toggleRect);

                    //draw proc toggle
                    var procGraphics = BxBuffer.Graphics;
                    procRect.Height = 15;
                    procRect.Width = procRect.Height;
                    procRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - procRect.Height) / 2.0);
                    procRect.X = (int)Math.Round(powerRect.Right - (procRect.Width + checked(powerRect.Height - procRect.Height) / 1.0));
                    procRect = ScaleDown(procRect);
                    var pRect = procRect;
                    var pCenter = new PointF(-0.25f, -0.33f);
                    var brush3 = !iSlot.ProcInclude
                        ? MakePathBrush(pRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                        : MakePathBrush(pRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

                    procGraphics.FillEllipse(brush3, procRect);
                    procGraphics.DrawEllipse(pen2, procRect);
                }
                SolidBrush solidBrush;
                //if (!System.Diagnostics.Debugger.IsAttached || !this.IsInDesignMode() || !System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
                for (var i = 0; i < iSlot.Slots.Length; i++)
                {
                    var slot = iSlot.Slots[i];
                    // Enhancement spacing and position?
                    rectangleF.X = slotLocation.X + (SzSlot.Width + 2) * i;
                    rectangleF.Y = slotLocation.Y;
                    //
                    if (slot.Enhancement.Enh < 0)
                    {
                        var clipRect3 = new Rectangle((int)Math.Round(rectangleF.X), slotLocation.Y, SzSlot.Width, SzSlot.Height); // New slot rectangle
                        BxBuffer.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, ScaleDown(clipRect3), 0, 0, SzSlot.Width, SzSlot.Height, GraphicsUnit.Pixel, PImageAttributes);
                        if (MidsContext.Config.CalcEnhLevel == 0 | slot.Level > MidsContext.Config.ForceLevel | (InterfaceMode == Enums.eInterfaceMode.PowerToggle & !iSlot.StatInclude) | (!iSlot.AllowFrontLoading & slot.Level < iSlot.Level))
                        {
                            solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                            BxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(rectangleF));
                            BxBuffer.Graphics.DrawEllipse(pen, ScaleDown(rectangleF));
                        }
                    }
                    else
                    {
                        // Controls if powers or slots are greyed out
                        if (_inDesigner)
                        {
                            continue;
                        }

                        var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                        var graphics6 = BxBuffer.Graphics;
                        var clipRect3 = new Rectangle((int)Math.Round(rectangleF.X), slotLocation.Y, SzSlot.Width, SzSlot.Height);
                        I9Gfx.DrawEnhancementAt(ref graphics6, ScaleDown(clipRect3), enhancement.ImageIdx, I9Gfx.ToGfxGrade(enhancement.TypeID, slot.Enhancement.Grade));
                        if (slot.Enhancement.RelativeLevel == 0 | slot.Level > MidsContext.Config.ForceLevel | (InterfaceMode == Enums.eInterfaceMode.PowerToggle & !iSlot.StatInclude) | (!iSlot.AllowFrontLoading & slot.Level < iSlot.Level) | (MidsContext.EnhCheckMode & !slot.Enhancement.Obtained))
                        {
                            solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                            var iValue3 = rectangleF;
                            iValue3.Inflate(1f, 1f);
                            BxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(iValue3));
                        }

                        if (slot.Enhancement.Enh > -1)
                        {
                            DrawEnhancementLevel(slot, font, BxBuffer.Graphics, ref rectangleF);
                        }
                    }

                    if (!MidsContext.Config.ShowSlotLevels)
                    {
                        continue;
                    }

                    var powerTextRect = rectangleF;
                    unchecked
                    {
                        //Positioning of slot level text
                        powerTextRect.Y += powerTextRect.Height + 12;
                        powerTextRect.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                        powerTextRect.Y -= powerTextRect.Height;
                        powerTextRect.X += powerTextRect.Width;
                        powerTextRect.X -= powerTextRect.Width - 1;
                    }

                    DrawOutlineText(
                        iStr: Convert.ToString(slot.Level + 1),
                        bounds: ScaleDown(powerTextRect),
                        textColor: Color.FromArgb(0, 255, 0),
                        outlineColor: Color.FromArgb(192, 0, 0, 0),
                        bFont: font,
                        outlineSpace: 1f,
                        g: BxBuffer.Graphics);
                }
                //Draws the new slot hover
                if (slotChk > -1 && (ePowerState != Enums.ePowerState.Empty && drawNewSlot))
                {
                    var slotHoverRect = new RectangleF(slotLocation.X + (SzSlot.Width + 2) * (iSlot.Slots.Length), slotLocation.Y, SzSlot.Width, SzSlot.Height);
                    BxBuffer.Graphics.DrawImage(BxNewSlot.Bitmap, ScaleDown(slotHoverRect));
                    slotHoverRect.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                    slotHoverRect.Y += (SzSlot.Height - slotHoverRect.Height) / 2f;
                    DrawOutlineText(Convert.ToString(slotChk + 1), ScaleDown(slotHoverRect),
                        Color.FromArgb(0, 255, 255), Color.FromArgb(192, 0, 0, 0), font, 1f, BxBuffer.Graphics);
                }
                //Power name text positioning
                solidBrush = new SolidBrush(Color.White);
                stringFormat = new StringFormat();
                rectangleF.X = location.X + 10;
                rectangleF.Y = location.Y + 4;
                rectangleF.Width = SzPower.Width;
                rectangleF.Height = _defaultFont.GetHeight() * 2f;
                var ePowerState2 = iSlot.State;
                if (ePowerState2 == Enums.ePowerState.Empty & ePowerState == Enums.ePowerState.Open)
                {
                    ePowerState2 = ePowerState;
                }

                switch (ePowerState2)
                {
                    case 0:
                        solidBrush = new SolidBrush(Color.Transparent);
                        text = "";
                        break;
                    case Enums.ePowerState.Empty:
                        solidBrush = new SolidBrush(Color.WhiteSmoke);
                        text = $"({iSlot.Level + 1})";
                        break;
                    case Enums.ePowerState.Used:
                        if (iSlot.PowerSet.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary
                            or Enums.ePowerSetType.Ancillary or Enums.ePowerSetType.Inherent
                            or Enums.ePowerSetType.Pool)
                        {
                            text2 = "";
                        }

                        solidBrush = !MidsContext.Character.IsHero() ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);

                        text = iSlot.Virtual
                            ? iSlot.Name
                            : $"({iSlot.Level + 1}) {iSlot.Name} {text2}";
                        break;
                    case Enums.ePowerState.Open:
                        solidBrush = new SolidBrush(Color.WhiteSmoke);
                        text = $"({iSlot.Level + 1})";
                        break;
                }

                if (ePowerState == Enums.ePowerState.Empty & iSlot.State == Enums.ePowerState.Used)
                {
                    solidBrush = new SolidBrush(Color.WhiteSmoke);
                }

                if (InterfaceMode == Enums.eInterfaceMode.PowerToggle && solidBrush.Color == Color.Black && !iSlot.CanIncludeForStats())
                {
                    solidBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
                }

                stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                if (MidsContext.Config.EnhanceVisibility)
                {
                    var bounds5 = ScaleDown(rectangleF);
                    //outlineColor = Color.FromArgb(192, 0, 0, 0);
                    //outlineColor = Color.FromArgb(192, 0, 0, 0);
                    BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    BxBuffer.Graphics.PageUnit = GraphicsUnit.Pixel;
                    BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    DrawOutlineText(text, bounds5, Color.White, Color.Black, font, 1f, BxBuffer.Graphics, false, true);
                }
                else
                {
                    BxBuffer.Graphics.DrawString(text, font, solidBrush, ScaleDown(rectangleF), stringFormat);
                }

                return location;
            }
        }

        private PathGradientBrush MakePathBrush(Rectangle iRect, PointF iCenter, Color iColor1, Color icolor2)
        {
            var num = (float)(iRect.Left + iRect.Width * 0.5);
            var num2 = (float)(iRect.Top + iRect.Height * 0.5);
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

            var centerPoint = new PointF((float)(num + (iCenter.X + iCenter.X * (iRect.Width * 0.5))), (float)(num2 + (iCenter.Y + iCenter.Y * (iRect.Height * 0.5))));
            pathGradientBrush2.CenterPoint = centerPoint;
            return pathGradientBrush;
        }

        public void FullRedraw()
        {
            ColorSwitch();
            _backColor = _cTarget.BackColor;
            BxBuffer.Graphics?.Clear(_backColor);
            DrawPowers();
            var location = new Point(0, 0);
            try
            {
                OutputUnscaled(ref BxBuffer, location);
            }
            catch (Exception)
            {
                // Call will fail if loading a build made with a different database
                // and auto switch
            }

            GC.Collect();
        }

        public void Refresh(Rectangle clip)
        {
            OutputRefresh(clip, clip, GraphicsUnit.Pixel);
        }

        private int GetVisualIdx(int powerIndex)
        {
            var nidPowerset = MidsContext.Character.CurrentBuild.Powers[powerIndex] != null
                ? MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPowerset
                : -1;
            var idxPower = MidsContext.Character.CurrentBuild.Powers[powerIndex] != null
                ? MidsContext.Character.CurrentBuild.Powers[powerIndex].IDXPower
                : -1;

            if (nidPowerset > -1)
            {
                int vIdx;
                if (DatabaseAPI.Database.Powersets[nidPowerset].SetType == Enums.ePowerSetType.Inherent)
                {
                    vIdx = DatabaseAPI.Database.Powersets[nidPowerset].Powers[idxPower].LocationIndex;
                }
                else
                {
                    vIdx = -1;
                    for (var i = 0; i <= powerIndex; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i]?.NIDPowerset > -1)
                        {
                            if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != Enums.ePowerSetType.Inherent)
                            {
                                vIdx++;
                            }
                        }
                        else
                        {
                            vIdx++;
                        }
                    }
                }

                return vIdx;
            }
            else
            {
                var vIdx = -1;
                for (var i = 0; i <= powerIndex; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i]?.NIDPowerset > -1)
                    {
                        if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != Enums.ePowerSetType.Inherent)
                        {
                            vIdx++;
                        }
                    }
                    else
                    {
                        vIdx++;
                    }
                }

                return vIdx;
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
            var layoutRectangle2 = new RectangleF(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, bFont.GetHeight(g));
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
                    if (MidsContext.Character.CurrentBuild.Powers[i] != null && (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen))
                        point = PowerPosition(GetVisualIdx(i));
                    else
                        point = PowerPosition(i);
                    if (iX >= point.X && iY >= point.Y && iX < SzPower.Width + point.X && iY < point.Y + SzPower.Height + (PaddingY / 2))
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
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i] != null && (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen))
                        point = PowerPosition(GetVisualIdx(i));
                    else
                        point = PowerPosition(i);

                    if (iX < point.X || iY < point.Y || iX >= SzPower.Width + point.X || iY >= point.Y + SzPower.Height + PaddingY / 2)
                    {
                        continue;
                    }

                    oPower = i;
                    break;
                }

                if (oPower <= -1)
                {
                    return -1;
                }

                var isValid = false;
                if (iY >= point.Y + OffsetY) // Y boundary of enhancments
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
                {
                    return -1;
                }

                iX -= point.X - checked(SzPower.Width - IcoOffset * 8); //X boundary of enhancments
                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers[oPower].Slots.Length; i++)
                {
                    var iZ = (i + 1) * IcoOffset;
                    if (iX <= iZ)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        public bool HighlightSlot(int idx, bool force = false)
        {
            checked
            {
                if (MidsContext.Character.CurrentBuild.Powers.Count >= 1)
                {
                    if (Highlight == idx && !force) return false;
                    if (idx != -1)
                    {
                        Build currentBuild;
                        PowerEntry? value;
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
                            iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + PaddingY);
                            rectangle = ScaleDown(iValue);
                            DrawSplit();
                            Output(ref BxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
                        }

                        Highlight = idx;
                        currentBuild = MidsContext.Character.CurrentBuild;
                        value = currentBuild.Powers[idx];
                        var point3 = DrawPowerSlot(ref value, true);
                        currentBuild.Powers[idx] = value;
                        point2 = point3;
                        iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + PaddingY);
                        rectangle = ScaleDown(iValue);
                        DrawSplit();
                        Output(ref BxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
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
                        var iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + PaddingY);
                        var rectangle = ScaleDown(iValue);
                        DrawSplit();
                        Output(ref BxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
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
            BxBuffer?.Graphics.Clear(_backColor);
        }

        public void SetScaling(Size iSize)
        {
            var origScaling = Scaling;
            var origScaleValue = ScaleValue;
            if ((iSize.Width < 10) | (iSize.Height < 10))
                return;
            var drawingArea = GetDrawingArea();
            if ((drawingArea.Width > iSize.Width) | (drawingArea.Height > iSize.Height))
            {
                // Enable scaling
                Scaling = true;
                if ((double)drawingArea.Width / iSize.Width > drawingArea.Height / (double)iSize.Height)
                {
                    // Shrink to fit width
                    ScaleValue = (float)(drawingArea.Width / (double)iSize.Width);
                }
                else
                {
                    // Fit height
                    ScaleValue = (float)(drawingArea.Height / (double)iSize.Height);
                }

                ResetTarget();
                BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;

                if (ScaleValue != origScaleValue)
                {
                    FullRedraw();
                    BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
                }
            }
            else
            {
                BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                ScaleValue = 1f;
                ResetTarget();
                Scaling = false;
                if (origScaling != Scaling | origScaleValue != ScaleValue)
                {
                    FullRedraw();
                }
            }
        }

        private void ResetTarget()
        {
            if (BxBuffer.Graphics != null)
                BxBuffer.Graphics.TextRenderingHint = ScaleValue > 1.125
                    ? TextRenderingHint.SystemDefault
                    : TextRenderingHint.ClearTypeGridFit;
            _gTarget.Dispose();
            if (_cTarget.IsDisposed) return;
            _gTarget = _cTarget.CreateGraphics();
            _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _gTarget.CompositingQuality = CompositingQuality.HighQuality;
            _gTarget.CompositingMode = CompositingMode.SourceOver;
            _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _gTarget.SmoothingMode = SmoothingMode.HighQuality;
        }

        public int ScaleDown(int iValue)
        {
            // Take a full size value and convert it to a scaled value
            // for co-ordinates, etc.
            int result;
            if (!Scaling)
            {
                result = iValue;
            }
            else
            {
                iValue = checked((int)Math.Round(iValue / ScaleValue));
                result = iValue;
            }

            return result;
        }

        public int ScaleUp(int iValue)
        {
            // Take a full size value and convert it to a scaled value
            // for co-ordinates, etc.
            int result;
            if (!Scaling)
            {
                result = iValue;
            }
            else
            {
                iValue = checked((int)Math.Round(iValue * ScaleValue));
                result = iValue;
            }

            return result;
        }

        private float ScaleDown(float iValue)
        {
            // Take a full size value and convert it to a scaled value
            // for co-ordinates, etc.
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
            // Take a full size value and convert it to a scaled value
            // for co-ordinates, etc.
            checked
            {
                Rectangle result;
                if (!Scaling)
                {
                    result = iValue;
                }
                else
                {
                    iValue.X = (int)Math.Round(iValue.X / ScaleValue);
                    iValue.Y = (int)Math.Round(iValue.Y / ScaleValue);
                    iValue.Width = (int)Math.Round(iValue.Width / ScaleValue);
                    iValue.Height = (int)Math.Round(iValue.Height / ScaleValue);
                    result = iValue;
                }

                return result;
            }
        }

        private RectangleF ScaleDown(RectangleF iValue)
        {
            // Take a full size value and convert it to a scaled value
            // for co-ordinates, etc.
            checked
            {
                if (!Scaling)
                    return iValue;
                iValue.X = (int)Math.Round(iValue.X / ScaleValue);
                iValue.Y = (int)Math.Round(iValue.Y / ScaleValue);
                iValue.Width = (int)Math.Round(iValue.Width / ScaleValue);
                iValue.Height = (int)Math.Round(iValue.Height / ScaleValue);
                return iValue;
            }
        }

        private void Output(ref ExtendedBitmap? buffer, Rectangle destRect, Rectangle srcRect, GraphicsUnit iUnit)
        {
            _gTarget.DrawImage(buffer.Bitmap, destRect, srcRect, iUnit);
        }

        private void OutputRefresh(Rectangle destRect, Rectangle srcRect, GraphicsUnit iUnit)
        {
            _gTarget.DrawImage(BxBuffer.Bitmap, destRect, srcRect, iUnit);
        }

        private void OutputUnscaled(ref ExtendedBitmap? buffer, Point location)
        {
            _gTarget.DrawImageUnscaled(buffer.Bitmap, location);
        }

        public void ColorSwitch()
        {
            /*bool useHeroColors = true;
            if (MidsContext.Character != null)
                useHeroColors = MidsContext.Character.IsHero();
            if (MidsContext.Config.DisableVillainColors)
                useHeroColors = true;
            VillainColor = !useHeroColors;*/
            _pColorMatrix = new ColorMatrix(HeroMatrix);
            PImageAttributes ??= new ImageAttributes();
            PImageAttributes.SetColorMatrix(_pColorMatrix);
        }

        public static ImageAttributes GetRecolorIa(bool hero)
        {
            var colorMatrix = new ColorMatrix(HeroMatrix);
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            return imageAttributes;
        }

        private ImageAttributes? GreySlot(bool grey, bool bypassIa = false)
        {
            if (!grey) return bypassIa ? new ImageAttributes() : PImageAttributes;

            checked
            {
                var colorMatrix = new ColorMatrix(HeroMatrix);
                var r = 0;
                do
                {
                    var c = 0;
                    do
                    {
                        if (!bypassIa) colorMatrix[r, c] = _pColorMatrix[r, c];

                        colorMatrix[r, c] = (float)(colorMatrix[r, c] / 1.5);

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                return imageAttributes;
            }
        }

        private ImageAttributes? Desaturate(bool grey, bool bypassIa = false)
        {
            var tMm = new ColorMatrix(new[]
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
            var tCm = new ColorMatrix(HeroMatrix);
            var r = 0;
            checked
            {
                do
                {
                    var c = 0;
                    do
                    {
                        //controls shading of inherents
                        if (!bypassIa) tCm[r, c] = (_pColorMatrix[r, c] + tMm[r, c]) / 2f;

                        if (grey) tCm[r, c] = (float)(tCm[r, c] / 1.5);

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(tCm);
                return imageAttributes;
            }
        }

        public Rectangle PowerBoundsUnScaled(int hIdx)
        {
            // Returns unscaled bounds
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
                        rectangle.Location = PowerPosition(GetVisualIdx(hIdx));

                    rectangle.Width = SzPower.Width;
                    var num = rectangle.Y + OffsetY;
                    num += SzSlot.Height;
                    rectangle.Height = num - rectangle.Y;
                    result = rectangle;
                }

                return result;
            }
        }

        public bool WithinPowerBar(Rectangle pBounds, Point e)
        {
            pBounds.Height = SzPower.Height;
            return e.X >= pBounds.Left && e.X < pBounds.Right && e.Y >= pBounds.Top && e.Y < pBounds.Bottom;
        }

        private Point PowerPosition(int powerEntryIdx)
        {
            return PowerPosition(MidsContext.Character.CurrentBuild.Powers[powerEntryIdx]);
        }

        private int[][] GetInherentGrid()
        {
            switch (_vcCols)
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

        private Point PowerPositionCr(PowerEntry? powerEntry, int displayLocation = -1)
        {
            var powerIdx = MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry);
            checked
            {
                // Assume that this is a copy and not the actual powerEntry item
                if (powerIdx == -1)
                {
                    const int num2 = 0;
                    var num3 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                    for (var i = num2; i <= num3; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i] == null) continue;
                        if (MidsContext.Character.CurrentBuild.Powers[i].Power.PowerIndex !=
                            powerEntry.Power.PowerIndex || MidsContext.Character.CurrentBuild.Powers[i].Level !=
                            powerEntry.Level)
                            continue;
                        powerIdx = i;
                        break;
                    }
                }

                //Inherent Grid
                var inherentGrid = GetInherentGrid();
                var flag = false;
                var iRow = 0;
                var iCol = 0;

                if (powerEntry is { Chosen: false })
                {
                    if (displayLocation == -1 && powerEntry.Power != null)
                    {
                        displayLocation = powerEntry.Power.DisplayLocation;
                    }

                    if (displayLocation <= -1)
                    {
                        return CRtoXy(iCol, iRow);
                    }

                    iRow = _vcRowsPowers;
                    for (var i = 0; i <= inherentGrid.Length - 1; i++)
                    {
                        for (var k = 0; k <= inherentGrid[i].Length - 1; k++)
                        {
                            if (displayLocation != inherentGrid[i][k]) continue;
                            if (_vcCols != 5)
                            {
                                iRow += i + 1;
                                iCol = k;
                            }
                            else
                            {
                                iRow += i + 2;
                                iCol = k;
                            }

                            flag = true;
                            break;
                        }

                        if (flag) break;
                    }
                }
                // Main Powers
                else if (powerIdx > -1)
                {
                    for (var i = 1; i <= _vcCols; i++)
                    {
                        if (_vcCols == 5)
                        {
                            iCol = (int)Math.Floor((double)powerIdx / _vcCols);
                            iRow = powerIdx % _vcCols;
                        }
                        else
                        {
                            if (powerIdx >= _vcRowsPowers * i)
                                continue;
                            iCol = i - 1;
                            iRow = powerIdx - _vcRowsPowers * iCol;
                        }
                        break;
                    }
                }

                return new Point(iCol, iRow);
            }
        }

        public Point PowerPosition(PowerEntry? powerEntry, int displayLocation = -1)
        {
            var crPos = PowerPositionCr(powerEntry, displayLocation);

            return CRtoXy(crPos.X, crPos.Y);
        }

        public Point PowerPosition(PowerEntry? powerEntry, bool ignorePadding, int displayLocation = -1)
        {
            var crPos = PowerPositionCr(powerEntry, displayLocation);

            return CRtoXy(crPos.X, crPos.Y, ignorePadding);
        }

        private Point CRtoXy(int iCol, int iRow, bool ignorePadding = false)
        {
            // Convert a column/row location to the top left XY co-ord of a power entry
            // 3 Columns, 15 Rows
            return new Point(
                iCol * (SzPower.Width + PaddingX * (ignorePadding ? 0 : 1)),
                iRow * (SzPower.Height + (PaddingY - (ignorePadding ? (int)Math.Round(5 / ScaleValue) : 0))) + (iRow >= _vcRowsPowers ? OffsetInherent : 0));
        }

        public Size GetDrawingArea()
        {
            var result = (Size)PowerPosition(VcPowers - 1);
            checked
            {
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + PaddingY;
                for (var i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i] != null && (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen && i > MidsContext.Character.CurrentBuild.LastPower))
                        continue;
                    var size = new Size(result.Width, PowerPosition(i).Y + SzPower.Height + PaddingY);
                    if (size.Height > result.Height) result.Height = size.Height;

                    if (size.Width > result.Width) result.Width = size.Width;
                }

                return result;
            }
        }

        private Size GetMaxDrawingArea()
        {
            var cols = _vcCols;
            MiniSetCol(6);
            var result = (Size)PowerPosition(VcPowers - 1);
            MiniSetCol(2);
            var inherentGrid = GetInherentGrid();
            checked
            {
                var size = (Size)CRtoXy(inherentGrid[^1].Length - 1, inherentGrid.Length - 1);
                if (size.Height > result.Height) result.Height = size.Height;

                if (size.Width > result.Width) result.Width = size.Width;

                MiniSetCol(cols);
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + PaddingY;
                return result;
            }
        }

        private void MiniSetCol(int cols)
        {
            if (cols == _vcCols)
                return;
            if ((cols < 2) | (cols > 6))
                return;
            _vcCols = cols;
            _vcRowsPowers = VcPowers / _vcCols;
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
                    if (!((MidsContext.Character.CurrentBuild.Powers[i].IDXPower > -1) | MidsContext.Character.CurrentBuild.Powers[i].Chosen))
                        continue;
                    var point = PowerPosition(i);
                    if (point.X > maxX) maxX = point.X;

                    if (point.Y > maxY) maxY = point.Y;
                }

                Size result = default;
                if ((maxX > -1) & (maxY > -1))
                {
                    var size = new Size(maxX + SzPower.Width, maxY + SzPower.Height + PaddingY);
                    result = size;
                }
                else
                {
                    if (_vcCols != 5)
                    {
                        var point2 = PowerPosition(MidsContext.Character.CurrentBuild.LastPower);
                        var size = new Size(point2.X + SzPower.Width, point2.Y + SzPower.Height + PaddingY + OffsetInherent);
                        result = size;
                    }
                    else
                    {
                        var point2 = PowerPosition(MidsContext.Character.CurrentBuild.LastPower);
                        var size = new Size(point2.X + SzPower.Width, point2.Y + SzPower.Height + PaddingY + OffsetInherent);
                        result = size;
                    }
                }

                return result;
            }
        }
    }
}