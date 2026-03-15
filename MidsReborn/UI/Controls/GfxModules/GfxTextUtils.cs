using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Drawing;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxTextUtils
{
    internal static void DrawEnhancementLevel(this ClsDrawX drawX, SlotEntry slot, Font font, ref RectangleF rect)
    {
        if (drawX.BxBuffer?.Graphics == null)
        {
            return;
        }

        var enhType = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID;
        if (enhType == Enums.eType.None)
        {
            return;
        }

        if (enhType is Enums.eType.SetO or Enums.eType.InventO)
        {
            var iValue = rect;
            iValue.Y -= 5f;
            iValue.Height = drawX._defaultFont.GetHeight(drawX.BxBuffer.Graphics);
            var relativeLevelNumeric = "";
            var enhInternalName = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].UID;
            var catalystSet = DatabaseAPI.EnhHasCatalyst(enhInternalName) ||
                              enhInternalName.ToLowerInvariant().Contains("overwhelming_force") ||
                              enhInternalName.ToLowerInvariant().Contains("cupids_crush");
            // Catalysed enhancements take character level no matter what.
            // Game does not allow boosters over enhancement catalysts.
            if (!catalystSet & (slot.Enhancement.RelativeLevel > Enums.eEnhRelative.Even) & MidsContext.Config.ShowEnhRel)
            {
                relativeLevelNumeric = Enums.GetRelativeString(slot.Enhancement.RelativeLevel, false);
            }

            // If enhancement has boosters, need to stretch the level drawing zone a little,
            // or relative level doesn't fit in.
            if (!string.IsNullOrEmpty(relativeLevelNumeric))
            {
                iValue.Width += 10f;
                iValue.X -= 5f;
            }

            var iStr = MidsContext.Config.I9.HideIOLevels
                ? string.Empty
                : $"{slot.Enhancement.IOLevel + 1}{relativeLevelNumeric}";
                
            if (catalystSet)
            {
                return;
            }

            var bounds = drawX.ScaleDown(iValue);
            var cyan = Color.Cyan;
            var outline = Color.FromArgb(128, 0, 0, 0);

            DrawOutlineText(iStr, bounds, cyan, outline, font, 1, drawX.BxBuffer.Graphics);

            return;
        }
        
        var iValue2 = rect;
        iValue2.Y -= 5f;
        iValue2.Height = drawX._defaultFont.GetHeight(drawX.BxBuffer.Graphics);

        var color = slot.Enhancement.RelativeLevel switch
        {
            0 => Color.Red,
            < Enums.eEnhRelative.Even => Color.Yellow,
            > Enums.eEnhRelative.Even => Color.FromArgb(0, 255, 0),
            _ => Color.White
        };

        // Always display relative level if present
        //if (MidsContext.Config.ShowEnhRel)
        var relativeString = Enums.GetRelativeString(slot.Enhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols);

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
            relativeString = $"50{relativeString}";
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

        if (string.IsNullOrEmpty(relativeString))
        {
            return;
        }

        DrawOutlineText(relativeString, drawX.ScaleDown(iValue2), color, Color.FromArgb(128, 0, 0, 0), font, 1, drawX.BxBuffer.Graphics);
    }

    internal static void DrawPowerText(this ClsDrawX drawX, DrawVariables drawVars)
    {
        drawX.DrawPowerText(drawVars.PowerEntry, drawVars.Location, drawVars.Font, drawVars.Text, drawVars.Text2, drawVars.PowerState);
    }

    internal static void DrawPowerText(this ClsDrawX drawX, PowerEntry powerEntry, Point location, Font font, string text, string text2, Enums.ePowerState powerState)
    {
        var solidBrush = new SolidBrush(Color.White);
        var stringFormat = new StringFormat();
        var rectangleF = new RectangleF(
            location.X + 10,
            location.Y + 4,
            drawX.SzPower.Width,
            drawX._defaultFont.GetHeight() * 2f
        );

        var powerState2 = powerEntry.State;
        if ((powerState2 == Enums.ePowerState.Empty) & (powerState == Enums.ePowerState.Open))
        {
            powerState2 = powerState;
        }

        switch (powerState2)
        {
            case 0:
                solidBrush = new SolidBrush(Color.Transparent);
                text = "";
                break;

            case Enums.ePowerState.Empty:
                solidBrush = new SolidBrush(Color.WhiteSmoke);
                text = $"({powerEntry.Level + 1})";
                break;

            case Enums.ePowerState.Used:
                if (powerEntry.PowerSet.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary
                    or Enums.ePowerSetType.Ancillary or Enums.ePowerSetType.Inherent
                    or Enums.ePowerSetType.Pool)
                {
                    text2 = "";
                }

                solidBrush = !MidsContext.Character.IsHero()
                    ? new SolidBrush(Color.White)
                    : new SolidBrush(Color.Black);

                text = powerEntry.Virtual
                    ? powerEntry.Name
                    : $"({powerEntry.Level + 1}) {powerEntry.Name} {text2}";
                break;

            case Enums.ePowerState.Open:
                solidBrush = new SolidBrush(Color.WhiteSmoke);
                text = $"({powerEntry.Level + 1})";
                break;
        }

        if ((powerState == Enums.ePowerState.Empty) & (powerEntry.State == Enums.ePowerState.Used))
        {
            solidBrush = new SolidBrush(Color.WhiteSmoke);
        }

        if (drawX.InterfaceMode == Enums.eInterfaceMode.PowerToggle && solidBrush.Color == Color.Black && !powerEntry.CanIncludeForStats())
        {
            solidBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
        }

        stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
        if (MidsContext.Config.EnhanceVisibility)
        {
            DrawOutlineText(text, drawX.ScaleDown(rectangleF), Color.White, Color.Black, font, 1f, drawX.BxBuffer.Graphics, false, true);

            return;
        }
        
        drawX.BxBuffer?.Graphics?.DrawString(text, font, solidBrush, drawX.ScaleDown(rectangleF), stringFormat);
    }

    // ////////////////////////////////////////////////

    public static void DrawOutlineText(string iStr, RectangleF bounds, Color textColor, Color outlineColor, Font bFont, float outlineSpace, Graphics g, bool smallMode = false, bool leftAlign = false)
    {
        var stringFormat = new StringFormat(StringFormatFlags.NoWrap)
        {
            LineAlignment = StringAlignment.Near,
            Alignment = leftAlign ? StringAlignment.Near : StringAlignment.Center
        };

        using var brush = new SolidBrush(outlineColor);
        var layoutRectangle2 = bounds with { Height = bFont.GetHeight(g) };
        layoutRectangle2.X -= outlineSpace;
        if (!smallMode)
        {
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        }

        layoutRectangle2.Y -= outlineSpace;
        g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        layoutRectangle2.X += outlineSpace;
        if (!smallMode)
        {
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        }

        layoutRectangle2.X += outlineSpace;
        g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        layoutRectangle2.Y += outlineSpace;
        if (!smallMode)
        {
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        }

        layoutRectangle2.Y += outlineSpace;
        g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        layoutRectangle2.X -= outlineSpace;
        if (!smallMode)
        {
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        }

        layoutRectangle2.X -= outlineSpace;
        g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        layoutRectangle2.Y -= outlineSpace;
        if (!smallMode)
        {
            g.DrawString(iStr, bFont, brush, layoutRectangle2, stringFormat);
        }

        g.DrawString(iStr, bFont, new SolidBrush(textColor), bounds, stringFormat);
    }
}