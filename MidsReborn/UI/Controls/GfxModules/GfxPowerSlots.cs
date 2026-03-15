using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxPowerSlots
{
    internal static void DrawPowers(this ClsDrawX drawX)
    {
        for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
        {
            if (MidsContext.Character.CanPlaceSlot & (drawX.Highlight == i))
            {
                var pe = MidsContext.Character.CurrentBuild.Powers[i];
                drawX.DrawPowerSlot(ref pe, true); // ref PowerEntry needed ?
                MidsContext.Character.CurrentBuild.Powers[i] = pe;
            }
            else if ((MidsContext.Character.CurrentBuild.Powers[i] != null &&
                      MidsContext.Character.CurrentBuild.Powers[i]?.Chosen == true) ||
                     (MidsContext.Character.CurrentBuild.Powers[i]?.Power != null &&
                      (MidsContext.Character.CurrentBuild.Powers[i]?.Power?.GroupName == "Incarnate") |
                      (MidsContext.Character.CurrentBuild.Powers[i]?.Power?.IncludeFlag == true)))
            {
                var value = MidsContext.Character.CurrentBuild.Powers[i];
                drawX.DrawPowerSlot(ref value);
                MidsContext.Character.CurrentBuild.Powers[i] = value;
            }
        }

        //Application.DoEvents();
        drawX.DrawSplit();
    }

    public static Point DrawPowerSlot(this ClsDrawX drawX, ref PowerEntry? powerEntry, bool singleDraw = false)
    {
        var drawVars = drawX.InitializeDrawVariables(powerEntry, singleDraw);
        drawX.UpdateSlotLocation(ref drawVars);
        drawX.DrawClipRectangle(drawVars);
        drawX.UpdatePowerState(ref drawVars, singleDraw);
        drawX.UpdateRectangleF(ref drawVars);
        drawX.UpdateImageAttributes(ref drawVars);
        drawX.DrawPowerComponents(drawVars);

        return drawVars.Location;
    }

    internal static void DrawClipRectangle(this ClsDrawX drawX, DrawVariables drawVars)
    {
        var brush = new SolidBrush(drawX._backColor);
        var clipRect = new Rectangle(drawVars.SlotLocation.X, drawVars.SlotLocation.Y, drawX.SzPower.Width, drawX.SzPower.Height);
        
        drawX.BxBuffer?.Graphics?.FillRectangle(brush, drawX.ScaleDown(clipRect));
    }

    internal static void UpdatePowerState(this ClsDrawX drawX, ref DrawVariables drawVars, bool singleDraw)
    {
        var toggling = drawX.InterfaceMode == Enums.eInterfaceMode.PowerToggle;
        if (toggling)
        {
            return;
        }

        if (drawVars.PowerEntry.Power != null)
        {
            switch (singleDraw)
            {
                case true when drawVars is { SlotCheck: > -1, CanPlaceSlot: true } &&
                               drawX.InterfaceMode != Enums.eInterfaceMode.PowerToggle &&
                               drawVars.PowerEntry is { PowerSet: not null, Slots.Length: < 6 } &&
                               drawVars.PowerEntry.Power.Slottable:
                    drawVars.PowerState = Enums.ePowerState.Open;
                    break;
                default:
                {
                    if (drawVars.PowerEntry.Chosen & !drawVars.CanPlaceSlot &
                        (drawX.InterfaceMode != Enums.eInterfaceMode.PowerToggle) & (drawX.Highlight ==
                            MidsContext.Character.CurrentBuild.Powers.IndexOf(drawVars.PowerEntry)))
                    {
                        drawVars.PowerState = Enums.ePowerState.Open;
                    }

                    break;
                }
            }
        }
        else if (MidsContext.Character.CurrentBuild.Powers.IndexOf(drawVars.PowerEntry) == drawX.IndexFromLevel())
        {
            drawVars.PowerState = Enums.ePowerState.Open;
        }
    }

    internal static void DrawPowerComponents(this ClsDrawX drawX, DrawVariables drawVars)
    {
        drawX.DrawToggles(drawVars);
        drawX.DrawSlotsAndEnhancements(drawVars);
        drawX.DrawNewSlotHover(drawVars);
        drawX.DrawPowerText(drawVars);
    }

    internal static Rectangle DrawPowerImage(this ClsDrawX drawX, PowerEntry? iSlot, Point location, Enums.ePowerState ePowerState, bool toggling, ImageAttributes? imageAttr, bool grey)
    {
        var powerRect = new Rectangle(location.X, location.Y, drawX.BxPower[(int)ePowerState].Size.Width, drawX.BxPower[(int)ePowerState].Size.Height);
        var destRect = drawX.ScaleDown(powerRect);
        var width = drawX.BxPower[(int)ePowerState].ClipRect.Width;
        var powerGfxIndex = (int)ePowerState;
        var clipRect2 = drawX.BxPower[powerGfxIndex].ClipRect;
        if (ePowerState == Enums.ePowerState.Used || toggling)
        {
            if (!MidsContext.Config.DisableDesaturateInherent & !iSlot.Chosen)
            {
                imageAttr = drawX.Desaturate(grey, ePowerState == Enums.ePowerState.Open);
            }

            drawX.BxBuffer?.Graphics?.DrawImage(drawX.BxPower[MidsContext.Character.IsHero() ? 2 : 4].Bitmap, destRect,
                0, 0, width, clipRect2.Height, GraphicsUnit.Pixel, imageAttr);
        }
        else if (ePowerState == Enums.ePowerState.Open)
        {
            //Image bitmap2 = bxPower[(int)ePowerState].Bitmap;
            drawX.BxBuffer?.Graphics?.DrawImage(drawX.BxPower[MidsContext.Character.IsHero() ? 3 : 5].Bitmap, destRect,
                0, 0, width, clipRect2.Height, GraphicsUnit.Pixel);
        }
        else
        {
            drawX.BxBuffer?.Graphics?.DrawImage(drawX.BxPower[(int)ePowerState].Bitmap, destRect, 0, 0, width,
                clipRect2.Height, GraphicsUnit.Pixel);
        }

        return powerRect;
    }

    internal static void DrawToggles(this ClsDrawX drawX, DrawVariables drawVars)
    {
        drawX.DrawToggles(drawVars.PowerEntry, drawVars.ToggleRect, drawVars.ProcRect, drawVars.PowerRect, drawVars.Pen2);
    }

    internal static void DrawToggles(this ClsDrawX drawX, PowerEntry powerEntry, Rectangle toggleRect, Rectangle procRect, Rectangle powerRect, Pen pen2)
    {
        // Toggle only
        if (powerEntry.CanIncludeForStats() && !powerEntry.HasProc())
        {
            toggleRect = new Rectangle(
                (int)Math.Round(powerRect.Right - (ClsDrawX.ToggleButtonSize + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 2f)),
                (int)Math.Round(powerRect.Top + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 2f),
                ClsDrawX.ToggleButtonSize,
                ClsDrawX.ToggleButtonSize
            );
            
            toggleRect = drawX.ScaleDown(toggleRect);
            var iCenter = new PointF(-0.25f, -0.33f);
            var brush2 = powerEntry.StatInclude
                ? GfxUtils.MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                : GfxUtils.MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

            drawX.BxBuffer?.Graphics?.FillEllipse(brush2, toggleRect);
            drawX.BxBuffer?.Graphics?.DrawEllipse(pen2, toggleRect);

            return;
        }

        // Proc only
        if (powerEntry.HasProc() && !powerEntry.CanIncludeForStats())
        {
            //draw proc toggle
            procRect = new Rectangle(
                (int)Math.Round(powerRect.Right - (ClsDrawX.ToggleButtonSize + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 2f)),
                (int)Math.Round(powerRect.Top + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 2f),
                ClsDrawX.ToggleButtonSize,
                ClsDrawX.ToggleButtonSize
            );

            procRect = drawX.ScaleDown(procRect);
            var pCenter = new PointF(-0.25f, -0.33f);
            using var brush3 = !powerEntry.ProcInclude
                ? GfxUtils.MakePathBrush(procRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                : GfxUtils.MakePathBrush(procRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
            
            drawX.BxBuffer?.Graphics?.FillEllipse(brush3, procRect);
            drawX.BxBuffer?.Graphics?.DrawEllipse(pen2, procRect);

            return;
        }

        // Toggle + proc
        if (powerEntry.HasProc() && powerEntry.CanIncludeForStats())
        {
            //draw power toggle
            toggleRect = new Rectangle(
                (int)Math.Round(powerRect.Right - (ClsDrawX.ToggleButtonSize + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 3f)),
                (int)Math.Round(powerRect.Top + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 2f),
                ClsDrawX.ToggleButtonSize,
                ClsDrawX.ToggleButtonSize
            );
            
            toggleRect = drawX.ScaleDown(toggleRect);
            var iCenter = new PointF(-0.25f, -0.33f);
            using var brush2 = powerEntry.StatInclude
                ? GfxUtils.MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 255, 96), Color.FromArgb(0, 32, 0))
                : GfxUtils.MakePathBrush(toggleRect, iCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));

            drawX.BxBuffer?.Graphics?.FillEllipse(brush2, toggleRect);
            drawX.BxBuffer?.Graphics?.DrawEllipse(pen2, toggleRect);

            //draw proc toggle
            procRect = new Rectangle(
                powerRect.Right - (ClsDrawX.ToggleButtonSize + (powerRect.Height - ClsDrawX.ToggleButtonSize)),
                (int)Math.Round(powerRect.Top + (powerRect.Height - ClsDrawX.ToggleButtonSize) / 2f),
                ClsDrawX.ToggleButtonSize,
                ClsDrawX.ToggleButtonSize
            );

            procRect = drawX.ScaleDown(procRect);
            var pRect = procRect;
            var pCenter = new PointF(-0.25f, -0.33f);
            using var brush3 = !powerEntry.ProcInclude
                ? GfxUtils.MakePathBrush(pRect, pCenter, Color.FromArgb(251, 255, 97), Color.FromArgb(91, 91, 0))
                : GfxUtils.MakePathBrush(pRect, pCenter, Color.FromArgb(96, 96, 96), Color.FromArgb(0, 0, 0));
            
            drawX.BxBuffer?.Graphics?.FillEllipse(brush3, procRect);
            drawX.BxBuffer?.Graphics?.DrawEllipse(pen2, procRect);
        }
    }

    internal static void DrawSlotsAndEnhancements(this ClsDrawX drawX, DrawVariables drawVars)
    {
        drawX.DrawSlotsAndEnhancements(drawVars.PowerEntry, drawVars.RectangleF, drawVars.SlotLocation, drawVars.Pen, drawVars.Font);
    }

    internal static void DrawSlotsAndEnhancements(this ClsDrawX drawX, PowerEntry powerEntry, RectangleF rectangleF, Point slotLocation, Pen pen, Font font)
    {
        for (var i = 0; i < powerEntry.Slots.Length; i++)
        {
            var slot = powerEntry.Slots[i];
            // Enhancement spacing and position?
            rectangleF.X = slotLocation.X + (drawX.SzSlot.Width + 2) * i;
            rectangleF.Y = slotLocation.Y;
            
            SolidBrush solidBrush;
            if (slot.Enhancement.Enh < 0)
            {
                var clipRect3 = new Rectangle((int)Math.Round(rectangleF.X), slotLocation.Y, drawX.SzSlot.Width, drawX.SzSlot.Height); // New slot rectangle
                drawX.BxBuffer?.Graphics?.DrawImage(I9Gfx.EnhTypes.Bitmap, drawX.ScaleDown(clipRect3), 0, 0, drawX.SzSlot.Width, drawX.SzSlot.Height, GraphicsUnit.Pixel, drawX.PImageAttributes);
                if ((MidsContext.Config.CalcEnhLevel == 0) | (slot.Level > MidsContext.Config.ForceLevel) |
                    ((drawX.InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !powerEntry.StatInclude) |
                    (!powerEntry.AllowFrontLoading & (slot.Level < powerEntry.Level)))
                {
                    solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                    drawX.BxBuffer?.Graphics?.FillEllipse(solidBrush, drawX.ScaleDown(rectangleF));
                    drawX.BxBuffer?.Graphics?.DrawEllipse(pen, drawX.ScaleDown(rectangleF));
                }
            }
            else
            {
                // Controls if powers or slots are greyed out
                if (drawX._inDesigner) continue;

                var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                var g = drawX.BxBuffer?.Graphics;
                var clipRect3 = new Rectangle((int)Math.Round(rectangleF.X), slotLocation.Y, drawX.SzSlot.Width, drawX.SzSlot.Height);
                Recipe.RecipeRarity? rarity = null;
                var isPvP = false;
                if (enhancement.TypeID == Enums.eType.SetO)
                {
                    rarity = enhancement.RecipeIDX < 0 ? null : DatabaseAPI.Database.Recipes[enhancement.RecipeIDX].Rarity;
                    var enhSet = enhancement.GetEnhancementSet();
                    isPvP = enhSet?.Bonus.Any(e => e.Index.Select(b => DatabaseAPI.Database.Power[b]).Any(p => p?.FullName.ToLowerInvariant().Contains("pvp") == true)) == true;
                }

                I9Gfx.DrawEnhancementAt(ref g, drawX.ScaleDown(clipRect3), enhancement.ImageIdx,
                    I9Gfx.ToGfxGrade(enhancement.TypeID, slot.Enhancement.Grade),
                    rarity, isPvP);
                
                if ((slot.Enhancement.RelativeLevel == 0) | (slot.Level > MidsContext.Config.ForceLevel) |
                    ((drawX.InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !powerEntry.StatInclude) |
                    (!powerEntry.AllowFrontLoading & (slot.Level < powerEntry.Level)) |
                    (MidsContext.EnhCheckMode & !slot.Enhancement.Obtained))
                {
                    solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                    var iValue3 = rectangleF;
                    iValue3.Inflate(1f, 1f);
                    drawX.BxBuffer?.Graphics?.FillEllipse(solidBrush, drawX.ScaleDown(iValue3));
                }

                if (slot.Enhancement.Enh > -1)
                {
                    drawX.DrawEnhancementLevel(slot, font, ref rectangleF);
                }
            }

            if (!MidsContext.Config.ShowSlotLevels) continue;

            var powerTextRect = rectangleF;
            //Positioning of slot level text
            powerTextRect.Y += powerTextRect.Height + 12;
            powerTextRect.Height = drawX._defaultFont.GetHeight(drawX.BxBuffer.Graphics);
            powerTextRect.Y -= powerTextRect.Height;
            powerTextRect.X += powerTextRect.Width;
            powerTextRect.X -= powerTextRect.Width - 1;

            GfxTextUtils.DrawOutlineText(
                Convert.ToString(slot.Level + 1),
                drawX.ScaleDown(powerTextRect),
                Color.FromArgb(0, 255, 0),
                Color.FromArgb(192, 0, 0, 0),
                font,
                1f,
                drawX.BxBuffer.Graphics);
        }
    }

    internal static void DrawNewSlotHover(this ClsDrawX drawX, DrawVariables drawVars)
    {
        drawX.DrawNewSlotHover(drawVars.PowerEntry, drawVars.SlotLocation, drawVars.Font, drawVars.PowerState, drawVars.SlotCheck, drawVars.DrawNewSlot);
    }

    internal static void DrawNewSlotHover(this ClsDrawX drawX, PowerEntry powerEntry, Point slotLocation, Font font, Enums.ePowerState powerState, int slotCheck, bool drawNewSlot)
    {
        if (slotCheck <= -1 || powerState is Enums.ePowerState.Empty || !drawNewSlot)
        {
            return;
        }

        var slotHoverRect = new RectangleF(slotLocation.X + (drawX.SzSlot.Width + 2) * powerEntry.Slots.Length, slotLocation.Y, drawX.SzSlot.Width, drawX.SzSlot.Height);
        drawX.BxBuffer?.Graphics?.DrawImage(drawX.BxNewSlot.Bitmap, drawX.ScaleDown(slotHoverRect));
        slotHoverRect.Height = drawX._defaultFont.GetHeight(drawX.BxBuffer.Graphics);
        slotHoverRect.Y += (drawX.SzSlot.Height - slotHoverRect.Height) / 2f;
        GfxTextUtils.DrawOutlineText(Convert.ToString(slotCheck + 1), drawX.ScaleDown(slotHoverRect), Color.FromArgb(0, 255, 255), Color.FromArgb(192, 0, 0, 0), font, 1f, drawX.BxBuffer.Graphics);
    }

    public static bool HighlightSlot(this ClsDrawX drawX, int idx, bool force = false)
    {
        if (MidsContext.Character.CurrentBuild.Powers.Count < 1)
        {
            return false;
        }

        if (drawX.Highlight == idx && !force)
        {
            return false;
        }

        var powers = MidsContext.Character.CurrentBuild.Powers;
        var highlight = drawX.Highlight;
        PowerEntry? highlightedPowerEntry;
        Point powerSlotLoc;
        Rectangle rect;
        Rectangle scaledRect;

        if (idx != -1)
        {
            if (drawX.Highlight != -1 && drawX.Highlight < MidsContext.Character.CurrentBuild.Powers.Count)
            {
                
                highlightedPowerEntry = powers[highlight];
                powerSlotLoc = drawX.DrawPowerSlot(ref highlightedPowerEntry);
                powers[highlight] = highlightedPowerEntry;
                rect = new Rectangle(powerSlotLoc.X, powerSlotLoc.Y, drawX.SzPower.Width, drawX.SzPower.Height + ClsDrawX.PaddingY);
                scaledRect = drawX.ScaleDown(rect);
                drawX.DrawSplit();
                drawX.Output(scaledRect, scaledRect, GraphicsUnit.Pixel);

                return true; // ???
            }

            drawX.Highlight = idx;
            highlightedPowerEntry = powers[idx];
            powerSlotLoc = drawX.DrawPowerSlot(ref highlightedPowerEntry, true);
            powers[idx] = highlightedPowerEntry;
            rect = new Rectangle(powerSlotLoc.X, powerSlotLoc.Y, drawX.SzPower.Width, drawX.SzPower.Height + ClsDrawX.PaddingY);
            scaledRect = drawX.ScaleDown(rect);
            drawX.DrawSplit();
            drawX.Output(scaledRect, scaledRect, GraphicsUnit.Pixel);

            return true;
        }

        if (drawX.Highlight == -1)
        {
            return false;
        }

        highlightedPowerEntry = powers[highlight];
        powerSlotLoc = drawX.DrawPowerSlot(ref highlightedPowerEntry);
        powers[highlight] = highlightedPowerEntry;
        rect = new Rectangle(powerSlotLoc.X, powerSlotLoc.Y, drawX.SzPower.Width, drawX.SzPower.Height + ClsDrawX.PaddingY);
        scaledRect = drawX.ScaleDown(rect);
        drawX.DrawSplit();
        drawX.Output(scaledRect, scaledRect, GraphicsUnit.Pixel);
        drawX.Highlight = idx;

        return true;

    }
}