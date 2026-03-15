using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Drawing;

namespace Mids_Reborn.UI.Controls.GfxModules;

public class DrawVariables
{
    public PowerEntry? PowerEntry { get; set; }
    public Pen Pen { get; set; }
    public Pen Pen2 { get; set; }
    public RectangleF RectangleF { get; set; }
    public StringFormat StringFormat { get; set; }
    public FontStyle FontStyle { get; set; }
    public Font Font { get; set; }
    public int SlotCheck { get; set; }
    public Enums.ePowerState PowerState { get; set; }
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

public static class DrawVariablesExt
{
    internal static void UpdateSlotLocation(this ClsDrawX drawX, ref DrawVariables drawVars)
    {
        drawVars.SlotLocation = new Point(
            (int)Math.Round(drawVars.Location.X - ClsDrawX.OffsetX + (drawX.SzPower.Width - drawX.SzSlot.Width * 6) / 2f),
            drawVars.Location.Y + ClsDrawX.OffsetY
        );
    }

    internal static void UpdateRectangleF(this ClsDrawX drawX, ref DrawVariables drawVars)
    {
        drawVars.RectangleF = drawVars.RectangleF with { Width = drawX.SzSlot.Width, Height = drawX.SzSlot.Height };
        drawVars.StringFormat.Alignment = StringAlignment.Center;
        drawVars.StringFormat.LineAlignment = StringAlignment.Center;
    }

    internal static DrawVariables InitializeDrawVariables(this ClsDrawX drawX, PowerEntry? powerEntry, bool singleDraw)
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
            ? new Font(drawX._defaultFont.FontFamily, drawX.FontScale(MidsContext.Config.RtFont.PowersBase), drawVars.FontStyle,
                GraphicsUnit.Point, 0)
            : new Font(drawX._defaultFont.FontFamily, drawX.FontScale(8), drawVars.FontStyle, GraphicsUnit.Point, 0);
        drawVars.SlotCheck = MidsContext.Character.SlotCheck(powerEntry);
        drawVars.PowerState = powerEntry.State;
        drawVars.CanPlaceSlot = MidsContext.Character.CanPlaceSlot;
        drawVars.DrawNewSlot = powerEntry.Power != null && powerEntry.State != Enums.ePowerState.Empty && drawVars.CanPlaceSlot &&
                          powerEntry.Slots.Length < 6 && singleDraw && powerEntry.Power.Slottable &
                          (drawX.InterfaceMode != Enums.eInterfaceMode.PowerToggle);
        drawVars.Location = drawX.PowerPosition(powerEntry);
        // Bug: App breaks if set in PowerPosition
        drawVars.Location = drawVars.Location with { Y = drawX._ColumnStackingMode != Enums.eColumnStacking.None ? drawVars.Location.Y + 18 : drawVars.Location.Y };
        drawVars.SlotLocation = default;

        return drawVars;
    }
}