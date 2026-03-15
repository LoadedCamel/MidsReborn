using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxHeaders
{
    internal static void InitHeadersVariables(this ClsDrawX drawX)
    {
        if (drawX._ColumnStackingMode != Enums.eColumnStacking.Horizontal)
        {
            drawX.HasNullColumn = false;
            drawX.LayoutColumns = 0;

            return;
        }

        var ps = drawX.GetDistinctPowersets();
        drawX.HasNullColumn = (ps.Count != 0 && ps[^1] == null && ps.Any(e => e != null)) | ps is [null];
        drawX.LayoutColumns = ps.Count;
    }

    internal static void DrawHeaders(this ClsDrawX drawX)
    {
        if (drawX._ColumnStackingMode == Enums.eColumnStacking.None)
        {
            return;
        }

        using var textFont = new Font(Fonts.Family("Noto Sans"), 9f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
        var ps = drawX.GetDistinctPowersets();

        const int y = 2;
        const int iconSize = 16;
        var k = 1;

        switch (drawX._ColumnStackingMode)
        {
            case Enums.eColumnStacking.Horizontal:
                for (var i = 0; i < drawX.LayoutColumns; i++)
                {
                    var hLabel = i switch
                    {
                        _ when i >= ps.Count => drawX.HasNullColumn ? "Unaffected Powers" : "",
                        _ when ps[i] is null => drawX.HasNullColumn ? "Unaffected Powers" : "",
                        0 => $"Pri.: {ps[i].DisplayName}",
                        1 => $"Sec.: {ps[i].DisplayName}",
                        >= 2 and <= 7 => i >= ps.Count
                            ? drawX.HasNullColumn
                                ? "Unaffected Powers"
                                : ""
                            : $"{(ps[i].SetType == Enums.ePowerSetType.Pool ? $"Pool {k++}" : MidsContext.Character?.IsHero() == false ? "Ancillary" : "Epic")}: {ps[i].DisplayName}",
                        _ when drawX.HasNullColumn => "Unaffected Powers",
                        _ => ""
                    };

                    var psImg = hLabel.Contains("Unaffected Powers")
                        ? Image.FromFile($"{I9Gfx.ImagePath()}\\Unknown.png")
                        : i >= ps.Count
                            ? null
                            : I9Gfx.GetPowersetImage(ps[i]);

                    // Unaffected powers will be drawn at 3rd column or farther no matter what.
                    var powerPos = drawX.CRtoXy(hLabel.Contains("Unaffected Powers") ? Math.Max(2, i) : i, 0);
                    var iconOffset = psImg == null ? 0 : 2 + iconSize;
                    var x = drawX.ScaleDown(powerPos.X) + 4;

                    if (psImg != null)
                    {
                        drawX.BxBuffer?.Graphics?.DrawImage(psImg, new Point(x, y));
                    }

                    TextRenderer.DrawText(drawX.BxBuffer.Graphics, hLabel, textFont, new Point(x + iconOffset, y), Color.WhiteSmoke);
                }

                break;

            case Enums.eColumnStacking.Vertical:
                var texts = new[] {
                        "Primary",
                        "Secondary",
                        MidsContext.Character?.IsHero() == false ? "Pools/Ancillary" : "Pools/Epic"
                    };

                for (var i = 0; i < 3; i++)
                {
                    var powerPos = drawX.CRtoXy(i, 0);
                    var x = drawX.ScaleDown(powerPos.X) + 4;
                    TextRenderer.DrawText(drawX.BxBuffer.Graphics, texts[i], textFont, new Point(x, y), Color.WhiteSmoke);
                }

                break;
        }
    }
}