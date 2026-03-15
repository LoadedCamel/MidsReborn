using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxPowersGrid
{
    public static Dictionary<int, Point> LayoutToGridPos(this ClsDrawX drawX, List<List<int>> powersLayout)
    {
        var ret = new Dictionary<int, Point>();
        for (var i = 0; i < powersLayout.Count; i++)
        {
            for (var j = 0; j < powersLayout[i].Count; j++)
            {
                ret.Add(powersLayout[i][j], new Point(i, j));
            }
        }

        return ret;
    }

    public static int WhichSlot(this ClsDrawX drawX, int x, int y)
    {
        for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
        {
            var point = MidsContext.Character.CurrentBuild.Powers[i] != null &&
                        (MidsContext.Character.CurrentBuild.Powers[i].Power == null ||
                         MidsContext.Character.CurrentBuild.Powers[i].Chosen)
                ? drawX.PowerPosition(drawX.GetVisualIdx(i))
                : drawX.PowerPosition(i);

            if (x >= point.X && y >= point.Y && x < drawX.SzPower.Width + point.X && y < point.Y + drawX.SzPower.Height + ClsDrawX.PaddingY / 2)
            {
                return i;
            }
        }

        return -1;
    }

    public static int WhichEnh(this ClsDrawX drawX, int x, int y)
    {
        var oPower = -1;
        try
        {
            var point = new Point();
            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                point = MidsContext.Character.CurrentBuild.Powers[i] != null &&
                        (MidsContext.Character.CurrentBuild.Powers[i]?.Power == null ||
                         MidsContext.Character.CurrentBuild.Powers[i]?.Chosen == true)
                    ? drawX.PowerPosition(drawX.GetVisualIdx(i))
                    : drawX.PowerPosition(i);

                if (x < point.X || y < point.Y || x >= drawX.SzPower.Width + point.X ||
                    y >= point.Y + drawX.SzPower.Height + ClsDrawX.PaddingY / 2)
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

            var isValid = y >= point.Y + ClsDrawX.OffsetY &&
                          MidsContext.Character.CurrentBuild.Powers[oPower] != null &&
                          MidsContext.Character.CurrentBuild.Powers[oPower]?.NIDPowerset > -1 &&
                          DatabaseAPI.Database
                              .Powersets[MidsContext.Character.CurrentBuild.Powers[oPower].NIDPowerset]
                              .Powers[MidsContext.Character.CurrentBuild.Powers[oPower].IDXPower].Slottable;

            if (!isValid)
            {
                return -1;
            }

            var column = drawX.SzPower.Width + ClsDrawX.PaddingX == 0
                ? 0
                : (int)Math.Floor(x / (decimal)(drawX.SzPower.Width + ClsDrawX.PaddingX));
            x -= column * (drawX.SzPower.Width + ClsDrawX.PaddingX); // Remove column x offset

            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers[oPower].Slots.Length; i++)
            {
                var iZ = (i + 1) * ClsDrawX.IcoOffset;
                if (x <= iZ)
                {
                    return i;
                }
            }

            return -1;
        }
        catch (IndexOutOfRangeException)
        {
            // May occur after db edits
            return -1;
        }
    }

    public static bool WithinPowerBar(this ClsDrawX drawX, Rectangle pBounds, Point e)
    {
        pBounds.Height = drawX.SzPower.Height;
        
        return e.X >= pBounds.Left && e.X < pBounds.Right && e.Y >= pBounds.Top && e.Y < pBounds.Bottom;
    }

    internal static Point CRtoXy(this ClsDrawX drawX, int iCol, int iRow, bool ignorePadding = false)
    {
        return new Point(
            iCol * (drawX.SzPower.Width + ClsDrawX.PaddingX * (ignorePadding ? 0 : 1)),
            iRow * (drawX.SzPower.Height +
                    (ClsDrawX.PaddingY - (ignorePadding ? (int)Math.Round(5 / drawX.ScaleValue) : 0))) +
            (iRow >= drawX._vcRowsPowers ? ClsDrawX.OffsetInherent : 0) +
            (drawX._ColumnStackingMode != Enums.eColumnStacking.None ? (int)Math.Round(drawX.SzPower.Height / 2f) : 0));
    }

    internal static Point PowerPositionCr(this ClsDrawX drawX, PowerEntry? powerEntry, int displayLocation = -1)
    {
        var powerIdx = MidsContext.Character.CurrentBuild.Powers.IndexOf(powerEntry);
        // Assume that this is a copy and not the actual powerEntry item
        if (powerIdx == -1)
        {
            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i] == null)
                {
                    continue;
                }

                if (MidsContext.Character.CurrentBuild.Powers[i].Power.PowerIndex !=
                    powerEntry.Power.PowerIndex || MidsContext.Character.CurrentBuild.Powers[i].Level !=
                    powerEntry.Level)
                {
                    continue;
                }

                powerIdx = i;
                break;
            }
        }

        //Inherent Grid
        var inherentGrid = drawX.GetInherentGrid();
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
                return drawX.CRtoXy(iCol, iRow);
            }

            iRow = drawX._vcRowsPowers;
            for (var i = 0; i < inherentGrid.Length; i++)
            {
                for (var k = 0; k < inherentGrid[i].Length; k++)
                {
                    if (displayLocation != inherentGrid[i][k])
                    {
                        continue;
                    }

                    iRow += drawX._vcCols != 5
                        ? i + 1
                        : i + 2;

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
        // Main Powers
        else if (powerIdx > -1)
        {
            switch (drawX._ColumnStackingMode)
            {
                case Enums.eColumnStacking.Horizontal or Enums.eColumnStacking.Vertical:
                    return !drawX.ColumnsPowersLayout.TryGetValue(powerIdx, out var p) ? new Point(0, 0) : p;

                default:
                    for (var i = 1; i <= drawX._vcCols; i++)
                    {
                        if (drawX._vcCols == 5)
                        {
                            iCol = (int)Math.Floor((double)powerIdx / drawX._vcCols);
                            iRow = powerIdx % drawX._vcCols;
                        }
                        else
                        {
                            if (powerIdx >= drawX._vcRowsPowers * i)
                            {
                                continue;
                            }

                            iCol = i - 1;
                            iRow = powerIdx - drawX._vcRowsPowers * iCol;
                        }

                        break;
                    }

                    break;
            }
        }

        return new Point(iCol, iRow);
    }

    internal static Point PowerPosition(this ClsDrawX drawX, int powerEntryIdx)
    {
        return drawX.PowerPosition(MidsContext.Character.CurrentBuild.Powers[powerEntryIdx]);
    }

    public static Point PowerPosition(this ClsDrawX drawX, PowerEntry? powerEntry, int displayLocation = -1)
    {
        var crPos = drawX.PowerPositionCr(powerEntry, displayLocation);

        return drawX.CRtoXy(crPos.X, crPos.Y);
    }

    public static Rectangle PowerBoundsUnScaled(this ClsDrawX drawX, int hIdx)
    {
        // Returns unscaled bounds
        var rectangle = new Rectangle(0, 0, 1, 1);
        checked
        {
            if ((hIdx < 0) | (hIdx > MidsContext.Character.CurrentBuild.Powers.Count - 1))
            {
                return rectangle;
            }
            
            rectangle.Location = !MidsContext.Character.CurrentBuild.Powers[hIdx].Chosen &&
                                 MidsContext.Character.CurrentBuild.Powers[hIdx].Power != null
                ? drawX.PowerPosition(hIdx)
                : drawX.PowerPosition(drawX.GetVisualIdx(hIdx));

            rectangle.Width = drawX.SzPower.Width;
            rectangle.Height = ClsDrawX.OffsetY + drawX.SzSlot.Height;
                
            return rectangle;
        }
    }
}