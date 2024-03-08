using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static Mids_Reborn.Core.Enums;

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
        internal float ScaleValue { get; set; } = 2f;
        private bool Scaling { get; set; } = true;

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
        private Size _baseControlSize;
        private Font _defaultFont;
        public int Highlight;
        public eInterfaceMode InterfaceMode;
        private readonly bool _inDesigner = AppDomain.CurrentDomain.FriendlyName.Contains("devenv");

        //bool VillainColor;

        public ClsDrawX(Control iTarget)
        {
            InterfaceMode = 0;
            //VillainColor = false;
            _vcCols = 6;
            _vcRowsPowers = 24;
            BxPower = new List<ExtendedBitmap>();
            checked
            {
                ColorSwitch();
                InitColumns = MidsContext.Config.Columns;
                _cTarget = iTarget;
                _baseControlSize = iTarget.Size;
                InitializeAsync();
                _gTarget = iTarget.CreateGraphics();
                _gTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
                _gTarget.CompositingQuality = CompositingQuality.HighQuality;
                _gTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _gTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                _gTarget.SmoothingMode = SmoothingMode.HighQuality;
                _gTarget.CompositingMode = CompositingMode.SourceOver;
                _gTarget.PageUnit = GraphicsUnit.Pixel;
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

        public static bool EpicColumns => MidsContext.Character is { Archetype.ClassType: eClassType.HeroEpic };

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
                if (enhType == eType.SetO || enhType == eType.InventO)
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
                        if (!catalystSet & slot.Enhancement.RelativeLevel > eEnhRelative.Even &
                            MidsContext.Config.ShowEnhRel)
                        {
                            relativeLevelNumeric = GetRelativeString(slot.Enhancement.RelativeLevel, false);
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
                else if (enhType == eType.Normal || enhType == eType.SpecialO)
                {
                    var iValue2 = rect;
                    iValue2.Y -= 5f;
                    iValue2.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                    Color color;

                    if (slot.Enhancement.RelativeLevel == 0)
                    {
                        color = Color.Red;
                    }
                    else if (slot.Enhancement.RelativeLevel < eEnhRelative.Even)
                    {
                        color = Color.Yellow;
                    }
                    else if (slot.Enhancement.RelativeLevel > eEnhRelative.Even)
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
                    relativeString = GetRelativeString(slot.Enhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols);

                    // +3 SO do not exist ingame, at least not through combinations.
                    // Display flat level instead.
                    if (slot.Enhancement.RelativeLevel == eEnhRelative.PlusThree)
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
                        slot.Enhancement.RelativeLevel != eEnhRelative.None &&
                        slot.Enhancement.RelativeLevel != eEnhRelative.Even &&
                        slot.Enhancement.RelativeLevel != eEnhRelative.PlusThree
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

        public Point DrawPowerSlot(ref PowerEntry? powerEntry, bool singleDraw = false)
        {
            var drawVars = InitializeDrawVariables(powerEntry, singleDraw);
            UpdateSlotLocation(ref drawVars);
            DrawClipRectangle(drawVars);
            UpdatePowerState(ref drawVars, singleDraw);
            UpdateRectangleF(ref drawVars);
            UpdateImageAttributes(ref drawVars);
            DrawPowerComponents(drawVars);
            return drawVars.Location;
        }

        private void UpdateSlotLocation(ref DrawVariables drawVars)
        {
            drawVars.SlotLocation = drawVars.SlotLocation with
            {
                X = (int)Math.Round(drawVars.Location.X - OffsetX + checked(SzPower.Width - SzSlot.Width * 6) / 2.0),
                Y = drawVars.Location.Y + OffsetY
            };
        }

        private void DrawClipRectangle(DrawVariables drawVars)
        {
            var brush = new SolidBrush(_backColor);
            var clipRect = new Rectangle(drawVars.SlotLocation.X, drawVars.SlotLocation.Y, SzPower.Width, SzPower.Height);
            BxBuffer.Graphics.FillRectangle(brush, ScaleDown(clipRect));
        }

        private void UpdatePowerState(ref DrawVariables drawVars, bool singleDraw)
        {
            var toggling = InterfaceMode == eInterfaceMode.PowerToggle;
            if (!toggling)
            {
                if (drawVars.PowerEntry.Power != null)
                {
                    switch (singleDraw)
                    {
                        case true when drawVars is { SlotCheck: > -1, CanPlaceSlot: true } &&
                                       InterfaceMode != eInterfaceMode.PowerToggle &&
                                       drawVars.PowerEntry is { PowerSet: not null, Slots.Length: < 6 } &&
                                       drawVars.PowerEntry.Power.Slottable:
                            drawVars.PowerState = ePowerState.Open;
                            break;
                        default:
                        {
                            if (drawVars.PowerEntry.Chosen & !drawVars.CanPlaceSlot &
                                (InterfaceMode != eInterfaceMode.PowerToggle) & (Highlight ==
                                    MidsContext.Character.CurrentBuild.Powers.IndexOf(drawVars.PowerEntry)))
                            {
                                drawVars.PowerState = ePowerState.Open;
                            }

                            break;
                        }
                    }
                }
                else if (MidsContext.Character.CurrentBuild.Powers.IndexOf(drawVars.PowerEntry) == IndexFromLevel())
                {
                    drawVars.PowerState = ePowerState.Open;
                }
            }
        }

        private void UpdateRectangleF(ref DrawVariables drawVars)
        {
            drawVars.RectangleF = drawVars.RectangleF with { Height = SzSlot.Height, Width = SzSlot.Width };
            drawVars.StringFormat.Alignment = StringAlignment.Center;
            drawVars.StringFormat.LineAlignment = StringAlignment.Center;
        }

        private void UpdateImageAttributes(ref DrawVariables drawVars)
        {
            bool grey;
            ImageAttributes? imageAttr;
            var toggling = InterfaceMode == eInterfaceMode.PowerToggle;
            if (toggling)
            {
                drawVars.PowerState = drawVars.PowerState switch
                {
                    ePowerState.Open => ePowerState.Empty,
                    _ => drawVars.PowerState
                };
                switch (drawVars.PowerEntry.StatInclude & (drawVars.PowerState == ePowerState.Used))
                {
                    case true:
                        drawVars.PowerState = ePowerState.Open;
                        grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
                        imageAttr = GreySlot(grey, true);
                        break;
                    default:
                    {
                        if (drawVars.PowerEntry.CanIncludeForStats())
                        {
                            grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
                            imageAttr = GreySlot(grey);
                        }
                        else
                        {
                            imageAttr = GreySlot(true);
                            grey = true;
                        }

                        break;
                    }
                }
            }
            else
            {
                grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
                imageAttr = GreySlot(grey);
            }

            drawVars.PowerRect = DrawPowerImage(drawVars.PowerEntry, drawVars.Location, drawVars.PowerState, toggling,
                imageAttr, grey);
        }

        private void DrawPowerComponents(DrawVariables drawVars)
        {
            DrawToggles(drawVars.PowerEntry, drawVars.ToggleRect, drawVars.ProcRect, drawVars.PowerRect, drawVars.Pen2);
            DrawSlotsAndEnhancements(drawVars.PowerEntry, drawVars.RectangleF, drawVars.SlotLocation, drawVars.Pen,
                drawVars.Font);
            DrawNewSlotHover(drawVars.PowerEntry, drawVars.SlotLocation, drawVars.Font, drawVars.PowerState,
                drawVars.SlotCheck, drawVars.DrawNewSlot);
            DrawPowerText(drawVars.PowerEntry, drawVars.Location, drawVars.RectangleF, drawVars.Font, drawVars.Text,
                drawVars.Text2, drawVars.PowerState);
        }

        private DrawVariables InitializeDrawVariables(PowerEntry? powerEntry, bool singleDraw)
        {
            var drawVars = new DrawVariables
            {
                PowerEntry = powerEntry,
                Pen = new Pen(Color.FromArgb(128, 0, 0, 0), 1f),
                Text = string.Empty,
                Text2 = string.Empty,
                RectangleF = new RectangleF(0f, 0f, 0f, 0f),
                StringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
                {
                    Trimming = StringTrimming.None
                },
                Pen2 = new Pen(Color.Black),
                ToggleRect = default,
                ProcRect = default,
                SingleDraw = singleDraw,
                FontStyle = !MidsContext.Config.RtFont.PowersBold ? FontStyle.Regular : FontStyle.Bold
            };
            drawVars.Font = MidsContext.Config.RtFont.PowersBase > 0
                ? new Font(_defaultFont.FontFamily, FontScale(MidsContext.Config.RtFont.PowersBase), drawVars.FontStyle,
                    GraphicsUnit.Point, 0)
                : new Font(_defaultFont.FontFamily, FontScale(8), drawVars.FontStyle, GraphicsUnit.Point, 0);
            drawVars.SlotCheck = MidsContext.Character.SlotCheck(powerEntry);
            drawVars.PowerState = powerEntry.State;
            drawVars.CanPlaceSlot = MidsContext.Character.CanPlaceSlot;
            drawVars.DrawNewSlot = powerEntry.Power != null && powerEntry.State != ePowerState.Empty && drawVars.CanPlaceSlot &&
                              powerEntry.Slots.Length < 6 && singleDraw && powerEntry.Power.Slottable &
                              (InterfaceMode != eInterfaceMode.PowerToggle);
            drawVars.Location = PowerPosition(powerEntry);
            drawVars.SlotLocation = default;
            return drawVars;
        }

        private Rectangle DrawPowerImage(PowerEntry? iSlot, Point location, ePowerState ePowerState, bool toggling, ImageAttributes? imageAttr, bool grey)
        {
            var powerRect = new Rectangle(location.X, location.Y, BxPower[(int)ePowerState].Size.Width, BxPower[(int)ePowerState].Size.Height);
            var destRect = ScaleDown(powerRect);
            var width = BxPower[(int)ePowerState].ClipRect.Width;
            var clipRect2 = BxPower[(int)ePowerState].ClipRect;
            if (ePowerState == ePowerState.Used || toggling)
            {
                if (!MidsContext.Config.DisableDesaturateInherent & !iSlot.Chosen)
                    imageAttr = Desaturate(grey, ePowerState == ePowerState.Open);

                Image bitmap = !MidsContext.Character.IsHero() ? BxPower[4].Bitmap : BxPower[2].Bitmap;
                BxBuffer.Graphics.DrawImage(bitmap, destRect, 0, 0, width, clipRect2.Height, GraphicsUnit.Pixel, imageAttr);
            }
            else if (ePowerState == ePowerState.Open)
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

            return powerRect;
        }

        private void DrawToggles(PowerEntry powerEntry, Rectangle toggleRect, Rectangle procRect, Rectangle powerRect, Pen pen2)
        {
            if (powerEntry.CanIncludeForStats() && !powerEntry.HasProc())
            {
                var toggleGraphics = BxBuffer.Graphics;
                toggleRect.Height = 15;
                toggleRect.Width = toggleRect.Height;
                toggleRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - toggleRect.Height) / 2.0);
                toggleRect.X = (int)Math.Round(powerRect.Right -
                                               (toggleRect.Width + checked(powerRect.Height - toggleRect.Height) /
                                                   2.0));
                toggleRect = ScaleDown(toggleRect);
                var iCenter = new PointF(-0.25f, -0.33f);
                var brush2 = powerEntry.StatInclude
                    ? MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                    : MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

                toggleGraphics.FillEllipse(brush2, toggleRect);
                toggleGraphics.DrawEllipse(pen2, toggleRect);
            }
            
            if (powerEntry.HasProc() && !powerEntry.CanIncludeForStats())
            {
                //draw proc toggle
                var procGraphics = BxBuffer.Graphics;
                procRect.Height = 15;
                procRect.Width = procRect.Height;
                procRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - procRect.Height) / 2.0);
                procRect.X = (int)Math.Round(powerRect.Right -
                                             (procRect.Width + checked(powerRect.Height - procRect.Height) / 2.0));
                procRect = ScaleDown(procRect);
                var pCenter = new PointF(-0.25f, -0.33f);
                var brush3 = !powerEntry.ProcInclude
                    ? MakePathBrush(procRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                    : MakePathBrush(procRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                procGraphics.FillEllipse(brush3, procRect);
                procGraphics.DrawEllipse(pen2, procRect);
            }

            if (powerEntry.HasProc() && powerEntry.CanIncludeForStats())
            {
                //draw power toggle
                var toggleGraphics = BxBuffer.Graphics;
                toggleRect.Height = 15;
                toggleRect.Width = toggleRect.Height;
                toggleRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - toggleRect.Height) / 2.0);
                toggleRect.X = (int)Math.Round(powerRect.Right -
                                               (toggleRect.Width + checked(powerRect.Height - toggleRect.Height) /
                                                   3.0));
                toggleRect = ScaleDown(toggleRect);
                var iCenter = new PointF(-0.25f, -0.33f);
                var brush2 = powerEntry.StatInclude
                    ? MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                    : MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

                toggleGraphics.FillEllipse(brush2, toggleRect);
                toggleGraphics.DrawEllipse(pen2, toggleRect);

                //draw proc toggle
                var procGraphics = BxBuffer.Graphics;
                procRect.Height = 15;
                procRect.Width = procRect.Height;
                procRect.Y = (int)Math.Round(powerRect.Top + checked(powerRect.Height - procRect.Height) / 2.0);
                procRect.X = (int)Math.Round(powerRect.Right -
                                             (procRect.Width + checked(powerRect.Height - procRect.Height) / 1.0));
                procRect = ScaleDown(procRect);
                var pRect = procRect;
                var pCenter = new PointF(-0.25f, -0.33f);
                var brush3 = !powerEntry.ProcInclude
                    ? MakePathBrush(pRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                    : MakePathBrush(pRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
                procGraphics.FillEllipse(brush3, procRect);
                procGraphics.DrawEllipse(pen2, procRect);
            }
        }

        private void DrawSlotsAndEnhancements(PowerEntry powerEntry, RectangleF rectangleF, Point slotLocation, Pen pen, Font font)
        {
            for (var i = 0; i < powerEntry.Slots.Length; i++)
            {
                var slot = powerEntry.Slots[i];
                // Enhancement spacing and position?
                rectangleF.X = slotLocation.X + (SzSlot.Width + 2) * i;
                rectangleF.Y = slotLocation.Y;
                //
                SolidBrush solidBrush;
                if (slot.Enhancement.Enh < 0)
                {
                    var clipRect3 = new Rectangle((int)Math.Round(rectangleF.X), slotLocation.Y, SzSlot.Width,
                        SzSlot.Height); // New slot rectangle
                    BxBuffer.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, ScaleDown(clipRect3), 0, 0, SzSlot.Width,
                        SzSlot.Height, GraphicsUnit.Pixel, PImageAttributes);
                    if ((MidsContext.Config.CalcEnhLevel == 0) | (slot.Level > MidsContext.Config.ForceLevel) |
                        ((InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !powerEntry.StatInclude) |
                        (!powerEntry.AllowFrontLoading & (slot.Level < powerEntry.Level)))
                    {
                        solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                        BxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(rectangleF));
                        BxBuffer.Graphics.DrawEllipse(pen, ScaleDown(rectangleF));
                    }
                }
                else
                {
                    // Controls if powers or slots are greyed out
                    if (_inDesigner) continue;

                    var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                    var graphics6 = BxBuffer.Graphics;
                    var clipRect3 = new Rectangle((int)Math.Round(rectangleF.X), slotLocation.Y, SzSlot.Width,
                        SzSlot.Height);
                    I9Gfx.DrawEnhancementAt(ref graphics6, ScaleDown(clipRect3), enhancement.ImageIdx,
                        I9Gfx.ToGfxGrade(enhancement.TypeID, slot.Enhancement.Grade));
                    if ((slot.Enhancement.RelativeLevel == 0) | (slot.Level > MidsContext.Config.ForceLevel) |
                        ((InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !powerEntry.StatInclude) |
                        (!powerEntry.AllowFrontLoading & (slot.Level < powerEntry.Level)) |
                        (MidsContext.EnhCheckMode & !slot.Enhancement.Obtained))
                    {
                        solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                        var iValue3 = rectangleF;
                        iValue3.Inflate(1f, 1f);
                        BxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(iValue3));
                    }

                    if (slot.Enhancement.Enh > -1)
                        DrawEnhancementLevel(slot, font, BxBuffer.Graphics, ref rectangleF);
                }

                if (!MidsContext.Config.ShowSlotLevels) continue;

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
                    Convert.ToString(slot.Level + 1),
                    ScaleDown(powerTextRect),
                    Color.FromArgb(0, 255, 0),
                    Color.FromArgb(192, 0, 0, 0),
                    font,
                    1f,
                    BxBuffer.Graphics);
            }
        }

        private void DrawNewSlotHover(PowerEntry powerEntry, Point slotLocation, Font font, ePowerState powerState, int slotCheck, bool drawNewSlot)
        {
            if (slotCheck > -1 && powerState is not ePowerState.Empty && drawNewSlot)
            {
                var slotHoverRect = new RectangleF(slotLocation.X + (SzSlot.Width + 2) * powerEntry.Slots.Length, slotLocation.Y, SzSlot.Width, SzSlot.Height);
                BxBuffer.Graphics.DrawImage(BxNewSlot.Bitmap, ScaleDown(slotHoverRect));
                slotHoverRect.Height = _defaultFont.GetHeight(BxBuffer.Graphics);
                slotHoverRect.Y += (SzSlot.Height - slotHoverRect.Height) / 2f;
                DrawOutlineText(Convert.ToString(slotCheck + 1), ScaleDown(slotHoverRect), Color.FromArgb(0, 255, 255), Color.FromArgb(192, 0, 0, 0), font, 1f, BxBuffer.Graphics);
            }
        }

        private void DrawPowerText(PowerEntry powerEntry, Point location, RectangleF rectangleF, Font font, string text, string text2, ePowerState powerState)
        {
            var solidBrush = new SolidBrush(Color.White);
            var stringFormat = new StringFormat();
            rectangleF.X = location.X + 10;
            rectangleF.Y = location.Y + 4;
            rectangleF.Width = SzPower.Width;
            rectangleF.Height = _defaultFont.GetHeight() * 2f;
            var powerState2 = powerEntry.State;
            if ((powerState2 == ePowerState.Empty) & (powerState == ePowerState.Open))
            {
                powerState2 = powerState;
            }

            switch (powerState2)
            {
                case 0:
                    solidBrush = new SolidBrush(Color.Transparent);
                    text = "";
                    break;
                case ePowerState.Empty:
                    solidBrush = new SolidBrush(Color.WhiteSmoke);
                    text = $"({powerEntry.Level + 1})";
                    break;
                case ePowerState.Used:
                    if (powerEntry.PowerSet.SetType is ePowerSetType.Primary or ePowerSetType.Secondary
                        or ePowerSetType.Ancillary or ePowerSetType.Inherent
                        or ePowerSetType.Pool)
                        text2 = "";

                    solidBrush = !MidsContext.Character.IsHero()
                        ? new SolidBrush(Color.White)
                        : new SolidBrush(Color.Black);

                    text = powerEntry.Virtual
                        ? powerEntry.Name
                        : $"({powerEntry.Level + 1}) {powerEntry.Name} {text2}";
                    break;
                case ePowerState.Open:
                    solidBrush = new SolidBrush(Color.WhiteSmoke);
                    text = $"({powerEntry.Level + 1})";
                    break;
            }

            if ((powerState == Enums.ePowerState.Empty) & (powerEntry.State == ePowerState.Used))
                solidBrush = new SolidBrush(Color.WhiteSmoke);

            if (InterfaceMode == Enums.eInterfaceMode.PowerToggle && solidBrush.Color == Color.Black &&
                !powerEntry.CanIncludeForStats()) solidBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));

            stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
            if (MidsContext.Config.EnhanceVisibility)
            {
                var bounds5 = ScaleDown(rectangleF);
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
        }
        
        private class DrawVariables
        {
            public PowerEntry PowerEntry { get; set; }
            public Pen Pen { get; set; }
            public Pen Pen2 { get; set; }
            public RectangleF RectangleF { get; set; }
            public StringFormat StringFormat { get; set; }
            public FontStyle FontStyle { get; set; }
            public Font Font { get; set; }
            public int SlotCheck { get; set; }
            public ePowerState PowerState { get; set; }
            public bool CanPlaceSlot { get; set; }
            public bool DrawNewSlot { get; set; }
            public Point Location { get; set; }
            public Rectangle PowerRect { get; set; }
            public Rectangle ToggleRect { get; set; }
            public Rectangle ProcRect { get; set; }
            public Point SlotLocation { get; set; }
            public string Text { get; set; }
            public string Text2 { get; set; }
            public bool SingleDraw { get; set; }
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
                if (DatabaseAPI.Database.Powersets[nidPowerset].SetType == ePowerSetType.Inherent)
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
                            if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != ePowerSetType.Inherent)
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
                        if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != ePowerSetType.Inherent)
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

        public static void DrawOutlineText(string iStr, RectangleF bounds, Color textColor, Color outlineColor, Font bFont, float outlineSpace, Graphics g, bool smallMode = false, bool leftAlign = false)
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
                    var point = MidsContext.Character.CurrentBuild.Powers[i] != null &&
                            (MidsContext.Character.CurrentBuild.Powers[i].Power == null ||
                             MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                        ? PowerPosition(GetVisualIdx(i))
                        : PowerPosition(i);
                    
                    if (iX >= point.X && iY >= point.Y && iX < SzPower.Width + point.X && iY < point.Y + SzPower.Height + PaddingY / 2)
                    {
                        return i;
                    }
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
                    point = MidsContext.Character.CurrentBuild.Powers[i] != null &&
                            (MidsContext.Character.CurrentBuild.Powers[i]?.Power == null ||
                             MidsContext.Character.CurrentBuild.Powers[i]?.Chosen == true)
                        ? PowerPosition(GetVisualIdx(i))
                        : PowerPosition(i);

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

                var isValid = iY >= point.Y + OffsetY &&
                               MidsContext.Character.CurrentBuild.Powers[oPower] != null &&
                               MidsContext.Character.CurrentBuild.Powers[oPower]?.NIDPowerset > -1 &&
                               DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[oPower].NIDPowerset]
                                   .Powers[MidsContext.Character.CurrentBuild.Powers[oPower].IDXPower].Slottable;

                if (!isValid)
                {
                    return -1;
                }

                var column = SzPower.Width + PaddingX == 0
                    ? 0
                    : (int)Math.Floor(iX / (decimal)(SzPower.Width + PaddingX));
                iX -= column * (SzPower.Width + PaddingX); // Remove column x offset

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
                    if (Highlight == idx && !force)
                    {
                        return false;
                    }

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
            var needsScaling = drawingArea.Width > iSize.Width || drawingArea.Height > iSize.Height;
            Scaling = needsScaling;

            if (needsScaling)
            {
                // Calculate scale factor based on DPI and control size
                var dpiScale = Math.Max(BxBuffer.Graphics.DpiX, BxBuffer.Graphics.DpiY) / 96f;
                ScaleValue = Math.Max(
                    drawingArea.Width / (float)iSize.Width,
                    drawingArea.Height / (float)iSize.Height
                ) * dpiScale;

                SetGraphicsQuality(); // Refactored method to set graphics quality

                if (ScaleValue != origScaleValue)
                {
                    FullRedraw();
                }
            }
            else
            {
                BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                ScaleValue = 1f;

                ResetTarget();

                if (origScaling != Scaling || origScaleValue != ScaleValue)
                {
                    FullRedraw();
                }
            }
        }
        
        private void SetGraphicsQuality()
        {
            BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
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
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
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
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
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
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
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
                    if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
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

            if (MidsContext.Character.Archetype.ClassType == eClassType.HeroEpic)
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

        // private Point CRtoXy(int iCol, int iRow, bool ignorePadding = false)
        // {
        //     // Convert a column/row location to the top left XY co-ord of a power entry
        //     // 3 Columns, 15 Rows
        //     return new Point(
        //         iCol * (SzPower.Width + PaddingX * (ignorePadding ? 0 : 1)),
        //         iRow * (SzPower.Height + (PaddingY - (ignorePadding ? (int)Math.Round(5 / ScaleValue) : 0))) + (iRow >= _vcRowsPowers ? OffsetInherent : 0));
        // }
        
        private Point CRtoXy(int iCol, int iRow, bool ignorePadding = false)
        {
            int dynamicPaddingX = DynamicPaddingX();
            int dynamicPaddingY = DynamicPaddingY();

            return new Point(
                iCol * (SzPower.Width + dynamicPaddingX * (ignorePadding ? 0 : 1)),
                iRow * (SzPower.Height + dynamicPaddingY - (ignorePadding ? (int)Math.Round(5 / ScaleValue) : 0)) + 
                (iRow >= _vcRowsPowers ? OffsetInherent : 0));
        }
        
        private int DynamicPaddingX()
        {
            // Calculate scaling factor based on width
            double scaleX = (double)_cTarget.Width / _baseControlSize.Width;
            // Apply scaling factor to original padding, ensuring it doesn't go below the original value
            return Math.Max(PaddingX, (int)(PaddingX * scaleX));
        }

        private int DynamicPaddingY()
        {
            // Calculate scaling factor based on height
            double scaleY = (double)_cTarget.Height / _baseControlSize.Height;
            // Apply scaling factor to original padding, ensuring it doesn't go below the original value
            return Math.Max(PaddingY, (int)(PaddingY * scaleY));
        }

        public Size GetDrawingArea()
        {
            var result = (Size)PowerPosition(VcPowers - 1);
            int dynamicPaddingX = DynamicPaddingX();
            int dynamicPaddingY = DynamicPaddingY();
            checked
            {
                result.Width += SzPower.Width + dynamicPaddingX;
                result.Height = result.Height + SzPower.Height + dynamicPaddingY;
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