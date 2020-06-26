using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace midsControls
{
    public class clsDrawX
    {
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

        int InitColumns
        {
            set
            {
                if (value == vcCols)
                    return;
                if (value < 2 | value > 4)
                    return;
                vcCols = value;
                vcRowsPowers = checked((int) Math.Round(24.0 / vcCols));
            }
        }

        public Size SzPower { get; set; }

        public void ReInit(Control iTarget)
        {
            gTarget = iTarget.CreateGraphics();
            cTarget = iTarget;
            //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
            DefaultFont = new Font("Arial", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);
            BackColor = iTarget.BackColor;
        }

        public clsDrawX(Control iTarget)
        {
            InterfaceMode = 0;
            //VillainColor = false;
            ScaleValue = 2f;
            Scaling = true;
            vcCols = 4;
            vcRowsPowers = 8;
            bxPower = new ExtendedBitmap[4];
            checked
            {
                int num2 = bxPower.Length - 1;
                for (int i = 0; i <= num2; i++)
                {
                    bxPower[i] = new ExtendedBitmap(FileIO.AddSlash(Application.StartupPath) + GfxPath + GfxPowerFn +
                                                    Strings.Trim(Convert.ToString(i)) + GfxFileExt);
                }

                ColourSwitch();
                InitColumns = MidsContext.Config.Columns;
                SzPower = bxPower[0].Size;
                szSlot = new Size(30, 30);
                szBuffer = GetMaxDrawingArea();
                Size size = new Size(szBuffer.Width, szBuffer.Height);
                bxBuffer = new ExtendedBitmap(size);
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                bxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                bxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                szBuffer = GetRequiredDrawingArea();
                bxNewSlot = new ExtendedBitmap(FileIO.AddSlash(Application.StartupPath) + GfxPath + "Addslot.png");
                gTarget = iTarget.CreateGraphics();
                cTarget = iTarget;
                gTarget.CompositingMode = CompositingMode.SourceCopy;
                gTarget.CompositingQuality = CompositingQuality.HighQuality;
                gTarget.InterpolationMode = InterpolationMode.HighQualityBilinear;
                gTarget.SmoothingMode = SmoothingMode.HighQuality;
                gTarget.TextRenderingHint = TextRenderingHint.SystemDefault;
                //DefaultFont = new Font(iTarget.Font.FontFamily, iTarget.Font.Size, FontStyle.Bold, iTarget.Font.Unit);
                DefaultFont = new Font("Segoe UI", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel);
                BackColor = iTarget.BackColor;
                if (szBuffer.Height < cTarget.Height)
                {
                    gTarget.FillRectangle(new SolidBrush(BackColor), 0, szBuffer.Height, cTarget.Width, cTarget.Height - szBuffer.Height);
                }
            }
        }

        void DrawSplit()
        {
            Pen pen = new Pen(Color.Goldenrod, 2f);
            checked
            {
                int iValue = 4 + vcRowsPowers * (SzPower.Height + 19) + 1;
                bxBuffer.Graphics.DrawLine(pen, 2, ScaleDown(iValue), ScaleDown(PowerPosition(15).X + SzPower.Width + 195), ScaleDown(iValue));
            }
        }

        void DrawPowers()
        {
            checked
            {
                for (int i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    if (MidsContext.Character.CanPlaceSlot & Highlight == i)
                    {
                        Build currentBuild = MidsContext.Character.CurrentBuild;
                        List<PowerEntry> powers = currentBuild.Powers;
                        int index = i;
                        PowerEntry value = powers[index];
                        DrawPowerSlot(ref value, true);
                        currentBuild.Powers[index] = value;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                    {
                        Build currentBuild = MidsContext.Character.CurrentBuild;
                        List<PowerEntry> powers2 = currentBuild.Powers;
                        int index = i;
                        PowerEntry value = powers2[index];
                        DrawPowerSlot(ref value);
                        currentBuild.Powers[index] = value;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[i].Power != null &&
                             (Operators.CompareString(MidsContext.Character.CurrentBuild.Powers[i].Power.GroupName, "Incarnate", false) == 0 |
                              MidsContext.Character.CurrentBuild.Powers[i].Power.IncludeFlag))
                    {
                        Build currentBuild = MidsContext.Character.CurrentBuild;
                        List<PowerEntry> powers3 = currentBuild.Powers;
                        int index = i;
                        PowerEntry value = powers3[index];
                        DrawPowerSlot(ref value);
                        currentBuild.Powers[index] = value;
                    }
                }

                Application.DoEvents();
                DrawSplit();
            }
        }

        float FontScale(float iSZ) => Math.Min(ScaleDown(iSZ) * 1.1f, iSZ);

        static int IndexFromLevel()
            => MidsContext.Character.CurrentBuild.Powers.FindIndex(pow => pow?.Level == MidsContext.Character.RequestedLevel);

        void DrawEnhancement(SlotEntry slot, Font font, Graphics g, ref RectangleF rect)
        {
            unchecked
            {
                Enums.eType enhType = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID;
                if (!MidsContext.Config.I9.HideIOLevels && (enhType == Enums.eType.SetO || enhType == Enums.eType.InventO))
                {
                    RectangleF iValue2 = rect;
                    iValue2.Y -= 3f;
                    iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                    string relativeLevelNumeric;
                    string enhInternalName = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].UID;
                    // Catalysed enhancements take character level no matter what.
                    // Game does not allow boosters over enhancement catalysts.
                    if (DatabaseAPI.EnhHasCatalyst(enhInternalName) || DatabaseAPI.EnhIsNaturallyAttuned(slot.Enhancement.Enh))
                    {
                        relativeLevelNumeric = string.Empty;
                    }
                    else
                    {
                        relativeLevelNumeric = slot.Enhancement.RelativeLevel switch
                        {
                            // Zed - need to move this in Enums.
                            Enums.eEnhRelative.PlusOne => "+1",
                            Enums.eEnhRelative.PlusTwo => "+2",
                            Enums.eEnhRelative.PlusThree => "+3",
                            Enums.eEnhRelative.PlusFour => "+4",
                            Enums.eEnhRelative.PlusFive => "+5",
                            _ => string.Empty
                        };
                    }

                    // If enhancement has boosters, need to stretch the level drawing zone a little.
                    // Or relative level doesn't fit in.
                    if (!string.IsNullOrEmpty(relativeLevelNumeric))
                    {
                        iValue2.Width += 10f;
                        iValue2.X -= 5f;
                    }
                    string iStr = Convert.ToString(checked(slot.Enhancement.IOLevel + 1)) + relativeLevelNumeric;
                    RectangleF bounds = ScaleDown(iValue2);
                    Color cyan = Color.Cyan;
                    Color outline = Color.FromArgb(128, 0, 0, 0);
                    float outlineSpace = 1f;
                    g = bxBuffer.Graphics;
                    DrawOutlineText(iStr, bounds, cyan, outline, font, outlineSpace, g);
                }
                else if (MidsContext.Config.ShowEnhRel && (enhType == Enums.eType.Normal || enhType == Enums.eType.SpecialO))
                {
                    RectangleF iValue2 = rect;
                    iValue2.Y -= 3f;
                    iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
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
                        color = Color.FromArgb(0, 255, 255);
                    }
                    else
                    {
                        color = Color.White;
                    }

                    string relativeString = Enums.GetRelativeString(slot.Enhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols);
                    RectangleF bounds2 = ScaleDown(iValue2);
                    Color text3 = color;
                    Color outline2 = Color.FromArgb(128, 0, 0, 0);
                    Font bFont2 = font;
                    float outlineSpace2 = 1f;
                    DrawOutlineText(relativeString, bounds2, text3, outline2, bFont2, outlineSpace2, g);
                }
            }
        }

        public Point DrawPowerSlot(ref PowerEntry iSlot, bool singleDraw = false)
        {
            Pen pen = new Pen(Color.FromArgb(128, 0, 0, 0), 1f);
            string text = string.Empty;
            string text2 = string.Empty;
            RectangleF rectangleF = new RectangleF(0f, 0f, 0f, 0f);
            var stringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
            {
                Trimming = StringTrimming.None
            };
            Pen pen2 = new Pen(Color.Black);
            Rectangle rectangle = default;
            //Font font = new Font(DefaultFont.FontFamily, FontScale(DefaultFont.SizeInPoints), DefaultFont.Style, GraphicsUnit.Point);
            Font font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            int slotChk = MidsContext.Character.SlotCheck(iSlot);
            Enums.ePowerState ePowerState = iSlot.State;
            bool canPlaceSlot = MidsContext.Character.CanPlaceSlot;
            bool drawNewSlot = iSlot.Power != null && (iSlot.State != Enums.ePowerState.Empty && canPlaceSlot) && iSlot.Slots.Length < 6 &&
                               singleDraw && iSlot.Power.Slottable & InterfaceMode != Enums.eInterfaceMode.PowerToggle;
            Point result = PowerPosition(iSlot);
            Point point = default;
            checked
            {
                point.X = (int) Math.Round(result.X + (checked(SzPower.Width - (szSlot.Width * 6)) / 2.0));
                point.Y = result.Y + 18;
                Graphics graphics = bxBuffer.Graphics;
                Brush brush = new SolidBrush(BackColor);
                Rectangle clipRect = new Rectangle(point.X, point.Y, SzPower.Width, SzPower.Height);
                graphics.FillRectangle(brush, ScaleDown(clipRect));
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
                        else if (iSlot.Chosen & !canPlaceSlot & InterfaceMode != Enums.eInterfaceMode.PowerToggle &
                                 Highlight == MidsContext.Character.CurrentBuild.Powers.IndexOf(iSlot))
                        {
                            ePowerState = Enums.ePowerState.Open;
                        }
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers.IndexOf(iSlot) == IndexFromLevel())
                    {
                        ePowerState = Enums.ePowerState.Open;
                    }
                }

                rectangleF.Height = szSlot.Height;
                rectangleF.Width = szSlot.Width;
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                bxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                bool grey;
                ImageAttributes imageAttr;
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
                        grey = (iSlot.Level >= MidsContext.Config.ForceLevel);
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

                Rectangle iValue = new Rectangle(result.X, result.Y, bxPower[(int) ePowerState].Size.Width,
                    bxPower[(int) ePowerState].Size.Height);
                if (ePowerState == Enums.ePowerState.Used || toggling)
                {
                    if (!MidsContext.Config.DisableDesaturateInherent & !iSlot.Chosen)
                    {
                        imageAttr = Desaturate(grey, ePowerState == Enums.ePowerState.Open);
                    }

                    Graphics graphics2 = bxBuffer.Graphics;
                    Image bitmap = bxPower[(int) ePowerState].Bitmap;
                    Rectangle destRect = ScaleDown(iValue);
                    int srcX = 0;
                    int srcY = 0;
                    int width = bxPower[(int) ePowerState].ClipRect.Width;
                    Rectangle clipRect2 = bxPower[(int) ePowerState].ClipRect;
                    graphics2.DrawImage(bitmap, destRect, srcX, srcY, width, clipRect2.Height, GraphicsUnit.Pixel, imageAttr);
                }
                else
                {
                    Graphics graphics3 = bxBuffer.Graphics;
                    Image bitmap2 = bxPower[(int) ePowerState].Bitmap;
                    Rectangle destRect2 = ScaleDown(iValue);
                    int srcX2 = 0;
                    int srcY2 = 0;
                    int width2 = bxPower[(int) ePowerState].ClipRect.Width;
                    clipRect = bxPower[(int) ePowerState].ClipRect;
                    graphics3.DrawImage(bitmap2, destRect2, srcX2, srcY2, width2, clipRect.Height, GraphicsUnit.Pixel);
                }

                if (iSlot.CanIncludeForStats())
                {
                    rectangle.Height = 15;
                    rectangle.Width = rectangle.Height;
                    rectangle.Y = (int) Math.Round(iValue.Top + checked(iValue.Height - rectangle.Height) / 2.0);
                    rectangle.X = (int) Math.Round(iValue.Right - (rectangle.Width + checked(iValue.Height - rectangle.Height) / 2.0));
                    rectangle = ScaleDown(rectangle);
                    PathGradientBrush brush2;
                    if (iSlot.StatInclude)
                    {
                        Rectangle iRect = rectangle;
                        PointF iCenter = new PointF(-0.25f, -0.33f);
                        brush2 = MakePathBrush(iRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0));
                    }
                    else
                    {
                        Rectangle iRect2 = rectangle;
                        PointF iCenter = new PointF(-0.25f, -0.33f);
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
                    rectangleF.X = point.X + szSlot.Width * i;
                    rectangleF.Y = point.Y;
                    if (slot.Enhancement.Enh < 0)
                    {
                        Rectangle clipRect2 = new Rectangle((int) Math.Round(rectangleF.X), point.Y, 30, 30);
                        bxBuffer.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, ScaleDown(clipRect2), 0, 0, 30, 30, GraphicsUnit.Pixel,
                            pImageAttributes);
                        if (MidsContext.Config.CalcEnhLevel == 0 | slot.Level >= MidsContext.Config.ForceLevel |
                            (InterfaceMode == Enums.eInterfaceMode.PowerToggle & !iSlot.StatInclude) |
                            (!iSlot.AllowFrontLoading & slot.Level < iSlot.Level))
                        {
                            solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                            bxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(rectangleF));
                            bxBuffer.Graphics.DrawEllipse(pen, ScaleDown(rectangleF));
                        }
                    }
                    else
                    {
                        if (inDesigner) continue;
                        IEnhancement enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                        Graphics graphics5 = bxBuffer.Graphics;
                        Rectangle clipRect2 = new Rectangle((int) Math.Round(rectangleF.X), point.Y, 30, 30);
                        I9Gfx.DrawEnhancementAt(ref graphics5, ScaleDown(clipRect2), enhancement.ImageIdx,
                            I9Gfx.ToGfxGrade(enhancement.TypeID, slot.Enhancement.Grade));
                        if (slot.Enhancement.RelativeLevel == 0 | slot.Level >= MidsContext.Config.ForceLevel |
                            (InterfaceMode == Enums.eInterfaceMode.PowerToggle & !iSlot.StatInclude) |
                            (!iSlot.AllowFrontLoading & slot.Level < iSlot.Level))
                        {
                            solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                            RectangleF iValue2 = rectangleF;
                            iValue2.Inflate(1f, 1f);
                            bxBuffer.Graphics.FillEllipse(solidBrush, ScaleDown(iValue2));
                        }

                        if (slot.Enhancement.Enh > -1)
                            DrawEnhancement(slot, font, graphics, ref rectangleF);
                    }

                    if (!MidsContext.Config.ShowSlotLevels)
                        continue;
                    {
                        RectangleF iValue2 = rectangleF;
                        unchecked
                        {
                            iValue2.Y += iValue2.Height;
                            iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                            iValue2.Y -= iValue2.Height;
                        }

                        Graphics graphics5 = bxBuffer.Graphics;
                        DrawOutlineText(
                            iStr: Convert.ToString(slot.Level + 1),
                            bounds: ScaleDown(iValue2),
                            textColor: Color.FromArgb(0, 255, 0),
                            outlineColor: Color.FromArgb(192, 0, 0, 0),
                            bFont: font,
                            outlineSpace: 1f,
                            g: graphics5);
                    }
                }

                if (slotChk > -1 && (ePowerState != Enums.ePowerState.Empty && drawNewSlot))
                {
                    Rectangle clipRect2 = new Rectangle(point.X + szSlot.Width * (iSlot.Slots.Length), point.Y, szSlot.Width, szSlot.Height);
                    RectangleF iValue2 = clipRect2;
                    bxBuffer.Graphics.DrawImage(bxNewSlot.Bitmap, ScaleDown(iValue2));
                    iValue2.Height = DefaultFont.GetHeight(bxBuffer.Graphics);
                    iValue2.Y += (szSlot.Height - iValue2.Height) / 2f;
                    DrawOutlineText(Convert.ToString(slotChk + 1), ScaleDown(iValue2),
                        Color.FromArgb(0, 255, 255), Color.FromArgb(192, 0, 0, 0), font, 1f, bxBuffer.Graphics);
                }

                solidBrush = new SolidBrush(Color.Black);
                stringFormat = new StringFormat();
                rectangleF.X = result.X + 10;
                rectangleF.Y = result.Y + 4;
                rectangleF.Width = SzPower.Width;
                rectangleF.Height = DefaultFont.GetHeight() * 2f;
                Enums.ePowerState ePowerState2 = iSlot.State;
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
                    string iStr4 = text;
                    RectangleF bounds5 = ScaleDown(rectangleF);
                    Color whiteSmoke = Color.WhiteSmoke;
                    Color outline5 = Color.FromArgb(192, 0, 0, 0);
                    Font bFont5 = font;
                    float outlineSpace5 = 1f;
                    Graphics graphics5 = bxBuffer.Graphics;
                    graphics5.CompositingQuality = CompositingQuality.HighQuality;
                    graphics5.SmoothingMode = SmoothingMode.HighQuality;
                    graphics5.TextRenderingHint = TextRenderingHint.SystemDefault;
                    graphics5.PageUnit = GraphicsUnit.Pixel;
                    graphics5.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    DrawOutlineText(iStr4, bounds5, whiteSmoke, outline5, bFont5, outlineSpace5, graphics5, false, true);
                }
                else
                {
                    bxBuffer.Graphics.DrawString(text, font, solidBrush, ScaleDown(rectangleF), stringFormat);
                }

                return result;
            }
        }

        PathGradientBrush MakePathBrush(Rectangle iRect, PointF iCenter, Color iColor1, Color icolor2)
        {
            float num = (float) (iRect.Left + iRect.Width * 0.5);
            float num2 = (float) (iRect.Top + iRect.Height * 0.5);
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(iRect);
            PathGradientBrush pathGradientBrush;
            PathGradientBrush pathGradientBrush2;
            checked
            {
                Color[] array = new Color[graphicsPath.PathPoints.GetUpperBound(0) + 1];
                int lowerBound = graphicsPath.PathPoints.GetLowerBound(0);
                int upperBound = graphicsPath.PathPoints.GetUpperBound(0);
                for (int i = lowerBound; i <= upperBound; i++)
                {
                    array[i] = icolor2;
                }

                pathGradientBrush = new PathGradientBrush(graphicsPath)
                {
                    CenterColor = iColor1, SurroundColors = array
                };
                pathGradientBrush2 = pathGradientBrush;
            }

            PointF centerPoint = new PointF((float) (num + (iCenter.X + iCenter.X * (iRect.Width * 0.5))),
                (float) (num2 + (iCenter.Y + iCenter.Y * (iRect.Height * 0.5))));
            pathGradientBrush2.CenterPoint = centerPoint;
            return pathGradientBrush;
        }

        public void FullRedraw()
        {
            ColourSwitch();
            BackColor = cTarget.BackColor;
            bxBuffer.Graphics.Clear(BackColor);
            DrawPowers();
            Point location = new Point(0, 0);
            OutputUnscaled(ref bxBuffer, location);
            GC.Collect();
        }

        public void Refresh(Rectangle Clip)
            => OutputRefresh(Clip, Clip, GraphicsUnit.Pixel);

        int GetVisualIDX(int PowerIndex)
        {
            int nidpowerset = MidsContext.Character.CurrentBuild.Powers[PowerIndex].NIDPowerset;
            int idxpower = MidsContext.Character.CurrentBuild.Powers[PowerIndex].IDXPower;
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
                        for (int i = 0; i <= PowerIndex; i++)
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset > -1)
                            {
                                if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType !=
                                    Enums.ePowerSetType.Inherent)
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
                    for (int i = 0; i <= PowerIndex; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset > -1)
                        {
                            if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType !=
                                Enums.ePowerSetType.Inherent)
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
        }

        public static void DrawOutlineText(string iStr, RectangleF bounds, Color textColor, Color outlineColor, Font bFont, float outlineSpace,
            Graphics g, bool smallMode = false, bool leftAlign = false)
        {
            StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap)
            {
                LineAlignment = StringAlignment.Near, Alignment = leftAlign ? StringAlignment.Near : StringAlignment.Center
            };

            SolidBrush brush = new SolidBrush(outlineColor);
            RectangleF layoutRectangle = bounds;
            RectangleF layoutRectangle2 = new RectangleF(layoutRectangle.X, layoutRectangle.Y, layoutRectangle.Width, bFont.GetHeight(g));
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
                for (int i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    Point point;
                    if (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                        point = PowerPosition(GetVisualIDX(i));
                    else
                        point = PowerPosition(i);
                    if (iX >= point.X && iY >= point.Y && iX < SzPower.Width + point.X && iY < point.Y + SzPower.Height + 17)
                        return i;
                }

                return -1;
            }
        }

        public int WhichEnh(int iX, int iY)
        {
            int oPower = -1;
            checked
            {
                Point point = default;
                for (int i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                    {
                        point = PowerPosition(GetVisualIDX(i));
                    }
                    else
                    {
                        point = PowerPosition(i);
                    }

                    if ((iX < point.X || iY < point.Y) || (iX >= SzPower.Width + point.X || iY >= point.Y + SzPower.Height + 17))
                        continue;
                    oPower = i;
                    break;
                }

                if (oPower <= -1)
                    return -1;
                {
                    bool isValid = false;
                    if (iY >= point.Y + 18)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[oPower].NIDPowerset > -1)
                        {
                            if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[oPower].NIDPowerset]
                                .Powers[MidsContext.Character.CurrentBuild.Powers[oPower].IDXPower].Slottable)
                            {
                                isValid = true;
                            }
                        }
                        else
                        {
                            isValid = true;
                        }
                    }

                    if (!isValid)
                        return -1;
                    iX = (int) Math.Round(iX - (point.X + checked(SzPower.Width - szSlot.Width * 6) / 2.0));
                    for (int i = 0; i <= MidsContext.Character.CurrentBuild.Powers[oPower].Slots.Length - 1; i++)
                    {
                        if (iX <= (i + 1) * szSlot.Width)
                        {
                            return i;
                        }
                    }

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
                            List<PowerEntry> powers = currentBuild.Powers;
                            int highlight = Highlight;
                            value = powers[highlight];
                            Point point = DrawPowerSlot(ref value);
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
                        Point point3 = DrawPowerSlot(ref value, true);
                        currentBuild.Powers[idx] = value;
                        point2 = point3;
                        iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + 17);
                        rectangle = ScaleDown(iValue);
                        DrawSplit();
                        Output(ref bxBuffer, rectangle, rectangle, GraphicsUnit.Pixel);
                    }
                    else if (Highlight != -1)
                    {
                        Build currentBuild = MidsContext.Character.CurrentBuild;
                        List<PowerEntry> powers2 = currentBuild.Powers;
                        int highlight = Highlight;
                        PowerEntry value = powers2[highlight];
                        Point point4 = DrawPowerSlot(ref value);
                        currentBuild.Powers[highlight] = value;
                        Point point2 = point4;
                        Rectangle iValue = new Rectangle(point2.X, point2.Y, SzPower.Width, SzPower.Height + 17);
                        Rectangle rectangle = ScaleDown(iValue);
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

        void Blank()
        {
            bxBuffer?.Graphics.Clear(BackColor);
        }

        public void SetScaling(Size iSize)
        {
            bool scaleEnabled = Scaling;
            float scaleValue = ScaleValue;
            if (iSize.Width < 10 | iSize.Height < 10)
                return;
            Size drawingArea = GetDrawingArea();
            if (drawingArea.Width > iSize.Width | drawingArea.Height > iSize.Height)
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
                bxBuffer.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                ScaleValue = 1f;
                ResetTarget();
                Scaling = false;
                if (scaleEnabled != Scaling | Math.Abs(scaleValue - ScaleValue) > float.Epsilon)
                {
                    FullRedraw();
                }
            }
        }

        void ResetTarget()
        {
            bxBuffer.Graphics.TextRenderingHint = ScaleValue > 1.125 ? TextRenderingHint.SystemDefault : TextRenderingHint.SystemDefault;
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

        float ScaleDown(float iValue)
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

        RectangleF ScaleDown(RectangleF iValue)
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

        void Output(ref ExtendedBitmap Buffer, Rectangle DestRect, Rectangle SrcRect, GraphicsUnit iUnit)
            => gTarget.DrawImage(Buffer.Bitmap, DestRect, SrcRect, iUnit);

        void OutputRefresh(Rectangle DestRect, Rectangle SrcRect, GraphicsUnit iUnit)
            => gTarget.DrawImage(bxBuffer.Bitmap, DestRect, SrcRect, iUnit);

        void OutputUnscaled(ref ExtendedBitmap Buffer, Point Location)
            => gTarget.DrawImageUnscaled(Buffer.Bitmap, Location);

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

        static readonly float[][] villainMatrix =
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

        public void ColourSwitch()
        {
            /*bool useHeroColors = true;
            if (MidsContext.Character != null)
                useHeroColors = MidsContext.Character.IsHero();
            if (MidsContext.Config.DisableVillainColours)
                useHeroColors = true;

            VillainColor = !useHeroColors;*/
            pColorMatrix = new ColorMatrix(heroMatrix);
            if (pImageAttributes == null)
                pImageAttributes = new ImageAttributes();
            pImageAttributes.SetColorMatrix(pColorMatrix);
        }

        public static ImageAttributes GetRecolourIa(bool hero)
        {
            var colorMatrix = new ColorMatrix(heroMatrix);
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            return imageAttributes;
        }

        ImageAttributes GreySlot(bool grey, bool bypassIa = false)
        {
            if (!grey)
            {
                return bypassIa ? new ImageAttributes() : pImageAttributes;
            }

            checked
            {
                ColorMatrix colorMatrix = new ColorMatrix(heroMatrix);
                int r = 0;
                do
                {
                    int c = 0;
                    do
                    {
                        if (!bypassIa)
                        {
                            colorMatrix[r, c] = pColorMatrix[r, c];
                        }

                        if (r != 4)
                        {
                            colorMatrix[r, c] = (float) (colorMatrix[r, c] / 1.5);
                        }

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                return imageAttributes;
            }
        }

        ImageAttributes Desaturate(bool Grey, bool BypassIA = false)
        {
            ColorMatrix tMM = new ColorMatrix(new[]
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
            ColorMatrix tCM = new ColorMatrix(heroMatrix);
            int r = 0;
            checked
            {
                do
                {
                    int c = 0;
                    do
                    {
                        if (!BypassIA)
                        {
                            tCM[r, c] = (pColorMatrix[r, c] + tMM[r, c]) / 2f;
                        }

                        if (Grey && r != 4)
                        {
                            tCM[r, c] = (float) (tCM[r, c] / 1.5);
                        }

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(tCM);
                return imageAttributes;
            }
        }

        public Rectangle PowerBoundsUnScaled(int hIdx)
        {
            Rectangle rectangle = new Rectangle(0, 0, 1, 1);
            checked
            {
                Rectangle result;
                if (hIdx < 0 | hIdx > MidsContext.Character.CurrentBuild.Powers.Count - 1)
                {
                    result = rectangle;
                }
                else
                {
                    if (!MidsContext.Character.CurrentBuild.Powers[hIdx].Chosen &&
                        MidsContext.Character.CurrentBuild.Powers[hIdx].Power != null)
                    {
                        rectangle.Location = PowerPosition(hIdx);
                    }
                    else
                    {
                        rectangle.Location = PowerPosition(GetVisualIDX(hIdx));
                    }

                    rectangle.Width = SzPower.Width;
                    int num = rectangle.Y + 18;
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
            return (e.X >= pBounds.Left && e.X < pBounds.Right) && (e.Y >= pBounds.Top && e.Y < pBounds.Bottom);
        }

        Point PowerPosition(int powerEntryIdx)
        {
            return PowerPosition(MidsContext.Character.CurrentBuild.Powers[powerEntryIdx]);
        }

        int[][] GetInherentGrid()
        {
            switch (vcCols)
            {
                case 2:
                    if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                    {
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
                    }

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
                    {
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
                    }

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
            }

            if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
            {
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
            int powerIdx = MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry);
            checked
            {
                if (powerIdx == -1)
                {
                    int num2 = 0;
                    int num3 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                    for (int i = num2; i <= num3; i++)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[i].Power.PowerIndex != powerEntry.Power.PowerIndex ||
                            MidsContext.Character.CurrentBuild.Powers[i].Level != powerEntry.Level)
                            continue;
                        powerIdx = i;
                        break;
                    }
                }

                int[][] inherentGrid = GetInherentGrid();
                bool flag = false;
                int iRow = 0;
                int iCol = 0;
                if (!powerEntry.Chosen)
                {
                    if (displayLocation == -1 && powerEntry.Power != null)
                    {
                        switch (vcCols)
                        {
                            case 2:
                                if (powerEntry.Power.GroupName.Equals("Inherent") && MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Brawl"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("Sprint"))
                                    {
                                        powerEntry.Power.DisplayLocation = 4;
                                    }
                                    else if (powName.Equals("Rest"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("Swift"))
                                    {
                                        powerEntry.Power.DisplayLocation = 1;
                                    }
                                    else if (powName.Equals("Health"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("Hurdle"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Stamina"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("prestige_DVD_Glidep"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("prestige_BestBuy_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("prestige_EB_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("prestige_generic_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("prestige_Gamestop_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Prestige_Ninja_Run"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Shadow_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Shadow_Recall"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("Dark_Nova_Bolt"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("Dark_Nova_Blast"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("Dark_Nova_Emanation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Dark_Nova_Detonation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Strike"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Smite"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Mire"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Drain"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Antagonize"))
                                    {
                                        powerEntry.Power.DisplayLocation = 27;
                                    }
                                    else if (powName.Equals("Energy_Flight"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Combat_Flight"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("Bright_Nova_Bolt"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("Bright_Nova_Blast"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("Bright_Nova_Scatter"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Bright_Nova_Detonation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("White_Dwarf_Strike"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("White_Dwarf_Smite"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("White_Dwarf_Flare"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("White_Dwarf_Sublimation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else if (powName.Equals("White_Dwarf_Antagonize"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else if (powName.Equals("White_Dwarf_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 27;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.Power.GroupName.Equals("Inherent") && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Brawl"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("Sprint"))
                                    {
                                        powerEntry.Power.DisplayLocation = 4;
                                    }
                                    else if (powName.Equals("Rest"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("Swift"))
                                    {
                                        powerEntry.Power.DisplayLocation = 1;
                                    }
                                    else if (powName.Equals("Health"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("Hurdle"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Stamina"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("prestige_DVD_Glidep"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("prestige_BestBuy_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("prestige_EB_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("prestige_generic_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("prestige_Gamestop_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Prestige_Ninja_Run"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Fast_Snipe"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Blood_Frenzy"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("FAST_MODE"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_1"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_2"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_3"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.PowerSet.SetType.Equals(Enums.ePowerSetType.Ancillary) && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Fast_Snipe"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Beast_Mastery"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Pack_Mentality"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Howler_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Alpha_Howler_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Lioness_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Dire_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Demon_Summoning"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Cold_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Fiery_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Hellfire_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Ember_Demon_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Hellfire_Gargoyle_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Demon_Prince_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Mercenaries"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Soldier_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Medic_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Spec_Ops_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Commando_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Necromancy"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Zombie_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Grave_Knight_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Lich_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Ninjas"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Genin_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Jounin_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Oni_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                if (powerEntry.PowerSet.SetName.Equals("Robotics"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Battle_Drone_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Protector_Bot_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Assault_Bot_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Thugs"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Punk_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Arsonist_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Enforcer_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Bruiser_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Dual_Pistols"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Chemical_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Cryo_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Incendiary_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Staff_Fighting"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Form_of_the_Body"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Form_of_the_Mind"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("Form_of_the_Soul"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Bio_Organic_Armor"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Defensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Defensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Defensive_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                if (powerEntry.Power.GroupName.Equals("Incarnate") && MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string setName = powerEntry.PowerSet.SetName;
                                    if (setName.Equals("Alpha"))
                                    {
                                        displayLocation = 24;
                                    }
                                    else if (setName.Equals("Judgement"))
                                    {
                                        displayLocation = 26;
                                    }
                                    else if (setName.Equals("Interface"))
                                    {
                                        displayLocation = 28;
                                    }
                                    else if (setName.Equals("Lore"))
                                    {
                                        displayLocation = 29;
                                    }
                                    else if (setName.Equals("Destiny"))
                                    {
                                        displayLocation = 30;
                                    }
                                    else if (setName.Equals("Hybrid"))
                                    {
                                        displayLocation = 31;
                                    }
                                    else if (setName.Equals("Genesis"))
                                    {
                                        displayLocation = 32;
                                    }
                                    else if (setName.Equals("Stance"))
                                    {
                                        displayLocation = 33;
                                    }
                                    else if (setName.Equals("Vitae"))
                                    {
                                        displayLocation = 34;
                                    }
                                    else if (setName.Equals("Omega"))
                                    {
                                        displayLocation = 35;
                                    }
                                    else
                                    {
                                        displayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.Power.GroupName.Equals("Incarnate") && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string setName = powerEntry.PowerSet.SetName;
                                    if (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[0].FullName.Contains("Bio_Organic_Armor") || MidsContext.Character.Powersets[1].FullName.Contains("Bio_Organic_Armor"))
                                    {
                                        if (setName.Equals("Alpha"))
                                        {
                                            displayLocation = 21;
                                        }
                                        else if (setName.Equals("Judgement"))
                                        {
                                            displayLocation = 23;
                                        }
                                        else if (setName.Equals("Interface"))
                                        {
                                            displayLocation = 25;
                                        }
                                        else if (setName.Equals("Lore"))
                                        {
                                            displayLocation = 26;
                                        }
                                        else if (setName.Equals("Destiny"))
                                        {
                                            displayLocation = 27;
                                        }
                                        else if (setName.Equals("Hybrid"))
                                        {
                                            displayLocation = 28;
                                        }
                                        else if (setName.Equals("Genesis"))
                                        {
                                            displayLocation = 29;
                                        }
                                        else if (setName.Equals("Stance"))
                                        {
                                            displayLocation = 30;
                                        }
                                        else if (setName.Equals("Vitae"))
                                        {
                                            displayLocation = 31;
                                        }
                                        else if (setName.Equals("Omega"))
                                        {
                                            displayLocation = 32;
                                        }
                                        else
                                        {
                                            displayLocation = powerEntry.Power.DisplayLocation;
                                        }
                                    }
                                    else
                                    {
                                        if (setName.Equals("Alpha"))
                                        {
                                            displayLocation = 20;
                                        }
                                        else if (setName.Equals("Judgement"))
                                        {
                                            displayLocation = 21;
                                        }
                                        else if (setName.Equals("Interface"))
                                        {
                                            displayLocation = 22;
                                        }
                                        else if (setName.Equals("Lore"))
                                        {
                                            displayLocation = 23;
                                        }
                                        else if (setName.Equals("Destiny"))
                                        {
                                            displayLocation = 24;
                                        }
                                        else if (setName.Equals("Hybrid"))
                                        {
                                            displayLocation = 25;
                                        }
                                        else if (setName.Equals("Genesis"))
                                        {
                                            displayLocation = 26;
                                        }
                                        else if (setName.Equals("Stance"))
                                        {
                                            displayLocation = 27;
                                        }
                                        else if (setName.Equals("Vitae"))
                                        {
                                            displayLocation = 28;
                                        }
                                        else if (setName.Equals("Omega"))
                                        {
                                            displayLocation = 29;
                                        }
                                        else
                                        {
                                            displayLocation = powerEntry.Power.DisplayLocation;
                                        }
                                    }
                                }
                                else
                                {
                                    displayLocation = powerEntry.Power.DisplayLocation;
                                }
                                break;
                            case 3:
                                if (powerEntry.Power.GroupName.Equals("Inherent") && MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Brawl"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Sprint"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("Rest"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("Swift"))
                                    {
                                        powerEntry.Power.DisplayLocation = 1;
                                    }
                                    else if (powName.Equals("Health"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("Hurdle"))
                                    {
                                        powerEntry.Power.DisplayLocation = 4;
                                    }
                                    else if (powName.Equals("Stamina"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("prestige_DVD_Glidep"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("prestige_BestBuy_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("prestige_EB_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("prestige_generic_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("prestige_Gamestop_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Prestige_Ninja_Run"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else if (powName.Equals("Shadow_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("Shadow_Recall"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("Dark_Nova_Bolt"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Dark_Nova_Blast"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Dark_Nova_Emanation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Dark_Nova_Detonation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Strike"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Smite"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Mire"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Drain"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Antagonize"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else if (powName.Equals("Energy_Flight"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("Combat_Flight"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("Bright_Nova_Bolt"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Bright_Nova_Blast"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Bright_Nova_Scatter"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Bright_Nova_Detonation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("White_Dwarf_Strike"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("White_Dwarf_Smite"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("White_Dwarf_Flare"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("White_Dwarf_Sublimation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("White_Dwarf_Antagonize"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("White_Dwarf_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.Power.GroupName.Equals("Inherent") && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Brawl"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Sprint"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("Rest"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("Swift"))
                                    {
                                        powerEntry.Power.DisplayLocation = 1;
                                    }
                                    else if (powName.Equals("Health"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("Hurdle"))
                                    {
                                        powerEntry.Power.DisplayLocation = 4;
                                    }
                                    else if (powName.Equals("Stamina"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("prestige_DVD_Glidep"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("prestige_BestBuy_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("prestige_EB_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("prestige_generic_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("prestige_Gamestop_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Prestige_Ninja_Run"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Fast_Snipe"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Blood_Frenzy"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("FAST_MODE"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_1"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_2"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_3"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.PowerSet.SetType.Equals(Enums.ePowerSetType.Ancillary) && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Fast_Snipe"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Beast_Mastery"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Pack_Mentality"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Howler_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Alpha_Howler_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Lioness_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Dire_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Demon_Summoning"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Cold_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Fiery_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Hellfire_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Ember_Demon_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Hellfire_Gargoyle_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Demon_Prince_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Mercenaries"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Soldier_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Medic_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Spec_Ops_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Commando_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Necromancy"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Zombie_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Grave_Knight_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Lich_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Ninjas"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Genin_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Jounin_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Oni_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                if (powerEntry.PowerSet.SetName.Equals("Robotics"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Battle_Drone_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Protector_Bot_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Assault_Bot_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Thugs"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Punk_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Arsonist_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Enforcer_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Bruiser_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Dual_Pistols"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Chemical_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Cryo_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Incendiary_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Staff_Fighting"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Form_of_the_Body"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("Form_of_the_Mind"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Form_of_the_Soul"))
                                    {
                                        powerEntry.Power.DisplayLocation = 27;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Bio_Organic_Armor"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Defensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Defensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Defensive_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                if (powerEntry.Power.GroupName.Equals("Incarnate") && MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string setName = powerEntry.PowerSet.SetName;
                                    if (setName.Equals("Alpha"))
                                    {
                                        displayLocation = 27;
                                    }
                                    else if (setName.Equals("Judgement"))
                                    {
                                        displayLocation = 28;
                                    }
                                    else if (setName.Equals("Interface"))
                                    {
                                        displayLocation = 29;
                                    }
                                    else if (setName.Equals("Lore"))
                                    {
                                        displayLocation = 30;
                                    }
                                    else if (setName.Equals("Destiny"))
                                    {
                                        displayLocation = 31;
                                    }
                                    else if (setName.Equals("Hybrid"))
                                    {
                                        displayLocation = 32;
                                    }
                                    else if (setName.Equals("Genesis"))
                                    {
                                        displayLocation = 33;
                                    }
                                    else if (setName.Equals("Stance"))
                                    {
                                        displayLocation = 34;
                                    }
                                    else if (setName.Equals("Vitae"))
                                    {
                                        displayLocation = 35;
                                    }
                                    else if (setName.Equals("Omega"))
                                    {
                                        displayLocation = 36;
                                    }
                                    else
                                    {
                                        displayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.Power.GroupName.Equals("Incarnate") && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string setName = powerEntry.PowerSet.SetName;
                                    if (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[0].FullName.Contains("Bio_Organic_Armor") || MidsContext.Character.Powersets[1].FullName.Contains("Bio_Organic_Armor"))
                                    {
                                        if (setName.Equals("Alpha"))
                                        {
                                            displayLocation = 22;
                                        }
                                        else if (setName.Equals("Judgement"))
                                        {
                                            displayLocation = 23;
                                        }
                                        else if (setName.Equals("Interface"))
                                        {
                                            displayLocation = 25;
                                        }
                                        else if (setName.Equals("Lore"))
                                        {
                                            displayLocation = 26;
                                        }
                                        else if (setName.Equals("Destiny"))
                                        {
                                            displayLocation = 28;
                                        }
                                        else if (setName.Equals("Hybrid"))
                                        {
                                            displayLocation = 29;
                                        }
                                        else if (setName.Equals("Genesis"))
                                        {
                                            displayLocation = 31;
                                        }
                                        else if (setName.Equals("Stance"))
                                        {
                                            displayLocation = 32;
                                        }
                                        else if (setName.Equals("Vitae"))
                                        {
                                            displayLocation = 34;
                                        }
                                        else if (setName.Equals("Omega"))
                                        {
                                            displayLocation = 35;
                                        }
                                        else
                                        {
                                            displayLocation = powerEntry.Power.DisplayLocation;
                                        }
                                    }
                                    else
                                    {
                                        if (setName.Equals("Alpha"))
                                        {
                                            displayLocation = 21;
                                        }
                                        else if (setName.Equals("Judgement"))
                                        {
                                            displayLocation = 22;
                                        }
                                        else if (setName.Equals("Interface"))
                                        {
                                            displayLocation = 23;
                                        }
                                        else if (setName.Equals("Lore"))
                                        {
                                            displayLocation = 24;
                                        }
                                        else if (setName.Equals("Destiny"))
                                        {
                                            displayLocation = 25;
                                        }
                                        else if (setName.Equals("Hybrid"))
                                        {
                                            displayLocation = 26;
                                        }
                                        else if (setName.Equals("Genesis"))
                                        {
                                            displayLocation = 27;
                                        }
                                        else if (setName.Equals("Stance"))
                                        {
                                            displayLocation = 28;
                                        }
                                        else if (setName.Equals("Vitae"))
                                        {
                                            displayLocation = 29;
                                        }
                                        else if (setName.Equals("Omega"))
                                        {
                                            displayLocation = 30;
                                        }
                                        else
                                        {
                                            displayLocation = powerEntry.Power.DisplayLocation;
                                        }
                                    }
                                }
                                else
                                {
                                    displayLocation = powerEntry.Power.DisplayLocation;
                                }
                                break;
                            case 4:
                                if (powerEntry.Power.GroupName.Equals("Inherent") && MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Brawl"))
                                    {
                                        powerEntry.Power.DisplayLocation = 4;
                                    }
                                    else if (powName.Equals("Sprint"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("Rest"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Swift"))
                                    {
                                        powerEntry.Power.DisplayLocation = 1;
                                    }
                                    else if (powName.Equals("Health"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("Hurdle"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("Stamina"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("prestige_DVD_Glidep"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("prestige_BestBuy_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("prestige_EB_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("prestige_generic_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("prestige_Gamestop_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Prestige_Ninja_Run"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else if (powName.Equals("Shadow_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("Shadow_Recall"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("Dark_Nova_Bolt"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("Dark_Nova_Blast"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Dark_Nova_Emanation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Dark_Nova_Detonation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Strike"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Smite"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Mire"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Drain"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Black_Dwarf_Antagonize"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else if (powName.Equals("Energy_Flight"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("Combat_Flight"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("Bright_Nova_Bolt"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("Bright_Nova_Blast"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("Bright_Nova_Scatter"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Bright_Nova_Detonation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else if (powName.Equals("White_Dwarf_Strike"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("White_Dwarf_Smite"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("White_Dwarf_Flare"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("White_Dwarf_Sublimation"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("White_Dwarf_Antagonize"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("White_Dwarf_Step"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.Power.GroupName.Equals("Inherent") && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Brawl"))
                                    {
                                        powerEntry.Power.DisplayLocation = 4;
                                    }
                                    else if (powName.Equals("Sprint"))
                                    {
                                        powerEntry.Power.DisplayLocation = 8;
                                    }
                                    else if (powName.Equals("Rest"))
                                    {
                                        powerEntry.Power.DisplayLocation = 12;
                                    }
                                    else if (powName.Equals("Swift"))
                                    {
                                        powerEntry.Power.DisplayLocation = 1;
                                    }
                                    else if (powName.Equals("Health"))
                                    {
                                        powerEntry.Power.DisplayLocation = 9;
                                    }
                                    else if (powName.Equals("Hurdle"))
                                    {
                                        powerEntry.Power.DisplayLocation = 5;
                                    }
                                    else if (powName.Equals("Stamina"))
                                    {
                                        powerEntry.Power.DisplayLocation = 13;
                                    }
                                    else if (powName.Equals("prestige_DVD_Glidep"))
                                    {
                                        powerEntry.Power.DisplayLocation = 2;
                                    }
                                    else if (powName.Equals("prestige_BestBuy_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 6;
                                    }
                                    else if (powName.Equals("prestige_EB_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 10;
                                    }
                                    else if (powName.Equals("prestige_generic_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 14;
                                    }
                                    else if (powName.Equals("prestige_Gamestop_Sprintp"))
                                    {
                                        powerEntry.Power.DisplayLocation = 18;
                                    }
                                    else if (powName.Equals("Fast_Snipe"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else if (powName.Equals("Blood_Frenzy"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("FAST_MODE"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_1"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_2"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("COMBO_LEVEL_3"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else if (powName.Equals("Prestige_Ninja_Run"))
                                    {
                                        powerEntry.Power.DisplayLocation = 22;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetType.Equals(Enums.ePowerSetType.Ancillary) && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Fast_Snipe"))
                                    {
                                        powerEntry.Power.DisplayLocation = 17;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Beast_Mastery"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Pack_Mentality"))
                                    {
                                        powerEntry.Power.DisplayLocation = 16;
                                    }
                                    else if (powName.Equals("Howler_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Alpha_Howler_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Lioness_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("Dire_Wolf_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Demon_Summoning"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Cold_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Fiery_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Hellfire_Demonling_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 28;
                                    }
                                    else if (powName.Equals("Ember_Demon_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("Hellfire_Gargoyle_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else if (powName.Equals("Demon_Prince_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 29;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Mercenaries"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Soldier_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Medic_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Spec_Ops_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("Commando_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Necromancy"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Zombie_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Grave_Knight_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Lich_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 28;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Ninjas"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Genin_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Jounin_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Oni_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 28;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                if (powerEntry.PowerSet.SetName.Equals("Robotics"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Battle_Drone_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Protector_Bot_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Assault_Bot_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 28;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Thugs"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Punk_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 20;
                                    }
                                    else if (powName.Equals("Arsonist_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 24;
                                    }
                                    else if (powName.Equals("Enforcer_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 21;
                                    }
                                    else if (powName.Equals("Bruiser_H"))
                                    {
                                        powerEntry.Power.DisplayLocation = 25;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Dual_Pistols"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Chemical_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Cryo_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("Incendiary_Ammunition"))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Staff_Fighting"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Form_of_the_Body"))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Form_of_the_Mind"))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Form_of_the_Soul"))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }

                                if (powerEntry.PowerSet.SetName.Equals("Bio_Organic_Armor"))
                                {
                                    string powName = powerEntry.Power.PowerName;
                                    if (powName.Equals("Defensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 27;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 31;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting")))
                                    {
                                        powerEntry.Power.DisplayLocation = 35;
                                    }
                                    else if (powName.Equals("Defensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 15;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 19;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling")))
                                    {
                                        powerEntry.Power.DisplayLocation = 23;
                                    }
                                    else if (powName.Equals("Defensive_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 3;
                                    }
                                    else if (powName.Equals("Efficient_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 7;
                                    }
                                    else if (powName.Equals("Offensive_Adaptation") && (!MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || !MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || !MidsContext.Character.Powersets[0].FullName.Contains("Staff_Mastery") || !MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || !MidsContext.Character.Powersets[1].FullName.Contains("Staff_Mastery")))
                                    {
                                        powerEntry.Power.DisplayLocation = 11;
                                    }
                                    else
                                    {
                                        powerEntry.Power.DisplayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                if (powerEntry.Power.GroupName.Equals("Incarnate") && MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string setName = powerEntry.PowerSet.SetName;
                                    if (setName.Equals("Alpha"))
                                    {
                                        displayLocation = 26;
                                    }
                                    else if (setName.Equals("Judgement"))
                                    {
                                        displayLocation = 27;
                                    }
                                    else if (setName.Equals("Interface"))
                                    {
                                        displayLocation = 30;
                                    }
                                    else if (setName.Equals("Lore"))
                                    {
                                        displayLocation = 31;
                                    }
                                    else if (setName.Equals("Destiny"))
                                    {
                                        displayLocation = 34;
                                    }
                                    else if (setName.Equals("Hybrid"))
                                    {
                                        displayLocation = 35;
                                    }
                                    else if (setName.Equals("Genesis"))
                                    {
                                        displayLocation = 28;
                                    }
                                    else if (setName.Equals("Stance"))
                                    {
                                        displayLocation = 29;
                                    }
                                    else if (setName.Equals("Vitae"))
                                    {
                                        displayLocation = 32;
                                    }
                                    else if (setName.Equals("Omega"))
                                    {
                                        displayLocation = 33;
                                    }
                                    else
                                    {
                                        displayLocation = powerEntry.Power.DisplayLocation;
                                    }
                                }
                                else if (powerEntry.Power.GroupName.Equals("Incarnate") && !MidsContext.Archetype.ClassType.Equals(Enums.eClassType.HeroEpic))
                                {
                                    string setName = powerEntry.PowerSet.SetName;
                                    if (MidsContext.Character.Powersets[0].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[0].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Water_Blast") || MidsContext.Character.Powersets[1].FullName.Contains("Brawling") || MidsContext.Character.Powersets[1].FullName.Contains("Staff_Fighting") || MidsContext.Character.Powersets[0].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[1].FullName.Contains("Dual_Pistols") || MidsContext.Character.Powersets[0].FullName.Contains("Bio_Organic_Armor") || MidsContext.Character.Powersets[1].FullName.Contains("Bio_Organic_Armor"))
                                    {
                                        if (setName.Equals("Alpha"))
                                        {
                                            displayLocation = 20;
                                        }
                                        else if (setName.Equals("Judgement"))
                                        {
                                            displayLocation = 21;
                                        }
                                        else if (setName.Equals("Interface"))
                                        {
                                            displayLocation = 24;
                                        }
                                        else if (setName.Equals("Lore"))
                                        {
                                            displayLocation = 25;
                                        }
                                        else if (setName.Equals("Destiny"))
                                        {
                                            displayLocation = 28;
                                        }
                                        else if (setName.Equals("Hybrid"))
                                        {
                                            displayLocation = 29;
                                        }
                                        else if (setName.Equals("Genesis"))
                                        {
                                            displayLocation = 32;
                                        }
                                        else if (setName.Equals("Stance"))
                                        {
                                            displayLocation = 33;
                                        }
                                        else if (setName.Equals("Vitae"))
                                        {
                                            displayLocation = 36;
                                        }
                                        else if (setName.Equals("Omega"))
                                        {
                                            displayLocation = 37;
                                        }
                                        else
                                        {
                                            displayLocation = powerEntry.Power.DisplayLocation;
                                        }
                                    }
                                    else
                                    {
                                        if (setName.Equals("Alpha"))
                                        {
                                            displayLocation = 3;
                                        }
                                        else if (setName.Equals("Judgement"))
                                        {
                                            displayLocation = 7;
                                        }
                                        else if (setName.Equals("Interface"))
                                        {
                                            displayLocation = 11;
                                        }
                                        else if (setName.Equals("Lore"))
                                        {
                                            displayLocation = 15;
                                        }
                                        else if (setName.Equals("Destiny"))
                                        {
                                            displayLocation = 19;
                                        }
                                        else if (setName.Equals("Hybrid"))
                                        {
                                            displayLocation = 23;
                                        }
                                        else if (setName.Equals("Genesis"))
                                        {
                                            displayLocation = 27;
                                        }
                                        else if (setName.Equals("Stance"))
                                        {
                                            displayLocation = 31;
                                        }
                                        else if (setName.Equals("Vitae"))
                                        {
                                            displayLocation = 35;
                                        }
                                        else if (setName.Equals("Omega"))
                                        {
                                            displayLocation = 39;
                                        }
                                        else
                                        {
                                            displayLocation = powerEntry.Power.DisplayLocation;
                                        }
                                    }
                                }
                                else
                                {
                                    displayLocation = powerEntry.Power.DisplayLocation;
                                }
                                break;
                        }
                    }

                    if (displayLocation <= -1) return CRtoXY(iCol, iRow);
                    iRow = vcRowsPowers;
                    for (int i = 0; i <= inherentGrid.Length - 1; i++)
                    {
                        for (int k = 0; k <= inherentGrid[i].Length - 1; k++)
                        {
                            if (displayLocation != inherentGrid[i][k]) continue;
                            iRow += i;
                            iCol = k;
                            flag = true;
                            break;
                        }

                        if (flag)
                        {
                            break;
                        }
                    }
                }
                else if (powerIdx > -1)
                {
                    for (int i = 1; i <= vcCols; i++)
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

        Point CRtoXY(int iCol, int iRow)
        {
            Point result = new Point(0, 0);
            checked
            {
                if (iRow >= vcRowsPowers)
                {
                    result.X = iCol * (SzPower.Width + 8);
                    result.Y = 6 + iRow * (SzPower.Height + 20);
                }
                else
                {
                    result.X = iCol * (SzPower.Width + 10);
                    result.Y = iRow * (SzPower.Height + 20);
                }

                return result;
            }
        }

        public Size GetDrawingArea()
        {
            Size result = (Size) PowerPosition(23);
            checked
            {
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + 20;
                for (int i = 0; i <= MidsContext.Character.CurrentBuild.Powers.Count - 1; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i].Power == null || MidsContext.Character.CurrentBuild.Powers[i].Chosen &&
                        i > MidsContext.Character.CurrentBuild.LastPower)
                        continue;
                    Size size = new Size(result.Width, PowerPosition(i).Y + SzPower.Height + 20);
                    if (size.Height > result.Height)
                    {
                        result.Height = size.Height + 20;
                    }

                    if (size.Width > result.Width)
                    {
                        result.Width = size.Width + 20;
                    }
                }

                return result;
            }
        }

        Size GetMaxDrawingArea()
        {
            int cols = vcCols;
            MiniSetCol(6);
            Size result = (Size) PowerPosition(23);
            MiniSetCol(2);
            int[][] inherentGrid = GetInherentGrid();
            checked
            {
                Size size = (Size) CRtoXY(inherentGrid[inherentGrid.Length - 1].Length - 1, inherentGrid.Length - 1);
                if (size.Height > result.Height)
                {
                    result.Height = size.Height + 20;
                }

                if (size.Width > result.Width)
                {
                    result.Width = size.Width;
                }

                MiniSetCol(cols);
                result.Width += SzPower.Width;
                result.Height = result.Height + SzPower.Height + 20;
                return result;
            }
        }

        void MiniSetCol(int cols)
        {
            if (cols == vcCols)
                return;
            if (cols < 2 | cols > 6)
                return;
            vcCols = cols;
            vcRowsPowers = checked((int) Math.Round(24.0 / vcCols));
        }

        public Size GetRequiredDrawingArea()
        {
            int maxY = -1;
            int maxX = -1;
            checked
            {
                int num4 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                for (int i = 0; i <= num4; i++)
                {
                    if (!(MidsContext.Character.CurrentBuild.Powers[i].IDXPower > -1 | MidsContext.Character.CurrentBuild.Powers[i].Chosen))
                        continue;
                    Point point = PowerPosition(i);
                    if (point.X > maxX)
                    {
                        maxX = point.X;
                    }

                    if (point.Y > maxY)
                    {
                        maxY = point.Y;
                    }
                }

                Size result;
                if (maxX > -1 & maxY > -1)
                {
                    Size size = new Size(maxX + SzPower.Width, maxY + SzPower.Height + 20);
                    result = size;
                }
                else
                {
                    Point point2 = PowerPosition(MidsContext.Character.CurrentBuild.LastPower);
                    Size size = new Size(point2.X + SzPower.Width, point2.Y + SzPower.Height + 20 + 4);
                    result = size;
                }

                return result;
            }
        }

        const string GfxPath = "images\\";

        const string GfxPowerFn = "pSlot";

        const string GfxFileExt = ".png";

        const string NewSlotName = "Addslot.png";

        const int PaddingX = 7;

        const int PaddingY = 17;

        public const int OffsetY = 20;

        const int OffsetInherent = 4;

        const int vcPowers = 24;

        Size szBuffer;

        public Size szSlot;

        Font DefaultFont;

        Color BackColor;

        public ExtendedBitmap bxBuffer;

        public ExtendedBitmap[] bxPower;

        readonly ExtendedBitmap bxNewSlot;

        Graphics gTarget;

        Control cTarget;

        public Enums.eInterfaceMode InterfaceMode;

        public int Highlight;

        ColorMatrix pColorMatrix;

        public ImageAttributes pImageAttributes;

        //bool VillainColor;

        float ScaleValue;

        int vcCols;
        int vcRowsPowers;
    }
}
